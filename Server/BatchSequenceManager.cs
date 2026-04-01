using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Runs ISA-88–inspired step-based sequences. Each sequence is a state machine
    /// that advances through steps based on transitions (conditions + timers).
    ///
    /// Published OPC variables per sequence (_Batch.{Name}.):
    ///   State          (String)  — Idle | Running | Held | Complete | Faulted | Aborted
    ///   CurrentStep    (String)  — active step name (empty when idle)
    ///   CurrentStepId  (String)  — active step ID
    ///   StepElapsed    (Double)  — seconds the current step has been active
    ///   TotalElapsed   (Double)  — seconds since the sequence started
    ///   StepIndex      (Int32)   — 0-based index of the current step
    ///   Message        (String)  — last status/error message
    /// </summary>
    public sealed class BatchSequenceManager : IDisposable
    {
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly List<SequenceRuntime> _sequences = new();
        private Timer? _timer;
        private readonly CancellationTokenSource _cts = new();

        private const string Folder = "_Batch";
        private const int TickMs = 500;

        public BatchSequenceManager(SimpleFileServerNodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }

        public void Initialize(List<BatchSequenceConfig> configs)
        {
            foreach (var cfg in configs)
            {
                DiagnosticsCollector.Instance.Register("Batch", cfg.Name, cfg.Enabled);
                if (!cfg.Enabled) continue;
                _sequences.Add(new SequenceRuntime(cfg));
            }

            if (_sequences.Count > 0)
            {
                _timer = new Timer(Tick, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(TickMs));
                Log.Information("BatchSequenceManager initialized with {Count} sequence(s).", _sequences.Count);
            }
        }

        private void Tick(object? state)
        {
            if (_cts.IsCancellationRequested) return;

            foreach (var seq in _sequences)
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    Evaluate(seq);
                    sw.Stop();
                    DiagnosticsCollector.Instance.RecordCycle("Batch", seq.Config.Name, sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    DiagnosticsCollector.Instance.RecordCycle("Batch", seq.Config.Name, sw.Elapsed.TotalMilliseconds, error: ex.Message);
                    Log.Error(ex, "Batch '{Name}' error: {Msg}", seq.Config.Name, ex.Message);
                }
            }
        }

        private void Evaluate(SequenceRuntime seq)
        {
            var prefix = $"{Folder}.{seq.Config.Name}";
            var now = DateTime.UtcNow;

            // --- Check external start/abort/hold signals ---
            if (seq.State == SeqState.Idle || seq.State == SeqState.Complete)
            {
                if (ReadBool(seq.Config.StartVariablePath))
                {
                    StartSequence(seq, now);
                }
            }

            if (seq.State == SeqState.Running || seq.State == SeqState.Held)
            {
                if (ReadBool(seq.Config.AbortVariablePath))
                {
                    seq.State = SeqState.Aborted;
                    seq.Message = "Aborted by operator";
                    ExecuteExitActions(seq);
                    Log.Information("Batch '{Name}' aborted.", seq.Config.Name);
                }
            }

            if (seq.State == SeqState.Running && !string.IsNullOrEmpty(seq.Config.HoldVariablePath))
            {
                if (ReadBool(seq.Config.HoldVariablePath))
                {
                    seq.State = SeqState.Held;
                    seq.Message = "Held";
                }
            }

            if (seq.State == SeqState.Held && !string.IsNullOrEmpty(seq.Config.HoldVariablePath))
            {
                if (!ReadBool(seq.Config.HoldVariablePath))
                {
                    seq.State = SeqState.Running;
                    seq.Message = $"Running — {seq.CurrentStepName}";
                }
            }

            // --- Evaluate transitions ---
            if (seq.State == SeqState.Running && seq.CurrentStepIndex >= 0)
            {
                var step = seq.Config.Steps[seq.CurrentStepIndex];
                var stepElapsed = (now - seq.StepStartUtc).TotalSeconds;

                // Check timeout
                if (step.TimeoutSeconds > 0 && stepElapsed >= step.TimeoutSeconds)
                {
                    // Find transition from this step, or fault
                    var trans = seq.Config.Transitions.FirstOrDefault(t =>
                        t.FromStepId == step.Id && string.IsNullOrEmpty(t.ConditionVariablePath));
                    if (trans != null)
                    {
                        AdvanceToStep(seq, trans.ToStepId, now, $"Timeout after {step.TimeoutSeconds}s");
                    }
                    else
                    {
                        // No timer transition — check if any condition transition exists
                        var anyTrans = seq.Config.Transitions.FirstOrDefault(t => t.FromStepId == step.Id);
                        if (anyTrans != null)
                        {
                            seq.State = SeqState.Faulted;
                            seq.Message = $"Step '{step.Name}' timed out ({step.TimeoutSeconds}s) — no transition fired";
                            ExecuteExitActions(seq);
                        }
                        else
                        {
                            // Last step, no transitions — sequence complete
                            CompleteSequence(seq);
                        }
                    }
                }
                else
                {
                    // Evaluate condition-based transitions
                    foreach (var trans in seq.Config.Transitions.Where(t => t.FromStepId == step.Id))
                    {
                        if (string.IsNullOrEmpty(trans.ConditionVariablePath))
                            continue; // Timer-only, handled above

                        // Check dwell time
                        if (trans.DelaySeconds > 0 && stepElapsed < trans.DelaySeconds)
                            continue;

                        if (EvaluateCondition(trans))
                        {
                            AdvanceToStep(seq, trans.ToStepId, now, $"Transition condition met");
                            break;
                        }
                    }
                }

                // Check if we're on the last step with no outgoing transitions and no timeout
                if (seq.State == SeqState.Running)
                {
                    var outgoing = seq.Config.Transitions.Any(t => t.FromStepId == step.Id);
                    if (!outgoing && step.TimeoutSeconds == 0)
                    {
                        // No transitions and no timeout: step completes immediately isn't right.
                        // This is a terminal step — stay here until explicitly advanced or aborted.
                    }
                }
            }

            // --- Publish OPC variables ---
            var totalElapsed = seq.State is SeqState.Running or SeqState.Held
                ? (now - seq.SequenceStartUtc).TotalSeconds : 0;
            var curStepElapsed = seq.State is SeqState.Running or SeqState.Held
                ? (now - seq.StepStartUtc).TotalSeconds : 0;

            WriteVar($"{prefix}.State", seq.State.ToString());
            WriteVar($"{prefix}.CurrentStep", seq.CurrentStepName);
            WriteVar($"{prefix}.CurrentStepId", seq.CurrentStepId);
            WriteVar($"{prefix}.StepElapsed", Math.Round(curStepElapsed, 1));
            WriteVar($"{prefix}.TotalElapsed", Math.Round(totalElapsed, 1));
            WriteVar($"{prefix}.StepIndex", seq.CurrentStepIndex);
            WriteVar($"{prefix}.Message", seq.Message);
        }

        private void StartSequence(SequenceRuntime seq, DateTime now)
        {
            if (seq.Config.Steps.Count == 0)
            {
                seq.Message = "No steps defined";
                return;
            }

            seq.State = SeqState.Running;
            seq.SequenceStartUtc = now;
            seq.CurrentStepIndex = 0;
            seq.StepStartUtc = now;
            var step = seq.Config.Steps[0];
            seq.CurrentStepId = step.Id;
            seq.CurrentStepName = step.Name;
            seq.Message = $"Running — {step.Name}";
            ExecuteEntryActions(seq);
            Log.Information("Batch '{Name}' started at step '{Step}'.", seq.Config.Name, step.Name);
        }

        private void AdvanceToStep(SequenceRuntime seq, string toStepId, DateTime now, string reason)
        {
            ExecuteExitActions(seq);

            var nextIdx = seq.Config.Steps.FindIndex(s => s.Id == toStepId);
            if (nextIdx < 0)
            {
                // End marker — sequence complete
                CompleteSequence(seq);
                return;
            }

            seq.CurrentStepIndex = nextIdx;
            seq.StepStartUtc = now;
            var step = seq.Config.Steps[nextIdx];
            seq.CurrentStepId = step.Id;
            seq.CurrentStepName = step.Name;
            seq.Message = $"Running — {step.Name}";
            ExecuteEntryActions(seq);
            Log.Debug("Batch '{Name}' advanced to '{Step}' ({Reason}).", seq.Config.Name, step.Name, reason);
        }

        private void CompleteSequence(SequenceRuntime seq)
        {
            ExecuteExitActions(seq);
            seq.State = seq.Config.AutoRestart ? SeqState.Idle : SeqState.Complete;
            seq.Message = "Complete";
            seq.CurrentStepId = "";
            seq.CurrentStepName = "";
            seq.CurrentStepIndex = -1;
            Log.Information("Batch '{Name}' completed.", seq.Config.Name);

            if (seq.Config.AutoRestart)
            {
                StartSequence(seq, DateTime.UtcNow);
            }
        }

        private void ExecuteEntryActions(SequenceRuntime seq)
        {
            if (seq.CurrentStepIndex < 0 || seq.CurrentStepIndex >= seq.Config.Steps.Count) return;
            var step = seq.Config.Steps[seq.CurrentStepIndex];
            foreach (var cmd in step.EntryActions)
                ExecuteCommand(cmd, seq.Config.Name, step.Name, "entry");
        }

        private void ExecuteExitActions(SequenceRuntime seq)
        {
            if (seq.CurrentStepIndex < 0 || seq.CurrentStepIndex >= seq.Config.Steps.Count) return;
            var step = seq.Config.Steps[seq.CurrentStepIndex];
            foreach (var cmd in step.ExitActions)
                ExecuteCommand(cmd, seq.Config.Name, step.Name, "exit");
        }

        private void ExecuteCommand(SymbolCommand cmd, string seqName, string stepName, string phase)
        {
            try
            {
                if (cmd.Action == "WriteVariable" && !string.IsNullOrEmpty(cmd.VariablePath))
                {
                    _nodeManager.WriteVariable(cmd.VariablePath, cmd.Value ?? "");
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Batch '{Seq}' step '{Step}' {Phase} command failed: {Err}",
                    seqName, stepName, phase, ex.Message);
            }
        }

        private bool EvaluateCondition(BatchTransition trans)
        {
            var val = _nodeManager.ReadVariable(trans.ConditionVariablePath);
            if (val == null) return false;

            var strVal = val.ToString() ?? "";
            return trans.ConditionOperator switch
            {
                "True" => IsTruthy(val),
                "False" => !IsTruthy(val),
                "==" => strVal.Equals(trans.ConditionValue, StringComparison.OrdinalIgnoreCase),
                "!=" => !strVal.Equals(trans.ConditionValue, StringComparison.OrdinalIgnoreCase),
                ">" => double.TryParse(strVal, out var d1) && double.TryParse(trans.ConditionValue, out var d2) && d1 > d2,
                "<" => double.TryParse(strVal, out var d3) && double.TryParse(trans.ConditionValue, out var d4) && d3 < d4,
                _ => false
            };
        }

        private bool ReadBool(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            try
            {
                var val = _nodeManager.ReadVariable(path);
                return IsTruthy(val);
            }
            catch { return false; }
        }

        private static bool IsTruthy(object? value)
        {
            if (value == null) return false;
            var s = value.ToString();
            return s is "True" or "true" or "1";
        }

        private void WriteVar(string path, object value)
        {
            try { _nodeManager.WriteVariable(path, value); }
            catch { /* variable may not exist on first tick */ }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _timer?.Dispose();
        }

        private enum SeqState { Idle, Running, Held, Complete, Faulted, Aborted }

        private sealed class SequenceRuntime
        {
            public BatchSequenceConfig Config { get; }
            public SeqState State { get; set; } = SeqState.Idle;
            public int CurrentStepIndex { get; set; } = -1;
            public string CurrentStepId { get; set; } = "";
            public string CurrentStepName { get; set; } = "";
            public DateTime SequenceStartUtc { get; set; }
            public DateTime StepStartUtc { get; set; }
            public string Message { get; set; } = "Idle";

            public SequenceRuntime(BatchSequenceConfig config) => Config = config;
        }
    }
}
