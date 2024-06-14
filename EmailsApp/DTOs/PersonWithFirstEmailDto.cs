namespace EmailsApp.DTOs;

public class PersonWithFirstEmailDto
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public bool? IsDescriptionTruncated { get; init; }
    public string? Description { get; init; }
    public string? EmailAddress { get; init; }
}