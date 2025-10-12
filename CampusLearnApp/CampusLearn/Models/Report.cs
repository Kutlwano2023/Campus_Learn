using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusLearnApplication.Models
{
    internal class Report
    {
        // Attributes (state)
        public int reporterID { get; set; }
        public string description { get; set; }
        public DateTime dateReported { get; set; }
        public string status { get; set; }

        public void submitReport()
        {
            //logic
        }
        public void resolveReport()
        {
            //logic
        }
    }
}
