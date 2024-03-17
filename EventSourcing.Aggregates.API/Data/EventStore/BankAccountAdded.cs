using EventSourcing.EventStoreDB.Common;

namespace EventSourcing.Aggregates.API.Data.EventStore;

public record BankAccountAdded : Event
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string MaskedAccountNumber { get; set; }
    public string MaskedSortCode { get; set; }

    public BankAccountAdded(string id, string name, string maskedAccountNumber, string maskedSortCode)
    {
        Id = id;
        Name = name;
        MaskedAccountNumber = maskedAccountNumber;
        MaskedSortCode = maskedSortCode;
        Timestamp = DateTime.UtcNow;
    }
}
