namespace DotnetAPI.DTOs.Post;

public partial class CreatePostDTO
{
    public int UserId { get; set; }
    public string PostTitle { get; set; } = "";
    public string PostContent { get; set; } = "";
    // public DateTime? PostCreated { get; set; }
    // public DateTime? PostUpdated { get; set; }
}