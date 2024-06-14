using EmailsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Database;

public class DataSeeder
{
    private readonly EmailsDbContext _context;

    public DataSeeder(EmailsDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (!await _context.Persons.AnyAsync())
        {
            var personsFaker = new Bogus.Faker<Person>()
                .RuleFor(p => p.Imie, f => f.Name.FirstName())
                .RuleFor(p => p.Nazwisko, f => f.Name.LastName())
                .RuleFor(p => p.Opis, f => f.Lorem.Paragraph());

            var persons = personsFaker.Generate(5);

            await _context.Persons.AddRangeAsync(persons);
            await _context.SaveChangesAsync();
        }

        if (!await _context.Emails.AnyAsync())
        {
            var emailsFaker = new Bogus.Faker<Email>()
                .RuleFor(e => e.EmailAddress, f => f.Internet.Email());

            foreach (var person in _context.Persons)
            {
                var emailsCount = new Random().Next(1, 4);
                var emails = emailsFaker
                    .RuleFor(e => e.PersonId, person.Id)
                    .Generate(emailsCount);

                await _context.Emails.AddRangeAsync(emails);
            }

            await _context.SaveChangesAsync();
        }
    }
}