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
using System.Xml.Linq;

namespace Skills
{
    /// <summary>
    /// Interaktionslogik für SearchEmployee2.xaml
    /// </summary>
    public partial class SearchEmployee2 : Window
    {
        public SearchEmployee2()
        {
            InitializeComponent();
           
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

        private void btnSearchEmployee_Click(object sender, RoutedEventArgs e)
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



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbxName = sender as TextBox;
            var searchTerm = tbxName.Text;

            using (var context = new EmployeeDb())
            {
                var employees = context.Employees
                    .Where(emp => emp.FirstName.Contains(searchTerm) || emp.LastName.Contains(searchTerm))
                    .ToList();
                dataGrid.ItemsSource = employees;
            }
        }


    }
}
