using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skills
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [Column("Employee_Id")]
        int employee_Id;
        [Column("FirstName")]
        string firstName;
        [Column("LastName")]
        string lastName;
        [Column("BirthDate")]
        DateTime birthdate;

        public int Employee_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public ICollection<Skill> Skills { get; set; } // Navigations-Eigenschaft

    }

   

}
