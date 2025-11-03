using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.services.Clients.Dto
{
    public class CreateClientDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required decimal Income { get; set; }
    }
    
}