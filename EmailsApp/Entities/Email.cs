using System.ComponentModel.DataAnnotations.Schema;

namespace EmailsApp.Entities;

[Table("Emails")]
public class Email : BaseEntity
{
    [Column(TypeName = "nvarchar(254)")]
    public required string EmailAddress { get; set; }

    [ForeignKey(nameof(Person))]
    public int PersonId { get; set; }
    public Person Person { get; set; }
}