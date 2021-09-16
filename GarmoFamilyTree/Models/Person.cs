using System.ComponentModel.DataAnnotations;

namespace GarmoFamilyTree.Models
{
  public class Person
  {
    public int Id { get; set; }
    [Required]
    public string Identifier { get; set; }
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public int? Age { get; set; }
    
    public int? ParentId { get; set; }
  }
}
