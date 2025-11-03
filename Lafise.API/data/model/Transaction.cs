using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.data.model
{
    public class Transaction : IEntity, IAuditable
    {
        public required int Id { get; set; }

        public required int AccountId { get; set; }

        public required string Type { get; set; }

        public required decimal Amount { get; set; }

        public required DateTime Date { get; set; } = DateTime.Now;

        public decimal BalanceAfter { get; set; }

        public DateTime DateCreated { get; set; }
        
        public DateTime? DateModified { get; set; }
        // Navigation Property:
        public Account? Account { get; set; }
    }
}