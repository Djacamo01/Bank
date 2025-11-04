using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.utils
{
    public class LafiseException : Exception
    {
        public int Code { get; }
        public LafiseException(int code, string message)
            : base(message)
        {
            Code = code;
        }
    }
}