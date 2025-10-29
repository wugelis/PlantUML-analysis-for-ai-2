using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Domain.Entities;

/// <summary>
/// 客戶實體類別，代表租車系統中的客戶資訊
/// 繼承自 Entity&lt;Guid&gt;，使用 GUID 作為唯一識別碼
/// 負責管理客戶的基本資料和相關業務規則
/// </summary>
public class Customer : Entity<Guid>
{
    /// <summary>
    /// 關聯的系統使用者識別碼
    /// 用於連結身份驗證系統中的使用者帳號
    /// </summary>
    public string UserId { get; private set; }
    
    /// <summary>
    /// 客戶姓名
    /// 用於顯示和身份識別，不可為空或空白
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// 客戶電子郵件地址
    /// 用於通知和聯繫，不可為空或空白
    /// </summary>
    public string Email { get; private set; }
    
    /// <summary>
    /// 客戶資料建立時間（UTC 時間）
    /// 用於追蹤客戶註冊時間和資料審計
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// 私有建構函式，防止外部直接建立實例
    /// 確保所有 Customer 實例都透過 Create 靜態方法建立，維持資料完整性
    /// </summary>
    /// <param name="id">客戶的唯一識別碼</param>
    /// <param name="userId">關聯的系統使用者識別碼</param>
    /// <param name="name">客戶姓名</param>
    /// <param name="email">客戶電子郵件地址</param>
    private Customer(Guid id, string userId, string name, string email) : base(id)
    {
        UserId = userId;
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow; // 設定為當前 UTC 時間
    }
    
    /// <summary>
    /// 靜態工廠方法，用於建立新的客戶實例
    /// 執行業務規則驗證，確保客戶資料的有效性
    /// </summary>
    /// <param name="userId">系統使用者識別碼，不可為空或空白</param>
    /// <param name="name">客戶姓名，不可為空或空白</param>
    /// <param name="email">電子郵件地址，不可為空或空白</param>
    /// <returns>新建立的客戶實例</returns>
    /// <exception cref="ArgumentException">
    /// 當任何參數為空或空白字串時拋出例外：
    /// - 使用者ID不能為空
    /// - 姓名不能為空  
    /// - 信箱不能為空
    /// </exception>
    public static Customer Create(string userId, string name, string email)
    {
        // 驗證使用者ID不能為空或空白
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("使用者ID不能為空");
            
        // 驗證姓名不能為空或空白
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("姓名不能為空");
            
        // 驗證電子郵件不能為空或空白
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("信箱不能為空");
            
        // 建立新的客戶實例，使用新的 GUID 作為識別碼
        return new Customer(Guid.NewGuid(), userId, name, email);
    }
    
    /// <summary>
    /// 更新客戶的個人資料
    /// 允許修改姓名和電子郵件，但不允許修改使用者ID（系統識別碼不可變）
    /// </summary>
    /// <param name="name">新的客戶姓名，不可為空或空白</param>
    /// <param name="email">新的電子郵件地址，不可為空或空白</param>
    /// <exception cref="ArgumentException">
    /// 當任何參數為空或空白字串時拋出例外：
    /// - 姓名不能為空
    /// - 信箱不能為空
    /// </exception>
    public void UpdateProfile(string name, string email)
    {
        // 驗證新姓名不能為空或空白
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("姓名不能為空");
            
        // 驗證新電子郵件不能為空或空白
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("信箱不能為空");
            
        // 更新客戶資料
        Name = name;
        Email = email;
        
        // 注意：這裡沒有更新 CreatedAt，因為它代表初始建立時間
        // 如果需要追蹤最後修改時間，應該加入 LastModifiedAt 屬性
    }
    
    /// <summary>
    /// 檢查指定的使用者ID是否為VIP客戶
    /// VIP客戶才能租用跑車 (SportsCar)
    /// </summary>
    /// <param name="userId">要檢查的使用者ID</param>
    /// <returns>如果是VIP客戶則回傳true，否則回傳false</returns>
    public static bool IfCustomerIsVIP(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;
            
        // VIP客戶判別邏輯：
        // 1. 使用者ID以 "VIP" 開頭 (例如: VIP001, VIPJohn)
        // 2. 或者使用者ID包含 "premium" (不區分大小寫)
        // 3. 或者特定的VIP客戶列表
        var vipUserIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "admin", "manager", "director", "ceo", "vip001", "vip002"
        };
        
        return userId.StartsWith("VIP", StringComparison.OrdinalIgnoreCase) ||
               userId.Contains("premium", StringComparison.OrdinalIgnoreCase) ||
               vipUserIds.Contains(userId);
    }
}