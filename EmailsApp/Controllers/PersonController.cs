using AutoMapper;
using EmailsApp.Database;
using EmailsApp.DTOs;
using EmailsApp.Entities;
using EmailsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Controllers;

[Route("people")]
public class PersonController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public PersonController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet("add")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("add")]
    public async Task<IActionResult> Create([FromForm] PersonCreateViewModel createViewModel)
    {
        var emailExists = await _dbContext.Emails.AnyAsync(e => e.EmailAddress == createViewModel.Email);
        if (emailExists)
            ModelState.AddModelError("Email", "Email already exists.");
        if (!ModelState.IsValid)
            return View("Create", createViewModel);

        var person = _mapper.Map<Person>(createViewModel);
        var entityEntry = await _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction("Details", new { id = entityEntry.Entity.Id });
    }

    [HttpDelete("{personId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int personId)
    {
        var person = await _dbContext.Persons.FindAsync(personId);
        if (person == null)
            return NotFound($"Person with id {personId} not found.");

        _dbContext.Persons.Remove(person);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details([FromRoute] int id)
    {
        var person = await _dbContext.Persons.FirstOrDefaultAsync(p => p.Id == id);
        if (person == null)
            return View("Error");

        var personDto = _mapper.Map<PersonDetailsDto>(person);
        var viewModel = new PersonDetailsViewModel { Person = personDto };
        return View(viewModel);
    }

    [HttpPost("{id:int}/update")]
    public async Task<IActionResult> Update(int id, PersonDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View("Details", viewModel);

        var person = await _dbContext.Persons.FindAsync(id);
        if (person == null)
            return View("Error");

        _mapper.Map(viewModel.Person, person);
        _dbContext.Persons.Update(person);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Details", new { id = viewModel.Person.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PaginationQuery query)
    {
        var personCount = await _dbContext.Persons.CountAsync();
        var pageCount = (int)Math.Ceiling(personCount / (double)query.PageSize);

        const int descriptionTrimLength = 30;
        var personsWithFirstEmail = await _dbContext.Persons
            .OrderByDescending(p => p.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PersonListItemDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                IsDescriptionTruncated = p.Description.Length > descriptionTrimLength,
                Description = p.Description.Length > descriptionTrimLength
                    ? p.Description.Substring(0, descriptionTrimLength)
                    : p.Description,
                EmailAddress = p.Emails.OrderBy(e => e.CreatedAt).First().EmailAddress
            })
            .ToListAsync();

        var model = new ListViewModel<PersonListItemDto>
        {
            Items = personsWithFirstEmail,
            Page = query.Page,
            PageCount = pageCount
        };

        return View("Index", model);
    }
}