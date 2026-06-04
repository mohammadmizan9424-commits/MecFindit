using MecFindit.DataLayer.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace MecFindit.App.Controllers;

public class NotificationsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var notifications = await _unitOfWork.Notifications.FindAsync(n => n.CampusUserId == userId.Value);
        return View(notifications.OrderByDescending(n => n.CreatedAt).ToList());
    }

    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id);

        if (notification != null)
        {
            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveAsync();
        }

        return RedirectToAction("Index");
    }
}
