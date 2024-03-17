using EventSourcing.Aggregates.API.Data.EventStore;
using EventSourcing.Aggregates.API.Requests;
using EventSourcing.EventStoreDB.Common;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.Aggregates.API;

[ApiController]
public class BankAccountsController : ControllerBase
{
    private readonly IEventStore _eventStore;

    public BankAccountsController(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    [HttpGet("/bank-accounts/{id}")]
    public async Task<IActionResult> GetBankAccount([FromRoute] string id)
    {
        var bankAccount = await LoadAggregate(id);

        return bankAccount.State.Status is not BankAccountStatus.DoesNotExist ?
            Ok(bankAccount.State) :
            NotFound();
    }

    [HttpPost("/bank-accounts")]
    public async Task<IActionResult> AddBankAccount([FromBody] AddBankAccountRequest request)
    {
        var id = Guid.NewGuid().ToString();

        var bankAccount = await LoadAggregate(id);
        bankAccount.AddBankAccount(id, request.Name, request.AccountNumber, request.SortCode);
        await SaveAggregate(bankAccount);

        return Ok(new { Id = id });
    }

    [HttpPost("/bank-accounts/{id}/actions/deposit")]
    public async Task<IActionResult> DepositCash([FromRoute] string id, [FromBody] DepositRequest request)
    {
        var bankAccount = await LoadAggregate(id);
        bankAccount.DepositCash(request.Amount);
        await SaveAggregate(bankAccount);

        return Ok();
    }

    private async Task<BankAccountAggregate> LoadAggregate(string id)
    {
        var events = await _eventStore.GetEvents($"{StreamNames.BankAccounts}-{id}");
        return new BankAccountAggregate(events);
    }

    private async Task SaveAggregate(BankAccountAggregate aggregate)
    {
        var streamName = $"{StreamNames.BankAccounts}-{aggregate.State.Id}";
        await _eventStore.WriteEvents(streamName, aggregate.NewEvents);
    }
}
