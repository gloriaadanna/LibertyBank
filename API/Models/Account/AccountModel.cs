using Domain.enums;

namespace API.Models.Account;

public record AccountModel
{
    public Guid Id { get; init; }
    
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

    public DateTime CreatedAt { get; init; }
}