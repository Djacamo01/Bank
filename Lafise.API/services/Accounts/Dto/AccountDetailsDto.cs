using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.services.Accounts.Dto
{
    public class AccountDetailsDto
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}