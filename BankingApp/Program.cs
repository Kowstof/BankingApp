using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

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
            /* Unfortunately I had to specify that account files begin with 'A'. Otherwise, the search pattern would also match shorter
               files names (specifically login.txt), due to some quirk with how windows looks for shortened file names. The official 
               documentation contains an error, saying the '?' wildcard means 'exactly one character' when in reality is acts as 
               'zero or one'. https://stackoverflow.com/a/963408 */

            var accountFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "A??????.txt");

            foreach (var accountFile in accountFiles)
            {
                var allLines = File.ReadAllLines(accountFile);
                var accountData = new string[7];
                for (var i = 0; i < 7; i++) accountData[i] = allLines[i].Split('|')[1];

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
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("|              CREATE A NEW ACCOUNT             |");
            Console.WriteLine("|═══════════════════════════════════════════════|");
            Console.WriteLine("|              ENTER ACCOUNT DETAILS            |");
            Console.WriteLine("|                                               |");
            Console.WriteLine("|    First Name:                                |");
            Console.WriteLine("|    Last Name:                                 |");
            Console.WriteLine("|    Address:                                   |");
            Console.WriteLine("|    Phone:                                     |");
            Console.WriteLine("|    Email:                                     |");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");

            Console.SetCursorPosition(17, 5);
            var firstName = Console.ReadLine();
            Console.SetCursorPosition(16, 6);
            var lastName = Console.ReadLine();
            Console.SetCursorPosition(14, 7);
            var address = Console.ReadLine();
            Console.SetCursorPosition(12, 8);
            var phone = Console.ReadLine();
  
            while(true)
            {
                // Move on if all good
                if (ValidatePhone(phone))
                {
                    Console.SetCursorPosition(0, 12);
                    Console.Write(""); // Clear Error Message
                    break;
                }
                // Display Error
                Console.SetCursorPosition(0, 12);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Phone number format incorrect");
                Console.ForegroundColor = ConsoleColor.White;
                // Try again
                Console.SetCursorPosition(0, 8);
                Console.Write("|    Phone:                                     |");
                Console.SetCursorPosition(12, 8);
                phone = Console.ReadLine();
            }
            
            Console.SetCursorPosition(12, 9);
            var email = Console.ReadLine();
            
            while (true)
            {
                // Move on if all good
                if (ValidateEmail(email))
                {
                    Console.SetCursorPosition(0, 12);
                    Console.Write(""); // Clear Error Message
                    break;
                }
                // Display Error
                Console.SetCursorPosition(0, 12);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Email format incorrect");
                Console.ForegroundColor = ConsoleColor.White;
                // Try again
                Console.SetCursorPosition(0, 9);
                Console.Write("|    Email:                                     |");
                Console.SetCursorPosition(12, 9);
                email = Console.ReadLine();
            }
            Console.WriteLine();
            Console.WriteLine("All Good!");
            Console.ReadKey();
        }

        private static bool ValidateEmail(string email)
        {
            Regex emailPattern = new Regex("^.+[@]((?:outlook\\.com)|(?:gmail\\.com)|(?:uts\\.edu\\.au))$");
            return emailPattern.IsMatch(email);
        }

        private static bool ValidatePhone(string phone)
        {
            Regex phonePattern = new Regex("^[0-9]{10}$");
            return phonePattern.IsMatch(phone);
        }

        private static void Login(ArrayList logins)
        {
            var valid = false;
            var error = "";
            while (!valid)
            {
                Console.Clear();
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

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Logged in!");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(1000);
            MainMenu();
        }

        private static void MainMenu()
        {
            var valid = false;
            var error = "";
            var choice = "";

            while (!valid)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════╗");
                Console.WriteLine("|        WELCOME TO SIMPLE BANKING SYSTEM       |");
                Console.WriteLine("|═══════════════════════════════════════════════|");
                Console.WriteLine("|    1. Create a new account                    |");
                Console.WriteLine("|    2. Search for an account                   |");
                Console.WriteLine("|    3. Deposit                                 |");
                Console.WriteLine("|    4. Withdraw                                |");
                Console.WriteLine("|    5. Account statement                       |");
                Console.WriteLine("|    6. Delete account                          |");
                Console.WriteLine("|    7. Exit                                    |");
                Console.WriteLine("|═══════════════════════════════════════════════|");
                Console.WriteLine("|    Enter your choice:                         |");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(24, 11);
                choice = Console.ReadLine();
                if (choice != null && Regex.IsMatch(choice, "[1-7]"))
                    valid = true;
                else
                    error = "Please enter a selection between 1 and 7";
            }

            switch (Convert.ToInt32(choice))
            {
                case 1:
                    CreateAccount();
                    break;
                case 2:
                    Console.WriteLine("fg");
                    break;
                case 3:
                    Console.WriteLine("Yeah");
                    break;
                case 4:
                    Console.WriteLine("fg");
                    break;
                case 5:
                    Console.WriteLine("Yeah");
                    break;
                case 6:
                    Console.WriteLine("fg");
                    break;
                case 7:
                    Console.WriteLine("fg");
                    break;
            }
        }
    }
}