using System;

namespace API.Helpers;
//ovo koristimo unutar Extensions, tacnije (HttpExtensions)
// Prosirujemo HttpResponse, da bi smo mogli unutrar kontrolera da koristimo Response.nasaMetoda 
// cilj je da vratimo kroz hedaer u response paginaciju klijentu
public class PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
{
    public int CurrentPage { get; set; } = currentPage;
    public int ItemsPerPage { get; set; } = itemsPerPage;
    public int TotalItems { get; set; } = totalItems;
    public int TotalPages { get; set; } = totalPages;
}
