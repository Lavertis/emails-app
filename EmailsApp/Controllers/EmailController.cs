using EmailsApp.Database;
using EmailsApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Controllers;

[Route("api/[controller]")]
public class EmailController : Controller
{
    private readonly EmailsDbContext _dbContext;

    public EmailController(EmailsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpDelete("{emailId:int}")]
    public async Task<IActionResult> Delete(int emailId)
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
    public async Task<IActionResult> AddPersonEmail(int personId, string emailAddress)
    {
        var emailAlreadyExists = await _dbContext.Emails
            .Where(e => e.PersonId == personId)
            .AnyAsync(e => e.EmailAddress == emailAddress);
        if (emailAlreadyExists)
        {
            return Conflict("Email already added.");
        }
    
        var email = new Email
        {
            EmailAddress = emailAddress,
            PersonId = personId
        };
    
        await _dbContext.Emails.AddAsync(email);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}