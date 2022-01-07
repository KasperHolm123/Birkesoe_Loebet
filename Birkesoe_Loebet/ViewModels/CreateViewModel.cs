using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Birkesoe_Loebet.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Birkesoe_Loebet.ViewModels
{
    public class CreateViewModel
    {
        //Properties bindet til tekst-felter i UI, her kan vi også evt enforce attributes' domæner. Den vil kun registrere brugeren med et gyldigt tlf-nr f.eks
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public int NumberOfRunners { get; set; }
        Runner model = new Runner();
        
        public RelayCommand CreateUser { get; set; }

        private SqlConnection connection;
        public CreateViewModel()
        {
            CreateUser = new RelayCommand(p => CreateCmd());
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
        }
        private void CreateCmd()
        {
            BuildModel();
            //INSERT INTO query 
            try
            {
                connection.Open();
                string query = "INSERT INTO Runners ([Name], Phone, Email, [Address])\n" + "VALUES(@Name, @Phone, @Email, @Address)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(CreateParameter("@Name", model.Name.Trim(), SqlDbType.NVarChar));
                command.Parameters.Add(CreateParameter("@Phone", model.PhoneNumber.Trim(), SqlDbType.NVarChar));
                command.Parameters.Add(CreateParameter("@Email", model.Email.Trim(), SqlDbType.NVarChar));
                command.Parameters.Add(CreateParameter("@Address", model.RunnerAddress.Trim(), SqlDbType.NVarChar));
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                
            }
            finally
            {
                if(connection != null)
                {
                    connection.Close();
                }
            }
        }
        private void BuildModel() //En 'Runner' model behøves sådan set ikke bruges her, da input allerede gemmes klassens properties
        {
            model.Name = Name;
            model.PhoneNumber = PhoneNumber;
            model.RunnerAddress = Address;
            model.Email = Email;
        }
        private bool ValidateInput() //Validerer at formattet af input stemmer
        {
            if (PhoneNumber.Length == 8 && Name.Length < 40)
            {
                return true;
            }
            else return false;
        }
        private void GetNumberOfRunners() //Angiver længde af Runners table, så vi ved hvad løber_nr vi er nået til.
        {
            string query = "SELECT COUNT(*) FROM Runners";

        }
        private SqlParameter CreateParameter(string paramName, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = paramName,
                Value = value,
                SqlDbType = type
            };
            return param;
        }
    }
}
