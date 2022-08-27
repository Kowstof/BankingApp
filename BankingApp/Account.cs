using System.Collections;

namespace BankingApp
{
    public class Account
    {
        private readonly int _accountNumber;
        private readonly string _address;
        private readonly string _email;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _phone;
        private readonly ArrayList _transactions = new ArrayList();
        private double _balance;

        public Account(int accountNumber, string firstName, string lastName, string address, string phone, string email,
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
    }
}