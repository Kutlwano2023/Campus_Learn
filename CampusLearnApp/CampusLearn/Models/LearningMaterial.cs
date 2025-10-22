using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class LearningMaterial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MaterialId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(50)]
        public string MaterialType { get; set; } // PDF, Video, Document, Presentation

        [StringLength(500)]
        public string FilePathUrl { get; set; }

        public string ModuleId { get; set; }
        public string TopicId { get; set; }
        public string UploadedById { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public long FileSize { get; set; }
        public int DownloadCount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}