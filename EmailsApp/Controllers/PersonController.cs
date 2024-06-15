using System.Diagnostics;
using EmailsApp.Database;
using EmailsApp.DTOs;
using EmailsApp.Entities;
using EmailsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Controllers;

public class PersonController : Controller
{
    private readonly EmailsDbContext _dbContext;

    public PersonController(EmailsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IActionResult> DeleteEmail(int personId, int emailId)
    {
        const int minEmailsCount = 1;
        var personEmailsCount = await _dbContext.Emails.Where(e => e.PersonId == personId).CountAsync();
        if (personEmailsCount <= minEmailsCount)
        {
            TempData["ErrorMessage"] = "Person must have at least one email address.";
            return RedirectToAction(nameof(Details), new { id = personId });
        }
        
        var email = await _dbContext.Emails
            .Where(e => e.PersonId == personId && e.Id == emailId)
            .FirstOrDefaultAsync();
        if (email == null)
        {
            return NotFound();
        }

        _dbContext.Emails.Remove(email);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = email.PersonId });
    }
    
    [HttpPost]
    public async Task<IActionResult> AddEmail(int personId, string emailAddress)
    {
        if (await _dbContext.Emails.AnyAsync(e => e.EmailAddress == emailAddress))
        {
            TempData["ErrorMessage"] = "Email address already exists.";
            return RedirectToAction(nameof(Details), new { id = personId });
        }

        var email = new Email
        {
            EmailAddress = emailAddress,
            PersonId = personId
        };

        await _dbContext.Emails.AddAsync(email);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = personId });
    }

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

    public async Task<IActionResult> People()
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}