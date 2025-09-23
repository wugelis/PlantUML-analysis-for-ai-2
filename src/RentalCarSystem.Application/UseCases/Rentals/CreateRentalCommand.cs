using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Application.Common;
using RentalCarSystem.Domain.Entities;
using RentalCarSystem.Domain.ValueObjects;
using RentalCarSystem.Domain.Services;
using RentalCarSystem.Domain.Exceptions;

namespace RentalCarSystem.Application.UseCases.Rentals;

public record CreateRentalCommand(
    Guid CustomerId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate
) : ICommand<Guid>;

public class CreateRentalCommandHandler : ICommandHandler<CreateRentalCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICarRepository _carRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RentalFeeCalculator _feeCalculator;

    public CreateRentalCommandHandler(
        ICustomerRepository customerRepository,
        ICarRepository carRepository,
        IRentalRepository rentalRepository,
        IUnitOfWork unitOfWork,
        RentalFeeCalculator feeCalculator)
    {
        _customerRepository = customerRepository;
        _carRepository = carRepository;
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
        _feeCalculator = feeCalculator;
    }

    public async Task<Guid> Handle(CreateRentalCommand command, CancellationToken cancellationToken = default)
    {
        // 驗證客戶存在
        var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }

        // 驗證車輛存在且可用
        var car = await _carRepository.GetByIdAsync(command.CarId, cancellationToken);
        if (car == null)
        {
            throw new DomainException($"車輛 '{command.CarId}' 未找到");
        }

        if (!car.IsAvailable)
        {
            throw new DomainException("車輛目前不可用");
        }

        // 建立租用期間
        var period = RentalPeriod.Create(command.StartDate, command.EndDate);
        
        // 計算租金
        var totalFee = _feeCalculator.CalculateRentalFee(car, period);
        
        // 建立租賃記錄
        var rental = Rental.Create(customer.Id, car.Id, period, totalFee);
        
        // 更新車輛可用狀態
        car.SetAvailability(false);
        
        await _rentalRepository.AddAsync(rental, cancellationToken);
        await _carRepository.UpdateAsync(car, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return rental.Id;
    }
}