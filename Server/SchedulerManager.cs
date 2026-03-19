using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Manages time-based schedulers that execute commands when time slots become active/inactive.
    /// Runs a background timer that checks every 15 seconds which schedulers should be active.
    /// </summary>
    public class SchedulerManager : IDisposable
    {
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly List<SchedulerState> _schedulers = new();
        private Timer? _timer;
        private readonly CancellationTokenSource _cts = new();

        public SchedulerManager(SimpleFileServerNodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }

        public void Initialize(List<SchedulerConfig> schedulers)
        {
            foreach (var config in schedulers)
            {
                DiagnosticsCollector.Instance.Register("Scheduler", config.Name, config.Enabled);
                if (config.Enabled)
                {
                    _schedulers.Add(new SchedulerState(config));
                }
            }

            if (_schedulers.Count > 0)
            {
                _timer = new Timer(Tick, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(15));
                Log.Information("SchedulerManager initialized with {Count} scheduler(s).", _schedulers.Count);
            }
        }

        private void Tick(object? state)
        {
            if (_cts.IsCancellationRequested) return;

            var now = DateTime.Now;
            foreach (var sched in _schedulers)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    var shouldBeActive = IsTimeSlotActive(sched.Config, now);

                    if (shouldBeActive && !sched.IsActive)
                    {
                        // Activate
                        sched.IsActive = true;
                        ExecuteCommands(sched.Config.Commands, sched.Config.Name, "activate");
                        Log.Debug("Scheduler '{Name}' activated at {Time}.", sched.Config.Name, now);
                    }
                    else if (!shouldBeActive && sched.IsActive)
                    {
                        // Deactivate
                        sched.IsActive = false;
                        ExecuteCommands(sched.Config.DeactivateCommands, sched.Config.Name, "deactivate");
                        Log.Debug("Scheduler '{Name}' deactivated at {Time}.", sched.Config.Name, now);
                    }

                    sw.Stop();
                    DiagnosticsCollector.Instance.RecordCycle("Scheduler", sched.Config.Name, sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    Log.Error(ex, "Scheduler '{Name}' tick error.", sched.Config.Name);
                    DiagnosticsCollector.Instance.RecordCycle("Scheduler", sched.Config.Name, sw.Elapsed.TotalMilliseconds, error: ex.Message);
                }
            }
        }

        private bool IsTimeSlotActive(SchedulerConfig config, DateTime now)
        {
            var dayOfWeek = (int)now.DayOfWeek; // 0=Sunday
            var minuteOfDay = now.Hour * 60 + now.Minute;
            bool isWeekend = dayOfWeek == 0 || dayOfWeek == 6;
            bool isHoliday = IsHoliday(config, now);

            // Determine which slot list to use
            List<WeeklyTimeSlot> slots;

            if (isHoliday)
            {
                switch (config.HolidayMode)
                {
                    case "Off":
                        return false;
                    case "Weekend":
                        slots = config.WeekendMode == "Custom" ? config.WeekendSlots : config.WeeklySlots;
                        break;
                    default: // "Same"
                        slots = GetSlotsForDay(config, isWeekend);
                        break;
                }
            }
            else
            {
                slots = GetSlotsForDay(config, isWeekend);
            }

            // Check if current time falls in any slot for this day
            return slots.Any(s => s.DayOfWeek == dayOfWeek && minuteOfDay >= s.StartMinute && minuteOfDay < s.EndMinute);
        }

        private static List<WeeklyTimeSlot> GetSlotsForDay(SchedulerConfig config, bool isWeekend)
        {
            if (isWeekend)
            {
                return config.WeekendMode switch
                {
                    "Off" => [],
                    "Custom" => config.WeekendSlots,
                    _ => config.WeeklySlots // "Same"
                };
            }
            return config.WeeklySlots;
        }

        private static bool IsHoliday(SchedulerConfig config, DateTime date)
        {
            // Check built-in holidays
            if (!string.IsNullOrEmpty(config.HolidayLocale) &&
                HolidayCalendars.BuiltIn.TryGetValue(config.HolidayLocale, out var builtIn))
            {
                if (builtIn.Any(h => h.Month == date.Month && h.Day == date.Day &&
                    (h.Year == 0 || h.Year == date.Year)))
                    return true;
            }

            // Check custom holidays
            return config.CustomHolidays.Any(h => h.Month == date.Month && h.Day == date.Day &&
                (h.Year == 0 || h.Year == date.Year));
        }

        private void ExecuteCommands(List<SymbolCommand> commands, string schedulerName, string phase)
        {
            foreach (var cmd in commands)
            {
                try
                {
                    switch (cmd.Action)
                    {
                        case "SetVariable":
                        case "WriteVariable":
                            if (!string.IsNullOrEmpty(cmd.VariablePath))
                                _nodeManager.WriteVariable(cmd.VariablePath, cmd.Value ?? "");
                            break;

                        case "ResetVariable":
                            if (!string.IsNullOrEmpty(cmd.VariablePath))
                                _nodeManager.WriteVariable(cmd.VariablePath, "0");
                            break;

                        case "ToggleVariable":
                            if (!string.IsNullOrEmpty(cmd.VariablePath))
                            {
                                var current = _nodeManager.ReadVariable(cmd.VariablePath);
                                var currentStr = current?.ToString() ?? "";
                                var isTruthy = currentStr is "True" or "true" or "1";
                                _nodeManager.WriteVariable(cmd.VariablePath, isTruthy ? false : true);
                            }
                            break;

                        case "IncrementVariable":
                            if (!string.IsNullOrEmpty(cmd.VariablePath))
                            {
                                var incCurrent = _nodeManager.ReadVariable(cmd.VariablePath);
                                if (double.TryParse(incCurrent?.ToString(), System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out var incVal))
                                {
                                    var step = 1.0;
                                    if (!string.IsNullOrEmpty(cmd.Value) &&
                                        double.TryParse(cmd.Value, System.Globalization.NumberStyles.Any,
                                        System.Globalization.CultureInfo.InvariantCulture, out var s))
                                        step = s;
                                    _nodeManager.WriteVariable(cmd.VariablePath, incVal + step);
                                }
                            }
                            break;

                        case "DecrementVariable":
                            if (!string.IsNullOrEmpty(cmd.VariablePath))
                            {
                                var decCurrent = _nodeManager.ReadVariable(cmd.VariablePath);
                                if (double.TryParse(decCurrent?.ToString(), System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out var decVal))
                                {
                                    var step = 1.0;
                                    if (!string.IsNullOrEmpty(cmd.Value) &&
                                        double.TryParse(cmd.Value, System.Globalization.NumberStyles.Any,
                                        System.Globalization.CultureInfo.InvariantCulture, out var s))
                                        step = s;
                                    _nodeManager.WriteVariable(cmd.VariablePath, decVal - step);
                                }
                            }
                            break;

                        case "GenerateReport":
                            if (!string.IsNullOrEmpty(cmd.TargetReport))
                            {
                                _ = _nodeManager.GenerateReportAsync(cmd.TargetReport);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning("Scheduler '{Name}' {Phase} command '{Action}' failed: {Error}",
                        schedulerName, phase, cmd.Action, ex.Message);
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _timer?.Dispose();
        }

        private class SchedulerState
        {
            public SchedulerConfig Config { get; }
            public bool IsActive { get; set; }

            public SchedulerState(SchedulerConfig config)
            {
                Config = config;
            }
        }
    }
}
