using Skills.Properties;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Skills
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            CreateEmployeeWindow cew = new CreateEmployeeWindow();
            

            cew.Show();
        }

        private void btnSearchEmployee_Click(object sender, RoutedEventArgs e)
        {
            SearchEmployeeWindow sew = new SearchEmployeeWindow();
            sew.Show();

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
            //    var employeesWithSkills = from e in db.Employees
            //                              join s in db.Skills on e.employee_id equals s.employee_id
            //                              where skillNames.Contains(s.skillName)
            //                              select new { e.firstname, e.lastname };

            //    var uniqueEmployees = employeesWithSkills.Distinct();

            //    foreach (var employee in uniqueEmployees)
            //    {
            //        Console.WriteLine("{0} {1}", employee.firstname, employee.lastname);
            //    }
            //}
        }

        private void btnSearchEmployeesBySkills_Click(object sender, RoutedEventArgs e)
        {
            RequiredSkillsForm requiredSkillsForm = new RequiredSkillsForm();
            requiredSkillsForm.Show();
        }
    }
}
