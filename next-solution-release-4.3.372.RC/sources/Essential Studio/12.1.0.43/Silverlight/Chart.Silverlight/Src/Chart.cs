#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Printing;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class represents Essential Chart Control.
    /// </summary>
    /// <remarks>
    /// Base Class for all classes.  This is Sealed class can't Inherit
    /// </remarks>
    /// <seealso cref="ChartSeries">ChartSeries class specification</seealso>
    /// <seealso cref="ChartArea">ChartArea class specification</seealso>
    /// <seealso cref="ChartAxis">ChartAxis class specification</seealso>
    /// <seealso cref="ChartTypes">ChartTypes enumeration</seealso>
    [ContentProperty("Areas")]
    public class Chart : Control, IDisposable
    {
        #region Chart Styles


        /// <summary>
        /// Identifies the ChartVisualStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartVisualStyleProperty =
       DependencyProperty.Register("ChartVisualStyle", typeof(ChartStyles), typeof(Chart), new PropertyMetadata(ChartStyles.Default, new PropertyChangedCallback(OnVisualStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public ChartStyles ChartVisualStyle
        {
            get
            {
                return (ChartStyles)GetValue(ChartVisualStyleProperty);
            }

            set
            {
                SetValue(ChartVisualStyleProperty, value);
            }
        }

        private static ResourceDictionary rd;
        /// <summary>
        /// Called when VisualStyle property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = d as Chart;
            if (chart != null)
            {
                rd = new SharedResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/ChartStyles.xaml", UriKind.RelativeOrAbsolute)
                };

                if (rd != null)
                {
                    Style Style = rd[chart.ChartVisualStyle.ToString()] as Style;
                    if (Style != null)
                    {
                        chart.Style = Style;
                    }
                }

            }
        }

        /// <summary>
        /// Identifies the AreaStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaStyleProperty =
         DependencyProperty.Register("AreaStyle", typeof(Style), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnAreaStyleChanged)));

        /// <summary>
        /// Gets or sets the LegendStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        public Style AreaStyle
        {
            get
            {
                return (Style)GetValue(AreaStyleProperty);
            }

            set
            {
                SetValue(AreaStyleProperty, value);
            }
        }

        /// <summary>
        /// Called when AreaStyle property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnAreaStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = d as Chart;
            if (chart != null)
            {
                foreach (ChartArea area in chart.Areas)
                {

                    {
                        string str = chart.ChartVisualStyle.ToString();

                        if ((str.Equals("GrayScale")) || (str.Equals("GrayWithBorder")) || (str.Equals("AlphaGray")) || (str.Equals("EnabledGray")) || (str.Equals("GrayScreen") || (str.Equals("BlendGray"))) )
                        {
                            area.ColorModel.Palette = ChartColorPalette.Palette1;
                        }
                        else if ((str.Equals("MixedFantacy")) || (str.Equals("MixedWithBorder")) || (str.Equals("AlphaFantacy")) || (str.Equals("EnabledMixed")) || (str.Equals("MixedScreen") || (str.Equals("MixedBlend"))))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Palette8;
                        }
                        else if ((str.Equals("BlueScale")) || (str.Equals("BlueWithBorder")) || (str.Equals("AlphaBlue")) || (str.Equals("EnabledBlue")) || (str.Equals("BlueScreen") || (str.Equals("BlueBlend"))))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Palette2;
                        }
                        else if ((str.Equals("MaroonRed")) || (str.Equals("RedWithBorder")) || (str.Equals("AlphaRed")) || (str.Equals("EnabledRed")) || (str.Equals("RedScreen") || (str.Equals("RedBlend"))))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Palette3;
                        }
                        else if ((str.Equals("GreenScale")) || (str.Equals("GreenWithBorder")) || (str.Equals("AlphaGreen")) || (str.Equals("EnabledGreen")) || (str.Equals("GreenScreen") || (str.Equals("GreenBlend"))))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Palette4;
                        }
                        else if ((str.Equals("MixedViolet")) || (str.Equals("VioletWithBorder")) || (str.Equals("AlphaViolet")) || (str.Equals("EnabledViolet")) || (str.Equals("VioletScreen") || (str.Equals("VioletBlend"))))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Palette5;
                        }
                        else if ((str.Equals("CoolBlueScale")) || (str.Equals("CoolBlueWithBorder")) || (str.Equals("AlphaCoolBlue")) || (str.Equals("EnabledCoolBlue")) || (str.Equals("CoolBlueScreen") || (str.Equals("CoolBlueBlend"))))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Palette6;
                        }
                        else if ((str.Equals("ChocolateOrange")) || (str.Equals("ChocolateWithBorder")) || (str.Equals("AlphaOrange")) || (str.Equals("EnabledChocolate")) || (str.Equals("ChocolateScreen") || (str.Equals("ChocolateBlend"))))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Palette7;
                        }
                        else if (str.Contains("Default") || (str.Equals("Blend")) || (str.Equals("Office2003")) || (str.Equals("Office2007Blue")) || (str.Equals("Office2007Black")) || (str.Equals("Office2007Silver")))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Default;
                        }
                        else if (str.Contains("Metro"))
                        {
                            area.ColorModel.Palette = ChartColorPalette.Metro;
                        }
                    }

                    area.Style = chart.AreaStyle;
                }
            }
        }




        #endregion
        /// <summary>
        /// Method for clear memory holding elements in chart
        /// </summary>
        public void MemoryLeak()
        {
            
            //SD12357-Comminting this dispatcher action since its triggered twise without calling MemoryLeak method. And this event is included for chart dispose exception and that excetion is not reproduced after removing this event.
             //this.Dispatcher.BeginInvoke((Action)(() =>                   {
                 
                 if (this.Areas != null)
                 {

                     for (int temp = 0; temp < this.Areas.Count; temp++)
                     {
                         for (int j = 0; j < this.Areas[temp].Series.Count; j++)
                         {
                             this.Areas[temp].Series[j].Dispose();
                         }
                             this.Areas[temp].Series.Clear();
                         this.Areas[temp].Dispose();
                     }
                    

                     this.Areas.Clear();
                     this.Areas = null;
                 }
                  
           

            if (m_areasCollection != null)
            {
                for (int temp = 0; temp < this.m_areasCollection.Count; temp++)
                    this.m_areasCollection[temp].Dispose();
                this.m_areasCollection.Clear();
                this.m_areasCollection = null;
            }

            if (AnnotationsLabel != null)
            {
                for (int temp = 0; temp < this.AnnotationsLabel.Count; temp++)
                    this.AnnotationsLabel[temp].Dispose();
                this.AnnotationsLabel.Clear();
                this.AnnotationsLabel = null;
            }

            #region ChartAreaContainer
            Grid mainGrid = this.GetTemplateChild("ChartGrid") as Grid;
            if (mainGrid != null)
            {
                //After implementing the header and footer support, the items control position is changed so modified below line
                ItemsControl ic = (ItemsControl)((mainGrid.Children[1] as Grid).Children[0]);
                if (ic != null)
                {
                    try
                    {
                        UniformChartGrid ug = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(ic, 0), 0) as UniformChartGrid;
                        ug.Dispose();
                        ug = null;
                    }
                    catch
                    {
                    }
                    ic.Items.Clear();
                    ic.ItemsSource = null;
                    ic.ItemsPanel = null;
                    ic.Template = null;
                    ic = null;
                }

                //After implementing the header and footer support, the items control position is changed so modified below line
                AnnotationPresenter ap = (AnnotationPresenter)((mainGrid.Children[1] as Grid).Children[1]);
                if (ap != null)
                {
                    try
                    {
                        Canvas can = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(ap, 0), 0) as Canvas;
                        can.Children.Clear();
                        can = null;
                    }
                    catch
                    {
                    }
                    ap.Dispose();
                    ap = null;
                }

                //if (mainGrid != null)
                //{
                mainGrid.Children.Clear();
                mainGrid = null;
            }
            Border b = this.GetTemplateChild("ChartBorder") as Border;
            if(b!=null)
            b.Child = null;

            #endregion

            this.DataContext = null;
            this.Resources.Clear();
            this.Resources = null;
            var mer_dic = SharedResourceDictionary.SharedDictionaries.Keys.GetEnumerator();
            while(mer_dic.MoveNext())            
            {
                for (int i = 0; i < SharedResourceDictionary.SharedDictionaries[mer_dic.Current].MergedDictionaries.Count; i++)
                {                   
                    SharedResourceDictionary.SharedDictionaries[mer_dic.Current].MergedDictionaries[i].MergedDictionaries.Clear();
                    SharedResourceDictionary.SharedDictionaries[mer_dic.Current].MergedDictionaries[i].Clear();                    
                }
                SharedResourceDictionary.SharedDictionaries[mer_dic.Current].MergedDictionaries.Clear();
                SharedResourceDictionary.SharedDictionaries[mer_dic.Current].Clear();
                
            }
            mer_dic.Dispose();
            SharedResourceDictionary.SharedDictionaries.Clear();
            SharedResourceDictionary.SharedDictionaries = null;
            
             //}));
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        #region Dependency properties
        internal static readonly DependencyProperty ChartPrintDialogStyleProperty =
         DependencyProperty.Register("PrintDialogStyle", typeof(Style), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnChartPrintDialogStyleChanged)));

        /// <summary>
        /// Gets or sets the PrintDialogStyle value.
        /// </summary>
        /// <value>The LegendStyle.</value>
        internal Style PrintDialogStyle
        {
            get
            {
                return (Style)GetValue(ChartPrintDialogStyleProperty);
            }

            set
            {
                SetValue(ChartPrintDialogStyleProperty, value);
            }
        }

        /// <summary>
        /// Called when ChartPrintDialogStyle property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnChartPrintDialogStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = d as Chart;
            if (chart != null)
            {

                chart.PrintDialogStyle = chart.PrintDialogStyle;

            }
        }
        /// <summary>
        /// Idenfities Areas dependency property.
        /// </summary>
        public static readonly DependencyProperty AreasProperty =
            DependencyProperty.Register("Areas", typeof(AreasCollection), typeof(Chart), new PropertyMetadata(null));

        /// <summary>
        /// AreasPanelProperty for the Chart dependency Property
        /// </summary>
        public static readonly DependencyProperty AreasPanelProperty =
            DependencyProperty.Register("AreasPanel", typeof(ItemsPanelTemplate), typeof(Chart), new PropertyMetadata(null));


        /// <summary>
        /// Idenfities Rows dependency property.
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(Chart), new PropertyMetadata(0, new PropertyChangedCallback(OnRowsPropertyChanged)));

        /// <summary>
        /// Idenfities Columns dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(Chart), new PropertyMetadata(0, new PropertyChangedCallback(OnColumnsPropertyChanged)));

        /// <summary>
        /// Idenfities Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Chart), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationPropertyChanged)));
        private AreasCollection m_areasCollection = new AreasCollection();

        /// <summary>
        /// CornerRadius of the Chart Depedency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Chart), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// AnnotationLabel for the Chart Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnnotationsLabelProperty = DependencyProperty.Register("AnnotationsLabel", typeof(ChartAnnotationLabelsCollection), typeof(Chart), new PropertyMetadata(null));


        /// <summary>
        /// AnnotationLabelTemplate for the Chart dependency Property
        /// </summary>
        public static readonly DependencyProperty AnnotationLabelTemplateProperty =
            DependencyProperty.Register("AnnotationLabelTemplate", typeof(DataTemplate), typeof(Chart), new PropertyMetadata(null));

        /// <summary>
        /// Header of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(Chart), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Header for chart area
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Footer of the Chart Area Depedency Property
        /// </summary>
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(object), typeof(Chart), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Footer for chart area
        /// </summary>
        public object Footer
        {
            get { return (object)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        public Chart()
        {
            DefaultStyleKey = typeof(Chart);
            this.printCommand= new PrintCommand(this);
            Areas = new AreasCollection();
        }

        static Chart()
        {
            if (DesignerProperties.IsInDesignTool)
            {
               // LoadDependentAssemblies load = new LoadDependentAssemblies();
               // load = null;

                
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            MemoryLeak();
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the AnnotationLabelTemplate for chart
        /// </summary>
        public DataTemplate AnnotationLabelTemplate
        {
            get { return (DataTemplate)GetValue(AnnotationLabelTemplateProperty); }
            set { SetValue(AnnotationLabelTemplateProperty, value); }
        }
        /// <summary>
        /// Gets or Sets the AnnotationLabel for chart
        /// </summary>
        public ChartAnnotationLabelsCollection AnnotationsLabel
        {
            get { return (ChartAnnotationLabelsCollection)GetValue(AnnotationsLabelProperty); }
            set { SetValue(AnnotationsLabelProperty, value); }
        }
        /// <summary>
        /// Gets or sets the CornerRadius for chart
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the areas collection. This is dependency property.
        /// </summary>
        /// <value>The areas.</value>
        public AreasCollection Areas
        {
            get { return (AreasCollection)GetValue(AreasProperty); }
            set { SetValue(AreasProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Rows. This is dependency property.
        /// </summary>
        /// <value>The Rows value.</value>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ItemPanelTemplate. This is dependency property.
        /// </summary>
        /// <value>The ItemPanelTemplate value.</value>
        public ItemsPanelTemplate AreasPanel
        {
            get { return (ItemsPanelTemplate)GetValue(AreasPanelProperty); }
            set { SetValue(AreasPanelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Columns. This is dependency property.
        /// </summary>
        /// <value>The Columns value.</value>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Orientation. This is dependency property.
        /// </summary>
        /// <value>The Orientation value.</value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Gets the child areas.
        /// </summary>
        public void getchild(DependencyObject control)
        {
            UniformChartGrid areagrid = control as UniformChartGrid;
            if (areagrid != null)
            {
                areagrid.Rows = this.Rows;
                areagrid.Columns = this.Columns;
                areagrid.Orientation = this.Orientation;
            }

            int children = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < children; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                this.getchild(child);
            }
        }

        /// <summary>
        /// Saves chart to disk.
        /// </summary>
        /// <seealso cref="Chart"/>
        public void Save()
        {
            
            WriteableBitmap _bitmap = new WriteableBitmap(this, null);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg,*.jpeg)|*.jpg;*.jpeg|Gif (*.gif)|*.gif|PNG(*.png)|*.png|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                using (Stream fs = sfd.OpenFile())
                {
                    int width = _bitmap.PixelWidth;
                    int height = _bitmap.PixelHeight;

                    ChartImage ei = new ChartImage(width, height);

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            int pixel = _bitmap.Pixels[(i * width) + j];
                            ei.SetPixel(j, i,
                                        (byte)((pixel >> 16) & 0xFF),
                                        (byte)((pixel >> 8) & 0xFF),
                                        (byte)(pixel & 0xFF),
                                        (byte)((pixel >> 24) & 0xFF)
                            );
                        }
                    }
                    Stream png = ei.GetStream();
                    int len = (int)png.Length;
                    byte[] bytes = new byte[len];
                    png.Read(bytes, 0, len);
                    fs.Write(bytes, 0, len);
                }

            }

        }

        /// <summary>
        /// Method for save chart in file format for Print operation
        /// </summary>
        /// <param name="SaveChartAlone"></param>
        public void Save(bool SaveChartAlone)
        {
            int flag = 0;
            if (SaveChartAlone == true)
            {
                if (ZoomingToolKit.GetZoomingToolkitVisibility(this.Areas[0]) == Visibility.Visible)
                {
                    ZoomingToolKit.SetZoomingToolkitVisibility(this.Areas[0], Visibility.Collapsed);
                    flag = 1;
                }
                if (this.Areas[0].PrimaryAxis.ZoomFactor < 1)
                    this.Areas[0].HorizontalBar.Visibility = System.Windows.Visibility.Collapsed;
                if (this.Areas[0].SecondaryAxis.ZoomFactor < 1)
                    this.Areas[0].VerticalBar.Visibility = System.Windows.Visibility.Collapsed;
                if (this.Areas[0].InteractiveCursors.Count >0)
                {
                    this.Areas[0].InteractiveCursors[0].VerticalCursorVisibility = Visibility.Collapsed;
                    this.Areas[0].InteractiveCursors[0].HorizontalCursorVisibility = Visibility.Collapsed;
                }
            }
                WriteableBitmap _bitmap = new WriteableBitmap(this, null);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg,*.jpeg)|*.jpg;*.jpeg|Gif (*.gif)|*.gif|PNG(*.png)|*.png|All files (*.*)|*.*";
                if (sfd.ShowDialog() == true)
                {
                    using (Stream fs = sfd.OpenFile())
                    {
                        int width = _bitmap.PixelWidth;
                        int height = _bitmap.PixelHeight;

                        ChartImage ei = new ChartImage(width, height);

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                int pixel = _bitmap.Pixels[(i * width) + j];
                                ei.SetPixel(j, i,
                                            (byte)((pixel >> 16) & 0xFF),
                                            (byte)((pixel >> 8) & 0xFF),
                                            (byte)(pixel & 0xFF),
                                            (byte)((pixel >> 24) & 0xFF)
                                );
                            }
                        }
                        Stream png = ei.GetStream();
                        int len = (int)png.Length;
                        byte[] bytes = new byte[len];
                        png.Read(bytes, 0, len);
                        fs.Write(bytes, 0, len);
                    }

                }
                if (flag==1)
                {
                    ZoomingToolKit.SetZoomingToolkitVisibility(this.Areas[0], Visibility.Visible);
                    flag = 0;
                }
                if (this.Areas[0].PrimaryAxis.ZoomFactor < 1)
                    this.Areas[0].HorizontalBar.Visibility = System.Windows.Visibility.Visible;
                if (this.Areas[0].SecondaryAxis.ZoomFactor < 1)
                    this.Areas[0].VerticalBar.Visibility = System.Windows.Visibility.Visible;
                if (this.Areas[0].InteractiveCursors.Count > 0)
                {
                    this.Areas[0].InteractiveCursors[0].VerticalCursorVisibility = Visibility.Visible;
                    this.Areas[0].InteractiveCursors[0].HorizontalCursorVisibility = Visibility.Visible;
                }
            
        }

        

        internal ChildWindow PrintDialogChildWindow;
        internal void Print()
        {
            ChartPrintDialog printdialog = new ChartPrintDialog(this);
            printdialog.PrintDialogStyle = this.PrintDialogStyle;
            PrintDialogChildWindow = new ChildWindow();
            PrintDialogChildWindow.Content = printdialog;
            PrintDialogChildWindow.Title = "Chart Print Dialog";
            PrintDialogChildWindow.Show();

        }


       



        /// <summary>
        /// Executes when row property changed
        /// </summary>
        private static void OnRowsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = (Chart)d;
            if (chart != null)
            {
                if (chart.Rows * chart.Columns >= chart.Areas.Count)
                {
                    chart.getchild(chart);
                }
            }
        }

        /// <summary>
        /// Executes when Column property changed
        /// </summary>
        private static void OnColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = (Chart)d;
            if (chart != null)
            {
                if (chart.Rows * chart.Columns >= chart.Areas.Count)
                {
                    chart.getchild(chart);
                }
            }
        }

        /// <summary>
        /// Executes when Orientation property changed
        /// </summary>
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = (Chart)d;
            if (chart != null)
            {
                if (chart.Rows * chart.Columns >= chart.Areas.Count)
                {
                    chart.getchild(chart);
                }
            }
        }
        #endregion

        #region Commands
        private ICommand printCommand;
        internal ICommand PrintCommand
        {
            get
            {
                return this.printCommand;
            }
        }
        #endregion
    }

    /// <summary>
    /// Class implementation for ChartICommandBase
    /// </summary>
    public abstract class ChartICommandBase : ICommand
    {
        private Chart chart;

        /// <summary>
        /// Called when instance created for ChartIcommandBase
        /// </summary>
        /// <param name="chart"></param>
        public ChartICommandBase(Chart chart)
        {
            this.chart = chart;
        }

        /// <summary>
        /// CLR property Declaration for Chart
        /// </summary>
        public Chart Chart
        {
            get { return this.chart; }
        }

        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public virtual bool CanExecute(object parameter)
        {
            var canExecuteHandler = this.CanExecuteChanged;
            if (canExecuteHandler != null)
            {
                canExecuteHandler(this, EventArgs.Empty);
            }

            return true;
        }

        /// <summary>
        /// Event for CanExecuteChanged
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public abstract void Execute(object parameter);

        #endregion
    }
    /// <summary>
    /// Class implementation for PrintCommand
    /// </summary>
    public class PrintCommand :ChartICommandBase
    {
        /// <summary>
        /// Called when instance created for PrintCommand
        /// </summary>
        /// <param name="chart"></param>
        public PrintCommand(Chart chart):base(chart)
        {

        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public override void Execute(object parameter)
        {
            Chart.Print();
        }
    }


    /// <summary>
    /// The shared resource dictionary is a specialized resource dictionary
    /// that loads it content only once. If a second instance with the same source
    /// is created, it only merges the resources from the cache.
    /// </summary>
    public class SharedResourceDictionary : ResourceDictionary,IDisposable
    {
        /// <summary>
        /// Internal cache of loaded dictionaries 
        /// </summary>
        public static Dictionary<string, ResourceDictionary> SharedDictionaries =
            new Dictionary<string, ResourceDictionary>();

        /// <summary>
        /// Local member of the source uri
        /// </summary>
        private Uri _sourceUri;

      
        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) to load resources from.
        /// </summary>
        public new Uri Source
        {
            get { return _sourceUri; }
            set
            {
                _sourceUri = value;
                if (_sourceUri != null)
                {
                    //base.Source = value;
                    if (SharedDictionaries == null)
                        SharedDictionaries = new Dictionary<string, ResourceDictionary>();
                    try
                    {
                        if (!SharedDictionaries.ContainsKey(value.ToString().ToLower()))
                        {

                            Application.LoadComponent(this, value);
                            // add it to the cache
                            SharedDictionaries.Add(value.ToString().ToLower(), this);
                        }
                        else
                        {

                            CopyInto(this, SharedDictionaries[value.ToString().ToLower()]);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errormesaage = "Syncfusion_SharedResourceDictionary " + value + " Error ";
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            throw new Exception(errormesaage, inner);
                        }
                    }
                }
            }
        }
        private static void CopyInto(ResourceDictionary copy, ResourceDictionary original)
        {
            foreach (var dictionary in original.MergedDictionaries)
            {
                var mergedCopy = new ResourceDictionary();
                CopyInto(mergedCopy, dictionary);
                copy.MergedDictionaries.Add(mergedCopy);
            }
            foreach (System.Collections.DictionaryEntry pair in original)
            {
                copy.Add(pair.Key, pair.Value);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
           // throw new NotImplementedException();
            SharedDictionaries = null;
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before the <see cref="T:System.Object"/> is reclaimed by garbage collection.
        /// </summary>
        ~SharedResourceDictionary()
        {
            SharedDictionaries = null;
            Source = null;
        }

        #endregion
    }
}
