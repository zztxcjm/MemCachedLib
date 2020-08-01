using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemCachedLib
{
    public class ConnectTimeoutException : Exception
    {
        public ConnectTimeoutException()
            : base($"Memcached Connect Timeout.")
        {
        }

    }
}
