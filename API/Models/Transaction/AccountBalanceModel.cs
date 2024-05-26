namespace API.Models.Transaction;

public record AccountBalanceModel
{
    public string AccountNumber { get; init; } = default!;
    
    public decimal Balance { get; init; }
    
    public DateTime BalanceAt { get; init; }
}