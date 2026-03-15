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
using System.ServiceModel;
using Syncfusion.OlapSilverlight.Base.Engine;
using Syncfusion.OlapSilverlight.Base.Report;

namespace Syncfusion.OlapSilverlight.Base
{
    [ServiceContract(Namespace = "")]
    [ServiceKnownType(typeof(Element))]
    [ServiceKnownType(typeof(ElementCollection))]
    [ServiceKnownType(typeof(DimensionElement))]
    [ServiceKnownType(typeof(HierarchyElement))]
    [ServiceKnownType(typeof(HierarchyElementCollection))]
    [ServiceKnownType(typeof(LevelElement))]
    [ServiceKnownType(typeof(LevelElementCollection))]
    [ServiceKnownType(typeof(MemberElement))]
    [ServiceKnownType(typeof(MemberElementCollection))]
    [ServiceKnownType(typeof(MeasureElement))]
    [ServiceKnownType(typeof(MemberElementCollection))]
    [ServiceKnownType(typeof(MeasureElements))]
    [ServiceKnownType(typeof(KpiElement))]
    [ServiceKnownType(typeof(KpiElementCollection))]
    [ServiceKnownType(typeof(KpiElements))]
    [ServiceKnownType(typeof(SortElement))]
    [ServiceKnownType(typeof(FilterElement))]
    public interface IOlapDataManager
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateReportFormReport(Syncfusion.OlapSilverlight.Base.Report.OlapReport report, AsyncCallback callback, Object state);

        Syncfusion.OlapSilverlight.Base.Report.OlapReport EndUpdateReportFormReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateReportFormReportAndMember(Syncfusion.OlapSilverlight.Base.Data.DrillReport drillReport, AsyncCallback callback, Object state);

        Syncfusion.OlapSilverlight.Base.Report.OlapReport EndUpdateReportFormReportAndMember(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecuteOlapTable(Syncfusion.OlapSilverlight.Base.Report.OlapReport report, AsyncCallback callback, Object state);

        Syncfusion.OlapSilverlight.Base.Data.Result EndExecuteOlapTable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecuteOlapTableWithoutSummary(Syncfusion.OlapSilverlight.Base.Report.OlapReport report, AsyncCallback callback, Object state);

        Syncfusion.OlapSilverlight.Base.Data.Result EndExecuteOlapTableWithoutSummary(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginToggleExpandState(Syncfusion.OlapSilverlight.Base.Data.DrillReport drillReport, AsyncCallback callback, Object state);

        Syncfusion.OlapSilverlight.Base.Data.Result EndToggleExpandState(IAsyncResult result);
    }
}