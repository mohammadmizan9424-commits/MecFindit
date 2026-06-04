using System.ComponentModel.DataAnnotations;

namespace MecFindit.DataLayer.Models;

public class ClaimRequest
{
    public int Id { get; set; }

    public int CampusUserId { get; set; }
    public CampusUser? CampusUser { get; set; }

    public int ItemReportId { get; set; }
    public ItemReport? ItemReport { get; set; }

    [Required, MaxLength(600)]
    public string ProofDescription { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string ClaimStatus { get; set; } = "Pending";

    [MaxLength(300)]
    public string? AdminMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
