using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.utils
{
    public interface ICryptor
    {

       
        Task<string> GeneratePassword();
        string EncryptString(string plainText);
        string DecryptString(string cipherText);
    }
}