using AutoMapper;
using EmailsApp.Database;
using EmailsApp.DTOs;
using EmailsApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Controllers;

[Route("emails")]
public class EmailController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public EmailController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var email = await _dbContext.Emails.FindAsync(id);
        if (email == null)
            return NotFound($"Email with id {id} not found.");

        const int minEmailsCount = 1;
        var personEmailsCount = await _dbContext.Emails.CountAsync(e => e.PersonId == email.PersonId);
        if (personEmailsCount <= minEmailsCount)
            return BadRequest("Person must have at least one email.");

        _dbContext.Emails.Remove(email);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddEmailRequest request)
    {
        var emailAlreadyExists = await _dbContext.Emails.AnyAsync(e => e.EmailAddress == request.EmailAddress);
        if (emailAlreadyExists)
            return Conflict("Email already exists.");

        var email = _mapper.Map<Email>(request);
        await _dbContext.Emails.AddAsync(email);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("person/{personId:int}")]
    public async Task<ActionResult<IList<EmailDto>>> GetEmails([FromRoute] int personId)
    {
        var emails = await _dbContext.Emails
            .Where(e => e.PersonId == personId)
            .OrderBy(e => e.CreatedAt)
            .Select(e => _mapper.Map<EmailDto>(e))
            .ToListAsync();
        return Ok(emails);
    }
}