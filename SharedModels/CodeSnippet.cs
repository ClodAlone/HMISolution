// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Text.Json.Serialization;

namespace SharedModels;

/// <summary>
/// Represents a reusable code snippet for scripts (C#, VB.NET) and PLC programs (ST, IL, LD).
/// Snippets can be built-in or user-defined, with support for categories and descriptions.
/// </summary>
public class CodeSnippet
{
    /// <summary>
    /// Unique identifier for the snippet.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Display name of the snippet (e.g., "For Loop", "Try-Catch", "Read Variable").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short description of what the snippet does.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The actual code content of the snippet.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Programming language: "CSharp", "VbNet", "PlcSt", "PlcIl", "PlcLd"
    /// </summary>
    public string Language { get; set; } = "CSharp";

    /// <summary>
    /// Category for organizing snippets (e.g., "Loops", "Conditionals", "IO", "Math", "Debugging")
    /// </summary>
    public string Category { get; set; } = "General";

    /// <summary>
    /// Whether this is a built-in (system) snippet that cannot be deleted by users.
    /// </summary>
    public bool IsBuiltIn { get; set; } = false;

    /// <summary>
    /// Author/creator of the snippet (for documentation).
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Tags for searching/filtering (e.g., "loop", "iteration", "performance")
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last modified timestamp in UTC.
    /// </summary>
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional: placeholder variables in the code (e.g., ${VARIABLE_NAME})
    /// for intelligent insertion or documentation.
    /// </summary>
    [JsonPropertyName("placeholders")]
    public Dictionary<string, string> Placeholders { get; set; } = new();

    /// <summary>
    /// Usage count (for sorting by popularity).
    /// </summary>
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// Optional: example of how to use this snippet.
    /// </summary>
    public string? ExampleUsage { get; set; }

    /// <summary>
    /// Optional: related snippet IDs that work well together.
    /// </summary>
    [JsonPropertyName("relatedSnippets")]
    public List<string> RelatedSnippets { get; set; } = new();

    /// <summary>
    /// Creates a copy of this snippet with a new ID (for cloning user-defined versions).
    /// </summary>
    public CodeSnippet Clone()
    {
        return new CodeSnippet
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"{Name} (Copy)",
            Description = Description,
            Code = Code,
            Language = Language,
            Category = Category,
            IsBuiltIn = false,
            Author = Author,
            Tags = new List<string>(Tags),
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            Placeholders = new Dictionary<string, string>(Placeholders),
            ExampleUsage = ExampleUsage,
            RelatedSnippets = new List<string>(RelatedSnippets)
        };
    }

    /// <summary>
    /// Gets a human-readable description for UI display.
    /// </summary>
    public string GetDisplayText() => $"{Name} ({Language}/{Category})";
}
