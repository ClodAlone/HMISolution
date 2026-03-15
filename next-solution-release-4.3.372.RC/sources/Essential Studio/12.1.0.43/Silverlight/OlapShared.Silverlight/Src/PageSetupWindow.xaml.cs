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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.OlapSilverlight.Manager;
using System.ComponentModel;

namespace Syncfusion.Silverlight.Shared.Olap
{
    [DesignTimeVisible(false)]
    public partial class PageSetupWindow : WindowControl
    {
        OlapDataManager DataManager;        

        public PageSetupWindow(OlapDataManager dataManager, DisplayMode dispMode)
        {
            InitializeComponent();
            this.DataManager = dataManager;
            switch (dispMode)
            {
                case DisplayMode.Both:
                    this.categoricalPageSettings.Visibility = System.Windows.Visibility.Visible;
                    this.seriesPageSettings.Visibility = System.Windows.Visibility.Visible;
                    break;
                case DisplayMode.CategorialOnly:
                    this.categoricalPageSettings.Visibility = System.Windows.Visibility.Visible;
                    this.seriesPageSettings.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case DisplayMode.SeriesOnly:
                    this.seriesPageSettings.Visibility = System.Windows.Visibility.Visible;
                    this.categoricalPageSettings.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            if (this.DataManager != null && this.DataManager.CurrentReport != null)
            {
                this.categoricalCurrentPage.Value = this.DataManager.CurrentReport.PagerOptions.CategorialCurrentPage;
                this.seriesCurrentPage.Value = this.DataManager.CurrentReport.PagerOptions.SeriesCurrentPage;
                this.categoricalPageSize.Value = this.DataManager.CurrentReport.PagerOptions.CategorialPageSize;
                this.seriesPageSize.Value = this.DataManager.CurrentReport.PagerOptions.SeriesPageSize;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataManager != null && this.DataManager.CurrentReport != null)
            {
                //int categoicalCurrentPage, seriesCurrentPage, categoricalPageSize, seriesPageSize;
                
                //if (!(Int32.TryParse(this.categoricalCurrentPage.Value.ToString(), out categoicalCurrentPage)))
                //{
                //    categoicalCurrentPage = 1;
                //}

                //if (!(Int32.TryParse(this.seriesCurrentPage.Value.ToString(), out seriesCurrentPage)))
                //{
                //    seriesCurrentPage = 1;
                //}

                //if (!(Int32.TryParse(this.categoricalPageSize.Value.ToString(), out categoricalPageSize)))
                //{
                //    categoricalPageSize = 50;
                //}

                //if (!(Int32.TryParse(this.seriesPageSize.Value.ToString(), out seriesPageSize)))
                //{
                //    seriesPageSize = 50;
                //}

                //this.DataManager.CurrentReport.PagerOptions.CategorialCurrentPage = categoicalCurrentPage;
                //this.DataManager.CurrentReport.PagerOptions.SeriesCurrentPage = seriesCurrentPage;
                //this.DataManager.CurrentReport.PagerOptions.CategorialPageSize = categoricalPageSize;
                //this.DataManager.CurrentReport.PagerOptions.SeriesPageSize = seriesPageSize;

                this.DataManager.CurrentReport.PagerOptions.CategorialCurrentPage = Convert.ToInt32(this.categoricalCurrentPage.Value);
                this.DataManager.CurrentReport.PagerOptions.SeriesCurrentPage = Convert.ToInt32(this.seriesCurrentPage.Value);
                this.DataManager.CurrentReport.PagerOptions.CategorialPageSize = Convert.ToInt32(this.categoricalPageSize.Value);
                this.DataManager.CurrentReport.PagerOptions.SeriesPageSize = Convert.ToInt32(this.seriesPageSize.Value);

                this.DataManager.ExecuteCellSet();
            }

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();            
        }
    }
}

