using EventSourcing.EventStoreDB.Common;
using EventSourcing.WithoutAggregates.API.Data.EventStore;
using EventSourcing.WithoutAggregates.API.Data.Sql;
using EventSourcing.WithoutAggregates.API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("/bank-accounts/{id}")]
    public async Task<IActionResult> GetBankAccount([FromRoute] string id)
    {
        var bankAccount = await _bankAccountContext.BankAccounts.FirstOrDefaultAsync(account => account.Id == id);

        return bankAccount is not null ?
            Ok(bankAccount) :
            NotFound();
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
        await _bankAccountContext.SaveChangesAsync();

        var @event = BankAccountAdded.From(bankAccount);
        await _eventStore.WriteEvents(@event.Stream, new [] { @event });

        return Ok(new { Id = id });
    }

    [HttpPost("/bank-accounts/{id}/actions/deposit")]
    public async Task<IActionResult> DepositCash([FromRoute] string id, [FromBody] DepositRequest request)
    {
        var bankAccount = await _bankAccountContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == id);

        if (bankAccount is null)
        {
            return BadRequest();
        }

        bankAccount.Balance += request.Amount;

        _bankAccountContext.BankAccounts.Update(bankAccount);
        await _bankAccountContext.SaveChangesAsync();

        var @event = CashDeposited.From(bankAccount, request.Amount);
        await _eventStore.WriteEvents(@event.Stream, new [] { @event });

        return Ok(new { bankAccount.Balance });
    }
}
