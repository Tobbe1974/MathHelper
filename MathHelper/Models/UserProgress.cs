using System.ComponentModel.DataAnnotations;
using MathHelper.Data;

namespace MathHelper.Models;

public class UserProgress
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public MathCategory Category { get; set; }

    public int CurrentLevel { get; set; } = 1;

    public int TotalCorrect { get; set; }

    public int TotalAttempts { get; set; }

    public int CorrectStreak { get; set; }

    public int WrongStreak { get; set; }
}
