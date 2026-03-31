using Opc.Ua;

namespace SimpleOpcFileServer;

/// <summary>
/// Discovers and loads IDriver implementations from the Drivers directory.
/// </summary>
internal static class DriverLoader
{
    /// <summary>
    /// Scans the Drivers folder next to the running executable and loads all IDriver implementations.
    /// Returns an empty list if no drivers are found or the directory does not exist.
    /// </summary>
    public static List<IDriver> LoadDrivers(ISystemContext context)
    {
        var drivers = new List<IDriver>();
        var driversDir = Path.Combine(AppContext.BaseDirectory, "Drivers");
        if (!Directory.Exists(driversDir))
            return drivers;

        foreach (var dll in Directory.GetFiles(driversDir, "*.dll"))
        {
            try
            {
                var asm = System.Reflection.Assembly.LoadFrom(dll);
                foreach (var type in asm.GetExportedTypes())
                {
                    if (!type.IsAbstract && typeof(IDriver).IsAssignableFrom(type))
                    {
                        if (Activator.CreateInstance(type) is IDriver driver)
                            drivers.Add(driver);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "Failed to load driver from {Dll}", dll);
            }
        }

        return drivers;
    }
}
