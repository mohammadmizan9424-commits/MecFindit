using MecFindit.DataLayer.Data;
using MecFindit.DataLayer.Models;
using MecFindit.DataLayer.Repositories;

namespace MecFindit.DataLayer.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IGenericRepository<CampusUser> CampusUsers { get; }
    public IGenericRepository<ItemReport> ItemReports { get; }
    public IGenericRepository<ItemStatus> ItemStatuses { get; }
    public IGenericRepository<ClaimRequest> ClaimRequests { get; }
    public IGenericRepository<Notification> Notifications { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        CampusUsers = new GenericRepository<CampusUser>(_context);
        ItemReports = new GenericRepository<ItemReport>(_context);
        ItemStatuses = new GenericRepository<ItemStatus>(_context);
        ClaimRequests = new GenericRepository<ClaimRequest>(_context);
        Notifications = new GenericRepository<Notification>(_context);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
