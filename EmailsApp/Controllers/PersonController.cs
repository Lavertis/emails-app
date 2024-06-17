using EmailsApp.Database;
using EmailsApp.DTOs;
using EmailsApp.Entities;
using EmailsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Controllers;

public class PersonController : Controller
{
    private readonly AppDbContext _dbContext;

    public PersonController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] PersonCreateViewModel createViewModel)
    {
        var emailExists = await _dbContext.Emails.AnyAsync(e => e.EmailAddress == createViewModel.Email);
        if (emailExists)
            ModelState.AddModelError("Email", "Email already exists.");
        if (!ModelState.IsValid)
        {
            return View("Create", createViewModel);
        }
        
        var person = new Person
        {
            FirstName = createViewModel.FirstName.Trim(),
            LastName = createViewModel.LastName.Trim(),
            Description = createViewModel.Description?.Trim(),
            Emails = new List<Email> { new() { EmailAddress = createViewModel.Email.Trim() } }
        };

        var entityEntry = await _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction("Details", new { id = entityEntry.Entity.Id });
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

        var personDto = new PersonDetailsDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Description = person.Description,
        };

        var emails = person.Emails
            .OrderBy(e => e.CreatedAt)
            .Select(e => new EmailDto
            {
                Id = e.Id,
                EmailAddress = e.EmailAddress
            });

        var viewModel = new PersonDetailsViewModel { Person = personDto, Emails = emails.ToList() };
        return View(viewModel);
    }


    [HttpPost("update")]
    public async Task<IActionResult> Update(PersonDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.Emails = await _dbContext.Emails
                .Where(e => e.PersonId == viewModel.Person.Id)
                .OrderBy(e => e.CreatedAt)
                .Select(e => new EmailDto { Id = e.Id, EmailAddress = e.EmailAddress })
                .ToListAsync();
            return View("Details", viewModel);
        }

        var person = await _dbContext.Persons.FindAsync(viewModel.Person.Id);
        if (person == null)
        {
            return NotFound($"Person with id {viewModel.Person.Id} not found.");
        }

        person.FirstName = viewModel.Person.FirstName.Trim();
        person.LastName = viewModel.Person.LastName.Trim();
        person.Description = viewModel.Person.Description?.Trim();

        _dbContext.Persons.Update(person);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Details", new { id = viewModel.Person.Id });
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
            .Select(p => new PersonListItemDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                IsDescriptionTruncated = p.IsDescriptionTruncated,
                Description = p.Description,
                EmailAddress = p.EmailAddress?.EmailAddress
            });

        var model = new ListViewModel<PersonListItemDto>
        {
            Items = personWithFirstEmailDtos,
            Page = query.Page,
            PageCount = pageCount
        };

        return View(model);
    }
}