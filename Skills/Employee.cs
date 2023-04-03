using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skills
{
  public class Employee
    {
        int employee_Id;
        string firstName;
        string lastName;
        DateTime birthdate;

        public int Employee_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        
    }

   

}
