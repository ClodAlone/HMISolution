using System.Globalization;

namespace RuntimeViewer.Shared.Services;

public sealed record HistoricalSelection(DateTime StartUtc, DateTime EndUtc, int IntervalSeconds, DateTime? CursorUtc);

public sealed class HistoricalCursorGroup
{
    private readonly Dictionary<string, HistoricalSelection> _selections = new(StringComparer.Ordinal);
    public event Action<string, HistoricalSelection>? Changed;
    public HistoricalSelection? Get(string group) => _selections.GetValueOrDefault(group);
    public void Publish(string group, HistoricalSelection selection)
    {
        if (string.IsNullOrWhiteSpace(group)) return;
        _selections[group] = selection;
        Changed?.Invoke(group, selection);
    }
}

public static class HistoricalSampling
{
    public const int MaxSamples = 2000;

    public static List<DateTime> Timeline(DateTime startUtc, DateTime endUtc, int intervalSeconds)
    {
        if (startUtc.Kind != DateTimeKind.Utc || endUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Historical ranges must use UTC timestamps.");
        if (startUtc >= endUtc) throw new ArgumentException("Start must precede end.");
        if (intervalSeconds <= 0) throw new ArgumentException("Sampling interval must be positive.");
        var count = (endUtc.Ticks - startUtc.Ticks) / TimeSpan.FromSeconds(intervalSeconds).Ticks + 1;
        if (count > MaxSamples)
            throw new ArgumentException($"Range contains {count:N0} samples. Increase the interval or shorten the range (maximum {MaxSamples}).");
        return Enumerable.Range(0, (int)count).Select(i => startUtc.AddSeconds((long)i * intervalSeconds)).ToList();
    }

    // Consume ordered archive changes once; a predecessor seeds otherwise unchanged values.
    public static HdaSeries Sample(string path, IEnumerable<HdaDataPoint> changes, IReadOnlyList<DateTime> timeline)
    {
        var result = new HdaSeries { VariableName = path };
        using var source = changes.GetEnumerator();
        var hasNext = source.MoveNext();
        HdaDataPoint? last = null;
        foreach (var time in timeline)
        {
            while (hasNext && source.Current.Time <= time)
            {
                last = source.Current;
                hasNext = source.MoveNext();
            }
            result.Points.Add(new HdaDataPoint { Time = time, Value = last?.Value, StringValue = last?.StringValue });
        }
        var values = result.Points.Where(p => p.Value.HasValue && double.IsFinite(p.Value.Value)).Select(p => p.Value!.Value).ToList();
        result.MinValue = values.Count == 0 ? 0 : values.Min();
        result.MaxValue = values.Count == 0 ? 1 : values.Max();
        return result;
    }

    public static string Format(HdaDataPoint point) => point.Value?.ToString("G6", CultureInfo.InvariantCulture) ?? point.StringValue ?? "—";
}
