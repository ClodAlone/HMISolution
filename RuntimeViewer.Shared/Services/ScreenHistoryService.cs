// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Tracks the sequence of screens (and non-screen routes, such as "/ar") the operator has
/// navigated through during the current session, enabling a "Go Back" command/button that
/// returns to wherever the user came from — including back into AR mode after a NavigateScreen
/// command took them away from it. Scoped per circuit/session, mirroring <see cref="ScreenPreloadService"/>.
/// </summary>
public class ScreenHistoryService
{
    /// <summary>Maximum number of entries retained to bound memory use in long-running sessions.</summary>
    private const int MaxDepth = 50;

    private readonly List<string> _history = new();

    /// <summary>Route currently considered "active" by the last call to <see cref="Push"/>.</summary>
    public string? CurrentRoute => _history.Count > 0 ? _history[^1] : null;

    /// <summary>True when there is somewhere to go back to.</summary>
    public bool CanGoBack => _history.Count > 1;

    /// <summary>
    /// Records a navigation to <paramref name="route"/> (e.g. "/screen/MotorDetail" or "/ar").
    /// No-op if it's identical to the current top entry (avoids duplicate consecutive entries
    /// from re-renders or redundant navigations).
    /// </summary>
    public void Push(string route)
    {
        if (string.IsNullOrEmpty(route)) return;
        if (_history.Count > 0 && string.Equals(_history[^1], route, StringComparison.OrdinalIgnoreCase))
            return;

        _history.Add(route);
        if (_history.Count > MaxDepth)
            _history.RemoveAt(0);
    }

    /// <summary>
    /// Pops the current route and returns the previous one to navigate back to, or null if
    /// there is no history to go back to. Does not itself perform navigation — callers should
    /// pass the result to NavigationManager.NavigateTo.
    /// </summary>
    public string? PopToPrevious()
    {
        if (!CanGoBack) return null;
        _history.RemoveAt(_history.Count - 1);
        return _history[^1];
    }

    /// <summary>Clears all recorded history (e.g. on logout or project reload).</summary>
    public void Clear() => _history.Clear();
}
