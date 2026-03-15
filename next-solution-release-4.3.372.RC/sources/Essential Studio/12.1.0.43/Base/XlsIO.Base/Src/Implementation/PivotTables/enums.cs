#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Filter operator
    /// </summary>
    public enum FilterOperator2007
    {
        /// <summary>
        /// Equal operator of filter
        /// </summary>
        Equal,

        /// <summary>
        /// Greater than operator of filter
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Greater of than equal of filter
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Less than filter of filter
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equal filter 
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Not equal filter
        /// </summary>
        NotEqual,
    }
    /// <summary>
    /// Filter operator
    /// </summary>
    public enum FilterOperator
    {
        /// <summary>
        /// Equal operator of filter
        /// </summary>
        equal = FilterOperator2007.Equal,

        /// <summary>
        /// Greater than operator of filter
        /// </summary>
        greaterThan = FilterOperator2007 .GreaterThan,

        /// <summary>
        /// Greater of than equal of filter
        /// </summary>
        greaterThanOrEqual = FilterOperator2007 .GreaterThanOrEqual,

        /// <summary>
        /// Less than filter of filter
        /// </summary>
        lessThan = FilterOperator2007 .LessThan,

        /// <summary>
        /// Less than or equal filter 
        /// </summary>
        lessThanOrEqual = FilterOperator2007 .LessThanOrEqual,

        /// <summary>
        /// Not equal filter
        /// </summary>
        notEqual = FilterOperator2007 .NotEqual,
    }
    ///<summary>
    ///Pivot filter type
    ///</summary>
    public enum PivotFilterType
    {
        /// <summary>
        /// Indicates the "begins with" filter for field captions.
        /// </summary>
        CaptionBeginsWith,

        /// <summary>
        /// Indicates the "is between" filter for field captions.
        /// </summary>
        CaptionBetween,

        /// <summary>
        /// Indicates the "contains" filter for field captions.
        /// </summary>
        CaptionContains,

        /// <summary>
        /// Indicates the "ends with" filter for field captions.
        /// </summary>
        CaptionEndsWith,

        /// <summary>
        /// Indicates the "equal" filter for field captions.
        /// </summary>
        CaptionEqual,

        /// <summary>
        /// Indicates the "is greater than" filter for field captions
        /// </summary>
        CaptionGreaterThan,

        /// <summary>
        /// Indicates the "is greater than or equal to" filter for field captions.
        /// </summary>
        CaptionGreaterThanOrEqual,

        /// <summary>
        /// Indicates the "is less than" filter for field captions.
        /// </summary>
        CaptionLessThan,

        /// <summary>
        /// Indicates the "is less than or equal to" filter for field captions.
        /// </summary>
        CaptionLessThanOrEqual,

        /// <summary>
        /// Indicates the "does not begin with" filter for field captions.
        /// </summary>
        CaptionNotBeginsWith,

        /// <summary>
        /// Indicates the "is not between" filter for field captions.
        /// </summary>
        CaptionNotBetween,

        /// <summary>
        /// Indicates the "does not contain" filter for field captions.
        /// </summary>
        CaptionNotContains,

        /// <summary>
        /// Indicates the "does not end with" filter for field captions.
        /// </summary>
        CaptionNotEndsWith,

        /// <summary>
        /// Indicates the "not equal" filter for field captions.
        /// </summary>
        CaptionNotEqual,

        /// <summary>
        /// Indicates the "Value between" filter for text and numeric values.
        /// </summary>
        ValueBetween,

        /// <summary>
        /// Indicates the "value equal" filter for text and numeric values.
        /// </summary>
        ValueEqual,

        /// <summary>
        /// Indicates the "value greater than" filter for text and numeric values.
        /// </summary>
        ValueGreaterThan,

        /// <summary>
        /// Indicates the "value greater than or equal to" filter for text and numeric values.
        /// </summary>
        ValueGreaterThanOrEqual,

        /// <summary>
        /// Indicates the "value less than" filter for text and numeric values.
        /// </summary>
        ValueLessThan,

        /// <summary>
        /// Indicates the "value less than or equal to" filter for text and numeric values
        /// </summary>
        ValueLessThanOrEqual,

        /// <summary>
        /// Indicates the "value not between" filter for text and numeric values.
        /// </summary>
        ValueNotBetween,

        /// <summary>
        /// Indicates the "value not equal" filter for text and numeric values.
        /// </summary>
        ValueNotEqual,

        /// <summary>
        /// Indicates the "count" filter.
        /// </summary>
        Count,
    }

    ///<summary>
    ///Filter type
    ///</summary>
    public enum PivotFilterType2007
    {
        /// <summary>
        ///  Indicates the "caption begins with" filter for text and numeric values
        /// </summary>
        captionBeginsWith = PivotFilterType .CaptionBeginsWith,

        /// <summary>
        ///  Indicates the "caption between" filter for text and numeric values
        /// </summary>
        captionBetween = PivotFilterType .CaptionBetween,

        /// <summary>
        ///  Indicates the "caption contains" filter for text and numeric values
        /// </summary>
        captionContains = PivotFilterType .CaptionContains,

        /// <summary>
        ///  Indicates the "caption ends with" filter for text and numeric values
        /// </summary>
        captionEndsWith = PivotFilterType .CaptionEndsWith,

        /// <summary>
        ///  Indicates the "caption equal" filter for field captions.
        /// </summary>
        captionEqual = PivotFilterType .CaptionEqual,

        /// <summary>
        ///  Indicates the "caption greater than" filter for field captions.
        /// </summary>
        captionGreaterThan = PivotFilterType .CaptionGreaterThan,

        /// <summary>
        ///  Indicates the "caption greater than or equal to" filter for field captions.
        /// </summary>
        captionGreaterThanOrEqual = PivotFilterType .CaptionGreaterThanOrEqual,

        /// <summary>
        ///  Indicates the "caption less than" filter for field captions.
        /// </summary>
        captionLessThan = PivotFilterType .CaptionLessThan,

        /// <summary>
        ///  Indicates the "caption less than or equal" filter for field captions.
        /// </summary>
        captionLessThanOrEqual = PivotFilterType .CaptionLessThanOrEqual,

        /// <summary>
        ///  Indicates the "caption not begins with" filter for field captions.
        /// </summary>
        captionNotBeginsWith = PivotFilterType .CaptionNotBeginsWith,

        /// <summary>
        ///  Indicates the "caption not between" filter for field captions.
        /// </summary>
        captionNotBetween = PivotFilterType .CaptionNotBetween,

        /// <summary>
        ///  Indicates the "caption not contains" filter for field captions.
        /// </summary>
        captionNotContains = PivotFilterType .CaptionNotContains,

        /// <summary>
        ///  Indicates the "caption not ends with" filter for field captions.
        /// </summary>
        captionNotEndsWith = PivotFilterType .CaptionNotEndsWith,

        /// <summary>
        ///  Indicates the "Caption not equal" filter for field captions.
        /// </summary>
        captionNotEqual = PivotFilterType .CaptionNotEqual,

        /// <summary>
        /// Indicates the "value between" filter for text and numeric values
        /// </summary>
        valueBetween = PivotFilterType .ValueBetween,

        /// <summary>
        /// Indicates the "value equal" filter for text and numeric values
        /// </summary>
        valueEqual = PivotFilterType .ValueEqual,

        /// <summary>
        /// Indicates the "value Greater than " filter for text and numeric values
        /// </summary>
        valueGreaterThan  = PivotFilterType .ValueGreaterThan,

        /// <summary>
        /// Indicates the "value greater than or equal to" filter for text and numeric values
        /// </summary>
        valueGreaterThanOrEqual = PivotFilterType .ValueGreaterThanOrEqual,

        /// <summary>
        /// Indicates the "value less than " filter for text and numeric values
        /// </summary>
        valueLessThan = PivotFilterType .ValueLessThan,

        /// <summary>
        /// Indicates the "value less than or equal to" filter for text and numeric values
        /// </summary>
        valueLessThanOrEqual = PivotFilterType .ValueLessThanOrEqual,

        /// <summary>
        /// Indicates the "value not between" filter for text and numeric values.
        /// </summary>
        valueNotBetween = PivotFilterType .ValueNotBetween,

        /// <summary>
        /// Indicates the "value not equal" filter for text and numeric values.
        /// </summary>
        valueNotEqual = PivotFilterType .ValueNotEqual,
        /// <summary>
        /// Indicates the "count" filter.
        /// </summary>
        count = PivotFilterType .Count ,
    }

    /// <summary>
    /// Axis types.
    /// </summary>
    public enum PivotAxisTypes
    {
        /// <summary>
        /// Represents the None axis type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Represents the Row axis type.
        /// </summary>
        Row = 1,
        /// <summary>
        /// Represents the Column axis type.
        /// </summary>
        Column = 2,
        /// <summary>
        /// Represents the Page axis type.
        /// </summary>
        Page = 4,
        /// <summary>
        /// Represents the Data axis type.
        /// </summary>
        Data = 8,
    }
    /// <summary>
    /// Axis types.
    /// </summary>
    public enum PivotAxisTypes2007
    {
        /// <summary>
        /// Represents the Row axis type.
        /// </summary>
        axisRow = PivotAxisTypes.Row,
        /// <summary>
        /// Represents the Column axis type.
        /// </summary>
        axisCol = PivotAxisTypes.Column,
        /// <summary>
        /// Represents the Page axis type.
        /// </summary>
        axisPage = PivotAxisTypes.Page,
        /// <summary>
        /// Represents the Data axis type.
        /// </summary>
        axisValues = PivotAxisTypes.Data,
    }
    /// <summary>
    /// Represents the  Subtotal types.
    /// </summary>
    [Flags]
    public enum PivotSubtotalTypes
    {
        /// <summary>
        /// Represents the None type.
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Represents the Default type.
        /// </summary>
        Default = 0x0001,
        /// <summary>
        /// Represents the Sum type.
        /// </summary>
        Sum = 0x0002,
        /// <summary>
        /// Represents the Counta type.
        /// </summary>
        Counta = 0x0004,
        /// <summary>
        /// Represents the Average type.
        /// </summary>
        Average = 0x0008,
        /// <summary>
        /// Represents the Max type.
        /// </summary>
        Max = 0x0010,
        /// <summary>
        /// Represents the Min type.
        /// </summary>
        Min = 0x0020,
        /// <summary>
        /// Represents the Product type.
        /// </summary>
        Product = 0x0040,
        /// <summary>
        /// Represents the Count type.
        /// </summary>
        Count = 0x0080,
        /// <summary>
        /// Represents the Stdev type.
        /// </summary>
        Stdev = 0x0100,
        /// <summary>
        /// Represents the Stdevp type.
        /// </summary>
        Stdevp = 0x0200,
        /// <summary>
        /// Represents the Var type.
        /// </summary>
        Var = 0x0400,
        /// <summary>
        /// Represents the Varp type.
        /// </summary>
        Varp = 0x0800,
    }
    /// <summary>
    /// Axis types in Excel 2007 file format notation.
    /// </summary>
    [Flags]
    public enum PivotSubtotalTypes2007
    {
        /// <summary>
        /// Represents the Average type.
        /// </summary>
        average = PivotSubtotalTypes.Average,
        /// <summary>
        /// Represents the Count type.
        /// </summary>
        count = PivotSubtotalTypes.Count,
        /// <summary>
        /// Represents the Counta type.
        /// </summary>
        countNums = PivotSubtotalTypes.Counta,
        /// <summary>
        /// Represents the Max type.
        /// </summary>
        max = PivotSubtotalTypes.Max,
        /// <summary>
        /// Represents the Min type.
        /// </summary>
        min = PivotSubtotalTypes.Min,
        /// <summary>
        /// Represents the Product type.
        /// </summary>
        product = PivotSubtotalTypes.Product,
        /// <summary>
        /// Represents the Stdev type.
        /// </summary>
        stdDev = PivotSubtotalTypes.Stdev,
        /// <summary>
        /// Represents the Stdevp type.
        /// </summary>
        stdDevp = PivotSubtotalTypes.Stdevp,
        /// <summary>
        /// Represents the Sum type.
        /// </summary>
        sum = PivotSubtotalTypes.Sum,
        /// <summary>
        /// Represents the Var type.
        /// </summary>
        var = PivotSubtotalTypes.Var,
        /// <summary>
        /// Represents the Varp type.
        /// </summary>
        varp = PivotSubtotalTypes.Varp,
    }
    /// <summary>
    /// Axis types in Excel 2007 file format notation.
    /// </summary>
    [Flags]
    public enum PivotSubtotalItems2007
    {
        /// <summary>
        /// Represents the Average type.
        /// </summary>
        avg = PivotSubtotalTypes.Average,
        /// <summary>
        /// Represents the Count type.
        /// </summary>
        count = PivotSubtotalTypes.Count,
        /// <summary>
        /// Represents the Counta type.
        /// </summary>
        countA = PivotSubtotalTypes.Counta,
        /// <summary>
        /// Represents the Max type.
        /// </summary>
        max = PivotSubtotalTypes.Max,
        /// <summary>
        /// Represents the Min type.
        /// </summary>
        min = PivotSubtotalTypes.Min,
        /// <summary>
        /// Represents the Product type.
        /// </summary>
        product = PivotSubtotalTypes.Product,
        /// <summary>
        /// Represents the Stdev type.
        /// </summary>
        stdDev = PivotSubtotalTypes.Stdev,
        /// <summary>
        /// Represents the Stdevp type.
        /// </summary>
        stdDevP = PivotSubtotalTypes.Stdevp,
        /// <summary>
        /// Represents the Sum type.
        /// </summary>
        sum = PivotSubtotalTypes.Sum,
        /// <summary>
        /// Represents the Var type.
        /// </summary>
        var = PivotSubtotalTypes.Var,
        /// <summary>
        /// Represents the Varp type.
        /// </summary>
        varP = PivotSubtotalTypes.Varp,
    }
    /// <summary>
    /// Data types for pivot table cache fields.
    /// </summary>
    [Flags]
    public enum PivotDataType
    {
        /// <summary>
        /// Indicates whether field contains number.
        /// </summary>
        Number = 1,
        /// <summary>
        /// Indicates whether field contains integer numbers.
        /// </summary>
        Integer = 2,
        /// <summary>
        /// Indicates whether field contains strings.
        /// </summary>
        String = 4,
        /// <summary>
        /// Indicates whether field contains blank values.
        /// </summary>
        Blank = 8,
        /// <summary>
        /// Indicates whether field contains dates.
        /// </summary>
        Date = 16,
        /// <summary>
        /// Indicates whether field contains booleans.
        /// </summary>
        Boolean = 32,
        /// <summary>
        /// Indicates whether field contains float numbers.
        /// </summary>
        Float = 64,
        /// <summary>
        /// Indicates whether filed contains long Text.
        /// </summary>
        LongText=128
    }
    /// <summary>
    /// Excel 2007 pivot table built in styles.
    /// </summary>
    public enum PivotBuiltInStyles
    {
        /// <summary>
        /// Represents PivotStyleMedium28 style.
        /// </summary>
        PivotStyleMedium28,
        /// <summary>
        /// Represents PivotStyleMedium27 style.
        /// </summary>
        PivotStyleMedium27,
        /// <summary>
        /// Represents PivotStyleMedium26 style.
        /// </summary>
        PivotStyleMedium26,
        /// <summary>
        /// Represents PivotStyleMedium25 style.
        /// </summary>
        PivotStyleMedium25,
        /// <summary>
        /// Represents PivotStyleMedium24 style.
        /// </summary>
        PivotStyleMedium24,
        /// <summary>
        /// Represents PivotStyleMedium23 style.
        /// </summary>
        PivotStyleMedium23,
        /// <summary>
        /// Represents PivotStyleMedium22 style.
        /// </summary>
        PivotStyleMedium22,
        /// <summary>
        /// Represents PivotStyleMedium21 style.
        /// </summary>
        PivotStyleMedium21,
        /// <summary>
        /// Represents PivotStyleMedium20 style.
        /// </summary>
        PivotStyleMedium20,
        /// <summary>
        /// Represents PivotStyleMedium19 style.
        /// </summary>
        PivotStyleMedium19,
        /// <summary>
        /// Represents PivotStyleMedium18 style.
        /// </summary>
        PivotStyleMedium18,
        /// <summary>
        /// Represents PivotStyleMedium17 style.
        /// </summary>
        PivotStyleMedium17,
        /// <summary>
        /// Represents PivotStyleMedium16 style.
        /// </summary>
        PivotStyleMedium16,
        /// <summary>
        /// Represents PivotStyleMedium15 style.
        /// </summary>
        PivotStyleMedium15,
        /// <summary>
        /// Represents PivotStyleMedium14 style.
        /// </summary>
        PivotStyleMedium14,
        /// <summary>
        /// Represents PivotStyleMedium13 style.
        /// </summary>
        PivotStyleMedium13,
        /// <summary>
        /// Represents PivotStyleMedium12 style.
        /// </summary>
        PivotStyleMedium12,
        /// <summary>
        /// Represents PivotStyleMedium11 style.
        /// </summary>
        PivotStyleMedium11,
        /// <summary>
        /// Represents PivotStyleMedium10 style.
        /// </summary>
        PivotStyleMedium10,
        /// <summary>
        /// Represents PivotStyleMedium9 style.
        /// </summary>
        PivotStyleMedium9,
        /// <summary>
        /// Represents PivotStyleMedium8 style.
        /// </summary>
        PivotStyleMedium8,
        /// <summary>
        /// Represents PivotStyleMedium7 style.
        /// </summary>
        PivotStyleMedium7,
        /// <summary>
        /// Represents PivotStyleMedium6 style.
        /// </summary>
        PivotStyleMedium6,
        /// <summary>
        /// Represents PivotStyleMedium5 style.
        /// </summary>
        PivotStyleMedium5,
        /// <summary>
        /// Represents PivotStyleMedium4 style.
        /// </summary>
        PivotStyleMedium4,
        /// <summary>
        /// Represents PivotStyleMedium3 style.
        /// </summary>
        PivotStyleMedium3,
        /// <summary>
        /// Represents PivotStyleMedium2 style.
        /// </summary>
        PivotStyleMedium2,
        /// <summary>
        /// Represents PivotStyleMedium1 style.
        /// </summary>
        PivotStyleMedium1,
        /// <summary>
        /// Represents PivotStyleLight28 style.
        /// </summary>
        PivotStyleLight28,
        /// <summary>
        /// Represents PivotStyleLight27 style.
        /// </summary>
        PivotStyleLight27,
        /// <summary>
        /// Represents PivotStyleLight26 style.
        /// </summary>
        PivotStyleLight26,
        /// <summary>
        /// Represents PivotStyleLight25 style.
        /// </summary>
        PivotStyleLight25,
        /// <summary>
        /// Represents PivotStyleLight24 style.
        /// </summary>
        PivotStyleLight24,
        /// <summary>
        /// Represents PivotStyleLight23 style.
        /// </summary>
        PivotStyleLight23,
        /// <summary>
        /// Represents PivotStyleLight22 style.
        /// </summary>
        PivotStyleLight22,
        /// <summary>
        /// Represents PivotStyleLight21 style.
        /// </summary>
        PivotStyleLight21,
        /// <summary>
        /// Represents PivotStyleLight20 style.
        /// </summary>
        PivotStyleLight20,
        /// <summary>
        /// Represents PivotStyleLight19 style.
        /// </summary>
        PivotStyleLight19,
        /// <summary>
        /// Represents PivotStyleLight18 style.
        /// </summary>
        PivotStyleLight18,
        /// <summary>
        /// Represents PivotStyleLight17 style.
        /// </summary>
        PivotStyleLight17,
        /// <summary>
        /// Represents PivotStyleLight16 style.
        /// </summary>
        PivotStyleLight16,
        /// <summary>
        /// Represents PivotStyleLight15 style.
        /// </summary>
        PivotStyleLight15,
        /// <summary>
        /// Represents PivotStyleLight14 style.
        /// </summary>
        PivotStyleLight14,
        /// <summary>
        /// Represents PivotStyleLight13 style.
        /// </summary>
        PivotStyleLight13,
        /// <summary>
        /// Represents PivotStyleLight12 style.
        /// </summary>
        PivotStyleLight12,
        /// <summary>
        /// Represents PivotStyleLight11 style.
        /// </summary>
        PivotStyleLight11,
        /// <summary>
        /// Represents PivotStyleLight10 style.
        /// </summary>
        PivotStyleLight10,
        /// <summary>
        /// Represents PivotStyleLight9 style.
        /// </summary>
        PivotStyleLight9,
        /// <summary>
        /// Represents PivotStyleLight8 style.
        /// </summary>
        PivotStyleLight8,
        /// <summary>
        /// Represents PivotStyleLight7 style.
        /// </summary>
        PivotStyleLight7,
        /// <summary>
        /// Represents PivotStyleLight6 style.
        /// </summary>
        PivotStyleLight6,
        /// <summary>
        /// Represents PivotStyleLight5 style.
        /// </summary>
        PivotStyleLight5,
        /// <summary>
        /// Represents PivotStyleLight4 style.
        /// </summary>
        PivotStyleLight4,
        /// <summary>
        /// Represents PivotStyleLight3 style.
        /// </summary>
        PivotStyleLight3,
        /// <summary>
        /// Represents PivotStyleLight2 style.
        /// </summary>
        PivotStyleLight2,
        /// <summary>
        /// Represents PivotStyleLight1 style.
        /// </summary>
        PivotStyleLight1,
        /// <summary>
        /// Represents PivotStyleDark28 style.
        /// </summary>
        PivotStyleDark28,
        /// <summary>
        /// Represents PivotStyleDark27 style.
        /// </summary>
        PivotStyleDark27,
        /// <summary>
        /// Represents PivotStyleDark26 style.
        /// </summary>
        PivotStyleDark26,
        /// <summary>
        /// Represents PivotStyleDark25 style.
        /// </summary>
        PivotStyleDark25,
        /// <summary>
        /// Represents PivotStyleDark24 style.
        /// </summary>
        PivotStyleDark24,
        /// <summary>
        /// Represents PivotStyleDark23 style.
        /// </summary>
        PivotStyleDark23,
        /// <summary>
        /// Represents PivotStyleDark22 style.
        /// </summary>
        PivotStyleDark22,
        /// <summary>
        /// Represents PivotStyleDark21 style.
        /// </summary>
        PivotStyleDark21,
        /// <summary>
        /// Represents PivotStyleDark20 style.
        /// </summary>
        PivotStyleDark20,
        /// <summary>
        /// Represents PivotStyleDark19 style.
        /// </summary>
        PivotStyleDark19,
        /// <summary>
        /// Represents PivotStyleDark18 style.
        /// </summary>
        PivotStyleDark18,
        /// <summary>
        /// Represents PivotStyleDark17 style.
        /// </summary>
        PivotStyleDark17,
        /// <summary>
        /// Represents PivotStyleDark16 style.
        /// </summary>
        PivotStyleDark16,
        /// <summary>
        /// Represents PivotStyleDark15 style.
        /// </summary>
        PivotStyleDark15,
        /// <summary>
        /// Represents PivotStyleDark14 style.
        /// </summary>
        PivotStyleDark14,
        /// <summary>
        /// Represents PivotStyleDark13 style.
        /// </summary>
        PivotStyleDark13,
        /// <summary>
        /// Represents PivotStyleDark12 style.
        /// </summary>
        PivotStyleDark12,
        /// <summary>
        /// Represents PivotStyleDark11 style.
        /// </summary>
        PivotStyleDark11,
        /// <summary>
        /// Represents PivotStyleDark10 style.
        /// </summary>
        PivotStyleDark10,
        /// <summary>
        /// Represents PivotStyleDark9 style.
        /// </summary>
        PivotStyleDark9,
        /// <summary>
        /// Represents PivotStyleDark8 style.
        /// </summary>
        PivotStyleDark8,
        /// <summary>
        /// Represents PivotStyleDark7 style.
        /// </summary>
        PivotStyleDark7,
        /// <summary>
        /// Represents PivotStyleDark6 style.
        /// </summary>
        PivotStyleDark6,
        /// <summary>
        /// Represents PivotStyleDark5 style.
        /// </summary>
        PivotStyleDark5,
        /// <summary>
        /// Represents PivotStyleDark4 style.
        /// </summary>
        PivotStyleDark4,
        /// <summary>
        /// Represents PivotStyleDark3 style.
        /// </summary>
        PivotStyleDark3,
        /// <summary>
        /// Represents PivotStyleDark2 style.
        /// </summary>
        PivotStyleDark2,
        /// <summary>
        /// Represents PivotStyleDark1 style.
        /// </summary>
        PivotStyleDark1,
    }
    /// <summary>
    /// Represents the data formats for a field in the PivotTable
    /// </summary>
    public enum PivotFieldDataFormat
    {
     /// <summary>
     /// Indicates the field is shown as the "difference from" a value.
     /// </summary>
     Difference,
     /// <summary>
     /// Indicates the field is shown as the "index.
     /// </summary>
     Index,
     /// <summary>
     /// Indicates that the field is shown as its normal data type.
     /// </summary>
     Normal,
     /// <summary>
     /// Indicates the field is show as the "percentage of".
     /// </summary>
     Percent,     
     /// <summary>
     /// Indicates the field is shown as the "percentage difference from" a value.
     /// </summary>
     PercentageOfDifference,
     /// <summary>
     /// Indicates the field is shown as the percentage of column.
     /// </summary>
     PercentageOfColumn,
     /// <summary>
     /// Indicates the field is shown as the percentage of row.
     /// </summary>
     PercentageOfRow,
     /// <summary>
     /// Indicates the field is shown as percentage of total.
     /// </summary>
     PercentageOfTotal,
     /// <summary>
     /// Indicates Percentage of parent total.
     /// </summary>
     PercentageOfParent ,
     /// <summary>
     /// Indicates Percentage of parent column total.
     /// </summary>
     PercentageOfParentColumn ,
     /// <summary>
     /// Indicates Percentage of parent row total.
     /// </summary>
     PercentageOfParentRow ,
     /// <summary>
     /// Indicates Rank descending.
     /// </summary>
     RankDecending ,
     /// <summary>
     /// Indicates Percentage of running total.
     /// </summary>
     PercentageOfRunningTotal,
     /// <summary>
     /// Indicates the field is shown as running total in the table.
     /// </summary>
     RunTotal,
     /// <summary>
     /// Indicates Rank ascending.
     /// </summary>
     RankAscending,

    }
    /// <summary>
    /// Excel 2007 table built in styles.
    /// </summary>
    public enum TableBuiltInStyles
    {
        /// <summary>
        /// Represents TableStyleMedium28 style.
        /// </summary>
        TableStyleMedium28,
        /// <summary>
        /// Represents TableStyleMedium27 style.
        /// </summary>
        TableStyleMedium27,
        /// <summary>
        /// Represents TableStyleMedium26 style.
        /// </summary>
        TableStyleMedium26,
        /// <summary>
        /// Represents TableStyleMedium25 style.
        /// </summary>
        TableStyleMedium25,
        /// <summary>
        /// Represents TableStyleMedium24 style.
        /// </summary>
        TableStyleMedium24,
        /// <summary>
        /// Represents TableStyleMedium23 style.
        /// </summary>
        TableStyleMedium23,
        /// <summary>
        /// Represents TableStyleMedium22 style.
        /// </summary>
        TableStyleMedium22,
        /// <summary>
        /// Represents TableStyleMedium21 style.
        /// </summary>
        TableStyleMedium21,
        /// <summary>
        /// Represents TableStyleMedium20 style.
        /// </summary>
        TableStyleMedium20,
        /// <summary>
        /// Represents TableStyleMedium19 style.
        /// </summary>
        TableStyleMedium19,
        /// <summary>
        /// Represents TableStyleMedium18 style.
        /// </summary>
        TableStyleMedium18,
        /// <summary>
        /// Represents TableStyleMedium17 style.
        /// </summary>
        TableStyleMedium17,
        /// <summary>
        /// Represents TableStyleMedium16 style.
        /// </summary>
        TableStyleMedium16,
        /// <summary>
        /// Represents TableStyleMedium15 style.
        /// </summary>
        TableStyleMedium15,
        /// <summary>
        /// Represents TableStyleMedium14 style.
        /// </summary>
        TableStyleMedium14,
        /// <summary>
        /// Represents TableStyleMedium13 style.
        /// </summary>
        TableStyleMedium13,
        /// <summary>
        /// Represents TableStyleMedium12 style.
        /// </summary>
        TableStyleMedium12,
        /// <summary>
        /// Represents TableStyleMedium11 style.
        /// </summary>
        TableStyleMedium11,
        /// <summary>
        /// Represents TableStyleMedium10 style.
        /// </summary>
        TableStyleMedium10,
        /// <summary>
        /// Represents TableStyleMedium9 style.
        /// </summary>
        TableStyleMedium9,
        /// <summary>
        /// Represents TableStyleMedium8 style.
        /// </summary>
        TableStyleMedium8,
        /// <summary>
        /// Represents TableStyleMedium7 style.
        /// </summary>
        TableStyleMedium7,
        /// <summary>
        /// Represents TableStyleMedium6 style.
        /// </summary>
        TableStyleMedium6,
        /// <summary>
        /// Represents TableStyleMedium5 style.
        /// </summary>
        TableStyleMedium5,
        /// <summary>
        /// Represents TableStyleMedium4 style.
        /// </summary>
        TableStyleMedium4,
        /// <summary>
        /// Represents TableStyleMedium3 style.
        /// </summary>
        TableStyleMedium3,
        /// <summary>
        /// Represents TableStyleMedium2 style.
        /// </summary>
        TableStyleMedium2,
        /// <summary>
        /// Represents TableStyleMedium1 style.
        /// </summary>
        TableStyleMedium1,
        /// <summary>
        /// Represents TableStyleLight21 style.
        /// </summary>
        TableStyleLight21,
        /// <summary>
        /// Represents TableStyleLight20 style.
        /// </summary>
        TableStyleLight20,
        /// <summary>
        /// Represents TableStyleLight19 style.
        /// </summary>
        TableStyleLight19,
        /// <summary>
        /// Represents TableStyleLight18 style.
        /// </summary>
        TableStyleLight18,
        /// <summary>
        /// Represents TableStyleLight17 style.
        /// </summary>
        TableStyleLight17,
        /// <summary>
        /// Represents TableStyleLight16 style.
        /// </summary>
        TableStyleLight16,
        /// <summary>
        /// Represents TableStyleLight15 style.
        /// </summary>
        TableStyleLight15,
        /// <summary>
        /// Represents TableStyleLight14 style.
        /// </summary>
        TableStyleLight14,
        /// <summary>
        /// Represents TableStyleLight13 style.
        /// </summary>
        TableStyleLight13,
        /// <summary>
        /// Represents TableStyleLight12 style.
        /// </summary>
        TableStyleLight12,
        /// <summary>
        /// Represents TableStyleLight11 style.
        /// </summary>
        TableStyleLight11,
        /// <summary>
        /// Represents TableStyleLight10 style.
        /// </summary>
        TableStyleLight10,
        /// <summary>
        /// Represents TableStyleLight9 style.
        /// </summary>
        TableStyleLight9,
        /// <summary>
        /// Represents TableStyleLight8 style.
        /// </summary>
        TableStyleLight8,
        /// <summary>
        /// Represents TableStyleLight7 style.
        /// </summary>
        TableStyleLight7,
        /// <summary>
        /// Represents TableStyleLight6 style.
        /// </summary>
        TableStyleLight6,
        /// <summary>
        /// Represents TableStyleLight5 style.
        /// </summary>
        TableStyleLight5,
        /// <summary>
        /// Represents TableStyleLight4 style.
        /// </summary>
        TableStyleLight4,
        /// <summary>
        /// Represents TableStyleLight3 style.
        /// </summary>
        TableStyleLight3,
        /// <summary>
        /// Represents TableStyleLight2 style.
        /// </summary>
        TableStyleLight2,
        /// <summary>
        /// Represents TableStyleLight1 style.
        /// </summary>
        TableStyleLight1,
        /// <summary>
        /// Represents TableStyleDark11 style.
        /// </summary>
        TableStyleDark11,
        /// <summary>
        /// Represents TableStyleDark10 style.
        /// </summary>
        TableStyleDark10,
        /// <summary>
        /// Represents TableStyleDark9 style.
        /// </summary>
        TableStyleDark9,
        /// <summary>
        /// Represents TableStyleDark8 style.
        /// </summary>
        TableStyleDark8,
        /// <summary>
        /// Represents TableStyleDark7 style.
        /// </summary>
        TableStyleDark7,
        /// <summary>
        /// Represents TableStyleDark6 style.
        /// </summary>
        TableStyleDark6,
        /// <summary>
        /// Represents TableStyleDark5 style.
        /// </summary>
        TableStyleDark5,
        /// <summary>
        /// Represents TableStyleDark4 style.
        /// </summary>
        TableStyleDark4,
        /// <summary>
        /// Represents TableStyleDark3 style.
        /// </summary>
        TableStyleDark3,
        /// <summary>
        /// Represents TableStyleDark2 style.
        /// </summary>
        TableStyleDark2,
        /// <summary>
        /// Represents TableStyleDark1 style.
        /// </summary>
        TableStyleDark1,
    }
    /// <summary>
    ///  sort orders that can be applied to fields in a PivotTable.
    /// </summary>
    public enum PivotFieldSortType
    {
        /// <summary>
        /// Indicates the field is sorted in ascending order.
        /// </summary>
        Ascending,
        /// <summary>
        /// Indicates the field is sorted in descending order.
        /// </summary>
        Descending,
        /// <summary>
        /// Indicates the field is sorted manually.
        /// </summary>
        Manual
    }
    /// <summary>
    /// This simple type defines the pivot type for a pivotItem.
    /// </summary>
    public enum PivotItemType
    {
        /// <summary>
        /// Represents the Average 
        /// </summary>
        Average,
        /// <summary>
        /// Represent the Blank line in the pivot Table
        /// </summary>
        Blank,
        /// <summary>
        /// Represent the count aggregate functions
        /// </summary>
        Count,
        /// <summary>
        /// Represent the count number aggregate functions
        /// </summary>
        CountA,
        /// <summary>
        /// Represent the data
        /// </summary>
        Data,
        /// <summary>
        /// Represent the default type of Pivot Table.
        /// (total aggregate is default)
        /// </summary>
        Default,
        /// <summary>
        /// Represent the grand total
        /// </summary>
        GrandTotal,
        /// <summary>
        /// Represent the maximum aggregate function
        /// </summary>
        Max,
        /// <summary>
        /// Represent the minimum aggregate function
        /// </summary>
        Min,
        /// <summary>
        /// Represent the product functions
        /// </summary>
        Product,
        /// <summary>
        /// Represents the "standard deviation" aggregate function.
        /// </summary>
        StdDev,
        /// <summary>
        /// Represents the "standard deviation population" aggregate function.
        /// </summary>
        StdDevP,
        /// <summary>
        /// Represents the "sum" aggregate value.
        /// </summary>
        Sum,
        /// <summary>
        /// Represents the "variance" aggregate value.
        /// </summary>
        Var,
        /// <summary>
        /// Represents the "variance population" aggregate value.
        /// </summary>
        VarP,
    }
    /// <summary>
    /// This simple type defines the Open Xml Pivot type for a pivotItem.
    /// </summary>
    public enum PivotItemType2007
    {
        /// <summary>
        /// Represents the Average 
        /// </summary>
        avg = PivotItemType.Average,
        /// <summary>
        /// Represent the Blank line in the pivot Table
        /// </summary>
        blank = PivotItemType.Blank,
        /// <summary>
        /// Represent the count aggregate functions
        /// </summary>
        count = PivotItemType.Count,
        /// <summary>
        /// Represent the count number aggregate functions
        /// </summary>
        countA = PivotItemType.CountA,
        /// <summary>
        /// Represent the data
        /// </summary>
        data = PivotItemType.Data,
        /// <summary>
        /// Represent the default type of Pivot Table.
        /// (total aggregate is default)
        /// </summary>
        defaults = PivotItemType.Default,
        /// <summary>
        /// Represent the grand total
        /// </summary>
        grand = PivotItemType.GrandTotal,
        /// <summary>
        /// Represent the maximum aggregate function
        /// </summary>
        max = PivotItemType.Max,
        /// <summary>
        /// Represent the minimum aggregate function
        /// </summary>
        min = PivotItemType.Min,
        /// <summary>
        /// Represent the product functions
        /// </summary>
        product = PivotItemType.Product,
        /// <summary>
        /// Represents the "standard deviation" aggregate function.
        /// </summary>
        stdDev = PivotItemType.StdDev,
        /// <summary>
        /// Represents the "standard deviation population" aggregate function.
        /// </summary>
        stdDevP = PivotItemType.StdDevP,
        /// <summary>
        /// Represents the "sum" aggregate value.
        /// </summary>
        sum = PivotItemType.Sum,
        /// <summary>
        /// Represents the "variance" aggregate value.
        /// </summary>
        var = PivotItemType.Var,
        /// <summary>
        /// Represents the "variance population" aggregate value.
        /// </summary>
        varP = PivotItemType.VarP
    }
    /// <summary>
    /// Represents data grouping that can be performed on a PivotTable.
    /// </summary>
    public enum PivotFieldGroupType
    {
        /// <summary>
        /// Indicates a grouping on "days" for date values.
        /// </summary>
        Days,
        /// <summary>
        /// Indicates a grouping on "hours" for date values.
        /// </summary>
        Hours,
        /// <summary>
        /// Indicates a grouping on "minutes" for date values.
        /// </summary>
        Minutes,
        /// <summary>
        /// Indicates a grouping on "months" for date values.
        /// </summary>
        Months,
        /// <summary>
        /// Indicates a grouping on "quarters" for date values
        /// </summary>
        Quarters,
        /// <summary>
        /// Indicates a grouping by numeric ranges for numeric values.
        /// </summary>
        Range,
        /// <summary>
        /// Indicates a grouping on "seconds" for date values.
        /// </summary>
        Seconds,
        /// <summary>
        /// Indicates a grouping on "years" for date values.
        /// </summary>
        Years,
        /// <summary>
        /// Indicates no auto grouping group 
        /// </summary>
        None
    }
    /// <summary>
    /// Indicates the type of rule being used to describe an area or aspect of the PivotTable.
    /// </summary>
    public enum PivotAreaType
    {
        /// <summary>
        /// Refers to the whole PivotTable.
        /// </summary>
        All,
        /// <summary>
        /// Refers to a field button.
        /// </summary>
        FieldButton,
        /// <summary>
        /// Refers to something in the data area.
        /// </summary>
        Data,
        /// <summary>
        /// Refers to no Pivot area.
        /// </summary>
        None,
        /// <summary>
        /// Refers to a header or item.
        /// </summary>
        Normal,
        /// <summary>
        /// Refers to the blank cells at the top-left of the PivotTable
        /// </summary>
        Orgin,
        /// <summary>
        /// Refers to the blank cells at the top of the PivotTable,
        ///on its trailing edge
        /// </summary>
        TopEnd
    }
    /// <summary>
    /// Reperesents the scope of conditional formatting applied in the PivotTable
    /// </summary>
    public enum ConditionalFormatScope
    {
        /// <summary>
        /// Indicates that conditional formatting is applied to the selected data fields.
        /// </summary>
        DataFields,
        /// <summary>
        /// Indicates that conditional formatting is applied to the
        ///selected PivotTable field intersections.
        /// </summary>
        FieldIntersections,
        /// <summary>
        /// Indicates that conditional formatting is applied to the
        ///selected cells.
        /// </summary>
        Selections
    }
    /// <summary>
    /// This simple type defines the values for the Top N conditional formatting evaluation for the PivotTable
    /// </summary>
    public enum ConditionalTopNType
    {
        /// <summary>
        /// Indicates that Top N conditional formatting is evaluated across the entire scope range.
        /// </summary>
        All,
        /// <summary>
        /// Indicates that Top N conditional formatting is evaluated for each column.
        /// </summary>
        Column,
        /// <summary>
        /// Indicates that Top N conditional formatting is not  evaluated
        /// </summary>
        None,
        /// <summary>
        /// Indicates that Top N conditional formatting is not evaluated
        /// </summary>
        Row
    }
    /// <summary>
    /// Repersents the Page field area order when there are
    /// multiple page fields in the page area
    /// </summary>
    public enum PivotPageAreaFieldsOrder
    {
        /// <summary>
        /// The page fields are laid, Down then over order
        /// </summary>
        DownThenOver,
        /// <summary>
        /// The page fields are laid, over then down order
        /// </summary>
        OverThenDown
    }
    /// <summary>
    /// Specifies the pivot table layout
    /// </summary>
    public enum PivotTableRowLayout
    {
        /// <summary>
        /// Compact Row
        /// </summary>
        Compact,
        /// <summary>
        /// Outline Row
        /// </summary>
        Outline,
        /// <summary>
        /// Tabular Row
        /// </summary>
        Tabular,
    }

}
