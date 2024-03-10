using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    [Table("Tasks")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [Required]
        public bool Status { get; set; }

        public ICollection<Task> Dependencies { get; } = new List<Task>();
        public ICollection<Task> DependentTasks { get; } = new List<Task>();

        public Task(string description, DateTime dueDate, int userId, bool status)
        {
            Description = description;
            DueDate = dueDate;
            UserId = userId;
            Status = status;
        }
    }
}
