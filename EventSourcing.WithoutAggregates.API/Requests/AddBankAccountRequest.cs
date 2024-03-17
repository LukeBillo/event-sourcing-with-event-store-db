namespace EventSourcing.WithoutAggregates.API.Requests;

public class AddBankAccountRequest
{
    public string Name { get; init; }
    public string AccountNumber { get; init; }
    public string SortCode { get; init; }
}
