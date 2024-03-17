namespace EventSourcing.Aggregates.API.Data.EventStore;

public record BankAccountState(
    string Id,
    string Name,
    string MaskedAccountNumber,
    string MaskedSortCode,
    decimal Balance,
    BankAccountStatus Status);
