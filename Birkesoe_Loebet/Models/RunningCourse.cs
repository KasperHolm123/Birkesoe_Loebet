using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Birkesoe_Loebet.Models
{
    public class RunningCourse
    {
        public int ID { get; set; }
        public decimal CourseDistance { get; set; }

        private TimeSpan startTime;

        public RunningCourse()
        {

        }

        public RunningCourse(int ID = 1)
        {
            this.ID = ID;
        }

        public RunningCourse(TimeSpan starttime, decimal distance = 2.3M, int ID = 1)
        {
            this.ID = ID;
            startTime = starttime;
            CourseDistance = distance;
        }
        
        public string StartTime
        {
            get { return startTime.ToString(); }
            set
            {
                startTime = TimeSpan.Parse(value);
            }
        }
    }
}
