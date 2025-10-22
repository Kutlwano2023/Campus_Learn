using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReportId { get; set; }

        [Required]
        public string ReporterId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime DateReported { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(50)]
        public string Category { get; set; } // Technical, Academic, Behavioral, Other

        public string ReportedUserId { get; set; } // User being reported (if applicable)
        public string CourseId { get; set; } // Related course (if applicable)
        public string ModuleId { get; set; } // Related module (if applicable)

        // Resolution fields
        public string ResolvedById { get; set; }
        public DateTime? DateResolved { get; set; }
        public string ResolutionNotes { get; set; }

        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        public void SubmitReport(string reporterId, string title, string description, string category = "Other")
        {
            ReporterId = reporterId;
            Title = title;
            Description = description;
            Category = category;
            DateReported = DateTime.UtcNow;
            Status = "Pending";
        }

        public void ResolveReport(string resolvedById, string resolutionNotes = null)
        {
            Status = "Resolved";
            ResolvedById = resolvedById;
            DateResolved = DateTime.UtcNow;
            ResolutionNotes = resolutionNotes;
        }

        public void UpdateStatus(string status, string updatedById = null)
        {
            Status = status;
            if (status == "Resolved" && !string.IsNullOrEmpty(updatedById))
            {
                ResolvedById = updatedById;
                DateResolved = DateTime.UtcNow;
            }
        }
    }

    public enum ReportStatus
    {
        Pending,
        UnderReview,
        Resolved,
        Rejected
    }

    public enum ReportPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}