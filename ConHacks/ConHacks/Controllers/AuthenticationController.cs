using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConHacks.Models;
using ConHacksModels.Shared;
using ConHacksModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ConHacks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthenticationController(IConfiguration configuration, ApplicationDbContext context,
        UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : base(configuration)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model) 
    {
        var usernameExists = await _userManager.FindByNameAsync(model.Username);
        var userEmailExists = await _userManager.FindByEmailAsync(model.Email);

        if (usernameExists != null || userEmailExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new UserResponse { Status = "Error", Message = "User already exists!" });

        var user = new IdentityUser{
            UserName = model.Username,
            Email = model.Email
        };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, model.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(7);
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        var tokenModel = new Token
        {
            UserId = Guid.Parse(user.Id),
            TokenValue = tokenValue,
            Expiration = expiration
        };

        return Ok(new { token = tokenValue });
    }
}