using System;
using System.Collections.Generic;

namespace CampusLearn.Models
{
    public class ForumTopic
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Author { get; set; }
        public string AuthorInitials { get; set; }
        public string Category { get; set; }
        public string PostedTime { get; set; }
        public int ReplyCount { get; set; }
        public int LikeCount { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPinned { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}