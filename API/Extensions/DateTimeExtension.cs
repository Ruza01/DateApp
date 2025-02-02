using System;

namespace API.Extensions;

public static class DateTimeExtension
{
    public static int CalculateAge(this DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        var age = today.Year - dob.Year;

        if(dob > today.AddYears(-age)) age--;   //proverava da li je imao vec rodj ove god, ako jeste, smanjuje se jedna god

        return age;
    }
}
