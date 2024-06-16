namespace EmailsApp.Models
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public required string ActionName { get; set; }
        public required string ControllerName { get; set; }
    }
}