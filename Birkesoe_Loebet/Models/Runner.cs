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
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<string> RunningCourses = new List<string>(); // Hver enkelte løber kan tilmælde sig én eller flere distancer
        public int EndTime { get; set; }

    }
}
