using Zentient.Tests;
using System.Threading.Tasks;

namespace ConsoleTesting;

public static class Program
{
public static async Task Main(string[] args)
{
TestManager manager = new TestManager();
await manager.Run();
}
}
