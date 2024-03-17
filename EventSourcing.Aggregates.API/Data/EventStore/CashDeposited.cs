using EventSourcing.EventStoreDB.Common;

namespace EventSourcing.Aggregates.API.Data.EventStore;

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
}
