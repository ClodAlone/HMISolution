using System.Text.Json;

namespace ServerEditorWeb.Services;

public class AiService
{
    public string[] AvailableEngines { get; } = ["OpenAI", "Gemini"];

    public async Task<(string newJson, string comments)> AskAsync(string engine, string prompt, string currentJson, string? referenceJson = null, string? referenceFileName = null)
    {
        return engine switch
        {
            "OpenAI" => await CallOpenAiAsync(prompt, currentJson, referenceJson, referenceFileName),
            "Gemini" => await CallGeminiAsync(prompt, currentJson, referenceJson, referenceFileName),
            _ => ("", $"Engine '{engine}' is not supported.")
        };
    }

    /// <summary>
    /// Lists available sample project files from a directory.
    /// Returns tuples of (displayName, fullPath, groupName).
    /// Scans for nodes.json in each sample folder, plus any .json files in imports/ subdirectories.
    /// </summary>
    public List<(string Name, string Path, string Group)> GetSampleFiles(string samplesDirectory)
    {
        var results = new List<(string, string, string)>();
        if (!Directory.Exists(samplesDirectory))
            return results;

        foreach (var dir in Directory.GetDirectories(samplesDirectory).OrderBy(d => d))
        {
            var dirName = System.IO.Path.GetFileName(dir);

            // Main nodes.json for the sample
            var nodesFile = System.IO.Path.Combine(dir, "nodes.json");
            if (File.Exists(nodesFile))
                results.Add((dirName, nodesFile, "Sample Projects"));

            // Import files in imports/ subdirectory
            var importsDir = System.IO.Path.Combine(dir, "imports");
            if (Directory.Exists(importsDir))
            {
                foreach (var importFile in Directory.GetFiles(importsDir, "*.json").OrderBy(f => f))
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(importFile);
                    var displayName = $"{dirName} / {fileName}";
                    results.Add((displayName, importFile, "Import Files"));
                }
            }
        }
        return results;
    }

    /// <summary>
    /// Reads and returns the content of a reference file.
    /// </summary>
    public string? ReadReferenceFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return null;
        return File.ReadAllText(filePath);
    }

    private static string BuildPrompt(string userPrompt, string oldJson, string? referenceJson, string? referenceFileName)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("You are a JSON editor helper for an OPC UA Server configuration. The user wants to modify the following JSON:\n\n");
        sb.Append(oldJson);

        if (!string.IsNullOrEmpty(referenceJson))
        {
            sb.Append("\n\n--- REFERENCE FILE");
            if (!string.IsNullOrEmpty(referenceFileName))
                sb.Append($" ({referenceFileName})");
            sb.Append(" ---\nThe following is a reference project that the user wants to import from. Extract variables, folders, screens, scripts, PLC programs, recipes, or other elements as instructed by the user:\n\n");
            sb.Append(referenceJson);
            sb.Append("\n--- END REFERENCE FILE ---\n");
        }

        sb.Append("\n\nUser Instruction: ");
        sb.Append(userPrompt);
        sb.Append("\n\nReturn the FULL valid JSON content as the result, incorporating the requested changes. Do not return just the difference or a snippet. Do not use markdown blocks around the JSON.\n\nAt the end of the JSON, print the delimiter '///COMMENTS///', followed by a summary of what has been changed.");
        return sb.ToString();
    }

    private async Task<(string, string)> CallOpenAiAsync(string userPrompt, string oldJson, string? referenceJson = null, string? referenceFileName = null)
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set OPENAI_API_KEY environment variable to use OpenAI.");

        string endpoint = "https://api.openai.com/v1/chat/completions";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var prompt = BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);

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

    private async Task<(string, string)> CallGeminiAsync(string userPrompt, string oldJson, string? referenceJson = null, string? referenceFileName = null)
    {
        string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set GEMINI_API_KEY environment variable to use Gemini.");

        string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

        using var client = new HttpClient();

        var prompt = BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);

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
