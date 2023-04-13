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
    /// Interaktionslogik für UpdateEmployeeWindow.xaml
    /// </summary>
    public partial class UpdateEmployeeWindow : Window
    {
        public UpdateEmployeeWindow()
        {
            InitializeComponent();


        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
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

        private void btnUpdate_Click(object sender, RoutedEventArgs d)
        {
            if (tbxFirstName.Text == "" || tbxLastName.Text == "" || dpcDateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Alle Felder müssen ausgefüllt sein!");
                return;
            }

            try
            {
                int empID = DatabaseConnections.GetIDByFirstNameLastNameAndDateOfBirth(tbxFirstName.Text, tbxLastName.Text, new System.Data.SqlTypes.SqlDateTime((DateTime)dpcDateOfBirth.SelectedDate)); 
                switch ((cbxChooseUpdate.SelectedItem as ComboBoxItem).Content.ToString())
                {
                    case "Vorname ändern":
                        DatabaseConnections.SetFirstNameByID(empID, tbxUpdateFirstName.Text);
                        MessageBox.Show("Der Vorname wurde erfolgreich geändert!");
                        break;
                    case "Nachname ändern":
                        DatabaseConnections.SetLastNameByID(empID, tbxUpdateLastName.Text);
                        MessageBox.Show("Der Nachname wurde erfolgreich geändert!");
                        break;
                    case "Geburtsdatum ändern":
                        DatabaseConnections.SetBirthDateByID(empID, new System.Data.SqlTypes.SqlDateTime((DateTime)dpcUpdateBirthdate.SelectedDate));
                        MessageBox.Show("Geburtsdatum wurde erfolgreich geändert!");
                        break;
                    case "Kenntnisse/Kenntnisstufe ändern":
                        DatabaseConnections.ModifySkill(DatabaseConnections.GetSkillIDBySkillNameAndOwnerID(tbxActualSkillName.Text, empID), tbxActualSkillNameChange.Text, AssignSkillLevel(cbxUpdateLevel));
                        MessageBox.Show("Kenntnis des Mitarbeiters wurde erfolgreich geändert!");
                        break;
                    case "Kenntnisse/Kenntnisstufe hinzufügen":
                        DatabaseConnections.AddSkill(tbxAddSkillName.Text, AssignSkillLevel(cbxAddLevel), empID);
                        MessageBox.Show("Kenntnis des Mitarbeiters wurde erfolgreich hinzugefügt!");
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            


        }

        //Sobald das Fenster UpdateEmployeeWindow geöffnet wird, werden die Felder initialisiert
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbxUpdateFirstName = FindName("tbxUpdateFirstName") as TextBox;
            tbxUpdateLastName = FindName("tbxUpdateLastName") as TextBox;
            dpcUpdateBirthdate = FindName("dpcUpdateBirthdate") as DatePicker;
            tbxUpdateSkillName = FindName("tbxUpdateSkillName") as TextBox;
            cbxUpdateLevel = FindName("cbxUpdateLevel") as ComboBox;
            cbxAddLevel = FindName("cbxAddLevel") as ComboBox;
            tbxAddSkillName = FindName("tbxAddSkillName") as TextBox;
            tbxActualSkillName = FindName("ActualSkillName") as TextBox;
            tbxActualSkillNameChange = FindName("ActualSkillNameChange") as TextBox;
        }


        private void cbxChooseUpdate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (tbxUpdateFirstName == null || tbxUpdateLastName == null || dpcUpdateBirthdate == null || tbxUpdateSkillName == null || cbxUpdateLevel == null || cbxAddLevel == null || tbxAddSkillName == null)
            {
                return;
            }


            if (cbxChooseUpdate != null)
            {
                ComboBoxItem selectedItem = cbxChooseUpdate.SelectedItem as ComboBoxItem;

                if (selectedItem != null)
                {
                    string selectedValue = selectedItem.Content.ToString();

                    if (selectedValue == "Vorname ändern")
                    {

                        lblUpdateSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateBirthdate.Visibility = Visibility.Hidden;
                        lblUpdateLastName.Visibility = Visibility.Hidden;
                        lblUpdateFirstName.Visibility = Visibility.Visible;
                        lblAddSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblActualSkillName_SkillLevel_Change.Visibility = Visibility.Hidden;
                        lblActualSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateSkill.Visibility = Visibility.Hidden;



                        tbxUpdateFirstName.Visibility = Visibility.Visible;
                        tbxUpdateSkillName.Visibility = Visibility.Hidden;
                        tbxUpdateLastName.Visibility = Visibility.Hidden;
                        dpcUpdateBirthdate.Visibility = Visibility.Hidden;
                        cbxUpdateLevel.Visibility = Visibility.Hidden;
                        cbxAddLevel.Visibility = Visibility.Hidden;
                        tbxAddSkillName.Visibility = Visibility.Hidden;
                        tbxActualSkillName.Visibility = Visibility.Hidden;
                        tbxActualSkillNameChange.Visibility = Visibility.Hidden;
                        cbxActualLevel.Visibility = Visibility.Hidden;
                    }
                    else if (selectedValue == "Nachname ändern")
                    {
                        lblUpdateSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateBirthdate.Visibility = Visibility.Hidden;
                        lblUpdateLastName.Visibility = Visibility.Visible;
                        lblUpdateFirstName.Visibility = Visibility.Hidden;
                        lblAddSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblActualSkillName_SkillLevel_Change.Visibility = Visibility.Hidden;
                        lblActualSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateSkill.Visibility = Visibility.Hidden;

                        tbxUpdateFirstName.Visibility = Visibility.Hidden;
                        tbxUpdateSkillName.Visibility = Visibility.Hidden;
                        tbxUpdateLastName.Visibility = Visibility.Visible;
                        dpcUpdateBirthdate.Visibility = Visibility.Hidden;
                        cbxUpdateLevel.Visibility = Visibility.Hidden;
                        cbxAddLevel.Visibility = Visibility.Hidden;
                        tbxAddSkillName.Visibility = Visibility.Hidden;
                        tbxActualSkillName.Visibility = Visibility.Hidden;
                        tbxActualSkillNameChange.Visibility = Visibility.Hidden;
                        cbxActualLevel.Visibility = Visibility.Hidden;

                    }
                    else if (selectedValue == "Geburtsdatum ändern")
                    {
                        lblUpdateSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateBirthdate.Visibility = Visibility.Visible;
                        lblUpdateLastName.Visibility = Visibility.Hidden;
                        lblUpdateFirstName.Visibility = Visibility.Hidden;
                        lblAddSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblActualSkillName_SkillLevel_Change.Visibility = Visibility.Hidden;
                        lblActualSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateSkill.Visibility = Visibility.Hidden;

                        tbxUpdateFirstName.Visibility = Visibility.Hidden;
                        tbxUpdateSkillName.Visibility = Visibility.Hidden;
                        tbxUpdateLastName.Visibility = Visibility.Hidden;
                        dpcUpdateBirthdate.Visibility = Visibility.Visible;
                        cbxUpdateLevel.Visibility = Visibility.Hidden;
                        cbxAddLevel.Visibility = Visibility.Hidden;
                        tbxAddSkillName.Visibility = Visibility.Hidden;
                        tbxActualSkillName.Visibility = Visibility.Hidden;
                        tbxActualSkillNameChange.Visibility = Visibility.Hidden;
                        cbxActualLevel.Visibility = Visibility.Hidden;
                    }
                    else if (selectedValue == "Kenntnisse/Kenntnisstufe ändern")
                    {
                        lblUpdateSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateBirthdate.Visibility = Visibility.Hidden;
                        lblUpdateLastName.Visibility = Visibility.Hidden;
                        lblUpdateFirstName.Visibility = Visibility.Hidden;
                        lblAddSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblActualSkillName_SkillLevel_Change.Visibility = Visibility.Visible;
                        lblActualSkillName_SkillLevel.Visibility = Visibility.Visible;
                        lblUpdateSkill.Visibility = Visibility.Visible;

                        tbxUpdateFirstName.Visibility = Visibility.Hidden;
                        tbxUpdateSkillName.Visibility = Visibility.Hidden;
                        tbxUpdateLastName.Visibility = Visibility.Hidden;
                        dpcUpdateBirthdate.Visibility = Visibility.Hidden;
                        cbxUpdateLevel.Visibility = Visibility.Visible;
                        cbxAddLevel.Visibility = Visibility.Hidden;
                        tbxAddSkillName.Visibility = Visibility.Hidden;
                        tbxActualSkillName.Visibility = Visibility.Visible;
                        tbxActualSkillNameChange.Visibility = Visibility.Visible;
                        cbxActualLevel.Visibility = Visibility.Visible;

                    }

                    else if (selectedValue == "Kenntnisse/Kenntnisstufe hinzufügen")
                    {
                        lblUpdateSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateBirthdate.Visibility = Visibility.Hidden;
                        lblUpdateLastName.Visibility = Visibility.Hidden;
                        lblUpdateFirstName.Visibility = Visibility.Hidden;
                        lblAddSkillName_SkillLevel.Visibility = Visibility.Visible;
                        lblActualSkillName_SkillLevel_Change.Visibility = Visibility.Hidden;
                        lblActualSkillName_SkillLevel.Visibility = Visibility.Hidden;
                        lblUpdateSkill.Visibility = Visibility.Hidden;

                        tbxUpdateFirstName.Visibility = Visibility.Hidden;
                        tbxUpdateSkillName.Visibility = Visibility.Hidden;
                        tbxUpdateLastName.Visibility = Visibility.Hidden;
                        dpcUpdateBirthdate.Visibility = Visibility.Hidden;
                        cbxUpdateLevel.Visibility = Visibility.Hidden;
                        cbxAddLevel.Visibility = Visibility.Visible;
                        tbxAddSkillName.Visibility = Visibility.Visible;
                        tbxActualSkillName.Visibility = Visibility.Hidden;
                        tbxActualSkillNameChange.Visibility = Visibility.Hidden;
                        cbxActualLevel.Visibility = Visibility.Hidden;

                    }
                }

            }
        }



    }


}