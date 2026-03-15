using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using ScreenSettings;
using UFInterfaces;
using DocumentManager.ComponentService;
using System.Globalization;
using System.Text;
#if !NET_STANDARD
using Microsoft.Expression.Shapes;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Data;
using Newtonsoft.Json.Linq;
using Utilities.WPF;
using Utilities;
using System.Reflection;
using Newtonsoft.Json;
using OPCUAViewModel;
using UFUAEditor.ComponentService;
using AnimationManager;
using CommandManager;
using System.Xml.Linq;
using ScreenSettings.Documents;
using ScreenSettings.Entities;
using WPFUtilities;
using log4net;
using DevExpress.Xpf.Editors.Helpers;
using ThemeHelper = WPFUtilities.ThemeHelper;
using MSSchedulerSettings.ComponentService;
#endif

namespace SVGHelper
{
    public static class TileSVGHelper
    {
        #region declaration
#if !NET_STANDARD
        private static readonly ILog logDeploy = LogManager.GetLogger(Properties.Resources.SvgGenerator);
#endif
        #endregion

        #region methods
        public static bool Save(string filePath, IDocument document, tile tile)
        {
#if !NET_STANDARD
            if (!System.IO.Directory.Exists(filePath))
                System.IO.Directory.CreateDirectory(filePath);

            filePath = $"{filePath}\\{Properties.Settings.Default.SVGTileDetails}";
            try
            {
                String ret = JSONHelper.IndentJSon(tile.ToJSON(), false);
                File.WriteAllText(filePath, ret.Replace(":http://progea.com", ""));
            }
            catch (Exception ex)
            {
                Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                string msg = string.Format(Properties.Resources.MsgWithException,
                    string.Format(Properties.Resources.ErrorExportingCommands, document.Title),
                    e.Message);
                logDeploy.Error(msg);
                return true;
            }

#endif
            return false;
        }
 
        #endregion
    }

    [DataContract(Name = "screenobject", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class screenobject
    {
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Color { get; set; }
    }
    [DataContract(Name = "tile", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class tile
    {
        [DataMember]
        public List<screenobject> ScreenObjects { get; set; }
#if !NET_STANDARD
        [DataMember]
        public string Color { get; set; }
#endif
    }
}
