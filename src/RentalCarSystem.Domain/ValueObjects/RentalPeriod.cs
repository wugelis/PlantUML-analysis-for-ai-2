using RentalCarSystem.Domain.Exceptions;

namespace RentalCarSystem.Domain.ValueObjects;

public record RentalPeriod
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public int Days => (EndDate - StartDate).Days;
    
    public RentalPeriod(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new InvalidRentalPeriodException("結束日期必須晚於開始日期");
            
        if (startDate < DateTime.Today)
            throw new InvalidRentalPeriodException("開始日期不能早於今天");
            
        StartDate = startDate;
        EndDate = endDate;
    }
    
    public static RentalPeriod Create(DateTime startDate, DateTime endDate)
    {
        return new RentalPeriod(startDate, endDate);
    }
    
    public override string ToString()
    {
        return $"{StartDate:yyyy-MM-dd} ~ {EndDate:yyyy-MM-dd} ({Days} 天)";
    }
}