using System.Threading.Tasks;
using GarmoFamilyTree.Models;

namespace GarmoFamilyTree.Interfaces
{
  public interface IFamilyTreeService
  {
    Task<Person> AddUpdatePerson(Person person);
    Task<Person> AddUpdatePersonChild(int parentId, Person person);

    Task<FamilyTree> GetFamilyTree(string personIdentifier);
  }
}
