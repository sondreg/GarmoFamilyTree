using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarmoFamilyTree.Models;
using Microsoft.EntityFrameworkCore;

namespace GarmoFamilyTree.DataAccess.Repositories
{
    public class FamilyTreeRepository
    {
        private readonly FamilyTreeContext _context;

        public FamilyTreeRepository(FamilyTreeContext context)
        {
            _context = context;

            if (_context.Persons.Any()) 
              return;

            _context.Persons.AddRange(
              new Person
              {
                Id = 1,
                ParentId = null,
                Identifier = "07123335419",
                LastName = "Olsen",
                FirstName = "Kari",
              },
              new Person
              {
                Id = 2,
                ParentId = null,
                Identifier = "24099038175",
                LastName = "Nilsen",
                FirstName = "Per",
              },
              new Person
              {
                Id = 3,
                Identifier = "20047038708",
                LastName = "Gundersen",
                FirstName = "Olai",
              },
              new Person
              {
                Id = 4,
                ParentId = 3,
                Identifier = "13091064469",
                Age = 11,
                LastName = "Gundersen",
                FirstName = "Cecilie",
              },
              new Person
              {
                Id = 5,
                ParentId = 3,
                Identifier = "28030685514",
                Age = 15,
                LastName = "Olsen",
                FirstName = "Per",
              });
            _context.SaveChanges();
        }

        public List<Person> GetPersons() =>
            _context.Persons.OrderBy(p => p.LastName).ToList();

        public IAsyncEnumerable<Person> GetPersonAsync() =>
            _context.Persons.OrderBy(p => p.LastName).AsAsyncEnumerable();

        public bool TryGetPerson(int id, out Person person)
        {
          person = _context.Persons.Find(id);

            return (person != null);
        }

        public async Task<int> AddPersonAsync(Person person)
        {
            _context.Persons.Add(person);
            int rowsAffected = await _context.SaveChangesAsync();
            return person.Id;
        }

        public async Task<int> UpdatePersonAsync(Person person)
        {

          var personToUpdate = _context.Persons.SingleOrDefault(p => p.Identifier == person.Identifier);
          if (personToUpdate == null)
            return -1;

          personToUpdate.LastName = person.LastName;
          personToUpdate.ParentId = person.ParentId;
          personToUpdate.Age = person.Age;
          personToUpdate.FirstName = person.FirstName;
          int rowsAffected = await _context.SaveChangesAsync();
          return person.Id;
        }
    }
}
