#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.RDL.Data;
using Syncfusion.RDL.ItemModel;
using Syncfusion.RDL.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.RDL.Controls
{
    internal sealed class ReportingSubReport : ContentControl
    {
        bool isPrintMode = false;

        Canvas subReportControl;

        SubReportModel SubReportModel
        {
            get;
            set;
        }

        internal bool IsPrintMode
        {
            get
            {
                return isPrintMode;
            }
            set
            {
                isPrintMode = value;

                if (isPrintMode)
                {
                    this.Margin = new Thickness(SubReportModel.PrintPageInfo.ActualLeft, SubReportModel.PrintPageInfo.ActualTop, 0, 0);
                }
                else
                {
                    this.Margin = new Thickness(SubReportModel.PageInfo.ActualLeft, SubReportModel.PageInfo.ActualTop, 0, 0);
                }
            }
        }

        internal Dictionary<int, PageInfo> PageSizes
        {
            get
            {
                if (this.IsPrintMode)
                {
                    return this.SubReportModel.PrintPageSizes;
                }

                return this.SubReportModel.PageSizes;
            }
        }

        int currentPage = -1;

        public int CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            set
            {
                this.currentPage = value;

                if (this.IsPrintMode)
                {
                    this.Margin = new Thickness(this.currentPage % this.SubReportModel.PrintPageColumnCount == 0 ?
                    this.SubReportModel.PrintPageInfo.ActualLeft : 0, this.currentPage < this.SubReportModel.PrintPageColumnCount ?
                    this.SubReportModel.PrintPageInfo.ActualTop : 0, 0, 0);
                }
                else
                {
                    this.Margin = new Thickness(this.SubReportModel.PageInfo.ActualLeft, this.currentPage == 0 ? this.SubReportModel.PageInfo.ActualTop : 0, 0, 0);
                }

                this.Height = this.PageSizes[this.CurrentPage].Height;
                this.Width = this.PageSizes[this.CurrentPage].Width;
            }
        }

        internal IReportItemModeler Model
        {
            get;
            set;
        }

        public ReportingSubReport(IReportItemModeler model)
        {
            this.Model = model;
            this.SubReportModel = this.Model as SubReportModel;
            this.subReportControl = new Canvas();
            this.Initalize();
            this.Content = this.subReportControl;
        }

        void Initalize()
        {
            this.Height = this.SubReportModel.Height;
            this.Width = this.SubReportModel.Width;

            if (this.SubReportModel.PrintPageInfo != null)
            {
                Canvas.SetLeft(this, this.SubReportModel.PrintPageInfo.ActualLeft);
                Canvas.SetTop(this, this.SubReportModel.PrintPageInfo.ActualTop);
            }
            else if (this.SubReportModel.IsTablixChild && this.SubReportModel.PageInfo != null)
            {
                Canvas.SetLeft(this, this.SubReportModel.PageInfo.ActualLeft);
                Canvas.SetTop(this, this.SubReportModel.PageInfo.ActualTop);
            }
        }
    }
}
