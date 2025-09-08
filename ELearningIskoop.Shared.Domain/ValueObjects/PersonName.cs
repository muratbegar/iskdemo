using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Shared.Domain.ValueObjects
{
    // Kişi ismi value object'i
    // Öğrenci ve eğitmen isimleri için kullanılır
    public class PersonName : ValueObject
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private PersonName() { } // EF için

        public PersonName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
    }
}
