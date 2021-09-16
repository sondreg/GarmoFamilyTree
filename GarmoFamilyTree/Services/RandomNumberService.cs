using System;
using System.Net.Http;
using System.Threading.Tasks;
using GarmoFamilyTree.Interfaces;

namespace GarmoFamilyTree.Services
{
  public class RandomNumberService: IRandomNumberService
  {
    public async Task<int> GetRandomNumber(int min, int max)
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri("https://www.random.org/");
        var response = await client.GetAsync("integers/?num=1&min=0&max=18&col=1&base=10&format=plain&rnd=new");
        var data = response.Content;

        if (!response.IsSuccessStatusCode) 
          return -1;

        Task<string> d = data.ReadAsStringAsync();
        //Console.Write(d.Result);
        return int.Parse(d.Result);

      }
    }
  }
}
