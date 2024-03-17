namespace EventSourcing.WithoutAggregates.API.Data.Sql;

public class BankAccount
{
    public const string Table = "BankAccounts";

    public string Id { get; set; }
    public string Name { get; set; }
    public string AccountNumber { get; set; }
    public string SortCode { get; set; }
    public decimal Balance { get; set; }
}
