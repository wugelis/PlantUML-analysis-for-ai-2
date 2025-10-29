### 1. Domain Layer (領域層) - RentalCarSystem.Domain

領域層包含核心業務邏輯，是系統的心臟，不依賴任何外部框架。

#### 1.1 抽象基底類別

**Entity<T> 類別** (`Entities/Entity.cs`)
- **用途**: 所有領域實體的抽象基底類別，提供統一的識別碼和相等性比較
- **泛型約束**: T 必須實作 IEquatable<T> 介面
- **主要方法**:
  - `Entity(T id)`: 受保護建構函式，確保所有實體都有識別碼
  - `Equals(object? obj)`: 覆寫相等性比較，基於識別碼比較
  - `GetHashCode()`: 覆寫雜湊碼方法，確保相等實體有相同雜湊碼
  - `operator ==` 和 `operator !=`: 相等性運算子多載，處理null值情況

#### 1.2 領域實體

**Customer 類別** (`Entities/Customer.cs`)
- **用途**: 代表租車系統的客戶實體
- **主要屬性**: UserId, Name, Email, Password, CreatedAt
- **主要方法**:
  - `Create(string userId, string name, string email)`: 靜態工廠方法建立新客戶
  - `SetPassword(string password)`: 設定客戶密碼
  - `VerifyPassword(string password)`: 驗證客戶密碼
  - `IfCustomerIsVIP(string userId)`: 檢查傳入帳號是否為 VIP 客戶

**Car 類別** (`Entities/Car.cs`)
- **用途**: 代表租車系統的車輛實體
- **主要屬性**: Model, TypeInfo(CarTypeInfo), DailyRate, IsAvailable
- **主要方法**:
  - `Create(string model, CarType carType, Money dailyRate)`: 靜態工廠方法建立新車輛
  - `SetAvailability(bool isAvailable)`: 設定車輛可用狀態
  - `UpdateDailyRate(Money newRate)`: 更新每日租金

**Rental 類別** (`Entities/Rental.cs`)
- **用途**: 代表租車記錄實體
- **主要屬性**: CustomerId, CarId, Period(RentalPeriod), TotalFee, Status, CreatedAt
- **主要方法**:
  - `Create(Guid customerId, Guid carId, RentalPeriod period, Money totalFee)`: 建立新租賃記錄
  - `Complete()`: 完成租賃
  - `Cancel()`: 取消租賃

#### 1.3 值物件 (Value Objects)

**Money 類別** (`ValueObjects/Money.cs`)
- **用途**: 表示金額的值物件，確保金額操作的安全性
- **主要屬性**: Amount, Currency
- **主要方法**:
  - `Money(decimal amount, string currency = "TWD")`: 建構函式，驗證金額不能為負數
  - `Add(Money other)`: 金額加法，檢查貨幣類型一致性
  - `Multiply(decimal multiplier)`: 金額乘法
  - `ToString()`: 格式化顯示金額
  - `static Zero`: 靜態屬性返回零金額
  - 隱式轉換運算子: decimal ↔ Money

**CarTypeInfo 類別** (`ValueObjects/CarTypeInfo.cs`)
- **用途**: 車型資訊值物件
- **主要屬性**: CarType, DefaultDailyRate
- **主要方法**:
  - `GetCarTypeInfo(CarType carType)`: 靜態方法獲取車型資訊
  - `GetAllCarTypes()`: 取得所有可用車型

**RentalPeriod 類別** (`ValueObjects/RentalPeriod.cs`)
- **用途**: 租期值物件，封裝租賃起迄日期和天數計算
- **主要屬性**: StartDate, EndDate, Days
- **主要方法**:
  - `RentalPeriod(DateTime startDate, DateTime endDate)`: 建構函式，驗證日期有效性
  - `IsValidPeriod()`: 驗證租期是否有效
  - `OverlapsWith(RentalPeriod other)`: 檢查與其他租期是否重疊

#### 1.4 領域服務

**RentalFeeCalculator 類別** (`Services/RentalFeeCalculator.cs`)
- **用途**: 租金計算領域服務
- **主要方法**:
  - `CalculateFee(CarTypeInfo carType, RentalPeriod period)`: 計算租賃總費用
  - `CalculateLateFee(RentalPeriod period, DateTime actualReturnDate)`: 計算延遲歸還費用

#### 1.5 領域例外

**DomainException 類別** (`Exceptions/DomainExceptions.cs`)
- **用途**: 領域層基底例外類別
- **衍生例外**:
  - `CarNotFoundException`: 車輛未找到例外
  - `CustomerNotFoundException`: 客戶未找到例外
  - `InvalidRentalPeriodException`: 無效租期例外