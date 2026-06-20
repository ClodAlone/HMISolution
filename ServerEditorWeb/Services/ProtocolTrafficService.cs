using System.Text.RegularExpressions;

namespace ServerEditorWeb.Services;

// ─── Model ────────────────────────────────────────────────────────────────────

public enum TrafficProtocol { Modbus, MQTT, OpcUa, REST, S7, TCP, EtherNetIP, KNX, Other }
public enum TrafficDirection { Rx, Tx, Internal }

public class TrafficFrame
{
    public int Sequence { get; init; }
    public DateTime Timestamp { get; init; }
    public TrafficProtocol Protocol { get; init; }
    public TrafficDirection Direction { get; init; }
    public string Level { get; init; } = "INF";
    public string Source { get; init; } = "";
    public string Summary { get; init; } = "";
    public string Raw { get; init; } = "";

    public string ProtocolCss => Protocol switch
    {
        TrafficProtocol.Modbus    => "ptm-proto-modbus",
        TrafficProtocol.MQTT      => "ptm-proto-mqtt",
        TrafficProtocol.OpcUa     => "ptm-proto-opcua",
        TrafficProtocol.REST      => "ptm-proto-rest",
        TrafficProtocol.S7        => "ptm-proto-s7",
        TrafficProtocol.TCP       => "ptm-proto-tcp",
        TrafficProtocol.EtherNetIP=> "ptm-proto-enip",
        TrafficProtocol.KNX       => "ptm-proto-knx",
        _                         => "ptm-proto-other"
    };

    public string LevelCss => Level switch
    {
        "ERR" or "FTL" => "ptm-level-error",
        "WRN"          => "ptm-level-warn",
        "DBG"          => "ptm-level-debug",
        _              => "ptm-level-info"
    };

    public string DirectionIcon => Direction switch
    {
        TrafficDirection.Tx       => "↑",
        TrafficDirection.Rx       => "↓",
        _                         => "•"
    };
}

// ─── Service ──────────────────────────────────────────────────────────────────

/// <summary>
/// Tails the server's Serilog log file and extracts driver/protocol-related
/// lines to produce a live stream of <see cref="TrafficFrame"/> entries.
/// Follows the same tail-polling approach as <see cref="LogViewerService"/>.
/// </summary>
public class ProtocolTrafficService : IDisposable
{
    private const int MaxFrames = 5000;

    // Serilog line regex — same format as LogViewerService
    private static readonly Regex _serilogRx = new(
        @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2})\s+\[(\w{3})\]\s+(.*)$",
        RegexOptions.Compiled);

    // Keywords that identify a log line as protocol-related
    private static readonly (string keyword, TrafficProtocol proto, TrafficDirection dir)[] _rules =
    [
        ("Modbus",     TrafficProtocol.Modbus,     TrafficDirection.Internal),
        ("modbus",     TrafficProtocol.Modbus,     TrafficDirection.Internal),
        ("MQTT",       TrafficProtocol.MQTT,        TrafficDirection.Internal),
        ("mqtt",       TrafficProtocol.MQTT,        TrafficDirection.Internal),
        ("publish",    TrafficProtocol.MQTT,        TrafficDirection.Tx),
        ("subscribe",  TrafficProtocol.MQTT,        TrafficDirection.Rx),
        ("OPC UA",     TrafficProtocol.OpcUa,       TrafficDirection.Internal),
        ("OpcUa",      TrafficProtocol.OpcUa,       TrafficDirection.Internal),
        ("OpcUA",      TrafficProtocol.OpcUa,       TrafficDirection.Internal),
        ("OPC-UA",     TrafficProtocol.OpcUa,       TrafficDirection.Internal),
        ("REST",       TrafficProtocol.REST,         TrafficDirection.Internal),
        ("HttpClient", TrafficProtocol.REST,         TrafficDirection.Tx),
        ("GET ",       TrafficProtocol.REST,         TrafficDirection.Tx),
        ("POST ",      TrafficProtocol.REST,         TrafficDirection.Tx),
        ("S7 ",        TrafficProtocol.S7,           TrafficDirection.Internal),
        ("Siemens",    TrafficProtocol.S7,           TrafficDirection.Internal),
        ("EtherNet/IP",TrafficProtocol.EtherNetIP,  TrafficDirection.Internal),
        ("EtherNetIP", TrafficProtocol.EtherNetIP,  TrafficDirection.Internal),
        ("KNX",        TrafficProtocol.KNX,          TrafficDirection.Internal),
        ("TCP",        TrafficProtocol.TCP,          TrafficDirection.Internal),
        (" Read ",     TrafficProtocol.Other,        TrafficDirection.Rx),
        (" Write ",    TrafficProtocol.Other,        TrafficDirection.Tx),
        ("poll",       TrafficProtocol.Other,        TrafficDirection.Rx),
        ("Poll",       TrafficProtocol.Other,        TrafficDirection.Rx),
        ("frame",      TrafficProtocol.Other,        TrafficDirection.Rx),
        ("Frame",      TrafficProtocol.Other,        TrafficDirection.Rx),
        ("Driver",     TrafficProtocol.Other,        TrafficDirection.Internal),
        ("driver",     TrafficProtocol.Other,        TrafficDirection.Internal),
        ("response",   TrafficProtocol.Other,        TrafficDirection.Rx),
        ("Response",   TrafficProtocol.Other,        TrafficDirection.Rx),
        ("request",    TrafficProtocol.Other,        TrafficDirection.Tx),
        ("Request",    TrafficProtocol.Other,        TrafficDirection.Tx),
    ];

    private readonly object _lock = new();
    private readonly List<TrafficFrame> _frames = new();

    private Timer? _timer;
    private string _logDirectory = "";
    private string _logPattern = "log-*.txt";
    private string _currentFile = "";
    private long _lastPosition;
    private int _sequence;

    public bool IsPaused { get; private set; }
    public bool IsRunning => _timer != null;

    /// <summary>All captured frames — lock on <see cref="Lock"/> when enumerating.</summary>
    public IReadOnlyList<TrafficFrame> Frames { get { lock (_lock) return _frames.ToList(); } }

    public event Action? StateChanged;

    // ─── Control ──────────────────────────────────────────────────────────────

    public void Start(string logDirectory, string logPattern = "log-*.txt")
    {
        _logDirectory = logDirectory;
        _logPattern   = logPattern;
        _lastPosition = 0;
        _currentFile  = "";
        _timer?.Dispose();
        _timer = new Timer(Poll, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public void Pause()  { IsPaused = true;  StateChanged?.Invoke(); }
    public void Resume() { IsPaused = false; StateChanged?.Invoke(); }

    public void Clear()
    {
        lock (_lock) { _frames.Clear(); }
        StateChanged?.Invoke();
    }

    // ─── Polling ──────────────────────────────────────────────────────────────

    private void Poll(object? _)
    {
        if (IsPaused) return;
        if (string.IsNullOrEmpty(_logDirectory) || !Directory.Exists(_logDirectory)) return;

        try
        {
            var files = Directory.GetFiles(_logDirectory, _logPattern)
                                 .OrderByDescending(f => f)
                                 .ToList();
            if (files.Count == 0) return;

            var latest = files[0];
            if (latest != _currentFile)
            {
                _currentFile  = latest;
                _lastPosition = 0;
            }

            using var fs = new FileStream(_currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (fs.Length < _lastPosition) _lastPosition = 0;
            if (fs.Length <= _lastPosition) return;

            fs.Seek(_lastPosition, SeekOrigin.Begin);
            using var reader = new StreamReader(fs, System.Text.Encoding.UTF8);
            var text = reader.ReadToEnd();
            _lastPosition = fs.Position;

            ParseAndAdd(text);
            StateChanged?.Invoke();
        }
        catch { /* swallow file-access races */ }
    }

    private void ParseAndAdd(string text)
    {
        var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var newFrames = new List<TrafficFrame>();

        foreach (var line in lines)
        {
            var frame = TryClassify(line);
            if (frame != null) newFrames.Add(frame);
        }

        if (newFrames.Count == 0) return;

        lock (_lock)
        {
            _frames.AddRange(newFrames);
            // Keep cap
            while (_frames.Count > MaxFrames)
                _frames.RemoveAt(0);
        }
    }

    private TrafficFrame? TryClassify(string line)
    {
        // Only parse Serilog-formatted lines
        var m = _serilogRx.Match(line);
        string ts, level, message;

        if (m.Success)
        {
            ts      = m.Groups[1].Value;
            level   = m.Groups[2].Value;
            message = m.Groups[3].Value;
        }
        else
        {
            // Not a structured log line; skip
            return null;
        }

        // Must contain at least one protocol keyword
        TrafficProtocol? proto = null;
        TrafficDirection dir   = TrafficDirection.Internal;

        foreach (var (keyword, p, d) in _rules)
        {
            if (message.Contains(keyword, StringComparison.Ordinal))
            {
                proto = p;
                dir   = d;
                break;
            }
        }

        if (proto is null) return null;

        // Derive direction from keywords if more specific hints are available
        if (message.Contains("read", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("response", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("received", StringComparison.OrdinalIgnoreCase))
            dir = TrafficDirection.Rx;
        else if (message.Contains("write", StringComparison.OrdinalIgnoreCase) ||
                 message.Contains("publish", StringComparison.OrdinalIgnoreCase) ||
                 message.Contains("request", StringComparison.OrdinalIgnoreCase) ||
                 message.Contains("send", StringComparison.OrdinalIgnoreCase))
            dir = TrafficDirection.Tx;

        // Extract source tag (first word that looks like a subsystem name)
        var sourceMatch = Regex.Match(message, @"\[([A-Za-z0-9_.]+)\]");
        var source = sourceMatch.Success ? sourceMatch.Groups[1].Value : proto.ToString();

        return new TrafficFrame
        {
            Sequence  = Interlocked.Increment(ref _sequence),
            Timestamp = DateTime.TryParse(ts, out var dt) ? dt : DateTime.Now,
            Protocol  = proto.Value,
            Direction = dir,
            Level     = level,
            Source    = source,
            Summary   = message.Length > 200 ? message[..200] + "…" : message,
            Raw       = line
        };
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
