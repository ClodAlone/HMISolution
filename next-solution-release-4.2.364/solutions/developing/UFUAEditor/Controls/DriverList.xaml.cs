using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using UFInterfaces.Editors;
using Utilities.WPF;
using Utilities;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for DriverList.xaml
    /// </summary>
    public partial class DriverList : UserControl, IDisposable
    {
        object record;

        bool isPopup;
        bool isDataContextChanging;

        public DriverList(bool bPopup = false)
        {
            InitializeComponent();
            isPopup = bPopup;

            List<DriverXmlInfo> drivers = new List<DriverXmlInfo>();
            try
            {
                var doc = XElement.Load(UFUAServerInfo.UFUAServerInfo.GetDriverListFile());
                doc.Descendants("Driver").ToList().ForEach(item =>
                {
                    var info = new DriverXmlInfo();
                    if (item.Attribute("Factory") != null)
                        info.Factory = item.Attribute("Factory").Value;
                    if (item.Attribute("FriendlyName") != null)
                        info.FriendlyName = item.Attribute("FriendlyName").Value;
                    if (item.Attribute("Help") != null)
                        info.Help = item.Attribute("Help").Value;
                    if (item.Attribute("AssemblyName") != null)
                        info.AssemblyName = item.Attribute("AssemblyName").Value;
                    if (item.Attribute("PackageType") != null)
                        info.PackageType = item.Attribute("PackageType").Value;

                    if (item.Attribute("Linux") != null)
                        info.Linux = bool.Parse(item.Attribute("Linux").Value);
                    if (item.Attribute("Win32") != null)
                        info.Win32 = bool.Parse(item.Attribute("Win32").Value);
                    if (item.Attribute("Win64") != null)
                        info.Win64 = bool.Parse(item.Attribute("Win64").Value);
                    drivers.Add(info);
                });
            }
            catch
            { }

            gridDataControl.ItemsSource = drivers;
            //gridDataControl.Columns["Factory"].GroupIndex = 0;
            //gridDataControl.GroupBy("Factory");
            //gridDataControl.CollapseAllGroups();

            if (isPopup)
            {
                Loaded += (o, e) => 
                {
                    var wnd = this.FindParent<Window>();
                    wnd.Closing += (s, c) =>
                    {
                        isDataContextChanging = true;
                        DataContext = gridDataControl.SelectedItem;
                        isDataContextChanging = false;
                    };
                };

                DataContextChanged += (o, e) =>
                {
                    if (!isDataContextChanging && DataContext is DriverXmlInfo)
                    {

                    }
                };
            }
        }

        internal DriverXmlInfo GetSelectedDriverInfo()
        {
            return gridDataControl.SelectedItem as DriverXmlInfo;
        }

        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var selected = gridDataControl.SelectedItem as DriverXmlInfo;
            if (selected != null && isPopup)
            {
                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            }
        }

        public void Dispose()
        {
            // gridDataControl.Model.Dispose();
            //try
            //{
            //    gridDataControl.Dispose();
            //}
            //catch (Exception ex)
            //{
                
            //}
        }

        private void gridDataControl_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            DataContext = gridDataControl.SelectedItem;
        }
    }
}
