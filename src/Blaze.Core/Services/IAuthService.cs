using Blaze.Core.Models;

namespace Blaze.Core.Services;

/// <summary>
/// Service interface for user authentication
/// </summary>
public interface IAuthService
{
    // Authentication state
    bool IsAuthenticated { get; }
    User? CurrentUser { get; }

    // Events
    event EventHandler<User>? UserLoggedIn;
    event EventHandler? UserLoggedOut;
    event EventHandler<User>? UserProfileUpdated;

    // Authentication operations
    Task<AuthResult> LoginAsync(string username, string password);
    Task<AuthResult> RegisterAsync(string username, string email, string password);
    Task LogoutAsync();
    Task<bool> ValidateSessionAsync();

    // Profile operations
    Task<User?> GetCurrentUserAsync();
    Task<bool> UpdateProfileAsync(User user);
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
    Task<bool> UpdateAvatarAsync(string avatarPath);

    // Account management
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> DeleteAccountAsync(string password);

    // Preferences
    Task<UserPreferences> GetPreferencesAsync();
    Task SavePreferencesAsync(UserPreferences preferences);
}

/// <summary>
/// Result of an authentication operation
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public User? User { get; set; }
    public string? ErrorMessage { get; set; }
    public AuthErrorCode? ErrorCode { get; set; }

    public static AuthResult Succeeded(User user) => new()
    {
        Success = true,
        User = user
    };

    public static AuthResult Failed(string message, AuthErrorCode code) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorCode = code
    };
}

public enum AuthErrorCode
{
    InvalidCredentials,
    UserNotFound,
    UserAlreadyExists,
    EmailNotVerified,
    AccountLocked,
    NetworkError,
    ServerError,
    Unknown
}
