using System.Collections.Immutable;
using EventSourcing.EventStoreDB.Common;

namespace EventSourcing.Aggregates.API.Data.EventStore;

public class BankAccountAggregate
{
    public const string Stream = StreamNames.BankAccounts;

    public IImmutableList<Event> ExistingEvents { get; private set; }
    public IList<Event> NewEvents { get; private set; }

    public BankAccountState State { get; private set; }

    public BankAccountAggregate(IEnumerable<Event> loadedEvents)
    {
        ExistingEvents = loadedEvents.ToImmutableList();
        NewEvents = new List<Event>();

        foreach (var @event in ExistingEvents)
        {
            Apply(@event);
        }
    }

    public void AddBankAccount(string id, string name, string accountNumber, string sortCode)
    {
        if (State.Status is not BankAccountStatus.DoesNotExist)
        {
            throw new InvalidOperationException($"The bank account with ID {id} already exists");
        }

        var maskedAccountNumber = $"******{accountNumber[^2..]}";
        var maskedSortCode = $"**-**-{sortCode[^2..]}";

        var @event = new BankAccountAdded(id, name, maskedAccountNumber, maskedSortCode)
        {
            Timestamp = DateTime.UtcNow
        };

        RaiseEvent(@event);
    }

    public void DepositCash(decimal amount)
    {
        var updatedBalance = State.Balance + amount;
        var @event = new CashDeposited(State.Id, amount, State.Balance, updatedBalance)
        {
            Timestamp = DateTime.UtcNow
        };

        RaiseEvent(@event);
    }

    private void RaiseEvent<TEvent>(TEvent @event) where TEvent : Event
    {
        Apply(@event);
        NewEvents.Add(@event);
    }

    private void Apply<TEvent>(TEvent @event) where TEvent : Event
    {
        var updatedState = @event switch
        {
            BankAccountAdded addedEvent => ApplyBankAccountAdded(addedEvent),
            CashDeposited depositEvent => ApplyCashDeposited(depositEvent),
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, $"Unknown event type: {@event.GetType()}")
        };

        State = updatedState;
    }

    private static BankAccountState ApplyBankAccountAdded(BankAccountAdded addedEvent)
    {
        return new BankAccountState(
            Id: addedEvent.Id,
            Name: addedEvent.Name,
            MaskedAccountNumber: addedEvent.MaskedAccountNumber,
            MaskedSortCode: addedEvent.MaskedSortCode,
            Balance: 0.0m,
            Status: BankAccountStatus.Open);
    }

    private BankAccountState ApplyCashDeposited(CashDeposited depositEvent)
    {
        return State with
        {
            Balance = depositEvent.UpdatedBalance
        };
    }
}
