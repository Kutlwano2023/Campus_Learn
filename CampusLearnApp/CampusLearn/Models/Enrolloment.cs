using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Enrollment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EnrollmentId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public string CourseId { get; set; }

        [Required]
        public string EnrollmentName { get; set; }

        public double Progress { get; set; } = 0.0;

        [BsonRepresentation(BsonType.String)]
        public CompletionStatus CompletionStatus { get; set; } = CompletionStatus.InProgress;

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletionDate { get; set; }
        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

        // Module progress tracking
        public Dictionary<string, double> ModuleProgress { get; set; } = new Dictionary<string, double>();

        public void UpdateProgress(double progress)
        {
            Progress = Math.Clamp(progress, 0.0, 100.0);
            LastAccessed = DateTime.UtcNow;

            if (Progress >= 100.0)
            {
                CompletionStatus = CompletionStatus.Completed;
                CompletionDate = DateTime.UtcNow;
            }
        }

        public void UpdateModuleProgress(string moduleId, double progress)
        {
            ModuleProgress[moduleId] = Math.Clamp(progress, 0.0, 100.0);
            UpdateProgress(CalculateOverallProgress());
        }

        private double CalculateOverallProgress()
        {
            if (ModuleProgress.Count == 0) return 0.0;
            return ModuleProgress.Values.Average();
        }

        public void CompleteModule(string moduleId)
        {
            UpdateModuleProgress(moduleId, 100.0);
        }
    }

    public enum CompletionStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Dropped
    }
}