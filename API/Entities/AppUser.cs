using System;

namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public required int UserName { get; set; }
}
