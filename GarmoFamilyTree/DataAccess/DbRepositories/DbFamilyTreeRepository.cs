using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using GarmoFamilyTree.Models;

namespace GarmoFamilyTree.DataAccess.DbRepositories
{
  public class DbFamilyTreeRepository
  {
    private static AmazonDynamoDBClient _client;
    private const string TableName = "Persons";

    public DbFamilyTreeRepository()
    {
      _client = new AmazonDynamoDBClient();
    }

    public async Task<bool> CreateTableIfNotCreated()
    {
      LambdaLogger.Log($"Creating {TableName} Table");

      var tableCollection = await _client.ListTablesAsync();

      if (tableCollection.TableNames.Contains(TableName))
        return true;

      try
      {
        await _client.CreateTableAsync(new CreateTableRequest
        {
          TableName = TableName,
          KeySchema = new List<KeySchemaElement>
          {
            new KeySchemaElement { AttributeName = "Id", KeyType = KeyType.HASH },
            new KeySchemaElement { AttributeName = "Identifier", KeyType = KeyType.RANGE }
          },
          AttributeDefinitions = new List<AttributeDefinition>
          {
            new AttributeDefinition { AttributeName = "Id", AttributeType = "N" },
            new AttributeDefinition { AttributeName = "Identifier", AttributeType = "S" }
          },
          ProvisionedThroughput = new ProvisionedThroughput
          {
            ReadCapacityUnits = 5,
            WriteCapacityUnits = 5
          },
        });
        return true;
      }
      catch (Exception e)
      {
        LambdaLogger.Log(e.Message);
        return false;
      }
    }

    public async Task<List<Person>> GetPersonAsync()
    {
      LambdaLogger.Log("Get persons from the table");
      var scanRequest = new ScanRequest(TableName);
      var response = await _client.ScanAsync(scanRequest);
      var persons = new List<Person>();
      foreach (Dictionary<string, AttributeValue> item in response.Items)
      {
        persons.Add(new Person
        {
          Id = int.Parse(item["Id"].N),
          Identifier = item["Identifier"].S,
          FirstName = item["FirstName"].S,
          LastName = item["LastName"].S,
          Age = int.Parse(item["Age"].N),
          ParentId = int.Parse(item["ParentId"].N),
        });
      }
      return persons;
    }


    public async Task<Person> AddPersonAsync(Person person)
    {
      LambdaLogger.Log("Insert person in the table");
      await _client.PutItemAsync(TableName, new Dictionary<string, AttributeValue>
      {
        { "Id", new AttributeValue { N = person.Id.ToString() } },
        { "Identifier", new AttributeValue(person.Identifier) },
        { "LastName", new AttributeValue(person.LastName) },
        { "FirstName", new AttributeValue(person.FirstName) },
        { "Age", new AttributeValue { N = person.Age.ToString() } },
        { "ParentId", new AttributeValue { N = person.ParentId.ToString() } }
      });

      LambdaLogger.Log($"Finished execution for function -- {"AddPersonAsync"} at {DateTime.Now}");
      return person;
    }

    public async Task<int> UpdatePersonAsync(Person person)
    {
      LambdaLogger.Log("Update person in the table");
      //await _client.UpdateItemAsync(TableName, new Dictionary<string, AttributeValue>
      //{
      //  { "Id", new AttributeValue { N = person.Id.ToString() } },
      //  { "Identifier", new AttributeValue(person.Identifier) }
      //},
      //  new Dictionary<string, AttributeValue>
      //  {
      //    { "Id", new AttributeValue { N = person.Id.ToString() } },
      //    { "Identifier", new AttributeValue(person.Identifier) },
      //    { "LastName", new AttributeValue(person.LastName) },
      //    { "FirstName", new AttributeValue(person.FirstName) },
      //    { "Age", new AttributeValue { N = person.Age.ToString() } },
      //    { "ParentId", new AttributeValue { N = person.ParentId.ToString() } }
      //  });
      
      LambdaLogger.Log($"Finished execution for function -- {"UpdatePersonAsync"} at {DateTime.Now}");
      return person.Id;
    }
  }
}
