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

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for WizardControlSmartTag.xaml
    /// </summary>
    public partial class WizardControlSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the WizardControlSmartTag class.
        /// </summary>
        public WizardControlSmartTag()
        {
            InitializeComponent();
            //SystemColors.
        }
        /// <summary>
        /// WizardControl declaration.
        /// </summary>
        public WizardControl WizardControl
        {
            get { return this.View as WizardControl; }
        }
        /// <summary>
        /// This method is called when the WizardControlSmartTag Item template is initialized.
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


        private void AddWizardPage(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            if (hyperlink != null && hyperlink.Tag != null)
                AddPageToWizard(hyperlink.Tag.ToString(), this.ModelItem, this.Context, -1);
        }

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

        private void BrowseTo(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            if (link != null)
            {
                if (link.Tag != null && this.WizardControl != null)
                    WizardControlSmartTag.BrowseTo(link.Tag.ToString(), this.WizardControl);
            }
        }

        internal static void BrowseTo(string destination, WizardControl wizard)
        {
            if (wizard != null)
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
            }
            //TODO: Have to activate this page.
        }

        private void pageupButton_Click(object sender, RoutedEventArgs e)
        {
            int curPosition = this.pageList.SelectedIndex;
            ModelItemCollection coll = this.ModelItem.Properties["Items"].Collection;
            
            if (curPosition != 0)
                coll.Move(curPosition, curPosition - 1);
        }

        private void pagedownButton_Click(object sender, RoutedEventArgs e)
        {
            int curPosition = this.pageList.SelectedIndex;
            ModelItemCollection coll = this.ModelItem.Properties["Items"].Collection;
            if(curPosition < coll.Count - 1)
                coll.Move(curPosition, curPosition + 1);
        }
       
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
    class EmptyNameConverter : IValueConverter
    {
        public string EmptyName { get; set; }

        public EmptyNameConverter() { this.EmptyName = "<unnamed>"; }
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.ToString().Length == 0)
                return this.EmptyName;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
