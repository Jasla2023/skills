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

        private int AssignSkillLevel(ComboBox skillLevel)
        {

            int level = 0;
            switch ((skillLevel.SelectedItem as ComboBoxItem).Content.ToString())
            {
                case "Grundkenntnisse": level = 1;
                    break;
                   
                case "Fortgeschrittene Kenntnisse": level = 2;
                    break;

                case "Bereits in Projekt eingesetzt": level = 3;
                    break;


                case "Umfangreiche Projekterfahrungen": level = 4;
                    break;

            }
            return level;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Employee employee = new Employee();
            employee.FirstName = tbxFirstName.Text;
            employee.LastName = tbxLastName.Text;

            Skill skill = new Skill();
            skill.SkillName = tbxSkill.Text;
            skill.SkillLevel = AssignSkillLevel(cbxLevel);
            

            //Connection with Sql using ConnectionString
            SqlConnection connection = new SqlConnection("Data Source=LAPTOP-AI5QJL80\\SQLEXPRESS;Initial Catalog=NeoxDatenbank;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //Open the Sql Connection
            connection.Open();

            //Sql Insert Command
            SqlCommand command = new SqlCommand("Insert into Employees (FirstName,LastName,Employee_Id) values (@FirstName,@LastName,(Select SCOPE_IDENTITY()))", connection);

            SqlCommand command1 = new SqlCommand("Select Skill, SkillLevel from Skills where Employee_Id = @Employee_Id", connection);
            SqlDataReader reader1;
            reader1 = command1.ExecuteReader();
            if (reader1.Read())
            {
               
            }

            SqlCommand command2 = new SqlCommand("Insert into Skills (Skill,SkillLevel,Employee_Id) values (@Skill,@SkillLevel,@Employee_Id)", connection);

            command.Parameters.AddWithValue("@FirstName", employee.FirstName);
            command.Parameters.AddWithValue("@LastName", employee.LastName);
            command.ExecuteNonQuery();

            command2.Parameters.AddWithValue("@Skill", skill.SkillName);
            command2.Parameters.AddWithValue("@SkillLevel", skill.SkillLevel);
            command2.ExecuteNonQuery();

            //Close Sql Connection
            connection.Close();

            MessageBox.Show("Successfully Saved");


        }
    }
}
