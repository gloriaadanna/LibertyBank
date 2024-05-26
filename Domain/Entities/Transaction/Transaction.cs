using System.ComponentModel.DataAnnotations;
using Domain.enums;

namespace Domain.Entities.Transaction;

public class Transaction
{
    protected Transaction() { }
    
    public Transaction(TransactionType transactionType, decimal amount, string sourceAccountNumber, DateTime transactionDate, 
        string sourceBank = "Liberty", string? destinationBank = null, string? destinationAccountNumber = null)
    {
        Id = Guid.NewGuid();
        Type = transactionType;
        Amount = amount;
        SourceBank = sourceBank;
        SourceAccountNumber = sourceAccountNumber;
        DestinationBank = destinationBank;
        DestinationAccountNumber = destinationAccountNumber;
        TransactionDate = transactionDate;
        Status = transactionType is TransactionType.Deposit or TransactionType.Withdrawal 
            ? TransactionStatus.Completed
            : !string.IsNullOrWhiteSpace(destinationBank) && string.Equals(destinationBank, "Liberty", StringComparison.OrdinalIgnoreCase) 
                ? TransactionStatus.Completed 
                : TransactionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
    
    [Key]
    public Guid Id { get; private set; }

    [Required]
    [EnumDataType(typeof(TransactionType))]
    public TransactionType Type { get; private set; }
    
    [Required]
    public decimal Amount { get; private set; }

    public string Description { get; private set; } = default!;

    [Required] 
    public string SourceAccountNumber { get; private set; } = default!;
    
    [Required]
    public string SourceBank { get; private set; } = default!;

    public string? DestinationAccountNumber { get; private set; }

    public string? DestinationBank { get; private set; }

    [Required]
    public DateTime TransactionDate { get; private set; }

    [Required]
    [EnumDataType(typeof(TransactionStatus))]
    public TransactionStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime? UpdatedAt { get; private set; }
    
    public void UpdateStatus(TransactionStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}