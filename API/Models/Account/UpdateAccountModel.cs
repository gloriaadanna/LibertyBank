using FluentValidation;

namespace API.Models.Account;

public record UpdateAccountModel
{
    public Guid Id { get; init; }
    
    public string Title { get; init; } = default!;
    
    public string FirstName { get; init; } = default!;
    
    public string LastName { get; init; } = default!;
    
    public string? MiddleName { get; init; }
    
    public string Address { get; init; } = default!;
    
    public string Email { get; init; } = default!;
    
    public string PhoneNumber { get; init; } = default!;
}

public class UpdateAccountModelValidator : AbstractValidator<UpdateAccountModel>
{
    public UpdateAccountModelValidator()
    {
        RuleFor(m => m.FirstName).NotEmpty();
        RuleFor(m => m.LastName).NotEmpty();
        RuleFor(m => m.Address).NotEmpty();
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.PhoneNumber).NotEmpty();
    }
}