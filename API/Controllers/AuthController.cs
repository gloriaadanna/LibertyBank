using System.ComponentModel.DataAnnotations;
using API.Helpers;
using API.Models;
using API.Models.Account;
using API.Models.Authentication;
using Domain.Entities.Account;
using Domain.enums;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(DatabaseContext databaseContext, IConfiguration configuration) : ControllerBase
{
    [HttpPost("signin")]
    [AllowAnonymous]
    public async Task<ActionResult<AccountModel>> SignIn([FromBody] SignInModel model)
    {
        var validationResult = await new SignInModelValidator().ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToString(Constants.ErrorSeparator));
        }

        var identifierType = GetSignInIdentifierType(model.Identifier);
        if (identifierType == IdentifierType.None)
        {
            return BadRequest("Identifier is not valid");
        }

        Account? account = identifierType switch
        {
            IdentifierType.Email => await databaseContext.Accounts.SingleOrDefaultAsync(p => p.Email == model.Identifier),
            IdentifierType.AccountNumber => await databaseContext.Accounts.SingleOrDefaultAsync(p => p.AccountNumber == model.Identifier),
            IdentifierType.PhoneNumber => await databaseContext.Accounts.SingleOrDefaultAsync(p => p.PhoneNumber == model.Identifier),
            _ => null
        };

        if (account is null)
        {
            return NotFound("Customer account not found.");
        }

        var isCorrectPassword =
            PasswordHelper.VerifyPassword(model.Password, account.PasswordHash, account.PasswordSalt);
        if (!isCorrectPassword)
        {
            return Unauthorized();
        }

        var authenticationResponse = new AuthResponse
        {
            Token = TokenHelper.GenerateToken(configuration, account.Id),
            CreatedAt = DateTime.UtcNow
        };

        return Ok(authenticationResponse);
    }
    
    private IdentifierType GetSignInIdentifierType(string identifier)
    {
        var identifierType = IdentifierType.None;
        if (identifier.All(char.IsDigit))
        {
            var identifierLength = identifier.Length;
            identifierType = identifierLength switch
            {
                10 => IdentifierType.AccountNumber,
                13 or 11 => IdentifierType.PhoneNumber,
                _ => identifierType
            };
        }
        else if (new EmailAddressAttribute().IsValid(identifier))
        {
            identifierType = IdentifierType.Email;
        }

        return identifierType;
    }
}