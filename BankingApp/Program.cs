using System;
using System.Collections;

namespace BankingApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ArrayList accounts = new ArrayList();
            
            Console.WriteLine("Hello World!");
            Login();
            Console.ReadKey();
        }

        private static void Login()
        {
            Console.WriteLine("Welcome to Krystof Bank!");
            Console.WriteLine("Customer ID: ");
            Console.WriteLine("Password: ");
        }
    }
}