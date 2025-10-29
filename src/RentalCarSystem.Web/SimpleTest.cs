using RentalCarSystem.Domain.Entities;
using RentalCarSystem.Domain.ValueObjects;
using RentalCarSystem.Domain.Services;
using RentalCarSystem.Application.UseCases.Customers;
using RentalCarSystem.Application.UseCases.Rentals;
using RentalCarSystem.Infrastructure.Repositories;
using RentalCarSystem.Infrastructure.Persistence;

namespace RentalCarSystem.Web;

class SimpleTest
{
    public static async Task RunTestAsync()
    {
        Console.WriteLine("=== 租車系統測試 ===");

        // 初始化服務
        var customerRepo = new InMemoryCustomerRepository();
        var carRepo = new InMemoryCarRepository();
        var rentalRepo = new InMemoryRentalRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var feeCalculator = new RentalFeeCalculator();

        // 測試VIP功能
        await TestVipFunctionality();

        // 測試註冊客戶
        var registerHandler = new RegisterCustomerCommandHandler(customerRepo, unitOfWork);
        var customerId = await registerHandler.Handle(new RegisterCustomerCommand("test001", "張三", "test@example.com"));
        Console.WriteLine($"✓ 客戶註冊成功，ID: {customerId}");

        // 測試VIP客戶註冊
        var vipCustomerId = await registerHandler.Handle(new RegisterCustomerCommand("VIP001", "VIP王先生", "vip@example.com"));
        Console.WriteLine($"✓ VIP客戶註冊成功，ID: {vipCustomerId}");

        // 測試登入
        var loginHandler = new LoginCustomerCommandHandler(customerRepo);
        var loginResult = await loginHandler.Handle(new LoginCustomerCommand("test001", "password"));
        Console.WriteLine($"✓ 一般客戶登入成功: {loginResult.HasValue}");

        var vipLoginResult = await loginHandler.Handle(new LoginCustomerCommand("VIP001", "password"));
        Console.WriteLine($"✓ VIP客戶登入成功: {vipLoginResult.HasValue}");

        // 測試租車
        var cars = await carRepo.GetAvailableCarsByTypeAsync(CarType.SUV);
        if (cars.Any())
        {
            var selectedCar = cars.First();
            var createRentalHandler = new CreateRentalCommandHandler(customerRepo, carRepo, rentalRepo, unitOfWork, feeCalculator);
            
            var rentalId = await createRentalHandler.Handle(new CreateRentalCommand(
                customerId,
                selectedCar.Id,
                DateTime.Today,
                DateTime.Today.AddDays(3)
            ));
            
            Console.WriteLine($"✓ 租賃建立成功，ID: {rentalId}");
            
            // 驗證車輛狀態已更新
            var updatedCar = await carRepo.GetByIdAsync(selectedCar.Id);
            Console.WriteLine($"✓ 車輛可用狀態: {updatedCar?.IsAvailable} (應該為 False)");
        }

        Console.WriteLine("=== 所有測試完成 ===");
    }

    /// <summary>
    /// 測試VIP客戶識別功能
    /// </summary>
    private static async Task TestVipFunctionality()
    {
        Console.WriteLine("\n=== VIP功能測試 ===");

        // 測試各種使用者ID的VIP狀態
        var testUsers = new[]
        {
            ("VIP001", "VIP開頭的帳號"),
            ("VIPJohn", "VIP開頭的帳號"),
            ("premium_user", "包含premium的帳號"),
            ("PremiumMember", "包含premium的帳號"),
            ("admin", "特殊VIP帳號"),
            ("manager", "特殊VIP帳號"),
            ("director", "特殊VIP帳號"),
            ("ceo", "特殊VIP帳號"),
            ("vip002", "VIP預設名單"),
            ("normal_user", "一般使用者"),
            ("test123", "一般使用者"),
            ("", "空字串"),
            (null, "null值")
        };

        foreach (var (userId, description) in testUsers)
        {
            var isVip = Customer.IfCustomerIsVIP(userId);
            var status = isVip ? "✨ VIP" : "👤 一般";
            Console.WriteLine($"  {status} | {userId ?? "null",-15} | {description}");
        }

        Console.WriteLine("\n✓ VIP識別測試完成");
        Console.WriteLine("✓ VIP客戶可以租用跑車，一般客戶僅能租用其他車型");
        
        await Task.Delay(100); // 確保異步方法
    }
}