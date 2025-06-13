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

    [HttpGet("Posts")]
    public IActionResult GetPosts()
    {
        string query = @"SELECT 
        PostId
        ,UserId 
        ,PostTitle 
        ,PostContent 
        ,PostCreated 
        ,PostUpdated 
        FROM TutorialAppSchema.Posts;";
        // Note: In a real application, you might want to use a more secure way to handle the query, such as parameterized queries.
        var posts = _dapper.LoadData<Post>(query);
        return Ok(posts);
    }

    [HttpGet("Post/{postId}")]
    public IActionResult GetPost(string postId)
    {
        string query = @"SELECT 
        PostId
        ,UserId 
        ,PostTitle 
        ,PostContent 
        ,PostCreated 
        ,PostUpdated 
        FROM TutorialAppSchema.Posts WHERE PostId = @PostId;";
        var parameters = new { PostId = postId };
        var post = _dapper.LoadData<Post>(query, parameters).FirstOrDefault();

        if (post == null)
        {
            return NotFound(new { Message = "Post not found." });
        }

        return Ok(post);
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
        post.PostCreated = DateTime.Now;
        post.PostUpdated = DateTime.Now;

        string query = @"INSERT INTO TutorialAppSchema.Posts (UserId, PostTitle, PostContent, PostCreated, PostUpdated) 
                         VALUES (@UserId, @PostTitle, @PostContent, @PostCreated, @PostUpdated);";

        var parameters = new
        {
            UserId = post.UserId,
            PostTitle = post.PostTitle,
            PostContent = post.PostContent,
            PostCreated = post.PostCreated,
            PostUpdated = post.PostUpdated
        };
        // Execute the insert query
        _dapper.ExecuteSql(query, parameters);

        // Optionally, you can return the created post or a success message
        // For example, you can return the created post with its ID
        // Note: If you want to return the created post with its ID, you might need to modify the query to return the inserted ID.
        string selectQuery = @"SELECT * FROM TutorialAppSchema.Posts WHERE PostId = SCOPE_IDENTITY();";
        var createdPost = _dapper.LoadSingleData<Post>(selectQuery, new { UserId = post.UserId });
        if (createdPost == null)
        {
            return StatusCode(500, new { Message = "An error occurred while creating the post." });
        }
        return CreatedAtAction(nameof(GetPost), new { postId = createdPost.PostId }, createdPost);
        // var result = _dapper.LoadData(query, post);

        // return CreatedAtAction(nameof(GetPost), new { postId = post.PostId }, post);
    }

}