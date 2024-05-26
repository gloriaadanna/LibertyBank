using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Transaction;

public class Transaction
{
    [Key]
    public Guid Id { get; set; }
}