using Serilog;
using Serilog.Sinks.Logtube;
using System.Security.Cryptography;

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Logtube()
    .CreateLogger();

Log.Logger = log;

var logger = new EventLogger();
var executor = new TransactionExecutor(logger);

foreach (var transaction in Generate())
{
    executor.Execute(transaction);
    Thread.Sleep(RandomNumberGenerator.GetInt32(20, 500));
}

IEnumerable<WalletTransaction> Generate()
{
    var wallets = Enumerable.Range(0, 10)
        .Select(x => new Wallet(RandomNumberGenerator.GetInt32(100, 1_000)))
        .ToArray();
    while (true)
    {
        var amount = RandomNumberGenerator.GetInt32(100);
        var walletA = wallets[RandomNumberGenerator.GetInt32(0, wallets.Length - 1)];
        var walletB = wallets
            .Where(x => x.Id != walletA.Id)
            .ToArray()[RandomNumberGenerator.GetInt32(0, wallets.Length - 1)];

        yield return new WalletTransaction(walletA, walletB, amount);
    }
}

class EventLogger
{
    public void WalletIncreased(Guid walletId, int amount, int newBalance)
    {
        Log.Information("{EventName} {WalletId} {Amount} {NewBalance}", 
            nameof(WalletIncreased), walletId, amount, newBalance);
    }

    public void WalletDecreased(Guid walletId, int amount, int newBalance)
    {
        Log.Information("{EventName} {WalletId} {Amount} {NewBalance}",
            nameof(WalletDecreased), walletId, amount, newBalance);
    }

    public void WalletNotEnoughBalance(Guid walletId, int amount, int currentBalance)
    {
        Log.Information("{EventName} {WalletId} {Amount} {CurrentBalance}",
            nameof(WalletNotEnoughBalance), walletId, amount, currentBalance);
    }
}

class Entity
{
    public Guid Id { get; } = Guid.NewGuid();
}

class Wallet(int startBalance = 0) : Entity
{
    public int Balance { get; private set; } = startBalance;

    public void Increase(int amount) 
    { 
        this.Balance += amount;
    }

    public bool TryDecrease(int amount)
    {
        var result = this.Balance - amount;
        if (result < 0)
        {
            return false;
        }

        this.Balance = result;
        return true;
    }
}

class WalletTransaction(Wallet from, Wallet to, int amount) : Entity
{
    public Wallet From { get; } = from;
    public Wallet To { get; } = to;
    public int Amount { get; } = amount;
}

class TransactionExecutor(EventLogger logger)
{
    public void Execute(WalletTransaction trans)
    {
        if(trans.From.TryDecrease(trans.Amount))
        {
            logger.WalletDecreased(trans.From.Id, trans.Amount, trans.From.Balance);
            trans.To.Increase(trans.Amount);
            logger.WalletIncreased(trans.To.Id, trans.Amount, trans.To.Balance);
        }
        else
        {
            logger.WalletNotEnoughBalance(trans.From.Id, trans.Amount, trans.From.Balance);
        }
    }
}