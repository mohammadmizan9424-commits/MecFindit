using MecFindit.DataLayer.Models;
using MecFindit.DataLayer.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace MecFindit.App.Controllers;

public class AdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("Role") == "Admin";
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        var items = await _unitOfWork.ItemReports.GetAllAsync();
        return View(items.OrderByDescending(i => i.CreatedAt).ToList());
    }

    public async Task<IActionResult> UpdateStatus(int id, int statusId)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        var item = await _unitOfWork.ItemReports.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        item.ItemStatusId = statusId;
        _unitOfWork.ItemReports.Update(item);

        string statusName = GetStatusName(statusId);
        if (item.ItemType == "Found" && statusId == 5)
        {
            _unitOfWork.ItemReports.Delete(item);
            await _unitOfWork.SaveAsync();

            TempData["Message"] = "Found item has been marked as returned and deleted.";
            TempData["MessageType"] = "Returned";

            return RedirectToAction("Dashboard");
        }

        await _unitOfWork.Notifications.AddAsync(new Notification
        {
            CampusUserId = item.CampusUserId,
            ItemReportId = item.Id,
            Message = $"Your item report '{item.ItemName}' has been {statusName}."
        });

        await _unitOfWork.SaveAsync();

        TempData["Message"] = $"Item report has been {statusName}.";
        TempData["MessageType"] = statusName;

        return RedirectToAction("Dashboard");
    }

    public async Task<IActionResult> ManageUsers()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        var users = await _unitOfWork.CampusUsers.GetAllAsync();
        return View(users.OrderByDescending(u => u.CreatedAt).ToList());
    }

    public async Task<IActionResult> ToggleBanUser(int id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _unitOfWork.CampusUsers.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        user.IsBanned = !user.IsBanned;

        _unitOfWork.CampusUsers.Update(user);
        await _unitOfWork.SaveAsync();

        TempData["Message"] = user.IsBanned
            ? $"User {user.FullName} has been banned."
            : $"User {user.FullName} has been unbanned.";

        TempData["MessageType"] = user.IsBanned ? "Rejected" : "Approved";

        return RedirectToAction("ManageUsers");
    }

    private string GetStatusName(int statusId)
    {
        if (statusId == 1) return "Pending";
        if (statusId == 2) return "Approved";
        if (statusId == 3) return "Rejected";
        if (statusId == 4) return "Claimed";
        if (statusId == 5) return "Returned";
        if (statusId == 6) return "Closed";

        return "Updated";
    }
}
