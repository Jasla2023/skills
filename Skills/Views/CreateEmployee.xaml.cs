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
    public partial class CreateEmployee : Window
    {

        private Label[] LabelsForSkills;
        private Label[] LabelsForLevels;
        private Label[] ActualSkills;
        private Label[] ActualLevels;

        private TextBox[] EditaleSkils;
        private ComboBox[] EditableLevls;

        private List<TextBox> newSkill;
        private List<ComboBox> newLevel;

        private Grid[] Grids;

        private List<int> skills;

        private int _id;
        private string firstName;
        private string lastName;
        private DateTime birthDate;

        private int employeeId;
        private Employee employee; // Instanzvariable für das Employee-Objekt

        private int numberOfSkills;
        /// <summary>
        /// Constructor initializing the window
        /// </summary>
        public CreateEmployee()
        {
            InitializeComponent();
            newSkill = new List<TextBox>();
            newLevel = new List<ComboBox>();
            tbxFirstName.PreviewKeyDown += MainWindow.SpecialCharacterHandler;
            tbxLastName.PreviewKeyDown += MainWindow.SpecialCharacterHandler;
        }




        /// <summary>
        /// Adds a new skill entry to the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            newSkill.Add(new TextBox { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center });
            Grid.SetRow(newSkill.Last(), 0);
            Grid.SetColumn(newSkill.Last(), 1);

            newLevel.Add(new ComboBox { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center });
            ComboBoxItem FirstLevel = new ComboBoxItem { Content = "Grundkenntnisse", IsSelected = true };
            ComboBoxItem SecondLevel = new ComboBoxItem { Content = "Fortgeschrittene Kenntnisse" };
            ComboBoxItem ThirdLevel = new ComboBoxItem { Content = "Bereits in Projekt eingesetzt" };
            ComboBoxItem FourthLevel = new ComboBoxItem { Content = "Umfangreiche Projekterfahrungen" };
            newLevel.Last().Items.Add(FirstLevel);
            newLevel.Last().Items.Add(SecondLevel);
            newLevel.Last().Items.Add(ThirdLevel);
            newLevel.Last().Items.Add(FourthLevel);
            Grid.SetRow(newLevel.Last(), 1);
            Grid.SetColumn(newLevel.Last(), 1);

            newGrid.Children.Add(newLabel1);
            newGrid.Children.Add(newLabel2);
            newGrid.Children.Add(newSkill.Last());
            newGrid.Children.Add(newLevel.Last());

            lvwSkillInput.Items.Insert(rowNumber, newGrid);











            //AddASkill addASkill = new AddASkill();
            //addASkill.Show();
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
        /// <summary>
        /// Checks if all skill entries are filled, if so, then saves the employee into the database, shows a success message and closes the window, otherwise shows an error
        /// message and stops
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            
            List<string> s = new List<string>();
            List<int> l = new List<int>();

            if (tbxFirstName.Text == "" || tbxLastName.Text == "" || dpcDateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Die oberen Felder müssen ausgefüllt werden!");
                return;
            }

            //if (tbxLastName.Text == "")
            //{
            //    MessageBox.Show("Geben Sie bitte Nachname ein!");
            //    return;
            //}

            //if (dpcDateOfBirth.SelectedDate == null)
            //{
            //    MessageBox.Show("Geben Sie bitte Geburtsdatum ein!");
            //    return;
            //}


            foreach (TextBox sk in newSkill)
            {
                if (sk.Text == "")
                {
                    MessageBox.Show("Es wurde kein Skill angegeben");
                    return;
                }
                s.Add(sk.Text);
                l.Add(AssignSkillLevel(newLevel[newSkill.IndexOf(sk)]));
            }
            
            //if (s.Contains(""))
            //{
            //    MessageBox.Show("Eine oder mehrere Kenntnisse sind leer");
            //    return;
            //}

           
            if(lvwSkillInput.Items.Count == 0)
            {
                MessageBox.Show("Mindestens eine Kenntnis muss eingegeben werden!");
                return; 
            }

            if(DatabaseConnections.EmployeeExists(tbxFirstName.Text, tbxLastName.Text, new SqlDateTime((DateTime)dpcDateOfBirth.SelectedDate)))
            {
                MessageBox.Show("Mitarbeiter existiert schon!");
                return;
            }
            
            foreach(string skill1 in s)
            {
                foreach(string skill2 in s)
                {
                    if (!Object.ReferenceEquals(skill1,skill2))
                    {
                        if(skill1 == skill2)
                        {
                            MessageBox.Show("Dieselbe Kenntnis kann nicht mehrfach eingegeben werden!");
                            return;
                        }
                    }
                }
            }
            
            try
            {
                DatabaseConnections.SaveEmployeeIntoDatabase(tbxFirstName.Text, tbxLastName.Text, new SqlDateTime((DateTime)dpcDateOfBirth.SelectedDate), s, l);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Mitarbeiter erfolgreich erfasst!");

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
        /// <summary>
        /// Closes the window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}