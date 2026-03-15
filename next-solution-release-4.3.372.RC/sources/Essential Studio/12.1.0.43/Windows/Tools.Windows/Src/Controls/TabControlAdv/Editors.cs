#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    [Syncfusion.Documentation.DocumentationExclude()]
    public class ReflectionHelper
    {
        public static ArrayList GetTabStyleNames(ArrayList rendererTypes)
        {
            ArrayList tabNames = new ArrayList();
            foreach (Type rendererType in rendererTypes)
            {
                string tabname = ReflectionHelper.GetTabNameFromType(rendererType);

                if (tabname != null && tabname.Length > 0)
                    tabNames.Add(tabname);
            }
            return tabNames;
        }
        public static string GetTabNameFromType(Type tabRenderer)
        {
            string tabname = string.Empty;
            try
            {
                tabname = tabRenderer.InvokeMember("TabStyleName", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static, null, null, new object[] { }) as string;                                
            }
            catch (Exception e) 
            {
                MessageBox.Show("Invalid TabStyle: " + e.Message);
            }
            if (tabname == string.Empty)
                tabname = tabRenderer.ToString();
            return tabname;
        }
        public static Type GetTabTypeFromName(string tabStyleName)
        {
            ArrayList rendererTypes = TabRendererFactory.GetRegisteredRenderers(false);
            foreach (Type rendererType in rendererTypes)
            {
                string tabname = string.Empty;
                try
                {
                    tabname = rendererType.InvokeMember("TabStyleName", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static, null, null, new object[] { }) as string;                       
                }
                catch 
                { 
                }
                if (tabname == tabStyleName)
                    return rendererType;
            }
            return null;
        }
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    public class TabStyleEditor : UITypeEditor
    {
        private ListBox tabList;
        private string addMoreTabs = "Add Custom Tabs...";
       private IWindowsFormsEditorService editorService;
        private void InitList(object value, bool insertAddMoreTabs)
        {
            if (tabList == null)
            {
                tabList = new ListBox();
                tabList.Click += new EventHandler(this.ListItemClicked);
            }

            tabList.Items.Clear();

            // List of tab renderer types
            ArrayList rendererTypes = TabRendererFactory.GetRegisteredRenderers(true);

            RemoveInvalidTypes(rendererTypes);

            ArrayList tabNames = ReflectionHelper.GetTabStyleNames(rendererTypes);
            foreach (object tabName in tabNames)
                tabList.Items.Add(tabName);

            if (insertAddMoreTabs)
                tabList.Items.Add(this.addMoreTabs);

            tabList.SelectedItem = value;

            tabList.Height = (ListBox.DefaultItemHeight * (this.tabList.Items.Count + 1)) + 3;
        }

        protected internal static void RemoveInvalidTypes(ArrayList rendererTypes)
        {
            if (rendererTypes != null && rendererTypes.Count > 0)
            {
                Type rendererType = null;

                for (int i = 0; i < rendererTypes.Count; i++)
                {
                    rendererType = rendererTypes[i] as Type;

                    if (!TabControlAdv.IsValidRendererType(rendererType))
                    {
                        rendererTypes.Remove(rendererType);
                    }
                }
            }
        }

        private void ListItemClicked(object sender, EventArgs e)
        {
            this.editorService.CloseDropDown();
        }

        public TabStyleEditor()
        {
            // So that the renderers will be registered and available in the dropdown.
            TabRenderer2D.RegisterTabType();
            TabRenderer3D.RegisterTabType();
            TabRendererWorkbookMode.RegisterTabType();
            OneNoteStyleRenderer.RegisterTabType();
            TabRendererOffice2003.RegisterTabType();
            TabRendererWhidbey.RegisterTabType();
            TabRendererIE7.RegisterTabType();
            TabRendererOffice2007.RegisterTabType();
            TabRendererDockingWhidbeyBeta.RegisterTabType();
            TabRendererDockingWhidbey.RegisterTabType();
            TabRendererBlendLight.RegisterTabType();
            TabRendererBlendDark.RegisterTabType();
            TabRendererVS2010.RegisterTabType();
            TabRendererMetro.RegisterTabType();
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null) return base.EditValue(context, provider, value);

            IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
            TabControlAdv tabControl = context.Instance as TabControlAdv;

            editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService != null)
            {
                InitList(ReflectionHelper.GetTabNameFromType((Type)value), host != null && tabControl != null);                
                editorService.DropDownControl((Control)this.tabList);
                if (this.tabList.SelectedItem != null)
                {
                    if ((string)this.tabList.SelectedItem == this.addMoreTabs)
                    {
                        IDesigner designer = host.GetDesigner(tabControl);
                        if (designer != null && designer is ITabControlAdvDesigner)
                        {
                            ((ITabControlAdvDesigner)designer).InitTypeLoaderComponent();
                            MessageBox.Show("You can add your Custom Tabs (following the implementation rules specified in the TabStyle documentation) to the list of available TabStyles in the designer. To do so simply add an entry to your custom Tab class in the designTimeTabTypeLoader component's TypesToLoadList list.", "Loading Custom Tab Types");                          
                        }
                    }
                    else
                        value = ReflectionHelper.GetTabTypeFromName((string)this.tabList.SelectedItem);
                }
            }
            return value;
        }
    }
    [Syncfusion.Documentation.DocumentationExclude()]
    public class TabStyleConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(
            ITypeDescriptorContext context)
        {
            ArrayList rendererTypes = TabRendererFactory.GetRegisteredRenderers(true);
            return new StandardValuesCollection(rendererTypes);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return ReflectionHelper.GetTabTypeFromName((string)value);

            return base.ConvertFrom(context, culture, value);
        }
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string)
                && value is Type)
            {
                return (object)ReflectionHelper.GetTabNameFromType((Type)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    [Syncfusion.Documentation.DocumentationExclude()]
    public class TabControlCollectionSerializationProvider : IDesignerSerializationProvider
    {
        private Control.ControlCollection collectionToWatch;
        private ITabControlAdvDesigner notifyDesigner;
        public TabControlCollectionSerializationProvider(Control.ControlCollection collectionToWatch, ITabControlAdvDesigner notifyDesigner)
        {
            this.collectionToWatch = collectionToWatch;
            this.notifyDesigner = notifyDesigner;
        }
        public virtual object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)         
        {
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if(objectType != null &&
				(objectType.IsSubclassOf(typeof(Control.ControlCollection))
				|| objectType == typeof(Control.ControlCollection))
				)
				notifyDesigner.RemoveUnserializableChildControls();
#endif
            return null;
        }
    }
}
