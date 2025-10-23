using System;
using System.Collections.Generic;

namespace CampusLearn.Models
{
    public class Quiz
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Subject { get; set; }
            public string Difficulty { get; set; }
            public int QuestionCount { get; set; }
            public int Duration { get; set; }
            public int AverageScore { get; set; }
            public string Author { get; set; }
            public string AuthorInitials { get; set; }
            public double Rating { get; set; }
            public int Attempts { get; set; }
            public List<string> Tags { get; set; }
        }

        public class QuizResult
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string QuizTitle { get; set; }
        public double Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public string TimeSpent { get; set; }
        public string Status { get; set; } // passed, review
        public int AttemptNumber { get; set; }
        public DateTime DateTaken { get; set; }
    }

    public class QuizCategory
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int QuizCount { get; set; }
    }

    public class QuizAnalytics
    {
        public int QuizzesTaken { get; set; }
        public double AverageScore { get; set; }
        public string StudyTime { get; set; }
        public int Streak { get; set; }
        public List<PerformanceTrend> Trends { get; set; } = new List<PerformanceTrend>();
        public List<SubjectPerformance> SubjectPerformance { get; set; } = new List<SubjectPerformance>();
    }

    public class PerformanceTrend
    {
        public string Period { get; set; }
        public double Score { get; set; }
    }

    public class SubjectPerformance
    {
        public string Subject { get; set; }
        public double Score { get; set; }
    }

    public class QuizViewModel
    {
        public List<Quiz> AvailableQuizzes { get; set; } = new List<Quiz>();
        public List<QuizResult> RecentResults { get; set; } = new List<QuizResult>();
        public List<QuizCategory> Categories { get; set; } = new List<QuizCategory>();
        public List<Quiz> RemainingQuizzes { get; set; } = new List<Quiz>();
        public QuizAnalytics Analytics { get; set; } = new QuizAnalytics();
        public string UserRole { get; set; } = "Student";
    }
}