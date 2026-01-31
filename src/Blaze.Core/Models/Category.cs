using System.ComponentModel.DataAnnotations;

namespace Blaze.Core.Models;

/// <summary>
/// Represents a category for organizing applications
/// </summary>
public class Category
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string IconName { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;

    public string ParentCategoryId { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int AppCount { get; set; } = 0;
}

/// <summary>
/// Standard category definitions
/// </summary>
public static class StandardCategories
{
    public static readonly Category Productivity = new()
    {
        Id = "productivity",
        Name = "Productivity",
        Description = "Office suites, note-taking, project management",
        IconName = "Briefcase",
        DisplayOrder = 1
    };

    public static readonly Category Development = new()
    {
        Id = "development",
        Name = "Development",
        Description = "IDEs, code editors, programming tools",
        IconName = "Code",
        DisplayOrder = 2
    };

    public static readonly Category Graphics = new()
    {
        Id = "graphics",
        Name = "Graphics & Design",
        Description = "Image editors, 3D modeling, design tools",
        IconName = "Palette",
        DisplayOrder = 3
    };

    public static readonly Category Multimedia = new()
    {
        Id = "multimedia",
        Name = "Multimedia",
        Description = "Video players, audio editors, media tools",
        IconName = "Film",
        DisplayOrder = 4
    };

    public static readonly Category Utilities = new()
    {
        Id = "utilities",
        Name = "Utilities",
        Description = "System tools, file managers, utilities",
        IconName = "Wrench",
        DisplayOrder = 5
    };

    public static readonly Category Internet = new()
    {
        Id = "internet",
        Name = "Internet",
        Description = "Browsers, download managers, networking",
        IconName = "Globe",
        DisplayOrder = 6
    };

    public static readonly Category Security = new()
    {
        Id = "security",
        Name = "Security",
        Description = "Antivirus, firewalls, privacy tools",
        IconName = "Shield",
        DisplayOrder = 7
    };

    public static readonly Category Education = new()
    {
        Id = "education",
        Name = "Education",
        Description = "Learning tools, reference, educational software",
        IconName = "GraduationCap",
        DisplayOrder = 8
    };

    public static readonly Category Communication = new()
    {
        Id = "communication",
        Name = "Communication",
        Description = "Messaging, email clients, video conferencing",
        IconName = "MessageCircle",
        DisplayOrder = 9
    };

    public static readonly Category Business = new()
    {
        Id = "business",
        Name = "Business",
        Description = "Accounting, CRM, business management",
        IconName = "Building",
        DisplayOrder = 10
    };

    public static List<Category> All => new()
    {
        Productivity, Development, Graphics, Multimedia, Utilities,
        Internet, Security, Education, Communication, Business
    };
}
