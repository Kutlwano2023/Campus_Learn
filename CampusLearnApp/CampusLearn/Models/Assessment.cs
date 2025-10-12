using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusLearnApplication.Models
{
    internal class Assessment
    {
        public int assessmentID { get; set; }
        public string title { get; set; }
        public double maxScore { get; set; }
        public string type { get; set; }
        public void grade()
        {
            //logic 
        }
        public void assignModule()
        {
            //logic
        }
    }
}
