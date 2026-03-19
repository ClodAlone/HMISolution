using System.Text.Json;

namespace ServerEditorWeb.Services;

public class AiService
{
    public string[] AvailableEngines { get; } = ["OpenAI", "Gemini", "Ollama"];

    // Ollama training state
    private bool _isTraining;
    private string _trainingStatus = "";
    private string _ollamaModel = "mistral";
    private string _ollamaBaseUrl = "http://localhost:11434";
    private string _trainingContext = "";

    public bool IsTraining => _isTraining;
    public string TrainingStatus => _trainingStatus;
    public string OllamaModel { get => _ollamaModel; set => _ollamaModel = value; }
    public string OllamaBaseUrl { get => _ollamaBaseUrl; set => _ollamaBaseUrl = value; }

    public async Task<(string newJson, string comments)> AskAsync(string engine, string prompt, string currentJson, string? referenceJson = null, string? referenceFileName = null)
    {
        return engine switch
        {
            "OpenAI" => await CallOpenAiAsync(prompt, currentJson, referenceJson, referenceFileName),
            "Gemini" => await CallGeminiAsync(prompt, currentJson, referenceJson, referenceFileName),
            "Ollama" => await CallOllamaAsync(prompt, currentJson, referenceJson, referenceFileName),
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

            // Import files in imports/ subdirectory (any format: .json, .xml, .csv, etc.)
            var importsDir = System.IO.Path.Combine(dir, "imports");
            if (Directory.Exists(importsDir))
            {
                foreach (var importFile in Directory.GetFiles(importsDir).OrderBy(f => f))
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(importFile);
                    var ext = System.IO.Path.GetExtension(importFile).ToLowerInvariant();
                    var displayName = $"{dirName} / {fileName}{ext}";
                    results.Add((displayName, importFile, "Driver Import Files"));
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
            sb.Append(" ---\n");

            // Detect file type and provide format-specific parsing instructions
            var ext = System.IO.Path.GetExtension(referenceFileName ?? "").ToLowerInvariant();
            sb.Append(ext switch
            {
                ".xml" => """
The reference file is a PLC program export (e.g. Siemens TIA Portal SimaticML DB export).
Parse the XML to extract variables. For each <Member> inside a <SW.Blocks.GlobalDB>, create a variable with:
- Folder path derived from the DB name (e.g. DB5_Filler → "Filler" folder)
- Name from the Member's Name attribute
- Type mapped from Siemens types: Real→Double, Int→Int32, DInt→Int32, Bool→Boolean, String[...]→String
- Access: "ReadWrite" for Remanence="Retain", "Read" for "NonRetain"
- Value from <StartValue>
- S7 driver config: { "IpAddress": from the XML comment or "192.168.0.10", "Rack": 0, "Slot": 1, "Address": "DB<Number>.DBB0" (calculate offsets), "PollTime": 1000 }
For <Tag> entries in <SW.Tags.PlcTagTable>, map Address (%I=input, %Q=output) to S7 addresses.
Also check the XML comments at the top for PLC IP address, Rack, and Slot information.

""",
                ".csv" when (referenceJson ?? "").Contains("Group Address") => """
The reference file is a KNX ETS group address export (CSV with semicolons).
Parse each row to extract variables. For each group address, create a variable with:
- Folder path derived from the Building/Floor/Room columns
- Name derived from the Description column (clean it into a valid identifier)
- Type mapped from KNX DPT: DPST-1-x→Boolean, DPST-5-x→Double (0-100), DPST-9-x→Double, DPST-12-x→Int32
- Access: "ReadWrite" for switches/dimmers/setpoints, "Read" for sensors
- KNX driver config: { "ConnectionType": "Tunneling", "GatewayAddress": "192.168.1.100", "GroupAddress": "<the group address>", "DPT": "<the datapoint type id>", "PollTime": 5000 }

""",
                ".csv" => """
The reference file is a Modbus register map (CSV).
Lines starting with # or ## are comments — read them for IP/port info. Parse data rows to extract variables. For each register, create a variable with:
- Folder path derived from the logical grouping in the Name (e.g. AHU1_SupplyAirTemp → "AHU1" folder)
- Name from the Name column (cleaned)
- Type mapped from DataType: Float32→Double, UInt16→Int32, Boolean→Boolean
- Access from R/W column: "R"→"Read", "RW"→"ReadWrite"
- Modbus driver config: { "IpAddress": from comments or "10.0.1.50", "Port": 502, "SlaveId": <SlaveID>, "RegisterType": "<RegisterType>", "Address": <Address>, "DataType": "<DataType>", "ScaleFactor": <ScaleFactor>, "PollTime": 1000 }
- Unit from the Unit column can be used for alarm/display purposes

""",
                _ => "The following is a reference file that the user wants to import from. Extract variables, folders, screens, scripts, PLC programs, recipes, or other elements as instructed by the user:\n\n"
            });

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


    // ── Ollama ──────────────────────────────────────────────────────────

    private async Task<(string, string)> CallOllamaAsync(string userPrompt, string oldJson, string? referenceJson = null, string? referenceFileName = null)
    {
        var prompt = BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);

        // Prepend training context if available
        if (!string.IsNullOrEmpty(_trainingContext))
            prompt = _trainingContext + "\n\n" + prompt;

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };

        var requestBody = new
        {
            model = _ollamaModel,
            prompt,
            stream = false,
            options = new { temperature = 0.1 }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync($"{_ollamaBaseUrl}/api/generate", content);
        }
        catch (HttpRequestException ex)
        {
            return ("", $"Cannot reach Ollama at {_ollamaBaseUrl}. Make sure Ollama is running. ({ex.Message})");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            return ("", $"Ollama returned {response.StatusCode}: {errorBody}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentText = doc.RootElement.GetProperty("response").GetString();

        return ParseAiResponse(contentText);
    }

    /// <summary>
    /// Scans a folder for sample projects (nodes.json) and PLC import files (.xml, .csv, .json),
    /// reads their content, and builds a context string that is prepended to every Ollama prompt.
    /// This teaches the local model about HMI project structure and PLC export formats.
    /// </summary>
    public async Task TrainOllamaAsync(string samplesFolder, Action<string>? onProgress = null)
    {
        if (_isTraining) return;
        _isTraining = true;
        _trainingStatus = "Scanning folder...";
        onProgress?.Invoke(_trainingStatus);

        try
        {
            if (!Directory.Exists(samplesFolder))
            {
                _trainingStatus = $"Folder not found: {samplesFolder}";
                onProgress?.Invoke(_trainingStatus);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== TRAINING CONTEXT: HMI/SCADA Project Examples ===");
            sb.AppendLine("You have been trained on the following real HMI/SCADA project files and PLC export formats.");
            sb.AppendLine("Use this knowledge to produce accurate JSON configurations when the user asks.\n");

            int fileCount = 0;

            // Collect all relevant files
            var allFiles = new List<(string path, string category)>();
            foreach (var dir in Directory.GetDirectories(samplesFolder).OrderBy(d => d))
            {
                var nodesFile = Path.Combine(dir, "nodes.json");
                if (File.Exists(nodesFile))
                    allFiles.Add((nodesFile, "Sample Project"));

                var importsDir = Path.Combine(dir, "imports");
                if (Directory.Exists(importsDir))
                {
                    foreach (var f in Directory.GetFiles(importsDir).OrderBy(x => x))
                        allFiles.Add((f, "PLC Import File"));
                }
            }

            foreach (var (filePath, category) in allFiles)
            {
                fileCount++;
                var dirName = Path.GetFileName(Path.GetDirectoryName(filePath) ?? "");
                var fileName = Path.GetFileName(filePath);
                _trainingStatus = $"Reading {fileCount}/{allFiles.Count}: {dirName}/{fileName}";
                onProgress?.Invoke(_trainingStatus);

                try
                {
                    var text = await File.ReadAllTextAsync(filePath);
                    // Limit individual file size to keep context manageable
                    if (text.Length > 15000)
                        text = text[..15000] + "\n... (truncated)";

                    sb.AppendLine($"--- {category}: {dirName}/{fileName} ---");
                    sb.AppendLine(text);
                    sb.AppendLine($"--- END {dirName}/{fileName} ---\n");
                }
                catch
                {
                    // Skip unreadable files
                }
            }

            sb.AppendLine("=== END TRAINING CONTEXT ===");
            sb.AppendLine("Use the patterns, variable structures, driver configs, folder hierarchies, screen layouts, alarm configs, and PLC export formats shown above as reference when generating JSON.\n");

            _trainingContext = sb.ToString();

            // Verify Ollama is reachable by making a small test call
            _trainingStatus = "Verifying Ollama connection...";
            onProgress?.Invoke(_trainingStatus);

            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            try
            {
                var testBody = new
                {
                    model = _ollamaModel,
                    prompt = "Reply with OK.",
                    stream = false,
                    options = new { num_predict = 5 }
                };
                var json = new StringContent(JsonSerializer.Serialize(testBody), System.Text.Encoding.UTF8, "application/json");
                var resp = await client.PostAsync($"{_ollamaBaseUrl}/api/generate", json);
                if (!resp.IsSuccessStatusCode)
                {
                    _trainingStatus = $"Trained with {fileCount} files. Warning: Ollama returned {resp.StatusCode} — check model '{_ollamaModel}' is pulled.";
                    onProgress?.Invoke(_trainingStatus);
                    return;
                }
            }
            catch (Exception ex)
            {
                _trainingStatus = $"Trained with {fileCount} files. Warning: Cannot reach Ollama at {_ollamaBaseUrl} ({ex.Message}).";
                onProgress?.Invoke(_trainingStatus);
                return;
            }

            _trainingStatus = $"Ready — {fileCount} files loaded ({_trainingContext.Length / 1024} KB context). Model: {_ollamaModel}";
            onProgress?.Invoke(_trainingStatus);
        }
        catch (Exception ex)
        {
            _trainingStatus = $"Training error: {ex.Message}";
            onProgress?.Invoke(_trainingStatus);
        }
        finally
        {
            _isTraining = false;
        }
    }

    /// <summary>
    /// Returns the list of available Ollama models from the local instance.
    /// </summary>
    public async Task<string[]> GetOllamaModelsAsync()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var response = await client.GetAsync($"{_ollamaBaseUrl}/api/tags");
            if (!response.IsSuccessStatusCode) return [];

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("models")
                .EnumerateArray()
                .Select(m => m.GetProperty("name").GetString() ?? "")
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(n => n)
                .ToArray();
        }
        catch
        {
            return [];
        }
    }

}