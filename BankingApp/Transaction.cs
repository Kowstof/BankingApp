using System;

namespace BankingApp
{
    public struct Transaction
    {
        private DateTime _date;
        private double _amount;

        public Transaction(DateTime date, double amount)
        {
            _date = date;
            _amount = amount;
        }
    }
}