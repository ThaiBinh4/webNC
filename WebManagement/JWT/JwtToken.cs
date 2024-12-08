using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using APIwebmoi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace ApiWebManagement.JWT
{
    public class JwtToken
    {
        //private string SecretKey { get; set; }
        //private int Expiration { get; set; }

        //public JwtToken(IConfiguration configuration)
        //{
        //    // Lấy giá trị từ cấu hình
        //    SecretKey = configuration["Jwt:SecretKey"];
        //    Expiration = int.Parse(configuration["Jwt:Expiration"]);
        //}

        //public string GenerateToken(NguoiDung nguoiDung)
        //{
        //    // Khởi tạo claims
        //    Dictionary<string, object> claims = new Dictionary<string, object>
        //{
        //    { "roles", new List<string> { "ROLE_" + nguoiDung.IdRoleNavigation.RoleName } }
        //};

        //    // Tạo token
        //    try
        //    {
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(new[]
        //            {
        //            new Claim(ClaimTypes.Name, nguoiDung.IdUser),
        //        }),
        //            Claims = claims,
        //            Expires = DateTime.UtcNow.AddSeconds(Expiration),
        //            SigningCredentials = creds
        //        };

        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var token = tokenHandler.CreateToken(tokenDescriptor);

        //        return tokenHandler.WriteToken(token);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Cannot generate JWT token: " + ex.Message);
        //    }
        //}

        //public ClaimsPrincipal ValidateToken(string token)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(SecretKey);

        //    try
        //    {
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            ClockSkew = TimeSpan.Zero
        //        };

        //        var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
        //        return principal;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public bool IsTokenExpired(string token)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        //    return jwtToken?.ValidTo < DateTime.UtcNow;
        //}

        //public string GenerateSecretKey()
        //{
        //    var key = new byte[32];
        //    using (var random = RandomNumberGenerator.Create())
        //    {
        //        random.GetBytes(key);
        //    }

        //    return Convert.ToBase64String(key);
        //}
    }
}
