using MecFindit.DataLayer.Models;
using MecFindit.DataLayer.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace MecFindit.App.Controllers;

public class AccountController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var users = await _unitOfWork.CampusUsers.FindAsync(u => u.Email == email);
        var user = users.FirstOrDefault();

        if (user == null || user.Password != password)
        {
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        if (user.IsBanned)
        {
            ViewBag.Error = "Your account has been banned. Please contact the administrator.";
            return View();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("FullName", user.FullName);
        HttpContext.Session.SetString("Role", user.Role);

        if (user.Role == "Admin")
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(CampusUser user, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Password is required.";
            return View(user);
        }

        var existing = await _unitOfWork.CampusUsers.FindAsync(u => u.Email == user.Email);

        if (existing.Any())
        {
            ViewBag.Error = "Email already exists.";
            return View(user);
        }

        user.Password = password;
        user.IsBanned = false;
        user.CreatedAt = DateTime.Now;

        await _unitOfWork.CampusUsers.AddAsync(user);
        await _unitOfWork.SaveAsync();

        TempData["Message"] = "Account created successfully. Please login.";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
