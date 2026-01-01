using Microsoft.EntityFrameworkCore;
using MathHelper.Data;
using MathHelper.Models;

namespace MathHelper.Services;

public class ProgressService(ApplicationDbContext db)
{
    private const int CorrectStreakToLevelUp = 10;
    private const double AccuracyThresholdToLevelUp = 0.8;
    private const int WrongStreakToLevelDown = 5;
    private const int MaxLevel = 10;
    private const int MinLevel = 1;

    public async Task<UserProgress> GetOrCreateProgressAsync(string userId, MathCategory category)
    {
        var progress = await db.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Category == category);

        if (progress == null)
        {
            progress = new UserProgress
            {
                UserId = userId,
                Category = category,
                CurrentLevel = 1
            };
            db.UserProgress.Add(progress);
            await db.SaveChangesAsync();
        }

        return progress;
    }

    public async Task<Dictionary<MathCategory, UserProgress>> GetAllProgressAsync(string userId)
    {
        var allProgress = await db.UserProgress
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var result = new Dictionary<MathCategory, UserProgress>();
        foreach (MathCategory category in Enum.GetValues<MathCategory>())
        {
            var progress = allProgress.FirstOrDefault(p => p.Category == category);
            if (progress == null)
            {
                progress = new UserProgress
                {
                    UserId = userId,
                    Category = category,
                    CurrentLevel = 1
                };
                db.UserProgress.Add(progress);
            }
            result[category] = progress;
        }
        await db.SaveChangesAsync();
        return result;
    }

    public async Task<UserProgress> RecordAnswerAsync(string userId, MathCategory category, bool correct)
    {
        var progress = await GetOrCreateProgressAsync(userId, category);

        progress.TotalAttempts++;
        if (correct)
        {
            progress.TotalCorrect++;
            progress.CorrectStreak++;
            progress.WrongStreak = 0;

            // Check for level up
            if (progress.CorrectStreak >= CorrectStreakToLevelUp && progress.CurrentLevel < MaxLevel)
            {
                var recentAccuracy = (double)progress.CorrectStreak / CorrectStreakToLevelUp;
                if (recentAccuracy >= AccuracyThresholdToLevelUp)
                {
                    progress.CurrentLevel++;
                    progress.CorrectStreak = 0;
                }
            }
        }
        else
        {
            progress.WrongStreak++;
            progress.CorrectStreak = 0;

            // Check for level down
            if (progress.WrongStreak >= WrongStreakToLevelDown && progress.CurrentLevel > MinLevel)
            {
                progress.CurrentLevel--;
                progress.WrongStreak = 0;
            }
        }

        await db.SaveChangesAsync();
        return progress;
    }

    public async Task<TrialResult> SaveTrialResultAsync(string userId, MathCategory category,
        int questionCount, int correctAnswers, double timeSeconds, int difficultyLevel)
    {
        var result = new TrialResult
        {
            UserId = userId,
            Category = category,
            QuestionCount = questionCount,
            CorrectAnswers = correctAnswers,
            TimeSeconds = timeSeconds,
            DifficultyLevel = difficultyLevel,
            CompletedAt = DateTime.UtcNow
        };

        db.TrialResults.Add(result);
        await db.SaveChangesAsync();

        return result;
    }

    public async Task<List<TrialResult>> GetTrialResultsAsync(string userId, MathCategory? category = null)
    {
        var query = db.TrialResults.Where(t => t.UserId == userId);

        if (category.HasValue)
            query = query.Where(t => t.Category == category.Value);

        return await query.OrderByDescending(t => t.CompletedAt).ToListAsync();
    }

    public async Task<TrialResult?> GetBestTrialAsync(string userId, MathCategory category, int questionCount)
    {
        return await db.TrialResults
            .Where(t => t.UserId == userId && t.Category == category && t.QuestionCount == questionCount)
            .OrderBy(t => t.TimeSeconds)
            .ThenByDescending(t => t.CorrectAnswers)
            .FirstOrDefaultAsync();
    }
}
