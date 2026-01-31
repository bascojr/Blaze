using System.ComponentModel.DataAnnotations;

namespace Blaze.Core.Models;

/// <summary>
/// Represents a user review for an application
/// </summary>
public class Review
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ApplicationId { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string UserAvatarUrl { get; set; } = string.Empty;

    public int Rating { get; set; } // 1-5 stars

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int HelpfulCount { get; set; } = 0;

    public int NotHelpfulCount { get; set; } = 0;

    public bool IsVerifiedPurchase { get; set; } = false;

    public TimeSpan UserPlayTime { get; set; } = TimeSpan.Zero;

    public string PlayTimeFormatted
    {
        get
        {
            if (UserPlayTime.TotalHours >= 1)
                return $"{(int)UserPlayTime.TotalHours} hours on record";
            if (UserPlayTime.TotalMinutes >= 1)
                return $"{(int)UserPlayTime.TotalMinutes} minutes on record";
            return "< 1 hour on record";
        }
    }

    public bool Recommended { get; set; } = true;

    public List<string> ImageUrls { get; set; } = new();

    public DeveloperResponse? DeveloperResponse { get; set; }
}

/// <summary>
/// Developer's response to a review
/// </summary>
public class DeveloperResponse
{
    public string Content { get; set; } = string.Empty;

    public DateTime RespondedAt { get; set; } = DateTime.UtcNow;
}
