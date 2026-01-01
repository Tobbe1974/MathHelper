using System.ComponentModel.DataAnnotations;
using MathHelper.Data;

namespace MathHelper.Models;

public class TrialResult
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public MathCategory Category { get; set; }

    public int QuestionCount { get; set; }

    public int CorrectAnswers { get; set; }

    public double TimeSeconds { get; set; }

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public int DifficultyLevel { get; set; }

    public double Accuracy => QuestionCount > 0 ? (double)CorrectAnswers / QuestionCount * 100 : 0;
}
