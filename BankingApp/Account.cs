using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankingApp
{
    public class Account
    {
        public int AccountNumber { get; }
        private readonly string _address;
        public string Email { get; }
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _phone;
        private readonly List<Transaction> _transactions = new List<Transaction>();
        public double Balance { get; private set; }
        
        public Account(int accountNumber, string firstName, string lastName, string address, string phone, string email,
            double balance)
        {
            AccountNumber = accountNumber;
            _firstName = firstName;
            _lastName = lastName;
            Email = email;
            _phone = phone;
            _address = address;
            Balance = balance;
        }

        public void Deposit(double amount)
        {
            var date = DateTime.Now;
            const string action = "Deposit";
            Balance += amount;
            AddTransaction(date, action, amount, Balance);
            
            var transactionText = $"{date:dd.MM.yyyy}|{action}|{amount}|{Balance}";
            File.AppendAllText($"A{AccountNumber}.txt", transactionText + Environment.NewLine);
            UpdateBalanceOnFile();
        }
        
        public void Withdraw(double amount)
        {
            var date = DateTime.Now;
            const string action = "Withdraw";
            Balance -= amount;
            
            AddTransaction(date, action, amount, Balance);
            
            var transactionText = $"{date:dd.MM.yyyy}|{action}|{amount}|{Balance}";
            File.AppendAllText($"A{AccountNumber}.txt",transactionText + Environment.NewLine);
            UpdateBalanceOnFile();
        }

        public void AddTransaction(DateTime date, string action, double amount, double balance)
        {
            var newTransaction = new Transaction(date, action, amount, balance);
            _transactions.Add(newTransaction);
        }

        private void UpdateBalanceOnFile()
        {
            var fileLines = File.ReadAllLines($"A{AccountNumber}.txt");
            fileLines[6] = $"Balance|{Balance}";
            File.WriteAllLines($"A{AccountNumber}.txt", fileLines);
        }

        public void GenerateSummary()
        {
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("|                 ACCOUNT DETAILS               |");
            Console.WriteLine("|═══════════════════════════════════════════════|");
            Console.WriteLine("|                                               |");
            Console.WriteLine("|    First Name:                                |");
            Console.WriteLine("|    Last Name:                                 |");
            Console.WriteLine("|    Address:                                   |");
            Console.WriteLine("|    Phone:                                     |");
            Console.WriteLine("|    Email:                                     |");
            Console.WriteLine("|    Balance:                                   |");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");

            // This whole thing has to be done so the box keep its shape and alignment.
            var endCursorTop = Console.CursorTop;
            Console.SetCursorPosition(17, endCursorTop - 7);
            Console.Write(_firstName);
            Console.SetCursorPosition(16, endCursorTop - 6);
            Console.Write(_lastName);
            Console.SetCursorPosition(14, endCursorTop - 5);
            Console.Write(_address);
            Console.SetCursorPosition(12, endCursorTop - 4);
            Console.Write(_phone);
            Console.SetCursorPosition(12, endCursorTop - 3);
            Console.Write(Email);
            Console.SetCursorPosition(14, endCursorTop - 2);
            Console.Write(Balance.ToString("c2")); // currency with 2 decimal places
            Console.SetCursorPosition(0, endCursorTop); // Reset cursor position
        }

        public string GenerateEmailSummary()
        {
            var body = $@"
                <h3>Account Details</h3>
                <ul>
                    <li>Account number: {AccountNumber}</li>
                    <li>First Name: {_firstName}</li>
                    <li>Last Name: {_lastName}</li>
                    <li>Address: {_address}</li>
                    <li>Phone Number: {_phone}</li>
                    <li>Email: {Email}</li>
                    <li>Balance: {Balance:c2}</li>
                </ul>";
            return body;
        }

        public void GenerateStatement()
        {
            GenerateSummary();
            Console.WriteLine();
            Console.WriteLine("Your 5 most recent transactions:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Date  |  Action  |  Amount  |  Balance");
            Console.ForegroundColor = ConsoleColor.White;
            var transactionsReversed = _transactions.AsEnumerable().Reverse().ToList();
            var maxCount = transactionsReversed.Count;
            if (maxCount > 5)
                maxCount = 5;
            for (var i = 0; i < maxCount; i++)
            {
                Console.WriteLine(transactionsReversed[i].Print());
            }
        }

        public string GenerateEmailStatement()
        {
            var summary = GenerateEmailSummary();
            var tables = $@"";
            var transactionsReversed = _transactions.AsEnumerable().Reverse().ToList();
            var maxCount = transactionsReversed.Count;
            if (maxCount > 5)
                maxCount = 5;

            for (var i = 0; i < maxCount; i++)
            {
                tables += transactionsReversed[i].PrintEmail();
            }
            
            var body = $@"
                <h2>Your Statement</h2>
                {summary}
                <h3>Most Recent Transactions</h3>
                <table>
                <thead>
                  <tr>
                    <th>Date<br></th>
                    <th>Action</th>
                    <th>Amount</th>
                    <th>Balance</th>
                  </tr>
                </thead>
                <tbody>
                {tables}
                </tbody>
                </table>";
            return body;
        }
    }
}