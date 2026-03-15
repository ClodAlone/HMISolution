using DevExpress.Xpf.Editors;
using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UFUAEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using WPFUtilities.Converters;
using WPFUtilities.PropertyDataTemplate;

namespace WPFUtilities
{
    public class SmartControlHelper
    {
        public static void GetInstanceName(string text, out string instance, out string name)
        {
            var tags = "Tags/";
            var tag = "Tag/";
            if (text.StartsWith(tags))
                text = text.Substring(tags.Length);
            else if (text.StartsWith(tag))
                text = text.Substring(tag.Length);
            if (text.Contains("("))
                text = text.Split(new String[] { " (" }, StringSplitOptions.RemoveEmptyEntries)[0];

            var split = text.Split(':');
            instance = split[0]?.Replace('/', '\\');
            name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;

            name = name.Replace('/', '\\');
        }
    }
}
