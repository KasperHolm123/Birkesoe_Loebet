﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Birkesoe_Loebet.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.ComponentModel;

namespace Birkesoe_Loebet.ViewModels
{
    public class CreateViewModel : INotifyPropertyChanged
    {
        public event WarningMessage WarningHandler;
        public event PropertyChangedEventHandler PropertyChanged;

        // Private fields
        private string name = "";
        private string address = "";
        private string phoneNumber = "";
        private string email = "";

        private int numberOfRunners;

        Runner model = new Runner();
        
        public RelayCommand CreateUser { get; set; }

        private SqlConnection connection;

        public CreateViewModel()
        {
            CreateUser = new RelayCommand(p => CreateCmd(), p => CanExecute());
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
        }

        // Properties bindet til tekst-felter i UI, her kan vi også evt enforce attributes' domæner.
        // Den vil kun registrere brugeren med et gyldigt tlf-nr f.eks
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        /// <summary>
        /// Hoved forfatter: Jonas Henckel
        /// </summary>
        private void CreateCmd()
        {
            BuildModel();
            //INSERT INTO query 
            try
            {
                connection.Open();
                string query = "INSERT INTO Runners ([Name], Phone, Email, [Address])\n" + "VALUES(@Name, @Phone, @Email, @Address)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(CreateParameter("@Name", model.Name.Trim(), SqlDbType.NVarChar)); // NullReferenceException
                command.Parameters.Add(CreateParameter("@Phone", model.PhoneNumber.Trim(), SqlDbType.NVarChar));
                command.Parameters.Add(CreateParameter("@Email", model.Email.Trim(), SqlDbType.NVarChar));
                command.Parameters.Add(CreateParameter("@Address", model.RunnerAddress.Trim(), SqlDbType.NVarChar));
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                OnWarning(ex.Message);
            }
            finally
            {
                if(connection != null)
                {
                    connection.Close();
                }
                GetNumberOfRunners();
            }
        }

        private void BuildModel() //En 'Runner' model behøves sådan set ikke bruges her, da input allerede gemmes klassens properties
        {
            model.Name = Name;
            model.PhoneNumber = PhoneNumber;
            model.RunnerAddress = Address;
            model.Email = Email;
        }

        /// <summary>
        /// Hoved forfatter: Jonas Henckel
        /// </summary>
        /// <returns></returns>
        private bool ValidateInput() //Validerer at formattet af input stemmer
        {
            if (PhoneNumber.Length == 8 && Name.Length < 40)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Hoved forfatter: Emil Thomsen
        /// </summary>
        private void GetNumberOfRunners() //Angiver længde af Runners table, så vi ved hvad løber_nr vi er nået til.
        {
            try
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Runners";
                
                using(SqlCommand command = new SqlCommand(query, connection))
                {
                    NumberOfRunners = (int)command.ExecuteScalar() + 99;
                }
            }
            catch
            {
                
            }
            finally
            {
                connection.Close();
            }

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
        public int NumberOfRunners
        {
            get { return numberOfRunners; }
            set
            {
                numberOfRunners = value;
                OnPropertyChanged("NumberOfRunners");
            }
        }

        /// <summary>
        /// Hoved forfatter: Emil Thomsen
        /// </summary>
        /// <param name="property"></param>
        private void OnPropertyChanged(string property)
        {
            if(this.PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
        public void OnWarning(string message)
        {
            if (WarningHandler != null) WarningHandler(this, new MessageEventArgs(message));
        }

        /// <summary>
        /// Hoved forfatter: Kasper Holm
        /// </summary>
        /// <returns></returns>
        public bool CanExecute()
        {
            if (Name == "" || PhoneNumber == "" || Address == "" || Email == "")
            { 
                return false; 
            }
            return true;
        }
    }
}
