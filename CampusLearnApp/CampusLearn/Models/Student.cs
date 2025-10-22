using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusLearn.Models
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StudentId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string UserId { get; set; } // Link to Identity User

        public void Enroll()
        {
            //logic 
        }

        public void SubmitAssessment()
        {
            //logic
        }

        public void GetReport()
        {
            //logic
        }
    }
}