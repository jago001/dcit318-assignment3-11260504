// Record to represent financial transactions
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface for transaction processing
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Concrete implementations of ITransactionProcessor
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing bank transfer: Amount: {transaction.Amount:C}, Category: {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing mobile money transfer: Amount: {transaction.Amount:C}, Category: {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing crypto wallet transfer: Amount: {transaction.Amount:C}, Category: {transaction.Category}");
    }
}

// Base Account class
public class Account
{
    public string AccountNumber { get; init; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
    }
}

// Sealed Savings Account class
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) 
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
            return;
        }

        base.ApplyTransaction(transaction);
    }
}

// Finance Application class
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        // Create a savings account
        var savingsAccount = new SavingsAccount("SAV001", 1000m);

        // Create sample transactions
        var transactions = new Transaction[]
        {
            new(1, DateTime.Now, 200m, "Groceries"),
            new(2, DateTime.Now, 150m, "Utilities"),
            new(3, DateTime.Now, 300m, "Entertainment")
        };

        // Create processors
        var mobileMoneyProcessor = new MobileMoneyProcessor();
        var bankTransferProcessor = new BankTransferProcessor();
        var cryptoWalletProcessor = new CryptoWalletProcessor();

        // Process transactions
        Console.WriteLine("\nProcessing transactions...\n");
        
        mobileMoneyProcessor.Process(transactions[0]);
        savingsAccount.ApplyTransaction(transactions[0]);
        _transactions.Add(transactions[0]);

        bankTransferProcessor.Process(transactions[1]);
        savingsAccount.ApplyTransaction(transactions[1]);
        _transactions.Add(transactions[1]);

        cryptoWalletProcessor.Process(transactions[2]);
        savingsAccount.ApplyTransaction(transactions[2]);
        _transactions.Add(transactions[2]);

        Console.WriteLine($"\nFinal balance: {savingsAccount.Balance:C}");
        Console.WriteLine($"Total transactions processed: {_transactions.Count}");
    }
}

// Main program
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Finance Management System\n");
        var app = new FinanceApp();
        app.Run();
    }
}
