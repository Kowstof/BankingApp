using System;

namespace BankingApp
{
    public class Transaction
    {
        private DateTime _date;
        private readonly string _action;
        private readonly double _amount;
        private readonly double _balance;

        public Transaction(DateTime date, string action, double amount, double balance)
        {
            _date = date;
            _action = action;
            _amount = amount;
            _balance = balance;
        }

        public string Print()
        {
            var date = _date.ToString("dd.MM.yyyy");
            return $"{date} | {_action} | {_amount:c2} | {_balance:c2}"; // 'c2' = currency format, 2 decimal places
        }

        public string PrintEmail()
        {
            var date = _date.ToString("dd.MM.yyyy");
            var row = $@"
            <tr>
                <td>{date}</td>
                <td>{_action}</td>
                <td>{_amount:c2}</td>
                <td>{_balance:c2}</td>
            </tr>";
            return row;
        }
    }
}