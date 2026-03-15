#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Olap.Reports;

namespace Syncfusion.Windows.Shared.Olap
{
    public class DesignerSettings
    {
        public DesignerSettings()
        {
            this.SeriesElements = new Items();
            this.CategoricalElements = new Items();
            this.SlicerElements = new Items();
        }
        public string ConnectionString { get; set; }

        public string CurrentCubeName { get; set; }

        public Items SeriesElements { get; set; }

        public Items CategoricalElements { get; set; }

        public Items SlicerElements { get; set; }

        public bool HasValidSettings
        {
            get
            {
                if (this.ConnectionString != null &&
                    this.ConnectionString != string.Empty &&
                    this.CurrentCubeName != null &&
                    this.CurrentCubeName != string.Empty)
                    return true;
                return false;
            }
        }

        public OlapReport GetOlapReport()
        {
            if (this.HasValidSettings)
            {
                OlapReport olapReport = new OlapReport();
                olapReport.CurrentCubeName = this.CurrentCubeName;
                olapReport.SeriesElements = this.SeriesElements;
                olapReport.CategoricalElements = this.CategoricalElements;
                olapReport.SlicerElements = this.SlicerElements;
                return olapReport;
            }
            return null;
        }
    }
}
