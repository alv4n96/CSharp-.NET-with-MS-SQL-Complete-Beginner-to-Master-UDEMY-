using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
using Microsoft.IdentityModel.Tokens;

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

            byte[] passwordHash = getPasswordHash(registerUser.Password, passwordSalt);

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
        byte[] passwordHash = getPasswordHash(loginUser.Password, user.PasswordSalt);

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
                {"Token", createToken(userId) },
            });
        }
        else
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

    }

    private byte[] getPasswordHash(string password, byte[] passwordSalt)
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

    private string createToken(int userId)
    {
        Claim[] claims = new[]
        {
            new Claim("UserId", userId.ToString()),
        };

        string? tokenKey = _configuration["AppSettings:TokenKey"];
        if (string.IsNullOrEmpty(tokenKey))
        {
            throw new InvalidOperationException("TokenKey is not configured.");
        }
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);

    }
}
