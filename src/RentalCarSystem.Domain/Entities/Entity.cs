namespace RentalCarSystem.Domain.Entities;

/// <summary>
/// 抽象基底實體類別，為所有領域實體提供統一的識別碼和相等性比較功能
/// 使用泛型約束確保識別碼類型實作 IEquatable&lt;T&gt; 介面
/// </summary>
/// <typeparam name="T">識別碼的類型，必須實作 IEquatable&lt;T&gt; 介面（如 Guid、int 等）</typeparam>
public abstract class Entity<T> where T : IEquatable<T>
{
    /// <summary>
    /// 實體的唯一識別碼
    /// 使用 protected init 確保只能在建構函式或繼承類別中設定，且僅能設定一次
    /// </summary>
    public T Id { get; protected init; }
    
    /// <summary>
    /// 受保護的建構函式，確保只有繼承類別可以建立實例
    /// 強制所有實體都必須有識別碼
    /// </summary>
    /// <param name="id">實體的唯一識別碼</param>
    protected Entity(T id)
    {
        Id = id;
    }
    
    /// <summary>
    /// 覆寫物件相等性比較方法
    /// 實體的相等性基於識別碼，而非參考或屬性值
    /// 這符合 DDD（領域驅動設計）中實體的語義
    /// </summary>
    /// <param name="obj">要比較的物件</param>
    /// <returns>
    /// 如果兩個實體具有相同的識別碼則返回 true，否則返回 false
    /// 如果傳入的物件不是相同類型的實體，則返回 false
    /// </returns>
    public override bool Equals(object? obj)
    {
        // 檢查傳入物件是否為相同類型的實體
        if (obj is not Entity<T> other)
            return false;
            
        // 檢查是否為同一個物件參考（效能優化）
        if (ReferenceEquals(this, other))
            return true;
            
        // 比較識別碼是否相等
        return Id.Equals(other.Id);
    }
    
    /// <summary>
    /// 覆寫雜湊碼方法，確保相等的實體具有相同的雜湊碼
    /// 這對於實體在 Dictionary、HashSet 等集合中的正確運作至關重要
    /// </summary>
    /// <returns>基於識別碼的雜湊碼</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    /// <summary>
    /// 相等運算子多載，提供更直觀的相等性比較語法
    /// 處理 null 值的情況，確保比較的安全性
    /// </summary>
    /// <param name="left">左邊的實體（可為 null）</param>
    /// <param name="right">右邊的實體（可為 null）</param>
    /// <returns>
    /// 如果兩個實體相等或都為 null 則返回 true
    /// 如果其中一個為 null 而另一個不為 null 則返回 false
    /// </returns>
    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        // 使用 null-conditional operator 和 null-coalescing operator 處理 null 情況
        // 如果 left 為 null，則檢查 right 是否也為 null
        // 如果 left 不為 null，則呼叫 Equals 方法比較
        return left?.Equals(right) ?? right is null;
    }
    
    /// <summary>
    /// 不相等運算子多載，提供直觀的不相等比較語法
    /// 直接使用相等運算子的反向結果
    /// </summary>
    /// <param name="left">左邊的實體（可為 null）</param>
    /// <param name="right">右邊的實體（可為 null）</param>
    /// <returns>如果兩個實體不相等則返回 true，否則返回 false</returns>
    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }
}