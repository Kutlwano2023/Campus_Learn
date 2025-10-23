using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Resource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // Lecture, Assignment, Reading, etc.
        public string FileType { get; set; } // PDF, DOC, PPT, etc.
        public double FileSizeMB { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string ModuleId { get; set; }
        public string TopicId { get; set; }
        public string UploadedBy { get; set; }
        public string Author { get; set; }
        public DateTime UploadDate { get; set; }
        public int Downloads { get; set; }
        public bool IsActive { get; set; } = true;
        // For display purposes
        [BsonIgnore]
        public string UploadedAgo => GetTimeAgo(UploadDate);

        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.UtcNow - date;
            if (timeSpan.TotalDays >= 365) return $"{(int)(timeSpan.TotalDays / 365)} years ago";
            if (timeSpan.TotalDays >= 30) return $"{(int)(timeSpan.TotalDays / 30)} months ago";
            if (timeSpan.TotalDays >= 1) return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalHours >= 1) return $"{(int)timeSpan.TotalHours} hours ago";
            return $"{(int)timeSpan.TotalMinutes} minutes ago";
        }
    }
}