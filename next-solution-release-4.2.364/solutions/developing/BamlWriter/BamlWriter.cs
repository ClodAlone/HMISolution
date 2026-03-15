using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.BuildEngine;

namespace BamlWriter
{
    public static class BamlWriter
    {
        public static byte[] Save(string xaml/*, ILogger logger*/)
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);

            try
            {
                string xamlFile = Path.Combine(path, "input.xaml");
                string projFile = Path.Combine(path, "project.proj");

                using (FileStream fs = File.Create(xamlFile))
                {
                    byte[] xamlBytes = Encoding.UTF8.GetBytes(xaml);
                    fs.Write(xamlBytes, 0, xamlBytes.Length);
                }

                Engine engine = new Engine()
                {
                    DefaultToolsVersion = "4.0"
                };
                // engine.RegisterLogger(logger);
                Project project = engine.CreateNewProject();
                BuildPropertyGroup pgroup = project.AddNewPropertyGroup(false);
                pgroup.AddNewProperty("AssemblyName", "temp");
                pgroup.AddNewProperty("OutputType", "Library");
                pgroup.AddNewProperty("IntermediateOutputPath", ".");
                pgroup.AddNewProperty("MarkupCompilePass1DependsOn", "ResolveReferences");

                BuildItemGroup igroup = project.AddNewItemGroup();
                igroup.AddNewItem("Page", "input.xaml");
                igroup.AddNewItem("Reference", "WindowsBase");
                igroup.AddNewItem("Reference", "PresentationCore");
                igroup.AddNewItem("Reference", "PresentationFramework");

                project.AddNewImport(@"$(MSBuildBinPath)\Microsoft.CSharp.targets", null);
                project.AddNewImport(@"$(MSBuildBinPath)\Microsoft.WinFX.targets", null);
                project.FullFileName = projFile;

                if (engine.BuildProject(project, "MarkupCompilePass1"))
                {
                    using (FileStream fs = File.OpenRead(Path.Combine(path, "input.baml")))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
                else
                {
                    // attach a logger to the Engine if you need better errors   
                    throw new System.Exception("Baml compilation failed.");
                }
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }
    }
}
