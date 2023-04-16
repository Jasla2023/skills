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
        public EmployeeFound(int employeeId)
        {
            InitializeComponent();

            this.employeeId = employeeId;

            // Skills aus der Datenbank abrufen
            List<Skill> skills = DatabaseConnections.GetEmployeeSkills(employeeId);

            // DataContext des Views auf die Skills setzen
            DataContext = skills;
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

        }

      
    }
}
