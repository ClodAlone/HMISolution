#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Reports.Designer.Editors
{
    internal class Descriptions
    {
        public class Built : ObservableCollection<ItemProperty>
        {
            public Built()
                : base()
            {
                Add(new ItemProperty("The date and time that the report began to run."));
                Add(new ItemProperty("The current page number. Can be used only in page header or footer."));
                Add(new ItemProperty("The full path to the folder containing the report. This does not include the report server URL."));
                Add(new ItemProperty("The name of the report as it is stored in the report server database."));
                Add(new ItemProperty("The URL of the report server on which the report is being run."));
                Add(new ItemProperty("The total number of pages in the report. Can be used only in page header and footer."));
                Add(new ItemProperty("The ID of the user running the report."));
                Add(new ItemProperty("The language ID of the client running the report."));
            }
        }

        public class ItemProperty
        {
            private string pty;


            public ItemProperty(string first)
            {
                this.pty = first;

            }

            public string ItemPty
            {
                get { return pty; }
                set { pty = value; }
            }


        }

        public class Arith : ObservableCollection<ArithProperty>
        {
            public Arith()
                : base()
            {
                Add(new ArithProperty("Raises a number to the power of another number."));
                Add(new ArithProperty("Multiplies two numbers."));
                Add(new ArithProperty("Divides two numbers and returns a floating-point result."));
                Add(new ArithProperty("Divides two numbers and returns an integer result."));
                Add(new ArithProperty("Divides two numbers and returns only the remainder."));
                Add(new ArithProperty("Adds two numbers. Also used to concatenate two strings."));
                Add(new ArithProperty("Yields the difference between two numbers or indicates the negative value of a numeric expression."));
            }
        }

        public class ArithProperty
        {
            private string arith;


            public ArithProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Compare : ObservableCollection<CompareProperty>
        {
            public Compare()
                : base()
            {
                Add(new CompareProperty("Less than."));
                Add(new CompareProperty("Less than or equal to."));
                Add(new CompareProperty("Greater than."));
                Add(new CompareProperty("Greater than or equal to."));
                Add(new CompareProperty("Equal to."));
                Add(new CompareProperty("Not equal to."));
                Add(new CompareProperty("Compares two strings."));
                Add(new CompareProperty("Compares two object reference variables."));
            }
        }

        public class CompareProperty
        {
            private string arith;


            public CompareProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Concate : ObservableCollection<ConcateProperty>
        {
            public Concate()
                : base()
            {
                Add(new ConcateProperty("Generates a string concatenation of two expressions."));
                Add(new ConcateProperty("Adds two numbers. Also used to concatenate two strings."));
            }
        }

        public class ConcateProperty
        {
            private string arith;

            public ConcateProperty(string first)
            {
                this.arith = first;
            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Logic : ObservableCollection<LogicProperty>
        {
            public Logic()
                : base()
            {
                Add(new LogicProperty("Performs a logical conjunction on two Boolean expressions, or bitwise conjunction on two numeric expressions."));
                Add(new LogicProperty("Performs logical negation on a Boolean expression, or bitwise negation on a numeric expression."));
                Add(new LogicProperty("Used to perform a logical disjunction on two Boolean expressions, or bitwise disjunction on two numeric values."));
                Add(new LogicProperty("Performs a logical exclusion operation on two Boolean expressions, or a bitwise exclusion on two numeric expressions."));
                Add(new LogicProperty("Performs short-circuiting logical conjunction on two expressions."));
                Add(new LogicProperty("Used to perform short-circuiting logical disjunction on two expressions."));
            }
        }

        public class LogicProperty
        {
            private string arith;


            public LogicProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Bitshift : ObservableCollection<BitshiftProperty>
        {
            public Bitshift()
                : base()
            {
                Add(new BitshiftProperty("Performs an arithmetic left shift on a bit pattern."));
                Add(new BitshiftProperty("Performs an arithmetic right shift on a bit pattern."));
            }
        }

        public class BitshiftProperty
        {
            private string arith;


            public BitshiftProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Text : ObservableCollection<TextProperty>
        {
            public Text()
                : base()
            {
                Add(new TextProperty("Returns an Integer value representing the character code corresponding to a character."));
                Add(new TextProperty("Returns an Integer value representing the character code corresponding to a character."));
                Add(new TextProperty("Returns the character associated with the specified character code."));
                Add(new TextProperty("Returns the character associated with the specified character code."));
                Add(new TextProperty("Returns a zero-based array containing a subset of a String array based on specified filter criteria."));
                Add(new TextProperty("Returns a string formatted according to instructions contained in a format String expression."));
                Add(new TextProperty("Returns an expression formatted as a currency value using the currency symbol defined in the system control panel."));
                Add(new TextProperty("Returns a string expression representing a date/time value."));
                Add(new TextProperty("Returns an expression formatted as a number."));
                Add(new TextProperty("Returns an expression formatted as a percentage (that is, multiplied by 100) with a trailing % character."));
                Add(new TextProperty("Returns a Char value representing the character from the specified index in the supplied string."));
                Add(new TextProperty("Returns an integer specifying the start position of the first occurrence of one string within another."));
                Add(new TextProperty("Returns the position of the first occurrence of one string within another, starting from the right side of the string."));
                Add(new TextProperty("Returns a string created by joining a number of substrings contained in an array."));
                Add(new TextProperty("Returns a string or character converted to lowercase."));
                Add(new TextProperty("Returns a string containing a specified number of characters from the left side of a string."));
                Add(new TextProperty("Returns an integer containing either the number of characters in a string or the number of bytes required to store a variable."));
                Add(new TextProperty("Returns a left-aligned string containing the specified string adjusted to the specified length."));
                Add(new TextProperty("Returns a string containing a copy of a specified string with no leading spaces (LTrim), no trailing spaces (RTrim), or no leading or trailing spaces (Trim)."));
                Add(new TextProperty("Returns a string containing a specified number of characters from a string."));
                Add(new TextProperty("Returns a string in which a specified substring has been replaced with another substring a specified number of times."));
                Add(new TextProperty("Returns a string containing a specified number of characters from the right side of a string."));
                Add(new TextProperty("Returns a right-aligned string containing the specified string adjusted to the specified length."));
                Add(new TextProperty("Returns a string containing a copy of a specified string with no leading spaces (LTrim), no trailing spaces (RTrim), or no leading or trailing spaces (Trim)."));
                Add(new TextProperty("Returns a string consisting of the specified number of spaces."));
                Add(new TextProperty("Returns a zero-based, one-dimensional array containing a specified number of substrings."));
                Add(new TextProperty("Returns -1, 0, or 1, based on the result of a string comparison."));
                Add(new TextProperty("Returns a string converted as specified."));
                Add(new TextProperty("Returns a string or object consisting of the specified character repeated the specified number of times."));
                Add(new TextProperty("Returns a string in which the character order of a specified string is reversed."));
                Add(new TextProperty("Returns a string containing a copy of a specified string with no leading spaces (LTrim), no trailing spaces (RTrim), or no leading or trailing spaces (Trim)."));
                Add(new TextProperty("Returns a string or character containing the specified string converted to uppercase."));
            }
        }

        public class TextProperty
        {
            private string arith;


            public TextProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class DateandTime : ObservableCollection<DateandTimeProperty>
        {
            public DateandTime()
                : base()
            {
                Add(new DateandTimeProperty("Convert to Date."));
                Add(new DateandTimeProperty("Returns a Date value containing a date and time value to which a specified time interval has been added."));
                Add(new DateandTimeProperty("Returns a Long value specifying the number of time intervals between two Date values.  "));
                Add(new DateandTimeProperty("Returns an Integer value containing the specified component of a given Date value.  "));
                Add(new DateandTimeProperty("Returns a Date value representing a specified year, month, and day, with the time information set to midnight (00:00:00)."));
                Add(new DateandTimeProperty("Returns or sets a String value representing the current date according to your system."));
                Add(new DateandTimeProperty("Returns a Date value containing the date information represented by a string, with the time information set to midnight (00:00:00). "));
                Add(new DateandTimeProperty("Returns an Integer value from 1 through 31 representing the day of the month. "));
                Add(new DateandTimeProperty("Returns a string expression representing a date/time value."));
                Add(new DateandTimeProperty("Returns an Integer value from 0 through 23 representing the hour of the day.    "));
                Add(new DateandTimeProperty("Returns an Integer value from 0 through 59 representing the minute of the hour."));
                Add(new DateandTimeProperty("Returns an Integer value from 1 through 12 representing the month of the year.  "));
                Add(new DateandTimeProperty("Returns a String value containing the name of the specified month. "));
                Add(new DateandTimeProperty("Returns a Date value containing the current date and time according to your system.       "));
                Add(new DateandTimeProperty("Returns an Integer value from 0 through 59 representing the second of the minute.  "));
                Add(new DateandTimeProperty("Returns or sets a Date value containing the current time of day according to your system. "));
                Add(new DateandTimeProperty("Returns a Double value representing the number of seconds elapsed since midnight."));
                Add(new DateandTimeProperty("Returns a Date value representing a specified hour, minute, and second, with the date information set relative to January 1 of the year 1."));
                Add(new DateandTimeProperty("Returns or sets a String value representing the current time of day according to your system."));
                Add(new DateandTimeProperty("Returns a Date value containing the time information represented by a string, with the date information set to January 1 of the year 1. "));
                Add(new DateandTimeProperty("Returns or sets a Date value containing the current date according to your system. "));
                Add(new DateandTimeProperty("Returns an Integer value containing a number representing the day of the week.   "));
                Add(new DateandTimeProperty("Returns a String value containing the name of the specified weekday."));
                Add(new DateandTimeProperty("Returns an Integer value from 1 through 9999 representing the year."));
            }
        }

        public class DateandTimeProperty
        {
            private string arith;


            public DateandTimeProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Maths : ObservableCollection<MathsProperty>
        {
            public Maths()
                : base()
            {
                Add(new MathsProperty("Returns the absolute value of a single-precision floating-point number. "));
                Add(new MathsProperty("Returns the angle whose cosine is the specified number."));
                Add(new MathsProperty("Returns the angle whose sine is the specified number."));
                Add(new MathsProperty("Returns the angle whose tangent is the specified number."));
                Add(new MathsProperty("Returns the angle whose tangent is the quotient of two specified numbers."));
                Add(new MathsProperty("Produces the full product of two 32-bit numbers."));
                Add(new MathsProperty("Returns the smallest integer greater than or equal to the specified double-precision floating-point number."));
                Add(new MathsProperty("Returns the cosine of the specified angle. "));
                Add(new MathsProperty("Returns the hyperbolic cosine of the specified angle."));
                Add(new MathsProperty("Returns e raised to the specified power. "));
                Add(new MathsProperty("Return the integer portion of a number."));
                Add(new MathsProperty("Returns the largest integer less than or equal to the specified double-precision floating-point number."));
                Add(new MathsProperty("Return the integer portion of a number."));
                Add(new MathsProperty("Returns the natural (base e) logarithm of a specified number. "));
                Add(new MathsProperty("Returns the base 10 logarithm of a specified number."));
                Add(new MathsProperty("Returns the maximum value from all non-null values of the specified expression."));
                Add(new MathsProperty("Returns the minimum value from all non-null values of the specified expression."));
                Add(new MathsProperty("Returns a specified number raised to the specified power. "));
                Add(new MathsProperty("Returns a random number of type Single. "));
                Add(new MathsProperty("Returns a random number of type Single. "));
                Add(new MathsProperty("Returns a value indicating the sign of an 8-bit signed integer."));
                Add(new MathsProperty("Returns the sine of the specified angle. "));
                Add(new MathsProperty("Returns the hyperbolic sine of the specified angle."));
                Add(new MathsProperty("Returns the square root of a specified number."));
                Add(new MathsProperty("Returns the tangent of the specified angle. "));
                Add(new MathsProperty("Returns the hyperbolic tangent of the specified angle."));
            }
        }

        public class MathsProperty
        {
            private string arith;


            public MathsProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Inspection : ObservableCollection<InspectionProperty>
        {
            public Inspection()
                : base()
            {
                Add(new InspectionProperty("Returns a Boolean value indicating whether a variable points to an array."));
                Add(new InspectionProperty("Returns a Boolean value indicating whether an expression represents a valid Date value."));
                Add(new InspectionProperty("Returns a Boolean value indicating whether an expression has no object assigned to it."));
                Add(new InspectionProperty("Returns a Boolean value indicating whether an expression can be evaluated as a number."));
            }
        }

        public class InspectionProperty
        {
            private string arith;


            public InspectionProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Programflow : ObservableCollection<ProgramflowProperty>
        {
            public Programflow()
                : base()
            {
                Add(new ProgramflowProperty("Selects and returns a value from a list of arguments."));
                Add(new ProgramflowProperty("Returns one of two objects, depending on the evaluation of an expression."));
                Add(new ProgramflowProperty("Evaluates a list of expressions and returns an Object value corresponding to the first expression in the list that is True."));
            }
        }

        public class ProgramflowProperty
        {
            private string arith;


            public ProgramflowProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Aggregate : ObservableCollection<AggregateProperty>
        {
            public Aggregate()
                : base()
            {
                Add(new AggregateProperty("Returns the average of all non-null values from the specified expression."));
                Add(new AggregateProperty("Returns a count of the values from the specified expression."));
                Add(new AggregateProperty("CounReturns a count of all distinct values from the specified expression.tDistinct"));
                Add(new AggregateProperty("Returns a count of rows within the specified scope."));
                Add(new AggregateProperty("Returns the first value from the specified expression."));
                Add(new AggregateProperty("Returns the last value from the specified expression."));
                Add(new AggregateProperty("Returns the maximum value from all non-null values of the specified expression."));
                Add(new AggregateProperty("Returns the minimum value from all non-null values of the specified expression."));
                Add(new AggregateProperty("Returns the standard deviation of all non-null values of the specified expression."));
                Add(new AggregateProperty("Returns the population standard deviation of all non-null values of the specified expression."));
                Add(new AggregateProperty("Returns a sum of the values of the specified expression."));
                Add(new AggregateProperty("Returns the variance of all non-null values of the specified expression."));
                Add(new AggregateProperty("Returns the population variance of all non-null values of the specified expression."));
                Add(new AggregateProperty("Uses a specified function to return a running aggregate of the specified expression."));
                Add(new AggregateProperty("Returns a custom aggregate of the specified expression, as defined by the data provider."));
            }
        }

        public class AggregateProperty
        {
            private string arith;


            public AggregateProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Financial : ObservableCollection<FinancialProperty>
        {
            public Financial()
                : base()
            {
                Add(new FinancialProperty("Returns a Double specifying the depreciation of an asset for a specific time period using the double-declining balance method or some other method you specify."));
                Add(new FinancialProperty("Returns a Double specifying the future value of an annuity based on periodic, fixed payments and a fixed interest rate."));
                Add(new FinancialProperty("Returns a Double specifying the interest payment for a given period of an annuity based on periodic, fixed payments and a fixed interest rate."));
                Add(new FinancialProperty("Returns a Double specifying the number of periods for an annuity based on periodic fixed payments and a fixed interest rate."));
                Add(new FinancialProperty("Returns a Double specifying the payment for an annuity based on periodic, fixed payments and a fixed interest rate."));
                Add(new FinancialProperty("Returns a Double specifying the principal payment for a given period of an annuity based on periodic fixed payments and a fixed interest rate."));
                Add(new FinancialProperty("Returns a Double specifying the present value of an annuity based on periodic, fixed payments to be paid in the future and a fixed interest rate."));
                Add(new FinancialProperty("Returns a Double specifying the interest rate per period for an annuity."));
                Add(new FinancialProperty("Returns a Double specifying the straight-line depreciation of an asset for a single period."));
                Add(new FinancialProperty("Returns a Double specifying the sum-of-years digits depreciation of an asset for a specified period."));
            }
        }

        public class FinancialProperty
        {
            private string arith;


            public FinancialProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Conversion : ObservableCollection<ConversionProperty>
        {
            public Conversion()
                : base()
            {
                Add(new ConversionProperty("Convert to Boolean."));
                Add(new ConversionProperty("Convert to Byte."));
                Add(new ConversionProperty("Convert to Char."));
                Add(new ConversionProperty("Convert to Date."));
                Add(new ConversionProperty("Convert to Double."));
                Add(new ConversionProperty("Convert to Decimal."));
                Add(new ConversionProperty("Convert to Integer."));
                Add(new ConversionProperty("Convert to Long."));
                Add(new ConversionProperty("Convert to Object."));
                Add(new ConversionProperty("Convert to Short."));
                Add(new ConversionProperty("Convert to Single."));
                Add(new ConversionProperty("Convert to String."));
                Add(new ConversionProperty("Return the integer portion of a number."));
                Add(new ConversionProperty("Returns a string representing the hexadecimal value of a number."));
                Add(new ConversionProperty("Return the integer portion of a number."));
                Add(new ConversionProperty("Returns a string representing the octal value of a number."));
                Add(new ConversionProperty("Returns a String representing of a number."));
                Add(new ConversionProperty("Returns the numbers contained in a string as a numeric value of appropriate type."));

            }
        }

        public class ConversionProperty
        {
            private string arith;


            public ConversionProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }

        public class Miscellaneous : ObservableCollection<MiscellaneousProperty>
        {
            public Miscellaneous()
                : base()
            {
                Add(new MiscellaneousProperty("Returns true if the current instance is within the specified scope."));
                Add(new MiscellaneousProperty("Returns a zero-based integer representing the current depth level of a recursive hierarchy."));
                Add(new MiscellaneousProperty("Returns the value of the expression for the previous row of data."));
                Add(new MiscellaneousProperty("Returns a running count of all rows in the specified scope."));
            }
        }

        public class MiscellaneousProperty
        {
            private string arith;


            public MiscellaneousProperty(string first)
            {
                this.arith = first;

            }

            public string ItemPty
            {
                get { return arith; }
                set { arith = value; }
            }


        }
    }
}
