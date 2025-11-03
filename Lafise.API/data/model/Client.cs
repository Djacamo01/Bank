using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.data.model
{
    public class Client : IEntity, IAuditable

    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string passwordHash { get; set; }
        public string passwordSalt { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required decimal Income { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
