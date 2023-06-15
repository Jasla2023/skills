using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
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
using System.Data.Common;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net.Http;
using Flurl;
using System.Transactions;



namespace Skills
{

    public class DatabaseConnections
    {
        public DatabaseConnections() 
        {
            Authenticate();
        }

        private static DatabaseConnections _instance;

        public static DatabaseConnections Instance { 
            get 
            {
                if (_instance == null)
                {
                    _instance = new DatabaseConnections();
                }
                return _instance;
            }
        }

        public string accessToken;

        public void Authenticate()
        {
            
            string instance = "https://login.microsoftonline.com";
            string tenant = "jochenscammellneox.onmicrosoft.com";
            string clientIdOrAppIdUri = "https://azuresqlaccesstestapp.jochenscammellneox.onmicrosoft.com"; //"6e08d520-cdd3-428e-9ff2-e769f6c4b744"
            string scope = "https://database.windows.net/";
            StoreLocation storeLocation = StoreLocation.CurrentUser; //StoreLocation.LocalMachine
            string thumbprint = "bd26eaad2179b5bd0972c5fbeb79b8d68a42275e"; //AzureSQLAccessTestApp Certificate Thumbprint
            try
            {
                var store = new X509Store(storeLocation);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                var x509cert = store.Certificates.OfType<X509Certificate2>().FirstOrDefault(c => c.Thumbprint.Equals(thumbprint, StringComparison.InvariantCultureIgnoreCase));
                if (x509cert != null)
                {
                    var clientAssertion = BuildClientAssertion(x509cert, instance, tenant, clientIdOrAppIdUri);
                    accessToken =  RequestAccessTokenStringAsync(instance, tenant, scope, clientIdOrAppIdUri, clientAssertion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static string BuildClientAssertion(X509Certificate2 x509cert, string instance, string tenant, string clientIdOrAppIdUri)
        {
            var claims = new Dictionary<string, object>();
            claims["aud"] = instance.AppendPathSegments(tenant, "v2.0").ToString();
            claims["iss"] = clientIdOrAppIdUri;
            claims["sub"] = clientIdOrAppIdUri;
            claims["jti"] = Guid.NewGuid().ToString("D");

            var signingCredentials = new Microsoft.IdentityModel.Tokens.X509SigningCredentials(x509cert);
            var securityTokenDescriptor =   new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                
                Claims = claims,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JsonWebTokenHandler();
            var clientAssertion = tokenHandler.CreateToken(securityTokenDescriptor);
            return clientAssertion;
        }

        private static string RequestAccessTokenStringAsync(string instance, string tenant, string scope, string clientIdOrAppIdUri, string clientAssertion)
        {
            var uri = instance.AppendPathSegments(tenant, "/oauth2/v2.0/token");
            var entries = new Dictionary<string, string>();
            entries.Add("scope", scope.AppendPathSegment(".default").ToString());
            entries.Add("client_id", clientIdOrAppIdUri);
            entries.Add("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
            entries.Add("client_assertion", clientAssertion);
            entries.Add("grant_type", "client_credentials");
            FormUrlEncodedContent content = new FormUrlEncodedContent(entries);
            var httpClient = new HttpClient();
            var response = httpClient.PostAsync(uri, content).Result;
            response.EnsureSuccessStatusCode();
            
            string jsonString =  response.Content.ReadAsStringAsync().Result;
            JsonNode jsonNode = JsonNode.Parse(jsonString);
            JsonNode token = jsonNode["access_token"];
            return token.GetValue<string>();
        }

        public static string connectionString = "Server=tcp:neox-expenses-sql-server.database.windows.net,1433;Initial Catalog=neox.expensesDB;Persist Security Info=False;";

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
        public void SaveEmployeeIntoDatabase(string firstName, string lastName, SqlDateTime bd,  List<string> s, List<int> l)
        {
            //Connection with Sql using ConnectionString
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
            //Open the Sql Connection
            try
            {
                int row = 0;
                using (TransactionScope ts = new TransactionScope())
                {
                    connection.Open();

                //Sql Insert Command
                SqlCommand command = new SqlCommand("Insert into Employees (FirstName,LastName, birthdate) values (@FirstName,@LastName, CONVERT (DATE, @birthdate))", connection);



                //SqlCommand command2 = new SqlCommand("Insert into Skills (SkillName,SkillLevel,Employee_Id) values (@Skill,@SkillLevel,(SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN AND birthdate = @birthdate))", connection);

                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@birthdate", bd);
                command.ExecuteNonQuery();

                //command2.Parameters.AddWithValue("@Skill", firstSkill);
                //command2.Parameters.AddWithValue("@SkillLevel", firstSkillLevel);
                //command2.Parameters.AddWithValue("@FN", firstName);
                //command2.Parameters.AddWithValue("@LN", lastName);
                //command2.Parameters.AddWithValue("@birthdate", bd);
                //command2.ExecuteNonQuery();


                
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

                    ts.Complete();
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
        public bool EmployeeExists(string FN, string LN, SqlDateTime BD)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
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
        public List<int> SearchEmployeeBySkills(string firstSkill, int firstSkillLevel, List<string> s, List<int> l)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
            List<int> EmployeeIDs = new List<int>();
            try
            {
                connection.Open();

                SqlCommand getEmployeeIDs = new SqlCommand();
                getEmployeeIDs.Connection = connection;
                getEmployeeIDs.CommandText = "SELECT employee_id FROM skills WHERE skillname = '" + firstSkill + "' AND skilllevel >= " + firstSkillLevel + "AND visible = 1";

                foreach (string skill in s)
                {
                    if (s.IndexOf(skill) == 0)
                        continue;
                    getEmployeeIDs.CommandText += " INTERSECT SELECT employee_id FROM skills WHERE skillname = '" + skill + "' AND skilllevel >= " + l[s.IndexOf(skill)] + "AND visible = 1";
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
        public string GetFirstNameByID(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
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
        public string GetLastNameByID(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
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
        /// <summary>
        /// Returns the date of birth of the employee with the specifies ID from the table employees in the database
        /// </summary>
        /// <param name="id">A valid existing employee_id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the argument does not represent an existing employee_id in the employees table</exception>
        public DateTime? GetDateOfBirthByID(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
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
        public int GetIDByFirstNameLastNameAndDateOfBirth(string fn, string ln, SqlDateTime bd)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
            int id = 0;

            try
            {
                connection.Open();
                SqlCommand getID = new SqlCommand("SELECT employee_id FROM employees WHERE firstname = @FN AND lastname = @LN AND birthdate = CONVERT(DATE, @BD) AND visible = 1", connection);
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
        public void SetFirstNameByID(int id, string newFN)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;

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
        public void SetLastNameByID(int id, string newLN)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;

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
        public void SetBirthDateByID(int id, SqlDateTime newBD)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;

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
        public void ModifySkill(int skillID, string newSkillName, int newLevel)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;

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
        public void AddSkill(string skillName, int skillLevel, int owner)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
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
        public void DeleteSkill(int skillID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;

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
        public List<int> GetSkillsOfAnEmployee(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
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
        public string GetSkillByID(int skillID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
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
        public int GetSkillLevelByID(int skillID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;

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
        public int GetSkillIDBySkillNameAndOwnerID(string skillName, int owner)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;

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
        public List<Skill> GetEmployeeSkills(int employeeId)
        {
            List<Skill> skills = new List<Skill>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.AccessToken = accessToken;
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
        /// <summary>
        /// Converts digit-based skill level into inst description
        /// </summary>
        /// <param name="skillLevel">An integer in the range [1;4]</param>
        /// <returns>A string, representing the skill level description</returns>
        /// <exception cref="ArgumentException">Throws an Argument exception if skillLevel is outside of the range [1;4]</exception>
        public string GetConvertedSkillLevelIntoString(int skillLevel)
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

        /// <summary>
        /// Checks if the given skill with the given name and the given owner ID exists in the skills table
        /// </summary>
        /// <param name="skill">The name of the skill</param>
        /// <param name="empID">A valid employee_id from the skills table</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws an argument exception if empID is not a valid employee_id from the skills table</exception>
        /// <exception cref="Exception">For internal use only</exception>
        public bool SkillExists(string skill, int empID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;
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


                SqlCommand skillExists = new SqlCommand("SELECT COUNT(1) FROM skills WHERE employee_id = @ID AND skillname = @SN", connection);
                skillExists.Parameters.AddWithValue("@ID", empID);
                skillExists.Parameters.AddWithValue("@SN", skill);
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


        /// <summary>
        /// Returns an employee with the given first name, last name and date of birth
        /// </summary>
        /// <param name="firstName">First name of the employee</param>
        /// <param name="lastName">Last name of the employee</param>
        /// <param name="birthdate">Date of birth of the employe</param>
        /// <returns>An employee object based on the data in the employees table</returns>
        /// <exception cref="Exception">Throws an exception if the employee with the specified first name, last name and date of birth does not exist in the employees table of the database</exception>
        public Employee GetEmployeeByFirstNameLastNameAndDateOfBirth(string firstName, string lastName, DateTime birthdate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.AccessToken = accessToken;
                SqlCommand command = new SqlCommand("SELECT * FROM Employees WHERE FirstName=@firstName AND LastName=@lastName AND BirthDate=@birthdate AND visible = 1", connection);
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
        /// <summary>
        /// Returns employee with the given id
        /// </summary>
        /// <param name="id">A valid existing employee_id from the employees table</param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws an exception if the specified id does not represent a valid existing employee_id in the employees table of the database</exception>
        public Employee GetEmployeeById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.AccessToken = accessToken;
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
        /// <summary>
        /// Converts a digit-based level to its description
        /// </summary>
        /// <param name="l">A digit within the range [1;4] representing the skill level</param>
        /// <returns>A skill description of the corresponding skill level</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thtows an ArgumentOutOfRangeException if l is not within the range [1;4]</exception>
        public string Level_DigitToString(int l)
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
        public void UpdateEmployee(int employeeId, string firstName, string lastName, DateTime dateOfBirth)
        {
            //Vorname, Nachname und Geburtsdatum akualisieren
            string sql = "UPDATE employees SET firstname = @FirstName, lastname = @LastName, birthdate = @Birthdate WHERE employee_id = @EmployeeId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.AccessToken = accessToken;
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
                        //MessageBox.Show("Die Daten wurden erfolgreich aktualisiert!");
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
        public void DeleteEmployee(int employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.AccessToken = accessToken;
                    connection.Open();

                    // Alle Skills des Mitarbeiters löschen, da Beziehung gelöscht werden muss
                    //using (SqlCommand command = new SqlCommand("DELETE FROM Skills WHERE Employee_Id = @employeeId", connection))
                    //{
                    //    command.Parameters.AddWithValue("@employeeId", employeeId);
                    //    command.ExecuteNonQuery();
                    //}

                    // Mitarbeiter löschen
                    using (SqlCommand command = new SqlCommand("UPDATE Employees SET visible = 0 WHERE Employee_Id = @employeeId", connection))
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



        public ComboBox SkillSuggestions()
        {
            List<string> skills = new List<string>();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.AccessToken = accessToken;

            try
            {
                connection.Open();

                SqlCommand skillHistory = new SqlCommand("SELECT DISTINCT skillname FROM skills", connection);
                SqlDataReader history = skillHistory.ExecuteReader();
                
                while (history.Read())
                {
                    skills.Add(history.GetString(0));
                }
            }
            catch(SqlException)
            {
                throw;
            }
            finally { connection.Close(); }

            ComboBox result = new ComboBox { IsEditable = true};

            List<ComboBoxItem> s = new List<ComboBoxItem>();
            
            foreach(string skill in skills)
            {
                s.Add(new ComboBoxItem { Content = skill });
                result.Items.Add(s.Last());
            }


            return result;
        }

    }
}