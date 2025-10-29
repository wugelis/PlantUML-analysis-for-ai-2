using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Domain.Entities;

/// <summary>
/// 租賃狀態列舉，定義租賃在整個生命週期中的各種狀態
/// 狀態轉換遵循特定的業務規則，確保租賃流程的完整性
/// </summary>
public enum RentalStatus
{
    /// <summary>
    /// 待確認狀態 - 租賃申請已提交，等待系統或管理員確認
    /// 這是新建立租賃的初始狀態
    /// </summary>
    Pending,
    
    /// <summary>
    /// 已確認狀態 - 租賃申請已通過審核，客戶可以開始租用
    /// 此狀態下車輛已被預留，等待客戶取車
    /// </summary>
    Confirmed,
    
    /// <summary>
    /// 進行中狀態 - 客戶已取車，租賃正在進行中
    /// 車輛在使用中，不能被其他客戶租用
    /// </summary>
    Active,
    
    /// <summary>
    /// 已完成狀態 - 客戶已還車，租賃正常結束
    /// 車輛已歸還並可供其他客戶租用
    /// </summary>
    Completed,
    
    /// <summary>
    /// 已取消狀態 - 租賃被取消（客戶取消或系統取消）
    /// 車輛重新變為可租用狀態
    /// </summary>
    Cancelled
}

/// <summary>
/// 租賃實體類別，代表客戶對特定車輛的租用記錄
/// 管理租賃的完整生命週期，包括狀態轉換和業務規則驗證
/// 繼承自 Entity&lt;Guid&gt;，使用 GUID 作為唯一識別碼
/// </summary>
public class Rental : Entity<Guid>
{
    /// <summary>
    /// 客戶識別碼，指向租用此車輛的客戶
    /// 用於建立租賃與客戶之間的關聯關係
    /// </summary>
    public Guid CustomerId { get; private set; }
    
    /// <summary>
    /// 車輛識別碼，指向被租用的車輛
    /// 用於建立租賃與車輛之間的關聯關係
    /// </summary>
    public Guid CarId { get; private set; }
    
    /// <summary>
    /// 租用期間，包含開始日期和結束日期
    /// 使用 RentalPeriod 值物件確保日期的有效性和業務規則
    /// </summary>
    public RentalPeriod Period { get; private set; }
    
    /// <summary>
    /// 租賃總費用，根據車輛日租金和租用天數計算
    /// 使用 Money 值物件處理金額和貨幣相關邏輯
    /// </summary>
    public Money TotalFee { get; private set; }
    
    /// <summary>
    /// 當前租賃狀態，控制租賃的生命週期
    /// 狀態轉換必須遵循特定的業務規則
    /// </summary>
    public RentalStatus Status { get; private set; }
    
    /// <summary>
    /// 租賃記錄建立時間（UTC 時間）
    /// 用於追蹤租賃申請時間和資料審計
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// 私有建構函式，防止外部直接建立實例
    /// 確保所有 Rental 實例都透過 Create 靜態方法建立，維持業務規則的一致性
    /// </summary>
    /// <param name="id">租賃的唯一識別碼</param>
    /// <param name="customerId">客戶識別碼</param>
    /// <param name="carId">車輛識別碼</param>
    /// <param name="period">租用期間</param>
    /// <param name="totalFee">租賃總費用</param>
    private Rental(Guid id, Guid customerId, Guid carId, RentalPeriod period, Money totalFee) : base(id)
    {
        CustomerId = customerId;
        CarId = carId;
        Period = period;
        TotalFee = totalFee;
        Status = RentalStatus.Pending; // 新建立的租賃預設為待確認狀態
        CreatedAt = DateTime.UtcNow; // 設定為當前 UTC 時間
    }
    
    /// <summary>
    /// 靜態工廠方法，用於建立新的租賃實例
    /// 建立時會自動設定為 Pending 狀態和當前時間
    /// </summary>
    /// <param name="customerId">租用車輛的客戶識別碼</param>
    /// <param name="carId">被租用的車輛識別碼</param>
    /// <param name="period">租用期間，必須是有效的日期範圍</param>
    /// <param name="totalFee">租賃總費用，通常由外部服務計算得出</param>
    /// <returns>新建立的租賃實例，狀態為 Pending</returns>
    public static Rental Create(Guid customerId, Guid carId, RentalPeriod period, Money totalFee)
    {
        return new Rental(Guid.NewGuid(), customerId, carId, period, totalFee);
    }
    
    /// <summary>
    /// 確認租賃申請
    /// 將租賃狀態從 Pending 轉換為 Confirmed
    /// 通常在系統驗證車輛可用性和客戶資格後執行
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// 當租賃狀態不是 Pending 時拋出例外
    /// 確保狀態轉換的業務規則正確性
    /// </exception>
    public void Confirm()
    {
        if (Status != RentalStatus.Pending)
            throw new InvalidOperationException("只有待確認的租賃才能被確認");
            
        Status = RentalStatus.Confirmed;
    }
    
    /// <summary>
    /// 開始租賃（客戶取車）
    /// 將租賃狀態從 Confirmed 轉換為 Active
    /// 表示客戶已實際取得車輛使用權
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// 當租賃狀態不是 Confirmed 時拋出例外
    /// 確保客戶只能在租賃已確認後才能取車
    /// </exception>
    public void Start()
    {
        if (Status != RentalStatus.Confirmed)
            throw new InvalidOperationException("只有已確認的租賃才能開始");
            
        Status = RentalStatus.Active;
    }
    
    /// <summary>
    /// 完成租賃（客戶還車）
    /// 將租賃狀態從 Active 轉換為 Completed
    /// 表示租賃正常結束，車輛已歸還
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// 當租賃狀態不是 Active 時拋出例外
    /// 確保只有進行中的租賃才能被標記為完成
    /// </exception>
    public void Complete()
    {
        if (Status != RentalStatus.Active)
            throw new InvalidOperationException("只有進行中的租賃才能完成");
            
        Status = RentalStatus.Completed;
    }
    
    /// <summary>
    /// 取消租賃
    /// 將租賃狀態轉換為 Cancelled
    /// 可以在 Pending、Confirmed 或 Active 狀態下取消
    /// 但不能取消已完成的租賃（業務規則）
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// 當嘗試取消已完成的租賃時拋出例外
    /// 已完成的租賃代表交易已結束，不應被取消
    /// </exception>
    public void Cancel()
    {
        if (Status == RentalStatus.Completed)
            throw new InvalidOperationException("已完成的租賃無法取消");
            
        Status = RentalStatus.Cancelled;
    }
}