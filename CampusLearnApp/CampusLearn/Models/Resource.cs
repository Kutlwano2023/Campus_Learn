using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusLearn.Models
{
    public class Resource
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // Study Guide, Research Paper, Notes, etc.
        public string FileType { get; set; } // PDF, DOCX, Audio, etc.
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public double FileSize { get; set; } // in MB
        public int DownloadCount { get; set; }
        public string Icon { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
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