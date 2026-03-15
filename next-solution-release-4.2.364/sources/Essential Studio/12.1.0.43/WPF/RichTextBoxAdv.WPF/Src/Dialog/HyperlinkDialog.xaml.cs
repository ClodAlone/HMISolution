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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HyperlinkDialog : Window
    {
        private const string DefaultHyperlinkText = "<<Selection in Document>>";

        private RichTextBoxAdv RichTB
        {
            get;
            set;
        }

        public HyperlinkDialog()
        {
            InitializeComponent();
        }

        public HyperlinkDialog(RichTextBoxAdv _richtext)
            : this()
        {
            RichTB = _richtext;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            Execute();
        }

        protected override void OnActivated(EventArgs e)
        {
            txthyperText.Text = string.Empty;
            if (GetHyperlinkText() == DefaultHyperlinkText)
            {
                txthyperText.Text = DefaultHyperlinkText;
                txthyperText.IsEnabled = false;
            }
            else
            {
                txthyperText.Text = GetHyperlinkText();
                txthyperText.IsEnabled = true;
            }

        }

        private void Execute()
        {
            HyperlinkAdv hyperlink = new HyperlinkAdv();
            hyperlink.Text = GetSelectedText();
            hyperlink.NavigationUrl = txtAddress.Text;
            if (hyperlink.Text != DefaultHyperlinkText)
            {
                RichTB.InsertInlineInParagraph(hyperlink);
            }
            hyperlinkDialog.Close();
            this.RichTB.Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                if (e.Key == Key.Enter)
                {
                    if (!string.IsNullOrEmpty(txtAddress.Text))
                    {
                        Execute();
                    }
                }
                this.Close();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowIcon.SetNull(this);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
        }

        internal string GetSelectedText()
        {
            string str = string.Empty;
            foreach (Inline inline in RichTB.Selection.GetInlinesToFormat())
            {
                str += inline.InternalText;
            }
            if (string.IsNullOrEmpty(str) && 
                !RichTB.Viewer.IsSelected)
            {
                str = txthyperText.Text;
            }
            return str;
        }

        private HyperlinkTargetType String2Target(object content)
        {
            if (content.ToString().ToLower() == "self")
            {
                return HyperlinkTargetType.Self;
            }
            else if (content.ToString().ToLower() == "new window")
            {
                return HyperlinkTargetType.Blank;
            }
            return HyperlinkTargetType.Self;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hyperlinkDialog.Close();
            this.RichTB.Focus();
        }

        internal string GetHyperlinkText()
        {
            if (RichTB != null)
            {
                string tempstr = string.Empty;
                if (RichTB.Selection.Blocks.Count==1 && RichTB.Viewer.IsSelected)
                {
                    if (RichTB.Selection.Blocks[0].IsParagraph)
                    {
                        foreach (Inline inline in RichTB.Selection.Blocks[0].Inlines)
                        {
                            if (inline.IsImageContainer || inline.IsUIContainer)
                            {
                                return DefaultHyperlinkText;
                            }
                            else
                                tempstr += inline.InternalText;
                        }
                    }
                    else
                        return DefaultHyperlinkText;
                }
                else 
                {
                    return DefaultHyperlinkText;
                }

                return tempstr;
            }
            return null;
        }
    }
}
