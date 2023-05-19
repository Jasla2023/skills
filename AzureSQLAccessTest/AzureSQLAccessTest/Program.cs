using System.Security.Cryptography.X509Certificates;
using System;
using System.Linq;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;
 
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json.Nodes;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using Neox.KnowHowTransfer.DatabaseContext;
using Neox.KnowHowTransfer.Models;

public class AzureSQLAccessTest
{
    private static async Task Main(string[] args)
    {
        //Token beschaffen
        string accessToken = null;
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
            var x509cert = store.Certificates.FirstOrDefault(c => c.Thumbprint.Equals(thumbprint, StringComparison.InvariantCultureIgnoreCase));
            if (x509cert != null)
            {
                    var clientAssertion = BuildClientAssertion(x509cert, instance, tenant, clientIdOrAppIdUri);
                    accessToken = await RequestAccessTokenStringAsync(instance, tenant, scope, clientIdOrAppIdUri, clientAssertion);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //Datenbankverbindung aufbauen
        string dbConnectionString = "Server=tcp:neox-expenses-sql-server.database.windows.net,1433;Initial Catalog=neox.expensesDB;Connect Timeout=30";
        SqlConnection sqlConnection = new SqlConnection(dbConnectionString);
        sqlConnection.AccessToken = accessToken;
        try
        {
            //Direkter Zugriff mit SqlDataAdapter
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "select * from tb_employees";
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            foreach(DataRow row in dataSet.Tables[0].Rows)
            {
                for(int i=0; i < dataSet.Tables[0].Columns.Count; i++)
                {
                    Console.WriteLine(dataSet.Tables[0].Columns[i].ColumnName + ": " + row[i].ToString());
                }
            }
            sqlConnection.Close();

            //Indirekter Zugriff über Entity Framework
            NeoxDBContext neoxDBContext = NeoxDBContext.CreatDBContext(sqlConnection);
            foreach(Employee employee in neoxDBContext.Employees)
            {
                Console.WriteLine(nameof(employee.FirstName) + ": " + employee.FirstName + ", " + nameof(employee.LastName) + ": "+ employee.LastName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        { 
            sqlConnection.Close();
        }  
    }

    private static string BuildClientAssertion(X509Certificate2 x509cert, string instance, string tenant, string clientIdOrAppIdUri)
    {
        var claims = new Dictionary<string, object>();
        claims["aud"] = instance.AppendPathSegments(tenant, "v2.0").ToString();
        claims["iss"] = clientIdOrAppIdUri;
        claims["sub"] = clientIdOrAppIdUri;
        claims["jti"] = Guid.NewGuid().ToString("D");

        var signingCredentials = new X509SigningCredentials(x509cert);
        var securityTokenDescriptor = new Microsoft.Tokens.SecurityTokenDescriptor
        {
            Claims = claims,
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JsonWebTokenHandler();
        var clientAssertion = tokenHandler.CreateToken(securityTokenDescriptor);
        return clientAssertion;
    }

    private static async Task<string> RequestAccessTokenStringAsync(string instance, string tenant, string scope, string clientIdOrAppIdUri, string clientAssertion)
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
        var response = await httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();

        string jsonString = await response.Content.ReadAsStringAsync();
        JsonNode jsonNode = JsonNode.Parse(jsonString);
        JsonNode token = jsonNode["access_token"];
        return token.GetValue<string>();
    }
}