using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces;

/// <summary>
/// AI Assistant service for customer support and recommendations
/// </summary>
public interface IAIAssistantService
{
    /// <summary>
    /// Send a message to the AI chatbot and get response
    /// </summary>
    Task<AIChatResponseDto> SendMessageAsync(AIChatRequestDto request, string userId);
    
    /// <summary>
    /// Get AI-powered route recommendations based on user preferences
    /// </summary>
    Task<List<RouteRecommendationDto>> GetRouteRecommendationsAsync(RoutePreferencesDto preferences, string userId);
    
    /// <summary>
    /// Get answers to frequently asked questions
    /// </summary>
    Task<string> GetFAQAnswerAsync(string question, string userId);
    
    /// <summary>
    /// Analyze user travel patterns and provide insights
    /// </summary>
    Task<TravelInsightsDto> GetTravelInsightsAsync(string userId);
}
