#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.OlapSilverlight.Manager;
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Windows.Media;

namespace Syncfusion.Silverlight.Shared.Olap
{
    /// <summary>
    /// OLAP Pager control for paging operations with Data Manager.
    /// </summary>
    public class OlapPager : Control
    {
        #region [ Members ]

        private OlapPagedCollectionView _categoriesPagedSource;
        private OlapPagedCollectionView _seriesPagedSource;
        private Button _pageSetupButton;
        private int _categoriesTotalCount;
        private int _seriesTotalCount;

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapPager"/> class.
        /// </summary>
        public OlapPager()
        {
            this.DefaultStyleKey = typeof(OlapPager);
            Loaded += new RoutedEventHandler(OlapPager_Loaded);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the categorical pager.
        /// </summary>
        /// <value>The categorical pager.</value>
        [Browsable(false)]
        public DataPagerExt CategoricalPager { get; set; }

        /// <summary>
        /// Gets or sets the series pager.
        /// </summary>
        /// <value>The series pager.</value>
        [Browsable(false)]
        public DataPagerExt SeriesPager { get; set; }

        /// <summary>
        /// Gets or sets the categorical pager text.
        /// </summary>
        /// <value>The categorical pager text.</value>
        [Browsable(false)]
        public TextBlock CategoricalPagerText { get; set; }

        /// <summary>
        /// Gets or sets the series pager text.
        /// </summary>
        /// <value>The series pager text.</value>
        [Browsable(false)]
        public TextBlock SeriesPagerText { get; set; }

        #endregion

        #region  [ Dependency Properties ]

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The visual style.</value>
        public VisualStyle VisualStyle
        {
            get { return (VisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(VisualStyle), typeof(OlapPager), new PropertyMetadata(Syncfusion.Windows.Shared.VisualStyle.Default));
        
        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        public DisplayMode DisplayMode
        {
            get { return (DisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(DisplayMode), typeof(OlapPager), new PropertyMetadata(DisplayMode.Both, OnDisplayModeChanged));

        /// <summary>
        /// Gets or sets the OLAP data manager.
        /// </summary>
        /// <value>The OLAP data manager.</value>
        public OlapDataManager OlapDataManager
        {
            get { return (OlapDataManager)GetValue(OlapDataManagerProperty); }
            set { SetValue(OlapDataManagerProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof(OlapDataManager), typeof(OlapPager), new PropertyMetadata(null, OnOlapDataManagerChanged));


        /// <summary>
        /// Gets or sets the size of the series page.
        /// </summary>
        /// <value>The size of the series page.</value>
        public int SeriesPageSize
        {
            get { return (int)GetValue(SeriesPageSizeProperty); }
            set { SetValue(SeriesPageSizeProperty, value); }
        }

        public static readonly DependencyProperty SeriesPageSizeProperty =
            DependencyProperty.Register("SeriesPageSize", typeof(int), typeof(OlapPager), new PropertyMetadata(50));

        /// <summary>
        /// Gets or sets the size of the categorical page.
        /// </summary>
        /// <value>The size of the categorical page.</value>
        public int CategoricalPageSize
        {
            get { return (int)GetValue(CategoricalPageSizeProperty); }
            set { SetValue(CategoricalPageSizeProperty, value); }
        }

        public static readonly DependencyProperty CategoricalPageSizeProperty =
            DependencyProperty.Register("CategoricalPageSize", typeof(int), typeof(OlapPager), new PropertyMetadata(50));

        /// <summary>
        /// Gets or sets the series current page.
        /// </summary>
        /// <value>The series current page.</value>
        public int SeriesCurrentPage
        {
            get { return (int)GetValue(SeriesCurrentPageProperty); }
            set { SetValue(SeriesCurrentPageProperty, value); }
        }

        public static readonly DependencyProperty SeriesCurrentPageProperty =
            DependencyProperty.Register("SeriesCurrentPage", typeof(int), typeof(OlapPager), new PropertyMetadata(1));


        /// <summary>
        /// Gets or sets the categorical current page.
        /// </summary>
        /// <value>The categorical current page.</value>
        public int CategoricalCurrentPage
        {
            get { return (int)GetValue(CategoricalCurrentPageProperty); }
            set { SetValue(CategoricalCurrentPageProperty, value); }
        }

        public static readonly DependencyProperty CategoricalCurrentPageProperty =
            DependencyProperty.Register("CategoricalCurrentPage", typeof(int), typeof(OlapPager), new PropertyMetadata(1));


        /// <summary>
        /// Called when [display mode changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapPager pagerControl = d as OlapPager;
            if (pagerControl != null && (args.NewValue != args.OldValue))
            {
                pagerControl.SetDisplayMode(pagerControl.DisplayMode);
                pagerControl.UpdatePageSizesFromCurrentReport();
            }
        }

        /// <summary>
        /// Called when [OLAP data manager changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOlapDataManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            OlapPager pagerControl = d as OlapPager;
            if (pagerControl != null && args.NewValue != args.OldValue)
            {
                if (pagerControl.OlapDataManager == null)
                {
                    pagerControl.ResetPager();
                    return;
                }
                pagerControl.OlapDataManagerChanged();
            }
        }

        #endregion

        #region [ Overridden Methods ]

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.CategoricalPager = GetTemplateChild("PART_CategoricalPager") as DataPagerExt;
            this.SeriesPager = GetTemplateChild("PART_SeriesPager") as DataPagerExt;

            this.CategoricalPagerText = GetTemplateChild("PART_CategoricalPagerText") as TextBlock;
            this.CategoricalPagerText.Text = Syncfusion.OlapShared.Silverlight.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapPager_ColumnPager");
            this.SeriesPagerText = GetTemplateChild("PART_SeriesPagerText") as TextBlock;
            this.SeriesPagerText.Text = Syncfusion.OlapShared.Silverlight.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapPager_RowPager");

            this._pageSetupButton = GetTemplateChild("PART_PageSetupButton") as Button;
            if (this._pageSetupButton != null) this._pageSetupButton.Click += new RoutedEventHandler(PageSetupButton_Click);
            SetDisplayMode(this.DisplayMode);
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Handles the Loaded event of the OlapPager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OlapPager_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePageSizesFromCurrentReport();
        }

        /// <summary>
        /// Called when data manager changed.
        /// </summary>
        internal void OlapDataManagerChanged()
        {
            if (this.OlapDataManager.CurrentReport != null)
                this.OlapDataManager.ExecuteCellSet();

            this.OlapDataManager.CellSetChanged += new CellSetChangedEventHandler(OlapDataManager_CellSetChanged);
        }

        /// <summary>
        /// Handles the CellSetChanged event of the OlapDataManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.OlapSilverlight.Manager.CellSetChangedEventArgs"/> instance containing the event data.</param>
        void OlapDataManager_CellSetChanged(object sender, CellSetChangedEventArgs e)
        {
            if (this.OlapDataManager.Counts == null) return;
            if (this.CategoricalPager == null) return;
            if (this.SeriesPager == null) return;
            this._categoriesTotalCount = OlapDataManager.Counts["Column"];
            this._seriesTotalCount = OlapDataManager.Counts["Row"];

            this.UpdatePageSizesFromCurrentReport();

            _categoriesPagedSource = new OlapPagedCollectionView();
            _categoriesPagedSource.SetTotalItemCount(_categoriesTotalCount);
            _categoriesPagedSource.SetPageIndex(this.OlapDataManager.CurrentReport.PagerOptions.CategorialCurrentPage - 1);
            _categoriesPagedSource.PageSize = CategoricalPageSize;
            this.CategoricalPager.Source = _categoriesPagedSource;
            this.CategoricalPager.UpdateControl();

            _categoriesPagedSource.PageChanged -= new EventHandler<EventArgs>(categoriesPagedSource_PageChanged);
            _categoriesPagedSource.PageChanged += new EventHandler<EventArgs>(categoriesPagedSource_PageChanged);

            _seriesPagedSource = new OlapPagedCollectionView();
            _seriesPagedSource.SetTotalItemCount(_seriesTotalCount);
            _seriesPagedSource.SetPageIndex(this.OlapDataManager.CurrentReport.PagerOptions.SeriesCurrentPage - 1);
            _seriesPagedSource.PageSize = SeriesPageSize;
            this.SeriesPager.Source = _seriesPagedSource;
            this.SeriesPager.UpdateControl();

            _seriesPagedSource.PageChanged -= new EventHandler<EventArgs>(seriesPagedSource_PageChanged);
            _seriesPagedSource.PageChanged += new EventHandler<EventArgs>(seriesPagedSource_PageChanged);
        }

        /// <summary>
        /// Handles the PageChanged event of the seriesPagedSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void seriesPagedSource_PageChanged(object sender, EventArgs e)
        {
            SeriesPager.PageIndex = _seriesPagedSource.PageIndex;
            if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null)
            {
                this.OlapDataManager.CurrentReport.PagerOptions.SeriesCurrentPage = SeriesPager.PageIndex + 1;
                this.OlapDataManager.ExecuteCellSet();
            }
            SeriesPager.UpdateControl();
        }

        /// <summary>
        /// Handles the PageChanged event of the categoriesPagedSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void categoriesPagedSource_PageChanged(object sender, EventArgs e)
        {
            CategoricalPager.PageIndex = _categoriesPagedSource.PageIndex;
            if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null)
            {
                this.OlapDataManager.CurrentReport.PagerOptions.CategorialCurrentPage = CategoricalPager.PageIndex + 1;
                this.OlapDataManager.ExecuteCellSet();
            }
            CategoricalPager.UpdateControl();
        }

        /// <summary>
        /// Updates the page sizes from current report.
        /// </summary>
        private void UpdatePageSizesFromCurrentReport()
        {
            if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.EnablePaging)
            {
                this.CategoricalCurrentPage = this.OlapDataManager.CurrentReport.PagerOptions.CategorialCurrentPage;
                this.SeriesCurrentPage = this.OlapDataManager.CurrentReport.PagerOptions.SeriesCurrentPage;
                this.CategoricalPageSize = this.OlapDataManager.CurrentReport.PagerOptions.CategorialPageSize;
                this.SeriesPageSize = this.OlapDataManager.CurrentReport.PagerOptions.SeriesPageSize;
               // if (this.OlapDataManager.CurrentCellSet == null)
                 //   this.OlapDataManager.ExecuteCellSet();
            }
            else
                this.ResetPager();
        }

        /// <summary>
        /// Resets the pager.
        /// </summary>
        private void ResetPager()
        {
            this.CategoricalCurrentPage = 1;
            this.CategoricalPageSize = 50;
            this._categoriesTotalCount = 0;
            this.SeriesCurrentPage = 1;
            this.SeriesPageSize = 50;
            this._seriesTotalCount = 0;
        }

        /// <summary>
        /// Sets the display mode.
        /// </summary>
        /// <param name="dispMode">The display mode.</param>
        private void SetDisplayMode(DisplayMode dispMode)
        {
            if (this.CategoricalPager != null && this.SeriesPager != null && this.SeriesPagerText != null && this.CategoricalPagerText != null)
            {
                switch (dispMode)
                {
                    case DisplayMode.Both:
                        this.CategoricalPagerText.Visibility = System.Windows.Visibility.Visible;
                        this.SeriesPagerText.Visibility = System.Windows.Visibility.Visible;
                        this.CategoricalPager.Visibility = System.Windows.Visibility.Visible;
                        this.SeriesPager.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case DisplayMode.CategorialOnly:
                        this.CategoricalPagerText.Visibility = System.Windows.Visibility.Visible;
                        this.SeriesPagerText.Visibility = System.Windows.Visibility.Collapsed;
                        this.CategoricalPager.Visibility = System.Windows.Visibility.Visible;
                        this.SeriesPager.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case DisplayMode.SeriesOnly:
                        this.CategoricalPagerText.Visibility = System.Windows.Visibility.Collapsed;
                        this.SeriesPagerText.Visibility = System.Windows.Visibility.Visible;
                        this.CategoricalPager.Visibility = System.Windows.Visibility.Collapsed;
                        this.SeriesPager.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }

        void PageSetupButton_Click(object sender, RoutedEventArgs e)
        {
            PageSetupWindow window = new PageSetupWindow(this.OlapDataManager, this.DisplayMode);
            window.Title = Syncfusion.OlapShared.Silverlight.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapPager_PageSettings");
            window.FlowDirection = this.FlowDirection;
            window.VisualStyle = this.VisualStyle;
            GradientStopCollection gradiantCollection = new GradientStopCollection();
            switch (window.VisualStyle)
            {
                case Syncfusion.Windows.Shared.VisualStyle.Aero:
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Blend:
                    window.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 59, 59, 59));
                    window.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Default:
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2003:
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2007Black:
                    gradiantCollection.Clear();
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 235, 237, 238), Offset = 0.132 });
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 224, 227, 230), Offset = 0.373 });
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 235, 237, 238), Offset = 0.379 });
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 216, 219, 222), Offset = 0.652 });
                    window.LayoutRoot.Background = new LinearGradientBrush() { GradientStops = gradiantCollection, StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2007Blue:
                    gradiantCollection.Clear();
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 203, 222, 243), Offset = 0.378 });
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 194, 212, 236), Offset = 1 });
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 203, 222, 245), Offset = 0.652 });
                    window.LayoutRoot.Background = new LinearGradientBrush() { GradientStops = gradiantCollection, StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2007Silver:
                    gradiantCollection.Clear();
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 229, 232, 244), Offset = 0.041 });
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 216, 220, 230), Offset = 0.376 });
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 229, 232, 244), Offset = 0.38 });
                    gradiantCollection.Add(new GradientStop { Color = Color.FromArgb(255, 208, 210, 220), Offset = 0.612 });
                    window.LayoutRoot.Background = new LinearGradientBrush() { GradientStops = gradiantCollection, StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2010Black:
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2010Blue:
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Office2010Silver:
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.VS2010:
                    break;
                case Syncfusion.Windows.Shared.VisualStyle.Windows7:
                    break;
                default:
                    break;
            }
            window.ShowDialog();
        }

        #endregion
    }
}
