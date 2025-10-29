using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Domain.Entities;

/// <summary>
/// 車輛實體類別，代表租車系統中的一輛車
/// 繼承自 Entity<Guid>，使用 GUID 作為唯一識別碼
/// </summary>
public class Car : Entity<Guid>
{
    /// <summary>
    /// 車輛型號（例如：Toyota Camry、Honda CR-V）
    /// 使用 private set 確保只能透過建構函式或特定方法設定
    /// </summary>
    public string Model { get; private set; }
    
    /// <summary>
    /// 車輛類型（轎車、SUV、卡車、跑車、電動車等）
    /// 使用 private set 確保資料完整性
    /// </summary>
    public CarType CarType { get; private set; }
    
    /// <summary>
    /// 每日租金，使用 Money 值物件來處理金額和貨幣
    /// 根據車輛類型自動設定對應的租金費率
    /// </summary>
    public Money DailyRate { get; private set; }
    
    /// <summary>
    /// 車輛是否可租用的狀態
    /// true: 可租用，false: 已被租用或維修中
    /// </summary>
    public bool IsAvailable { get; private set; }
    
    /// <summary>
    /// 私有建構函式，防止外部直接建立實例
    /// 確保所有 Car 實例都透過 Create 靜態方法建立，維持業務規則的一致性
    /// </summary>
    /// <param name="id">車輛的唯一識別碼</param>
    /// <param name="model">車輛型號</param>
    /// <param name="carType">車輛類型</param>
    /// <param name="dailyRate">每日租金</param>
    private Car(Guid id, string model, CarType carType, Money dailyRate) : base(id)
    {
        Model = model;
        CarType = carType;
        DailyRate = dailyRate;
        IsAvailable = true; // 新建立的車輛預設為可租用狀態
    }
    
    /// <summary>
    /// 靜態工廠方法，用於建立新的車輛實例
    /// 確保車輛建立時遵循業務規則（型號驗證、租金設定等）
    /// </summary>
    /// <param name="model">車輛型號，不可為空或空白字串</param>
    /// <param name="carType">車輛類型，用於決定租金費率</param>
    /// <returns>新建立的車輛實例</returns>
    /// <exception cref="ArgumentException">當車輛型號為空或空白時拋出例外</exception>
    public static Car Create(string model, CarType carType)
    {
        // 驗證車輛型號不能為空
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("車輛型號不能為空");
            
        // 根據車輛類型取得對應的租金資訊
        var carTypeInfo = CarTypeInfo.GetByType(carType);
        
        // 建立新的車輛實例，使用新的 GUID 作為識別碼
        return new Car(Guid.NewGuid(), model, carType, new Money(carTypeInfo.DailyRate));
    }
    
    /// <summary>
    /// 設定車輛的可租用狀態
    /// 用於車輛租出、歸還、維修等狀態變更
    /// </summary>
    /// <param name="isAvailable">
    /// true: 設定為可租用（車輛歸還或維修完成）
    /// false: 設定為不可租用（車輛已租出或進行維修）
    /// </param>
    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }
    
    /// <summary>
    /// 計算指定租用期間的總租金
    /// 根據每日租金和租用天數計算總費用
    /// </summary>
    /// <param name="period">租用期間，包含開始日期和結束日期</param>
    /// <returns>總租金金額（Money 值物件）</returns>
    /// <exception cref="ArgumentException">當租用天數小於等於 0 時拋出例外</exception>
    public Money CalculateRentalFee(RentalPeriod period)
    {
        // 驗證租用天數必須為正數
        if (period.Days <= 0)
            throw new ArgumentException("租用天數必須大於0");
            
        // 使用 Money 值物件的 Multiply 方法計算總租金
        // 每日租金 × 租用天數 = 總租金
        return DailyRate.Multiply(period.Days);
    }
}