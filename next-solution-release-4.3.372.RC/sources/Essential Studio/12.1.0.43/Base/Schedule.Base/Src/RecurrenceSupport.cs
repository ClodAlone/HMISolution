//-------------------------------------------------------------------------------------------------
// <copyright file="RecurrenceSupport.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Schedule
{
    #region RecurrenceSupport class

    /// <summary>
    /// This is a helper class that exposes methods that allow you find dates that satisfy
    /// some rule that designate when recurring appointments happen. The rule may be something as
    /// simple as 'every month on 13' to indicate that the appointment should occur on the 13th
    /// day of each month. Other rules might be 'every year on Jan 13', or
    /// 'every year on weekday after Apr 15'. A recurrence rule may be defined either as a string
    /// (as with the three examples so far), or as an <see cref="RecurrenceRule"/>. More samples
    /// of each use case follows.
    /// </summary>
    /// <remarks>
    /// A recurrence rule has three main pieces; an Every clause, an On clause and an BeforeAfter
    /// clause. Each of the three main pieces also has an integer count property associated with
    /// it; EveryCount, OnCount and BeforeAfterCount. The RecurrenceRule class has public
    /// properties that reflect these values; Every, EveryCount, On, OnCount, BeforeAfter, and
    /// BeforeAfterCount. You may create a RecurrenceRule either by passing the constructor a
    /// well formaed string holding the rule, or you can use the default constructor and explicitly
    /// set the six properties mentioned above to define the RecurrenceRule.
    /// </remarks>
    [Serializable]
    public class RecurrenceSupport
    {
        private char countMarker = ':';

        private string daysOfWeekTokens = string.Empty;
        private string mDouble = "  "; ////2 MARKERS

        private string mark = " "; ////1 MARKER
        private char marker = ' ';

        private ArrayList parseableStrings;

        [ThreadStatic]
        private static char ruledDelimiter = ';';

        [ThreadStatic]
        private static string spanMarker = "x";

        private static RecurrenceSupport supportClass = null;
        private char tilde = '~';

        private ArrayList tokens_ = new ArrayList(new string[]
{
                        "a",  ////"EVERY"
                        "b",  ////"ON"
                        "c",  ////"AFTER"
                        "f",   ////DAY
                        "g",   ////"WEEK",
                        "h",   ////"MONTH",
                        "i",   ////"QUARTER",
                        "j",   ////"YEAR",
                        "$",   ////"WEEKDAY",
                        "%",   ////"WEEKEND",
                        "k",   ////"MON",
                        "l",   ////"TUE",
                        "m",   ////"WED",
                        "n",  ////"THU",
                        "o",  ////"FRI",
                        "p",  ////"SAT",
                        "q",    ////"SUN",
                        "r",    ////"JAN"
                        "s",    ////"FEB"
                        "t",    ////"MAR"
                        "u",    ////"APR"
                        "v",    ////"MAY"
                        "w",    ////"JUN"
                        "x",    ////"JUL"
                        "y",    ////"AUG"
                        "z",    ////"SEP"
                        "!",    ////"OCT"
                        "@",    ////"NOV"
                        "#"     ////"DEC"
        });
        private string validAfterTokens = "fghij$%klmnopqrstuvwxyz!@#";
        private string validEveryTokens = "fghij$%klmnopqrstuvwxyz!@#";
        private string validOnTokens = "$%klmnopqrstuvwxyz!@#";

        private ArrayList weekEndDays;

        /// <summary>
        /// Initializes a new instance of the RecurrenceSupport class.
        /// </summary>
        public RecurrenceSupport()
        {
            supportClass = this;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the character that is used to indicate a count parameter
        /// (an integer) is applied to the preceding value. Default value is colon (:).
        /// </summary>Use this character to delimit an object from a count.  For example,
        /// if you want to specify the second Wednesday of every month, then the recurrence
        /// definition could be represented as 'Every month On Wed:2'. The idea is that
        /// Wed:2 represents the second Wednesday.
        /// <remarks>
        /// </remarks>
        public char CountMarker
        {
            get { return countMarker; }
            set { countMarker = value; }
        }

        internal string DaysOfWeekTokens
        {
            get
            {
                if (daysOfWeekTokens == null || daysOfWeekTokens.Length == 0)
                {
                    daysOfWeekTokens = ((char)(int)ParseTokens.MON).ToString() + ((char)(int)ParseTokens.TUE).ToString() + ((char)(int)ParseTokens.WED).ToString()
                 + ((char)(int)ParseTokens.THU).ToString() + ((char)(int)ParseTokens.FRI).ToString() + ((char)(int)ParseTokens.SAT).ToString() + ((char)(int)ParseTokens.SUN).ToString();
                }

                return daysOfWeekTokens;
            }
        }

        /// <summary>
        /// Gets an ArrayList of strings that can be used to compose a text version of
        /// a recurrence definition.
        /// </summary>
        /// <remarks>This list is exposed in case you want to localize the strings
        /// used to define a recurrence definition in text. Note each string
        /// should be padded with a single space at each end. The strings are
        /// defined by the code below. When you replace them, the replacements must
        /// be in the same order.
        /// <code lang="C#">
        /// gridTableControl.UpdateWithCustomPaint(bounds, new PaintEventHandler(TableControl_CustomPaint));
        /// void TableControl_CustomPaint(object sender, PaintEventArgs e)
        /// {
        ///     Rectangle clipBounds = Rectangle.Truncate(e.Graphics.ClipBounds);
        ///     gridTableControl.DrawClippedGrid(e.Graphics, clipBounds, false);
        /// }
        /// </code>
        /// public ArrayList ParseableStrings
        /// {
        ///     get
        ///     {
        ///         if (parseableStrings == null)
        ///         {
        ///             parseableStrings = new ArrayList(new string[]{
        ///                 " EVERY ",
        ///                 " ON ",
        ///                 " AFTER ",
        ///                 " DAY ",
        ///                 " WEEK ",
        ///                 " MONTH ",
        ///                 " QUARTER ",
        ///                 " YEAR ",
        ///                 " WEEKDAY ",
        ///                 " WEEKEND ",
        ///                 " MON ",
        ///                 " TUE ",
        ///                 " WED ",
        ///                 " THU ",
        ///                 " FRI ",
        ///                 " SAT ",
        ///                 " SUN ",
        ///                 " JAN ",
        ///                 " FEB ",
        ///                 " MAR ",
        ///                 " APR ",
        ///                 " MAY ",
        ///                 " JUN ",
        ///                 " JUL ",
        ///                 " AUG ",
        ///                 " SEP ",
        ///                 " OCT ",
        ///                 " NOV ",
        ///                 " DEC "
        ///             });
        ///         }
        ///         return parseableStrings;
        ///      }
        ///  }
        /// </remarks>
        public ArrayList ParseableStrings
        {
            get
            {
                if (parseableStrings == null)
                {
                    parseableStrings = new ArrayList(new string[]
{
                        " EVERY ",
                        " ON ",
                        " AFTER ",
                        " DAY ",
                        " WEEK ",
                        " MONTH ",
                        " QUARTER ",
                        " YEAR ",
                        " WEEKDAY ",
                        " WEEKEND ",
                        " MON ",
                        " TUE ",
                        " WED ",
                        " THU ",
                        " FRI ",
                        " SAT ",
                        " SUN ",
                        " JAN ",
                        " FEB ",
                        " MAR ",
                        " APR ",
                        " MAY ",
                        " JUN ",
                        " JUL ",
                        " AUG ",
                        " SEP ",
                        " OCT ",
                        " NOV ",
                        " DEC "
                    });
                }

                return parseableStrings;
            }
        }

        /// <summary>
        /// Gets or sets the character that indicates a recurrence definition
        /// string has been parsed. Parsed strings begin with this character.
        /// Default is tilde (~).
        /// </summary>
        public char ParsedMarker
        {
            get { return tilde; }
            set { tilde = value; }
        }

        /// <summary>
        /// Gets or sets the character used to delimit multiple recurrence definitions
        /// in asingle string. Default is semicolon (;).
        /// </summary>
        public static char RuleDelimiter
        {
            get
            {
                if (ruledDelimiter == '\0')
                {
                    ruledDelimiter = ';';
                }

                return ruledDelimiter;
            }

            set
            {
                ruledDelimiter = value;
            }
        }

        /// <summary>
        /// Gets or sets the character used to delimit multiple recurrence definitions
        /// in asingle string. Default is semicolon (;).
        /// </summary>
        public static string SpanMarker
        {
            get
            {
                if (spanMarker == null || spanMarker.Length == 0)
                {
                    spanMarker = "x";
                }

                return spanMarker;
            }

            set
            {
                spanMarker = value;
            }
        }

        internal static RecurrenceSupport SupportClass
        {
            get
            {
                return supportClass;
            }
        }

        /// <summary>
        /// Gets or sets an arraylist that holds the days of the weeks that are considered the weekend.
        /// It defaults to two days: DayOfWeek.Saturday, DayOfWeek.Sunday.
        /// </summary>
        /// <remarks> If you modify this list, you must make sure you only add DayOfWeek
        /// enumerations to this list. The implementation expects this class to hold DayOfWeek values.
        /// </remarks>
        public ArrayList WeekEndDays
        {
            get
            {
                if (weekEndDays == null)
                {
                    weekEndDays = new ArrayList();
                    weekEndDays.Add(DayOfWeek.Saturday);
                    weekEndDays.Add(DayOfWeek.Sunday);
                }

                return weekEndDays;
            }

            set
            {
                weekEndDays = value;
            }
        }
        #endregion

        private DateTime FirstDayOfMonth(DateTime date)
        {
            return date.AddDays(-date.Day + 1);
        }

        private DateTime FirstDayOfQuarter(DateTime date)
        {
            int month = (date.Month - 1) % 3;
            if (month != 0)
            {
                date = date.AddMonths(-month);
            }

            return FirstDayOfMonth(date);
        }

        private DateTime FirstDayOfYear(DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        ////returns the integer staring at loc in parsedRule
        private int GetCount(ref int loc, string parsedRule)
        {
            int number = 0;

            while (loc < parsedRule.Length && char.IsDigit(parsedRule[loc]))
            {
                number = 10 * number + (int)(parsedRule[loc] - '0');
                loc++;
            }

            return number;
        }

        /// <summary>
        /// Locates the first date that satisfies the given recurrence definition that follows the
        /// given date.
        /// </summary>
        /// <param name="dt">The date where the search begins, returns as the newly located date.</param>
        /// <param name="rule">The RecurrenceRule that defines the recurrence.</param>
        /// <returns>True if a date was located and dt holds the next occurrence.
        /// </returns>
        /// <remarks>
        /// The usage is to call GetFirstDate to begin accumulating the dates that satisfiy the recurrence
        /// definition. After returning the first date, then use the GetNextDate method to find each addition date.
        /// Notice that GetFirstDate may find the passed in value (dt) satisfies the recurrence definition, but
        /// GetNextDate does not consider the passed in date when it does it search.
        /// </remarks>
        public bool GetFirstDate(ref DateTime dt, RecurrenceRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException("calcStruct cannot be null in this function call.");
            }

            return GetFirstDate(ref dt, string.Empty, ref rule);
        }

        /// <summary>
        /// Locates the first date that satisfies the given recurrence definition that follows the
        /// given date.
        /// </summary>
        /// <param name="dt">The date where the search begins and returns as the newly found date.</param>
        /// <param name="parsedExpression">A string holding the recurrence definition.</param>
        /// <param name="rule">The Recurrence object defines the recurrence.</param>
        /// <returns>True if a new recurrence date is located.</returns>
        /// <remarks> If rule is passed in as null, then parsedExpression must hold the
        /// recurrence defintion. If parsedExpression is passed in as empty, then rule must hold the
        /// recurrence definition.
        /// The usage is to call GetFirstDate to begin accumulating the dates that satisfiy the recurrence
        /// definition. After returning the first date, then use the GetNextDate method to find each addition date.
        /// Notice that GetFirstDate may find the passed in value (dt) satisfies the recurrence definition, but
        /// GetNextDate does not consider the passed in date when it does it search.
        /// The typical use case will not require the use of this method. Usually,
        /// you would define a <see cref="RecurrenceList"/> object, and work with that object and its
        /// IsValidRecurrence method to get recurrent dates.
        /// </remarks>
        public bool GetFirstDate(ref DateTime dt, string parsedExpression, ref RecurrenceRule rule)
        {
            bool b = true;
            if (rule == null)
            {
                rule = GetRecurrenceRule(parsedExpression);
            }

            DateTime newDate = dt;

            ////handle  frequecy first by moving it back so it can be incremented again below
            switch (rule.Every)
            {
                case ParseTokens.MONTH:
                    newDate = newDate.AddMonths(-Math.Max(1, rule.EveryCount));
                    break;
                case ParseTokens.WEEK:
                    newDate = newDate.AddDays(-7 * Math.Max(1, rule.EveryCount));
                    break;
                case ParseTokens.QUARTER:
                    newDate = newDate.AddMonths(-3 * Math.Max(1, rule.EveryCount));
                    break;
                case ParseTokens.YEAR:
                    newDate = newDate.AddYears(-Math.Max(1, rule.EveryCount));
                    break;
                case ParseTokens.WEEKEND:
                case ParseTokens.WEEKDAY:
                case ParseTokens.DAY:
                    newDate = newDate.AddDays(-Math.Max(1, rule.EveryCount));
                    break;
                default:
                    if (rule.Every == ParseTokens.NULL && rule.EveryCount > 0)
                    {
                        rule.On = ParseTokens.NULL;
                        rule.OnCount = rule.EveryCount;
                        rule.Every = ParseTokens.MONTH;
                        rule.EveryCount = 0;
                    }

                    break;
            }

            b = GetNextDate(ref newDate, ref rule);
            if (newDate.CompareTo(dt) < 0)
            {
                b = GetNextDate(ref newDate, ref rule);
            }

            dt = newDate;
            return b;
        }

        /// <summary>
        /// Get the next date that satisfies the recurrence definition.
        /// </summary>
        /// <param name="dt">The date after which the search begins. The newly found date
        /// is returned through this parameter.</param>
        /// <param name="parsedExpression">A parsed expression that defines the recurrence defintion.</param>
        /// <param name="rule">The recurrence definition.</param>
        /// <returns>True if a new date is located that satisfies the recurrence.</returns>
        public bool GetNextDate(ref DateTime dt, string parsedExpression, ref RecurrenceRule rule)
        {
            bool b = true;
            if (rule == null)
            {
                rule = GetRecurrenceRule(parsedExpression);
            }

            DateTime newDate = dt;

            ////handle  frequecy
            switch (rule.Every)
            {
                case ParseTokens.MONTH:
                    newDate = newDate.AddMonths(Math.Max(1, rule.EveryCount));
                    newDate = FirstDayOfMonth(newDate);
                    HandleBeforeAfter(ref newDate, rule);
                    HandleOn(ref newDate, rule);
                    break;
                case ParseTokens.WEEK:
                    newDate = newDate.AddDays(7 * Math.Max(1, rule.EveryCount));
                    HandleBeforeAfter(ref newDate, rule);
                    if ((char)rule.On != '\0' || rule.OnCount != 0)
                    {
                        HandleOn(ref newDate, rule);
                    }

                    break;
                case ParseTokens.QUARTER:
                    newDate = newDate.AddMonths(3 * Math.Max(1, rule.EveryCount));
                    newDate = FirstDayOfQuarter(newDate);
                    HandleBeforeAfter(ref newDate, rule);
                    HandleOn(ref newDate, rule);
                    break;
                case ParseTokens.YEAR:
                    newDate = newDate.AddYears(Math.Max(1, rule.EveryCount));
                    newDate = FirstDayOfYear(newDate);
                    HandleBeforeAfter(ref newDate, rule);
                    HandleOn(ref newDate, rule);
                    break;
                case ParseTokens.WEEKEND:
                    newDate = newDate.AddDays(Math.Max(1, rule.EveryCount));
                    while (!WeekEndDays.Contains(newDate.DayOfWeek))
                    {
                        newDate = newDate.AddDays(1);
                    }

                    HandleBeforeAfter(ref newDate, rule);
                    if ((char)rule.On != '\0')
                    {
                        HandleOn(ref newDate, rule);
                    }
                    else if (rule.OnCount > 0)
                    {
                        DateTime dt1 = new DateTime(newDate.Year, newDate.Month, rule.OnCount);

                        while (dt1 <= dt || !WeekEndDays.Contains(dt1.DayOfWeek))
                        {
                            dt1 = dt1.AddMonths(1);
                        }

                        ////if (dt1 <= dt)
                        //    dt1 = dt1.AddMonths(1);
                        ////if (WeekEndDays.Contains(dt1.DayOfWeek))
                        newDate = dt1;
                    }

                    break;
                case ParseTokens.WEEKDAY:
                    newDate = newDate.AddDays(Math.Max(1, rule.EveryCount));
                    while (WeekEndDays.Contains(newDate.DayOfWeek))
                    {
                        newDate = newDate.AddDays(1);
                    }

                    HandleBeforeAfter(ref newDate, rule);
                    if (rule.On != ParseTokens.NULL)
                    {
                        HandleOn(ref newDate, rule);
                    }
                    else if (rule.OnCount > 0)
                    {
                        while (true) ////there is a break inside
                        {
                            DateTime dt1 = newDate;
                            while (dt1.Day < rule.OnCount && dt1.Month == newDate.Month)
                            {
                                dt1 = dt1.AddDays(1);
                            }

                            if (!WeekEndDays.Contains(dt1.DayOfWeek) && dt1.Day == rule.OnCount)
                            {
                                newDate = dt1;
                                break;
                            }
                            else 
                            {
                                ////try next month
                                newDate = newDate.AddMonths(1);
                                newDate = new DateTime(newDate.Year, newDate.Month, rule.OnCount);
                            }
                        }
                    }

                    break;
                case ParseTokens.DAY:
                    newDate = newDate.AddDays(Math.Max(1, rule.EveryCount));
                    HandleBeforeAfter(ref newDate, rule);
                    if ((char)rule.On != '\0')
                    {
                        HandleOn(ref newDate, rule);
                    }

                    break;
                case ParseTokens.SUN:
                case ParseTokens.MON:
                case ParseTokens.TUE:
                case ParseTokens.WED:
                case ParseTokens.THU:
                case ParseTokens.FRI:
                case ParseTokens.SAT:
                    rule.On = rule.Every;
                    rule.OnCount = rule.EveryCount;
                    rule.EveryCount = 0;
                    rule.Every = ParseTokens.WEEK;

                    break;
                case ParseTokens.JAN:
                case ParseTokens.FEB:
                case ParseTokens.MAR:
                case ParseTokens.APR:
                case ParseTokens.MAY:
                case ParseTokens.JUN:
                case ParseTokens.JUL:
                case ParseTokens.AUG:
                case ParseTokens.SEP:
                case ParseTokens.OCT:
                case ParseTokens.NOV:
                case ParseTokens.DEC:
                    ////if (calcStruct.On != ParseTokens.NULL)
                    ////{
                    //    calcStruct.BeforeAfter = calcStruct.On;
                    //    calcStruct.BeforeAfterType = ParseTokens.AFTER;
                    //    calcStruct.BeforeAfterCount = calcStruct.OnCount;
                    ////}
                    rule.On = rule.Every;
                    rule.OnCount = rule.EveryCount;
                    rule.Every = ParseTokens.YEAR;
                    rule.EveryCount = 0;
                    break;
                default:
                    if (rule.Every == ParseTokens.NULL && rule.EveryCount > 0)
                    {
                        rule.On = ParseTokens.NULL;
                        rule.OnCount = rule.EveryCount;
                        rule.Every = ParseTokens.MONTH;
                        rule.EveryCount = 0;
                    }
                    else
                    {
                        throw new Exception("Invalid Every value");
                    }

                    break;
            }

            dt = newDate;
            return b;
        }

        /// <summary>
        /// Get the next date that satisfies the recurrence definition.
        /// </summary>
        /// <param name="dt">The date after which the search begins. The newly found date
        /// is returned through this parameter.</param>
        /// <param name="rule">The recurrence definition.</param>
        /// <returns>True if a new date is located that satisfies the recurrence.</returns>
        /// <remarks>The typical use case will not require the use of this method. Usually,
        /// you would define a <see cref="RecurrenceList"/> object, and work with that object and its
        /// IsValidRecurrence method to get recurrent dates.
        /// </remarks>
        public bool GetNextDate(ref DateTime dt, ref RecurrenceRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException("calcStruct cannot be null in this function call.");
            }

            return GetNextDate(ref dt, string.Empty, ref rule);
        }

        /// <summary>
        /// Returns a <see cref="RecurrenceRule"/> represented by the input text.
        /// </summary>
        /// <param name="text">A string holding a valid recurrence definition.</param>
        /// <returns>A RecurrenceRule.</returns>
        /// <remarks> If the input text holds multiple recurrence definitions (separated
        /// by semicolons), then the first definition is used to define the single RecurrenceRule
        /// that is returned.
        /// </remarks>
        public RecurrenceRule GetRecurrenceRule(string text)
        {
            int i = text.IndexOf(RuleDelimiter);
            string parsedRule = (i == -1) ? text : text.Substring(0, i);

            if (parsedRule.Length > 0 && parsedRule[0] != tilde)
            {
                parsedRule = ParseDefinition(parsedRule);
            }

            RecurrenceRule calcStruct = new RecurrenceRule(parsedRule);

            int loc = 1; ////skip Tilde
            while (loc < parsedRule.Length)
            {
                switch (parsedRule[loc])
                {
                    case (char)(int)ParseTokens.EVERY:////token_EVERYChar:
                        loc++; ////moveoff everyChar
                        if (!char.IsDigit(parsedRule[loc]))
                        {
                            calcStruct.Every = (ParseTokens)(int)parsedRule[loc++];
                        }

                        calcStruct.EveryCount = GetCount(ref loc, parsedRule);
                        break;
                    case (char)(int)ParseTokens.AFTER: ////token_AFTERChar:
                    case (char)(int)ParseTokens.BEFORE: ////token_BEFOREChar:
                        calcStruct.BeforeAfterType = (ParseTokens)(int)parsedRule[loc++]; ////save BEFORE/AFTER
                        calcStruct.BeforeAfter = (ParseTokens)(int)(!char.IsDigit(parsedRule[loc]) ? parsedRule[loc++] : '\0');
                        calcStruct.BeforeAfterCount = GetCount(ref loc, parsedRule);
                        break;
                    ////case token_INChar:
                    ////    loc++; //moveoff INChar
                    ////    calcStruct.In = parsedRule[loc++];
                    ////    calcStruct.InCount = GetCount(ref loc, parsedRule);
                    ////    break;
                    case (char)(int)ParseTokens.ON: ////token_ONChar:
                        loc++; ////move off ONChar...
                        ////increment loc below only if not a digit
                        calcStruct.On = (ParseTokens)(int)(!char.IsDigit(parsedRule[loc]) ? parsedRule[loc++] : '\0');
                        while (loc < parsedRule.Length && DaysOfWeekTokens.IndexOf(parsedRule[loc]) > -1)
                        {
                            loc++;
                        }

                        calcStruct.OnCount = GetCount(ref loc, parsedRule);
                        break;
                    default:
                        throw new Exception(string.Format("Unknown character in tokenized formula {0} at {1}.", parsedRule[loc], loc));
                }
            }

            if ((char)calcStruct.On == '\0')
            {
                switch (calcStruct.Every)
                {
                    case ParseTokens.SUN:
                    case ParseTokens.MON:
                    case ParseTokens.TUE:
                    case ParseTokens.WED:
                    case ParseTokens.THU:
                    case ParseTokens.FRI:
                    case ParseTokens.SAT:
                        calcStruct.On = calcStruct.Every;
                        calcStruct.OnCount = calcStruct.EveryCount;
                        calcStruct.Every = ParseTokens.WEEK;
                        calcStruct.EveryCount = 0;
                        break;
                    case ParseTokens.JAN:
                    case ParseTokens.FEB:
                    case ParseTokens.MAR:
                    case ParseTokens.APR:
                    case ParseTokens.MAY:
                    case ParseTokens.JUN:
                    case ParseTokens.JUL:
                    case ParseTokens.AUG:
                    case ParseTokens.SEP:
                    case ParseTokens.OCT:
                    case ParseTokens.NOV:
                    case ParseTokens.DEC:
                        calcStruct.On = calcStruct.Every;
                        calcStruct.OnCount = calcStruct.EveryCount;
                        calcStruct.Every = ParseTokens.YEAR;
                        calcStruct.EveryCount = 0;
                        break;
                    default:
                        break;
                }
            }

            return calcStruct;
        }

        /// <summary>
        /// Returns a string holding the recurrenced defined by the given RecurrenceRule.
        /// </summary>
        /// <param name="rule">The RecurrenceRule.</param>
        /// <returns>The string representing the recurrence definition.</returns>
        public string GetTextFromRecurrenceRule(RecurrenceRule rule)
        {
            string s = string.Empty;
            if (rule.Every != ParseTokens.NULL)
            {
                s += "EVERY " + rule.Every.ToString() + " ";
            }

            if (rule.EveryCount > 0)
            {
                if (rule.Every == ParseTokens.NULL)
                {
                    s += "EVERY ";
                }

                s += rule.EveryCount.ToString() + " ";
            }

            if (rule.On != ParseTokens.NULL)
            {
                s += "ON " + rule.On.ToString() + " ";
            }

            if (rule.OnCount > 0)
            {
                s += rule.OnCount.ToString() + " ";
            }

            if (rule.BeforeAfter != ParseTokens.NULL) 
            {
                s += "AFTER " + rule.BeforeAfter.ToString() + " ";
            }

            if (rule.BeforeAfterCount > 0)
            {
                s += rule.BeforeAfterCount.ToString() + " ";
            }

            return s;
        }

        ////handles the beforeafter caluse
        private void HandleBeforeAfter(ref DateTime dt, RecurrenceRule calcStruct)
        {
            switch (calcStruct.BeforeAfterType)
            {
                case ParseTokens.AFTER:
                    if ((char)calcStruct.BeforeAfter == '\0')
                    {
                        if (dt.Day > 1)
                        {
                            if (dt.Day < calcStruct.BeforeAfterCount)
                            {
                                dt = dt.AddDays(-dt.Day + 1);
                                dt = dt.AddDays(calcStruct.BeforeAfterCount - 1);
                            }
                        }
                        else
                        {
                            dt = dt.AddDays(calcStruct.BeforeAfterCount - 1);
                        }
                    }
                    else
                    {
                        bool isDay = true;
                        DayOfWeek day = DayOfWeek.Monday;
                        switch ((ParseTokens)calcStruct.BeforeAfter)
                        {
                            case ParseTokens.MON:
                                break;
                            case ParseTokens.TUE:
                                day = DayOfWeek.Tuesday;
                                break;
                            case ParseTokens.WED:
                                day = DayOfWeek.Wednesday;
                                break;
                            case ParseTokens.THU:
                                day = DayOfWeek.Thursday;
                                break;
                            case ParseTokens.FRI:
                                day = DayOfWeek.Friday;
                                break;
                            case ParseTokens.SAT:
                                day = DayOfWeek.Saturday;
                                break;
                            case ParseTokens.SUN:
                                day = DayOfWeek.Sunday;
                                break;
                            case ParseTokens.MONTH:
                                isDay = false;
                                dt = dt.AddMonths(Math.Max(calcStruct.BeforeAfterCount, 1) - 1);
                                break;
                            default: ////explicit month
                                isDay = false;
                                int mon = tokens_.IndexOf(((char)(int)calcStruct.BeforeAfter).ToString()) - tokens_.IndexOf(((char)(int)ParseTokens.JAN).ToString()) + 1;
                                if (mon > 0 && mon < 13)
                                    dt = new DateTime(dt.Year, mon, Math.Max(calcStruct.BeforeAfterCount, 1));
                                break;
                        }

                        if (isDay)
                        {
                            int offset = day - dt.DayOfWeek;
                            offset += 7 * ((Math.Max(calcStruct.BeforeAfterCount, 1) - 1) + ((offset < 0) ? 1 : 0));
                            dt = dt.AddDays(offset);
                        }
                    }

                    break;
                case ParseTokens.BEFORE:
                    throw new Exception("Not currently supported");
                ////break;
                default:
                    break;
            }
        }

        ////handles the On clause
        private void HandleOn(ref DateTime dt, RecurrenceRule calcStruct)
        {
            if ((char)calcStruct.On == '\0')
            {
                if (calcStruct.OnCount == 0)
                {
                    return; ////On clause missing
                }
                ////assumes dt is at firstday of month
                DateTime dt1 = dt.AddDays(calcStruct.OnCount - 1);
                if (dt1.Month > dt.Month)
                {
                    while (dt1.Month != dt.Month)
                    {
                        dt1 = dt1.AddDays(-1);
                    }
                }

                dt = dt1;
            }
            else
            {
                int month = -1;
                DayOfWeek day = DayOfWeek.Monday;
                switch ((ParseTokens)calcStruct.On)
                {
                    case ParseTokens.MON:
                        day = DayOfWeek.Monday;
                        break;
                    case ParseTokens.TUE:
                        day = DayOfWeek.Tuesday;
                        break;
                    case ParseTokens.WED:
                        day = DayOfWeek.Wednesday;
                        break;
                    case ParseTokens.THU:
                        day = DayOfWeek.Thursday;
                        break;
                    case ParseTokens.FRI:
                        day = DayOfWeek.Friday;
                        break;
                    case ParseTokens.SAT:
                        day = DayOfWeek.Saturday;
                        break;
                    case ParseTokens.SUN:
                        day = DayOfWeek.Sunday;
                        break;
                    case ParseTokens.JAN:
                        dt = new DateTime(dt.Year, 1, Math.Max(1, calcStruct.OnCount));
                        break;
                    case ParseTokens.WEEKDAY:
                        while (WeekEndDays.Contains(dt.DayOfWeek))
                        {
                            dt = dt.AddDays(1);
                        }

                        if (calcStruct.OnCount > 0)
                        {
                            int i = calcStruct.OnCount - 1;
                            while (i > 0)
                            {
                                dt = dt.AddDays(1);
                                if (!WeekEndDays.Contains(dt.DayOfWeek))
                                {
                                    i--;
                                }
                            }
                        }

                        month = -2;
                        break;
                    case ParseTokens.WEEKEND:
                        while (!WeekEndDays.Contains(dt.DayOfWeek))
                        {
                            dt = dt.AddDays(1);
                        }

                        if (calcStruct.OnCount > 0)
                        {
                            int i = calcStruct.OnCount - 1;
                            while (i > 0)
                            {
                                dt = dt.AddDays(1);
                                if (WeekEndDays.Contains(dt.DayOfWeek))
                                {
                                    i--;
                                }
                            }
                        }

                        month = -2;
                        break;
                    default:
                        month = tokens_.IndexOf(((char)(int)calcStruct.On).ToString()) - tokens_.IndexOf(((char)(int)ParseTokens.JAN).ToString()) + 1;
                        break;
                }

                if (month == -1)
                {
                    int offset = day - dt.DayOfWeek;
                    offset += 7 * ((Math.Max(calcStruct.OnCount, 1) - 1) + ((offset < 0) ? 1 : 0));
                    dt = dt.AddDays(offset);
                }
                else if (month > 0 && month < 13)
                {
                    dt = new DateTime(dt.Year, month, Math.Max(1, calcStruct.OnCount));
                }
            }
        }

        /// <summary>
        /// Determines if the passed AppointmentItem is a Span Item or not.
        /// </summary>
        /// <param name="item">Schedule Appointment.</param>
        /// <returns>True if the Schedule Appointment is Span Item, false otherwise.</returns>
        public static bool IsSpanItem(IScheduleAppointment item)
        {
            IRecurringScheduleAppointment item1 = item as IRecurringScheduleAppointment;
            bool isSpan = item1 != null && item1.RecurrenceRule.StartsWith(RecurrenceSupport.SpanMarker);
            ////if(isSpan && item1.DateList == null)
            //    System.Diagnostics.Debug.WriteLine("Schedule DataSource is of old version. This wont support latest functionalities for Span Item");
            return isSpan;
        }

        /// <summary>
        /// Checks whether the provided string holds a valid recurrence defintion.
        /// </summary>
        /// <param name="rule">A string containingth eproposed rule.</param>
        /// <returns>True if the rule is valid.</returns>
        public bool IsValidRule(string rule)
        {
            string errorMessage = string.Empty;
            return IsValidRule(rule, out errorMessage);
        }

        /// <summary>
        /// Checks whether the provided string holds a valid recurrence defintion.
        /// </summary>
        /// <param name="rule">A string containingth eproposed rule.</param>
        /// <param name="errorMessage">Returns an error message if teh rule is not valid.</param>
        /// <returns>True if the rule is valid.</returns>
        public bool IsValidRule(string rule, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool b = true;
            if (rule == null || rule.Length == 0)
            {
                errorMessage = "The rule cannot be an empty string.";
                b = false;
            }
            else
            {
                if (rule[0] != ParsedMarker)
                {
                    try
                    {
                        rule = ParseDefinition(rule);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        b = false;
                    }
                }

                if (b) 
                {
                    ////check for valid options
                    int loc = 1; ////skip tilde
                    while (b && loc < rule.Length)
                    {
                        if (rule[loc] == RuleDelimiter)
                        {
                            loc++;
                            if (loc < rule.Length && rule[loc] == ParsedMarker)
                            {
                                loc++;
                            }
                        }
                        else
                        {
                            switch (rule[loc])
                            {
                                case (char)(int)ParseTokens.EVERY:
                                    loc++;
                                    if (loc == rule.Length)
                                    {
                                        errorMessage = string.Format("Missing Every clause.");
                                        b = false;
                                    }
                                    else if (validEveryTokens.IndexOf(rule[loc]) > -1)
                                    {
                                        loc++;
                                    }

                                    if (b && loc < rule.Length && char.IsDigit(rule[loc]))
                                    {
                                        int val = GetCount(ref loc, rule);
                                    }

                                    break;
                                case (char)(int)ParseTokens.ON:
                                    loc++;
                                    if (loc == rule.Length)
                                    {
                                        errorMessage = string.Format("Missing On clause.");
                                        b = false;
                                    }
                                    else if (validOnTokens.IndexOf(rule[loc]) > -1)
                                    {
                                        loc++;
                                    }

                                    if (b && loc < rule.Length && char.IsDigit(rule[loc]))
                                    {
                                        int val = GetCount(ref loc, rule);
                                    }

                                    break;

                                case (char)(int)ParseTokens.AFTER:
                                    loc++;
                                    if (loc == rule.Length)
                                    {
                                        errorMessage = string.Format("Missing After clause.");
                                        b = false;
                                    }
                                    else if (validAfterTokens.IndexOf(rule[loc]) > -1)
                                    {
                                        loc++;
                                    }

                                    if (b && loc < rule.Length && char.IsDigit(rule[loc]))
                                    {
                                        int val = GetCount(ref loc, rule);
                                    }

                                    break;
                                default:
                                    errorMessage = string.Format("illegal character: '{0}'", rule[loc]);
                                    b = false;
                                    break;
                            }
                        }
                    }
                }
            }

            return b;
        }

        /// <summary>
        /// Returns a tokenized version of the recurrence definition represented by text.
        /// </summary>
        /// <param name="text">The text of the definition. eg. Every month On Wed After 15</param>
        /// <returns>A tokenized string.</returns>
        public string ParseDefinition(string text)
        {
            StringBuilder sb = new StringBuilder(marker + text.ToUpper() + marker);

            sb = sb.Replace(RuleDelimiter.ToString(), " " + RuleDelimiter.ToString() + " ");
            sb = sb.Replace(CountMarker, marker);
            sb = sb.Replace(',', marker).Replace('(', marker).Replace(')', marker).Replace(' ', marker).Replace(mark, mDouble);

            for (int i = 0; i < ParseableStrings.Count; ++i)
            {
                sb = sb.Replace(ParseableStrings[i].ToString(), tokens_[i].ToString());
            }

            text = sb.ToString();
            while (text.IndexOf(mark) > -1)
            {
                text = text.Replace(mark, string.Empty);
            }

            text = text.Replace(RuleDelimiter.ToString(), RuleDelimiter.ToString() + ParsedMarker);

            return ParsedMarker + text;
        }

        /// <summary>
        /// Accepts a tokenized string representing a recurrence definition and returns a non-tokenized version.
        /// </summary>
        /// <param name="parsedDefintion">Tokenized string</param>
        /// <returns>Non tokenized string.</returns>
        public string UnparseDefinition(string parsedDefintion)
        {
            string expression = string.Empty;
            foreach (string s in parsedDefintion.Split(new char[] { RuleDelimiter }))
            {
                if (expression.Length > 0)
                {
                    expression += RuleDelimiter;
                }

                expression += UnparseSimpleDefinition(s);
            }

            return expression;
        }

        private string UnparseSimpleDefinition(string parsedDefintion)
        {
            string expression = string.Empty;

            int loc = 1; ////skip the ParsedMarker
            while (loc < parsedDefintion.Length)
            {
                int tokeIndex = tokens_.IndexOf(parsedDefintion[loc].ToString());
                if (tokeIndex > -1)
                {
                    expression += ParseableStrings[tokeIndex].ToString();
                    loc++;
                }
                else
                {
                    ////should be digits
                    string num = string.Empty;
                    while (loc < parsedDefintion.Length && char.IsDigit(parsedDefintion[loc]))
                    {
                        num += parsedDefintion[loc];
                        loc++;
                    }

                    if (num.Length > 0)
                    {
                        expression += string.Format(":{0} ", num);
                    }
                    else
                    {
                        throw new Exception(string.Format("invalid character '{0}' at {1}", parsedDefintion[loc], loc));
                    }
                }
            }

            string s = mark + countMarker;
            expression = expression.Replace(s, countMarker.ToString());
            s = ParseTokens.ON.ToString() + countMarker.ToString();
            string s1 = s.Replace(countMarker, marker);
            expression = expression.Replace(s, s1);
            s = ParseTokens.AFTER.ToString() + countMarker.ToString();
            s1 = s.Replace(countMarker, marker);
            expression = expression.Replace(s, s1);
            s = ParseTokens.BEFORE + countMarker.ToString();
            ////s = ParseableStrings[token_ON].ToString().TrimEnd() + COUNTMARKER.ToString();
            ////string s1 = s.Replace(COUNTMARKER, MARKER);
            ////expression = expression.Replace(s, s1);
            ////s = ParseableStrings[token_AFTER].ToString().TrimEnd() + COUNTMARKER.ToString();
            ////s1 = s.Replace(COUNTMARKER, MARKER);
            ////expression = expression.Replace(s, s1);
            ////s = ParseableStrings[token_BEFORE].ToString().TrimEnd() + COUNTMARKER.ToString();
            s1 = s.Replace(countMarker, marker);
            expression = expression.Replace(s, s1);

            return expression;
        }
    }
    #endregion

    #region ParseTokens enum
    /// <summary>
    /// Represents the various pieces that go into making a valid rule that defines a set of recurring dates.
    /// </summary>
    public enum ParseTokens
    {
        /// <summary>
        /// Indicates the beginning of an Every clause.
        /// </summary>
        EVERY = 'a',
        
        /// <summary>
        /// Indicates the beginning of an On clause.
        /// </summary>
        ON = 'b',
        
        /// <summary>
        /// Indicates the beginning of an BeforeAfter clause.
        /// </summary>
        AFTER = 'c',
        
        /// <summary>
        /// Indicates the beginning of an BeforeAfter clause. Not currently used.
        /// </summary>
        BEFORE = 'd',
        
        /// <summary>
        /// Indicates the beginning of an In clause. Not currently used.
        /// </summary>
        IN = 'e',
        
        /// <summary>
        /// Represents a Day.
        /// </summary>
        DAY = 'f',
        
        /// <summary>
        /// Represents a Week.
        /// </summary>
        WEEK = 'g',
        
        /// <summary>
        /// Represents a Month.
        /// </summary>
        MONTH = 'h',
        
        /// <summary>
        /// Represents a Quarter.
        /// </summary>
        QUARTER = 'i',
        
        /// <summary>
        /// Represents a Year.
        /// </summary>
        YEAR = 'j',
        
        /// <summary>
        /// Represents a Week Day.
        /// </summary>
        WEEKDAY = '$',
        
        /// <summary>
        /// Represents a Week End.
        /// </summary>
        WEEKEND = '%',
        
        /// <summary>
        /// Represents a Monday.
        /// </summary>
        MON = 'k',
        
        /// <summary>
        /// Represents a Tuesday.
        /// </summary>
        TUE = 'l',
        
        /// <summary>
        /// Represents a Wednesday.
        /// </summary>
        WED = 'm',
        
        /// <summary>
        /// Represents a Thursday.
        /// </summary>
        THU = 'n',
        
        /// <summary>
        /// Represents a Friday.
        /// </summary>
        FRI = 'o',
        
        /// <summary>
        /// Represents a Saturday.
        /// </summary>
        SAT = 'p',
        
        /// <summary>
        /// Represents a Sunday.
        /// </summary>
        SUN = 'q',
        
        /// <summary>
        /// Represents January.
        /// </summary>
        JAN = 'r',
        
        /// <summary>
        /// Represents February.
        /// </summary>
        FEB = 's',
        
        /// <summary>
        /// Represents March.
        /// </summary>
        MAR = 't',
        
        /// <summary>
        /// Represents April.
        /// </summary>
        APR = 'u',
        
        /// <summary>
        /// Represents May.
        /// </summary>
        MAY = 'v',
        
        /// <summary>
        /// Represents June.
        /// </summary>
        JUN = 'w',
        
        /// <summary>
        /// Represents July.
        /// </summary>
        JUL = 'x',
        
        /// <summary>
        /// Represents August.
        /// </summary>
        AUG = 'y',
        
        /// <summary>
        /// Represents September.
        /// </summary>
        SEP = 'z',
        
        /// <summary>
        /// Represents October.
        /// </summary>
        OCT = '!',
        
        /// <summary>
        /// Represents November.
        /// </summary>
        NOV = '@',
        
        /// <summary>
        /// Represents December.
        /// </summary>
        DEC = '#',
        
        /// <summary>
        /// Represents no entry.
        /// </summary>
        NULL = '\0'
    }

    #endregion

    #region RecurrenceList class

    /// <summary>
    /// This class encapsulates the idea of a recurrence defintion. Its purpose is to accept a
    /// recurrence definition (either as a string or as RecurrenceRules) and allow you to easily
    /// decide whether particular date satisfies the defineition of recurrence. The class derives from
    /// ArrayList. The ArayList may (depending upon <see cref="EnableDateCache"/>) hold a list
    /// of known valid recurrent dates. This list is dynamic, and will grow as you use the method
    /// <see cref="IsValidRecurrence"/> to determine whether a date satisfies the definition of
    /// recurrence.
    /// </summary>
    public class RecurrenceList : ArrayList
    {
        private IScheduleAppointment appointment = null;

        private DateTime baseDate;

        private bool enableDateCache = true;

        private RecurrenceRule[] rules;

        private string ruleString;

        private DateTime terminalDate = DateTime.MaxValue;
        private char tilde = '~';

        /// <summary>
        /// Initializes a new instance of the RecurrenceList class.
        /// </summary>
        public RecurrenceList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RecurrenceList class. Accepts a string rule and a basedate to define the recurrence.
        /// </summary>
        /// <param name="rule">The string rule. <see cref="Syncfusion.Schedule.RecurrenceRule.Text"/></param>
        /// <param name="baseDate">The base date.</param>
        /// <param name="appointment">Defines the appointment associated with this recurrence.</param>
        public RecurrenceList(string rule, DateTime baseDate, IScheduleAppointment appointment)
        {
            if (rule == null || rule.Length == 0)
            {
                throw new ArgumentException("must pass a non-empty parsed rule string.");
            }

            if (RecurrenceSupport.SupportClass == null)
            {
                new RecurrenceSupport();
            }

            ////  throw new ArgumentException("must instantiate an instance of RecurrenceSupport before using this class.");
            if (rule.IndexOf(tilde) == -1)
            {
                rule = RecurrenceSupport.SupportClass.ParseDefinition(rule);
            }

            this.appointment = appointment;
            this.ruleString = rule;
            this.baseDate = baseDate;

            if (rule.IndexOf(RecurrenceSupport.RuleDelimiter) > -1)
            {
                string[] ruleStrings = rule.Split(new char[] { RecurrenceSupport.RuleDelimiter });
                rules = new RecurrenceRule[ruleStrings.GetLength(0)];
                for (int i = 0; i < ruleStrings.GetLength(0); ++i)
                {
                    rules[i] = RecurrenceSupport.SupportClass.GetRecurrenceRule(ruleStrings[i]);
                }
            }
            else
            {
                rules = new RecurrenceRule[1];
                rules[0] = RecurrenceSupport.SupportClass.GetRecurrenceRule(rule);
            }
        }

        /// <summary>
        /// Initializes a new instance of the RecurrenceList class. Uses a RecurenceRule and BaseDate to define the recurrence.
        /// </summary>
        /// <param name="rule">The recurrence rule.</param>
        /// <param name="baseDate">The basedate.</param>
        /// <param name="appointment">Defines the appointment associated with this recurrence.</param>
        public RecurrenceList(RecurrenceRule rule, DateTime baseDate, IScheduleAppointment appointment)
        {
            if (rule == null)
            {
                throw new ArgumentException("must pass a non-null rule.");
            }

            if (RecurrenceSupport.SupportClass == null)
            {
                throw new ArgumentException("must instantiate an instance of RecurrenceSupport before using this class.");
            }

            this.appointment = appointment;
            rules = new RecurrenceRule[] { rule };
            this.baseDate = baseDate;
            this.ruleString = RecurrenceSupport.SupportClass.GetTextFromRecurrenceRule(rule);
        }

        /// <summary>
        /// Initializes a new instance of the RecurrenceList class. Accepts a fixed list of dates.
        /// </summary>
        /// <param name="dates">An Array of DateTime values.</param>
        /// <param name="appointment">Defines the appointment associated with this recurrence.</param>
        /// <remarks>
        /// This constructor allows you to specify a recurrence as a fix set of dates. So,
        /// if you want to set up a series of 5 meetings on 5 different dates, then
        /// you can use this constructor to do so as a recurring appointment.
        /// </remarks>
        public RecurrenceList(DateTime[] dates, IScheduleAppointment appointment)
        {
            if (dates == null || dates.GetLength(0) == 0)
            {
                throw new ArgumentException("dates must contain some entries.");
            }

            this.appointment = appointment;
            for (int i = 0; i < dates.GetLength(0); ++i)
            {
                dates[i] = dates[i].Date;
            }

            this.AddRange(dates);
            this.Sort();
            this.baseDate = this[0];
        }

        /// <summary>
        /// A property that gets/sets appointment information for this recurring event.
        /// </summary>
        /// <remarks>When a Calendar is displaying a date that holds a recurring
        /// appointment, the information provided through this property will
        /// be used to create a placeholder appointment for this recurring event.
        /// </remarks>
        public IScheduleAppointment Appointment
        {
            get { return appointment; }
            set { appointment = value; }
        }

        /// <summary>
        /// Gets or sets the start date for this recurrence.
        /// </summary>
        /// <remarks>
        /// This value should be the earliest possible date that you want to be considered
        /// for this set of recurrences.
        /// </remarks>
        public DateTime BaseDate
        {
            get { return baseDate; }
            set { baseDate = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this RecurrencList maintains a list of dates that satisfies
        /// the recurrence definition.
        /// </summary>
        /// <remarks>
        /// If this property is true, the ReccurenceList is an ArrayList object hodling a sorted
        /// list of the known occurrences to date. The list is fully populated in the sense that
        /// all dates between the first date in that list and the last date in the list which
        /// satisfy the recurrence definintion are also in the list. This list is used in the
        /// <see cref="IsValidRecurrence"/> method to check for occurences. This method first
        /// checks whether the requested date is in the list which means the method returns true.
        /// If not, it checks to see if the requested date is less than teh last date in the list,
        /// and if so, returns false. Finally, if the requested date is beyond the dates in the list,
        /// the method computes new dates that satisfy the rule and adds them to the list until
        /// ether the requested date is found, or exceeded. This means that the list dynamically
        /// grows as you request further and further dates into the future.
        /// This dynamic list technique makes validating dates to be very efficient at the expense
        /// of the memory load of maintaining the date list.
        /// </remarks>
        public bool EnableDateCache
        {
            get { return enableDateCache; }
            set { enableDateCache = value; }
        }

        /// <summary>
        /// Gets or sets an array of <see cref="RecurrenceRule"/>s that defines this recurrence collection.
        /// </summary>
        public RecurrenceRule[] Rules
        {
            get
            {
                return rules;
            }

            set
            {
                rules = value;
            }
        }

        /// <summary>
        /// Gets or sets a string that defines this recurrence collection.
        /// </summary>
        /// <remarks>The string may be a simple rule that qualifies (<see cref="RecurrenceRule.Text"/>
        /// for sample rules). It also can be several simple rules appended together with
        /// semicolons. This RuleString is equivalent to the collection of <see cref="Rules"/>
        /// in that both RuleString and Rules define exactly the same set of recurrent dates.
        /// </remarks>
        public string RuleString
        {
            get
            {
                return ruleString;
            }

            set
            {
                ruleString = value;
            }
        }

        /// <summary>
        /// Gets or sets the last date associated with this recurrence definition.
        /// </summary>
        public DateTime TerminalDate
        {
            get { return terminalDate; }
            set { terminalDate = value; }
        }

        /// <summary>
        /// Gets of sets the ith date in this RecurrentList.
        /// </summary>
        /// <param name="i">The requested index.</param>
        /// <returns>A date satisfying this recurrence.</returns>
        public new DateTime this[int i]
        {
            get
            {
                if (i < 0 || i >= base.Count || base[i] == null)
                {
                    throw new ArgumentException("Invalid index value.");
                }

                return (DateTime)base[i];
            }

            set
            {
                if (i > -1 && i < base.Count)
                {
                    base[i] = value;
                }
            }
        }

        /// <summary>
        /// Returns whether the given date satisfies the recurrence definition.
        /// </summary>
        /// <param name="dt">The date to be tested.</param>
        /// <returns>True is the date satisfies the recurrence definition.</returns>
        /// <remarks>If <see cref="EnableDateCache"/> is true, then this routine first
        /// does a binary search on the dates that are cached in the underlying ArrayList.
        /// If the date is not found and exceeds the last cached date, then
        /// Syncfusion.Schedule.RecurrenceSupport.GetFirstDate is called with a base date of the
        /// last known cached recurrence date. Then GetNextDate is continually called until
        /// either the given date is found to satisfy the recurrence, or the given date has
        /// been exceeded. As the dates are generated, they are inserted into the underlying
        /// ArrayList, extending the known date cache. Care is taken to insert the new dates
        /// properly so the underlying date cache is always sorted, and BinarySearch is valid.
        /// If EnableDateCache is false, then no searching is attempted in the cache which is
        /// always empty. Instead, GetFirstDate is always called with the BaseDate, and the
        /// GetNextDate is called until the given date is found or exceeded. There is no
        /// cacheing involved and all checks require starting at the beginning and computing
        /// all the dates from scratch everytime.
        /// </remarks>
        public bool IsValidRecurrence(DateTime dt)
        {
            bool b = false;
            //// if (EnableDateCache && this.Contains(dt.Date)) //linear code
            if (EnableDateCache && this.BinarySearch(dt.Date) > -1)
            {
                b = true;
            }
            else if (Count > 0 && this[Count - 1] > dt)
            {
                b = false;
            }
            else if (Rules == null || Rules.GetLength(0) == 0)
            {
                b = false;
            }
            else
            {
                DateTime basedate = this.Count == 0 ? BaseDate : this[Count - 1];
                DateTime terminalDate = dt.Date; //// endDate;
                int index = 0;
                foreach (RecurrenceRule rule1 in Rules)
                {
                    DateTime d = baseDate.Date;
                    RecurrenceRule rule = rule1;
                    if (RecurrenceSupport.SupportClass.GetFirstDate(ref d, rule))
                    {
                        d = d.Date;
                        int loc = -1;
                        ////if (EnableDateCache && !this.Contains(d)) //linear code
                        if (EnableDateCache && (loc = this.BinarySearch(d)) < 0)
                        {
                            //// this.Add(d);  //linear code
                            this.Insert(-(loc + 1), d);
                        }

                        if (d <= terminalDate)
                        {
                            if (dt.Date == d.Date)
                            {
                                b = true;
                                terminalDate = d;
                                continue;
                            }
                        }

                        if (d < terminalDate)
                        {
                            while (RecurrenceSupport.SupportClass.GetNextDate(ref d, ref rule))
                            {
                                d = d.Date;
                                ////if (EnableDateCache && !this.Contains(d)) //linear code
                                if (EnableDateCache && (loc = this.BinarySearch(d)) < 0)
                                {
                                    // this.Add(d); //linear code
                                    this.Insert(-(loc + 1), d);
                                }

                                if (d <= terminalDate)
                                {
                                    if (dt.Date == d.Date)
                                    {
                                        b = true;
                                        terminalDate = d;
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    index++;
                }
            }

            return b;
        }
    }

    #endregion

    #region RecurrenceRule class

    /// <summary>
    /// The RecurrenceRule class holds the definition of a simple recurrence. There are two representations
    /// available for the recurrence definition. One is a simple text string that hold words defining the
    /// recurrence. The property Text holds this text string. The other representation is based on the
    /// <see cref="ParseTokens"/> enumerations along with some supporting integer properties. The enumerations
    /// are Every, On, and BeforeAfter. The supporting properties are EveryCount, OnCount and BeforeAfterCount.
    /// </summary>
    /// <remarks>It is important to understand that there are two ways to define a set of recurrences
    /// in a RecurrenceRule object. One is through the Text property. The other is through the ParseToken
    /// enumeration properties and their associated integer properties. The Text definition is easier to
    /// serialize and present to the user. The enumeration properties allow for straight-forward calculation
    /// support.
    /// </remarks>
    public class RecurrenceRule
    {
        private ParseTokens beforeAfter;

        private int beforeAfterCount = 0;

        private ParseTokens beforeAfterType;

        private ParseTokens every;

        private int everyCount = 0;

        private ParseTokens on;

        private int onCount = 0;
        private string text;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RecurrenceRule class. Accepts a string used in the recurrence defintion.
        /// </summary>
        /// <param name="text">The string that defines this recurrence.</param>
        public RecurrenceRule(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Initializes a new instance of the RecurrenceRule class.
        /// </summary>
        public RecurrenceRule()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets after clause for this rule.
        /// </summary>
        /// <remarks>
        /// The valid values for this property are:
        ///        MON
        ///        TUE
        ///        WED
        ///        THU
        ///        FRI
        ///        SAT
        ///        SUN
        ///        JAN
        ///        FEB
        ///        MAR
        ///        APR
        ///        MAY
        ///        JUN
        ///        JUL
        ///        AUG
        ///        SEP
        ///        OCT
        ///        NOV
        ///        DEC
        ///        NULL
        /// <para></para>
        /// If BeforeAfter has a value of ParseToken.NULL, then BeforeAfterCount should be positive and
        /// represent a date. For example, the string "Every month on Wed After 15" would pick out the
        /// first Wed of every month on or after the 15th of the month. In this case, the enumeration
        /// representation of this recurrence definition would have BeforeAfter = ParseToken.NULL and
        /// BeforeAfterCount = 15.
        /// Also, this is a required entry when specifying a recurrence definition.
        /// </remarks>
        public ParseTokens BeforeAfter
        {
            get { return beforeAfter; }
            set { beforeAfter = value; }
        }

        /// <summary>
        /// Gets or sets an integer value used in conjunction with the BeforeAfter property to determine the After clause of the
        /// definition of this recurrence. So, if Text = "Every month After Wed:2", then this would tranlate into
        /// the BeforeAfter = ParseToken.WED and BeforeAfterCount = 2.
        /// </summary>
        public int BeforeAfterCount
        {
            get { return beforeAfterCount; }
            set { beforeAfterCount = value; }
        }

        /// <summary>
        /// Gets or sets the BeforeAfterType which is not currently being used.
        /// </summary>
        /// <remarks>For the current implementation, this property should always have the value of
        /// ParseTokens.After.</remarks>
        public ParseTokens BeforeAfterType
        {
            get { return beforeAfterType; }
            set { beforeAfterType = value; }
        }

        /// <summary>
        /// Gets or sets Every clause for this rule.
        /// </summary>
        /// <remarks>
        /// The valid values for this property are:
        ///        DAY
        ///        WEEK
        ///        MONTH
        ///        QUARTER
        ///        YEAR
        ///        WEEKDAY
        ///        WEEKEND
        ///        MON
        ///        TUE
        ///        WED
        ///        THU
        ///        FRI
        ///        SAT
        ///        SUN
        ///        JAN
        ///        FEB
        ///        MAR
        ///        APR
        ///        MAY
        ///        JUN
        ///        JUL
        ///        AUG
        ///        SEP
        ///        OCT
        ///        NOV
        ///        DEC
        ///        NULL
        /// If Every = ParseTokens.NULL, then EveryCount must be positive. In this case, the EveryCount represents a
        /// date. So, the string Text = "Every 21" would result in Every = ParseTokens.NULL and EveryCount = 21. The
        /// recurrence definition would represent the 21st of every month.
        /// Please note that the month entries, normally have a non-zero value set for EveryCount.
        /// Also, this is a required entry when specifying a recurrence definition.
        /// </remarks>
        public ParseTokens Every
        {
            get { return every; }
            set { every = value; }
        }

        /// <summary>
        /// Gets or sets an integer value used in conjunction with the Every property to determine the Every clause of the
        /// definition of this recurrence. So, if Text = "Every Jan 20", then this would translate into
        /// the Every = ParseToken.JAN and EveryCount = 20.
        /// </summary>
        public int EveryCount
        {
            get { return everyCount; }
            set { everyCount = value; }
        }

        /// <summary>
        /// A property that gets/sets on clause for this rule.
        /// </summary>
        /// <remarks>
        /// The valid values for this property are:
        ///        WEEKDAY
        ///        WEEKEND
        ///        MON
        ///        TUE
        ///        WED
        ///        THU
        ///        FRI
        ///        SAT
        ///        SUN
        ///        JAN
        ///        FEB
        ///        MAR
        ///        APR
        ///        MAY
        ///        JUN
        ///        JUL
        ///        AUG
        ///        SEP
        ///        OCT
        ///        NOV
        ///        DEC
        /// </remarks>
        public ParseTokens On
        {
            get { return on; }
            set { on = value; }
        }

        /// <summary>
        /// Gets or sets an integer value used in conjunction with the On property to determine the On clause of the
        /// definition of this recurrence. So, if Text = "Every month On Wed:2", then this would tranlate into
        /// the On = ParseToken.WED and OnCount = 2.
        /// </summary>
        public int OnCount
        {
            get { return onCount; }
            set { onCount = value; }
        }

        /// <summary>
        /// A property that gets a string representation of a recurrence definition.
        /// </summary>
        /// <remarks>Here are some examples:
        /// 1)      Every Month On 13
        /// 2)      Every Month On Wed:2
        /// 3)      Every Year On Apr 15
        /// 4)      Every Apr 15
        /// 5)      Every Year On weekday After Apr 15
        /// 6)      Every Year On Tue After Apr 15
        /// 7)      Every Week on Thu
        /// 8)      Every Week:2 On Wed
        /// 9)      Every Week On Thu
        /// 10)     Every Month On 13
        /// <para></para> Many of the example strings are self explanatory. In the second example, the Wed:2 indicates
        /// the second Wed. In the 8th example, Week:2 indicates every 2 weeks. Examples 3 and 4 define
        /// exactly the same recurrence. Examples 5 and 6 have After clauses in addition to Every and On
        /// clauses.
        /// From the examples, you can see that a Text rule that defines a recurrence can contain an
        /// Every clause, an On clause and an After clause. The only piece that is required is the
        /// Every clause (as seen in example 4). The order and case of these clause does not matter. So,
        /// On 'Apr 15 Every Year' is valid and the same as 'EVERY year ON apr 15'. See the indivdual
        /// ParseToken properties (Every, On and BeforeAfter) for a discussion of the valid forms of these
        /// clauses.
        /// The words used in a Text definition of a recurrence rule can be localized through the
        /// <see cref="RecurrenceSupport.ParseableStrings"/> property.
        /// </remarks>
        public string Text
        {
            get { return text; }
        }
        #endregion

        private string HandleNull(ParseTokens c)
        {
            return c == ParseTokens.NULL ? "null" : c.ToString();
        }

        /// <summary>
        /// An Overridden method that returns a string holding the RecurrenceRule object.
        /// </summary>
        /// <returns>String representation of this object.</returns>
        public override string ToString()
        {
            return string.Format(
                "{0}{1}EVERY= {2} {3}{1}BEFOREAFTER= {4} {5} {8}{1} ON= {6} {7}",
                            text,
                            Environment.NewLine,
                            HandleNull(every),
                            everyCount,
                            HandleNull(beforeAfter),
                            beforeAfterCount,
                            HandleNull(on),
                            onCount,
                            HandleNull(beforeAfterType));
        }
    }
    #endregion
}
