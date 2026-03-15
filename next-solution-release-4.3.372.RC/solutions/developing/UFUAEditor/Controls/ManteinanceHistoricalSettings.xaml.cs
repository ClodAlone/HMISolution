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
using UFUAEditor.Document;
using DevExpress.Xpo;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for ManteinanceHistoricalSettings.xaml
    /// </summary>
    /// // https://support.progea.com/Products/default.asp?11454
    public partial class ManteinanceHistoricalSettings : UserControl //, IDisposable
    {
        #region Declarations
        readonly UFUAServerDocument Document;
        UFUAModel.UFUAHistorianSettings hs;
        #endregion

        public ManteinanceHistoricalSettings(UFUAServerDocument doc)
        {
            InitializeComponent();
            //Document = doc;
            //gridDataControl.SourceType = typeof(UFUAModel.UFUATag);
            //gridDataControlData.SourceType = typeof(UFUAHistorianModel.UFUAAuditDataItem);
            //Init();

            //DataContextChanged += (o, e) =>
            //    {
            //        Init();
            //    };

            //ChartArea.SetGridLineStroke(ChartArea2.PrimaryAxis, new Pen { Brush = Brushes.LightGray, DashStyle = DashStyles.Dash, Thickness = 2 });
            //ChartArea.SetGridLineStroke(ChartArea2.SecondaryAxis, new Pen { Brush = Brushes.LightGray, DashStyle = DashStyles.Dash, Thickness = 2 });

            //Chart1.Loaded += (o, e) =>
            //    {
            //        SetBindings();
            //    };
        }

        //void SetBindings()
        //{
        //    MultiBinding StartBinding = new MultiBinding() { Converter = new StarttimeConvertor(), Mode = BindingMode.TwoWay };
        //    Binding zoomFactorBinding = new Binding("ZoomFactor") { Source = ChartArea2.PrimaryAxis, Mode = BindingMode.TwoWay };
        //    Binding zoomPositionBinding = new Binding("ZoomPosition") { Source = ChartArea2.PrimaryAxis, Mode = BindingMode.TwoWay };
        //    Binding axisBinding = new Binding("PrimaryAxis") { Source = ChartArea2, Mode = BindingMode.TwoWay };
        //    StartBinding.Bindings.Add(zoomFactorBinding);
        //    StartBinding.Bindings.Add(zoomPositionBinding);
        //    StartBinding.Bindings.Add(axisBinding);
        //    StartBinding.Converter = new StarttimeConvertor();
        //    StartBinding.ConverterParameter = Chart1.Areas[1];
        //    ChartArea1.SetBinding(ChartArea.StartValueProperty, StartBinding);


        //    MultiBinding EndBinding = new MultiBinding() { Converter = new EndtimeConvertor(), Mode = BindingMode.TwoWay };
        //    Binding zoomFactorBinding1 = new Binding("ZoomFactor") { Source = ChartArea2.PrimaryAxis, Mode = BindingMode.TwoWay };
        //    Binding zoomPositionBinding1 = new Binding("ZoomPosition") { Source = ChartArea2.PrimaryAxis, Mode = BindingMode.TwoWay };
        //    Binding axisBinding1 = new Binding("PrimaryAxis") { Source = ChartArea2, Mode = BindingMode.TwoWay };
        //    EndBinding.Bindings.Add(zoomFactorBinding1);
        //    EndBinding.Bindings.Add(zoomPositionBinding1);
        //    EndBinding.Bindings.Add(axisBinding1);
        //    EndBinding.Converter = new EndtimeConvertor();
        //    EndBinding.ConverterParameter = Chart1.Areas[1];
        //    ChartArea1.SetBinding(ChartArea.EndValueProperty, EndBinding);
        //}

        //void Init()
        //{
        //    hs = DataContext as UFUAModel.UFUAHistorianSettings;
        //    if (hs == null)
        //        return;

        //    var list = Document.GetHistoricalSettingsTags(hs.Name);
        //    if (list.Count > 0)
        //        gridDataControl.ItemsSource = list;
        //    else
        //    {
        //        stackPanel.Visibility = Visibility.Visible;
        //        gridDataControl.Visibility = Visibility.Collapsed;
        //    }
        //}

        ///// <summary>
        ///// Handles the 1 event of the InteractiveCursor_MouseMove control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        //private void InteractiveCursor_MouseMove_1(object sender, MouseEventArgs e)
        //{
        //    if (ChartArea2.SecondaryAxis.InteractiveCursorLabelContent != null)
        //        ChartArea2.InteractiveCursors[0].ToolTip = ChartArea2.SecondaryAxis.InteractiveCursorLabelContent.Y.ToString();
        //}

        //public void Dispose()
        //{
        //    // gridDataControl.Model.Dispose();
        //    try
        //    {
        //        gridDataControl.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }
        //}
    }

    //public class StarttimeConvertor : IMultiValueConverter
    //{

    //    #region IMultiValueConverter Members

    //    public static ChartAxis axis;
    //    ChartArea area;

    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        area = parameter as ChartArea;
    //        double zoomingFactor = (double)values[0];
    //        double zoomingPosition = (double)values[1];
    //        axis = (ChartAxis)values[2];

    //        return axis.VisibleRange.Start;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        double StartValue = (double)value;
    //        double endValue = area.EndValue;
    //        double TotalValueStart = axis.Range.Start;
    //        double TotalValueEnd = axis.Range.End;

    //        double ZoomingFactor = (endValue - StartValue) / (TotalValueEnd - TotalValueStart);
    //        double ZoomingPosition = ((StartValue - axis.Range.Start)) / axis.Range.Delta; //(StartValue + ((endValue - StartValue) / 2)) / axis.Range.Delta;

    //        object[] ret = { ZoomingFactor, ZoomingPosition, axis };
    //        return ret;
    //    }

    //    #endregion
    //}

    //public class EndtimeConvertor : IMultiValueConverter
    //{

    //    #region IMultiValueConverter Members

    //    public ChartAxis axis;
    //    ChartArea area;

    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        area = parameter as ChartArea;
    //        double zoomingFactor = (double)values[0];
    //        double zoomingPosition = (double)values[1];
    //        axis = (ChartAxis)values[2];

    //        return axis.VisibleRange.End;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        double endValue = (double)value;
    //        double StartValue = area.StartValue;
    //        double TotalValueStart = axis.Range.Start;
    //        double TotalValueEnd = axis.Range.End;

    //        double ZoomingFactor = (endValue - StartValue) / (TotalValueEnd - TotalValueStart);
    //        double ZoomingPosition = (StartValue - axis.Range.Start) / axis.Range.Delta;

    //        object[] ret = { ZoomingFactor, ZoomingPosition, axis };
    //        return ret;
    //    }

    //    #endregion
    //}

}
