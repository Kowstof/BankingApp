using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace BankingApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logins = new ArrayList();
            var accounts = new ArrayList();

            LoadLogins(logins);
            LoadAccounts(accounts);
            Login(logins);
            Console.ReadKey();
        }

        private static void LoadLogins(ArrayList logins)
        {
            var entries = File.ReadAllLines("login.txt");
            foreach (var entry in entries)
            {
                var credentials = entry.Split('|');
                var newLogin = new Login(credentials[0], credentials[1]);
                logins.Add(newLogin);
            }
        }

        private static void LoadAccounts(ArrayList accounts)
        {
            var accountFiles = Directory.GetFiles("", "??????.txt");
            foreach (var file in accountFiles)
            {
                /*var fileOutput = File.ReadAllLines(file);
                var accountData = new ArrayList();
                for (int i = 0; i < fileOutput.Length; i++)
                {
                    accountData[i] = fileOutput[i].Split('|')[1];
                }

                
                for (int i = 0; i < account.Length; i++)
                {
                    var newAccount = new Account(account[0], )
                }*/
            }
        }

        private static void CreateAccount()
        {
            Console.WriteLine("Please Create a new account");
            Console.Write("First name: ");
            var firstName = Console.ReadLine();
            Console.Write("Last Name: ");
            var lastName = Console.ReadLine();
            Console.Write("Address: ");
            var address = Console.ReadLine();
            Console.Write("Phone: ");
            var phone = Console.ReadLine();
            Console.Write("Email: ");
            var email = Console.ReadLine();
        }

        private static bool ValidateEmail(string email)
        {
            const string emailPattern = "/^.+[@]((?:outlook\\.com)|(?:gmail\\.com)|(?:uts\\.edu\\.au))$/gm";
            var result = Regex.Match(email, emailPattern);
            return result.Success;
        }

        private static bool ValidatePhone(string phone)
        {
            const string phonePattern = "^[0-9]{10}$";
            var result = Regex.Match(phone, phonePattern);
            return result.Success;
        }

        private static void Login(ArrayList logins)
        {
            var valid = false;
            while (!valid)
            {
                Console.WriteLine("Welcome to Krystof Bank!");
                Console.Write("Customer ID: ");
                var username = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();

                foreach (Login login in logins)
                    if (login.Validate(username, password))
                        valid = true;
            }

            Console.Write("Logged in!");
        }
    }
}