# Claude AI Integration

AI Core HMI now supports **Anthropic Claude** as an AI engine alongside OpenAI, Google Gemini, and local Ollama models.

## Configuration

### 1. Editor AI Assistant (nodes.json editing, import suggestions)

**Environment Variable:**
```bash
export ANTHROPIC_API_KEY="sk-ant-api03-..."
```

**Docker Compose:**
```yaml
services:
  server-editor:
    environment:
      - ANTHROPIC_API_KEY=sk-ant-api03-...
```

Select **Claude** from the AI engine dropdown in the editor's AI panel.

### 2. Natural Language Query Service (runtime operator questions)

**In nodes.json** (`Settings.NaturalLanguageQuery`):
```json
{
  "Enabled": true,
  "Engine": "Claude",
  "Model": "claude-3-5-sonnet-20241022",
  "ApiKey": "sk-ant-api03-...",
  "MaxDataPoints": 500,
  "DefaultTimeRangeMinutes": 1440
}
```

Or set via environment variable:
```bash
export ANTHROPIC_API_KEY="sk-ant-api03-..."
```

## Supported Models

- `claude-3-5-sonnet-20241022` (default, recommended for production)
- `claude-3-5-haiku-20241022` (faster, lower cost)
- `claude-3-opus-20240229` (highest capability)

## API Endpoints

- **Messages API:** `https://api.anthropic.com/v1/messages`
- **API Version:** `2023-06-01`
- **Max Tokens:** 4096 (editor), 2048 (NL queries)
- **Temperature:** 0.1 (editor), 0.2 (runtime queries)

## Use Cases

### Editor AI Assistant
- Generate OPC UA variables from PLC exports (Siemens, Beckhoff, Allen-Bradley)
- Import KNX ETS group addresses
- Parse Modbus register maps
- Suggest driver configurations
- Create screen layouts and symbols
- Write ladder logic, structured text, or C#/VB scripts

### Natural Language Queries
Operators can ask questions like:
- *"What was the average reactor temperature in the last 8 hours?"*
- *"When did the pump last start?"*
- *"Show me all alarms from yesterday"*
- *"Which tank level dropped the fastest today?"*

Claude processes historical data context and generates concise answers displayed in the runtime's chat widget.

### AI Daily Summary Reports
Add a report section with `"Type": "AiSummary"` to any `ReportConfig` (see `Reports` in nodes.json) to get an
AI-generated operations summary — alarms/events, variable trends, and anything needing attention — as part of a
scheduled report. It reuses the same `Settings.NaturalLanguageQuery` engine/model/API key configuration described
above, so no separate setup is required.

```json
{
  "Name": "DailyOverview",
  "Enabled": true,
  "Title": "Daily Operations Summary",
  "Sections": [
    {
      "Id": "ai1",
      "Type": "AiSummary",
      "Title": "AI Summary",
      "AiSummaryVariablePaths": ["Line1.Temperature", "Tank2.Level"],
      "AiSummaryTimeRangeMinutes": 1440,
      "AiSummaryIncludeEvents": true
    }
  ],
  "Delivery": { "Method": "Email", "EmailRecipients": "ops@example.com" }
}
```

Trigger it daily by adding a `SchedulerConfig` with a time slot covering the desired run time and a
`GenerateReport` command targeting `DailyOverview` (Schedulers already support daily/weekly time slots,
weekends, and holidays).

## Comparison

| Feature | Claude 3.5 Sonnet | GPT-4o | Gemini Pro | Ollama (Mistral) |
|---------|-------------------|--------|------------|------------------|
| **Cost** | $3/$15 per 1M tokens | $5/$15 per 1M tokens | $0.50/$1.50 per 1M chars | Free (local) |
| **Speed** | Fast | Fast | Fast | Depends on hardware |
| **Context** | 200k tokens | 128k tokens | 32k tokens | 8k-32k tokens |
| **Privacy** | Cloud (Anthropic) | Cloud (OpenAI) | Cloud (Google) | Local (air-gapped) |
| **JSON parsing** | Excellent | Excellent | Good | Good |
| **Code generation** | Excellent | Excellent | Good | Fair |

## Getting an API Key

1. Sign up at [console.anthropic.com](https://console.anthropic.com)
2. Navigate to **API Keys**
3. Click **Create Key**
4. Copy the key (starts with `sk-ant-api03-`)
5. Set `ANTHROPIC_API_KEY` environment variable or paste into editor/project settings

## Pricing (as of 2024)

**Claude 3.5 Sonnet:**
- Input: $3 per million tokens
- Output: $15 per million tokens

**Typical Usage:**
- Editor import (parse 1000-line CSV): ~$0.05
- Natural language query (operator question): ~$0.01
- Anomaly explanation (AI-generated alarm message): ~$0.002

**Cost Control:**
- Use Ollama for development/testing (free, local)
- Use Claude for production operator queries (excellent quality/cost ratio)
- Use GPT-4o when maximum reasoning capability is needed
- Set rate limits and budgets in Anthropic console

## Troubleshooting

**"Anthropic API key not configured"**
→ Set `ANTHROPIC_API_KEY` environment variable or configure in `Settings.NaturalLanguageQuery.ApiKey`

**"Claude error (401): Invalid API key"**
→ Check that the key starts with `sk-ant-api03-` and is still active in console.anthropic.com

**"Claude error (429): Rate limit exceeded"**
→ You've exceeded your account's requests-per-minute limit. Wait or upgrade your tier.

**"Claude error (529): Overloaded"**
→ Anthropic's API is temporarily unavailable. Retry in a few seconds or fall back to another engine.

## Code References

- **Editor AI Service:** `ServerEditorWeb/Services/AiService.cs` → `CallClaudeAsync()`
- **Runtime NL Query:** `RuntimeViewer.Shared/Services/NaturalLanguageQueryService.cs`
- **AI Engine Client (shared):** `SharedModels/AiEngineClient.cs` → `AskAsync()` (OpenAI/Gemini/Claude/Ollama dispatch, used by both NL queries and AI report sections)
- **AI Daily Summary Reports:** `Server/ReportManager.cs` → `RenderAiSummarySection()`
- **Configuration Model:** `SharedModels/NodeModels.cs` → `NaturalLanguageQueryConfig`, `ReportSection` (`AiSummary*` properties)
- **Available Engines:** `AiService.AvailableEngines = ["OpenAI", "Gemini", "Claude", "Ollama"]`
