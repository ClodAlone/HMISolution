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
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

#if !SILVERLIGHT
using Syncfusion.Olap.Engine;
namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.OlapSilverlight.Engine;
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class OlapGridHyperlinkCell : ContentControl
    {
        #region Initialize/Finalize

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="OlapGridHyperlinkCell"/> class.
        /// </summary>
        static OlapGridHyperlinkCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OlapGridHyperlinkCell), new FrameworkPropertyMetadata(typeof(OlapGridHyperlinkCell)));
        }
#else
        /// <summary>
        /// Initializes the <see cref="OlapGridHyperlinkCell"/> class.
        /// </summary>
        public OlapGridHyperlinkCell()
        {
            DefaultStyleKey = typeof(OlapGridHyperlinkCell);
        }
#endif
        #endregion              

        #region Private Members
        private const string m_SummaryText = "Total";
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the OlapGridBase to get OlapGrid instance
        /// </summary>        
        public OlapGridBase GridBase { get; set; }

        /// <summary>
        /// Gets or sets the associated PivotCellDescriptor for each Cell
        /// </summary>
        public PivotCellDescriptor CellDescriptor
        {
            get { return (PivotCellDescriptor)GetValue(PivotCellInfoProperty); }
            set { SetValue(PivotCellInfoProperty, value); }
        }

        /// <summary>
        /// Gets the Horizontal alignment of Textblock.
        /// </summary>
        /// <value>The Horizontal alignment of Textblock.</value>
        public HorizontalAlignment HAlignment
        {
            get
            {
                if (this.CellDescriptor.CellValue == m_SummaryText)
                    return System.Windows.HorizontalAlignment.Left;
                else
                    return System.Windows.HorizontalAlignment.Right;
            }
        }

        /// <summary>
        /// Gets or sets the ValueCell Textblock.
        /// </summary>
        public TextBlock ValueTextBlock { get; set; }

        #endregion

        #region Dependency Property Declaration

        public static readonly DependencyProperty PivotCellInfoProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapGridHyperlinkCell), new UIPropertyMetadata(null));
#else
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapGridHyperlinkCell), new PropertyMetadata(null));
#endif
        #endregion

        #region Overrides
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ValueTextBlock = GetTemplateChild("PART_CellValueTextBlock") as TextBlock;
            if (this.ValueTextBlock != null)
            {
#if !SILVERLIGHT
                this.ValueTextBlock.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(ValueTextBlock_PreviewMouseLeftButtonDown);
#else
                this.ValueTextBlock.MouseEnter += new MouseEventHandler(ValueTextBlock_MouseEnter);
                this.ValueTextBlock.MouseLeave += new MouseEventHandler(ValueTextBlock_MouseLeave);
                this.ValueTextBlock.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(ValueTextBlock_PreviewMouseLeftButtonDown);                
#endif
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when link cell clicked.
        /// </summary>
        public event LinkLabelClickEventHander LinkClicked;        

#if SILVERLIGHT
        /// <summary>
        /// Handles the MouseLeave event of the ValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void ValueTextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }

        /// <summary>
        /// Handles the MouseEnter event of the ValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void ValueTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", false);
        }
#endif
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the ValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void ValueTextBlock_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.LinkClicked != null)
            {
                //// Triggering when the hyperlink cell is clicked
                this.LinkClicked(this, new LinkLabelEventArgs(CellDescriptor));
            }
        }

        #endregion        
    }
}
