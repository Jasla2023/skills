using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skills
{
    class Employee
    {
        int employee_Id;
        string firstName;
        string lastName;

        public int Employee_Id
        {
            get
            {
                return employee_Id;
            }

            set
            {
                employee_Id = value;
            }
        }


        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
            }
        }
    }

   

}
