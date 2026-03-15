#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.ProjIO
{
    //Project.cs
    /// <summary>
    /// The Fiscal Year starting month
    /// </summary>
    public enum FYStartDate
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        January = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        February = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        March = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        April = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        May = 5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        June = 6,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        July = 7,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        August = 8,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        September = 9,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        October = 10,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        November = 11,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("12")]
        December = 12,
    }

    /// <summary>
    /// The position of the currency symbol
    /// </summary>
    public enum CurrencySymbolPosition
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Before,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        After,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        BeforeWithSpace,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        AfterWthSpace,
    }

    /// <summary>
    /// The default type of new tasks
    /// </summary>
    public enum TaskType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        FixedUnits,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        FixedDuration,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        FixedWork,
    }

    /// <summary>
    /// The default from where fixed costs are accrued
    /// </summary>
    public enum DefaultFixedCostAccrual
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Start = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Prorated = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        End = 3,
    }

    /// <summary>
    /// The format for expressing the bulk duration
    /// </summary>
    public enum DurationFormat
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Minutes = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        ElapsedMinutes = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Hours = 5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        ElapsedHours = 6,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Days = 7,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        ElapsedDays = 8,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        Weeks = 9,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        ElapsedWeeks = 10,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        Months = 11,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("12")]
        ElapsedMonths = 12,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("19")]
        Percent = 19,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("20")]
        ElapsedPercent = 20,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("21")]
        Null = 21,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("35")]
        EstimatedMinutes = 35,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("36")]
        ElapsedEstimatedMinutes = 36,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("37")]
        EstimatedHours = 37,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("38")]
        ElapsedEstimatedHours = 38,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("39")]
        EstimatedDays = 39,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("40")]
        ElapsedEstimatedDays = 40,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("41")]
        EstimatedWeeks = 41,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("42")]
        ElapsedEstimatedWeeks = 42,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("43")]
        EstimatedMonths = 43,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("44")]
        ElapsedEstimatedMonths = 44,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("51")]
        EstimatedPercent = 51,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("52")]
        ElapsedEstimatedPercent = 52,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("53")]
        Empty = 53,
    }

    /// <summary>
    /// The default work unit format
    /// </summary>
    public enum WorkFormat
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Minutes = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Hours = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Days = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Weeks = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Months = 5,
    }

    /// <summary>
    /// The default method for calculating earned value
    /// </summary>
    public enum EarnedValueMethod
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        PercentComplete,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        PhysicalPercentComplete,
    }

    /// <summary>
    /// Start day of the week
    /// </summary>
    public enum WeekStartDay
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Sunday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Monday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Tuesday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Wednesday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Thursday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Friday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Saturday,
    }

    /// <summary>
    /// The specific baseline used to calculate Variance values
    /// </summary>
    public enum BaselineForEarnedValue
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Baseline,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Baseline1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Baseline2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Baseline3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Baseline4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Baseline5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Baseline6,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Baseline7,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        Baseline8,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        Baseline9,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        Baseline10,
    }

    /// <summary>
    /// The default date for new tasks start
    /// </summary>
    public enum NewTaskStartDate
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        ProjectStartDate,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        CurrentDate,
    }

    //TimephasedDataType.cs
    /// <summary>
    /// The type of task timephased data
    /// </summary>
    public enum TimephasedDataTypeType
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        AssignmentRemainingWork = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        AssignmentActualWork = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        AssignmentActualOvertimeWork = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        AssignmentBaselineWork = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        AssignmentBaselineCos = 5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        AssignmentActualCost = 6,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        ResourceBaselineWork = 7,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        ResourceBaselineCost = 8,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        TaskBaselineWork = 9,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        TaskBaselineCost = 10,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        TaskPercentComplete = 11,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("16")]
        AssignmentBaseline1Work = 16,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("17")]
        AssignmentBaseline1Cost = 17,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("18")]
        TaskBaseline1Work = 18,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("19")]
        TaskBaseline1Cost = 19,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("20")]
        ResourceBaseline1Work = 20,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("21")]
        ResourceBaseline1Cost = 21,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("22")]
        AssignmentBaseline2Work = 22,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("23")]
        AssignmentBaseline2Cost = 23,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("24")]
        TaskBaseline2Work = 24,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("25")]
        TaskBaseline2Cost = 25,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("26")]
        ResourceBaseline2Work = 26,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("27")]
        ResourceBaseline2Cost = 27,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("28")]
        AssignmentBaseline3Work = 28,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("29")]
        AssignmentBaseline3Cost = 29,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("30")]
        TaskBaseline3Work = 30,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("31")]
        TaskBaseline3Cost = 31,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("32")]
        ResourceBaseline3Work = 32,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("33")]
        ResourceBaseline3Cost = 33,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("34")]
        AssignmentBaseline4Work = 34,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("35")]
        AssignmentBaseline4Cost = 35,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("36")]
        TaskBaseline4Work = 36,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("37")]
        TaskBaseline4Cost = 37,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("38")]
        ResourceBaseline4Work = 38,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("39")]
        ResourceBaseline4Cost = 39,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("40")]
        AssignmentBaseline5Work = 40,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("41")]
        AssignmentBaseline5Cost = 41,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("42")]
        TaskBaseline5Work = 42,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("43")]
        TaskBaseline5Cost = 43,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("44")]
        ResourceBaseline5Work = 44,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("45")]
        ResourceBaseline5Cost = 45,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("46")]
        AssignmentBaseline6Work = 46,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("47")]
        AssignmentBaseline6Cost = 47,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("48")]
        TaskBaseline6Work = 48,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("49")]
        TaskBaseline6Cost = 49,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("50")]
        ResourceBaseline6Work = 50,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("51")]
        ResourceBaseline6Cost = 51,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("52")]
        AssignmentBaseline7Work = 52,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("53")]
        AssignmentBaseline7Cost = 53,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("54")]
        TaskBaseline7Work = 54,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("55")]
        TaskBaseline7Cost = 55,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("56")]
        ResourceBaseline7Work = 56,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("57")]
        ResourceBaseline7Cost = 57,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("58")]
        AssignmentBaseline8Work = 58,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("59")]
        AssignmentBaseline8Cost = 59,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("60")]
        TaskBaseline8Work = 60,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("61")]
        TaskBaseline8Cost = 61,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("62")]
        ResourceBaseline8Work = 62,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("63")]
        ResourceBaseline8Cost = 63,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("64")]
        AssignmentBaseline9Work = 64,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("65")]
        AssignmentBaseline9Cost = 65,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("66")]
        TaskBaseline9Work = 66,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("67")]
        TaskBaseline9Cost = 67,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("68")]
        ResourceBaseline9Work = 68,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("69")]
        ResourceBaseline9Cost = 69,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("70")]
        AssignmentBaseline10Work = 70,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("71")]
        AssignmentBaseline10Cost = 71,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("72")]
        TaskBaseline10Work = 72,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("73")]
        TaskBaseline10Cost = 73,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("74")]
        ResourceBaseline10Work = 74,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("75")]
        ResourceBaseline10Cost = 75,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("76")]
        PhysicalPercentComplete = 76,
    }

    /// <summary>
    /// The time unit of the timephased data period
    /// </summary>
    public enum TimephasedDataTypeUnit
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Minutes = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Hours = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Days = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Weeks = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Months = 5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        Years = 8,
    }

    /// <summary>
    /// How the fixed cost is accrued against the task
    /// </summary>
    public enum TaskFixedCostAccrual
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Start = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Prorated = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        End = 3,
    }

    //WBSMasks.cs
    /// <summary>
    /// The type of the node value
    /// </summary>
    public enum WBSMaskType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Numbers,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        UppercaseLetters,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        LowercaseLetters,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Characters,
    }

    //OutlineCode.cs
    /// <summary>
    /// The outline code type
    /// </summary>
    public enum OutlineCodeValueType
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Date = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Duration = 6,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        Cost = 9,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("15")]
        Number = 15,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("17")]
        Flag = 17,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("21")]
        Text = 21,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("27")]
        FinishDate = 27,
    }

    /// <summary>
    /// The type of mask
    /// </summary>
    public enum OutlineCodeMaskType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Numbers,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        UppercaseLetters,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        LowercaseLetters,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Characters,
    }

    //ExtendedAttributes.cs
    /// <summary>
    /// The custom field type
    /// </summary>
    public enum CFType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Cost,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Date,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Duration,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Finish,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Flag,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Number,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Start,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Text,
    }

    /// <summary>
    /// Element Type of ExtendedAttribute
    /// </summary>
    public enum ElemType
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("20")]
        Task = 20,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("21")]
        Resource = 21,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("22")]
        Calendar = 22,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("23")]
        Assignment = 23,
    }

    /// <summary>
    /// The way rollups are calculated
    /// </summary>
    public enum RollupType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Maximum,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Minimum,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        CountAll,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Sum,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Average,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        AverageFirstSubLevel,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        CountFirstSubLevel,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        CountNonSummaries,
    }

    /// <summary>
    /// Whether rollups are calculated for task and group summary rows 
    /// </summary>
    public enum CalculationType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        None,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Rollup,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Calculation,
    }

    /// <summary>
    /// The way value lists are sorted
    /// </summary>
    public enum ValuelistSortOrder
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Descending,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Ascending,
    }

    //WeekDay.cs
    /// <summary>
    /// The type of day
    /// </summary>
    public enum DayType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Exception,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Sunday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Monday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Tuesday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Wednesday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Thursday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Friday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Saturday,
    }

    //CalendarException.cs
    /// <summary>
    /// The exception type
    /// </summary>
    public enum ExceptionType
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Daily = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        YearlyByDayOfMonth = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        YearlyByPosition = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        MonthlyByDayOfMonth = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        MonthlyByPosition = 5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Weekly = 6,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        ByDayCount = 7,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        ByWeekdayCount = 8,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        NoException = 9,
    }

    /// <summary>
    /// The month item for which an exception recurrence is scheduled
    /// </summary>
    public enum ExceptionMonthItem
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Day,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Weekday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        WeekendDay,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Sunday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Monday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Tuesday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Wednesday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Thursday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        Friday,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        Saturday,
    }

    /// <summary>
    /// The position of a month item within a month
    /// </summary>
    public enum ExceptionMonthPosition
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        First,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Second,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Third,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Fourth,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Last,
    }

    /// <summary>
    /// The month for which an exception recurrence is scheduled
    /// </summary>
    public enum ExceptionMonth
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        January,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        February,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        March,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        April,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        May,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        June,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        July,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        August,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        September,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        October,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        November,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        December,
    }

    //Task.cs
    /// <summary>
    /// The constraint on the start or finish date of the task
    /// </summary>
    public enum TaskConstraintType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        AsSoonAsPossible,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        AsLateAsPossible,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        MustStartOn,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        MustFinishOn,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        StartNoEarlierThan,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        StartNoLaterThan,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        FinishNoEarlierThan,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        FinishNoLaterThan,
    }

    /// <summary>
    /// The format for expressing the duration of the delay
    /// </summary>
    public enum DelayFormat
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Minutes = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        ElapsedMinutes = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Hours = 5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        ElapsedHours = 6,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Days = 7,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        ElapsedDays = 8,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        Weeks = 9,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("10")]
        ElapsedWeeks = 10,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        Months = 11,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("12")]
        ElapsedMonths = 12,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("19")]
        Percent = 19,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("20")]
        ElapsedPercent = 20,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("21")]
        Null = 21,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("35")]
        Estimatedminutes = 35,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("36")]
        ElapsedEstimatedMinutes = 36,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("37")]
        EstimatedHours = 37,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("38")]
        ElapsedEstiimatedHours = 38,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("39")]
        EstimatedDays = 39,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("40")]
        ElapsedEstimatedDays = 40,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("41")]
        EstimatedWeeks = 41,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("42")]
        ElapsedEstimatedWeeks = 42,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("43")]
        EstimatedMonths = 43,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("44")]
        ElapsedEstimatedMonths = 44,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("51")]
        EstimatedPercent = 51,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("52")]
        ElapsedEstimatedPercent = 52,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("53")]
        Empty = 53,
    }

    //TaskPredecessorLink.cs
    /// <summary>
    /// The link type
    /// </summary>
    public enum TaskLinkType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        FinishToFinish,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        FinishToStart,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        StartToFinish,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        StartToStart,
    }

    //TaskOutlineCode.cs
    /// <summary>
    /// Specifies whether the task has an associated deliverable or a dependency on an associated deliverable
    /// </summary>
    public enum TaskCommitmentType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        NoDeliverableOrDependencyOnDeliverable,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        HasAssociatedDeliverable,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        HasDependencyOnAssociatedDeliverable,
    }

    //Resource.cs
    /// <summary>
    /// Type of resource
    /// </summary>
    public enum ResourceType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Material,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Work,
    }

    /// <summary>
    /// Type of workgroup to which the resource belongs
    /// </summary>
    public enum ResourceWorkGroup
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Default,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        None,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Email,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Web,
    }

    /// <summary>
    /// How cost is accrued against the resource
    /// </summary>
    public enum ResourceAccrueAt
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Start = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        End = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Prorated = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Invalid = 4,
    }

    /// <summary>
    /// The booking type of the resource
    /// </summary>
    public enum BookingType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Committed,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Proposed,
    }

    //ResourceRate.cs
    /// <summary>
    /// The unique identifier of the rate table for the resource
    /// </summary>
    public enum RateTable
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        A,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        B,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        C,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        D,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        E,
    }

    /// <summary>
    /// The units used by Microsoft Project to display the overtime rate
    /// </summary>
    public enum ResourceOvertimeRateFormat
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Minutes = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Hours = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Days = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Weeks = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Months = 5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Years = 7,
    }

    /// <summary>
    /// The units used by Microsoft Project to display the standard rate
    /// </summary>
    public enum ResourceStandardRateFormat
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Undefined = 0,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Minutes = 1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Hours = 2,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Days = 3,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Weeks = 4,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Months = 5,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Years = 7,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        MaterialResourceRate = 8,
    }

    //Assignment.cs
    /// <summary>
    /// Indicates how work is to be distributed across the duration of the assignment
    /// </summary>
    public enum AssignmentWorkContour
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Flat,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        BackLoaded,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        FrontLoaded,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        DoublePeak,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        EarlyPeak,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        LatePeak,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Bell,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Turtle,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        Contoured,
    }

    /// <summary>
    /// The time unit for the usage rate of the material resource assignment
    /// </summary>
    public enum RateScale
    {
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        None,

        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Seconds,

        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Minutes,

        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Hours,

        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Days,

        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Weeks,

        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Months
    }

    public class Enum
    { }
}
