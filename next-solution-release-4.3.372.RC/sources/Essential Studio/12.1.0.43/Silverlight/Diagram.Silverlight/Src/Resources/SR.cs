#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace Syncfusion.Windows.Diagram
{
    sealed class SR
    {
        // Fields
        private ResourceManager resources;

        private static SR loader = null;

        private SR(string LocalizationPath)
        {
            System.Resources.ResourceManager localizedManager = GetLocalizedResourceManager(Assembly.GetExecutingAssembly(), LocalizationPath);
            if (localizedManager == null)
            {
#if SILVERLIGHT
                this.resources = Syncfusion.Windows.Diagram.Resources.Syncfusion_Diagram_Silverlight.ResourceManager;
#endif
#if WPF
                this.resources = Syncfusion.Windows.Diagram.Resources.Syncfusion_Diagram_Wpf.ResourceManager;
#endif
                //Or
                //this.resources = new ResourceManager("SyncLocalizationLibrary.Resources.SyncLocalizationLibrary", typeof(SyncLocalizationLibrary.Resources.SyncLocalizationLibrary).Assembly);
            }
            else
            {
                this.resources = localizedManager;
            }
        }

        private static System.Resources.ResourceManager GetLocalizedResourceManager(System.Reflection.Assembly controlAssembly, string LocalizationPath)
        {
            if (Application.Current != null)
            {
                try
                {
                    Assembly assembly = Application.Current.GetType().Assembly;
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager(string.Format("{0}." + LocalizationPath + (string.IsNullOrEmpty(LocalizationPath)? "" : ".") + "{1}", assembly.FullName.Split(new char[] { ',' })[0],
                        controlAssembly.FullName.Split(new char[] { ',' })[0]), assembly);
                    if (manager != null)
                    {
                        foreach (var resourceName in assembly.GetManifestResourceNames())
                        {
                            if (resourceName.Equals(
                                string.Format(
                                    "{0}." + LocalizationPath + (string.IsNullOrEmpty(LocalizationPath) ? "" : ".") + "{1}.resources",
                                    assembly.FullName.Split(new char[] { ',' })[0],
                                    controlAssembly.FullName.Split(new char[] { ',' })[0])
                                    )
                                )
                            {
                                var currentUICulture = CultureInfo.CurrentUICulture;
                                if (manager.GetResourceSet(currentUICulture, true, true) != null)
                                {
                                    return manager;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public static void ReleaseResources()
        {
            SR.loader.resources.ReleaseAllResources();
        }

        // Methods
        private static SR GetLoader(string LocalizationPath)
        {
            lock (typeof(SR))
            {
                if (SR.loader == null)
                    SR.loader = new SR(LocalizationPath);
                return SR.loader;
            }
        }

        public static string GetString(CultureInfo culture, string name, string LocalizationPath, params object[] args)
        {
            SR sr = SR.GetLoader(LocalizationPath);
            string value;

            if (sr == null)
                return null;

            try
            {
                value = sr.resources.GetString(name, culture);
                if (value != null && args != null && args.Length > 0)
                    return String.Format(value, args);

                return value;
            }
            catch
            {
                return name;
            }
        }

        public static string GetString(string name, string LocalizationPath)
        {
            return SR.GetString(null, name, LocalizationPath);
        }

        public static string GetString(string name, string LocalizationPath, params object[] args)
        {
            return SR.GetString(null, name, LocalizationPath, args);
        }

        public static string GetString(CultureInfo culture, string name, string LocalizationPath)
        {
            SR sr = SR.GetLoader(LocalizationPath);
            if (sr == null)
                return null;
            string value = "";
            try
            {
                value = sr.resources.GetString(name, culture);
            }
            catch
            {
#if SILVERLIGHT
                value = Syncfusion.Windows.Diagram.Resources.Syncfusion_Diagram_Silverlight.ResourceManager.GetString(name);
#endif
#if WPF
                value = Syncfusion.Windows.Diagram.Resources.Syncfusion_Diagram_Wpf.ResourceManager.GetString(name);
#endif
            }
            return value;
        }

        public static object GetObject(CultureInfo culture, string name, string LocalizationPath)
        {
            SR sr = SR.GetLoader(LocalizationPath);
            if (sr == null)
                return null;
            return sr.resources.GetObject(name, culture);
        }

        public static object GetObject(string name, string LocalizationPath)
        {
            return SR.GetObject(null, name, LocalizationPath);
        }

        public static bool GetBoolean(CultureInfo culture, string name, string LocalizationPath)
        {
            bool value;
            SR sr = SR.GetLoader(LocalizationPath);
            object obj;
            value = false;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Boolean)
                    value = ((bool)obj);
            }
            return value;
        }

        public static bool GetBoolean(string name)
        {
            return SR.GetBoolean(name);
        }

        public static byte GetByte(CultureInfo culture, string name, string LocalizationPath)
        {
            byte value;
            SR sr = SR.GetLoader(LocalizationPath);
            object obj;
            value = (byte)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Byte)
                    value = ((byte)obj);
            }
            return value;
        }

        public static byte GetByte(string name, string LocalizationPath)
        {
            return SR.GetByte(null, name, LocalizationPath);
        }

        public static char GetChar(CultureInfo culture, string name, string LocalizationPath)
        {
            char value;
            SR sr = SR.GetLoader(LocalizationPath);
            object obj;
            value = (char)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Char)
                    value = (char)obj;
            }
            return value;
        }

        public static char GetChar(string name, string LocalizationPath)
        {
            return SR.GetChar(null, name, LocalizationPath);
        }

        public static double GetDouble(CultureInfo culture, string name, string LocalizationPath)
        {
            double value;
            SR sr = SR.GetLoader(LocalizationPath);
            object obj;
            value = 0.0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Double)
                    value = ((double)obj);
            }
            return value;
        }

        public static double GetDouble(string name, string LocalizationPath)
        {
            return SR.GetDouble(null, name, LocalizationPath);
        }

        public static float GetFloat(CultureInfo culture, string name, string LocalizationPath)
        {
            float value;
            SR sr = SR.GetLoader(LocalizationPath);
            object obj;
            value = 0.0f;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Single)
                    value = ((float)obj);
            }
            return value;
        }

        public static float GetFloat(string name, string LocalizationPath)
        {
            return SR.GetFloat(null, name, LocalizationPath);
        }

        public static int GetInt(string name, string LocalizationPath)
        {
            return SR.GetInt(null, name, LocalizationPath);
        }

        public static int GetInt(CultureInfo culture, string name, string LocalizationPath)
        {
            int value;
            SR sr = SR.GetLoader(LocalizationPath);
            object obj;
            value = 0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Int32)
                    value = ((int)obj);
            }
            return value;
        }

        public static long GetLong(string name, string LocalizationPath)
        {
            return SR.GetLong(null, name, LocalizationPath);
        }

        public static long GetLong(CultureInfo culture, string name, string LocalizationPath)
        {
            Int64 value;
            SR sr = SR.GetLoader(LocalizationPath);
            object obj;
            value = ((Int64)0);
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Int64)
                    value = ((Int64)obj);
            }
            return value;
        }

        public static short GetShort(CultureInfo culture, string name, string LocalizationPath)
        {
            short value;
            SR sr = SR.GetLoader(LocalizationPath);
            object obj;
            value = (short)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Int16)
                    value = ((short)obj);
            }
            return value;
        }

        public static short GetShort(string name, string LocalizationPath)
        {
            return SR.GetShort(null, name, LocalizationPath);
        }
    }

    internal class ResourceWrapper
    {
        const string ContextMenu_DeleteValue = "ContextMenu_Delete";
        const string ContextMenu_GroupingValue = "ContextMenu_Grouping";
        const string ContextMenu_Grouping_GroupValue = "ContextMenu_Grouping_Group";
        const string ContextMenu_Grouping_UngroupValue = "ContextMenu_Grouping_Ungroup";
        const string ContextMenu_OrderValue = "ContextMenu_Order";
        const string ContextMenu_Order_BringForwardValue = "ContextMenu_Order_BringForward";
        const string ContextMenu_Order_BringToFrontValue = "ContextMenu_Order_BringToFront";
        const string ContextMenu_Order_SendBackwardValue = "ContextMenu_Order_SendBackward";
        const string ContextMenu_Order_SendToBackValue = "ContextMenu_Order_SendToBack";
        const string SymbolPaletteFilter_AllValue = "SymbolPaletteFilter_All";
        const string SymbolPaletteFilter_ConnectorsValue = "SymbolPaletteFilter_Connectors";
        const string SymbolPaletteFilter_CustomShapesValue = "SymbolPaletteFilter_CustomShapes";
        const string SymbolPaletteFilter_ElectricalShapesValue = "SymbolPaletteFilter_ElectricalShapes";
        const string SymbolPaletteFilter_FlowchartValue = "SymbolPaletteFilter_Flowchart";
        const string SymbolPaletteFilter_ShapesValue = "SymbolPaletteFilter_Shapes";
        const string SymbolPaletteGroup_ConnectorsValue = "SymbolPaletteGroup_Connectors";
        const string SymbolPaletteGroup_CustomShapesValue = "SymbolPaletteGroup_CustomShapes";
        const string SymbolPaletteGroup_ElectricalShapesValue = "SymbolPaletteGroup_ElectricalShapes";
        const string SymbolPaletteGroup_FlowchartValue = "SymbolPaletteGroup_Flowchart";
        const string SymbolPaletteGroup_ShapesValue = "SymbolPaletteGroup_Shapes";

        const string PrintDialog_PrintValue = "PrintDialog_Print";
        const string PrintDialog_CancelValue="PrintDialog_Cancel";
        const string PrintDialog_AdvancedValue = "PrintDialog_Advanced";
        const string PrintDialog_PrintStretchValue = "PrintDialog_PrintStretch";
        const string PrintDialog_ColorModeValue="PrintDialog_ColorMode";
        const string PrintDialog_BlackandWhiteModeValue = "PrintDialog_BlackandWhiteMode";
        const string PrintDialog_NameValue = "PrintDialog_Name";
        const string PrintDialog_Stretch_NoneValue = "PrintDialog_Stretch_None";
        const string PrintDialog_Stretch_FillValue = "PrintDialog_Stretch_Fill";
        const string PrintDialog_Stretch_UniformValue = "PrintDialog_Stretch_Uniform";
        const string PrintDialog_Stretch_UniformtoFillValue = "PrintDialog_Stretch_UniformtoFill";

        public string ContextMenu_Delete { get; set; }
        public string ContextMenu_Grouping { get; set; }
        public string ContextMenu_Grouping_Group { get; set; }
        public string ContextMenu_Grouping_Ungroup { get; set; }
        public string ContextMenu_Order { get; set; }
        public string ContextMenu_Order_BringForward { get; set; }
        public string ContextMenu_Order_BringToFront { get; set; }
        public string ContextMenu_Order_SendBackward { get; set; }
        public string ContextMenu_Order_SendToBack { get; set; }
        public string SymbolPaletteFilter_All { get; set; }
        public string SymbolPaletteFilter_Connectors { get; set; }
        public string SymbolPaletteFilter_CustomShapes { get; set; }
        public string SymbolPaletteFilter_ElectricalShapes { get; set; }
        public string SymbolPaletteFilter_Flowchart { get; set; }
        public string SymbolPaletteFilter_Shapes { get; set; }
        public string SymbolPaletteGroup_Connectors { get; set; }
        public string SymbolPaletteGroup_CustomShapes { get; set; }
        public string SymbolPaletteGroup_ElectricalShapes { get; set; }
        public string SymbolPaletteGroup_Flowchart { get; set; }
        public string SymbolPaletteGroup_Shapes { get; set; }
       

        public string PrintDialog_Print { get; set; }
        public string PrintDialog_Cancel { get; set; }
        public string PrintDialog_Advanced { get; set; }
        public string PrintDialog_PrintStretch { get; set; }
        public string PrintDialog_ColorMode { get; set; }
        public string PrintDialog_BlackandWhiteMode { get; set; }
        public string PrintDialog_Name { get; set; }
        public string PrintDialog_Stretch_None { get; set; }
        public string PrintDialog_Stretch_Fill { get; set; }
        public string PrintDialog_Stretch_Uniform { get; set; }
        public string PrintDialog_Stretch_UniformtoFill { get; set; }

        public ResourceWrapper(string LocalizationPath)
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            ContextMenu_Delete = SR.GetString(ci, ContextMenu_DeleteValue, LocalizationPath);
            ContextMenu_Grouping = SR.GetString(ci, ContextMenu_GroupingValue, LocalizationPath);
            ContextMenu_Grouping_Group = SR.GetString(ci, ContextMenu_Grouping_GroupValue, LocalizationPath);
            ContextMenu_Grouping_Ungroup = SR.GetString(ci, ContextMenu_Grouping_UngroupValue, LocalizationPath);
            ContextMenu_Order = SR.GetString(ci, ContextMenu_OrderValue, LocalizationPath);
            ContextMenu_Order_BringForward = SR.GetString(ci, ContextMenu_Order_BringForwardValue, LocalizationPath);
            ContextMenu_Order_BringToFront = SR.GetString(ci, ContextMenu_Order_BringToFrontValue, LocalizationPath);
            ContextMenu_Order_SendBackward = SR.GetString(ci, ContextMenu_Order_SendBackwardValue, LocalizationPath);
            ContextMenu_Order_SendToBack = SR.GetString(ci, ContextMenu_Order_SendToBackValue, LocalizationPath);
            SymbolPaletteFilter_All = SR.GetString(ci, SymbolPaletteFilter_AllValue, LocalizationPath);
            SymbolPaletteFilter_Connectors = SR.GetString(ci, SymbolPaletteFilter_ConnectorsValue, LocalizationPath);
            SymbolPaletteFilter_CustomShapes = SR.GetString(ci, SymbolPaletteFilter_CustomShapesValue, LocalizationPath);
            SymbolPaletteFilter_ElectricalShapes = SR.GetString(ci, SymbolPaletteFilter_ElectricalShapesValue, LocalizationPath);
            SymbolPaletteFilter_Flowchart = SR.GetString(ci, SymbolPaletteFilter_FlowchartValue, LocalizationPath);
            SymbolPaletteFilter_Shapes = SR.GetString(ci, SymbolPaletteFilter_ShapesValue, LocalizationPath);
            SymbolPaletteGroup_Connectors = SR.GetString(ci, SymbolPaletteGroup_ConnectorsValue, LocalizationPath);
            SymbolPaletteGroup_CustomShapes = SR.GetString(ci, SymbolPaletteGroup_CustomShapesValue, LocalizationPath);
            SymbolPaletteGroup_ElectricalShapes = SR.GetString(ci, SymbolPaletteGroup_ElectricalShapesValue, LocalizationPath);
            SymbolPaletteGroup_Flowchart = SR.GetString(ci, SymbolPaletteGroup_FlowchartValue, LocalizationPath);
            SymbolPaletteGroup_Shapes = SR.GetString(ci, SymbolPaletteGroup_ShapesValue, LocalizationPath);
            PrintDialog_Print = SR.GetString(ci, PrintDialog_PrintValue, LocalizationPath);
            PrintDialog_Cancel = SR.GetString(ci, PrintDialog_CancelValue, LocalizationPath);
            PrintDialog_Advanced = SR.GetString(ci, PrintDialog_AdvancedValue, LocalizationPath);
            PrintDialog_PrintStretch = SR.GetString(ci, PrintDialog_PrintStretchValue, LocalizationPath);
            PrintDialog_ColorMode = SR.GetString(ci, PrintDialog_ColorModeValue, LocalizationPath);
            PrintDialog_BlackandWhiteMode = SR.GetString(ci, PrintDialog_BlackandWhiteModeValue, LocalizationPath);
            PrintDialog_Name = SR.GetString(ci, PrintDialog_NameValue, LocalizationPath);
            PrintDialog_Stretch_None = SR.GetString(ci, PrintDialog_Stretch_NoneValue, LocalizationPath);
            PrintDialog_Stretch_Fill = SR.GetString(ci, PrintDialog_Stretch_FillValue, LocalizationPath);
            PrintDialog_Stretch_Uniform = SR.GetString(ci, PrintDialog_Stretch_UniformValue, LocalizationPath);
            PrintDialog_Stretch_UniformtoFill = SR.GetString(ci, PrintDialog_Stretch_UniformtoFillValue, LocalizationPath);
        }
    }

}
