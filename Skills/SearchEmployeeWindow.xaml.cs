using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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


        /// <summary>
        ///When entering the first name, last name, and date of birth in the SearchEmployeeWindow, the employee with their name, skill and skill level will be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="d"></param>
        ///<remarks></remarks>
        private void btnSearch_Click(object sender, RoutedEventArgs d)
        {
            lbxOutput.Items.Clear();
            if (tbxFirstName.Text == "" || tbxLastName.Text == "" || dpcDateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Alle Felder müssen ausgefüllt sein!");
                return;
            }

            using (var context = new EmployeeDb())
            {

                var firstName = tbxFirstName.Text;
                var lastName = tbxLastName.Text;
                var dateOfBirth = dpcDateOfBirth.SelectedDate;

                // Der Query sucht in der Datenbank nach Mitarbeitern mit dem angegebenen Vornamen, Nachnamen und Geburtsdatum.
                // Für jeden gefundenen Mitarbeiter wird eine anonyme Typ-Instanz erstellt, die den Vor- und Nachnamen, das Geburtsdatum und eine Liste der Fähigkeiten des Mitarbeiters enthält.
                // Die Liste der Fähigkeiten wird durch eine Unterabfrage erstellt, die nach Fähigkeiten sucht, die dem Mitarbeiter zugeordnet sind, und eine anonyme Typ-Instanz mit dem Namen der Fähigkeit und Level zurückgibt.   
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

                // Falls der Query mindestens einen Mitarbeiter gefunden hat:
                // Die bestehenden Einträge werden aus der ListBox gelöscht
                // - ListBox zurück auf null     
                // - Erstellt einen Text, der die Fähigkeiten des Mitarbeiters auflistet
                // - Fügt den Namen des Mitarbeiters und die Fähigkeiten in die ListBox ein
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
                // Falls der Query keinen Mitarbeiter gefunden hat:
                // - Füge eine Meldung hinzu, dass keine Mitarbeiter gefunden wurden
                // - ListBox zurück auf null
                else
                {
                    lbxOutput.Items.Add("Keine Mitarbeiter gefunden.");
                    lbxOutput.ItemsSource = null;
                }
            }





        }


        /// <summary>
        /// Gibt den Namen eines Skill-Levels als Text zurück, der auf der angegebenen Zahl basiert.
        /// </summary>
        /// <param name="level">Die Nummer des Skill-Levels (1-4).</param>
        /// <returns>Der Name des Skill-Levels als Text.</returns>
        static Expression<Func<int, string>> GetSkillLevelText = (level) =>
     level == 1 ? "Grundkenntnisse" :
     level == 2 ? "Fortgeschrittene Kenntnisse" :
     level == 3 ? "Bereits in Projekt eingesetzt" :
     level == 4 ? "Umfangreiche Kenntnisse" :
     "Keine Kenntnisse";









    }

}
       




    




