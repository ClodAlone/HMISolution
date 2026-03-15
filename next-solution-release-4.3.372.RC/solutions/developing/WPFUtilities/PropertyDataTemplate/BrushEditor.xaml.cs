using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Text;
using Utilities.WPF;
using System.Xml.Serialization;
using System.Windows.Markup;
using DocumentManager.ComponentService;
using UFInterfaces.Converters;
using WPFUtilities.Converters;
using VFS;
using DevExpress.Xpf.Editors.Internal;
using System.Globalization;
using DevExpress.Xpf.Core.Commands;
//using OPCUAViewModelService.ComponentService;
//using UFInterfaces;
//using WPFUtilities.Converters;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for BrushPropertyEditor.xaml
    /// </summary>
    public partial class BrushEditor : UserControl, IDisposable
    {


        #region LinearBrush
        public static readonly DependencyProperty LinearBrushProperty = DependencyProperty.Register("LinearBrush", typeof(LinearGradientBrush), typeof(BrushEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnLinearBrushChanged), new CoerceValueCallback(OnCoerceLinearBrush)));

        private static object OnCoerceLinearBrush(DependencyObject o, object value)
        {
            BrushEditor control = o as BrushEditor;
            if (control != null)
                return control.OnCoerceLinearBrush((LinearGradientBrush)value);
            else
                return value;
        }

        private static void OnLinearBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BrushEditor control = o as BrushEditor;
            if (control != null)
                control.OnLinearBrushChanged((LinearGradientBrush)e.OldValue, (LinearGradientBrush)e.NewValue);
        }

        protected virtual LinearGradientBrush OnCoerceLinearBrush(LinearGradientBrush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLinearBrushChanged(LinearGradientBrush oldValue, LinearGradientBrush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnBrushChanged(newValue);
        }

        public LinearGradientBrush LinearBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (LinearGradientBrush)GetValue(LinearBrushProperty);
            }
            set
            {
                SetValue(LinearBrushProperty, value);
            }
        }

        #endregion

        #region RadialBrush
        public static readonly DependencyProperty RadialBrushProperty = DependencyProperty.Register("RadialBrush", typeof(RadialGradientBrush), typeof(BrushEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRadialBrushChanged), new CoerceValueCallback(OnCoerceRadialBrush)));

        private static object OnCoerceRadialBrush(DependencyObject o, object value)
        {
            BrushEditor control = o as BrushEditor;
            if (control != null)
                return control.OnCoerceRadialBrush((RadialGradientBrush)value);
            else
                return value;
        }

        private static void OnRadialBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BrushEditor control = o as BrushEditor;
            if (control != null)
                control.OnRadialBrushChanged((RadialGradientBrush)e.OldValue, (RadialGradientBrush)e.NewValue);
        }

        protected virtual RadialGradientBrush OnCoerceRadialBrush(RadialGradientBrush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRadialBrushChanged(RadialGradientBrush oldValue, RadialGradientBrush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnBrushChanged(newValue);
        }

        public RadialGradientBrush RadialBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (RadialGradientBrush)GetValue(RadialBrushProperty);
            }
            set
            {
                SetValue(RadialBrushProperty, value);
            }
        }

        #endregion

        #region SolidBrush
        public static readonly DependencyProperty SolidBrushProperty = DependencyProperty.Register("SolidBrush", typeof(SolidColorBrush), typeof(BrushEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSolidBrushChanged), new CoerceValueCallback(OnCoerceSolidBrush)));

        private static object OnCoerceSolidBrush(DependencyObject o, object value)
        {
            BrushEditor control = o as BrushEditor;
            if (control != null)
                return control.OnCoerceSolidBrush((SolidColorBrush)value);
            else
                return value;
        }

        private static void OnSolidBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BrushEditor control = o as BrushEditor;
            if (control != null)
                control.OnSolidBrushChanged((SolidColorBrush)e.OldValue, (SolidColorBrush)e.NewValue);
        }

        protected virtual SolidColorBrush OnCoerceSolidBrush(SolidColorBrush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSolidBrushChanged(SolidColorBrush oldValue, SolidColorBrush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnBrushChanged(newValue);
        }

        public SolidColorBrush SolidBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SolidColorBrush)GetValue(SolidBrushProperty);
            }
            set
            {
                SetValue(SolidBrushProperty, value);
            }
        }

        #endregion


        #region ColorTypeSelection
        public static readonly DependencyProperty ColorTypeSelectionProperty = DependencyProperty.Register("ColorTypeSelection", typeof(ColorType), typeof(BrushEditor), new UIPropertyMetadata(ColorType.Solid, new PropertyChangedCallback(OnColorTypeSelectionChanged), new CoerceValueCallback(OnCoerceColorTypeSelection)));

        private static object OnCoerceColorTypeSelection(DependencyObject o, object value)
        {
            BrushEditor control = o as BrushEditor;
            if (control != null)
                return control.OnCoerceColorTypeSelection((ColorType)value);
            else
                return value;
        }

        private static void OnColorTypeSelectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BrushEditor control = o as BrushEditor;
            if (control != null)
                control.OnColorTypeSelectionChanged((ColorType)e.OldValue, (ColorType)e.NewValue);
        }

        protected virtual ColorType OnCoerceColorTypeSelection(ColorType value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnColorTypeSelectionChanged(ColorType oldValue, ColorType newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateColorEditLayout();
            NotifyBrushChanged();
        }

        public ColorType ColorTypeSelection
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ColorType)GetValue(ColorTypeSelectionProperty);
            }
            set
            {
                SetValue(ColorTypeSelectionProperty, value);
            }
        }

        #endregion


        public event EventHandler BrushChanged;
        public event EventHandler UndoChanges;
        public System.Windows.Media.Brush Brush { get; set; }
        System.Windows.Media.Brush OldBrush { get; set; }


        public String ResourceName { get; set; }

        private bool _spreadColor;
        public Boolean SpreadColor
        {
            get { return _spreadColor; }
            set
            {
                if (value == _spreadColor) return;

                _spreadColor = value;
                var temp = BrushChanged;
                if (temp != null)
                    temp(null/*this*/, new EventArgs());
            }
        }

        public Boolean OnlySolidColors { get; set; }

        IUIMsgBoxAlertService UIService;
        IDocument Document;
        IUriToUriAbsoluteImageConverter UriToUriAbsoluteImageConverter;
        public Dictionary<Object, Object> mapResourceBrushes;
        public Dictionary<Object, Object> mapUserResourceBrushes = new Dictionary<Object, Object>();
    
        List<object> objRadialList = new List<object>();
        List<object> objLinearList = new List<object>();
        List<object> objVisualList = new List<object>();
        List<object> objSolidList = new List<object>();
        List<object> objNamedList = new List<object>();
        public readonly Dictionary<String, String> mapItems = new Dictionary<String, String>();

        private void solidBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorTypeSelection = ColorType.Solid;
        }

        private void linearBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorTypeSelection = ColorType.Linear;
        }

        private void radialBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorTypeSelection = ColorType.Radial;
        }

        private LinearGradientBrush GetLinear(System.Windows.Media.Color color)
        {
            var b = new LinearGradientBrush();
            b.GradientStops.Add(new GradientStop() { Color = color, Offset = 0 });
            b.GradientStops.Add(new GradientStop() { Color = color, Offset = 100 });
            return b;
        }

        private RadialGradientBrush GetRadial(System.Windows.Media.Color color)
        {
            var b = new RadialGradientBrush();
            b.GradientStops.Add(new GradientStop() { Color = color, Offset = 0 });
            b.GradientStops.Add(new GradientStop() { Color = color, Offset = 100 });
            return b;
        }
        
        private void UpdateColorEditLayout()
        {
            switch (ColorTypeSelection)
            {
                case ColorType.Solid:
                    linearGradientMultiSlider.Visibility = Visibility.Collapsed;
                    radialGradientMultiSlider.Visibility = Visibility.Collapsed;
                    radialOptions.Visibility = Visibility.Collapsed;
                    linearOptions.Visibility = Visibility.Collapsed;
                    break;
                case ColorType.Linear:
                    linearGradientMultiSlider.Visibility = Visibility.Visible;
                    radialGradientMultiSlider.Visibility = Visibility.Collapsed;
                    radialOptions.Visibility = Visibility.Collapsed;
                    linearOptions.Visibility = Visibility.Visible;
                    break;
                case ColorType.Radial:
                    linearGradientMultiSlider.Visibility = Visibility.Collapsed;
                    radialGradientMultiSlider.Visibility = Visibility.Visible;
                    radialOptions.Visibility = Visibility.Visible;
                    linearOptions.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        public BrushEditor(IDocument document, IUIMsgBoxAlertService uiService = null, IUriToUriAbsoluteImageConverter uriToUriAbsoluteImageConverter = null, bool enableColorReset = false, bool enableColorSpread = false)
        {
            InitializeComponent();
            tyeSelection.DataContext = this;
            linearOptions.DataContext = this;
            radialOptions.DataContext = this;

            if (uiService == null && document != null)
                uiService = UIService ?? document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (UIService == null)
                UIService = uiService;

            if (uriToUriAbsoluteImageConverter == null && !String.IsNullOrEmpty(document?.FilePath))
            {
                uriToUriAbsoluteImageConverter = new UriToUriAbsoluteImageConverter();
                string dest = System.IO.Path.GetDirectoryName(document.FilePath) + "\\";
                var absolutePath = new Uri(dest, UriKind.RelativeOrAbsolute);
                var absolutePath2 = document.GetSpecialFolder(SpecialFolders.Images);
                uriToUriAbsoluteImageConverter.FileSystemProviderBase = document.fileSystemProviderBase;
                uriToUriAbsoluteImageConverter.AbsolutePath = absolutePath;
                uriToUriAbsoluteImageConverter.AbsolutePath2 = absolutePath2;
            }
            UriToUriAbsoluteImageConverter = uriToUriAbsoluteImageConverter;

            ResourceToBrushConverter converter = (ResourceToBrushConverter)this.FindResource("MyConverter");
            converter.Window = this;
            Document = document;

            Loaded += (o, e) =>
            {
                if (enableColorReset)
                {
                    resetColor.Width = new GridLength(33, GridUnitType.Star);
                    colorResetButton.Visibility = Visibility.Visible;
                    spreadColor.Visibility = Visibility.Collapsed;
                }
                else if(!enableColorSpread)
                {
                    spreadColor.Visibility = Visibility.Collapsed;
                }

                spreadColor.DataContext = this;

                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                string startingPath = String.Format("{0}.{1}\\Cultures\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), mainversion);
                string fileToOpen = string.Empty;

                fileToOpen = string.Format("{0}{1}\\StringTable_Brushes.xml", startingPath, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);

                if (!File.Exists(fileToOpen))
                    fileToOpen = string.Format("{0}StringTable_Brushes.xml", startingPath);
                if (File.Exists(fileToOpen))
                {
                    LoadFromXml(fileToOpen);
                }

                Dictionary<Object, Object> _mapResource = new Dictionary<object, object>();

                var map = RecentColorsHelper.LoadBrushResources();
                foreach (object key in map.Keys)
                {
                    try
                    {
                        if (mapItems.ContainsKey(key.ToString()))
                        {
                            var _key = mapItems[key.ToString()];
                            if (_mapResource.ContainsKey(_key))
                                _mapResource[_key] = map[key];
                            else
                                _mapResource.Add(_key, map[key]);
                        }
                        else
                        {
                            if (_mapResource.ContainsKey(key))
                                _mapResource[key] = map[key];
                            else
                                _mapResource.Add(key, map[key]);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                mapResourceBrushes = _mapResource;
                LoadResourcesFromSpecialFolder(mapResourceBrushes);
                LoadUserBushes();
                FillResourceList(mapResourceBrushes);
                FillUserResourceList(mapUserResourceBrushes);
                UpdateMapResources();

                UpdateColornameName(true);

                OldBrush = Brush;

                InitColorEdit();

                colorEdit.ColorChanged += (d, evt) =>
                {
                    ResourceName = null;
                    if (bInit)
                        switch (ColorTypeSelection)
                        {
                            case ColorType.Solid:
                                SolidBrush = new SolidColorBrush(colorEdit.Color);
                                break;
                            case ColorType.Linear:
                                UpdateLinearThumb(colorEdit.Color);
                                break;
                            case ColorType.Radial:
                                UpdateRadialThumb(colorEdit.Color);
                                break;
                            default:
                                break;
                        }
                };

                linearGradientMultiSlider.ThumbColorChanged += (obj, ea) =>
                {
                    if (bInit)
                        switch (ColorTypeSelection)
                        {
                            case ColorType.Linear:
                                ResourceName = null;
                                colorEdit.Color = ea;
                                break;
                            default:
                                break;
                        }
                };
                radialGradientMultiSlider.ThumbColorChanged += (obj, ea) =>
                {
                    if (bInit)
                        switch (ColorTypeSelection)
                        {
                            case ColorType.Radial:
                                ResourceName = null;
                                colorEdit.Color = ea;
                                break;
                            default:
                                break;
                        }
                };

                linearGradientMultiSlider.BrushChanged += (obj, ea) =>
                {
                    if (bInit)
                        switch (ColorTypeSelection)
                        {
                            case ColorType.Linear:
                                ResourceName = null;
                                LinearBrush = ea as LinearGradientBrush;
                                break;
                            default:
                                break;
                        }
                };
                radialGradientMultiSlider.BrushChanged += (obj, ea) =>
                {
                    if (bInit)
                        switch (ColorTypeSelection)
                        {
                            case ColorType.Radial:
                                ResourceName = null;
                                RadialBrush = ea as RadialGradientBrush;
                                break;
                            default:
                                break;
                        }
                };
            };
        }

        private void UpdateLinearThumb(System.Windows.Media.Color color)
        {
            linearGradientMultiSlider.SelectedThumbColor = color;
            LinearBrush = linearGradientMultiSlider.Brush as LinearGradientBrush;
        }

        private void UpdateRadialThumb(System.Windows.Media.Color color)
        {
            radialGradientMultiSlider.SelectedThumbColor = color;
            RadialBrush = radialGradientMultiSlider.Brush as RadialGradientBrush;
        }

        public void LoadFromXml(string filepath)
        {
            //mapItems.Clear();

            try
            {
                var keyexpandolist = Utilities.XmlHelper.GetExpandoAttributeFromXml(File.ReadAllText(filepath), "resources", true);
                if (keyexpandolist.Count() != 0)
                {
                    keyexpandolist.ToList().ForEach(e =>
                    {
                        var regkeydictionary = e as IDictionary<string, object>;
                        regkeydictionary.ToList().ForEach(r =>
                        {
                            mapItems.Add(r.Key.ToString(), r.Value.ToString());
                        });
                    });
                }
            }
            catch (Exception e)
            {

            }

        }
        public bool IsEqual(System.Windows.Media.Brush aBrush1, System.Windows.Media.Brush aBrush2)
        {
            if (aBrush1.GetType() != aBrush2.GetType())
                return false;
            else
            {
                if (aBrush1 is SolidColorBrush)
                {
                    return (aBrush1 as SolidColorBrush).Color ==
                      (aBrush2 as SolidColorBrush).Color &&
                      (aBrush1 as SolidColorBrush).Opacity == (aBrush2 as SolidColorBrush).Opacity;
                }
                else if (aBrush1 is LinearGradientBrush)
                {
                    bool result = true;
                    result = (aBrush1 as LinearGradientBrush).ColorInterpolationMode ==
                      (aBrush2 as LinearGradientBrush).ColorInterpolationMode && result;
                    result = (aBrush1 as LinearGradientBrush).EndPoint ==
                      (aBrush2 as LinearGradientBrush).EndPoint && result;
                    result = (aBrush1 as LinearGradientBrush).MappingMode ==
                      (aBrush2 as LinearGradientBrush).MappingMode && result;
                    result = (aBrush1 as LinearGradientBrush).Opacity ==
                      (aBrush2 as LinearGradientBrush).Opacity && result;
                    result = (aBrush1 as LinearGradientBrush).StartPoint ==
                      (aBrush2 as LinearGradientBrush).StartPoint && result;
                    result = (aBrush1 as LinearGradientBrush).SpreadMethod ==
                      (aBrush2 as LinearGradientBrush).SpreadMethod && result;
                    result = (aBrush1 as LinearGradientBrush).GradientStops.Count ==
                      (aBrush2 as LinearGradientBrush).GradientStops.Count && result;
                    if (result && (aBrush1 as LinearGradientBrush).GradientStops.Count ==
                              (aBrush2 as LinearGradientBrush).GradientStops.Count)
                    {
                        for (int i = 0; i < (aBrush1 as LinearGradientBrush).GradientStops.Count; i++)
                        {
                            result = (aBrush1 as LinearGradientBrush).GradientStops[i].Color ==
                              (aBrush2 as LinearGradientBrush).GradientStops[i].Color && result;
                            result = (aBrush1 as LinearGradientBrush).GradientStops[i].Offset ==
                              (aBrush2 as LinearGradientBrush).GradientStops[i].Offset && result;
                            if (!result)
                                return result;
                        }
                    }
                    return result;
                }
                else if (aBrush1 is RadialGradientBrush)
                {
                    bool result = true;
                    result = (aBrush1 as RadialGradientBrush).ColorInterpolationMode ==
                                 (aBrush2 as RadialGradientBrush).ColorInterpolationMode && result;
                    result = (aBrush1 as RadialGradientBrush).GradientOrigin ==
                                (aBrush2 as RadialGradientBrush).GradientOrigin && result;
                    result = (aBrush1 as RadialGradientBrush).MappingMode == (aBrush2 as RadialGradientBrush).MappingMode && result;
                    result = (aBrush1 as RadialGradientBrush).Opacity == (aBrush2 as RadialGradientBrush).Opacity && result;
                    result = (aBrush1 as RadialGradientBrush).RadiusX == (aBrush2 as RadialGradientBrush).RadiusX && result;
                    result = (aBrush1 as RadialGradientBrush).RadiusY == (aBrush2 as RadialGradientBrush).RadiusY && result;
                    result = (aBrush1 as RadialGradientBrush).SpreadMethod == (aBrush2 as RadialGradientBrush).SpreadMethod && result;
                    result = (aBrush1 as RadialGradientBrush).GradientStops.Count == (aBrush2 as RadialGradientBrush).GradientStops.Count && result;
                    if (result && (aBrush1 as RadialGradientBrush).GradientStops.Count == (aBrush2 as RadialGradientBrush).GradientStops.Count)
                    {
                        for (int i = 0; i < (aBrush1 as RadialGradientBrush).GradientStops.Count; i++)
                        {
                            result = (aBrush1 as RadialGradientBrush).GradientStops[i].Color ==
                                          (aBrush2 as RadialGradientBrush).GradientStops[i].Color && result;
                            result = (aBrush1 as RadialGradientBrush).GradientStops[i].Offset ==
                                              (aBrush2 as RadialGradientBrush).GradientStops[i].Offset && result;
                            if (!result)
                                return result;
                        }
                    }
                    return result;
                }
                else if (aBrush1 is VisualBrush)
                {
                    bool result = true;
                    result = (aBrush1 as VisualBrush).AlignmentX == (aBrush2 as VisualBrush).AlignmentX && result;
                    result = (aBrush1 as VisualBrush).AlignmentY == (aBrush2 as VisualBrush).AlignmentY && result;
                    result = (aBrush1 as VisualBrush).Opacity == (aBrush2 as VisualBrush).Opacity && result;
                    result = (aBrush1 as VisualBrush).Stretch == (aBrush2 as VisualBrush).Stretch && result;
                    result = (aBrush1 as VisualBrush).TileMode == (aBrush2 as VisualBrush).TileMode && result;
                    result = (aBrush1 as VisualBrush).Viewbox == (aBrush2 as VisualBrush).Viewbox && result;
                    result = (aBrush1 as VisualBrush).ViewboxUnits == (aBrush2 as VisualBrush).ViewboxUnits && result;
                    result = (aBrush1 as VisualBrush).Viewport == (aBrush2 as VisualBrush).Viewport && result;
                    result = (aBrush1 as VisualBrush).ViewportUnits == (aBrush2 as VisualBrush).ViewportUnits && result;
                    if ((aBrush1 as VisualBrush).Visual is MediaElement && (aBrush2 as VisualBrush).Visual is MediaElement)
                        result = ((aBrush1 as VisualBrush).Visual as MediaElement).Source ==  ((aBrush2 as VisualBrush).Visual as MediaElement).Source && result;
                    else if ((aBrush1 as VisualBrush).Visual is ImageBrush && (aBrush2 as VisualBrush).Visual is ImageBrush)
                        result = (aBrush1 as ImageBrush).ImageSource == (aBrush2 as ImageBrush).ImageSource && result;
                    else if ((aBrush1 as VisualBrush).Visual is System.Windows.Controls.Image && (aBrush2 as VisualBrush).Visual is System.Windows.Controls.Image)
                    {
                        try
                        {
                            var source1 = ((aBrush1 as VisualBrush).Visual as System.Windows.Controls.Image).Source;
                            var source2 = ((aBrush2 as VisualBrush).Visual as System.Windows.Controls.Image).Source;
                            result = (source1 as System.Windows.Media.Imaging.BitmapImage).UriSource == (source2 as System.Windows.Media.Imaging.BitmapImage).UriSource && result;
                        }
                        catch (Exception)
                        {
                            result = false;
                        }
                    }


                    return result;
                }
            }
            return false;
        }
        private void UpdateColornameName(bool bInit = false)
        {
            string name = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ResourceName))
                {
                    selectedColor.Text = ResourceName;
                    return;
                }

                if (bInit && Brush != null)
                {
                    var b = (from p in mapResourceBrushes.Keys
                             where IsEqual(mapResourceBrushes[p] as System.Windows.Media.Brush, Brush)
                                    select p).FirstOrDefault();
                    if (b!= null)
                    {
                        name = b.ToString();

                        if (!string.IsNullOrEmpty(name))
                        {
                            selectedColor.Text = name;
                            if (listLinear.Items.Contains(name))
                            {
                                tabControl.SelectedItem = linear;
                                listLinear.SelectedItem = name;
                            }
                            else if (listRadial.Items.Contains(name))
                            {
                                tabControl.SelectedItem = radial;
                                listRadial.SelectedItem = name;
                            }
                            else if (listSolid.Items.Contains(name))
                            {
                                tabControl.SelectedItem = solid;
                                listSolid.SelectedItem = name;
                            }
                            else if (listVisual.Items.Contains(name))
                            {
                                tabControl.SelectedItem = visual;
                                listVisual.SelectedItem = name;
                            }
                            else if(mapResourceBrushes[b] is SolidColorBrush)
                            {
                                var sb = (b as SolidColorBrush).Color;
                                name = sb.GetColorName();
                                selectedColor.Text = name;
                                tabControl.SelectedItem = named;
                            }
                            return;
                        }
                    }

                    b = (from p in mapUserResourceBrushes.Keys
                         where IsEqual(mapUserResourceBrushes[p] as System.Windows.Media.Brush, Brush)
                                    select p).FirstOrDefault();

                    if (b != null)
                    {
                        name = b.ToString();

                        if (!string.IsNullOrEmpty(name))
                        {
                            selectedColor.Text = name;
                            if (listUser.Items.Contains(name))
                            {
                                tabControl.SelectedItem = user;
                                listUser.SelectedItem = name;
                                listUser.ScrollIntoView(listUser.SelectedItem);
                            }
                            return;
                        }
                    }


                    if (Brush is SolidColorBrush)
                    {
                        System.Windows.Media.Color color = (Brush as SolidColorBrush).Color;
                        try
                        {
                            name = color.GetColorName();
                            if (!string.IsNullOrEmpty(name))
                            {
                                selectedColor.Text = name;
                                tabControl.SelectedItem = named;
                                return;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                selectedColor.Text = name;
            }
            catch (Exception ex)
            {
            }
        }
        bool bInit;
        private void InitColorEdit()
        {
            //colorEdit.Brush = Brush;
            if (OnlySolidColors)
            {
                linear.Visibility = radial.Visibility = visual.Visibility = user.Visibility = selectedColorGrid.Visibility = buttonsGrid.Visibility = Visibility.Collapsed;
                mainGrid.RowDefinitions[1].Height = mainGrid.RowDefinitions[2].Height = new GridLength(0);
                tyeSelection.Visibility = Visibility.Collapsed;
            }

            if (Brush is SolidColorBrush)
            {
                SolidBrush = (Brush as SolidColorBrush);
                colorEdit.Color = SolidBrush.Color;
                ColorTypeSelection = ColorType.Solid;
            }
            else
            {
                if (OnlySolidColors)
                    return;

                if (Brush is LinearGradientBrush)
                {
                    var c = Brush as LinearGradientBrush;
                    var b = new LinearGradientBrush();
                    b.ColorInterpolationMode = c.ColorInterpolationMode;
                    b.EndPoint = c.EndPoint;
                    b.StartPoint = c.StartPoint;
                    b.MappingMode = c.MappingMode;
                    b.Opacity = c.Opacity;
                    b.SpreadMethod = c.SpreadMethod;
                    foreach (var gradient in c.GradientStops)
                        b.GradientStops.Add(new GradientStop()
                        {
                            Color = gradient.Color,
                            Offset = gradient.Offset
                        });

                    LinearBrush = b;
                    linearGradientMultiSlider.Brush = LinearBrush;
                    colorEdit.Color = linearGradientMultiSlider.SelectedThumbColor;
                    ColorTypeSelection = ColorType.Linear;
                }
                else if (Brush is RadialGradientBrush)
                {
                    var c = Brush as RadialGradientBrush;
                    var b = new RadialGradientBrush();
                    b.RadiusX = c.RadiusX;
                    b.RadiusY = c.RadiusY;
                    b.GradientOrigin = c.GradientOrigin;
                    b.Center = c.Center;
                    b.ColorInterpolationMode = c.ColorInterpolationMode;
                    b.MappingMode = c.MappingMode;
                    b.Opacity = c.Opacity;
                    b.SpreadMethod = c.SpreadMethod;
                    foreach (var gradient in c.GradientStops)
                        b.GradientStops.Add(new GradientStop()
                        {
                            Color = gradient.Color,
                            Offset = gradient.Offset
                        });

                    RadialBrush = b;
                    radialGradientMultiSlider.Brush = RadialBrush;
                    colorEdit.Color = radialGradientMultiSlider.SelectedThumbColor;
                    ColorTypeSelection = ColorType.Radial;
                    UpdateColorEditLayout();
                }
            }
            bInit = true;
        }

        private ObservableCollection<object> InitContextMenu()
        {
            ObservableCollection<object> ContextItems = new ObservableCollection<object>();

            System.Windows.Controls.Image delete = new System.Windows.Controls.Image();
            BitmapImage bmpdelete = new BitmapImage(new Uri("/ScreenManager;component/Images/DeleteFolder.png", UriKind.RelativeOrAbsolute));
            delete.Source = bmpdelete;
            delete.Width = 16;
            delete.Height = 16;
            ContextItems.Add(new MenuItem() { Header = Properties.Resources.RemoveColor, Command = new DelegateCommand<object>(param => Delete()), Icon = delete });

            return ContextItems;
        }

        private void Delete()
        {
            foreach (var v in listUser.SelectedItems)
            {
                ResourceName = v as String;
                try
                {
                    mapUserResourceBrushes.Remove(v);
                    mapResourceBrushes.Remove(v);
                }
                catch
                {
                }
            }
            SaveUserBushes();
            FillUserResourceList(mapUserResourceBrushes); 
        }


        private void FillUserResourceList(Dictionary<Object, Object> map)
        {
            listUser.Items.Clear();

            var list1 = (from entry in map/*.AsParallel()*/ where entry.Value is RadialGradientBrush select entry.Key).ToList();
            var list2 = (from entry in map/*.AsParallel()*/ where entry.Value is LinearGradientBrush select entry.Key).ToList();
            var list3 = (from entry in map/*.AsParallel()*/ where entry.Value is VisualBrush select entry.Key).ToList();
            var list4 = (from entry in map/*.AsParallel()*/ where entry.Value is SolidColorBrush select entry.Key).ToList();

            try
            {
                list1.ForEach(obj => listUser.Items.Add(obj));
                list2.ForEach(obj => listUser.Items.Add(obj));
                list3.ForEach(obj => listUser.Items.Add(obj));
                list4.ForEach(obj => listUser.Items.Add(obj));

                listUser.ContextMenu = new ContextMenu()
                {
                    ItemsSource = InitContextMenu()
                };
            }
            catch
            {
            }
        }

        private void UpdateMapResources()
        {
            (from entry in mapUserResourceBrushes/*.AsParallel()*/ select entry.Key).ToList().ForEach(obj => mapResourceBrushes[obj] = mapUserResourceBrushes[obj]);
        }
        private void FillResourceList(Dictionary<Object, Object> map)
        {
            listRadial.ItemsSource = null;
            listLinear.ItemsSource = null;
            listVisual.ItemsSource = null;
            listSolid.ItemsSource = null;
            listNamed.ItemsSource = null;

            var list1 = (from entry in map/*.AsParallel()*/ where entry.Value is RadialGradientBrush select entry.Key).ToList();
            var list2 = (from entry in map/*.AsParallel()*/ where entry.Value is LinearGradientBrush select entry.Key).ToList();
            var list3 = (from entry in map/*.AsParallel()*/ where entry.Value is VisualBrush select entry.Key).ToList();
            var list4 = (from entry in map/*.AsParallel()*/ where entry.Value is SolidColorBrush select entry.Key).ToList();
            var namedBrush = new BrushCollection();
            var list5 = (from entry in namedBrush/*.AsParallel()*/ select entry.Brush).ToList();

            list1.ForEach(obj =>
            {
                if (objRadialList.Contains(obj))
                    objRadialList[objRadialList.IndexOf(obj)] = obj;
                else
                    objRadialList.Add(obj);
             });
            list2.ForEach(obj =>
            {
                if (objLinearList.Contains(obj))
                    objLinearList[objLinearList.IndexOf(obj)] = obj;
                else
                    objLinearList.Add(obj);
            });
            list3.ForEach(obj =>
            {
                var visualObject = (from o in objVisualList where (o is VisualInfo) && 
                                    (o as VisualInfo).Key == obj select o).FirstOrDefault(); 
                if (visualObject != null)
                    objVisualList[objVisualList.IndexOf(visualObject)] = obj;
                else
                    objVisualList.Add(new VisualInfo() { Key = obj, Visual = map[obj]});
            });
            list4.ForEach(obj =>
            {
                if (objSolidList.Contains(obj))
                    objSolidList[objSolidList.IndexOf(obj)] = obj;
                else
                    objSolidList.Add(obj);
            });

            //mapResourceBrushes = map;

            list5.ForEach(obj => {
                if (objNamedList.Contains(obj))
                    objNamedList[objNamedList.IndexOf(obj)] = obj;
                else
                    objNamedList.Add(obj);
                if (mapResourceBrushes.ContainsKey(obj))
                    mapResourceBrushes[obj] = obj;
                else
                    mapResourceBrushes.Add(obj, obj);
            });

            listRadial.ItemsSource = objRadialList.OrderBy(x => x as string); 
            listLinear.ItemsSource = objLinearList.OrderBy(x => x as string);
            listVisual.ItemsSource = objVisualList.OrderBy(x => x as string);
            listSolid.ItemsSource = objSolidList.OrderBy(x => x as string);
            listNamed.ItemsSource = objNamedList.OrderBy(x => x as string);
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var v in e.AddedItems)
            {
                ResourceName = v as String;
                if (v is VisualInfo)
                {
                    var brush = mapResourceBrushes[(v as VisualInfo).Key] as System.Windows.Media.Brush;
                    ResourceName = (v as VisualInfo).Key.ToString();
                    OnBrushChanged((v as VisualInfo).Visual as System.Windows.Media.Brush);
                }
                else
                {
                    var brush = mapResourceBrushes[v] as System.Windows.Media.Brush;
                    if (string.IsNullOrEmpty(ResourceName) && brush is System.Windows.Media.SolidColorBrush)
                        ResourceName = (brush as System.Windows.Media.SolidColorBrush).Color.GetColorName();
                    OnBrushChanged(brush);
                }
            }

            ListBox listBox = (sender as ListBox);
            listBox.Dispatcher.BeginInvokeIfRequired(
                (Action)(() =>
                {
                    listBox.UpdateLayout();
                    if (listBox.SelectedItem !=
                        null)
                        listBox.ScrollIntoView(
                            listBox.SelectedItem);
                }));
        }

        #region OnBrushChanged
        /// <summary>
        /// Triggers the BrushChanged event.
        /// </summary>
        public virtual void OnBrushChanged(System.Windows.Media.Brush brush)
        {
            if (brush is LinearGradientBrush)
            {
                var c = brush as LinearGradientBrush;
                var b = new LinearGradientBrush();
                b.ColorInterpolationMode = c.ColorInterpolationMode;
                b.EndPoint = c.EndPoint;
                b.StartPoint = c.StartPoint;
                b.MappingMode = c.MappingMode;
                b.Opacity = c.Opacity;
                b.SpreadMethod = c.SpreadMethod;
                foreach (var gradient in c.GradientStops)
                    b.GradientStops.Add(new GradientStop()
                    {
                        Color = gradient.Color,
                        Offset = gradient.Offset
                    });

                Brush = b;
            }
            else if (brush is RadialGradientBrush)
            {
                var c = brush as RadialGradientBrush;
                var b = new RadialGradientBrush();
                b.RadiusX = c.RadiusX;
                b.RadiusY = c.RadiusY;
                b.GradientOrigin = c.GradientOrigin;
                b.Center = c.Center;
                b.ColorInterpolationMode = c.ColorInterpolationMode;
                b.MappingMode = c.MappingMode;
                b.Opacity = c.Opacity;
                b.SpreadMethod = c.SpreadMethod;
                foreach (var gradient in c.GradientStops)
                    b.GradientStops.Add(new GradientStop()
                    {
                        Color = gradient.Color,
                        Offset = gradient.Offset
                    });

                Brush = b;
            }
            else
                Brush = brush;

            UpdateColornameName();
            var temp = BrushChanged;
            if (temp != null)
                temp(null/*this*/, new EventArgs());
        }
        #endregion

        public event EventHandler SelectionCompleted;
        void OnSelectionCompleted()
        {
            SelectionCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void colorEdit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as System.Windows.Shapes.Path) != null)
                (e.OriginalSource as System.Windows.Shapes.Path).Visibility = Visibility.Collapsed;
            string name = (e.OriginalSource as FrameworkElement).Name;
            if(name == "ColorPalitte" ||
                name == "SelectedColor" ||
                name == "CurrentColor")
                OnSelectionCompleted();
        }

        private void colorEdit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as System.Windows.Shapes.Path) != null)
                e.Handled = true;
        }

        private void TransparentButton_Click(object sender, RoutedEventArgs e)
        {
            MyBrush brush = new MyBrush("Transparent", new SolidColorBrush(Colors.Transparent));
            colorEdit.Color = brush.Brush.Color;
            OnBrushChanged(brush.Brush);
        }

        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            var temp = UndoChanges;
            if (temp != null)
                temp(null/*this*/, new EventArgs());
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            //add Color to map and then
            var colorName = CustomColorDialog();
            if (string.IsNullOrEmpty(colorName))
                return;

            try
            {
                Random random = new Random();

                while (mapResourceBrushes.ContainsKey((object)colorName) || mapUserResourceBrushes.ContainsKey((object)colorName))
                {
                    colorName = string.Format("{0}{1}", colorName, random.Next());
                }

                mapUserResourceBrushes.Add((object)colorName, (object)Brush);
                mapResourceBrushes.Add((object)colorName, (object)Brush);
                SaveUserBushes();
                listUser.Items.Add((object)colorName);
            }
            catch
            {
            }
        }

        public string CustomColorDialog()
        {
            CustomColorNameEditor colorwnd = new CustomColorNameEditor();

            var dialog = new GeneralDialogContent(colorwnd, GeneralDialogButtons.OkCancelButtons)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = Properties.Resources.CustomColorSaveTitle,
                HelpLink = ""
            };

            if (dialog.ShowDialog() == true)
            {
                return colorwnd.textbox_colorName.Text;
            }

            return string.Empty;
        }

        #region Isolated Storage
        static String GetStoreFileNameUserBrush()
        {
            return String.Format("BrushEditor_UserBrush.dat");
        }

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        #region Private Members
        private const string DictionaryNodeName = "Dictionary";
        private const string ItemNodeName = "Item";
        private const string KeyNodeName = "Key";
        private const string ValueNodeName = "Value";
        #endregion
        void SaveUserBushes()
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                try
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreFileNameUserBrush(), FileMode.Create, isoStorage))
                    {

                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8,
                            ConformanceLevel = ConformanceLevel.Auto
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
                            DataContractSerializer serializer = new DataContractSerializer(typeof(String));
                            writer.WriteStartElement(DictionaryNodeName);
                            foreach (KeyValuePair<object, object> kvp in mapUserResourceBrushes)
                            {
                                try
                                {
                                    writer.WriteStartElement(ItemNodeName);

                                    writer.WriteStartElement(KeyNodeName);
                                    serializer.WriteObject(writer, kvp.Key);
                                    writer.WriteEndElement();

                                    writer.WriteStartElement(ValueNodeName);
                                    string _value = XamlWriter.Save(kvp.Value);
                                    valueSerializer.Serialize(writer, _value);
                                    //writer.WriteValue(XamlWriterEx.XamlWriter.Save(kvp.Value));
                                    writer.WriteEndElement();
                                }
                                catch (Exception ex)
                                {
                                }

                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {

            }
        }

        void LoadUserBushes()
        {
            var isoStorage = GetStorage();
            if (null == isoStorage)
                return;

            using (var stream = new IsolatedStorageFileStream(GetStoreFileNameUserBrush(), FileMode.OpenOrCreate, isoStorage))
            {
                try
                {
                    stream.Position = 0;
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Auto,
                        CloseInput = true
                    };

                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        mapUserResourceBrushes.Clear();
                        XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
                        DataContractSerializer serializer = new DataContractSerializer(typeof(String));
                        reader.ReadStartElement(DictionaryNodeName);
                        while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
                        {
                            try
                            {
                                XmlSerializer ValueSerializer = new XmlSerializer(typeof(String));
                                reader.ReadStartElement(ItemNodeName);
                                reader.ReadStartElement(KeyNodeName);
                                Object key = (Object)serializer.ReadObject(reader);
                                reader.ReadEndElement();

                                reader.ReadStartElement(ValueNodeName);
                                String svalue = (String)valueSerializer.Deserialize(reader);
                                Object value = XamlReader.Parse(svalue);

                                reader.ReadEndElement();
                                reader.ReadEndElement();

                                mapUserResourceBrushes.Add(key, value);
                                reader.MoveToContent();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        reader.ReadEndElement();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        #endregion

        VisualBrush LoadFromFile(string file)
        {
            if (Document == null)
                return null;

            try
            {
                Uri uriFound = null;
                var dropHelper = new DropFileHelper(Document, UriToUriAbsoluteImageConverter, UIService, SourceFileCopyOption.Always);
                var mp = dropHelper.DropFile(file, out uriFound);
                return new VisualBrush(mp);
            }
            catch
            {
                return null;
            }
        }

        void LoadResourcesFromSpecialFolder(Dictionary<Object, Object> map)
        {
            try
            {
                if(Document != null)
                {
                    if (Document.fileSystemProviderBase == null)
                    {
                        Uri docUri = Document.GetSpecialFolder(SpecialFolders.Images);
                        string[] files = Directory.GetFiles(docUri.OriginalString, "*.*", SearchOption.AllDirectories);
                        files.ToList().ForEach(file =>
                        {
                            try
                            {
                                VisualBrush db = LoadFromFile(file);
                                if (db != null)
                                {
                                    if (map.ContainsKey(file))
                                        map.Remove(file);
                                    map.Add(file, db);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        });
                    }
                    else
                    {
                        FileSystemProviderBase fileSystemProviderBase = Document.fileSystemProviderBase;
                        var fmFolder = new FileManagerFolder(fileSystemProviderBase, Document.GetSpecialFolder(SpecialFolders.Images).OriginalString);
                        if(fmFolder != null)
                        {
                            fmFolder.GetFiles().ToList().ForEach(file =>
                            {
                                try
                                {
                                    VisualBrush db = LoadFromFile(file.FullName);
                                    if (db != null)
                                    {
                                        if (map.ContainsKey(file))
                                            map.Remove(file);
                                        map.Add(file, db);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UIService == null)
                return;

            String file = UIService.ShowOpenFileDialog("*.xaml|*.*");
            if (String.IsNullOrEmpty(file))
                return;

            file = file.ToLower();
            if (file.Contains(".xaml"))
            {
                try
                {
                    var map = ResourceDictionaryExtensions.LoadFromFile(file, typeof(System.Windows.Media.Brush));
                    FillResourceList(map);
                }
                catch (Exception ex)
                {
                    UIService.ShowError(ex.Message);
                }
            }
            else
            {
                try
                {
                    VisualBrush db = LoadFromFile(file);
                    if(db != null)
                    {
                        if (mapResourceBrushes.ContainsKey(file))
                            mapResourceBrushes.Remove(file);
                        mapResourceBrushes.Add(file, db);

                        Dictionary<object, object> map = new Dictionary<object, object>();
                        map.Add(file, db);

                        FillResourceList(map);
                        tabControl.SelectedItem = listVisual;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        //private void foreclr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (!bLoaded || foreclr.SelectedItem == null)
        //        return;
        //    var brush = foreclr.SelectedItem as MyBrush;
        //    colorEdit.Color = brush.Brush.Color;
        //    OnBrushChanged(brush.Brush);
        //}

        private BitmapSource screenimage;
        private bool ineyedropmode;
        private System.Windows.Point? previousposition = null;

        public void Dispose()
        {
            listRadial.ItemsSource = null;
            listLinear.ItemsSource = null;
            listVisual.ItemsSource = null;
            listSolid.ItemsSource = null;
            listNamed.ItemsSource = null;
            objRadialList.Clear();
            objLinearList.Clear();
            objVisualList.Clear();
            objSolidList.Clear();
            objNamedList.Clear();
            mapItems.Clear();
        }
        private BitmapSource TakeScreenshot(int StartX, int StartY, int Width, int Height)
        {
            Bitmap Screenshot = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(Screenshot);
            G.CopyFromScreen(StartX, StartY, 0, 0, new System.Drawing.Size(Width, Height), CopyPixelOperation.SourceCopy);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Screenshot.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Seek(0, SeekOrigin.Begin);

                if (InvokeRequired)
                    return (BitmapSource)System.Windows.Application.Current.Dispatcher.Invoke(
                        new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
                        DispatcherPriority.Normal,
                        memoryStream);

                return CreateBitmapSourceFromBitmap(memoryStream);
            }
        }
        private static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != System.Windows.Application.Current.Dispatcher; }
        }

        private static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
        {
            BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            return writable;
        }

        private void Button_ResetToDefault(object sender, RoutedEventArgs e)
        {
            Brush = null;
            //colorEdit.Brush = Brush;
            OnBrushChanged(Brush);
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnSelectionCompleted();
        }

        private void options_EditValueChanging(object sender, DevExpress.Xpf.Editors.EditValueChangingEventArgs e)
        {
            NotifyBrushChanged();
        }

        void NotifyBrushChanged()
        {
            if (bInit)
                switch (ColorTypeSelection)
                {
                    case ColorType.Solid:
                        if (this.ReadLocalValue(SolidBrushProperty) == DependencyProperty.UnsetValue)
                            SolidBrush = new SolidColorBrush(colorEdit.Color);
                        else
                            OnBrushChanged(SolidBrush);
                        break;
                    case ColorType.Linear:
                        if (this.ReadLocalValue(LinearBrushProperty) == DependencyProperty.UnsetValue)
                            LinearBrush = linearGradientMultiSlider.Brush as LinearGradientBrush;
                        else
                            OnBrushChanged(LinearBrush);
                        break;
                    case ColorType.Radial:
                        if (this.ReadLocalValue(RadialBrushProperty) == DependencyProperty.UnsetValue)
                            RadialBrush = radialGradientMultiSlider.Brush as RadialGradientBrush;
                        else
                            OnBrushChanged(RadialBrush);
                        break;
                    default:
                        break;
                }
        }
    }

    public class BrushCollection : ObservableCollection<MyBrush>
    {
        public BrushCollection()
            : base()
        {
            Type type = typeof(System.Windows.Media.Brushes);
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (propertyInfo.PropertyType == typeof(SolidColorBrush))
                    Add(new MyBrush(propertyInfo.Name, (SolidColorBrush)propertyInfo.GetValue(null, null)));
            }
        }
    }
    public class MyBrush
    {
        string _name;
        SolidColorBrush _brush;
        public MyBrush()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public SolidColorBrush Brush
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
            }
        }
        public MyBrush(string name, SolidColorBrush brush)
        {
            _name = name;
            _brush = brush;
            // Color color = brush.Color;
        }
    }

    //[ValueConversion(typeof(System.Windows.Media.Brush), typeof(System.Windows.Media.Brush))]
    public class ResourceToBrushConverter : IValueConverter
    {
        public BrushEditor Window { get; set; }

        #region IValueConverter Membres
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (targetType != typeof(System.Windows.Media.Brush) && targetType != typeof(String))
                throw new InvalidOperationException("The target must be a Brush or a String");

            if (Window == null)
                throw new InvalidOperationException("The target Window must be set");

            if (targetType == typeof(System.Windows.Media.Brush))
                return Window.mapResourceBrushes[value] as System.Windows.Media.Brush;

            if (parameter != null && parameter != DependencyProperty.UnsetValue && parameter.ToString() == "1" && value is string)
            {
                //if (Window.mapItems.ContainsKey(value as string))
                //    return Window.mapItems[value as string];
                //else
                    return value as string;
            }
            else
                return (Window.mapResourceBrushes[value] as System.Windows.Media.SolidColorBrush).Color.GetColorName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class VisualInfo
    {
        public object Visual { get; set; }
        public object Key { get; set; }
    }

    public class MyGradientMultiSlider : GradientMultiSlider
    {
        public event EventHandler<System.Windows.Media.Color> ThumbColorChanged;
        public event EventHandler<GradientBrush> BrushChanged;
        protected void IsThumbColorChanged(System.Windows.Media.Color color)
        {
            ThumbColorChanged?.Invoke(this, color);
        }
        protected void IsBrushChanged(GradientBrush newValue)
        {
            BrushChanged?.Invoke(this, newValue);
        }
        protected override void OnSelectedThumbChanged(GradientMultiSliderThumb newValue)
        {
            base.OnSelectedThumbChanged(newValue);
            IsThumbColorChanged(newValue.Color);
        }
        protected override void OnBrushChanged(GradientBrush newValue)
        {
            base.OnBrushChanged(newValue);
            IsBrushChanged(newValue);
        }
    }

    public enum ColorType
    {
        Solid,
        Linear,
        Radial
    }
    public class ColorTypeToIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null && value is ColorType)
                    switch (parameter.ToString())
                    {
                        case "0":
                            return (ColorType)value == ColorType.Solid;
                        case "1":
                            return (ColorType)value == ColorType.Linear;
                        case "2":
                            return (ColorType)value == ColorType.Radial;
                        default:
                            break;
                    }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
