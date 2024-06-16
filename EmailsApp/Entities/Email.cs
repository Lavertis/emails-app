using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Entities;

[Table("Emails")]
[Index(nameof(EmailAddress), IsUnique = true)]
public class Email : BaseEntity
{
    [Column(TypeName = "nvarchar(254)")]
    public required string EmailAddress { get; set; }

    [ForeignKey(nameof(Person))]
    public int PersonId { get; set; }
    public Person Person { get; set; }
}