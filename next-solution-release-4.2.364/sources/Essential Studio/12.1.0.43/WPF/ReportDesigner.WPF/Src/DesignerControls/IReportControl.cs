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
using Syncfusion.RDL.DOM;

namespace Syncfusion.Windows.Reports.Designer.Controls
{
    internal interface IReportControl
    {
        List<IReportItemControl> SelectedReportItems { get; set; }

        void DrawReportItemControl(DrawingReportItem itemType);

        void AddEmbeddedImage();

        void RemoveEmbeddedImage(string Name);

        void AddDataSource();

        void RemoveDataSource(string name);

        void ModifyDataSource(string name);

        void AddDataSet();

        void RemoveDataSet(string name);

        void ModifyDataSet(string name);

        void AddReportParameter();

        void RemoveReportParameter(string name);

        void ModifyReportParameter(string name);

        void RaiseReportItemDrawnEvent();
        
        void RaiseDataSourceCollectionModifiedEvent();

        void RaiseDataSetCollectionModifiedEvent();

        void RaiseParameterCollectionModifiedEvent();

        void RaiseEmbeddedImageCollectionModifiedEvent();

        event ReportItemDrawnEventHanlder ReportReportItemDrawn;

        event ReportDataSourceCollectionModifedHandler ReportDataSourceCollectionModified;

        event ReportDataSetCollectionModifedHandler ReportDataSetCollectionModified;

        event ReportParameterCollectionModifedHandler ReportParameterCollectionModified;

        event ReportEmbeddedImageCollectionModifedHandler ReportEmbeddedImageCollectionModified;
    }

    internal delegate void ReportDataSourceCollectionModifedHandler(object sender, ReportDataSourceCollectionChangeEventArgs e);

    internal delegate void ReportDataSetCollectionModifedHandler(object sender, ReportDataSetCollectionChangeEventArgs e);

    internal delegate void ReportParameterCollectionModifedHandler(object sender, ReportParameterCollectionChangeEventArgs e);

    internal delegate void ReportEmbeddedImageCollectionModifedHandler(object sender, ReportEmbeddedImageCollectionChangeEventArgs e);

    internal class ReportDataSourceCollectionChangeEventArgs : EventArgs
    {
        internal ModifiedType ModifiedType { get; set; }

        internal DataSources DataSources { get; set; }
    }

    internal class ReportDataSetCollectionChangeEventArgs : EventArgs
    {
        internal ModifiedType ModifiedType { get; set; }

        internal DataSets DataSets { get; set; }
    }

    internal class ReportParameterCollectionChangeEventArgs : EventArgs
    {
        internal ModifiedType ModifiedType { get; set; }

        internal ReportParameters Parameters { get; set; }
    }

    internal class ReportEmbeddedImageCollectionChangeEventArgs : EventArgs
    {
        internal ModifiedType ModifiedType { get; set; }

        internal EmbeddedImages Parameters { get; set; }
    }

    internal enum ModifiedType
    {
        Added,
        Modified,
        Removed
    }

    internal enum RDLFormat
    {
        SQL2008,
        SQL2008R2
    }
}
