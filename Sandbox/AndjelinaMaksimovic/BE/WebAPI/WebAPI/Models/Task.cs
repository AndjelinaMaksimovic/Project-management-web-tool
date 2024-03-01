using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string? shortDescription { get; set; }
        public string? assignedTo { get; set; }
    }
}
