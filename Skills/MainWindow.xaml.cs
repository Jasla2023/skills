using Skills.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
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
        /// <summary>
        /// Evemt, happening upon clicking on the border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /// <summary>
        /// The method restricting the special characters upon entry into the first and last name text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SpecialCharacterHandler(object sender, KeyEventArgs e)
        {
            Key[] forbiddenKeys = { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.Decimal, Key.Divide, Key.OemBackslash, Key.OemOpenBrackets,
                                    Key.OemCloseBrackets, Key.OemCloseBrackets, Key.OemComma, Key.OemPlus, Key.OemMinus, Key.OemQuestion, Key.OemPeriod, Key.OemQuotes,
                                    Key.OemSemicolon, Key.OemTilde, Key.Separator, Key.Add, Key.Subtract, Key.Multiply};
            if (forbiddenKeys.Contains(e.Key))
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// Opens an employee adding prompt window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            Views.CreateEmployee cew = new Views.CreateEmployee();

            string init = DatabaseConnections.Instance.accessToken;

            cew.Show();
        }

        
        

    

        
        /// <summary>
        /// Opens the employee search form window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch2_Click(object sender, RoutedEventArgs e)
        {
            string init = DatabaseConnections.Instance.accessToken;
            SearchEmployee2 se = new SearchEmployee2();
            
            se.Show();
        }
        /// <summary>
        /// Opens the required skills entry form window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch3_Click(object sender, RoutedEventArgs e)
        {
            RequiredSkills re = new RequiredSkills();
            string init = DatabaseConnections.Instance.accessToken;
            re.Show();
        }
    }
}
