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
            foreach (var accountFile in accountFiles)
            {
                var allLines = File.ReadAllLines(accountFile);
                var accountData = new ArrayList();
                for (var i = 0; i < 6; i++)
                {
                    accountData[i] = allLines[i].Split('|')[1];
                }

                var accountId = Convert.ToInt32(accountData[0]);
                var firstName = Convert.ToString(accountData[1]);
                var lastName = Convert.ToString(accountData[2]);
                var address = Convert.ToString(accountData[3]);
                var phone = Convert.ToString(accountData[4]);
                var email = Convert.ToString(accountData[5]);
                var balance = Convert.ToDouble(accountData[6]);
                var loadedAccount = new Account(accountId, firstName, lastName, address, phone, email, balance);

                accounts.Add(loadedAccount);
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
            var error = "r";
            while (!valid)
            {
                //Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════╗");
                Console.WriteLine("|        WELCOME TO SIMPLE BANKING SYSTEM       |");
                Console.WriteLine("|═══════════════════════════════════════════════|");
                Console.WriteLine("|                 LOGIN TO START                |");
                Console.WriteLine("|                                               |");
                Console.WriteLine("|    Username:                                  |");
                Console.WriteLine("|    Password:                                  |");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = ConsoleColor.White;
                
                Console.SetCursorPosition(15, 5);
                var username = Console.ReadLine();
                Console.SetCursorPosition(15, 6);
                var password = Console.ReadLine();

                foreach (Login login in logins)
                    if (login.Validate(username, password))
                        valid = true;
                    else
                        error = "Incorrect username or password";
            }

            Console.Write("Logged in!");
        }
    }
}