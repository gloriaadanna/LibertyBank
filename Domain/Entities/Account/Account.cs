using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Domain.enums;

namespace Domain.Entities.Account;

public class Account
{
    protected Account() { }
    
    public Account(string title, string firstName, string lastName, string? middleName, string address, string email, string phoneNumber,
        string bankVerificationNumber, string nationalIdentityNumber, AccountType accountType, string passwordSalt, string passwordHash)
    {
        Id = Guid.NewGuid();
        AccountNumber = GenerateAccountNumber();
        Title = title;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Address = address;
        Email = email;
        PhoneNumber = phoneNumber;
        BankVerificationNumber = bankVerificationNumber;
        NationalIdentityNumber = nationalIdentityNumber;
        AccountType = accountType;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }
    
    [Key]
    public Guid Id { get; private set; }

    [Required]
    public string AccountNumber { get; private set; } = default!;
    
    public string? Title { get; private set; }

    [Required]
    public string FirstName { get; private  set; } = default!;

    [Required] 
    public string LastName { get; private set; } = default!;
    
    public string? MiddleName { get; private set; }
    
    [Required] 
    public string Address { get; private set; } = default!;
    
    [Required] 
    public string Email { get; private set; } = default!;
    
    [Required] 
    public string PhoneNumber { get; private set; } = default!;
    
    [Required] 
    public string BankVerificationNumber { get; private set; } = default!;
    
    [Required] 
    public string NationalIdentityNumber { get; private set; } = default!;

    [Required]
    [EnumDataType(typeof(AccountType))]
    public AccountType AccountType { get; private set; }
    
    [Required]
    public string PasswordHash { get; private set; }

    [Required]
    public string PasswordSalt { get; private set; }

    public DateTime CreatedAt { get; private set; }
    
    public DateTime? UpdatedAt { get; private set; }

    public void UpdateAccount(string title, string firstName, string lastName, string? middleName, string address, string phoneNumber)
    {
        Title = title;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Address = address;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    private string GenerateAccountNumber()
    {
        var random = RandomNumberGenerator.GetInt32(100000, 999999);
        return random.ToString("D10");
    }
}