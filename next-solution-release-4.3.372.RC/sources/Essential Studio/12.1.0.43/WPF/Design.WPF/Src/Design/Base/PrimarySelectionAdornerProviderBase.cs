// <copyright file="PrimarySelectionAdornerProviderBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;

namespace Syncfusion.Windows.Design
{
    /// <summary>
    /// PrimarySelectionAdornerProviderBase class help us to provide the adorner provider for SmartTag designer support to the controls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class PrimarySelectionAdornerProviderBase : PrimarySelectionAdornerProvider
    {
        #region Properties
        /// <summary>
        /// Gets or sets the smart tag.
        /// </summary>
        /// <value>The smart tag.</value>
        public SmartTagBase SmartTag
        {
            get;
            set;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Creates the smart tag.
        /// </summary>
        /// <param name="item">The ModelItem.</param>
        /// <returns>It returns the SmartTagBase</returns>
        protected abstract SmartTagBase CreateSmartTag(ModelItem item);

        /// <summary>
        /// Handles the PropertyChanged event of the Model control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
        #endregion

        #region Override methods

        /// <summary>
        /// Called when adorners are requested for the first time by the designer.
        /// </summary>
        /// <param name="item">A <see cref="T:Microsoft.Windows.Design.Model.ModelItem"/> representing the adorned element.</param>
        /// <param name="view">An instance of the adorned element.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view"/> is null.
        /// </exception>

#if SyncfusionFramework4_0 
        protected override void Activate(ModelItem item)
        {
            IsPopUpActive = false;

            if (this.SmartTag == null)
            {
                // Some common properties required for the SmartTag.
                this.SmartTag = CreateSmartTag(item);
                this.SmartTag.ModelItem = item;
                this.SmartTag.Context = item.Context;
                //  this.SmartTag.vieView = view;
            }

            AdornerPanel panel = new AdornerPanel();
            panel.IsContentFocusable = true;
            panel.Children.Add(this.SmartTag);
            this.Adorners.Add(panel);

            this.SmartTag.ModelItem.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

            AdornerPanel.SetHorizontalStretch(SmartTag, AdornerStretch.None);
            AdornerPanel.SetVerticalStretch(SmartTag, AdornerStretch.None);

            AdornerPlacementCollection placement = new AdornerPlacementCollection();

            placement.SizeRelativeToContentWidth(1.0, 0);
            placement.SizeRelativeToContentWidth(1.0, 0);

            placement.SizeRelativeToAdornerDesiredHeight(1.0, 0);
            placement.SizeRelativeToAdornerDesiredWidth(1.0, 0);

            // placement.PositionRelativeToAdornerHeight(0.0, 0.0, view);
            // placement.PositionRelativeToAdornerWidth(0.0, 0.0, view);

            AdornerPanel.SetPlacements(this.SmartTag, placement);

            base.Activate(item);
        }
#elif SyncfusionFramework4_5
        protected override void Activate(ModelItem item)
        {
            IsPopUpActive = false;

            if (this.SmartTag == null)
            {
                // Some common properties required for the SmartTag.
                this.SmartTag = CreateSmartTag(item);
                this.SmartTag.ModelItem = item;
                this.SmartTag.Context = item.Context;
                //  this.SmartTag.vieView = view;
            }

            AdornerPanel panel = new AdornerPanel();
            panel.IsContentFocusable = true;
            panel.Children.Add(this.SmartTag);
            this.Adorners.Add(panel);

            this.SmartTag.ModelItem.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

            AdornerPanel.SetHorizontalStretch(SmartTag, AdornerStretch.None);
            AdornerPanel.SetVerticalStretch(SmartTag, AdornerStretch.None);

            AdornerPlacementCollection placement = new AdornerPlacementCollection();

            placement.SizeRelativeToContentWidth(1.0, 0);
            placement.SizeRelativeToContentWidth(1.0, 0);

            placement.SizeRelativeToAdornerDesiredHeight(1.0, 0);
            placement.SizeRelativeToAdornerDesiredWidth(1.0, 0);

            // placement.PositionRelativeToAdornerHeight(0.0, 0.0, view);
            // placement.PositionRelativeToAdornerWidth(0.0, 0.0, view);

            AdornerPanel.SetPlacements(this.SmartTag, placement);

            base.Activate(item);
        }

#else
        protected override void Activate(ModelItem item, DependencyObject view)
        {
            IsPopUpActive = false;

            if (this.SmartTag == null)
            {
                // Some common properties required for the SmartTag.
                this.SmartTag = CreateSmartTag(item);
                this.SmartTag.ModelItem = item;
                this.SmartTag.Context = base.Context;
                this.SmartTag.View = view;
            }

            AdornerPanel panel = new AdornerPanel();
            panel.IsContentFocusable = true;
            panel.Children.Add(this.SmartTag);
            this.Adorners.Add(panel);

            this.SmartTag.ModelItem.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

            AdornerPanel.SetHorizontalStretch(SmartTag, AdornerStretch.None);
            AdornerPanel.SetVerticalStretch(SmartTag, AdornerStretch.None);

            AdornerPlacementCollection placement = new AdornerPlacementCollection();

            placement.SizeRelativeToContentWidth(1.0, 0);
            placement.SizeRelativeToContentWidth(1.0, 0);

            placement.SizeRelativeToAdornerDesiredHeight(1.0, 0);
            placement.SizeRelativeToAdornerDesiredWidth(1.0, 0);

            placement.PositionRelativeToAdornerHeight(0.0, 0.0, view);
            placement.PositionRelativeToAdornerWidth(0.0, 0.0, view);

            AdornerPanel.SetPlacements(this.SmartTag, placement);

            base.Activate(item, view);
        }
#endif


        /// <summary>
        /// Called when an adorner provider is about to be discarded by the designer.
        /// </summary>
        protected override void Deactivate()
        {
            this.SmartTag.ModelItem.PropertyChanged -= new PropertyChangedEventHandler(Model_PropertyChanged);
            base.Deactivate();
            this.SmartTag = null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pop up active.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pop up active; otherwise, <c>false</c>.
        /// </value>
        public static bool IsPopUpActive { get; set; }

        #endregion
    }
}
