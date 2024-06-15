namespace EmailsApp.DTOs;

public class PersonWithEmailsDto
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Description { get; init; }
    public required ICollection<EmailDto> Emails { get; init; }
}