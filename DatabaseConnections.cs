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


namespace Skills
{

	public static class DatabaseConnections
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        public static void SaveEmployeeIntoDatabase(string firstName, string lastName, string firstSkill, int firstSkillLevel, List<string> s, List<int> l)
        {
            //Connection with Sql using ConnectionString
            SqlConnection connection = new SqlConnection("Data Source=LAPTOP-AI5QJL80\\SQLEXPRESS;Initial Catalog=NeoxDatenbank;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //Open the Sql Connection
            try
            {
                connection.Open();

                //Sql Insert Command
                SqlCommand command = new SqlCommand("Insert into Employees (FirstName,LastName) values (@FirstName,@LastName)", connection);

              

                SqlCommand command2 = new SqlCommand("Insert into Skills (Skill,SkillLevel,Employee_Id) values (@Skill,@SkillLevel,(SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN))", connection);

                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.ExecuteNonQuery();

                command2.Parameters.AddWithValue("@Skill", firstSkill);
                command2.Parameters.AddWithValue("@SkillLevel", firstSkillLevel);
                command2.Parameters.AddWithValue("@FN", firstName);
                command2.Parameters.AddWithValue("@LN", lastName);
                command2.ExecuteNonQuery();




                foreach (string skill in s)
                {
                    SqlCommand insertAllAdditionalSkills = new SqlCommand("INSERT INTO skills (skill, skillLevel, employee_id) VALUES (@NextSkill, @NextSkillLevel, (SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN))", connection);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@NextSkill", skill );
                    insertAllAdditionalSkills.Parameters.AddWithValue("@NextSkillLevel", l[s.IndexOf(skill)]);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@FN", firstName);
                    insertAllAdditionalSkills.Parameters.AddWithValue("@LN", lastName);

                    insertAllAdditionalSkills.ExecuteNonQuery();

                }
            }
            catch (SqlException ex)
            {
                throw;
            }
            //Close Sql Connection
            finally
            { connection.Close(); }

            

        }


}