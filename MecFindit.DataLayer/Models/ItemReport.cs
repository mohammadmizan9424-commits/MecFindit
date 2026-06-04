using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MecFindit.DataLayer.Models;

public class ItemReport
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string ItemType { get; set; } = "Lost"; // Lost or Found

    [Required, MaxLength(120)]
    public string ItemName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? Description { get; set; }

    [Required, MaxLength(150)]
    public string Location { get; set; } = string.Empty;

    public DateTime ItemDate { get; set; } = DateTime.Today;

    public string? PhotoPath { get; set; }

    [MaxLength(20)]
    public string? ContactNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int CampusUserId { get; set; }
    public CampusUser? CampusUser { get; set; }

    public int ItemStatusId { get; set; } = 1;
    public ItemStatus? ItemStatus { get; set; }

    public List<ClaimRequest> ClaimRequests { get; set; } = new();
    public List<Notification> Notifications { get; set; } = new();
}
