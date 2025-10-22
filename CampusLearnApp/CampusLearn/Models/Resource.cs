using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusLearn.Models
{
    public class Resource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Type { get; set; } // Notes, Study Guide, Research Paper
        public string FileType { get; set; } // PDF, DOCX
        public double FileSizeMB { get; set; }
        public string UploadedAgo { get; set; } // e.g. "3 days ago"
        public int Downloads { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}