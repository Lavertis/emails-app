using System.ComponentModel.DataAnnotations.Schema;

namespace EmailsApp.Entities;

[Table("Emails")]
public class Email
{
    public int Id { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    public string EmailAddress { get; set; }

    [ForeignKey(nameof(Person))]
    public int PersonId { get; set; }
    public Person Person { get; set; }
}