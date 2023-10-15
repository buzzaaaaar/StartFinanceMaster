using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class PersonalInfo
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

        public string DOB { get; set; }

        [NotNull]
        public string Gender { get; set; }

        [NotNull]
        public string EmailAddress { get; set; }

        [NotNull]
        public int MobilePhone { get; set; }
    }
}
