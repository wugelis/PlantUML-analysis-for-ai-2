namespace RentalCarSystem.Domain.ValueObjects;

public enum CarType
{
    Car,
    SUV,
    Truck,
    SportsCar
}

public record CarTypeInfo(CarType Type, decimal DailyRate, string DisplayName)
{
    public static readonly Dictionary<CarType, CarTypeInfo> CarTypes = new()
    {
        { CarType.Car, new CarTypeInfo(CarType.Car, 1000m, "轎車") },
        { CarType.SUV, new CarTypeInfo(CarType.SUV, 1500m, "休旅車") },
        { CarType.Truck, new CarTypeInfo(CarType.Truck, 2000m, "卡車") },
        { CarType.SportsCar, new CarTypeInfo(CarType.SportsCar, 3000m, "跑車") }
    };
    
    public static CarTypeInfo GetByType(CarType type)
    {
        return CarTypes.TryGetValue(type, out var info) 
            ? info 
            : throw new ArgumentException($"未知車型: {type}");
    }
    
    public static CarTypeInfo GetByName(string carTypeName)
    {
        if (Enum.TryParse<CarType>(carTypeName, out var carType))
        {
            return GetByType(carType);
        }
        
        throw new ArgumentException($"未知車型名稱: {carTypeName}");
    }
}