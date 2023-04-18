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
using System.Data;

namespace Skills
{

    public static class DatabaseConnections
    {


        private static string connectionString = "Data Source=LAPTOP-AI5QJL80\\SQLEXPRESS;Initial Catalog=NeoxDatenbank;Integrated Security=True;Connect Timeout=30;Encrypt=False";

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



                SqlCommand command2 = new SqlCommand("Insert into Skills (SkillName,SkillLevel,Employee_Id) values (@Skill,@SkillLevel,(SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN AND birthdate = @birthdate))", connection);

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
                    SqlCommand insertAllAdditionalSkills = new SqlCommand("INSERT INTO skills (skillName, skillLevel, employee_id) VALUES (@NextSkill, @NextSkillLevel, (SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN AND birthdate = @birthdate))", connection);
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
                getEmployeeIDs.CommandText = "SELECT employee_id FROM skills WHERE skillname = '" + firstSkill + "' AND skilllevel >= " + firstSkillLevel;

                foreach (string skill in s)
                {
                    getEmployeeIDs.CommandText += " INTERSECT SELECT employee_id FROM skills WHERE skillname = '" + skill + "' AND skilllevel >= " + l[s.IndexOf(skill)];
                }

                SqlDataReader IDs = getEmployeeIDs.ExecuteReader();
                while (IDs.Read())
                    EmployeeIDs.Add(IDs.GetInt32(0));

            }
            catch (SqlException)
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
        /// <exception cref="ArgumentException">Throws an ArgumentException if the argument does not represent an existing employee_id in the employees table</exception>
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

        public static DateTime? GetDateOfBirthByID(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DateTime? DOB;

            try
            {
                connection.Open();
                SqlCommand getDateOfBirth = new SqlCommand("SELECT birthdate FROM employees WHERE employee_id = @EID", connection);
                getDateOfBirth.Parameters.AddWithValue("@EID", id);
                SqlDataReader dateOfBirth = getDateOfBirth.ExecuteReader();
                DOB = null;
                while (dateOfBirth.Read())
                    DOB = dateOfBirth.GetDateTime(0);
                if (DOB == null)
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

            return DOB;
        }


        /// <summary>
        /// Returns the id of an employee that has the specifies first name, last name and the date of birth
        /// </summary>
        /// <param name="fn">First name of the employee</param>
        /// <param name="ln">Last name of the employee</param>
        /// <param name="bd">Date of birth of the employee</param>
        /// <returns>Int representing an ID in the employees table (Primary key)</returns>
        /// <exception cref="ArgumentException">Throws an argument exception if an employee with the specified first name, last name and date of birth does not exist</exception>
        public static int GetIDByFirstNameLastNameAndDateOfBirth(string fn, string ln, SqlDateTime bd)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            int id = 0;

            try
            {
                connection.Open();
                SqlCommand getID = new SqlCommand("SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN AND birthdate = CONVERT(DATE, @BD)", connection);
                getID.Parameters.AddWithValue("@FN", fn);
                getID.Parameters.AddWithValue("@LN", ln);
                getID.Parameters.AddWithValue("@BD", bd);
                SqlDataReader empID = getID.ExecuteReader();
                id = 0;
                while (empID.Read())
                    id = empID.GetInt32(0);
                if (id == 0)
                    throw new ArgumentException("Mitarbeiter existiert nicht!");
            }
            catch(SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return id;
        }
        /// <summary>
        /// Updates the first name of the employee with the given ID in the database in the employees table
        /// </summary>
        /// <param name="id">A valid employee_id from the employees table</param>
        /// <param name="newFN">A new desired first name</param>
        /// <exception cref="ArgumentException">Throws an argument exception if an employee with the given id does not exist in the employees table of the database</exception>
        public static void SetFirstNameByID(int id, string newFN)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM employees WHERE employee_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", id);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if(c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");
                SqlCommand setFirstName = new SqlCommand("UPDATE employees SET firstname = @FN WHERE employee_id = @ID", connection);
                setFirstName.Parameters.AddWithValue("@FN", newFN);
                setFirstName.Parameters.AddWithValue("@ID", id);
                c.Close();
                setFirstName.ExecuteNonQuery();
                
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close ();
            }
        }

      

        /// <summary>
        /// Updates the last name of the employee with the given ID in the database in the employees table
        /// </summary>
        /// <param name="id">A valid employee_id from the employees table</param>
        /// <param name="newLN">A new desired last name</param>
        /// <exception cref="ArgumentException">Throws an argument exception if an employee with the given id does not exist in the employees table of the database</exception>
        public static void SetLastNameByID(int id, string newLN)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM employees WHERE employee_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", id);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");
                SqlCommand setLastName = new SqlCommand("UPDATE employees SET lastname = @LN WHERE employee_id = @ID", connection);
                setLastName.Parameters.AddWithValue("@LN", newLN);
                setLastName.Parameters.AddWithValue("@ID", id);
                c.Close();
                setLastName.ExecuteNonQuery();
             
            }
            catch (ArgumentException)
            {
                //throw;
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Updates the date of birth of the employee with the given ID in the database in the employees table
        /// </summary>
        /// <param name="id">A valid employee_id from the employees table</param>
        /// <param name="newBD">A new desired date of birth</param>
        /// <exception cref="ArgumentException">Throws an argument exception if an employee with the given id does not exist in the employees table of the database</exception>
        public static void SetBirthDateByID(int id, SqlDateTime newBD)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM employees WHERE employee_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", id);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");
                SqlCommand setDateOfBirth = new SqlCommand("UPDATE employees SET birthdate = CONVERT(DATE,@BD) WHERE employee_id = @ID", connection);
                setDateOfBirth.Parameters.AddWithValue("@BD", newBD);
                setDateOfBirth.Parameters.AddWithValue("@ID", id);
                c.Close();
                setDateOfBirth.ExecuteNonQuery();
              
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

        /// <summary>
        /// Modifies a skill with a known ID. Sets a new name and a new level for the skill
        /// </summary>
        /// <param name="skillID">A valid skill_id from the table skills from the database</param>
        /// <param name="newSkillName">A new desired name for the skill</param>
        /// <param name="newLevel">A new desired level for the skill (in the digital representation). Must be within range [1;4]</param>
        /// <exception cref="ArgumentException">Throws an argument exception if a skill with the given id does not exist in the skills table of the database</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throws an ArgumentOutOfRangeException if newLevel is outside the permitted range [1;4]</exception>
        public static void ModifySkill(int skillID, string newSkillName, int newLevel)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            if (newLevel < 1 || newLevel > 4)
                throw new ArgumentOutOfRangeException("Not a valid skill level!");

            try
            {
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM skills WHERE skill_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", skillID);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");


                SqlCommand updateSkill = new SqlCommand("UPDATE skills SET skillname = @SN, skilllevel = @SL WHERE skill_id = @ID", connection);
                updateSkill.Parameters.AddWithValue("@SN", newSkillName);
                updateSkill.Parameters.AddWithValue("@SL", newLevel);
                updateSkill.Parameters.AddWithValue("@ID", skillID);
                c.Close();
                updateSkill.ExecuteNonQuery();
            }
            catch (SqlException) 
            { 
                throw; 
            }
            finally
            {
                connection.Close() ;
            }
        }
        /// <summary>
        /// Adds a new skill that an existing employee with a valid id owner possesses
        /// </summary>
        /// <param name="skillName">Name of the new skill</param>
        /// <param name="skillLevel">Level of the new skill in the digital form. Must be in the range [1;4]</param>
        /// <param name="owner">A valid employee_id from the table employees from the database</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws an ArgumentOutOfRangeException if newLevel is outside the permitted range [1;4]</exception>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the owner does not represent a valid existing employee_id in the employees table</exception>
        public static void AddSkill(string skillName, int skillLevel, int owner)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            if (skillLevel < 1 || skillLevel > 4)
                throw new ArgumentOutOfRangeException("Not a valid skill level!");

            try
            {
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM employees WHERE employee_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", owner);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Owner is not a valid employee_id!");

                SqlCommand addSkill = new SqlCommand("INSERT INTO skills (skillname, skilllevel, employee_id) values (@SN, @SL, @EID)", connection) ;
                addSkill.Parameters.AddWithValue("@SN", skillName);
                addSkill.Parameters.AddWithValue("@SL", skillLevel);
                addSkill.Parameters.AddWithValue("@EID", owner);
                c.Close();
                addSkill.ExecuteNonQuery();
            }
            catch(SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

        }
        /// <summary>
        /// Deletes a skill with a known ID
        /// </summary>
        /// <param name="skillID">A valid skill ID from the skills table</param>
        /// <exception cref="ArgumentException">Throws an argument exception if a skill with the given id does not exist in the skills table of the database</exception>
        public static void DeleteSkill(int skillID)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM skills WHERE skill_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", skillID);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");

                SqlCommand deleteSkill = new SqlCommand("DELETE FROM skills WHERE skill_id = @ID", connection);
                deleteSkill.Parameters.AddWithValue("@ID", skillID);
                c.Close();
                deleteSkill.ExecuteNonQuery();
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
        /// <summary>
        /// Returns a list os skill_ids of the skills, possessed by an employee with the specified id
        /// </summary>
        /// <param name="id">A valid employee_id from the table employees from the database</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws an argument exception if an employee with the given id does not exist in the employees table of the database</exception>
        public static List<int> GetSkillsOfAnEmployee(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            List<int> skills = new List<int>();

            try
            {
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM employees WHERE employee_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", id);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");
                SqlCommand getSkills = new SqlCommand("SELECT skill_id FROM skills WHERE employee_id = @EID", connection);
                getSkills.Parameters.AddWithValue("@EID", id);
                c.Close();
                SqlDataReader s = getSkills.ExecuteReader();
                while(s.Read())
                    skills.Add(s.GetInt32(0));
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return skills;
        }

        /// <summary>
        /// Returns a name of the skill that correesponds to the skillID
        /// </summary>
        /// <param name="skillID">A valid skill_id from the skills table from the database</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws an argument exception if a skill with the given id does not exist in the skills table of the database</exception>
        public static string GetSkillByID(int skillID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sn = "";
            try
            {
                
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM skills WHERE skill_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", skillID);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");
                SqlCommand getSkillName = new SqlCommand("SELECT skillname FROM skills WHERE skill_id = @ID", connection);
                getSkillName.Parameters.AddWithValue("@ID", skillID);
                c.Close();
                SqlDataReader skillName = getSkillName.ExecuteReader();
                while(skillName.Read())
                    sn = skillName.GetString(0);
                
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return sn;
        }
        /// <summary>
        /// Returns a skill level of the skill with the skill ID in the digital format
        /// </summary>
        /// <param name="skillID">A valid skill_id from the skills table from the database</param>
        /// <returns>An int between 1 and 4</returns>
        /// <exception cref="ArgumentException">Throws an argument exception if a skill with the given id does not exist in the skills table of the database</exception>
        public static int GetSkillLevelByID(int skillID)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            int level = 0;
            try
            {

                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM skills WHERE skill_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", skillID);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");
                SqlCommand getLevel = new SqlCommand("SELECT skilllevel FROM skills WHERE skill_id = @ID", connection);
                getLevel.Parameters.AddWithValue("@ID", skillID);
                c.Close();
                SqlDataReader l = getLevel.ExecuteReader();
                while(l.Read())
                    level = l.GetInt32(0);

            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return level;
        }
        /// <summary>
        /// Returns a skill id from the table skills by its name and the id of the employee who possesses it
        /// </summary>
        /// <param name="skillName">Name of the skill</param>
        /// <param name="owner">Valid employee_id from the table employees from the database</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the owner does not represent a valid existing employee_id in the employees table</exception>
        public static int GetSkillIDBySkillNameAndOwnerID(string skillName, int owner)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            int skillID = 0;
            try
            {

                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM employees WHERE employee_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", owner);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Owner is not a valid employee_id!");
                SqlCommand getSkillID = new SqlCommand("SELECT skill_id FROM skills WHERE skillname LIKE '%"+skillName+"%' AND employee_id = @ID", connection);
                
                getSkillID.Parameters.AddWithValue("@ID", owner);
                c.Close();
                SqlDataReader id = getSkillID.ExecuteReader();
                while(id.Read())
                    skillID = id.GetInt32(0);
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
         
            return skillID;
        }

        /// <summary>
        /// get all details from skill table and save it in skills list
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>skills list</returns>
        public static List<Skill> GetEmployeeSkills(int employeeId)
        {
            List<Skill> skills = new List<Skill>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM skills WHERE employee_id = @employee_Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@employee_Id", employeeId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Skill skill = new Skill();
                        skill.Skill_Id = (int)reader["skill_id"];
                        skill.SkillName = (string)reader["skillname"];


                        skill.SkillLevel = Level_DigitToString((int)reader["skilllevel"]);
                        //skill.SkillLevelString = GetConvertedSkillLevelIntoString((int)reader["skilllevel"]);
                        skill.Employee_Id = employeeId;

                        skills.Add(skill);
                    }
                }
            }

            return skills;
        }

        public static string GetConvertedSkillLevelIntoString(int skillLevel)
        {

            string level;
            switch (skillLevel)
            {
                case 1:
                    level = "Grundkenntnisse";
                    break;

                case 2:
                    level = "Fortgeschrittene Kenntnisse";
                    break;

                case 3:
                    level = "Bereits in Projekt eingesetzt";
                    break;


                case 4:
                    level = "Umfangreiche Projekterfahrungen";
                    break;


                default: throw new ArgumentException("Not a skilllevel!");
            }
            return level;
        }


        public static bool SkillExists(string skill, int empID)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            bool exists;

            try
            {
                connection.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM employees WHERE employee_id = @ID", connection);
                check.Parameters.AddWithValue("@ID", empID);
                SqlDataReader c = check.ExecuteReader();
                while (c.Read())
                    if (c.GetInt32(0) == 0)
                        throw new ArgumentException("Not a valid ID!");
                c.Close();


                SqlCommand skillExists = new SqlCommand("SELECT COUNT(1) FROM skills WHERE employee_id = @ID", connection);
                skillExists.Parameters.AddWithValue("@ID", empID);
                SqlDataReader skillDuplicates = skillExists.ExecuteReader();
                int numberOfDuplicates = -1;
                while (skillDuplicates.Read())
                    numberOfDuplicates = skillDuplicates.GetInt32(0);
                if (numberOfDuplicates == -1)
                    throw new Exception("Error in the SQL-query");
                exists = numberOfDuplicates > 0;

            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return exists;
        }



        public static Employee GetEmployeeByFirstNameLastNameAndDateOfBirth(string firstName, string lastName, DateTime birthdate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Employees WHERE FirstName=@firstName AND LastName=@lastName AND BirthDate=@birthdate", connection);
                command.Parameters.AddWithValue("@firstName", firstName);
                command.Parameters.AddWithValue("@lastName", lastName);
                command.Parameters.AddWithValue("@birthdate", birthdate);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Employee employee = new Employee();
                    employee.Employee_Id = reader.GetInt32(0);
                    employee.FirstName = reader.GetString(1);
                    employee.LastName = reader.GetString(2);
                    employee.BirthDate = reader.GetDateTime(3);
                    return employee;
                }
                else
                {
                    throw new Exception("Employee not found");
                }
            }
        }

        public static Employee GetEmployeeById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Employees WHERE Employee_Id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Employee employee = new Employee()
                    {
                        Employee_Id = (int)reader["Employee_Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        BirthDate = (DateTime)reader["BirthDate"]
                    };
                    return employee;
                }
                else
                {
                    throw new Exception("Employee not found");
                }
            }
        }

        public static string Level_DigitToString(int l)
        {
            switch (l)
            {
                case 1: return "Grundkenntnisse";
                case 2: return "Fortgeschrittene Kenntnisse";
                case 3: return "Bereits in Projekt eingesetzt";
                case 4: return "Umfangreiche Projekterfahrungen";
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Update Firstname, Lastname, Birthdate of the employee in the database
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="dateOfBirth"></param>
        public static void UpdateEmployee(int employeeId, string firstName, string lastName, DateTime dateOfBirth)
        {
            //Vorname, Nachname und Geburtsdatum akualisieren
            string sql = "UPDATE employees SET firstname = @FirstName, lastname = @LastName, birthdate = @Birthdate WHERE employee_id = @EmployeeId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Birthdate", dateOfBirth);
                command.Parameters.AddWithValue("@EmployeeId", employeeId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Die Daten wurden erfolgreich aktualisiert!");
                    }
                    else
                    {
                        MessageBox.Show("Mitarbeiter wurde nicht gefunden.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating employee: " + ex.Message);
                }
            }
        }
        /// <summary>
        /// delete the employee from the database
        /// </summary>
        /// <param name="employeeId"></param>
        public static void DeleteEmployee(int employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Alle Skills des Mitarbeiters löschen, da Beziehung gelöscht werden muss
                    using (SqlCommand command = new SqlCommand("DELETE FROM Skills WHERE Employee_Id = @employeeId", connection))
                    {
                        command.Parameters.AddWithValue("@employeeId", employeeId);
                        command.ExecuteNonQuery();
                    }

                    // Mitarbeiter löschen
                    using (SqlCommand command = new SqlCommand("DELETE FROM Employees WHERE Employee_Id = @employeeId", connection))
                    {
                        command.Parameters.AddWithValue("@employeeId", employeeId);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Mitarbeiter wurde erfolgreich gelöscht.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }





    }
}