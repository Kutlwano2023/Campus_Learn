using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Tutor
    {
        [Key]
        public int TutorID { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string Specialization { get; set; }

        // Link to Identity User
        public string UserId { get; set; }
        public virtual Users User { get; set; }
    }
}