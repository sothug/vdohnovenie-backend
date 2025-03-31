using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Database;
using backend.Database.Models;
using backend.Models;
using backend.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SportClubContext _context;

    public AuthController(IConfiguration configuration, SportClubContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => 
                u.Username == login.Username
                || u.Email == login.Username
            );

        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            return Unauthorized("Неверные учетные данные");

        var token = GenerateJwtToken(user);
        return Ok(new { User = user, Token = token });
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel register)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Проверка уникальности Username и Email
        if (await _context.Users.AnyAsync(u => u.Username == register.Username))
            return BadRequest("Пользователь с таким Username уже существует");

        if (await _context.Users.AnyAsync(u => u.Email == register.Email))
            return BadRequest("Пользователь с таким Email уже существует");

        if (new string[] { "Parent", "Child" }.Contains(register.Role))
            return BadRequest("Можно зарегистрировать только родителя и ребенка");

        var user = new User
        {
            Username = register.Username,
            Email = register.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.Password),
            Role = register.Role,
            FullName = register.FullName,
            DateOfBirth = register.DateOfBirth?.ToUniversalTime(),
            ParentId = register.ParentId
        };

        // Валидация через IValidatableObject
        var validationResults = user.Validate(new ValidationContext(user)).ToList();
        if (validationResults.Any())
        {
            foreach (var result in validationResults)
                ModelState.AddModelError(result.MemberNames.FirstOrDefault() ?? "", result.ErrorMessage);
            return BadRequest(ModelState);
        }

        // Проверка ParentId
        if (user.ParentId.HasValue && !await _context.Users.AnyAsync(u => u.Id == user.ParentId && u.Role == "Parent"))
            return BadRequest("Указанный родитель не существует или не является родителем");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}