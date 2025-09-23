using RentalCarSystem.Domain.Entities;
using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Domain.Services;

public class RentalFeeCalculator
{
    public Money CalculateRentalFee(Car car, RentalPeriod period)
    {
        if (car == null)
            throw new ArgumentNullException(nameof(car));
            
        if (period == null)
            throw new ArgumentNullException(nameof(period));
            
        return car.CalculateRentalFee(period);
    }
    
    public Money CalculateRentalFeeWithDiscount(Car car, RentalPeriod period, decimal discountPercentage = 0)
    {
        var baseFee = CalculateRentalFee(car, period);
        
        if (discountPercentage <= 0)
            return baseFee;
            
        var discount = baseFee.Multiply(discountPercentage / 100);
        return baseFee.Add(new Money(-discount.Amount));
    }
}