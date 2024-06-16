using EmailsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Database.Seeders;

public class DataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (await _context.Persons.AnyAsync() || await _context.Emails.AnyAsync())
            return;

        var random = new Random();
        var faker = new Bogus.Faker();
        var usedEmails = new HashSet<string>();

        const int personCount = 25;
        var people = new List<Person>();
        for (var i = 0; i < personCount; i++)
        {
            var person = new Person
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                Description = faker.Lorem.Paragraphs(random.Next(3, 5))
            };

            var emailsCount = random.Next(1, 4);
            var emails = new List<Email>();
            for (var j = 0; j < emailsCount; j++)
            {
                var email = new Email
                {
                    EmailAddress = GenerateRandomEmail(faker, usedEmails),
                    PersonId = person.Id
                };
                emails.Add(email);
            }

            person.Emails = emails;
            people.Add(person);
        }

        await _context.AddRangeAsync(people);
        await _context.SaveChangesAsync();
    }
    
    private static string GenerateRandomEmail(Bogus.Faker faker, HashSet<string> usedEmails)
    {
        string newEmail;
        do
        {
            newEmail = faker.Internet.Email();
        } while (usedEmails.Contains(newEmail));
        usedEmails.Add(newEmail);
        return newEmail;
    }
}