using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akavache;

namespace akavache_issue_1.consoleclient
{
    class Program
    {
        private static int _cnt;

        static async Task Main(string[] args)
        {
            BlobCache.ApplicationName = $"akavache-{Guid.NewGuid()}";

            await TestIt(BlobCache.LocalMachine);
            await TestIt(BlobCache.InMemory);
        }

        private static async Task TestIt(IBlobCache cache)
        {
            _cnt = 0;
            Console.WriteLine(await GetOrFetchAsync(cache, DateTime.UtcNow + TimeSpan.FromMilliseconds(1000)));
            await cache.Invalidate("a");
            Console.WriteLine(await GetOrFetchAsync(cache, DateTime.UtcNow + TimeSpan.FromMilliseconds(1000)));
            Console.WriteLine("=====================");
            Console.WriteLine();
        }

        private static async Task<string> GetOrFetchAsync(IBlobCache cache, DateTimeOffset? absoluteExpiration)
        {
            return await cache.GetOrFetchObject("a", async () =>
            {
                await Task.Delay(500);
                var a = $"b{Interlocked.Increment(ref _cnt)}";
                Console.WriteLine("A:" + a);
                return a;
            }, absoluteExpiration);
        }
    }
}