using Microsoft.EntityFrameworkCore;
using MathHelper.Data;
using MathHelper.Models;

namespace MathHelper.Services;

public class AdminService(ApplicationDbContext db)
{
    // Hardcoded admin credentials
    private const string AdminUsername = "admin";
    private const string AdminPassword = "math123";

    public bool ValidateAdmin(string username, string password)
    {
        return username == AdminUsername && password == AdminPassword;
    }

    public async Task<List<UserStats>> GetAllUserStatsAsync()
    {
        var users = await db.Users.ToListAsync();
        var result = new List<UserStats>();

        foreach (var user in users)
        {
            var progress = await db.UserProgress
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            var trials = await db.TrialResults
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.CompletedAt)
                .ToListAsync();

            var totalAttempts = progress.Sum(p => p.TotalAttempts);
            var totalCorrect = progress.Sum(p => p.TotalCorrect);

            result.Add(new UserStats
            {
                UserId = user.Id,
                UserName = user.UserName ?? user.Email ?? "Unknown",
                Email = user.Email ?? "",
                TotalAttempts = totalAttempts,
                TotalCorrect = totalCorrect,
                Accuracy = totalAttempts > 0 ? (double)totalCorrect / totalAttempts * 100 : 0,
                TrialCount = trials.Count,
                LastActivity = trials.FirstOrDefault()?.CompletedAt,
                ProgressByCategory = progress.ToDictionary(p => p.Category, p => p)
            });
        }

        return result.OrderByDescending(u => u.LastActivity ?? DateTime.MinValue).ToList();
    }

    public async Task<UserStats?> GetUserStatsAsync(string userId)
    {
        var user = await db.Users.FindAsync(userId);
        if (user == null) return null;

        var progress = await db.UserProgress
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var trials = await db.TrialResults
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CompletedAt)
            .ToListAsync();

        var totalAttempts = progress.Sum(p => p.TotalAttempts);
        var totalCorrect = progress.Sum(p => p.TotalCorrect);

        return new UserStats
        {
            UserId = user.Id,
            UserName = user.UserName ?? user.Email ?? "Unknown",
            Email = user.Email ?? "",
            TotalAttempts = totalAttempts,
            TotalCorrect = totalCorrect,
            Accuracy = totalAttempts > 0 ? (double)totalCorrect / totalAttempts * 100 : 0,
            TrialCount = trials.Count,
            LastActivity = trials.FirstOrDefault()?.CompletedAt,
            ProgressByCategory = progress.ToDictionary(p => p.Category, p => p),
            RecentTrials = trials.Take(20).ToList()
        };
    }
}

public class UserStats
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalAttempts { get; set; }
    public int TotalCorrect { get; set; }
    public double Accuracy { get; set; }
    public int TrialCount { get; set; }
    public DateTime? LastActivity { get; set; }
    public Dictionary<MathCategory, UserProgress> ProgressByCategory { get; set; } = new();
    public List<TrialResult> RecentTrials { get; set; } = new();
}
