using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers;

public class AuthHelper
{
    private readonly IConfiguration _configuration;
    public AuthHelper(IConfiguration configuration)
    {
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

}