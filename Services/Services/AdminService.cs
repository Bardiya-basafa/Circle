namespace Services.Services;

using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Interfaces;
using Microsoft.EntityFrameworkCore;


public class AdminService : IAdminService {

    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Report>> GetPostReports(int postId)
    {
        var reports = await _context.Reports.Where(report => report.PostId == postId).ToListAsync();

        return reports;
    }

}
