using EventSourcing.EventStoreDB.Common;
using EventSourcing.WithoutAggregates.API.Data.Sql;
using Newtonsoft.Json;

namespace EventSourcing.WithoutAggregates.API.Data.EventStore;

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

    public static BankAccountAdded From(BankAccount bankAccount) => new(
        bankAccount.Id,
        bankAccount.Name,
        $"******{bankAccount.AccountNumber[^2..]}",
        $"**-**-{bankAccount.SortCode[^2..]}")
    {
        Timestamp = DateTime.UtcNow
    };

    [JsonIgnore]
    public string Stream => $"{StreamNames.BankAccounts}-{Id}";
}
