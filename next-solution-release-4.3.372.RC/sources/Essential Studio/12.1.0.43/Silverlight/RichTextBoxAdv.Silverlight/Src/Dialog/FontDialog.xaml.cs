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
using Syncfusion.Windows.Shared;
using System.Windows.Data;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Theming;

namespace Syncfusion.Windows.Tools.Controls
{
    public partial class FontDialog : WindowControl,IDataErrorInfo
    {        
        public RichTextBoxAdv RichTB 
        { get; set; }
        private bool IsFontFamilySelectionChanged;
        private bool IsFontSizeSelectionChanged;
        private bool IsFontStyleChanged;
        private bool IsTextColorChanged;

        public FontDialog()
        {
            InitializeComponent();
            //SkinManager.SetVisualStyle(this, Windows.Controls.Theming.VisualStyle.Office2010Blue);
        }
        
        public FontDialog(RichTextBoxAdv _richtext)
            : this()
        {
            RichTB = _richtext;
            UpdateControls(); 
            this.fontcolorpicker.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            this.fontcolorpicker.ColorChanged += new PropertyChangedCallback(fontcolorpicker_ColorChanged);
            lstfontFamily.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(lstfontFamily_SelectionChanged);
            lstfontsize.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(lstfontsize_SelectionChanged);
            lstfontstyle.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(lstfontstyle_SelectionChanged);
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            Execute();
            this.Close();
        }

        private void Execute()
        {
            ApplyFontFamily();
            ApplyFontSize();
            ApplyFontStyle();
            ApplyDoubleStrikeThrough();
            ApplySingleStrikeThrough();
            ApplySubScript();
            ApplySuperScript();
            ApplyFontColor();
            this.RichTB.Focus();
        }


        private bool IsSelectedInlinesEqual()
        {
            bool isEqual = false;
            List<Inline> inlines = RichTB.Selection.GetInlinesToFormat();
            if (inlines != null)
            {
                for (int i = 0; i < inlines.Count - 1; i++)
                {
                    if (i + 1 < inlines.Count)
                        isEqual = inlines[i].IsEqualInStyle(inlines[i + 1]);
                }
            }

            return isEqual;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                if (e.Key == Key.Enter)
                {
                    Execute();
                }
                this.Close();
            }
        }

        private HyperlinkAdv GetHyperlink(InlineStyle inlinestyle)
        {
            HyperlinkAdv hyperlink = new HyperlinkAdv();
            hyperlink.Foreground = inlinestyle.Foreground;
            hyperlink.StrikeThrough = inlinestyle.StrikeThrough;
            hyperlink.Underline = inlinestyle.IsUnderline;
            hyperlink.HighlightColor = inlinestyle.HighlightColor;
            hyperlink.FontStyle = inlinestyle.FontStyle;
            hyperlink.FontWeight = inlinestyle.FontWeight;
            hyperlink.FontSize = inlinestyle.FontSize;
            hyperlink.FontFamily = inlinestyle.FontFamily;
            hyperlink.Baseline = inlinestyle.Baseline;
            return hyperlink;
        }

        private void UpdateControls()
        {
            if (!RichTB.Viewer.IsSelected)
            {
                if (RichTB.CurrentInlineStyle!=null)
                {
                    if (RichTB.CurrentInlineStyle.InlineType == typeof(SpanAdv))
                    {
                        TriggerFontFamily(SpanAdv.CreateNewSpan(RichTB.CurrentInlineStyle));
                        TriggerFontSize(SpanAdv.CreateNewSpan(RichTB.CurrentInlineStyle));
                        TriggerFontStyle(SpanAdv.CreateNewSpan(RichTB.CurrentInlineStyle));
                    }
                    else if (RichTB.CurrentInlineStyle.InlineType == typeof(HyperlinkAdv))
                    {
                        TriggerFontFamily(GetHyperlink(RichTB.CurrentInlineStyle));
                        TriggerFontSize(GetHyperlink(RichTB.CurrentInlineStyle));
                        TriggerFontStyle(GetHyperlink(RichTB.CurrentInlineStyle));
                    }
                    if (RichTB.CurrentInlineStyle.IsSuperscript)
                        chksuperscript.IsChecked = true;
                    if (RichTB.CurrentInlineStyle.IsSubscript)
                        chksubscript.IsChecked = true;
                    if (RichTB.CurrentInlineStyle.StrikeThrough == StrikeThrough.SingleStrike)
                        chksingle.IsChecked = true;
                    if (RichTB.CurrentInlineStyle.StrikeThrough == StrikeThrough.DoubleStrike)
                        chkdouble.IsChecked = true;
                    if (RichTB.CurrentInlineStyle.FontWeight == FontWeights.Bold)
                        lstfontstyle.SelectedIndex = 1;
                    if (RichTB.CurrentInlineStyle.FontWeight == FontWeights.Bold && RichTB.CurrentInlineStyle.FontStyle == FontStyles.Italic)
                        lstfontstyle.SelectedIndex = 3;
                }
            }
            else if (RichTB.Viewer.IsSelected && (IsSelectedInlinesEqual() || RichTB.Selection.GetInlinesToFormat().Count==1))
            {
                TriggerFontFamily(RichTB.Selection.GetInlinesToFormat()[0]);
                TriggerFontSize(RichTB.Selection.GetInlinesToFormat()[0]);
                TriggerFontStyle(RichTB.Selection.GetInlinesToFormat()[0]);
                if (((SpanAdv)RichTB.Selection.GetInlinesToFormat()[0]).IsSuperscript)
                    chksuperscript.IsChecked = true;
                if (((SpanAdv)RichTB.Selection.GetInlinesToFormat()[0]).IsSubScript)
                    chksubscript.IsChecked = true;
                if (((SpanAdv)RichTB.Selection.GetInlinesToFormat()[0]).StrikeThrough == StrikeThrough.SingleStrike)
                    chksingle.IsChecked = true;
                if (((SpanAdv)RichTB.Selection.GetInlinesToFormat()[0]).StrikeThrough == StrikeThrough.DoubleStrike)
                    chkdouble.IsChecked = true;
                if (((SpanAdv)RichTB.Selection.GetInlinesToFormat()[0]).FontWeight == FontWeights.Bold)
                    lstfontstyle.SelectedIndex = 1;
                if (((SpanAdv)RichTB.Selection.GetInlinesToFormat()[0]).FontWeight == FontWeights.Bold && ((SpanAdv)RichTB.Selection.GetInlinesToFormat()[0]).FontStyle == FontStyles.Italic)
                    lstfontstyle.SelectedIndex = 3;
            }
        }

        private void TriggerFontFamily(Inline inline)
        {
            foreach (ListBoxItem item in lstfontFamily.Items)
            {
                if (inline is SpanAdv)
                {
                    string name = item.Content as string;
                    if (((SpanAdv)inline).FontFamily.Source == name)
                        lstfontFamily.SelectedItem = item;
                }
                else if (inline is HyperlinkAdv)
                {
                    string name = item.Content as string;
                    if (((HyperlinkAdv)inline).FontFamily.Source == name)
                        lstfontFamily.SelectedItem = item;
                }
            }
        }

        private void TriggerFontSize(Inline inline)
        {
            foreach (ListBoxItem item in lstfontsize.Items)
            {
                if (inline is SpanAdv)
                {
                    string size = item.Content as string;
                    if (((SpanAdv)inline).FontSize == double.Parse(size))
                        lstfontsize.SelectedItem = item;
                }
                if (inline is HyperlinkAdv)
                {
                    string size = item.Content as string;
                    if (((HyperlinkAdv)inline).FontSize == double.Parse(size))
                        lstfontsize.SelectedItem = item;
                }
            }
        }

        private void TriggerFontStyle(Inline inline)
        {
            foreach (ListBoxItem item in lstfontstyle.Items)
            {
                if (inline is SpanAdv)
                {
                    string style = item.Content as string;
                    if (((SpanAdv)inline).FontStyle.ToString() == style)
                        lstfontstyle.SelectedItem = item;
                }
                else if (inline is HyperlinkAdv)
                {
                    string style = item.Content as string;
                    if (((HyperlinkAdv)inline).FontStyle.ToString() == style)
                        lstfontstyle.SelectedItem = item;
                }
            }
        }

        private void ApplyFontStyle()
        {
            if (lstfontstyle == null) return;

            var item = lstfontstyle.SelectedItem;
            if (item != null && item is ListBoxItem)
            {
                ListBoxItem boxitem = item as ListBoxItem;
                if (boxitem.Name == bolditem.Name)
                {
                    if (IsFontStyleChanged)
                    {
                        if (RichTB.CurrentInlineStyle.FontStyle == FontStyles.Italic)
                            RichTB.Selection.Italic();
                        if(RichTB.CurrentInlineStyle.FontWeight!=FontWeights.Bold)
                            RichTB.Selection.Bold();
                        IsFontStyleChanged = false;
                    }
                }
                else if (boxitem.Name == italicitem.Name)
                {
                    if (IsFontStyleChanged)
                    {
                        if (RichTB.CurrentInlineStyle.FontWeight == FontWeights.Bold)
                            RichTB.Selection.Bold();
                        if(RichTB.CurrentInlineStyle.FontStyle!=FontStyles.Italic)
                            RichTB.Selection.Italic();
                        IsFontStyleChanged = false;
                    }
                }
                else if (boxitem.Name == regularitem.Name)
                {
                    if (IsFontStyleChanged)
                    {
                        if (RichTB.CurrentInlineStyle.FontStyle == FontStyles.Italic)
                            RichTB.Selection.Italic();
                        if (RichTB.CurrentInlineStyle.FontWeight == FontWeights.Bold)
                            RichTB.Selection.Bold();
                        IsFontStyleChanged = false;
                    }
                }
                else if (boxitem.Name == bolditalicitem.Name)
                {
                    if (IsFontStyleChanged)
                    {
                        if(RichTB.CurrentInlineStyle.FontWeight!=FontWeights.Bold)
                            RichTB.Selection.Bold();
                        if(RichTB.CurrentInlineStyle.FontStyle!=FontStyles.Italic)
                            RichTB.Selection.Italic();
                        IsFontStyleChanged = false;
                    }
                }
            }
        }

        private void ApplyFontColor()
        {
            if (fontcolorpicker != null && IsTextColorChanged)
                RichTB.Selection.ChangeForeground(fontcolorpicker.Color);
            IsTextColorChanged = false;  
        }

        private void ApplyFontSize()
        {
            if (lstfontsize == null) return;
            var fontsize = (lstfontsize.SelectedItem as ListBoxItem).Content.ToString();
            if (fontsize != null)
            {
                double size = Convert.ToDouble(fontsize.ToString());
                if (IsFontSizeSelectionChanged)
                {
                    RichTB.Selection.ChangeFontSize(size);
                    IsFontSizeSelectionChanged = false;
                }
            }
        }
        
        private void ApplyFontFamily()
        {
            if (lstfontFamily == null) return;

            var fontFamily = (lstfontFamily.SelectedItem as ListBoxItem).Content.ToString();
            if (fontFamily != null)
            {           
                FontFamily fontName = new System.Windows.Media.FontFamily(fontFamily);
                if (IsFontFamilySelectionChanged)
                {
                    RichTB.Selection.ChangeFontFamily(fontName);
                    IsFontFamilySelectionChanged = false;
                }
            }
        }

        private void ApplySingleStrikeThrough()
        {
            if (chksingle != null)
            {
                if ((bool)chksingle.IsChecked)
                {
                    RichTB.Selection.ChangeSingleStrikeThrough();
                }
            }
        }

        private void ApplyDoubleStrikeThrough()
        {
            if (chkdouble != null)
            {
                if ((bool)chkdouble.IsChecked)
                {
                    RichTB.Selection.ChangeDoubleStrikeThrough();
                }
            }
        }

        private void ApplySuperScript()
        {
            if (chksuperscript != null)
            {
                if ((bool)chksuperscript.IsChecked)
                {
                    RichTB.Selection.ChangeSuperscript();
                }
            }
        }

        private void ApplySubScript()
        {
            if (chksubscript!= null)
            {
                if ((bool)chksubscript.IsChecked)
                {
                    RichTB.Selection.ChangeSubscript();
                }
            }
        }

        private void OnSingleChecked(object sender, EventArgs e)
        {
            chkdouble.IsChecked = false;
        }

        private void OnDoubleChecked(object sender, EventArgs e)
        {
            chksingle.IsChecked = false;
        }

        private void OnSuperscriptChecked(object sender, EventArgs e)
        {
            chksubscript.IsChecked = false;
        }

        private void OnSubscriptChecked(object sender, EventArgs e)
        {
            chksuperscript.IsChecked = false;
        }


        void fontcolorpicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsTextColorChanged = true;
        }

        void lstfontstyle_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IsFontStyleChanged = true;
        }

        void lstfontsize_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IsFontSizeSelectionChanged = true;
        }

        void lstfontFamily_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IsFontFamilySelectionChanged = true;
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //cmbhightlight.IsDropDownOpen = false;
            //cmbtextcolor.IsDropDownOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fontDialog.Close();
            this.RichTB.Focus();
        }

        private void contextmenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenuItemAdv menitem = sender as ContextMenuItemAdv;
            if (menitem != null)
            {
                Rectangle rect = (Rectangle)menitem.Icon;
                RichTB.Selection.ChangeHighlightColor((rect.Fill as SolidColorBrush).Color);
                //cmbhightlight.IsDropDownOpen = false;
            }
        }

        public string Error
        {
            get 
            {
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string tempstring = null;

                return tempstring;
            }
        }

    }

}
