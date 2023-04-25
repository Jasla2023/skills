using System;
using System.Collections.Generic;
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

namespace Skills.Views
{
    /// <summary>
    /// Interaktionslogik für CreateEmployeeWindow1.xaml
    /// </summary>
    public partial class CreateEmployeeWindow1 : Window
    {

        private Label[] LabelsForSkills;
        private Label[] LabelsForLevels;
        private Label[] ActualSkills;
        private Label[] ActualLevels;

        private TextBox[] EditaleSkils;
        private ComboBox[] EditableLevls;

        private TextBox newSkill;
        private ComboBox newLevel;

        private Grid[] Grids;

        private List<int> skills;

        private int _id;
        private string firstName;
        private string lastName;
        private DateTime birthDate;

        private int employeeId;
        private Employee employee; // Instanzvariable für das Employee-Objekt

        private int numberOfSkills;

        public CreateEmployeeWindow1()
        {
            InitializeComponent();
        }





        private void btnSkillAdd_Click(object sender, RoutedEventArgs e)
        {
            // Zähle die Anzahl der vorhandenen Zeilen im ListView
            int rowNumber = lvwSkillInput.Items.Count;

            // Erstelle eine neue Grid-Zeile und füge sie hinzu
            Grid newGrid = new Grid();
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25.0) });
            newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70.0) });
            newGrid.RowDefinitions.Add(new RowDefinition());
            newGrid.RowDefinitions.Add(new RowDefinition());

            Label newLabel1 = new Label { Content = "Kenntnisse: ", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.Bold };
            Grid.SetRow(newLabel1, 0);
            Grid.SetColumn(newLabel1, 0);

            Label newLabel2 = new Label { Content = "Stufe: ", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.Bold };
            Grid.SetRow(newLabel2, 1);
            Grid.SetColumn(newLabel2, 0);

            newSkill = new TextBox { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(newSkill, 0);
            Grid.SetColumn(newSkill, 1);

            newLevel = new ComboBox { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
            ComboBoxItem FirstLevel = new ComboBoxItem { Content = "Grundkenntnisse", IsSelected = true };
            ComboBoxItem SecondLevel = new ComboBoxItem { Content = "Fortgeschrittene Kenntnisse" };
            ComboBoxItem ThirdLevel = new ComboBoxItem { Content = "Bereits in Projekt eingesetzt" };
            ComboBoxItem FourthLevel = new ComboBoxItem { Content = "Umfangreiche Projekterfahrungen" };
            newLevel.Items.Add(FirstLevel);
            newLevel.Items.Add(SecondLevel);
            newLevel.Items.Add(ThirdLevel);
            newLevel.Items.Add(FourthLevel);
            Grid.SetRow(newLevel, 1);
            Grid.SetColumn(newLevel, 1);

            newGrid.Children.Add(newLabel1);
            newGrid.Children.Add(newLabel2);
            newGrid.Children.Add(newSkill);
            newGrid.Children.Add(newLevel);

            lvwSkillInput.Items.Insert(rowNumber, newGrid);











            //AddASkill addASkill = new AddASkill();
            //addASkill.Show();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }



        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
         
          int _id =  DatabaseConnections.GetIDByFirstNameLastNameAndDateOfBirth(tbxFirstName.Text, tbxLastName.Text, (DateTime)dpcDateOfBirth.SelectedDate);
          


            Close();

            //EmployeeFound newWindow = new EmployeeFound(_id, DatabaseConnections.GetFirstNameByID(_id), DatabaseConnections.GetLastNameByID(_id), new SqlDateTime((DateTime)DatabaseConnections.GetDateOfBirthByID(_id)));
            //newWindow.Show();

            //for (int i = 0; i < skills.Count; i++)
            //{
            //    DatabaseConnections.ModifySkill(DatabaseConnections.GetSkillIDBySkillNameAndOwnerID(ActualSkills[i].Content.ToString(), _id), EditaleSkils[i].Text, AssignSkillLevel(EditableLevls[i]));

            //}
            //MessageBox.Show("Die Daten wurden erfolgreich geändert!");
            //Close();





        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       
    }
}
