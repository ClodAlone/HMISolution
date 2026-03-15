using DevExpress.Xpf.Bars;
using StartupWelcome.Model;
using StartupWelcome.View_Model;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;

namespace StartupWelcome
{
    internal static class RecentMenuHelper
    {
        internal static void LoadRecentFileList(BarSubItem subItem, RecentRepository recent)
        {
            subItem.Items.Clear();
            var recentList = recent.RecentList.Take(Properties.Settings.Default.MaxRecentFileNumber).ToList();
            if(recentList.Count > 0)
                foreach (var recentInfo in recentList)
                {
                    var path = GetPath(recentInfo);
                    var btnItem = new BarButtonItem();
                    ToolTipService.SetShowOnDisabled(btnItem, true);
                    btnItem.Content = path;
                    btnItem.ToolTip = recentInfo.Name;
                    btnItem.Command = ApplicationCommands.Open;
                    btnItem.CommandParameter = recentInfo.ProjectPath;
                    btnItem.MergeOrder = 10;
                    subItem.Items.Add(btnItem);
                }
            else
            {
                var btnItem = new BarButtonItem();
                ToolTipService.SetShowOnDisabled(btnItem, true);
                btnItem.Content = "...";
                btnItem.Command = ApplicationCommands.Open;
                btnItem.CommandParameter = null;
                btnItem.MergeOrder = 10;
                subItem.Items.Add(btnItem);
            }
        }

        internal static string GetPath(RecentInfo recentInfo)
        {
            string path = recentInfo?.Path;
            try
            {
                if (string.IsNullOrEmpty(path) || Properties.Settings.Default.MaxRecentFilePathLenght <= 0)
                    return null;
                else if (path.Length <= Properties.Settings.Default.MaxRecentFilePathLenght)
                    return $"{path}";
                else
                {
                    if (Properties.Settings.Default.MaxRecentFilePrefixLenght >= 0 && 
                        Properties.Settings.Default.MaxRecentFilePrefixLenght < Properties.Settings.Default.MaxRecentFilePathLenght)

                        return $"{path.Substring(0, Properties.Settings.Default.MaxRecentFilePrefixLenght)}...\\{path.Substring(path.Length - (Properties.Settings.Default.MaxRecentFilePathLenght - Properties.Settings.Default.MaxRecentFilePrefixLenght))}";
                    else
                        return $"...\\{path.Substring(path.Length - Properties.Settings.Default.MaxRecentFilePathLenght)}";
                }
            }
            catch (Exception)
            {
                return $"{path}";
            }
        }
    }
}