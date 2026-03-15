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
using Syncfusion.UI.Xaml.Controls;
using System.Windows.Data;
using Syncfusion.Windows.Controls.Theming;

namespace Syncfusion.UI.Xaml.Controls
{
    public partial class HyperlinkDialog : WindowControl
    {
        private const string DefaultHyperlinkText ="<<Selection in Document>>";

        private RichTextBoxAdv RichTB
        {
            get;
            set;
        }

        public HyperlinkDialog()
        {
            InitializeComponent();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key==Key.Enter || e.Key==Key.Escape)
            {
                if (e.Key == Key.Enter)
                {
                    if(!string.IsNullOrEmpty(txtAddress.Text))
                    {
                        Execute();
                    }
                }
                this.Close();
            }
        }

        public HyperlinkDialog(RichTextBoxAdv _richtext)
            :this()
        {
            RichTB = _richtext;
            SkinManager.SetVisualStyle(this, Windows.Controls.Theming.VisualStyle.Office2010Blue);
            this.Opened += new RoutedEventHandler(HyperlinkDialog_Opened);
        }


        void HyperlinkDialog_Opened(object sender, RoutedEventArgs e)
        {
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

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            Execute();
        }

        private void Execute()
        {
            HyperlinkAdv hyperlink = new HyperlinkAdv();
            hyperlink.Text = GetSelectedText();
            hyperlink.TargetType = String2Target((cmbTarget.SelectedItem as ComboBoxItem).Content);
            hyperlink.NavigationUrl = txtAddress.Text;
            RichTB.InsertInlineInParagraph(hyperlink);
            hyperlinkDialog.Close();
            this.RichTB.Focus();
        }

        internal string GetSelectedText()
        {
            string str = string.Empty;
            foreach (Inline inline in RichTB.Selection.GetInlinesToFormat())
            {
                str+=inline.InternalText;
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
                string tempstr=string.Empty;
                if (RichTB.Selection.Blocks.Count == 1)
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
                    return DefaultHyperlinkText;

                return tempstr;
            }
            return null;
        }
    }
}
