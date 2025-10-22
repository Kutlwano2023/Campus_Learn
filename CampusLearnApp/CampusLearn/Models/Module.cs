using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Module
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ModuleId { get; set; }

        [Required]
        public string ModuleName { get; set; }

        [Required]
        public string Description { get; set; }

        public int Credits { get; set; }
        public string CourseId { get; set; }
        public string TutorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties (for reference)
        public List<string> TopicIds { get; set; } = new List<string>();
        public List<string> AssessmentIds { get; set; } = new List<string>();

        public void AddTopic(string topicId)
        {
            if (!TopicIds.Contains(topicId))
            {
                TopicIds.Add(topicId);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void AddAssessment(string assessmentId)
        {
            if (!AssessmentIds.Contains(assessmentId))
            {
                AssessmentIds.Add(assessmentId);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void GetLearningMaterials()
        {
            // Logic to retrieve learning materials
        }
    }
}