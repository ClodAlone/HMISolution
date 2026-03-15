//-------------------------------------------------------------------------------------------------
// <copyright file="CalculatedMember.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the calculated member element.
    /// </summary>
    [Serializable]
    public class CalculatedMember : Element, ICloneable<CalculatedMember>
#else
using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Common;
namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class CalculatedMember : Element
#endif
    {
        #region Private Variables
        /// <summary>
        /// Gets or sets the calculated member unique name.
        /// </summary>
        /// <value>The unique name of the calculated member.</value>
        [DefaultValue("")]
        string _uniqueName;

        /// <summary>
        /// Gets or sets the expression for Calculated member.
        /// </summary>
        string _expression;

        TypeOfMember _typeOfMember;

        DimensionElement _dimensionElement;

        MeasureElement _measureElement;

        private string _fullExpression;
        private string _formatString;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedMember"/> class.
        /// </summary>
        public CalculatedMember()
        {
            this.Name = string.Empty;
#if !SILVERLIGHT
            this.Properties = new PropertyCollection();
#endif
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the unique name of the Calculated Member.
        /// </summary>
        /// <value>The Unique name of the Calculated Member.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        //[XmlIgnoreAttribute()]
        public string UniqueName
        {
            get
            {
                if (_uniqueName != string.Empty)
                {
                    return _uniqueName;// this.Replace(_uniqueName);
                }

                return string.Empty;
            }
            set
            {
                _uniqueName = value;
            }
        }

        /// <summary>
        /// Gets or sets the expression which contains the formula for Calculated member.
        /// </summary>
        /// <value>The expression.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public string Expression
        {
            get
            {
                return _expression;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _expression = value;
                    if (this._measureElement != null || this._dimensionElement != null)
                    {
                        this.GenerateExpression((this._measureElement != null ? this._measureElement as Element : this._dimensionElement as Element));
                    }
                }
                else
                {
                    throw new Exception("Please specify the expression for Calculated Member");
                }
            }
        }

        /// <summary>
        /// Gets the custom expression.
        /// </summary>
        /// <value>The custom expression.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public string CustomExpression
        {
            get { return _fullExpression; }
            set { _fullExpression = value; }
        }


#if SILVERLIGHT
        [DataMember] 
#endif
        /// <summary>
        /// Gets or sets the type of the member.
        /// </summary>
        /// <value>The member type.</value>
        public TypeOfMember Type
        {
            get
            {
                return _typeOfMember;
            }
            set
            {
                _typeOfMember = value;
            }
        }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
#if SILVERLIGHT
        [DataMember] 
#endif
        public string FormatString
        {
            get { return _formatString; }
            set
            {
                _formatString = value;
                if (!string.IsNullOrEmpty(this._fullExpression) && !this._fullExpression.Contains("FORMAT_STRING") && !string.IsNullOrEmpty(_formatString))
                {
                    this._fullExpression += ", FORMAT_STRING =" + " \"" + _formatString + "\"";
                }
            }
        }


        #endregion

        #region Public Methods
#if !SILVERLIGHT

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>CalculatedMember</returns>
        public new CalculatedMember Clone()
        {
            CalculatedMember calculatedMember = new CalculatedMember();
            calculatedMember.Name = this.Name;
            calculatedMember.Expression = this.Expression;
            calculatedMember.Type = this.Type;
            calculatedMember.Properties = this.Properties.Clone();
            return calculatedMember;
        }
     
#endif

        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void AddElement(Element element)
        {
            if (element != null)
            {
#if !SILVERLIGHT
                if (element is DimensionElement)
                {

                    this._dimensionElement = (element as DimensionElement).Clone();
                    this._measureElement = null;
                }
                else if (element is MeasureElement)
                {
                    this._measureElement = (element as MeasureElement).Clone();
                    this._dimensionElement = null;
                }
                if (!string.IsNullOrEmpty(this._expression))
                {
                    this.GenerateExpression(element);
                }
#else
                if (element is DimensionElement)
                {

                    this._dimensionElement = element as DimensionElement;
                    this._measureElement = null;
                }
                else if (element is MeasureElement)
                {
                    this._measureElement = element as MeasureElement;
                    this._dimensionElement = null;
                }
                if (!string.IsNullOrEmpty(this._expression))
                {
                    this.GenerateExpression(element);
                }
#endif
            }
        }

        private void GenerateExpression(Element element)
        {
            if (element != null)
            {
                string name = string.Empty;
                bool isMember = false;
                if (element is MeasureElement)
                {
                    MeasureElement measure = element as MeasureElement;
                    this.Type = TypeOfMember.Measure;
                    name = measure.UniqueName;
                }
                else if (element is DimensionElement)
                {
                    DimensionElement dimension = element as DimensionElement;
                    this.Type = TypeOfMember.Dimension;
                    if (dimension != null && dimension.Hierarchy != null)
                    {
                        HierarchyElement hierarchy = dimension.Hierarchy;

                        if (hierarchy.LevelElements != null && hierarchy.LevelElements.Count > 0)
                        {
                            if (hierarchy.LevelElements[0].MemberElements != null && hierarchy.LevelElements[0].MemberElements.Count > 0)
                            {
                                MemberElement member = hierarchy.LevelElements[0].MemberElements[0];
                                isMember = true;
                                if (member.ChildMemberElements != null && member.ChildMemberElements.Count > 0)
                                {
                                    name = member.ChildMemberElements[0].UniqueName;
                                }
                                else
                                {
                                    name = member.UniqueName;
                                }
                            }
                            else
                            {
                                name = hierarchy.LevelElements[0].UniqueName;
                            }
                        }
                        else
                        {
                            name = hierarchy.UniqueName;
                        }
                    }
                }

                if (name != string.Empty && !isMember)
                {
                    name = this.Replace(name);
                    this._fullExpression = " MEMBER " + name + " As " + "\' " + this._expression + " \'";
                    if (!string.IsNullOrEmpty(this._formatString)) this._fullExpression += ", FORMAT_STRING =" + " \"" + this._formatString + "\"";
                    this._uniqueName = name;
                }
                else if (name != string.Empty && isMember)
                {
                    name = name + ".[" + this.Name + "]";
                    this._fullExpression = " MEMBER " + name + " As " + "\' " + this._expression + " \'";
                    if (!string.IsNullOrEmpty(this._formatString)) this._fullExpression += ", FORMAT_STRING =" + " \"" + this._formatString + "\"";
                    this._uniqueName = name;
                }
            }
        }

        private string Replace(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                int indx = name.LastIndexOf('[');
                name = name.Remove(indx);
                name = name + "[" + this.Name + "]";

                return name;
            }
            return string.Empty;
        }

        #endregion
    }

#if SILVERLIGHT
    [DataContract]
#endif
    /// <summary>
    /// Type of the Calculated Member
    /// </summary>
    public enum TypeOfMember
    {
        /// <summary>
        /// Calculated Measure
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "Measure")]
#endif  
        Measure,
        /// <summary>
        /// Calculated Dimension
        /// </summary>
#if SILVERLIGHT
        [EnumMember(Value = "Dimension")]
#endif 
        Dimension
    }
}
