using EmailsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Database;

public class EmailsDbContext : DbContext
{
    public EmailsDbContext(DbContextOptions<EmailsDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Email> Emails { get; set; }
}