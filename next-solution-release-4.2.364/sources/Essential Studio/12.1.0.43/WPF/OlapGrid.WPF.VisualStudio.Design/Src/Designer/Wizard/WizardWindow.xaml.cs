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
using System.Windows.Threading;
using Microsoft.Windows.Design.Model;
using System.Windows.Media.Effects;

namespace Syncfusion.OlapGrid.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WizardWindow 
        : Window,
        IWizardNavigationButtonsStatus
    {
        #region Members

        private ModelItem selectedControl;

        private ConnectionPropertiesView connectionPropertiesViewInstance;

        private SummariesView summariesViewInstance;

        #endregion        

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardWindow"/> class.
        /// </summary>
        /// <param name="selectedControl">The selected control.</param>
        public WizardWindow(ModelItem selectedControl)
        {
            InitializeComponent();

            this.selectedControl = selectedControl;

            this.gridConentLayout.Height = 544;

            //// Initializing the connection properties window.
            this.connectionPropertiesViewInstance = new ConnectionPropertiesView(selectedControl, this);
            this.connectionPropertiesViewInstance.Height = 544;
            this.connectionPropertiesViewInstance.Visibility = Visibility.Visible;
            this.CurrentPage = Pages.ConnectionProperties;

            //// Initializing the summaries window.
            this.summariesViewInstance = new SummariesView(selectedControl, this);
            this.summariesViewInstance.Height = 544;
            this.summariesViewInstance.Visibility = Visibility.Collapsed;

            //// Adding the connection properties and summaries control to the layout grid.
            this.gridConentLayout.Children.Add(connectionPropertiesViewInstance);
            this.gridConentLayout.Children.Add(summariesViewInstance);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the wizard title.
        /// </summary>
        /// <value>The wizard title.</value>
        //public Pages CurrentPage { get; set; }

        public Pages CurrentPage
        {
            get { return (Pages)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(Pages), typeof(WizardWindow), new PropertyMetadata(Pages.ConnectionProperties, OnCurrentPagePropertyChanged));

        public static void OnCurrentPagePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WizardWindow wizardWindow = dependencyObject as WizardWindow;

            if (wizardWindow != null)
            {
                if ((Pages)e.NewValue == Pages.Summaries)
                {
                    //// Make the side bar glow for page summaires.
                    RemoveGlow(wizardWindow.textBlockConnectionProperties);
                    ApplyGlow(wizardWindow.textBlockSummaires);
                    wizardWindow.buttonTestConnection.Visibility = Visibility.Hidden;
                }
                else if((Pages)e.NewValue == Pages.ConnectionProperties)
                {
                    //// Glow the connection properties.
                    RemoveGlow(wizardWindow.textBlockSummaires);
                    ApplyGlow(wizardWindow.textBlockConnectionProperties);
                    wizardWindow.buttonTestConnection.Visibility = Visibility.Visible;
                }
            }
        }


        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the buttonNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this.Next();
        }

        /// <summary>
        /// Handles the Click event of the buttonFinish control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            this.Finish();
        }

        /// <summary>
        /// Handles the Click event of the buttonBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Back();
        }

        /// <summary>
        /// Handles the Click event of the buttonTestConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void buttonTestConnection_Click(object sender, RoutedEventArgs e)
        {
            this.TestConnection();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        public void Next()
        {            
            if (this.CurrentPage == Pages.ConnectionProperties)
            {
                if (this.connectionPropertiesViewInstance.TestConnection())
                {
                    this.connectionPropertiesViewInstance.Visibility = Visibility.Collapsed;
                    try
                    {                                                       
                        this.summariesViewInstance.InitializeControls(this.connectionPropertiesViewInstance.GetConnectionString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    this.summariesViewInstance.Visibility = Visibility.Visible;
                    this.CurrentPage = Pages.Summaries;
                    this.UpdateButtonStatus(true, false, true);
                }
                else
                {
                    if (MessageBox.Show("Connection could not be establised. Problem with connection string. \n Do you want to save the connection string ?", "Connection Faild", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        this.Finish();
                    }
                }
            }
            else if (this.CurrentPage == Pages.Summaries)
            {
                //// Next button won't be visible. No need to validate this.
                this.Finish();
            }
        }

        /// <summary>
        /// Backs this instance.
        /// </summary>
        public void Back()
        {            
            if(this.CurrentPage == Pages.Summaries)
            {
                //// Logic for navigating back from the summaries page.
                this.summariesViewInstance.Visibility = Visibility.Collapsed;
                this.connectionPropertiesViewInstance.Visibility = Visibility.Visible;
                this.CurrentPage = Pages.ConnectionProperties;
                this.UpdateButtonStatus(false, true, true);
            }
        }

        /// <summary>
        /// Finishes this instance. Commits the changes of the edit.
        /// </summary>
        public void Finish()
        {
            if (this.connectionPropertiesViewInstance != null)
            {
                this.connectionPropertiesViewInstance.CommitChanges();
                this.summariesViewInstance.CommitChanges();
                this.Close();
            }
        }

        public static void ApplyGlow(TextBlock textBlock)
        {
            textBlock.BitmapEffect = GlowEffect(true);
            textBlock.FontSize = 12;
            textBlock.FontWeight = FontWeights.Bold;
        }

        private static BitmapEffect GlowEffect(bool canGlow)
        {
            BitmapEffect bitmapEffect = new OuterGlowBitmapEffect()
            {
                GlowColor = (Color)new ColorConverter().ConvertFromInvariantString("White"),
                GlowSize = canGlow == true ? 2 : 0,
                Opacity = 0.35
            };

            return bitmapEffect;
        }

        public static void RemoveGlow(TextBlock textBlock)
        {
            textBlock.BitmapEffect = GlowEffect(false);
            textBlock.FontSize = 10;
            textBlock.FontWeight = FontWeights.Normal;
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        public void TestConnection()
        {
            if (this.connectionPropertiesViewInstance.TestConnection())
            {
                MessageBox.Show("Test connection succeeded.", "Test result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("This connection cannot be tested because the specifed database does not exist or is not visible to the specified user.", "Test result", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region IWizardNavigationButtonsStatus Members

        private bool _CanBackButtonEnabled;
        public bool CanBackButtonEnabled
        {
            get
            {
                return this._CanBackButtonEnabled;
            }
            set
            {
                this._CanBackButtonEnabled = value;
                this.buttonBack.IsEnabled = _CanBackButtonEnabled;
            }
        }

        private bool _CanFinishButtonEnabled;
        public bool CanFinishButtonEnabled
        {
            get
            {
                return this._CanFinishButtonEnabled;
            }
            set
            {
                this._CanFinishButtonEnabled = value;
                this.buttonFinish.IsEnabled = this._CanFinishButtonEnabled;
            }
        }

        private bool _CanNextButtonEnabled;
        public bool CanNextButtonEnabled
        {
            get
            {
                return this._CanNextButtonEnabled;
            }
            set
            {
                this._CanNextButtonEnabled = value;
                this.buttonNext.IsEnabled = this._CanNextButtonEnabled;
            }
        }

        public void UpdateButtonStatus(bool back, bool next, bool finish)
        {
            this.CanBackButtonEnabled = back;
            this.CanNextButtonEnabled = next;
            this.CanFinishButtonEnabled = finish;
        }

        #endregion
    }

    #region Wizard Pages

    /// <summary>
    /// Wizard pages.
    /// </summary>
    public enum Pages
    {
        /// <summary>
        /// Indicates the page is used to create or edit data source connections.
        /// </summary>
        ConnectionProperties,

        /// <summary>
        /// Indicates the page is used to slice and dice dimensions.
        /// </summary>
        SliceDice,

        /// <summary>
        /// Indicates the page is used to display the summary of edits.
        /// </summary>
        Summaries,

        /// <summary>
        /// Indicates the page is used for customizing general properties of the olap grid.
        /// </summary>
        UIGeneralProperties,

        /// <summary>
        /// Indicates the page is used for customizing advanced properties of the olap grid.
        /// </summary>
        UIAdvancedProperties
    }

    #endregion
}
