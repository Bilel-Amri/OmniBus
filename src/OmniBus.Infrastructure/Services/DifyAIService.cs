using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmniBus.Application.DTOs;
using OmniBus.Application.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace OmniBus.Infrastructure.Services;

/// <summary>
/// Dify AI integration service for intelligent features
/// </summary>
public class DifyAIService : IAIAssistantService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DifyAIService> _logger;
    private readonly string _difyApiKey;
    private readonly string _difyBaseUrl;

    public DifyAIService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<DifyAIService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        _difyApiKey = _configuration["Dify:ApiKey"] ?? throw new InvalidOperationException("Dify API key not configured");
        _difyBaseUrl = _configuration["Dify:BaseUrl"] ?? "https://api.dify.ai/v1";
        
        // Ensure trailing slash for proper relative URL resolution
        var baseUrl = _difyBaseUrl.EndsWith("/") ? _difyBaseUrl : _difyBaseUrl + "/";
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_difyApiKey}");
    }

    public async Task<AIChatResponseDto> SendMessageAsync(AIChatRequestDto request, string userId)
    {
        // Use local AI responses for OmniBus - no external API needed
        var response = GetLocalAIResponse(request.Message);
        
        return await Task.FromResult(new AIChatResponseDto
        {
            Message = response,
            ConversationId = request.ConversationId ?? Guid.NewGuid().ToString(),
            SuggestedQuestions = GetSuggestedQuestions(request.Message),
            Timestamp = DateTime.UtcNow
        });
    }
    
    private string GetLocalAIResponse(string message)
    {
        var lowerMessage = message.ToLower();
        
        // Greetings
        if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi") || lowerMessage.Contains("slm") || lowerMessage.Contains("سلام") || lowerMessage.Contains("aslema") || lowerMessage.Contains("marhba"))
            return "Marhba bik! 👋 I'm your OmniBus AI assistant. I can help you with:\n• Booking bus tickets\n• Finding routes and schedules\n• Tracking your bus in real-time\n• Payment information\n• Refunds and cancellations\n\nKifesh najem n3awnek lyoum? (How can I help you today?)";
        
        // Tunisian Darija - Bus/Car questions
        if (lowerMessage.Contains("el car") || lowerMessage.Contains("el kar") || lowerMessage.Contains("lkar") || lowerMessage.Contains("الكار") || lowerMessage.Contains("كار"))
            return "🚌 **El Kar (The Bus):**\n\nBesh t3ref waktesh yji el kar:\n1. Odkhol lel 'Search' fil page principale\n2. Ikteb mnin w winek (From/To)\n3. Khtar el youm\n4. Tchouf les horaires el kol!\n\nWala rouh lel 'My Tickets' besh tsuivi el kar mte3ek en temps réel GPS! 📍";
        
        // Booking tickets
        if (lowerMessage.Contains("book") || lowerMessage.Contains("ticket") || lowerMessage.Contains("reserve") || lowerMessage.Contains("billet") || lowerMessage.Contains("teskra") || lowerMessage.Contains("تذكرة"))
            return "📝 **To book a ticket:**\n\n1. Enter your departure city in 'From' field\n2. Enter your destination in 'To' field\n3. Select your travel date\n4. Click 'Search Schedules'\n5. Choose a bus and select your seat\n6. Pay securely with D17, Flouci, or card\n7. Receive your e-ticket instantly!\n\nYou can book from the home page or go to 'Book Now' in the menu. Need help with a specific route?";
        
        // Routes
        if (lowerMessage.Contains("route") || lowerMessage.Contains("available") || lowerMessage.Contains("where") || lowerMessage.Contains("destination") || lowerMessage.Contains("win") || lowerMessage.Contains("fama"))
            return "🗺️ **Popular Routes:**\n\n• Tunis ↔ Sousse (2h 30min)\n• Tunis ↔ Sfax (4h 30min)\n• Tunis ↔ Monastir (2h 45min)\n• Sousse ↔ Sfax (2h)\n• Tunis ↔ Nabeul (1h 30min)\n• Tunis ↔ Bizerte (1h 15min)\n• Tunis ↔ Kairouan (2h 30min)\n\nUse the search on home page to find schedules for any route!";
        
        // Track bus
        if (lowerMessage.Contains("track") || lowerMessage.Contains("where is") || lowerMessage.Contains("location") || lowerMessage.Contains("gps") || lowerMessage.Contains("suivi") || lowerMessage.Contains("winou"))
            return "📍 **Track Your Bus:**\n\n1. Go to 'My Tickets' in your profile\n2. Find your upcoming trip\n3. Click 'Track Bus' button\n4. See real-time location on map!\n\nBus tracking is available for trips that are currently in progress. The GPS updates every 30 seconds for accurate tracking.";
        
        // Payment
        if (lowerMessage.Contains("pay") || lowerMessage.Contains("price") || lowerMessage.Contains("cost") || lowerMessage.Contains("flouss") || lowerMessage.Contains("money") || lowerMessage.Contains("9addesh") || lowerMessage.Contains("combien") || lowerMessage.Contains("فلوس"))
            return "💳 **Payment Options:**\n\n• **D17** - Mobile payment (instant)\n• **Flouci** - E-wallet payment\n• **Credit/Debit Card** - Visa, Mastercard\n• **Konnect** - Digital wallet\n\nAll payments are secure and encrypted. Your e-ticket is sent immediately after payment confirmation!";
        
        // Refund
        if (lowerMessage.Contains("refund") || lowerMessage.Contains("cancel") || lowerMessage.Contains("money back") || lowerMessage.Contains("annuler") || lowerMessage.Contains("rembours"))
            return "💰 **Refund Policy:**\n\n• **24+ hours before:** Full refund\n• **12-24 hours before:** 75% refund\n• **6-12 hours before:** 50% refund\n• **Less than 6 hours:** No refund\n\nTo cancel: Go to 'My Tickets' → Select ticket → Click 'Cancel Booking'. Refunds are processed within 3-5 business days.";
        
        // Schedule/Time - Including Tunisian "waktesh", "emta", "ki"
        if (lowerMessage.Contains("schedule") || lowerMessage.Contains("time") || lowerMessage.Contains("when") || lowerMessage.Contains("waktash") || lowerMessage.Contains("waktesh") || lowerMessage.Contains("emta") || lowerMessage.Contains("departure") || lowerMessage.Contains("horaire") || lowerMessage.Contains("tji") || lowerMessage.Contains("yji") || lowerMessage.Contains("توقيت"))
            return "🕐 **Bus Schedules / Horaires el Kar:**\n\nLes cars yemchiw kol youm mel 5:00 sbah lel 10:00 msa.\n\nBesh tchouf les horaires:\n1. Odkhol lel Search fil page principale\n2. Khtar mnin w winek (From/To)\n3. Khtar el youm\n4. Tchouf el horaires el kol!\n\n⏰ Les cars el sbah w el 3chiya homa el akther demandés - réservi bekri!";
        
        // Seat selection
        if (lowerMessage.Contains("seat") || lowerMessage.Contains("blassa") || lowerMessage.Contains("place") || lowerMessage.Contains("window") || lowerMessage.Contains("korsi") || lowerMessage.Contains("كرسي"))
            return "💺 **Seat Selection / Khtar Blasstek:**\n\n• Window seats: Besh tchouf el vue\n• Aisle seats: Sahel besh tokhroj\n• Front seats: 9lil ma3na\n• Back seats: Akther privacy\n\nEl blayess el khodhra = disponible ✅\nEl blayess el 7amra = mabrouka ❌\n\nKhtar blasstek wakt el booking!";
        
        // Help/Support
        if (lowerMessage.Contains("help") || lowerMessage.Contains("support") || lowerMessage.Contains("contact") || lowerMessage.Contains("problem") || lowerMessage.Contains("3awenni") || lowerMessage.Contains("مساعدة"))
            return "📞 **Need Help? / T7eb 3awna?**\n\n• **Email:** support@omnibus.tn\n• **Phone:** +216 71 XXX XXX\n• **Hours:** 8h sbah - 8h msa\n\nWala Ess2elni 3la:\n• Booking tickets\n• Routes & schedules\n• Payments\n• Tracking\n• Refunds";
        
        // Accurate tracking question
        if (lowerMessage.Contains("accurate") || lowerMessage.Contains("précis") || lowerMessage.Contains("daki9"))
            return "📍 **GPS Tracking Accuracy:**\n\nOur buses are tracked with high-precision GPS:\n• Updates every **30 seconds**\n• Accuracy within **10-50 meters**\n• Works on all routes\n• Shows estimated arrival time\n\nEl tracking ya3mel fi kol el routes w yetba3 el kar en temps réel!";
        
        // Default response
        return "Marhba! 🚌 Ana OmniBus AI Assistant.\n\nNajem n3awnek fi:\n• **Booking** - Kifesh ta3mel réservation\n• **Routes** - Les destinations disponibles\n• **Horaires** - Waktesh yemchi el kar\n• **Tracking** - Winou el kar taw\n• **Payment** - Kifesh tkhalles\n• **Refunds** - Kifesh trajja3 floussek\n\nEss2elni b'Anglais, Français, wala Tounsi!";
    }
    
    private List<string>? GetSuggestedQuestions(string message)
    {
        var lowerMessage = message.ToLower();
        
        if (lowerMessage.Contains("book") || lowerMessage.Contains("ticket"))
            return new List<string> { "What payment methods are available?", "Can I choose my seat?", "How do I get my ticket?" };
        
        if (lowerMessage.Contains("track") || lowerMessage.Contains("bus"))
            return new List<string> { "How accurate is the tracking?", "What routes are available?", "How do I book a ticket?" };
        
        if (lowerMessage.Contains("pay") || lowerMessage.Contains("price"))
            return new List<string> { "Is my payment secure?", "Can I get a refund?", "How do I book?" };
        
        return new List<string> { "How do I book a ticket?", "What routes are available?", "Can I track my bus?", "How do refunds work?" };
    }

    public async Task<List<RouteRecommendationDto>> GetRouteRecommendationsAsync(
        RoutePreferencesDto preferences, 
        string userId)
    {
        try
        {
            var query = BuildRouteRecommendationQuery(preferences);
            
            var payload = new
            {
                inputs = new
                {
                    origin = preferences.Origin,
                    destination = preferences.Destination,
                    departure_time = preferences.PreferredDepartureTime?.ToString("yyyy-MM-dd HH:mm"),
                    bus_type = preferences.BusTypePreference,
                    max_price = preferences.MaxPrice,
                    prefer_direct = preferences.PreferDirectRoutes
                },
                query = query,
                response_mode = "blocking",
                user = userId
            };

            var response = await _httpClient.PostAsJsonAsync("chat-messages", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DifyChatResponse>();
            
            // Parse AI response to extract recommendations
            return ParseRecommendations(result?.Answer ?? "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting route recommendations from Dify");
            return new List<RouteRecommendationDto>();
        }
    }

    public async Task<string> GetFAQAnswerAsync(string question, string userId)
    {
        try
        {
            var payload = new
            {
                inputs = new { },
                query = $"FAQ: {question}",
                response_mode = "blocking",
                user = userId
            };

            var response = await _httpClient.PostAsJsonAsync("chat-messages", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DifyChatResponse>();
            
            return result?.Answer ?? "I don't have an answer for that question right now.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting FAQ answer from Dify");
            return "I'm having trouble retrieving that information. Please contact support.";
        }
    }

    public async Task<TravelInsightsDto> GetTravelInsightsAsync(string userId)
    {
        try
        {
            var query = "Analyze my travel patterns and provide insights on my bus usage, spending, and recommendations for optimization.";
            
            var payload = new
            {
                inputs = new { user_id = userId },
                query = query,
                response_mode = "blocking",
                user = userId
            };

            var response = await _httpClient.PostAsJsonAsync("chat-messages", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DifyChatResponse>();
            
            return ParseTravelInsights(result?.Answer ?? "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting travel insights from Dify");
            return new TravelInsightsDto
            {
                Summary = "Unable to generate insights at this time.",
                Recommendations = new List<string> { "Please try again later." }
            };
        }
    }

    private string BuildRouteRecommendationQuery(RoutePreferencesDto preferences)
    {
        return $"I need to travel from {preferences.Origin} to {preferences.Destination}" +
               $"{(preferences.PreferredDepartureTime.HasValue ? $" around {preferences.PreferredDepartureTime.Value:HH:mm}" : "")}. " +
               $"Please recommend the best bus routes for me.";
    }

    private List<RouteRecommendationDto> ParseRecommendations(string aiResponse)
    {
        // Simple parsing - in production, you'd use structured output from Dify
        var recommendations = new List<RouteRecommendationDto>();
        
        // This is a placeholder - Dify can return structured JSON
        // You would configure your Dify workflow to return proper JSON format
        
        return recommendations;
    }

    private TravelInsightsDto ParseTravelInsights(string aiResponse)
    {
        // Simple parsing - in production, configure Dify for structured output
        return new TravelInsightsDto
        {
            Summary = aiResponse,
            FrequentRoutes = new List<string>(),
            Recommendations = new List<string>()
        };
    }

    // Dify API response models
    private class DifyChatResponse
    {
        public string Answer { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public List<string>? SuggestedQuestions { get; set; }
    }
}
