using System.Collections;

namespace BankingApp
{
    internal class Account
    {
        private readonly int _accountNumber;
        private readonly string _address;
        private readonly string _email;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _phone;
        private readonly ArrayList _transactions = new ArrayList();
        private double _balance;

        public Account(int accountNumber, string firstName, string lastName, string email, string phone, string address,
            double balance)
        {
            _accountNumber = accountNumber;
            _firstName = firstName;
            _lastName = lastName;
            _email = email;
            _phone = phone;
            _address = address;
            _balance = balance;
        }

        /*****************************
         * GET METHODS
         *****************************/

        public int GetAccountNumber()
        {
            return _accountNumber;
        }

        public string GetFirstName()
        {
            return _firstName;
        }

        public string GetLastName()
        {
            return _lastName;
        }

        public string GetEmail()
        {
            return _email;
        }

        public string GetPhone()
        {
            return _phone;
        }

        public string GetAddress()
        {
            return _address;
        }

        public double GetBalance()
        {
            return _balance;
        }

        public ArrayList GetTransactions()
        {
            return _transactions;
        }

        /*****************************
         * SET METHODS
         *****************************/

        public void SetBalance(double newBalance)
        {
            _balance = newBalance;
        }
    }
}