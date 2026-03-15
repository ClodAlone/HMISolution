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
using System.Xml;
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class ReportParameters : List<ReportParameter>
    {
    }

    public class ReportParameter
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public DataTypes DataType { get; set; }
        public bool Nullable { get; set; }
        public DefaultValue DefaultValue { get; set; }
        public bool AllowBlank { get; set; }
        public string Prompt { get; set; }
        public bool Hidden { get; set; }
        public ValidValues ValidValues { get; set; }
        public bool MultiValue { get; set; }
        public BooleanOptions UsedInQuery { get; set; }

        #region Serialization methods

        public bool ShouldSerializeNullable()
        {
            return this.Nullable != false;
        }

        public void ResetQueryNullable()
        {
            this.Nullable = false;
        }

        public bool ShouldSerializeAllowBlank()
        {
            return this.AllowBlank != false;
        }

        public void ResetAllowBlank()
        {
            this.AllowBlank = false;
        }

        public bool ShouldSerializeMultiValue()
        {
            return this.MultiValue != false;
        }

        public void ResetMultiValue()
        {
            this.MultiValue = false;
        }

        public bool ShouldSerializeHidden()
        {
            return this.Hidden != false;
        }

        public void ResetHidden()
        {
            this.Hidden = false;
        }

        public bool ShouldSerializeUsedInQuery()
        {
            return this.UsedInQuery != BooleanOptions.Auto;
        }

        public void ResetUsedInQuery()
        {
            this.UsedInQuery = BooleanOptions.Auto;
        }

        #endregion

        public ReportParameter Clone()
        {
            ReportParameter reportParameter = new ReportParameter();
            reportParameter.Update(this);
            return reportParameter;
        }

        internal void Update(ReportParameter reportParameter)
        {
            this.Name = reportParameter.Name ;
            this.Nullable = reportParameter.Nullable;
            this.DataType = reportParameter.DataType;
            this.AllowBlank = reportParameter.AllowBlank;
            this.Prompt = reportParameter.Prompt;
            this.Hidden = reportParameter.Hidden;
            this.MultiValue = reportParameter.MultiValue;
            this.UsedInQuery = reportParameter.UsedInQuery;
            if (reportParameter.ValidValues != null)
                this.ValidValues = new ValidValues(reportParameter.ValidValues);
            if (reportParameter.DefaultValue != null)
                this.DefaultValue = new DefaultValue(reportParameter.DefaultValue);
        }
    }
}
