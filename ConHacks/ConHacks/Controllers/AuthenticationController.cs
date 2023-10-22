using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConHacks.Models;
using ConHacksModels.Shared;
using ConHacksModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace ConHacks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public AuthenticationController(IConfiguration configuration, ApplicationDbContext context,
        UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender) : base(configuration)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
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
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
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

        // Sends verification email
        _ = SendVerificationEmail(user.Id);

        return Ok(new { token = tokenValue });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            if (!user.EmailConfirmed)
            {
                return BadRequest("Confirm your email to log into the account.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid login attempt.");
            }

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new (JwtRegisteredClaimNames.Email, user.Email!),
                new (JwtRegisteredClaimNames.Jti, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: exp,
                signingCredentials: creds
            );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenModel = new Token
            {
                UserId = Guid.Parse(user.Id),
                TokenValue = tokenValue,
                Expiration = exp
            };

            // TODO: store the JWT in Redis cache

            return Ok(new
            {
                token = tokenValue,
                expiration = exp
            });
        }

        return Unauthorized();
    }
    [HttpPost("SendConfirmation/{uid}")]
    public async Task<IActionResult> SendVerificationEmail(string uid)
    {
        var user = await _userManager.FindByIdAsync(uid);

        if (user == null)
        {
            return NotFound();
        }

        var email = await _userManager.GetEmailAsync(user);

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

        var verificationUrl = Url.Action("ConfirmEmail", "Authentication", new { userId = uid, token = confirmationToken }, Request.Scheme);

        
        await _emailSender.SendEmailAsync(email!, "Confirm your email", $"<!DOCTYPE html>"+
$"<html>"+
$"<head>"+

$"  <meta charset='utf-8'>"+
$"  <meta http-equiv='x-ua-compatible' content='ie=edge'>"+
$"  <title>Email Confirmation</title>"+
$"  <meta name='viewport' content='width=device-width, initial-scale=1'>"+
$"  <style type='text/css'>"+
$"  /**"+
$"   * Google webfonts. Recommended to include the .woff version for cross-client compatibility."+
$"   */"+
"  @media screen {"+
"    @font-face {"+
$"      font-family: 'Source Sans Pro';"+
$"      font-style: normal;"+
$"      font-weight: 400;"+
$"      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');"+
"    }"+
"    @font-face {"+
$"      font-family: 'Source Sans Pro';"+
$"      font-style: normal;"+
$"      font-weight: 700;"+
$"      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');"+
"    }"+
"  }"+
$"  /**"+
$"   * Avoid browser level font resizing."+
$"   * 1. Windows Mobile"+
$"   * 2. iOS / OSX"+
$"   */"+
$"  body,"+
$"  table,"+
$"  td,"+
"  a {"+
$"    -ms-text-size-adjust: 100%; /* 1 */"+
$"    -webkit-text-size-adjust: 100%; /* 2 */"+
"  }"+
$"  /**"+
$"   * Remove extra space added to tables and cells in Outlook."+
$"   */"+
$"  table,"+
"  td {"+
$"    mso-table-rspace: 0pt;"+
$"    mso-table-lspace: 0pt;"+
"  }"+
$"  /**"+
$"   * Better fluid images in Internet Explorer."+
$"   */"+
"  img {"+
$"    -ms-interpolation-mode: bicubic;"+
"  }"+
$"  /**"+
$"   * Remove blue links for iOS devices."+
$"   */"+
"  a[x-apple-data-detectors] {"+
$"    font-family: inherit !important;"+
$"    font-size: inherit !important;"+
$"    font-weight: inherit !important;"+
$"    line-height: inherit !important;"+
$"    color: inherit !important;"+
$"    text-decoration: none !important;"+
"  }"+
$"  /**"+
$"   * Fix centering issues in Android 4.4."+
$"   */"+
"  div[style*='margin: 16px 0;'] {"+
$"    margin: 0 !important;"+
"  }"+
"  body {"+
$"    width: 100% !important;"+
$"    height: 100% !important;"+
$"    padding: 0 !important;"+
$"    margin: 0 !important;"+
"  }"+
$"  /**"+
$"   * Collapse table borders to avoid space between cells."+
$"   */"+
"  table {"+
$"    border-collapse: collapse !important;"+
"  }"+
"  a {"+
$"    color: #1a82e2;"+
"  }"+
"  img {"+
$"    height: auto;"+
$"    line-height: 100%;"+
$"    text-decoration: none;"+
$"    border: 0;"+
$"    outline: none;"+
"  }"+
$"  </style>"+

$"</head>"+
$"<body style='background-color: #e9ecef;'>"+

$"  <!-- start preheader -->"+
$"  <div class='preheader' style='display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;'>"+
$"    A preheader is the short summary text that follows the subject line when an email is viewed in the inbox."+
$"  </div>"+
$"  <!-- end preheader -->"+

$"  <!-- start body -->"+
$"  <table border='0' cellpadding='0' cellspacing='0' width='100%'>"+

$"    <!-- start logo -->"+
$"    <tr>"+
$"      <td align='center' bgcolor='#e9ecef'>"+
$"        <!--[if (gte mso 9)|(IE)]>"+
$"        <table align='center' border='0' cellpadding='0' cellspacing='0' width='600'>"+
$"        <tr>"+
$"        <td align='center' valign='top' width='600'>"+
$"        <![endif]-->"+
$"        <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px;'>"+
$"          <tr>"+
$"            <td align='center' valign='top' style='padding: 36px 24px;'>"+
$"              <a href='https://www.blogdesire.com' target='_blank' style='display: inline-block;'>"+
$"                <img src='https://www.blogdesire.com/wp-content/uploads/2019/07/blogdesire-1.png' alt='Logo' border='0' width='48' style='display: block; width: 48px; max-width: 48px; min-width: 48px;'>"+
$"              </a>"+
$"            </td>"+
$"          </tr>"+
$"        </table>"+
$"        <!--[if (gte mso 9)|(IE)]>"+
$"        </td>"+
$"        </tr>"+
$"        </table>"+
$"        <![endif]-->"+
$"      </td>"+
$"    </tr>"+
$"    <!-- end logo -->"+

$"    <!-- start hero -->"+
$"    <tr>"+
$"      <td align='center' bgcolor='#e9ecef'>"+
$"        <!--[if (gte mso 9)|(IE)]>"+
$"        <table align='center' border='0' cellpadding='0' cellspacing='0' width='600'>"+
$"        <tr>"+
$"        <td align='center' valign='top' width='600'>"+
$"        <![endif]-->"+
$"        <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px;'>"+
$"          <tr>"+
$"            <td align='left' bgcolor='#ffffff' style='padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;'>"+
$"              <h1 style='margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;'>Confirm Your Email Address</h1>"+
$"            </td>"+
$"          </tr>"+
$"        </table>"+
$"        <!--[if (gte mso 9)|(IE)]>"+
$"        </td>"+
$"        </tr>"+
$"        </table>"+
$"        <![endif]-->"+
$"      </td>"+
$"    </tr>"+
$"    <!-- end hero -->"+

$"    <!-- start copy block -->"+
$"    <tr>"+
$"      <td align='center' bgcolor='#e9ecef'>"+
$"        <!--[if (gte mso 9)|(IE)]>"+
$"        <table align='center' border='0' cellpadding='0' cellspacing='0' width='600'>"+
$"        <tr>"+
$"        <td align='center' valign='top' width='600'>"+
$"        <![endif]-->"+
$"        <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px;'>"+

$"          <!-- start copy -->"+
$"          <tr>"+
$"            <td align='left' bgcolor='#ffffff' style='padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;'>"+
$"              <p style='margin: 0;'>Tap the button below to confirm your email address.</p>"+
$"            </td>"+
$"          </tr>"+
$"          <!-- end copy -->"+

$"          <!-- start button -->"+
$"          <tr>"+
$"            <td align='left' bgcolor='#ffffff'>"+
$"              <table border='0' cellpadding='0' cellspacing='0' width='100%'>"+
$"                <tr>"+
$"                  <td align='center' bgcolor='#ffffff' style='padding: 12px;'>"+
$"                    <table border='0' cellpadding='0' cellspacing='0'>"+
$"                      <tr>"+
$"                        <td align='center' bgcolor='#1a82e2' style='border-radius: 6px;'>"+
$"                          <a href='{verificationUrl}' target='_blank' style='text-decoration:none; display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #FFFFFF; text-decoration: none; border-radius: 6px;'>Verify Email</a>"+
$"                        </td>"+
$"                      </tr>"+
$"                    </table>"+
$"                  </td>"+
$"                </tr>"+
$"              </table>"+
$"            </td>"+
$"          </tr>"+
$"          <!-- end button -->"+

$"          <!-- start copy -->"+
$"          <tr>"+
$"            <td align='left' bgcolor='#ffffff' style='padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;'>"+
$"              <p style='margin: 0;'>If that doesn't work, copy and paste the following link in your browser:</p>"+
$"              <p style='margin: 0;'><a href='{verificationUrl}' target='_blank'>https://comiccrazewebapi20230307210253.azurewebsites.net/api/User/Confirm</a></p>" +
$"            </td>"+
$"          </tr>"+
$"          <!-- end copy -->"+

$"          <!-- start copy -->"+
$"          <tr>"+
$"            <td align='left' bgcolor='#ffffff' style='padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf'>"+
$"              <p style='margin: 0;'>Cheers,<br> Paste</p>"+
$"            </td>"+
$"          </tr>"+
$"          <!-- end copy -->"+

$"        </table>"+
$"        <!--[if (gte mso 9)|(IE)]>"+
$"        </td>"+
$"        </tr>"+
$"        </table>"+
$"        <![endif]-->"+
$"      </td>"+
$"    </tr>"+
$"    <!-- end copy block -->"+

$"    <!-- start footer -->"+
$"    <tr>"+
$"      <td align='center' bgcolor='#e9ecef' style='padding: 24px;'>"+
$"        <!--[if (gte mso 9)|(IE)]>"+
$"        <table align='center' border='0' cellpadding='0' cellspacing='0' width='600'>"+
$"        <tr>"+
$"        <td align='center' valign='top' width='600'>"+
$"        <![endif]-->"+
$"        <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px;'>"+

$"          <!-- start permission -->"+
$"          <tr>"+
$"            <td align='center' bgcolor='#e9ecef' style='padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;'>"+
$"              <p style='margin: 0;'>You received this email because we received a request for [type_of_action] for your account. If you didn't request [type_of_action] you can safely delete this email.</p>"+
$"            </td>"+
$"          </tr>"+
$"          <!-- end permission -->"+

$"        </table>"+
$"        <!--[if (gte mso 9)|(IE)]>"+
$"        </td>"+
$"        </tr>"+
$"        </table>"+
$"        <![endif]-->"+
$"      </td>"+
$"    </tr>"+
$"    <!-- end footer -->"+

$"  </table>"+
$"  <!-- end body -->"+

$"</body>"+
$"</html>");

        return Ok(verificationUrl);
    }
    [HttpGet("Confirm/{userId}/{token}")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
}