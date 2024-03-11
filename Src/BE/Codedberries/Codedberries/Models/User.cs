using System;

public class User
{
    public int UserId { get; set; }
    private string? Username { get; set; }
    public string? Password { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public string? Email { get; set; }
    private int RoleID { get; set; }

}
