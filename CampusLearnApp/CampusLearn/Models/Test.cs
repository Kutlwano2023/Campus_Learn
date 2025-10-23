public class Test
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Difficulty { get; set; }
    public int QuestionCount { get; set; }
    public int Duration { get; set; }
    public decimal AverageScore { get; set; }
    public string Author { get; set; }
    public decimal Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserTest
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string UserId { get; set; }
    public string Status { get; set; } // NotStarted, InProgress, Completed
    public int QuestionsAnswered { get; set; }
    public int TotalQuestions { get; set; }
    public decimal? Score { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime DueDate { get; set; }
}