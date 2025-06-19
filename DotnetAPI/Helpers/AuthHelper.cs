using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs.User;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers;

public class AuthHelper
{
    private readonly IConfiguration _configuration;
    private readonly DataContextDapper _dapper;
    public AuthHelper(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _configuration = configuration;
    }

    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
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

    public string CreateToken(int userId)
    {
        Claim[] claims = new[]
        {
            new Claim("UserId", userId.ToString()),
        };

        System.Console.WriteLine($"Creating token for UserId: {userId}");
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


    public bool SetPassword(LoginUserDTO userForSetPassword)
    {

        byte[] passwordSalt = new byte[128 / 8];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

        string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert
                @Email = @EmailParam, 
                @PasswordHash = @PasswordHashParam, 
                @PasswordSalt = @PasswordSaltParam";

        DynamicParameters sqlParameters = new DynamicParameters();

        // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
        // emailParameter.Value = userForLogin.Email;
        // sqlParameters.Add(emailParameter);

        sqlParameters.Add("@EmailParam", userForSetPassword.Email, DbType.String);
        sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
        sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

        return _dapper.ExecuteSql(sqlAddAuth, sqlParameters);
    }

}