using System.Data;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs.User;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _configuration;
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
        _dapper = new DataContextDapper(_configuration);
    }

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

            byte[] passwordHash = GetPasswordHash(registerUser.Password, passwordSalt);

            string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth (Email, PasswordHash, PasswordSalt)
            VALUES (@Email, @PasswordHash, @PasswordSalt);";
            // var addAuthParameters = new DynamicParameters();
            parameters = new DynamicParameters();
            parameters.Add("@Email", registerUser.Email);
            parameters.Add("@PasswordHash", passwordHash, DbType.Binary);
            parameters.Add("@PasswordSalt", passwordSalt, DbType.Binary);
            if (_dapper.ExecuteSql(sqlAddAuth, parameters))
            {
                return Ok(new { Message = "User signed up successfully." });
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
        byte[] passwordHash = GetPasswordHash(loginUser.Password, user.PasswordSalt);

        // if (passwordHash == user.PasswordHash)
        for (int i = 0; i < passwordHash.Length; i++)
        {
            if (passwordHash[i] != user.PasswordHash[i])
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }
        }

        if (passwordHash.SequenceEqual(user.PasswordHash))
        {
            return Ok(new { Message = "Login successful." });
        }
        else
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

    }

    private byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        );
    }

}
