using CampusLearn.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CampusLearn.Services
{
    public class MongoService
    {
        private readonly IMongoDatabase _db;

        public MongoService(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _db = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Identity and User collections
        public IMongoCollection<Users> Users => _db.GetCollection<Users>("Users");

        // Chat collections
        public IMongoCollection<ChatConversation> ChatConversations => _db.GetCollection<ChatConversation>("ChatConversations");
        public IMongoCollection<ChatMessage> ChatMessages => _db.GetCollection<ChatMessage>("ChatMessages");

        // Academic collections
        public IMongoCollection<Resource> Resources => _db.GetCollection<Resource>("Resources");
        public IMongoCollection<Tutor> Tutors => _db.GetCollection<Tutor>("Tutors");
        public IMongoCollection<Topic> Topics => _db.GetCollection<Topic>("Topics");
        public IMongoCollection<Submission> Submissions => _db.GetCollection<Submission>("Submissions");
        public IMongoCollection<Module> Modules => _db.GetCollection<Module>("Modules");
        public IMongoCollection<Enrollment> Enrollments => _db.GetCollection<Enrollment>("Enrollments");
        public IMongoCollection<Review> Reviews => _db.GetCollection<Review>("Reviews");
        public IMongoCollection<Student> Students => _db.GetCollection<Student>("Students");

        // Assessment and Report collections
        public IMongoCollection<Assessment> Assessments => _db.GetCollection<Assessment>("Assessments");
        public IMongoCollection<Report> Reports => _db.GetCollection<Report>("Reports");

        // Learning Material collection
        public IMongoCollection<LearningMaterial> LearningMaterials => _db.GetCollection<LearningMaterial>("LearningMaterials");
    }
}