// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SharedModels
{
    /// <summary>
    /// Shared HTTP client for calling AI text-generation engines (OpenAI, Gemini, Claude, Ollama)
    /// using a <see cref="NaturalLanguageQueryConfig"/> for engine selection and credentials.
    /// Used by natural-language query features and AI-generated report sections so the
    /// calling code and API key configuration only need to live in one place.
    /// </summary>
    public static class AiEngineClient
    {
        /// <summary>Send a prompt to the configured AI engine and return the generated text response.</summary>
        public static Task<string> AskAsync(string prompt, NaturalLanguageQueryConfig cfg, CancellationToken ct = default)
        {
            return cfg.Engine switch
            {
                "OpenAI" => CallOpenAiAsync(prompt, cfg, ct),
                "Gemini" => CallGeminiAsync(prompt, cfg, ct),
                "Claude" => CallClaudeAsync(prompt, cfg, ct),
                _ => CallOllamaAsync(prompt, cfg, ct)
            };
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
}
