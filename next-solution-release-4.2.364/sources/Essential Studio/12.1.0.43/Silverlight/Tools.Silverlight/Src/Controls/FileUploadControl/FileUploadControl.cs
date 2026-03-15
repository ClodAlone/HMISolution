#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.ComponentModel;
        

    /// <summary>
    /// Defines the Column Names
    /// </summary>
    public enum ColumnNames
    {
        /// <summary>
        /// Column Name : FileName
        /// </summary>
        FileName,

        /// <summary>
        /// Column Name : FileSize
        /// </summary>
        FileSize,

        /// <summary>
        /// Column Name : Status
        /// </summary>
        Status,

        /// <summary>
        /// Column Name : SizeUploaded
        /// </summary>
        SizeUploaded,

        /// <summary>
        /// Column Name : Progress
        /// </summary>
        Progress,

        /// <summary>
        /// Column Name : Cancel
        /// </summary>
        Cancel,

        /// <summary>
        /// Column Name : Remove
        /// </summary>
        Remove
    }


    /// <summary>
    /// Represents a FileUpload Control
    /// </summary>
    /// <example>
    /// <para><b>Creating </b>FileUpload<b> Control in XAML</b></para>
    /// <para></para>
    /// <code>&lt;UserControl x:Class=&quot;SilverlightSampleBrowser.FileUploadDemo&quot;
    ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
    ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
    ///     xmlns:syncfusion=&quot;clr-     namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight&quot;     Height=&quot;600&quot; Width=&quot;850&quot;&gt;</code>
    /// <para></para>
    /// <code>&lt;StackPanel x:Name=&quot;StackPanel1&quot;&gt;
    /// &lt;syncfusion:FileUploadControl Name=&quot;fileUploadControl1&quot; Width=&quot;125&quot; Height=&quot;30&quot;  VerticalAlignment=&quot;Center&quot;    &gt;&lt;/syncfusion:FileUploadControl&gt;&lt;/StackPanel&gt;
    /// &lt;/UserControl&gt;</code>
    /// <para></para>
    /// <para></para>
    /// <para><b>Creating FileUpload Control using C#</b></para>
    /// <para> public partial class FileUploadControlDemo : UserControl</para>
    /// <para>    {</para>
    /// <para>        public FileUploadControlDemo</para>
    /// <para>        {</para>
    /// <para>            InitializeComponent();</para>
    /// <para>            FileUploadControl fileUploadControl1= new FileUploadControl();</para>
    /// <para>             StackPanel1.Children.Add(fileUploadControl1);</para>
    /// <para>       }</para>
    /// <para>} </para>
    /// <para>}</para>
    /// </example>

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Blend;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Office2007Black;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Default;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Office2003;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.VS2010;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Metro;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Windows7;component/FileUploadControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
       Type = typeof(FileUploadControl), XamlResource = "/Syncfusion.Theming.Transparent;component/FileUploadControl.xaml")]
    public class FileUploadControl : Control
    {

        /// <summary>
        /// Gets or sets the header border thickness.
        /// </summary>
        /// <value>The header border thickness.</value>
        public Thickness HeaderBorderThickness
        {
            get { return (Thickness)GetValue(HeaderBorderThicknessProperty); }
            set { SetValue(HeaderBorderThicknessProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderBorderThicknessProperty =
            DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(Thickness), new PropertyMetadata(OnHeaderContentTemplateChanged));

        /// <summary>
        /// Gets or sets the header border brush.
        /// </summary>
        /// <value>The header border brush.</value>
        public Brush HeaderBorderBrush
        {
            get { return (Brush)GetValue(HeaderBorderBrushProperty); }
            set { SetValue(HeaderBorderBrushProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderBorderBrushProperty =
            DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(Brush), new PropertyMetadata(OnHeaderContentTemplateChanged));

        /// <summary>
        /// Gets or sets the header margin.
        /// </summary>
        /// <value>The header margin.</value>
        public Thickness HeaderMargin
        {
            get { return (Thickness)GetValue(HeaderMarginProperty); }
            set { SetValue(HeaderMarginProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.Register("HeaderMargin", typeof(Thickness), typeof(Thickness), new PropertyMetadata(OnHeaderContentTemplateChanged));

        /// <summary>
        /// Gets or sets the header content template.
        /// </summary>
        /// <value>The header content template.</value>
        public DataTemplate HeaderContentTemplate
        {
            get { return (DataTemplate)GetValue(HeaderContentTemplateProperty); }
            set { SetValue(HeaderContentTemplateProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderContentTemplateProperty =
            DependencyProperty.Register("HeaderContentTemplate", typeof(DataTemplate), typeof(FileUploadControl), new PropertyMetadata(OnHeaderContentTemplateChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnHeaderContentTemplateChanged(DependencyObject obj,DependencyPropertyChangedEventArgs args){}

        /// <summary>
        /// 
        /// </summary>
        public ControlTemplate HeaderTemplate
        {
            get { return (ControlTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(ControlTemplate), typeof(FileUploadControl), new PropertyMetadata(OnHeaderContentTemplateChanged));

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public Object Header
        {
            get { return (Object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(Object), typeof(FileUploadControl), new PropertyMetadata(OnHeaderChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnHeaderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FileUploadControl), new PropertyMetadata(OnCornerRadiusChanged));

        /// <summary>
        /// Called when [corner radius changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #region Private Fields

        /// <summary>
        /// Flag Variable indicates whether file size is exceeded or not
        /// </summary>
        private int filesizeexceededflag = 0;

        /// <summary>
        /// Bytes upLoaded
        /// </summary>
        private long bytesuploaded = 0;

        /// <summary>
        /// Counter Variable
        /// </summary>
        private int counter = 1;

        /// <summary>
        /// Details Grid
        /// </summary>
        private Grid mdetailsgrid;

        /// <summary>
        /// Represents total upload size of string type
        /// </summary>
        private string mtemptotaluploadsize = string.Empty;

        /// <summary>
        ///  Represents total upload size of string type
        /// </summary>
        private long totalUploadSizeBytes = 0;

        /// <summary>
        /// Thumbnail Image
        /// </summary>
        private Image mthumbnailimage;

        /// <summary>
        /// Stackpanel for displaying the thumbnail image
        /// </summary>
        private StackPanel mthumbnailstackpanel;

        /// <summary>
        /// Total Bytes
        /// </summary>
        private long totalbytes = 0;

        /// <summary>
        /// Text Details
        /// </summary>
        private TextBlock txtdetails;

        /// <summary>
        /// TextBlock that displays total bytes uploaded
        /// </summary>
        private TextBlock txttotalbytes;

        /// <summary>
        /// Total Bytes Heading
        /// </summary>
        private TextBlock txttotalbytesheading;

        /// <summary>
        /// Total files uploaded
        /// </summary>
        private TextBlock txttotalfiles;

        /// <summary>
        /// Total Files Heading
        /// </summary>
        private TextBlock txttotalfilesheading;

        ///// <summary>
        ///// Upload Button
        ///// </summary>
        //private Button uploadbutton;

        /// <summary>
        /// Upload file collection
        /// </summary>
        private UploadFileInfoCollection uploadfilecollection = new UploadFileInfoCollection();

        /// <summary>
        /// 
        /// </summary>
        public UploadFileInfoCollection UploadFileCollection
        {
            get
            {
                return uploadfilecollection;
            }
        }
        ///// <summary>
        ///// Upload file info class
        ///// </summary>
        //private UploadFileInfo uploadfilinfo;

        #endregion end of private variables

        #region Public Dependency Properties

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.CanOverwriteProperty">CanOverwrite</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty CanOverwriteProperty = DependencyProperty.Register("CanOverwrite", typeof(bool), typeof(FileUploadControl), new PropertyMetadata(false, new PropertyChangedCallback(OnCanOverwriteChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.ColumnsToHideProperty">ColumnsToHide</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty ColumnsToHideProperty = DependencyProperty.Register("ColumnsToHide", typeof(ColumnNames[]), typeof(FileUploadControl), new PropertyMetadata(new PropertyChangedCallback(OnColumnsToHideChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.EnableDetailsProperty">EnableDetails</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableDetailsProperty = DependencyProperty.Register("EnableDetails", typeof(bool), typeof(FileUploadControl), new PropertyMetadata(true, new PropertyChangedCallback(OnEnableDetailsChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.EnableThumbnailProperty">EnableThumbnail</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableThumbnailProperty = DependencyProperty.Register("EnableThumbnail", typeof(bool), typeof(FileUploadControl), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableThumbnailChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.FileUploadSizeProperty">FileUploadSize</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty FileUploadSizeProperty = DependencyProperty.Register("FileUploadSize", typeof(long), typeof(FileUploadControl), new PropertyMetadata(50L, new PropertyChangedCallback(OnFileUploadSizeChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.FileUploadSizeUnitProperty">FileUploadSizeUnit</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty FileUploadSizeUnitProperty = DependencyProperty.Register("FileUploadSizeUnit", typeof(string), typeof(FileUploadControl), new PropertyMetadata("KB", new PropertyChangedCallback(OnFileUploadSizeUnitChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.FilterIndexProperty">FilterIndex</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty FilterIndexProperty = DependencyProperty.Register("FilterIndex", typeof(int), typeof(FileUploadControl), new PropertyMetadata(1, new PropertyChangedCallback(OnFilterIndexChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.FilterProperty">Filter</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(FileUploadControl), new PropertyMetadata("AllFiles(*.*)|*.*", new PropertyChangedCallback(OnFilterChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.IsAutomaticUploadProperty">IsAutomaticUpload</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty IsAutomaticUploadProperty = DependencyProperty.Register("IsAutomaticUpload", typeof(bool), typeof(FileUploadControl), new PropertyMetadata(false, new PropertyChangedCallback(OnIsAutomaticUploadChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.IsMultiSelectProperty">IsMultiSelect</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty IsMultiSelectProperty = DependencyProperty.Register("IsMultiSelect", typeof(bool), typeof(FileUploadControl), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMultiSelectChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.NoOfFilesAllowedProperty">NoOfFilesAllowed</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty NoOfFilesAllowedProperty = DependencyProperty.Register("NoOfFilesAllowed", typeof(int), typeof(FileUploadControl), new PropertyMetadata(10, new PropertyChangedCallback(OnNoOfFilesAllowedChanged)));

       
        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalPercentageUploadedProperty">TotalPercentageUploaded</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty TotalPercentageUploadedProperty = DependencyProperty.Register("TotalPercentageUploaded", typeof(long), typeof(FileUploadControl), new PropertyMetadata(100L, new PropertyChangedCallback(OnTotalPercentageUploadedChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalSizeUploadedProperty">TotalSizeUploaded</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty TotalSizeUploadedProperty = DependencyProperty.Register("TotalSizeUploaded", typeof(long), typeof(FileUploadControl), new PropertyMetadata(new PropertyChangedCallback(OnTotalSizeUploadedChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalUploadSizeProperty">TotalUploadSize</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty TotalUploadSizeProperty = DependencyProperty.Register("TotalUploadSize", typeof(long), typeof(FileUploadControl), new PropertyMetadata(100L, new PropertyChangedCallback(OnTotalUploadSizeChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalUploadSizeUnitProperty">TotalUploadSizeUnit</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty TotalUploadSizeUnitProperty = DependencyProperty.Register("TotalUploadSizeUnit", typeof(string), typeof(FileUploadControl), new PropertyMetadata("MB", new PropertyChangedCallback(OnTotalUploadSizeChanged)));

        /// <summary>
        /// Identifies <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.UploadFolderProperty">UploadFolder</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty UploadFolderProperty = DependencyProperty.Register("UploadFolder", typeof(string), typeof(FileUploadControl), new PropertyMetadata(new PropertyChangedCallback(OnUploadFolderChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty QueryStringProperty =
            DependencyProperty.Register("QueryString", typeof(string), typeof(FileUploadControl), new PropertyMetadata(string.Empty));

        #endregion end of Public Dependency properties

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.FileUploadControl">FileUploadControl</see> class
        /// </summary>
        public FileUploadControl()
        {
            DefaultStyleKey = typeof(Syncfusion.Windows.Tools.Controls.FileUploadControl);
            this.uploadfilecollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.Uploadfilecollection_CollectionChanged);
        }

        //static FileUploadControl()
        //{
        //    if (System.ComponentModel.DesignerProperties.IsInDesignTool)
        //    {
        //        Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
        //        load = null;
        //    }
        //}

        #endregion Constructor

        #region Events
        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.CanOverwrite">CanOverwrite</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback CanOverwriteChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.ColumnsToHide">ColumnsToHide</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback ColumnsToHideChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.EnableDetails">EnableDetails</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback EnableDetailsChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.EnableThumbnail">EnableThumbnail</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback EnableThumbnailChanged;

        /// <summary>
        /// Event that is raised when File Count Exceeds the limit
        /// </summary>
        public event FileUploadControlDelegates.FileCountExceededEventHandler FileCountExceededEvent;

        /// <summary>
        /// Event that is raised when the file to be uploaded already exists.
        /// </summary>
        public event FileUploadControlDelegates.FileExistsEventHandler FileExistsEvent;

        /// <summary>
        /// Event that is raised when the file is selected.
        /// </summary>
        public event FileUploadControlDelegates.FilesSelectedEventHandler FilesSelectedEvent;

        /// <summary>
        /// Event that is raised when the selected file too large to handle.
        /// </summary>
        public event FileUploadControlDelegates.FilesTooLargeEventHandler FilesTooLargeEvent;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.FileUploadSize">FileUploadSize</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback FileUploadSizeChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.FileUploadSizeUnit">FileUploadSizeUnit</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback FileUploadSizeUnitChanged;

        /// <summary>
        /// Event that is raised when file uploading starts.
        /// </summary>
        public event FileUploadControlDelegates.FileUploadStartingEventHandler FileUploadStartingEvent;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.Filter">Filter</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback FilterChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.FilterIndex">FilterIndex</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback FilterIndexChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.IsAutomaticUpload">IsAutomaticUpload</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback IsAutomaticUploadChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.IsMultiSelect">IsMultiSelect</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback IsMultiSelectChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.NoOfFilesAllowed">NoOfFilesAllowed</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback NoOfFilesAllowedChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.Theme">Theme</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback ThemeChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalPercentageUploaded">TotalPercentageUploaded</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback TotalPercentageUploadedChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalSizeUploaded">TotalSizeUploaded</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback TotalSizeUploadedChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalUploadSize">TotalUploadSize</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback TotalUploadSizeChanged;

        /// <summary>
        /// Event that is raised when total upload size exceeded.
        /// </summary>
        public event FileUploadControlDelegates.TotalUploadSizeExceededEventHandler TotalUploadSizeExceededEvent;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalUploadSizeUnit">TotalUploadSizeUnit</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback TotalUploadSizeUnitChanged;

        /// <summary>
        /// Event that is raised when the uploading of file is finished.
        /// </summary>
        public event FileUploadControlDelegates.UploadFinishedEventHandler UploadFinishedEvent;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Tools.Controls.FileUploadControl.UploadFolder">UploadFolder</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback UploadFolderChanged;

        /// <summary>
        /// Event that is raised when the uploading of file is paused.
        /// </summary>
        public event FileUploadControlDelegates.UploadPausedEventHandler UploadPausedEvent;

        /// <summary>
        /// Event that is raised when the uploading if resumed.
        /// </summary>
        public event FileUploadControlDelegates.UploadResumedEventHandler UploadResumedEvent;

        #endregion Events

        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether the file can be overwritten or not
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// CanOverwrite=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.CanOverwrite=true;  </para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool CanOverwrite
        {
            get
            {
                return (bool)GetValue(CanOverwriteProperty);
            }

            set
            {
                SetValue(CanOverwriteProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating the column names to be hided
        /// By default all the columns are visible
        /// </summary>
        public ColumnNames[] ColumnsToHide
        {
            get
            {
                return (ColumnNames[])GetValue(ColumnsToHideProperty);
            }

            set
            {
                SetValue(ColumnsToHideProperty, value);
                if (this.Part_DataGrid != null)
                {
                    for (int i = 0; i < value.Count(); i++)
                    {
                        switch (this.ColumnsToHide[i])
                        {
                            case ColumnNames.FileName:
                                this.Part_DataGrid.Columns[0].Visibility = Visibility.Collapsed;
                                break;
                            case ColumnNames.FileSize:
                                this.Part_DataGrid.Columns[1].Visibility = Visibility.Collapsed;
                                break;
                            case ColumnNames.Status:
                                this.Part_DataGrid.Columns[2].Visibility = Visibility.Collapsed;
                                break;
                            case ColumnNames.SizeUploaded:
                                this.Part_DataGrid.Columns[3].Visibility = Visibility.Collapsed;
                                break;
                            case ColumnNames.Progress:
                                this.Part_DataGrid.Columns[4].Visibility = Visibility.Collapsed;
                                break;
                            case ColumnNames.Cancel:
                                this.Part_DataGrid.Columns[5].Visibility = Visibility.Collapsed;
                                break;
                            case ColumnNames.Remove:
                                this.Part_DataGrid.Columns[6].Visibility = Visibility.Collapsed;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the details is enabled or not
        /// true.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// EnableDetails=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.EnableDetails=true;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool EnableDetails
        {
            get { return (bool)GetValue(EnableDetailsProperty); }
            set { SetValue(EnableDetailsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether viewing of Thumbnail images is enabled or not
        /// Default value is false.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// EnableThumbnail=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.EnableThumbnail=true;  </para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool EnableThumbnail
        {
            get
            {
                return (bool)GetValue(EnableThumbnailProperty);
            }

            set
            {
                SetValue(EnableThumbnailProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the size of the file to upload. Default value is 50.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// FileUploadSize=&quot;100&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.FileUploadSize=100;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.int64.aspx">System.Int64</a>
        /// </value>
        public long FileUploadSize
        {
            get
            {
                return (long)GetValue(FileUploadSizeProperty);
            }

            set
            {
                SetValue(FileUploadSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Unit for FileUploadSize Default value is KB.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// FileUploadSizeUnit=&quot;MB&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.FileUploadSizeUnit=&quot;MB&quot;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.string.aspx">System.string</a>
        /// </value>
        public string FileUploadSizeUnit
        {
            get
            {
                return (string)GetValue(FileUploadSizeUnitProperty);
            }

            set
            {
                string temp = value.ToUpper();
                SetValue(FileUploadSizeUnitProperty, temp);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the Filter for selecting the files Default value is
        /// AllFiles(*.*)|*.*.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// Filter=&quot;.jpeg&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.Filter=&quot;.jpeg&quot;; </para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.string.aspx">System.String</a>
        /// </value>
        public string Filter
        {
            get
            {
                return (string)GetValue(FilterProperty);
            }

            set
            {
                SetValue(FilterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the Filter Index Default value is 1.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// FilterIndex=&quot;1&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.FilterIndex=1;  </para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.int32.aspx">System.Int32</a>
        /// </value>
        public int FilterIndex
        {
            get
            {
                return (int)GetValue(FilterIndexProperty);
            }

            set
            {
                if (this.Filter != null)
                {
                    string[] str = this.Filter.Split('|');
                    int index = str.Length / 2;

                    if (value >= 1 && value <= index)
                    {
                        SetValue(FilterIndexProperty, value);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Filter Index Value is out of Range");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Uploading is automatic or not Default value is false.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// IsAutomaticUpload=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.IsAutomaticUpload=true;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool IsAutomaticUpload
        {
            get
            {
                return (bool)GetValue(IsAutomaticUploadProperty);
            }

            set
            {
                SetValue(IsAutomaticUploadProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the multiple selection of file is
        /// allowed or not.
        /// <para> Default value is false.</para>
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// IsMultiSelect=&quot;True&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.IsMultiSelect=true;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.boolean.aspx">System.Boolean</a>
        /// </value>
        public bool IsMultiSelect
        {
            get
            {
                return (bool)GetValue(IsMultiSelectProperty);
            }

            set
            {
                SetValue(IsMultiSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the no of files allowed to upload Default value is 10.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// NoOfFilesAllowed=&quot;10&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.NoOfFilesAllowed=10; </para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.int32.aspx">System.Int32</a>
        /// </value>
        public int NoOfFilesAllowed
        {
            get
            {
                return (int)GetValue(NoOfFilesAllowedProperty);
            }

            set
            {
                // counter = 1;
                SetValue(NoOfFilesAllowedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets  a value indicates the size of Total File Upload. Default value is 100.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// TotalUploadSize=&quot;100&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.TotalUploadSize=100;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.int64.aspx">System.Int64</a>
        /// </value>
        public long TotalUploadSize
        {
            get
            {
                return (long)GetValue(TotalUploadSizeProperty);
            }

            set
            {
                SetValue(TotalUploadSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the Unit for TotalFileUploadSize Default value is
        /// MB.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// TotalUploadSizeUnit=&quot;MB&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.TotalUploadSizeUnit=&quot;MB&quot;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.string.aspx">System.string</a>
        /// </value>
        public string TotalUploadSizeUnit
        {
            get
            {
                return (string)GetValue(TotalUploadSizeUnitProperty);
            }

            set
            {
                SetValue(TotalUploadSizeUnitProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicates the Folder Name where the uploaded files will be saved
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:FileUploadControl Name=&quot;fileupload&quot;
        /// UploadFolder=&quot;NewFolder&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>FileUploadControl fileupload=new FileUploadControl();</para>
        /// <para>fileupload.UploadFolder=&quot;NewFolder&quot;;</para>
        /// </remarks>
        /// <value>
        /// Type: <a href="http://msdn.microsoft.com/en-us/library/system.string.aspx">System.String</a>
        /// </value>
        public string UploadFolder
        {
            get
            {
                return (string)GetValue(UploadFolderProperty);
            }

            set
            {
                SetValue(UploadFolderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public string QueryString
        {
            get { return (string)GetValue(QueryStringProperty); }
            set { SetValue(QueryStringProperty, value); }
        }

        #endregion Public Properties

        #region PrivateProperties

        /// <summary>
        /// Gets or sets a value indicating the total size uploaded
        /// </summary>
        private long TotalSizeUploaded
        {
            get
            {
                return (long)GetValue(TotalSizeUploadedProperty);
            }

            set
            {
                SetValue(TotalSizeUploadedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the percentage uploaded
        /// </summary>
        private long TotalPercentageUploaded
        {
            get
            {
                return (long)GetValue(TotalPercentageUploadedProperty);
            }

            set
            {
                SetValue(TotalPercentageUploadedProperty, value);
            }
        }

        #endregion
        
        #region Overrides
        DataGrid Part_DataGrid;
        Grid PART_Root;

        /// <summary>
        /// Applies the Template for the File Upload control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Part_DataGrid != null)
            {
                this.Part_DataGrid.SelectionChanged -= new SelectionChangedEventHandler(Part_DataGrid_SelectionChanged);
            }
            this.PART_Root = this.GetTemplateChild("PART_Root") as Grid;
            this.Part_DataGrid = this.GetTemplateChild("Part_DataGrid") as DataGrid;
            if (this.Part_DataGrid != null)
            {
                if (this.Part_DataGrid.ItemsSource == null && this.UploadFileCollection.Count > 0)
                {
                    this.Part_DataGrid.ItemsSource = this.UploadFileCollection;
                }
                this.Part_DataGrid.SelectionChanged += new SelectionChangedEventHandler(Part_DataGrid_SelectionChanged);
            }

            this.mthumbnailstackpanel = this.GetTemplateChild("ThumbnailStackpanel") as StackPanel;
            this.mthumbnailimage = this.GetTemplateChild("ThumbnailImage") as Image;
            this.txtdetails = this.GetTemplateChild("TxtDetails") as TextBlock;
            this.txttotalfilesheading = this.GetTemplateChild("TxtTotalFilesHeading") as TextBlock;
            this.txttotalbytesheading = this.GetTemplateChild("TxtTotalBytesHeading") as TextBlock;
            this.txttotalfiles = this.GetTemplateChild("TxtTotalFiles") as TextBlock;
            this.txttotalbytes = this.GetTemplateChild("TxtTotalBytes") as TextBlock;

            this.mdetailsgrid = this.GetTemplateChild("DetailsGrid") as Grid;

            if (this.ColumnsToHide != null)
            {
                for (int i = 0; i < this.ColumnsToHide.Count(); i++)
                {
                    switch (this.ColumnsToHide[i])
                    {
                        case ColumnNames.FileName:
                            this.Part_DataGrid.Columns[0].Visibility = Visibility.Collapsed;
                            break;
                        case ColumnNames.FileSize:
                            this.Part_DataGrid.Columns[1].Visibility = Visibility.Collapsed;
                            break;
                        case ColumnNames.Status:
                            this.Part_DataGrid.Columns[2].Visibility = Visibility.Collapsed;
                            break;
                        case ColumnNames.SizeUploaded:
                            this.Part_DataGrid.Columns[3].Visibility = Visibility.Collapsed;
                            break;
                        case ColumnNames.Progress:
                            this.Part_DataGrid.Columns[4].Visibility = Visibility.Collapsed;
                            break;
                        case ColumnNames.Cancel:
                            this.Part_DataGrid.Columns[5].Visibility = Visibility.Collapsed;
                            break;
                        case ColumnNames.Remove:
                            this.Part_DataGrid.Columns[6].Visibility = Visibility.Collapsed;
                            break;
                    }
                }
            }
        }

        void Part_DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.EnableThumbnail && Part_DataGrid.SelectedItem!=null)
            {
                FileInfo file = ((UploadFileInfo)this.Part_DataGrid.SelectedItem).File;

                string extent = file.Extension.ToLower();
                if (extent == ".jpg" || extent == ".gif" || extent == ".bmp" || extent == ".png" || extent == ".icon")
                {
                    Stream stream = file.OpenRead();
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(stream);
                    this.mthumbnailimage.Source = bitmapImage;
                    this.mthumbnailimage.Width = 125;
                    this.mthumbnailimage.Height = 65;
                    this.mthumbnailimage.Visibility = Visibility.Visible;
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Method that raised UploadFinished Event
        /// </summary>
        /// <param name="flag" >Boolean variable</param>
        /// <param name="uploadfileinfo" >Indicates file upon which this event fires</param>
        internal void RaiseUploadFinishedEvent(bool flag, UploadFileInfo uploadfileinfo)
        {
            if (flag)
            {
                FileEventArgs args = new FileEventArgs();
                args.File = uploadfileinfo;
                if (this.UploadFinishedEvent != null)
                {
                    this.UploadFinishedEvent(this, args);
                }
            }
        }

        /// <summary>
        /// Method that raised FileExistsEvent
        /// </summary>
        /// <param name="flag" >Boolean varible</param>
        /// <param name="uploadfileinfo" >Indicates file upon which this event fires</param>
        internal void RaiseFileExistsEvent(bool flag, UploadFileInfo uploadfileinfo)
        {
            if (flag)
            {
                FileEventArgs args = new FileEventArgs();
                args.File = uploadfileinfo;
                if (this.FileExistsEvent != null)
                {
                    if (this.CanOverwrite != true)
                    {
                        this.txttotalfiles.Text = (int.Parse(this.txttotalfiles.Text) - 1).ToString();
                        this.totalbytes = this.totalbytes - args.File.FileLength;
                        this.txttotalbytes.Text = this.BytesConvertor(this.totalbytes);
                    }

                    this.FileExistsEvent(this, args);
                }
            }
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.UploadFolderChanged">UploadFolderChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnUploadFolderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.UploadFolderChanged != null)
            {
                this.UploadFolderChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalUploadSizeUnitChanged">TotalUploadSizeUnitChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTotalUploadSizeUnitChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TotalUploadSizeUnitChanged != null)
            {
                this.TotalUploadSizeUnitChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalUploadSizeChanged">TotalUploadSizeChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTotalUploadSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TotalUploadSizeChanged != null)
            {
                this.TotalUploadSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalSizeUploadedChanged">TotalSizeUploadedChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTotalSizeUploadedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TotalSizeUploadedChanged != null)
            {
                this.TotalSizeUploadedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.TotalPercentageUploadedChanged">TotalPercentageUploadedChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTotalPercentageUploadedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TotalPercentageUploadedChanged != null)
            {
                this.TotalPercentageUploadedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.ThemeChanged">ThemeChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnThemeChanged(DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl newfileupload = new FileUploadControl();

            if (this.ThemeChanged != null)
            {
                this.ThemeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.NoOfFilesAllowedChanged">NoOfFilesAllowedChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnNoOfFilesAllowedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.NoOfFilesAllowedChanged != null)
            {
                this.NoOfFilesAllowedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.IsMultiSelectChanged">IsMultiSelectChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsMultiSelectChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsMultiSelectChanged != null)
            {
                this.IsMultiSelectChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.IsAutomaticUploadChanged">IsAutomaticUploadChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsAutomaticUploadChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsAutomaticUploadChanged != null)
            {
                this.IsAutomaticUploadChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.FilterIndexChanged">FilterIndexChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFilterIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FilterIndexChanged != null)
            {
                this.FilterIndexChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.FilterChanged">FilterChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFilterChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FilterChanged != null)
            {
                this.FilterChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.FileUploadSizeUnitChanged">FileUploadSizeUnitChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFileUploadSizeUnitChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FileUploadSizeUnitChanged != null)
            {
                this.FileUploadSizeUnitChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.FileUploadSizeChanged">FileUploadSizeChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFileUploadSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FileUploadSizeChanged != null)
            {
                this.FileUploadSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.EnableThumbnailChanged">EnableThumbnailChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnEnableThumbnailChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EnableThumbnailChanged != null)
            {
                this.EnableThumbnailChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.EnableDetailsChanged">EnableDetailsChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnEnableDetailsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EnableDetailsChanged != null)
            {
                this.EnableDetailsChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.ColumnsToHideChanged">ColumnsToHideChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnColumnsToHideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ColumnsToHideChanged != null)
            {
                this.ColumnsToHideChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="F:Syncfusion.Windows.Tools.Controls.FileUploadControl.CanOverwriteChanged">CanOverwriteChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCanOverwriteChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CanOverwriteChanged != null)
            {
                this.CanOverwriteChanged(this, e);
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Calls OnCanOverwriteChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCanOverwriteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnCanOverwriteChanged(e);
        }

        /// <summary>
        /// Calls OnColumnsToHideChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnColumnsToHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnColumnsToHideChanged(e);
        }

        /// <summary>
        /// Calls OnEnableDetailsChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnEnableDetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnEnableDetailsChanged(e);
        }

        /// <summary>
        /// Calls OnEnableThumbnailChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnEnableThumbnailChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnEnableThumbnailChanged(e);
        }

        /// <summary>
        /// Calls OnFileUploadSizeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnFileUploadSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnFileUploadSizeChanged(e);
        }

        /// <summary>
        /// Calls OnFileUploadSizeUnitChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnFileUploadSizeUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnFileUploadSizeUnitChanged(e);
        }

        /// <summary>
        /// Calls OnFilterChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnFilterChanged(e);
        }

        /// <summary>
        /// Calls OnFilterIndexChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnFilterIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnFilterIndexChanged(e);
        }

        /// <summary>
        /// Calls OnIsAutomaticUploadChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsAutomaticUploadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnIsAutomaticUploadChanged(e);
        }

        /// <summary>
        /// Calls OnIsMultiSelectChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsMultiSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnIsMultiSelectChanged(e);
        }

        /// <summary>
        /// Calls OnNoOfFilesAllowedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnNoOfFilesAllowedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnNoOfFilesAllowedChanged(e);
        }

        /// <summary>
        /// Calls OnThemeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnThemeChanged(e);
        }

        /// <summary>
        /// Calls OnTotalPercentageUploadedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTotalPercentageUploadedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnTotalPercentageUploadedChanged(e);
        }

        /// <summary>
        /// Calls OnTotalSizeUploadedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTotalSizeUploadedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnTotalSizeUploadedChanged(e);
        }

        /// <summary>
        /// Calls OnTotalUploadSizeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTotalUploadSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnTotalUploadSizeChanged(e);
        }

        /// <summary>
        /// Calls OnTotalUploadSizeUnitChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTotalUploadSizeUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnTotalUploadSizeUnitChanged(e);
        }

        /// <summary>
        /// Calls OnUploadFolderChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d" >Current FileUpload Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnUploadFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileUploadControl obj = (FileUploadControl)d;
            obj.OnUploadFolderChanged(e);
        }
        #endregion Private StaticMethods

        #region Priavate Methods
        /// <summary>
        /// Method add the files for uploading into the collection 
        /// </summary>
        /// <param name="uploadfileinfo">Contains information about the file to be uploaded</param>
        private void AddtoUploadFileCollection(UploadFileInfo uploadfileinfo)
        {
            bool fileduplicateflag = false;
            for (int i = 0; i < this.uploadfilecollection.Count; i++)
            {
                if (uploadfileinfo.FileName == this.uploadfilecollection[i].FileName)
                {
                    fileduplicateflag = true;
                    break;
                }
            }

            if (fileduplicateflag)
            {
                this.counter = this.counter - 1;
                MessageBox.Show(uploadfileinfo.FileName + " already  exists in the list", "File Upload Control", MessageBoxButton.OK);
            }
            else
            {
                this.uploadfilecollection.Add(uploadfileinfo);
                if (Part_DataGrid != null)
                {
                    this.Part_DataGrid.ItemsSource = this.uploadfilecollection;
                }
            }
        }

        /// <summary>
        /// Method thats converts bytes into string(KB, MB, GB)
        /// </summary>
        /// <param name="val">Byes value to be converted</param>
        /// <returns>Returns converted String value</returns>
        private string BytesConvertor(long val)
        {
            string size = "0 KB";
            double count = (double)val;
            if (count >= 1073741824)
            {
                size = String.Format("{0:##.##}", count / 1073741824) + " GB";
            }
            else if (count >= 1048576)
            {
                size = String.Format("{0:##.##}", count / 1048576) + " MB";
            }
            else if (count >= 1024)
            {
                size = String.Format("{0:##.##}", count / 1024) + " KB";
            }
            else if (count > 0 && count < 1024)
            {
                size = "1 KB";
            }

            return size;
        }

        /// <summary>
        /// Method that determines the files to upload
        /// </summary>
        /// <param name="file">The FileInfo </param>
        private void Filestoupload(FileInfo file)
        {
            long value = 0;
            FilesEventArgs sizeexceededeventargs = new FilesEventArgs();
            if (this.counter > this.NoOfFilesAllowed)
            {
                MessageBox.Show(string.Format("Number of files allowed : {0}", this.NoOfFilesAllowed), "File Upload Control", MessageBoxButton.OK);
            }
            else
            {
                bool totaluploadsizeexceededflag = false;

                UploadFileInfo uploadFileInfo = new UploadFileInfo(this.Dispatcher, this);
                this.counter = this.counter + 1;
                uploadFileInfo.File = file;
                uploadFileInfo.FileId = this.counter;
                uploadFileInfo.FileName = file.Name;
                Stream tempstream = file.OpenRead();
                uploadFileInfo.FileStatus = "Queueing";

                value = tempstream.Length;
                uploadFileInfo.FileLength = value;
                tempstream.Close();
                tempstream = null;
                if (value <= 0)
                {
                    MessageBox.Show("File : " + uploadFileInfo.FileName + "   Empty", "File Upload Control", MessageBoxButton.OK);
                }
                else
                {
                    uploadFileInfo.FileSize = this.BytesConvertor(value);
                    string[] strcomp1 = uploadFileInfo.FileSize.Split(' ');

                    switch (this.FileUploadSizeUnit)
                    {
                        case "KB":
                            if (strcomp1[1] == "KB")
                            {
                                if (double.Parse(strcomp1[0]) > double.Parse(this.FileUploadSize.ToString()))
                                {
                                    sizeexceededeventargs.FilesList.Add(file);
                                    this.filesizeexceededflag = 1;
                                    MessageBox.Show("File size exceeds maximum size limit", "File Upload Control", MessageBoxButton.OK);
                                }
                                else
                                {
                                    this.totalUploadSizeBytes = this.totalUploadSizeBytes + uploadFileInfo.FileLength;
                                    this.mtemptotaluploadsize = this.BytesConvertor(this.totalUploadSizeBytes);

                                    totaluploadsizeexceededflag = this.TotolUploadSizeCheck(this.mtemptotaluploadsize);

                                    if (totaluploadsizeexceededflag)
                                    {
                                        this.AddtoUploadFileCollection(uploadFileInfo);
                                    }
                                    else
                                    {
                                        this.counter = this.counter - 1;
                                        this.totalUploadSizeBytes = this.totalUploadSizeBytes - uploadFileInfo.FileLength;
                                    }
                                }
                            }
                            else
                            {
                                sizeexceededeventargs.FilesList.Add(file);
                                filesizeexceededflag = 1;
                                MessageBox.Show("File size exceeds maximum size limit", "File Upload Control", MessageBoxButton.OK);
                            }

                            break;
                        case "MB":
                            if (strcomp1[1] == "KB" || strcomp1[1] == "MB")
                            {
                                if (strcomp1[1] == "MB")
                                {
                                    if (double.Parse(strcomp1[0]) > double.Parse(this.FileUploadSize.ToString()))
                                    {
                                        sizeexceededeventargs.FilesList.Add(file);
                                        this.filesizeexceededflag = 1;
                                        MessageBox.Show("File size exceeds maximum size limit", "File Upload Control", MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        this.totalUploadSizeBytes = this.totalUploadSizeBytes + uploadFileInfo.FileLength;
                                        this.mtemptotaluploadsize = this.BytesConvertor(this.totalUploadSizeBytes);
                                        totaluploadsizeexceededflag = this.TotolUploadSizeCheck(this.mtemptotaluploadsize);
                                        if (totaluploadsizeexceededflag)
                                        {
                                            this.AddtoUploadFileCollection(uploadFileInfo);
                                        }
                                        else
                                        {
                                            this.counter = this.counter - 1;
                                            this.totalUploadSizeBytes = this.totalUploadSizeBytes - uploadFileInfo.FileLength;
                                        }
                                    }
                                }
                                else
                                {
                                    this.totalUploadSizeBytes = this.totalUploadSizeBytes + uploadFileInfo.FileLength;
                                    this.mtemptotaluploadsize = this.BytesConvertor(this.totalUploadSizeBytes);
                                    totaluploadsizeexceededflag = this.TotolUploadSizeCheck(this.mtemptotaluploadsize);
                                    if (totaluploadsizeexceededflag)
                                    {
                                        this.AddtoUploadFileCollection(uploadFileInfo);
                                    }
                                    else
                                    {
                                        this.counter = this.counter - 1;
                                        this.totalUploadSizeBytes = this.totalUploadSizeBytes - uploadFileInfo.FileLength;
                                    }
                                }
                            }

                            break;
                        case "GB":
                            if (strcomp1[1] == "KB" || strcomp1[1] == "MB" || strcomp1[1] == "GB")
                            {
                                if (strcomp1[1] == "GB")
                                {
                                    if (double.Parse(strcomp1[0]) > double.Parse(this.FileUploadSize.ToString()))
                                    {
                                        sizeexceededeventargs.FilesList.Add(file);
                                        this.filesizeexceededflag = 1;
                                        MessageBox.Show("File size exceeds maximum size limit", "File Upload Control", MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        this.totalUploadSizeBytes = this.totalUploadSizeBytes + uploadFileInfo.FileLength;
                                        this.mtemptotaluploadsize = this.BytesConvertor(this.totalUploadSizeBytes);
                                        totaluploadsizeexceededflag = this.TotolUploadSizeCheck(this.mtemptotaluploadsize);
                                        if (totaluploadsizeexceededflag)
                                        {
                                            this.AddtoUploadFileCollection(uploadFileInfo);
                                        }
                                        else
                                        {
                                            this.counter = this.counter - 1;
                                            this.totalUploadSizeBytes = this.totalUploadSizeBytes - uploadFileInfo.FileLength;
                                        }
                                    }
                                }
                                else
                                {
                                    this.totalUploadSizeBytes = this.totalUploadSizeBytes + uploadFileInfo.FileLength;
                                    this.mtemptotaluploadsize = this.BytesConvertor(this.totalUploadSizeBytes);
                                    totaluploadsizeexceededflag = this.TotolUploadSizeCheck(this.mtemptotaluploadsize);
                                    if (totaluploadsizeexceededflag)
                                    {
                                        this.AddtoUploadFileCollection(uploadFileInfo);
                                    }
                                    else
                                    {
                                        this.counter = this.counter - 1;
                                        this.totalUploadSizeBytes = this.totalUploadSizeBytes - uploadFileInfo.FileLength;
                                    }
                                }
                            }

                            break;
                        default:
                            MessageBox.Show("File size exceeds maximum size limit", "File Upload Control", MessageBoxButton.OK);
                            break;
                    }

                    if (this.filesizeexceededflag == 1)
                    {
                        if (this.FilesTooLargeEvent != null)
                        {
                            this.FilesTooLargeEvent(this, sizeexceededeventargs);
                        }
                    }

                    if (totaluploadsizeexceededflag)
                    {
                        if (this.TotalUploadSizeExceededEvent != null)
                        {
                            this.TotalUploadSizeExceededEvent(this, sizeexceededeventargs);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Provides handles for UploadFileCollection Collectionchanged event
        /// </summary>
        /// <param name="sender" >The uploadfilecollection</param>
        /// <param name="e" >Event Argument</param>
        private void Uploadfilecollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.totalbytes = 0;
            this.txttotalbytes.Text = this.BytesConvertor(this.totalbytes);
            if (this.uploadfilecollection.Count >= 0)
            {
                this.txttotalfiles.Text = "0";
            }

            for (int i = 0; i < this.uploadfilecollection.Count; i++)
            {
                if (this.uploadfilecollection[i].FileStatus != "Removed")
                {
                    this.txttotalfiles.Text = (int.Parse(this.txttotalfiles.Text) + 1).ToString();
                    this.totalbytes = this.totalbytes + this.uploadfilecollection[i].FileLength;
                    this.bytesuploaded = this.bytesuploaded + this.uploadfilecollection[i].SizeUploaded;
                    this.txttotalbytes.Text = this.BytesConvertor(this.totalbytes);
                }
            }
        }

        /// <summary>
        /// ReadCallback  Method
        /// </summary>
        /// <param name="asynchronousResult" >The Asynchronous Result</param>
        private void ReadCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webrequest = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)webrequest.EndGetResponse(asynchronousResult);
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responsestring = reader.ReadToEnd();
            reader.Close();
        }

        /// <summary>
        /// Method that removes the file from the server
        /// </summary>
        /// <param name="result" >IAsyncResult Object</param>
        private void RemoveFromServer(IAsyncResult result)
        {
            HttpWebRequest webrequest = (HttpWebRequest)result.AsyncState;
            webrequest.BeginGetResponse(new AsyncCallback(this.ReadCallback), webrequest);
        }
        
        /// <summary>
        /// Method that Validates the totalUploadsize
        /// </summary>
        /// <param name="totaluploadsize" >Size to be Validated</param>
        /// <returns >Returns true or false</returns>
        private bool TotolUploadSizeCheck(string totaluploadsize)
        {
            string[] strcomp1 = totaluploadsize.Split(' ');
            switch (this.TotalUploadSizeUnit)
            {
                case "KB":
                    if (strcomp1[1] == "KB")
                    {
                        if (double.Parse(strcomp1[0]) > double.Parse(this.TotalUploadSize.ToString()))
                        {
                            MessageBox.Show("Total upload limit exceeded", "File Upload Control", MessageBoxButton.OK);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Total upload limit exceeded", "File Upload Control", MessageBoxButton.OK);
                        return false;
                    }

                case "MB":
                    if (strcomp1[1] == "KB" || strcomp1[1] == "MB")
                    {
                        if (strcomp1[1] == "MB")
                        {
                            if (double.Parse(strcomp1[0]) > double.Parse(this.TotalUploadSize.ToString()))
                            {
                                MessageBox.Show("Total upload limit exceeded", "File Upload Control", MessageBoxButton.OK);
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                    break;
                case "GB":
                    if (strcomp1[1] == "KB" || strcomp1[1] == "MB" || strcomp1[1] == "GB")
                    {
                        if (strcomp1[1] == "GB")
                        {
                            if (double.Parse(strcomp1[0]) > double.Parse(this.TotalUploadSize.ToString()))
                            {
                                MessageBox.Show("Total upload limit exceeded", "File Upload Control", MessageBoxButton.OK);
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                    break;
                default:
                    return false;
            }

            return false;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Uploads this instance.
        /// </summary>
        /// <returns></returns>
        public bool Upload()
        {
            if (this.uploadfilecollection.Count > 0)
            {
                bool uploadcheckflag = false;
                for (int i = 0; i < this.uploadfilecollection.Count; i++)
                {
                    if (this.uploadfilecollection[i].FileStatus == "Queueing" || this.uploadfilecollection[i].FileStatus == "Canceled")
                    {
                        uploadcheckflag = true;
                        //return false;
                        break;
                    }
                }

                if (uploadcheckflag)
                {
                    for (int i = 0; i < this.uploadfilecollection.Count; i++)
                    {
                        if (this.uploadfilecollection[i].FileStatus != "Uploading" && this.uploadfilecollection[i].FileStatus != "Completed" && this.uploadfilecollection[i].FileStatus != "Removed")
                        {
                            FileEventArgs fileeventargs = new FileEventArgs();
                            fileeventargs.File = (UploadFileInfo)this.uploadfilecollection[i];
                            if (this.FileUploadStartingEvent != null)
                            {
                                this.FileUploadStartingEvent(this, fileeventargs);
                            }

                            if (this.uploadfilecollection[i].FileStatus == "Canceled")
                            {
                                if (this.UploadResumedEvent != null)
                                {
                                    this.UploadResumedEvent(this, fileeventargs);
                                }
                            }

                            if (this.UploadFolder == null)
                            {
                                throw new Exception("Upload Folder is Empty");
                            }
                            else
                            {
                                this.uploadfilecollection[i].Upload(this.UploadFolder, this.QueryString);
                            }
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Uploads the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        public bool Upload(UploadFileInfo info)
        {
            int i = this.UploadFileCollection.IndexOf(info);
            if (i < 0)
                return false;
            else
            {
                if (this.uploadfilecollection[i].FileStatus == "Queueing" || this.uploadfilecollection[i].FileStatus == "Canceled")
                    return false;
                else
                {
                    if (this.uploadfilecollection[i].FileStatus != "Uploading" && this.uploadfilecollection[i].FileStatus != "Completed" && this.uploadfilecollection[i].FileStatus != "Removed")
                    {
                        this.uploadfilecollection[i].Upload(this.UploadFolder, this.QueryString);
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// Method that removes the file from the server and DataGrid
        /// </summary>
        public bool Remove(UploadFileInfo info)
        {
            int index = this.UploadFileCollection.IndexOf(info);
            MessageBoxResult result;
            if (index < 0)
                return false;

            if (this.uploadfilecollection[index].FileStatus == "Uploading" || this.uploadfilecollection[index].FileStatus == "Overwriting")
            {
                MessageBox.Show("Upload in progress, stop upload and try again.", "File Upload Control", MessageBoxButton.OK);
                return false;
            }

            else if (this.uploadfilecollection[index].FileStatus == "Queueing")
            {
                result = MessageBox.Show("Do you want to continue file removal?", "File Upload Control", MessageBoxButton.OKCancel);
                //result = 

                if (result == MessageBoxResult.OK)
                {
                    this.totalUploadSizeBytes = this.totalUploadSizeBytes - this.uploadfilecollection[index].FileLength;
                    this.uploadfilecollection.RemoveAt(index);
                    this.counter = this.counter - 1;
                    MessageBox.Show("File successfully removed", "File Upload Control", MessageBoxButton.OK);
                    return true;
                }
                else
                    return false;
            }
            else
                if (this.uploadfilecollection[index].FileStatus == "Canceled" || this.uploadfilecollection[index].FileStatus == "Uploading" || this.uploadfilecollection[index].FileStatus == "Completed")
                {
                    result = MessageBox.Show("Do you want to continue file removal?", "File Upload Control", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        this.totalUploadSizeBytes = this.totalUploadSizeBytes - this.uploadfilecollection[index].FileLength;
                        this.uploadfilecollection[index].UploadRemove();
                        long totalbytes = 0;
                        for (int i = 0; i < this.uploadfilecollection.Count; i++)
                        {
                            if (this.uploadfilecollection[i].FileStatus == "Completed" || this.uploadfilecollection[i].FileStatus == "Canceled" || this.uploadfilecollection[i].FileStatus == "Uploading")
                                this.totalbytes = this.totalbytes + this.uploadfilecollection[i].FileLength;
                        }

                        this.txttotalbytes.Text = this.BytesConvertor(totalbytes);
                        int filecount = int.Parse(this.txttotalfiles.Text) - 1;
                        this.txttotalfiles.Text = (int.Parse(this.txttotalfiles.Text) - 1).ToString();
                        this.counter = this.counter - 1;
                        this.uploadfilecollection[index].SizeUploaded = 0;
                        this.uploadfilecollection[index].PercentageUploaded = 0;
                        UriBuilder uribuilder = new UriBuilder(this.UploadFolder);
                        uribuilder.Query = "filename=" + this.uploadfilecollection[index].FileName + "&Remove=" + "Remove";
                        HttpWebRequest httpwebrequest = (HttpWebRequest)WebRequest.Create(uribuilder.Uri);
                        httpwebrequest.Method = "POST";
                        httpwebrequest.BeginGetRequestStream(new AsyncCallback(this.RemoveFromServer), httpwebrequest);
                        this.uploadfilecollection.RemoveAt(index);
                        result = MessageBox.Show("File successfully removed", "File Upload Control", MessageBoxButton.OK);
                        if (result == MessageBoxResult.OK)
                        {
                        }
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
        }

        void w_Closed(object sender, ClosedEventArgs e)
        {
            
        }

        /// <summary>
        /// Cancels the upload.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        public bool CancelUpload(UploadFileInfo info)
        {
            int index = this.UploadFileCollection.IndexOf(info);
            if (index < 0)
                return false;
            MessageBoxResult result;

            if (this.uploadfilecollection[index].FileStatus == "Uploading" || this.uploadfilecollection[index].FileStatus == "Overwriting")
            {
                result = MessageBox.Show("Do you want to cancel upload?", "File Upload Control", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    this.uploadfilecollection[index].UploadCancel();
                    FileEventArgs fileuploadpausedeventargs = new FileEventArgs();
                    fileuploadpausedeventargs.File = (UploadFileInfo)this.uploadfilecollection[index];

                    FireUploadPausedEvent(fileuploadpausedeventargs);

                    MessageBox.Show("Upload cancelled.", "File Upload Control", MessageBoxButton.OK);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        
        #endregion

        #region Events
        internal void FireUploadPausedEvent(FileEventArgs fileuploadpausedeventargs)
        {
            if (this.UploadPausedEvent != null)
            {
                this.UploadPausedEvent(this, fileuploadpausedeventargs);
            }
        }
        #endregion

        private FileUploadCommand uploadCommand;
        /// <summary>
        /// 
        /// </summary>
        public FileUploadCommand UploadCommand
        {
            get
            {
                if (uploadCommand == null)
                {
                    uploadCommand = new FileUploadCommand(outputParam => OnUploadCommand());
                }
                return uploadCommand;
            }
        }

        private void OnUploadCommand()
        {
            Upload();
        }

        private FileUploadCommand clearCommand;
        /// <summary>
        /// 
        /// </summary>
        public FileUploadCommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                {
                    clearCommand = new FileUploadCommand(outputParam => OnClearCommand());
                }
                return clearCommand;
            }
        }

        private void OnClearCommand()
        {
            bool cancelflag = false;
            if (this.uploadfilecollection.Count > 0)
            {
                for (int i = 0; i < this.uploadfilecollection.Count; i++)
                {
                    if (this.uploadfilecollection[i].FileStatus == "Queueing" || this.uploadfilecollection[i].FileStatus == "Uploading" || this.uploadfilecollection[i].FileStatus == "Overwriting")
                    {
                        cancelflag = true;
                        break;
                    }
                }

                if (cancelflag)
                {
                    MessageBox.Show("Files pending.", "File Upload Control", MessageBoxButton.OK);
                }
                else
                {
                    this.Part_DataGrid.ItemsSource = null;
                    this.uploadfilecollection.Clear();
                    this.totalUploadSizeBytes = 0;
                    this.counter = 1;
                }
            }
        }

        private FileUploadCommand browseCommand;
        /// <summary>
        /// 
        /// </summary>
        public FileUploadCommand BrowseCommand
        {
            get
            {
                if (browseCommand == null)
                {
                    browseCommand = new FileUploadCommand(outputParam => OnBrowseCommand());
                }
                return browseCommand;
            }
        }

        private void OnBrowseCommand()
        {
            FilesEventArgs fileseventargs = new FilesEventArgs();
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = this.Filter;
            filedialog.FilterIndex = this.FilterIndex;
            filedialog.Multiselect = this.IsMultiSelect;

            if (filedialog.ShowDialog() == true)
            {
                if (filedialog.Multiselect == true)
                {
                    foreach (FileInfo fileinfo in filedialog.Files)
                    {
                        fileseventargs.FilesList.Add(fileinfo);
                        fileseventargs.FilesCount = fileseventargs.FilesList.Count;
                        if (this.counter > this.NoOfFilesAllowed)
                        {
                            if (this.FileCountExceededEvent != null)
                            {
                                this.FileCountExceededEvent(this, fileseventargs);
                            }
                            MessageBox.Show(string.Format("Number of files allowed :  {0}", this.NoOfFilesAllowed), "File Upload Control", MessageBoxButton.OK);
                            break;
                        }
                        else
                            this.Filestoupload(fileinfo);
                    }
                    if (this.IsAutomaticUpload)
                        this.OnUploadCommand();
                }
                else
                {
                    FileInfo file = filedialog.File;
                    this.Filestoupload(file);
                    file = null;
                    if (this.IsAutomaticUpload)
                        this.OnUploadCommand();
                }

                if (this.FilesSelectedEvent != null)
                {
                    this.FilesSelectedEvent(this, fileseventargs);
                }
            }
        }
    }
}
