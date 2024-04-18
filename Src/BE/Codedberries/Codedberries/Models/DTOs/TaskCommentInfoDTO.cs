namespace Codedberries.Models.DTOs
{
    public class TaskCommentInfoDTO
    {
        public int CommentId { get; set; }
        public string Comment { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }

    }
}
