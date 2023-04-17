using Microsoft.EntityFrameworkCore;
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
using System.Windows.Shapes;

namespace Skills
{
    /// <summary>
    /// Interaktionslogik für EmployeeFound.xaml
    /// </summary>
    public partial class EmployeeFound : Window
    {
        private int employeeId;
        private Employee employee; // Instanzvariable für das Employee-Objekt

        
        public EmployeeFound(int employeeId)
        {
            InitializeComponent();

            this.employeeId = employeeId;

            //Employeedaten aus der Datenbank abrufen
            employee = DatabaseConnections.GetEmployeeById(employeeId); // Initialisierung der Instanzvariable
            tbxFirstName.DataContext = employee;
            tbxLastName.DataContext = employee;
            dpcDateOfBirth.DataContext = employee;

            //// Skills aus der Datenbank abrufen
            //List<Skill> skills = DatabaseConnections.GetEmployeeSkills(employeeId);

            //// DataContext des Views auf die Skills setzen
            //DataContext = skills;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    

        
        private void DeleteSkillButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditSkillButton_Click(object sender, RoutedEventArgs e)
        {
            // Änderungen an Employee-Objekt speichern
            employee.FirstName = tbxFirstName.Text;
            employee.LastName = tbxLastName.Text;
            employee.BirthDate = (DateTime)dpcDateOfBirth.SelectedDate;

            //// Änderungen in der Datenbank speichern
            //using (var context = new EmployeeDb())
            //{
            //    var employeeToUpdate = context.Employees.SingleOrDefault(x => x.Employee_Id == employee.Employee_Id);
            //    if (employeeToUpdate != null)
            //    {
            //        employeeToUpdate.FirstName = employee.FirstName;
            //        employeeToUpdate.LastName = employee.LastName;
            //        employeeToUpdate.BirthDate = employee.BirthDate;

            //        context.SaveChanges();
            //    }
            //}

            // Änderungen in der Datenbank speichern
            using (var context = new EmployeeDb())
            {
                var employeeToUpdate = context.Employees.SingleOrDefault(x => x.Employee_Id == employee.Employee_Id);
                if (employeeToUpdate != null)
                {
                    employeeToUpdate.FirstName = employee.FirstName;
                    employeeToUpdate.LastName = employee.LastName;
                    employeeToUpdate.BirthDate = employee.BirthDate;

                    var selectedSkill = listView.SelectedItem as Skill;
                    if (selectedSkill != null)
                    {
                        var skillToUpdate = context.Skills.SingleOrDefault(x => x.Skill_Id == selectedSkill.Skill_Id);
                        if (skillToUpdate != null)
                        {
                            skillToUpdate.SkillName = selectedSkill.SkillName;
                            skillToUpdate.SkillLevel = selectedSkill.SkillLevel;
                            context.SaveChanges(); // Hier die Änderungen an der Skill-Instanz speichern
                        }
                    }

                    context.SaveChanges(); // Hier die Änderungen an der Employee-Instanz speichern
                }
            }

            //using (var context = new EmployeeDb())
            //{
            //    var selectedSkill = listView.SelectedItem as Skill;
            //    var employeeToUpdate = context.Employees.Include(em => em.Skills).SingleOrDefault(x => x.Employee_Id == employee.Employee_Id);
            //    if (employeeToUpdate != null)
            //    {
            //        foreach (Skill skill in employeeToUpdate.Skills)
            //        {
            //            if (skill.Skill_Id == selectedSkill.Skill_Id)
            //            {
            //                skill.SkillName = selectedSkill.SkillName;
            //                skill.SkillLevelString = selectedSkill.SkillLevelString;
            //            }
            //        }

            //        context.SaveChanges();
            //    }
            //}







        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
