// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Runs one or more LLM-driven AI Agents (<see cref="AiAgentConfig"/>) that periodically
    /// read a scoped set of variables, ask an LLM to reason over them against natural-language
    /// instructions, and execute the resulting actions (write variable / execute script /
    /// send notification / log event).
    ///
    /// Safety model:
    ///   - The agent only ever reads variables listed in <see cref="AiAgentConfig.ReadableVariables"/>.
    ///   - WriteVariable actions are rejected unless the target path is in
    ///     <see cref="AiAgentConfig.WritableVariables"/>.
    ///   - ExecuteScript actions are rejected unless the script name is in
    ///     <see cref="AiAgentConfig.AllowedScripts"/>.
    ///   - When <see cref="AiAgentConfig.DryRun"/> is true, actions are logged only, never applied.
    ///   - A per-agent cooldown limits how often actions can actually be applied.
    ///   - Every requested action (accepted or rejected) is written to the event log for audit.
    /// </summary>
    public sealed class AiAgentManager : IDisposable
    {
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly List<AgentState> _states = new();
        private readonly CancellationTokenSource _cts = new();
        private static readonly HttpClient s_http = new() { Timeout = TimeSpan.FromSeconds(60) };

        public AiAgentManager(SimpleFileServerNodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }

        public void Initialize(List<AiAgentConfig> agents)
        {
            foreach (var agent in agents)
            {
                DiagnosticsCollector.Instance.Register("AiAgent", agent.Name, agent.Enabled);
                if (!agent.Enabled) continue;

                var interval = Math.Max(5, agent.PollingIntervalSeconds);
                var state = new AgentState(agent);
                state.Timer = new Timer(_ => _ = TickAsync(state), null,
                    TimeSpan.FromSeconds(interval), TimeSpan.FromSeconds(interval));
                _states.Add(state);
            }

            if (_states.Count > 0)
                Log.Information("AiAgentManager initialized with {Count} agent(s).", _states.Count);
        }

        private async Task TickAsync(AgentState state)
        {
            if (_cts.IsCancellationRequested) return;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await EvaluateAgentAsync(state);
                sw.Stop();
                DiagnosticsCollector.Instance.RecordCycle("AiAgent", state.Config.Name, sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                Log.Error(ex, "AiAgent '{Name}' tick error: {Error}", state.Config.Name, ex.Message);
                DiagnosticsCollector.Instance.RecordCycle("AiAgent", state.Config.Name, sw.Elapsed.TotalMilliseconds, error: ex.Message);
            }
        }

        private async Task EvaluateAgentAsync(AgentState state)
        {
            var cfg = state.Config;

            // Prevent overlapping evaluations of the same agent.
            if (!await state.Gate.WaitAsync(0)) return;
            try
            {
                var snapshot = ReadContext(cfg);
                var prompt = BuildPrompt(cfg, snapshot);

                string? rawResponse;
                try
                {
                    rawResponse = await CallLlmAsync(cfg, prompt);
                }
                catch (Exception ex)
                {
                    Log.Warning("AiAgent '{Name}' LLM call failed: {Error}", cfg.Name, ex.Message);
                    _nodeManager.EventLogger?.LogSystem("Warning", $"AiAgent:{cfg.Name}", $"LLM call failed: {ex.Message}");
                    return;
                }

                var actions = ParseActions(rawResponse);
                if (actions.Count == 0) return;

                if (actions.Count > cfg.MaxActionsPerCycle)
                    actions = actions.Take(cfg.MaxActionsPerCycle).ToList();

                // Cooldown applies to actually-applied (non-dry-run) action batches.
                bool withinCooldown = cfg.CooldownSeconds > 0 && state.LastAppliedAt.HasValue &&
                    (DateTime.Now - state.LastAppliedAt.Value).TotalSeconds < cfg.CooldownSeconds;

                foreach (var action in actions)
                    ExecuteAction(cfg, action, dryRunOverride: withinCooldown && !cfg.DryRun);

                if (!withinCooldown)
                    state.LastAppliedAt = DateTime.Now;
            }
            finally
            {
                state.Gate.Release();
            }
        }

        // ─── Context building ────────────────────────────────────────────────────

        private Dictionary<string, string> ReadContext(AiAgentConfig cfg)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in cfg.ReadableVariables)
            {
                try
                {
                    var v = _nodeManager.ReadVariable(path);
                    values[path] = v?.ToString() ?? "null";
                }
                catch (Exception ex)
                {
                    values[path] = $"<error: {ex.Message}>";
                }
            }
            return values;
        }

        private static string BuildPrompt(AiAgentConfig cfg, Dictionary<string, string> context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are an industrial automation AI agent embedded in an HMI/SCADA platform.");
            sb.AppendLine("Follow the instructions below and decide which actions, if any, should be taken this cycle.");
            sb.AppendLine();
            sb.AppendLine("INSTRUCTIONS:");
            sb.AppendLine(cfg.Instructions);
            sb.AppendLine();
            sb.AppendLine("CURRENT VARIABLE VALUES (JSON):");
            sb.AppendLine(JsonSerializer.Serialize(context));
            sb.AppendLine();
            sb.AppendLine("You may ONLY write to these variables: " + string.Join(", ", cfg.WritableVariables));
            sb.AppendLine("You may ONLY execute these scripts: " + string.Join(", ", cfg.AllowedScripts));
            sb.AppendLine();
            sb.AppendLine("Respond with ONLY a JSON array (no prose, no markdown fences) of actions using this schema:");
            sb.AppendLine("""[{"type":"WriteVariable|SendNotification|ExecuteScript|LogEvent","variablePath":"","value":"","message":"","scriptName":"","reasoning":""}]""");
            sb.AppendLine("If no action is needed, respond with an empty JSON array: []");
            return sb.ToString();
        }

        // ─── LLM dispatch (reuses provider conventions from ServerEditorWeb.AiService) ──

        private static async Task<string?> CallLlmAsync(AiAgentConfig cfg, string prompt)
        {
            return cfg.Engine switch
            {
                "Claude" => await CallClaudeAsync(cfg, prompt),
                "Gemini" => await CallGeminiAsync(cfg, prompt),
                "Ollama" => await CallOllamaAsync(cfg, prompt),
                _        => await CallOpenAiAsync(cfg, prompt),
            };
        }

        private static async Task<string?> CallOpenAiAsync(AiAgentConfig cfg, string prompt)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
            if (string.IsNullOrEmpty(apiKey)) throw new InvalidOperationException("OPENAI_API_KEY not set.");

            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            req.Headers.Add("Authorization", $"Bearer {apiKey}");
            var model = string.IsNullOrWhiteSpace(cfg.Model) ? "gpt-4o-mini" : cfg.Model;
            var body = new { model, messages = new[] { new { role = "user", content = prompt } }, temperature = 0.0 };
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            using var resp = await s_http.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }

        private static async Task<string?> CallClaudeAsync(AiAgentConfig cfg, string prompt)
        {
            var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
            if (string.IsNullOrEmpty(apiKey)) throw new InvalidOperationException("ANTHROPIC_API_KEY not set.");

            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
            req.Headers.Add("x-api-key", apiKey);
            req.Headers.Add("anthropic-version", "2023-06-01");
            var model = string.IsNullOrWhiteSpace(cfg.Model) ? "claude-sonnet-4-5" : cfg.Model;
            var body = new { model, max_tokens = 1024, messages = new[] { new { role = "user", content = prompt } }, temperature = 0.0 };
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            using var resp = await s_http.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString();
        }

        private static async Task<string?> CallGeminiAsync(AiAgentConfig cfg, string prompt)
        {
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
            if (string.IsNullOrEmpty(apiKey)) throw new InvalidOperationException("GEMINI_API_KEY not set.");

            var model = string.IsNullOrWhiteSpace(cfg.Model) ? "gemini-pro" : cfg.Model;
            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
            var body = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            using var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            using var resp = await s_http.PostAsync(endpoint, content);
            resp.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
        }

        private static async Task<string?> CallOllamaAsync(AiAgentConfig cfg, string prompt)
        {
            var host = Environment.GetEnvironmentVariable("OLLAMA_HOST");
            string baseUrl;
            if (!string.IsNullOrEmpty(host))
                baseUrl = host.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? host : "http://" + host;
            else
                baseUrl = "http://127.0.0.1:11434";

            var model = string.IsNullOrWhiteSpace(cfg.Model) ? "mistral" : cfg.Model;
            var body = new { model, prompt, stream = false, options = new { temperature = 0.0 } };
            using var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            using var resp = await s_http.PostAsync($"{baseUrl}/api/generate", content);
            resp.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("response").GetString();
        }

        // ─── Response parsing ────────────────────────────────────────────────────

        private static List<AiAgentAction> ParseActions(string? rawResponse)
        {
            if (string.IsNullOrWhiteSpace(rawResponse)) return new();

            var text = rawResponse.Trim();
            // Strip common markdown code fences the model may still emit.
            if (text.StartsWith("```"))
            {
                var firstNewline = text.IndexOf('\n');
                if (firstNewline >= 0) text = text[(firstNewline + 1)..];
                var lastFence = text.LastIndexOf("```", StringComparison.Ordinal);
                if (lastFence >= 0) text = text[..lastFence];
                text = text.Trim();
            }

            try
            {
                var actions = JsonSerializer.Deserialize<List<AiAgentAction>>(text,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return actions ?? new();
            }
            catch (Exception ex)
            {
                Log.Warning("AiAgent response was not valid JSON action array: {Error}. Raw: {Raw}", ex.Message, text);
                return new();
            }
        }

        // ─── Action execution with allow-list enforcement ──────────────────────

        private void ExecuteAction(AiAgentConfig cfg, AiAgentAction action, bool dryRunOverride)
        {
            bool dryRun = cfg.DryRun || dryRunOverride;
            string outcome;

            try
            {
                switch (action.Type)
                {
                    case "WriteVariable":
                        if (string.IsNullOrEmpty(action.VariablePath) ||
                            !cfg.WritableVariables.Contains(action.VariablePath, StringComparer.OrdinalIgnoreCase))
                        {
                            outcome = "Rejected (variable not in write allow-list)";
                            break;
                        }
                        if (!dryRun)
                            _nodeManager.WriteVariable(action.VariablePath, action.Value ?? "");
                        outcome = dryRun ? "DryRun" : "Applied";
                        break;

                    case "ExecuteScript":
                        if (string.IsNullOrEmpty(action.ScriptName) ||
                            !cfg.AllowedScripts.Contains(action.ScriptName, StringComparer.OrdinalIgnoreCase))
                        {
                            outcome = "Rejected (script not in allow-list)";
                            break;
                        }
                        if (!dryRun)
                            _nodeManager.ScriptManager?.TriggerByName(action.ScriptName);
                        outcome = dryRun ? "DryRun" : "Applied";
                        break;

                    case "SendNotification":
                        if (!dryRun)
                            _nodeManager.NotificationService?.SendNotification(
                                title: $"AI Agent: {cfg.Name}", message: action.Message, severity: 500);
                        outcome = dryRun ? "DryRun" : "Applied";
                        break;

                    case "LogEvent":
                        outcome = "Applied"; // Logging always happens below regardless of dry-run.
                        break;

                    default:
                        outcome = $"Rejected (unknown action type '{action.Type}')";
                        break;
                }
            }
            catch (Exception ex)
            {
                outcome = $"Failed: {ex.Message}";
            }

            var detail = string.IsNullOrEmpty(action.Reasoning) ? "" : $" | reasoning: {action.Reasoning}";
            _nodeManager.EventLogger?.LogSystem(
                severity: outcome.StartsWith("Rejected") || outcome.StartsWith("Failed") ? "Warning" : "Info",
                source: $"AiAgent:{cfg.Name}",
                message: $"[{action.Type}] {outcome}{(string.IsNullOrEmpty(action.Message) ? "" : $" | {action.Message}")}{detail}");

            Log.Debug("AiAgent '{Agent}' action {Type} -> {Outcome}", cfg.Name, action.Type, outcome);
        }

        public void Dispose()
        {
            _cts.Cancel();
            foreach (var state in _states)
            {
                state.Timer?.Dispose();
                state.Gate.Dispose();
            }
            _cts.Dispose();
        }

        private sealed class AgentState
        {
            public AiAgentConfig Config { get; }
            public Timer? Timer { get; set; }
            public DateTime? LastAppliedAt { get; set; }
            public SemaphoreSlim Gate { get; } = new(1, 1);

            public AgentState(AiAgentConfig config) => Config = config;
        }
    }
}
