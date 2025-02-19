using System;

namespace API.Helpers;

public class UserParams : PaginationParams
{
    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; }     //ovo nam treba jer zelimo kada vrati response da iz filtera iskljucimo trenutnog korisnika
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActive";
}
