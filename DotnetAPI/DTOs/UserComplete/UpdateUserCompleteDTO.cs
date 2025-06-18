namespace DotnetAPI.DTOs.UserComplete;

public partial class UpdateUserCompleteDTO
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Gender { get; set; } = "";
    public string JobTitle { get; set; } = "";
    public string Department { get; set; } = "";
    public decimal Salary { get; set; }
    public bool Active { get; set; }
}