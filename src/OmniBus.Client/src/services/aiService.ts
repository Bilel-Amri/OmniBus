import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

export interface AIChatRequest {
  message: string;
  conversationId?: string;
  context?: Record<string, any>;
}

export interface AIChatResponse {
  message: string;
  conversationId: string;
  suggestedQuestions?: string[];
  timestamp: string;
}

export interface RoutePreferences {
  origin?: string;
  destination?: string;
  preferredDepartureTime?: string;
  busTypePreference?: string;
  maxPrice?: number;
  preferDirectRoutes: boolean;
}

export interface RouteRecommendation {
  routeId: string;
  routeName: string;
  recommendationReason: string;
  confidenceScore: number;
  highlights: string[];
}

export interface TravelInsights {
  summary: string;
  frequentRoutes: string[];
  preferredTravelTime: string;
  averageMonthlySpending: number;
  recommendations: string[];
}

class AIService {
  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }

  async sendChatMessage(request: AIChatRequest): Promise<AIChatResponse> {
    const response = await axios.post<AIChatResponse>(
      `${API_BASE_URL}/aiassistant/chat`,
      request,
      { headers: this.getAuthHeaders() }
    );
    return response.data;
  }

  async getRouteRecommendations(
    preferences: RoutePreferences
  ): Promise<RouteRecommendation[]> {
    const response = await axios.post<RouteRecommendation[]>(
      `${API_BASE_URL}/aiassistant/recommendations`,
      preferences,
      { headers: this.getAuthHeaders() }
    );
    return response.data;
  }

  async getFAQAnswer(question: string): Promise<string> {
    const response = await axios.post<{ answer: string }>(
      `${API_BASE_URL}/aiassistant/faq`,
      JSON.stringify(question),
      { headers: this.getAuthHeaders() }
    );
    return response.data.answer;
  }

  async getTravelInsights(): Promise<TravelInsights> {
    const response = await axios.get<TravelInsights>(
      `${API_BASE_URL}/aiassistant/insights`,
      { headers: this.getAuthHeaders() }
    );
    return response.data;
  }
}

export const aiService = new AIService();
