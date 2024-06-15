namespace EmailsApp.DTOs;

public class AddEmailRequest
{
    public int PersonId { get; set; }
    public required string EmailAddress { get; set; }
}