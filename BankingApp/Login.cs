﻿namespace BankingApp
{
    public readonly struct Login
    {
        private readonly string _username;
        private readonly string _password;

        public Login(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public bool Validate(string username, string password)
        {
            return username == _username && password == _password;
        }
    }
}