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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Reports.Viewer.Dialogs
{
    /// <summary>
    /// Interaction logic for ExceptionDetail.xaml
    /// </summary>
#if SILVERLIGHT
    internal partial class ExceptionDetail : WindowControl
#else
    internal partial class ExceptionDetail : ChromelessWindow
#endif
    {
        string errorMessage = String.Empty;

        public ExceptionDetail(string errorMessage)
        {
            InitializeComponent();

            this.errorMessage = errorMessage;

#if SILVERLIGHT
            foreach (string exception in this.errorMessage.Split('\n'))
            {
                Paragraph para = new Paragraph();
                Run run = new Run();
                run.Text = exception;
                para.Inlines.Add(run);
                this.textBlockStackTrace.Blocks.Add(para);
            }
#else
            this.textBlockStackTrace.AppendText(errorMessage);
#endif
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            Clipboard.SetText(errorMessage);
#else
            Clipboard.SetDataObject(errorMessage, true);
#endif
        }
    }
}
