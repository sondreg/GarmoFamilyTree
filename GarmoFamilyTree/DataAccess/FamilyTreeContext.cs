using GarmoFamilyTree.Models;
using Microsoft.EntityFrameworkCore;

namespace GarmoFamilyTree.DataAccess
{
    public class FamilyTreeContext : DbContext
    {
        public FamilyTreeContext(DbContextOptions<FamilyTreeContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
    }
}
