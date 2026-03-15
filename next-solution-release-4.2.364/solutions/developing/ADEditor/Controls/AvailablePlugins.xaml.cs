using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using ADEditor.ComponentService;
using HelpProvider.ComponentService;
using Utilities;
using Utilities.WPF;

namespace ADEditor.Controls
{
    /// <summary>
    /// Interaction logic for AvailablePlugins.xaml
    /// </summary>
    public partial class AvailablePlugins : UserControl
    {

        //bool alreadyLoaded = false;

        public AvailablePlugins(ADModel.ADGeneralSettings genSet = null)
        {
            InitializeComponent();

            if (genSet != null)
            {
                var plugins = from item in genSet.ADPlugins
                              select new pluginDesc
                              {
                                  Name = item.Name,
                                  Description = item.Description,
                                  AssemblyName = item.AssemblyName,
                                  NodeId = item.NodeId
                              };
                PluginGrid.ItemsSource = plugins;
                return;
            }

            try
            {
                var doc = XElement.Load(ADServerInfo.ADServerInfo.GetPluginListFile());
                var plugin = from item in doc.Descendants("Plugin")
                             select new pluginDesc
                             {
                                 Name = item.Attribute("FriendlyName").Value,
                                 Description = item.Attribute("Description").Value,
                                 AssemblyName = item.Attribute("AssemblyName").Value
                             };

                PluginGrid.ItemsSource = plugin;
            }
            catch
            {
                if (ADEditorManagerComponent.adeditorManagerComponent.UIInterface != null)
                {
                    ADEditorManagerComponent.adeditorManagerComponent.UIInterface.ShowError(
                        String.Format(Properties.Resources.ErrorReadingPluginList.Replace("'newline'", Environment.NewLine),
                        ADServerInfo.ADServerInfo.GetPluginListFile()));
                }
            }
            /*Loaded += (o, e) =>
                {
                    if (alreadyLoaded)
                        return;

                    alreadyLoaded = true;
                    PluginGrid.ItemsSource = DataContext as List<ADEditor.pluginDesc>;
                };*/
        }

        internal pluginDesc GetSelectedPluginInfo()
        {
            return PluginGrid.SelectedItem as pluginDesc;
        }

        private void PluginSelect_DblClick(object sender, RoutedEventArgs e)
        {
            var wnd = this.FindParent<Window>();// FindParentElementOfType<Window>();
            if (wnd != null)
            {
                wnd.DialogResult = true;
                wnd.Close();
            }
        }
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                string prop = string.Empty;

                prop = this.GetType().FullName;

                if (prop.Length > 0)
                {
                    if (ADEditorManagerComponent.adeditorManagerComponent.HelpProvider != null)
                        ADEditorManagerComponent.adeditorManagerComponent.HelpProvider.OpenDialogHelpPage(prop, true, true);
                    e.Handled = true;
                }
            }
        }
    }
}
