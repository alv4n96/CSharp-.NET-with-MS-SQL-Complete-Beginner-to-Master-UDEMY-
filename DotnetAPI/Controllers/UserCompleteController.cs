using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs.User;
using DotnetAPI.DTOs.UserComplete;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    public class CurrentDateTimeResult
    {
        public DateTime CurrentDateTime { get; set; }
    }

    [HttpGet("test-connection")]
    public DateTime TestConnection()
    {
        var result = _dapper.LoadSingleData<CurrentDateTimeResult>("SELECT GETDATE() AS CurrentDateTime");
        return result!.CurrentDateTime;
    }

    [HttpGet()]
    public IActionResult GetUsers()
    {
        try
        {
            string sql = @"TutorialAppSchema.spUsers_Get";

            var users = _dapper.LoadData<UserComplete>(sql);

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{userId}/{isActive}")]
    public IActionResult GetUsers(int userId, bool isActive)
    {
        try
        {
            string sql = @"TutorialAppSchema.spUsers_Get";

            var parameters = new DynamicParameters();
            if (userId != 0)
            {
                parameters.Add("UserId", userId, DbType.Int32);
            }
            if (userId == 0)
            {
                parameters.Add("Active", isActive, DbType.Boolean);
            }

            object result;
            if (userId != 0)
            {
                result = _dapper.LoadSingleData<UserComplete>(sql, parameters);
                if (result == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }
            }
            else
            {
                result = _dapper.LoadData<UserComplete>(sql, parameters);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception if needed
            if (ex.Message == "Sequence contains no elements")
            {
                return NotFound($"User with ID {userId} not found.");
            }
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{userId}")]
    public IActionResult UpdateandInsertUser(int userId, UpdateUserCompleteDTO user)
    {
        string sql = "TutorialAppSchema.spUser_Upsert";

        var parameters = new DynamicParameters();
        parameters.Add("FirstName", user.FirstName, DbType.String, size: 50);
        parameters.Add("LastName", user.LastName, DbType.String, size: 50);
        parameters.Add("Email", user.Email, DbType.String, size: 50);
        parameters.Add("Gender", user.Gender, DbType.String, size: 50);
        parameters.Add("JobTitle", user.JobTitle, DbType.String, size: 50);
        parameters.Add("Department", user.Department, DbType.String, size: 50);
        parameters.Add("Salary", user.Salary, DbType.Decimal, precision: 18, scale: 4);
        parameters.Add("Active", user.Active, DbType.Boolean);
        parameters.Add("UserId", userId, DbType.Int32); // existing user to update

        bool result = _dapper.ExecuteSql(sql, parameters, CommandType.StoredProcedure);

        if (!result)
        {
            return NotFound($"User with ID {userId} not found or update failed.");
        }

        return Ok(new { Message = "User updated successfully." });
    }

    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"TutorialAppSchema.spUser_Delete";

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId, DbType.Int32);

        bool result = _dapper.ExecuteSql(sql, parameters);
        if (!result)
        {
            return NotFound();
        }
        return Ok();
    }

}