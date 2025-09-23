# 租車系統 - Hexagonal Architecture

這是一個基於 Hexagonal Architecture (六角架構) 的租車系統，使用 .NET 9 和 C# 實作。

## 專案結構

```
src/
├── RentalCarSystem.Domain/          # 領域層 - 核心商業邏輯
│   ├── Entities/                    # 領域實體
│   │   ├── Entity.cs               # 基底實體類別
│   │   ├── Customer.cs             # 客戶實體
│   │   ├── Car.cs                  # 車輛實體
│   │   └── Rental.cs               # 租賃實體
│   ├── ValueObjects/                # 值物件
│   │   ├── CarTypeInfo.cs          # 車型資訊
│   │   ├── RentalPeriod.cs         # 租用期間
│   │   └── Money.cs                # 金額
│   ├── Services/                    # 領域服務
│   │   └── RentalFeeCalculator.cs  # 租金計算器
│   └── Exceptions/                  # 領域例外
│       └── DomainExceptions.cs     # 領域相關例外
│
├── RentalCarSystem.Application/     # 應用層 - Use Cases
│   ├── Abstractions/               # 介面定義 (Ports)
│   │   ├── ICustomerRepository.cs  # 客戶倉儲介面
│   │   ├── ICarRepository.cs       # 車輛倉儲介面
│   │   ├── IRentalRepository.cs    # 租賃倉儲介面
│   │   └── IUnitOfWork.cs          # 工作單元介面
│   ├── UseCases/                   # 使用案例
│   │   ├── Customers/              # 客戶相關操作
│   │   ├── Cars/                   # 車輛相關操作
│   │   └── Rentals/                # 租賃相關操作
│   └── Common/                     # 共用元件
│       ├── ICommands.cs            # CQRS 介面
│       └── Dtos.cs                 # 資料傳輸物件
│
├── RentalCarSystem.Infrastructure/ # 基礎設施層 - Adapters
│   ├── Repositories/               # 倉儲實作
│   │   ├── InMemoryCustomerRepository.cs
│   │   ├── InMemoryCarRepository.cs
│   │   └── InMemoryRentalRepository.cs
│   └── Persistence/                # 持久化
│       └── InMemoryUnitOfWork.cs   # 記憶體工作單元
│
└── RentalCarSystem.Web/            # 介面層 - UI/API
    ├── Services/                   # 服務設定
    │   ├── ServiceCollectionExtensions.cs  # DI 設定
    │   └── ConsoleRentalService.cs         # Console UI 服務
    ├── Program.cs                  # 程式進入點
    └── SimpleTest.cs              # 簡單測試
```

## 架構原則

### 依賴方向
- `Web → Application → Domain`
- `Infrastructure → Domain` (實作介面)
- 所有依賴注入在 `Web` 層完成

### 分層職責

#### Domain 層
- 包含核心商業邏輯
- 無任何外部依賴
- 定義實體、值物件、領域服務

#### Application 層
- 定義使用案例 (Use Cases)
- 定義 Ports (介面)
- 協調領域物件完成業務流程

#### Infrastructure 層
- 實作 Application 層定義的介面
- 處理資料持久化、外部服務呼叫

#### Web 層
- 使用者介面 (此案例為 Console)
- 依賴注入設定 (Composition Root)
- 協調 Application 層的 Use Cases

## 核心功能

1. **客戶管理**
   - 註冊新客戶
   - 客戶登入

2. **車輛管理**
   - 查看可用車輛
   - 依車型篩選

3. **租賃管理**
   - 建立租賃記錄
   - 計算租金
   - 更新車輛狀態

## 執行方式

### 編譯專案
```bash
dotnet build
```

### 運行應用程式
```bash
dotnet run --project src\RentalCarSystem.Web\RentalCarSystem.Web.csproj
```

### 運行測試
```bash
dotnet run --project src\RentalCarSystem.Web\RentalCarSystem.Web.csproj test
```

## 技術特點

1. **清晰的分層架構** - 遵循六角架構原則
2. **CQRS 模式** - 分離命令和查詢操作
3. **依賴注入** - 使用 Microsoft.Extensions.DependencyInjection
4. **值物件設計** - 使用 Record 實作不可變值物件
5. **領域驅動設計** - Rich Domain Model
6. **可測試性** - 透過介面隔離實作細節

## 保留的原始邏輯

原始 `RentalCarSystem.cs` 中的租車流程邏輯被完整保留並重構為：

1. **車型選擇** → `CarTypeInfo` 值物件
2. **租用期間選擇** → `RentalPeriod` 值物件
3. **租金計算** → `RentalFeeCalculator` 領域服務
4. **租賃確認** → `CreateRentalCommand` Use Case

這個實作展示了如何將傳統的程式碼重構為符合現代軟體架構原則的設計。