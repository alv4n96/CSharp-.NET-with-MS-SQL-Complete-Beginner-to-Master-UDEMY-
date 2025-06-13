using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs.User;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;
    public AuthController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _authHelper = new AuthHelper(configuration);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(RegisterUserDTO registerUser)
    {
        if (registerUser.Password == registerUser.PasswordConfirm)
        {
            string query = @" SELECT * FROM TutorialAppSchema.Auth 
            WHERE EMAIL = @Email;";
            var parameters = new DynamicParameters();
            parameters.Add("@Email", registerUser.Email);
            // parameters.Add("@Password", registerUser.Password); // In a real application, hash the password before storing it
            // _dapper.Execute(query, parameters); 
            IEnumerable<string> existingUsers = _dapper.LoadData<string>(query, parameters);
            if (existingUsers.Any())
            {
                return BadRequest(new { Message = "User with this email already exists." });
            }

            byte[] passwordSalt = new byte[128 / 2];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            // string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(registerUser.Password, passwordSalt);

            string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth (Email, PasswordHash, PasswordSalt)
            VALUES (@Email, @PasswordHash, @PasswordSalt);";
            // var addAuthParameters = new DynamicParameters();
            parameters = new DynamicParameters();
            parameters.Add("@Email", registerUser.Email);
            parameters.Add("@PasswordHash", passwordHash, DbType.Binary);
            parameters.Add("@PasswordSalt", passwordSalt, DbType.Binary);
            if (_dapper.ExecuteSql(sqlAddAuth, parameters))
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

                parameters = new DynamicParameters();
                // parameters.Add("UserId", userId, DbType.Int32);
                parameters.Add("FirstName", registerUser.FirstName, DbType.String);
                parameters.Add("LastName", registerUser.LastName, DbType.String);
                parameters.Add("Email", registerUser.Email, DbType.String);
                parameters.Add("Gender", registerUser.Gender, DbType.String);
                parameters.Add("Active", registerUser.Active, DbType.Boolean);

                if (_dapper.ExecuteSql(sql, parameters))
                {
                    return Ok(new { Message = "User signed up successfully." });
                }
                else
                {
                    throw new Exception("Failed to create user");
                }

            }
            else
            {
                return BadRequest(new { Message = "Error occurred while signing up." });
            }

            // Here you would typically hash the password and save the user to the database
            // For simplicity, we are just returning a success message

        }
        else
        {
            return BadRequest(new { Message = "Passwords do not match." });
        }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(LoginUserDTO loginUser)
    {
        string sqlForHashandSalt = @"SELECT PasswordHash, PasswordSalt FROM TutorialAppSchema.Auth 
        WHERE Email = @Email;";
        var parameters = new DynamicParameters();
        parameters.Add("@Email", loginUser.Email);
        var user = _dapper.LoadSingleData<LoginConfirmationUserDTO>(sqlForHashandSalt, parameters);
        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }
        // string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value + user.PasswordSalt;
        byte[] passwordHash = _authHelper.GetPasswordHash(loginUser.Password, user.PasswordSalt);

        // if (passwordHash == user.PasswordHash)
        for (int i = 0; i < passwordHash.Length; i++)
        {
            if (passwordHash[i] != user.PasswordHash[i])
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }
        }

        string userIdSQL = @"SELECT UserId FROM TutorialAppSchema.Users 
        WHERE Email = @Email;";
        var userIdParameters = new DynamicParameters();
        userIdParameters.Add("@Email", loginUser.Email);
        int userId = _dapper.LoadSingleData<int>(userIdSQL, userIdParameters);

        if (passwordHash.SequenceEqual(user.PasswordHash))
        {
            return Ok(new Dictionary<string, string>
            {
                {"Token", _authHelper.CreateToken(userId) },
            });
        }
        else
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        string usedId = User.FindFirst("UserId")?.Value + "";
        System.Console.WriteLine($"Refreshing token for UserId: {usedId}");
        string userIdSQL = @"SELECT UserId FROM TutorialAppSchema.Users 
        WHERE UserId = @UserId;";
        var userIdParameters = new DynamicParameters();
        userIdParameters.Add("@UserId", usedId);
        int userId = _dapper.LoadSingleData<int>(userIdSQL, userIdParameters);

        if (userId == 0)
        {
            return Unauthorized(new { Message = "Invalid user." });
        }

        return Ok(new Dictionary<string, string>
        {
            {"Token", _authHelper.CreateToken(userId) },
        });


    }
}
