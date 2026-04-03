using System.Text.Json;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Learns screen navigation patterns using a simple Markov chain (transition counts)
/// and predicts the most likely next screen. The model is persisted between sessions.
/// </summary>
public class NavigationPredictorService
{
    private static readonly string ModelDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                     "RuntimeViewer");
    private static readonly string ModelPath = Path.Combine(ModelDir, "nav-predictor.json");

    // Transition counts: _transitions[fromScreen][toScreen] = count
    private Dictionary<string, Dictionary<string, int>> _transitions = new(StringComparer.OrdinalIgnoreCase);
    private string? _currentScreen;

    /// <summary>
    /// The predicted next screen name (null if no prediction is available).
    /// Updated after each <see cref="RecordNavigation"/> call.
    /// </summary>
    public string? PredictedNextScreen { get; private set; }

    /// <summary>
    /// Confidence of the prediction (0.0–1.0). Represents the fraction of transitions
    /// from the current screen that go to the predicted screen.
    /// </summary>
    public double PredictionConfidence { get; private set; }

    /// <summary>Minimum confidence to act on a prediction.</summary>
    private const double MinConfidence = 0.3;

    /// <summary>Minimum total transitions from a screen before predictions are made.</summary>
    private const int MinSamples = 3;

    public NavigationPredictorService()
    {
        Load();
    }

    /// <summary>
    /// Records a navigation event and updates the prediction for the next screen.
    /// </summary>
    public void RecordNavigation(string screenName)
    {
        if (string.IsNullOrEmpty(screenName))
            return;

        // Record transition from previous screen
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

        // Update prediction
        UpdatePrediction();
    }

    /// <summary>
    /// Returns the list of variable paths that the predicted next screen would need,
    /// or an empty list if no confident prediction exists.
    /// </summary>
    public List<string> GetPredictedVariablePaths(IReadOnlyList<SharedModels.ScreenConfig> screens)
    {
        if (string.IsNullOrEmpty(PredictedNextScreen) || PredictionConfidence < MinConfidence)
            return [];

        var screen = screens.FirstOrDefault(s =>
            s.Name.Equals(PredictedNextScreen, StringComparison.OrdinalIgnoreCase));

        if (screen == null)
            return [];

        var paths = new List<string>();
        foreach (var sym in screen.Symbols)
        {
            if (!string.IsNullOrEmpty(sym.VariablePath))
                paths.Add(sym.VariablePath);

            foreach (var cmd in sym.Commands)
            {
                if (!string.IsNullOrEmpty(cmd.VariablePath))
                    paths.Add(cmd.VariablePath);
            }

            if (sym.Type == "trend")
            {
                foreach (var pen in sym.TrendPens)
                {
                    if (!string.IsNullOrEmpty(pen.VariablePath))
                        paths.Add(pen.VariablePath);
                }
            }
        }

        return paths.Distinct().ToList();
    }

    private void UpdatePrediction()
    {
        PredictedNextScreen = null;
        PredictionConfidence = 0;

        if (string.IsNullOrEmpty(_currentScreen))
            return;

        if (!_transitions.TryGetValue(_currentScreen, out var targets) || targets.Count == 0)
            return;

        int totalTransitions = targets.Values.Sum();
        if (totalTransitions < MinSamples)
            return;

        // Find the most likely next screen
        var best = targets.MaxBy(kv => kv.Value);
        if (best.Value > 0)
        {
            PredictedNextScreen = best.Key;
            PredictionConfidence = (double)best.Value / totalTransitions;
        }
    }

    private void Load()
    {
        try
        {
            if (File.Exists(ModelPath))
            {
                var json = File.ReadAllText(ModelPath);
                var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json);
                if (data != null)
                    _transitions = new Dictionary<string, Dictionary<string, int>>(data, StringComparer.OrdinalIgnoreCase);
            }
        }
        catch { /* ignore corrupt model — will rebuild */ }
    }

    private void Save()
    {
        try
        {
            if (!Directory.Exists(ModelDir))
                Directory.CreateDirectory(ModelDir);

            var json = JsonSerializer.Serialize(_transitions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ModelPath, json);
        }
        catch { /* best effort */ }
    }
}
