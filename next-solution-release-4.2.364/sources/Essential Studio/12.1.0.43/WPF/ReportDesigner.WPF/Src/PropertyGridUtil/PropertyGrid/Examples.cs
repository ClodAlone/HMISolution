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
    internal class Built : ObservableCollection<ItemProperty>
    {
        public Built()
            : base()
        {
            Add(new ItemProperty("Globals!ExecutionTime"));
            Add(new ItemProperty("Globals!PageNumber"));
            Add(new ItemProperty("Globals!ReportFolder"));
            Add(new ItemProperty("Globals!ReportName"));
            Add(new ItemProperty("Globals!ReportServerUrl"));
            Add(new ItemProperty("Globals!TotalPages"));
            Add(new ItemProperty("User!UserID"));
            Add(new ItemProperty("User!Language"));
        }
    }

    internal class ItemProperty
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

    internal class Arith : ObservableCollection<ArithProperty>
    {
        public Arith()
            : base()
        {
            Add(new ArithProperty("=Fields!NumberCarsOwned.Value ^ 3"));
            Add(new ArithProperty("=Fields!NumberCarsOwned.Value * 2"));
            Add(new ArithProperty("=Fields!YearlyIncome.Value / 2"));
            Add(new ArithProperty(@"=Fields!YearlyIncome.Value \ 2"));
            Add(new ArithProperty("=Fields!YearlyIncome.Value Mod 12"));
            Add(new ArithProperty("=Fields!NumberCarsOwned.Value + 2"));
            Add(new ArithProperty("=Fields!NumberCarsOwned.Value - 2"));
        }
    }

    internal class ArithProperty
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

    internal class Compare : ObservableCollection<CompareProperty>
    {
        public Compare()
            : base()
        {
            Add(new CompareProperty("=Fields!YearlyIncome.Value < 25000"));
            Add(new CompareProperty("=Fields!YearlyIncome.Value <= 25000"));
            Add(new CompareProperty("=Fields!YearlyIncome.Value > 25000"));
            Add(new CompareProperty(@"=Fields!YearlyIncome.Value >= 25000"));
            Add(new CompareProperty("=Fields!YearlyIncome.Value = 50000"));
            Add(new CompareProperty("=Fields!YearlyIncome.Value <> 50000"));
            Add(new CompareProperty("=Fields!FirstName.Value Like \"T*\" "));
            Add(new CompareProperty("=Fields!FirstName.Value Is Fields!LastName.Value"));
        }
    }

    internal class CompareProperty
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

    internal class Concate : ObservableCollection<ConcateProperty>
    {
        public Concate()
            : base()
        {
            Add(new ConcateProperty("=Fields!FirstName.Value & \" \" & Fields!LastName.Value"));
            Add(new ConcateProperty("=Fields!FirstName.Value + \" \" + Fields!LastName.Value"));
        }
    }

    internal class ConcateProperty
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

    internal class Logic : ObservableCollection<LogicProperty>
    {
        public Logic()
            : base()
        {
            Add(new LogicProperty("=(Fields!YearlyIncome.Value > 50000) And (Fields!NumberCarsOwned.Value > 2)"));
            Add(new LogicProperty("=Not (Fields!YearlyIncome.Value > 50000)"));
            Add(new LogicProperty("=(Fields!YearlyIncome.Value > 50000) Or (Fields!NumberCarsOwned.Value > 1)"));
            Add(new LogicProperty(@"=(Fields!YearlyIncome.Value > 50000) Xor (Fields!NumberCarsOwned.Value) > 1"));
            Add(new LogicProperty("=(Fields!YearlyIncome.Value) > 50000 AndAlso (Fields!NumberCarsOwned.Value > 1)"));
            Add(new LogicProperty("=(Fields!YearlyIncome.Value > 50000) OrElse (Fields!NumberCarsOwned.Value > 1)"));
        }
    }

    internal class LogicProperty
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

    internal class Bitshift : ObservableCollection<BitshiftProperty>
    {
        public Bitshift()
            : base()
        {
            Add(new BitshiftProperty("=(Fields!IntegerCounter.Value) << 4"));
            Add(new BitshiftProperty("=(Fields!IntegerCounter.Value) >> 4"));
        }
    }

    internal class BitshiftProperty
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

    internal class Text : ObservableCollection<TextProperty>
    {
        public Text()
            : base()
        {
            Add(new TextProperty("=Asc(Fields!Description.Value)"));
            Add(new TextProperty("=AscW(Fields!Description.Value)"));
            Add(new TextProperty("=Chr(65)"));
            Add(new TextProperty(@"=ChrW(241)"));
            Add(new TextProperty("=Filter(Parameters!MultivalueParameter.Value, \"3\",True, CompareMethod.Binary)"));
            Add(new TextProperty("=Format(Globals!ExecutionTime, \"Long Date\")"));
            Add(new TextProperty("=FormatCurrency(Fields!YearlyIncome.Value,0)"));
            Add(new TextProperty("=FormatDateTime(Fields!BirthDate.Value,DateFormat.ShortDate)"));
            Add(new TextProperty("=FormatNumber(Fields!Weight.Value,2)"));
            Add(new TextProperty("=FormatPercent(Fields!Sales.Value/Sum(Fields!Sales.Value, \"DataSet1\"),0)"));
            Add(new TextProperty("=GetChar(Fields!Description.Value, 5)"));
            Add(new TextProperty("=InStr(Fields!Description.Value, \"car\")"));
            Add(new TextProperty("=InStrRev(Fields!Description.Value, \"car\")"));
            Add(new TextProperty("=Join(Parameters!MultivalueParameter.Value,\", \")"));
            Add(new TextProperty("=LCase(Fields!Description.Value)"));
            Add(new TextProperty("=Left(Fields!Description.Value,4)"));
            Add(new TextProperty("=Len(Fields!Description.Value)"));
            Add(new TextProperty("=LSet(Fields!Description.Value,4)"));
            Add(new TextProperty("=LTrim(Fields!Description.Value)"));
            Add(new TextProperty("=Mid(Fields!Description.Value,3,4)"));
            Add(new TextProperty("=Replace(Fields!Description.Value,\"tube\",\"headlight\")"));
            Add(new TextProperty("=Right(Fields!Description.Value,4)"));
            Add(new TextProperty("=RSet(Fields!Description.Value,4)"));
            Add(new TextProperty("=RTrim(Fields!Description.Value)"));
            Add(new TextProperty("=Space(3)"));
            Add(new TextProperty("=Split(Fields!ListWithCommas.Value,\", \")"));
            Add(new TextProperty("=StrComp(Fields!Description.Value,First(Fields!Description.Value))"));
            Add(new TextProperty("=StrConv(Fields!Description.Value,vbProperCase)"));
            Add(new TextProperty("=StrDup(3,\"M\")"));
            Add(new TextProperty("=StrReverse(Fields!Description.Value)"));
            Add(new TextProperty("=Trim(Fields!Description.Value)"));
            Add(new TextProperty("=UCase(Fields!Description.Value)"));
        }
    }

    internal class TextProperty
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

    internal class DateandTime : ObservableCollection<DateandTimeProperty>
    {
        public DateandTime()
            : base()
        {
            Add(new DateandTimeProperty("=CDate(Fields!BirthDate.Value)"));
            Add(new DateandTimeProperty("=DateAdd(\"d\",3,Fields!BirthDate.Value)=DateAdd(DateInterval.Day,3,Fields!BirthDate.Value)"));
            Add(new DateandTimeProperty("=DateDiff(\"yyyy\",Fields!BirthDate.Value,\"1/1/2007\")=DateDiff(DateInterval.Year,Fields!BirthDate.Value,\"1/1/2007\")"));
            Add(new DateandTimeProperty("=DatePart(\"q\",Fields!BirthDate.Value,0,0) \n =DatePart(DateInterval.Quarter,Fields!BirthDate.Value,FirstDayOfWeek.System,FirstWeekOfYear.System)"));
            Add(new DateandTimeProperty("=DateSerial(DatePart(\"yyyy\",Fields!BirthDate.Value)-10, DatePart(\"m\",Fields!BirthDate.Value)+3,DatePart(\"d\",Fields!BirthDate.Value)-1) \n =DateSerial(DatePart(DateInterval.Year,Fields!BirthDate.Value)-10,DatePart(\"m\",Fields!BirthDate.Value)+3,DatePart(\"d\",Fields!BirthDate.Value)-1) \n =DateSerial(2007,10,24)"));
            Add(new DateandTimeProperty("=DateString()\n =DatePart(\"m\",DateString())"));
            Add(new DateandTimeProperty("=DateValue(\"January 15, 2007\")"));
            Add(new DateandTimeProperty("=Day(Fields!BirthDate.Value)"));
            Add(new DateandTimeProperty("=FormatDateTime(Fields!BirthDate.Value, DateFormat.ShortDate)"));
            Add(new DateandTimeProperty("=Hour(Fields!BirthDate.Value)"));
            Add(new DateandTimeProperty("=Minute(Fields!BirthDate.Value)"));
            Add(new DateandTimeProperty("=Month(Fields!BirthDate.Value)"));
            Add(new DateandTimeProperty("=MonthName(10,True)\n =MonthName(Month(Fields!BirthDate.Value),False) \n =\"The month of your birthday is \" & MonthName(Month(Fields!BirthDate.Value))"));
            Add(new DateandTimeProperty("=Now() \n =\"This time tomorrow is \" & DateAdd(\"d\",1,Now()) \n =\"This time tomorrow is \" & DateAdd(DateInterval.Day,1,Now())"));
            Add(new DateandTimeProperty("=Second(Fields!BirthDate.Value)"));
            Add(new DateandTimeProperty("=TimeOfDay() \n =\"Time of the day is \" & TimeOfDay()"));
            Add(new DateandTimeProperty("=Timer() \n =\"Number of seconds since midnight \" & Timer()"));
            Add(new DateandTimeProperty("=TimeSerial(DatePart(\"h\",Fields!BirthDate.Value),DatePart(\"n\",Fields!BirthDate.Value),DatePart(\"s\",Fields!BirthDate.Value)) \n =TimeSerial(DatePart(DateInterval.Hour,Fields!BirthDate.Value),DatePart(DateInterval.Minute,Fields!BirthDate.Value),DatePart(DateInterval.Second,Fields!BirthDate.Value)) \n =TimeSerial(23,49,52)"));
            Add(new DateandTimeProperty("=TimeString()"));
            Add(new DateandTimeProperty("=TimeValue(\"16:20:17\") \n =TimeValue(Fields!BirthDate.Value)"));
            Add(new DateandTimeProperty("=Today() \n =\"Tomorrow is \" & DateAdd(\"d\",1,Today()) \n =\"Tomorrow is \" & DateAdd(DateInterval.Day,1,Today())"));
            Add(new DateandTimeProperty("=Weekday(Fields!BirthDate.Value,0) \n =Weekday(Fields!BirthDate.Value,FirstDayOfWeek.System)"));
            Add(new DateandTimeProperty("=WeekdayName(2,True,0) \n =WeekDayName(DatePart(\"w\",Fields!BirthDate.Value),True,0) \n =WeekDayName(DatePart(DateInterval.Weekday,Fields!BirthDate.Value),True,FirstDayOfWeek.System)"));
            Add(new DateandTimeProperty("=Year(Fields!BirthDate.Value)"));
        }
    }

    internal class DateandTimeProperty
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

    internal class Maths : ObservableCollection<MathsProperty>
    {
        public Maths()
            : base()
        {
            Add(new MathsProperty("=Abs(-5.5) \n =Abs(Fields!YearlyIncome.Value - 80000)"));
            Add(new MathsProperty("=Acos(.5) \n =Acos(Fields!Angle.Value)"));
            Add(new MathsProperty("=Asin(.5) \n =Asin(Fields!Angle.Value)"));
            Add(new MathsProperty("=Atan(45) \n =Atan(Fields!Tangent.Value)"));
            Add(new MathsProperty("=Atan2(3,5) \n =Atan2(Fields!CoordinateY.Value,Fields!CoordinateX.Value)"));
            Add(new MathsProperty("=BigMul(2147483647,-2147483647) \n =BigMul(Fields!Int32Value.Value, Fields!Int32Value.Value)"));
            Add(new MathsProperty("=Ceiling(34.3352) \n =Ceiling(Fields!YearlyIncome.Value / 7)"));
            Add(new MathsProperty("=Cos(67) \n =Cos(Fields!Angle.Value)"));
            Add(new MathsProperty("=Cosh(67) \n =Cosh(Fields!Angle.Value)"));
            Add(new MathsProperty("=Exp(5) \n =Exp(Fields!IntegerCounter.Value)"));
            Add(new MathsProperty("=Fix(-9.25) \n =Fix(Fields!YearlyIncome.Value / -3)"));
            Add(new MathsProperty("=Floor(4.67) \n =Floor(Fields!YearlyIncome.Value / 12)"));
            Add(new MathsProperty("=Int(-93.4) \n =Int(Fields!YearlyIncome.Value / 12)"));
            Add(new MathsProperty("=Log(33.5) \n =Log(Fields!NumberValue.Value)"));
            Add(new MathsProperty("=Log10(33.5) \n =Log10(Fields!NumberValue.Value)"));
            Add(new MathsProperty("=Max(Fields!YearlyIncome.Value) \n =Max(Fields!YearlyIncome.Value,\"AdventureWorks\") \n =Max(Fields!YearlyIncome.Value,\"AdventureWorks\",Recursive)"));
            Add(new MathsProperty("=Min(Fields!YearlyIncome.Value) \n =Min(Fields!YearlyIncome.Value,\"AdventureWorks\") \n =Min(Fields!YearlyIncome.Value,\"AdventureWorks\",Recursive)"));
            Add(new MathsProperty("=Pow(Fields!YearlyIncome.Value,2)"));
            Add(new MathsProperty("=Rnd() \n =Rnd(0) \n =Rnd(-1)"));
            Add(new MathsProperty("=Round(12.456) \n =Round(12.453,2) \n =Round(Fields!YearlyIncome.Value /12,2) \n =Round(2.5, System.MidpointRounding.AwayFromZero) \n =Round(2.5, System.MidpointRounding.ToEven)"));
            Add(new MathsProperty("=Sign(Fields!YearlyIncome.Value - 60000)"));
            Add(new MathsProperty("=Sin(90) \n =Sin(Fields!Angle.Value)"));
            Add(new MathsProperty("=Sinh(90) \n =Sinh(Fields!Angle.Value)"));
            Add(new MathsProperty("=Sqrt(144) \n =Sqrt(Fields!Area.Value)"));
            Add(new MathsProperty("=Tan(135) \n =Tan(Fields!Angle.Value)"));
            Add(new MathsProperty("=Tanh(135) \n =Tanh(Fields!Angle.Value)"));
        }
    }

    internal class MathsProperty
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

    internal class Inspection : ObservableCollection<InspectionProperty>
    {
        public Inspection()
            : base()
        {
            Add(new InspectionProperty("=IsArray(Parameters!Initials.Value)"));
            Add(new InspectionProperty("=IsDate(Fields!BirthDate.Value) \n =IsDate(\"31/12/2000\")"));
            Add(new InspectionProperty("=IsNothing(Fields!MiddleInitial.Value)"));
            Add(new InspectionProperty("=IsNumeric(Fields!YearlyIncome.Value)"));
        }
    }

    internal class InspectionProperty
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

    internal class Programflow : ObservableCollection<ProgramflowProperty>
    {
        public Programflow()
            : base()
        {
            Add(new ProgramflowProperty("=Choose(2,\"13\",\"15\",\"21\")  \n =Choose(Datepart(\"w\",Fields!BirthDate.Value),\"First\",\"Second\",\"Third\",\"Fourth\",\"Fifth\",\"Sixth\",\"Seventh\")"));
            Add(new ProgramflowProperty("=IIf(Fields!YearlyIncome.Value >= 60000,\"High\",\"Low\")"));
            Add(new ProgramflowProperty("=Switch(Fields!State.Value = \"OR\",\"Oregon\",Fields!State.Value = \"WA\",\"Washington\") \n =Switch(Fields!FirstName.Value = \"Sue\",\"Susan\",Fields!FirstName.Value = \"Bob\",\"Robert\")"));
        }
    }

    internal class ProgramflowProperty
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

    internal class Aggregate : ObservableCollection<AggregateProperty>
    {
        public Aggregate()
            : base()
        {
            Add(new AggregateProperty("=Avg(Fields!YearlyIncome.Value) \n =Avg(Fields!YearlyIncome.Value,\"GroupByGender\") \n =Avg(Fields!YearlyIncome.Value,\"GroupByGender\",Recursive)"));
            Add(new AggregateProperty("=Count(Fields!FirstName.Value) \n =Count(Fields!FirstName.Value,\"GroupByInitial\") \n =Count(Fields!FirstName.Value,\"GroupByInitial\",Recursive)"));
            Add(new AggregateProperty("=CountDistinct(Fields!MiddleInitial.Value) \n =CountDistinct(Fields!MiddleInitial.Value,\"GroupByInitial\" \n =CountDistinct(Fields!MiddleInitial.Value,\"GroupByInitial\",Recursive)"));
            Add(new AggregateProperty("=CountRows() \n =CountRows(\"GroupByInitial\") \n =CountRows(\"GroupByInitial\",Recursive)"));
            Add(new AggregateProperty("=First(Fields!MiddleInitial.Value = \"P\") \n =First(Fields!MiddleInitial.Value = Parameters!MiddleInitial.Value(0)) \n =First(Fields!MiddleInitial.Value,\"AdventureWorks\")"));
            Add(new AggregateProperty("=Last(Fields!MiddleInitial.Value = \"P\") \n =Last(Fields!MiddleInitial.Value = Parameters!MiddleInitial.Value(0)) \n =Last(Fields!MiddleInitial.Value,\"AdventureWorks\")"));
            Add(new AggregateProperty("=Max(Fields!YearlyIncome.Value) \n =Max(Fields!YearlyIncome.Value,\"AdventureWorks\") \n =Max(Fields!YearlyIncome.Value,\"AdventureWorks\",Recursive)"));
            Add(new AggregateProperty("=Min(Fields!YearlyIncome.Value) \n =Min(Fields!YearlyIncome.Value,\"AdventureWorks\") \n =Min(Fields!YearlyIncome.Value,\"AdventureWorks\",Recursive)"));
            Add(new AggregateProperty("=StDev(Fields!YearlyIncome.Value) \n =StDev(Fields!YearlyIncome.Value,\"GroupByInitial\") \n =StDev(Fields!YearlyIncome.Value,\"GroupByInitial\",Recursive)"));
            Add(new AggregateProperty("=StDevP(Fields!YearlyIncome.Value) \n =StDevP(Fields!YearlyIncome.Value,\"GroupByInitial\") \n =StDevP(Fields!YearlyIncome.Value,\"GroupByInitial\",Recursive)"));
            Add(new AggregateProperty("=Sum(Fields!YearlyIncome.Value) \n =Sum(Fields!YearlyIncome.Value,\"GroupByInitial\") \n =Sum(Fields!YearlyIncome.Value,\"GroupByInitial\",Recursive)"));
            Add(new AggregateProperty("=Var(Fields!YearlyIncome.Value) \n =Var(Fields!YearlyIncome.Value,\"GroupByInitial\") \n =Var(Fields!YearlyIncome.Value,\"GroupByInitial\",Recursive)"));
            Add(new AggregateProperty("=VarP(Fields!YearlyIncome.Value) \n =VarP(Fields!YearlyIncome.Value,\"GroupByInitial\") \n =VarP(Fields!YearlyIncome.Value,\"GroupByInitial\",Recursive)"));
            Add(new AggregateProperty("=RunningValue(Fields!YearlyIncome.Value,Sum,\"GroupByInitial\") \n =RunningValue(Fields!YearlyIncome.Value,Sum,\"AdventureWorks\")"));
            Add(new AggregateProperty("=Aggregate(Fields!Order_Count.Value)"));
        }
    }

    internal class AggregateProperty
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

    internal class Financial : ObservableCollection<FinancialProperty>
    {
        public Financial()
            : base()
        {
            Add(new FinancialProperty("=DDB(12000,100,12,6,2) \n =DDB(Fields!CostOfProperty.Value,100,12,6,2) \n =DDB(Fields!CostOfProperty.Value,Fields!Salvage.Value,Parameters!Life.Value,Parameters!Period.Value,2)"));
            Add(new FinancialProperty("=FV(.01,48,100,10,DueDate.EndOfPeriod) \n =FV(Parameters!Rate.Value,Parameters!NumberOfPayments.Value,Parameters!PaymentAmount.Value,Fields!PropertyCost.Value,DueDate.EndOfPeriod)"));
            Add(new FinancialProperty("=IPmt(.01,2,48,10000,0,DueDate.EndOfPeriod) \n =IPmt(Parameters!Rate.Value,Parameters!PaymentPeriod.Value,Parameters!NumberOfPayments.Value,Parameters!PresentValue.Value,0,DueDate.EndOfPeriod) \n =IPmt(Parameters!Rate.Value,Parameters!PaymentPeriod.Value,Parameters!NumberOfPayments.Value,Fields!PropertyCost.Value,0,DueDate.EndOfPeriod)"));
            Add(new FinancialProperty("=NPer(.01,100,10,1000,DueDate.EndOfPeriod) \n =NPer(Parameters!Rate.Value,Parameters!PaymentAmount.Value,Parameters!PresentValue.Value,0,DueDate.EndOfPeriod) \n =NPer(Parameters!Rate.Value,Parameters!PaymentAmount.Value,Fields!PropertyCost.Value,0,DueDate.EndOfPeriod)"));
            Add(new FinancialProperty("=Pmt(.01,48,10000,0,DueDate.EndOfPeriod) \n =Pmt(Parameters!Rate.Value,Parameters!NumberOfPayments.Value,Fields!PropertyCost.Value,0, DueDate.EndOfPeriod)"));
            Add(new FinancialProperty("=PPmt(.01,6,48,10000,0,DueDate.EndOfPeriod) \n =PPmt(Parameters!Rate.Value,Parameters!Period.Value,Parameters!NumberOfPayments.Value,Fields!PropertyCost.Value,0,DueDate.EndOfPeriod)"));
            Add(new FinancialProperty("=PV(.01,48,500,0,DueDate.EndOfPeriod) \n =PV(Parameters!Rate.Value,Parameters!NumberOfPayments.Value,Fields!PaymentAmount.Value,0,DueDate.EndOfPeriod) \n =PV(Parameters!Rate.Value,Parameters!NumberOfPayments.Value,Parameters!PaymentAmount.Value,0,DueDate.EndOfPeriod)"));
            Add(new FinancialProperty("=Rate(48,-500,10000,0,DueDate.EndOfPeriod,0.1) \n =Rate(Parameters!NumberOfPayments.Value,Parameters!PaymentAmount.Value,Parameters!PresentValue.Value,DueDate.EndOfPeriod,0.1)"));
            Add(new FinancialProperty("=SLN(10000,100,5) \n =SLN(Fields!PropertyCost.Value,Parameters!Salvage.Value,Parameters!Life.Value)"));
            Add(new FinancialProperty("=SYD(10000, 100,7,2) \n =SYD(Fields!PropertyCost.Value,Parameters!Salvage.Value,Parameters!Life.Value,Parameters!Period.Value)"));
        }
    }

    internal class FinancialProperty
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

    internal class Conversion : ObservableCollection<ConversionProperty>
    {
        public Conversion()
            : base()
        {
            Add(new ConversionProperty("=CBool(Fields!HouseOwnerFlag.Value)"));
            Add(new ConversionProperty("=CByte(Fields!NumberCarsOwned.Value) \n =CByte(Fields!Number.Value)"));
            Add(new ConversionProperty("=CChar(Fields!MaritalStatus.Value) \n =CChar(“ABC”))"));
            Add(new ConversionProperty("=CDate(Fields!BirthDate.Value) \n =CDate(\"October 2, 2001\")"));
            Add(new ConversionProperty("=CDbl(Fields!YearlyIncome.Value) \n =CDbl(573.45 * .4287 * 82561)"));
            Add(new ConversionProperty("=CDec(Fields!YearlyIncome.Value) \n =CDec(573.4503)"));
            Add(new ConversionProperty("=CInt(Fields!YearlyIncome.Value) \n =CInt(734.62)"));
            Add(new ConversionProperty("=CLng(Fields!YearlyIncome.Value) \n =CInt(73462.23)"));
            Add(new ConversionProperty("=CObj(Fields!YearlyIncome.Value) \n =CObj(73462.23)"));
            Add(new ConversionProperty("=CShort(Fields!NumberCarsOwned.Value) \n =CShort(200)"));
            Add(new ConversionProperty("=CSng(Fields!YearlyIncome.Value) \n =CSng(12.746522945)"));
            Add(new ConversionProperty("=CStr(Fields!YearlyIncome.Value) \n =CStr(12.746)"));
            Add(new ConversionProperty("=Fix(-9.25) \n =Fix(Fields!YearlyIncome.Value / -3)"));
            Add(new ConversionProperty("=Hex(Fields!CellColor.Value) \n =Hex(46)"));
            Add(new ConversionProperty("=Int(-93.4) \n =Int(Fields!YearlyIncome.Value / 12)"));
            Add(new ConversionProperty("=Oct(Fields!BitString.Value) \n =Oct(8)"));
            Add(new ConversionProperty("=Str(Fields!YearlyIncome.Value) \n =Str(352.75)"));
            Add(new ConversionProperty("=Val(Fields!AddressLine1.Value) \n =Val(\"6706 Ridgeview Dr\")"));
        }
    }

    internal class ConversionProperty
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

    internal class Miscellaneous : ObservableCollection<MiscellaneousProperty>
    {
        public Miscellaneous()
            : base()
        {
            Add(new MiscellaneousProperty("=InScope(\"table1_Group1\") \n =InScope(\"GroupByInitial\")"));
            Add(new MiscellaneousProperty("=Level() \n =Level(\"GroupByInitial\")"));
            Add(new MiscellaneousProperty("=Previous(Fields!FirstName.Value)"));
            Add(new MiscellaneousProperty("=RowNumber(\"AdventureWorks\")"));
        }
    }

    internal class MiscellaneousProperty
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