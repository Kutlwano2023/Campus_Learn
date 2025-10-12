using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusLearnApplication.Models
{
    internal class Enrolloment
    {
        public int enrollmentId { get; set; }
        public string enrollmentName { get; set; }
        public string progress { get; set; }
        public string completionStatus { get; set; }
     
        public void updateProgress(string progress)
        {
            //logic
        }
        public void completeModule()
        {
            //logic
        }
    }
}
