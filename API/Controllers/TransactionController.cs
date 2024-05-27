using API.Models;
using API.Models.Transaction;
using Domain.Entities.Transaction;
using Domain.enums;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionStatus = Domain.enums.TransactionStatus;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController(DatabaseContext databaseContext) : ControllerBase
{
    [HttpGet("balance/{accountNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountBalanceModel>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<List<AccountBalanceModel>>> GetAccountBalance(string accountNumber)
    {
        var account = await databaseContext.Accounts.SingleOrDefaultAsync(p => p.AccountNumber == accountNumber);
        if (account is null)
        {
            return NotFound("Customer account not found.");
        }

        var transactions = await databaseContext.Transactions
            .Where(p => (p.SourceAccountNumber == accountNumber || p.DestinationAccountNumber == accountNumber) && p.Status == TransactionStatus.Completed)
            .Select(transaction => new
            {
                transaction.Amount,
                transaction.Type
            }).ToListAsync();

        var balance = 0.0m;
        foreach (var transaction in transactions)
        {
            switch (transaction.Type)
            {
                case TransactionType.Deposit:
                    balance += transaction.Amount;
                    break;
                case TransactionType.Withdrawal:
                case TransactionType.Transfer:
                    balance -= transaction.Amount;
                    break;
            }
        }

        var accountBalanceModel = new AccountBalanceModel
        {
            AccountNumber = account.AccountNumber,
            Balance = balance,
            BalanceAt = DateTime.UtcNow
        };
        return Ok(accountBalanceModel);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransactionModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<TransactionModel>> PostTransaction([FromBody] CreateTransactionModel model)
    {
        var validationResult = await new CreateTransactionModelValidator().ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToString(Constants.ErrorSeparator));
        }
        
        var account = await databaseContext.Accounts.SingleOrDefaultAsync(p => p.AccountNumber == model.SourceAccountNumber);
        if (account is null)
        {
            return NotFound("Customer account not found.");
        }

        //TODO: Evaluate existing  balance to block transactions that will lead to negative balance 
        
        var transaction = new Transaction(model.Type, model.Amount, model.SourceAccountNumber, model.TransactionDate,
            sourceBank: model.SourceBank, destinationBank: model.DestinationBank, destinationAccountNumber: model.DestinationAccountNumber,
            description: model.Description);
        
        databaseContext.Transactions.Add(transaction);
        if (await databaseContext.SaveChangesAsync() <= 0)
        {
            throw new Exception("Error occured while creating account");
        }
        
        var transactionModel = new TransactionModel
        {
            Id = transaction.Id,
            Type = transaction.Type,
            Amount = transaction.Amount,
            SourceAccountNumber = transaction.SourceAccountNumber,
            SourceBank = transaction.SourceBank,
            DestinationAccountNumber = transaction.DestinationAccountNumber,
            DestinationBank = transaction.DestinationBank,
            TransactionDate = transaction.TransactionDate,
            Status = transaction.Status,
            CreatedAt = transaction.CreatedAt
        };
        return Ok(transactionModel);
    }
    
    [HttpGet("history/{accountNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<List<TransactionModel>>> GetTransactionHistory(string accountNumber, [FromQuery] TransactionStatus? status, 
        [FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 20)
    {
        var account = await databaseContext.Accounts.SingleOrDefaultAsync(p => p.AccountNumber == accountNumber);
        if (account is null)
        {
            return NotFound("Customer account not found.");
        }

        var transactionsQuery = databaseContext.Transactions.Where(p => p.SourceAccountNumber == accountNumber 
                                                                              || p.DestinationAccountNumber == accountNumber);
        if (status is not null)
        {
            transactionsQuery = transactionsQuery.Where(p => p.Status == status);
        }
        var skip = (pageNumber - 1) * pageSize ?? 0;
        var transactions = await transactionsQuery.OrderByDescending(p => p.CreatedAt)
            .Skip(skip).Take(pageSize ?? 20)
            .ToListAsync();

        var transactionModels = new List<TransactionModel>();
        foreach (var transaction in transactions)
        {
            var transactionModel = new TransactionModel
            {
                Id = transaction.Id,
                Type = transaction.Type,
                Amount = transaction.Amount,
                SourceAccountNumber = transaction.SourceAccountNumber,
                SourceBank = transaction.SourceBank,
                DestinationAccountNumber = transaction.DestinationAccountNumber,
                DestinationBank = transaction.DestinationBank,
                TransactionDate = transaction.TransactionDate,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt
            };
            transactionModels.Add(transactionModel);
        }

        return Ok(transactionModels);
    }
}