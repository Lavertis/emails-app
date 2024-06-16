using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailsApp.Entities;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime UpdatedAt { get; set; }
}