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
using System.Runtime.Serialization;
using System.ComponentModel;
#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using System.ComponentModel;
#endif

#if !SILVERLIGHT
namespace Syncfusion.Olap.Reports
{
    [Serializable]
    public class VirtualKpiElement : Element, ICloneable<VirtualKpiElement>
    {
#else
namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class VirtualKpiElement : Element
    {
#endif
        #region private varialbes
        private string _expression;
        private string _kpiValueExpression;
        private string _kpiGoalExpression;
        private string _kpiStatusExpression;
        private string _kpiTrendExpression;

        internal static string vKpiValue;
        internal static string vKpiGoal;
        internal static string vKpiStatus;
        internal static string vKpiTrend;

        internal string virtualKpiValue;
        internal string virtualKpiGoal;
        internal string virtualKpiStatus;
        internal string virtualKpiTrend;
       // private string _name; 
        #endregion

        private string _trendGraphic;

#if SILVERLIGHT
        [DataMember]
#endif
        public string TrendGraphic
        {
            get { return _trendGraphic; }
            set { _trendGraphic = value; }
        }

        private string _statusGraphic;
#if SILVERLIGHT
        [DataMember]
#endif
        public string StatusGraphic
        {
            get { return _statusGraphic; }
            set { _statusGraphic = value; }
        }
        internal string ValueUniqueName
        {
            get { return "[Measures].[" + this.Name + " Value]"; }
        }

        internal string GoalUniqueName
        {
            get { return "[Measures].[" + Name + " Goal]"; }
        }

        internal string TrendUniqueName
        {
            get { return "[Measures].[" + Name + " Trend]"; }
        }
   
        internal string StatusUniqueName
        {
            get { return "[Measures].["+Name+" Status]"; }
        }

        public string UniqueName
        {
            get { return "[Measures].["+Name+"]"; }
        }

#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(true)]
        public bool ShowVirtualKPIGoal { get; set; }
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(true)]
        public bool ShowVirtualKPIStatus { get; set; }
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(true)]
        public bool ShowVirtualKPIValue { get; set; }
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(true)]
        public bool ShowVirtualKPITrend { get; set; }
        /// <summary>
        /// Gets or sets a value which contains expression for the KPI element
        /// </summary>
#if SILVERLIGHT
        [DataMember]
#endif
        public string Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating KPI value expression
        /// </summary>
#if SILVERLIGHT
        [DataMember]
#endif
        public string KpiValueExpression
        {
            get { return _kpiValueExpression; }
            set 
            {
                _kpiValueExpression = value;
                if (_kpiValueExpression != "")
                    vKpiValue = _kpiValueExpression;
                virtualKpiValue = vKpiValue;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating KPI goal expression
        /// </summary>
#if SILVERLIGHT
        [DataMember]
#endif
        public string KpiGoalExpression
        {
            get { return _kpiGoalExpression; }
            set 
            { 
                _kpiGoalExpression = value;
                if (_kpiGoalExpression != "")
                    vKpiGoal = _kpiGoalExpression;
                virtualKpiGoal = vKpiGoal;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating KPI status expression
        /// </summary>
#if SILVERLIGHT
        [DataMember]
#endif
        public string KpiStatusExpression
        {
            get { return _kpiStatusExpression; }
            set 
            { 
                _kpiStatusExpression = value;
                if (_kpiStatusExpression != "")
                    vKpiStatus = _kpiStatusExpression;
                virtualKpiStatus = vKpiStatus;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating KPI trend expression
        /// </summary>
#if SILVERLIGHT
        [DataMember]
#endif
        public string KpiTrendExpression
        {
            get { return _kpiTrendExpression; }
            set 
            { 
                _kpiTrendExpression = value;
                if (_kpiTrendExpression != "")
                    vKpiTrend = _kpiTrendExpression;
                virtualKpiTrend = vKpiTrend;
            }
        }

#if SILVERLIGHT
        public VirtualKpiElement()
        {
            this.Name = string.Empty;
            this.KpiGoalExpression = "";
            this.KpiStatusExpression = "";
            this.KpiValueExpression = "";
            this.KpiTrendExpression = "";
            this.ShowVirtualKPIValue = true;
            this.ShowVirtualKPITrend = true;
            this.ShowVirtualKPIStatus = true;
            this.ShowVirtualKPIGoal = true;
        }

#else
        public VirtualKpiElement()
        {
            this.Name = string.Empty;
            this.ShowVirtualKPIValue = true;
            this.ShowVirtualKPITrend = true;
            this.ShowVirtualKPIStatus = true;
            this.ShowVirtualKPIGoal = true;
        }
        public new VirtualKpiElement Clone()
        {
            VirtualKpiElement virtualKpiElement = new VirtualKpiElement();
            virtualKpiElement.Expression = this.Expression;
            virtualKpiElement.ElementName = this.ElementName;
            virtualKpiElement.KpiGoalExpression = this.KpiGoalExpression;
            virtualKpiElement.KpiStatusExpression = this.KpiStatusExpression;
            virtualKpiElement.KpiTrendExpression = this.KpiTrendExpression;
            virtualKpiElement.KpiValueExpression = this.KpiValueExpression;
            virtualKpiElement.Name = this.Name;
            virtualKpiElement.Properties = this.Properties;
            virtualKpiElement.Visible = this.Visible;
            return virtualKpiElement;
        }
#endif
    }
}
