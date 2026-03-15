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

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for DynamicPropertyInspector.xaml
    /// </summary>
    public partial class DynamicPropertyInspector : UserControl
    {
        bool bLoaded;
        public DynamicPropertyInspector(ScreenDocument document, List<String> items, 
                                        IPropertyControl property, IDictionary<IScriptable, Grid> scriptControls)
        {
            InitializeComponent();

            tabControl.BeginInit();

            var tabGeneralItem = new DXTabItem()
            {
                Header = Properties.Resources.DynamicPropertyInspectorSummary,
                Content = new DynamicPropertyInspectorPage(document, items, property)
            };
            tabGeneralItem.InitItemTemplate();
            tabControl.Items.Add(tabGeneralItem);

            items.ForEach(item =>
                {
                    var tabItem = new DXTabItem() 
                    { 
                        Header = document.CleanInnerName(item),
                        Content = new DynamicPropertyInspectorPage(document, item, property, scriptControls) 
                    };
                    tabItem.InitItemTemplate();
                    tabControl.Items.Add(tabItem);
                });

            tabControl.EndInit();

            tabControl.SelectionChanged += (o, e) =>
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            using (new WaitCursor())
                            {
                                if (e.OldSelectedItem != null)
                                {
                                    var content = (e.OldSelectedItem as DXTabItem)?.Content as DynamicPropertyInspectorPage;
                                    content?.UnSelected();
                                }

                                if (e.NewSelectedItem != null)
                                {
                                    var content = (e.NewSelectedItem as DXTabItem)?.Content as DynamicPropertyInspectorPage;
                                    content?.Selected();
                                }
                            }
                        });
                };

            Loaded += (s, e) =>
                {
                    if (bLoaded)
                        return;
                    bLoaded = true;

                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Closing += (o, ev) =>
                        {
                            if (wnd.DialogResult == true)
                            {
                                using (new WaitCursor())
                                {
                                    var content = (tabControl.SelectedItem as DXTabItem)?.Content as DynamicPropertyInspectorPage;
                                    content?.UnSelected();

                                    foreach (DXTabItem t in tabControl.Items)
                                    {
                                        if ((t as DXTabItem).Content is DynamicPropertyInspectorPage &&
                                            ((t as DXTabItem).Content as DynamicPropertyInspectorPage).isChanged)
                                        {
                                            document.NeedsSave = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        };
                    }
                };
        }
    }
}
