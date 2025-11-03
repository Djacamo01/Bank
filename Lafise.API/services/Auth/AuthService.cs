using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data.model;

namespace Lafise.API.services.Auth
{
    public class AuthService


    {
        private void CreatePasswordHash(string password, Client client)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                client.PasswordHash = Convert.ToBase64String(hmac.Key);
                client.PasswordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }

        private bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(passwordSalt)))
            {
                var computedHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
                return computedHash == passwordHash;
            }
        }

        private void ValidatePassword(string pwd)
        {
            //Longitud
            if (pwd.Length < 8)
                throw new Exception("Password must be at least 8 characters long");

            //Mayúscula
            if (!pwd.Any(char.IsUpper))
                throw new Exception("Password must contain at least one uppercase letter");

            //Dígito numérico
            if (!pwd.Any(char.IsDigit))
                throw new Exception("Password must contain at least one number");

            //Caracter especial            
            if (!hasSpecialChar(pwd))
                throw new Exception("Password must contain at least one special character");
        }

        private bool hasSpecialChar(string input)
        {
            string specialChar = @"@!#$%&*-+()=|¬/\,;.:-_{}[]´¡?¿'";
            foreach (var item in specialChar)
            {
                if (input.Contains(item)) return true;
            }

            return false;
        }

        private string GenerateRandomCode(int length)
        {
            var random = new Random();
            var code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(code);
        }
    }
}