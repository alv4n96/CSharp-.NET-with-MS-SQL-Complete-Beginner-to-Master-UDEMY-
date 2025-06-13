namespace DotnetAPI.DTOs.User;

public partial class LoginConfirmationUserDTO
{
    public Byte[] PasswordHash { get; set; } = new byte[0];
    public Byte[] PasswordSalt { get; set; } = new byte[0];
}