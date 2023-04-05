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
        /// Adds a new record into the employees table in the databese and a record into the skills table in the database for each skill
        /// </summary>
        /// <param name="firstName">First name of the employee</param>
        /// <param name="lastName">Last name of the employee</param>
        /// <param name="bd">Date of birth of the employee</param>
        /// <param name="firstSkill">First skill of the employee</param>
        /// <param name="firstSkillLevel">Level of the first skill of the employee</param>
        /// <param name="s">List of all the additional skills of the employee</param>
        /// <param name="l">List of all the levels corresponding to the additional skills of the employee</param>
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
        /// <summary>
        /// Checks for duplicate EMployees in the database
        /// </summary>
        /// <param name="FN">First name of the employee</param>
        /// <param name="LN">Last name of the employee</param>
        /// <param name="BD">Date of birth of the employee</param>
        /// <returns>True if the employee with the specified first name, last name and date of birth exsists, otherwise false</returns>
        /// <exception cref="Exception">For internal use only</exception>
        public static bool EmployeeExists(string FN, string LN, SqlDateTime BD)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            bool result;

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

                result = numberOfDuplicates > 0;

                
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close(); 
            }

            return result;
        }

        /// <summary>
        /// Returns a list of employee_ids of the employee that possess the all the given skills at the corresponding given levels or better
        /// </summary>
        /// <param name="firstSkill">The name of the first skill</param>
        /// <param name="firstSkillLevel">The numeric representation of its level</param>
        /// <param name="s">The name of all further skills (maximal number, including the firstSkill is 6)</param>
        /// <param name="l">The numeric representation of their levels</param>
        /// <returns></returns>
        public static List<int> SearchEmployeeBySkills(string firstSkill, int firstSkillLevel, List<string> s, List<int> l)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            List<int> EmployeeIDs = new List<int>();
            try
            {
                connection.Open();
                
                SqlCommand getEmployeeIDs = new SqlCommand();
                getEmployeeIDs.Connection = connection;
                getEmployeeIDs.CommandText = "SELECT employee_id FROM skills WHERE skillname LIKE '%" + firstSkill + "%' AND skilllevel >= " + firstSkillLevel;

                foreach(string skill in s)
                {
                    getEmployeeIDs.CommandText += "INTERSECT SELECT employee_id FROM skills WHERE skillname LIKE '%" + skill + "%' AND skillevel >= " + l[s.IndexOf(skill)];
                }

                SqlDataReader IDs = getEmployeeIDs.ExecuteReader();
                while (IDs.Read())
                    EmployeeIDs.Add(IDs.GetInt32(0));

            }
            catch(SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return EmployeeIDs;
        }

        /// <summary>
        /// Returns the first name of the employee with the specifies ID from the table employees in the database
        /// </summary>
        /// <param name="id">A valid existing employee_id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws a new ArgumentException if the argument does not represent an existing employee_id in the employees table</exception>
        public static string GetFirstNameByID(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string FN;

            try
            {
                connection.Open();
                SqlCommand getFirstName = new SqlCommand("SELECT firstname FROM employees WHERE employee_id = @EID", connection);
                getFirstName.Parameters.AddWithValue("@EID", id);
                SqlDataReader firstName = getFirstName.ExecuteReader();
                FN = "";
                while (firstName.Read())
                    FN = firstName.GetString(0);
                if (FN == "")
                    throw new ArgumentException("Not a valid ID!");
            }
            catch (SqlException)
            {
                throw;
            }
            finally 
            { 
                connection.Close(); 
            }

            return FN;
        }

        /// <summary>
        /// Returns the last name of the employee with the specifies ID from the table employees in the database
        /// </summary>
        /// <param name="id">A valid existing employee_id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws a new ArgumentException if the argument does not represent an existing employee_id in the employees table</exception>
        public static string GetLastNameByID(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string LN;

            try
            {
                connection.Open();
                SqlCommand getLastName = new SqlCommand("SELECT lastname FROM employees WHERE employee_id = @EID", connection);
                getLastName.Parameters.AddWithValue("@EID", id);
                SqlDataReader lastName = getLastName.ExecuteReader();
                LN = "";
                while (lastName.Read())
                    LN = lastName.GetString(0);
                if (LN == "")
                    throw new ArgumentException("Not a valid ID!");
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return LN;
        }

    }
}