using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Application.Common;
using RentalCarSystem.Domain.Entities;

namespace RentalCarSystem.Application.UseCases.Customers;

public record RegisterCustomerCommand(
    string UserId,
    string Name,
    string Email
) : ICommand<Guid>;

public class RegisterCustomerCommandHandler : ICommandHandler<RegisterCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegisterCustomerCommand command, CancellationToken cancellationToken = default)
    {
        // 檢查使用者ID是否已存在
        var existingCustomer = await _customerRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (existingCustomer != null)
        {
            throw new InvalidOperationException($"使用者ID '{command.UserId}' 已存在");
        }

        // 建立新客戶
        var customer = Customer.Create(command.UserId, command.Name, command.Email);
        
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return customer.Id;
    }
}