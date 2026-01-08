using OmniBus.Domain.Common;
using OmniBus.Domain.Enums;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a user in the system (passenger, driver, or admin)
/// </summary>
public class User : AuditableEntity
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address (unique)
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// User's role in the system
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Passenger;
    
    /// <summary>
    /// Whether the user's email is verified
    /// </summary>
    public bool EmailVerified { get; set; } = false;
    
    /// <summary>
    /// Profile picture URL
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
    
    /// <summary>
    /// National ID number (CIN for Tunisia)
    /// </summary>
    public string? NationalId { get; set; }
    
    // Navigation properties
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    /// <summary>
    /// For drivers: the bus assigned to this driver
    /// </summary>
    public Guid? AssignedBusId { get; set; }
    public virtual Bus? AssignedBus { get; set; }
}
