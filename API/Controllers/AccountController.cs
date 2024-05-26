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
public class AccountTransaction(DatabaseContext databaseContext, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
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

        Account? account = null;
        switch (identifierType)
        {
            case IdentifierType.Email:
                account = await databaseContext.Accounts.FirstOrDefaultAsync(p => p.Email == model.Identifier);
                break;
            case IdentifierType.AccountNumber:
                account = await databaseContext.Accounts.FirstOrDefaultAsync(p => p.AccountNumber == model.Identifier);
                break;
            case IdentifierType.PhoneNumber:
                account = await databaseContext.Accounts.FirstOrDefaultAsync(p => p.PhoneNumber == model.Identifier);
                break;
        }
        
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
    
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    public async Task<ActionResult<AccountModel>> CreateAccount([FromBody] CreateAccountModel model)
    {
        var validationResult = await new CreateAccountModelValidator().ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToString(Constants.ErrorSeparator));
        }
        
        //Validate BVN
        if (false)
        {
            return BadRequest("Invalid BVN");
        }

        //Validate NIN
        if (false)
        {
            return BadRequest("Invalid NIN");
        }
        
        var existingCustomer = await databaseContext.Accounts.FirstOrDefaultAsync(p =>
                p.Email == model.Email && p.PhoneNumber == model.PhoneNumber);
        if (existingCustomer is not null)
        {
            return BadRequest("Customer already exists.");
        }

        var passwordSalt = Guid.NewGuid().ToString().Replace("-", "");
        var passwordHash = PasswordHelper.HashPassword(model.Password, passwordSalt);
        var account = new Account(model.Title, model.FirstName, model.LastName, model.MiddleName, model.Address, model.Email, model.PhoneNumber,
            model.BankVerificationNumber, model.NationalIdentityNumber, model.AccountType, passwordSalt, passwordHash);
        databaseContext.Accounts.Add(account);
        if (await databaseContext.SaveChangesAsync() <= 0)
        {
            throw new Exception("Error occured while creating account");
        }

        var accountModel = new AccountModel
        {
            Id = account.Id,
            Title = account.Title,
            FirstName = account.FirstName,
            LastName = account.LastName,
            MiddleName = account.MiddleName,
            AccountType = account.AccountType,
            Email = account.Email,
            PhoneNumber = account.PhoneNumber,
            Address = account.Address,
            BankVerificationNumber = account.BankVerificationNumber,
            NationalIdentityNumber = account.NationalIdentityNumber,
            CreatedAt = account.CreatedAt
        };
        return CreatedAtAction("", accountModel);
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<AccountModel>> UpdateAccount([FromBody] UpdateAccountModel model)
    {
        var validationResult = await new UpdateAccountModelValidator().ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToString(Constants.ErrorSeparator));
        }
        
        var existingAccount = await databaseContext.Accounts.FirstOrDefaultAsync(p => p.Id == model.Id);
        if (existingAccount is null)
        {
            return NotFound("Customer account not found.");
        }
        
        existingAccount.UpdateAccount(model.Title, model.FirstName, model.LastName, model.MiddleName, model.Address, model.PhoneNumber);
        databaseContext.Accounts.Update(existingAccount);
        
        if (await databaseContext.SaveChangesAsync() <= 0)
        {
            throw new Exception("Error occured while creating account");
        }
        
        var accountModel = new AccountModel
        {
            Id = existingAccount.Id,
            Title = existingAccount.Title,
            FirstName = existingAccount.FirstName,
            LastName = existingAccount.LastName,
            MiddleName = existingAccount.MiddleName,
            AccountType = existingAccount.AccountType,
            Email = existingAccount.Email,
            PhoneNumber = existingAccount.PhoneNumber,
            Address = existingAccount.Address,
            BankVerificationNumber = existingAccount.BankVerificationNumber,
            NationalIdentityNumber = existingAccount.NationalIdentityNumber,
            CreatedAt = existingAccount.CreatedAt
        };
        return Ok(accountModel);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<AccountModel>> GetAccount(Guid id)
    {
        var account = await databaseContext.Accounts.FirstOrDefaultAsync(p => p.Id == id);
        if (account is null)
        {
            return NotFound("Customer account not found.");
        }
        
        var accountModel = new AccountModel
        {
            Id = account.Id,
            Title = account.Title,
            FirstName = account.FirstName,
            LastName = account.LastName,
            MiddleName = account.MiddleName,
            AccountType = account.AccountType,
            Email = account.Email,
            PhoneNumber = account.PhoneNumber,
            Address = account.Address,
            BankVerificationNumber = account.BankVerificationNumber,
            NationalIdentityNumber = account.NationalIdentityNumber,
            CreatedAt = account.CreatedAt
        };
        return Ok(accountModel);
    }
    
    [HttpGet("accountNumber/{accountNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<AccountModel>> GetAccount(string accountNumber)
    {
        var account = await databaseContext.Accounts.FirstOrDefaultAsync(p => p.AccountNumber == accountNumber);
        if (account is null)
        {
            return NotFound("Customer account not found.");
        }
        
        var accountModel = new AccountModel
        {
            Id = account.Id,
            Title = account.Title,
            FirstName = account.FirstName,
            LastName = account.LastName,
            MiddleName = account.MiddleName,
            AccountType = account.AccountType,
            Email = account.Email,
            PhoneNumber = account.PhoneNumber,
            Address = account.Address,
            BankVerificationNumber = account.BankVerificationNumber,
            NationalIdentityNumber = account.NationalIdentityNumber,
            CreatedAt = account.CreatedAt,
        };
        return Ok(accountModel);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountModel>))]
    public async Task<ActionResult<List<AccountModel>>> GetAccount(int? pageNumber = 1, int? pageSize = 20)
    {
        var skip = (pageNumber - 1) * pageSize ?? 0;
        var accounts = await databaseContext.Accounts.OrderByDescending(p => p.CreatedAt)
            .Skip(skip).Take(pageSize ?? 20)
            .ToListAsync();

        if (accounts.Count == 0)
        {
            return Ok(accounts);
        }

        var accountModels = new List<AccountModel>();
        foreach (var account in accounts)
        {
            var accountModel = new AccountModel
            {
                Id = account.Id,
                Title = account.Title,
                FirstName = account.FirstName,
                LastName = account.LastName,
                MiddleName = account.MiddleName,
                AccountType = account.AccountType,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber,
                Address = account.Address,
                BankVerificationNumber = account.BankVerificationNumber,
                NationalIdentityNumber = account.NationalIdentityNumber,
                CreatedAt = account.CreatedAt,
            };
            accountModels.Add(accountModel);
        }
        return Ok(accountModels);
    }

    private IdentifierType GetSignInIdentifierType(string identifier)
    {
        IdentifierType identifierType = IdentifierType.None;
        if (identifier.All(char.IsDigit))
        {
            var identifierLength = identifier.Length;
            if (identifierLength == 10)
            {
                identifierType = IdentifierType.AccountNumber;
            }
            else if(identifierLength == 13 || identifierLength == 11)
            {
                identifierType = IdentifierType.PhoneNumber;
            }
        }
        else if (new EmailAddressAttribute().IsValid(identifier))
        {
            identifierType = IdentifierType.Email;
        }

        return identifierType;
    }
}