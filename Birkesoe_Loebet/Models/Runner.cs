using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Birkesoe_Loebet.Models
{
    /// <summary>
    /// Model for hver enkelte løber. 
    /// Modelen bruges som skiste der skal indeholde data om de enkelte løbere
    /// </summary>
    public class Runner
    {
        public string Name { get; set; }
        public string RunnerAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<RunningCourse> RunningCourses = new List<RunningCourse>(); // Hver enkelte løber kan tilmælde sig én eller flere distancer
        public int EndTime { get; set; }
        public int RunnerID { get; set; }
        public Runner(string name, string address, string phone, string email) //Build Runner model
        {
            Name = name;
            RunnerAddress = address;
            PhoneNumber = phone;
            Email = email;
        }


    }
}
