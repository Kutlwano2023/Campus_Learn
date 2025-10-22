using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Assessment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AssessmentId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public double MaxScore { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } // Quiz, Assignment, Exam, Project

        [Required]
        public string ModuleId { get; set; }

        public string Description { get; set; }
        public string Instructions { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Assessment settings
        public int TimeLimitMinutes { get; set; } = 0; // 0 = no time limit
        public int AttemptsAllowed { get; set; } = 1;
        public double PassingScore { get; set; } = 0;

        public void Grade(string submissionId, double score, string gradedById = null)
        {
            // Logic for grading assessment
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignModule(string moduleId)
        {
            ModuleId = moduleId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAssessment(string title, string type, double maxScore, DateTime dueDate)
        {
            Title = title;
            Type = type;
            MaxScore = maxScore;
            DueDate = dueDate;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum AssessmentType
    {
        Quiz,
        Assignment,
        Exam,
        Project,
        Practical
    }
}