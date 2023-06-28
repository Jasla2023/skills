using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

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
        [Column("Visible")]
        bool visible;

        public int Employee_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public bool Visible { get; set; }

        public ICollection<Skill> Skills { get; set; } // Navigations-Eigenschaft

    }

   

}
