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
using System.Threading.Tasks;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class SummaryColumnBuilder<T> where T:class
    {
        private SummaryColumn<T> summaryColumn=new SummaryColumn<T>();
        private SummaryRows<T> summaryRow = new SummaryRows<T>();
        GridProperties<T> model = new GridProperties<T>();
        public SummaryColumnBuilder(SummaryRows<T> summaryRow)
        {
            this.summaryRow = summaryRow;
        
        }
        public SummaryColumnBuilder<T> SummaryType(SummaryType summaryType)
        {
            summaryColumn.SummaryType = summaryType;
            return this;
        }
        public SummaryColumnBuilder<T> DisplayColumn(String displayColumn)
        {
            summaryColumn.DisplayColumn = displayColumn;
            return this;
        }
        public SummaryColumnBuilder<T> Prefix(String prefix)
        {
            summaryColumn.Prefix = prefix;
            return this;
        }
        public SummaryColumnBuilder<T> Suffix(String suffix)
        {
            summaryColumn.Suffix = suffix;
            return this;
        }
        public SummaryColumnBuilder<T> DataMember(String dataMember)
        {
            summaryColumn.DataMember = dataMember;
            return this;
        }
        public SummaryColumnBuilder<T> Format(String format)
        {
            summaryColumn.Format = format;
            return this;
        }
        public SummaryColumnBuilder<T> CustomSummaryValue(String customSummaryValue)
        {
            summaryColumn.CustomSummaryValue = customSummaryValue;
            return this;
        }
        public void Add()
        {
            this.summaryRow.SummaryColumns.Add(summaryColumn);
            summaryColumn = new SummaryColumn<T>();
            //return this;
        }
    }
}
