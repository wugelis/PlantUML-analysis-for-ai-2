namespace RentalCarSystem.Domain.Entities;

public abstract class Entity<T> where T : IEquatable<T>
{
    public T Id { get; protected init; }
    
    protected Entity(T id)
    {
        Id = id;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<T> other)
            return false;
            
        if (ReferenceEquals(this, other))
            return true;
            
        return Id.Equals(other.Id);
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        return left?.Equals(right) ?? right is null;
    }
    
    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }
}