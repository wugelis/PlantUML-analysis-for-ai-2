using System.ComponentModel.DataAnnotations;

namespace RentalCarSystem.WebUI.Models.ViewModels;

/// <summary>
/// 使用者註冊的 ViewModel
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "使用者ID不能為空")]
    [Display(Name = "使用者ID")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "姓名不能為空")]
    [Display(Name = "姓名")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "電子郵件不能為空")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    [Display(Name = "電子郵件")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "密碼不能為空")]
    [StringLength(100, ErrorMessage = "密碼長度必須至少為 {2} 個字符。", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "密碼")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "確認密碼")]
    [Compare("Password", ErrorMessage = "密碼和確認密碼不匹配。")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// 使用者登入的 ViewModel
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "使用者ID不能為空")]
    [Display(Name = "使用者ID")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "密碼不能為空")]
    [DataType(DataType.Password)]
    [Display(Name = "密碼")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "記住我")]
    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;
}

/// <summary>
/// 車輛租用的 ViewModel
/// </summary>
public class RentalViewModel
{
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "請選擇車輛")]
    [Display(Name = "選擇車輛")]
    public Guid CarId { get; set; }

    [Required(ErrorMessage = "請選擇開始日期")]
    [DataType(DataType.Date)]
    [Display(Name = "租用開始日期")]
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "請選擇結束日期")]
    [DataType(DataType.Date)]
    [Display(Name = "租用結束日期")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(2);

    [Display(Name = "車輛類型")]
    public string CarType { get; set; } = string.Empty;

    public List<CarSelectionItem> AvailableCars { get; set; } = new();

    public decimal? EstimatedFee { get; set; }
}

/// <summary>
/// 車輛選擇項目
/// </summary>
public class CarSelectionItem
{
    public Guid CarId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string CarType { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public string DisplayText => $"{Brand} {Model} ({CarType}) - NT${DailyRate:N0}/天";
}

/// <summary>
/// 租用確認的 ViewModel
/// </summary>
public class RentalConfirmationViewModel
{
    public Guid RentalId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CarInfo { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int RentalDays { get; set; }
    public decimal TotalFee { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// 首頁的 ViewModel
/// </summary>
public class HomeViewModel
{
    public bool IsLoggedIn { get; set; }
    public string? CustomerName { get; set; }
    public List<string> CarTypes { get; set; } = new() { "Car", "SUV", "Truck", "SportsCar", "ElectricCar" };
    public Dictionary<string, decimal> CarTypePrices { get; set; } = new()
    {
        { "Car", 1000 },
        { "SUV", 1500 },
        { "Truck", 2000 },
        { "SportsCar", 3000 },
        { "ElectricCar", 2800 }
    };
}