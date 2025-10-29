using Microsoft.AspNetCore.Mvc;
using RentalCarSystem.Application.UseCases.Customers;
using RentalCarSystem.WebUI.Models.ViewModels;

namespace RentalCarSystem.WebUI.Controllers;

public class AccountController : Controller
{
    private readonly RegisterCustomerCommandHandler _registerHandler;
    private readonly LoginCustomerCommandHandler _loginHandler;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        RegisterCustomerCommandHandler registerHandler,
        LoginCustomerCommandHandler loginHandler,
        ILogger<AccountController> logger)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new RegisterCustomerCommand(
                model.UserId,
                model.Name,
                model.Email);

            var customerId = await _registerHandler.Handle(command);

            TempData["SuccessMessage"] = "註冊成功！請登入您的帳戶。";
            return RedirectToAction(nameof(Login));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("UserId", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊時發生錯誤");
            ModelState.AddModelError("", "註冊時發生錯誤，請稍後再試。");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl ?? string.Empty
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new LoginCustomerCommand(model.UserId, model.Password);
            var customerId = await _loginHandler.Handle(command);

            if (customerId.HasValue)
            {
                // 設定 Session
                HttpContext.Session.SetString("CustomerId", customerId.Value.ToString());
                HttpContext.Session.SetString("CustomerName", model.UserId);

                TempData["SuccessMessage"] = "登入成功！";

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "使用者ID或密碼錯誤。");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入時發生錯誤");
            ModelState.AddModelError("", "登入時發生錯誤，請稍後再試。");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["SuccessMessage"] = "已成功登出。";
        return RedirectToAction("Index", "Home");
    }

    private bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId"));
    }
}