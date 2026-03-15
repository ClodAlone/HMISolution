using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace ServerEditor.ViewModels
{
    public class LogEntry
    {
        public string Timestamp { get; set; } = "";
        public string Level { get; set; } = "";
        public string Message { get; set; } = "";
        
        public string Color => Level switch {
            "ERR" => "Red",
            "WRN" => "DarkOrange",
            "INF" => "Black",
            "DBG" => "Gray",
            "FTL" => "DarkRed",
            _ => "Black"
        };
    }

    public class LogViewerViewModel : ObservableObject
    {
        private string _logDirectory = "";
        private string _logPattern = "log-*.txt";
        private DispatcherTimer _timer;
        private long _lastPosition = 0;
        private string _currentFile = "";
        private const int MaxLogLines = 2000; 

        // Observable collection for logs
        public ObservableCollection<LogEntry> AllLogs { get; } = new();
        public ICollectionView LogsView { get; }

        private string _filterText = "";
        public string FilterText
        {
            get => _filterText;
            set 
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    OnPropertyChanged();
                    LogsView.Refresh();
                }
            }
        }

        public LogViewerViewModel()
        {
            LogsView = CollectionViewSource.GetDefaultView(AllLogs);
            LogsView.Filter = FilterLogs;
            
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => PollLogs();
            _timer.Start();
        }

        private bool FilterLogs(object item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            if (item is LogEntry entry)
            {
                return entry.Message.Contains(FilterText, StringComparison.OrdinalIgnoreCase) || 
                       entry.Level.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public void SetLogDirectory(string path, string pattern = "log-*.txt")
        {
            _logDirectory = path;
            _logPattern = pattern;
            _lastPosition = 0;
            _currentFile = "";
            AllLogs.Clear();
        }

        public void AddLog(string level, string message)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => 
            {
                AllLogs.Add(new LogEntry 
                { 
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"), 
                    Level = level, 
                    Message = message 
                });
                // Auto-scroll logic is in View code-behind listening to CollectionChanged
            });
        }

        private void PollLogs()
        {
            if (string.IsNullOrEmpty(_logDirectory) || !Directory.Exists(_logDirectory)) return;

            // Find latest log file
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
                    AllLogs.Clear();
                    Dispatcher.CurrentDispatcher.Invoke(() => 
                        AllLogs.Add(new LogEntry { Message = $"[Switched to log file: {Path.GetFileName(latest)}]", Level = "INF", Timestamp = DateTime.Now.ToString("HH:mm:ss") })
                    );
                }
            
                using (var fs = new FileStream(_currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length < _lastPosition) _lastPosition = 0; // File truncated

                    if (fs.Length > _lastPosition)
                    {
                        fs.Seek(_lastPosition, SeekOrigin.Begin);
                        using (var reader = new StreamReader(fs, Encoding.UTF8))
                        {
                            string newContent = reader.ReadToEnd();
                            ParseAndAddLogs(newContent);
                            _lastPosition = fs.Position;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignore errors
            }
        }

        private void ParseAndAddLogs(string text)
        {
            // Simple parsing: split by newlines, try to match regex
            // Regex for: 2024-03-21 10:15:30.123 +00:00 [INF] Message
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2})\s+\[(\w{3})\]\s+(.*)$");

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    var entry = new LogEntry
                    {
                        Timestamp = match.Groups[1].Value,
                        Level = match.Groups[2].Value,
                        Message = match.Groups[3].Value
                    };
                    AllLogs.Add(entry);
                }
                else
                {
                    // If parsing fails (or continuation line), append to last entry or add as raw
                     // "raw" maps to Black default
                     AllLogs.Add(new LogEntry { Message = line, Level = "RAW" });
                }
            }

            // Trim old logs
            while (AllLogs.Count > MaxLogLines)
            {
                AllLogs.RemoveAt(0);
            }
        }
    }
}
