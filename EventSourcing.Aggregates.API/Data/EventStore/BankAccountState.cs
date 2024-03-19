namespace EventSourcing.Aggregates.API.Data.EventStore;

public record BankAccountState(
    string Id,
    string? Name,
    string? MaskedAccountNumber,
    string? MaskedSortCode,
    decimal Balance,
    BankAccountStatus Status)
{
    public static BankAccountState Initial(string id)
    {
        return new BankAccountState(
            id,
            null,
            null,
            null,
            0.0m,
            BankAccountStatus.DoesNotExist);
    }
}
