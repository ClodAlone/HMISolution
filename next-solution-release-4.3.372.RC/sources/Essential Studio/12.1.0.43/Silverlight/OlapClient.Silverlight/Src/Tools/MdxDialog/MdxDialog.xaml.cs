#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;
using System.Windows;

namespace Syncfusion.Silverlight.Tools.Olap
{
    [DesignTimeVisible(false)]
    public partial class MdxDialog : WindowControl
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MdxDialog"/> class.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        public MdxDialog(string mdxQuery)
        {
            InitializeComponent();
            if (mdxQuery == null)
            {
                mdxQuery = string.Empty;
            }

            this.mdxQueryBox.Text = mdxQuery;
        } 

        #endregion       

        private void OK_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void Copy_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.mdxQueryBox.Text))
            {
                try
                {
                    Clipboard.SetText(this.mdxQueryBox.Text);
                }
                catch (System.Security.SecurityException se)
                {
                    WindowControl.ShowAlert(se.Message, Syncfusion.Silverlight.Client.Olap.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "Message_SecurityException"), DialogIcon.Error, DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                }
            }

        }

    }
}
