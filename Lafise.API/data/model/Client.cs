using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.data.model
{
    public class Client : IEntity, IAuditable

    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required decimal Income { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        
        public ICollection<Account>  Accounts { get; set; }
    }
}
