namespace RentACar.Application.DTOs.UserDtos;

public sealed class UserFilterDto
{
    
    public string? Search { get; set; }        
    public string? Role { get; set; }         
    public bool? EmailConfirmed { get; set; }   
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
