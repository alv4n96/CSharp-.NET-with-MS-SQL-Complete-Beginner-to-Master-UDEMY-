using System.Data;
using System.Net.WebSockets;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs.Post;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public PostController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
    }

    [HttpGet("{postId}/{searchParam}")]
    public IActionResult GetPosts(int postId, string searchParam = "None")
    {

        string userIdString = this.User.FindFirst("UserId")?.Value + "";
        int userId = !string.IsNullOrEmpty(userIdString) ? int.Parse(userIdString) : 0;
        string query = @"TutorialAppSchema.spPosts_Get";

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId, DbType.Int32);

        if (postId != 0)
            parameters.Add("PostId", postId, DbType.Int32);

        if (searchParam != "None")
            parameters.Add("SearchValue", searchParam, DbType.String);

        // Note: In a real application, you might want to use a more secure way to handle the query, such as parameterized queries.
        var posts = _dapper.LoadData<Post>(query);
        return Ok(posts);
    }


    [HttpGet("UserPosts")]
    public IActionResult GetUserPosts()
    {
        string userId = this.User.FindFirst("UserId")?.Value + "";

        string query = @"SELECT 
        PostId
        ,UserId 
        ,PostTitle 
        ,PostContent 
        ,PostCreated 
        ,PostUpdated 
        FROM TutorialAppSchema.Posts WHERE UserId = @UserId;";
        var parameters = new { UserId = userId };
        var posts = _dapper.LoadData<Post>(query, parameters);

        if (posts == null || !posts.Any())
        {
            return NotFound(new { Message = "No posts found for this user." });
        }

        return Ok(posts);
    }

    [HttpPost("CreatePost")]
    public IActionResult CreatePost(CreatePostDTO post)
    {
        if (post == null || string.IsNullOrEmpty(post.PostTitle) || string.IsNullOrEmpty(post.PostContent))
        {
            return BadRequest(new { Message = "Invalid post data." });
        }

        string userId = this.User.FindFirst("UserId")?.Value + "";
        post.UserId = int.Parse(userId);
        DateTime now = DateTime.UtcNow;
        // post.PostUpdated = DateTime.Now;

        // string query = @"INSERT INTO TutorialAppSchema.Posts (UserId, PostTitle, PostContent, PostCreated, PostUpdated) 
        //  VALUES (@UserId, @PostTitle, @PostContent, @PostCreated, @PostUpdated);";
        string insertQuery = @"
        INSERT INTO TutorialAppSchema.Posts
        (UserId, PostTitle, PostContent, PostCreated, PostUpdated)
        VALUES (@UserId, @PostTitle, @PostContent, @PostCreated, @PostUpdated);

        SELECT CAST(SCOPE_IDENTITY() AS int);
    ";

        var parameters = new DynamicParameters();

        parameters.Add("@UserId", post.UserId, DbType.Int32);
        parameters.Add("@PostTitle", post.PostTitle, DbType.String);
        parameters.Add("@PostContent", post.PostContent, DbType.String);
        parameters.Add("@PostCreated", now, DbType.DateTime);
        parameters.Add("@PostUpdated", now, DbType.DateTime);
        System.Console.WriteLine($"Parameter : {parameters}");

        // string getIdQuery = "SELECT CAST(SCOPE_IDENTITY() AS int);";
        int postId = _dapper.LoadSingleData<int>(insertQuery, parameters);
        System.Console.WriteLine($"PostId : {postId}");

        string selectQuery = @"SELECT 
        PostId
        ,UserId 
        ,PostTitle 
        ,PostContent 
        ,PostCreated 
        ,PostUpdated 
        FROM TutorialAppSchema.Posts WHERE PostId = @PostId;";
        // Note: Using SCOPE_IDENTITY() to get the last inserted ID in the same session.
        parameters = new DynamicParameters();
        parameters.Add("@PostId", postId, DbType.Int32);
        // System.Console.WriteLine($"Select Query : {selectQuery}");
        var createdPost = _dapper.LoadData<Post>(selectQuery, parameters).FirstOrDefault();
        System.Console.WriteLine($"Created Post: {createdPost}");
        if (createdPost == null)
        {
            return StatusCode(500, new { Message = "Post creation failed." });
        }

        return Ok(createdPost);

    }

    [HttpPut("UpdatePost/{postId}")]
    public IActionResult UpdatePost(int postId, UpdatePostDTO post)
    {
        if (post == null || string.IsNullOrEmpty(post.PostTitle) || string.IsNullOrEmpty(post.PostContent))
        {
            return BadRequest(new { Message = "Invalid post data." });
        }

        string userId = this.User.FindFirst("UserId")?.Value + "";
        post.UserId = int.Parse(userId);
        post.PostUpdated = DateTime.Now;

        string query = @"UPDATE TutorialAppSchema.Posts 
                         SET PostTitle = @PostTitle, PostContent = @PostContent, PostUpdated = @PostUpdated 
                         WHERE PostId = @PostId AND UserId = @UserId;";

        var parameters = new
        {
            PostId = postId,
            UserId = post.UserId,
            PostTitle = post.PostTitle,
            PostContent = post.PostContent,
            PostUpdated = DateTime.Now
        };

        if (_dapper.ExecuteSql(query, parameters))
        {
            return NoContent(); // Return 204 No Content on successful update
        }
        else
        {
            return NotFound(new { Message = "Post not found or you do not have permission to update this post." });
        }

    }

    [HttpDelete("DeletePost/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string userId = this.User.FindFirst("UserId")?.Value + "";
        string query = @"DELETE FROM TutorialAppSchema.Posts WHERE PostId = @PostId AND UserId = @UserId;";

        var parameters = new
        {
            PostId = postId,
            UserId = int.Parse(userId)
        };

        if (_dapper.ExecuteSql(query, parameters))
        {
            return NoContent(); // Return 204 No Content on successful deletion
        }
        else
        {
            return NotFound(new { Message = "Post not found or you do not have permission to delete this post." });
        }
    }
}