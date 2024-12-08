using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiWebManagement.Token
{
    public class BypassTokenHandler
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public BypassTokenHandler(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var bypassPaths = new[] { "/api/nguoidung/login", "/api/nguoidung/register" };

            // Kiểm tra xem URL hiện tại có nằm trong danh sách bỏ qua không
            var requestPath = context.Request.Path.Value.ToLower();
            if (bypassPaths.Any(path => requestPath.StartsWith(path)))
            {
                await _next(context);
                return;
            }

            // Xác thực JWT Token
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is missing.");
                return;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out _);

                // Tiếp tục xử lý nếu token hợp lệ
                await _next(context);
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
            }
        }
    }
}

