using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Domain.Entities;

public class Car : Entity<Guid>
{
    public string Model { get; private set; }
    public CarType CarType { get; private set; }
    public Money DailyRate { get; private set; }
    public bool IsAvailable { get; private set; }
    
    private Car(Guid id, string model, CarType carType, Money dailyRate) : base(id)
    {
        Model = model;
        CarType = carType;
        DailyRate = dailyRate;
        IsAvailable = true;
    }
    
    public static Car Create(string model, CarType carType)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("車輛型號不能為空");
            
        var carTypeInfo = CarTypeInfo.GetByType(carType);
        return new Car(Guid.NewGuid(), model, carType, new Money(carTypeInfo.DailyRate));
    }
    
    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }
    
    public Money CalculateRentalFee(RentalPeriod period)
    {
        if (period.Days <= 0)
            throw new ArgumentException("租用天數必須大於0");
            
        return DailyRate.Multiply(period.Days);
    }
}