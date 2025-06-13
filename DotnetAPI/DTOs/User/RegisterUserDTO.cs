namespace DotnetAPI.DTOs.User;

public partial class RegisterUserDTO
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string PasswordConfirm { get; set; } = "";

    // Additional properties can be added as needed
}