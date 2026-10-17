using ServerEditorWeb.Models;

namespace ServerEditorWeb.Services;

public sealed class JsonEditorSession : IDisposable
{
    public bool IsInitialized { get; set; }
    public bool ResourceOnly { get; set; } = true;
    public bool JsonNeedsRefresh { get; set; }
    public string ResourceJson { get; set; } = "";
    public ProjectNode? ResourceTarget { get; set; }
    public string Message { get; set; } = "";
    public bool Success { get; set; }
    public bool AiExpanded { get; set; }
    public string SelectedEngine { get; set; } = "OpenAI";
    public string Prompt { get; set; } = "";
    public string AiComments { get; set; } = "";
    public string AiError { get; set; } = "";
    public bool IsProcessing { get; set; }
    public List<(string Name, string Path, string Group)> SampleFiles { get; set; } = new();
    public string SelectedSamplePath { get; set; } = "";
    public string CustomFilePath { get; set; } = "";
    public string ReferencePreview { get; set; } = "";
    public bool ShowPreview { get; set; }
    public bool ShowBrowse { get; set; }
    public string BrowseInput { get; set; } = "";
    public string OllamaModel { get; set; } = "mistral";
    public string OllamaUrl { get; set; } = "";
    public string[] OllamaModels { get; set; } = [];
    public string TrainFolder { get; set; } = Path.Combine("C:", "Work", "samples");
    public CancellationTokenSource? StreamCts { get; set; }
    public int StreamCharCount { get; set; }
    public string StreamTail { get; set; } = "";
    public TimeSpan StreamElapsed { get; set; }
    public SimpleJsonDiff.Stats DiffStats { get; set; }
    public DateTime LastUiUpdateUtc { get; set; }
    public string? CopyStatusMsg { get; set; }
    public int ReferenceCharCount { get; set; }
    public bool ChunkedMode { get; set; }
    public int ChunkChars { get; set; } = 20_000;
    public int CurrentChunk { get; set; }
    public int TotalChunks { get; set; }
    public string ScopePath { get; set; } = "";
    public List<AiService.ScopeEntry> FolderScopes { get; set; } = new();
    public bool AdditionsOnly { get; set; } = true;
    public bool ShowSuggestions { get; set; }
    public List<string> RecentPrompts { get; set; } = new();
    public string SelectedTaskId { get; set; } = "";

    public event Func<Task>? Changed;
    private bool _disposed;

    public async Task NotifyChangedAsync()
    {
        if (_disposed || Changed is not { } handlers) return;
        foreach (Func<Task> handler in handlers.GetInvocationList())
            await handler();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        StreamCts?.Cancel();
        Changed = null;
    }
}
