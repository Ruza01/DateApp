using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]  // account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if(await UserExists(registerDto.Username)) return BadRequest("Usename is taken");

        using var hmac = new HMACSHA512();  //generise slucajnu vrednost (salt/kljuc), odnosno to se zove hash

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), //rastavlja lozinku na niz bajtova jer HMAC radi sa bajtovima i racuna hes sa slucajnom vrednoscu salt
            PasswordSalt = hmac.Key //pristup toj slucajnoj vrednosti (salt/kljuc), kasnije kad se korisnik loguje ovo se koristi da desifrujemo sifru i proverimo da li se poklapa sifra prijavljnog korisnika sa onim sto je u bazi 
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if(user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt); //koristimo PasswordSalt koji je smesten u bazi da bi generisali hash, i treba da bude isti, odnosno da se poklopi kada korisnik hoce da se prijavi 

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for(int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return new UserDto
        { 
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());   //Bob != bob, zato sve konvertujemo u mala slova i tako poredimo
    }
}
