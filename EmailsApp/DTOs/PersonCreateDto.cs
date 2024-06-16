namespace EmailsApp.DTOs;

public class PersonCreateDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Description { get; set; }
    public required string Email { get; set; }
}