using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarmoFamilyTree.DataAccess.DbRepositories;
using GarmoFamilyTree.DataAccess.Repositories;
using GarmoFamilyTree.Interfaces;
using GarmoFamilyTree.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GarmoFamilyTree.Services
{
  public class FamilyTreeService : IFamilyTreeService
  {
    private readonly IMemoryCache _memoryCache;
    private readonly IRandomNumberService _randomNumberService;
    private readonly FamilyTreeRepository _repository;
    private readonly DbFamilyTreeRepository _dbRepository;

    private readonly string familyTreeCacheKey = "familyTreeKey";

    public FamilyTreeService(IMemoryCache memoryCache, IRandomNumberService randomNumberService, FamilyTreeRepository repository, DbFamilyTreeRepository dbRepository)
    {
      _memoryCache = memoryCache;
      _randomNumberService = randomNumberService;
      _repository = repository;
      _dbRepository = dbRepository;
    }

    public async Task<Person> AddUpdatePerson(Person person)
    {
      int personId;
      var persons = await GetPersons();
      var personExist = persons.FirstOrDefault(p => p.Identifier == person.Identifier);
      if (personExist != null)
      {
        personId = await _repository.UpdatePersonAsync(person);
      }
      else
      {
        personId = await _repository.AddPersonAsync(person);
        // Uncomment to use DB
        //_memoryCache.Remove(familyTreeCacheKey);
        //return await _dbRepository.AddPersonAsync(person);
      }

      _repository.TryGetPerson(personId, out var personResult);
      _memoryCache.Remove(familyTreeCacheKey);
      return personResult;
    }

    public async Task<Person> AddUpdatePersonChild(int parentId, Person person)
    {
      var persons = await GetPersons();
      var personExist = persons.Exists(p => p.Id == parentId);
      if (!personExist)
      {
        return null;
      }

      if (person.Age == null)
      {
        person.Age = await _randomNumberService.GetRandomNumber(0, 18);
      }

      person.ParentId = parentId;
      return await AddUpdatePerson(person);
    }

    public async Task<FamilyTree> GetFamilyTree(string personIdentifier)
    {
      var persons = await GetPersons();
      var person = persons.FirstOrDefault(p => p.Identifier == personIdentifier);
      if (person == null)
      {
        return null;
      }

      var relations = persons.FindAll(p => p.ParentId == person.Id);
      return new FamilyTree { Relations = relations, Person = person };
    }

    private async Task<List<Person>> GetPersons()
    {
      if (_memoryCache.TryGetValue(familyTreeCacheKey, out List<Person> persons))
      {
        return persons;
      }
      
      persons = _repository.GetPersons();
      // Uncomment to use DB
      //await _dbRepository.CreateTableIfNotCreated();
      //persons = await _dbRepository.GetPersonAsync();

      var cacheOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
      _memoryCache.Set(familyTreeCacheKey, persons, cacheOptions);

      return persons;
    }
  }
}
