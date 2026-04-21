using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using SharedModels;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Translates natural-language operator questions into queries against historical
/// process data and returns AI-generated answers.
/// Uses the same database back-end as <see cref="HdaReaderService"/> and calls
/// OpenAI, Gemini, or a local Ollama model to interpret and answer the question.
/// </summary>
public class NaturalLanguageQueryService
{
    private readonly HdaReaderService _hda;
    private readonly ProjectService _project;

    public NaturalLanguageQueryService(HdaReaderService hda, ProjectService project)
    {
        _hda = hda;
        _project = project;
    }

    private NaturalLanguageQueryConfig Config =>
        _project.Settings.NaturalLanguageQuery ?? new NaturalLanguageQueryConfig();

    /// <summary>
    /// Ask a natural-language question about historical process data and get an answer.
    /// </summary>
    public async Task<NlQueryResult> AskAsync(string question, CancellationToken ct = default)
    {
        var cfg = Config;
        if (!cfg.Enabled)
            return new NlQueryResult { Answer = "Natural language query is not enabled." };

        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // 1. Gather available logged variables
            var loggedVars = _hda.GetLoggedVariables();
            if (loggedVars.Count == 0)
                return new NlQueryResult { Answer = "No variables with data logging enabled were found." };

            // 2. Identify which variables are relevant using keyword matching
            var relevant = FindRelevantVariables(question, loggedVars);
            if (relevant.Count == 0)
                relevant = loggedVars.Take(20).ToList(); // fallback: use first 20

            // 3. Determine time range from the question
            var (start, end) = ParseTimeRange(question, cfg.DefaultTimeRangeMinutes);

            // 4. Fetch historical data for relevant variables
            var dataSummary = await BuildDataContextAsync(relevant, start, end, cfg.MaxDataPoints);

            // 5. Build the AI prompt
            var prompt = BuildPrompt(question, dataSummary, loggedVars);

            // 6. Call the AI engine
            var answer = cfg.Engine switch
            {
                "OpenAI" => await CallOpenAiAsync(prompt, cfg, ct),
                "Gemini" => await CallGeminiAsync(prompt, cfg, ct),
                "Claude" => await CallClaudeAsync(prompt, cfg, ct),
                _ => await CallOllamaAsync(prompt, cfg, ct)
            };

            sw.Stop();
            return new NlQueryResult
            {
                Answer = answer,
                VariablesUsed = relevant,
                TimeRangeStart = start,
                TimeRangeEnd = end,
                DurationMs = sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new NlQueryResult
            {
                Answer = $"Error: {ex.Message}",
                DurationMs = sw.ElapsedMilliseconds
            };
        }
    }

    /// <summary>Find variables whose path segments match words in the question.</summary>
    private static List<string> FindRelevantVariables(string question, List<string> allVars)
    {
        var words = question
            .Split([' ', ',', '.', '?', '!', '"', '\'', '(', ')'], StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLowerInvariant())
            .Where(w => w.Length > 2)
            .ToHashSet();

        var scored = new List<(string Path, int Score)>();

        foreach (var path in allVars)
        {
            var segments = path.Split('.').Select(s => s.ToLowerInvariant()).ToList();
            int score = 0;
            foreach (var seg in segments)
            {
                // Exact match
                if (words.Contains(seg))
                    score += 10;
                else
                {
                    // Partial match (variable name contains query word or vice versa)
                    foreach (var w in words)
                    {
                        if (seg.Contains(w) || w.Contains(seg))
                        {
                            score += 5;
                            break;
                        }
                    }
                }
            }

            if (score > 0)
                scored.Add((path, score));
        }

        return scored
            .OrderByDescending(x => x.Score)
            .Take(10)
            .Select(x => x.Path)
            .ToList();
    }

    /// <summary>
    /// Parse natural language time references into a (start, end) range.
    /// Supports: "yesterday", "today", "last N hours/minutes/days", "this week", etc.
    /// </summary>
    private static (DateTime Start, DateTime End) ParseTimeRange(string question, int defaultMinutes)
    {
        var now = DateTime.UtcNow;
        var q = question.ToLowerInvariant();

        if (q.Contains("yesterday"))
            return (now.Date.AddDays(-1), now.Date);

        if (q.Contains("today"))
            return (now.Date, now);

        if (q.Contains("this week"))
            return (now.Date.AddDays(-(int)now.DayOfWeek), now);

        if (q.Contains("last week"))
        {
            var weekStart = now.Date.AddDays(-(int)now.DayOfWeek - 7);
            return (weekStart, weekStart.AddDays(7));
        }

        if (q.Contains("this month"))
            return (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc), now);

        // "last N hours/minutes/days"
        var match = System.Text.RegularExpressions.Regex.Match(q, @"last\s+(\d+)\s+(hour|minute|min|day|week)s?");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var n))
        {
            var unit = match.Groups[2].Value;
            var span = unit switch
            {
                "hour" => TimeSpan.FromHours(n),
                "minute" or "min" => TimeSpan.FromMinutes(n),
                "day" => TimeSpan.FromDays(n),
                "week" => TimeSpan.FromDays(n * 7),
                _ => TimeSpan.FromMinutes(defaultMinutes)
            };
            return (now - span, now);
        }

        // "past N hours" variant
        match = System.Text.RegularExpressions.Regex.Match(q, @"past\s+(\d+)\s+(hour|minute|min|day|week)s?");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var n2))
        {
            var unit = match.Groups[2].Value;
            var span = unit switch
            {
                "hour" => TimeSpan.FromHours(n2),
                "minute" or "min" => TimeSpan.FromMinutes(n2),
                "day" => TimeSpan.FromDays(n2),
                "week" => TimeSpan.FromDays(n2 * 7),
                _ => TimeSpan.FromMinutes(defaultMinutes)
            };
            return (now - span, now);
        }

        // Default fallback
        return (now.AddMinutes(-defaultMinutes), now);
    }

    /// <summary>Read historical data and build a compact textual summary for the AI context.</summary>
    private async Task<string> BuildDataContextAsync(
        List<string> variables, DateTime start, DateTime end, int maxPoints)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Time range: {start:yyyy-MM-dd HH:mm} UTC to {end:yyyy-MM-dd HH:mm} UTC");
        sb.AppendLine();

        var seriesList = await _hda.ReadHistoryAsync(variables, 0, maxPoints);

        // Apply time filter manually since ReadHistoryAsync with 0 minutes reads all
        // We actually need to pass the correct timeRangeMinutes
        var totalMinutes = (int)(end - start).TotalMinutes;
        if (totalMinutes <= 0) totalMinutes = 1440;
        seriesList = await _hda.ReadHistoryAsync(variables, totalMinutes, maxPoints);

        foreach (var series in seriesList)
        {
            if (series.Points.Count == 0)
            {
                sb.AppendLine($"Variable: {series.VariableName} — No data in range.");
                continue;
            }

            // Compute statistics
            var numericPoints = series.Points.Where(p => p.Value.HasValue).Select(p => p.Value!.Value).ToList();

            sb.AppendLine($"Variable: {series.VariableName}");
            sb.AppendLine($"  Data points: {series.Points.Count}");

            if (numericPoints.Count > 0)
            {
                var min = numericPoints.Min();
                var max = numericPoints.Max();
                var avg = numericPoints.Average();
                var last = numericPoints.Last();
                var first = numericPoints.First();
                var sum = numericPoints.Sum();
                var stdDev = numericPoints.Count > 1
                    ? Math.Sqrt(numericPoints.Average(v => Math.Pow(v - avg, 2)))
                    : 0;

                sb.AppendLine($"  Min: {min:G6}");
                sb.AppendLine($"  Max: {max:G6}");
                sb.AppendLine($"  Average: {avg:G6}");
                sb.AppendLine($"  Std Dev: {stdDev:G6}");
                sb.AppendLine($"  Sum: {sum:G6}");
                sb.AppendLine($"  First value: {first:G6} at {series.Points.First().Time:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"  Last value: {last:G6} at {series.Points.Last().Time:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"  Min at: {series.Points.Where(p => p.Value.HasValue).OrderBy(p => p.Value!.Value).First().Time:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"  Max at: {series.Points.Where(p => p.Value.HasValue).OrderByDescending(p => p.Value!.Value).First().Time:yyyy-MM-dd HH:mm:ss}");

                // Include a sample of raw data points (first 10 + last 10)
                sb.AppendLine("  Sample data (time → value):");
                var sample = series.Points.Take(10)
                    .Concat(series.Points.Count > 20 ? series.Points.Skip(series.Points.Count - 10) : [])
                    .Distinct()
                    .OrderBy(p => p.Time)
                    .ToList();
                foreach (var pt in sample)
                {
                    var val = pt.Value.HasValue ? pt.Value.Value.ToString("G6", CultureInfo.InvariantCulture) : (pt.StringValue ?? "null");
                    sb.AppendLine($"    {pt.Time:yyyy-MM-dd HH:mm:ss} → {val}");
                }
            }
            else
            {
                // String-only data
                sb.AppendLine($"  (String values only)");
                foreach (var pt in series.Points.Take(20))
                    sb.AppendLine($"    {pt.Time:yyyy-MM-dd HH:mm:ss} → {pt.StringValue}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string BuildPrompt(string question, string dataContext, List<string> allVariables)
    {
        return $"""
            You are an industrial process data analyst assistant for an HMI/SCADA system.
            An operator is asking a question about their process data. Answer concisely and accurately based ONLY on the data provided below.
            If you cannot answer from the data, say so clearly.
            Use the variable names from the data. Include relevant numbers, times, and units where applicable.
            Format your answer as plain text suitable for display in a chat widget. Keep it brief — 2-5 sentences for simple questions.

            Available logged variables in the system:
            {string.Join(", ", allVariables.Take(50))}

            === HISTORICAL DATA ===
            {dataContext}
            === END DATA ===

            Operator question: {question}

            Answer:
            """;
    }

    private static async Task<string> CallOpenAiAsync(string prompt, NaturalLanguageQueryConfig cfg, CancellationToken ct)
    {
        var apiKey = !string.IsNullOrEmpty(cfg.ApiKey) ? cfg.ApiKey : Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return "OpenAI API key not configured. Set the OPENAI_API_KEY environment variable.";

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var model = string.IsNullOrEmpty(cfg.Model) || cfg.Model == "mistral" ? "gpt-4o" : cfg.Model;
        var body = new
        {
            model,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.2
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            return $"OpenAI error ({response.StatusCode}): {err}";
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
    }

    private static async Task<string> CallGeminiAsync(string prompt, NaturalLanguageQueryConfig cfg, CancellationToken ct)
    {
        var apiKey = !string.IsNullOrEmpty(cfg.ApiKey) ? cfg.ApiKey : Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return "Gemini API key not configured. Set the GEMINI_API_KEY environment variable.";

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };

        var body = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } }
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}",
            content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            return $"Gemini error ({response.StatusCode}): {err}";
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";
    }

    private static async Task<string> CallClaudeAsync(string prompt, NaturalLanguageQueryConfig cfg, CancellationToken ct)
    {
        var apiKey = !string.IsNullOrEmpty(cfg.ApiKey) ? cfg.ApiKey : Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return "Anthropic API key not configured. Set the ANTHROPIC_API_KEY environment variable.";

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var model = string.IsNullOrEmpty(cfg.Model) || cfg.Model == "mistral" ? "claude-3-5-sonnet-20241022" : cfg.Model;
        var body = new
        {
            model,
            max_tokens = 2048,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.2
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://api.anthropic.com/v1/messages", content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            return $"Claude error ({response.StatusCode}): {err}";
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString() ?? "";
    }

    private static async Task<string> CallOllamaAsync(string prompt, NaturalLanguageQueryConfig cfg, CancellationToken ct)
    {
        var baseUrl = string.IsNullOrEmpty(cfg.OllamaBaseUrl) ? "http://127.0.0.1:11434" : cfg.OllamaBaseUrl;
        var model = string.IsNullOrEmpty(cfg.Model) ? "mistral" : cfg.Model;

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };

        var body = new
        {
            model,
            prompt,
            stream = false,
            options = new { temperature = 0.2 }
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync($"{baseUrl}/api/generate", content, ct);
        }
        catch (HttpRequestException ex)
        {
            return $"Cannot reach Ollama at {baseUrl}. Make sure Ollama is running. ({ex.Message})";
        }

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            return $"Ollama error ({response.StatusCode}): {err}";
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("response").GetString() ?? "";
    }
}

/// <summary>Result of a natural language query.</summary>
public class NlQueryResult
{
    public string Answer { get; set; } = "";
    public List<string> VariablesUsed { get; set; } = [];
    public DateTime? TimeRangeStart { get; set; }
    public DateTime? TimeRangeEnd { get; set; }
    public long DurationMs { get; set; }
}
