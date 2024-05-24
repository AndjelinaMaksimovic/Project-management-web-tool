using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    public class TaskUser
    {
        [Key, Column(Order = 0)]
        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public Task Task { get; set; }

        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
