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
    public class SummaryRowsBuilder<T> where T : class
    {
        private GridProperties<T> model;
        private List<SummaryRows<T>> summaryRowList = new List<SummaryRows<T>>();
        private SummaryRows<T> summaryRow=new SummaryRows<T>();
        public SummaryRowsBuilder(GridProperties<T> grid)
        {
            model = grid;
        }
        public SummaryRowsBuilder<T> Title(String title)
        {
            summaryRow.Title = title;
            return this;
        }
        public SummaryRowsBuilder<T> ShowCaptionSummary()
        {
            summaryRow.ShowCaptionSummary = true;
            return this;
        }
        public SummaryRowsBuilder<T> ShowCaptionSummary(bool showCaptionSummary)
        {
            summaryRow.ShowCaptionSummary = showCaptionSummary;
            return this;
        }
        public SummaryRowsBuilder<T> ShowTotalSummary()
        {
            summaryRow.ShowTotalSummary = true;
            return this;
        }
        public SummaryRowsBuilder<T> ShowTotalSummary(bool showTotalSummary)
        {
            summaryRow.ShowTotalSummary = showTotalSummary;
            return this;
        }
        public SummaryRowsBuilder<T> SummaryColumns(Action<SummaryColumnBuilder<T>> summaryColumn)
        {
            var builder = new SummaryColumnBuilder<T>(summaryRow);
            if (summaryColumn != null)
                summaryColumn.Invoke(builder);
            return this; 
        }
        public SummaryRowsBuilder<T> SummaryColumns(List<SummaryColumn<T>> summaryColumn)
        {
            summaryRow.SummaryColumns = summaryColumn;
            return this;
        }
        public void Add()
        {
            this.model.SummaryRow.Add(summaryRow);
            summaryRow = new SummaryRows<T>();
            //return this;
        }
    }
}
