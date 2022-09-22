using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Mail;

namespace BankingApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logins = new List<Login>();
            var accounts = new List<Account>();

            LoadLogins(logins);
            LoadAccounts(accounts);
            Login(logins, accounts);
        }

        private static void LoadLogins(List<Login> logins)
        {
            var entries = File.ReadAllLines("login.txt");
            foreach (var entry in entries)
            {
                var credentials = entry.Split('|');
                var newLogin = new Login(credentials[0], credentials[1]);
                logins.Add(newLogin);
            }
        }

        private static void LoadAccounts(List<Account> accounts)
        {
            /* Unfortunately I had to specify that account files begin with 'A'. Otherwise, the search pattern would also match shorter
               files names (specifically login.txt), due to some quirk with how windows looks for shortened file names. The official 
               documentation contains an error, saying the '?' wildcard means 'exactly one character' when in reality is acts as 
               'zero or one'. https://stackoverflow.com/a/963408 */

            var accountFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "A??????.txt");

            foreach (var accountFile in accountFiles)
            {
                var allLines = File.ReadAllLines(accountFile);
                var accountData = new string[7]; //First 7 fields are data, rest are transactions

                // Separate data from label, keep only data
                for (var i = 0; i < 7; i++) accountData[i] = allLines[i].Split('|')[1];

                var accountId = Convert.ToInt32(accountData[0]);
                var firstName = Convert.ToString(accountData[1]);
                var lastName = Convert.ToString(accountData[2]);
                var address = Convert.ToString(accountData[3]);
                var phone = Convert.ToString(accountData[4]);
                var email = Convert.ToString(accountData[5]);
                var balance = Convert.ToDouble(accountData[6]);
                var loadedAccount = new Account(accountId, firstName, lastName, address, phone, email, balance);

                // Process and convert transaction lines
                for (var i = 7; i < allLines.Length; i++)
                {
                    var transactionData = allLines[i].Split('|');
                    var date = DateTime.ParseExact(transactionData[0], "dd.MM.yyyy", null);
                    var action = transactionData[1];
                    var amount = Convert.ToDouble(transactionData[2]);
                    var balanceRecord = Convert.ToDouble(transactionData[3]);

                    loadedAccount.AddTransaction(date, action, amount, balanceRecord);
                }

                accounts.Add(loadedAccount);
            }
        }

        private static void Login(List<Login> logins, List<Account> accounts)
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

                foreach (var login in logins)
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
            MainMenu(accounts);
        }

        private static void MainMenu(List<Account> accounts)
        {
            while (true)
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
                        CreateAccount(accounts);
                        break;
                    case 2:
                        FindAccount(accounts);
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
                        Exit();
                        break;
                }
            }
        }

        private static void CreateAccount(List<Account> accounts)
        {
            while (true)
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

                while (true)
                {
                    // Move on if all good
                    if (ValidatePhone(phone))
                    {
                        Console.SetCursorPosition(0, 12);
                        Console.Write(new string(' ', Console.WindowWidth)); // Clear Error Message
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
                        Console.Write(new string(' ', Console.WindowWidth)); // Clear Error Message
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

                var choice = YesNoChoice("Is the information correct? (y/n): ");
                if (choice == "n") continue;
                
                // Create the new account
                var numAccounts = accounts.Count;
                var latest = accounts[numAccounts - 1].AccountNumber;
                var newAccNumber = latest + 1;
                var newAccount = new Account(newAccNumber, firstName, lastName, address, phone, email, 0.0);
                accounts.Add(newAccount);

                // Write new account file
                string[] textFileLines =
                {
                    $"AccountNo|{newAccNumber}",
                    $"First Name|{firstName}",
                    $"Last Name|{lastName}",
                    $"Address|{address}",
                    $"Phone|{phone}",
                    $"Email|{email}",
                    "Balance|0"
                };
                File.WriteAllLines($"A{newAccNumber}.txt", textFileLines);

                // Send confirmation email
                var sendFrom = "krystofpavlis2@gmail.com";
                var sendTo = email;
                var subject = $"Account {newAccNumber} was created successfully!";
                var body = $@"
                    <h3>Account summary</h3>
                    <p>Account number: {newAccNumber}</p>
                    <p>First Name: {firstName}</p>
                    <p>Last Name: {lastName}</p>
                    <p>Address: {address}</p>
                    <p>Phone Number: {phone}</p>
                    <p>Email: {email}</p>
                    <p>Initial Balance: $0.00</p>";

                try
                {
                    var smtpServer = new SmtpClient("smtp.gmail.com", 587);
                    smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                    var mail = new MailMessage();
                    mail.From = new MailAddress(sendFrom);
                    mail.To.Add(sendTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    smtpServer.Timeout = 5000;
                    smtpServer.EnableSsl = true;
                    smtpServer.UseDefaultCredentials = false;
                    smtpServer.Credentials = new NetworkCredential(sendFrom, "tflnyvdwaosotdwo");
                    smtpServer.Send(mail);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.ReadKey();
                }

                Console.WriteLine(
                    $"Account {newAccNumber} successfully created! A confirmation email has been sent.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
        }

        private static void FindAccount(List<Account> accounts)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════╗");
                Console.WriteLine("|                 FIND AN ACCOUNT               |");
                Console.WriteLine("|═══════════════════════════════════════════════|");
                Console.WriteLine("|          ENTER 6-DIGIT ACCOUNT NUMBER         |");
                Console.WriteLine("|                                               |");
                Console.WriteLine("|    Number:                                    |");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");

                Console.SetCursorPosition(13, 5);
                var account = SearchAccounts(accounts);
                if (account != null) // if a matching account was found
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Account found!");
                    Console.ForegroundColor = ConsoleColor.White;
                    account.AccountSummary();
                    var again = YesNoChoice("Find another account? (y/n): ");
                    if (again == "y") continue;
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Account not found!");
                Console.ForegroundColor = ConsoleColor.White;
                var tryAgain = YesNoChoice("Find another account? (y/n): ");
                if (tryAgain == "y") continue;
                return;
            }
        }


        private static Account SearchAccounts(List<Account> accounts)
        {
            while (true)
            {
                var initialCursorTop = Console.CursorTop;
                var initialCursorLeft = Console.CursorLeft;
                var query = Console.ReadLine();
                if (query != null && Regex.IsMatch(query, "[0-9]{6}")) // If the input is 6 numbers...
                {
                    Console.SetCursorPosition(0, initialCursorTop + 3);
                    Console.Write(new string(' ', Console.WindowWidth)); // clear any error messages
                    var queryNum = Convert.ToInt32(query); // after regex check we can assume it will parse
                    foreach (var account in accounts)
                        return queryNum == account.AccountNumber
                            ? account
                            : null; // return either the found account or null
                }

                // If incorrect format
                Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
                Console.Write(new string(' ',
                    Console.WindowWidth -
                    initialCursorLeft)); // Fills in rest of line with blank spaces without spilling to next line
                Console.SetCursorPosition(0, initialCursorTop + 3); // Put the cursor below the box
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect account number format");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static string YesNoChoice(string query)
        {
            Console.Write(query);
            var tempCursorTop = Console.CursorTop;
            while (true)
            {
                var choice = Console.ReadLine();
                if (choice != null && (choice.Equals("y") || choice.Equals("n")))
                {
                    Console.SetCursorPosition(0, tempCursorTop + 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    return choice;
                }

                Console.SetCursorPosition(0, tempCursorTop + 1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please type 'y' or 'n'");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, tempCursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, tempCursorTop);
                Console.Write(query);
            }
        }

        private static bool ValidateEmail(string email)
        {
            var emailPattern = new Regex("^.+[@]((?:outlook\\.com)|(?:gmail\\.com)|(?:uts\\.edu\\.au))$");
            return emailPattern.IsMatch(email);
        }

        private static bool ValidatePhone(string phone)
        {
            var phonePattern = new Regex("^[0-9]{10}$");
            return phonePattern.IsMatch(phone);
        }

        private static void DeleteAccount()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("|                DELETE AN ACCOUNT              |");
            Console.WriteLine("|═══════════════════════════════════════════════|");
            Console.WriteLine("|          ENTER 6-DIGIT ACCOUNT NUMBER         |");
            Console.WriteLine("|                                               |");
            Console.WriteLine("|    Number:                                    |");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
        }

        private static void Exit()
        {
            Console.Clear();
            Console.WriteLine("Thank you for using Krystof's banking system! Exiting...");
            Thread.Sleep(1500);
            Environment.Exit(1);
        }
    }
}