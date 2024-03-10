using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    public class TaskDependency
    {
        [Key]
        [Column(Order = 1)]
        public int TaskId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int DependentTaskId { get; set; }
    }
}
