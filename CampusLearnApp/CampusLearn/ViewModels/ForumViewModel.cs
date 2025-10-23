using CampusLearn.Models;

namespace CampusLearn.ViewModels
{
    public class ForumViewModel
    {
        public List<ForumTopic> Topics { get; set; } = new List<ForumTopic>();
        public List<ForumTopic> PopularTopics { get; set; } = new List<ForumTopic>();
        public List<ForumTopic> UnansweredTopics { get; set; } = new List<ForumTopic>();
        public string UserRole { get; set; } = "Student";
        public ForumStats Stats { get; set; } = new ForumStats();
    }

    public class ForumStats
    {
        public int TotalTopics { get; set; }
        public int TotalPosts { get; set; }
        public int ActiveUsers { get; set; }
    }
}