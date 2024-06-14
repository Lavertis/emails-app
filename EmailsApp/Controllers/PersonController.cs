using System.Diagnostics;
using EmailsApp.Database;
using EmailsApp.DTOs;
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