using DevExpress.Xpf.PdfViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFInterfaces.PropertyControl;
using Utilities;
using System.ComponentModel;
using ScreenSettings;
using CommonControls.PropertyDataTemplate;
using PdfViewer.Enums;
using DevExpress.Xpf.DocumentViewer;
using DocumentManager.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Converters;
using DevExpress.Xpf.Bars;
using UFInterfaces;
using System.Xml.Serialization;

namespace PdfViewer
{
    /// <summary>
    /// Interaction logic for PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : UserControl, IContainPropertyEditors, IDataErrorInfo, IDisposable
    {
        #region DP
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }


        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(PdfViewer), new UIPropertyMetadata(null));
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ToolbarBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ToolbarBackgroundProperty);
            }
            set
            {
                SetValue(ToolbarBackgroundProperty, value);
            }
        }
        #endregion

        #region ToolbarForeground
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(PdfViewer), new UIPropertyMetadata(null));
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ToolbarForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ToolbarForegroundProperty);
            }
            set
            {
                SetValue(ToolbarForegroundProperty, value);
            }
        }

        #endregion

        #region DocumentPath
        public static readonly DependencyProperty DocumentPathProperty = DependencyProperty.Register("DocumentPath", typeof(Uri), typeof(PdfViewer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDocumentPathChanged), new CoerceValueCallback(OnCoerceDocumentPath)));

        private static object OnCoerceDocumentPath(DependencyObject o, object value)
        {
            PdfViewer pdfViewer = o as PdfViewer;
            if (pdfViewer != null)
                return pdfViewer.OnCoerceDocumentPath((Uri)value);
            else
                return value;
        }

        private static void OnDocumentPathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PdfViewer pdfViewer = o as PdfViewer;
            if (pdfViewer != null)
                pdfViewer.OnDocumentPathChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceDocumentPath(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDocumentPathChanged(Uri oldValue, Uri newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!(DesignerProperties.GetIsInDesignMode(this) || bDesign) && (oldValue != newValue) && bInit)
            {
                SetDocumentPath(newValue);
            }
        }

        private void SetDocumentPath(Uri newValue)
        {
            if (bDispose)
                return;

            try
            {
                String filepath = null;
                var document = ScreenDocument.GetScreenDocument(this);
                if (document != null && DocumentPath != null)
                {
                    Uri uri = UriToAbsoluteUriConverter.Convert(newValue, document, SpecialFolders.Documents);
                    if (uri != null)
                        filepath = uri.GetPathString();
                }
                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    if (String.IsNullOrEmpty(filepath) || !System.IO.File.Exists((filepath)))
                    {
                        pdfViewerControl.DocumentSource = null;
                    }
                    else
                    {
                        pdfViewerControl.DocumentSource = filepath;
                    }
                });
            }
            catch (Exception)
            {
            }
        }

        public Uri DocumentPath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(DocumentPathProperty);
            }
            set
            {
                SetValue(DocumentPathProperty, value);
            }
        }
        #endregion
        #region CommandBarStyle
        public static readonly DependencyProperty CommandBarStyleProperty = DependencyProperty.Register("CommandBarStyle", typeof(CommandStyle), typeof(PdfViewer), new UIPropertyMetadata(CommandStyle.Bars, new PropertyChangedCallback(OnCommandBarStyleChanged), new CoerceValueCallback(OnCoerceCommandBarStyle)));

        private static object OnCoerceCommandBarStyle(DependencyObject o, object value)
        {
            PdfViewer pdfViewer = o as PdfViewer;
            if (pdfViewer != null)
                return pdfViewer.OnCoerceCommandBarStyle((CommandStyle)value);
            else
                return value;
        }

        private static void OnCommandBarStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PdfViewer pdfViewer = o as PdfViewer;
            if (pdfViewer != null)
                pdfViewer.OnCommandBarStyleChanged((CommandStyle)e.OldValue, (CommandStyle)e.NewValue);
        }

        protected virtual CommandStyle OnCoerceCommandBarStyle(CommandStyle value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCommandBarStyleChanged(CommandStyle oldValue, CommandStyle newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            pdfViewerControl.CommandBarStyle = (CommandBarStyle)CommandBarStyle;
        }

        public CommandStyle CommandBarStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (CommandStyle)GetValue(CommandBarStyleProperty);
            }
            set
            {
                SetValue(CommandBarStyleProperty, value);
            }
        }
        #endregion


        #region PageNumber
        public static readonly DependencyProperty PageNumberProperty = DependencyProperty.Register("PageNumber", typeof(int), typeof(PdfViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPageNumberChanged), new CoerceValueCallback(OnCoercePageNumber)));

        private static object OnCoercePageNumber(DependencyObject o, object value)
        {
            PdfViewer control = o as PdfViewer;
            if (control != null)
                return control.OnCoercePageNumber((int)value);
            else
                return value;
        }

        private static void OnPageNumberChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PdfViewer control = o as PdfViewer;
            if (control != null)
                control.OnPageNumberChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoercePageNumber(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPageNumberChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!(DesignerProperties.GetIsInDesignMode(this) || bDesign) && (oldValue != newValue) && bInit)
            {
                SetDocumentPage(newValue);
            }
        }

        private void SetDocumentPage(int newValue)
        {
           if(newValue <= pdfViewerControl.PageCount)
            {
                try
                {
                    pdfViewerControl.CurrentPageNumber = newValue;
                }
                catch
                {
                }
            }
        }

        public int PageNumber
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(PageNumberProperty);
            }
            set
            {
                SetValue(PageNumberProperty, value);
            }
        }

        #endregion


        #endregion

        #region IContainPropertyEditors Members

        [Browsable(false)]
        public Type ObjectType
        {
            get
            {
                return this.GetType();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {

                DocumentManager.ComponentService.IDocument Document;
                IWorkspace Workspace;
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                Workspace = Document?.GetService(typeof(IWorkspace)) as IWorkspace;

                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                // Defines Data Template for 'DocumentPathProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(SourceFilePropertyEditor));
                factory.SetValue(SourceFilePropertyEditor.WorkspaceProperty, Workspace);
                factory.SetValue(SourceFilePropertyEditor.FilterProperty, Properties.Resources.FilterOption);
                factory.SetValue(SourceFilePropertyEditor.CopyOptionProperty, SourceFileCopyOption.Ask);
                factory.SetValue(SourceFilePropertyEditor.DefaultFolderProperty, SpecialFolders.Documents);
                factory.SetValue(SourceFilePropertyEditor.DefaultExtProperty, "pdf");
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                mapDataTemplates.Add(DocumentPathProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "DocumentPath")
                {
                    if (DocumentPath != null)
                    {
                        if (!DocumentPath.IsValidFile())
                            return Properties.Resources.InvalidFilePath;

                        var document = ScreenDocument.GetScreenDocument(this);
                        if (document != null)
                        {
                            Uri uri = UriToAbsoluteUriConverter.Convert(DocumentPath,document,SpecialFolders.Documents);
                            var filepath = uri.GetPathString();
                            if (String.IsNullOrEmpty(filepath) || !System.IO.File.Exists(filepath))
                                return Properties.Resources.InvalidFilePath;
                        }
                    }
                }

                return null;
            }
        }

        #endregion

        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }

     #region Constructor
        public PdfViewer()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            CommonConstructor();
        }
        bool bLoaded;
        bool bDesign;
        bool bInit;
        void CommonConstructor()
        {
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;

                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                    {
                        bDesign = true;
                        pdfViewerControl.IsEnabled = false;
                    } 
                    
                    pdfViewerControl.CommandBarStyle = (CommandBarStyle)CommandBarStyle;
                    pdfViewerControl.DocumentLoaded += pdfViewerControl_DocumentLoaded;

                    if (RunningOnServer)
                    {
                        var link = TryFindResource("pdfCommandProvider") as PdfCommandProvider;
                        pdfViewerControl.CommandProvider = link;
                    }

                    SetDocumentPath(DocumentPath);
                    bInit = true;
                }
            };
        }

        private void pdfViewerControl_DocumentLoaded(object sender, RoutedEventArgs e)
        {
            SetDocumentPage(PageNumber);
        }

        #endregion

        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;


            pdfViewerControl.DocumentLoaded -= pdfViewerControl_DocumentLoaded;
        }
    }


    internal class ConvertDefaultBrushValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is PdfViewer)
            {
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
                ret.Add("Foreground", foreground);
                ret.Add("ControlForeground", ret["Foreground"]);
                ret.Add("Background", background);
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null)
                return null;
            if (value is bool)
                return value;
            PdfViewer pdfViewer = sender as PdfViewer;
            string prop = ((DependencyProperty)property).Name;
            Brush defColor = pdfViewer.Foreground;
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
            try
            {
                if (prop.Equals(PdfViewer.ToolbarBackgroundProperty.Name))
                {
                    if (background is SolidColorBrush)
                    {
                        SolidColorBrush solidColorBrush = (background as SolidColorBrush);

                        defColor = new SolidColorBrush(WPFUtilities.DeployHelper.GetColorInContrast(solidColorBrush.Color, (document as ScreenDocument).Theme));
                    }
                    else if (background is LinearGradientBrush)
                    {
                        LinearGradientBrush linearGradientBrush = background.Clone() as LinearGradientBrush;
                        if (linearGradientBrush.GradientStops.Count > 0)
                        {
                            linearGradientBrush.GradientStops.ToList().ForEach(gradient =>
                            {
                                gradient.Color = WPFUtilities.DeployHelper.GetColorInContrast(gradient.Color, (document as ScreenDocument).Theme);
                            });
                        }
                        defColor = linearGradientBrush;
                    }
                    else
                        defColor = background;
                }
                else if (prop.Equals(PdfViewer.ToolbarForegroundProperty.Name))
                    defColor = foreground;
                else
                    return value;
            }
            catch (Exception)
            {
                return value;
            }

            return defColor;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }
}
