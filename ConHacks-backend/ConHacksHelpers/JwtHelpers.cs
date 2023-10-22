using System.IdentityModel.Tokens.Jwt;

namespace ConHacksHelpers;

public static class JwtHelpers
{
    public static string GetUserEmailFromToken(string tokenValue)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(tokenValue) as JwtSecurityToken;
        
        if (token is null)
            return String.Empty;
        
        return token.Claims.First(claim => claim.Type == "email").Value;
    }

    public static string GetUserIdFromToken(string tokenValue)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(tokenValue) as JwtSecurityToken;

        if (token is null)
            return string.Empty;

        return token.Claims.First(claim => claim.Type == "nameid").Value;
    }

    public static bool IsTokenValid(string tokenValue)
    {
        JwtSecurityToken jwtSecurityToken;
        try
        {
            jwtSecurityToken = new JwtSecurityToken(tokenValue);
        }
        catch (Exception)
        {
            return false;
        }
    
        return jwtSecurityToken.ValidTo > DateTime.UtcNow;
    }
}