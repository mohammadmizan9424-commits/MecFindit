using MecFindit.DataLayer.Models;
using MecFindit.DataLayer.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace MecFindit.App.Controllers;

public class ItemsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public ItemsController(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _unitOfWork.ItemReports.GetAllAsync();
        return View(items.OrderByDescending(i => i.CreatedAt).ToList());
    }

    public IActionResult ReportLost()
    {
        return View("ItemForm", new ItemReport { ItemType = "Lost", ItemDate = DateTime.Today, ItemStatusId = 1 });
    }

    public IActionResult ReportFound()
    {
        return View("ItemForm", new ItemReport { ItemType = "Found", ItemDate = DateTime.Today, ItemStatusId = 1 });
    }

    [HttpPost]
    public async Task<IActionResult> Save(ItemReport item, IFormFile? photo)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        item.CampusUserId = userId.Value;
        item.ItemStatusId = 1;
        item.CreatedAt = DateTime.Now;

        if (photo != null && photo.Length > 0)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var fullPath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await photo.CopyToAsync(stream);

            item.PhotoPath = "/uploads/" + fileName;
        }

        await _unitOfWork.ItemReports.AddAsync(item);
        await _unitOfWork.SaveAsync();

        TempData["Message"] = "Item report submitted successfully.";
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var item = await _unitOfWork.ItemReports.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return View("ItemForm", item);
    }

    [HttpPost]
    public async Task<IActionResult> Update(ItemReport item, IFormFile? photo)
    {
        var oldItem = await _unitOfWork.ItemReports.GetByIdAsync(item.Id);

        if (oldItem == null)
        {
            return NotFound();
        }

        oldItem.ItemName = item.ItemName;
        oldItem.Category = item.Category;
        oldItem.Description = item.Description;
        oldItem.Location = item.Location;
        oldItem.ItemDate = item.ItemDate;
        oldItem.ContactNumber = item.ContactNumber;

        if (photo != null && photo.Length > 0)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var fullPath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await photo.CopyToAsync(stream);

            oldItem.PhotoPath = "/uploads/" + fileName;
        }

        _unitOfWork.ItemReports.Update(oldItem);
        await _unitOfWork.SaveAsync();

        TempData["Message"] = "Item report updated successfully.";

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var item = await _unitOfWork.ItemReports.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        if (item.CampusUserId != userId.Value && role != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        _unitOfWork.ItemReports.Delete(item);
        await _unitOfWork.SaveAsync();

        TempData["Message"] = "Item report deleted successfully.";

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Search(string? keyword)
    {
        var items = await _unitOfWork.ItemReports.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            items = items.Where(i =>
                i.ItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                i.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                i.Location.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        ViewBag.Keyword = keyword;
        return View(items.OrderByDescending(i => i.CreatedAt).ToList());
    }
}
