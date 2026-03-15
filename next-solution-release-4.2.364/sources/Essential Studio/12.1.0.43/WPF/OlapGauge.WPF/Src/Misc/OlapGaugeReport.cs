#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;

namespace Syncfusion.Windows.Gauge.Olap
{
    #region Dimesion Class

    public class Dimension : DependencyObject, IAxisElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the hierarchy.
        /// </summary>
        /// <value>The name of the hierarchy.</value>
        public string HierarchyName
        {
            get { return (string)GetValue(HierarchyNameProperty); }
            set { SetValue(HierarchyNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the level.
        /// </summary>
        /// <value>The name of the level.</value>
        public string LevelName
        {
            get { return (string)GetValue(LevelNameProperty); }
            set { SetValue(LevelNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the include members.
        /// </summary>
        /// <value>The include members.</value>
        [TypeConverter(typeof(StringCollectionToList))]
        public List<string> IncludeMembers
        {
            get { return (List<string>)GetValue(IncludeMembersProperty); }
            set { SetValue(IncludeMembersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the dimension.
        /// </summary>
        /// <value>The type of the dimension.</value>
        public DimesnionType DimensionType
        {
            get { return (DimesnionType)GetValue(DimensionTypeProperty); }
            set { SetValue(DimensionTypeProperty, value); }
        }


        /// <summary>
        /// Gets or sets the member properties.
        /// </summary>
        /// <value>The member properties.</value>
        public List<MemberProperty> MemberProperties
        {
            get { return (List<MemberProperty>)GetValue(MemberPropertiesProperty); }
            set { SetValue(MemberPropertiesProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty HierarchyNameProperty =
            DependencyProperty.Register("HierarchyName", typeof(string), typeof(Dimension), new UIPropertyMetadata(null));



        public static readonly DependencyProperty LevelNameProperty =
            DependencyProperty.Register("LevelName", typeof(string), typeof(Dimension), new UIPropertyMetadata(null));



        public static readonly DependencyProperty IncludeMembersProperty =
            DependencyProperty.Register("IncludeMembers", typeof(List<string>), typeof(Dimension), new UIPropertyMetadata(null));



        public static readonly DependencyProperty DimensionTypeProperty =
            DependencyProperty.Register("DimensionType", typeof(DimesnionType), typeof(Dimension), new UIPropertyMetadata(DimesnionType.Include));


        public static readonly DependencyProperty MemberPropertiesProperty =
            DependencyProperty.Register("MemberProperties", typeof(List<MemberProperty>), typeof(Dimension), new UIPropertyMetadata(new List<MemberProperty>()));

        #endregion

        #region IAxisElement Members

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }

    #endregion

    #region Measure Class

    public class Measure : DependencyObject, IAxisElement
    {
        #region IAxisElement Members

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }

    #endregion

    #region KPI Class

    public class Kpi : DependencyObject, IAxisElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [show status].
        /// </summary>
        /// <value><c>true</c> if [show status]; otherwise, <c>false</c>.</value>
        public bool ShowStatus
        {
            get { return (bool)GetValue(ShowStatusProperty); }
            set { SetValue(ShowStatusProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show trend].
        /// </summary>
        /// <value><c>true</c> if [show trend]; otherwise, <c>false</c>.</value>
        public bool ShowTrend
        {
            get { return (bool)GetValue(ShowTrendProperty); }
            set { SetValue(ShowTrendProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show goal].
        /// </summary>
        /// <value><c>true</c> if [show goal]; otherwise, <c>false</c>.</value>
        public bool ShowGoal
        {
            get { return (bool)GetValue(ShowGoalProperty); }
            set { SetValue(ShowGoalProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show value].
        /// </summary>
        /// <value><c>true</c> if [show value]; otherwise, <c>false</c>.</value>
        public bool ShowValue
        {
            get { return (bool)GetValue(ShowValueProperty); }
            set { SetValue(ShowValueProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ShowStatusProperty =
          DependencyProperty.Register("ShowStatus", typeof(bool), typeof(Kpi), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ShowTrendProperty =
           DependencyProperty.Register("ShowTrend", typeof(bool), typeof(Kpi), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ShowGoalProperty =
            DependencyProperty.Register("ShowGoal", typeof(bool), typeof(Kpi), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ShowValueProperty =
            DependencyProperty.Register("ShowValue", typeof(bool), typeof(Kpi), new UIPropertyMetadata(null));

        #endregion

        #region IAxisElement Members

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }
    #endregion

    #region Member Properties Class
    public class MemberProperty : DependencyObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        public string UniqueName
        {
            get { return (string)GetValue(UniqueNameProperty); }
            set { SetValue(UniqueNameProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty NameProperty =
           DependencyProperty.Register("Name", typeof(string), typeof(MemberProperty), new UIPropertyMetadata(null));

        public static readonly DependencyProperty UniqueNameProperty =
            DependencyProperty.Register("UniqueName", typeof(string), typeof(MemberProperty), new UIPropertyMetadata(null));

        #endregion
    }
    #endregion

    #region SortElement Class
    public class Sort : DependencyObject, IAxisElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the measure unique.
        /// </summary>
        /// <value>The name of the measure unique.</value>
        public string MeasureUniqueName
        {
            get { return (string)GetValue(MeasureUniqueNameProperty); }
            set { SetValue(MeasureUniqueNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public OrderOfSort SortOrder
        {
            get { return (OrderOfSort)GetValue(SortOrderProperty); }
            set { SetValue(SortOrderProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty MeasureUniqueNameProperty =
            DependencyProperty.Register("MeasureUniqueName", typeof(string), typeof(Sort), new UIPropertyMetadata(null));

        public static readonly DependencyProperty SortOrderProperty =
            DependencyProperty.Register("SortOrder", typeof(OrderOfSort), typeof(Sort), new UIPropertyMetadata(null));

        #endregion

        #region IAxisElement Members
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion
    }

    #endregion

    #region NamedSet Class
    public class NamedSet : DependencyObject, IAxisElement
    {
        #region IAxisElement Members
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the dimension.
        /// </summary>
        /// <value>The name of the dimension.</value>
        public string DimensionName
        {
            get { return (string)GetValue(DimensionNameProperty); }
            set { SetValue(DimensionNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the dimension unique.
        /// </summary>
        /// <value>The name of the dimension unique.</value>
        public string DimensionUniqueName
        {
            get { return (string)GetValue(DimensionUniqueNameProperty); }
            set { SetValue(DimensionUniqueNameProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty DimensionNameProperty =
           DependencyProperty.Register("DimensionName", typeof(string), typeof(NamedSet), new UIPropertyMetadata(null));
        public static readonly DependencyProperty DimensionUniqueNameProperty =
            DependencyProperty.Register("DimensionUniqueName", typeof(string), typeof(NamedSet), new UIPropertyMetadata(null));

        #endregion

    }
    #endregion

    #region SubSet Class
    public class SubSet : DependencyObject, IAxisElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end index.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex
        {
            get { return (int)GetValue(EndIndexProperty); }
            set { SetValue(EndIndexProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty StartIndexProperty =
           DependencyProperty.Register("StartIndex", typeof(int), typeof(SubSet), new UIPropertyMetadata(-1));

        public static readonly DependencyProperty EndIndexProperty =
            DependencyProperty.Register("EndIndex", typeof(int), typeof(SubSet), new UIPropertyMetadata(-1));

        #endregion

        #region IAxisElement Members
        public string Name { get; set; }
        #endregion
    }
    #endregion

    #region TopCount
    public class TopCount : DependencyObject, IAxisElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the field count.
        /// </summary>
        /// <value>The field count.</value>
        public int FieldCount
        {
            get { return (int)GetValue(FieldCountProperty); }
            set { SetValue(FieldCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the measure.
        /// </summary>
        /// <value>The name of the measure.</value>
        public string MeasureName
        {
            get { return (string)GetValue(MeasureNameProperty); }
            set { SetValue(MeasureNameProperty, value); }
        }
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty MeasureNameProperty =
            DependencyProperty.Register("MeasureName", typeof(string), typeof(TopCount), new UIPropertyMetadata(null));
        public static readonly DependencyProperty FieldCountProperty =
            DependencyProperty.Register("FieldCount", typeof(int), typeof(TopCount), new UIPropertyMetadata(0));
        #endregion

        #region IAxisElement Members
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion
    }
    #endregion

    #region IAxisElement Interface
    public interface IAxisElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }
    }
    #endregion

    #region Axes Classes

    /// <summary>
    /// Holds the list of memebers in Categorical(Column) axis
    /// </summary>
    public class CategoricalAxis : List<IAxisElement>
    {

    }

    /// <summary>
    /// Holds list of members in Series(Row) axis.
    /// </summary>
    public class SeriesAxis : List<IAxisElement>
    {

    }

    /// <summary>
    /// Holds list of members in Slicer(Filter) axis.
    /// </summary>
    public class SlicerAxis : List<IAxisElement>
    {

    }

    #region Calculated Members Class
    /// <summary>
    /// Holds list of calculated members/measures.
    /// </summary>
    public class CalculatedMembers : List<CalculatedMember>
    {
    }

    public class CalculatedMember : DependencyObject
    {

        #region Properties
        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        public string UniqueName
        {
            get { return (string)GetValue(UniqueNameProperty); }
            set { SetValue(UniqueNameProperty, value); }
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public string Expression
        {
            get { return (string)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>The name of the element.</value>
        public string ElementName
        {
            get { return (string)GetValue(ElementNameProperty); }
            set { SetValue(ElementNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the element hierarchy.
        /// </summary>
        /// <value>The name of the element hierarchy.</value>
        public string ElementHierarchyName
        {
            get { return (string)GetValue(ElementHierarchyNameProperty); }
            set { SetValue(ElementHierarchyNameProperty, value); }
        }


        /// <summary>
        /// Gets or sets the name of the element level.
        /// </summary>
        /// <value>The name of the element level.</value>
        public string ElementLevelName
        {
            get { return (string)GetValue(ElementLevelNameProperty); }
            set { SetValue(ElementLevelNameProperty, value); }
        }

        public static readonly DependencyProperty ElementLevelNameProperty =
            DependencyProperty.Register("ElementLevelName", typeof(string), typeof(CalculatedMember), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the type of the member.
        /// </summary>
        /// <value>The type of the member.</value>
        public MemberyType MemberType
        {
            get { return (MemberyType)GetValue(MemberTypeProperty); }
            set { SetValue(MemberTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the axis position.
        /// </summary>
        /// <value>The axis position.</value>
        public PositionOfAxis AxisPosition
        {
            get { return (PositionOfAxis)GetValue(AxisPositionProperty); }
            set { SetValue(AxisPositionProperty, value); }
        }
        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty UniqueNameProperty =
            DependencyProperty.Register("UniqueName", typeof(string), typeof(CalculatedMember), new UIPropertyMetadata(null));

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(CalculatedMember), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ExpressionProperty =
           DependencyProperty.Register("Expression", typeof(string), typeof(CalculatedMember), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ElementNameProperty =
          DependencyProperty.Register("ElementName", typeof(string), typeof(CalculatedMember), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ElementHierarchyNameProperty =
            DependencyProperty.Register("ElementHierarchyName", typeof(string), typeof(CalculatedMember), new UIPropertyMetadata(null));

        public static readonly DependencyProperty MemberTypeProperty =
            DependencyProperty.Register("MemberType", typeof(MemberyType), typeof(CalculatedMember), new UIPropertyMetadata(MemberyType.Measure));

        public static readonly DependencyProperty AxisPositionProperty =
            DependencyProperty.Register("AxisPosition", typeof(PositionOfAxis), typeof(CalculatedMember), new UIPropertyMetadata(PositionOfAxis.Categorical));

        #endregion
    }

    #endregion

    #endregion

    #region StringCollectionToList Converter Class
    /// <summary>
    /// Converter class which is used to convert the string collection into List of string.
    /// </summary>
    public class StringCollectionToList : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string stringData = value as string;
            if (stringData != null || !stringData.Equals(string.Empty))
            {
                List<string> tokenizedList = null;
                if (value != null && !value.ToString().Trim().Equals(string.Empty))
                {
                    tokenizedList = new List<string>(value.ToString().Trim().Split(new char[] { ',' }));
                }
                return tokenizedList;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    #endregion

    #region DimesionType Enum
    /// <summary>
    /// Type of Dimension.
    /// </summary>
    public enum DimesnionType
    {
        /// <summary>
        /// Specifies the Included Dimension.
        /// </summary>
        Include = 0,
        /// <summary>
        /// Specifies the Excluded Dimension.
        /// </summary>
        Exclude = 1
    }
    #endregion

    #region SortOrder Enum
    /// <summary>
    /// Sort Orders.
    /// </summary>
    public enum OrderOfSort
    {
        /// <summary>
        /// Ascending order.
        /// </summary>
        Asc,
        /// <summary>
        /// Descending order
        /// </summary>
        Desc,
        /// <summary>
        /// Break hierarchy with ascending(on sorting).
        /// </summary>
        BAsc,
        /// <summary>
        /// Break hierachy with descending(on sorting).
        /// </summary>
        BDesc
    }
    #endregion

    #region AxisType Enum
    /// <summary>
    /// Axis Position.
    /// </summary>
    public enum PositionOfAxis
    {
        /// <summary>
        /// Categorical(column) axis.
        /// </summary>
        Categorical,
        /// <summary>
        /// Series(row) axis.
        /// </summary>
        Series,
        /// <summary>
        /// Slicer(filter) axis.
        /// </summary>
        Slicer
    }
    #endregion

    #region CalculaterMemmberType Enum
    /// <summary>
    /// Type of Calculated Member.
    /// </summary>
    public enum MemberyType
    {
        /// <summary>
        /// Specifies Calculated Measure.
        /// </summary>
        Measure,
        /// <summary>
        /// Specifies Calculated Dimension.
        /// </summary>
        Dimension
    }
    #endregion
}
