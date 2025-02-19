using System;

namespace API.Entities;

public class UserLike
{
    public AppUser SourceUser { get; set; } = null!;    //user koji lajkuje
    public int SourceUserId { get; set; }
    public AppUser TargetUser { get; set; } = null!;    //user koji prima like
    public int TargetUserId { get; set; }

}  