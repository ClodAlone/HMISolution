//-------------------------------------------------------------------------------------------------
// <copyright file="QueryParameters.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;
using System.Windows.Input;
using System.ComponentModel;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Essential WPF RibbonWindow1.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class ParameterQuery
        : ChromelessWindow
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the list text.
        /// </summary>
        /// <value>The list text.</value>
        public List<string> ListText
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterQuery"/> class.
        /// </summary>
        public ParameterQuery()
        {
            InitializeComponent();
            this.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(ParameterQuery_PreviewKeyUp);
            this.ListText = new List<string>();
            this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleQueryParameters");
        }
        #endregion

        #region Event Handler Methods

        void ParameterQuery_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }

        } 
        #endregion        

        #region Custom Event Handler Methods

        private void ParamQueryOk_Click(object sender, RoutedEventArgs e)
        {
            this.CollectTextBoxes();            
            this.DialogResult = true;
            this.Close();
        }           
       
        private void ParamQueryCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }   

        #endregion

        #region Helper Methods

        /// <summary>
        /// To Iterate the TextBox Values
        /// </summary>
        private void CollectTextBoxes()
        {                  
            IEnumerable<StackPanel> collectionStackPanel = Mainpanel.Children.OfType<StackPanel>();
            foreach (StackPanel spanel in collectionStackPanel)
            {
                IEnumerable<TextBox> collectionTextBox = spanel.Children.OfType<TextBox>();
                if (collectionTextBox != null && collectionTextBox.Count() > 0)
                {
                    foreach (TextBox tbox in collectionTextBox)
                    {
                        this.ListText.Add(tbox.Text.ToString());
                    }
                }
                else
                {
                    IEnumerable<ComboBox> collectionCombo = spanel.Children.OfType<ComboBox>();
                    foreach (ComboBox tbox in collectionCombo)
                    {
                        this.ListText.Add(tbox.Text.ToString());
                    }
                }
            }
        }
       
        #endregion
    }

}
