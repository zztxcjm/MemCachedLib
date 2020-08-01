using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemCachedLib
{
    public class DataTooLargeException : Exception
    {
        public DataTooLargeException()
            : base($"Memcached Data Too Large.(Limit {Setting.DataMaxSize} bytes)")
        {
        }
    }
}
