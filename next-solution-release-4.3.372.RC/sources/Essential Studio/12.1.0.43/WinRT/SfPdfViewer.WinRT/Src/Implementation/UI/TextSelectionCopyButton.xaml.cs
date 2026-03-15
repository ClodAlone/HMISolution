#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Syncfusion.Windows.PdfViewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class TextSelectionCopyButton : UserControl
    {
        internal Button SelectionCopyButton
        {
            get
            {
                return this.BtnCopyText;
            }
            set
            {
                this.BtnCopyText = value;
            }
        }

        public TextSelectionCopyButton()
        {
            this.InitializeComponent();
            this.BtnCopyText.PointerEntered += BtnCopyText_PointerEntered;
            this.BtnCopyText.PointerExited += BtnCopyText_PointerExited;
        }

        void BtnCopyText_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.Fill.Fill = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 00, 00, 00));
        }

        void BtnCopyText_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.Fill.Fill = new SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 81, 81, 81));
        }
    }
}
