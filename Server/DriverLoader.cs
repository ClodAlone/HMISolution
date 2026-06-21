using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Opc.Ua;
using Serilog;

namespace SimpleOpcFileServer
{
    public static class DriverLoader
    {
        public static List<IDriver> LoadDrivers(ISystemContext context)
        {
            var drivers = new List<IDriver>();
            var searchPaths = new List<string>();

            // Primary: "drivers" subdirectory next to the executable
            var driversDir = Path.Combine(AppContext.BaseDirectory, "drivers");
            if (Directory.Exists(driversDir))
                searchPaths.Add(driversDir);

            // Secondary: application base directory (for development where all DLLs are together)
            searchPaths.Add(AppContext.BaseDirectory);

            // Collect all driver DLL paths first
            var driverDlls = new List<string>();
            var loadedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var searchPath in searchPaths)
            {
                IEnumerable<string> dlls;
                try
                {
                    dlls = Directory.GetFiles(searchPath, "Drivers.*.dll", SearchOption.AllDirectories);
                }
                catch
                {
                    continue;
                }

                foreach (var dll in dlls)
                {
                    var fileName = Path.GetFileName(dll);
                    if (loadedFileNames.Contains(fileName))
                        continue;

                    // Skip the shared abstractions assembly — it contains no driver implementations
                    if (fileName.Equals("Drivers.Abstractions.dll", StringComparison.OrdinalIgnoreCase))
                        continue;

                    loadedFileNames.Add(fileName);
                    driverDlls.Add(dll);
                }
            }

            // Resolve driver dependencies (MQTTnet, NModbus, etc.) from the drivers/ folder.
            // Assembly.LoadFrom does not trigger AssemblyLoadContext.Resolving for its
            // dependencies, but it does trigger AppDomain.AssemblyResolve.
            ResolveEventHandler resolveHandler = (sender, args) =>
            {
                var name = new AssemblyName(args.Name);

                // If the assembly is already loaded (possibly a different version), reuse it.
                // This avoids FileLoadException when the host and a driver reference
                // different minor versions of the same package (e.g. Serilog 4.0 vs 4.3).
                var loaded = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => string.Equals(a.GetName().Name, name.Name, StringComparison.OrdinalIgnoreCase));
                if (loaded != null)
                    return loaded;

                var candidate = Path.Combine(driversDir, name.Name + ".dll");
                if (File.Exists(candidate))
                {
                    try
                    {
                        return Assembly.LoadFrom(candidate);
                    }
                    catch { }
                }
                return null;
            };

            AppDomain.CurrentDomain.AssemblyResolve += resolveHandler;
            try
            {
                foreach (var dll in driverDlls)
                {
                    var fileName = Path.GetFileName(dll);
                    try
                    {
                        var assembly = Assembly.LoadFrom(dll);

                        Type[] types;
                        try
                        {
                            types = assembly.GetTypes();
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            Log.Error(ex, "Some types could not be loaded from {FileName}", fileName);
                            foreach (var le in ex.LoaderExceptions ?? [])
                                Log.Debug("  LoaderException: {Message}", le?.Message);
                            types = ex.Types.Where(t => t != null).ToArray()!;
                        }

                        foreach (var type in types)
                        {
                            try
                            {
                                if (!typeof(IDriver).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                                    continue;

                                var driver = (IDriver)Activator.CreateInstance(type, context)!;
                                drivers.Add(driver);
                                Log.Debug("Loaded driver: {DriverKey} from {FileName}", driver.Key, fileName);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Failed to load driver type {TypeName} from {FileName}", type.FullName, fileName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to load driver assembly: {Dll}", dll);
                    }
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= resolveHandler;
            }

            Log.Information("Loaded {Count} driver(s): {Drivers}", drivers.Count, string.Join(", ", drivers.Select(d => d.Key)));
            return drivers;
        }
    }
}
