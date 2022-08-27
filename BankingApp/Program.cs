using System;
using System.Collections;

namespace BankingApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ArrayList logins = new ArrayList();
            ArrayList accounts = new ArrayList();

            LoadLogins();
            Login();
            Console.ReadKey();

            void LoadLogins()
            {
                string[] entries = System.IO.File.ReadAllLines("login.txt");
                foreach (string entry in entries)
                {
                    string[] credentials = entry.Split('|');
                    Login newLogin = new Login(credentials[0], credentials[1]);
                    logins.Add(newLogin);
                }
            }
            
            void Login()
            {
                bool valid = false;
                while (!valid)
                {
                    Console.WriteLine("Welcome to Krystof Bank!");
                    Console.Write("Customer ID: ");
                    string username = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();

                    foreach (Login login in logins)
                    {
                        if (login.Validate(username, password))
                        {
                            valid = true;
                        }
                    }
                }
                Console.Write("Logged in!");
            }
        }
    }
}