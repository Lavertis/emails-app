using System.ComponentModel.DataAnnotations.Schema;

namespace EmailsApp.Entities;

[Table("Persons")]
public class Person
{
    public int Id { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    public string Imie { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    public string Nazwisko { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string Opis { get; set; }

    public ICollection<Email> Emails { get; set; }
}