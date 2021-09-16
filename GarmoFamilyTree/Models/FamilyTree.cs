using System.Collections.Generic;

namespace GarmoFamilyTree.Models
{
  public class FamilyTree
  {
    public Person Person { get; set; }
    public List<Person> Relations { get; set; }
  }
}
