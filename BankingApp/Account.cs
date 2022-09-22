using System;
using System.Collections.Generic;
using System.IO;

namespace BankingApp
{
    public class Account
    {
        public int AccountNumber { get; }
        private readonly string _address;
        private readonly string _email;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _phone;
        private List<Transaction> _transactions = new List<Transaction>();
        public double Balance { get; private set; }
        
        public Account(int accountNumber, string firstName, string lastName, string address, string phone, string email,
            double balance)
        {
            AccountNumber = accountNumber;
            _firstName = firstName;
            _lastName = lastName;
            _email = email;
            _phone = phone;
            _address = address;
            Balance = balance;
        }

        public void Deposit(double amount)
        {
            var date = DateTime.Now;
            var action = "Deposit";
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

        public void AccountSummary()
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
            Console.WriteLine("╚═══════════════════════════════════════════════╝");

            // This whole thing has to be done so the box keep its shape and alignment.
            var endCursorTop = Console.CursorTop;
            Console.SetCursorPosition(17, endCursorTop - 6);
            Console.Write(_firstName);
            Console.SetCursorPosition(16, endCursorTop - 5);
            Console.Write(_lastName);
            Console.SetCursorPosition(14, endCursorTop - 4);
            Console.Write(_address);
            Console.SetCursorPosition(12, endCursorTop - 3);
            Console.Write(_phone);
            Console.SetCursorPosition(12, endCursorTop - 2);
            Console.Write(_email);
            Console.SetCursorPosition(0, endCursorTop); // Reset cursor position
        }
    }
}