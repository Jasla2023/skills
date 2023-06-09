﻿using System;
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
    /// Interaktionslogik für RequiredSkills.xaml
    /// </summary>
    public partial class RequiredSkills : Window
    {

        private List<StackPanel> requiredSkills;
        private List<ComboBox> requiredSkillsTextBoxes;
        private List<ComboBox> requiredSkillsLevels;
        private int numOfSkills;
        /// <summary>
        /// Initializes the window 
        /// </summary>
        public RequiredSkills()
        {

            InitializeComponent();

            //MessageBox.Show("Bitte sortieren Sie die Kenntnisse absteigend nach Wichtigkeit beim Eingeben");

            requiredSkills = new List<StackPanel>();
            requiredSkillsTextBoxes = new List<ComboBox>();
            requiredSkillsLevels = new List<ComboBox>();

            requiredSkills.Add(new StackPanel { Orientation = Orientation.Horizontal});

            requiredSkillsTextBoxes.Add(DatabaseConnections.Instance.SkillSuggestions());
            requiredSkillsTextBoxes.Last().MinWidth = 260.0;
            requiredSkillsTextBoxes.Last().Margin = new Thickness(20.0);
            requiredSkillsTextBoxes.Last().HorizontalAlignment = HorizontalAlignment.Left ;
            requiredSkills[0].Children.Add(requiredSkillsTextBoxes[0]);

            requiredSkillsLevels.Add(new ComboBox() {Height = 30.0 , Margin = new Thickness(20.0), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center  });
            ComboBoxItem FirstLevel = new ComboBoxItem { Content = "Grundkenntnisse" , IsSelected = true };
            ComboBoxItem SecondLevel = new ComboBoxItem { Content = "Fortgeschrittene Kenntnisse" };
            ComboBoxItem ThirdLevel = new ComboBoxItem { Content = "Bereits in Projekt eingesetzt" };
            ComboBoxItem FourthLevel = new ComboBoxItem { Content = "Umfangreiche Projekterfahrungen" };
            requiredSkillsLevels[0].Items.Add(FirstLevel);
            requiredSkillsLevels[0].Items.Add(SecondLevel);
            requiredSkillsLevels[0].Items.Add(ThirdLevel);
            requiredSkillsLevels[0].Items.Add(FourthLevel);
            requiredSkills[0].Children.Add(requiredSkillsLevels[0]);

            lvwInput.Items.Add(requiredSkills[0]);

            numOfSkills = 1;

        }

        

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
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Adds a new skill entry into the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSkillChange_Click(object sender, RoutedEventArgs e)
        {
            if(numOfSkills >= 5)
            {
                btnAdd.Visibility = Visibility.Hidden;
            }
            requiredSkills.Add(new StackPanel { Orientation = Orientation.Horizontal });

            requiredSkillsTextBoxes.Add(DatabaseConnections.Instance.SkillSuggestions());
            requiredSkillsTextBoxes.Last().MinWidth = 260.0;
            requiredSkillsTextBoxes.Last().Margin = new Thickness(20.0);
            requiredSkillsTextBoxes.Last().HorizontalAlignment = HorizontalAlignment.Left;
            requiredSkills[numOfSkills].Children.Add(requiredSkillsTextBoxes[numOfSkills]);


            requiredSkillsLevels.Add(new ComboBox() { Height = 30.0, Margin = new Thickness(20.0), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center });
            ComboBoxItem FirstLevel = new ComboBoxItem { Content = "Grundkenntnisse", IsSelected = true};
            ComboBoxItem SecondLevel = new ComboBoxItem { Content = "Fortgeschrittene Kenntnisse" };
            ComboBoxItem ThirdLevel = new ComboBoxItem { Content = "Bereits in Projekt eingesetzt" };
            ComboBoxItem FourthLevel = new ComboBoxItem { Content = "Umfangreiche Projekterfahrungen" };
            requiredSkillsLevels[numOfSkills].Items.Add(FirstLevel);
            requiredSkillsLevels[numOfSkills].Items.Add(SecondLevel);
            requiredSkillsLevels[numOfSkills].Items.Add(ThirdLevel);
            requiredSkillsLevels[numOfSkills].Items.Add(FourthLevel);
            requiredSkills[numOfSkills].Children.Add(requiredSkillsLevels[numOfSkills]);

            lvwInput.Items.Add(requiredSkills[numOfSkills]);

            numOfSkills++;
        }
        /// <summary>
        /// Searches for employees that have all of the entered skills in the database. If some of the skill text boxes are empty, shows an error message and stops.
        /// If the employees having all of the skills are found, shows them in the lower list view with all the skills they have with the skills that were entered highlihted
        /// in bold. If the employees having all of the skills are not found, the corresponging message is shown, the last entered skill is discarded, and the operation continues
        /// again. If all of the skill have been discarded and no employee has been found, the corresponding message is shown. After the search tzhe search button becomes hidden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> s = new List<string>();
            List<int> l = new List<int>();

            foreach(ComboBox sk in requiredSkillsTextBoxes)
            {
                if (sk.Text == "")
                {
                    MessageBox.Show("Bitte füllen Sie die leeren Felder aus!");
                    return;
                }
                s.Add(sk.Text);
                l.Add(AssignSkillLevel(requiredSkillsLevels[requiredSkillsTextBoxes.IndexOf(sk)]));
            }
            lvwOutput.Items.Clear();
            try
            {
                
                List<int> SearchResult = DatabaseConnections.Instance.SearchEmployeeBySkills(s[0], l[0],s, l);
                if (SearchResult.Count > 0)
                {
                    List<Grid> empGrids = new List<Grid>();

                    
                    
                    foreach(int emp in SearchResult)
                    {
                        empGrids.Add(new Grid());
                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition() { });
                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition() {  });
                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition() {  });
                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition() {  });
                        empGrids[SearchResult.IndexOf(emp)].RowDefinitions.Add(new RowDefinition());

                        Label fn = new Label { Content = DatabaseConnections.Instance.GetFirstNameByID(emp), FontWeight = FontWeights.Bold };
                        Grid.SetRow(fn, 0);
                        Grid.SetColumn(fn, 0);
                        empGrids[SearchResult.IndexOf(emp)].Children.Add(fn);

                        Label ln = new Label { Content = DatabaseConnections.Instance.GetLastNameByID(emp), FontWeight = FontWeights.Bold };
                        Grid.SetRow(ln, 0);
                        Grid.SetColumn(ln, 1);
                        empGrids[SearchResult.IndexOf(emp)].Children.Add(ln);

                        List <Skill> skills = DatabaseConnections.Instance.GetEmployeeSkills(emp);

                        foreach(Skill skill in skills)
                        {
                            empGrids[SearchResult.IndexOf(emp)].RowDefinitions.Add(new RowDefinition());

                            Label minus = new Label { Content = "*" };
                            Grid.SetRow(minus, skills.IndexOf(skill) + 1);
                            Grid.SetColumn(minus, 0);
                            empGrids[SearchResult.IndexOf(emp)].Children.Add(minus);

                            Label sn = new Label { Content = skill.SkillName, FontWeight = s.Contains(skill.SkillName) && l[s.IndexOf(skill.SkillName)] <= DatabaseConnections.Instance.GetSkillLevelByID(DatabaseConnections.Instance.GetSkillIDBySkillNameAndOwnerID(skill.SkillName, emp)) ? FontWeights.Bold  : FontWeights.Normal };
                            Grid.SetRow(sn, skills.IndexOf(skill) + 1);
                            Grid.SetColumn(sn, 2);
                            empGrids[SearchResult.IndexOf(emp)].Children.Add(sn);

                            Label sl = new Label { Content = skill.SkillLevel, FontWeight = s.Contains(skill.SkillName) && l[s.IndexOf(skill.SkillName)] <= DatabaseConnections.Instance.GetSkillLevelByID(DatabaseConnections.Instance.GetSkillIDBySkillNameAndOwnerID(skill.SkillName, emp)) ? FontWeights.Bold : FontWeights.Normal };
                            Grid.SetRow(sl, skills.IndexOf(skill) + 1);
                            Grid.SetColumn(sl, 3);
                            empGrids[SearchResult.IndexOf(emp)].Children.Add(sl);
                        }
                        //lvwOutput.Items.Add(empGrids[SearchResult.IndexOf(emp)]);
                        lvwOutput.Items.Add(new ListBoxItem() { Content = empGrids[SearchResult.IndexOf(emp)] });

                    }

                }
                else
                {
                    if (s.Count == 1)
                    {
                        MessageBox.Show("Leider konnte bei Neox keine Mitarbeiter gefunden werden, die den angegebenen Anforderungen exakt entsprechen.");
                    }
                    else
                    {
                        MessageBox.Show("Leider verfügen bei Neox keine Mitarbeiter über alle erforderlichen Kenntnisse. Es werden jedoch die Mitarbeiter angezeigt, die am besten zu den Anforderungen passen.");
                        for(int i = s.Count; i > 0; i--)
                        {
                            if (s.Count == 1)
                                break;
                            else
                            {
                                s.Remove(s.Last());


                                SearchResult = DatabaseConnections.Instance.SearchEmployeeBySkills(s[0], l[0], s, l);
                                if (SearchResult.Count > 0)
                                {
                                    List<Grid> empGrids = new List<Grid>();



                                    foreach (int emp in SearchResult)
                                    {
                                        empGrids.Add(new Grid());
                                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition());
                                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition());
                                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition());
                                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition());
                                        empGrids[SearchResult.IndexOf(emp)].RowDefinitions.Add(new RowDefinition());

                                        Label fn = new Label { Content = DatabaseConnections.Instance.GetFirstNameByID(emp), FontWeight = FontWeights.Bold };
                                        Grid.SetRow(fn, 0);
                                        Grid.SetColumn(fn, 0);
                                        empGrids[SearchResult.IndexOf(emp)].Children.Add(fn);

                                        Label ln = new Label { Content = DatabaseConnections.Instance.GetLastNameByID(emp), FontWeight = FontWeights.Bold };
                                        Grid.SetRow(ln, 0);
                                        Grid.SetColumn(ln, 1);
                                        empGrids[SearchResult.IndexOf(emp)].Children.Add(ln);

                                        List<Skill> skills = DatabaseConnections.Instance.GetEmployeeSkills(emp);

                                        foreach (Skill skill in skills)
                                        {
                                            empGrids[SearchResult.IndexOf(emp)].RowDefinitions.Add(new RowDefinition());

                                            Label minus = new Label { Content = "*" };
                                            Grid.SetRow(minus, skills.IndexOf(skill) + 1);
                                            Grid.SetColumn(minus, 0);
                                            empGrids[SearchResult.IndexOf(emp)].Children.Add(minus);

                                            Label sn = new Label { Content = skill.SkillName, FontWeight = s.Contains(skill.SkillName, StringComparer.OrdinalIgnoreCase) && l[s.IndexOf(skill.SkillName)] <= DatabaseConnections.Instance.GetSkillLevelByID(DatabaseConnections.Instance.GetSkillIDBySkillNameAndOwnerID(skill.SkillName, emp)) ? FontWeights.Bold : FontWeights.Normal };
                                            Grid.SetRow(sn, skills.IndexOf(skill) + 1);
                                            Grid.SetColumn(sn, 2);
                                            empGrids[SearchResult.IndexOf(emp)].Children.Add(sn);

                                            Label sl = new Label { Content = skill.SkillLevel, FontWeight = s.Contains(skill.SkillName, StringComparer.OrdinalIgnoreCase) && l[s.IndexOf(skill.SkillName)] <= DatabaseConnections.Instance.GetSkillLevelByID(DatabaseConnections.Instance.GetSkillIDBySkillNameAndOwnerID(skill.SkillName, emp)) ? FontWeights.Bold : FontWeights.Normal };
                                            Grid.SetRow(sl, skills.IndexOf(skill) + 1);
                                            Grid.SetColumn(sl, 3);
                                            empGrids[SearchResult.IndexOf(emp)].Children.Add(sl);
                                        }
                                        lvwOutput.Items.Add(empGrids[SearchResult.IndexOf(emp)]);

                                    }

                                }
                            }
                            
                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnAdd2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                requiredSkillsTextBoxes.Remove(requiredSkillsTextBoxes[lvwInput.Items.IndexOf(lvwInput.SelectedItem)]);
                requiredSkillsLevels.Remove(requiredSkillsLevels[lvwInput.Items.IndexOf(lvwInput.SelectedItem)]);
                requiredSkills.Remove(requiredSkills[lvwInput.Items.IndexOf(lvwInput.SelectedItem)]);
                lvwInput.Items.Remove(lvwInput.SelectedItem);
                SaveButton_Click(sender, e);
                numOfSkills--;
            }
            catch(ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("Sie haben nichts ausgewählt?!");
            }
        }
    }
}
