namespace Util.DTOs.FeedbackDTOs
{
    public class GetFeedbackDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatHistory { get; set; }
        public string? AIAnswer { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string? CreatedAtFormatted => CreatedTime?.ToString("yyyy-MM-dd HH:mm:ss");
        public string? Email { get; set; }
    }
    public class PostFeedbackDto
    {
        public Guid UserId { get; set; }
        public Guid ChatHistory { get; set; }
        public string? AIAnswer { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
    public class PutFeedbackDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatHistory { get; set; }
        public string? AIAnswer { get; set; }
        public string? Content { get; set; }
    }
}
