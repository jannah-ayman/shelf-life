using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YourApp.DTOs;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DBcontext _context;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration, DBcontext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // ---------------------------------------------
        //                REGISTER
        // ---------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (existingUser != null)
                return Conflict(new { message = "Email already registered" });

            // Create hash
            string passwordHash = HashPassword(dto.Password);

            // Map DTO → User
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Address = dto.Location,
                City = dto.City,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UserType = dto.UserType == "BUSINESS"
                    ? UserType.BUSINESS
                    : UserType.NORMAL_USER
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully",
                userId = user.UserID
            });
        }

        // ---------------------------------------------
        //                LOGIN
        // ---------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (loginDto == null ||
                string.IsNullOrEmpty(loginDto.Email) ||
                string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            var hashedPassword = HashPassword(loginDto.Password);

            if (user.PasswordHash != hashedPassword)
                return Unauthorized(new { message = "Invalid credentials." });

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

           
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
        new Claim("userId", user.UserID.ToString()),   
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Role, user.UserType.ToString())
    }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                message = "Login successful",
                token = tokenHandler.WriteToken(token),
                user = new
                {
                    user.UserID,
                    user.Name,
                    user.Email,
                    user.UserType
                }
            });
        }

        // ---------------------------------------------
        //          FORGOT PASSWORD (SEND EMAIL)
        // ---------------------------------------------
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Always return Ok for security
            if (user == null)
                return Ok(new { message = "If an account exists, you’ll receive a reset link." });

            // Remove old tokens
            var oldTokens = _context.PasswordResetTokens.Where(t => t.Email == dto.Email);
            _context.PasswordResetTokens.RemoveRange(oldTokens);

            // Create new token
            var token = Guid.NewGuid().ToString();
            var resetToken = new PasswordResetToken
            {
                Email = dto.Email,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var resetLink = $"http://localhost:3000/reset-password?token={token}";

            await SendEmailAsync(dto.Email, "Password Reset",
                $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return Ok(new { message = "Reset link sent if account exists." });
        }

        // ---------------------------------------------
        //              RESET PASSWORD
        // ---------------------------------------------
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var token = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == dto.Token);

            if (token == null || token.Expiration < DateTime.UtcNow)
                return BadRequest(new { message = "Invalid or expired token." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == token.Email);
            if (user == null)
                return BadRequest(new { message = "User not found." });

            // Update password
            user.PasswordHash = HashPassword(dto.NewPassword);

            // Remove used token
            _context.PasswordResetTokens.Remove(token);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successful." });
        }

        // ---------------------------------------------
        //          SEND EMAIL (GMAIL SMTP)
        // ---------------------------------------------
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            string fromEmail = "aabdula2712@gmail.com";
            string fromPassword = "eblmolyvvxfyrqef";

            var mail = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            using var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }

        // ---------------------------------------------
        //          HASH PASSWORD (SHA256)
        // ---------------------------------------------
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}
