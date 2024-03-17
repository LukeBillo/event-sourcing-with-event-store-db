using EventSourcing.EventStoreDB.Common;
using EventSourcing.WithoutAggregates.API.Data.EventStore;
using EventSourcing.WithoutAggregates.API.Data.Sql;
using EventSourcing.WithoutAggregates.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.WithoutAggregates.API;

[ApiController]
public class BankAccountsController : ControllerBase
{
    private readonly BankAccountContext _bankAccountContext;
    private readonly IEventStore _eventStore;

    public BankAccountsController(BankAccountContext bankAccountContext, IEventStore eventStore)
    {
        _bankAccountContext = bankAccountContext;
        _eventStore = eventStore;
    }

    [HttpPost("/bank-accounts")]
    public async Task<IActionResult> AddBankAccount([FromBody] AddBankAccountRequest request)
    {
        var id = Guid.NewGuid().ToString();

        var bankAccount = new BankAccount
        {
            Id = id,
            Name = request.Name,
            AccountNumber = request.AccountNumber,
            SortCode = request.SortCode,
            Balance = 0.0m,
        };

        await _bankAccountContext.BankAccounts.AddAsync(bankAccount);

        var @event = BankAccountAdded.From(bankAccount);
        await _eventStore.WriteEvents(@event.Stream, new [] { @event });

        return Ok(new { Id = id });
    }

    [HttpPatch("/bank-accounts/{id}")]
    public async Task<IActionResult> AddBankAccount([FromRoute] string id, [FromBody] AddBankAccountRequest request)
    {
        var bankAccount = new BankAccount
        {
            Id = id,
            Name = request.Name,
            Balance = 0.0m,
        };

        await _bankAccountContext.BankAccounts.AddAsync(bankAccount);

        var @event = BankAccountAdded.From(bankAccount);
        await _eventStore.WriteEvents(@event.Stream, new [] { @event });

        return Ok(new { Id = id });
    }
}
