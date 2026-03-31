using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Manages recipe storage and runtime operations.
/// Each RecipeConfig gets a SQLite database with a "recipes" table and per-variable columns.
/// Provides Load, Save, Activate, and Delete operations accessible via OPC variables.
/// </summary>
public class RecipeManager : IDisposable
{
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly List<RecipeRuntime> _runtimes = new();
    private readonly string _configDir;

    public RecipeManager(SimpleFileServerNodeManager nodeManager, string configPath)
    {
        _nodeManager = nodeManager;
        _configDir = Path.GetDirectoryName(Path.GetFullPath(configPath)) ?? ".";
    }

    public void Initialize(List<RecipeConfig> recipes)
    {
        foreach (var recipe in recipes)
        {
            DiagnosticsCollector.Instance.Register("Recipe", recipe.Name, recipe.Enabled);
            if (!recipe.Enabled) continue;

            try
            {
                var rt = new RecipeRuntime(recipe, _nodeManager, _configDir);
                rt.Initialize();
                _runtimes.Add(rt);
                Log.Information("Recipe '{Name}' initialized with {Count} variables", recipe.Name, recipe.Variables.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize recipe '{Name}'", recipe.Name);
                DiagnosticsCollector.Instance.SetStatus("Recipe", recipe.Name, "Error", ex.Message);
            }
        }
    }

    /// <summary>Dispatch a recipe command to the appropriate runtime.</summary>
    public void Execute(string recipeName, string action, string targetRecipeName)
    {
        var rt = _runtimes.FirstOrDefault(r => r.Name == recipeName);
        if (rt == null)
        {
            Log.Warning("Recipe runtime '{Name}' not found for action '{Action}'", recipeName, action);
            return;
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            switch (action)
            {
                case "Load": rt.LoadRecipe(targetRecipeName); break;
                case "Save": rt.SaveRecipe(targetRecipeName); break;
                case "Activate": rt.ActivateRecipe(targetRecipeName); break;
                case "Delete": rt.DeleteRecipe(targetRecipeName); break;
                default:
                    Log.Warning("Recipe '{Name}': unknown action '{Action}'", recipeName, action);
                    break;
            }
            sw.Stop();
            DiagnosticsCollector.Instance.RecordCycle("Recipe", recipeName, sw.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            DiagnosticsCollector.Instance.RecordCycle("Recipe", recipeName, sw.Elapsed.TotalMilliseconds, error: ex.Message);
            throw;
        }
    }

    /// <summary>Returns the names of all configured recipes.</summary>
    public List<string> GetRecipeNames()
    {
        return _runtimes.Select(r => r.Name).ToList();
    }

    public void Dispose()
    {
        foreach (var rt in _runtimes) rt.Dispose();
        _runtimes.Clear();
    }
}

/// <summary>
/// Runtime state for a single recipe configuration. Manages the SQLite database
/// and exposes Load/Save/Activate/Delete operations.
/// </summary>
internal class RecipeRuntime : IDisposable
{
    private readonly RecipeConfig _config;
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly string _dbPath;
    private Microsoft.Data.Sqlite.SqliteConnection? _connection;

    public string Name => _config.Name;

    public RecipeRuntime(RecipeConfig config, SimpleFileServerNodeManager nodeManager, string configDir)
    {
        _config = config;
        _nodeManager = nodeManager;
        _dbPath = Path.Combine(configDir, $"recipe_{config.Name}.db");
    }

    public void Initialize()
    {
        var dir = Path.GetDirectoryName(_dbPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        _connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_dbPath}");
        _connection.Open();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS recipes (
            name TEXT PRIMARY KEY,
            data TEXT NOT NULL,
            created TEXT NOT NULL,
            modified TEXT NOT NULL
        )";
        cmd.ExecuteNonQuery();
    }

    public void LoadRecipe(string name)
    {
        if (_connection == null) return;

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT data FROM recipes WHERE name = @name";
        cmd.Parameters.AddWithValue("@name", name);

        var result = cmd.ExecuteScalar();
        if (result is string json)
        {
            var values = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (values != null)
            {
                foreach (var kvp in values)
                {
                    try { _nodeManager.WriteVariable(kvp.Key, kvp.Value); }
                    catch (Exception ex) { Log.Warning("Recipe load: failed to write {Var}: {Error}", kvp.Key, ex.Message); }
                }
            }
            Log.Information("Recipe '{Recipe}' loaded recipe '{Name}'", _config.Name, name);
        }
        else
        {
            Log.Warning("Recipe '{Recipe}': recipe '{Name}' not found", _config.Name, name);
        }
    }

    public void SaveRecipe(string name)
    {
        if (_connection == null) return;

        var values = new Dictionary<string, string>();
        foreach (var v in _config.Variables)
        {
            var val = _nodeManager.ReadVariable(v.VariablePath);
            values[v.VariablePath] = val?.ToString() ?? "";
        }

        var json = System.Text.Json.JsonSerializer.Serialize(values);
        var now = DateTime.UtcNow.ToString("o");

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO recipes (name, data, created, modified) VALUES (@name, @data, @now, @now)
            ON CONFLICT(name) DO UPDATE SET data = @data, modified = @now";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@data", json);
        cmd.Parameters.AddWithValue("@now", now);
        cmd.ExecuteNonQuery();

        Log.Information("Recipe '{Recipe}' saved recipe '{Name}'", _config.Name, name);
    }

    public void ActivateRecipe(string name) => LoadRecipe(name);

    public void DeleteRecipe(string name)
    {
        if (_connection == null) return;

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM recipes WHERE name = @name";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.ExecuteNonQuery();

        Log.Information("Recipe '{Recipe}' deleted recipe '{Name}'", _config.Name, name);
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
        _connection = null;
    }
}
