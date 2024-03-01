using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Item
    {
        public int? Id { get; set; }

        public string? Desc { get; set; }
        public string? Date { get; set; }
    }
}
