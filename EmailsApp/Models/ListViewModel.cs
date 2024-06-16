namespace EmailsApp.Models;

public class ListViewModel<T>
{
    public required IEnumerable<T> Items { get; set; }
    public int Page { get; set; }
    public int PageCount { get; set; }
}
