using FluentValidation;

namespace API.Models.Authentication;

public record SignInModel
{
    public string Identifier { get; init; }
    
    public string Password { get; init; }
}

public class SignInModelValidator : AbstractValidator<SignInModel>
{
    public SignInModelValidator()
    {
        RuleFor(m => m.Identifier).NotEmpty();
        RuleFor(m => m.Password).NotEmpty().MinimumLength(6);
    }
}