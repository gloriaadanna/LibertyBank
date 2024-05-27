using Domain.enums;
using FluentValidation;

namespace API.Models.Transaction;

public record CreateTransactionModel
{
    public TransactionType Type { get; init; }
    
    public decimal Amount { get; init; }

    public string? Description { get; init; } = default!;

    public string SourceAccountNumber { get; init; } = default!;
    
    public string SourceBank { get; init; } = default!;

    public string? DestinationAccountNumber { get; init; }

    public string? DestinationBank { get; init; }
    
    public DateTime TransactionDate { get; init; }
}

public class CreateTransactionModelValidator : AbstractValidator<CreateTransactionModel>
{
    public CreateTransactionModelValidator()
    {
        RuleFor(m => m.Type).IsInEnum();
        RuleFor(m => m.Amount).NotEmpty();
        RuleFor(m => m.SourceAccountNumber).NotEmpty().Length(10).Matches("^[0-9]*${10}");;
        RuleFor(m => m.SourceBank).NotEmpty();
        RuleFor(m => m.DestinationAccountNumber).NotEmpty().Length(10).Matches("^[0-9]*${10}")
            .When(p => p.DestinationAccountNumber is not null);
        RuleFor(m => m.TransactionDate).NotEmpty();
        When(p => p.Type == TransactionType.Transfer, () =>
        {
            RuleFor(m => m.DestinationBank).NotEmpty();
            RuleFor(m => m.DestinationAccountNumber).NotEmpty().Length(10);
        });
    }
}