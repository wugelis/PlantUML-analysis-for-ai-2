using RentalCarSystem.Application.Common;
using RentalCarSystem.Application.UseCases.Customers;
using RentalCarSystem.Application.UseCases.Cars;
using RentalCarSystem.Application.UseCases.Rentals;
using RentalCarSystem.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace RentalCarSystem.Web.Services;

public class ConsoleRentalService
{
    private readonly IServiceProvider _serviceProvider;
    private Guid? _currentCustomerId;

    public ConsoleRentalService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync()
    {
        Console.WriteLine("=== 歡迎使用租車系統 ===");

        while (true)
        {
            try
            {
                if (_currentCustomerId == null)
                {
                    await ShowLoginMenuAsync();
                }
                else
                {
                    await ShowMainMenuAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
                Console.WriteLine("按任意鍵繼續...");
                Console.ReadKey();
            }
        }
    }

    private async Task ShowLoginMenuAsync()
    {
        Console.Clear();
        Console.WriteLine("=== 登入選單 ===");
        Console.WriteLine("1. 註冊帳號");
        Console.WriteLine("2. 登入");
        Console.WriteLine("3. 離開系統");
        Console.Write("請選擇功能 (1-3): ");

        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                await RegisterAsync();
                break;
            case "2":
                await LoginAsync();
                break;
            case "3":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("無效選擇，請重新輸入");
                await Task.Delay(1000);
                break;
        }
    }

    private async Task ShowMainMenuAsync()
    {
        Console.Clear();
        Console.WriteLine("=== 主選單 ===");
        Console.WriteLine("1. 租車");
        Console.WriteLine("2. 登出");
        Console.Write("請選擇功能 (1-2): ");

        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                await RentCarAsync();
                break;
            case "2":
                _currentCustomerId = null;
                Console.WriteLine("已登出");
                await Task.Delay(1000);
                break;
            default:
                Console.WriteLine("無效選擇，請重新輸入");
                await Task.Delay(1000);
                break;
        }
    }

    private async Task RegisterAsync()
    {
        Console.Clear();
        Console.WriteLine("=== 註冊帳號 ===");

        Console.Write("請輸入使用者ID: ");
        var userId = Console.ReadLine();

        Console.Write("請輸入姓名: ");
        var name = Console.ReadLine();

        Console.Write("請輸入Email: ");
        var email = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("所有欄位都是必填的");
            Console.WriteLine("按任意鍵繼續...");
            Console.ReadKey();
            return;
        }

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<RegisterCustomerCommand, Guid>>();
        var command = new RegisterCustomerCommand(userId, name, email);

        try
        {
            var customerId = await handler.Handle(command);
            Console.WriteLine($"註冊成功！客戶ID: {customerId}");
            Console.WriteLine("按任意鍵繼續...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"註冊失敗: {ex.Message}");
            Console.WriteLine("按任意鍵繼續...");
            Console.ReadKey();
        }
    }

    private async Task LoginAsync()
    {
        Console.Clear();
        Console.WriteLine("=== 登入 ===");

        Console.Write("請輸入使用者ID: ");
        var userId = Console.ReadLine();

        Console.Write("請輸入密碼: ");
        var password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("使用者ID和密碼都是必填的");
            Console.WriteLine("按任意鍵繼續...");
            Console.ReadKey();
            return;
        }

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<LoginCustomerCommand, Guid?>>();
        var command = new LoginCustomerCommand(userId, password);

        try
        {
            var customerId = await handler.Handle(command);
            if (customerId.HasValue)
            {
                _currentCustomerId = customerId.Value;
                Console.WriteLine("登入成功！");
                Console.WriteLine("按任意鍵繼續...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("登入失敗：使用者ID或密碼錯誤");
                Console.WriteLine("按任意鍵繼續...");
                Console.ReadKey();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"登入失敗: {ex.Message}");
            Console.WriteLine("按任意鍵繼續...");
            Console.ReadKey();
        }
    }

    private async Task RentCarAsync()
    {
        Console.Clear();
        Console.WriteLine("=== 租車流程 ===");

        // 1. 選擇車型
        var carType = await SelectCarTypeAsync();
        if (carType == null) return;

        // 2. 顯示可用車輛
        var selectedCar = await SelectAvailableCarAsync(carType.Value);
        if (selectedCar == null) return;

        // 3. 選擇租用期間
        var period = SelectRentalPeriod();
        if (period == null) return;

        // 4. 確認並建立租賃
        await ConfirmRentalAsync(selectedCar, period.Value);
    }

    private Task<CarType?> SelectCarTypeAsync()
    {
        Console.WriteLine("請選擇車型:");
        Console.WriteLine("1. 轎車 (Car) - $1,000/天");
        Console.WriteLine("2. 休旅車 (SUV) - $1,500/天");
        Console.WriteLine("3. 卡車 (Truck) - $2,000/天");
        Console.WriteLine("4. 跑車 (SportsCar) - $3,000/天");
        Console.WriteLine("0. 返回主選單");
        Console.Write("請選擇 (0-4): ");

        var choice = Console.ReadLine();
        CarType? result = choice switch
        {
            "1" => CarType.Car,
            "2" => CarType.SUV,
            "3" => CarType.Truck,
            "4" => CarType.SportsCar,
            "0" => null,
            _ => throw new ArgumentException("無效的車型選擇")
        };
        return Task.FromResult(result);
    }

    private async Task<CarDto?> SelectAvailableCarAsync(CarType carType)
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<GetAvailableCarsByTypeQuery, IReadOnlyList<CarDto>>>();
        var query = new GetAvailableCarsByTypeQuery(carType);

        var cars = await handler.Handle(query);

        if (!cars.Any())
        {
            Console.WriteLine($"抱歉，目前沒有可用的 {carType} 車輛");
            Console.WriteLine("按任意鍵返回...");
            Console.ReadKey();
            return null;
        }

        Console.WriteLine($"\n可用的 {carType} 車輛:");
        for (int i = 0; i < cars.Count; i++)
        {
            var car = cars[i];
            Console.WriteLine($"{i + 1}. {car.Model} - ${car.DailyRate}/天");
        }
        Console.WriteLine("0. 返回");

        Console.Write($"請選擇車輛 (0-{cars.Count}): ");
        var choice = Console.ReadLine();

        if (choice == "0") return null;

        if (int.TryParse(choice, out var index) && index >= 1 && index <= cars.Count)
        {
            return cars[index - 1];
        }

        throw new ArgumentException("無效的車輛選擇");
    }

    private (DateTime startDate, DateTime endDate)? SelectRentalPeriod()
    {
        Console.WriteLine("\n=== 選擇租用期間 ===");

        Console.Write("請輸入開始日期 (yyyy-MM-dd，按Enter使用今天): ");
        var startInput = Console.ReadLine();
        var startDate = string.IsNullOrWhiteSpace(startInput) ? DateTime.Today : DateTime.Parse(startInput);

        Console.Write("請輸入結束日期 (yyyy-MM-dd): ");
        var endInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(endInput))
        {
            Console.WriteLine("結束日期是必填的");
            Console.WriteLine("按任意鍵返回...");
            Console.ReadKey();
            return null;
        }

        var endDate = DateTime.Parse(endInput);

        if (endDate <= startDate)
        {
            Console.WriteLine("結束日期必須晚於開始日期");
            Console.WriteLine("按任意鍵返回...");
            Console.ReadKey();
            return null;
        }

        return (startDate, endDate);
    }

    private async Task ConfirmRentalAsync(CarDto car, (DateTime startDate, DateTime endDate) period)
    {
        var days = (period.endDate - period.startDate).Days;
        var totalFee = car.DailyRate * days;

        Console.WriteLine("\n=== 租賃確認 ===");
        Console.WriteLine($"車輛: {car.Model}");
        Console.WriteLine($"租用期間: {period.startDate:yyyy-MM-dd} ~ {period.endDate:yyyy-MM-dd} ({days} 天)");
        Console.WriteLine($"每日租金: ${car.DailyRate:N0}");
        Console.WriteLine($"總租金: ${totalFee:N0}");
        Console.WriteLine();
        Console.Write("確認租賃？(y/n): ");

        var confirm = Console.ReadLine();
        if (confirm?.ToLower() != "y" && confirm?.ToLower() != "yes")
        {
            Console.WriteLine("已取消租賃");
            Console.WriteLine("按任意鍵返回...");
            Console.ReadKey();
            return;
        }

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<CreateRentalCommand, Guid>>();
        var command = new CreateRentalCommand(_currentCustomerId!.Value, car.Id, period.startDate, period.endDate);

        try
        {
            var rentalId = await handler.Handle(command);
            Console.WriteLine($"\n租賃成功建立！租賃ID: {rentalId}");
            Console.WriteLine("感謝您的使用！");
            Console.WriteLine("按任意鍵返回主選單...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"租賃建立失敗: {ex.Message}");
            Console.WriteLine("按任意鍵返回...");
            Console.ReadKey();
        }
    }
}