using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Domain.Entities;

public class Customer : Entity<Guid>
{
    public string UserId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private Customer(Guid id, string userId, string name, string email) : base(id)
    {
        UserId = userId;
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static Customer Create(string userId, string name, string email)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("使用者ID不能為空");
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("姓名不能為空");
            
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("信箱不能為空");
            
        return new Customer(Guid.NewGuid(), userId, name, email);
    }
    
    public void UpdateProfile(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("姓名不能為空");
            
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("信箱不能為空");
            
        Name = name;
        Email = email;
    }
}