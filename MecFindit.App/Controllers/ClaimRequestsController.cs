using MecFindit.DataLayer.Models;
using MecFindit.DataLayer.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace MecFindit.App.Controllers;

public class ClaimRequestsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ClaimRequestsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult CreateForItem(int itemReportId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var claim = new ClaimRequest
        {
            ItemReportId = itemReportId
        };

        return View("Create", claim);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ClaimRequest claimRequest)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        claimRequest.CampusUserId = userId.Value;
        claimRequest.ClaimStatus = "Pending";
        claimRequest.CreatedAt = DateTime.Now;

        await _unitOfWork.ClaimRequests.AddAsync(claimRequest);
        await _unitOfWork.SaveAsync();

        TempData["Message"] = "Your claim request has been sent to admin.";
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> AdminClaims()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Login", "Account");
        }

        var claims = await _unitOfWork.ClaimRequests.GetAllAsync();
        return View(claims.OrderByDescending(c => c.CreatedAt).ToList());
    }

    public async Task<IActionResult> MyClaims()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var claims = await _unitOfWork.ClaimRequests.FindAsync(c => c.CampusUserId == userId.Value);
        return View(claims.OrderByDescending(c => c.CreatedAt).ToList());
    }

    public async Task<IActionResult> Approve(int id)
    {
        return await UpdateClaimStatus(id, "Approved");
    }

    public async Task<IActionResult> Reject(int id)
    {
        return await UpdateClaimStatus(id, "Rejected");
    }

    private async Task<IActionResult> UpdateClaimStatus(int id, string status)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Login", "Account");
        }

        var claim = await _unitOfWork.ClaimRequests.GetByIdAsync(id);

        if (claim == null)
        {
            return NotFound();
        }

        claim.ClaimStatus = status;
        claim.AdminMessage = $"Your claim request has been {status}.";

        _unitOfWork.ClaimRequests.Update(claim);

        await _unitOfWork.Notifications.AddAsync(new Notification
        {
            CampusUserId = claim.CampusUserId,
            ItemReportId = claim.ItemReportId,
            ClaimRequestId = claim.Id,
            Message = $"Your claim request has been {status}."
        });

        if (status == "Approved")
        {
            var item = await _unitOfWork.ItemReports.GetByIdAsync(claim.ItemReportId);

            if (item != null)
            {
                item.ItemStatusId = 4; // Claimed
                _unitOfWork.ItemReports.Update(item);

                await _unitOfWork.Notifications.AddAsync(new Notification
                {
                    CampusUserId = item.CampusUserId,
                    ItemReportId = item.Id,
                    ClaimRequestId = claim.Id,
                    Message = $"The claim request for your item '{item.ItemName}' has been approved."
                });
            }
        }

        await _unitOfWork.SaveAsync();

        TempData["Message"] = $"Claim request has been {status}.";
        TempData["MessageType"] = status;

        return RedirectToAction("AdminClaims");
    }
}
