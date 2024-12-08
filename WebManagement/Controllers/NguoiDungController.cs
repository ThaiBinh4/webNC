using ApiWebManagement.DTO;
using ApiWebManagement.Service;
using APIwebmoi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiWebManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NguoiDungController : Controller
    {
        private readonly UserService userService;
        private readonly WebNangCaoQlda2Context dbContext;
        private readonly IConfiguration configuration;

        public NguoiDungController(WebNangCaoQlda2Context dbContext, IConfiguration configuration, UserService userService)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.userService = userService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return Ok(new { Message = "API is working!" });
        }


        [HttpGet("Profile")]
        [AllowAnonymous]
        public async Task<IActionResult> Profile(string sdt)
        {
            var user = await dbContext.NguoiDungs.FirstOrDefaultAsync(x => x.IdUser == sdt);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(new
            {
                user.IdUser,
                user.Email,
                user.Username,
                user.Avatar,
                user.Fullname,
                user.IdRole,
                user.IsActive
            });
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Message = "Invalid input data." });

                var user = await dbContext.NguoiDungs.FirstOrDefaultAsync(x => x.IdUser == loginDto.IdUser);
                if (user == null)
                    return Unauthorized(new { Message = "Invalid username or password." });

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Pass, user.Pass);
                if (!isPasswordValid)
                    return Unauthorized(new { Message = "Invalid username or password." });

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    Token = token,
                    User = user
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết để kiểm tra
                Console.WriteLine("Login API Error: " + ex.Message);
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        private string GenerateJwtToken(NguoiDung user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.IdUser),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("IdUser", user.IdUser.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine($"Generated Token: {encodedToken}");

            return encodedToken;
       
          
        }
          //var claims = new[]
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //};

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            //var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //// Lấy thời gian hết hạn từ cấu hình
            //var expirationDays = int.Parse(configuration["Jwt:ExpirationDays"]);

            //var token = new JwtSecurityToken(
            //    issuer: configuration["Jwt:Issuer"],
            //    audience: configuration["Jwt:Audience"],
            //    claims: claims,
            //    expires: DateTime.UtcNow.AddDays(expirationDays),
            //    signingCredentials: signIn
            //);

            //return new JwtSecurityTokenHandler().WriteToken(token);


        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public IActionResult Register(NguoiDungDTO nguoiDungDTO)
        {
            // Kiểm tra nếu người dùng đã tồn tại
            var existingUser = dbContext.NguoiDungs
                .FirstOrDefault(x => x.IdUser == nguoiDungDTO.IdUser);

            if (existingUser != null)
            {
                return BadRequest(new { Message = "User already exists with the given IdUser or Email." });
            }

            var hashedPassword = userService.HashPassword(nguoiDungDTO.Pass);

            var newUser = new NguoiDung
            {
                IdUser = nguoiDungDTO.IdUser,
                Email = nguoiDungDTO.Email,
                Pass = hashedPassword,
                Username = nguoiDungDTO.Username,
                Avatar = nguoiDungDTO.Avatar,
                IdRole = nguoiDungDTO.IdRole,
                IsActive = nguoiDungDTO.IsActive ?? true,
                Fullname = nguoiDungDTO.Fullname,
            };

            try
            {
                // Lưu vào cơ sở dữ liệu
                dbContext.NguoiDungs.Add(newUser);
                dbContext.SaveChanges();

                return Ok(new { Message = "Registration successful.", NguoiDung = newUser });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, new { Message = "An error occurred while registering the user.", Error = ex.Message });
            }
        }

        [HttpPut]
        //[AllowAnonymous]
        [Authorize]
        [Route("Update/IdUser")]
        public async Task<IActionResult> UpdateAsync(string IdUser, [FromBody] NguoiDungDTO nguoiDungDTO)
        {
            if (string.IsNullOrWhiteSpace(IdUser) || string.IsNullOrWhiteSpace(nguoiDungDTO.IdUser))
            {
                return BadRequest("Invalid IdUser.");
            }

            if (IdUser != nguoiDungDTO.IdUser)
            {
                return BadRequest("IdUser mismatch.");
            }

            var nguoiDung = await dbContext.NguoiDungs.FindAsync(IdUser);
            if (nguoiDung == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Cập nhật các trường hợp lệ
            nguoiDung.Email = !string.IsNullOrWhiteSpace(nguoiDungDTO.Email) ? nguoiDungDTO.Email : nguoiDung.Email;
            nguoiDung.Pass = !string.IsNullOrWhiteSpace(nguoiDungDTO.Pass) ? userService.HashPassword(nguoiDungDTO.Pass) : nguoiDung.Pass;
            nguoiDung.Username = !string.IsNullOrWhiteSpace(nguoiDungDTO.Username) ? nguoiDungDTO.Username : nguoiDung.Username;
            nguoiDung.Avatar = !string.IsNullOrWhiteSpace(nguoiDungDTO.Avatar) ? nguoiDungDTO.Avatar : nguoiDung.Avatar;
            nguoiDung.IdRole = nguoiDungDTO.IdRole ?? nguoiDung.IdRole;
            nguoiDung.IsActive = nguoiDungDTO.IsActive ?? nguoiDung.IsActive;
            nguoiDung.Fullname = !string.IsNullOrWhiteSpace(nguoiDungDTO.Fullname) ? nguoiDungDTO.Fullname : nguoiDung.Fullname;

            try
            {
                await dbContext.SaveChangesAsync();
                return Ok(new { Message = "Cập nhật thành công.", User = nguoiDung });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi trong quá trình lưu: {ex.Message}");
            }
        }

    }
}
