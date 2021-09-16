using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using GarmoFamilyTree.Services;
using Newtonsoft.Json;
using Xunit;

namespace GarmoFamilyTree.Tests
{
    public class FamilyTreeControllerTests
    {
      [Fact]
      public async Task TestGetRandomNumberFromRandomOrg()
      {
        var randomNumberService = new RandomNumberService();
        var result = await randomNumberService.GetRandomNumber(0, 18);
        Assert.InRange(result, 0, 18);
      }

      [Fact]
        public async Task TestGetExistingFamilyTree()
        {
            var lambdaFunction = new LambdaEntryPoint();

            var requestStr = File.ReadAllText("./SampleRequests/FamilyTreeController-GetExistingFamilyTree.json");
            var request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
            var context = new TestLambdaContext();
            var response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.True(response.MultiValueHeaders.ContainsKey("Content-Type"));
            Assert.Equal("application/json; charset=utf-8", response.MultiValueHeaders["Content-Type"][0]);
        }

        [Fact]
        public async Task TestGetNotFoundFamilyTree()
        {
          var lambdaFunction = new LambdaEntryPoint();

          var requestStr = File.ReadAllText("./SampleRequests/FamilyTreeController-GetNotFoundFamilyTree.json");
          var request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
          var context = new TestLambdaContext();
          var response = await lambdaFunction.FunctionHandlerAsync(request, context);

          Assert.Equal(404, response.StatusCode);
          Assert.True(response.MultiValueHeaders.ContainsKey("Content-Type"));
          Assert.Equal("text/plain; charset=utf-8", response.MultiValueHeaders["Content-Type"][0]);
        }


  }
}
