namespace BankingApp
{
    public class Account
    {
        private int accountNumber;
        private string firstName;
        private string lastName;
        private string email;
        private string phone;
        private string address;
        private double balance;

        public Account(int accountNumber, string firstName, string lastName, string email, string phone, string address,
            double balance)
        {
            this.accountNumber = accountNumber;
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.phone = phone;
            this.address = address;
            this.balance = balance;
        }
        
        /*****************************
         * GET METHODS
         *****************************/

        public int GetAccountNumber()
        {
            return accountNumber;
        }

        public string GetFirstName()
        {
            return firstName;
        }

        public string GetLastName()
        {
            return lastName;
        }

        public string GetEmail()
        {
            return email;
        }

        public string GetPhone()
        {
            return phone;
        }

        public string GetAddress()
        {
            return address;
        }

        public double GetBalance()
        {
            return balance;
        }
        
        /*****************************
         * SET METHODS
         *****************************/

        public void SetBalance(double newBalance)
        {
            balance = newBalance;
        }
    }
}