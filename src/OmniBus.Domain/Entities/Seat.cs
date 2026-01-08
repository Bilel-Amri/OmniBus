using OmniBus.Domain.Common;

namespace OmniBus.Domain.Entities;

/// <summary>
/// Represents a seat in a bus
/// </summary>
public class Seat : BaseEntity
{
    /// <summary>
    /// The bus this seat belongs to
    /// </summary>
    public Guid BusId { get; set; }
    public virtual Bus Bus { get; set; } = null!;
    
    /// <summary>
    /// Seat number
    /// </summary>
    public int SeatNumber { get; set; }
    
    /// <summary>
    /// Row number in the bus
    /// </summary>
    public int Row { get; set; }
    
    /// <summary>
    /// Column/position in the row (A, B, C, D, etc.)
    /// </summary>
    public string Column { get; set; } = string.Empty;
    
    /// <summary>
    /// Seat type (Regular, Window, Aisle, etc.)
    /// </summary>
    public string SeatType { get; set; } = "Regular";
    
    /// <summary>
    /// Whether this is a window seat
    /// </summary>
    public bool IsWindowSeat { get; set; } = false;
    
    /// <summary>
    /// Whether this is an aisle seat
    /// </summary>
    public bool IsAisleSeat { get; set; } = false;
    
    /// <summary>
    /// Additional price for premium seats
    /// </summary>
    public decimal PriceModifier { get; set; } = 0;
}
