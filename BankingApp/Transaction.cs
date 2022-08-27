using System;

namespace BankingApp
{
    public class Transaction
    {
        private DateTime date;
        private double amount;

        public Transaction(DateTime date, double amount)
        {
            this.date = date;
            this.amount = amount;
        }
        
        
    }
}