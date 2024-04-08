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

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [ForeignKey("TaskId")]
        public int TaskId { get; set; }

        public TaskComment(string comment, int userid, int taskid)
        {
            Comment = comment;
            UserId = userid;
            TaskId = taskid;
        }
    }
}
