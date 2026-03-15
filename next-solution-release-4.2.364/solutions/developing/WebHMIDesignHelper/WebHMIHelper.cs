using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WebHMIDesignHelper
{
    public static class WebHMIHelper
    {
        static List<string> visibleHMIScreenControls;
        public static List<string> VisibleHMIScreenControls
        {
            get
            {
                if (visibleHMIScreenControls == null)
                    UpdateControlList();
                return visibleHMIScreenControls;
            }
        }
        static List<string> visibleHMIToolboxCategories;
        public static List<string> VisibleHMIToolboxCategories
        {
            get
            {
                if (visibleHMIToolboxCategories == null)
                    UpdateControlList();
                return visibleHMIToolboxCategories;
            }
        }
        static List<string> visibleHMIDocumentManager;
        public static List<string> VisibleHMIDocumentManagers
        {
            get
            {
                if (visibleHMIDocumentManager == null)
                    UpdateControlList();
                return visibleHMIDocumentManager;
            }
        }

        static List<string> visibleHMICommands;
        public static List<string> VisibleHMICommands
        {
            get
            {
                if (visibleHMICommands == null)
                    UpdateControlList();
                return visibleHMICommands;
            }
        }

        static List<string> visibleHMIAnimations;
        public static List<string> VisibleHMIAnimations
        {
            get
            {
                if (visibleHMIAnimations == null)
                    UpdateControlList();
                return visibleHMIAnimations;
            }
        }
        private static void UpdateControlList()
        {
            visibleHMIScreenControls = new List<string>();
            visibleHMIToolboxCategories = new List<string>();
            visibleHMIDocumentManager = new List<string>();
            visibleHMIAnimations = new List<string>();
            visibleHMICommands = new List<string>();
            string fileHMIpath = string.Format("{0}\\{1}.xml", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Properties.Settings.Default.VisibleHMICategoriesFileName);
            XDocument xmlDocument;
            visibleHMIScreenControls = GetDescendants(fileHMIpath, "VisibleHMIScreenControls", out xmlDocument);
            visibleHMIToolboxCategories = GetDescendants(xmlDocument, "VisibleHMIToolboxCategories");

            fileHMIpath = string.Format("{0}\\{1}.xml", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Properties.Settings.Default.VisibleHMIDocumentManagerFileName);
            visibleHMIDocumentManager = GetDescendants(fileHMIpath, "VisibleHMIDocumentManagers", out xmlDocument);
            fileHMIpath = string.Format("{0}\\{1}.xml", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Properties.Settings.Default.VisibleHMIAnimationFileName);
            visibleHMIAnimations = GetDescendants(fileHMIpath, "VisibleHMIAnimations", out xmlDocument);
            fileHMIpath = string.Format("{0}\\{1}.xml", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Properties.Settings.Default.VisibleHMICommandFileName);
            visibleHMICommands = GetDescendants(fileHMIpath, "VisibleHMICommands", out xmlDocument);
        }

        private static List<string> GetDescendants(string fileHMIpath, string descendantRoot, out XDocument xmlDocument)
        {
            XDocument xml = null;
            List<string> visibleHMIs = new List<string>();
            if (System.IO.File.Exists(fileHMIpath))
            {
                try
                {
                    xml = XDocument.Load(fileHMIpath);
                    var _visibleHMIDescendants = xml.Root.Descendants(descendantRoot).FirstOrDefault();
                    foreach (XElement node in _visibleHMIDescendants.Descendants("Name").ToList())
                    {
                        var componentName = node.Value as String;
                        if (!String.IsNullOrEmpty(componentName))
                            visibleHMIs.Add(componentName.Trim());
                    }
                }
                catch (Exception)
                {
                }
            }
            xmlDocument = xml;
            return visibleHMIs;
        }

        private static List<string> GetDescendants(XDocument xml, string descendantRoot)
        {
            List<string> visibleHMIs = new List<string>();
            if (xml != null)
            {
                try
                {
                    var _visibleHMIDescendants = xml.Root.Descendants(descendantRoot).FirstOrDefault();
                    foreach (XElement node in _visibleHMIDescendants.Descendants("Name").ToList())
                    {
                        var componentName = node.Value as String;
                        if (!String.IsNullOrEmpty(componentName))
                            visibleHMIs.Add(componentName.Trim());
                    }
                }
                catch (Exception)
                {
                }
            }
            return visibleHMIs;
        }
    }
}
