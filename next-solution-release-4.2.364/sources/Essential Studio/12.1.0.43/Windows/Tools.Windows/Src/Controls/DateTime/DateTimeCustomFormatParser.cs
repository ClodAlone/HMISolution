#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// DateTime CustomFormatParser.
    /// </summary>
    public class DateTimeCustomFormatParser
    {
        private static string pattern = null;
        private static string[] groupNames = { "Day", "Hour", "Minute", "Month", "Seconds", "AMPM", "Year", "String" };
        private static Regex regex = null;

        private DateTimeCustomFormatParser()
        { 
        }

        static DateTimeCustomFormatParser()
        {
            string dayExpression = @"(?<Day>[d]{1,4})";

            string hourExpresion = @"(?<Hour>[hH]{1,2})";

            string minuteExpresion = @"(?<Minute>[m]{1,2})";

            string monthExpresion = @"(?<Month>[M]{1,4})";

            string secondsExpresion = @"(?<Seconds>[s]{1,2})";

            string ampmExpresion = @"(?<AMPM>[t]{1,2})";

            string yearExpresionOne = @"(?<Year>[y]{1,2})";

            string yearExpresionTwo = @"(?<Year>[y]{4})";

            string stringExpressionOne = @"('(?<String>.*?)')";

            string stringExpressionTwo = @"(?<String>.+?)";

            DateTimeCustomFormatParser.pattern = "(" +
                                                            dayExpression + "|" +
                                                            hourExpresion + "|" +
                                                            minuteExpresion + "|" +
                                                            monthExpresion + "|" +
                                                            secondsExpresion + "|" +
                                                            ampmExpresion + "|" +
                                                            yearExpresionTwo + "|" +
                                                            yearExpresionOne + "|" +
                                                            stringExpressionOne + "|" +
                                                            stringExpressionTwo
                                                        + ")";

            DateTimeCustomFormatParser.regex = new Regex(DateTimeCustomFormatParser.pattern, RegexOptions.Compiled);
        }

        public static FieldDefinitionCollection Parse(string s)
        {
            MatchCollection matches = DateTimeCustomFormatParser.regex.Matches(s);

            FieldDefinitionCollection fdc = new FieldDefinitionCollection();

            foreach (Match match in matches)
            {
                FieldDefinition df = DateTimeCustomFormatParser.GetFieldDefinition(match.Groups);
                fdc.Add(df);
            }

            return fdc;
        }

        private static FieldDefinition GetFieldDefinition(GroupCollection gc)
        {
            foreach (string groupName in DateTimeCustomFormatParser.groupNames)
            {
                if (gc[groupName].Success == true)
                    return DateTimeCustomFormatParser.CreateFieldType(groupName, gc[groupName].Value);
            }

            throw new FormatException();
        }

        private static FieldDefinition CreateFieldType(string groupName, string value)
        {
            int valueLength = value.Length;

            switch (groupName)
            {
                case "Day":
                    {
                        return new FieldDefinition(FieldType.Day, (DayFormat)valueLength);
                    }
                case "Hour":
                    {
                        int valueBase = valueLength;
                        if (value == value.ToUpper())
                            valueBase += 10;

                        return new FieldDefinition(FieldType.Hour, (HourFormat)valueBase);
                    }
                case "Minute":
                    {
                        return new FieldDefinition(FieldType.Minute, (MinuteFormat)valueLength);
                    }
                case "Month":
                    {
                        return new FieldDefinition(FieldType.Month, (MonthFormat)valueLength);
                    }
                case "Seconds":
                    {
                        return new FieldDefinition(FieldType.Seconds, (SecondsFormat)valueLength);
                    }
                case "AMPM":
                    {
                        return new FieldDefinition(FieldType.AMPM, (AMPMFormat)valueLength);
                    }
                case "Year":
                    {
                        return new FieldDefinition(FieldType.Year, (YearFormat)valueLength);
                    }
                case "String":
                    {
                        return new FieldDefinition(FieldType.String, DateTimeStringFormat.Default, value);
                    }
            }

            throw new FormatException();
        }
    }
}
