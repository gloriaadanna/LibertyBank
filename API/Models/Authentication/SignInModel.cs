using FluentValidation;

namespace API.Models.Authentication;

public record SignInModel
{
    public string Identifier { get; init; } = default!;
    
    public string Password { get; init; } = default!;
}

public class SignInModelValidator : AbstractValidator<SignInModel>
{
    public SignInModelValidator()
    {
        RuleFor(m => m.Identifier).NotEmpty();
        RuleFor(m => m.Password).NotEmpty().MinimumLength(6);
    }
}