namespace OmniBus.Application.DTOs;

/// <summary>
/// AI Chat request from user
/// </summary>
public class AIChatRequestDto
{
    public string Message { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
    public Dictionary<string, object>? Context { get; set; }
}

/// <summary>
/// AI Chat response
/// </summary>
public class AIChatResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public List<string>? SuggestedQuestions { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// User preferences for route recommendations
/// </summary>
public class RoutePreferencesDto
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public DateTime? PreferredDepartureTime { get; set; }
    public string? BusTypePreference { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool PreferDirectRoutes { get; set; }
}

/// <summary>
/// AI-powered route recommendation
/// </summary>
public class RouteRecommendationDto
{
    public Guid RouteId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public string RecommendationReason { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
    public List<string> Highlights { get; set; } = new();
}

/// <summary>
/// Travel insights for user
/// </summary>
public class TravelInsightsDto
{
    public string Summary { get; set; } = string.Empty;
    public List<string> FrequentRoutes { get; set; } = new();
    public string PreferredTravelTime { get; set; } = string.Empty;
    public decimal AverageMonthlySpending { get; set; }
    public List<string> Recommendations { get; set; } = new();
}
