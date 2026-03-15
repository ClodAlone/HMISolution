using System.Text.Json;

namespace ServerEditorWeb.Services;

public class AiService
{
    public string[] AvailableEngines { get; } = ["OpenAI", "Gemini"];

    public async Task<(string newJson, string comments)> AskAsync(string engine, string prompt, string currentJson)
    {
        return engine switch
        {
            "OpenAI" => await CallOpenAiAsync(prompt, currentJson),
            "Gemini" => await CallGeminiAsync(prompt, currentJson),
            _ => ("", $"Engine '{engine}' is not supported.")
        };
    }

    private async Task<(string, string)> CallOpenAiAsync(string userPrompt, string oldJson)
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set OPENAI_API_KEY environment variable to use OpenAI.");

        string endpoint = "https://api.openai.com/v1/chat/completions";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var prompt = $"You are a JSON editor helper for an OPC UA Server configuration. The user wants to modify the following JSON:\n\n{oldJson}\n\nUser Instruction: {userPrompt}\n\nReturn the FULL valid JSON content as the result, incorporating the requested changes. Do not return just the difference or a snippet. Do not use markdown blocks around the JSON.\n\nAt the end of the JSON, print the delimiter '///COMMENTS///', followed by a summary of what has been changed.";

        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.1
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        return ParseAiResponse(contentText);
    }

    private async Task<(string, string)> CallGeminiAsync(string userPrompt, string oldJson)
    {
        string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set GEMINI_API_KEY environment variable to use Gemini.");

        string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

        using var client = new HttpClient();

        var prompt = $"You are a JSON editor helper for an OPC UA Server configuration. The user wants to modify the following JSON:\n\n{oldJson}\n\nUser Instruction: {userPrompt}\n\nReturn the FULL valid JSON content as the result, incorporating the requested changes. Do not return just the difference or a snippet. Do not use markdown blocks around the JSON.\n\nAt the end of the JSON, print the delimiter '///COMMENTS///', followed by a summary of what has been changed.";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

        return ParseAiResponse(contentText);
    }

    private static (string newJson, string comments) ParseAiResponse(string? contentText)
    {
        if (contentText == null)
            return ("", "");

        var parts = contentText.Split(["///COMMENTS///"], StringSplitOptions.None);
        string newJson = parts[0].Replace("```json", "").Replace("```", "").Trim();
        string comments = parts.Length > 1 ? parts[1].Trim() : "";
        return (newJson, comments);
    }
}
