using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusLearn.Models
{
    public class Resource
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
        public string FileType { get; set; }
        public double FileSizeMB { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadDate { get; set; }
    }
    public class ResourcePack
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public int FileCount { get; set; }
        public double TotalSize { get; set; } // in MB
        public string Category { get; set; }
        public string Icon { get; set; }
    }

    public class ResourceViewModel
    {
        public List<Resource> NotesAndDocuments { get; set; } = new List<Resource>();
        public List<Resource> StudyGuides { get; set; } = new List<Resource>();
        public List<Resource> ResearchPapers { get; set; } = new List<Resource>();
        public List<ResourcePack> CourseDownloads { get; set; } = new List<ResourcePack>();
        public string UserRole { get; set; } = "Student";
    }
}