using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;

namespace StudioScheduler.Infrastructure.Repositories;

public class PassRepository : IPassRepository
{
    private readonly ApplicationDbContext _context;

    public PassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Pass?> GetByIdAsync(Guid id)
    {
        return await _context.Passes
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pass>> GetAllAsync()
    {
        return await _context.Passes
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pass>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Passes
            .Where(p => p.UserId == userId)
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pass>> GetActivePassesAsync()
    {
        var today = DateTime.Today;
        return await _context.Passes
            .Where(p => p.IsActive && p.StartDate <= today && p.EndDate >= today)
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pass>> GetExpiredPassesAsync()
    {
        var today = DateTime.Today;
        return await _context.Passes
            .Where(p => p.EndDate < today)
            .Include(p => p.User)
            .OrderByDescending(p => p.EndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pass>> GetPassesByTypeAsync(string passType)
    {
        return await _context.Passes
            .Where(p => p.Type.ToString() == passType)
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Pass?> GetCurrentActivePassForUserAsync(Guid userId)
    {
        var today = DateTime.Today;
        return await _context.Passes
            .Where(p => p.UserId == userId && 
                       p.IsActive && 
                       p.StartDate <= today && 
                       p.EndDate >= today)
            .Include(p => p.User)
            .OrderByDescending(p => p.StartDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Pass>> GetPassesExpiringInDaysAsync(int days)
    {
        var cutoffDate = DateTime.Today.AddDays(days);
        var today = DateTime.Today;
        
        return await _context.Passes
            .Where(p => p.IsActive && 
                       p.StartDate <= today && 
                       p.EndDate >= today &&
                       p.EndDate <= cutoffDate)
            .Include(p => p.User)
            .OrderBy(p => p.EndDate)
            .ToListAsync();
    }

    public async Task<Pass> AddAsync(Pass pass)
    {
        _context.Passes.Add(pass);
        await _context.SaveChangesAsync();
        return pass;
    }

    public async Task<Pass> UpdateAsync(Pass pass)
    {
        pass.UpdatedAt = DateTime.UtcNow;
        _context.Entry(pass).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return pass;
    }

    public async Task DeleteAsync(Guid id)
    {
        var pass = await _context.Passes.FindAsync(id);
        if (pass != null)
        {
            _context.Passes.Remove(pass);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Passes.AnyAsync(p => p.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Passes.CountAsync();
    }

    public async Task<int> CountActiveAsync()
    {
        var today = DateTime.Today;
        return await _context.Passes
            .CountAsync(p => p.IsActive && p.StartDate <= today && p.EndDate >= today);
    }

    public async Task<int> CountByTypeAsync(string passType)
    {
        return await _context.Passes
            .CountAsync(p => p.Type.ToString() == passType);
    }
}
