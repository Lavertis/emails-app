using EmailsApp.Database;
using EmailsApp.DTOs;
using EmailsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Controllers;

[Route("api/[controller]")]
public class PersonController : Controller
{
    private readonly AppDbContext _dbContext;

    public PersonController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpDelete("{personId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int personId)
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
    public async Task<IActionResult> Details([FromRoute] int id)
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
    
    [HttpPost("{personId:int}")]
    public async Task<IActionResult> Update([FromRoute] int personId, [FromForm] PersonUpdateDto updateDto)
    {
        var person = await _dbContext.Persons.FindAsync(personId);
        if (person == null)
        {
            return NotFound($"Person with id {personId} not found.");
        }
        
        person.FirstName = updateDto.FirstName;
        person.LastName = updateDto.LastName; 
        person.Description = updateDto.Description;

        _dbContext.Persons.Update(person);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Details", new { id = personId });
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PaginationQuery query)
    {
        const int descriptionTruncationLength = 30;
        var personCount = await _dbContext.Persons.CountAsync();
        var pageCount = (int)Math.Ceiling(personCount / (double)query.PageSize);

        var personsWithFirstEmail = await _dbContext.Persons
            .OrderByDescending(p => p.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                IsDescriptionTruncated = p.Description != null && p.Description.Length > descriptionTruncationLength,
                Description = p.Description != null ? p.Description.Substring(0, descriptionTruncationLength) : null,
                EmailAddress = p.Emails.OrderBy(e => e.CreatedAt).FirstOrDefault()
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
            Page = query.Page,
            PageCount = pageCount
        };

        return View(model);
    }
}