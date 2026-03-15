using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.Xml;
using DocumentManager.ComponentService;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Controls;
using System.ServiceModel.Discovery;
using VFS;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using System.Windows.Media;
#elif NET_STANDARD
using System.Windows.Media;
using System.Xml.Serialization;
#else
using Windows.UI;
#endif
using ViewModelLib;
using Utilities;
using UriResolver.ComponentService;
using UFProjectManager.ComponentService;
using System.Windows.Input;
using System.ComponentModel;
using System.ServiceModel;
using OPCUAViewModel;
using UFInterfaces;
using UFInterfaces.PropertyControl;

namespace UFProjectManager
{
    [DataContract(Name = "ControllerData", Namespace = Namespaces.UriProgea)]
    public class UFControllerData : ViewModelBase, INotifyPropertyVisibilityChanged
#if !WINDOWS_UWP
        , ICloneable
#endif
    {
        #region Declarations

        //PropertyObserver<OPCUAEntityReference> observer;
        //PropertyObserver<MonitoredItemViewModel> datavalueobserver;
        internal bool IsGeoScadaSupported { get; set; }

#endregion

#region Persistance

#if !NET_STANDARD
        [DataMember]
#else
        [XmlIgnore]
#endif
        Color color = Colors.Transparent;
        [DataMember]
        bool visible = true;
        [DataMember]
        TileSize tileSize = TileSize.Small;
        [DataMember]
        String description;
        [DataMember]
        double latitude = Double.NaN;
        [DataMember]
        OPCUAEntityReference latitudeTag;
        [DataMember]
        double longitude = Double.NaN;
        [DataMember]
        OPCUAEntityReference longitudeTag;
        //[DataMember]
        //OPCUAEntityReference opcuaEntityReference;
        [DataMember]
        double mapZoomLevelVisibility = 1;
        [DataMember]
        double mapMaxZoomLevelVisibility = 20;

        [DataMember]
        String usersVisibility;
        [DataMember]
        String rolesVisibility;

        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            color = Colors.Transparent;
            visible = true;
            tileSize = TileSize.Small;
            latitude = Double.NaN;
            longitude = Double.NaN;
            mapZoomLevelVisibility = 1;
            mapMaxZoomLevelVisibility = 20;
        }
        #endregion

        #region Constructors

        public UFControllerData()
        { }

        internal UFControllerData(UFControllerData template)
        {
            if (template == null)
                return;

            Color = template.Color;
            Visible = template.Visible;
            TileSize = template.TileSize;
            Description = template.Description;
            Latitude = template.Latitude;
            Longitude = template.Longitude;
            if (template.latitudeTag != null)
                latitudeTag = new OPCUAEntityReference(template.latitudeTag);
            if (template.longitudeTag != null)
                longitudeTag = new OPCUAEntityReference(template.longitudeTag);
            mapZoomLevelVisibility = template.mapZoomLevelVisibility;
            mapMaxZoomLevelVisibility = template.mapMaxZoomLevelVisibility;
            usersVisibility = template.usersVisibility;
            rolesVisibility = template.rolesVisibility;
        }

#endregion

#region Properties

        public bool Visible
        {
            get { return visible; }
            set
            {
                Set(ref visible, value, "Visible");
            }
        }

        public TileSize TileSize
        {
            get { return tileSize; }
            set
            {
                Set(ref tileSize, value, "TileSize");
            }
        }

        public double MapMinZoomLevelVisibility
        {
            get { return mapZoomLevelVisibility; }
            set
            {
                Set(ref mapZoomLevelVisibility, value, "MapMinZoomLevelVisibility");
            }
        }

        public double MapMaxZoomLevelVisibility
        {
            get { return mapMaxZoomLevelVisibility; }
            set
            {
                Set(ref mapMaxZoomLevelVisibility, value, "MapMaxZoomLevelVisibility");
            }
        }

        public String UsersVisibility
        {
            get { return usersVisibility; }
            set
            {
                Set(ref usersVisibility, value, "UsersVisibility");
            }
        }

        public String RolesVisibility
        {
            get { return rolesVisibility; }
            set
            {
                Set(ref rolesVisibility, value, "RolesVisibility");
            }
        }

        public Color Color
        {
            get 
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (color == Colors.Transparent)
                {
                    var random = new Random();
                    Set(ref color, Color.FromRgb((byte)random.Next(255),
                                                    (byte)random.Next(255),
                                                    (byte)random.Next(255)), "Color");                 
                }
#endif
                return color; 
            }
            set
            {
                Set(ref color, value, "Color");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                Set(ref description, value, "Description");
            }
        }

        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                Set(ref latitude, value, "Latitude");
            }
        }

        public OPCUAEntityReference LatitudeTag
        {
            get
            {
                return latitudeTag;
            }
            set
            {
                Set(ref latitudeTag, value, "LatitudeTag");
            }
        }

        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                Set(ref longitude, value, "Longitude");
            }
        }

        public OPCUAEntityReference LongitudeTag
        {
            get
            {
                return longitudeTag;
            }
            set
            {
                Set(ref longitudeTag, value, "LongitudeTag");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool HasGeoCoordinates
        {
            get
            {
                return !Double.IsNaN(Longitude) && !Double.IsNaN(Latitude);
            }
        }
#endregion

#region Methods

        public virtual void Init(IEntityReference entity, String sessionname)
        {
            /*
            Entity = entity;
            SessionName = sessionname;
            if (OpcuaEntityReference != null)
            {
                observer = new PropertyObserver<OPCUAEntityReference>(OpcuaEntityReference)
                    .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                        if (n.MonitoredItemViewModel.HasRange)
                            range = n.MonitoredItemViewModel.Range;
                        datavalueobserver = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel)
                        .RegisterHandler(d => d.DataValue, d =>
                        {
                            Control.Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                InternalExecute(d.DataValue);
                            });
                        });

                        Control.Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            InternalExecute(n.MonitoredItemViewModel.DataValue);
                        });
                    });

                OpcuaEntityReference.Resolve(SessionName);
                OpcuaEntityReference.SetInUse(Entity, true);
            }
            */
        }

        public virtual void Terminate()
        {
            /*
            if (OpcuaEntityReference != null)
            {
                OpcuaEntityReference.SetInUse(Entity, false);

                if (observer != null)
                    observer.Dispose();
                if (datavalueobserver != null)
                    datavalueobserver.Dispose();
                observer = null;
                datavalueobserver = null;
            }
            */
        }

#endregion

#region PropertyChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        protected void Set<T>(ref T field, T value, string propertyName)
        {
            if (!Object.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

#endregion

#region ICloneable Members

#if !WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Clone()
        {
            return new UFControllerData(this);
        }
#endif
#endregion

#region Validations
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Error
        {
            get
            {
                return null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Latitude")
            {
                if (!Double.IsNaN(Latitude) && (Latitude < -90 || Latitude > 90))
                    return Properties.Resources.LatitudeNoInValidRange;
            }
            else if (propertyName == "Longitude")
            {
                if (!Double.IsNaN(Latitude) && (Longitude < -180 || Longitude > 180))
                    return Properties.Resources.LongitudeNoInValidRange;
            }

            return null;
        }
#endif
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "MapMinZoomLevelVisibility" ||
                    propertyName == "MapMaxZoomLevelVisibility" ||
                    propertyName == "Latitude" ||
                    propertyName == "LatitudeTag" ||
                    propertyName == "Longitude" ||
                    propertyName == "LongitudeTag" ||
                    propertyName == "HasGeoCoordinates")
                {
                    return IsGeoScadaSupported;
                }

                return true;
            }
        }
        #endregion
    }
}
