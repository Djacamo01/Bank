using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.services.Accounts
{
    public class AccountSettings
    {

        public const string SettingsName = "AccountSettings";

       
        public string[] ValidAccountTypes { get; set; } = Array.Empty<string>();
    }
    
}