namespace DotnetAPI.Models;

public partial class Post
{
    public string PostId { get; set; } = "";
    public string UserId { get; set; } = "";
    public int PostTitle { get; set; }
    public int PostContent { get; set; }
    public DateTime? PostCreated { get; set; }
    public DateTime? PostUpdated { get; set; }
}