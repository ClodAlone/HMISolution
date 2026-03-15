using System.Text;
using System.Text.RegularExpressions;

namespace ServerEditorWeb.Services;

public class LogEntry
{
    public string Timestamp { get; set; } = "";
    public string Level { get; set; } = "";
    public string Message { get; set; } = "";

    public string CssClass => Level switch
    {
        "ERR" => "log-error",
        "WRN" => "log-warning",
        "INF" => "log-info",
        "DBG" => "log-debug",
        "FTL" => "log-fatal",
        _ => "log-raw"
    };
}

public class LogViewerService : IDisposable
{
    private string _logDirectory = "";
    private string _logPattern = "log-*.txt";
    private Timer? _timer;
    private long _lastPosition;
    private string _currentFile = "";
    private const int MaxLogLines = 2000;

    public List<LogEntry> Logs { get; } = new();
    public string FilterText { get; set; } = "";

    public event Action? LogsChanged;

    private readonly object _lock = new();

    public List<LogEntry> FilteredLogs
    {
        get
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(FilterText))
                    return Logs.ToList();

                return Logs.Where(e =>
                    e.Message.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                    e.Level.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }

    public void SetLogDirectory(string path, string pattern = "log-*.txt")
    {
        _logDirectory = path;
        _logPattern = pattern;
        _lastPosition = 0;
        _currentFile = "";
        lock (_lock) { Logs.Clear(); }
        LogsChanged?.Invoke();
    }

    public void AddLog(string level, string message)
    {
        lock (_lock)
        {
            Logs.Add(new LogEntry
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"),
                Level = level,
                Message = message
            });
            TrimLogs();
        }
        LogsChanged?.Invoke();
    }

    public void Start()
    {
        _timer = new Timer(PollLogs, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void PollLogs(object? state)
    {
        if (string.IsNullOrEmpty(_logDirectory) || !Directory.Exists(_logDirectory)) return;

        try
        {
            var files = Directory.GetFiles(_logDirectory, _logPattern)
                                 .OrderByDescending(f => f)
                                 .ToList();

            if (files.Count == 0) return;

            var latest = files.First();

            if (latest != _currentFile)
            {
                _currentFile = latest;
                _lastPosition = 0;
                lock (_lock)
                {
                    Logs.Clear();
                    Logs.Add(new LogEntry
                    {
                        Message = $"[Switched to log file: {Path.GetFileName(latest)}]",
                        Level = "INF",
                        Timestamp = DateTime.Now.ToString("HH:mm:ss")
                    });
                }
            }

            using var fs = new FileStream(_currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (fs.Length < _lastPosition) _lastPosition = 0;

            if (fs.Length > _lastPosition)
            {
                fs.Seek(_lastPosition, SeekOrigin.Begin);
                using var reader = new StreamReader(fs, Encoding.UTF8);
                string newContent = reader.ReadToEnd();
                ParseAndAddLogs(newContent);
                _lastPosition = fs.Position;
            }

            LogsChanged?.Invoke();
        }
        catch { }
    }

    private void ParseAndAddLogs(string text)
    {
        var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2})\s+\[(\w{3})\]\s+(.*)$");

        lock (_lock)
        {
            foreach (var line in lines)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    Logs.Add(new LogEntry
                    {
                        Timestamp = match.Groups[1].Value,
                        Level = match.Groups[2].Value,
                        Message = match.Groups[3].Value
                    });
                }
                else
                {
                    Logs.Add(new LogEntry { Message = line, Level = "RAW" });
                }
            }

            TrimLogs();
        }
    }

    private void TrimLogs()
    {
        while (Logs.Count > MaxLogLines)
            Logs.RemoveAt(0);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
