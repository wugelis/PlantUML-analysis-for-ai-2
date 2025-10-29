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


#### 2.3 用例實現 (Use Cases)

**客戶管理用例**:

**RegisterCustomerCommand** (`UseCases/Customers/RegisterCustomerCommand.cs`)
- **用途**: 註冊新客戶的命令
- **處理器方法**:
  - `Handle()`: 檢查使用者ID唯一性，建立新客戶實體，保存至資料庫

**LoginCustomerCommand** (`UseCases/Customers/LoginCustomerCommand.cs`)
- **用途**: 客戶登入驗證命令
- **處理器方法**:
  - `Handle()`: 驗證使用者憑證，檢驗使用者是否為 VIP，返回客戶ID或null

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
