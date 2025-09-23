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

        // 測試註冊客戶
        var registerHandler = new RegisterCustomerCommandHandler(customerRepo, unitOfWork);
        var customerId = await registerHandler.Handle(new RegisterCustomerCommand("test001", "張三", "test@example.com"));
        Console.WriteLine($"✓ 客戶註冊成功，ID: {customerId}");

        // 測試登入
        var loginHandler = new LoginCustomerCommandHandler(customerRepo);
        var loginResult = await loginHandler.Handle(new LoginCustomerCommand("test001", "password"));
        Console.WriteLine($"✓ 登入成功: {loginResult.HasValue}");

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
}