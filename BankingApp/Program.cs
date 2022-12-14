using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            LoadLogins(logins, accounts);
            Exit();
        }
        
        /// <summary>
        /// Finds the 'login.txt' file and creates Login objects to add to the global list.
        /// Handles file not found and exits - can't use program without logins.
        /// </summary>

        private static void LoadLogins(List<Login> logins, List<Account> accounts)
        {
            try
            {
                var entries = File.ReadAllLines("login.txt");
                foreach (var entry in entries)
                {
                    var credentials = entry.Split('|');
                    var newLogin = new Login(credentials[0], credentials[1]);
                    logins.Add(newLogin);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                Error(0, "The 'login.txt' file could not be found");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            LoadAccounts(logins, accounts);
        }

        /// <summary>
        /// Unfortunately I had to specify that account files begin with 'A'. Otherwise, the search pattern would also match shorter
        /// files names (specifically login.txt), due to some quirk with how windows looks for shortened file names. The official
        /// documentation contains an error, saying the '?' wildcard means 'exactly one character' when in reality is acts as
        /// 'zero or one'. https://stackoverflow.com/a/963408 
        /// </summary>
        
        private static void LoadAccounts(List<Login> logins, List<Account> accounts)
        {
            var accountFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "A??????.txt");

            try
            {
                foreach (var accountFile in accountFiles) // no need for try-catch as the code gets skipped if the array is empty
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                Error(0, "Existing accounts have been found but their data seems corrupted. Fix or remove the corrupted files.");
            }

            Login(logins, accounts); // after all external files are loaded in, launch login window
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
                Error(0, error);

                Console.SetCursorPosition(15, 5);
                var username = Console.ReadLine();
                Console.SetCursorPosition(15, 6);
                var password = GetPassword();

                foreach (var login in logins)
                    if (login.Validate(username, password))
                        valid = true;
                    else
                        error = "Incorrect username or password";
            }

            Console.Clear();
            Success(0, "Logged in!");
            Thread.Sleep(1000);
            MainMenu(accounts);
        }
        

        private static string GetPassword()
        {
            var password = "";
            while (true)
            {
                var k = Console.ReadKey(true); // captured keystroke
                if (k.Key == ConsoleKey.Enter) break; // enter submits attempt
                if (k.Key == ConsoleKey.Backspace)
                {
                    if (password.Length <= 0) continue; // dont allow user to delete further backward
                    password.Remove(password.Length - 1); // remove deleted char from password
                    Console.Write("\b \b"); // remove deleted char from console
                }
                else if (k.KeyChar != '\u0000' ) // ignoring function keys
                {
                    password += k.KeyChar; //store the keystroke
                    Console.Write("*"); // replace with '*'
                }
            }
            return password;
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
                    if (choice != null && Regex.IsMatch(choice, "^[1-7]$"))
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
                        Deposit(accounts);
                        break;
                    case 4:
                        Withdraw(accounts);
                        break;
                    case 5:
                        Statement(accounts);
                        break;
                    case 6:
                        DeleteAccount(accounts);
                        break;
                    case 7:
                        return;
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
                    Error(4, "Phone number format incorrect");
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
                    Error(3, "Incorrect email format");
                    // Clear prev attempt and try again
                    Console.SetCursorPosition(0, 9);
                    Console.Write("|    Email:                                     |");
                    Console.SetCursorPosition(12, 9);
                    email = Console.ReadLine();
                }

                var choice = YesNoChoice("Is the information correct? (y/n): ");
                if (choice == "n") continue;
                
                // Create the new account
                var numAccounts = accounts.Count;
                var latestAccNum = accounts[numAccounts - 1].AccountNumber;
                var newAccNumber = latestAccNum + 1;
                var newAccount = new Account(newAccNumber, firstName, lastName, address, phone, email, 0.0);
                accounts.Add(newAccount);

                // Write new account .txt file
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
                var subject = $"Account {newAccNumber} was created successfully!";
                var body = newAccount.GenerateEmailSummary();
                
                SendEmail(email, subject, body);
                
                Console.WriteLine($"Account {newAccNumber} successfully created! A confirmation email has been sent.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
        }

        private static void SendEmail(string sendTo, string subject, string body)
        {
            const string sendFrom = "krystofpavlis2@gmail.com";
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
                Console.WriteLine("|                                               |");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");

                Console.SetCursorPosition(13, 5);
                var account = SearchAccounts(accounts);
                if (account != null) // if a matching account was found
                {
                    Console.Clear();
                    Success(0,"Account found!");
                    account.GenerateSummary();
                    var again = YesNoChoice("Find another account? (y/n): ");
                    if (again == "y") continue;
                    return;
                }
                
                Error(3, "Account not found!");
                var tryAgain = YesNoChoice("Find another account? (y/n): ");
                if (tryAgain == "y") continue;
                return;
            }
        }


        // The core account searching function used by most other methods. 
        private static Account SearchAccounts(List<Account> accounts)
        {
            // Remember where the cursor was initially
            var initialCursorTop = Console.CursorTop; 
            var initialCursorLeft = Console.CursorLeft;
            while (true)
            {
                Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
                var query = Console.ReadLine();
                if (query != null && Regex.IsMatch(query, "[0-9]{6}")) // If the input is 6 numbers...
                {
                    Console.SetCursorPosition(0, initialCursorTop + 3); // go under the box borders
                    Console.Write(new string(' ', Console.WindowWidth)); // clear any error messages
                    Console.SetCursorPosition(initialCursorLeft, initialCursorTop); 
                    var queryNum = Convert.ToInt32(query); // after regex check we can assume it will parse
                    return accounts.FirstOrDefault(account => queryNum == account.AccountNumber); // return either a matching account, or null
                }

                // If incorrect format
                Console.SetCursorPosition(0, initialCursorTop);
                Console.Write("|    Number:                                    |"); // Resets the input field
                Error(3, "Incorrect number format");
            }
        }

        private static void Deposit(List<Account> accounts)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════╗");
                Console.WriteLine("|                    DEPOSIT                    |");
                Console.WriteLine("|═══════════════════════════════════════════════|");
                Console.WriteLine("|          ENTER 6-DIGIT ACCOUNT NUMBER         |");
                Console.WriteLine("|                                               |");
                Console.WriteLine("|    Number:                                    |");
                Console.WriteLine("|    Amount: $                                  |");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");
                Console.SetCursorPosition(13, 5);
                var account = SearchAccounts(accounts);
                // If no matching account is found
                if (account == null)
                {
                    Error(3, "Account not found");
                    var choice = YesNoChoice("Find another account? (y/n): ");
                    if (choice == "y") continue;
                    return;
                }
                // If an account is found
                Success(3, "Account found! Please enter amount to deposit");
                while (true)
                {
                    Console.SetCursorPosition(0, 6);
                    Console.Write("|    Amount: $                                  |");
                    Console.SetCursorPosition(14, 6);
                    var amount = Console.ReadLine();
                    if (!double.TryParse(amount, out var amountNum) || amountNum <= 0) // if it doesn't parse as double OR is less than 0
                    {
                        Error(2, "Please enter a number greater than 0");
                        continue;
                    }
                    // If input is validated
                    account.Deposit(amountNum);
                    Console.Clear();
                    Success(0, "Deposit successful!");
                    Thread.Sleep(1000);
                    return;
                }
            }
        }
        
        private static void Withdraw(List<Account> accounts)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════╗");
                Console.WriteLine("|                     WITHDRAW                  |");
                Console.WriteLine("|═══════════════════════════════════════════════|");
                Console.WriteLine("|          ENTER 6-DIGIT ACCOUNT NUMBER         |");
                Console.WriteLine("|                                               |");
                Console.WriteLine("|    Number:                                    |");
                Console.WriteLine("|    Amount: $                                  |");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");
                Console.SetCursorPosition(13, 5);
                var account = SearchAccounts(accounts);
                // If no matching account is found
                if (account == null)
                {
                    Error(3, "Account not found");
                    var choice = YesNoChoice("Find another account? (y/n): ");
                    if (choice == "y") continue;
                    return;
                }
                // If an account is found
                Success(3, "Account found! Please enter amount to withdraw");
                while (true)
                {
                    Console.SetCursorPosition(0, 6);
                    Console.Write("|    Amount: $                                  |");
                    Console.SetCursorPosition(14, 6);
                    var amount = Console.ReadLine();
                    if (!double.TryParse(amount, out var amountNum) || amountNum <= 0) // if it doesn't parse as double OR is less than 0
                    {
                        Error(2, "Please enter a number greater than 0");
                        continue;
                    }

                    if (amountNum > account.Balance)
                    {
                        Error(2, "Can't withdraw: Insufficient funds");
                        continue;
                    }
                    // If input is validated
                    account.Withdraw(amountNum);
                    Console.Clear();
                    Success(0, "Withdrawal successful!");
                    Thread.Sleep(1000);
                    return;
                }
            }
        }

        private static void Statement(List<Account> accounts)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════╗");
                Console.WriteLine("|              GET ACCOUNT STATEMENT            |");
                Console.WriteLine("|═══════════════════════════════════════════════|");
                Console.WriteLine("|          ENTER 6-DIGIT ACCOUNT NUMBER         |");
                Console.WriteLine("|                                               |");
                Console.WriteLine("|    Number:                                    |");
                Console.WriteLine("|                                               |");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");

                Console.SetCursorPosition(13, 5);
                var account = SearchAccounts(accounts);
                if (account != null) // if a matching account was found
                {
                    Console.Clear();
                    Success(0,"Account found!");
                    account.GenerateStatement();
                    Console.WriteLine();
                    
                    var sendEmail = YesNoChoice("Do you want your statement emailed? (y/n): ");
                    if (sendEmail == "n") return;

                    var subject = $"Statement for your account {account.AccountNumber}";
                    var body = account.GenerateEmailStatement();
                    
                    SendEmail(account.Email, subject, body);
                    Success(0, $"Your statement has been successfully emailed to {account.Email}.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
                
                Error(3, "Account not found!");
                var tryAgain = YesNoChoice("Find another account? (y/n): ");
                if (tryAgain == "y") continue;
                return;
            }
        }

        // Standard yes/no selectors most other methods leverage. returns 'y' or 'n'
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

                Error(1, "Please type 'y' or 'n'");
                Console.SetCursorPosition(0, tempCursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, tempCursorTop);
                Console.Write(query);
            }
        }

        // Goes a few lines down (offset from current position), writes message in RED
        private static void Error(int offset, string message)
        {
            Console.SetCursorPosition(0, Console.CursorTop + offset);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Same thing, but in GREEN :D
        private static void Success(int offset, string message)
        {
            Console.SetCursorPosition(0, Console.CursorTop + offset);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
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

        private static void DeleteAccount(List<Account> accounts)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════╗");
                Console.WriteLine("|               DELETE AN ACCOUNT               |");
                Console.WriteLine("|═══════════════════════════════════════════════|");
                Console.WriteLine("|          ENTER 6-DIGIT ACCOUNT NUMBER         |");
                Console.WriteLine("|                                               |");
                Console.WriteLine("|    Number:                                    |");
                Console.WriteLine("|                                               |");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");

                Console.SetCursorPosition(13, 5);
                var account = SearchAccounts(accounts);
                if (account != null) // if a matching account was found
                {
                    
                    Success(3,"Account found!");
                    Error(0, "WARNING: This will delete the account permanently!");
                    var delete = YesNoChoice("Do you want to delete the selected account? (y/n): ");
                    if (delete == "n") return;
                    // Rename account file so it doesn't get loaded again. Keep it for archiving purposes
                    File.Move($"A{account.AccountNumber}.txt", $"DELETED-A{account.AccountNumber}.txt");
                    // Delete account
                    accounts.Remove(account);
                    Console.WriteLine("The account has been successfully deleted");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
                
                Error(3, "Account not found!");
                var tryAgain = YesNoChoice("Find another account? (y/n): ");
                if (tryAgain == "y") continue;
                return;
            }
        }

        private static void Exit()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Thank you for using Krystof's banking system! Exiting...");
            Thread.Sleep(2100);
            Environment.Exit(0);
        }
    }
}