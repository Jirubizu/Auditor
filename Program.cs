using System.Threading.Tasks;

namespace Auditor
{
    public static class Program
    {
        private static async Task Main()
            => await new Auditor().SetupAsync("./config.json");

    }
}