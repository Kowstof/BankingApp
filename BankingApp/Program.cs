using System;
using System.Collections;
using System.IO;

namespace BankingApp
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var logins = new ArrayList();
            var accounts = new ArrayList();

            LoadLogins();
            Login();
            Console.ReadKey();


            void LoadLogins()
            {
                var entries = File.ReadAllLines("login.txt");
                foreach (var entry in entries)
                {
                    var credentials = entry.Split('|');
                    var newLogin = new Login(credentials[0], credentials[1]);
                    logins.Add(newLogin);
                }
            }

            void Login()
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
}