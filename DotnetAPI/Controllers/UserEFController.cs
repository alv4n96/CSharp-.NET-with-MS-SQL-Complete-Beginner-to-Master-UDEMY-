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
    // DataContextEF _entityFramework;
    IMapper _mapper;
    IUserRepository _userRepository;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        // _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateUserDTO, User>();
                cfg.CreateMap<UpdateUserDTO, User>();
            }));
    }

    [HttpGet("test-connection")]
    public DateTime TestConnection()
    {
        return _userRepository.TestConnection();
    }

    [HttpGet()]
    public IActionResult GetUsers()
    {
        try
        {
            var users = _userRepository.GetUsers();

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
            User? result = _userRepository.GetUserById(userId);

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
        User? userDb = _userRepository.GetUserById(userId);
        if (userDb == null)
        {
            return NotFound($"User with ID {userId} not found.");
        }
        userDb = _mapper.Map(user, userDb);
        // _userRepository.UpdateEntity(userDb);
        if (_userRepository.SaveChanges())
        {
            return Ok(userDb);
        }
        else
        {
            return StatusCode(500, "Failed to update user");
        }
    }

    [HttpPost()]
    public IActionResult CreateUser(CreateUserDTO user)
    {
        User userDb = _mapper.Map<User>(user);

        _userRepository.AddEntity<User>(userDb);

        if (_userRepository.SaveChanges())
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
        User? userDb = _userRepository.GetUserById(userId);
        if (userDb != null)
        {
            _userRepository.DeleteEntity<User>(userDb);

            if (_userRepository.SaveChanges())
            {
                return Ok($"User with ID {userId} deleted successfully.");
            }
            else
            {
                return StatusCode(500, "Failed to delete user");
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