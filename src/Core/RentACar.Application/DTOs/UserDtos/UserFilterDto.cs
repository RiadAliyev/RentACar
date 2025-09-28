namespace RentACar.Application.DTOs.UserDtos;

public sealed class UserFilterDto
{
    
    public string? Search { get; set; }          // ad və ya email üzrə axtarış
    public string? Role { get; set; }            // konkret rola görə filter
    public bool? EmailConfirmed { get; set; }    // true/false
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
