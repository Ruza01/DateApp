using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.Name);  //uzimanje username-a iz tokena

        if (username == null) throw new Exception("Cannot get username from token");

        return username;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);  
        if (userId != 0) return userId;
        
        throw new Exception("Cannot get userId from token");
    }
}
