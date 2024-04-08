using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    [Table("TaskComments")]
    public class TaskComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public Task Task { get; set; }

        public TaskComment(string comment, int userid, int taskid)
        {
            Comment = comment;
            UserId = userid;
            TaskId = taskid;
        }
    }
}
