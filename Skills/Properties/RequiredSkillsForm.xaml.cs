﻿using System;
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

namespace Skills.Properties
{
    /// <summary>
    /// Interaktionslogik für RequiredSkillsForm.xaml
    /// </summary>
    public partial class RequiredSkillsForm : Window
    {
        public RequiredSkillsForm()
        {
            countAddClicks = 0;
            InitializeComponent();
        }

        private int countAddClicks;

        private List<Label> addedSkillLabels = new List<Label>();
        private List<TextBox> addedSkillTextBoxes = new List<TextBox>();
        private List<Label> addedSkillLevelLabels = new List<Label>();
        private List<ComboBox> addedSkillLevelComboBoxes = new List<ComboBox>();
        private int rowsAdded = 0;

        /// <summary>
        /// Adds additional skill and skill level entry into the UI and in the corresponding lists
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
            Grid.SetRow(addedSkillLabels.Last(), 3 + rowsAdded);
            Grid.SetColumn(addedSkillLabels.Last(), 0);
            addedSkillLabels.Last().HorizontalAlignment = HorizontalAlignment.Left;
            addedSkillLabels.Last().VerticalAlignment = VerticalAlignment.Center;

            addedSkillTextBoxes.Add(new TextBox());
            Grid.SetRow(addedSkillTextBoxes.Last(), 3 + rowsAdded);
            Grid.SetColumn(addedSkillTextBoxes.Last(), 1);
            addedSkillTextBoxes.Last().HorizontalAlignment = HorizontalAlignment.Stretch;
            addedSkillTextBoxes.Last().VerticalAlignment = VerticalAlignment.Center;

            addedSkillTextBoxes.Last().Style = new Style();
            addedSkillTextBoxes.Last().Height = 25;

            addedSkillLevelLabels.Add(new Label());
            addedSkillLevelLabels.Last().Content = "Kentnissstufe: ";
            Grid.SetRow(addedSkillLevelLabels.Last(), 4 + rowsAdded);
            Grid.SetColumn(addedSkillLevelLabels.Last(), 0);
            addedSkillLevelLabels.Last().HorizontalAlignment = HorizontalAlignment.Left;
            addedSkillLevelLabels.Last().VerticalAlignment = VerticalAlignment.Center;

            addedSkillLevelComboBoxes.Add(new ComboBox());
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Grundkenntnisse", IsSelected = true });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Fortgeschrittene Kenntnisse" });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Bereits in Projekt eingesetzt" });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Umfangreiche Projekterfahrungen" });
            Grid.SetRow(addedSkillLevelComboBoxes.Last(), 4 + rowsAdded);
            Grid.SetColumn(addedSkillLevelComboBoxes.Last(), 1);
            addedSkillLevelComboBoxes.Last().HorizontalAlignment = HorizontalAlignment.Stretch;
            addedSkillLevelComboBoxes.Last().VerticalAlignment = VerticalAlignment.Center;
            addedSkillLevelComboBoxes.Last().Height = 25;

            Grid.SetRow(btnAddMoreSkills, 5 + rowsAdded);

            grdEntries.Children.Add(addedSkillLabels.Last());
            grdEntries.Children.Add(addedSkillTextBoxes.Last());
            grdEntries.Children.Add(addedSkillLevelLabels.Last());
            grdEntries.Children.Add(addedSkillLevelComboBoxes.Last());

            Grid.SetRow(btnClose, 7 + rowsAdded);
            Grid.SetRow(btnSave, 7 + rowsAdded);

            rowsAdded += 2;

            countAddClicks++;

            if (countAddClicks >= 5)
                grdEntries.Children.Remove(btnAddMoreSkills);

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
        /// <summary>
        /// Closing the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Send the search form to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}