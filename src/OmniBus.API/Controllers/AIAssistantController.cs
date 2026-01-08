using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using System.Security.Claims;

namespace OmniBus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIAssistantController : ControllerBase
{
    private readonly IAIAssistantService _aiService;
    private readonly ILogger<AIAssistantController> _logger;

    public AIAssistantController(
        IAIAssistantService aiService,
        ILogger<AIAssistantController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Send a message to the AI chatbot
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<AIChatResponseDto>> Chat([FromBody] AIChatRequestDto request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var response = await _aiService.SendMessageAsync(request, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message");
            return StatusCode(500, new { message = "An error occurred while processing your message" });
        }
    }

    /// <summary>
    /// Get AI-powered route recommendations
    /// </summary>
    [HttpPost("recommendations")]
    public async Task<ActionResult<List<RouteRecommendationDto>>> GetRecommendations(
        [FromBody] RoutePreferencesDto preferences)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var recommendations = await _aiService.GetRouteRecommendationsAsync(preferences, userId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting route recommendations");
            return StatusCode(500, new { message = "An error occurred while getting recommendations" });
        }
    }

    /// <summary>
    /// Get answer to a frequently asked question
    /// </summary>
    [HttpPost("faq")]
    public async Task<ActionResult<string>> GetFAQAnswer([FromBody] string question)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var answer = await _aiService.GetFAQAnswerAsync(question, userId);
            return Ok(new { answer });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting FAQ answer");
            return StatusCode(500, new { message = "An error occurred while retrieving the answer" });
        }
    }

    /// <summary>
    /// Get personalized travel insights
    /// </summary>
    [HttpGet("insights")]
    public async Task<ActionResult<TravelInsightsDto>> GetTravelInsights()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User must be authenticated to view insights" });
            }

            var insights = await _aiService.GetTravelInsightsAsync(userId);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting travel insights");
            return StatusCode(500, new { message = "An error occurred while generating insights" });
        }
    }
}
