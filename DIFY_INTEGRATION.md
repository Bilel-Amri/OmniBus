# Dify AI Integration Guide for OmniBus

## Overview
This document explains how to integrate and configure Dify AI in the OmniBus bus service platform.

## What is Dify?
Dify is an open-source LLM application development platform that provides:
- AI chatbot capabilities
- Workflow automation
- RAG (Retrieval-Augmented Generation) pipelines
- Agent capabilities with 50+ built-in tools
- Support for multiple LLM providers (OpenAI, Claude, Llama, etc.)

## Features Added to OmniBus

### 1. **AI Customer Support Chatbot**
- 24/7 automated customer support
- Natural language query understanding
- Context-aware responses
- Multi-turn conversations

### 2. **Intelligent Route Recommendations**
- AI-powered route suggestions based on user preferences
- Consider factors like time, price, and comfort
- Personalized recommendations based on travel history

### 3. **FAQ System**
- Instant answers to common questions
- Natural language question processing
- Continuously learning from user interactions

### 4. **Travel Insights**
- Analyze user travel patterns
- Provide spending insights
- Suggest optimizations and cost savings

## Setup Instructions

### Step 1: Set Up Dify

#### Option A: Use Dify Cloud (Recommended for Quick Start)
1. Go to [Dify Cloud](https://cloud.dify.ai/)
2. Sign up for a free account (includes 200 free GPT-4 calls)
3. Create a new "Chat App" or "Agent App"
4. Get your API key from the app settings

#### Option B: Self-Host Dify (For Production)
```bash
# Clone Dify repository
git clone https://github.com/langgenius/dify.git
cd dify/docker

# Copy environment file
cp .env.example .env

# Edit .env with your settings (PostgreSQL, Redis, etc.)
nano .env

# Start Dify with Docker Compose
docker compose up -d

# Access Dify at http://localhost
```

### Step 2: Configure Your Dify Application

1. **Create a Chatbot App in Dify:**
   - Log in to Dify dashboard
   - Click "Create App" → "Chat App"
   - Name it "OmniBus Support Assistant"

2. **Configure the Prompt:**
   ```
   You are an AI assistant for OmniBus, a bus transportation service in Tunisia.
   
   Your responsibilities:
   - Help users find and book bus routes
   - Answer questions about schedules, prices, and policies
   - Provide travel recommendations
   - Assist with booking issues
   
   Be friendly, professional, and concise. Always prioritize user safety and satisfaction.
   
   Available bus types:
   - Standard: Basic comfort, affordable
   - Express: Faster routes, fewer stops
   - Luxury: Premium comfort, entertainment
   
   Context: {context}
   User Message: {query}
   ```

3. **Add Knowledge Base (Optional):**
   - Upload documents about routes, policies, FAQs
   - Dify will use RAG to provide accurate answers

4. **Configure Model:**
   - Select your preferred LLM (GPT-4, Claude, Llama, etc.)
   - Adjust temperature (0.7 recommended for balanced responses)
   - Set max tokens (1000-2000 for detailed responses)

5. **Get API Credentials:**
   - Go to "API Access" in your app
   - Copy the API Key
   - Note the Base URL

### Step 3: Configure OmniBus Backend

1. **Update appsettings.json:**
   ```json
   {
     "Dify": {
       "ApiKey": "your-actual-api-key-here",
       "BaseUrl": "https://api.dify.ai/v1"
     }
   }
   ```

   Or for self-hosted:
   ```json
   {
     "Dify": {
       "ApiKey": "your-local-api-key",
       "BaseUrl": "http://localhost/v1"
     }
   }
   ```

2. **Restore NuGet packages:**
   ```bash
   cd src/OmniBus.API
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

### Step 4: Test the Integration

1. **Start the backend:**
   ```bash
   dotnet run
   ```

2. **Test the API endpoint:**
   ```bash
   curl -X POST http://localhost:5000/api/aiassistant/chat \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer your-jwt-token" \
     -d '{
       "message": "What routes are available from Tunis to Sousse?",
       "conversationId": null
     }'
   ```

### Step 5: Configure Frontend

1. **Set API URL in .env:**
   ```bash
   cd src/OmniBus.Client
   echo "VITE_API_URL=http://localhost:5000/api" > .env
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Add the chat widget to your main app:**
   ```tsx
   import { AIChatWidget } from './components/AIChatWidget';
   
   function App() {
     const [chatOpen, setChatOpen] = useState(false);
     
     return (
       <>
         {/* Your app content */}
         
         {/* Floating chat button */}
         <Fab 
           color="primary" 
           onClick={() => setChatOpen(true)}
           sx={{ position: 'fixed', bottom: 16, right: 16 }}
         >
           <ChatIcon />
         </Fab>
         
         {/* AI Chat Widget */}
         <AIChatWidget open={chatOpen} onClose={() => setChatOpen(false)} />
       </>
     );
   }
   ```

4. **Start the dev server:**
   ```bash
   npm run dev
   ```

## Advanced Configuration

### Custom Workflows in Dify

Create advanced workflows for specific use cases:

1. **Route Finder Workflow:**
   - Input: Origin, Destination, Time
   - Steps: Query database → Filter available routes → Rank by preference → Return top 3
   
2. **Booking Assistant:**
   - Input: User request
   - Steps: Understand intent → Check availability → Guide through booking → Confirm

3. **Travel Advisor:**
   - Input: Travel history
   - Steps: Analyze patterns → Identify trends → Generate insights → Provide recommendations

### Adding RAG (Knowledge Base)

Upload documents to Dify for accurate, contextual responses:
- Route schedules (PDF)
- Company policies (DOC)
- FAQ documents (TXT)
- Terms and conditions (PDF)

### Monitoring and Analytics

- Use Dify's built-in analytics dashboard
- Track conversation metrics
- Identify common user questions
- Continuously improve prompts

## API Endpoints

### Chat
```http
POST /api/aiassistant/chat
Authorization: Bearer {token}
Content-Type: application/json

{
  "message": "How do I book a ticket?",
  "conversationId": "optional-existing-conversation-id"
}
```

### Route Recommendations
```http
POST /api/aiassistant/recommendations
Authorization: Bearer {token}
Content-Type: application/json

{
  "origin": "Tunis",
  "destination": "Sousse",
  "preferredDepartureTime": "2026-01-15T08:00:00",
  "preferDirectRoutes": true
}
```

### FAQ
```http
POST /api/aiassistant/faq
Authorization: Bearer {token}
Content-Type: application/json

"How do refunds work?"
```

### Travel Insights
```http
GET /api/aiassistant/insights
Authorization: Bearer {token}
```

## Cost Optimization

### For Dify Cloud:
- Free tier: 200 GPT-4 calls/month
- Use GPT-3.5-turbo for cost savings
- Implement caching for common queries

### For Self-Hosted:
- Use open-source models (Llama 3, Mistral)
- Host on your own infrastructure
- No per-call costs

## Troubleshooting

### Common Issues:

1. **401 Unauthorized:**
   - Check API key is correct
   - Verify JWT token is valid

2. **No AI responses:**
   - Verify Dify service is running
   - Check network connectivity
   - Review Dify logs

3. **Slow responses:**
   - Optimize prompts
   - Use faster models (GPT-3.5 instead of GPT-4)
   - Implement response caching

## Security Best Practices

1. **Never commit API keys to version control**
2. **Use environment variables for sensitive data**
3. **Implement rate limiting on AI endpoints**
4. **Validate and sanitize user inputs**
5. **Log and monitor AI interactions**

## Next Steps

- [ ] Configure production Dify instance
- [ ] Upload knowledge base documents
- [ ] Fine-tune prompts based on user feedback
- [ ] Add more specialized AI features
- [ ] Implement conversation analytics
- [ ] Set up A/B testing for different prompts

## Support

- Dify Documentation: https://docs.dify.ai/
- Dify GitHub: https://github.com/langgenius/dify
- OmniBus Issues: [Your repository]/issues
