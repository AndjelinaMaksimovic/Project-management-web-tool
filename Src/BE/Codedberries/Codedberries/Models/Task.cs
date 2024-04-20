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
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishedDate { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        [Required]
        public int PriorityId { get; set; }

        [Required]
        [ForeignKey("PriorityId")]
        public Priority Priority { get; set; }

        [Required]
        public int DifficultyLevel { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public Boolean Archived { get; set; }

        public ICollection<Task> Dependencies { get; } = new List<Task>();
        public ICollection<Task> DependentTasks { get; } = new List<Task>();

        public Task(string name, string description, DateTime dueDate,DateTime startDate ,int userId, int statusId, int priorityId, int difficultyLevel, int categoryId, int projectId)
        {
            Name = name;
            Description = description;
            DueDate = dueDate;
            UserId = userId;
            StatusId = statusId;
            PriorityId = priorityId;
            DifficultyLevel = difficultyLevel;
            CategoryId = categoryId;
            ProjectId = projectId;
            StartDate = startDate;
        }
    }
}
