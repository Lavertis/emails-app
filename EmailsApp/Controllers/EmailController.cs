using EmailsApp.Database;
using EmailsApp.DTOs;
using EmailsApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Controllers;

public class EmailController : Controller
{
    private readonly AppDbContext _dbContext;

    public EmailController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpDelete("{emailId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int emailId)
    {
        var email = await _dbContext.Emails.FindAsync(emailId);
        if (email == null)
        {
            return NotFound($"Email with id {emailId} not found.");
        }

        const int minEmailsCount = 1;
        var personEmailsCount = await _dbContext.Emails.CountAsync(e => e.PersonId == email.PersonId);
        if (personEmailsCount <= minEmailsCount)
        {
            return BadRequest("Person must have at least one email.");
        }

        _dbContext.Emails.Remove(email);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddEmailRequest request)
    {
        var emailAlreadyExists = await _dbContext.Emails.AnyAsync(e => e.EmailAddress == request.EmailAddress);
        if (emailAlreadyExists)
        {
            return Conflict("Email already exists.");
        }

        var email = new Email
        {
            EmailAddress = request.EmailAddress,
            PersonId = request.PersonId
        };

        await _dbContext.Emails.AddAsync(email);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}