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
using System.Diagnostics;

namespace Birkesoe_Loebet.ViewModels
{
    class RegisterViewModel
    {
        public event WarningMessage WarningHandler; // Event der håndterer advarsler
        public int RunnerID { get; set; }
        public int Course { get; set; }
        private bool route1Enabled;
        private bool route2Enabled;
        private bool route3Enabled;
        private List<RunningCourse> courses; //Brugeren skal på en eller anden måde kunne se hvilke ruter som er valgt før det registreres

        // Modeler som skal bruges i SQL queries
        Runner model = new Runner();
        RunningCourse courseModel = new RunningCourse();

        // Command til knappen i viewet
        public RelayCommand RegisterRunner { get; set; }
        
        private SqlConnection connection;

        public RegisterViewModel()
        {
            courses = new List<RunningCourse>();
            RegisterRunner = new RelayCommand(p => RegisterCmd());
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
        }

        private void RegisterCmd()
        {
            Trace.WriteLine("yeet");
            BuildModel();
            //INSERT INTO query 
            try
            {
                connection.Open();
                string query = "INSERT INTO Registered (RunnerID, Distance, StartTime)\n" +
                               "VALUES ((SELECT RunnerID FROM Runners WHERE RunnerID = @RunnerID), " +
                               "(SELECT Distance FROM [Route] WHERE ID = @Course), " +
                               "(SELECT StartTime FROM [Route] WHERE ID = @Course))";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(CreateParameter("@RunnerID", model.RunnerID, SqlDbType.Int));
                command.Parameters.Add(CreateParameter("@Course", courseModel.ID, SqlDbType.Int));
                //command.Parameters.Add(CreateParameter("@StartTime", courseModel.ID, SqlDbType.Int));
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
        private void BuildModel()
        {
            model.RunnerID = RunnerID;
            courseModel.ID = Course;
        }
        #region gammelt kode
        //public bool Route1Enabled
        //{
        //    get
        //    {
        //        return route1Enabled;
        //    }
        //    set
        //    {
        //        route1Enabled = value;
        //        if (route1Enabled)
        //        {
        //            courses[0] = new RunningCourse(1);
        //        }
        //        else
        //        {
        //            courses[0] = null;
        //        }
        //    }
        //}
        //public bool Route2Enabled
        //{
        //    get
        //    {
        //        return route2Enabled;
        //    }
        //    set
        //    {
        //        route2Enabled = value;
        //        if (route2Enabled)
        //        {
        //            courses[1] = new RunningCourse(2);
        //        }
        //        else
        //        {
        //            courses[1] = null;
        //        }
        //    }
        //}
        //public bool Route3Enabled
        //{
        //    get
        //    {
        //        return route3Enabled;
        //    }
        //    set
        //    {
        //        route3Enabled = value;
        //        if (route3Enabled)
        //        {
        //            courses[2] = new RunningCourse(3);
        //        }
        //        else
        //        {
        //            courses[2] = null;
        //        }
        //    }
        //}
        #endregion

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
    }
}
