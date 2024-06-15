using EmailsApp.Database;
using EmailsApp.DTOs;
using EmailsApp.Models;
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

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        const int descriptionTruncationLength = 20;
        var personCount = await _dbContext.Persons.CountAsync();
        var pageCount = (int)Math.Ceiling(personCount / (double)pageSize);

        var personsWithFirstEmail = await _dbContext.Persons
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                IsDescriptionTruncated = p.Description != null && p.Description.Length > descriptionTruncationLength,
                Description = p.Description != null ? p.Description.Substring(0, descriptionTruncationLength) : null,
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

        var model = new PersonListViewModel
        {
            Persons = personWithFirstEmailDtos,
            Page = page,
            PageCount = pageCount
        };

        return View(model);
    }
}