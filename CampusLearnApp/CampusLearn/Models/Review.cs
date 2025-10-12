using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusLearnApplication.Models
{
    internal class Review
    {
        public int ReviewId { get; set; }
        public string CommentText { get; set; }
        public int Rating { get; set; }
        public DateTime DateCommented { get; set; }
       
        public void AddReview()
        {
           //logic
        }
        public void UpdateReview()
        {
            //logic
        }
        }
    }
