// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Globalization;
using System.Text;
using SharedModels;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Builds a proactive, AI-generated "plant status" narrative by combining live variable
/// values/historical stats with active alarms and recent events — no operator question required.
/// Reuses the same AI back-end configuration as <see cref="NaturalLanguageQueryService"/>
/// (<see cref="NaturalLanguageQueryConfig"/> / <see cref="AiEngineClient"/>).
/// </summary>
public class PlantOverviewService
{
    private readonly HdaReaderService _hda;
    private readonly EventLogReaderService _events;
    private readonly OpcRuntimeClient _opc;
    private readonly ProjectService _project;

    public PlantOverviewService(HdaReaderService hda, EventLogReaderService events, OpcRuntimeClient opc, ProjectService project)
    {
        _hda = hda;
        _events = events;
        _opc = opc;
        _project = project;
    }

    private NaturalLanguageQueryConfig Config =>
        _project.Settings.NaturalLanguageQuery ?? new NaturalLanguageQueryConfig();

    /// <summary>
    /// Gathers live plant data (variables, active alarms, recent events) and asks the
    /// configured AI engine to produce a general plant health/status overview.
    /// </summary>
    public async Task<PlantOverviewResult> GenerateAsync(
        string variableFilter, int timeRangeMinutes, string eventCategories,
        string? extraInstructions, CancellationToken ct = default)
    {
        var cfg = Config;
        if (!cfg.Enabled)
            return new PlantOverviewResult { Narrative = "AI Overview is not enabled (configure Natural Language Query settings in the project)." };

        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // 1. Determine which logged variables to include
            var loggedVars = _hda.GetLoggedVariables();
            var scoped = string.IsNullOrWhiteSpace(variableFilter)
                ? loggedVars
                : loggedVars.Where(v => v.StartsWith(variableFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            if (scoped.Count == 0) scoped = loggedVars;

            // 2. Historical stats context
            var dataContext = await BuildDataContextAsync(scoped, timeRangeMinutes);

            // 3. Active alarms
            var alarms = _opc.IsConnected
                ? await _opc.CollectDetailedAlarmsAsync()
                : new List<AlarmEntry>();
            var alarmContext = BuildAlarmContext(alarms);

            // 4. Recent events (system/alarm log)
            var recentEvents = await _events.ReadEventsAsync(timeRangeMinutes, 100, eventCategories);
            var eventContext = BuildEventContext(recentEvents);

            // 5. Build prompt and call AI
            var prompt = BuildPrompt(dataContext, alarmContext, eventContext, timeRangeMinutes, extraInstructions);
            var narrative = await AiEngineClient.AskAsync(prompt, cfg, ct);

            sw.Stop();
            return new PlantOverviewResult
            {
                Narrative = narrative,
                VariablesUsed = scoped,
                ActiveAlarmCount = alarms.Count(a => !a.IsShelved),
                UnackedAlarmCount = alarms.Count(a => !a.IsAcked && !a.IsShelved),
                EventCount = recentEvents.Count,
                GeneratedAt = DateTime.Now,
                DurationMs = sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new PlantOverviewResult
            {
                Narrative = $"Error generating overview: {ex.Message}",
                DurationMs = sw.ElapsedMilliseconds
            };
        }
    }

    private async Task<string> BuildDataContextAsync(List<string> variables, int timeRangeMinutes)
    {
        var sb = new StringBuilder();
        if (variables.Count == 0)
        {
            sb.AppendLine("(No data-logged variables found.)");
            return sb.ToString();
        }

        var seriesList = await _hda.ReadHistoryAsync(variables, timeRangeMinutes, 300);

        foreach (var series in seriesList)
        {
            if (series.Points.Count == 0)
            {
                sb.AppendLine($"- {series.VariableName}: no data in range");
                continue;
            }

            var numericPoints = series.Points.Where(p => p.Value.HasValue).Select(p => p.Value!.Value).ToList();
            if (numericPoints.Count == 0)
            {
                sb.AppendLine($"- {series.VariableName}: last value = {series.Points.Last().StringValue}");
                continue;
            }

            var min = numericPoints.Min();
            var max = numericPoints.Max();
            var avg = numericPoints.Average();
            var last = numericPoints.Last();
            sb.AppendLine(FormattableString.Invariant(
                $"- {series.VariableName}: current={last:G6}, min={min:G6}, max={max:G6}, avg={avg:G6} (over last {timeRangeMinutes} min, {series.Points.Count} samples)"));
        }

        return sb.ToString();
    }

    private static string BuildAlarmContext(List<AlarmEntry> alarms)
    {
        if (alarms.Count == 0)
            return "No active alarms — plant is currently alarm-free.";

        var sb = new StringBuilder();
        sb.AppendLine($"{alarms.Count} active alarm(s):");
        foreach (var a in alarms.OrderByDescending(a => a.Time).Take(30))
        {
            sb.AppendLine($"- [{a.Severity}] {a.SourceName}: {a.Message} (since {a.Time:yyyy-MM-dd HH:mm:ss}, acked={a.IsAcked}, shelved={a.IsShelved})");
        }
        return sb.ToString();
    }

    private static string BuildEventContext(List<EventLogEntry> events)
    {
        if (events.Count == 0)
            return "No recent events in range.";

        var sb = new StringBuilder();
        foreach (var e in events.Take(50))
            sb.AppendLine($"- {e.Time:yyyy-MM-dd HH:mm:ss} [{e.Category}/{e.Severity}] {e.Source}: {e.Message}");
        return sb.ToString();
    }

    private static string BuildPrompt(string dataContext, string alarmContext, string eventContext,
        int timeRangeMinutes, string? extraInstructions)
    {
        return $"""
            You are an industrial plant operations analyst assistant for an HMI/SCADA system.
            Produce a concise, operator-friendly general status overview of the plant based ONLY on the
            live data below. Cover: overall health (normal/degraded/critical), any values outside a safe
            or expected envelope, notable trends, and a summary of active alarms and recent notable events.
            Be specific with numbers and variable names. Keep it to a short paragraph or a few bullet points.
            Do not invent data that is not present below.
            {(string.IsNullOrWhiteSpace(extraInstructions) ? "" : $"\nAdditional instructions from the plant engineer: {extraInstructions}\n")}

            === PROCESS DATA (last {timeRangeMinutes} minutes) ===
            {dataContext}

            === ACTIVE ALARMS ===
            {alarmContext}

            === RECENT EVENTS ===
            {eventContext}

            Plant status overview:
            """;
    }
}

/// <summary>Result of an AI-generated plant status overview.</summary>
public class PlantOverviewResult
{
    public string Narrative { get; set; } = "";
    public List<string> VariablesUsed { get; set; } = [];
    public int ActiveAlarmCount { get; set; }
    public int UnackedAlarmCount { get; set; }
    public int EventCount { get; set; }
    public DateTime GeneratedAt { get; set; }
    public long DurationMs { get; set; }
}
