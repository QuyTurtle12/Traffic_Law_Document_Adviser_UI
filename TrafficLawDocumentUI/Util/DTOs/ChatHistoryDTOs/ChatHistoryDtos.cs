namespace Util.DTOs.ChatHistoryDTOs
{
    public class PostChatHistoryDto
    {
        public Guid UserId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }
    public class GetChatHistoryDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public string? CreatedAtFormatted => CreatedTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
