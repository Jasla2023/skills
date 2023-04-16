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
    /// Interaktionslogik für SuccessfullyFound.xaml
    /// </summary>
    public partial class SuccessfullyFound : Window
    {

        private Label[] LabelsForSkills;
        private Label[] LabelsForLevels;
        private Label[] ActualSkills;
        private Label[] ActualLevels;
        private Button[] DeleteSkill;
        private Button[] EditSkill;
        private Button[] EditLevel;
        private TextBox[] EditaleSkils;
        private ComboBox[] EditableLevls;

        private List<int> skills;

        private int _id;


        public SuccessfullyFound(int id, string vornameNachname)
        {

            InitializeComponent();

            _id = id;
            
            tblWindowName.Text += "Kenntnisse von " + vornameNachname;


            skills = DatabaseConnections.GetSkillsOfAnEmployee(id);
            int numberOfSkills = skills.Count;

            LabelsForSkills = new Label[numberOfSkills];
            LabelsForLevels = new Label[numberOfSkills];
            ActualSkills = new Label[numberOfSkills];
            ActualLevels = new Label[numberOfSkills];
            DeleteSkill = new Button[numberOfSkills];
            EditSkill = new Button[numberOfSkills];
            EditLevel = new Button[numberOfSkills];

            EditaleSkils = new TextBox[numberOfSkills];
            EditableLevls = new ComboBox[numberOfSkills];

            int rowsAdded = 0;
            
            for (int i = 0; i < numberOfSkills; i++)
            {
                grdAlter.RowDefinitions.Add(new RowDefinition());
                grdAlter.RowDefinitions.Add(new RowDefinition());
                rowsAdded += 2;

                LabelsForSkills[i] = new Label { Content = "Kenntnisse: ", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Height = 25 };
                Grid.SetRow(LabelsForSkills[i], i * 2);
                Grid.SetColumn(LabelsForSkills[i], 0);

                LabelsForLevels[i] = new Label { Content = "Stufe: ", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Height = 25 };
                Grid.SetRow(LabelsForLevels[i], i * 2 + 1);
                Grid.SetColumn(LabelsForLevels[i], 0);

                ActualSkills[i] = new Label { Content = DatabaseConnections.GetSkillByID(skills[i]), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25 };
                Grid.SetRow(ActualSkills[i], i * 2);
                Grid.SetColumn(ActualSkills[i], 1);

                ActualLevels[i] = new Label { Content = Level_DigitToString(DatabaseConnections.GetSkillLevelByID(skills[i])), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25 };
                Grid.SetRow(ActualLevels[i], i * 2 + 1);
                Grid.SetColumn(ActualLevels[i], 1);

                DeleteSkill[i] = new Button { Content = "X"};
                DeleteSkill[i].Click += deleteSkill_Click;
                Grid.SetRow(DeleteSkill[i], i * 2);
                Grid.SetColumn(DeleteSkill[i], 2);

                EditSkill[i] = new Button { Content = "Ändern"};
                EditSkill[i].Click += editSkill_Click;
                Grid.SetRow(EditSkill[i], i * 2);
                Grid.SetColumn(EditSkill[i], 3);

                EditLevel[i] = new Button { Content = "Ändern" };
                EditLevel[i].Click += editLevel_Click;
                Grid.SetRow(EditLevel[i], i * 2 + 1);
                Grid.SetColumn(EditLevel[i], 3);

                EditaleSkils[i] = new TextBox { Text = (string)ActualSkills[i].Content, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Height = 25, Visibility = Visibility.Hidden };
                Grid.SetRow(EditaleSkils[i], i * 2);
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



                grdAlter.Children.Add(LabelsForSkills[i]);
                grdAlter.Children.Add(LabelsForLevels[i]);
                grdAlter.Children.Add(ActualSkills[i]);
                grdAlter.Children.Add(ActualLevels[i]);
                grdAlter.Children.Add(DeleteSkill[i]);
                grdAlter.Children.Add(EditSkill[i]);
                grdAlter.Children.Add(EditLevel[i]);
                grdAlter.Children.Add(EditaleSkils[i]);
                grdAlter.Children.Add(EditableLevls[i]);


            }
           
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// Converts a digit representing the skill level into its descriprion
        /// </summary>
        /// <param name="l">a level in the digital form within the range [1;4]</param>
        /// <returns>Descriprion of the level</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws an ArgumentOutOfRangeException if the argument is not an integer between 1 and 4</exception>
        private string Level_DigitToString(int l)
        {
            switch (l)
            {
                case 1: return "Grundkenntnisse";
                case 2: return "Fortgeschrittene Kenntnisse";
                case 3: return "Bereits in Projekt eingesetzt";
                case 4: return "Umfangreiche Projekterfahrungen";
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void deleteSkill_Click(object sender, RoutedEventArgs e)
        {
            int orderWithinOneEmployee = Array.IndexOf(DeleteSkill, (Button)sender);
            DatabaseConnections.DeleteSkill(skills[orderWithinOneEmployee]);
            LabelsForSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            LabelsForLevels[orderWithinOneEmployee].Visibility= Visibility.Hidden;
            ActualSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            ActualLevels[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            DeleteSkill[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            EditSkill[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            EditLevel[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            EditaleSkils[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            EditableLevls[orderWithinOneEmployee].Visibility = Visibility.Hidden;
        }

        private void editSkill_Click(object sender, RoutedEventArgs e)
        {
            int orderWithinOneEmployee = Array.IndexOf(EditSkill, (Button)sender);
            ActualSkills[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            EditaleSkils[orderWithinOneEmployee].Visibility = Visibility.Visible;

            /* foreach(Label ActSk in ActualSkills)
             {
                 if (ActSk != ActualSkills[orderWithinOneEmployee])
                     ActSk.Visibility = Visibility.Visible;
             }

             foreach(TextBox EdSk in EditaleSkils)
             {
                 if(EdSk != EditaleSkils[orderWithinOneEmployee])
                     EdSk.Visibility = Visibility.Collapsed;
             }

             foreach(Label ActLvl in ActualLevels)
                 ActLvl.Visibility = Visibility.Visible;

             foreach (ComboBox EdLvl in EditableLevls)
                 EdLvl.Visibility = Visibility.Collapsed;
            */


        }


        private void editLevel_Click(object sender, RoutedEventArgs e)
        {
            int orderWithinOneEmployee = Array.IndexOf(EditLevel, (Button)sender);
            ActualLevels[orderWithinOneEmployee].Visibility = Visibility.Hidden;
            EditableLevls[orderWithinOneEmployee].Visibility = Visibility.Visible;


            /*foreach (Label ActSk in ActualSkills)
            {
                
                    ActSk.Visibility = Visibility.Visible;
            }

            foreach (TextBox EdSk in EditaleSkils)
            {
                
                    EdSk.Visibility = Visibility.Collapsed;
            }

            foreach (Label ActLvl in ActualLevels)
            {
                if(ActLvl != ActualLevels[orderWithinOneEmployee])
                    ActLvl.Visibility = Visibility.Visible;
            }

            foreach (ComboBox EdLvl in EditableLevls)
            {
                if(EdLvl != EditableLevls[orderWithinOneEmployee])
                    EdLvl.Visibility = Visibility.Collapsed;
            }*/
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < skills.Count; i++)
            {
                DatabaseConnections.ModifySkill(DatabaseConnections.GetSkillIDBySkillNameAndOwnerID(ActualSkills[i].Content.ToString(), _id), EditaleSkils[i].Text, AssignSkillLevel(EditableLevls[i]));

            }
            MessageBox.Show("Die Daten wurden erfolgreich geändert!");
            Close();
        }
    }
}
