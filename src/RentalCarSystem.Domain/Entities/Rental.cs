using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Domain.Entities;

public enum RentalStatus
{
    Pending,
    Confirmed,
    Active,
    Completed,
    Cancelled
}

public class Rental : Entity<Guid>
{
    public Guid CustomerId { get; private set; }
    public Guid CarId { get; private set; }
    public RentalPeriod Period { get; private set; }
    public Money TotalFee { get; private set; }
    public RentalStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private Rental(Guid id, Guid customerId, Guid carId, RentalPeriod period, Money totalFee) : base(id)
    {
        CustomerId = customerId;
        CarId = carId;
        Period = period;
        TotalFee = totalFee;
        Status = RentalStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static Rental Create(Guid customerId, Guid carId, RentalPeriod period, Money totalFee)
    {
        return new Rental(Guid.NewGuid(), customerId, carId, period, totalFee);
    }
    
    public void Confirm()
    {
        if (Status != RentalStatus.Pending)
            throw new InvalidOperationException("只有待確認的租賃才能被確認");
            
        Status = RentalStatus.Confirmed;
    }
    
    public void Start()
    {
        if (Status != RentalStatus.Confirmed)
            throw new InvalidOperationException("只有已確認的租賃才能開始");
            
        Status = RentalStatus.Active;
    }
    
    public void Complete()
    {
        if (Status != RentalStatus.Active)
            throw new InvalidOperationException("只有進行中的租賃才能完成");
            
        Status = RentalStatus.Completed;
    }
    
    public void Cancel()
    {
        if (Status == RentalStatus.Completed)
            throw new InvalidOperationException("已完成的租賃無法取消");
            
        Status = RentalStatus.Cancelled;
    }
}