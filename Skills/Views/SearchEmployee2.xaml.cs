using System;
using System.Collections.Generic;
using System.Data;
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
using System.Xml.Linq;
using System.Data.SqlTypes;
using System.Runtime.Remoting.Contexts;

namespace Skills
{
    /// <summary>
    /// Interaktionslogik für SearchEmployee2.xaml
    /// </summary>
    public partial class SearchEmployee2 : Window
    {

        private EmployeeDb context;

        private List<Employee> employees;   
        public SearchEmployee2()
        {
            InitializeComponent();

            context = new EmployeeDb();
            employees =context.Employees.ToList();
            dataGrid.DataContext = employees;

          

        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        

        private void ButtonSearchEmployee2_Click(object sender, RoutedEventArgs e)
        {

        }

        



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbxName = sender as TextBox;
            var searchTerm = tbxName.Text;

            var emps = employees
                   .Where(emp => emp.FirstName.Contains(searchTerm) || emp.LastName.Contains(searchTerm))
                   .ToList();
            //dataGrid.ItemsSource = emps;

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee sr = (Employee)((DataGrid)dataGrid).SelectedItem;
            EmployeeFound ef = new EmployeeFound(DatabaseConnections.GetIDByFirstNameLastNameAndDateOfBirth(sr.FirstName, sr.LastName, new SqlDateTime(sr.BirthDate)), sr.FirstName, sr.LastName, new SqlDateTime(sr.BirthDate));
            Close();
            //SearchEmployee2 se = new SearchEmployee2();
            //se.Show();
            ef.Show();
            
            
        }

        private void btnSearchEmployee_Click(object sender, KeyEventArgs e)
        {
            var searchNames = tbxName.Text.Split(' ');
            using (var context = new EmployeeDb())
            {
                var employees = context.Employees
                    .AsEnumerable()
                    .Where(emp => searchNames.All(name => emp.FirstName.ToLower().Contains(name.ToLower()) || emp.LastName.ToLower().Contains(name.ToLower())))
                    .ToList();
                dataGrid.ItemsSource = employees;
            }
        }
    }
}
