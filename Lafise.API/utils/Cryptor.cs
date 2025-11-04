using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lafise.API.data.model;

namespace Lafise.API.utils
{
    public class Cryptor:ICryptor
    {
        private readonly IConfiguration _config;
        public Cryptor(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GeneratePassword()
        {
            int length = 12;
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var bytes = new byte[length];
            var apiKeyChars = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            for (int i = 0; i < length; i++)
            {
                apiKeyChars[i] = allowedChars[bytes[i] % allowedChars.Length];
            }

            return new string(apiKeyChars);
        }


        public string EncryptString(string plainText)
        {
            string key = _config["encrypt-key"];

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }


        public string DecryptString(string cipherText)
        {
            string key = _config["encrypt-key"];

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }





        public void CreatePasswordHash(string password, Client client)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                client.PasswordSalt = Convert.ToBase64String(hmac.Key);
                client.PasswordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }

        public bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(passwordSalt)))
            {
                var computedHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
                return computedHash == passwordHash;
            }
        }

        public void ValidatePassword(string pwd)
        {
            //Longitud
            if (pwd.Length < 8)
                throw new LafiseException(400, "Password must be at least 8 characters long");

            //Mayúscula
            if (!pwd.Any(char.IsUpper))
                throw new LafiseException(400, "Password must contain at least one uppercase letter");

            //Dígito numérico
            if (!pwd.Any(char.IsDigit))
                throw new LafiseException(400, "Password must contain at least one number");

            //Caracter especial            
            if (!hasSpecialChar(pwd))
                throw new LafiseException(400, "Password must contain at least one special character");
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
    }
}