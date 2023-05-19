using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using Skills;
using System.Data.SqlTypes;

namespace Skills.Properties
{
    /// <summary>
    /// Interaktionslogik für CreateEmployeeWindow.xaml
    /// </summary>
    public partial class CreateEmployeeWindow : Window
    {
        public CreateEmployeeWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Behavior upon clicking on the border of the window (moving the window with the mouse button pressed)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

     


       

        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}

        private int rowsAdded = 0;

        private List<Label> addedSkillLabels = new List<Label>();
        private List<TextBox> addedSkillTextBoxes = new List<TextBox>();
        private List<Label> addedSkillLevelLabels = new List<Label>();
        private List<ComboBox> addedSkillLevelComboBoxes = new List<ComboBox>();

        /// <summary>
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }





        /// <summary>
        /// Adds additional skill and skill level entry  into the UI and in the corresponding lists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnAddMoreSkills_Click(object sender, RoutedEventArgs e)
        {
            Height += 70;
            //grdEntries.Height += 60;
            
            grdEntries.RowDefinitions.Add(new RowDefinition());
            grdEntries.RowDefinitions.Add(new RowDefinition());
            grdEntries.RowDefinitions.Add(new RowDefinition());

            addedSkillLabels.Add(new Label());
            addedSkillLabels.Last().Content = "Kenntnisse: ";
            Grid.SetRow(addedSkillLabels.Last(), 6 + rowsAdded);
            Grid.SetColumn(addedSkillLabels.Last(), 0);
            addedSkillLabels.Last().HorizontalAlignment = HorizontalAlignment.Left;
            addedSkillLabels.Last().VerticalAlignment = VerticalAlignment.Center;

            addedSkillTextBoxes.Add(new TextBox());
            Grid.SetRow(addedSkillTextBoxes.Last(), 6 + rowsAdded);
            Grid.SetColumn(addedSkillTextBoxes.Last(), 1);
            addedSkillTextBoxes.Last().HorizontalAlignment = HorizontalAlignment.Stretch;
            addedSkillTextBoxes.Last().VerticalAlignment = VerticalAlignment.Center;
            
            addedSkillTextBoxes.Last().Style = new Style();
            addedSkillTextBoxes.Last().Height = 25;
                
            addedSkillLevelLabels.Add(new Label());
            addedSkillLevelLabels.Last().Content = "Kentnissstufe: ";
            Grid.SetRow(addedSkillLevelLabels.Last(), 7 + rowsAdded);
            Grid.SetColumn(addedSkillLevelLabels.Last(), 0);
            addedSkillLevelLabels.Last().HorizontalAlignment = HorizontalAlignment.Left;
            addedSkillLevelLabels.Last().VerticalAlignment = VerticalAlignment.Center;

            addedSkillLevelComboBoxes.Add(new ComboBox());
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Grundkenntnisse", IsSelected = true });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Fortgeschrittene Kenntnisse" });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Bereits in Projekt eingesetzt" });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Umfangreiche Projekterfahrungen" });
            Grid.SetRow(addedSkillLevelComboBoxes.Last(), 7 + rowsAdded);
            Grid.SetColumn(addedSkillLevelComboBoxes.Last(), 1);
            addedSkillLevelComboBoxes.Last().HorizontalAlignment = HorizontalAlignment.Stretch;
            addedSkillLevelComboBoxes.Last().VerticalAlignment = VerticalAlignment.Center;
            addedSkillLevelComboBoxes.Last().Height = 25;

            Grid.SetRow(btnAddMoreSkills, 8 + rowsAdded);

            grdEntries.Children.Add(addedSkillLabels.Last());
            grdEntries.Children.Add(addedSkillTextBoxes.Last());
            grdEntries.Children.Add(addedSkillLevelLabels.Last());
            grdEntries.Children.Add(addedSkillLevelComboBoxes.Last());

            Grid.SetRow(btnClose, 7 + rowsAdded);
            Grid.SetRow(btnSave, 7 + rowsAdded);

            rowsAdded+=2;

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        /// <summary>
        /// Saves the new employee in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dpcDateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Geben Sie bitte ein Geburtsdatum ein!");
                return;
            }

            if (tbxFirstName.Text == ""||tbxLastName.Text == ""||tbxSkill.Text=="")
            {
                MessageBox.Show("Alle Felder müssen ausgefüllt sein!");
                return ;
            }

            if (addedSkillTextBoxes.Count > 0)
                foreach (TextBox tbx in addedSkillTextBoxes)
                    if (tbx.Text == "")
                    {
                        MessageBox.Show("Alle Felder müssen ausgefüllt sein!");
                        return;
                    }

            if (DatabaseConnections.Instance.EmployeeExists(tbxFirstName.Text, tbxLastName.Text, new SqlDateTime((DateTime)dpcDateOfBirth.SelectedDate)))
            {
                MessageBox.Show("Der Mitarbeiter existiert schon!");
                return;
            }


            List<string> skillNames = new List<string>();
            List<int> sls = new List<int>();

            foreach (TextBox tb in addedSkillTextBoxes)

            {
                skillNames.Add(tb.Text);
            }

            foreach (ComboBox cb in addedSkillLevelComboBoxes)
            {
                sls.Add(AssignSkillLevel(cb));
            }


            try
            {
                //DatabaseConnections.SaveEmployeeIntoDatabase(tbxFirstName.Text, tbxLastName.Text, new SqlDateTime((DateTime)dpcDateOfBirth.SelectedDate), tbxSkill.Text, AssignSkillLevel(cbxLevel), skillNames, sls);
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message);
            }

            MessageBox.Show("Mitarbeiterdaten erfolgreich erfasst!");
            Close();
        }
    }
}
