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
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Reports.Designer.Wizard
{
    /// <summary>
    /// Interaction logic for ReportOpenDialog.xaml
    /// </summary>
    public partial class ReportOpenDialog : ChromelessWindow
    {
        //File Format Filtering
        internal static readonly DependencyProperty FileTypeproperty;

        internal DialogFileType FileType
        {
            get
            {
                return (DialogFileType)this.GetValue(FileTypeproperty);
            }
            set
            {
                this.SetValue(FileTypeproperty, value);
            }
        }

        //File Path
        internal static readonly DependencyProperty FilePathproperty;

        internal string FilePath
        {
            get
            {
                return (string)this.GetValue(FilePathproperty);
            }
            set
            {
                this.SetValue(FilePathproperty, value);
            }
        }

        //Server url
        internal static readonly DependencyProperty ReportServerURLProperty;

        internal string ReportServerURL
        {
            get
            {
                return (string)this.GetValue(ReportServerURLProperty);
            }
            private set
            {
                this.SetValue(ReportServerURLProperty, value);
            }
        }

        //Crentical
        internal System.Net.ICredentials ReportServerCredential { get; private set; }

        //Dialogmodeproperty 
        internal static readonly DependencyProperty DialogModeProperty;

        internal DialogMode DialogMode
        {
            get
            {
                return (DialogMode)this.GetValue(DialogModeProperty);
            }
            set
            {
                this.SetValue(DialogModeProperty, value);
            }
        }

        public string LoginUser { get; set; }

        public string LoginPassword { get; set; }
        
        static ReportOpenDialog()
        {
            DialogModeProperty = DependencyProperty.Register("DialogMode", typeof(DialogMode), typeof(ReportOpenDialog), new System.Windows.PropertyMetadata(DialogMode.Local));
            FileTypeproperty = DependencyProperty.Register("FileType", typeof(DialogFileType), typeof(ReportOpenDialog), new PropertyMetadata(DialogFileType.RDL));
            FilePathproperty = DependencyProperty.Register("FilePath", typeof(string), typeof(ReportOpenDialog), new PropertyMetadata(string.Empty));
            ReportServerURLProperty = DependencyProperty.Register("ReportServerURL", typeof(string), typeof(ReportOpenDialog), new PropertyMetadata(string.Empty));
        }

        public ReportOpenDialog()
        {
            InitializeComponent();
            this.Title = Syncfusion.Windows.ReportDesigner.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "titlteOpenReport");
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.OpenSaveDialog.DialogType = DialogType.Open;
            this.SetDetails();
            this.OpenSaveDialog.UpdateReportDialog();
        }

        private void ChromelessWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.GetDetails();
        }
        private void GetDetails()
        {
            this.FilePath = OpenSaveDialog.FilePath;
            this.ReportServerURL = OpenSaveDialog.ReportServerURL;
            this.ReportServerCredential = OpenSaveDialog.ReportServerCredential;
            this.LoginUser = OpenSaveDialog.LoginUsername;
            this.LoginPassword = OpenSaveDialog.LoginPassword;
        }
        private void SetDetails()
        {
            this.OpenSaveDialog.FileType = this.FileType;
            this.OpenSaveDialog.DialogMode = this.DialogMode;
        }
    }

}
