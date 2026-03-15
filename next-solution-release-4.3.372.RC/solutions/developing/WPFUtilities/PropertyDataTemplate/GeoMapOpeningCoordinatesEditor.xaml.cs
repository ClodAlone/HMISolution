using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using UFInterfaces;
using Utilities.WPF;
using Utilities;
using System.Globalization;
using WPFUtilities.Converters;
using DevExpress.Xpf.Editors;

namespace WPFUtilities.PropertyDataTemplate
{
    public partial class GeoMapOpeningParametersEditor : UserControl
    {
        #region Declarations
        public Point3D GeoMapOpeningParameters = new Point3D(0, 0, 1);
        private DoubleToLongitudeDegreesConverter _longitudeConverter = new DoubleToLongitudeDegreesConverter();
        private DoubleToLatitudeDegreesConverter _latitudeConverter = new DoubleToLatitudeDegreesConverter();
        private CultureInfo _culture = CultureInfo.InvariantCulture;
        #endregion

        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(GeoMapOpeningParametersEditor), new UIPropertyMetadata(null));
        public IWorkspace Workspace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IWorkspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }
        #endregion

        #region Constructors
        public GeoMapOpeningParametersEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                var dataContextParameters = DataContext as Mindscape.WpfElements.PropertyEditing.ObjectWrapper<Point3D>;

                if (dataContextParameters != null)
                {
                    GeoMapOpeningParameters = dataContextParameters.Value;
                }

                longitude.EditValue = _longitudeConverter.Convert(GeoMapOpeningParameters.X, null, null, _culture);
                latitude.EditValue = _latitudeConverter.Convert(GeoMapOpeningParameters.Y, null, null, _culture);
                zoomLevel.EditValue = GeoMapOpeningParameters.Z;
            };
        }
        #endregion

        #region EventsHandlers
        private void btnOpenMap_Click(object sender, RoutedEventArgs e)
        {
            Button buttonOpenMap = sender as Button;
            GeoMapOpeningParameters = (Point3D)buttonOpenMap.Tag;

            var geoMapCoordinatesInputUserControl = new GeoMapCoordinatesInput(GeoMapOpeningParameters) { Document = Workspace.ContextDocument };
            GeneralDialogContent window = new GeneralDialogContent(geoMapCoordinatesInputUserControl, GeneralDialogButtons.HelpButton)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "GeoMapParametersInput",
            };

            window.ShowDialog();
            window.Close();

            GeoMapOpeningParameters.X = geoMapCoordinatesInputUserControl.GeoMapOpeningParameters.X;
            GeoMapOpeningParameters.Y = geoMapCoordinatesInputUserControl.GeoMapOpeningParameters.Y;
            GeoMapOpeningParameters.Z = geoMapCoordinatesInputUserControl.GeoMapOpeningParameters.Z;

            btnOpenMap.Tag = GeoMapOpeningParameters;

            longitude.EditValue = _longitudeConverter.Convert(GeoMapOpeningParameters.X, null, null, _culture);
            latitude.EditValue = _latitudeConverter.Convert(GeoMapOpeningParameters.Y, null, null, _culture);
            zoomLevel.EditValue = GeoMapOpeningParameters.Z;
        }

        private void longitude_LostFocus(object sender, RoutedEventArgs e)
        {
            var editedLongitude = e.Source as TextEdit;
            GeoMapOpeningParameters.X = (double)_longitudeConverter.ConvertBack(editedLongitude.EditText, null, null, _culture);
            btnOpenMap.Tag = GeoMapOpeningParameters;
        }

        private void latitude_LostFocus(object sender, RoutedEventArgs e)
        {
            var editedLatitude = e.Source as TextEdit;
            GeoMapOpeningParameters.Y = (double)_latitudeConverter.ConvertBack(editedLatitude.EditText, null, null, _culture);
            btnOpenMap.Tag = GeoMapOpeningParameters;
        }

        private void zoomLevel_LostFocus(object sender, RoutedEventArgs e)
        {
            var editedZoomLevel = e.Source as SpinEdit;
            GeoMapOpeningParameters.Z = double.Parse(editedZoomLevel.EditText);
            btnOpenMap.Tag = GeoMapOpeningParameters;
        }
        #endregion
    }
}
