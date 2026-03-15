using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua.Client;

namespace ServerEditor.Services
{
    public class OpcSessionManager
    {
        private static OpcSessionManager? _instance;
        public static OpcSessionManager Instance => _instance ??= new OpcSessionManager();

        private readonly List<Session> _sessions = new();
        private readonly object _lock = new();
        private readonly CancellationTokenSource _cts = new();
        private Task? _monitorTask;

        public event EventHandler<string>? SessionErrorMessage;

        private OpcSessionManager()
        {
            _monitorTask = Task.Run(MonitorLoop);
        }

        public void ReportError(string message)
        {
             SessionErrorMessage?.Invoke(this, message);
        }

        public void Register(Session session)
        {
            lock (_lock)
            {
                if (!_sessions.Contains(session))
                {
                    _sessions.Add(session);
                }
            }
        }

        public void Unregister(Session session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }

        private async Task MonitorLoop()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    List<Session> sessionsToCheck;
                    lock (_lock)
                    {
                        sessionsToCheck = _sessions.ToList();
                    }

                    foreach (var session in sessionsToCheck)
                    {
                        if (session == null || session.Disposed)
                        {
                            Unregister(session);
                            continue;
                        }

                        if (!session.Connected)
                        {
                            try
                            {
                                // Attempt reconnect
                                session.Reconnect();
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error reconnecting session: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in session monitor: {ex.Message}");
                }

                try
                {
                    await Task.Delay(5000, _cts.Token);
                }
                catch (OperationCanceledException) { break; }
            }
        }
    }
}