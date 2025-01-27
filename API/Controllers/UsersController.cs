using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController(DataContext context) : BaseApiController
{
    [AllowAnonymous]    //dostupno svima, bez obzira ko je autentifikovan
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();

        return users;
    }

    [Authorize] //samo autentifikovanim korisnicima dozvoljene ove metode i onima gde ima allowAnonymus
    [HttpGet("{id}")]   // /api/users/2
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await context.Users.FindAsync(id);

        if(user == null) return NotFound();

        return user;
    }
}
