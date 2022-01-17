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

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event WarningMessage WarningHandler;

        private decimal routeDistance;

        public int NumberOfRunners { get; set; }

        private bool IsResultList;

        private System.Windows.Visibility startgridVisibility;
        private System.Windows.Visibility resultgridVisibility;

        public ObservableCollection<Runner> Runners { get; set; }

        public RelayCommand CreateUser { get; set; }
        public RelayCommand RegisterUser { get; set; }
        public RelayCommand SetCmd { get; set; }
        public RelayCommand FindRunner { get; set; }
        public SqlConnection connection;

        public MainViewModel()
        {
            Runners = new ObservableCollection<Runner>();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
            RegisterUser = new RelayCommand(p => OpenRegisterWindow());
            CreateUser = new RelayCommand(p => OpenCreateWindow());
            SetCmd = new RelayCommand(p => SetRoute(Convert.ToDecimal(p)));
            FindRunner = new RelayCommand(p => GetRunner(), p => CanSearch());
            IsResultList = false;
        }

        private void OpenCreateWindow()
        {
            AddRunnerWindow window = new AddRunnerWindow();
            window.ShowDialog();
        }


        private void OpenRegisterWindow()
        {
            RegisterRunnerWindow window = new RegisterRunnerWindow();
            window.ShowDialog();
        }

        private void StartListSearch()
        {
            NewSearch();
            try
            {
                connection.Open();
                string query = "SELECT Runners.RunnerID, [Name], Phone, Email, [Address], StartTime, EndTime " +
                               "FROM Runners INNER JOIN Registered " +
                               "ON Runners.RunnerID = Registered.RunnerID " +
                               "WHERE Distance = @routeDistance";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(CreateParameter("@routeDistance", routeDistance, SqlDbType.Decimal));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Runners.Add(new Runner((int)reader["RunnerID"], (string)reader["Name"], (string)reader["Phone"], (string)reader["Email"], (string)reader["Address"]));
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

        private void ResultListSearch()
        {
            NewSearch();
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
                                   (string)reader["Name"], new RunningCourse((TimeSpan)reader["StartTime"], (decimal)reader["Distance"]), (int)reader["RunnerID"],getTime(reader["EndTime"])));
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

        // Clear og instantier ny løber model.
        private void NewSearch()
        {
            Runners.Clear();
            Runner runner = new Runner();
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
            RouteDistance = dist;
        }
        private static TimeSpan getTime(object obj) //Scuffed løsning, men det skal håndteres at database returnerer null på EndTime
        {
            if(obj == DBNull.Value)
            {
                return new TimeSpan(0, 0, 0); //TimeSpan er normalt ikke nullable, så dette svarer til null i vores tilfælde
            }
            else
            {
                return (TimeSpan)obj;
            }
        }
        public bool IsResultListShown
        {
            get
            {
                return IsResultList;
            }
            set
            {
                IsResultList = value;
                if (IsResultList)
                {
                    ResultListSearch();
                    ResultGridVisibility = System.Windows.Visibility.Visible;
                    StartGridVisibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    StartListSearch();
                    StartGridVisibility = System.Windows.Visibility.Visible;
                    ResultGridVisibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        public System.Windows.Visibility ResultGridVisibility
        {
            get 
            {
                return resultgridVisibility;
            }
            set
            {
                resultgridVisibility = value;
                OnPropertyChanged("ResultGridVisibility");
            }
        }
        public System.Windows.Visibility StartGridVisibility
        {
            get
            {
                return startgridVisibility;
            }
            set
            {
                startgridVisibility = value;
                OnPropertyChanged("StartGridVisibility");
            }
        }
        public decimal RouteDistance
        {
            set
            {
                routeDistance = value;
                if (IsResultList)
                {
                    ResultListSearch();
                }
                else
                {
                    StartListSearch();
                }
            }
        }

        private void GetRunner()
        {
            NewSearch();
            try
            {
                connection.Open();
                string query = "SELECT Runners.RunnerID, [Name], Distance, StartTime, EndTime " +
               "FROM Runners INNER JOIN Registered " +
               "ON Runners.RunnerID = Registered.RunnerID " +
               "WHERE Runners.RunnerID = @RunnerID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(CreateParameter("@RunnerID", NumberOfRunners, SqlDbType.Int)); 
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Runners.Add(new Runner(new PropertyChangedEventHandler(Runner_PropertyChanged),
                         (string)reader["Name"], new RunningCourse((TimeSpan)reader["StartTime"], (decimal)reader["Distance"]), (int)reader["RunnerID"], getTime(reader["EndTime"])));
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

        public bool CanSearch()
        {
            if (IsResultList && NumberOfRunners > 99)
            {
                return true;
            }
            else return false;
        }

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
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
