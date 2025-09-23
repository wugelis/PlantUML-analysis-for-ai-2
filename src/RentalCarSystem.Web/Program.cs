using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RentalCarSystem.Web.Services;

namespace RentalCarSystem.Web;

class Program
{
    static async Task Main(string[] args)
    {
        // 如果命令列參數包含 "test"，運行簡單測試
        if (args.Length > 0 && args[0] == "test")
        {
            await SimpleTest.RunTestAsync();
            return;
        }

        var host = CreateHostBuilder(args).Build();

        try
        {
            var rentalService = host.Services.GetRequiredService<ConsoleRentalService>();
            await rentalService.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"應用程式發生錯誤: {ex.Message}");
            Console.WriteLine("按任意鍵結束...");
            Console.ReadKey();
        }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddRentalCarServices();
                services.AddScoped<ConsoleRentalService>();
            });
}