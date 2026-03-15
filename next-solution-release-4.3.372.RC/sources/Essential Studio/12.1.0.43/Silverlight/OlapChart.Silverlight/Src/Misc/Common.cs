#region Copyright Syncfusion Inc. 2001 - 2014
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.Windows.Chart;
using System.Collections.ObjectModel;
using Syncfusion.OlapSilverlight.Reports;
using System.Collections.Generic;

namespace Syncfusion.Silverlight.Chart.Olap
{
    #region Olap Supported Chart Types

    public enum OlapChartTypes
    {
        /// <summary>
        /// Default.
        /// Compares values across categories. The series values are displayed 
        /// as individual columns, grouped by category. The height of each column 
        /// is determined by the series value.
        /// </summary>
        Column,

        /// <summary>
        /// A bar chart, also known as a bar graph, is a chart with rectangular bars 
        /// of lengths proportional to that value that they represent. 
        /// Bar charts are used for comparing two or more values.
        /// </summary>
        Bar,

        /// <summary>
        /// A type of presentation graphic that emphasizes 
        /// a change in values by filling in the portion of the graph beneath 
        /// the line connecting various data points.
        /// </summary>
        Area,

        /// <summary>
        /// A style of chart that is created by connecting a series of data points 
        /// together with a line. This is the most basic type of chart used in finance and 
        /// it is generally created by connecting a series of past prices together with a line.
        /// </summary>
        Line,

        /// <summary>
        /// A spline chart is simply a line chart that plots a fitted curve through 
        /// each data point in a series.
        /// </summary>
        Spline,

        /// <summary>
        /// A scatter type contains one or more scatter plots each of which use Cartesian 
        /// co-ordinates to display values for two series values for a set of data. 
        /// The data is displayed as a collection of points, each having the value 
        /// of one variable determining the position on the horizontal axis and 
        /// the value of the other variable determining the position on the vertical axis.
        /// </summary>
        Scatter,

        /// <summary>
        /// A pie chart (or a circle graph) is a circular chart divided into sectors, 
        /// illustrating relative magnitudes or frequencies or percent. 
        /// In a pie chart, the arc length of each sector (and consequently its central angle and area), 
        /// is proportional to the quantity it represents. Together, the sectors create a full disk.
        /// It is named for its resemblance to a pie which has been sliced.
        /// </summary>
        Pie,

        /// <summary>
        /// A stacked column chart displays the relationship of individual items to the whole, 
        /// comparing the contributions of each value to a total across categories.
        /// The series values are stacked in a single column for each category. 
        /// The height of each column is determined by the total of all series values for the category.
        /// </summary>
        StackingColumn,

        /// <summary>
        /// A column chart where multiple series are stacked vertically to fit 100% 
        /// of the chart area. If there is only one series in your chart, 
        /// all the column bars will fit to 100% of the chart area.
        /// </summary>
        StackingColumn100,

        /// <summary>
        /// A stacked bar chart displays the relationship of individual items to the whole, 
        /// comparing the contributions of each value to a total across categories.
        /// The series values are stacked in a single column for each category. 
        /// The height of each column is determined by the total of all series values for the category.
        /// </summary>
        StackingBar,

        /// <summary>
        /// A bar chart where multiple series are stacked horizontally to fit 100% 
        /// of the chart area. If there is only one series in chart, 
        /// all the bars will fit to 100% of area.
        /// </summary>
        StackingBar100,

        /// <summary>
        /// StackingArea is an area chart with Y values stacked over one another, in series order.
        /// </summary>
        StackingArea,

        /// <summary>
        /// The StepArea chart consists of pairs of values (plotted as points) on a line 
        /// for each series in the given dataset. Each pair of values consists of one date (value) and one numeric value. 
        /// This chart will have one date\time (value) based axis (the domain axis) and one numeric axis (the range axis).  
        /// Each line is drawn at a 90 degree angle from point to point. 
        /// Each line will fill from the line to the bottom of the plot with its series color.
        /// </summary>
        StepArea,

        /// <summary>
        /// Spline Chart is an Area chart in which each area is given a color to 
        /// emphasize the relationships between the pieces of charted information.
        /// </summary>
        SplineArea,

        /// <summary>
        /// Step Line charts are line charts, with values drawn continuously, step by step without any gaps between them.
        /// </summary>
        StepLine,

        /// <summary>
        /// Rotated Spline chart defines chart series points as interconnected rotated smooth curves.
        /// </summary>
        RotatedSpline,

        /// <summary>
        /// A radar chart is two-dimensional chart of three or more quantitative 
        /// variables represented on axes starting from the same point. The relative position 
        /// and angle of the axes is uninformative.  Radar charts are usually used to compare 
        /// performance of different entities on a same set of axes.
        /// </summary>
        Radar,

        /// <summary>
        /// Polar chart is used to display chart data points, connected with a line, 
        /// in a polar co-ordinate system.
        /// </summary>
        Polar,

        /// <summary>
        /// The Funnel Chart type displays data that equals 100% when totaled. This type
        /// of chart is a single series chart representing the data as portions of 100%, 
        /// and this chart does not use any axes.
        /// </summary>
        Funnel,

        /// <summary>
        /// Pyramid charts are another type of accumulation chart which has a triangular 
        /// upper surface that converges at one point. Similar to a 
        /// Syncfusion.Windows.Chart.ChartTypes.Funnel chart, the height of a segment is 
        /// proportional to the Y value of the corresponding point.
        /// </summary>
        Pyramid
    } 

    #endregion

    #region Series Tooltip Converter

    public class ToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PivotCellDescriptor cellDescriptor = value as PivotCellDescriptor;

            if (cellDescriptor != null && cellDescriptor.CellData != null)
            {
                switch (parameter.ToString())
                {
                    case "Measure":
                        {
                            return cellDescriptor.CellData.Measure;
                        }
                    case "Columns":
                        {
                            return cellDescriptor.CellData.Columns;
                        }
                    case "Rows":
                        {
                            return cellDescriptor.CellData.Rows;
                        }
                    case "Value":
                        {
                            return cellDescriptor.CellData.Value;
                        }
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 
    
    #endregion

    #region Label Rotation value converter

    public class RotateAngleConverter
: IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    } 

    #endregion

    public class FallbackValueToVisibilityConverter
        : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
                try
                {
                    var cellValue = (value as string);
                    if (cellValue != null && !string.IsNullOrEmpty(cellValue) && !string.IsNullOrWhiteSpace(cellValue))
                        return Visibility.Visible;
                    return Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    return Visibility.Collapsed;
                }            
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ToolTipDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime date;
            OlapLabelPresenter formatlabel = value as OlapLabelPresenter;
            if (formatlabel != null)
            {
                if (formatlabel.CellDescriptor != null)
                {
                    if (formatlabel.PrimaryAxisLabelDateTimeFormat == string.Empty)
                    {

                        return formatlabel.CellDescriptor.CellValue;
                    }
                    else
                    {
                        string actualformat = formatlabel.PrimaryAxisLabelDateTimeFormat;
                        string actualvalue = formatlabel.CellDescriptor.CellValue;
                        bool canDateConvrt = DateTime.TryParse(actualvalue, out date);

                        if (canDateConvrt)
                        {
                            actualvalue = date.ToString(actualformat);
                        }

                        return actualvalue;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #region Design Time Data

    public class ProductSales
    {
        public string Product { get; set; }

        public string Date { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public int Quantity { get; set; }

        public int ProductionQuantity { get; set; }

        public double Amount { get; set; }

        public double SalesAmount { get; set; }

        public static ProductSalesCollection GetSalesData()
        {
            /// Geography
            string[] countries = new string[] { "Australia", "Canada", "France", "Germany", "United Kingdom", "United States" };
            string[] ausStates = new string[] { "New South Wales", "Queensland", "South Australia", "Tasmania", "Victoria" };
            string[] canadaStates = new string[] { "Alberta", "British Columbia", "Brunswick", "Manitoba", "Ontario", "Quebec" };
            string[] franceStates = new string[] { "Charente-Maritime", "Essonne", "Garonne (Haute)", "Gers", };
            string[] germanyStates = new string[] { "Bayern", "Brandenburg", "Hamburg", "Hessen", "Nordrhein-Westfalen", "Saarland" };
            string[] ukStates = new string[] { "England" };
            string[] ussStates = new string[] { "New York", "North Carolina", "Alabama", "California", "Colorado", "New Mexico", "South Carolina" };

            /// Time
            string[] dates = new string[] { "FY 2005" };

            /// Products
            string[] products = new string[] { "Bike", "Car" };
            Random r = new Random(123345345);

            int numberOfRecords = 20;
            ProductSalesCollection listOfProductSales = new ProductSalesCollection();
            for (int i = 0; i < numberOfRecords; i++)
            {
                ProductSales sales = new ProductSales();
                sales.Country = countries[r.Next(1, countries.GetLength(0))];
                sales.Quantity = r.Next(1, 12);
                /// 1 percent discount for 1 quantity
                double discount = (30000 * sales.Quantity) * (double.Parse(sales.Quantity.ToString()) / 100);
                sales.Amount = (30000 * sales.Quantity) - discount;
                sales.Date = dates[r.Next(r.Next(dates.GetLength(0) + 1))];
                sales.Product = products[r.Next(r.Next(products.GetLength(0) + 1))];
                sales.ProductionQuantity = sales.Quantity + r.Next(1, 3);
                switch (sales.Country)
                {
                    case "Australia":
                        {
                            sales.State = ausStates[r.Next(ausStates.GetLength(0))];
                            break;
                        }
                    case "Canada":
                        {
                            sales.State = canadaStates[r.Next(canadaStates.GetLength(0))];
                            break;
                        }
                    case "France":
                        {
                            sales.State = franceStates[r.Next(franceStates.GetLength(0))];
                            break;
                        }
                    case "Germany":
                        {
                            sales.State = germanyStates[r.Next(germanyStates.GetLength(0))];
                            break;
                        }
                    case "United Kingdom":
                        {
                            sales.State = ukStates[r.Next(ukStates.GetLength(0))];
                            break;
                        }
                    case "United States":
                        {
                            sales.State = ussStates[r.Next(ussStates.GetLength(0))];
                            break;
                        }
                }
                listOfProductSales.Add(sales);
            }

            return listOfProductSales;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", this.Country, this.State, this.Product);
        }

        #region Design Time Report

        public static OlapReport CreateOlapReport()
        {
            OlapReport olapReport = new OlapReport();

            // Specifying the Row Dimension Element
            DimensionElement dimensionElementRow = new DimensionElement();
            dimensionElementRow.Name = "Geography";
            dimensionElementRow.Hierarchy = new HierarchyElement() { Name = "Product Hierarchy" };

            dimensionElementRow.Hierarchy.LevelElements.Add(new LevelElement() { Name = "Product" });
            dimensionElementRow.Hierarchy.LevelElements.Add(new LevelElement() { Name = "Date" });

            // Specifying the Column Dimension Element
            DimensionElement dimensionElementColumn = new DimensionElement();
            dimensionElementColumn.Name = "Geography";
            dimensionElementColumn.Hierarchy = new HierarchyElement() { Name = "Geography Hierarchy" };

            dimensionElementColumn.Hierarchy.LevelElements.Add(new LevelElement() { Name = "Country" });
            dimensionElementColumn.Hierarchy.LevelElements.Add(new LevelElement() { Name = "State" });

            // Specifying the Summary Elements
            SummaryElements summaries = new SummaryElements();
            summaries.Add(new SummaryInfo { Column = "Quantity", Key = "Quantity", Type = SummaryType.Sum });
            //summaries.Add(new SummaryInfo { Column = "Amount", Key = "Amount", Type = SummaryType.Sum, FormatString = "{0:c}" });
            summaries.Add(new SummaryInfo { Column = "ProductionQuantity", Key = "ProductionQuantity", Type = SummaryType.Sum });

            // Adding the Row Elements
            olapReport.SeriesElements.Add(new Item { ElementValue = summaries });
            olapReport.SeriesElements.Add(new Item { ElementValue = dimensionElementRow });
            // Adding the Column Elements
            olapReport.CategoricalElements.Add(new Item { ElementValue = dimensionElementColumn });
            olapReport.ShowExpanders = false;
            return olapReport;
        }

        #endregion
    }

    public class ProductSalesCollection : ObservableCollection<ProductSales>
    {
    }    

    #endregion

       /// <summary>
    /// Specifies the Visual Style for OlapChart
    /// </summary>
    public enum OlapChartVisualStyle
    {
        /// <summary>
        /// Provide Blend Style for OlapGrid
        /// </summary>
        Blend,
        /// <summary>
        /// Provide Metro Style for OlapGrid
        /// </summary>
        Metro,
        /// <summary>
        /// Provide Default Style for OlapGrid
        /// </summary>
        Default,
        /// <summary>
        /// Provide Office 2007 Blue Style for OlapGrid
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Provide Office 2007 Black Style for OlapGrid
        /// </summary>
        Office2007Black,
        /// <summary>
        /// Provide Office 2007 Silver Style for OlapGrid
        /// </summary>
        Office2007Silver,
        /// <summary>
        /// Provide Office 2010 Black Style for OlapGrid
        /// </summary>
        Office2010Black,
        /// <summary>
        /// Provide office 2010 Blue Style for OlapGrid
        /// </summary>
        Office2010Blue,
        /// <summary>
        /// Provides office 2010 Silver Style for OlapGrid
        /// </summary>
        Office2010Silver,
        /// <summary>
        /// Provides transparent style for olapgrid
        /// </summary>
        Transparent,
    }
}
