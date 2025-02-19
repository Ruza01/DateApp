using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLike(int sourceId, int targetUserId);    //vraca like
    Task<PagedList<MemberDto>> GetUsersLikes(LikesParams likesParams);   //vraca listu korisika koje je odredjeni korisnik lajkovao ili koji su lajkovali njega
    Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);    //vraca listu id-jeva korisnika koje je odredjeni korisnik lajkovao
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
    Task<bool> SaveChanges();
}
