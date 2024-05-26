using Domain.enums;

namespace API.Models.Transaction;

public record TransactionModel
{
    public Guid Id { get; init; }
    
    public TransactionType Type { get; init; }
    
    public decimal Amount { get; init; }
    
    public string SourceAccountNumber { get; init; } = default!;
    
    public string SourceBank { get; init; } = default!;

    public string? DestinationAccountNumber { get; init; }

    public string? DestinationBank { get; init; }
    
    public DateTime TransactionDate { get; init; }
    
    public TransactionStatus Status { get; init; }
    
    public DateTime CreatedAt { get; init; }
}