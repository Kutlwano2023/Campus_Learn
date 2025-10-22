using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Tutor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TutorId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string Specialization { get; set; }

        // Link to Identity User
        [Required]
        public string UserId { get; set; }

        // Additional properties
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}