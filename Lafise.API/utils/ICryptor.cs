using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data.model;
using Lafise.API.services.Auth.Dto;

namespace Lafise.API.utils
{
    public interface ICryptor
    {
        void CreatePasswordHash(string password, Client client);
        bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt);
        void ValidatePassword(string pwd);
        
       
        Task<string> GeneratePassword();
        string EncryptString(string plainText);
        string DecryptString(string cipherText);
    }
}