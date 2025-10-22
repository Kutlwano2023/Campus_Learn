using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusLearn.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReviewId { get; set; }

        public string CommentText { get; set; }
        public int Rating { get; set; }
        public DateTime DateCommented { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
        public string CourseId { get; set; }

        public void AddReview()
        {
            //logic
        }

        public void UpdateReview()
        {
            //logic
        }
    }
}