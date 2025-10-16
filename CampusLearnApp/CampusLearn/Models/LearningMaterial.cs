using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn.Models
{
    public class LearningMaterial
    {
        [Key]
        public int MaterialID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(50)]
        public string MaterialType { get; set; }

        [StringLength(500)]
        public string FilePathURL { get; set; }
    }
}