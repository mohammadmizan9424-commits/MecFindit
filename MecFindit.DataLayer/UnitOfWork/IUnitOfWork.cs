using MecFindit.DataLayer.Models;
using MecFindit.DataLayer.Repositories;

namespace MecFindit.DataLayer.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<CampusUser> CampusUsers { get; }
    IGenericRepository<ItemReport> ItemReports { get; }
    IGenericRepository<ItemStatus> ItemStatuses { get; }
    IGenericRepository<ClaimRequest> ClaimRequests { get; }
    IGenericRepository<Notification> Notifications { get; }

    Task<int> SaveAsync();
}
