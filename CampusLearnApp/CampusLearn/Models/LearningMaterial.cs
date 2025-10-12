using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusLearnApplication.Models
{
    internal class LearningMaterial
    {
        public int materialID { get; set; }
        public string title { get; set; }
        public string materialType { get; set; }
        public string filePathURL { get; set; }

        public void download()
        {
            //logic 
        }

        public void preview()
        {
            //logic
        }
    }
}
