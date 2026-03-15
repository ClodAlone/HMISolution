#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System.Windows;
    using System.Windows.Controls;
using System;
    using System.Collections;
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [TemplatePart(Name = GridListControl.TemplateGrid, Type = typeof(GridListControl))]
    public class GridListControl : ContentControl
    {
        public const string TemplateGrid = "PART_GridControl";

        static GridListControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridListControl), new FrameworkPropertyMetadata(typeof(GridListControl)));
            /*CommandManager.RegisterClassCommandBinding(typeof(GridListControl), new CommandBinding(EndEditCommand, new ExecutedRoutedEventHandler(OnExecutedEndEdit), new CanExecuteRoutedEventHandler(OnCanExecuteEndEdit)));*/
        }

        public GridListControl()
        {
        }

        private GridListModel model;
        public GridListModel Model
        {
            get
            {
                if (this.model == null)
                {
                    this.model = this.OnCreateModel();
                }

                return this.model;
            }
        }

        private GridListModel OnCreateModel()
        {
            return new GridListModel();
        }

        public GridListControlImpl InternalGrid
        {
            get;
            private set;
        }

        /// <summary>
        /// DependencyProperty for <see cref = "GridListControl.ItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(GridListControl),
            new FrameworkPropertyMetadata(OnItemsSourceChanged));

        private bool isItemsSourceLoadedBeforeGridLoaded = false;

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridListControl grid = d as GridListControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.Model.ItemsSource = (IEnumerable)args.NewValue;
            }
            else
            {
                grid.isItemsSourceLoadedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(GridListControl.ItemsSourceProperty);
            }

            set
            {
                this.SetValue(GridListControl.ItemsSourceProperty, value);
            }
        }

        private bool isGridLoaded = false;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.InternalGrid = this.GetTemplateChild(GridListControl.TemplateGrid) as GridListControlImpl;
            if (this.model == null)
            {
                this.model = this.OnCreateModel();
            }
            this.InternalGrid.Model = this.model;
            this.isGridLoaded = true;
            if (this.GridLoaded != null)
            {
                this.GridLoaded(this, EventArgs.Empty);
            }

            if (this.isItemsSourceLoadedBeforeGridLoaded)
            {
                this.InternalGrid.Model.ItemsSource = this.ItemsSource;
            }
        }

        public event EventHandler GridLoaded;
    }
}