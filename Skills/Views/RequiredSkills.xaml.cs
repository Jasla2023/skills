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
        private List<TextBox> requiredSkillsTextBoxes;
        private List<ComboBox> requiredSkillsLevels;
        private int numOfSkills;

        public RequiredSkills()
        {

            InitializeComponent();

            requiredSkills = new List<StackPanel>();
            requiredSkillsTextBoxes = new List<TextBox>();
            requiredSkillsLevels = new List<ComboBox>();

            requiredSkills.Add(new StackPanel { Orientation = Orientation.Horizontal});

            requiredSkillsTextBoxes.Add(new TextBox { MinWidth = 200.0 });
            requiredSkills[0].Children.Add(requiredSkillsTextBoxes[0]);

            requiredSkillsLevels.Add(new ComboBox());
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

       



           



            


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSkillChange_Click(object sender, RoutedEventArgs e)
        {
            if(numOfSkills >= 5)
            {
                btnAdd.Visibility = Visibility.Hidden;
            }
            requiredSkills.Add(new StackPanel { Orientation = Orientation.Horizontal });

            requiredSkillsTextBoxes.Add(new TextBox { MinWidth = 200.0});
            requiredSkills[numOfSkills].Children.Add(requiredSkillsTextBoxes[numOfSkills]);

            requiredSkillsLevels.Add(new ComboBox());
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> s = new List<string>();
            List<int> l = new List<int>();

            foreach(TextBox sk in requiredSkillsTextBoxes)
            {
                s.Add(sk.Text);
                l.Add(AssignSkillLevel(requiredSkillsLevels[requiredSkillsTextBoxes.IndexOf(sk)]));
            }

            try
            {
                List<int> SearchResult = DatabaseConnections.SearchEmployeeBySkills(s[0], l[0],s, l);
                if (SearchResult.Count > 0)
                {
                    List<Grid> empGrids = new List<Grid>();

                    
                    
                    foreach(int emp in SearchResult)
                    {
                        empGrids.Add(new Grid());
                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition());
                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition());
                        empGrids[SearchResult.IndexOf(emp)].ColumnDefinitions.Add(new ColumnDefinition());
                        empGrids[SearchResult.IndexOf(emp)].RowDefinitions.Add(new RowDefinition());

                        Label fn = new Label { Content = DatabaseConnections.GetFirstNameByID(emp) };
                        Grid.SetRow(fn, 0);
                        Grid.SetColumn(fn, 0);
                        empGrids[SearchResult.IndexOf(emp)].Children.Add(fn);

                        Label ln = new Label { Content = DatabaseConnections.GetLastNameByID(emp) };
                        Grid.SetRow(ln, 0);
                        Grid.SetColumn(ln, 1);
                        empGrids[SearchResult.IndexOf(emp)].Children.Add(ln);

                        List <Skill> skills = DatabaseConnections.GetEmployeeSkills(emp);

                        foreach(Skill skill in skills)
                        {
                            empGrids[SearchResult.IndexOf(emp)].RowDefinitions.Add(new RowDefinition());

                            Label minus = new Label { Content = "-" };
                            Grid.SetRow(minus, skills.IndexOf(skill) + 1);
                            Grid.SetColumn(minus, 0);
                            empGrids[SearchResult.IndexOf(emp)].Children.Add(minus);

                            Label sn = new Label { Content = skill.SkillName, FontWeight = s.Contains(skill.SkillName) && l[s.IndexOf(skill.SkillName)] <= DatabaseConnections.GetSkillLevelByID(DatabaseConnections.GetSkillIDBySkillNameAndOwnerID(skill.SkillName, emp)) ? FontWeights.Bold  : FontWeights.Normal };
                            Grid.SetRow(sn, skills.IndexOf(skill) + 1);
                            Grid.SetColumn(sn, 1);
                            empGrids[SearchResult.IndexOf(emp)].Children.Add(sn);

                            Label sl = new Label { Content = skill.SkillLevel, FontWeight = s.Contains(skill.SkillName) && l[s.IndexOf(skill.SkillName)] <= DatabaseConnections.GetSkillLevelByID(DatabaseConnections.GetSkillIDBySkillNameAndOwnerID(skill.SkillName, emp)) ? FontWeights.Bold : FontWeights.Normal };
                            Grid.SetRow(sl, skills.IndexOf(skill) + 1);
                            Grid.SetColumn(sl, 2);
                            empGrids[SearchResult.IndexOf(emp)].Children.Add(sl);
                        }
                        lvwOutput.Items.Add(empGrids[SearchResult.IndexOf(emp)]);
                        
                    }
                    
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
