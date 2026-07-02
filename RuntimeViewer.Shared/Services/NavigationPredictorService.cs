// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Learns screen navigation patterns using a simple Markov chain (transition counts)
/// and predicts the most likely next screen. The model is persisted per-project between sessions.
/// </summary>
public class NavigationPredictorService
{
    private static readonly string ModelDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                     "RuntimeViewer", "nav-predictor");

    private Dictionary<string, Dictionary<string, int>> _transitions = new(StringComparer.OrdinalIgnoreCase);
    private string? _currentScreen;
    private string? _modelPath;

    public string? PredictedNextScreen { get; private set; }
    public double PredictionConfidence { get; private set; }
    private const double MinConfidence = 0.3;
    private const int MinSamples = 3;

    /// <summary>
    /// Binds this predictor to a specific project. Must be called before RecordNavigation.
    /// The config path is hashed to produce a unique per-project file name.
    /// </summary>
    public void SetProject(string configPath)
    {
        if (string.IsNullOrEmpty(configPath)) return;
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
            Path.GetFullPath(configPath).ToLowerInvariant())))[..16];
        _modelPath = Path.Combine(ModelDir, $"nav-{hash}.json");
        _transitions = new(StringComparer.OrdinalIgnoreCase);
        _currentScreen = null;
        PredictedNextScreen = null;
        PredictionConfidence = 0;
        Load();
    }
    /// <summary>Records a navigation event and updates the prediction.</summary>
    public void RecordNavigation(string screenName)
    {
        if (string.IsNullOrEmpty(screenName) || _modelPath == null) return;
        if (!string.IsNullOrEmpty(_currentScreen) &&
            !_currentScreen.Equals(screenName, StringComparison.OrdinalIgnoreCase))
        {
            if (!_transitions.TryGetValue(_currentScreen, out var targets))
            {
                targets = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                _transitions[_currentScreen] = targets;
            }
            targets.TryGetValue(screenName, out var count);
            targets[screenName] = count + 1;
            Save();
        }
        _currentScreen = screenName;
        UpdatePrediction();
    }
    /// <summary>Returns the predicted next screen name(s), ordered by probability.</summary>
    public List<string> GetPredictedScreenNames()
    {
        if (string.IsNullOrEmpty(PredictedNextScreen) || PredictionConfidence < MinConfidence)
            return [];
        return [PredictedNextScreen];
    }

    /// <summary>Returns variable paths the predicted next screen would need.</summary>
    public List<string> GetPredictedVariablePaths(IReadOnlyList<SharedModels.ScreenConfig> screens)
    {
        if (string.IsNullOrEmpty(PredictedNextScreen) || PredictionConfidence < MinConfidence)
            return [];
        var screen = screens.FirstOrDefault(s =>
            s.Name.Equals(PredictedNextScreen, StringComparison.OrdinalIgnoreCase));
        if (screen == null) return [];
        var paths = new List<string>();
        foreach (var sym in screen.Symbols)
        {
            if (!string.IsNullOrEmpty(sym.VariablePath)) paths.Add(sym.VariablePath);
            foreach (var cmd in sym.Commands)
                if (!string.IsNullOrEmpty(cmd.VariablePath)) paths.Add(cmd.VariablePath);
            if (sym.Type == "trend")
                foreach (var pen in sym.TrendPens)
                    if (!string.IsNullOrEmpty(pen.VariablePath)) paths.Add(pen.VariablePath);
        }
        return paths.Distinct().ToList();
    }
    private void UpdatePrediction()
    {
        PredictedNextScreen = null;
        PredictionConfidence = 0;
        if (string.IsNullOrEmpty(_currentScreen)) return;
        if (!_transitions.TryGetValue(_currentScreen, out var targets) || targets.Count == 0) return;
        int total = targets.Values.Sum();
        if (total < MinSamples) return;
        var best = targets.MaxBy(kv => kv.Value);
        if (best.Value > 0)
        {
            PredictedNextScreen = best.Key;
            PredictionConfidence = (double)best.Value / total;
        }
    }

    private void Load()
    {
        try
        {
            if (_modelPath != null && File.Exists(_modelPath))
            {
                var json = File.ReadAllText(_modelPath);
                var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json);
                if (data != null)
                    _transitions = new Dictionary<string, Dictionary<string, int>>(data, StringComparer.OrdinalIgnoreCase);
            }
        }
        catch { }
    }

    private void Save()
    {
        try
        {
            if (_modelPath == null) return;
            if (!Directory.Exists(ModelDir)) Directory.CreateDirectory(ModelDir);
            var json = JsonSerializer.Serialize(_transitions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_modelPath, json);
        }
        catch { }
    }
}