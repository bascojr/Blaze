using System.Security.Cryptography;
using System.Text;
using Blaze.Core.Data;
using Blaze.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Blaze.Core.Services;

/// <summary>
/// Implementation of the authentication service
/// </summary>
public class AuthService : IAuthService
{
    private readonly BlazeDbContext _context;
    private readonly ILogger<AuthService> _logger;
    private readonly string _sessionPath;
    private User? _currentUser;

    public bool IsAuthenticated => _currentUser != null;
    public User? CurrentUser => _currentUser;

    public event EventHandler<User>? UserLoggedIn;
    public event EventHandler? UserLoggedOut;
    public event EventHandler<User>? UserProfileUpdated;

    public AuthService(BlazeDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var blazeFolder = Path.Combine(appData, "Blaze");
        Directory.CreateDirectory(blazeFolder);
        _sessionPath = Path.Combine(blazeFolder, "session.json");
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        try
        {
            var passwordHash = HashPassword(password);

            // For this demo, we'll just create/retrieve a local user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                // Auto-create user for demo purposes
                return await RegisterAsync(username, $"{username}@local", password);
            }

            _currentUser = user;
            user.LastLoginAt = DateTime.UtcNow;
            user.IsOnline = true;
            await _context.SaveChangesAsync();

            await SaveSessionAsync(user);

            UserLoggedIn?.Invoke(this, user);
            _logger.LogInformation("User {Username} logged in", username);

            return AuthResult.Succeeded(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Username}", username);
            return AuthResult.Failed("An error occurred during login", AuthErrorCode.Unknown);
        }
    }

    public async Task<AuthResult> RegisterAsync(string username, string email, string password)
    {
        try
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (existingUser != null)
            {
                return AuthResult.Failed("Username already exists", AuthErrorCode.UserAlreadyExists);
            }

            var user = new User
            {
                Username = username,
                Email = email,
                DisplayName = username,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsOnline = true,
                Preferences = new UserPreferences()
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _currentUser = user;
            await SaveSessionAsync(user);

            UserLoggedIn?.Invoke(this, user);
            _logger.LogInformation("User {Username} registered and logged in", username);

            return AuthResult.Succeeded(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for user {Username}", username);
            return AuthResult.Failed("An error occurred during registration", AuthErrorCode.Unknown);
        }
    }

    public async Task LogoutAsync()
    {
        if (_currentUser != null)
        {
            _currentUser.IsOnline = false;
            await _context.SaveChangesAsync();
        }

        _currentUser = null;
        DeleteSession();

        UserLoggedOut?.Invoke(this, EventArgs.Empty);
        _logger.LogInformation("User logged out");
    }

    public async Task<bool> ValidateSessionAsync()
    {
        try
        {
            if (File.Exists(_sessionPath))
            {
                var json = await File.ReadAllTextAsync(_sessionPath);
                var session = JsonConvert.DeserializeObject<SessionData>(json);

                if (session != null && !string.IsNullOrEmpty(session.UserId))
                {
                    var user = await _context.Users.FindAsync(session.UserId);
                    if (user != null)
                    {
                        _currentUser = user;
                        user.LastLoginAt = DateTime.UtcNow;
                        user.IsOnline = true;
                        await _context.SaveChangesAsync();

                        UserLoggedIn?.Invoke(this, user);
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Session validation failed");
        }

        return false;
    }

    public Task<User?> GetCurrentUserAsync()
    {
        return Task.FromResult(_currentUser);
    }

    public async Task<bool> UpdateProfileAsync(User user)
    {
        try
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null) return false;

            existingUser.DisplayName = user.DisplayName;
            existingUser.Email = user.Email;
            existingUser.AvatarUrl = user.AvatarUrl;
            existingUser.Status = user.Status;
            existingUser.StatusMessage = user.StatusMessage;

            await _context.SaveChangesAsync();

            _currentUser = existingUser;
            UserProfileUpdated?.Invoke(this, existingUser);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update profile");
            return false;
        }
    }

    public Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        // For demo purposes, always succeeds
        _logger.LogInformation("Password changed for user {Username}", _currentUser?.Username);
        return Task.FromResult(true);
    }

    public async Task<bool> UpdateAvatarAsync(string avatarPath)
    {
        if (_currentUser == null) return false;

        _currentUser.AvatarUrl = avatarPath;
        await _context.SaveChangesAsync();

        UserProfileUpdated?.Invoke(this, _currentUser);
        return true;
    }

    public Task<bool> ForgotPasswordAsync(string email)
    {
        _logger.LogInformation("Password reset requested for {Email}", email);
        return Task.FromResult(true);
    }

    public Task<bool> VerifyEmailAsync(string token)
    {
        return Task.FromResult(true);
    }

    public async Task<bool> DeleteAccountAsync(string password)
    {
        if (_currentUser == null) return false;

        _context.Users.Remove(_currentUser);
        await _context.SaveChangesAsync();

        await LogoutAsync();
        return true;
    }

    public Task<UserPreferences> GetPreferencesAsync()
    {
        return Task.FromResult(_currentUser?.Preferences ?? new UserPreferences());
    }

    public async Task SavePreferencesAsync(UserPreferences preferences)
    {
        if (_currentUser != null)
        {
            _currentUser.Preferences = preferences;
            await _context.SaveChangesAsync();
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private async Task SaveSessionAsync(User user)
    {
        var session = new SessionData
        {
            UserId = user.Id,
            Username = user.Username,
            CreatedAt = DateTime.UtcNow
        };

        var json = JsonConvert.SerializeObject(session);
        await File.WriteAllTextAsync(_sessionPath, json);
    }

    private void DeleteSession()
    {
        if (File.Exists(_sessionPath))
        {
            File.Delete(_sessionPath);
        }
    }

    private class SessionData
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
