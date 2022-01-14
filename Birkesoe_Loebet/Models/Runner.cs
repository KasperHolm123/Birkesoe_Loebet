using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
namespace Birkesoe_Loebet.Models
{
    /// <summary>
    /// Model for hver enkelte løber. 
    /// Modelen bruges som skiste der skal indeholde data om de enkelte løbere
    /// </summary>
    public class Runner : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
        public string RunnerAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public int RunnerID { get; set; }
        private string result;
        
        public List<RunningCourse> RunningCourses = new List<RunningCourse>(); // Hver enkelte løber kan tilmelde sig én eller flere distancer
        
        private TimeSpan endTime;
        
        public RunningCourse Course { get; set; }

        public Runner()
        {

        }

        // Constructor til søg i start liste
        public Runner(int runnerID, string name, string phone, string email, string address)
        {
            RunnerID = runnerID;
            Name = name;
            RunnerAddress = address;
            PhoneNumber = phone;
            Email = email;
        }

        // Contructor til søg i resultat liste
        public Runner(PropertyChangedEventHandler eventHandler, string name, RunningCourse course, int runnerID, TimeSpan endtime)
        {
            endTime = endtime;
            PropertyChanged += eventHandler;
            Name = name;
            Course = course;
            RunnerID = runnerID;
        }


        public Runner(PropertyChangedEventHandler eventHandler, string name, int id)
        {
            PropertyChanged += eventHandler;
            Name = name;
            RunnerID = id;
        }
        
        public string EndTime
        {
            get
            {
                if (0 == TimeSpan.Compare(endTime, new TimeSpan(0, 0, 0)))
                {
                    return null;
                }
                return endTime.ToString();
            }

            set
            {
                endTime = TimeSpan.Parse(value);
                OnPropertyChanged("EndTime");
            }
        }

        public string Result
        {
            get
            {
                if(0 == TimeSpan.Compare(endTime, new TimeSpan(0,0,0)) || Course == null)
                {
                    return null;
                }
                TimeSpan dateDifference = endTime.Subtract(TimeSpan.Parse(Course.StartTime));
                return dateDifference.ToString();
            }
        }

        private void OnPropertyChanged(string property)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
