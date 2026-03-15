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
    internal class Collections
    {
        public class Built : ObservableCollection<ItemProperty>
        {
            public Built()
                : base()
            {
                Add(new ItemProperty("Execution Time"));
                Add(new ItemProperty("Page Number"));
                Add(new ItemProperty("ReportFolder"));
                Add(new ItemProperty("ReportName"));
                Add(new ItemProperty("ReportServerUrl"));
                Add(new ItemProperty("TotalPages"));
                Add(new ItemProperty("UserID"));
                Add(new ItemProperty("Language"));
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
                Add(new ArithProperty("^"));
                Add(new ArithProperty("*"));
                Add(new ArithProperty("/"));
                Add(new ArithProperty(@"\"));
                Add(new ArithProperty("Mod"));
                Add(new ArithProperty("+"));
                Add(new ArithProperty("-"));
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
                Add(new CompareProperty("<"));
                Add(new CompareProperty("<="));
                Add(new CompareProperty(">"));
                Add(new CompareProperty(@">="));
                Add(new CompareProperty("=="));
                Add(new CompareProperty("<>"));
                Add(new CompareProperty("Like"));
                Add(new CompareProperty("Is"));
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
                Add(new ConcateProperty("&"));
                Add(new ConcateProperty("+"));
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
                Add(new LogicProperty("And"));
                Add(new LogicProperty("Not"));
                Add(new LogicProperty("Or"));
                Add(new LogicProperty(@"Xor"));
                Add(new LogicProperty("AndAlso"));
                Add(new LogicProperty("OrElse"));
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
                Add(new BitshiftProperty("<<"));
                Add(new BitshiftProperty(">>"));
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
                Add(new TextProperty("Asc"));
                Add(new TextProperty("AscW"));
                Add(new TextProperty("Chr"));
                Add(new TextProperty(@"ChrW"));
                Add(new TextProperty("Filter"));
                Add(new TextProperty("Format"));
                Add(new TextProperty("FormatCurrency"));
                Add(new TextProperty("FormatDateTime"));
                Add(new TextProperty("FormatNumber"));
                Add(new TextProperty("FormatPercent"));
                Add(new TextProperty("GetChar"));
                Add(new TextProperty("InStr"));
                Add(new TextProperty("InStrRev"));
                Add(new TextProperty("Join"));
                Add(new TextProperty("LCase"));
                Add(new TextProperty("Left"));
                Add(new TextProperty("Len"));
                Add(new TextProperty("LSet"));
                Add(new TextProperty("LTrim"));
                Add(new TextProperty("Mid"));
                Add(new TextProperty("Replace"));
                Add(new TextProperty("Right"));
                Add(new TextProperty("RSet"));
                Add(new TextProperty("RTrim"));
                Add(new TextProperty("Space"));
                Add(new TextProperty("Split"));
                Add(new TextProperty("StrComp"));
                Add(new TextProperty("StrConv"));
                Add(new TextProperty("StrDup"));
                Add(new TextProperty("StrReverse"));
                Add(new TextProperty("Trim"));
                Add(new TextProperty("UCase"));
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
                Add(new DateandTimeProperty("CDate"));
                Add(new DateandTimeProperty("DateAdd"));
                Add(new DateandTimeProperty("DateDiff"));
                Add(new DateandTimeProperty(@"DatePart"));
                Add(new DateandTimeProperty("DateSerial"));
                Add(new DateandTimeProperty("DateString"));
                Add(new DateandTimeProperty("DateValue"));
                Add(new DateandTimeProperty("Day"));
                Add(new DateandTimeProperty("FormatDateTime"));
                Add(new DateandTimeProperty("Hour"));
                Add(new DateandTimeProperty("Minute"));
                Add(new DateandTimeProperty("Month"));
                Add(new DateandTimeProperty("MonthName"));
                Add(new DateandTimeProperty("Now"));
                Add(new DateandTimeProperty("Second"));
                Add(new DateandTimeProperty("TimeOfDay"));
                Add(new DateandTimeProperty("Timer"));
                Add(new DateandTimeProperty("TimeSerial"));
                Add(new DateandTimeProperty("TimeString"));
                Add(new DateandTimeProperty("TimeValue"));
                Add(new DateandTimeProperty("Today"));
                Add(new DateandTimeProperty("Weekday"));
                Add(new DateandTimeProperty("WeekdayName"));
                Add(new DateandTimeProperty("Year"));
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
                Add(new MathsProperty("Abs"));
                Add(new MathsProperty("Acos"));
                Add(new MathsProperty("Asin"));
                Add(new MathsProperty("Atan"));
                Add(new MathsProperty("Atan2"));
                Add(new MathsProperty("BidMul"));
                Add(new MathsProperty("Ceiling"));
                Add(new MathsProperty("Cos"));
                Add(new MathsProperty("Cosh"));
                Add(new MathsProperty("Exp"));
                Add(new MathsProperty("Fix"));
                Add(new MathsProperty("Floor"));
                Add(new MathsProperty("Int"));
                Add(new MathsProperty("Log"));
                Add(new MathsProperty("Log10"));
                Add(new MathsProperty("Max"));
                Add(new MathsProperty("Min"));
                Add(new MathsProperty("Pow"));
                Add(new MathsProperty("Rnd"));
                Add(new MathsProperty("Round"));
                Add(new MathsProperty("Sign"));
                Add(new MathsProperty("sin"));
                Add(new MathsProperty("Sinh"));
                Add(new MathsProperty("Sqrt"));
                Add(new MathsProperty("Tan"));
                Add(new MathsProperty("Tanh"));
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
                Add(new InspectionProperty("IsArray"));
                Add(new InspectionProperty("IsDate"));
                Add(new InspectionProperty("IsNothing"));
                Add(new InspectionProperty("IsNumeric"));
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
                Add(new ProgramflowProperty("Choose"));
                Add(new ProgramflowProperty("IIf"));
                Add(new ProgramflowProperty("Switch"));
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
                Add(new AggregateProperty("Avg"));
                Add(new AggregateProperty("Count"));
                Add(new AggregateProperty("CountDistinct"));
                Add(new AggregateProperty(@"CountRows"));
                Add(new AggregateProperty("First"));
                Add(new AggregateProperty("Last"));
                Add(new AggregateProperty("Max"));
                Add(new AggregateProperty("Min"));
                Add(new AggregateProperty("StDev"));
                Add(new AggregateProperty("StDevP"));
                Add(new AggregateProperty("Sum"));
                Add(new AggregateProperty("Var"));
                Add(new AggregateProperty("VarP"));
                Add(new AggregateProperty("RunningValue"));
                Add(new AggregateProperty("Aggregate"));
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
                Add(new FinancialProperty("DDB"));
                Add(new FinancialProperty("FV"));
                Add(new FinancialProperty("IPmt"));
                Add(new FinancialProperty(@"NPer"));
                Add(new FinancialProperty("Pmt"));
                Add(new FinancialProperty("PPmt"));
                Add(new FinancialProperty("PV"));
                Add(new FinancialProperty("Rate"));
                Add(new FinancialProperty("SLN"));
                Add(new FinancialProperty("SYD"));
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
                Add(new ConversionProperty("CBool"));
                Add(new ConversionProperty("CByte"));
                Add(new ConversionProperty("CChar"));
                Add(new ConversionProperty("CDate"));
                Add(new ConversionProperty("CDbl"));
                Add(new ConversionProperty("CDec"));
                Add(new ConversionProperty("CInt"));
                Add(new ConversionProperty("CLng"));
                Add(new ConversionProperty("CObj"));
                Add(new ConversionProperty("CShort"));
                Add(new ConversionProperty("CSng"));
                Add(new ConversionProperty("CStr"));
                Add(new ConversionProperty("Fix"));
                Add(new ConversionProperty("Hex"));
                Add(new ConversionProperty("Int"));
                Add(new ConversionProperty("Oct"));
                Add(new ConversionProperty("Str"));
                Add(new ConversionProperty("Val"));

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
                Add(new MiscellaneousProperty("InScope"));
                Add(new MiscellaneousProperty("Level"));
                Add(new MiscellaneousProperty("Previous"));
                Add(new MiscellaneousProperty("RowNumber"));
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



