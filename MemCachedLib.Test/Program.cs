using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemCachedLib.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MemCachedLib.Setting.PoolMaxSize = 5;
            MemCachedLib.Setting.DataMaxSize = 1 * 1024 * 1024;

            var client = MemCachedLib.Cached.MemCached.Create(
                new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11211));

            object val;
            while(Console.ReadKey().Key == ConsoleKey.Enter)
            {
                if (!client.TryGet("A", out val))
                {
                    client.Set("A", "中华人民共和国", TimeSpan.FromSeconds(100));
                }
                Console.WriteLine(val);
            }

        }
    }
}
