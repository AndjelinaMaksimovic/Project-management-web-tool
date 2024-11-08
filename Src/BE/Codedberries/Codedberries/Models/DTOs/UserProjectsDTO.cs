﻿namespace Codedberries.Models.DTOs
{
    public class UserProjectsDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public int ProjectId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
