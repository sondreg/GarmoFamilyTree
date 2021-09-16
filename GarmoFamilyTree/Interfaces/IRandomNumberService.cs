using System.Threading.Tasks;

namespace GarmoFamilyTree.Interfaces
{
  public interface IRandomNumberService
  {
    Task<int> GetRandomNumber(int min, int max);
  }
}
