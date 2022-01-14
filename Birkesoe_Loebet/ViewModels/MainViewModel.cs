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
    public delegate void WarningMessage(object sender, MessageEventArgs e);

    public class MessageEventArgs
    {
        public string Message { get; private set; }

        public MessageEventArgs(string message)
        {
            Message = message;
        }
    }

    public class MainViewModel
    {
        public event WarningMessage WarningHandler;

        private decimal routeDistance;

        private int numberOfRunners;

        public ObservableCollection<Runner> Runners { get; set; }

        public RelayCommand CreateUser { get; set; }
        public RelayCommand RegisterUser { get; set; }
        public RelayCommand SetCmd { get; set; }
        public RelayCommand SearchCmd { get; set; }

        public SqlConnection connection;

        public MainViewModel()
        {
            Runners = new ObservableCollection<Runner>();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
            RegisterUser = new RelayCommand(p => OpenRegisterWindow());
            CreateUser = new RelayCommand(p => OpenCreateWindow());
            SetCmd = new RelayCommand(p => SetRoute(Convert.ToDecimal(p)));
            SearchCmd = new RelayCommand(p => Search());
        }

        private void OpenCreateWindow()
        {
            AddRunnerWindow window = new AddRunnerWindow(new CancelEventHandler(GetRunner));
            window.ShowDialog();
        }


        private void OpenRegisterWindow()
        {
            RegisterRunnerWindow window = new RegisterRunnerWindow();
            window.ShowDialog();
        }

        private void Search()
        {
            Runners.Clear();
            Runner runner = new Runner();
            try
            {
                connection.Open();
                string query = "SELECT Runners.RunnerID, [Name], Distance, StartTime, EndTime " +
                               "FROM Runners INNER JOIN Registered " +
                               "ON Runners.RunnerID = Registered.RunnerID " +
                               "WHERE Distance = @routeDistance";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(CreateParameter("@routeDistance", routeDistance, SqlDbType.Decimal));
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Runners.Add(new Runner(new PropertyChangedEventHandler(Runner_PropertyChanged),
                                   (string)reader["Name"], new RunningCourse((TimeSpan)reader["StartTime"], (decimal)reader["Distance"]), (int)reader["RunnerID"]));
                    }
                }
            }
            catch (Exception ex)
            {
                OnWarning(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        private void Runner_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Runner runner = new Runner();
            if(sender is Runner)
            {
                runner = (Runner)sender;
            }
            try
            {
                connection.Open();
                string query = "UPDATE Registered\n" + "SET EndTime = @EndTime\n" + "WHERE RunnerID = @RunnerID AND Distance = @Distance";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(CreateParameter("@EndTime", TimeSpan.Parse(runner.EndTime), SqlDbType.Time));
                command.Parameters.Add(CreateParameter("@Distance", runner.Course.CourseDistance, SqlDbType.Decimal));
                command.Parameters.Add(CreateParameter("@RunnerID", runner.RunnerID, SqlDbType.Int));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                OnWarning(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
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

        public void OnWarning(string message)
        {
            if (WarningHandler != null) WarningHandler(this, new MessageEventArgs(message));
        }

        private void SetRoute(decimal dist)
        {
            routeDistance = dist;
        }

        private void GetRunner(object sender, CancelEventArgs e)
        {
            Runners = new ObservableCollection<Runner>();
            GetNumberOfRunners();
            try
            {
                connection.Open();
                string query = "SELECT RunnerID, [Name], Phone, Email, [Address]" + "FROM Runners\n" + "WHERE RunnerID = @RunnerID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(CreateParameter("@RunnerID", numberOfRunners, SqlDbType.Int)); 
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Runners.Add(new Runner(new PropertyChangedEventHandler(Runner_PropertyChanged), (string)reader["Name"], (int)reader["RunnerID"]));
                    }
                }
            }
            catch (Exception ex)
            {
                OnWarning(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                GetNumberOfRunners();
            }
        }

        private void GetNumberOfRunners() //Angiver længde af Runners table, så vi ved hvad løber_nr vi er nået til.
        {
            try
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Runners";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    numberOfRunners = (int)command.ExecuteScalar() + 99;
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
