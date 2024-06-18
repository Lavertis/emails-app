using EmailsApp.DTOs;

namespace EmailsApp.Models;

public class PersonDetailsViewModel
{
    public required PersonDetailsDto Person { get; set; }
    public bool IsEditMode { get; set; }
}