using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSourcing.WithoutAggregates.API.Data.Sql;

[Table(Table)]
public class BankAccount
{
    public const string Table = "BankAccounts";

    [Required]
    public string Id { get; set; }
    public string Name { get; set; }
    public string AccountNumber { get; set; }
    public string SortCode { get; set; }
    public decimal Balance { get; set; }
}
