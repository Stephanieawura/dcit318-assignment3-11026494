using System;
using System.Collections.Generic;

// --- Record definition (immutable) ---
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// --- Interface for processors ---
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// --- Concrete processors ---
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Sent {transaction.Amount:C} for {transaction.Category}");
    }
}

// --- Base Account class ---
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    // Default behavior: deduct amount (no validation)
    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Applied transaction (base). Balance is now: {Balance:C}");
    }
}

// --- Sealed SavingsAccount (cannot be inherited further) ---
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }
}

// --- Finance application that coordinates everything ---
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        // i) Create a SavingsAccount
        var account = new SavingsAccount("SA-1001", 1000m);
        Console.WriteLine($"Created SavingsAccount {account.AccountNumber} with balance {account.Balance:C}\n");

        // ii) Create three Transaction records
        var t1 = new Transaction(1, DateTime.Now, 200m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 150m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 50m, "Entertainment");

        // iii) Prepare processors and process each
        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();
        ITransactionProcessor p3 = new CryptoWalletProcessor();

        // Process and apply transaction 1
        p1.Process(t1);
        account.ApplyTransaction(t1);
        _transactions.Add(t1);
        Console.WriteLine();

        // Process and apply transaction 2
        p2.Process(t2);
        account.ApplyTransaction(t2);
        _transactions.Add(t2);
        Console.WriteLine();

        // Process and apply transaction 3
        p3.Process(t3);
        account.ApplyTransaction(t3);
        _transactions.Add(t3);
        Console.WriteLine();

        // Summary
        Console.WriteLine("Transactions recorded:");
        foreach (var t in _transactions)
        {
            Console.WriteLine($"- ID:{t.Id} {t.Category} {t.Amount:C} on {t.Date}");
        }

        Console.WriteLine($"\nFinal account balance: {account.Balance:C}");
    }
}

// --- Program entry point ---
class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
