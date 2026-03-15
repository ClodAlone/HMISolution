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

namespace Syncfusion.RDL.Internal
{
    internal class ParameterField
    {
        public string Name { get; set; }

        public string ParamName { get; set; }

        public string Data { get; set; }

        public string ParamType { get; set; }
    }

    internal class DataField
    {
        ReportingComputationType reportingComputationType = ReportingComputationType.First;

        string functionName = null;

        public string GroupName { get; set; }

        public bool IsDataSetField { get; set; }

        public bool IsRecursiveField { get; set; }

        public string Name { get; set; }

        public string FieldName { get; set; }

        public string DataSetName { get; set; }

        public string DataTypeName { get; set; }

        public string Expression { get; set; }

        // Only use for ReportingEngine
        public ReportingEngineFieldValue Value { get; set; }

        internal ComputationType ComputaitonType { get; set; }

        public string FunctionName
        {
            get
            {
                return functionName;
            }
            set
            {
                this.functionName = value;
                this.reportingComputationType = (ReportingComputationType)Enum.Parse(typeof(ReportingComputationType), this.functionName, true);
            }
        }

        internal ReportingComputationType ReportingComputationType
        {
            get
            {
                return this.reportingComputationType;
            }
        }

        internal SummaryBase GetSummaryInstance()
        {
            SummaryBase sb = null;

            switch (this.ReportingComputationType)
            {
                case ReportingComputationType.Sum:
                    sb = new DoubleTotalSummary();
                    break;
                case ReportingComputationType.Avg:
                    sb = new DoubleAverageSummary();
                    break;
                case ReportingComputationType.Count:
                    sb = new CountSummary();
                    break;
                case ReportingComputationType.CountDistinct:
                    sb = new CountDistinctSummary();
                    break;
                case ReportingComputationType.Min:
                    sb = new DoubleMinSummary();
                    break;
                case ReportingComputationType.Max:
                    sb = new DoubleMaxSummary();
                    break;
                case ReportingComputationType.StDev:
                    sb = new DoubleStDevSummary();
                    break;
                case ReportingComputationType.StDevP:
                    sb = new DoubleStDevPSummary();
                    break;
                case ReportingComputationType.Var:
                    sb = new DoubleVarianceSummary();
                    break;
                case ReportingComputationType.VarP:
                    sb = new DoubleVariancePSummary();
                    break;
                case ReportingComputationType.First:
                    sb = new FirstSummary();
                    break;
                case ReportingComputationType.Last:
                    sb = new LastSummary();
                    break;
                case ReportingComputationType.Data:
                    sb = new DataSummary();
                    break;
                default:
                    break;
            }
            return sb;
        }

        #region ICloneable Members

        public object Clone()
        {
            DataField field = new DataField();
            field.Name = this.Name;
            field.FieldName = this.FieldName;
            field.FunctionName = this.FunctionName;
            field.DataSetName = this.DataSetName;
            field.Expression = this.Expression;
            return field;
        }

        #endregion
    }

}
