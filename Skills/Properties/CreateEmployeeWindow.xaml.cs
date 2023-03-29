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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private int rowsAdded = 0;

        private List<Label> addedSkillLabels = new List<Label>();
        private List<TextBox> addedSkillTextBoxes = new List<TextBox>();
        private List<Label> addedSkillLevelLabels = new List<Label>();
        private List<ComboBox> addedSkillLevelComboBoxes = new List<ComboBox>();

        private void btnAddMoreSkills_Click(object sender, RoutedEventArgs e)
        {
            CreateEmployee.Height += 30;
            grd2createEmployee.RowDefinitions.Add(new RowDefinition());

            addedSkillLabels.Add(new Label());
            addedSkillLabels.Last().Content = "Kenntniss/Erfahrung: ";
            Grid.SetRow(addedSkillLabels.Last(), 1 + rowsAdded);
            Grid.SetColumn(addedSkillLabels.Last(), 0);
            addedSkillLabels.Last().HorizontalAlignment = HorizontalAlignment.Right;
            addedSkillLabels.Last().VerticalAlignment = VerticalAlignment.Center;

            addedSkillTextBoxes.Add(new TextBox());
            Grid.SetRow(addedSkillTextBoxes.Last(), 1 + rowsAdded);
            Grid.SetColumn(addedSkillTextBoxes.Last(), 1);
            addedSkillTextBoxes.Last().HorizontalAlignment = HorizontalAlignment.Left;
            addedSkillTextBoxes.Last().VerticalAlignment= VerticalAlignment.Center;
            addedSkillTextBoxes.Last().Width = 150;

            addedSkillLevelLabels.Add(new Label());
            addedSkillLevelLabels.Last().Content = "Stufe: ";
            Grid.SetRow(addedSkillLevelLabels.Last(), 1 + rowsAdded);
            Grid.SetColumn(addedSkillLevelLabels.Last(), 2);
            addedSkillLevelLabels.Last().HorizontalAlignment= HorizontalAlignment.Right;
            addedSkillLevelLabels.Last().VerticalAlignment = VerticalAlignment.Center;

            addedSkillLevelComboBoxes.Add(new ComboBox());
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Grundkenntnisse", IsSelected=true});
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "fortgeschrittene Kenntnisse" });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "bereits in Projekt eingesetzt" });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "umfangreiche Projekterfahrungen" });
            Grid.SetRow(addedSkillLevelComboBoxes.Last(), 1 + rowsAdded);
            Grid.SetColumn(addedSkillLevelComboBoxes.Last(), 3);
            addedSkillLevelComboBoxes.Last().HorizontalAlignment = HorizontalAlignment.Left;
            addedSkillLevelComboBoxes.Last().VerticalAlignment= VerticalAlignment.Center;

            Grid.SetRow(btnAddMoreSkills, 2 + rowsAdded);
            
            grd2createEmployee.Children.Add(addedSkillLabels.Last());
            grd2createEmployee.Children.Add(addedSkillTextBoxes.Last());
            grd2createEmployee.Children.Add(addedSkillLevelLabels.Last());
            grd2createEmployee.Children.Add(addedSkillLevelComboBoxes.Last());

            rowsAdded++;

        }
    }
}
