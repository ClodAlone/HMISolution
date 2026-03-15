using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardPluginHelpers
{
    public static class Helper
    {
        public class ProjectInfo
        {
            public string Name { get; set; }
            public string FilePath { get; set; }
            public string Folder { get; set; }
        }
        public static ProjectInfo GetNewPath(string folder, string name, string fileType)
        {
            ProjectInfo projectInfo = new ProjectInfo()
            {
                Name = name,
                Folder = $"{folder}\\{name}",
                FilePath = $"{folder}\\{name}\\{name}{fileType}"
            };

            int i = 0;
            while (System.IO.File.Exists(projectInfo.FilePath))
            {
                i += 1;
                projectInfo.Name = String.Format($"{name}{i}", name, i);
                projectInfo.Folder = $"{folder}\\{projectInfo.Name}";
                projectInfo.FilePath = $"{projectInfo.Folder}\\{projectInfo.Name}{fileType}";
            }

            return projectInfo;
        }
    }
}
