using System;
using System.Threading.Tasks;

namespace KovanFaucet
{
    internal class Program
    {
        private const string ADDRESS_FILE = "kovan_addresses.dat";

        static void Main(string[] args)
        {
            var loop = true;
            var invalidCommand = false;

            ConfigureBot();

            do
            {
                Console.Write("Do you want to add ETH TestNET Address (y/n): ");
                var continueCommand = Console.ReadKey(true)
                    .Key
                    .ToString();

                Console.WriteLine();

                if (string.Equals(continueCommand, "y", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.Write("Enter ETH TestNET Address: ");
                    var address = Console.ReadLine();

                    KovanFileProcessor.SaveToFile(new(address), ADDRESS_FILE)
                        .Wait();
                }
                else if (string.Equals(continueCommand, "n", StringComparison.InvariantCultureIgnoreCase))
                {
                    loop = false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid command!");
                    Console.ResetColor();
                    invalidCommand = true;
                    loop = false;
                }
            } while (loop);

            if (invalidCommand)
            {
                Console.WriteLine("Press any kay to exit app.");
                Console.ReadKey();
                return;
            }

            var loadFromFile = KovanFileProcessor.LoadFromFile(ADDRESS_FILE);

            while (true)
            {
                KovanFaucetProcessor.SendRequestAsync(loadFromFile, x =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(x);
                    Console.ResetColor();
                },
                x =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine(x);
                    Console.ResetColor();
                }).Wait();

                Task.Delay(TimeSpan.FromDays(1))
                    .Wait();
            }
        }

        private static void ConfigureBot()
        {
            if (!KovanAuthProcessor.IsAuthenticated())
            {
                Console.Write("Enter AccessToken: ");
                var token = Console.ReadLine();

                KovanAuthProcessor.ConfigAuthentication(new(token));
            }
        }
    }
}
