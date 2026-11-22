# DBSR Travel Assistant

**AI-powered travel planning using .NET 10, Ollama, and Qwen 3.5**

## 1. Project Overview

DBSR Travel Assistant is a locally hosted, AI-powered travel planning application built using .NET 10 and a modular architecture.

The application accepts a natural-language travel request, generates a structured travel itinerary, and provides estimated travel expenses.

It uses **Ollama with Qwen 3.5 9B** to generate travel plans locally, without depending on paid cloud AI APIs.

### Key capabilities

- Natural-language travel request processing.
- AI-generated multi-day travel itineraries.
- Chronological activity scheduling.
- Activity categorization.
- Estimated travel expenses and budget breakdowns.
- Currency-aware budget calculations.
- Structured JSON responses.
- Validation of AI-generated travel plans.
- Local AI inference using GPU acceleration where supported.

## 2. Technology Stack

| Component | Technology |
|---|---|
| Programming language | C# |
| Framework | .NET 10 |
| AI runtime | Ollama |
| AI model | Qwen 3.5 9B |
| API | ASP.NET Core Minimal API |
| Serialization | System.Text.Json |
| Testing | xUnit |
| Development environment | macOS |
| AI hosting | Local machine |

### Local AI configuration

- Model: `qwen3.5:9b`
- Ollama API: `http://localhost:11434`
- AI inference: Local
- Model storage: `models/` directory under the project workspace

No paid AI API subscription is required for local inference.

## 3. Solution Architecture

The solution follows a modular architecture with separation between business logic, infrastructure integrations, and the API layer.

### High-level architecture

```text
                 USER
                   |
                   v
          Travel Request
                   |
                   v
          ASP.NET Core Web API
                   |
                   v
       ITravelPlanGenerator
                   |
                   v
      OllamaTravelPlanGenerator
                   |
          +--------+--------+
          |                 |
          v                 v
   TravelPromptBuilder   TravelPlanValidator
          |
          v
      ITravelAiService
          |
          v
   OllamaTravelAiService
          |
          v
     Ollama HTTP API
          |
          v
      Qwen 3.5 9B
          |
          v
     Generated JSON
          |
          v
   JSON Deserialization
          |
          v
   Travel Plan Validation
          |
          v
   Structured API Response
```

### Project responsibilities

**DBSR.TravelAssistant.Core**

Contains the application's business models, interfaces, validation, mapping, and domain logic.

It is independent of the Ollama implementation.

**DBSR.TravelAssistant.Infrastructure**

Contains external integrations and infrastructure implementations, including communication with the local Ollama service.

**DBSR.TravelAssistant.Web**

Contains the ASP.NET Core API, dependency injection configuration, and HTTP endpoints.

**DBSR.TravelAssistant.Tests**

Contains unit tests and API integration tests.

## 4. Solution Folder Structure

```text
DBSR/
└── TravelAssistant/
    │
    ├── models/
    │   └── Ollama model storage
    │
    ├── src/
    │   │
    │   ├── DBSR.TravelAssistant.slnx
    │   ├── global.json
    │   │
    │   ├── DBSR.TravelAssistant.Core/
    │   │   │
    │   │   ├── AI/
    │   │   │   ├── ITravelAiService.cs
    │   │   │   ├── ITravelPlanGenerator.cs
    │   │   │   ├── TravelPromptBuilder.cs
    │   │   │   ├── TravelPlanResponse.cs
    │   │   │   ├── TravelPlanDayResponse.cs
    │   │   │   ├── TravelPlanActivityResponse.cs
    │   │   │   ├── TravelPlanExpenseResponse.cs
    │   │   │   ├── TravelPlanJsonSerializer.cs
    │   │   │   ├── TravelPlanValidator.cs
    │   │   │   └── TravelPlanMapper.cs
    │   │   │
    │   │   ├── Accommodations/
    │   │   │   ├── Accommodation.cs
    │   │   │   ├── AccommodationBooking.cs
    │   │   │   ├── AccommodationBookingStatus.cs
    │   │   │   ├── AccommodationRoom.cs
    │   │   │   ├── AccommodationGuest.cs
    │   │   │   ├── AccommodationType.cs
    │   │   │   └── AccommodationAmenity.cs
    │   │   │
    │   │   ├── Budgets/
    │   │   │   ├── TripBudgetCalculator.cs
    │   │   │   ├── TripBudgetSummary.cs
    │   │   │   └── Budget domain models
    │   │   │
    │   │   ├── Itineraries/
    │   │   │   └── Itinerary domain models
    │   │   │
    │   │   └── Trips/
    │   │       └── Trip domain models
    │   │
    │   ├── DBSR.TravelAssistant.Infrastructure/
    │   │   │
    │   │   └── AI/
    │   │       ├── OllamaTravelAiService.cs
    │   │       └── OllamaTravelPlanGenerator.cs
    │   │
    │   ├── DBSR.TravelAssistant.Web/
    │   │   └── Program.cs
    │   │
    │   └── DBSR.TravelAssistant.Tests/
    │       └── UnitTest1.cs
    │
    └── README.md
```

*Note: This represents the main source files and folders. Additional project files, build output, and supporting files are omitted for readability.*

## 5. Prerequisites

Install the following software:

1. macOS or another supported development environment.
2. .NET 10 SDK.
3. Ollama.
4. Qwen 3.5 9B model.

Verify the .NET installation:

```bash
dotnet --version
```

Expected version:

```text
10.0.401
```

Verify Ollama:

```bash
ollama --version
```

## 6. Local AI Setup

### Step 1: Install Ollama

Download and install Ollama:

https://ollama.com/download

### Step 2: Download the Qwen model

```bash
ollama pull qwen3.5:9b
```

### Step 3: Verify the installed model

```bash
ollama list
```

Expected model:

```text
NAME
qwen3.5:9b
```

### Step 4: Test local AI inference

```bash
ollama run qwen3.5:9b "Reply with exactly: Local AI is working."
```

Expected response:

```text
Local AI is working.
```

### Step 5: Verify Ollama API

```bash
curl http://localhost:11434/api/tags
```

The response should contain the installed Qwen model.

## 7. Build the Solution

Navigate to the source directory:

```bash
cd ~/Documents/DBSR/TravelAssistant/src
```

Restore and build:

```bash
dotnet build DBSR.TravelAssistant.slnx
```

Expected result:

```text
Build succeeded.
```

## 8. Run the Application

Start the Web API:

```bash
ASPNETCORE_URLS=http://localhost:5025 \
dotnet run --project DBSR.TravelAssistant.Web/DBSR.TravelAssistant.Web.csproj
```

Expected output:

```text
Now listening on: http://localhost:5025
Application started.
Hosting environment: Development
```

The API communicates with Ollama through:

```text
http://localhost:11434
```

Ensure Ollama is running before submitting travel requests.

## 9. Submit a Travel Request

The current API accepts a JSON request containing a `userRequest` string.

### API endpoint

```http
POST /api/travel-plans/generate
```

### Example CLI command

Open another terminal window and execute:

```bash
curl -X POST http://localhost:5025/api/travel-plans/generate \
  -H "Content-Type: application/json" \
  -d '{
    "userRequest": "Plan a 2-day family trip to Denver, Colorado for 2 adults and one 14-year-old child. Travel dates December 1 to December 2, 2026. Budget is 1500 USD. Include family-friendly attractions, meal breaks, realistic activity timings and estimated expenses."
  }'
```

### Example input

```json
{
  "userRequest": "Plan a 2-day family trip to Denver, Colorado for 2 adults and one 14-year-old child. Travel dates December 1 to December 2, 2026. Budget is 1500 USD. Include family-friendly attractions, meal breaks, realistic activity timings and estimated expenses."
}
```

### Example output structure

```json
{
  "tripName": "Denver Family Adventure",
  "overview": "A family-friendly Denver travel plan.",
  "currencyCode": "USD",
  "startDate": "2026-12-01",
  "endDate": "2026-12-02",
  "numberOfTravelers": 3,
  "budgetLimit": 1500,
  "assumptions": [],
  "days": [
    {
      "dayNumber": 1,
      "date": "2026-12-01",
      "title": "Downtown Exploration",
      "activities": [
        {
          "name": "Union Station Tour",
          "category": "Sightseeing",
          "startTime": "10:00:00",
          "endTime": "12:00:00",
          "estimatedCost": 0,
          "currencyCode": "USD",
          "sequenceNumber": 1
        }
      ]
    }
  ],
  "expenses": []
}
```

*This JSON is an illustrative example of the response structure, not a guaranteed AI response.*

## 10. How Travel Plan Generation Works

The application processes a travel request through the following stages.

### Stage 1: User request

The user provides travel details in natural language.

For example:

- Destination
- Travel dates
- Number of travellers
- Budget
- Preferences and requirements

Currently, the API receives these details through the `userRequest` property.

### Stage 2: Prompt construction

`TravelPromptBuilder.cs` constructs a structured prompt.

The prompt instructs the AI to:

- Preserve the requested destination.
- Respect travel dates and traveller count.
- Generate chronological activities.
- Include realistic activity durations.
- Avoid overlapping activities.
- Provide estimated expenses.
- Use valid JSON.
- Follow the application's response schema.

### Stage 3: Local AI inference

`OllamaTravelAiService.cs` sends the prompt to Ollama.

The model configuration uses:

```text
Model: qwen3.5:9b
Endpoint: http://localhost:11434/api/generate
Streaming: Disabled
Temperature: 0.3
```

Qwen processes the request locally and generates a JSON response.

### Stage 4: JSON deserialization

`TravelPlanJsonSerializer.cs` converts the generated JSON into strongly typed C# objects.

This provides structured access to:

- Trip details
- Itinerary days
- Activities
- Estimated expenses

Enum values are deserialized from strings rather than numeric values.

### Stage 5: Travel plan validation

`TravelPlanValidator.cs` checks the generated plan.

Current validation includes:

- Required trip information.
- Valid trip dates.
- Positive traveller count.
- Valid currency codes.
- Valid activity categories.
- Valid budget categories.
- Unique itinerary day numbers.
- Unique activity sequence numbers.
- Activity time validity.
- Chronological sequence consistency.
- Activity overlap detection.
- Nonnegative activity and expense amounts.
- Expense dates within trip dates.

Invalid generated plans are rejected rather than silently accepted.

### Stage 6: Structured response

A validated `TravelPlanResponse` is returned through the API.

`TravelPlanMapper.cs` also provides mapping functionality from AI response models to the application's itinerary and expense domain models.

Database persistence is not yet implemented.

## 11. Budget Management

The Core project contains a budget calculation engine.

`TripBudgetCalculator.cs` supports:

- Expense aggregation.
- Daily expense summaries.
- Category-based expense summaries.
- Currency conversion.
- Historical exchange-rate selection based on expense dates.
- Budget validation.

The AI-generated expenses are estimates, not confirmed bookings or actual payments.

## 12. Automated Testing

The solution includes an xUnit test project.

Run all tests:

```bash
dotnet test DBSR.TravelAssistant.Tests/DBSR.TravelAssistant.Tests.csproj
```

Current test status:

| Test category | Coverage |
|---|---|
| Budget calculator | Unit tests |
| Travel plan validator | Unit tests |
| Travel plan mapper | Unit tests |
| JSON serializer | Unit tests |
| Ollama plan generator | Unit tests |
| Web API | Integration tests |

**Current result: 43 tests passed, 0 failed.**

Build and test commands:

```bash
dotnet build DBSR.TravelAssistant.slnx

dotnet test DBSR.TravelAssistant.Tests/DBSR.TravelAssistant.Tests.csproj
```

## 13. Current Limitations

The current implementation is an AI-powered backend foundation.

The following items are not yet complete:

- Blazor user interface.
- Structured accommodation details in AI responses.
- Deterministic accommodation-night reconciliation.
- Database persistence.
- User authentication and authorization.
- Live attraction opening-hour verification.
- Live hotel availability and pricing.
- Live route and distance verification.
- Booking integrations.
- Production deployment configuration.

The local AI model does not automatically access real-time travel information. Attraction information, opening hours, and prices must be treated as unverified unless separately checked against reliable sources.

## 14. Development Roadmap

### Phase 1 – Backend and Local AI Foundation

- [x] Create .NET solution and project structure.
- [x] Define travel domain models.
- [x] Implement budget calculation.
- [x] Configure Ollama.
- [x] Download and configure Qwen 3.5 9B.
- [x] Implement AI service integration.
- [x] Implement prompt construction.
- [x] Define structured AI response models.
- [x] Implement JSON deserialization.
- [x] Implement travel plan validation.
- [x] Implement itinerary and expense mapping.
- [x] Add automated unit and integration tests.
- [x] Generate a real travel plan using local AI.

### Phase 2 – Travel Planning UI

- [ ] Create a Blazor-based user interface.
- [ ] Add destination input.
- [ ] Add start date and end date selectors.
- [ ] Calculate inclusive trip duration automatically.
- [ ] Add traveller count.
- [ ] Add USD budget.
- [ ] Add optional travel preferences.
- [ ] Construct the natural-language `userRequest` in the UI.
- [ ] Integrate the existing generation API.
- [ ] Display the generated itinerary and expenses.

### Phase 3 – Enhanced Travel Planning

- [ ] Add structured accommodation response models.
- [ ] Calculate accommodation nights deterministically.
- [ ] Improve accommodation expense validation.
- [ ] Add retry or correction handling for invalid AI output.
- [ ] Improve geographic and scheduling validation.

### Phase 4 – Data and Integrations

- [ ] Add database persistence.
- [ ] Save and retrieve travel plans.
- [ ] Integrate live maps and routing.
- [ ] Integrate attraction information.
- [ ] Explore accommodation and booking integrations.

### Phase 5 – Production Readiness

- [ ] Add authentication and authorization.
- [ ] Add application logging and monitoring.
- [ ] Add deployment configuration.
- [ ] Add performance and security testing.

## 15. Design Principles

The application follows these principles:

1. **Local-first AI:** Run inference locally using Ollama and avoid paid cloud AI APIs during initial development.
2. **Separation of concerns:** Keep business logic independent of AI infrastructure.
3. **Strongly typed responses:** Convert generated JSON into C# models.
4. **Validation before acceptance:** Validate AI-generated plans before returning them.
5. **Extensibility:** Keep the architecture ready for additional AI models, UI development, databases, and travel integrations.
6. **Transparent estimates:** Clearly distinguish estimated costs from confirmed prices and actual expenses.

## 16. Project Outcome

DBSR Travel Assistant currently provides a working .NET 10 backend capable of accepting a natural-language travel request, processing it through a locally hosted Qwen model, and returning a structured travel plan with itinerary activities and estimated expenses.

The foundation includes:

- Modular .NET architecture.
- Local AI integration.
- Structured travel response models.
- Prompt engineering.
- Validation and mapping.
- Budget calculation.
- 43 passing automated tests.
- A working end-to-end API generation flow.

**The next milestone is the development of the user interface**, where users will enter their destination, travel dates, number of travellers, budget, and additional requirements. The UI will construct the `userRequest` and send it to the existing API.

---

**Repository:** DBSR Travel Assistant  
**Framework:** .NET 10  
**AI Runtime:** Ollama  
**Model:** Qwen 3.5 9B  
**Current Status:** Backend foundation completed; UI development next.
