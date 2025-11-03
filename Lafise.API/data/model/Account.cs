using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.data.model
{
    public class Account : IEntity, IAuditable
    {
        public required int Id { get; set; }
        public required string AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; } = 0.00m;

        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        // Foreign Key
        public required int ClientId { get; set; }

        // Navigation Properties:
        public Client? Client { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}