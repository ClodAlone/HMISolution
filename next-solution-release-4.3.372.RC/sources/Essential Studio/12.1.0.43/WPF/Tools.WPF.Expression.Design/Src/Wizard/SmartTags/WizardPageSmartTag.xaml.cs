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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Design;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Tools.WPF.Expression.Design
{
    /// <summary>
    /// Interaction logic for WizardControlSmartTag.xaml
    /// </summary>
    public partial class WizardPageSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardPageSmartTag"/> class.
        /// </summary>
        public WizardPageSmartTag()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            base.BindCheckBox(this.helpVisibleCB, "HelpVisible");
            base.BindCheckBox(this.cancelVisibleCB, "CancelVisible");
            base.BindCheckBox(this.backVisibleCB, "BackVisible");
            base.BindCheckBox(this.nextVisibleCB, "NextVisible");
            base.BindCheckBox(this.finishVisibleCB, "FinishVisible");

            base.BindCheckBox(this.cancelEnabledCB, "CancelEnabled");
            base.BindCheckBox(this.backEnabledCB, "BackEnabled");
            base.BindCheckBox(this.nextEnabledCB, "NextEnabled");
            base.BindCheckBox(this.finishEnabledCB, "FinishEnabled");

            base.BindTextBox(this.pageTitle, "Title");
            base.BindTextBox(this.pageDesc, "Description");
            base.BindSelectorWithEnum(this.pageTypeCombo, "PageType", typeof(WizardPageType));
        }
        /// <summary>
        /// Adds the wizard page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddWizardPage(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            
            WizardControlSmartTag.AddPageToWizard(hyperlink.Tag.ToString(), this.ModelItem.Parent, this.Context, this.GetThisPageIndex() + 1);
        }

        /// <summary>
        /// Gets the index of the this page.
        /// </summary>
        /// <returns></returns>
        private int GetThisPageIndex()
        {
            return this.ModelItem.Parent.Properties["Items"].Collection.IndexOf(this.ModelItem);
        }

        /// <summary>
        /// Removes the page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemovePage(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected WizardPage?", "Verify page deletion", MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.OK)
                this.ModelItem.Parent.Properties["Items"].Collection.Remove(this.ModelItem);
        }

        /// <summary>
        /// Browses to.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BrowseTo(object sender, RoutedEventArgs e)
        {
             #if SyncfusionFramework3_5
            Hyperlink link = sender as Hyperlink;
            WizardControlSmartTag.BrowseTo(link.Tag.ToString(), this.ModelItem.Parent.View as WizardControl);
    #endif
        }

    }
}
