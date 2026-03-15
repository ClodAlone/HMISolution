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
using System.Windows;
using Syncfusion.Windows.Shared;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Grid
{

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class AdvanceFilteringWindow:Window
    {
        #region Constructor

        /// <summary>
        /// Initializes the <see cref="AdvanceFilteringWindow"/> class.
        /// </summary>
        static AdvanceFilteringWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvanceFilteringWindow), new FrameworkPropertyMetadata(typeof(AdvanceFilteringWindow)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvanceFilteringWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="visualStyle">The visual style.</param>
        public AdvanceFilteringWindow(AdvanceFilteringViewModel viewModel, string visualStyle)
        {
            SetVisualStyle(visualStyle);
            this.DataContext = viewModel;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        #endregion

        #region  Controls      

        /// <summary>
        /// Gets or sets the part_ ok button.
        /// </summary>
        /// <value>The part_ ok button.</value>
        Button Part_OkButton
        {
            get;
            set;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Height = 195;
            this.Width = 765;
            this.Part_OkButton = this.GetTemplateChild("Part_OkButton") as Button;
            WireEvents();
        }


        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        #endregion

        #region Events

        /// <summary>
        /// Called when [ok button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Called when [advance filtering window unloaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnAdvanceFilteringWindowUnloaded(object sender, RoutedEventArgs e)
        {
            UnWireEvents();
        }
        #endregion

        #region PrivateMethods

        /// <summary>
        /// Uns the wire events.
        /// </summary>
        void UnWireEvents()
        {
            if (this.Part_OkButton != null)
            {
                this.Part_OkButton.Click -= new RoutedEventHandler(OnOkButtonClick);
            }
            this.Unloaded -= new RoutedEventHandler(OnAdvanceFilteringWindowUnloaded);
        }

        /// <summary>
        /// Wires the events.
        /// </summary>
        void WireEvents()
        {
            if (this.Part_OkButton != null)
            {
                this.Part_OkButton.Click += new RoutedEventHandler(OnOkButtonClick);
            }
            this.Unloaded += new RoutedEventHandler(OnAdvanceFilteringWindowUnloaded);
        }       

        /// <summary>
        /// Sets the visual style.
        /// </summary>
        /// <param name="visualStyle">The visual style.</param>
        void SetVisualStyle(string visualStyle)
        {
            string sourcePath = "";
            if (visualStyle != null && visualStyle!="Default" && visualStyle != "Office2003" && visualStyle != "Custom" && visualStyle != "DefaultOffice2007Blue" && visualStyle != "DefaultOffice2007Black" && visualStyle != "DefaultOffice2007Silver")
            {
                sourcePath = @"/Syncfusion.Grid.Wpf;component/GridDataControl/ExcelLikeFiltering/Themes/" + visualStyle.ToString() + ".xaml";
            }
            else
            {
                sourcePath = @"/Syncfusion.Grid.Wpf;component/GridDataControl/ExcelLikeFiltering/Themes/Generic.xaml";
            }

            ResourceDictionary rd1 = new ResourceDictionary();
            rd1.Source = new Uri(sourcePath, UriKind.RelativeOrAbsolute);
            this.Resources.MergedDictionaries.Add(rd1);

        }

        #endregion     
      
    }
}
