﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MuloApi.Interfaces;

namespace MuloApi.Classes
{
    public class CheckDataUser : ICheckData
    {
        private const string RegEmail = @"^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[;]{0,1}\s*)+$";
        private const string RegPassword = @"^[a-zA-Z][a-zA-Z]{5}$";

        private static CheckDataUser _instance;

        public CheckDataUser Current
        {
            get { return _instance ??= new CheckDataUser(); }
        }

        public bool CheckLoginRegular(string login)
        {
            return Regex.IsMatch(login, RegEmail, RegexOptions.IgnoreCase);
        }

        public bool CheckLoginSmtp(string login)
        {
            throw new NotImplementedException();
        }

        public bool CheckPasswordRegular(string pass)
        {
            return Regex.IsMatch(pass, RegPassword, RegexOptions.IgnoreCase);
        }

        public string GetHash(int idUser, string agent)
        {
            var rnd = new Random();
            var randomKey = rnd.Next(10000, 100000);
            var setHashCode = Md5Hash(Md5Hash(idUser + agent + randomKey));
            return setHashCode;
        }

        public static string Md5Hash(string input)
        {
            var hash = new StringBuilder();
            var md5Provider = new MD5CryptoServiceProvider();
            var bytes = md5Provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            foreach (var t in bytes)
                hash.Append(t.ToString("x2"));

            return hash.ToString();
        }
    }
}