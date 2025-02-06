using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]     //samo autentifikovanim korisnicima dozvoljene ove metode i onima gde ima allowAnonymus
public class UsersController(
        IUserRepository userRepository,
        IMapper mapper,
        IPhotoService photoService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();

        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username);

        if(user == null) return NotFound();

        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;    //uzimanje username-a iz tokena

        if (username == null) return BadRequest("No username found in token");

        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) return BadRequest("Could not find user");

        mapper.Map(memberUpdateDto, user);

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;    

        if (username == null) return BadRequest("No username found in token");

        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) return BadRequest("Cannot find user");

        var result = await photoService.AddPhotoAsync(file);    //ovaj result je cloudinary response (URL i publicID)

        if (result.Error != null) return BadRequest(result.Error.Message);  //ako je sve kako treba, slika je smestena na cloudinary

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        user.Photos.Add(photo);

        if (await userRepository.SaveAllAsync()) return mapper.Map<PhotoDto>(photo);    //ako je uspesno, mapiramo sliku nasu na dto

        return BadRequest("Problem adding photo");     

    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;    

        if (username == null) return BadRequest("No username found in token");

        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) return BadRequest("Cannot find user");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("Cannot use this photo as main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);    //pronalazi trnutnu glavnu foto

        if (currentMain != null) currentMain.IsMain = false;            //postavlja je na false

        photo.IsMain = true;        //postavlja novu foto kao main

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem setting main photo");

    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;    

        if (username == null) return BadRequest("No username found in token");

        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) return BadRequest("Cannot find user");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("This photo is main, you cannot delete this photo");

        if (photo.PublicId != null) //brisemo sliku sa Cloudinary-ja
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);  //brisemo sliku iz baze

        if (await userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting photo");
    }

}
