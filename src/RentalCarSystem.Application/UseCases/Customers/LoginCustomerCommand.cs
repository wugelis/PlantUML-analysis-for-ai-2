using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Application.Common;

namespace RentalCarSystem.Application.UseCases.Customers;

public record LoginCustomerCommand(
    string UserId,
    string Password
) : ICommand<Guid?>;

public record LoginResult(bool Success, Guid? CustomerId);

public class LoginCustomerCommandHandler : ICommandHandler<LoginCustomerCommand, Guid?>
{
    private readonly ICustomerRepository _customerRepository;

    public LoginCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Guid?> Handle(LoginCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        
        if (customer == null)
        {
            return null;
        }

        // 在實際應用中，這裡應該要驗證密碼
        // 目前簡化為只要使用者存在就回傳客戶ID
        return customer.Id;
    }
}