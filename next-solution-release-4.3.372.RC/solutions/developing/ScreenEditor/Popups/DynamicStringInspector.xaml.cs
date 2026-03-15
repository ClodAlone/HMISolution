using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ScreenSettings;
using UFInterfaces.Scriptable;
using PropertyControl.ComponentService;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpf.Core;
using System.Windows;
using static ScreenManager.Popups.DynamicStringInspectorPage;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for DynamicPropertyInspector.xaml
    /// </summary>
    public partial class DynamicStringInspector : UserControl
    {
        internal List<StringsData> editedStringsData = new List<StringsData>();
        public DynamicStringInspector(ScreenDocument document, List<String> items, Dictionary<string, Tuple<DependencyProperty, string>> textsList, Dictionary<string, UIElement> elementsList)
        {
            InitializeComponent();

            tabControl.BeginInit();

            var tabGeneralItem = new DXTabItem()
            {
                Header = Properties.Resources.DynamicStringInspectorSummary,
                Content = new DynamicStringInspectorPage(document, items, textsList, elementsList, editedStringsData)
            };
            tabGeneralItem.InitItemTemplate();
            tabControl.Items.Add(tabGeneralItem);

            tabControl.EndInit();
        }
    }
}
