using System;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber;
        TotalPages = (int) Math.Ceiling(count / (double)pageSize);  //ako je count = 12 i zelimo 5 stavke po stranici, 12 / 5 je 2.nesto, ceiling,zaokruzuje to na 3)
        PageSize = pageSize;
        TotalCount = count;
        AddRange(items);
    }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();  //vraca npr 12 korisnika iz baze
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items,count,pageNumber,pageSize);
    }
}

