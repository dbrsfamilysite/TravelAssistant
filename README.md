# DBSR Travel Assistant

**AI-powered travel planning using .NET 10, Ollama, and Qwen 3.5 9B**

## 1. Project Overview

DBSR Travel Assistant is a locally hosted, AI-powered travel planning application built using .NET 10 and a modular architecture.

The application accepts a natural-language travel request, generates a structured multi-day travel itinerary, and provides estimated travel expenses.

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
- Local AI inference using available CPU/GPU acceleration.

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
| Development environment | macOS / Windows / Linux |
| AI hosting | Local machine |

### Local AI configuration

- Model: `qwen3.5:9b`
- Ollama API: `http://localhost:11434`
- AI inference: Local
- Model storage: Managed by Ollama, or optionally configured to use a local project directory.

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
    TravelPromptBuilder  TravelPlanValidator
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

## 4. Repository Structure

```text
TravelAssistant/
│
├── .gitignore
├── global.json
├── README.md
│
├── models/                         # Optional local AI model storage
│   └── (Not committed to Git)
│
└── src/
    ├── DBSR.TravelAssistant.slnx
    │
    ├── DBSR.TravelAssistant.Core/
    │   ├── AI/
    │   ├── Accommodations/
    │   ├── Budgets/
    │   ├── Destinations/
    │   ├── Itineraries/
    │   ├── Preferences/
    │   ├── Transportation/
    │   ├── TravelInsurance/
    │   ├── Travelers/
    │   └── Trips/
    │
    ├── DBSR.TravelAssistant.Infrastructure/
    │   └── AI/
    │       ├── OllamaTravelAiService.cs
    │       └── OllamaTravelPlanGenerator.cs
    │
    ├── DBSR.TravelAssistant.Web/
    │   ├── Program.cs
    │   ├── appsettings.json
    │   └── Properties/
    │
    └── DBSR.TravelAssistant.Tests/
        ├── BudgetApiIntegrationTests.cs
        ├── TravelAssistantApiFactory.cs
        ├── TravelPlanApiIntegrationTests.cs
        └── UnitTest1.cs
```

*Note: This represents the main source files and folders. Additional project files and supporting files are omitted for readability.*

## 5. Prerequisites

Before running the application, install:

1. Git
2. .NET 10 SDK
3. Ollama
4. Qwen 3.5 9B model

A development machine with sufficient memory and free disk space for the model is recommended. Actual inference performance depends on hardware, memory, and available CPU/GPU acceleration.

### Install .NET 10

Download:

https://dotnet.microsoft.com/download/dotnet/10.0

Verify installation:

```bash
dotnet --version
```

The repository currently pins the SDK using `global.json`. The expected SDK is:

```text
10.0.401
```

If the SDK version is not installed, install the version specified in `global.json` or update the SDK pin after verifying compatibility.

### Install Git

Verify:

```bash
git --version
```

### Install Ollama

Download:

https://ollama.com/download

Install the version appropriate for your operating system.

Verify:

```bash
ollama --version
```

## 6. Clone the Repository

Open Terminal, PowerShell, or your preferred command-line application.

Clone the repository:

```bash
git clone https://github.com/dbrsfamilysite/TravelAssistant.git
```

Navigate into the project:

```bash
cd TravelAssistant
```

Verify the repository structure:

```bash
ls
```

Expected main entries:

```text
.gitignore
global.json
README.md
src/
```

The `models/` directory may not exist immediately after cloning. This is expected.

**Important:** AI model files are intentionally excluded from GitHub because they occupy several gigabytes. Every developer must download the model locally.

## 7. Local AI Setup

### Step 1: Start Ollama

On macOS and Windows, open the installed Ollama application.

On Linux, start the Ollama service or run:

```bash
ollama serve
```

Keep Ollama running while using the Travel Assistant API.

If Ollama is already running as a background service or desktop application, do not start a second server unnecessarily.

### Step 2: Download Qwen 3.5 9B

Run:

```bash
ollama pull qwen3.5:9b
```

This downloads the model to the local machine.

The initial download can take some time and requires several gigabytes of available storage.

### Step 3: Verify the model

Run:

```bash
ollama list
```

Expected model entry:

```text
NAME
qwen3.5:9b
```

### Step 4: Test local AI inference

Run:

```bash
ollama run qwen3.5:9b "Reply with exactly: Local AI is working."
```

Expected response:

```text
Local AI is working.
```

This confirms that the model is installed and can generate a response locally.

### Step 5: Verify the Ollama API

Run:

```bash
curl http://localhost:11434/api/tags
```

The response should contain the installed Qwen model.

### Step 6: Optional — Configure project-local model storage

By default, Ollama manages its model files in its own local storage location.

Developers who prefer to store models under the project workspace can configure the `OLLAMA_MODELS` environment variable.

The project uses `models/` as its optional local storage directory. This directory is ignored by Git.

**macOS example:**

```bash
mkdir -p "$HOME/Documents/DBSR/TravelAssistant/models"

launchctl setenv OLLAMA_MODELS "$HOME/Documents/DBSR/TravelAssistant/models"
```

After changing the environment variable, completely quit and reopen the Ollama application so it receives the updated environment.

**Linux example:**

Configure `OLLAMA_MODELS` in the environment used to launch the Ollama service. If Ollama runs under systemd, configure the variable in its service environment and restart the service.

**Windows example:**

Set `OLLAMA_MODELS` as a user or system environment variable, then restart Ollama.

The path must point to the actual model storage directory.

If you change model storage after downloading a model, make sure the model is available in the newly configured location. You may need to download it again.

Developers can skip this optional configuration and use Ollama's default model storage.

## 8. Build the Solution

From the repository root:

```bash
cd src
```

Restore and build:

```bash
dotnet build DBSR.TravelAssistant.slnx
```

Expected result:

```text
Build succeeded.
```

## 9. Run the Application

Ensure Ollama is running and that the Qwen model is available.

From the `src` directory, start the Web API:

```bash
ASPNETCORE_URLS=http://localhost:5025 \
dotnet run --project DBSR.TravelAssistant.Web/DBSR.TravelAssistant.Web.csproj
```

Expected output includes:

```text
Now listening on: http://localhost:5025
Application started.
Hosting environment: Development
```

The API communicates with Ollama through:

```text
http://localhost:11434
```

Keep the API terminal open while testing requests.

Open another terminal window for API testing.

## 10. Submit a Travel Request

The current API accepts a JSON request containing a `userRequest` string.

### API endpoint

```http
POST http://localhost:5025/api/travel-plans/generate
```

### Example CLI command

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

The actual itinerary and costs are generated dynamically by the local AI model.

## 11. How Travel Plan Generation Works

The application processes a travel request through the following stages.

### Stage 1: User request

The user provides travel details in natural language, including:

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

## 12. Budget Management

The Core project contains a budget calculation engine.

`TripBudgetCalculator.cs` supports:

- Expense aggregation.
- Daily expense summaries.
- Category-based expense summaries.
- Currency conversion.
- Historical exchange-rate selection based on expense dates.
- Budget validation.

AI-generated expenses are estimates, not confirmed bookings or actual payments.

## 13. Automated Testing

The solution includes an xUnit test project.

From the `src` directory, run:

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

Build and test:

```bash
dotnet build DBSR.TravelAssistant.slnx

dotnet test DBSR.TravelAssistant.Tests/DBSR.TravelAssistant.Tests.csproj
```

## 14. Troubleshooting

### Problem: `dotnet` command not found

Install the .NET 10 SDK and reopen your terminal.

Verify:

```bash
dotnet --version
```

### Problem: Ollama connection refused

Make sure Ollama is running.

Test:

```bash
curl http://localhost:11434/api/tags
```

If the API is unavailable, start Ollama using the appropriate method for your operating system.

### Problem: Model not found

Check installed models:

```bash
ollama list
```

Download the required model:

```bash
ollama pull qwen3.5:9b
```

### Problem: API cannot connect to Ollama

Verify:

1. Ollama is running.
2. The Ollama API is accessible at `http://localhost:11434`.
3. The model name matches `qwen3.5:9b`.
4. The model is installed in the storage location used by the running Ollama service.

### Problem: AI generation takes too long

Model inference time depends on:

- Available RAM.
- CPU/GPU hardware.
- Model loading time.
- Prompt size.
- Number of requested itinerary activities.

Allow sufficient time for the first model load and generation.

## 15. Current Limitations

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

## 16. Development Roadmap

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

## 17. Design Principles

1. **Local-first AI:** Run inference locally using Ollama and avoid paid cloud AI APIs during initial development.
2. **Separation of concerns:** Keep business logic independent of AI infrastructure.
3. **Strongly typed responses:** Convert generated JSON into C# models.
4. **Validation before acceptance:** Validate AI-generated plans before returning them.
5. **Extensibility:** Keep the architecture ready for additional AI models, UI development, databases, and travel integrations.
6. **Transparent estimates:** Clearly distinguish estimated costs from confirmed prices and actual expenses.
7. **Reproducible setup:** Document the required SDK, model, and configuration so developers can run the application independently.

## 18. Project Outcome

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
- Documented local developer setup.

**The next milestone is the development of the user interface**, where users will enter their destination, travel dates, number of travellers, budget, and additional requirements. The UI will construct the `userRequest` and send it to the existing API.

---

**Repository:** DBSR Travel Assistant  
**Framework:** .NET 10  
**AI Runtime:** Ollama  
**Model:** Qwen 3.5 9B  
**Current Status:** Backend foundation completed; UI development next.


Screenshot Directory

docs/
└── screenshots/
    ├── 01-api-running.png
    ├── 02-travel-plan-output.png
    └── 03-formatted-json-response.png

These screenshots provide a visual reference for developers setting up, running, and testing the DBSR Travel Assistant locally.
