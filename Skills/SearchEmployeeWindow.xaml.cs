using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Skills
{
    /// <summary>
    /// Interaktionslogik für SearchEmployeeWindow.xaml
    /// </summary>
    public partial class SearchEmployeeWindow : Window
    {
        public SearchEmployeeWindow()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (dpcDateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Geben Sie bitte ein Geburtsdatum ein!");
                return;
            }

            if (tbxFirstName.Text == null)
            {
                MessageBox.Show("Geben Sie bitte den Vornamen ein!");
                return;
            }

            if (tbxLastName.Text == null)
            {
                MessageBox.Show("Geben Sie bitte den Nachnamen ein!");
                return;
            }




            try
            {
                //using (var db = new EmployeeDb())
                //{
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var employee in db.Employees)
                //    {
                //        string birthDate = employee.BirthDate.ToString("dd.MM.yyyy");
                //        sb.AppendLine($"{employee.Employee_Id} {employee.FirstName} {employee.LastName} {birthDate}");
                //    }
                //    MessageBox.Show(sb.ToString(), "Alle Mitarbeiter");
                //}


                //string[] skillNames = { "Java", "C#" };

                //using (var db = new EmployeeDb())
                //{
                //    var employeesWithSkills = from em in db.Employees
                //                              join sk in db.Skills on em.employee_id equals sk.employee_id
                //                              where skillNames.Contains(sk.skillName)
                //                              select new { em.firstname, em.lastname };

                //    var uniqueEmployees = employeesWithSkills.Distinct();

                //    foreach (var employee in uniqueEmployees)
                //    {
                //        Console.WriteLine("{0} {1}", employee.firstname, employee.lastname);
                //    }
                //}


                //using (var db = new EmployeeDb())
                //{
                //    var employeesWithSkills = from em in db.Employees
                //                              join sk in db.Skills on em.Employee_Id equals sk.Employee_Id
                //                              where em.FirstName == tbxFirstName.Text && em.LastName == tbxLastName.Text && em.BirthDate == dpcDateOfBirth.SelectedDate
                //                              select new { em.FirstName, em.LastName, sk.SkillName, sk.SkillLevel };

                //    string result = "";
                //    foreach (var employee in employeesWithSkills)
                //    {
                //        result += $"{employee.FirstName} {employee.LastName} - {employee.SkillName}: {employee.SkillLevel}\n";
                //    }


                //    tbxOutput.Text = result;
                //}

                using (var db = new EmployeeDb())
                {
                    var employeesWithSkills = from em in db.Employees
                                              join g in (
                                                  from sk in db.Skills
                                                  group sk by new { sk.Employee_Id, sk.SkillName } into g
                                                  select new { g.Key.Employee_Id, g.Key.SkillName, SkillLevel = g.Max(s => s.SkillLevel) }
                                              ) on em.Employee_Id equals g.Employee_Id
                                              where em.FirstName == tbxFirstName.Text && em.LastName == tbxLastName.Text && em.BirthDate == dpcDateOfBirth.SelectedDate
                                              group g by new { em.FirstName, em.LastName } into g
                                              select new { g.Key.FirstName, g.Key.LastName, Skills = g.Select(s => new { s.SkillName, s.SkillLevel }) };

                    string result = "";
                    foreach (var employee in employeesWithSkills)
                    {
                        result += $"{employee.FirstName} {employee.LastName} -";

                        foreach (var skill in employee.Skills)
                        {
                            result += $" {skill.SkillName}: {skill.SkillLevel},";
                        }

                        result = result.TrimEnd(',') + "\n";
                    }

                    tbxOutput.Text = result;
                }







            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message);
            }

        }
    }
}
