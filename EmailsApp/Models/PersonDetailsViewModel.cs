using EmailsApp.DTOs;

namespace EmailsApp.Models;

public class PersonDetailsViewModel
{
    public required PersonDetailsDto Person { get; set; }
    public IList<EmailDto> Emails { get; set; } = new List<EmailDto>();
    public bool IsEditMode { get; set; }
}