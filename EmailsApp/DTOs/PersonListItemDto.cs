namespace EmailsApp.DTOs;

public class PersonListItemDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsDescriptionTruncated { get; set; }
    public required string Description { get; set; }
    public string? EmailAddress { get; set; }
}