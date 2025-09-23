using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Application.Common;
using RentalCarSystem.Application.UseCases.Customers;
using RentalCarSystem.Application.UseCases.Rentals;
using RentalCarSystem.Application.UseCases.Cars;
using RentalCarSystem.Domain.Services;
using RentalCarSystem.Infrastructure.Repositories;
using RentalCarSystem.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace RentalCarSystem.Web.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRentalCarServices(this IServiceCollection services)
    {
        // Domain Services
        services.AddScoped<RentalFeeCalculator>();

        // Repositories (Infrastructure)
        services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
        services.AddSingleton<ICarRepository, InMemoryCarRepository>();
        services.AddSingleton<IRentalRepository, InMemoryRentalRepository>();
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

        // Command Handlers (Application)
        services.AddScoped<ICommandHandler<RegisterCustomerCommand, Guid>, RegisterCustomerCommandHandler>();
        services.AddScoped<ICommandHandler<LoginCustomerCommand, Guid?>, LoginCustomerCommandHandler>();
        services.AddScoped<ICommandHandler<CreateRentalCommand, Guid>, CreateRentalCommandHandler>();

        // Query Handlers (Application)
        services.AddScoped<IQueryHandler<GetAvailableCarsByTypeQuery, IReadOnlyList<CarDto>>, GetAvailableCarsByTypeQueryHandler>();

        return services;
    }
}