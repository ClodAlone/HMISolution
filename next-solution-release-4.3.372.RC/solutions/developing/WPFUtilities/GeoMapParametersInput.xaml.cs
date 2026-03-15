using DevExpress.Xpf.Map;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;


namespace WPFUtilities
{
    public partial class GeoMapCoordinatesInput : UserControl
    {
        #region Declarations
        public Point3D GeoMapOpeningParameters = new Point3D(0, 0, 1);
        public string BingMapKey = Properties.Settings.Default.BingMapKey;
        internal IDocument Document;
        #endregion

        #region Constructors
        public GeoMapCoordinatesInput(Point3D geoMapOpeningParameters)
        {
            InitializeComponent();
            SetInitialParameters(geoMapOpeningParameters);
            SetMapOpeningParameters(GeoMapOpeningParameters);
        }
        #endregion

        #region Methods
        private void SetMapOpeningParameters(Point3D geoMapOpeningParameters)
        {
            mapControl.CenterPoint = new GeoPoint(geoMapOpeningParameters.Y, geoMapOpeningParameters.X);
            mapControl.ZoomLevel = geoMapOpeningParameters.Z;

            if (geoMapOpeningParameters.X == 0 && geoMapOpeningParameters.Y == 0)
            {
                mapControl.ZoomLevel = 1;
            }
            else
            {
                mapControl.ZoomLevel = geoMapOpeningParameters.Z;
            }
        }

        private void SetInitialParameters(Point3D geoMapOpeningParameters)
        {
            GeoMapOpeningParameters.X = geoMapOpeningParameters.X;
            GeoMapOpeningParameters.Y = geoMapOpeningParameters.Y;
            GeoMapOpeningParameters.Z = geoMapOpeningParameters.Z;
        }
        #endregion

        #region Events Handlers
        private void btnSaveView_Click(object sender, RoutedEventArgs e)
        {
            GeoMapOpeningParameters.X = mapControl.CenterPoint.GetX();
            GeoMapOpeningParameters.Y = mapControl.CenterPoint.GetY();
            GeoMapOpeningParameters.Z = mapControl.ZoomLevel;

            if (GeoMapOpeningParameters.Z <= 1)
            {
                GeoMapOpeningParameters.Z = 1;
            }
            else if (GeoMapOpeningParameters.Z >= 20)
            {
                GeoMapOpeningParameters.Z = 20;
            }

            if (Document != null)
            {
                var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                {
                    uiMsgBox.ShowInformation(Properties.Resources.MapSettingsUpdated);
                }   
            }
        }
        #endregion
    }
}