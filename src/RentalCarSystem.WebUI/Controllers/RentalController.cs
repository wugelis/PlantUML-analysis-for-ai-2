using Microsoft.AspNetCore.Mvc;
using RentalCarSystem.Application.UseCases.Cars;
using RentalCarSystem.Application.UseCases.Rentals;
using RentalCarSystem.Domain.Services;
using RentalCarSystem.WebUI.Models.ViewModels;
using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.WebUI.Controllers;

public class RentalController : Controller
{
    private readonly GetAvailableCarsByTypeQueryHandler _getAvailableCarsHandler;
    private readonly CreateRentalCommandHandler _createRentalHandler;
    private readonly RentalFeeCalculator _feeCalculator;
    private readonly ILogger<RentalController> _logger;

    public RentalController(
        GetAvailableCarsByTypeQueryHandler getAvailableCarsHandler,
        CreateRentalCommandHandler createRentalHandler,
        RentalFeeCalculator feeCalculator,
        ILogger<RentalController> logger)
    {
        _getAvailableCarsHandler = getAvailableCarsHandler;
        _createRentalHandler = createRentalHandler;
        _feeCalculator = feeCalculator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
        }

        var model = new RentalViewModel
        {
            CustomerId = GetCurrentCustomerId(),
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2)
        };

        await LoadAvailableCars(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LoadCarsByType(string carType, DateTime startDate, DateTime endDate)
    {
        try
        {
            var query = new GetAvailableCarsByTypeQuery(carType, startDate, endDate);
            var cars = await _getAvailableCarsHandler.Handle(query);

            var carItems = cars.Select(car => new CarSelectionItem
            {
                CarId = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                CarType = car.CarType.Type,
                DailyRate = car.CarType.DailyRate.Amount
            }).ToList();

            return Json(carItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "載入車輛時發生錯誤");
            return Json(new List<CarSelectionItem>());
        }
    }

    [HttpPost]
    public IActionResult CalculateFee(string carType, DateTime startDate, DateTime endDate)
    {
        try
        {
            var period = new RentalPeriod(startDate, endDate);
            var carTypeInfo = CarTypeInfo.Create(carType);
            var fee = _feeCalculator.CalculateFee(carTypeInfo, period);

            return Json(new { success = true, fee = fee.Amount, days = period.Days });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "計算租金時發生錯誤");
            return Json(new { success = false, error = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RentalViewModel model)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
        }

        model.CustomerId = GetCurrentCustomerId();

        if (!ModelState.IsValid)
        {
            await LoadAvailableCars(model);
            return View(model);
        }

        try
        {
            var command = new CreateRentalCommand(
                model.CustomerId,
                model.CarId,
                model.StartDate,
                model.EndDate);

            var rentalId = await _createRentalHandler.Handle(command);

            TempData["SuccessMessage"] = "租車預約成功！";
            return RedirectToAction(nameof(Confirmation), new { id = rentalId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立租車時發生錯誤");
            ModelState.AddModelError("", ex.Message);
            await LoadAvailableCars(model);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Confirmation(Guid id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        // 這裡應該從資料庫載入租車詳細資訊
        // 目前簡化為建立一個範例確認頁面
        var model = new RentalConfirmationViewModel
        {
            RentalId = id,
            CustomerName = HttpContext.Session.GetString("CustomerName") ?? "",
            CarInfo = "範例車輛資訊",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2),
            RentalDays = 1,
            TotalFee = 1000,
            CreatedDate = DateTime.Now
        };

        return View(model);
    }

    private bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId"));
    }

    private Guid GetCurrentCustomerId()
    {
        var customerIdString = HttpContext.Session.GetString("CustomerId");
        return Guid.TryParse(customerIdString, out var customerId) ? customerId : Guid.Empty;
    }

    private async Task LoadAvailableCars(RentalViewModel model)
    {
        try
        {
            if (!string.IsNullOrEmpty(model.CarType))
            {
                var query = new GetAvailableCarsByTypeQuery(model.CarType, model.StartDate, model.EndDate);
                var cars = await _getAvailableCarsHandler.Handle(query);

                model.AvailableCars = cars.Select(car => new CarSelectionItem
                {
                    CarId = car.Id,
                    Brand = car.Brand,
                    Model = car.Model,
                    CarType = car.CarType.Type,
                    DailyRate = car.CarType.DailyRate.Amount
                }).ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "載入可用車輛時發生錯誤");
            model.AvailableCars = new List<CarSelectionItem>();
        }
    }
}