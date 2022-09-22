using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BankingApp
{
    public class Account
    {
        //private readonly int _accountNumber;
        public int AccountNumber { get; }
        private readonly string _address;
        private readonly string _email;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _phone;
        private List<Transaction> _transactions = new List<Transaction>();
        private double _balance;

        public Account(int accountNumber, string firstName, string lastName, string address, string phone, string email,
            double balance)
        {
            AccountNumber = accountNumber;
            _firstName = firstName;
            _lastName = lastName;
            _email = email;
            _phone = phone;
            _address = address;
            _balance = balance;
        }

        public void Deposit(double amount)
        {
            var balance = _balance + amount;
            AddTransaction(DateTime.Now, "Deposit", amount, balance);
        }
        
        public void Withdraw(double amount)
        {
            var balance = _balance - amount;
            AddTransaction(DateTime.Now, "Withdraw", amount, balance);
        }

        public void AddTransaction(DateTime date, string action, double amount, double balance)
        {
            if (action == "Deposit")
                _balance += amount;
            else
                _balance -= amount;
            
            var newTransaction = new Transaction(date, action, amount, balance);
            _transactions.Add(newTransaction);

            var transactionText = $"{date}|{action}|{amount}|{balance}";
            File.AppendAllText($"A{AccountNumber}.txt", transactionText + Environment.NewLine);
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