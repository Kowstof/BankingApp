using System;

namespace BankingApp
{
    public struct Transaction
    {
        private DateTime _date;
        private string _action;
        private double _amount;
        private double _balance;

        public Transaction(DateTime date, string action, double amount, double balance)
        {
            _date = date;
            _action = action;
            _amount = amount;
            _balance = balance;
        }
    }
}