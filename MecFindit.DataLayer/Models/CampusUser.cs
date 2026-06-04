using System.ComponentModel.DataAnnotations;

namespace MecFindit.DataLayer.Models;

public class CampusUser
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required, MaxLength(30)]
    public string Role { get; set; } = "Student";

    public bool IsBanned { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<ItemReport> ItemReports { get; set; } = new();
    public List<ClaimRequest> ClaimRequests { get; set; } = new();
    public List<Notification> Notifications { get; set; } = new();
}
