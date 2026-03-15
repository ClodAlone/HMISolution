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
using DriverCodeBase;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using Utilities.WPF;
using DriverCodeBase.Helpers;
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
        DriverCodeBase.UI.Controls.BaseDynamicSettings baseDyn = null;
        public DynamicSettingsEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
                baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();
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
                if (!thisTag.IsMethod)
                {
                    baseDyn.CmbMethod.Visibility = System.Windows.Visibility.Collapsed;
                    baseDyn.TMethod.Visibility = System.Windows.Visibility.Collapsed;
                    thisTagSettings.MethodID = -1;
                }
                DataContext = null;
                DataContext = thisTagSettings;
                baseDyn.DataContext = DataContext;
                if (LoadbaseDyn)
                    MainStack.Children.Insert(0,baseDyn);

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    MessageBox.Show(string.Format(DriverCodeBase.Properties.Resources.ErrorOpeningDocument, Connection, DriverName));
                    return;
                }

                ufw = new UnitOfWork(idl);

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
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                }
                baseDyn.CheckSwapBytes.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytesText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWords.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWordsText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.OutputAtStartup.Visibility = System.Windows.Visibility.Visible;
                baseDyn.OutputAtStartupText.Visibility = System.Windows.Visibility.Visible;
                baseDyn.TElemNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = System.Windows.Visibility.Collapsed;

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));
                baseDyn.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Visible;
                baseDyn.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Visible;

                CmbStation_TextChanged();

                descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(baseDyn.CmbStation, CmbStation_TextChanged);
                CmbStation_TextChanged();
                //if (alreadyLoaded)
                //    descriptor.RemoveValueChanged(baseDyn.CmbStation, CmbStation_TextChanged);
            };

            IsVisibleChanged += (o, e) =>
            {
                bVisibleOnce |= (bool)e.NewValue;
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded && bVisibleOnce)
                {
                    descriptor.RemoveValueChanged(baseDyn.CmbStation, CmbStation_TextChanged);
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
                        }
                        else
                        {
                            CmbSQLDriverColumn.Visibility = System.Windows.Visibility.Collapsed;
                            TxtSQLDriverColumn.Visibility = System.Windows.Visibility.Collapsed;
                            CmbSQLDriverColumn.SelectedValue = string.Empty;
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
            descriptor.RemoveValueChanged(baseDyn.CmbStation, CmbStation_TextChanged);
        }
        #endregion

    }
}
