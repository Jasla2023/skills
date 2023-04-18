using Microsoft.EntityFrameworkCore;
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

namespace Skills
{
    /// <summary>
    /// Interaktionslogik für EmployeeFound.xaml
    /// </summary>
    public partial class EmployeeFound : Window
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


        public EmployeeFound(int id, string firstName, string lastName, SqlDateTime birthDate)
        {

            InitializeComponent();

            _id = id;
            
            this.firstName = firstName;
            this.lastName = lastName;
            this.birthDate = (DateTime)birthDate;


         
            skills = DatabaseConnections.GetSkillsOfAnEmployee(id);
            numberOfSkills = skills.Count;

            Grids = new Grid[numberOfSkills];

            LabelsForSkills = new Label[numberOfSkills];
            LabelsForLevels = new Label[numberOfSkills];
            ActualSkills = new Label[numberOfSkills];
            ActualLevels = new Label[numberOfSkills];
            

            EditaleSkils = new TextBox[numberOfSkills];
            EditableLevls = new ComboBox[numberOfSkills];

            

            //int rowsAdded = 0;



            for (int i = 0; i < numberOfSkills; i++)
            {
                Grids[i] = new Grid();
                Grids[i].ColumnDefinitions.Add(new ColumnDefinition());
                Grids[i].ColumnDefinitions.Add(new ColumnDefinition());
                Grids[i].ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25.0) });
                Grids[i].ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70.0) });

                Grids[i].RowDefinitions.Add(new RowDefinition());
                Grids[i].RowDefinitions.Add(new RowDefinition());
                //rowsAdded += 2;

                LabelsForSkills[i] = new Label { Content = "Kenntnisse: ", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Height = 25, FontWeight = FontWeights.Bold };
                Grid.SetRow(LabelsForSkills[i], 0);
                Grid.SetColumn(LabelsForSkills[i], 0);

                LabelsForLevels[i] = new Label { Content = "Stufe: ", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25, FontWeight = FontWeights.Bold };
                Grid.SetRow(LabelsForLevels[i], 1);
                Grid.SetColumn(LabelsForLevels[i], 0);

                ActualSkills[i] = new Label { Content = DatabaseConnections.GetSkillByID(skills[i]), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25 };
                Grid.SetRow(ActualSkills[i], 0);
                Grid.SetColumn(ActualSkills[i], 1);

                ActualLevels[i] = new Label { Content = DatabaseConnections.Level_DigitToString(DatabaseConnections.GetSkillLevelByID(skills[i])), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25 };
                Grid.SetRow(ActualLevels[i], 1);
                Grid.SetColumn(ActualLevels[i], 1);

                /*DeleteSkill[i] = new Button { Content = "X", Width = 25, HorizontalContentAlignment = HorizontalAlignment.Center, Background = Brushes.Red, Foreground = Brushes.White };
                DeleteSkill[i].Click += deleteSkill_Click;
                Grid.SetRow(DeleteSkill[i], 0);
                Grid.SetColumn(DeleteSkill[i], 2);

                EditSkill[i] = new Button { Content = "Ändern", Width = 60, HorizontalContentAlignment = HorizontalAlignment.Center };
                EditSkill[i].Click += editSkill_Click;
                Grid.SetRow(EditSkill[i], 0);
                Grid.SetColumn(EditSkill[i], 6);

                EditLevel[i] = new Button { Content = "Ändern", Width = 60, HorizontalContentAlignment = HorizontalAlignment.Center };
                EditLevel[i].Click += editLevel_Click;
                Grid.SetRow(EditLevel[i], 1);
                Grid.SetColumn(EditLevel[i], 6);*/


                EditaleSkils[i] = new TextBox { Text = (string)ActualSkills[i].Content, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 45, Visibility = Visibility.Hidden };
                Grid.SetRow(EditaleSkils[i], 0);
                Grid.SetColumn(EditaleSkils[i], 1);

                EditableLevls[i] = new ComboBox { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25, Visibility = Visibility.Hidden };
                ComboBoxItem FirstLevel = new ComboBoxItem { Content = "Grundkenntnisse" };
                ComboBoxItem SecondLevel = new ComboBoxItem { Content = "Fortgeschrittene Kenntnisse" };
                ComboBoxItem ThirdLevel = new ComboBoxItem { Content = "Bereits in Projekt eingesetzt" };
                ComboBoxItem FourthLevel = new ComboBoxItem { Content = "Umfangreiche Projekterfahrungen" };
                EditableLevls[i].Items.Add(FirstLevel);
                EditableLevls[i].Items.Add(SecondLevel);
                EditableLevls[i].Items.Add(ThirdLevel);
                EditableLevls[i].Items.Add(FourthLevel);
                switch (DatabaseConnections.GetSkillLevelByID(skills[i]))
                {
                    case 1:
                        FirstLevel.IsSelected = true;
                        break;
                    case 2:
                        SecondLevel.IsSelected = true;
                        break;
                    case 3:
                        ThirdLevel.IsSelected = true;
                        break;
                    case 4:
                        FourthLevel.IsSelected = true;
                        break;
                }
                Grid.SetRow(EditableLevls[i], i * 2 + 1);
                Grid.SetColumn(EditableLevls[i], 1);



                Grids[i].Children.Add(LabelsForSkills[i]);
                Grids[i].Children.Add(LabelsForLevels[i]);
                Grids[i].Children.Add(ActualSkills[i]);
                Grids[i].Children.Add(ActualLevels[i]);
                
                Grids[i].Children.Add(EditaleSkils[i]);
                Grids[i].Children.Add(EditableLevls[i]);


                lvwOutput.Items.Add(Grids[i]);

                tbxFirstName.Text = DatabaseConnections.GetFirstNameByID(id);
                tbxLastName.Text = DatabaseConnections.GetLastNameByID(id);

                dpcDateOfBirth.SelectedDate = DatabaseConnections.GetDateOfBirthByID(id);
            }

        }

        //private void deleteSkill_Click(object sender, RoutedEventArgs e)
        //{
        //    int orderWithinOneEmployee = Array.IndexOf(DeleteSkill, (Button)sender);
        //    DatabaseConnections.DeleteSkill(skills[orderWithinOneEmployee]);
        //    LabelsForSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    LabelsForLevels[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    ActualSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    ActualLevels[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    DeleteSkill[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    EditSkill[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    EditLevel[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    EditaleSkils[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    EditableLevls[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    //Skills an der Stelle i löschen
        //    //i ist ListView SelectedItem
        //}


        //private void editSkill_Click(object sender, RoutedEventArgs e)
        //{
        //    int orderWithinOneEmployee = Array.IndexOf(EditSkill, (Button)sender);
        //    ActualSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    EditaleSkils[orderWithinOneEmployee].Visibility = Visibility.Visible;

           
        //}

        //private void editLevel_Click(object sender, RoutedEventArgs e)
        //{
        //    int orderWithinOneEmployee = Array.IndexOf(EditLevel, (Button)sender);
        //    ActualLevels[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        //    EditableLevls[orderWithinOneEmployee].Visibility = Visibility.Visible;
        //}

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    

        
        private void DeleteSkillButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Converts a skill level ComboBox into a digit representing the skill level
        /// </summary>
        /// <param name="skillLevel">The ComboBox used for selecting a skill level</param>
        /// <returns>Returns a level based on its description selected in the ComboBox within the range [1;4]</returns>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the ComboBox is not suitable for selecting a skill level aka doesn't have the necerssary ComboBoxItems</exception>
        private int AssignSkillLevel(ComboBox skillLevel)
        {

            int level;
            switch ((skillLevel.SelectedItem as ComboBoxItem).Content.ToString())
            {
                case "Grundkenntnisse":
                    level = 1;
                    break;

                case "Fortgeschrittene Kenntnisse":
                    level = 2;
                    break;

                case "Bereits in Projekt eingesetzt":
                    level = 3;
                    break;


                case "Umfangreiche Projekterfahrungen":
                    level = 4;
                    break;


                default: throw new ArgumentException("Not a skilllevel ComboBox!");
            }
            return level;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string actualFirstName = tbxFirstName.Text;
            string actualLastName = tbxLastName.Text;
            DateTime actualDateOfBirth = (DateTime)dpcDateOfBirth.SelectedDate;


            if (DatabaseConnections.GetFirstNameByID(_id) != actualFirstName || DatabaseConnections.GetLastNameByID(_id) != actualLastName || DatabaseConnections.GetDateOfBirthByID(_id) != actualDateOfBirth)
            {
                DatabaseConnections.UpdateEmployee(_id, actualFirstName, actualLastName, actualDateOfBirth);
               
            }
           


                for (int i = 0; i < skills.Count; i++)
                {

                if (DatabaseConnections.SkillExists(EditaleSkils[i].Text, _id) && EditaleSkils[i].Text != DatabaseConnections.GetSkillByID(DatabaseConnections.GetSkillsOfAnEmployee(_id)[i]))
                {
                    MessageBox.Show("Dieser Mitarbeiter hat bereits diesen Kenntnis.");
                    return;
                }
                else
                    DatabaseConnections.ModifySkill(DatabaseConnections.GetSkillIDBySkillNameAndOwnerID(ActualSkills[i].Content.ToString(), _id), EditaleSkils[i].Text, AssignSkillLevel(EditableLevls[i]));
                }

            if (newLevel != null)
            {


                if (DatabaseConnections.SkillExists(newSkill.Text, _id))
                {
                    MessageBox.Show("Dieser Mitarbeiter hat bereits diesen Kenntnis.");
                    return;
                }
                else
                    DatabaseConnections.AddSkill(newSkill.Text, AssignSkillLevel(newLevel), _id);
            }
            

            
            Close();

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

        private void btnAddSkill_Click(object sender, RoutedEventArgs e)
        {
            // Zähle die Anzahl der vorhandenen Zeilen im ListView
            int rowNumber = lvwOutput.Items.Count;

            // Erstelle eine neue Grid-Zeile und füge sie hinzu
            Grid newGrid = new Grid();
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25.0) });
            newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70.0) });
            newGrid.RowDefinitions.Add(new RowDefinition());
            newGrid.RowDefinitions.Add(new RowDefinition());

            Label newLabel1 = new Label { Content = "Kenntnisse: ", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Height = 25, FontWeight = FontWeights.Bold };
            Grid.SetRow(newLabel1, 0);
            Grid.SetColumn(newLabel1, 0);

            Label newLabel2 = new Label { Content = "Stufe: ", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25, FontWeight = FontWeights.Bold };
            Grid.SetRow(newLabel2, 1);
            Grid.SetColumn(newLabel2, 0);

            newSkill = new TextBox {  HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 45 };
            Grid.SetRow(newSkill, 0);
            Grid.SetColumn(newSkill, 1);

            newLevel = new ComboBox { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25 };
            ComboBoxItem FirstLevel = new ComboBoxItem { Content = "Grundkenntnisse" , IsSelected = true};
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

            lvwOutput.Items.Insert(rowNumber, newGrid);











            //AddASkill addASkill = new AddASkill();
            //addASkill.Show();
        }

        private void btnSkillChange_Click(object sender, RoutedEventArgs e)
        {

            int orderWithinOneEmployee = lvwOutput.Items.IndexOf(lvwOutput.SelectedItem);
            try
            {
                ActualSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
                EditaleSkils[orderWithinOneEmployee].Visibility = Visibility.Visible;
                ActualLevels[orderWithinOneEmployee].Visibility = Visibility.Hidden;
                EditableLevls[orderWithinOneEmployee].Visibility = Visibility.Visible;
            }
            catch(IndexOutOfRangeException) 
            {
                MessageBox.Show("Was genau möchten Sie ändern?");
            }
        }

        private void btnDeleteSkill_Click(object sender, RoutedEventArgs e)
        {
            int orderWithinOneEmployee = lvwOutput.Items.IndexOf(lvwOutput.SelectedItem);
            DatabaseConnections.DeleteSkill(skills[orderWithinOneEmployee]);
            try
            {
                LabelsForSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
                LabelsForLevels[orderWithinOneEmployee].Visibility = Visibility.Hidden;
                ActualSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
                ActualLevels[orderWithinOneEmployee].Visibility = Visibility.Hidden;

                EditaleSkils[orderWithinOneEmployee].Visibility = Visibility.Hidden;
                EditableLevls[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Was genau möchten Sie ändern?");
            }
        }

        private void btnDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            DatabaseConnections.DeleteEmployee(_id);
            
        }
    }
}
