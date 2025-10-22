using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Topic
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TopicId { get; set; }

        [Required]
        public string TopicName { get; set; }

        public string Description { get; set; }

        [Required]
        public string ModuleId { get; set; }

        public int Order { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Learning materials associated with this topic
        public List<string> MaterialUrls { get; set; } = new List<string>();
        public List<string> VideoUrls { get; set; } = new List<string>();

        public void AddMaterial(string materialUrl)
        {
            MaterialUrls.Add(materialUrl);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddVideo(string videoUrl)
        {
            VideoUrls.Add(videoUrl);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}