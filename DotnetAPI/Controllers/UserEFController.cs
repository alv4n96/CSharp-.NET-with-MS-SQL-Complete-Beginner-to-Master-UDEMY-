using System.Data;
using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs.User;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    DataContextEF _entityFramework;
    IMapper _mapper;
    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateUserDTO, User>();
            cfg.CreateMap<UpdateUserDTO, User>();
        }));
    }

    [HttpGet("test-connection")]
    public DateTime TestConnection()
    {
        using var command = _entityFramework.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT GETDATE()";
        _entityFramework.Database.OpenConnection();

        var result = command.ExecuteScalar();
        return Convert.ToDateTime(result);

    }

    [HttpGet()]
    public IActionResult GetUsers()
    {
        try
        {
            var users = _entityFramework.Users.ToList<User>();

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
            User? result = _entityFramework.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefault();

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
        // User userDb = (User)GetUsers(userId);


        User? userDb = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault();


        if (userDb != null)
        {
            userDb = _mapper.Map(user, userDb);
            
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok(userDb);
            }
            else
            {
                return StatusCode(500, "Failed to update user");
            }
        }
        else
        {
            return NotFound($"User with ID {userId} not found.");
        }
    }

    [HttpPost()]
    public IActionResult CreateUser(CreateUserDTO user)
    {
        User userDb = _mapper.Map<User>(user);

        _entityFramework.Users.Add(userDb);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok(userDb);
        }
        else
        {
            return StatusCode(500, "Failed to update user");
        }
    }


    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {

        User? userDb = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

        if (userDb != null)
        {
            _entityFramework.Users.Remove(userDb);

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok(userDb);
            }
            else
            {
                return StatusCode(500, "Failed to update user");
            }
        }
        else
        {
            return NotFound($"User with ID {userId} not found.");
        }
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