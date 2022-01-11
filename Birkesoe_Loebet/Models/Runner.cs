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
        public List<RunningCourse> RunningCourses = new List<RunningCourse>(); // Hver enkelte løber kan tilmælde sig én eller flere distancer
        private int endTime;
        public int RunnerID { get; set; }
        public RunningCourse course;
        public Runner(PropertyChangedEventHandler eventHandler, string name, string address, string phone, string email) //Build Runner model
        {
            PropertyChanged += eventHandler;
            Name = name;
            RunnerAddress = address;
            PhoneNumber = phone;
            Email = email;
        }
        public Runner()
        {

        }
        public int EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                OnPropertyChanged("EndTime");
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
