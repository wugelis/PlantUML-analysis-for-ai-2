using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Application.UseCases.Customers;
using RentalCarSystem.Application.UseCases.Rentals;
using RentalCarSystem.Application.UseCases.Cars;
using RentalCarSystem.Domain.Services;
using RentalCarSystem.Infrastructure.Repositories;
using RentalCarSystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 註冊 Application Services (CQRS Commands & Queries)
builder.Services.AddScoped<RegisterCustomerCommandHandler>();
builder.Services.AddScoped<LoginCustomerCommandHandler>();
builder.Services.AddScoped<CreateRentalCommandHandler>();
builder.Services.AddScoped<GetAvailableCarsByTypeQueryHandler>();

// 註冊 Domain Services
builder.Services.AddScoped<RentalFeeCalculator>();

// 註冊 Infrastructure Services (Repositories)
builder.Services.AddScoped<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddScoped<ICarRepository, InMemoryCarRepository>();
builder.Services.AddScoped<IRentalRepository, InMemoryRentalRepository>();
builder.Services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

// 配置 Session (用於登入狀態維護)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();