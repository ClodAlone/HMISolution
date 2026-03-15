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
    /// Interaction logic for ReportSaveDialog.xaml
    /// </summary>
    public partial class ReportSaveDialog : ChromelessWindow
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

        static ReportSaveDialog()
        {
            DialogModeProperty = DependencyProperty.Register("DialogMode", typeof(DialogMode), typeof(ReportSaveDialog), new System.Windows.PropertyMetadata(DialogMode.Local));
            FileTypeproperty = DependencyProperty.Register("FileType", typeof(DialogFileType), typeof(ReportSaveDialog), new PropertyMetadata(DialogFileType.RDL));
            FilePathproperty = DependencyProperty.Register("FilePath", typeof(string), typeof(ReportSaveDialog), new PropertyMetadata(string.Empty));
            ReportServerURLProperty = DependencyProperty.Register("ReportServerURL", typeof(string), typeof(ReportSaveDialog), new PropertyMetadata(string.Empty));
        }

        public ReportSaveDialog()
        {
            InitializeComponent();
            this.Title = Syncfusion.Windows.ReportDesigner.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "titleSaveReport");
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.OpenSaveDialog.DialogType = DialogType.Save;
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
        }

        private void SetDetails()
        {
            this.OpenSaveDialog.FileType = this.FileType;
            this.OpenSaveDialog.DialogMode = this.DialogMode;
        }
    }
}
