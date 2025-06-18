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

        string query = @"TutorialAppSchema.spPosts_Get";
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId, DbType.Int32);

        var posts = _dapper.LoadData<Post>(query, parameters);

        if (posts == null || !posts.Any())
        {
            return NotFound(new { Message = "No posts found for this user." });
        }

        return Ok(posts);
    }

    [HttpPut("UpdatePost")]
    public IActionResult UpdatePost(UpdatePostDTO post)
    {
        if (post == null || string.IsNullOrEmpty(post.PostTitle) || string.IsNullOrEmpty(post.PostContent))
        {
            return BadRequest(new { Message = "Invalid post data." });
        }

        string userId = this.User.FindFirst("UserId")?.Value + "";
        post.UserId = int.Parse(userId);
        // post.PostUpdated = DateTime.Now;

        string query = @"TutorialAppSchema.spPosts_Upsert";


        var parameters = new DynamicParameters();

        parameters.Add("PostId", post.PostId, DbType.Int32);
        parameters.Add("UserId", userId, DbType.Int32);
        parameters.Add("PostTitle", post.PostTitle, DbType.String);
        parameters.Add("PostContent", post.PostContent, DbType.String);
        // parameters.Add("PostUpdated", post.PostUpdated, DbType.Int32);


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
        string userIdString = this.User.FindFirst("UserId")?.Value + "";
        int userId = !string.IsNullOrEmpty(userIdString) ? int.Parse(userIdString) : 0;
        string query = @"TutorialAppSchema.spPost_Delete";

        var parameters = new DynamicParameters();
        parameters.Add("PostId", postId, DbType.Int32);
        parameters.Add("UserId", userId, DbType.Int32);

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