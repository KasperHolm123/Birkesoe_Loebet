using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Birkesoe_Loebet.Models
{
    public class RunningCourse
    {
        public int CourseDistance { get; set; }
        public int StartTime { get; set; }
        public RunningCourse(int distance, int startTime)
        {
            CourseDistance = distance;
            StartTime = startTime;
        }
    }
}
