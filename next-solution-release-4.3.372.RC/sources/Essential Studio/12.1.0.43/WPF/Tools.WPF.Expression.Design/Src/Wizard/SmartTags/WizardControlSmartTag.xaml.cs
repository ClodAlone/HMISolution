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
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design;

namespace Syncfusion.Tools.WPF.Expression.Design
{
    /// <summary>
    /// Interaction logic for WizardControlSmartTag.xaml
    /// </summary>
    public partial class WizardControlSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardControlSmartTag"/> class.
        /// </summary>
        public WizardControlSmartTag()
        {
            InitializeComponent();
            //SystemColors.
        }

        /// <summary>
        /// Gets the wizard control.
        /// </summary>
        /// <value>The wizard control.</value>
        public WizardControl WizardControl
        {
            get { return this.View as WizardControl; }
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

            base.BindCheckBox(this.cancelBtnCancelsWin, "CancelButtonCancelsWindow");
            base.BindCheckBox(this.finishBtnClosesWin, "FinishButtonClosesWindow");
            base.BindCheckBox(this.nextAndFinishIsDefault, "NextAndFinishAreDefaultButtons");

            ModelItemCollection pagesColl = this.ModelItem.Properties["Items"].Collection;
            this.pageList.ItemsSource = pagesColl;
            this.pageList.SelectionChanged += new SelectionChangedEventHandler(pageList_SelectionChanged);
        }


        /// <summary>
        /// Adds the wizard page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddWizardPage(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            AddPageToWizard(hyperlink.Tag.ToString(), this.ModelItem, this.Context, -1);
        }

        /// <summary>
        /// Adds the page to wizard.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="wizardModel">The wizard model.</param>
        /// <param name="context">The context.</param>
        /// <param name="index">The index.</param>
        internal static void AddPageToWizard(string type, ModelItem wizardModel, EditingContext context, int index)
        {
            ModelItem item = ModelFactory.CreateItem(context, typeof(WizardPage), new object[0]);

            switch (type)
            {
                case "Exterior":
                    item.Properties["PageType"].SetValue(WizardPageType.Exterior);
                    break;
                case "Blank":
                    item.Properties["PageType"].SetValue(WizardPageType.Blank);
                    break;
                case "Interior":
                default:
                    item.Properties["PageType"].SetValue(WizardPageType.Interior);
                    break;
            }
            if(index == -1)
                wizardModel.Properties["Items"].Collection.Add(item);
            else
                wizardModel.Properties["Items"].Collection.Insert(index, item);

            //TODO: Have to activate this page.
        }

        /// <summary>
        /// Browses to.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BrowseTo(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            WizardControlSmartTag.BrowseTo(link.Tag.ToString(), this.WizardControl);
        }

        /// <summary>
        /// Browses to.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="wizard">The wizard.</param>
        internal static void BrowseTo(string destination, WizardControl wizard)
        {
            switch (destination)
            {
                case "Next":
                    wizard.MoveNext();
                    break;
                case "Back":
                    wizard.MovePrevious();
                    break;
                case "Last":
                    wizard.MoveLast();
                    break;
                case "First":
                default:
                    wizard.MoveFirst();
                    break;
            }
            //TODO: Have to activate this page.
        }

        /// <summary>
        /// Handles the Click event of the pageupButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void pageupButton_Click(object sender, RoutedEventArgs e)
        {
            int curPosition = this.pageList.SelectedIndex;
            ModelItemCollection coll = this.ModelItem.Properties["Items"].Collection;
            
            if (curPosition != 0)
                coll.Move(curPosition, curPosition - 1);
        }

        /// <summary>
        /// Handles the Click event of the pagedownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void pagedownButton_Click(object sender, RoutedEventArgs e)
        {
            int curPosition = this.pageList.SelectedIndex;
            ModelItemCollection coll = this.ModelItem.Properties["Items"].Collection;
            if(curPosition < coll.Count - 1)
                coll.Move(curPosition, curPosition + 1);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the pageList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void pageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
               #if SyncfusionFramework3_5
            if (this.pageList.SelectedItem != null)
            {
                ModelItem pageModel = this.pageList.SelectedItem as ModelItem;
                this.WizardControl.SelectedWizardPage = pageModel.View as WizardPage;
            }
#endif
        }

    }
    /// <summary>
    /// Represents IValueConverter
    /// </summary>
    class EmptyNameConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the empty name.
        /// </summary>
        /// <value>The empty name.</value>
        public string EmptyName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyNameConverter"/> class.
        /// </summary>
        public EmptyNameConverter() { this.EmptyName = "<unnamed>"; }
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.ToString().Length == 0)
                return this.EmptyName;
            else
                return value;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
