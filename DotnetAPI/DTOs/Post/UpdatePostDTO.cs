namespace DotnetAPI.DTOs.Post;

public partial class UpdatePostDTO
{
    public int UserId { get; set; }
    public int PostId { get; set; }
    public string PostTitle { get; set; } = "";
    public string PostContent { get; set; } = "";
    // public DateTime PostUpdated { get; set; } = DateTime.UtcNow;
}