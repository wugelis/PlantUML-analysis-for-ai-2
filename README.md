# 租車系統 (Rental Car System)

基於 Clean Architecture 和 Domain-Driven Design (DDD) 設計模式的租車管理系統，使用 .NET 9 開發。

## 系統架構概述

本系統採用四層架構設計，各層職責分明，遵循依賴反轉原則：

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│                  (RentalCarSystem.Web)                     │
│  - Console UI Interface                                     │
│  - User Input/Output Handling                               │
│  - Service Registration                                     │
└─────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
│                (RentalCarSystem.Application)               │
│  - Use Cases (Commands/Queries)                            │
│  - DTOs (Data Transfer Objects)                            │
│  - Repository Abstractions                                 │
│  - Business Workflow Orchestration                         │
└─────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                             │
│                 (RentalCarSystem.Domain)                   │
│  - Entities (Business Objects)                             │
│  - Value Objects (Immutable Values)                        │
│  - Domain Services                                         │
│  - Domain Exceptions                                       │
└─────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│               (RentalCarSystem.Infrastructure)             │
│  - Repository Implementations                              │
│  - Data Persistence                                        │
│  - External Services Integration                           │
└─────────────────────────────────────────────────────────────┘
```

## 各層詳細說明

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

### 2. Application Layer (應用層) - RentalCarSystem.Application

應用層協調領域物件執行業務用例，定義應用程式的工作流程。

#### 2.1 抽象介面

**IUnitOfWork 介面** (`Abstractions/IUnitOfWork.cs`)
- **用途**: 工作單元模式介面，管理資料異動的交易邊界
- **主要方法**:
  - `SaveChangesAsync()`: 保存變更
  - `BeginTransactionAsync()`: 開始交易
  - `CommitTransactionAsync()`: 提交交易
  - `RollbackTransactionAsync()`: 回滾交易

**Repository 介面群**:
- **ICarRepository** (`Abstractions/ICarRepository.cs`): 車輛資料存取介面
- **ICustomerRepository** (`Abstractions/ICustomerRepository.cs`): 客戶資料存取介面
- **IRentalRepository** (`Abstractions/IRentalRepository.cs`): 租賃資料存取介面

#### 2.2 CQRS 模式基礎設施

**ICommand/IQuery 介面** (`Common/ICommands.cs`)
- **用途**: 實現 CQRS (Command Query Responsibility Segregation) 模式
- **介面**:
  - `ICommand<TResult>`: 命令介面標記
  - `ICommandHandler<TCommand, TResult>`: 命令處理器介面
  - `IQuery<TResult>`: 查詢介面標記
  - `IQueryHandler<TQuery, TResult>`: 查詢處理器介面

#### 2.3 資料傳輸物件 (DTOs)

**DTO 類別群** (`Common/Dtos.cs`)
- **CustomerDto**: 客戶資料傳輸物件
- **CarDto**: 車輛資料傳輸物件
- **RentalDto**: 租賃資料傳輸物件
- **CreateCustomerDto**: 建立客戶的請求物件
- **CreateRentalDto**: 建立租賃的請求物件

#### 2.4 用例實現 (Use Cases)

**客戶管理用例**:

**RegisterCustomerCommand** (`UseCases/Customers/RegisterCustomerCommand.cs`)
- **用途**: 註冊新客戶的命令
- **處理器方法**:
  - `Handle()`: 檢查使用者ID唯一性，建立新客戶實體，保存至資料庫

**LoginCustomerCommand** (`UseCases/Customers/LoginCustomerCommand.cs`)
- **用途**: 客戶登入驗證命令
- **處理器方法**:
  - `Handle()`: 驗證使用者憑證，返回客戶ID或null

**車輛管理用例**:

**GetAvailableCarsByTypeQuery** (`UseCases/Cars/GetAvailableCarsByTypeQuery.cs`)
- **用途**: 根據車型查詢可用車輛
- **處理器方法**:
  - `Handle()`: 查詢指定車型的可用車輛清單

**租賃管理用例**:

**CreateRentalCommand** (`UseCases/Rentals/CreateRentalCommand.cs`)
- **用途**: 建立新租賃記錄命令
- **處理器方法**:
  - `Handle()`: 驗證客戶和車輛存在性，計算租金，建立租賃記錄

### 3. Infrastructure Layer (基礎設施層) - RentalCarSystem.Infrastructure

基礎設施層提供具體的技術實現，如資料存取、外部服務整合等。

#### 3.1 資料持久化

**InMemoryUnitOfWork 類別** (`Persistence/InMemoryUnitOfWork.cs`)
- **用途**: 記憶體內工作單元實現
- **主要方法**:
  - `SaveChangesAsync()`: 記憶體實現無需實際保存
  - `BeginTransactionAsync()`: 記憶體實現不支援交易
  - `CommitTransactionAsync()`: 記憶體實現不支援交易
  - `RollbackTransactionAsync()`: 記憶體實現不支援交易

#### 3.2 Repository 實現

**InMemoryCarRepository 類別** (`Repositories/InMemoryCarRepository.cs`)
- **用途**: 車輛資料的記憶體存取實現
- **主要方法**:
  - `GetByIdAsync()`: 根據ID查詢車輛
  - `GetAvailableCarsByTypeAsync()`: 查詢指定車型的可用車輛
  - `AddAsync()`: 新增車輛
  - `UpdateAsync()`: 更新車輛資訊
  - `DeleteAsync()`: 刪除車輛

**InMemoryCustomerRepository 類別** (`Repositories/InMemoryCustomerRepository.cs`)
- **用途**: 客戶資料的記憶體存取實現
- **主要方法**:
  - `GetByIdAsync()`: 根據ID查詢客戶
  - `GetByUserIdAsync()`: 根據使用者ID查詢客戶
  - `AddAsync()`: 新增客戶
  - `UpdateAsync()`: 更新客戶資訊

**InMemoryRentalRepository 類別** (`Repositories/InMemoryRentalRepository.cs`)
- **用途**: 租賃資料的記憶體存取實現
- **主要方法**:
  - `GetByIdAsync()`: 根據ID查詢租賃記錄
  - `GetByCustomerIdAsync()`: 查詢客戶的租賃記錄
  - `AddAsync()`: 新增租賃記錄
  - `UpdateAsync()`: 更新租賃記錄

### 4. Presentation Layer (展示層) - RentalCarSystem.Web

展示層處理使用者互動，目前實現為控制台應用程式。

#### 4.1 應用程式進入點

**Program 類別** (`Program.cs`)
- **用途**: 應用程式主要進入點
- **主要功能**:
  - `Main()`: 程式主方法，支援測試模式參數
  - `CreateHostBuilder()`: 建立主機建構器，設定依賴注入

#### 4.2 服務層

**ConsoleRentalService 類別** (`Services/ConsoleRentalService.cs`)
- **用途**: 控制台租車服務，處理使用者互動流程
- **私有屬性**:
  - `_serviceProvider`: 服務提供者，用於解析依賴
  - `_currentCustomerId`: 當前登入客戶ID

**主要方法說明**:

- **`StartAsync()`**: 啟動服務主迴圈
  - 顯示歡迎訊息
  - 根據登入狀態顯示對應選單
  - 處理使用者輸入和例外狀況

- **`ShowLoginMenuAsync()`**: 顯示登入選單
  - 提供註冊、登入、離開選項
  - 處理使用者選擇並導向對應功能

- **`ShowMainMenuAsync()`**: 顯示主選單 (已登入)
  - 提供租車、登出選項
  - 處理已登入使用者的操作

- **`RegisterAsync()`**: 處理使用者註冊
  - 收集使用者基本資料 (ID、姓名、Email)
  - 驗證必填欄位
  - 呼叫註冊命令處理器
  - 顯示註冊結果

- **`LoginAsync()`**: 處理使用者登入
  - 收集登入憑證 (使用者ID、密碼)
  - 呼叫登入命令處理器
  - 設定當前使用者狀態

- **`RentCarAsync()`**: 租車流程主方法
  - 協調整個租車流程的各個步驟
  - 依序執行車型選擇、車輛選擇、期間選擇、確認租賃

- **`SelectCarTypeAsync()`**: 車型選擇功能
  - 顯示可用車型清單和價格
  - 處理使用者車型選擇
  - 返回選中的車型或null

- **`SelectAvailableCarAsync()`**: 車輛選擇功能
  - 查詢指定車型的可用車輛
  - 顯示車輛清單供使用者選擇
  - 處理無可用車輛的情況

- **`SelectRentalPeriod()`**: 租期選擇功能
  - 收集租賃開始和結束日期
  - 驗證日期邏輯正確性
  - 返回有效的租期或null

- **`ConfirmRentalAsync()`**: 租賃確認功能
  - 計算和顯示租賃摘要資訊
  - 處理使用者最終確認
  - 呼叫建立租賃命令處理器
  - 顯示租賃結果

**ServiceCollectionExtensions 類別** (`Services/ServiceCollectionExtensions.cs`)
- **用途**: 依賴注入擴充方法
- **主要方法**:
  - `AddRentalCarServices()`: 註冊系統所有服務到DI容器

#### 4.3 測試工具

**SimpleTest 類別** (`SimpleTest.cs`)
- **用途**: 提供簡單的系統功能測試
- **主要方法**:
  - `RunTestAsync()`: 執行基本功能測試流程

## 系統操作指南

### 環境需求

- .NET 9 SDK
- 支援 C# 12 的開發環境 (Visual Studio 2022 或 VS Code)

### 編譯和執行

1. **編譯專案**:
   ```bash
   dotnet build
   ```

2. **執行系統**:
   ```bash
   cd src/RentalCarSystem.Web
   dotnet run
   ```

3. **執行測試模式**:
   ```bash
   cd src/RentalCarSystem.Web
   dotnet run test
   ```

### 詳細操作步驟

#### 1. 系統啟動
- 執行程式後會顯示歡迎訊息：「=== 歡迎使用租車系統 ===」
- 系統會自動顯示登入選單

#### 2. 用戶註冊流程
1. 在登入選單選擇「1. 註冊帳號」
2. 依序輸入以下資訊：
   - **使用者ID**: 英數字組合，用於登入 (必填)
   - **姓名**: 使用者真實姓名 (必填)
   - **Email**: 有效的電子郵件地址 (必填)
3. 系統驗證資料格式和使用者ID唯一性
4. 註冊成功後顯示客戶ID，按任意鍵返回登入選單

#### 3. 用戶登入流程
1. 在登入選單選擇「2. 登入」
2. 輸入註冊時設定的使用者ID和密碼
3. 登入成功後進入主選單
4. 登入失敗會顯示錯誤訊息並返回登入選單

#### 4. 租車操作流程

**步驟1: 選擇車型**
1. 在主選單選擇「1. 租車」
2. 系統顯示車型選項：
   - `1. 轎車 (Car) - $1,000/天`
   - `2. 休旅車 (SUV) - $1,500/天`
   - `3. 卡車 (Truck) - $2,000/天`
   - `4. 跑車 (SportsCar) - $3,000/天`
   - `5. 電動車 (ElectricCar) - $2,800/天`
   - `0. 返回主選單`
3. 輸入對應數字選擇車型

**步驟2: 選擇具體車輛**
1. 系統顯示選中車型的可用車輛清單
2. 每輛車顯示：車型名稱和每日租金
3. 輸入對應數字選擇車輛
4. 如無可用車輛，系統提示並返回車型選擇

**步驟3: 設定租期**
1. **開始日期**: 
   - 輸入格式：`yyyy-MM-dd` (例：2024-01-15)
   - 按Enter使用今天日期作為開始日期
2. **結束日期**: 
   - 輸入格式：`yyyy-MM-dd` (例：2024-01-20)
   - 必須晚於開始日期
3. 系統驗證日期邏輯正確性

**步驟4: 確認租賃**
1. 系統顯示租賃摘要：
   - 選中車輛型號
   - 租用期間和天數
   - 每日租金
   - 總租金金額
2. 輸入 `y` 或 `yes` 確認租賃
3. 輸入其他任何內容取消租賃
4. 確認後系統建立租賃記錄並顯示租賃ID

#### 5. 登出操作
- 在主選單選擇「2. 登出」
- 系統清除當前使用者狀態並返回登入選單

#### 6. 系統退出
- 在登入選單選擇「3. 離開系統」
- 程式結束執行

### 錯誤處理說明

系統具備完整的錯誤處理機制：

1. **輸入驗證錯誤**: 
   - 空白或無效輸入
   - 日期格式錯誤
   - 數字格式錯誤

2. **業務邏輯錯誤**:
   - 使用者ID重複
   - 登入憑證錯誤
   - 無可用車輛
   - 無效租期

3. **系統錯誤**:
   - 資料存取錯誤
   - 服務解析失敗

所有錯誤都會顯示友善的中文訊息，並提示使用者按任意鍵繼續操作。

### 測試模式使用

測試模式提供自動化的系統功能驗證：
1. 自動建立測試客戶
2. 自動建立測試車輛
3. 執行基本租賃流程測試
4. 驗證各層間的整合正常運作

執行測試模式可快速驗證系統各功能模組是否正常運作。

## 設計模式和架構特點

1. **Clean Architecture**: 依賴反轉，核心業務邏輯獨立於外部框架
2. **Domain-Driven Design**: 豐富的領域模型，業務邏輯封裝在領域物件中
3. **CQRS Pattern**: 命令查詢職責分離，清晰的讀寫操作區分
4. **Repository Pattern**: 抽象化資料存取，便於測試和替換實現
5. **Unit of Work Pattern**: 管理交易邊界和資料一致性
6. **Value Object Pattern**: 不可變值物件，確保資料完整性
7. **Factory Pattern**: 靜態工廠方法建立領域物件
8. **Dependency Injection**: 鬆耦合的依賴管理

此架構設計讓系統具備良好的可測試性、可維護性和可擴展性。