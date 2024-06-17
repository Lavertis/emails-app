namespace EmailsApp.Models;

public class PersonCreateViewModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Description { get; set; }
    public required string Email { get; set; }
}
