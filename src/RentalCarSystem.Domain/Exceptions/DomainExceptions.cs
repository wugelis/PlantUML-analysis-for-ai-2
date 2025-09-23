namespace RentalCarSystem.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
    
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class CarNotFoundException : DomainException
{
    public CarNotFoundException(string carType) : base($"車型 '{carType}' 未找到")
    {
    }
}

public class CustomerNotFoundException : DomainException
{
    public CustomerNotFoundException(Guid customerId) : base($"客戶 '{customerId}' 未找到")
    {
    }
}

public class InvalidRentalPeriodException : DomainException
{
    public InvalidRentalPeriodException(string message) : base(message)
    {
    }
}