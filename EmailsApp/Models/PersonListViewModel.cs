using EmailsApp.DTOs;

namespace EmailsApp.Models;

public class PersonListViewModel
{
    public IEnumerable<PersonWithFirstEmailDto> Persons { get; set; }
    public int Page { get; set; }
    public int PageCount { get; set; }
}
