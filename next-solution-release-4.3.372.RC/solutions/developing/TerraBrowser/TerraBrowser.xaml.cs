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
using DevExpress.Xpf.Map;
using System.Windows.Threading;
using System.Globalization;
using ViewModelLib;
using GoogleMaps.LocationServices;

namespace TerraBrowser
{
    /// <summary>
    /// Interaction logic for TerraBrowser.xaml
    /// </summary>
    public partial class TerraBrowser : UserControl
    {

        bool bLoaded;
        bool bInit;
        public TerraBrowser()
        {
            InitializeComponent();

            //if (Double.IsNaN(Latitude) || Double.IsNaN(Longitude))
            //    return;

            Loaded += (o, e) =>
                {
                    if(!bLoaded)
                    {
                        bLoaded = true;
                        UpdateCenterPoint();
                        mapControl.ZoomLevel = 5.0;
                        bInit = true;
                    }
                };
        }
        RelayCommand _fetch;
        public ICommand FetchCommand
        {
            get
            {
                if (_fetch == null)
                {
                    _fetch = new RelayCommand(
                        param => CallFetchCommand(),
                        param => IsEnableCallFetchCommand
                        );
                }
                return _fetch;
            }
        }

        bool bCallingFetch;
        internal void CallFetchCommand()
        {
            try
            {
                bCallingFetch = true;
                var gls = new GoogleLocationService();
                var latlong = gls.GetLatLongFromAddress(txtAddress.Text);
                if(latlong != null)
                {
                    Longitude = latlong.Longitude;
                    Latitude = latlong.Latitude;

                    mapControl.ZoomLevel = 6.0;
                    UpdateCenterPoint();
                }
            }
            catch (System.Net.WebException ex)
            {
            }
            finally
            {
                bCallingFetch = false;
            }
        }

        internal bool IsEnableCallFetchCommand
        {
            get
            {
                if (bCallingFetch)
                    return false;
                else
                {
                    return !string.IsNullOrEmpty(txtAddress.Text);
                }

            }
        }

        void UpdateCenterPoint()
        {
            try
            {
                //mapDot.Location = mapControl.CenterPoint = new GeoPoint(DoubleToDegreesConvert.ParseStringToDoubleValue(txtLatidute.Text), DoubleToDegreesConvert.ParseStringToDoubleValue(txtLongitude.Text));
                if (!Double.IsNaN(Latitude) && !Double.IsNaN(Longitude))
                    mapDot.Location = mapControl.CenterPoint = new GeoPoint(Latitude, Longitude);
            }
            catch (Exception ex)
            {
                
            }
        }


        #region Latitude
        public static readonly DependencyProperty LatitudeProperty = DependencyProperty.Register("Latitude", typeof(double), typeof(TerraBrowser), new UIPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnLatitudeChanged), new CoerceValueCallback(OnCoerceLatitude)));

        private static object OnCoerceLatitude(DependencyObject o, object value)
        {
            TerraBrowser control = o as TerraBrowser;
            if (control != null)
                return control.OnCoerceLatitude((double)value);
            else
                return value;
        }

        private static void OnLatitudeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TerraBrowser control = o as TerraBrowser;
            if (control != null)
                control.OnLatitudeChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceLatitude(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLatitudeChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit)
                UpdateCenterPoint();
        }

        public double Latitude
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                try
                {
                    return (double)GetValue(LatitudeProperty);
                }
                catch (Exception)
                {
                    return Double.NaN;
                }
            }
            set
            {
                if (Double.IsNaN(value))
                    return;
                SetValue(LatitudeProperty, value);
            }
        }

        #endregion


        #region Longitude
        public static readonly DependencyProperty LongitudeProperty = DependencyProperty.Register("Longitude", typeof(double), typeof(TerraBrowser), new UIPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnLongitudeChanged), new CoerceValueCallback(OnCoerceLongitude)));

        private static object OnCoerceLongitude(DependencyObject o, object value)
        {
            TerraBrowser control = o as TerraBrowser;
            if (control != null)
                return control.OnCoerceLongitude((double)value);
            else
                return value;
        }

        private static void OnLongitudeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TerraBrowser control = o as TerraBrowser;
            if (control != null)
                control.OnLongitudeChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceLongitude(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLongitudeChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateCenterPoint();
        }

        public double Longitude
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                try
                {
                    return (double)GetValue(LongitudeProperty);
                }
                catch (Exception)
                {
                    return Double.NaN;
                } 
            }
            set
            {
                if (Double.IsNaN(value))
                    return;
                SetValue(LongitudeProperty, value);
            }
        }

        #endregion
        

        //private double _latitude;
        //public double Latitude
        //{
        //    get
        //    {
        //        try
        //        {
        //            return _latitude; //DoubleToDegreesConvert.ParseStringToDoubleValue(txtLatidute.Text);
        //        }
        //        catch (Exception)
        //        {
        //            return Double.NaN;
        //        }
        //    }
        //    set
        //    {
        //        if (Double.IsNaN(value))
        //            return;
        //        //txtLatidute.Text = value.ToString();
        //        _latitude = value; //txtLatidute.Text = DoubleToDegreesConvert.ParseDoubleToStringValue(value, true);
        //        //UpdateCenterPoint();
        //    }
        //}

        //public static readonly DependencyProperty LongitudeProperty = DependencyProperty.RegisterAttached("Longitude", typeof(double), typeof(TerraBrowser), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
        //private double _longitude;
        //public double Longitude
        //{
        //    get
        //    {
        //        try
        //        {
        //            return _longitude; //DoubleToDegreesConvert.ParseStringToDoubleValue(txtLongitude.Text);
        //        }
        //        catch (Exception)
        //        {
        //            return Double.NaN;
        //        }
        //    }
        //    set
        //    {
        //        if (Double.IsNaN(value))
        //            return;
        //        //txtLongitude.Text = value.ToString();
        //        _longitude = value; //txtLongitude.Text = DoubleToDegreesConvert.ParseDoubleToStringValue(value, false);
        //        //UpdateCenterPoint();
        //    }
        //}

        private void mapControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var centerpoint = mapControl.Layers[0].ScreenToGeoPoint(e.GetPosition(mapControl));
                mapDot.Location = mapControl.CenterPoint = centerpoint;

                //txtLongitude.Text = DoubleToDegreesConvert.ParseDoubleToStringValue(centerpoint.Longitude, false);
                //txtLatidute.Text = DoubleToDegreesConvert.ParseDoubleToStringValue(centerpoint.Latitude, true);
                Longitude = centerpoint.Longitude;
                Latitude = centerpoint.Latitude;
                txtAddress.Text = string.Empty;
            }
            catch (Exception ex)
            {

            }
        }
    }
}

