using MecFindit.DataLayer.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace MecFindit.App.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _unitOfWork.ItemReports.GetAllAsync();

        if (HttpContext.Session.GetString("Role") == "Admin")
        {
            return View(items.OrderByDescending(i => i.CreatedAt).ToList());
        }

        var visibleItems = items
            .Where(i => i.ItemStatusId == 2 || i.ItemStatusId == 4 || i.ItemStatusId == 5)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();

        return View(visibleItems);
    }
}
