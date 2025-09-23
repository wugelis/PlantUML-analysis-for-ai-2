using System;

namespace RentalCarSystem
{
    public class Customer
    {
        public void RegisterAccount(UserInfo userInfo)
        {
            // 註冊帳號邏輯
        }

        public bool Login(string userId, string password)
        {
            // 登入邏輯
            return true;
        }

        public void ToRentalCar(string userId)
        {
            // 進入租車流程
        }
    }

    public class Account
    {
        public void RegisterAccount(UserInfo userInfo)
        {
            // 註冊帳號邏輯
        }

        public bool Login(string userId, string password)
        {
            // 登入邏輯
            return true;
        }
    }

    public class RentalService
    {
        public void ToRentalCar(string userId)
        {
            // 1. 選擇車型
            var car = new Car();
            string carType = "SUV"; // 假設由 UI 或參數取得
            car.SelectCarType(carType);

            // 2. 選擇租用期間
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(3); // 假設由 UI 或參數取得
            this.SelectRentalPeriod(startDate, endDate);

            // 3. 計算租金
            var feeCalculator = new FeeCalculator();
            decimal rentalFee = feeCalculator.CalculateRentalFee(carType, startDate, endDate);

            // 4. 回傳租賃確認 (可用 Console 或回傳值)
            Console.WriteLine($"租賃確認：車型={carType}, 期間={startDate:yyyy-MM-dd}~{endDate:yyyy-MM-dd}, 租金={rentalFee:C}");
        }

        public void SelectRentalPeriod(DateTime startDate, DateTime endDate)
        {
            // 選擇租用期間
        }
    }

    public enum CarType
    {
        Car,
        SUV,
        Truck,
        SportsCar
    }

    public class Car
    {
        public CarType Type { get; private set; }
        public decimal DailyRate { get; private set; }

        public void SelectCarType(string carType)
        {
            switch (carType)
            {
                case "Car":
                    Type = CarType.Car;
                    DailyRate = 1000m;
                    break;
                case "SUV":
                    Type = CarType.SUV;
                    DailyRate = 1500m;
                    break;
                case "Truck":
                    Type = CarType.Truck;
                    DailyRate = 2000m;
                    break;
                case "SportsCar":
                    Type = CarType.SportsCar;
                    DailyRate = 3000m;
                    break;
                default:
                    throw new ArgumentException($"未知車型: {carType}");
            }
        }
    }

    public class FeeCalculator
    {
        public decimal CalculateRentalFee(string carType, DateTime startDate, DateTime endDate)
        {
            // 計算租金邏輯
            return 0m;
        }
    }

    public class UserInfo
    {
        // 使用者資訊屬性
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        // 其他必要欄位
    }
}
