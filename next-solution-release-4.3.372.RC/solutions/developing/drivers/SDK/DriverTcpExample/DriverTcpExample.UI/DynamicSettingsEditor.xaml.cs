////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DynamicSettingsEditor.xaml.cs
//
// summary:	Implements the dynamic settings editor.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace DriverTcpExample.UI
{
    /// <summary>   Interaction logic for DynamicSettingsEditor.xaml. </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {

        /// <summary>   true if the data was loaded. </summary>
        bool bLoaded;
        /// <summary>   true to visible once. </summary>
        bool bVisibleOnce;
        /// <summary>   The idl. </summary>
        IDataLayer idl;
        /// <summary>   The ufw. </summary>
        UnitOfWork ufw;
        /// <summary>   The configuration. </summary>
        DriverTcpExampleDriverSettings configuration;
        /// <summary>   this tag. </summary>
        IDynamicSettingsEditing thisTag;
        /// <summary>   this tag settings. </summary>
        DriverTcpExampleDynTagSettings thisTagSettings;
        /// <summary>   Default constructor. </summary>
        public DynamicSettingsEditor()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
                var baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();
                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new DriverTcpExampleDynTagSettings() { IsMethod = thisTag.IsMethod };
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

                //insert base DynamicSettings setup
                if (LoadbaseDyn)
                    MainStack.Children.Add(baseDyn);
                

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                ufw = new UnitOfWork(idl);

                try
                {
                    configuration = (from tag in new XPQuery<DriverTcpExampleDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new DriverTcpExampleDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    //insert Station in ComboBox baseDyn.CmbStation
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                //insert option in ComboBox CmbFunctionCode
                CmbFunctionCode.ItemsSource = Enum.GetValues(typeof(FunctionCodes));

                //insert option in ComboBox baseDyn.CmbLinkType
                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                //insert option in ComboBox baseDyn.CmbMethod
                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));
            };

            IsVisibleChanged += (o, e) =>
            {
                bVisibleOnce |= (bool)e.NewValue;
            };

            Unloaded += (o, e) => 
            {
                if (bLoaded && bVisibleOnce)
                {
                    bLoaded = false;
                    var s = DataContext as DriverTcpExampleDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        /// <summary>   The connection. </summary>
        private string _Connection;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the connection. </summary>
        ///
        /// <value> The connection. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Connection
        {
            get { return _Connection; }
            set
            {
                _Connection = value;
            }
        }


        #region IDisposable Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
        }
        #endregion
    }
}
