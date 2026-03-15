#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid.Ria
{
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
    using System.Collections.ObjectModel;
    using Syncfusion.Windows.ComponentModel;
    using System.Windows.Data;
    using Syncfusion.Windows.Data;
    using System.Windows.Media.Imaging;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Markup;
    using System.IO;
    using System.Windows.Controls.Primitives;

    public class DomainGridDataControl : GridDataControl
    {
        public DomainGridDataControl()
            : base()
        {
        }


        internal DomainGridDataTableProperties TableProperties
        {
            get;
            set;
        }

        protected override GridDataTableProperties GetTableProperties()
        {
            this.TableProperties = new DomainGridDataTableProperties();
            return this.TableProperties as GridDataTableProperties;
        }

        private Storyboard Rotate
        {
            get;
            set;
        }

        private FrameworkElement LoadingIndicator
        {
            get;
            set;
        }

        public new DomainGridDataTableModel Model
        {
            get
            {
                return this.model;
            }
        }

        private DomainGridDataTableModel model = null;

        protected override GridDataTableModel OnModelCreated()
        {
            this.model = new DomainGridDataTableModel();
            if (this.TableProperties != null)
            {
                this.TableProperties.Model = this.Model;
            }

            return this.model as GridDataTableModel;
        }


        public static readonly DependencyProperty RIASourceProperty = DependencyProperty.Register(
          "RIASource",
          typeof(DomainDataSource),
          typeof(DomainGridDataControl),
          new PropertyMetadata(OnRIASourcePropertyChanged));


        private bool isRiaSourcechanged = false;
        private static void OnRIASourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as DomainGridDataControl;
            if (grid.Model != null)
            {
                grid.Model.RIASource = (DomainDataSource)args.NewValue;
                if ((DomainDataSource)args.NewValue != null)
                {
                    grid.ItemsSource = ((DomainDataSource)args.NewValue).Data;
                }
            }
            else
            {
                grid.isRiaSourcechanged = true;
            }
        }

        public string LoadingText
        {
            get;
            set;
        }

        private TextBlock LoadingTextBlock
        {
            get;
            set;
        }


        public DomainDataSource RIASource
        {
            get
            {
                return (DomainDataSource)this.GetValue(DomainGridDataControl.RIASourceProperty);
            }
            set
            {
                this.SetValue(DomainGridDataControl.RIASourceProperty, value);
            }
        }



        private void SetRIAProperties()
        {
            this.Model.HasRIASource = true;
            if (this.Relations != null)
            {
                this.Relations.Clear();
            }

            this.AutoPopulateRelations = false;
            this.Model.RIASource.SubmittingChanges += new EventHandler<SubmittingChangesEventArgs>(RIASource_SubmittingChanges);
            this.Model.RIASource.SubmittedChanges += new EventHandler<SubmittedChangesEventArgs>(RIASource_SubmittedChanges);
            this.Model.RIASource.LoadingData += new EventHandler<LoadingDataEventArgs>(RIASource_LoadingData);
            this.Model.RIASource.LoadedData += new EventHandler<LoadedDataEventArgs>(RIASource_LoadedData);
#if SyncfusionFramework3_5
            if (this.Model.RIASource.FilterDescriptors == null && this.Model.RIASource.CanLoad)
            {
                this.Model.RIASource.FilterDescriptors = new FilterDescriptorCollection();
            }
#endif

            if (this.Model.RIASource.SortDescriptors != null && this.Model.RIASource.SortDescriptors.Count > 0)
            {
                foreach (var desc in this.Model.RIASource.SortDescriptors)
                {
#if SyncfusionFramework3_5
                    this.Model.View.SortDescriptions.Add(new System.ComponentModel.SortDescription(desc.PropertyPath.ParameterName, desc.Direction));
#else
                    this.Model.View.SortDescriptions.Add(new System.ComponentModel.SortDescription(desc.PropertyPath, desc.Direction));
#endif
                }
            }
        }

        void RIASource_SubmittedChanges(object sender, SubmittedChangesEventArgs e)
        {
            this.Model.View.CommitEdit();
            this.EndAnimation();
            this.Model.View.Refresh();
        }

        void RIASource_SubmittingChanges(object sender, SubmittingChangesEventArgs e)
        {
            this.BeginAnimation();
            this.Model.View.CommitEdit();
        }

        void RIASource_LoadedData(object sender, LoadedDataEventArgs e)
        {
            this.EndAnimation();
        }

        void RIASource_LoadingData(object sender, LoadingDataEventArgs e)
        {
            this.BeginAnimation();
        }

        internal void BeginAnimation()
        {
            foreach (var col in this.VisibleColumns)
            {
                if (col.FilterPane != null)
                {
                    col.FilterPane.LostFocus += new RoutedEventHandler(FilterPane_LostFocus);
                    col.FilterPane.IsEnabled = false;
                }
            }

            double headerHeight = 0;
            for (int i = 0; i < this.Model.HeaderRows; i++)
            {
                headerHeight += this.Model.RowHeights[0];
            }

            double groupDropArea = headerHeight;
            if (this.ShowGroupDropArea)
            {
                groupDropArea += this.GroupDropAreaGrid.ActualHeight;
            }

            if (LoadingIndicator != null)
            {
                var size = this.LoadingIndicator.RenderSize;
                this.LoadingIndicator.Height = this.Model.Views.First().ActualHeight >= 19 ? this.Model.Views.First().ActualHeight - 19 : 19;
                this.LoadingIndicator.Width = this.Model.Views.First().ActualWidth;
                this.LoadingIndicator.Margin = new Thickness(this.Margin.Left, this.Margin.Top + groupDropArea, 0, 0);
                this.LoadingIndicator.Visibility = Visibility.Visible;
                this.LoadingTextBlock.Text = this.LoadingText;
                this.Rotate.Begin();
            }
        }

        void FilterPane_LostFocus(object sender, RoutedEventArgs e)
        {
            this.filterText = e.OriginalSource as TextBox;
        }

        private TextBox filterText
        {
            get;
            set;
        }

        private static ResourceDictionary GetDictionary()
        {
            var stream = Application.GetResourceStream(new Uri("/Syncfusion.Grid.Ria.Silverlight;component/DomainGridDataControl/Control/Themes/generic.xaml", UriKind.Relative)).Stream;
            var streamreader = new StreamReader(stream);
            var rd = XamlReader.Load(streamreader.ReadToEnd()) as ResourceDictionary;
            streamreader.Close();
            return rd;
        }

        private static DataTemplate DataTemplate
        {
            get
            {
                var rd = GetDictionary();
                return rd["PART_DataTemplate"] as DataTemplate;
            }
        }

        public override void OnApplyTemplate()
        {
            if (this.RIASource != null)
            {
                this.LoadingIndicator = DataTemplate.LoadContent() as Grid;
                this.Rotate = this.LoadingIndicator.FindName("PART_Rotate") as Storyboard;
                this.LoadingTextBlock = this.LoadingIndicator.FindName("PART_LoadingText") as TextBlock;
                var mainGrid = this.GetTemplateChild("PART_MainGrid") as Grid;
                mainGrid.Children.Add(this.LoadingIndicator);
            }

            base.OnApplyTemplate();
            if (this.isRiaSourcechanged)
            {
                this.model.RIASource = RIASource;
                this.ItemsSource = RIASource.Data;
            }
        }


        internal void EndAnimation()
        {
            foreach (var col in this.VisibleColumns)
            {
                if (col.FilterPane != null)
                {
                    col.FilterPane.LostFocus -= new RoutedEventHandler(FilterPane_LostFocus);
                    col.FilterPane.IsEnabled = true;
                }
            }

            if (this.LoadingIndicator != null)
            {
                this.LoadingIndicator.Visibility = Visibility.Collapsed;
                this.Rotate.Stop();
            }

            if (this.filterText != null)
            {
                this.filterText.Focus();
            }
        }

        protected override void EnsureProperties()
        {
            base.EnsureProperties();

            if (this.RIASource != null)
            {
                this.SetRIAProperties();
            }
        }
    }
}
