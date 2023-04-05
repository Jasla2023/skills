using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void btnSearch_Click(object sender, RoutedEventArgs d)
        {
            using (var context = new EmployeeDb())
            {
                var firstName = tbxFirstName.Text;
                var lastName = tbxLastName.Text;
                var dateOfBirth = dpcDateOfBirth.SelectedDate;

                var query = (from employee in context.Employees
                             where employee.FirstName.Contains(firstName) &&
                                   employee.LastName.Contains(lastName) &&
                                   employee.BirthDate == dateOfBirth
                             select new
                             {
                                 employee.FirstName,
                                 employee.LastName,
                                 employee.BirthDate,
                                 Skills = (from skill in context.Skills
                                           where skill.Employee_Id == employee.Employee_Id
                                           select new { SkillName = skill.SkillName, SkillLevel = skill.SkillLevel })
                                         .ToList()
                             }).ToList();

                if (query.Any())
                {
                    lbxOutput.Items.Clear();
                    lbxOutput.ItemsSource = null;
                    foreach (var item in query)
                    {
                        var skillsText = "";
                        foreach (var skill in item.Skills)
                        {
                            skillsText += $"{skill.SkillName} ({GetSkillLevelText.Compile()(skill.SkillLevel)})\n";
                        }

                        lbxOutput.Items.Add($"{item.FirstName} {item.LastName}:");
                        lbxOutput.Items.Add(skillsText);

                    }
                }
                else
                {
                    lbxOutput.Items.Add("Keine Mitarbeiter gefunden.");
                    lbxOutput.ItemsSource = null;
                }
            }





        }



        static Expression<Func<int, string>> GetSkillLevelText = (level) =>
     level == 1 ? "Grundkenntnisse" :
     level == 2 ? "Fortgeschrittene Kenntnisse" :
     level == 3 ? "Bereits in Projekt eingesetzt" :
     level == 4 ? "Umfangreiche Kenntnisse" :
     "Keine Kenntnisse";









    }

}
       




    




