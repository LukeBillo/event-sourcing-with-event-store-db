using EventSourcing.EventStoreDB.Common;
using EventSourcing.WithoutAggregates.API.Data.Sql;
using Newtonsoft.Json;

namespace EventSourcing.WithoutAggregates.API.Data.EventStore;

public record CashDeposited : Event
{
    public string Id { get; set; }
    public decimal Amount { get; set; }
    public decimal PreviousBalance { get; set; }
    public decimal UpdatedBalance { get; set; }

    public CashDeposited(string id, decimal amount, decimal previousBalance, decimal updatedBalance)
    {
        Id = id;
        Amount = amount;
        PreviousBalance = previousBalance;
        UpdatedBalance = updatedBalance;
        Timestamp = DateTime.UtcNow;
    }

    public static CashDeposited From(BankAccount bankAccount, decimal depositAmount) => new(
        bankAccount.Id,
        depositAmount,
        bankAccount.Balance,
        bankAccount.Balance + depositAmount)
    {
        Timestamp = DateTime.UtcNow
    };

    [JsonIgnore]
    public string Stream => $"{StreamNames.BankAccounts}-{Id}";
}
