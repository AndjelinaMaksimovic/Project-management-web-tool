using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Codedberries.Common;

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
        [EnumDataType(typeof(StatusEnum))]
        public string Status { get; set; }

        [Required]
        [EnumDataType(typeof(PriorityEnum))]
        public string Priority { get; set; }

        [Required]
        public int DifficultyLevel { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public ICollection<Task> Dependencies { get; } = new List<Task>();
        public ICollection<Task> DependentTasks { get; } = new List<Task>();

        public Task(string description, DateTime dueDate, int userId, string status, string priority, int difficultyLevel, int categoryId)
        {
            Description = description;
            DueDate = dueDate;
            UserId = userId;
            Status = status;
            Priority = priority;
            DifficultyLevel = difficultyLevel;
            CategoryId = categoryId;
        }
    }
}
