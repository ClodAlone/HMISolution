#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Text;
using Syncfusion.Data;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.ComponentModel;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class GridSummaryColumn :DependencyObject, ISummaryColumn
    {
        #region Dependency Registration

        public static readonly DependencyProperty CustomAggregateProperty = DependencyProperty.Register("CustomAggregate", typeof(ISummaryAggregate), typeof(GridSummaryColumn), new PropertyMetadata(null));

        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(string), typeof(GridSummaryColumn), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MappingNameProperty = DependencyProperty.Register("MappingName", typeof(string), typeof(GridSummaryColumn), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(GridSummaryColumn), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SummaryTypeProperty = DependencyProperty.Register("SummaryType", typeof(SummaryType), typeof(GridSummaryColumn), new PropertyMetadata(SummaryType.CountAggregate));

        #endregion

        #region ISummaryColumn Members

        /// <summary>
        /// Gets or sets the Custom Aggregate for Custom summaries
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ISummaryAggregate CustomAggregate
        {
            get
            {
                return (ISummaryAggregate)this.GetValue(CustomAggregateProperty) ;
            }
            set
            {
                this.SetValue(CustomAggregateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Format should show in summary
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WinRT
        [TypeConverter(typeof(GridSummaryFormatConverter))]
#endif
        public string Format
        {
            get
            {
                return (string)this.GetValue(FormatProperty);
            }
            set
            {
#if !WPF
                var formattedValue = value.SummaryFormatedString();
                this.SetValue(FormatProperty, formattedValue);
#else
                this.SetValue(FormatProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the Column Maping name
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string MappingName
        {
            get
            {
                return (string)this.GetValue(MappingNameProperty);
            }
            set
            {
                this.SetValue(MappingNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Name of Summary Column
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Name
        {
            get
            {
                return (string)this.GetValue(NameProperty);
            }
            set
            {
                this.SetValue(NameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Summary Type
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public SummaryType SummaryType
        {
            get
            {
                return (SummaryType)this.GetValue(SummaryTypeProperty);
            }
            set
            {
                this.SetValue(SummaryTypeProperty, value);
            }
        }

        #endregion
    }

    internal static class GridSummaryFormatterExtenstion
    {
        public static string SummaryFormatedString(this object value)
        {
            var formatString = value.ToString();
            if (formatString.Length > 1)
            {
                var sb = new StringBuilder();
                var startIdx = formatString.IndexOf("\'") + 1;
                var endIdx = formatString.LastIndexOf("\'");
                if (startIdx > 0)
                {
                    sb.Append(formatString.Substring(0, startIdx - 1));
                }
                for (int i = startIdx; i < endIdx; i++)
                {
                    char c = formatString[i];
                    sb.Append(c);
                }
                if (sb.Length > 0)
                {
                    return sb.ToString();
                }
            }
            return formatString;
        }
    }

#if !WinRT
    class GridSummaryFormatConverter : TypeConverter
    {
        public GridSummaryFormatConverter()
        {
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return value.SummaryFormatedString();
        }
    }
#endif
}
