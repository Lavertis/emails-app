using EmailsApp.Database;
using EmailsApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Controllers;

[Route("api/[controller]")]
public class PersonController : Controller
{
    private readonly EmailsDbContext _dbContext;

    public PersonController(EmailsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpDelete("{personId:int}")]
    public async Task<IActionResult> Delete(int personId)
    {
        var person = await _dbContext.Persons.FindAsync(personId);
        if (person == null)
        {
            return NotFound($"Person with id {personId} not found.");
        }

        _dbContext.Persons.Remove(person);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var person = await _dbContext.Persons
            .Include(p => p.Emails)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
        {
            return NotFound();
        }

        var personDto = new PersonWithEmailsDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Description = person.Description,
            Emails = person.Emails.Select(e => new EmailDto
            {
                Id = e.Id,
                EmailAddress = e.EmailAddress
            }).ToList()
        };

        return View(personDto);
    }

    public async Task<IActionResult> Index()
    {
        const int maxDescriptionLength = 20;
        var personsWithFirstEmail = await _dbContext.Persons
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                IsDescriptionTruncated = p.Description != null && p.Description.Length > maxDescriptionLength,
                Description = p.Description != null ? p.Description.Substring(0, maxDescriptionLength) : null,
                EmailAddress = p.Emails.OrderBy(e => e.AddedAt).FirstOrDefault()
            })
            .ToListAsync();

        var personWithFirstEmailDtos = personsWithFirstEmail
            .Select(p => new PersonWithFirstEmailDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                IsDescriptionTruncated = p.IsDescriptionTruncated,
                Description = p.Description,
                EmailAddress = p.EmailAddress?.EmailAddress
            });
        return View(personWithFirstEmailDtos);
    }
}