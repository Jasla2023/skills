﻿using System;
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
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Fortgeschrittene Kenntnisse" });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Bereits in Projekt eingesetzt" });
            addedSkillLevelComboBoxes.Last().Items.Add(new ComboBoxItem { Content = "Umfangreiche Projekterfahrungen" });
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

            int level;
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
                default: throw new ArgumentException() ;

            }
            return level;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            /*Employee employee = new Employee();
            employee.FirstName = tbxFirstName.Text;
            employee.LastName = tbxLastName.Text;

            Skill skill = new Skill();
            skill.SkillName = tbxSkill.Text;
            skill.SkillLevel = AssignSkillLevel(cbxLevel);
            */

            //Connection with Sql using ConnectionString
            SqlConnection connection = new SqlConnection("Data Source=LAPTOP-AI5QJL80\\SQLEXPRESS;Initial Catalog=NeoxDatenbank;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //Open the Sql Connection
            try
            {
                connection.Open();

                //Sql Insert Command
                SqlCommand command = new SqlCommand("Insert into Employees (FirstName,LastName) values (@FirstName,@LastName)", connection);

                /*SqlCommand command1 = new SqlCommand("Select Skill, SkillLevel from Skills where Employee_Id = @Employee_Id", connection);
                SqlDataReader reader1;
                reader1 = command1.ExecuteReader();
                if (reader1.Read())
                {

                }*/

                SqlCommand command2 = new SqlCommand("Insert into Skills (Skill,SkillLevel,Employee_Id) values (@Skill,@SkillLevel,SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN)", connection);

                command.Parameters.AddWithValue("@FirstName", tbxFirstName.Text);
                command.Parameters.AddWithValue("@LastName", tbxLastName.Text);
                command.ExecuteNonQuery();

                command2.Parameters.AddWithValue("@Skill", tbxSkill.Text);
                command2.Parameters.AddWithValue("@SkillLevel", AssignSkillLevel(cbxLevel));
                command2.Parameters.AddWithValue("@FN", tbxFirstName.Text);
                command2.Parameters.AddWithValue("@LN", tbxLastName.Text);
                command2.ExecuteNonQuery();


                int row = 0;

                foreach (TextBox skill in addedSkillTextBoxes)
                {
                    SqlCommand insertAllAdditionalSkills = new SqlCommand("INSERT INTO skills (skill, skilllevel, employee_id) VALUES (@NextSkill, @NextSkillLevel, (SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN))", connection);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@NextSkill", skill.Text);

                    insertAllAdditionalSkills.Parameters.AddWithValue("@NextSkillLevel", AssignSkillLevel(addedSkillLevelComboBoxes[addedSkillTextBoxes.IndexOf(skill)]));
                

                insertAllAdditionalSkills.Parameters.AddWithValue("@FN", tbxFirstName.Text);
                insertAllAdditionalSkills.Parameters.AddWithValue("@LN", tbxLastName.Text);

                insertAllAdditionalSkills.ExecuteNonQuery();

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            //Close Sql Connection
            finally
            { connection.Close(); }

            MessageBox.Show("Erfolgreich gespeichert");



        }
    }
}
