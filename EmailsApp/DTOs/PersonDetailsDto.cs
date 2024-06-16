using System.ComponentModel.DataAnnotations;

namespace EmailsApp.DTOs;

public class PersonDetailsDto
{
    public int Id { get; init; }
    
    [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters")]
    public required string FirstName { get; init; }
    
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters")]
    public required string LastName { get; init; }

    [MaxLength(10_000, ErrorMessage = "Description must be less than 10,000 characters")]
    public string? Description { get; init; }
}