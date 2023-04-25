﻿using Skills.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Skills
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            Views.CreateEmployee cew = new Views.CreateEmployee();
            

            cew.Show();
        }

      
        private void btnSearchEmployeesBySkills_Click(object sender, RoutedEventArgs e)
        {
            RequiredSkillsForm requiredSkillsForm = new RequiredSkillsForm();
            requiredSkillsForm.Show();
        }

    

        //private void btnSearch1_Click(object sender, RoutedEventArgs e)
        //{
        //    SearchEmployee1 searchEmployee2 = new SearchEmployee1();
        //    searchEmployee2.Show();
        //}

        private void btnSearch2_Click(object sender, RoutedEventArgs e)
        {
            SearchEmployee2 se = new SearchEmployee2();
            se.Show();
        }

        private void btnSearch3_Click(object sender, RoutedEventArgs e)
        {
            RequiredSkills re = new RequiredSkills();
            re.Show();
        }
    }
}
