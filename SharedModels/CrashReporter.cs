// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedModels
{
    /// <summary>
    /// Structured crash/unhandled-exception report written as a JSON file.
    /// </summary>
    public class CrashReport
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public string Process { get; set; } = "";
        public int ProcessId { get; set; }
        public string MachineName { get; set; } = Environment.MachineName;
        public string OsDescription { get; set; } = RuntimeInformation.OSDescription;
        public string RuntimeVersion { get; set; } = RuntimeInformation.FrameworkDescription;
        public string ExceptionType { get; set; } = "";
        public string Message { get; set; } = "";
        public string StackTrace { get; set; } = "";
        public string? InnerException { get; set; }
        public string Source { get; set; } = ""; // "AppDomain", "TaskScheduler", "Blazor", "Manual"
        public long MemoryMB { get; set; }
        public int ThreadCount { get; set; }
    }

    [JsonSerializable(typeof(CrashReport))]
    [JsonSerializable(typeof(List<CrashReport>))]
    public partial class CrashReportJsonContext : JsonSerializerContext { }

    /// <summary>
    /// Captures unhandled exceptions and writes structured crash report JSON files.
    /// Call <see cref="Install"/> once during startup in each process.
    /// </summary>
    public static class CrashReporter
    {
        private static string _crashDir = "";
        private static string _processName = "";
        private static bool _installed;
        private static CrashEmailConfig? _emailConfig;

        /// <summary>
        /// Install global unhandled exception handlers and configure the crash reports directory.
        /// If the requested directory can't be created (e.g. read-only Program Files install),
        /// falls back to a per-user location under LocalApplicationData. Never throws.
        /// </summary>
        /// <param name="crashDirectory">Preferred directory to write crash report JSON files.</param>
        /// <param name="processName">Friendly process name (e.g. "Server", "Editor", "RuntimeViewer").</param>
        public static void Install(string crashDirectory, string processName)
        {
            if (_installed) return;
            _installed = true;

            _processName = processName;
            _crashDir = ResolveWritableCrashDir(crashDirectory, processName);

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    WriteCrashReport(ex, "AppDomain");
            };

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                WriteCrashReport(e.Exception, "TaskScheduler");
                // Don't observe — let it propagate normally
            };
        }

        private static string ResolveWritableCrashDir(string requested, string processName)
        {
            // 1. Try the requested directory
            if (TryEnsureWritable(requested)) return requested;

            // 2. Fallback: %LOCALAPPDATA%\HMI Solution\<processName>\crash_reports
            try
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (!string.IsNullOrEmpty(appData))
                {
                    var fb = Path.Combine(appData, "HMI Solution", processName, "crash_reports");
                    if (TryEnsureWritable(fb))
                    {
                        Debug.WriteLine($"CrashReporter: '{requested}' not writable, using '{fb}'");
                        return fb;
                    }
                }
            }
            catch { }

            // 3. Fallback: system temp
            try
            {
                var tmp = Path.Combine(Path.GetTempPath(), "HMI Solution", processName, "crash_reports");
                if (TryEnsureWritable(tmp)) return tmp;
            }
            catch { }

            // 4. Give up: return empty; WriteCrashReport will skip file output
            return "";
        }

        private static bool TryEnsureWritable(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir)) return false;
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var probe = Path.Combine(dir, ".write_test_" + Guid.NewGuid().ToString("N")[..8]);
                File.WriteAllText(probe, "");
                File.Delete(probe);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Configure SMTP email for automatic crash notifications.
        /// Call after <see cref="Install"/> once the project config is loaded.
        /// </summary>
        public static void ConfigureEmail(CrashEmailConfig? config)
        {
            _emailConfig = config;
        }

        /// <summary>
        /// Manually write a crash report for an exception (e.g. from a Blazor error boundary or catch block).
        /// </summary>
        public static void Report(Exception ex, string source = "Manual")
        {
            WriteCrashReport(ex, source);
        }

        /// <summary>
        /// Get the configured crash reports directory.
        /// </summary>
        public static string CrashDirectory => _crashDir;

        /// <summary>
        /// Get the current email configuration (for the UI).
        /// </summary>
        public static CrashEmailConfig? EmailConfig => _emailConfig;

        private static void WriteCrashReport(Exception ex, string source)
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var report = new CrashReport
                {
                    TimestampUtc = DateTime.UtcNow,
                    Process = _processName,
                    ProcessId = process.Id,
                    ExceptionType = ex.GetType().FullName ?? ex.GetType().Name,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace ?? "",
                    InnerException = ex.InnerException != null
                        ? $"{ex.InnerException.GetType().FullName}: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}"
                        : null,
                    Source = source,
                    MemoryMB = process.WorkingSet64 / (1024 * 1024),
                    ThreadCount = process.Threads.Count
                };

                var fileName = $"crash_{_processName}_{report.TimestampUtc:yyyyMMdd_HHmmss}_{report.Id}.json";
                var filePath = Path.Combine(_crashDir, fileName);

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(report, options);
                File.WriteAllText(filePath, json);

                // Auto-send email if configured
                if (_emailConfig is { Enabled: true, AutoSend: true })
                {
                    try { SendEmail(report, _emailConfig); } catch { }
                }
            }
            catch
            {
                // Last resort — never throw from the crash reporter itself
            }
        }

        /// <summary>
        /// Read all crash reports from the configured directory, sorted newest first.
        /// </summary>
        public static List<CrashReport> ReadAll(string? directory = null)
        {
            var dir = directory ?? _crashDir;
            var reports = new List<CrashReport>();
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                return reports;

            foreach (var file in Directory.GetFiles(dir, "crash_*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var report = JsonSerializer.Deserialize<CrashReport>(json);
                    if (report != null)
                        reports.Add(report);
                }
                catch
                {
                    // Skip corrupt files
                }
            }

            reports.Sort((a, b) => b.TimestampUtc.CompareTo(a.TimestampUtc));
            return reports;
        }

        /// <summary>
        /// Delete a crash report file by its Id.
        /// </summary>
        public static bool Delete(string id, string? directory = null)
        {
            var dir = directory ?? _crashDir;
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir)) return false;

            foreach (var file in Directory.GetFiles(dir, $"crash_*_{id}.json"))
            {
                try { File.Delete(file); return true; } catch { }
            }
            return false;
        }

        /// <summary>
        /// Delete all crash report files.
        /// </summary>
        public static int ClearAll(string? directory = null)
        {
            var dir = directory ?? _crashDir;
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir)) return 0;

            int count = 0;
            foreach (var file in Directory.GetFiles(dir, "crash_*.json"))
            {
                try { File.Delete(file); count++; } catch { }
            }
            return count;
        }

        // ─── Email ──────────────────────────────────────────────────────────

        /// <summary>
        /// Send a crash report email using the provided SMTP configuration.
        /// Returns null on success, or the error message on failure.
        /// </summary>
        public static string? SendEmail(CrashReport report, CrashEmailConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.SmtpHost) ||
                string.IsNullOrWhiteSpace(config.To) ||
                string.IsNullOrWhiteSpace(config.From))
                return "SMTP configuration incomplete (Host, From, and To are required).";

            try
            {
                var projectLabel = string.IsNullOrWhiteSpace(config.ProjectName)
                    ? ""
                    : $"[{config.ProjectName}] ";

                var subject = $"{projectLabel}Crash Report — {report.Process} — {report.ExceptionType.Split('.')[^1]} on {report.MachineName}";

                var body = $"""
                    CRASH REPORT
                    ════════════════════════════════════════════

                    Project:     {(string.IsNullOrWhiteSpace(config.ProjectName) ? "(not set)" : config.ProjectName)}
                    Machine:     {report.MachineName}
                    Time (UTC):  {report.TimestampUtc:yyyy-MM-dd HH:mm:ss}
                    Process:     {report.Process} (PID {report.ProcessId})
                    Source:      {report.Source}
                    Report ID:   {report.Id}

                    ENVIRONMENT
                    ────────────────────────────────────────────
                    OS:          {report.OsDescription}
                    Runtime:     {report.RuntimeVersion}
                    Memory:      {report.MemoryMB} MB
                    Threads:     {report.ThreadCount}

                    EXCEPTION
                    ────────────────────────────────────────────
                    Type:        {report.ExceptionType}
                    Message:     {report.Message}

                    STACK TRACE
                    ────────────────────────────────────────────
                    {report.StackTrace}
                    {(report.InnerException != null ? $"\nINNER EXCEPTION\n────────────────────────────────────────────\n{report.InnerException}" : "")}
                    """;

                using var msg = new MailMessage();
                msg.From = new MailAddress(config.From);
                foreach (var to in config.To.Split(',', ';'))
                {
                    var addr = to.Trim();
                    if (!string.IsNullOrEmpty(addr))
                        msg.To.Add(addr);
                }
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = false;

                using var smtp = new SmtpClient(config.SmtpHost, config.SmtpPort);
                smtp.EnableSsl = config.UseSsl;
                smtp.Timeout = 15000; // 15 seconds
                if (!string.IsNullOrWhiteSpace(config.Username))
                    smtp.Credentials = new NetworkCredential(config.Username, config.Password);
                smtp.Send(msg);

                return null; // success
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
