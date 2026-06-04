using System.ComponentModel.DataAnnotations;

namespace MecFindit.DataLayer.Models;

public class Notification
{
    public int Id { get; set; }

    [Required, MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int CampusUserId { get; set; }
    public CampusUser? CampusUser { get; set; }

    public int? ItemReportId { get; set; }
    public ItemReport? ItemReport { get; set; }

    public int? ClaimRequestId { get; set; }
    public ClaimRequest? ClaimRequest { get; set; }
}
