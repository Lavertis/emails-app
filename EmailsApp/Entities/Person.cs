using System.ComponentModel.DataAnnotations.Schema;

namespace EmailsApp.Entities;

[Table("Persons")]
public class Person
{
    public int Id { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    public required string FirstName { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    public required string LastName { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string? Description { get; set; }

    public ICollection<Email> Emails { get; set; }
}