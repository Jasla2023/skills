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
    /// Interaktionslogik für SearchAlter.xaml
    /// </summary>
    public partial class SearchAlter : Window
    {
        public SearchAlter()
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

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int empID = DatabaseConnections.GetIDByFirstNameLastNameAndDateOfBirth(tbxFirstName.Text, tbxLastName.Text, new System.Data.SqlTypes.SqlDateTime((DateTime)dpcDateOfBirth.SelectedDate)); 
                SuccessfullyFound successfullyFound = new SuccessfullyFound(empID, tbxFirstName.Text + " " + tbxLastName.Text);
                successfullyFound.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }


        private void SuccessfullyFound(int id)
        {
            
        }

    }
}
