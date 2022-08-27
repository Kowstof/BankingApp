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
         * GET/SET
         *****************************/

        public int AccountNumber => _accountNumber;
        public string Address => _address;
        public string Email => _email;
        public string FirstName => _firstName;
        public string LastName => _lastName;
        public string Phone => _phone;
        public ArrayList Transactions => _transactions;

        public double Balance
        {
            get => _balance;
            set => _balance = value;
        }
    }
}