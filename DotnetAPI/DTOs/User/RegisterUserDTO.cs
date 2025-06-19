namespace DotnetAPI.DTOs.User;

public partial class RegisterUserDTO : Models.User
{
    // public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string PasswordConfirm { get; set; } = "";
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public decimal Salary { get; set; }

    // Additional properties can be added as needed
}