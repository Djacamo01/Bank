using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.data
{
    public interface IAuditable
    {
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}