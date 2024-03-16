﻿using System.ComponentModel.DataAnnotations.Schema;
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
        [EnumDataType(typeof(StatusEnum))]
        public string Status { get; set; }

        [Required]
        [EnumDataType(typeof(PriorityEnum))]
        public string Priority { get; set; }

        [Required]
        public string DifficultyLevel { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public ICollection<Task> Dependencies { get; } = new List<Task>();
        public ICollection<Task> DependentTasks { get; } = new List<Task>();

        public Task(string description, DateTime dueDate, string status, string priority, string difficultyLevel, int categoryId)
        {
            Description = description;
            DueDate = dueDate;
            Status = status;
            Priority = priority;
            DifficultyLevel = difficultyLevel;
            CategoryId = categoryId;
        }
    }

    public enum StatusEnum
    {
        Open,
        InProgress,
        Pending,
        Completed
    }

    public enum PriorityEnum
    {
        Low,
        Medium,
        High
    }
}
