using RentalCarSystem.Application.Common;
using RentalCarSystem.Application.UseCases.Customers;
using RentalCarSystem.Application.UseCases.Cars;
using RentalCarSystem.Application.UseCases.Rentals;
using RentalCarSystem.Domain.ValueObjects;
using RentalCarSystem.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace RentalCarSystem.Web.Services;

public class ConsoleRentalService
{
    private readonly IServiceProvider _serviceProvider;
    private Guid? _currentCustomerId;
    private string? _currentUserId; // 新增：儲存當前使用者的UserId以進行VIP驗證

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
                _currentUserId = null; // 清除當前使用者ID
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
                _currentUserId = userId; // 儲存當前使用者ID以進行VIP驗證
                Console.WriteLine("登入成功！");
                
                // 顯示VIP狀態
                if (Customer.IfCustomerIsVIP(userId))
                {
                    Console.WriteLine("✨ 歡迎VIP客戶！您可以租用所有車型包括跑車。");
                }
                
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

        // 1. 選擇車型 (包含VIP檢查)
        var carType = await SelectCarTypeAsync();
        if (carType == null) return; // 使用者選擇返回主選單

        // 2. 顯示可用車輛
        var selectedCar = await SelectAvailableCarAsync(carType.Value);
        if (selectedCar == null) return;

        // 3. 選擇租用期間
        var period = SelectRentalPeriod();
        if (period == null) return;

        // 4. 確認並建立租賃
        await ConfirmRentalAsync(selectedCar, period.Value);
    }

    private async Task<CarType?> SelectCarTypeAsync()
    {
        while (true) // 循環直到使用者做出有效選擇
        {
            Console.Clear();
            Console.WriteLine("=== 選擇車型 ===");
            Console.WriteLine("1. 轎車 (Car) - $1,000/天");
            Console.WriteLine("2. 休旅車 (SUV) - $1,500/天");
            Console.WriteLine("3. 卡車 (Truck) - $2,000/天");
            
            // 檢查是否為VIP客戶來決定是否顯示跑車選項
            bool isVip = !string.IsNullOrEmpty(_currentUserId) && Customer.IfCustomerIsVIP(_currentUserId);
            
            if (isVip)
            {
                Console.WriteLine("4. 跑車 (SportsCar) - $3,000/天 ✨ VIP專屬");
            }
            else
            {
                Console.WriteLine("4. 跑車 (SportsCar) - $3,000/天 🚫 僅限VIP客戶");
            }
            
            Console.WriteLine("5. 電動車 (ElectricCar) - $2,800/天");
            Console.WriteLine("0. 返回主選單");
            Console.Write("請選擇 (0-5): ");

            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    return CarType.Car;
                case "2":
                    return CarType.SUV;
                case "3":
                    return CarType.Truck;
                case "4":
                    var sportsCarResult = await HandleSportsCarSelectionAsync(isVip);
                    if (sportsCarResult.HasValue)
                        return sportsCarResult.Value;
                    // 如果返回null表示非VIP用戶選擇跑車，繼續循環讓用戶重新選擇
                    break;
                case "5":
                    return CarType.ElectricCar;
                case "0":
                    return null;
                default:
                    Console.WriteLine("無效的選擇，請重新輸入");
                    Console.WriteLine("按任意鍵繼續...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    /// <summary>
    /// 處理跑車選擇，檢查VIP權限
    /// </summary>
    /// <param name="isVip">是否為VIP客戶</param>
    /// <returns>如果是VIP則返回SportsCar，否則顯示錯誤訊息並返回null</returns>
    private async Task<CarType?> HandleSportsCarSelectionAsync(bool isVip)
    {
        if (isVip)
        {
            Console.WriteLine("✨ VIP客戶專屬跑車租用服務已啟用");
            await Task.Delay(1500); // 短暫停留顯示VIP訊息
            return CarType.SportsCar;
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("🚫 抱歉，跑車租用服務僅開放給VIP客戶");
            Console.WriteLine("💡 VIP客戶識別規則:");
            Console.WriteLine("   • 使用者ID以 'VIP' 開頭 (如: VIP001, VIPJohn)");
            Console.WriteLine("   • 使用者ID包含 'premium' (如: premium_user)");
            Console.WriteLine("   • 特殊VIP帳號: admin, manager, director, ceo");
            Console.WriteLine();
            Console.WriteLine("如需升級為VIP會員，請聯繫客服");
            Console.WriteLine("按任意鍵返回車型選單...");
            Console.ReadKey();
            return null; // 返回null將重新顯示車型選單
        }
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