using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs.User;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
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
            string sql = @"SELECT 
        [UserId]
        , [FirstName]
        , [LastName]
        , [Email]
        , [Gender]
        , [Active]
        FROM TutorialAppSchema.Users";

            var users = _dapper.LoadData<User>(sql);

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

    [HttpGet("{userId}")]
    public IActionResult GetUsers(int userId)
    {
        try
        {
            string sql = @"SELECT
            [UserId]
            , [FirstName]
            , [LastName]
            , [Email]
            , [Gender]
            , [Active]
            FROM TutorialAppSchema.Users
            WHERE UserId = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Int32);

            var result = _dapper.LoadSingleData<User>(sql, parameters);

            if (result == null)
            {
                return NotFound($"User with ID {userId} not found.");
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
    public IActionResult UpdateUser(int userId, UpdateUserDTO user)
    {
        string sql = @"UPDATE TutorialAppSchema.Users
        SET [FirstName] = @FirstName,
        [LastName] = @LastName,
        [Email] = @Email,
        [Gender] = @Gender,
        [Active] = @Active
        WHERE UserId = @UserId;";

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId, DbType.Int32);
        parameters.Add("FirstName", user.FirstName, DbType.String);
        parameters.Add("LastName", user.LastName, DbType.String);
        parameters.Add("Email", user.Email, DbType.String);
        parameters.Add("Gender", user.Gender, DbType.String);
        parameters.Add("Active", user.Active, DbType.Boolean);

        bool result = _dapper.ExecuteSql(sql, parameters);
        if (!result)
        {
            return NotFound();
        }
        return Ok();
    }

    [HttpPost()]
    public IActionResult CreateUser(CreateUserDTO user)
    {
        string sql = @"
        INSERT INTO TutorialAppSchema.Users
            (
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active]
            )
        VALUES
            (
            @FirstName
            ,@LastName
            ,@Email
            ,@Gender
            ,@Active
                )";

        var parameters = new DynamicParameters();
        // parameters.Add("UserId", userId, DbType.Int32);
        parameters.Add("FirstName", user.FirstName, DbType.String);
        parameters.Add("LastName", user.LastName, DbType.String);
        parameters.Add("Email", user.Email, DbType.String);
        parameters.Add("Gender", user.Gender, DbType.String);
        parameters.Add("Active", user.Active, DbType.Boolean);

        if (_dapper.ExecuteSql(sql, parameters))
        {
            return Ok();
            // return CreatedAtAction(nameof(GetUsers), new { userId = userId }, user);
        }
        else
        {
            throw new Exception("Failed to create user");
        }

    }


    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.Users WHERE UserId = @UserId;";

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId, DbType.Int32);

        bool result = _dapper.ExecuteSql(sql, parameters);
        if (!result)
        {
            return NotFound();
        }
        return Ok();
    }


    [HttpGet("test-param")]
    public string[] TestParam(string text)
    {
        string[] responseArray;
        if (string.IsNullOrEmpty(text))
        {
            responseArray = new string[]
            {
                "Hello",
                "World",
                "This",
                "Is",
                "A",
                "Test"
            };
        }
        else
        {
            responseArray = text.Split(' ');
            foreach (var item in responseArray)
            {
                Console.WriteLine(item);
            }
        }
        return responseArray;
    }

    [HttpGet("GetUser/{text}")]
    public string[] GetUser(string text)
    {
        string[] responseArray;
        if (string.IsNullOrEmpty(text))
        {
            responseArray = new string[]
            {
                "Hello",
                "World",
                "This",
                "Is",
                "A",
                "Test"
            };
        }
        else
        {
            responseArray = text.Split(' ');
            // List<string> tempList = responseArray.ToList();
            // tempList.Add("value : " + text);
            // responseArray = tempList.ToArray();

            responseArray = responseArray.Append("value : " + text).ToArray();
            // responseArray.Append("value : " + text);
            foreach (var item in responseArray)
            {
                Console.WriteLine(item);
            }
        }
        return responseArray;
    }


}