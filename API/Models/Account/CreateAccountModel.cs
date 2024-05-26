using Domain.enums;
using FluentValidation;

namespace API.Models.Account;

public record CreateAccountModel
{
    public string? Title { get; init; }
    
    public string FirstName { get; init; } = default!;
    
    public string LastName { get; init; } = default!;
    
    public string? MiddleName { get; init; }
    
    public string Address { get; init; } = default!;
    
    public string Email { get; init; } = default!;
    
    public string PhoneNumber { get; init; } = default!;
    
    public string BankVerificationNumber { get; init; } = default!;
    
    public string NationalIdentityNumber { get; init; } = default!;
    
    public AccountType AccountType { get; init; }

    public string Password { get; init; } = default!;
}

public class CreateAccountModelValidator : AbstractValidator<CreateAccountModel>
{
    public CreateAccountModelValidator()
    {
        RuleFor(m => m.FirstName).NotEmpty();
        RuleFor(m => m.LastName).NotEmpty();
        RuleFor(m => m.Address).NotEmpty();
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.PhoneNumber).NotEmpty().Length(13).Matches("^[0-9]${13}");
        RuleFor(m => m.BankVerificationNumber).NotEmpty();
        RuleFor(m => m.NationalIdentityNumber).NotEmpty();
        RuleFor(m => m.AccountType).IsInEnum();
        RuleFor(m => m.Password).NotEmpty().MinimumLength(6);
    }
}