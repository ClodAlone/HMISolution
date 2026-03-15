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
using SQLDriver;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using Utilities.WPF;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using Utilities;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;

namespace SQLDriver.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {

        bool bLoaded;
        bool bVisibleOnce;
        bool alreadyLoaded = false;
        IDataLayer idl;
        UnitOfWork ufw;
        SQLDriverDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        SQLDriverDynTagSettings thisTagSettings;
        DependencyPropertyDescriptor descriptor = null;
        //DriverCodeBaseEx.UI.Controls.BaseDynamicSettings baseDyn = null;

        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"TagNameTable", new SettingsControls.TagNameTable() },
            {"SelectColumn", new SettingsControls.SelectColumn() }
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        ComboBox CmbSQLDriverColumn = null;
        TextBlock TxtSQLDriverColumn = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
                //baseDyn = new DriverCodeBaseEx.UI.Controls.BaseDynamicSettings();
                baseSettingsCtrls = new DriverCodeBaseEx.UI.BaseSettings().GetBaseDynamicSettings();
                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    string Name = thisTag.Name;
                    string Folder = thisTag.FolderPath;
                    if (!string.IsNullOrEmpty(thisTag.TagOwnerPath))
                    {
                        Folder = String.Format("{0}\\{1}", thisTag.TagOwnerPath, Folder);
                        Folder = Folder.Replace("/", "\\");
                    }
                    thisTagSettings = new SQLDriverDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
                    if (thisTagSettings.TagName == string.Empty)
                    {
                        if (!string.IsNullOrEmpty(Folder))
                            thisTagSettings.TagName = String.Format("{0}\\{1}", Folder, Name);
                        else
                            thisTagSettings.TagName = Name;
                    }

                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                    LoadbaseDyn = true;
                }
                //if (!thisTag.IsMethod)
                //{
                //    baseDyn.CmbMethod.Visibility = System.Windows.Visibility.Collapsed;
                //    baseDyn.TMethod.Visibility = System.Windows.Visibility.Collapsed;
                //    thisTagSettings.MethodID = -1;
                //}
                DataContext = null;
                DataContext = thisTagSettings;
                UserControl row;
                //Address
                if (DynamicSettingsDict.TryGetValue("TagNameTable", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                if (DynamicSettingsDict.TryGetValue("SelectColumn", out row))
                {
                    row.DataContext = DataContext;
                    TxtSQLDriverColumn = (TextBlock)row.FindName("TxtSQLDriverColumn");
                    CmbSQLDriverColumn = (ComboBox)row.FindName("CmbSQLDriverColumn");
                    MainStack.Children.Add(row);
                }

                //Method*
                if (baseSettingsCtrls.TryGetValue("Method", out row))
                {
                    var cm = (ComboBox)row.FindName("CmbMethod");
                    if (cm != null)
                        cm.ItemsSource = Enum.GetValues(typeof(DriverMethods));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                    if (!thisTag.IsMethod)
                    {
                        row.Visibility = System.Windows.Visibility.Collapsed;
                        thisTagSettings.MethodID = -1;
                    }
                }
                //Link Type*
                if (baseSettingsCtrls.TryGetValue("LinkType", out row))
                {
                    row.DataContext = DataContext;
                    CmbLinkType = (ComboBox)row.FindName("CmbLinkType");
                    if (CmbLinkType != null)
                    {
                        CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                    }
                    MainStack.Children.Add(row);
                }
                //Station*
                if (baseSettingsCtrls.TryGetValue("Station", out row))
                {
                    row.DataContext = DataContext;
                    CmbStation = (ComboBox)row.FindName("CmbStation");
                    MainStack.Children.Add(row);
                }
                //Output at Startup*
                if (baseSettingsCtrls.TryGetValue("OutputStartup", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Conditional Variable*
                if (baseSettingsCtrls.TryGetValue("JobConditionalVariable", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //baseDyn.DataContext = DataContext;
                //if (LoadbaseDyn)
                //    MainStack.Children.Insert(0,baseDyn);

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBaseEx.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorOpeningDocument, Connection, DriverName));
                    return;
                }

                ufw = new UnitOfWork(idl);
                DriverCodeBaseEx.CommunicationDriver.UpdateDriverSchema(ufw);
                try
                {
                    configuration = (from tag in new XPQuery<SQLDriverDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new SQLDriverDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                }
                //baseDyn.CheckSwapBytes.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.CheckSwapBytesText.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.CheckSwapWords.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.CheckSwapWordsText.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.OutputAtStartup.Visibility = System.Windows.Visibility.Visible;
                //baseDyn.OutputAtStartupText.Visibility = System.Windows.Visibility.Visible;
                //baseDyn.TElemNumber.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.edtElementNumber.Visibility = System.Windows.Visibility.Collapsed;

                //baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                //baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));
                //baseDyn.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Visible;
                //baseDyn.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Visible;

                CmbStation_TextChanged();

                descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbStation, CmbStation_TextChanged);
                CmbStation_TextChanged();
                //if (alreadyLoaded)
                //    descriptor.RemoveValueChanged(baseDyn.CmbStation, CmbStation_TextChanged);
                CmbSQLDriverColumn.Visibility = System.Windows.Visibility.Collapsed;
                TxtSQLDriverColumn.Visibility = System.Windows.Visibility.Collapsed;
            };

            IsVisibleChanged += (o, e) =>
            {
                bVisibleOnce |= (bool)e.NewValue;
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded && bVisibleOnce)
                {
                    descriptor.RemoveValueChanged(CmbStation, CmbStation_TextChanged);
                    bLoaded = false;
                    var s = DataContext as SQLDriverDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };

        }

        private void CmbStation_TextChanged(object sender = null, EventArgs e = null)
        {
            if (!String.IsNullOrEmpty(thisTagSettings.StationName))
            {
                var st = from s in configuration.StationSettings
                         where s.Name == thisTagSettings.StationName
                         select ((SQLDriverStationSettings)s);

                Char delimiter = '|';
                List<string> ColumnList = new List<string>();
                if (st.FirstOrDefault() != null)
                    if(st.FirstOrDefault().SQLDriverColumnValue!= null)
                    {

                        if (st.FirstOrDefault().SQLDriverColumnValue.Contains(delimiter))
                        {
                            string[] stringList = st.FirstOrDefault().SQLDriverColumnValue.Split(delimiter);
                            foreach (var s in stringList)
                            {
                                ColumnList.Add(s);
                            }
                            CmbSQLDriverColumn.ItemsSource = ColumnList;
                            CmbSQLDriverColumn.Visibility = System.Windows.Visibility.Visible;
                            TxtSQLDriverColumn.Visibility = System.Windows.Visibility.Visible;
                            if((CmbSQLDriverColumn.Visibility == System.Windows.Visibility.Visible) &&
                                (CmbSQLDriverColumn.SelectedItem == null))
                            {
                                CmbSQLDriverColumn.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            CmbSQLDriverColumn.SelectedValue = string.Empty;
                            CmbSQLDriverColumn.Visibility = System.Windows.Visibility.Collapsed;
                            TxtSQLDriverColumn.Visibility = System.Windows.Visibility.Collapsed;
                        }
                            
                    }
                alreadyLoaded = true;
            }
        }

        private string _Connection;
        public string Connection
        {
            get { return _Connection; }
            set
            {
                _Connection = value;
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
            descriptor.RemoveValueChanged(CmbStation, CmbStation_TextChanged);
        }
        #endregion

    }
}
