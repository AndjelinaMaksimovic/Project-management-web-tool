﻿namespace Codedberries.Models.DTOs
{
    public class ProjectFilterDTO
    {
        public int? ProjectId { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime? DueDateAfter { get; set; }
        public DateTime? DueDateBefore { get; set; }
    }
}