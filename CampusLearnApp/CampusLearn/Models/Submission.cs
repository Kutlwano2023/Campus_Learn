using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Submission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SubmissionId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public string AssessmentId { get; set; }

        public float Score { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
        public DateTime? GradedDate { get; set; }

        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string Comments { get; set; }

        [BsonRepresentation(BsonType.String)]
        public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;

        public void NewSubmission(string fileUrl, string fileName)
        {
            FileUrl = fileUrl;
            FileName = fileName;
            SubmissionDate = DateTime.UtcNow;
            Status = SubmissionStatus.Submitted;
        }

        public void ViewScore()
        {
            // Logic to display score
        }

        public void GradeSubmission(float score, string comments = null)
        {
            Score = score;
            Comments = comments;
            GradedDate = DateTime.UtcNow;
            Status = SubmissionStatus.Graded;
        }
    }

    public enum SubmissionStatus
    {
        Submitted,
        UnderReview,
        Graded,
        Returned
    }
}