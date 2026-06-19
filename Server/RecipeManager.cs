using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Manages recipe storage and runtime operations.
    /// Each RecipeConfig gets a SQLite database with a "recipes" table and per-variable columns.
    /// Provides Load, Save, Activate, and Delete operations accessible via OPC variables.
    /// 
    /// OPC variables created per recipe (under Recipe.{Name}):
    ///   Recipe.{Name}.Load        — Write a recipe name to load values into OPC variables
    ///   Recipe.{Name}.Save        — Write a recipe name to save current OPC variable values
    ///   Recipe.{Name}.Activate    — Write a recipe name to set it as the active recipe
    ///   Recipe.{Name}.Delete      — Write a recipe name to delete it from the database
    ///   Recipe.{Name}.ActiveName  — Read the currently active recipe name
    ///   Recipe.{Name}.RecipeList  — Read a comma-separated list of stored recipe names
    ///   Recipe.{Name}.LastStatus  — Read the result of the last operation
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

        public void Dispose()
        {
            foreach (var rt in _runtimes) rt.Dispose();
            _runtimes.Clear();
        }

        /// <summary>
        /// Activates a named recipe across the first runtime that contains it.
        /// Convenience method for AutomationRuleManager and other callers.
        /// </summary>
        public void ActivateRecipe(string recipeName)
        {
            // Try each runtime until one succeeds (recipe may exist in any of them)
            foreach (var rt in _runtimes)
            {
                try
                {
                    rt.ActivateRecipe(recipeName);
                    return;
                }
                catch { }
            }
            Log.Warning("RecipeManager.ActivateRecipe: recipe '{Name}' not found in any runtime.", recipeName);
        }
    }

    internal class RecipeRuntime : IDisposable
    {
        private readonly RecipeConfig _config;
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly string _configDir;
        private SqliteConnection? _connection;
        private string _activeName = "";
        private string _lastStatus = "";

        public string Name => _config.Name;

        public RecipeRuntime(RecipeConfig config, SimpleFileServerNodeManager nodeManager, string configDir)
        {
            _config = config;
            _nodeManager = nodeManager;
            _configDir = configDir;
        }

        public void Initialize()
        {
            // Resolve DB path
            var dbPath = _config.DbPath;
            if (string.IsNullOrWhiteSpace(dbPath))
                dbPath = Path.Combine("recipes", ResourceFileManager.SanitizeFileName(_config.Name) + ".db");
            if (!Path.IsPathRooted(dbPath))
                dbPath = Path.Combine(_configDir, "Data", dbPath);

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            _connection = new SqliteConnection($"Data Source={dbPath}");
            _connection.Open();

            using var walCmd = _connection.CreateCommand();
            walCmd.CommandText = "PRAGMA journal_mode=WAL;";
            walCmd.ExecuteNonQuery();

            EnsureSchema();
        }

        private void EnsureSchema()
        {
            if (_connection == null) return;

            // Create the recipes table with a name column and one column per variable index
            // Column names: recipe_name TEXT PRIMARY KEY, v_0 REAL, v_1 REAL, ...
            var columns = string.Join(", ",
                _config.Variables.Select(v => $"v_{v.Index} TEXT"));

            var createSql = $@"CREATE TABLE IF NOT EXISTS recipes (
                recipe_name TEXT PRIMARY KEY
                {(columns.Length > 0 ? ", " + columns : "")});";

            using var cmd = _connection.CreateCommand();
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();

            // Add any missing columns (for when variables are added later)
            var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using var pragmaCmd = _connection.CreateCommand();
            pragmaCmd.CommandText = "PRAGMA table_info(recipes);";
            using var reader = pragmaCmd.ExecuteReader();
            while (reader.Read())
                existingColumns.Add(reader.GetString(1));

            foreach (var v in _config.Variables)
            {
                var colName = $"v_{v.Index}";
                if (!existingColumns.Contains(colName))
                {
                    using var alterCmd = _connection.CreateCommand();
                    alterCmd.CommandText = $"ALTER TABLE recipes ADD COLUMN {colName} TEXT;";
                    alterCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Save current OPC variable values to a named recipe.</summary>
        public void SaveRecipe(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName) || _connection == null) return;

            try
            {
                var colNames = string.Join(", ", _config.Variables.Select(v => $"v_{v.Index}"));
                var paramNames = string.Join(", ", _config.Variables.Select(v => $"@v_{v.Index}"));

                var sql = $@"INSERT OR REPLACE INTO recipes (recipe_name{(colNames.Length > 0 ? ", " + colNames : "")})
                             VALUES (@name{(paramNames.Length > 0 ? ", " + paramNames : "")});";

                using var cmd = _connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@name", recipeName);

                foreach (var v in _config.Variables)
                {
                    object? val = null;
                    try { val = _nodeManager.ReadVariable(v.VariablePath); }
                    catch { /* variable may not exist */ }
                    cmd.Parameters.AddWithValue($"@v_{v.Index}",
                        val != null ? Convert.ToString(val, CultureInfo.InvariantCulture) ?? "" : "");
                }

                cmd.ExecuteNonQuery();
                _lastStatus = $"Saved recipe '{recipeName}'";
                UpdateStatusVariables();
                Log.Information("Recipe '{Recipe}': saved '{Name}'", _config.Name, recipeName);
            }
            catch (Exception ex)
            {
                _lastStatus = $"Error saving '{recipeName}': {ex.Message}";
                UpdateStatusVariables();
                Log.Error(ex, "Recipe '{Recipe}': error saving '{Name}'", _config.Name, recipeName);
            }
        }

        /// <summary>Load a named recipe from the database and write values to OPC variables.</summary>
        public void LoadRecipe(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName) || _connection == null) return;

            try
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM recipes WHERE recipe_name = @name;";
                cmd.Parameters.AddWithValue("@name", recipeName);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _lastStatus = $"Recipe '{recipeName}' not found";
                    UpdateStatusVariables();
                    return;
                }

                foreach (var v in _config.Variables)
                {
                    var colName = $"v_{v.Index}";
                    try
                    {
                        int ordinal = reader.GetOrdinal(colName);
                        if (!reader.IsDBNull(ordinal))
                        {
                            var strVal = reader.GetString(ordinal);
                            // Try to convert to the target variable's type
                            WriteValueToVariable(v.VariablePath, strVal);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("Recipe '{Recipe}': failed to load variable '{Var}': {Msg}",
                            _config.Name, v.VariablePath, ex.Message);
                    }
                }

                _lastStatus = $"Loaded recipe '{recipeName}'";
                UpdateStatusVariables();
                Log.Information("Recipe '{Recipe}': loaded '{Name}'", _config.Name, recipeName);
            }
            catch (Exception ex)
            {
                _lastStatus = $"Error loading '{recipeName}': {ex.Message}";
                UpdateStatusVariables();
                Log.Error(ex, "Recipe '{Recipe}': error loading '{Name}'", _config.Name, recipeName);
            }
        }

        /// <summary>Activate a recipe: load it and set it as the active recipe name.</summary>
        public void ActivateRecipe(string recipeName)
        {
            LoadRecipe(recipeName);
            _activeName = recipeName;
            _lastStatus = $"Activated recipe '{recipeName}'";
            UpdateStatusVariables();
        }

        /// <summary>Delete a named recipe from the database.</summary>
        public void DeleteRecipe(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName) || _connection == null) return;

            try
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = "DELETE FROM recipes WHERE recipe_name = @name;";
                cmd.Parameters.AddWithValue("@name", recipeName);
                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    _lastStatus = $"Deleted recipe '{recipeName}'";
                    if (_activeName == recipeName) _activeName = "";
                }
                else
                {
                    _lastStatus = $"Recipe '{recipeName}' not found";
                }
                UpdateStatusVariables();
            }
            catch (Exception ex)
            {
                _lastStatus = $"Error deleting '{recipeName}': {ex.Message}";
                UpdateStatusVariables();
            }
        }

        /// <summary>Get list of stored recipe names.</summary>
        public List<string> GetRecipeNames()
        {
            var names = new List<string>();
            if (_connection == null) return names;

            try
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = "SELECT recipe_name FROM recipes ORDER BY recipe_name;";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    names.Add(reader.GetString(0));
            }
            catch { }
            return names;
        }

        private void WriteValueToVariable(string variablePath, string strValue)
        {
            // Read current value to detect type, then convert
            object? current = null;
            try { current = _nodeManager.ReadVariable(variablePath); }
            catch { return; }

            object converted;
            if (current is double || current is float)
                converted = double.TryParse(strValue, CultureInfo.InvariantCulture, out var d) ? d : 0.0;
            else if (current is int)
                converted = int.TryParse(strValue, out var i32) ? i32 : 0;
            else if (current is long)
                converted = long.TryParse(strValue, out var i64) ? i64 : 0L;
            else if (current is bool)
                converted = strValue is "True" or "true" or "1";
            else if (current is short)
                converted = short.TryParse(strValue, out var i16) ? i16 : (short)0;
            else if (current is ushort)
                converted = ushort.TryParse(strValue, out var u16) ? u16 : (ushort)0;
            else if (current is uint)
                converted = uint.TryParse(strValue, out var u32) ? u32 : 0u;
            else
                converted = strValue;

            _nodeManager.WriteVariable(variablePath, converted);
        }

        private void UpdateStatusVariables()
        {
            var prefix = $"Recipe.{_config.Name}";
            TryWrite($"{prefix}.ActiveName", _activeName);
            TryWrite($"{prefix}.LastStatus", _lastStatus);
            TryWrite($"{prefix}.RecipeList", string.Join(",", GetRecipeNames()));
        }

        private void TryWrite(string path, object value)
        {
            try { _nodeManager.WriteVariable(path, value); }
            catch { /* variable may not exist yet during initialization */ }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
