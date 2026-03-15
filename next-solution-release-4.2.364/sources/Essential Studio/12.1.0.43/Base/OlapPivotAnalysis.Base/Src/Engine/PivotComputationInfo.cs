#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if XLSIO
using System.Runtime.Serialization.Formatters;
namespace Syncfusion.XlsIO.Implementation.PivotAnalysis
#elif !SILVERLIGHT
using System.Runtime.Serialization.Formatters;
namespace Syncfusion.PivotAnalysis.Base
#else
namespace Syncfusion.PivotAnalysis.Base.Silverlight
#endif
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Linq.Expressions;
    using System.Collections.ObjectModel;
    using System.Reflection;

    #region PivotComputationInfo class

    /// <summary>
    /// This class holds the information needed for the calculations that appear in a Pivot Grid. For each calculation seen, there is an associated
    /// PivotComputationInfo object that is added to the <see cref="PivotComputationInfo"/> collection.
    /// </summary>
#if !SILVERLIGHT
    public class PivotComputationInfo : INotifyPropertyChanged, IXmlSerializable
#else
    public class PivotComputationInfo : INotifyPropertyChanged
#endif
    {
        #region Initilize/Finalize
        /// <summary>
        /// Performs certain Manipulations with data
        /// </summary>
        public PivotComputationInfo()
        {
            this.SummaryType = SummaryType.Count;
        }

        #endregion

        #region properties

        private bool allowSort = false;

        /// <summary>
        /// Gets or sets whether this calculation column can be sorted when
        /// RowPivotsOnly is true in the PivotEngine. 
        /// </summary>
        public bool AllowSort
        {
            get { return allowSort; }
            set { allowSort = value; }
        }
        private bool allowFilter = false;

        /// <summary>
        /// Gets or sets whether this calculation column can be filtered when
        /// RowPivotsOnly is true in the PivotEngine. 
        /// </summary>
        public bool AllowFilter
        {
            get { return allowFilter; }
            set { allowFilter = value; }
        }

        private bool enableHyperLinks = false;

        /// <summary>
        /// Gets or sets whether this calculation column should be hyperlinked when RowPivotsOnly is true in the PivotEngine.
        /// </summary>
        public bool EnableHyperlinks
        {
            get { return enableHyperLinks; }
            set { enableHyperLinks = value; }
        }

        private string fieldName;
        private string fieldHeader = string.Empty;
        /// <summary>
        /// Gets or sets the name of the property to be used in this calculation.
        /// </summary>
        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                fieldName = value;
                this.OnPropertyChanged(pi => pi.FieldName);
            }
        }

        /// <summary>
        /// Gets or sets the title you want to see in the header for this pivot item.
        /// </summary>
        /// <value>The field header.</value>
        public string FieldHeader
        {
            get
            {
                return fieldHeader;
            }
            set
            {
                fieldHeader = value;
                this.OnPropertyChanged(pi => pi.FieldHeader);
            }
        }

        private string description;

        /// <summary>
        /// Gets or sets a description of the calculation.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                this.OnPropertyChanged(pi => pi.Description);
            }
        }

        private SummaryDisplayLevel innerMostComputationsOnly = SummaryDisplayLevel.All;

        /// <summary>
        /// Gets or sets whether the aggregation results appear for only the innermost
        /// level. 
        /// </summary>
        public SummaryDisplayLevel InnerMostComputationsOnly
        {
            get { return innerMostComputationsOnly; }
            set { innerMostComputationsOnly = value; this.OnPropertyChanged(pi => pi.PadString); }
        }


        private string padString = "*";

        /// <summary>
        /// Gets or sets the PadString used when SummaryType is DisplayIfDiscreteValuesEqual.
        /// </summary>
        public string PadString
        {
            get
            {
                return padString;
            }
            set
            {
                padString = value;
                //if you change the padString && summary has already been set to DisplayIfDiscreteValuesEqual,
                //then you need to also set summary.PadString.
                if (summary != null && summary is DisplayIfDiscreteValuesEqual)
                {
                    ((DisplayIfDiscreteValuesEqual)summary).PadString = padString;
                }
                this.OnPropertyChanged(pi => pi.PadString);
            }
        }

        private string calculationName;

        /// <summary>
        /// Gets or sets what is displayed in the pivot table if more than one calculation is included in the Pivot Grid.
        /// </summary>
        public string CalculationName
        {
            get
            {
                return calculationName;
            }
            set
            {
                calculationName = value;
                this.OnPropertyChanged(pi => pi.CalculationName);
            }
        }

        private bool allowRunTimeGroupByField = true;
        /// <summary>
        /// Gets or sets the value to enable/disable grouping for this pivot item. Default value is true.
        /// </summary>
        public bool AllowRunTimeGroupByField
        {
            get 
            {
                return allowRunTimeGroupByField;
            }
            set 
            {
                if (Formula != null)
                {
                    allowRunTimeGroupByField = false;
                }
                else if (allowRunTimeGroupByField != value)
                {
                    allowRunTimeGroupByField = value;

                } 
                this.OnPropertyChanged(pi => pi.AllowRunTimeGroupByField);
            }
        }

        private SummaryBase summary;

        /// <summary>
        /// Gets or sets the <see cref="SummaryBase"/> object that is used to define this calculation. This value is altomatically set 
        /// when you specify any non-custom value of <see cref="SummaryType"/>. If you specify SummaryType.Custom, then you are required
        /// to set Summary to be an instance of your custom SummaryBase derived object.
        /// </summary>
        [XmlIgnore]
        public SummaryBase Summary
        {
            get
            {
                return summary;
            }
            set
            {
                summary = value;
                this.OnPropertyChanged(pi => pi.Summary);
            }
        }

        SummaryType _SummaryType;
        /// <summary>
        /// Gets or sets the SummaryType enumeration for this calculation. Setting it to any value of than Custom
        /// will also properly set Summary.
        /// </summary>
        [DefaultValue(SummaryType.Count)]
        public SummaryType SummaryType
        {
            get
            {
                return _SummaryType;
            }
            set
            {
                _SummaryType = value;
#if !SILVERLIGHT
                if (!(_SummaryType == SummaryType.Custom))
#endif 
                {
                    this.summary = GetSummaryInstance(_SummaryType);
                    if (this._SummaryType == SummaryType.DisplayIfDiscreteValuesEqual)
                    {
                        ((DisplayIfDiscreteValuesEqual)this.summary).PadString = this.PadString;
                    }
                    this.OnPropertyChanged(pi => pi.SummaryType);
                }
            }
        }

        private CalculationType _calculationType;

        /// <summary>
        /// Gets or sets the CalculationType enumeration for this computation object.
        /// </summary>
        [DefaultValue(CalculationType.NoCalculation)]
        public CalculationType CalculationType
        {
            get { return _calculationType; }
            set
            {
                _calculationType = value;
                this.OnPropertyChanged(pi => pi.CalculationType);
            }
        }

        private string _baseField;
        /// <summary>
        /// Gets or sets the Base Field value for calculations.
        /// </summary>
        public string BaseField
        {
            get { return _baseField; }
            set
            {
                _baseField = value;
                this.OnPropertyChanged(pi => pi.BaseField);
            }
        }
        private string _formula;
         /// <summary>
         /// Gets or sets a formula that defines the value as an algebraic expresion of other computations where these 
         /// computations are referenced by their <see cref="CalculationName"/> enclosed within square brackets.
         /// </summary>
         /// <remarks>
         /// For example is the value of Formula is "[AvgPrice]-[AvgCost]", then there would need to be two other computations whose names are
         /// AvgPrice and AvgCost, and the value displayed for this computation would be the difference between those two values.
         /// </remarks>
         public string Formula
         {
             get { return _formula; }
             set { _formula = value; }
         }

        private FilterExpression _expression;

        /// <summary>
        /// Used internally for Formula calculation types.
        /// </summary>
        [XmlIgnore]
        public FilterExpression Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        /// <summary>
        /// Returns a <see cref="SummaryBase"/> object of the specified <see cref="SummaryType"/>.
        /// </summary>
        /// <param name="st">The SummaryType.</param>
        /// <returns>A SummaryBase object.</returns>
        public static SummaryBase GetSummaryInstance(SummaryType st)
        {
            SummaryBase sb = null;
            switch (st)
            {
                case SummaryType.IntTotalSum:
                    sb = new IntTotalSummary();
                    break;
                case SummaryType.DecimalTotalSum:
                    sb = new DecimalTotalSummary();
                    break;
                case SummaryType.DoubleTotalSum:
                    sb = new DoubleTotalSummary();
                    break;
                case SummaryType.DoubleAverage:
                    sb = new DoubleAverageSummary();
                    break;
                case SummaryType.Count:
                    sb = new CountSummary();
                    break;
                case SummaryType.DoubleMinimum:
                    sb = new DoubleMinSummary();
                    break;
                case SummaryType.DoubleMaximum:
                    sb = new DoubleMaxSummary();
                    break;
                case SummaryType.DoubleStandardDeviation:
                    sb = new DoubleStDevSummary();
                    break;
                case SummaryType.DoubleVariance:
                    sb = new DoubleVarianceSummary();
                    break;
                case SummaryType.DisplayIfDiscreteValuesEqual:
                    sb = new DisplayIfDiscreteValuesEqual();
                    break;
                case SummaryType.Custom:
                default:
                    break;
            }
            return sb;
        }

        private string format = "#.##";

        /// <summary>
        /// Gets of sets the format string to be used to format this calculation results in the Pivot Grid. Default format string is #.##
        /// </summary>
        public string Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
                this.OnPropertyChanged(pi => pi.Format);
            }
        }

        private object defaultValue;

        /// <summary>
        /// Gets or sets the default value to be used if this summary calculation gets null value.
        /// </summary>        
        public object DefaultValue
        {
            get { return defaultValue; }
            set
            {
                defaultValue = value;
                this.OnPropertyChanged(pi => pi.DefaultValue);
            }
        }


        #endregion

        #region Overrides
        /// <summary>
        /// Converts to String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.CalculationName;
        }

        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Event denoting whether the property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged<R>(Expression<Func<PivotComputationInfo, R>> expr)
        {
            OnPropertyChanged(((MemberExpression)expr.Body).Member.Name);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Returns a sorted list of computation names based on the <see cref="SummaryType"/> enumerations.
        /// </summary>
        /// <returns>A list of computation names.</returns>
        public static List<string> GetComputationTypes()
        {
#if SILVERLIGHT
            List<string> list = new List<string>(GetEnumNames(typeof(SummaryType)));
#else

            List<string> list = new List<string>(Enum.GetNames(typeof(SummaryType)));
#endif
            list.Remove("Custom");
            list.Sort();
            return list;
        }

#if SILVERLIGHT
        private static string[] GetEnumNames(Type t)
        {
            List<string> enumNames = new List<string>();
            foreach (System.Reflection.FieldInfo fi in t.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumNames.Add(fi.Name);
            }
            return enumNames.ToArray();
        } 
#endif

#if !SILVERLIGHT
        #region IXmlSerializable Methods

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            if (!reader.HasAttributes)
            {
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.Name == "CalculationName")
                        this.CalculationName = reader.ReadElementContentAsString();
                    else if (reader.Name == "FieldName")
                        this.FieldName = reader.ReadElementContentAsString();
                    else if (reader.Name == "Description")
                        this.Description = reader.ReadElementContentAsString();
                    else if (reader.Name == "SummaryType")
                        this.SummaryType = (SummaryType)Enum.Parse(typeof(SummaryType), reader.ReadElementContentAsString());
                    else if (reader.Name == "Format")
                        this.Format = reader.ReadElementContentAsString();
                    else if (reader.Name == "PadString")
                        this.PadString = reader.ReadElementContentAsString();
                }
                reader.Read();
            }
            else
            {
                this.FieldHeader = reader.GetAttribute("FieldHeader");
                this.FieldName = reader.GetAttribute("FieldName");
                this.PadString = reader.GetAttribute("PadString");
                this.SummaryType = (SummaryType)Enum.Parse(typeof(SummaryType), reader.GetAttribute("SummaryType"), false);
                this.Format = reader.GetAttribute("Format");
                this.Description = reader.GetAttribute("Description");
                this.DefaultValue = reader.GetAttribute("DefaultValue");
                if (reader.GetAttribute("AllowRunTimeGroupByField") != null)
                    this.AllowRunTimeGroupByField = Boolean.Parse(reader.GetAttribute("AllowRunTimeGroupByField"));
                if (reader.GetAttribute("InnerMostComputationsOnly") != null)
                    this.InnerMostComputationsOnly = (SummaryDisplayLevel)Enum.Parse(typeof(SummaryDisplayLevel), reader.GetAttribute("InnerMostComputationsOnly"), false);
                if (this.SummaryType == SummaryType.Custom)
                {
                    reader.ReadStartElement("PivotComputationInfo");
                    reader.ReadStartElement("CustomType");
                    reader.ReadStartElement("Summary");
                    Type summaryType = Type.GetType(reader.ReadString());
                    reader.ReadEndElement();
                    reader.ReadStartElement("Value");
                    XmlSerializer deSerializer = new XmlSerializer(summaryType);
                    this.Summary = deSerializer.Deserialize(reader) as SummaryBase;
                    reader.ReadEndElement();
                    reader.ReadEndElement();
                }
                reader.Read();
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("FieldHeader", this.FieldHeader);
            writer.WriteAttributeString("FieldName", this.FieldName);
            writer.WriteAttributeString("PadString", this.PadString);
            writer.WriteAttributeString("SummaryType", this.SummaryType.ToString());
            writer.WriteAttributeString("Format", this.Format);
            writer.WriteAttributeString("Description", this.Description);
            writer.WriteAttributeString("AllowRunTimeGroupByField", this.AllowRunTimeGroupByField.ToString());
            writer.WriteAttributeString("InnerMostComputationsOnly", this.InnerMostComputationsOnly.ToString());
            if (this.DefaultValue != null)
            {
                writer.WriteAttributeString("DefaultValue", this.DefaultValue.ToString());
            }

            if (this.SummaryType == SummaryType.Custom)
            {
                writer.WriteStartElement("CustomType");
                writer.WriteStartElement("Summary");
                writer.WriteValue(this.Summary.GetType().AssemblyQualifiedName);
                writer.WriteEndElement();
                writer.WriteStartElement("Value");
                XmlSerializer valueSerializer = new XmlSerializer(this.Summary.GetType());
                valueSerializer.Serialize(writer, this.Summary);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        #endregion
#endif
    }

    #endregion

    #region SummaryDisplayLevel enums

    /// <summary>
    /// Controls whether a Summary calculation is displayed for all levels, or for the inner-most level only.
    /// </summary>
    public enum SummaryDisplayLevel
    {
        /// <summary>
        /// Indicates to display the summary at all pivot levels.
        /// </summary>
        All,
        /// <summary>
        /// Indicates to display the summary at only the inner most pivot level.
        /// </summary>
        InnerMostOnly
    }

    #endregion

    #region SummaryType enums

    /// <summary>
    /// Enumerates the summary types availabe for use as calculations in the Pivot Grid.
    /// </summary>
    /// <remarks>
    /// If you use the value Custom in a ComputationInfo object, then you are required to explicitly set the ComputationInfo.Summary value.
    /// </remarks>
    public enum SummaryType
    {
        /// <summary>
        /// Computes the sum of double or integer values.
        /// </summary>
        DoubleTotalSum,
        /// <summary>
        /// Computes the simple average of double or integer values.
        /// </summary>
        DoubleAverage,
        /// <summary>
        /// Computes the maximum of double or integer values.
        /// </summary>
        DoubleMaximum,
        /// <summary>
        /// Computes the minimum of double or integer values.
        /// </summary>
        DoubleMinimum,
        /// <summary>
        /// Computes the standard deviation of double or integer values.
        /// </summary>
        DoubleStandardDeviation,
        /// <summary>
        /// Computes the variance of double or integer values.
        /// </summary>
        DoubleVariance,
        /// <summary>
        /// Computes the count of double or integer values.
        /// </summary>
        Count,
        /// <summary>
        /// Computes the sum of decimal values.
        /// </summary>
        DecimalTotalSum,
        /// <summary>
        /// Computes the sum of integer values.
        /// </summary>
        IntTotalSum,
        /// <summary>
        /// Specifies that you are using a custom SummaryBase object to define the calculation.
        /// </summary>
        Custom,
        /// <summary>
        /// Displays the common value if all the values to be aggregated are the same, and displays a <see cref="DisplayIfDiscreteValuesEqual"/>
        /// if the values to be aggregated are not all the same.
        /// </summary>
        DisplayIfDiscreteValuesEqual
    }

    #endregion

    #region CalculationType enums
    /// <summary>
    /// Calculation type defines the view for a particular computational object (Or value field).
    /// </summary>
    public enum CalculationType
    {
        /// <summary>
        /// Remove the custom calculations and restore to original values (Default value).
        /// </summary>
        NoCalculation,
        /// <summary>
        /// Displays a value cell as a percentage of grand total of all value cells of Pivot Engine.
        /// </summary>
        PercentageOfGrandTotal,
        /// <summary>
        /// Displays all value cells in each column as a percentage of its corresponding column total.
        /// </summary>
        PercentageOfColumnTotal,
        /// <summary>
        /// Displays all value cells in each row as a percentage of its corresponding row total.
        /// </summary>
        PercentageOfRowTotal,
        /// <summary>
        /// Displays a value cell as a percentage of parent column item values.
        /// </summary>
        PercentageOfParentColumnTotal,
        /// <summary>
        /// Displays a value cell as a percentage of parent row item values.
        /// </summary>
        PercentageOfParentRowTotal,
        /// <summary>
        /// Displays a value cell as a percentage of Base Field (Parent Row/Column Total).
        /// </summary>
        PercentageOfParentTotal,
        /// <summary>
        /// Displays a value cell as an index value based on PivotEngine generation.
        /// </summary>
        Index,
        /// <summary>
        /// Displays a calculation based on a well formed algebraic expression involving other calculations.
        /// </summary>
        Formula
    }

    #endregion

    #region SummaryBase class
    /// <summary>
    /// This class is an abstract class that defines the necessary functionality to do pivot calculations.
    /// </summary>
    public abstract class SummaryBase
    {
        /// <summary>
        /// Use this method to combine a value from an object in the Pivot Grid's data source with the accumulation values held in this instance.
        /// </summary>
        /// <param name="other">The value to be included in the compuation.</param>
        public abstract void Combine(object other);

        /// <summary>
        /// Resets all internal values so the calculations begins anew.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Returns the calculation value.
        /// </summary>
        /// <returns>The calculation value.</returns>
        public abstract object GetResult();

        /// <summary>
        /// Provides a new instance of this SummaryBase object.
        /// </summary>
        /// <returns>New instance of this SummaryBase.</returns>
        public abstract SummaryBase GetInstance();

        /// <summary>
        /// Use this method to combine another SummaryBase object with the accumulation values held in this instance.
        /// </summary>
        /// <param name="other">The other SummaryBase object.</param>
        public abstract void CombineSummary(SummaryBase other);

        private bool _showNullAsBlank = false;
        /// <summary>
        /// Gets or sets whether the PivotGrid cell should display Null value as blank instead of 0(which is the default behavior)
        /// </summary>
        public bool ShowNullAsBlank
        {
            get
            {
                return _showNullAsBlank;
            }
            set
            {
                _showNullAsBlank = value;
            }
        }
       
    }

    #endregion

    #region IAdjustableSummary
    /// <summary>
    /// Use this interface to enable short cut calculations to adjust a summary when an underlying
    /// value changes. The idea is to avoid recomputing the summary from scratch. For example, if 
    /// your summary computes the total of a set of values, you can quickly adjust this computed
    /// total when a value changes by subtracting the old value and adding the new value without
    /// having to recompute the total from scratch. Not all types of calculations lend themselves
    /// to this short cut behavior.
    /// </summary>
    public interface IAdjustable
    {
        /// <summary>
        /// Adjusts the summary for a change in one of its underlying values.
        /// </summary>
        /// <param name="newContribution">The new value of the changed field.</param>
        void AdjustForNewContribution(object newContribution);

        /// <summary>
        /// Adjusts the summary for a change in one of its underlying values.
        /// </summary>
        /// <param name="oldContribution">The old value of the changed field.</param>
        void AdjustForOldContribution(object oldContribution);
    }
    #endregion

    #region CountSummary
    /// <summary>
    /// Counts the summary values
    /// </summary>
    public class CountSummary : SummaryBase, IAdjustable
    {
        internal int? count = null;
        /// <summary>
        /// Converts the data to string
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return "Count";
        }
        /// <summary>
        /// Comblnes different values into one
        /// </summary>
        /// <param name="other"></param>
        public override void Combine(object other)
        {
            count = count ?? 0;               
           
#if SILVERLIGHT
            if (other == null)
#else
            if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank)
                    count = null;
                else
                    count = 0;
            }
            else
            {
                count++;
            }
        }
        /// <summary>
        /// Combines the summary into one
        /// </summary>
        /// <param name="other">SummaryBase</param>
        public override void CombineSummary(SummaryBase other)
        {
            count = count ?? 0;
            if (other.GetResult() == null)
                count += 0;
            else
                count += ((CountSummary)other).count;            
        }
        /// <summary>
        /// Resets teh variable
        /// </summary>
        public override void Reset()
        {
            count = null;
        }
        /// <summary>
        /// Obtains the result
        /// </summary>
        /// <returns></returns>
        public override object GetResult()
        {
            return count;
        }
        /// <summary>
        /// Gets the Instance of the Class
        /// </summary>
        /// <returns></returns>
        public override SummaryBase GetInstance()
        {
            return new CountSummary();
        }

        #region IAdjustable Members
        /// <summary>
        /// AdjustForNewContribution
        /// </summary>
        /// <param name="newContribution">object</param>
        public void AdjustForNewContribution(object newContribution)
        {	 
            count = count ?? 0;
            count++;
        }
        /// <summary>
        /// AdjustForOldContribution
        /// </summary>
        /// <param name="oldContribution">object</param>
        public void AdjustForOldContribution(object oldContribution)
        {	 
            count = count ?? 0;
            count--;
        }

        #endregion
    }

    #endregion

    #region double summaries

    #region DoubleMinSummary
    /// <summary>
    /// Calculates the Double Minimum summary
    /// </summary>
    public class DoubleMinSummary : SummaryBase
    {
        /// <summary>
        /// Converts the value to String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "DoubleMinimum";
        }

        internal double? min = null;
        /// <summary>
        /// Combines the different values together
        /// </summary>
        /// <param name="other">object</param>
        public override void Combine(object other)
        {
            min = min ?? double.MaxValue;
            double d = double.MaxValue;
            if (other is double)
            {
                d = (double)other;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                d = (double)Convert.ChangeType(other, typeof(double) , null);
#else
                d = (double)Convert.ChangeType(other, typeof(double));
#endif
            }

#if SILVERLIGHT
            else if (other == null)
#else
            else if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank && min == double.MaxValue)
                    min = null;
            }
            else if (other is string)
            {
                double value;
                if (double.TryParse(other.ToString(), out value))
                    d = value;

            }
            if (min > d)
                min = d;

        }
        /// <summary>
        /// Combines the different Summary into one
        /// </summary>
        /// <param name="other">SummaryBase</param>
        public override void CombineSummary(SummaryBase other)
        {
            min = min ?? double.MaxValue;
            DoubleMinSummary asum = (DoubleMinSummary)other;
            if (min > asum.min)
                min = asum.min;
        }
        /// <summary>
        /// Resets teh variable
        /// </summary>
        public override void Reset()
        {
            min = null;
        }
        /// <summary>
        /// Calculates the result
        /// </summary>
        /// <returns></returns>
        public override object GetResult()
        {
            return min;
        }
        /// <summary>
        /// Gets the insance
        /// </summary>
        /// <returns>DoubleMinSummary</returns>
        public override SummaryBase GetInstance()
        {
            return new DoubleMinSummary();
        }
    }

    #endregion

    #region DoubleMaxSummary
    /// <summary>
    /// Class that performs the operation related to maximum summary values.
    /// </summary>
    public class DoubleMaxSummary : SummaryBase
    {
        /// <summary>
        /// Converts the value to string type
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return "DoubleMaximum";
        }

        internal double? max = null;

        /// <summary>
        /// Combines the different values together
        /// </summary>
        /// <param name="other">object</param>
        public override void Combine(object other)
        {
            max = max ?? double.MinValue;
            double d = 0d;
            if (other is double)
            {
                d = (double)other;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                d = (double)Convert.ChangeType(other, typeof(double), null);
#else
                d = (double)Convert.ChangeType(other, typeof(double));
#endif
            }

#if SILVERLIGHT
            else if (other == null)
#else
            else if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank && max == double.MinValue)
                    max = null;
            }
            else if (other is string)
            {
                double value;
                if (double.TryParse(other.ToString(), out value))
                    d = value;

            }
            if (max < d)
                max = d;

        }

        /// <summary>
        /// Combines the different summaries together
        /// </summary>
        /// <param name="other"></param>
        public override void CombineSummary(SummaryBase other)
        {
            max = max ?? double.MinValue;
            DoubleMaxSummary asum = (DoubleMaxSummary)other;
            if (max < asum.max)
                max = asum.max;
        }
        /// <summary>
        /// Resets the value of the variables
        /// </summary>
        public override void Reset()
        {
            max = null;
        }

        /// <summary>
        /// Obtains the Maximum value 
        /// </summary>
        /// <returns></returns>
        public override object GetResult()
        {
            return max;
        }
        /// <summary>
        /// returns the instance of type DoubleMaxSummary
        /// </summary>
        /// <returns></returns>
        public override SummaryBase GetInstance()
        {
            return new DoubleMaxSummary();
        }
    }

    #endregion

    #region DoubleStDevSummary

    //formula used is for sample standard deviation (as per excel)
    // sqrt(sum((x-xbar)*(x-xbar)) / (n-1) )
    // or the equivalent form is used below...
    // sqrt(sum(x*x) - n * xbar * xbar) / (n-1))

    /// <summary>
    /// Class that holds different functions to perform operations on summary row
    /// </summary>
    public class DoubleStDevSummary : SummaryBase
    {
        /// <summary>
        /// Converts the Data to String type
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return "DoubleStandardDeviation";
        }

        internal double? sumX2 = null;
        internal double? sumX = null;
        internal int? n = null;
        /// <summary>
        /// Combines the different values together
        /// </summary>
        /// <param name="other">object</param>
        public override void Combine(object other)
        {
            sumX2 = sumX2 ?? 0d;
            sumX = sumX ?? 0d;
            n = n ?? 0;
            double d = 0d;
            if (other is double)
            {
                d = (double)other;
                n++;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                d = (double)Convert.ChangeType(other, typeof(double), null);
#else
                d = (double)Convert.ChangeType(other, typeof(double));
#endif
                n++;
            }
            else if (other is string)
            {
                double value;
                if (double.TryParse(other.ToString(), out value))
                    d = value;
                n++;

            }

#if SILVERLIGHT
            else if (other == null)
#else
            else if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank)
                {
                    if (sumX2 == 0d)
                        sumX2 = null;
                    if (sumX == 0d)
                        sumX = null;
                    n = null;
                }
            }
            sumX2 += d * d;
            sumX += d;

        }
        /// <summary>
        /// Combines the different summaries together
        /// </summary>
        /// <param name="other"></param>
        public override void CombineSummary(SummaryBase other)
        {
            sumX2 = sumX2 ?? 0d;
            sumX = sumX ?? 0d;
            n = n ?? 0;
            if (other.GetResult() == null)
            {
                sumX2 += 0d;
                sumX += 0d;
                n += 0;
            }
            else
            {
                DoubleStDevSummary asum = (DoubleStDevSummary)other;
                sumX2 += asum.sumX2;
                sumX += asum.sumX;
                n += asum.n;
            }
        }
        /// <summary>
        /// Resets the Value of the variables.
        /// </summary>
        public override void Reset()
        {
            sumX2 = null;
            sumX = null;
            n = null;
        }
        /// <summary>
        /// Obtains the result
        /// </summary>
        /// <returns>object</returns>
        public override object GetResult()
        {
           if (n < 2)
                return double.NaN;            
           if (sumX != null && sumX != null && n != null)
               return Math.Sqrt((double)((sumX2 - sumX * sumX / n) / (n - 1)));           
           return null;
        }
        /// <summary>
        ///Gets the Instance of the Class
        /// </summary>
        /// <returns>SummaryBase</returns>
        public override SummaryBase GetInstance()
        {
            return new DoubleStDevSummary();
        }
    }

    #endregion

    #region DoubleVarianceSummary
    /// <summary>
    /// class that performs methods related to DoubleVarianceSummary
    /// </summary>
    public class DoubleVarianceSummary : SummaryBase
    {
        /// <summary>
        /// Returns the Variable in String Format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "DoubleVariance";
        }

        internal double? sumX2 = null;
        internal double? sumX = null;
        internal int? n = null;
        /// <summary>
        /// Combines the different values together
        /// </summary>
        /// <param name="other"></param>
        public override void Combine(object other)
        {
            sumX2 = sumX2 ?? 0d;
            sumX = sumX ?? 0d;
            n = n ?? 0;
            double d = 0d;
            if (other is double)
            {
                d = (double)other;
                n++;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                d = (double)Convert.ChangeType(other, typeof(double), null);
#else
                d = (double)Convert.ChangeType(other, typeof(double));
#endif
                n++;
            }
            else if (other is string)
            {
                double value;
                if (double.TryParse(other.ToString(), out value))
                    d = value;
                n++;

            }

#if SILVERLIGHT
            else if (other == null)
#else
            else if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank)
                {
                    if (sumX2 == 0d)
                        sumX2 = null;
                    if (sumX == 0d)
                        sumX = null;
                    n = null;
                }
            }
            sumX2 += d * d;
            sumX += d;

        }
        /// <summary>
        /// Combines the different summaries together
        /// </summary>
        /// <param name="other">SummaryBase</param>
        public override void CombineSummary(SummaryBase other)
        {
            sumX2 =sumX2??0d;
            sumX =sumX??0d;
            n=n??0;           
            if (other.GetResult() == null)
            {
                sumX2 += 0d;
                sumX += 0d;
                n += 0;
            }
            else
            {
                DoubleVarianceSummary asum = (DoubleVarianceSummary)other;
                sumX2 += asum.sumX2;
                sumX += asum.sumX;
                n += asum.n;
            }            
        }
        /// <summary>
        /// Resets sumX2,sumX,n to Null
        /// </summary>
        public override void Reset()
        {            
            sumX2 = null;
            sumX = null;
            n = null;
        }
        /// <summary>
        /// Gets the result
        /// </summary>
        /// <returns>object</returns>
        public override object GetResult()
        {
            if (n < 2)
                return double.NaN;
            return (sumX2 - sumX * sumX / n) / (n - 1);
        }
        /// <summary>
        /// Gets the instance of the Summary
        /// </summary>
        /// <returns></returns>
        public override SummaryBase GetInstance()
        {
            return new DoubleVarianceSummary();
        }
    }

    #endregion

    #region DoubleAverageSummary

    /// <summary>
    /// Performs computation and calculates the Average Summary.
    /// </summary>
    public class DoubleAverageSummary : SummaryBase, IAdjustable
    {
        /// <summary>
        /// Converts the data to String type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "DoubleAverage";
        }

        internal double? total = null;
        internal int? count = null;
        /// <summary>
        /// Merges the values
        /// </summary>
        /// <param name="other"></param>
        public override void Combine(object other)
        {
            total = total ?? 0d;
            count = count ?? 0;
            if (other is double)
            {
                total += (double)other;
                count++;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                total += (double)Convert.ChangeType(other, typeof(double), null);
#else
                total += (double)Convert.ChangeType(other, typeof(double));
#endif
                count++;
            }
            else if (other is string)
            {
                double value;
                if (double.TryParse(other.ToString(), out value))
                total += value;
                count++;

            }

#if SILVERLIGHT
            else if (other == null)
#else
            else if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank)
                {
                    if (total == 0d)
                        total = null;
                    if (count == 0d)
                        count = null;
                }
            }

        }
        /// <summary>
        /// Combines different summaries into one
        /// </summary>
        /// <param name="other">SummaryBase</param>
        public override void CombineSummary(SummaryBase other)
        {
            total = total ?? 0d;
            count = count ?? 0;    
            if (other.GetResult() == null)
            {               
                    total += 0d;
                    count += 0;
            }
            else
            {
                DoubleAverageSummary asum = (DoubleAverageSummary)other;            
                total += asum.total;
                count += asum.count;                    
            }            
        }
        /// <summary>
        /// Resets the total and count
        /// </summary>
        public override void Reset()
        {
            total = null;
            count = null;
        }
        /// <summary>
        /// Gets the result
        /// </summary>
        /// <returns>result</returns>
        public override object GetResult()
        {
            if (count == 0)
                return double.NaN;
            return total / count;
        }
        /// <summary>
        /// Gets the Instance of the object
        /// </summary>
        /// <returns>DoubleAverageSummary</returns>
        public override SummaryBase GetInstance()
        {
            return new DoubleAverageSummary();
        }

        #region IAdjustable Members
        /// <summary>
        /// AdjustForNewContribution
        /// </summary>
        /// <param name="newContribution">object</param>
        public void AdjustForNewContribution(object newContribution)
        {		
               total = total ?? 0.0d;
            if(newContribution != null)
#if SILVERLIGHT
                total += (double)Convert.ChangeType(newContribution, typeof(double), null);
#else
            total += (double)Convert.ChangeType(newContribution, typeof(double));
#endif
        }
        /// <summary>
        /// AdjustForOldContribution
        /// </summary>
        /// <param name="oldContribution">object</param>
        public void AdjustForOldContribution(object oldContribution)
        { 
               total = total ?? 0.0d;
            if(oldContribution != null)
#if SILVERLIGHT
                total -= (double)Convert.ChangeType(oldContribution, typeof(double), null);
#else
                total -= (double)Convert.ChangeType(oldContribution, typeof(double));
#endif
        }

        #endregion
    }

    #endregion

    #region DoubleTotalSummary
    /// <summary>
    /// Doubles the Total Summary
    /// </summary>
    public class DoubleTotalSummary : SummaryBase, IAdjustable
    {
        /// <summary>
        /// Returns the string for the concerned data
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return "DoubleTotal";
        }

        internal double? total = null;
        /// <summary>
        /// Comblies one with other type
        /// </summary>
        /// <param name="other"></param>
        public override void Combine(object other)
        {

            total = total ?? 0.0d;

            if (other is double)
            {
                total += (double)other;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                total += (double)Convert.ChangeType(other, typeof(double), null);
#else
                total += (double)Convert.ChangeType(other, typeof(double));
#endif
            }

#if SILVERLIGHT
            else if (other == null)
#else
            else if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank && total == 0.0d)
                    total = null;
            }
            else if (other is string)
            {
                double value;
                if (double.TryParse(other.ToString(), out value))
                    total += value;
            }
        }
        /// <summary>
        /// Combines different Summaries
        /// </summary>
        /// <param name="other">SummaryBase</param>
        public override void CombineSummary(SummaryBase other)
        {
            if (other.GetResult() == null)
                total += 0.0;
            else
                Combine(((DoubleTotalSummary)other).total);
        }
        /// <summary>
        /// Resets the total to Null
        /// </summary>
        public override void Reset()
        {
            total = null;
        }
        /// <summary>
        /// Gets the toal value
        /// </summary>
        /// <returns>object</returns>
        public override object GetResult()
        {
            return total;
        }
        /// <summary>
        /// Gets the Instance of the Class
        /// </summary>
        /// <returns></returns>
        public override SummaryBase GetInstance()
        {
            return new DoubleTotalSummary();
        }

        #region IAdjustable Members
        /// <summary>
        /// AdjustForNewContribution
        /// </summary>
        /// <param name="newContribution">object</param>
        public void AdjustForNewContribution(object newContribution)
        {
            total = total ?? 0.0d;
#if SILVERLIGHT
                total += (double)Convert.ChangeType(newContribution, typeof(double), null);
                
#else
            total += (double)Convert.ChangeType(newContribution, typeof(double));

#endif
        }
        /// <summary>
        /// AdjustForOldContribution
        /// </summary>
        /// <param name="oldContribution">object</param>
        public void AdjustForOldContribution(object oldContribution)
        {
            total = total ?? 0.0d;
            if (oldContribution != null)
#if SILVERLIGHT
                total -= (double)Convert.ChangeType(oldContribution, typeof(double), null);
#else
                total -= (double)Convert.ChangeType(oldContribution, typeof(double));
#endif
        }

        #endregion
    }

    #endregion

    #endregion

    #region IntTotalSummary
    /// <summary>
    /// Includes different methods for performing different calculations
    /// </summary>
    public class IntTotalSummary : SummaryBase, IAdjustable
    {
        /// <summary>
        /// Converts to the string type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "IntTotal";
        }

        internal int? total = null;
        /// <summary>
        /// Combines different Values together
        /// </summary>
        /// <param name="other">object</param>
        public override void Combine(object other)
        {
            object o = new object { };
            total = total ?? 0;
            if (other is int)
            {
                total += (int)other;
            }
            else if (other is double || other is decimal)
            {
#if SILVERLIGHT
                total += (int)Convert.ChangeType(other, typeof(int), null);
#else
                total += (int)Convert.ChangeType(other, typeof(int));
#endif
            }
#if SILVERLIGHT
            else if (other == null)
#else
            else if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank && total == 0)
                    total = null;
            }
            else if (other is string)
            {
                decimal value;
                if (decimal.TryParse(other.ToString(), out value))
                {
#if SILVERLIGHT
                total += (int)Convert.ChangeType(value, typeof(int), null);
#else
                    total += (int)Convert.ChangeType(value, typeof(int));
#endif
                }
            }   
            
        }
        /// <summary>
        /// Comblies different Summary
        /// </summary>
        /// <param name="other">SummaryBase</param>
        public override void CombineSummary(SummaryBase other)
        {
            if (other.GetResult() == null)
                total += 0;
            else
                Combine(((IntTotalSummary)other).total);
        }
        /// <summary>
        /// Resets the variable
        /// </summary>
        public override void Reset()
        {
            total = null;
        }
        /// <summary>
        /// Obtains the result
        /// </summary>
        /// <returns></returns>
        public override object GetResult()
        {
            return total;
        }
        /// <summary>
        /// Obtains the instance
        /// </summary>
        /// <returns></returns>
        public override SummaryBase GetInstance()
        {
            return new IntTotalSummary();
        }

      #region IAdjustable Members
        /// <summary>
        /// AdjustForNewContribution
        /// </summary>
        /// <param name="newContribution">object</param>
        public void AdjustForNewContribution(object newContribution)
        {	 
            total = total ?? 0;
            if(newContribution != null)
#if SILVERLIGHT
                total += (int)Convert.ChangeType(newContribution, typeof(int), null);
                
#else
            total += (int)Convert.ChangeType(newContribution, typeof(int));
            
#endif
        }
        /// <summary>
        /// AdjustForOldContribution
        /// </summary>
        /// <param name="oldContribution">object</param>
        public void AdjustForOldContribution(object oldContribution)
        {
            total = total ?? 0;
            if(oldContribution != null)
#if SILVERLIGHT
                total -= (int)Convert.ChangeType(oldContribution, typeof(int), null);
#else
            total -= (int)Convert.ChangeType(oldContribution, typeof(int));
#endif
        }


        #endregion
    }

    #endregion

    #region decimal summaries

    #region DecimalTotalSummary
    /// <summary>
    /// Decimal Total Summary
    /// </summary>
    public class DecimalTotalSummary : SummaryBase, IAdjustable
    {
        /// <summary>
        /// Converts the data to String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "DecimalTotal";
        }

        internal decimal? total = null;
        /// <summary>
        /// Combines the different values together
        /// </summary>
        /// <param name="other"></param>
        public override void Combine(object other)
        {
            total = total ?? 0m;
            if (other is decimal)
            {
                total += (decimal)other;
            }
            else if (other is double || other is int)
            {
#if SILVERLIGHT
                total += (decimal)Convert.ChangeType(other, typeof(decimal), null);
#else
               total += (decimal)Convert.ChangeType(other, typeof(decimal));
#endif
            }

#if SILVERLIGHT
            else if (other == null)
#else
            else if (other is System.DBNull || other == null)
#endif
            {
                if (ShowNullAsBlank && total == 0m)
                    total = null;
            }
            else if (other is string)
            {
                decimal value;
                if (decimal.TryParse(other.ToString(), out value))
                    total += value;
            }   

        }
        /// <summary>
        /// combines the summaries together
        /// </summary>
        /// <param name="other">SummaryBase</param>
        public override void CombineSummary(SummaryBase other)
        {
            total = total ?? 0m;
            if (other.GetResult() == null)
                total += 0m;
            else
                Combine(((DecimalTotalSummary)other).total);
        }
        /// <summary>
        /// Resets teh variable
        /// </summary>
        public override void Reset()
        {
            total = null;
        }
        /// <summary>
        /// Calculates the result
        /// </summary>
        /// <returns>total</returns>
        public override object GetResult()
        {
            return total;
        }
        /// <summary>
        /// Obtains the instance of the object
        /// </summary>
        /// <returns>DecimalTotalSummary</returns>
        public override SummaryBase GetInstance()
        {
            return new DecimalTotalSummary();
        }

        #region IAdjustable Members
        /// <summary>
        /// AdjustForNewContribution
        /// </summary>
        /// <param name="newContribution">object</param>
        public void AdjustForNewContribution(object newContribution)
        {	
            total = total ?? 0m;
            if (newContribution is decimal)
            {
                total += (decimal)newContribution;
            }
            else if (newContribution is double || newContribution is int)
            {
#if SILVERLIGHT
                total += (decimal)Convert.ChangeType(newContribution, typeof(decimal), null);
#else
                total += (decimal)Convert.ChangeType(newContribution, typeof(decimal));
#endif
            }
        }
        /// <summary>
        /// AdjustForOldContribution
        /// </summary>
        /// <param name="oldContribution">object</param>
        public void AdjustForOldContribution(object oldContribution)
        {
            total = total ?? 0m;
            if (oldContribution is decimal)
            {
                total -= (decimal)oldContribution;
            }
            else if (oldContribution is double || oldContribution is int)
            {
#if SILVERLIGHT
                total -= (decimal)Convert.ChangeType(oldContribution, typeof(decimal), null);
#else
                total -= (decimal)Convert.ChangeType(oldContribution, typeof(decimal));
#endif
            }
        }

        #endregion
    }
    #endregion

    #endregion

    #region DisplayIfDiscreteValuesEqual
    /// <summary>
    /// Displays if te discrete values are equal
    /// </summary>
    public class DisplayIfDiscreteValuesEqual : SummaryBase
    {
        object commonValue = null;
        bool isCommon = false;
        string padString = "*";
        /// <summary>
        /// Returns teh padded string
        /// </summary>
        public string PadString
        {
            get { return padString; }
            set { padString = value; }
        }
        /// <summary>
        /// Displays if the discrete values are equal.
        /// </summary>
        public DisplayIfDiscreteValuesEqual()
        {
            commonValue = null;
            isCommon = false;
        }
        /// <summary>
        /// Combies the different values together 
        /// </summary>
        /// <param name="other">object</param>
        public override void Combine(object other)
        {
            if (other != null)
            {
                if (commonValue == null)
                {
                    commonValue = other;
                    isCommon = commonValue.Equals(other);
                    return;
                }
                isCommon &= commonValue.Equals(other);
            }
            else if (other == null)
                isCommon = true;
        }
        /// <summary>
        ///  Resetst the value for the varibles
        /// </summary>
        public override void Reset()
        {
            commonValue = null;
            isCommon = false;
        }
        /// <summary>
        /// Gets the Resultant value.
        /// </summary>
        /// <returns></returns>
        public override object GetResult()
        {
            return isCommon ? commonValue : PadString;
        }
        /// <summary>
        /// Gets the instance of the DisplayIfDiscreteValuesEqual class
        /// </summary>
        /// <returns></returns>
        public override SummaryBase GetInstance()
        {
            DisplayIfDiscreteValuesEqual dc = new DisplayIfDiscreteValuesEqual();
            dc.PadString = this.PadString;
            return dc;
        }
        /// <summary>
        /// Combines the different summaries into one.
        /// </summary>
        /// <param name="other"></param>
        public override void CombineSummary(SummaryBase other)
        {
            if (other is DisplayIfDiscreteValuesEqual)
            {
                DisplayIfDiscreteValuesEqual dc = other as DisplayIfDiscreteValuesEqual;
                if (this.commonValue == null && dc.commonValue != null)
                {
                    this.commonValue = dc.commonValue;
                    this.isCommon = dc.isCommon;
                }
                else
                {
                    this.isCommon = (dc.commonValue == null && commonValue == null) || (this.commonValue.Equals(dc.commonValue) && dc.isCommon && isCommon);
                }
            }
        }
    }
    #endregion

    #region TextSummary - for debug purposes

    internal class TextSummary : SummaryBase
    {
        internal string text = "";

        public override void Combine(object other)
        {
            text = text + "_" + other.ToString();
        }
        public override void CombineSummary(SummaryBase other)
        {
            text = text + "_" + ((TextSummary)other).text;
        }

        public override void Reset()
        {
            text = "";
        }

        public override object GetResult()
        {
            return text;
        }
        public override SummaryBase GetInstance()
        {
            return new TextSummary();
        }
    }
    #endregion
}
