using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Birkesoe_Loebet.Views;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Birkesoe_Loebet.Models;

namespace Birkesoe_Loebet.ViewModels
{
    public class MainViewModel
    {
        public RelayCommand CreateUser { get; set; }
        public RelayCommand RegisterUser { get; set; }

        public SqlConnection connection;
        public MainViewModel()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
            RegisterUser = new RelayCommand(p => OpenRegisterWindow());
            CreateUser = new RelayCommand(p => OpenCreateWindow());
        }

        private void OpenCreateWindow()
        {
            AddRunnerWindow window = new AddRunnerWindow();
            window.ShowDialog();
        }

        private void GetRunners()
        {
            try
            {
                connection.Open();
                string query = "";
                SqlCommand command = new SqlCommand(query, connection);
            }
            catch
            {

            }
        }

        private void OpenRegisterWindow()
        {
            RegisterRunnerWindow window = new RegisterRunnerWindow();
            window.ShowDialog();
        }
    }


    public class RelayCommand : ICommand
    {
        readonly Action<object> execute;
        readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute)
          : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) //Return true hvis delegate unassigned, ellers håndtere event handler 
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)//Invoke delegate
        {
            execute(parameter);
        }
    }
}
