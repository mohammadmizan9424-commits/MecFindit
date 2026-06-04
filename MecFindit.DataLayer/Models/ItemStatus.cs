using System.ComponentModel.DataAnnotations;

namespace MecFindit.DataLayer.Models;

public class ItemStatus
{
    public int Id { get; set; }

    [Required, MaxLength(30)]
    public string StatusName { get; set; } = string.Empty;

    public List<ItemReport> ItemReports { get; set; } = new();
}
