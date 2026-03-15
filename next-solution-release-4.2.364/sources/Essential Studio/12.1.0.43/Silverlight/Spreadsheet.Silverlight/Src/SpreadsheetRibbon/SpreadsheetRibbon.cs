#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#if SILVERLIGHT
using Syncfusion.Windows.Controls.Theming;
#endif
using Syncfusion.Windows.Tools.Controls;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    [TemplatePart(Name = "Backstage", Type = typeof(Backstage))]
    public class SpreadsheetRibbon : Control, ISkinStylePropagator
    {
        public event EventHandler RibbonLoaded;

        #region Constructor
        public SpreadsheetRibbon()
        {
#if SILVERLIGHT
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
#endif
            DefaultStyleKey = typeof(SpreadsheetRibbon);
            Binding bind = new Binding("DataContext")
                               {
                                   RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                                   Mode = BindingMode.TwoWay
                               };
            SetBinding(MyDataContextProperty, bind);

            FontFamilySource = new ObservableCollection<FontFamily>
                                   {
                                       new FontFamily("Arial"),
                                       new FontFamily("Arial Black"),
                                       new FontFamily("Arial Unicode MS"),
                                       new FontFamily("Calibri"),
                                       new FontFamily("Calibri(Body)"),
                                       new FontFamily("Cambria"),
                                       new FontFamily("Cambria Math"),
                                       new FontFamily("Comic Sans MS"),
                                       new FontFamily("Candara"),
                                       new FontFamily("Consolas"),
                                       new FontFamily("Constantia"),
                                       new FontFamily("Corbel"),
                                       new FontFamily("Courier New"),
                                       new FontFamily("Georgia"),
                                       new FontFamily("Lucida Sans Unicode"),
                                       new FontFamily("Portable User Interface"),
                                       new FontFamily("Segoe UI"),
                                       new FontFamily("Symbol"),
                                       new FontFamily("Tahoma"),
                                       new FontFamily("Times New Roman"),
                                       new FontFamily("Trebuchet MS"),
                                       new FontFamily("Verdana"),
                                       new FontFamily("Webdings"),
                                       new FontFamily("Wingdings 2"),
                                       new FontFamily("Wingdings 3")
                                   };
#if SILVERLIGHT
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
#endif
                NumberFormatSource = new ObservableCollection<NumberFormat>
                                     {
                                         new NumberFormat(SpreadsheetResourceWrapper.General,"General"),
                                         new NumberFormat(SpreadsheetResourceWrapper.Number,"0.00"),
                                         new NumberFormat(SpreadsheetResourceWrapper.Currency,"$#,##0.00"),
                                         new NumberFormat(SpreadsheetResourceWrapper.Accounting,"$* #,##0.00;$*(#,##0.00);$* -??;@"),
                                         new NumberFormat(SpreadsheetResourceWrapper.ShortDate,"m/d/yyyy"),
                                         new NumberFormat(SpreadsheetResourceWrapper.LongDate,"dddd, mmmm dd, yyyy"),
                                         new NumberFormat(SpreadsheetResourceWrapper.Time,"[$-F400]h:mm:ss AM/PM"),
                                         new NumberFormat(SpreadsheetResourceWrapper.Percentage,"0.00%")
                                     };
#if SILVERLIGHT
            }
#endif
#if SILVERLIGHT
            this.Loaded += (s, e) =>
                {
                    IsLoaded = true;
                    if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                        this.UpdatedVisualStyle();
                };
            this.Unloaded += (s, e) =>
                {
                    IsLoaded = false;
                };
#else
            this.Loaded += (s, e) =>
                {
                    this.UpdatedVisualStyle();
                };
#endif
        }
        
        #endregion

        #region Properties

        private Backstage _Backstage;
        public Backstage Backstage
        {
            get { return _Backstage; }
            private set { _Backstage = value; }
        }


        private RibbonTab _HomeRibbonTab;
        public RibbonTab HomeRibbonTab
        {
            get { return _HomeRibbonTab; }
            private set { _HomeRibbonTab = value; }
        }

        private RibbonTab _OthersRibbonTab;
        public RibbonTab OthersRibbonTab
        {
            get { return _OthersRibbonTab; }
            private set { _OthersRibbonTab = value; }
        }

        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(SpreadsheetRibbon), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        public string BackStageHeader
        {
            get { return (string)GetValue(BackStageHeaderProperty); }
            set { SetValue(BackStageHeaderProperty, value); }
        }
        public static readonly DependencyProperty BackStageHeaderProperty =
            DependencyProperty.Register("BackStageHeader", typeof(string), typeof(SpreadsheetRibbon), new PropertyMetadata(SpreadsheetResourceWrapper.File));

        public double BackStageHeight
        {
            get { return (double)GetValue(BackStageHeightProperty); }
            set { SetValue(BackStageHeightProperty, value); }
        }

        public double BackStageWidth
        {
            get { return (double)GetValue(BackStageWidthProperty); }
            set { SetValue(BackStageWidthProperty, value); }
        }

        public static readonly DependencyProperty BackStageHeightProperty =
            DependencyProperty.Register("BackStageHeight", typeof(double), typeof(SpreadsheetRibbon), new PropertyMetadata(null));

        public static readonly DependencyProperty BackStageWidthProperty =
            DependencyProperty.Register("BackStageWidth", typeof(double), typeof(SpreadsheetRibbon), new PropertyMetadata(null));

        private SpreadsheetControl _associatedExcelEditor;
        public SpreadsheetControl AssociatedSpreadsheet
        {
            get { return _associatedExcelEditor; }
            set
            {
                _associatedExcelEditor = value;
            }
        }

        internal object MyDataContext
        {
            get { return GetValue(MyDataContextProperty); }
            set { SetValue(MyDataContextProperty, value); }
        }

        internal static readonly DependencyProperty MyDataContextProperty =
            DependencyProperty.Register("MyDataContext", typeof(object), typeof(SpreadsheetRibbon), new PropertyMetadata(null, OnMyDataContextChanged));

        protected static void OnMyDataContextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SpreadsheetRibbon excelEditorRibbon = (SpreadsheetRibbon)dependencyObject;
            excelEditorRibbon.OnMyDataContextChanged(args);
        }

        protected virtual void OnMyDataContextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is SpreadsheetControl)
            {
                UnWireEvents(e.OldValue as SpreadsheetControl);
            }
            if (e.NewValue is SpreadsheetControl)
            {
                AssociatedSpreadsheet = (SpreadsheetControl)e.NewValue;
                WireEvents(AssociatedSpreadsheet);
            }
        }

        internal void WireEvents(SpreadsheetControl excelEditor)
        {
            excelEditor.Loaded += AssociatedExcelEditorLoaded;
            excelEditor.SizeChanged += AssociatedExcelEditorSizeChanged;
            if (excelEditor != null && excelEditor.ExcelProperties != null)
                excelEditor.ExcelProperties.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ExcelProperties_PropertyChanged);

        }

        internal void UnWireEvents(SpreadsheetControl excelEditor)
        {
            excelEditor.Loaded -= AssociatedExcelEditorLoaded;
            excelEditor.SizeChanged -= AssociatedExcelEditorSizeChanged;
            if (excelEditor != null && excelEditor.ExcelProperties != null)
                excelEditor.ExcelProperties.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(ExcelProperties_PropertyChanged);

        }

        void AssociatedExcelEditorSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Parent is FrameworkElement)
            {
                double height = (Parent as FrameworkElement).ActualHeight;
                BackStageHeight = height > 0 ? height  : 0;
                BackStageWidth = (Parent as FrameworkElement).ActualWidth;
            }
        }

        void AssociatedExcelEditorLoaded(object sender, RoutedEventArgs e)
        {
            if (Parent is FrameworkElement)
            {
                double height = (Parent as FrameworkElement).ActualHeight;
                BackStageHeight = height  > 0 ? height  : 0;
                BackStageWidth = (Parent as FrameworkElement).ActualWidth;
                
            }
        }
        #endregion

        #region OnApplyTemplate

        private Ribbon ExcelRibbon;
        private RibbonComboBox _fontFamily, _fontSizeCombo;
        private ColorPickerPalette _fillColorPicker, _fontColorPicker;
        private RibbonComboBox _numberFormatCombo;
        private RibbonButton _growFontButton;
        private RibbonButton _shrinkFontButton;
        private Backstage _ribbonBackStage;
        private Viewbox _imageViewBox;
        
        private RibbonMenuItem _BottomBorder;
        private RibbonMenuItem _TopBorder;
        private RibbonMenuItem _LeftBorder;
        private RibbonMenuItem _RightBorder;
        private RibbonMenuItem _NoBorder;
        private RibbonMenuItem _AllBorder;
        private RibbonMenuItem _OutsideBorder;
        private RibbonMenuItem _ThickBoxBorder;
        private RibbonMenuItem _ThickBottomBorder;
        
#if !SILVERLIGHT
        private SplitButton _fillColorSplitButton;
        private SplitButton _fontColorSplitbutton;
        private RibbonTextBox _Category;
        private RibbonTextBox _subject;
        private RibbonTextBox _Company;
        private RibbonTextBox _AppName;
        private RibbonTextBox _Author;
        private RibbonTextBox _Manager;
        private SplitButton _borderSplitButton;
#else
        private RibbonSplitButton _fillColorSplitButton;
        private RibbonSplitButton _fontColorSplitbutton;
        private RibbonSplitButton _borderSplitButton;
        BackStageCommandButton _Exit;
#endif
        RibbonButton _NewWorkBook;
        BackStageCommandButton _SaveButton;
        BackStageCommandButton _SaveAsButton;
        BackStageCommandButton _OpenButton;

        private Button _propertiesButton;

      

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_fontFamily != null)
                _fontFamily.SelectionChanged -= FontNameComboSelectionChanged;
            if (_fontSizeCombo != null)
                _fontSizeCombo.SelectionChanged -= FontSizeComboSelectionChanged;
            if (_growFontButton != null)
                _growFontButton.Click -= GrowFontButtonClick;
            if (_shrinkFontButton != null)
                _shrinkFontButton.Click -= ShrinkFontButtonClick;
            if (_fillColorPicker != null)
                _fillColorPicker.ColorChanged -= FillColorPickerColorChanged;
            if (_fontColorPicker != null)
                _fontColorPicker.ColorChanged -= FontColorPickerColorChanged;
            if (_numberFormatCombo != null)
                _numberFormatCombo.SelectionChanged -= NumberFormatComboSelectionChanged;
            if (_ribbonBackStage != null)
                _ribbonBackStage.Loaded -= new RoutedEventHandler(_ribbonBackStage_Loaded);
#if SILVERLIGHT
            if (_Exit != null)
                _Exit.Click -= Exit_Click;
#endif
            if (this._NewWorkBook != null)
                _NewWorkBook.Click -= ClickOnCloseBackStage;
            if (this._OpenButton != null)
                _OpenButton.Click -= ClickOnCloseBackStage;
            if (this._SaveButton != null)
                _SaveButton.Click -= ClickOnCloseBackStage;
            if (_fillColorSplitButton != null)
                _fillColorSplitButton.Click -= _fillColorSplitButton_Click;
            if (_fontColorSplitbutton != null)
                _fontColorSplitbutton.Click -= _fontColorSplitbutton_Click;

            if (this.ExcelRibbon != null)
            {
                this.ExcelRibbon.Loaded -= new RoutedEventHandler(ExcelRibbon_Loaded);
#if !SILVERLIGHT
                this.ExcelRibbon.BackStageOpened -= new EventHandler(ExcelRibbon_BackStageOpened);
#else
                this.ExcelRibbon.BackStage.Loaded -= new RoutedEventHandler(BackStage_Loaded);
#endif
            }

            _fontFamily = GetTemplateChild("FontFamilyCombo") as RibbonComboBox;
            _fontSizeCombo = GetTemplateChild("FontSizeCombo") as RibbonComboBox;
            _growFontButton = GetTemplateChild("GrowFontButton") as RibbonButton;
            _shrinkFontButton = GetTemplateChild("ShrinkFontButton") as RibbonButton;
            _numberFormatCombo = GetTemplateChild("NumberFormatCombo") as RibbonComboBox;
            _fillColorPicker = GetTemplateChild("FillColorPicker") as ColorPickerPalette;
            _fontColorPicker = GetTemplateChild("FontColorPicker") as ColorPickerPalette;
            _ribbonBackStage = GetTemplateChild("RibbonBackStage") as Backstage;
            Backstage = GetTemplateChild("RibbonBackStage") as Backstage;
            _imageViewBox = GetTemplateChild("imageViewBox") as Viewbox;
            ExcelRibbon = GetTemplateChild("ExcelRibbon") as Ribbon;
#if SILVERLIGHT
            _Exit = GetTemplateChild("Exit") as BackStageCommandButton;
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
#endif
            UpdatedVisualStyle();
            _NewWorkBook = GetTemplateChild("NewWorkBook") as RibbonButton;
            _HomeRibbonTab = GetTemplateChild("HomeRibbonTab") as RibbonTab;
            _OthersRibbonTab = GetTemplateChild("OthersRibbonTab") as RibbonTab;
            _SaveButton = this.GetTemplateChild("SaveButton") as BackStageCommandButton;
            _SaveAsButton = this.GetTemplateChild("SaveAsButton") as BackStageCommandButton;
            _OpenButton = this.GetTemplateChild("OpenButton") as BackStageCommandButton;
            _propertiesButton = this.GetTemplateChild("ShowPropertiesButton") as Button;
            
            _BottomBorder = this.GetTemplateChild("BottomBorder") as RibbonMenuItem;
            _TopBorder = this.GetTemplateChild("TopBorder") as RibbonMenuItem;
            _LeftBorder = this.GetTemplateChild("LeftBorder") as RibbonMenuItem;
            _RightBorder = this.GetTemplateChild("RightBorder") as RibbonMenuItem;
            _NoBorder = this.GetTemplateChild("NoBorder") as RibbonMenuItem;
            _AllBorder = this.GetTemplateChild("AllBorder") as RibbonMenuItem;
            _OutsideBorder = this.GetTemplateChild("OutsideBorder") as RibbonMenuItem;
            _ThickBoxBorder = this.GetTemplateChild("ThickBoxBorder") as RibbonMenuItem;
            _ThickBottomBorder = this.GetTemplateChild("ThickBottomBorder") as RibbonMenuItem;
            _TopBorder = this.GetTemplateChild("TopBorder") as RibbonMenuItem;
            _TopBorder = this.GetTemplateChild("TopBorder") as RibbonMenuItem;
           // _DocumentPropertiesGrid = this.GetTemplateChild("DocumentPropertiesGrid") as Grid;

#if SILVERLIGHT
            _borderSplitButton = this.GetTemplateChild("BorderSplitButton") as RibbonSplitButton;
            _fillColorSplitButton = GetTemplateChild("FillColorSplitButton") as RibbonSplitButton;
            _fontColorSplitbutton = GetTemplateChild("FontColorSplitbutton") as RibbonSplitButton;
            if (ExcelRibbon != null)
            {
                if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                    SkinManager.SetVisualStyle(ExcelRibbon, VisualStyle);
            }
#else
            _borderSplitButton = this.GetTemplateChild("BorderSplitButton") as SplitButton;
            _fillColorSplitButton = GetTemplateChild("FillColorSplitButton") as SplitButton;
            _fontColorSplitbutton = GetTemplateChild("FontColorSplitbutton") as SplitButton;
            _Category = GetTemplateChild("TxtCategory") as RibbonTextBox;
            _subject = GetTemplateChild("TxtSubject") as RibbonTextBox;
            _Company = GetTemplateChild("TxtCompany") as RibbonTextBox;
            _AppName = GetTemplateChild("TxtAppName") as RibbonTextBox;
            _Author = GetTemplateChild("TxtAuthor") as RibbonTextBox;
            _Manager = GetTemplateChild("TxtManager") as RibbonTextBox;

#endif
            if (_fontFamily != null)
                _fontFamily.SelectionChanged += FontNameComboSelectionChanged;
            if (_fontSizeCombo != null)
                _fontSizeCombo.SelectionChanged += FontSizeComboSelectionChanged;
            if(_growFontButton!=null)
                _growFontButton.Click += GrowFontButtonClick;
            if(_shrinkFontButton != null)
                _shrinkFontButton.Click += ShrinkFontButtonClick;
            if (_fillColorPicker != null)
                _fillColorPicker.ColorChanged += FillColorPickerColorChanged;
            if (_fontColorPicker != null)
                _fontColorPicker.ColorChanged += FontColorPickerColorChanged;
            if(_fillColorSplitButton != null)
                _fillColorSplitButton.Click += _fillColorSplitButton_Click;
            if(_fontColorSplitbutton != null)
                _fontColorSplitbutton.Click += _fontColorSplitbutton_Click;
            if(_BottomBorder!= null)
                _BottomBorder.Click += new RoutedEventHandler(Border_Click);
            if(_TopBorder!=null)
                _TopBorder.Click += new RoutedEventHandler(Border_Click);
            if (_LeftBorder != null)
                _LeftBorder.Click += new RoutedEventHandler(Border_Click);
            if (_RightBorder != null)
                _RightBorder.Click += new RoutedEventHandler(Border_Click);
            if (_NoBorder != null)
                _NoBorder.Click += new RoutedEventHandler(Border_Click);
            if (_AllBorder != null)
                _AllBorder.Click += new RoutedEventHandler(Border_Click);
            if (_OutsideBorder != null)
                _OutsideBorder.Click += new RoutedEventHandler(Border_Click);
            if (_ThickBoxBorder != null)
                _ThickBoxBorder.Click += new RoutedEventHandler(Border_Click);
            if (_ThickBottomBorder != null)
                _ThickBottomBorder.Click += new RoutedEventHandler(Border_Click);
            if (_borderSplitButton!= null)
                _borderSplitButton.Click += new RoutedEventHandler(_borderSplitButton_Click);

            if (_propertiesButton != null)
            {
                _propertiesButton.Click += new RoutedEventHandler(_propertiesButton_Click);
#if SILVERLIGHT
                _propertiesButton.MouseMove += new System.Windows.Input.MouseEventHandler(_propertiesButton_MouseMove);
                _propertiesButton.MouseLeave += new System.Windows.Input.MouseEventHandler(_propertiesButton_MouseLeave);
#endif
            }

            if (_imageViewBox != null)
            {

#if !SILVERLIGHT
            
                _imageViewBox.MouseDown += new System.Windows.Input.MouseButtonEventHandler(_imageViewBox_MouseDown);
#else
                _imageViewBox.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_imageViewBox_MouseLeftButtonDown);
#endif

               
            }
            if (_numberFormatCombo != null)
            {
#if SILVERLIGHT
                if(_Exit != null)
                    _Exit.Click += Exit_Click;
                if (!(System.ComponentModel.DesignerProperties.IsInDesignTool))
                {
                    _numberFormatCombo.SelectedIndex = 0;
                }
#endif
                _numberFormatCombo.SelectionChanged += NumberFormatComboSelectionChanged;                
            }
            if(_ribbonBackStage !=null)
                _ribbonBackStage.Loaded += new RoutedEventHandler(_ribbonBackStage_Loaded);
            if(this._NewWorkBook != null)
                _NewWorkBook.Click += ClickOnCloseBackStage;
            if (this._OpenButton != null)
                _OpenButton.Click += ClickOnCloseBackStage;
            if(this._SaveButton != null)
                _SaveButton.Click += ClickOnCloseBackStage;
            if (this.ExcelRibbon != null)
            {
                this.ExcelRibbon.Loaded += new RoutedEventHandler(ExcelRibbon_Loaded);
#if !SILVERLIGHT
                this.ExcelRibbon.BackStageOpened += new EventHandler(ExcelRibbon_BackStageOpened);
#else
                this.ExcelRibbon.BackStage.Loaded += new RoutedEventHandler(BackStage_Loaded);
#endif
            }
        }

        void _borderSplitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_borderSplitButton != null)
            {
                string CommandParameter = "BottomBorder";
                if (_borderSplitButton.Tag != null)
                    CommandParameter = _borderSplitButton.Tag.ToString().TrimEnd(".png".ToCharArray());
                if (AssociatedSpreadsheet != null)
                    AssociatedSpreadsheet.BorderCommand.Execute(CommandParameter);
            }
        }

        /// <summary>
        /// Set the small icon for the BorderSplitButton
        /// </summary>
        /// <param name="sender">RibbonMenuItem</param>
        /// <param name="e">RoutedEventArgs</param>
        void Border_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RibbonMenuItem).Tag != null)
            {
#if SILVERLIGHT
                string imagename = "/Syncfusion.Spreadsheet.Silverlight;component/SpreadsheetRibbon/ExcelUI/" + (sender as RibbonMenuItem).Tag as string;
#else
#if ClientProfile
                string imagename = "/Syncfusion.Spreadsheet.WPF.ClientProfile;component/SpreadsheetRibbon/ExcelUI/" + (sender as RibbonMenuItem).Tag as string;
#else
                string imagename = "/Syncfusion.Spreadsheet.Wpf;component/SpreadsheetRibbon/ExcelUI/" + (sender as RibbonMenuItem).Tag as string;
#endif
#endif
                BitmapImage Image = new BitmapImage(new Uri(imagename, UriKind.Relative));
                if (Image != null && _borderSplitButton != null)
                {
                    _borderSplitButton.SmallIcon = Image;
                    _borderSplitButton.Tag = (sender as RibbonMenuItem).Tag;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the _propertiesButton control to set forground of properties button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void _propertiesButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
          _propertiesButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        /// <summary>
        /// Handles the MouseMove event of the _propertiesButton control to set the foreground of properties button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void _propertiesButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
          _propertiesButton.Foreground = new SolidColorBrush(Colors.Orange);
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the _imageViewBox control to open home tab.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void _imageViewBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.HomeRibbonTab.IsChecked = true;
            this.CloseBackStage();
        }

        /// <summary>
        /// Handles the MouseDown event of the _imageViewBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void _imageViewBox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.HomeRibbonTab.IsChecked  = true;
            this.CloseBackStage();
        }

        /// <summary>
        /// Handles the Click event of the _propertiesButton control to swap between all changes to fewer changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void _propertiesButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Grid grd = _propertiesButton.Parent as System.Windows.Controls.Grid;
            
            if (_propertiesButton.Content.ToString() == "Show All Properties")
            {
                _propertiesButton.Content = "Show Fewer Properties";
                grd.RowDefinitions[4].Height = new GridLength(20.0);
               
                grd.RowDefinitions[5].Height = new GridLength(20.0);
                grd.RowDefinitions[6].Height = new GridLength(20.0);
                grd.RowDefinitions[7].Height = new GridLength(20.0);
                grd.RowDefinitions[9].Height = new GridLength(20.0);

#if !SILVERLIGHT
                _Category.Visibility = Visibility.Visible;
                _subject.Visibility = Visibility.Visible;
                _AppName.Visibility = Visibility.Visible;
                _Manager.Visibility = Visibility.Visible;
                _Company.Visibility = Visibility.Visible;
#endif
 


            }

            else
            {
                _propertiesButton.Content = "Show All Properties";

#if !SILVERLIGHT
                _Category.Visibility = Visibility.Hidden;
                _subject.Visibility = Visibility.Hidden;
                _AppName.Visibility = Visibility.Hidden;
                _Manager.Visibility = Visibility.Hidden;
                _Company.Visibility = Visibility.Hidden;
#endif
 
                
                grd.RowDefinitions[4].Height = new GridLength(0.0);
                grd.RowDefinitions[5].Height = new GridLength(0.0);
                grd.RowDefinitions[6].Height = new GridLength(0.0);
                grd.RowDefinitions[7].Height = new GridLength(0.0);
                grd.RowDefinitions[9].Height = new GridLength(0.0);
            }

        }


        void ExcelProperties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CurrentExcelRangeStyle"))
            {
                if (this._numberFormatCombo != null)
                    this._numberFormatCombo.SelectedValue = this.AssociatedSpreadsheet.ExcelProperties.CurrentExcelRangeStyle.NumberFormat;
            }
        }

        void ExcelRibbon_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.RibbonLoaded != null)
                this.RibbonLoaded(this, EventArgs.Empty);
        }

        void ExcelRibbon_BackStageOpened(object sender, EventArgs e)
        {
            if (this.AssociatedSpreadsheet.GridProperties.IsEditing)
            {
                this.AssociatedSpreadsheet.GridProperties.CurrentCell.EndEdit();
            }
        }

        void BackStage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.AssociatedSpreadsheet.GridProperties.IsEditing)
            {
                this.AssociatedSpreadsheet.GridProperties.CurrentCell.EndEdit();
            }
        }

#if SILVERLIGHT
        void Exit_Click(object sender, RoutedEventArgs e)
        {
            CloseBackStage();
        }
#endif
        void ClickOnCloseBackStage(object sender, RoutedEventArgs e)
        {
            CloseBackStage();
        }

        public void CloseBackStage()
        {
            if (this.ExcelRibbon != null && this.ExcelRibbon.BackStageButton != null)
                this.ExcelRibbon.BackStageButton.IsOpen = false;
        }

        public void OpenBackStage()
        {
            if (this.ExcelRibbon != null && this.ExcelRibbon.BackStageButton != null)
                this.ExcelRibbon.BackStageButton.IsOpen = true;
        }

        void _ribbonBackStage_Loaded(object sender, RoutedEventArgs e)
        {
            if (AssociatedSpreadsheet != null)
            {
#if SILVERLIGHT
            WriteableBitmap bi = new WriteableBitmap((int)AssociatedSpreadsheet.ActualWidth, (int)AssociatedSpreadsheet.ActualHeight);
            bi.Render(AssociatedSpreadsheet, new MatrixTransform());
            bi.Invalidate();
#else
                double width = AssociatedSpreadsheet.ActualWidth;
                double height = AssociatedSpreadsheet.ActualHeight;
                RenderTargetBitmap bi = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(AssociatedSpreadsheet);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
                }
                bi.Render(dv);
#endif
                Image img = new Image();
                img.Source = bi;
                img.Margin = new Thickness(5);
                _imageViewBox.Child = img;
            }
        }

        #endregion

        #region FontProperties

        public static readonly DependencyProperty FontSizeSourceProperty = DependencyProperty.Register("FontSizeSource", typeof(double[]), typeof(SpreadsheetRibbon),
            new PropertyMetadata(new double[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 26, 28, 36, 48, 72 }));

        public double[] FontSizeSource
        {
            get
            {
                return (double[])GetValue(FontSizeSourceProperty);
            }
            set
            {
                SetValue(FontSizeSourceProperty, value);
            }
        }

        void FontSizeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_fontSizeCombo != null && _fontSizeCombo.SelectedValue != null && this.AssociatedSpreadsheet != null)
            {
                double fontsize;
                if (double.TryParse(_fontSizeCombo.SelectedValue.ToString(), out fontsize))
                {
                    CommandExtensions.CommandExtensions.ChangeFontSize(AssociatedSpreadsheet, fontsize);
                }
            }
        }

        public IEnumerable FontFamilySource
        {
            get { return (IEnumerable)GetValue(FontFamilySourceProperty); }
            set { SetValue(FontFamilySourceProperty, value); }
        }

        public static readonly DependencyProperty FontFamilySourceProperty =
            DependencyProperty.Register("FontFamilySource", typeof(IEnumerable), typeof(SpreadsheetRibbon), new PropertyMetadata(null));

        void FontNameComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_fontFamily != null && _fontFamily.SelectedValue != null)
                CommandExtensions.CommandExtensions.ChangeFontFamily(AssociatedSpreadsheet, _fontFamily.SelectedValue.ToString());
        }

        void GrowFontButtonClick(object sender, RoutedEventArgs e)
        {
            if (_fontSizeCombo != null && _fontSizeCombo.SelectedValue != null)
            {
                double fontsize;
                if (double.TryParse(_fontSizeCombo.SelectedValue.ToString(), out fontsize))
                {
                    for (int i = 0; i < FontSizeSource.Length; i++)
                    {
                        if (FontSizeSource[i] == fontsize)
                        {
                            CommandExtensions.CommandExtensions.ChangeFontSize(AssociatedSpreadsheet, i + 1 < FontSizeSource.Length ? FontSizeSource[i + 1]
                                                                                   : FontSizeSource[FontSizeSource.Length - 1]);
                        }
                    }
                }
            }
        }

        void ShrinkFontButtonClick(object sender, RoutedEventArgs e)
        {
            if (_fontSizeCombo != null && _fontSizeCombo.SelectedValue != null)
            {
                double fontsize;
                if (double.TryParse(_fontSizeCombo.SelectedValue.ToString(), out fontsize))
                {
                    for (int i = 0; i < FontSizeSource.Length; i++)
                    {
                        if (FontSizeSource[i] == fontsize)
                        {
                            CommandExtensions.CommandExtensions.ChangeFontSize(AssociatedSpreadsheet, i - 1 > 0 ? FontSizeSource[i - 1]
                                                                                   : FontSizeSource[0]);
                        }
                    }
                }
            }
        }

        void FillColorPickerColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_fillColorPicker != null && _fillColorPicker.Color != null)
            {
                CommandExtensions.CommandExtensions.ChangeFillColor(AssociatedSpreadsheet, _fillColorPicker.Color);
                if (_fillColorSplitButton != null) _fillColorSplitButton.IsDropDownOpen = false;
            }
        }

        void FontColorPickerColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_fontColorPicker != null && _fontColorPicker.Color != null)
            {
                CommandExtensions.CommandExtensions.ChangeFontColor(AssociatedSpreadsheet, _fontColorPicker.Color);
                if (_fontColorSplitbutton != null) _fontColorSplitbutton.IsDropDownOpen = false;
            }
        }

        void _fontColorSplitbutton_Click(object sender, RoutedEventArgs e)
        {
            if (_fontColorPicker != null && _fontColorPicker.Color != null)
            {
                CommandExtensions.CommandExtensions.ChangeFontColor(AssociatedSpreadsheet, _fontColorPicker.Color);
                if (_fontColorSplitbutton != null) _fontColorSplitbutton.IsDropDownOpen = false;
            }
        }

        void _fillColorSplitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fillColorPicker != null && _fillColorPicker.Color != null)
            {
                CommandExtensions.CommandExtensions.ChangeFillColor(AssociatedSpreadsheet, _fillColorPicker.Color);
                if (_fillColorSplitButton != null) _fillColorSplitButton.IsDropDownOpen = false;
            }
        }
        #endregion

        #region Number Format

        public IEnumerable NumberFormatSource
        {
            get { return (IEnumerable) GetValue(NumberFormatSourceProperty); }
            set { SetValue(NumberFormatSourceProperty, value); }
        }

        public static readonly DependencyProperty NumberFormatSourceProperty =
            DependencyProperty.Register("NumberFormatSource", typeof (IEnumerable), typeof (SpreadsheetRibbon), new PropertyMetadata(null));

        void NumberFormatComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_numberFormatCombo != null && e.AddedItems.Count > 0)
            {
                var format = (e.AddedItems[0] as NumberFormat).Format;
                CommandExtensions.CommandExtensions.ChangeNumberFormat(AssociatedSpreadsheet, format);
            }
        }

        #endregion

        #region VisualStyle

#if SILVERLIGHT
        bool IsLoaded = false;
        internal Syncfusion.Windows.Controls.Theming.VisualStyle VisualStyle
        {
            get { return (Syncfusion.Windows.Controls.Theming.VisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        internal static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(Syncfusion.Windows.Controls.Theming.VisualStyle), typeof(SpreadsheetRibbon), new PropertyMetadata(Syncfusion.Windows.Controls.Theming.VisualStyle.Office2010Blue, OnVisualStylePropertyChanged));


        public void OnStyleChanged(Syncfusion.Windows.Controls.Theming.VisualStyle _visualStyle)
        {
            VisualStyle = _visualStyle;
        }
#else

        public string VisualStyle
        {
            get { return (string)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(string), typeof(SpreadsheetRibbon), new PropertyMetadata("Office2010Blue", OnVisualStylePropertyChanged));

        public void OnStyleChanged(string _visualStyle)
        {
            VisualStyle = _visualStyle;
        }
        
#endif
        private static void OnVisualStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpreadsheetRibbon control = obj as SpreadsheetRibbon;
#if SILVERLIGHT
            if (control != null && !System.ComponentModel.DesignerProperties.IsInDesignTool)
#endif
                control.UpdatedVisualStyle();
        }

        private void UpdatedVisualStyle()
        {
            if (this.IsLoaded)
            {
#if !SILVERLIGHT
                SkinStorage.SetVisualStyle(this, VisualStyle);
#else
                //SkinManager.SetVisualStyle(this, VisualStyle);
                if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                  SkinManager.SetVisualStyle(ExcelRibbon, VisualStyle);
#endif
            }
        }
#endregion

    }

    public class NumberFormat
    {

        public NumberFormat()
        {
        }

        public NumberFormat(string name,string format)
        {
            Name = name;
            Format = format;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _format;
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }
    }
}
