using Zentient.Tests;


namespace Tests;

class Program
{
    static async Task Main(string[] args)
    {
        TestManager manager = new TestManager();

        await manager.Run();
    }
}
