//-------------------------------------------------------------------------------------------------
// <copyright file="PageModelFactory.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.RDL.Layout
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using Syncfusion.RDL.Data;
    using Syncfusion.RDL.Layout;
    using Syncfusion.RDL.Internal;
    using Syncfusion.RDL.DOM;
    using Syncfusion.RDL.ItemModel;

    /// <summary>
    /// Helps creating the pages for the report.
    /// </summary>
    internal class PageModelFactory
    {
        #region Private Members

        private int maxHeight = 1056;
        private int widthPageCount = 1;
        //int heightPageCount = 1;
        private double normalPageWidth = 0;

        private Dictionary<int, ReportPageInfo> pageDictionary;
        private Dictionary<int, ReportPageInfo> printLayoutPageDictionary;
        private Dictionary<int, ReportPageInfo> flowLayoutDictionary;

        #endregion

        #region internal properties


        internal double BodyBottomGap { get; set; }
        internal double BodyRightGap { get; set; }
        internal double HeaderBottomGap { get; set; }
        internal double FooterBottomGap { get; set; }
        internal double HeaderHeight { get; set; }
        internal double FooterHeight { get; set; }
        
        internal bool IsPrintMode
        {
            get;
            set;
        }


        internal ReportModel Model 
        { 
            get; 
            set; 
        }

        internal Dictionary<int, ReportPageInfo> FlowLayoutDictionary
        {
            get
            {
                if (this.flowLayoutDictionary == null)
                {
                   this. UpdateFlowPageLayout();
                }

                return this.flowLayoutDictionary;
            }
        }

        internal Dictionary<int, ReportPageInfo> PageDictionary
        {
            get
            {
                if (this.IsPrintMode)
                {
                    return this.PrintLayoutPageDictionary;
                }
                return this.pageDictionary;
            }
        }

        internal Dictionary<int, ReportPageInfo> PrintLayoutPageDictionary
        {
            get
            {
                if (this.printLayoutPageDictionary == null)
                {
                    this.UpdatePrintPageLayout();
                }

                return this.printLayoutPageDictionary;
            }
        }

        internal ReportModelContentCollection ReportBodyModelerLists { get; set; }

        internal ReportModelContentCollection ReportHeaderModelerLists { get; set; }


        internal ReportModelContentCollection ReportFooterModelerLists { get; set; }

        public List<ReportItemContiner> LeftOrder { get; set; }

        public List<ReportItemContiner> TopOrder { get; set; }

        public List<ReportItemContiner> HeaderLeftOrder { get; set; }

        public List<ReportItemContiner> HeaderTopOrder { get; set; }

        public List<ReportItemContiner> FooterLeftOrder { get; set; }

        public List<ReportItemContiner> FooterTopOrder { get; set; }

        #endregion

        #region Public Properties

        public ReportDefinition Report { get; set; }

        public double PageHeight { get; set; }

        public int PageCount
        {
            get { return this.PageDictionary.Count; }
        }

        public LayoutThicknessInfo Margin { get; set; }

        public double PageWidth { get; set; }

        #endregion

        #region Constructor

        public PageModelFactory(ReportModel model)
        {
            this.Model = model;
            this.Intialize(this.Model.BodyReportItemModels, this.Model.HeaderReportItemModels,
                           this.Model.FooterReportItemModels);
        }

        public PageModelFactory(ReportModelContentCollection pageModelCollection,
                                ReportModelContentCollection headerModelCollection,
                                ReportModelContentCollection footerModelCollection)
        {
            this.Intialize(pageModelCollection, headerModelCollection, footerModelCollection);
        }

        #endregion

        #region Helper Methods

        private void Intialize(ReportModelContentCollection pageModelCollection,
                               ReportModelContentCollection headerModelCollection,
                               ReportModelContentCollection footerModelCollection)
        {
            double maxBottom = 0;
            double maxRight = 0;

            this.FooterHeight = 0;
            this.HeaderHeight = 0;
            this.ReportBodyModelerLists = new ReportModelContentCollection();

            if (this.Model != null && !this.Model.isContainsToggle)
            {
                this.ReportBodyModelerLists.AddRange(pageModelCollection.Where(item => item.Hidden != true));
            }
            else
            {
                this.ReportBodyModelerLists.AddRange(pageModelCollection);
            }

            this.BodyBottomGap = this.Model.Body.Height.PixelValue;
            this.BodyRightGap = this.Model.Report.Width.PixelValue;

            if (Model.Page != null && Model.Page.PageHeight != null)
            {
                this.maxHeight = (int)Model.Page.PageHeight.PixelValue;
            }
            if (headerModelCollection != null)
            {
                this.ReportHeaderModelerLists = new ReportModelContentCollection();
                this.ReportHeaderModelerLists.AddRange(headerModelCollection.Where(item => item.Hidden != true));
                //// Page Header
                this.UpdateRenderingOrder(this.ReportHeaderModelerLists, null, UpdateSection.Header);
                this.HeaderHeight = this.Model.Page.PageHeader.Height != null
                                        ? this.Model.Page.PageHeader.Height.PixelValue
                                        : 0;
                this.HeaderBottomGap = this.HeaderHeight;

                maxBottom = 0;

                foreach (var model in this.ReportHeaderModelerLists)
                {
                    maxBottom = maxBottom < (model.Top + model.Height) ? (model.Top + model.Height) : maxBottom;
                }

                this.HeaderBottomGap = this.HeaderBottomGap - maxBottom;
            }

            if (footerModelCollection != null)
            {
                this.ReportFooterModelerLists = new ReportModelContentCollection();
                this.ReportFooterModelerLists.AddRange(footerModelCollection.Where(item => item.Hidden != true));
                //// Page Footer                
                this.UpdateRenderingOrder(this.ReportFooterModelerLists, null, UpdateSection.Footer);
                this.FooterHeight = this.Model.Page.PageFooter.Height != null
                                        ? this.Model.Page.PageFooter.Height.PixelValue
                                        : 0;
                this.FooterBottomGap = this.FooterHeight;

                maxBottom = 0;

                foreach (var model in this.ReportFooterModelerLists)
                {
                    maxBottom = maxBottom < (model.Top + model.Height) ? (model.Top + model.Height) : maxBottom;
                }

                this.FooterBottomGap = this.FooterBottomGap - maxBottom;
            }

            this.UpdateRenderingOrder(this.ReportBodyModelerLists, null, UpdateSection.Body);

            foreach (var model in ReportBodyModelerLists)
            {
                maxBottom = maxBottom < (model.Top + model.Height) ? (model.Top + model.Height) : maxBottom;
                maxRight = maxRight < (model.Left + model.Width) ? (model.Left + model.Width) : maxRight;
            }

            this.BodyBottomGap = this.BodyBottomGap - maxBottom;
            this.BodyRightGap = this.BodyRightGap - maxRight;

            pageDictionary = new Dictionary<int, ReportPageInfo>();
        }

        private int FindPageStart(double y, double pageHeight)
        {
            return (int) Math.Floor(y/pageHeight);
        }

        private bool IsContainerReportItem(ReportItemContiner reportItemContainer)
        {
            if (reportItemContainer.ReportItem.ModelType == ModelType.RectangleModel ||
                reportItemContainer.ReportItem.ModelType == ModelType.SubReportModel)
            {
                return true;
            }

            return false;
        }

        private bool IsTablixReportItem(ReportItemContiner reportItemContainer)
        {
            if (reportItemContainer.ReportItem.ModelType == ModelType.TablixModel)
            {
                return true;
            }

            return false;
        }

        private void UpdateRenderingOrder(ReportModelContentCollection pageModelCollection, ReportItemContiner parent,
                                          UpdateSection updateSection)
        {
            var modelCollection = ((from reportItemModel in pageModelCollection
                                    where
                                        (parent == null
                                             ? reportItemModel.ContainerModel == null
                                             : reportItemModel.ContainerModel == parent.ReportItem)
                                    select
                                        new ReportItemContiner()
                                            {
                                                Name = reportItemModel.Name,
                                                ReportItem = reportItemModel
                                            })).ToList();

            if (updateSection == UpdateSection.Container)
            {
                parent.TopOrder = this.GetTopOrder(modelCollection);
                parent.LeftOrder = this.GetLeftOrder(modelCollection);
                this.GetBottomGap(parent);
            }
            else if (updateSection == UpdateSection.Body)
            {
                this.TopOrder = this.GetTopOrder(modelCollection);
                this.LeftOrder = this.GetLeftOrder(modelCollection);
            }
            else if (updateSection == UpdateSection.Header)
            {
                this.HeaderTopOrder = this.GetTopOrder(modelCollection);
                this.HeaderLeftOrder = this.GetLeftOrder(modelCollection);
            }
            else if (updateSection == UpdateSection.Footer)
            {
                this.FooterTopOrder = this.GetTopOrder(modelCollection);
                this.FooterLeftOrder = this.GetLeftOrder(modelCollection);
            }

            foreach (var itemModel in modelCollection)
            {
                if (this.IsContainerReportItem(itemModel) && itemModel.ReportItem.ReportItemModelers != null)
                {
                    this.UpdateRenderingOrder(itemModel.ReportItem.ReportItemModelers, itemModel,
                                              UpdateSection.Container);
                }
            }
        }

        private void GetBottomGap(ReportItemContiner parent)
        {
            if (parent != null && parent.TopOrder != null && parent.TopOrder.Count > 0)
            {
                parent.BGap = new Dictionary<string, double>();
                double parentHeight = parent.ReportItem.Height;
                double bottom = (from topItem in parent.TopOrder select (parentHeight - (topItem.ReportItem.Top + topItem.ReportItem.Height))).Min();
                bottom = bottom > 0 ? bottom : 0;
                parent.BGap.Add(parent.Name, bottom);
            }
        }

        private List<ReportItemContiner> GetLeftOrder(IEnumerable<ReportItemContiner> reportItemContainerCollection)
        {
            List<ReportItemContiner> reportItemContainers = reportItemContainerCollection.ToList();

            reportItemContainers.Sort(delegate(ReportItemContiner first, ReportItemContiner second)
                {
                    return first.ReportItem.Left.CompareTo(second.ReportItem.Left);
                });

            foreach (var reportItemContainer in reportItemContainers)
            {
                var leftReportItemContainers = from itemContainer in reportItemContainers
                                               where
                                                   (itemContainer != reportItemContainer &&
                                                    (itemContainer.ReportItem.Left + itemContainer.ReportItem.Width) <=
                                                    reportItemContainer.ReportItem.Left)
                                               select itemContainer;

                if (leftReportItemContainers.Count() > 0)
                {
                    reportItemContainer.PreviousLeftControlNames = new List<string>();
                    reportItemContainer.LGap = new Dictionary<string, double>();

                    foreach (var container in leftReportItemContainers)
                    {
                        reportItemContainer.PreviousLeftControlNames.Add(container.Name);
                        reportItemContainer.LGap.Add(container.Name,
                                                     reportItemContainer.ReportItem.Left -
                                                     (container.ReportItem.Left + container.ReportItem.Width));
                    }
                }
            }

            return reportItemContainers;
        }

        private List<ReportItemContiner> GetTopOrder(IEnumerable<ReportItemContiner> reportItemContainerCollection)
        {
            List<ReportItemContiner> reportItemContainers = reportItemContainerCollection.ToList();

            reportItemContainers.Sort(delegate(ReportItemContiner first, ReportItemContiner second)
                {
                    return first.ReportItem.Top.CompareTo(second.ReportItem.Top);
                });

            foreach (var reportItemContainer in reportItemContainers)
            {
                var topReportItemContainers = from itemContainer in reportItemContainers
                                              where
                                                  (itemContainer != reportItemContainer &&
                                                   ((itemContainer.ReportItem.Top + itemContainer.ReportItem.Height) <=
                                                   reportItemContainer.ReportItem.Top || ((itemContainer.ReportItem.Top + itemContainer.ReportItem.Height / 2) <=
                                                   reportItemContainer.ReportItem.Top) && ((reportItemContainer.ReportItem.Left + reportItemContainer.ReportItem.Width > itemContainer.ReportItem.Left)
                                                   && (itemContainer.ReportItem.Left + itemContainer.ReportItem.Width > reportItemContainer.ReportItem.Left))))
                                              select itemContainer;

                if (topReportItemContainers.Count() > 0)
                {
                    reportItemContainer.PreviousTopControlNames = new List<string>();
                    reportItemContainer.TGap = new Dictionary<string, double>();

                    foreach (var container in topReportItemContainers)
                    {
                        reportItemContainer.PreviousTopControlNames.Add(container.Name);
                        double tgap = reportItemContainer.ReportItem.Top - (container.ReportItem.Top + container.ReportItem.Height);
                        reportItemContainer.TGap.Add(container.Name, tgap > 0 ? tgap : 0);
                    }
                }
            }

            return reportItemContainers;
        }

        internal void UpdateFlowPageLayout()
        {
            this.flowLayoutDictionary = new Dictionary<int, ReportPageInfo>();
            double pageHeight = double.NaN;
            double pageWidth = double.NaN;
            UpdateReportItem(new LayoutSize(pageWidth, pageHeight), ReportItemViewMode.None);
        }

        internal void UpdatePageLayout()
        {
            UpdateFlowPageLayout();

            double pageHeight = maxHeight;
            double pageWidth = double.NaN;
            UpdateReportItem(new LayoutSize(pageWidth, pageHeight), ReportItemViewMode.Normal);
        }

        internal void UpdatePrintPageLayout()
        {
            this.printLayoutPageDictionary = new Dictionary<int, ReportPageInfo>();
            double pageHeight = this.PageHeight - Margin.Top - Margin.Bottom - this.HeaderHeight - this.FooterHeight;
            double pageWidth = this.PageWidth - Margin.Left - Margin.Right;

            UpdateReportItem(new LayoutSize(pageWidth, pageHeight), ReportItemViewMode.Print);
        }

        private void UpdateReportItemTop(ReportItemContiner reportItemContainer, ReportItemContiner parentContainer,
                                         double maxHeight, ReportItemViewMode updateMode, UpdateSection updateSection)
        {
            List<ReportItemContiner> topOrder = null;

            LayoutReportItemModel locationInfo = reportItemContainer.ReportItem.FlowLayoutInfo;

            if (updateMode == ReportItemViewMode.Print)
            {
                locationInfo = reportItemContainer.ReportItem.PrintPageInfo;
            }
            else if (updateMode == ReportItemViewMode.Normal)
            {
                locationInfo = reportItemContainer.ReportItem.PageInfo;
            }

            if (updateSection == UpdateSection.Body)
            {
                topOrder = this.TopOrder;
            }
            else if (updateSection == UpdateSection.Header)
            {
                topOrder = this.HeaderTopOrder;
            }
            else if (updateSection == UpdateSection.Footer)
            {
                topOrder = this.FooterTopOrder;
            }
            else
            {
                topOrder = parentContainer.TopOrder;
            }

            if (reportItemContainer.PreviousTopControlNames != null)
            {
                var reportItems = (from rptItemContainer in topOrder
                                   where reportItemContainer.PreviousTopControlNames.Contains(rptItemContainer.Name)
                                   select rptItemContainer.ReportItem).ToList();

                reportItems.Sort(delegate(IReportItemModeler first, IReportItemModeler second)
                    {
                        var bottom1 = first.FlowLayoutInfo.ActualTop + first.FlowLayoutInfo.ActualHeight;
                        var bottom2 = second.FlowLayoutInfo.ActualTop + second.FlowLayoutInfo.ActualHeight;

                        if (updateMode == ReportItemViewMode.Normal)
                        {
                            bottom1 = first.PageInfo.ActualTop + first.PageInfo.ActualHeight;
                            bottom2 = second.PageInfo.ActualTop + second.PageInfo.ActualHeight;
                        }
                        else if (updateMode == ReportItemViewMode.Print)
                        {
                            bottom1 = first.PrintPageInfo.ActualTop + first.PrintPageInfo.ActualHeight;
                            bottom2 = second.PrintPageInfo.ActualTop + second.PrintPageInfo.ActualHeight;
                        }

                        return bottom1.CompareTo(bottom2);
                    });

                var reportItem = reportItems.Last();
                double prevGap = reportItemContainer.TGap[reportItem.Name];

                if (updateMode == ReportItemViewMode.Print)
                {
                    locationInfo.ActualTop = reportItem.PrintPageInfo.ActualTop + reportItem.PrintPageInfo.ActualHeight +
                                             prevGap;
                    reportItemContainer.ReportItem.PrintPageInfo = locationInfo;
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    locationInfo.ActualTop = reportItem.PageInfo.ActualTop + reportItem.PageInfo.ActualHeight + prevGap;
                    reportItemContainer.ReportItem.PageInfo = locationInfo;
                }
                else
                {
                    locationInfo.ActualTop = reportItem.FlowLayoutInfo.ActualTop +
                                             reportItem.FlowLayoutInfo.ActualHeight + prevGap;
                    reportItemContainer.ReportItem.FlowLayoutInfo = locationInfo;
                }
            }
            else
            {
                locationInfo.ActualTop = reportItemContainer.ReportItem.Top;

                if (updateSection == UpdateSection.Container)
                {
                    var reportItem = reportItemContainer.ReportItem;

                    if (reportItem.ContainerModel != null)
                    {
                        var parentReportItem = reportItem.ContainerModel;

                        if (updateMode == ReportItemViewMode.Print)
                        {
                            locationInfo.ActualTop += parentReportItem.PrintPageInfo.ActualTop;
                        }
                        else if (updateMode == ReportItemViewMode.Normal)
                        {
                            locationInfo.ActualTop += parentReportItem.PageInfo.ActualTop;
                        }
                        else
                        {
                            locationInfo.ActualTop += parentReportItem.FlowLayoutInfo.ActualTop;
                        }
                    }
                }

                if (updateMode == ReportItemViewMode.Print)
                {
                    reportItemContainer.ReportItem.PrintPageInfo = locationInfo;
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    reportItemContainer.ReportItem.PageInfo = locationInfo;
                }
                else
                {
                    reportItemContainer.ReportItem.FlowLayoutInfo = locationInfo;
                }
            }
            if (reportItemContainer.BGap != null)
            {
                locationInfo.ActualHeight += reportItemContainer.BGap[reportItemContainer.Name];
            }
            this.UpdateContainerModelHeight(reportItemContainer, maxHeight, updateMode);

            double topValue = locationInfo.ActualTop;
            double firstPreferedHeight = (updateMode == ReportItemViewMode.None &&
                                          reportItemContainer.ReportItem.KeepTogether)
                                             ? double.NaN
                                             : maxHeight - topValue;

            if (!double.IsNaN(firstPreferedHeight))
            {
                int pageNo = this.FindPageStart(topValue, maxHeight);

                if (pageNo != 0)
                {
                    double topPageValue = (topValue%(pageNo*maxHeight));
                    firstPreferedHeight = maxHeight - topPageValue;
                }
            }

            reportItemContainer.ReportItem.UpdateHeight(firstPreferedHeight, locationInfo, maxHeight, updateMode);

            if (this.UpdateTop(reportItemContainer, locationInfo, maxHeight, updateMode))
            {
                if (this.IsContainerReportItem(reportItemContainer))
                {
                    this.UpdateContainerModelHeight(reportItemContainer, maxHeight, updateMode);
                }

                topValue = locationInfo.ActualTop;

                if (!double.IsNaN(firstPreferedHeight))
                {
                    firstPreferedHeight = maxHeight - topValue;
                    int pageNo = this.FindPageStart(topValue, maxHeight);

                    if (pageNo != 0)
                    {
                        double topPageValue = (topValue%(pageNo*maxHeight));
                        firstPreferedHeight = maxHeight - topPageValue;
                    }
                }

                reportItemContainer.ReportItem.UpdateHeight(firstPreferedHeight, locationInfo, maxHeight, updateMode);
            }
        }

        private void UpdateContainerModelHeight(ReportItemContiner reportItemContainer, double maxHeight,
                                                ReportItemViewMode updateMode)
        {
            if (this.IsContainerReportItem(reportItemContainer) && reportItemContainer.TopOrder != null)
            {
                foreach (var model in reportItemContainer.TopOrder)
                {
                    this.UpdateReportItemTop(model, reportItemContainer, maxHeight, updateMode, UpdateSection.Container);
                }
            }
        }

        private bool UpdateTop(ReportItemContiner rptItemContainer, LayoutReportItemModel locationInfo, double maxHeight,
                               ReportItemViewMode updateMode)
        {
            double startTop = locationInfo.ActualTop%maxHeight;
            try
            {
                if (double.IsNaN(maxHeight) && (updateMode == ReportItemViewMode.None))
                {
                    maxHeight = this.maxHeight;
                }

                if (!double.IsNaN(maxHeight))
                {
                    var containerModel = (from itemContainer in this.TopOrder
                                          where
                                              rptItemContainer.PreviousTopControlNames != null &&
                                              rptItemContainer.PreviousTopControlNames.Contains(itemContainer.Name)
                                          select itemContainer).LastOrDefault();
                    var reportItem = containerModel != null ? containerModel.ReportItem : null;
                    var breakLocation = rptItemContainer.ReportItem.PageBreak;

                    if (containerModel == null)
                    {
                        containerModel = (from item in this.TopOrder
                                          where
                                              rptItemContainer.ReportItem.ContainerModel != null &&
                                              item.Name.Equals(rptItemContainer.ReportItem.ContainerModel.Name)
                                          select item).LastOrDefault();
                    }
                    if (rptItemContainer.ReportItem is RectangleModel)
                    {
                        if (breakLocation != BreakLocation.StartAndEnd)
                        {
                            var item = rptItemContainer.TopOrder.FirstOrDefault();
                            breakLocation = item != null &&
                                            (item.ReportItem.PageBreak == BreakLocation.Start ||
                                             item.ReportItem.PageBreak == BreakLocation.StartAndEnd)
                                                ? item.ReportItem.PageBreak
                                                : breakLocation;
                        }
                    }
                    if (breakLocation != BreakLocation.None && breakLocation != BreakLocation.End)
                    {
                        if (breakLocation == BreakLocation.Start || breakLocation == BreakLocation.StartAndEnd)
                        {
                            if (reportItem != null)
                            {
                                if ((reportItem.PageBreak == BreakLocation.End ||
                                     reportItem.PageBreak == BreakLocation.StartAndEnd || reportItem is RectangleModel))
                                {
                                    if (reportItem is RectangleModel)
                                    {
                                        reportItem = containerModel.TopOrder.LastOrDefault() != null
                                                         ? containerModel.TopOrder.LastOrDefault().ReportItem
                                                         : null;

                                        if (reportItem != null)
                                        {
                                            locationInfo.ActualTop += reportItem.PageBreak == BreakLocation.Start ||
                                                                      reportItem.PageBreak == BreakLocation.None
                                                                          ? maxHeight - startTop
                                                                          : ((2*maxHeight) - startTop);
                                        }
                                        else
                                            locationInfo.ActualTop -= startTop;
                                    }
                                    else
                                    {
                                        locationInfo.ActualTop += reportItem.PageBreak == BreakLocation.Start ||
                                                                  reportItem.PageBreak == BreakLocation.None
                                                                      ? maxHeight - startTop
                                                                      : ((2*maxHeight) - startTop);
                                    }
                                }
                                else
                                {
                                    locationInfo.ActualTop += (reportItem.PageBreak == BreakLocation.Start ||
                                                               reportItem.PageBreak == BreakLocation.None)
                                                                  ? (maxHeight - startTop)
                                                                  : (2*maxHeight) - startTop;
                                }
                            }
                            else
                            {
                                if (reportItem == null && rptItemContainer.ReportItem.ContainerModel != null &&
                                    rptItemContainer.ReportItem.ContainerModel is RectangleModel)
                                {
                                    reportItem = (from item in containerModel.TopOrder
                                                  where
                                                      rptItemContainer.PreviousTopControlNames != null &&
                                                      rptItemContainer.PreviousTopControlNames.Contains(item.Name)
                                                  select item.ReportItem).LastOrDefault();
                                    if (reportItem != null)
                                    {
                                        locationInfo.ActualTop += (reportItem.PageBreak == BreakLocation.Start ||
                                                                   reportItem.PageBreak == BreakLocation.None)
                                                                      ? (maxHeight - startTop)
                                                                      : ((2*maxHeight) - startTop);
                                    }
                                    else
                                        locationInfo.ActualTop -= startTop;
                                }
                                else
                                    locationInfo.ActualTop += maxHeight - startTop;
                            }

                            return true;
                        }
                    }
                    else if ((breakLocation == BreakLocation.None || breakLocation == BreakLocation.End) &&
                             (reportItem != null ||
                              (rptItemContainer.ReportItem.ContainerModel != null &&
                               rptItemContainer.ReportItem.ContainerModel is RectangleModel)))
                    {
                        if (reportItem == null && containerModel!=null && rptItemContainer.ReportItem.ContainerModel != null)
                        {
                            reportItem = (from item in containerModel.TopOrder
                                          where
                                              rptItemContainer.PreviousTopControlNames != null &&
                                              rptItemContainer.PreviousTopControlNames.Contains(item.Name)
                                          select item.ReportItem).LastOrDefault();

                        }
                        if (reportItem != null &&
                            (reportItem.PageBreak == BreakLocation.End ||
                             reportItem.PageBreak == BreakLocation.StartAndEnd))
                        {
                            startTop = startTop - rptItemContainer.TGap[reportItem.Name];
                            locationInfo.ActualTop += (maxHeight - startTop);

                            return true;
                        }
                    }
                }
            }
            catch
            {
            }

            if (updateMode != ReportItemViewMode.None && !(this.IsTablixReportItem(rptItemContainer) || this.IsContainerReportItem(rptItemContainer)) &&
                !double.IsNaN(maxHeight))
            {
                var reportItem = rptItemContainer.ReportItem;

                if (updateMode == ReportItemViewMode.Normal && reportItem.ContainerModel != null &&
                    reportItem.KeepTogether)
                {
                    return false;
                }

                if ((startTop + locationInfo.ActualHeight) >= maxHeight && locationInfo.ActualHeight <= maxHeight)
                {
                    double diff = maxHeight - startTop;
                    locationInfo.ActualTop += diff;
                    return true;
                }
            }

            return false;
        }

        private void UpdateReportItemLeft(ReportItemContiner reportItemContainer, ReportItemContiner parentContainer,
                                          double maxWidth, ReportItemViewMode updateMode, UpdateSection updateSection)
        {
            List<ReportItemContiner> leftOrder = null;
            LayoutReportItemModel locationInfo = new LayoutReportItemModel();

            if (updateSection == UpdateSection.Body)
            {
                leftOrder = this.LeftOrder;
            }
            else if (updateSection == UpdateSection.Header)
            {
                leftOrder = this.HeaderLeftOrder;
            }
            else if (updateSection == UpdateSection.Footer)
            {
                leftOrder = this.FooterLeftOrder;
            }
            else
            {
                leftOrder = parentContainer.LeftOrder;
            }

            if (reportItemContainer.PreviousLeftControlNames != null)
            {
                var reportItems = (from rptItemContainer in leftOrder
                                   where reportItemContainer.PreviousLeftControlNames.Contains(rptItemContainer.Name)
                                   select rptItemContainer.ReportItem).ToList();

                reportItems.Sort(delegate(IReportItemModeler first, IReportItemModeler second)
                    {
                        var right1 = first.FlowLayoutInfo.ActualLeft + first.FlowLayoutInfo.ActualWidth;
                        var right2 = second.FlowLayoutInfo.ActualLeft + second.FlowLayoutInfo.ActualWidth;

                        if (updateMode == ReportItemViewMode.Normal)
                        {
                            right1 = first.PageInfo.ActualLeft + first.PageInfo.ActualWidth;
                            right2 = second.PageInfo.ActualLeft + second.PageInfo.ActualWidth;
                        }
                        else if (updateMode == ReportItemViewMode.Print)
                        {
                            right1 = first.PrintPageInfo.ActualLeft + first.PrintPageInfo.ActualWidth;
                            right2 = second.PrintPageInfo.ActualLeft + second.PrintPageInfo.ActualWidth;
                        }

                        return right1.CompareTo(right2);
                    });

                var reportItem = reportItems.Last();
                double prevGap = reportItemContainer.LGap[reportItem.Name];

                if (updateMode == ReportItemViewMode.Print)
                {
                    locationInfo.ActualLeft = reportItem.PrintPageInfo.ActualLeft + reportItem.PrintPageInfo.ActualWidth +
                                              prevGap;
                    reportItemContainer.ReportItem.PrintPageInfo = locationInfo;
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    locationInfo.ActualLeft = reportItem.PageInfo.ActualLeft + reportItem.PageInfo.ActualWidth + prevGap;
                    reportItemContainer.ReportItem.PageInfo = locationInfo;
                }
                else
                {
                    locationInfo.ActualLeft = reportItem.FlowLayoutInfo.ActualLeft +
                                              reportItem.FlowLayoutInfo.ActualWidth + prevGap;
                    reportItemContainer.ReportItem.FlowLayoutInfo = locationInfo;
                }
            }
            else
            {
                locationInfo.ActualLeft = reportItemContainer.ReportItem.Left;

                if (updateSection == UpdateSection.Container)
                {
                    var reportItem = reportItemContainer.ReportItem;

                    if (reportItem.ContainerModel != null)
                    {
                        var parentReportItem = reportItem.ContainerModel;

                        if (updateMode == ReportItemViewMode.Print)
                        {
                            locationInfo.ActualLeft += parentReportItem.PrintPageInfo.ActualLeft;
                        }
                        else if (updateMode == ReportItemViewMode.Normal)
                        {
                            locationInfo.ActualLeft += parentReportItem.PageInfo.ActualLeft;
                        }
                        else
                        {
                            locationInfo.ActualLeft += parentReportItem.FlowLayoutInfo.ActualLeft;
                        }
                    }
                }

                if (updateMode == ReportItemViewMode.Print)
                {
                    reportItemContainer.ReportItem.PrintPageInfo = locationInfo;
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    reportItemContainer.ReportItem.PageInfo = locationInfo;
                }
                else
                {
                    reportItemContainer.ReportItem.FlowLayoutInfo = locationInfo;
                }
            }

            this.UpdateContainerModelWidth(reportItemContainer, maxWidth, updateMode);

            double leftValue = locationInfo.ActualLeft;
            double firstPreferedWidth = (updateMode == ReportItemViewMode.None &&
                                         reportItemContainer.ReportItem.KeepTogether)
                                            ? double.NaN
                                            : maxWidth - leftValue;

            if (!double.IsNaN(firstPreferedWidth))
            {
                int pageNo = this.FindPageStart(leftValue, maxWidth);

                if (pageNo != 0)
                {
                    double leftPageValue = (leftValue%(pageNo*maxWidth));
                    firstPreferedWidth = maxWidth - leftPageValue;
                }
            }

            reportItemContainer.ReportItem.UpdateWidth(firstPreferedWidth, locationInfo, maxWidth, updateMode);

            if (this.UpdateLeft(reportItemContainer, locationInfo, maxWidth, updateMode))
            {
                if (this.IsContainerReportItem(reportItemContainer))
                {
                    this.UpdateContainerModelWidth(reportItemContainer, maxWidth, updateMode);
                }

                leftValue = locationInfo.ActualLeft;

                if (!double.IsNaN(firstPreferedWidth))
                {
                    firstPreferedWidth = maxWidth - leftValue;

                    int pageNo = this.FindPageStart(leftValue, maxWidth);

                    if (pageNo != 0)
                    {
                        double leftPageValue = (leftValue%(pageNo*maxWidth));
                        firstPreferedWidth = maxWidth - leftPageValue;
                    }
                }

                reportItemContainer.ReportItem.UpdateWidth(firstPreferedWidth, locationInfo, maxWidth, updateMode);
            }
        }

        private void UpdateContainerModelWidth(ReportItemContiner reportItemContainer, double maxWidth,
                                               ReportItemViewMode updateMode)
        {
            if (this.IsContainerReportItem(reportItemContainer) && reportItemContainer.LeftOrder != null)
            {
                foreach (var model in reportItemContainer.LeftOrder)
                {
                    this.UpdateReportItemLeft(model, reportItemContainer, maxWidth, updateMode, UpdateSection.Container);
                }
            }
        }

        private bool UpdateLeft(ReportItemContiner rptItemContainer, LayoutReportItemModel locationInfo, double maxWidth,
                                ReportItemViewMode updateMode)
        {
            if (updateMode != ReportItemViewMode.None && !this.IsTablixReportItem(rptItemContainer) &&
                !double.IsNaN(maxWidth))
            {
                var reportItem = rptItemContainer.ReportItem;
                double startLeft = locationInfo.ActualLeft%maxWidth;

                if ((startLeft + locationInfo.ActualWidth) > maxWidth && locationInfo.ActualWidth <= maxWidth)
                {
                    double diff = maxWidth - startLeft;
                    locationInfo.ActualLeft += diff;
                    return true;
                }
            }

            return false;
        }

        private void UpdateReportItem(LayoutSize size, ReportItemViewMode updateMode)
        {
            if (updateMode != ReportItemViewMode.Print)
            {
                if (this.HeaderLeftOrder != null)
                {
                    foreach (var model in this.HeaderLeftOrder)
                    {
                        this.UpdateReportItemLeft(model, null, double.NaN, updateMode, UpdateSection.Header);
                    }

                    foreach (var model in this.HeaderTopOrder)
                    {
                        this.UpdateReportItemTop(model, null, double.NaN, updateMode, UpdateSection.Header);
                    }
                }

                if (this.FooterLeftOrder != null)
                {
                    foreach (var model in this.FooterLeftOrder)
                    {
                        this.UpdateReportItemLeft(model, null, double.NaN, updateMode, UpdateSection.Footer);
                    }

                    foreach (var model in this.FooterTopOrder)
                    {
                        this.UpdateReportItemTop(model, null, double.NaN, updateMode, UpdateSection.Footer);
                    }
                }
            }

            foreach (var model in this.LeftOrder)
            {
                this.UpdateReportItemLeft(model, null, size.Width, updateMode, UpdateSection.Body);
            }

            foreach (var model in this.TopOrder)
            {
                this.UpdateReportItemTop(model, null, size.Height, updateMode, UpdateSection.Body);
            }

            if (this.Model != null && this.Model.isContainsToggle)
            {
                ReportModelContentCollection itemCollection = new ReportModelContentCollection();
                itemCollection.AddRange(this.ReportBodyModelerLists.Where(t => t.Hidden == false));
                this.ReportBodyModelerLists = itemCollection;
            }
            //if (updateMode != ReportItemViewMode.None)
            {
                UpdatePageDetails(size.Height, size.Width, updateMode);
            }
        }

        private void UpdatePageDetails(double pageHeight, double pageWidth, ReportItemViewMode updateMode)
        {
            if (updateMode == ReportItemViewMode.Print)
            {
                double maxWidth = this.BodyRightGap;

                if (this.ReportBodyModelerLists.Count > 0)
                {
                    var pageModels = (from model in this.ReportBodyModelerLists
                                      where model.ContainerModel == null
                                      select model).ToList();

                    pageModels.Sort(delegate(IReportItemModeler first, IReportItemModeler second)
                        {
                            double firstRight = first.PrintPageInfo.ActualLeft + first.PrintPageInfo.ActualWidth;
                            double secondRight = second.PrintPageInfo.ActualLeft + second.PrintPageInfo.ActualWidth;
                            return firstRight.CompareTo(secondRight);
                        });

                    maxWidth = pageModels.Last().PrintPageInfo.ActualLeft + pageModels.Last().PrintPageInfo.ActualWidth +
                               this.BodyRightGap;
                }

                if (maxWidth <= pageWidth)
                {
                    this.widthPageCount = 1;
                }
                else
                {
                    this.widthPageCount = this.FindPageStart(maxWidth, pageWidth) + 1;
                }
            }

            bool isPageBreaked = false;
            //// Categorizing and assigning page
            foreach (IReportItemModeler ipm in this.ReportBodyModelerLists)
            {
                //// Starting Page
                int startPage = 0;
                int leftStartPage = 1;
                int ipmColumnPageCount = 1;
                int pageDiff = 1;

                double top = 0;

                if (updateMode == ReportItemViewMode.Print)
                {
                    top = ipm.PrintPageInfo.ActualTop;
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    top = ipm.PageInfo.ActualTop;
                }
                else
                {
                    top = ipm.FlowLayoutInfo.ActualTop;
                }

                if (pageHeight != 0 && updateMode != ReportItemViewMode.None)
                {
                    int topStartPage = this.FindPageStart(top, pageHeight);
                    startPage = this.widthPageCount*topStartPage;
                }

                if (updateMode == ReportItemViewMode.None && this.Model.isContainsPageBreak)
                {
                    int topStartPage = this.FindPageStart(top, maxHeight);
                    startPage = this.widthPageCount*topStartPage;
                }

                if (updateMode == ReportItemViewMode.Normal && ipm.ContainerModel != null)
                {
                    if ((ipm.PageBreak == BreakLocation.None && ipm.ContainerModel.KeepTogether && !isPageBreaked) &&
                        ipm.ContainerModel.PageInfo.BelongsTo != null)
                    {
                        startPage = ipm.ContainerModel.PageInfo.BelongsTo.First().Key;
                    }
                    else if ((ipm.PageBreak != BreakLocation.None && ipm.ContainerModel.KeepTogether) &&
                             ipm.ContainerModel.PageInfo.BelongsTo != null)
                    {
                        isPageBreaked = true;
                    }
                }

                if (updateMode == ReportItemViewMode.Print)
                {
                    leftStartPage = this.FindPageStart(ipm.PrintPageInfo.ActualLeft, pageWidth);
                    startPage += leftStartPage;
                    int leftEndPage = this.FindPageStart(
                        (ipm.PrintPageInfo.ActualLeft + ipm.PrintPageInfo.ActualWidth), pageWidth);
                    ipmColumnPageCount = leftEndPage - leftStartPage + 1;
                    pageDiff = this.widthPageCount == leftEndPage ? 1 : this.widthPageCount - leftEndPage;
                }

                if (updateMode == ReportItemViewMode.None && ipm.ContainerModel != null)
                {
                    if ((ipm.PageBreak == BreakLocation.None && !isPageBreaked) &&
                        ipm.ContainerModel.FlowLayoutInfo.BelongsTo != null)
                    {
                        startPage = ipm.ContainerModel.FlowLayoutInfo.BelongsTo.First().Key;
                    }
                    else if ((ipm.PageBreak != BreakLocation.None && ipm.ContainerModel.KeepTogether) &&
                             ipm.ContainerModel.FlowLayoutInfo.BelongsTo != null)
                    {
                        isPageBreaked = true;
                    }
                }

                Dictionary<int, int> blongsTo = new Dictionary<int, int>();

                int totalPages = 0;

                if (updateMode == ReportItemViewMode.None)
                {
                    totalPages = ipm.FlowLayoutInfo.TotalPages;
                }
                if (updateMode == ReportItemViewMode.Normal)
                {
                    totalPages = ipm.PageInfo.TotalPages;
                }
                if (updateMode == ReportItemViewMode.Print)
                {
                    totalPages = ipm.PrintPageInfo.TotalPages;
                }

#if WINRT
    //// TablixModel in all other pages except first page
                if (ipm is TablixModel || ipm is RectangleModel)
#else
                //// TablixModel in all other pages except first page
                if (ipm is TablixModel || ipm is RectangleModel || ipm is TextboxModel)
#endif
                {
                    for (int currPage = 0; currPage < totalPages;)
                    {
                        blongsTo.Add(startPage, currPage++);

                        if (updateMode == ReportItemViewMode.Print && (currPage%ipmColumnPageCount == 0))
                        {
                            startPage += pageDiff;
                            startPage += leftStartPage;
                        }
                        else
                        {
                            startPage++;
                        }
                    }
                }
                else
                {
                    //// Other controls in all pages except the first page
                    blongsTo.Add(startPage, 0);
                }

                if (updateMode == ReportItemViewMode.Print)
                {
                    ipm.PrintPageInfo.BelongsTo = blongsTo;
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    ipm.PageInfo.BelongsTo = blongsTo;
                }
                else
                {
                    ipm.FlowLayoutInfo.BelongsTo = blongsTo;
                }

                if (updateMode != ReportItemViewMode.None)
                {
                    if (ipm is TablixModel)
                    {
                        this.UpdatePageBelongsTablix(ipm as TablixModel, updateMode);
                    }
                    ////else if (ipm is RectangleModel)
                    //{
                    //    this.UpdatePageBlongsRectangle(ipm as RectangleModel, updateMode);
                    //}
                }
            }


            // Re-Assigning new Top
            foreach (IReportItemModeler ipmContent in this.ReportBodyModelerLists)
            {
                ipmContent.GUID = System.Guid.NewGuid();

                if (updateMode == ReportItemViewMode.Print)
                {
                    if (ipmContent.PrintPageInfo.ActualTop > 0 && ipmContent.PrintPageInfo.ActualTop >= pageHeight)
                    {
                        ipmContent.PrintPageInfo.ActualTop = ipmContent.PrintPageInfo.ActualTop%pageHeight;
                    }

                    if (ipmContent.PrintPageInfo.ActualLeft > 0 && ipmContent.PrintPageInfo.ActualLeft >= pageWidth)
                    {
                        ipmContent.PrintPageInfo.ActualLeft = ipmContent.PrintPageInfo.ActualLeft%pageWidth;
                    }
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    if (ipmContent.PageInfo.ActualTop > 0 && ipmContent.PageInfo.ActualTop >= pageHeight &&
                        !(ipmContent.ContainerModel != null && ipmContent.ContainerModel.KeepTogether))
                    {
                        ipmContent.PageInfo.ActualTop = ipmContent.PageInfo.ActualTop%
                                                        (ipmContent.PageInfo.BelongsTo.Keys.FirstOrDefault()*pageHeight);
                    }
                    else if (ipmContent.ContainerModel != null && ipmContent.ContainerModel.KeepTogether)
                    {
                        ipmContent.PageInfo.ActualTop = ipmContent.PageInfo.ActualTop -
                                                        (ipmContent.PageInfo.BelongsTo.Keys.FirstOrDefault()*pageHeight);
                    }
                }
            }

            UpdatePageDictionary(pageHeight, pageWidth, updateMode);
        }

        private void UpdatePageDictionary(double pageHeight, double pageWidth, ReportItemViewMode updateMode)
        {
            //// Finding total number of pages
            int maxPageNumber = 0;
            foreach (IReportItemModeler ipm in this.ReportBodyModelerLists)
            {
                Dictionary<int, int> belongsTo = null;

                if (updateMode == ReportItemViewMode.None)
                {
                    belongsTo = ipm.FlowLayoutInfo.BelongsTo;
                }
                if (updateMode == ReportItemViewMode.Normal)
                {
                    belongsTo = ipm.PageInfo.BelongsTo;
                }
                if (updateMode == ReportItemViewMode.Print)
                {
                    belongsTo = ipm.PrintPageInfo.BelongsTo;
                }

                foreach (int key in belongsTo.Keys)
                {
                    maxPageNumber = key > maxPageNumber ? key : maxPageNumber;
                }
            }

            //Increasing page count if last report item has page break
            if (this.TopOrder.Count != 0)
            {
                maxPageNumber = this.TopOrder.LastOrDefault() != null &&
                                this.TopOrder.Last().ReportItem.PageBreak == BreakLocation.StartAndEnd
                                || this.TopOrder.Last().ReportItem.PageBreak == BreakLocation.End
                                    ? maxPageNumber + 1
                                    : maxPageNumber;
            }

            //// Controls specific to the page number where grouped - Cliping is not done yet
            ReportModelContentCollection newPageModelContentCollection = null;
            List<Guid> guidList = new List<Guid>();


            for (int pageNumbers = 0; pageNumbers <= maxPageNumber; pageNumbers++)
            {
                newPageModelContentCollection = new ReportModelContentCollection();

                foreach (IReportItemModeler finalIPM in this.ReportBodyModelerLists)
                {
                    Dictionary<int, int> belongsTo = null;
                    if (updateMode == ReportItemViewMode.None)
                    {
                        belongsTo = finalIPM.FlowLayoutInfo.BelongsTo;
                    }
                    if (updateMode == ReportItemViewMode.Normal)
                    {
                        belongsTo = finalIPM.PageInfo.BelongsTo;
                    }
                    if (updateMode == ReportItemViewMode.Print)
                    {
                        belongsTo = finalIPM.PrintPageInfo.BelongsTo;
                    }

                    if (belongsTo.Keys.Contains(pageNumbers))
                    {
                        newPageModelContentCollection.Add(finalIPM);
                    }
                }

                double maximumPageWidth = this.normalPageWidth;
                double maximumPageHeight = 0;
                double prevControlHeight = 0;
                double prevControlWidth = 0;

                if (updateMode == ReportItemViewMode.Print)
                {
                    maximumPageHeight = pageHeight;
                    maximumPageWidth = pageWidth;
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    foreach (IReportItemModeler model in newPageModelContentCollection)
                    {
                        prevControlHeight = model.PageInfo.ActualTop + model.PageInfo.ActualHeight;
                        prevControlWidth = model.PageInfo.ActualLeft + model.PageInfo.ActualWidth;

                        if (model is TablixModel)
                        {
                            prevControlHeight = model.PageInfo.ActualTop +
                                                (model as TablixModel).PageSizes[model.PageInfo.BelongsTo[pageNumbers]]
                                                    .Height;
                        }

                        else if (model is RectangleModel)
                        {
                            prevControlHeight = model.PageInfo.ActualTop +
                                                (model as RectangleModel).PageSizes[model.PageInfo.BelongsTo[pageNumbers]]
                                                    .Height;
                        }


                        maximumPageHeight = prevControlHeight > maximumPageHeight
                                                ? prevControlHeight
                                                : maximumPageHeight;
                        maximumPageWidth = prevControlWidth > maximumPageWidth ? prevControlWidth : maximumPageWidth;
                    }
                }

                ReportPageInfo info = new ReportPageInfo();

                info.Height = maximumPageHeight;
                info.Width = maximumPageWidth;

                info.ReportModelCollection = newPageModelContentCollection;

                //// This PaegDictinoary will hold the pages and the IPagemodeler of controls to be rendered in each page
                if (updateMode == ReportItemViewMode.Print)
                {
                    this.printLayoutPageDictionary.Add(pageNumbers, info);
#if WINRT
                    this.setPageNo(info.ReportModelCollection, pageNumbers);
#endif
                }
                else if (updateMode == ReportItemViewMode.Normal)
                {
                    info.Width = info.Width + this.BodyRightGap;
                    this.pageDictionary.Add(pageNumbers, info);
#if !WINRT
                    this.setPageNo(info.ReportModelCollection, pageNumbers);
#endif
                }
                else
                {
                    this.flowLayoutDictionary.Add(pageNumbers, info);
                }
            }

            if (updateMode == ReportItemViewMode.Normal)
            {
                this.pageDictionary.Last().Value.Height = this.pageDictionary.Last().Value.Height + this.BodyBottomGap;
            }
        }

        #endregion

        void setPageNo(ReportModelContentCollection collection, int pageNumber)
        {
            foreach (IReportItemModeler reportItemModeler in collection)
            {
                reportItemModeler.UpdatePageNo(pageNumber);
            }
        }

        private void UpdatePageBelongsTablix(TablixModel model, ReportItemViewMode viewMode)
        {
            var blongsTo = viewMode == ReportItemViewMode.Print? model.PrintPageInfo.BelongsTo: model.PageInfo.BelongsTo;
            var pageSizes = viewMode == ReportItemViewMode.Print ? model.PrintPageSizes : model.PageSizes;
            foreach (var page in blongsTo)
            {
                if (pageSizes != null)
                {
                    if (pageSizes.ContainsKey(page.Value))
                    {
                        var pageSize = pageSizes[page.Value];
                        if (model.ItemPosition!=null && model.ItemPosition.Count > 0)
                        {
                            foreach (var rowPair in model.ItemPosition)
                            {
                                var i = rowPair.Key;
                                foreach (var colPair in rowPair.Value)
                                {
                                    var j = colPair;
                                    var cellModel = model.Data[i][j];

                                    if (cellModel != null && cellModel.ItemModel != null)
                                    {
                                        if (cellModel.ItemModel.ModelType == ModelType.TablixModel ||
                                            cellModel.ItemModel.ModelType == ModelType.RectangleModel)
                                        {
                                            if (this.Model.EnableVirtualEvaluation &&
                                                cellModel.ItemModel.ModelType == ModelType.TablixModel)
                                            {
                                                var model1 = (cellModel.ItemModel as TablixModel);
                                                TablixEvaluationItems items = cellModel.Rows[i].TablixValues[j][model1.Name];
                                                model1.UpdateTablixValue(model1, items);
                                            }

                                            if (pageSize.RowIndices.Contains(i + 1) &&
                                                pageSize.ColumnIndices.Contains(j + 1))
                                            {
                                                if (viewMode != ReportItemViewMode.Print)
                                                {
                                                    if (cellModel.ItemModel.PageInfo.BelongsTo != null)
                                                    {
                                                        if (!cellModel.ItemModel.PageInfo.BelongsTo.ContainsKey(page.Value))
                                                        {
                                                            var pageValue = cellModel.ItemModel.PageInfo.BelongsTo.Last().Value;
                                                            cellModel.ItemModel.PageInfo.BelongsTo.Add(page.Value, pageValue + 1);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        cellModel.ItemModel.PageInfo.BelongsTo = new Dictionary<int, int>();
                                                        cellModel.ItemModel.PageInfo.BelongsTo.Add(page.Value, 0);
                                                    }
                                                }
                                                else
                                                {
                                                    if (cellModel.ItemModel.PrintPageInfo.BelongsTo != null)
                                                    {
                                                        if (!cellModel.ItemModel.PrintPageInfo.BelongsTo.ContainsKey(page.Value))
                                                        {
                                                            var pageValue = cellModel.ItemModel.PrintPageInfo.BelongsTo.Last().Value;
                                                            cellModel.ItemModel.PrintPageInfo.BelongsTo.Add(page.Value,pageValue + 1);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        cellModel.ItemModel.PrintPageInfo.BelongsTo = new Dictionary<int, int>();
                                                        cellModel.ItemModel.PrintPageInfo.BelongsTo.Add(page.Value, 0);
                                                    }
                                                }
                                                if (cellModel.ItemModel.ModelType == ModelType.RectangleModel)
                                                {
                                                    this.UpdatePageBlongsRectangle(cellModel.ItemModel as RectangleModel,
                                                                                   viewMode);
                                                }
                                                else
                                                {
                                                    this.UpdatePageBelongsTablix(cellModel.ItemModel as TablixModel,
                                                                                 viewMode);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdatePageBlongsRectangle(RectangleModel model, ReportItemViewMode viewMode)
        {
            var blongsTo = viewMode == ReportItemViewMode.Print ? model.PrintPageInfo.BelongsTo : model.PageInfo.BelongsTo;
            var pageSizes = viewMode == ReportItemViewMode.Print ? model.PrintPageSizes : model.PageSizes;
            var width = 0.0;
            var height = 0.0;
            var left = model.Left;
            var top = model.Top;
            foreach (var page in blongsTo)
            {
                if (pageSizes.ContainsKey(page.Value))
                {
                    var pageSize = pageSizes[page.Value];
                    if (width != pageSize.Width)
                    {
                        width += pageSize.Width;
                        left = width - pageSize.Width;
                    }
                    if (height != pageSize.Height)
                    {
                        height += pageSize.Height;
                        top = height - pageSize.Height;
                    }
                    foreach (var reportItem in model.ReportItemModelers)
                    {
                        if (reportItem.ModelType == ModelType.RectangleModel || reportItem.ModelType == ModelType.TablixModel)
                        {
                            if (((left <= reportItem.Left && width >= reportItem.Left) && (top <= reportItem.Top && height >= reportItem.Top)) || ((left <= reportItem.Width && width >= reportItem.Width) && (top <= reportItem.Height && height >= reportItem.Height)))
                            {
                                if (viewMode != ReportItemViewMode.Print)
                                {
                                    if (reportItem.PageInfo.BelongsTo != null)
                                    {
                                        if (!reportItem.PageInfo.BelongsTo.ContainsKey(page.Value))
                                        {
                                            var pageValue = reportItem.PageInfo.BelongsTo.Last().Value;
                                            reportItem.PageInfo.BelongsTo.Add(page.Value, pageValue + 1);
                                        }
                                    }
                                    else
                                    {
                                        reportItem.PageInfo.BelongsTo = new Dictionary<int, int>();
                                        reportItem.PageInfo.BelongsTo.Add(page.Value, 0);
                                    }
                                }
                                else
                                {
                                    if (reportItem.PrintPageInfo.BelongsTo != null)
                                    {
                                        if (!reportItem.PrintPageInfo.BelongsTo.ContainsKey(page.Value))
                                        {

                                            var pageValue = reportItem.PrintPageInfo.BelongsTo.Last().Value;
                                            reportItem.PrintPageInfo.BelongsTo.Add(page.Value, pageValue + 1);
                                        }
                                    }
                                    else
                                    {
                                        reportItem.PrintPageInfo.BelongsTo = new Dictionary<int, int>();
                                        reportItem.PrintPageInfo.BelongsTo.Add(page.Value, 0);
                                    }
                                }
                                if (reportItem.ModelType == ModelType.TablixModel)
                                {
                                    this.UpdatePageBelongsTablix(reportItem as TablixModel, viewMode);
                                }
                                else
                                {
                                    this.UpdatePageBlongsRectangle(reportItem as RectangleModel, viewMode);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

