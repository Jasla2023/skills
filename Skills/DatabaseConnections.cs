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
using System.Data.SqlTypes;
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace Skills
{

    public static class DatabaseConnections
    {


        private static string connectionString = "Data Source=LAPTOP-AI5QJL80\\SQLEXPRESS;Initial Catalog=NeoxDatenbank;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        public static void SaveEmployeeIntoDatabase(string firstName, string lastName, SqlDateTime bd, string firstSkill, int firstSkillLevel, List<string> s, List<int> l)
        {
            //Connection with Sql using ConnectionString
            SqlConnection connection = new SqlConnection(connectionString);
            //Open the Sql Connection
            try
            {
                connection.Open();

                //Sql Insert Command
                SqlCommand command = new SqlCommand("Insert into Employees (FirstName,LastName, birthdate) values (@FirstName,@LastName, CONVERT (DATE, @birthdate))", connection);



                SqlCommand command2 = new SqlCommand("Insert into Skills (Skill,SkillLevel,Employee_Id) values (@Skill,@SkillLevel,(SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN AND birthdate = @birthdate))", connection);

                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@birthdate", bd);
                command.ExecuteNonQuery();

                command2.Parameters.AddWithValue("@Skill", firstSkill);
                command2.Parameters.AddWithValue("@SkillLevel", firstSkillLevel);
                command2.Parameters.AddWithValue("@FN", firstName);
                command2.Parameters.AddWithValue("@LN", lastName);
                command2.Parameters.AddWithValue("@birthdate", bd);
                command2.ExecuteNonQuery();


                int row = 0;

                foreach (string skill in s)
                {
                    SqlCommand insertAllAdditionalSkills = new SqlCommand("INSERT INTO skills (skill, skillLevel, employee_id) VALUES (@NextSkill, @NextSkillLevel, (SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN AND birthdate = @birthdate))", connection);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@NextSkill", skill);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@NextSkillLevel", l[row]);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@FN", firstName);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@LN", lastName);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@birthdate", bd);

                    insertAllAdditionalSkills.ExecuteNonQuery();
                    row++;
                }
            }
            catch (SqlException)
            {
                throw;
            }
            //Close Sql Connection
            finally
            { connection.Close(); }



        }

        public static bool EmployeeExists(string FN, string LN, SqlDateTime BD)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand getDuplicates = new SqlCommand("SELECT COUNT(1) \"c\" FROM employees WHERE firstname = @FN AND lastname = @LN AND birthdate = @birthdate",connection);
                getDuplicates.Parameters.AddWithValue("FN", FN);
                getDuplicates.Parameters.AddWithValue("LN", LN);
                getDuplicates.Parameters.AddWithValue("@birthdate", BD);
                SqlDataReader duplicates = getDuplicates.ExecuteReader();
                int numberOfDuplicates = -1;
                while (duplicates.Read())
                    numberOfDuplicates = Convert.ToInt32(duplicates["c"]);

                if (numberOfDuplicates == -1)
                    throw new Exception("Error in the SQL-query");

                return numberOfDuplicates > 0;

                
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close(); 
            }
        }

    }
}