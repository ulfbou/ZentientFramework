using Zentient.Tests;
using System.Threading.Tasks;

namespace Tests;

public static class Program
{
    public static async Task Main(string[] args)
    {
        TestManager manager = new TestManager();
        await manager.Run(true);
    }
}
