namespace EmailsApp.DTOs;

public class PersonUpdateDto
{
    public required string  FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Description { get; set; }
}