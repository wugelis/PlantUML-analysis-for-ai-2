#### 1.0 資料傳輸物件 (DTOs)

**DTO 類別群** (`Common/Dtos.cs`)
- **CustomerDto**: 客戶資料傳輸物件
  - `Id` (Guid): 客戶的唯一識別碼，用於系統內部區分不同客戶
  - `UserId` (string): 客戶的使用者帳號或登入識別碼，用於身份驗證和登入
  - `Name` (string): 客戶的姓名，用於顯示和識別客戶身份
  - `Email` (string): 客戶的電子郵件地址，用於聯絡和通知服務
  - `CreatedAt` (DateTime): 客戶帳號建立的時間戳記，用於追蹤註冊時間和帳號管理

- **CarDto**: 車輛資料傳輸物件
- **RentalDto**: 租賃資料傳輸物件
- **CreateCustomerDto**: 建立客戶的請求物件
- **CreateRentalDto**: 建立租賃的請求物件