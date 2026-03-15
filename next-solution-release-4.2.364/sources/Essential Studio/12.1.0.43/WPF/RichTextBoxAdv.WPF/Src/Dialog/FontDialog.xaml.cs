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
using System.Windows.Markup;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FontDialog : Window
    {

        public RichTextBoxAdv RichTB
        { get; set; }
        private bool IsFontFamilySelectionChanged;
        private bool IsFontSizeSelectionChanged;
        private bool IsFontStyleChanged;
        private bool IsTextColorChanged;
        private bool IsHighlightColorChanged;

        public FontDialog()
        {
            InitializeComponent();

            this.Loaded += (sender, e) =>
                {
                    List<FontFamily> FontsCollection = new List<System.Windows.Media.FontFamily>();
                    XmlLanguage userLanguage = XmlLanguage.GetLanguage("en-US");
                    foreach (FontFamily fontfamily in Fonts.SystemFontFamilies)
                    {
                        LanguageSpecificStringDictionary dictionary = fontfamily.FamilyNames;
                        if (dictionary.Keys.Contains(userLanguage))
                        {
                            FontsCollection.Add(fontfamily);
                        }
                    }
                    lstfontFamily.ItemsSource = FontsCollection;
                };
        } 

        public FontDialog(RichTextBoxAdv _richtext)
            : this()
        {
            RichTB = _richtext;;
            this.colorpicker.ColorChanged += new PropertyChangedCallback(fontcolorpicker_ColorChanged);
            lstfontFamily.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(lstfontFamily_SelectionChanged);
            lstfontsize.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(lstfontsize_SelectionChanged);
            lstfontstyle.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(lstfontstyle_SelectionChanged);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowIcon.SetNull(this);
        }

        protected override void OnActivated(EventArgs e)
        {
            UpdateControls();
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
            ApplyHighlightColor();
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
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
                if (RichTB.CurrentInlineStyle != null)
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

                    colorpicker.Color = RichTB.CurrentInlineStyle.Foreground;
                }
            }
            else if (RichTB.Viewer.IsSelected && (IsSelectedInlinesEqual() || RichTB.Selection.GetInlinesToFormat().Count == 1))
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

                colorpicker.Color = (RichTB.Selection.GetInlinesToFormat()[0] as SpanAdv).Foreground;
            }
        }

        private void TriggerFontFamily(Inline inline)
        {
            foreach (object item in lstfontFamily.Items)
            {
                FontFamily fontfamily = item as FontFamily;
                if (fontfamily != null)
                {
                    if (inline is SpanAdv)
                    {
                        string name = fontfamily.ToString();
                        if (((SpanAdv)inline).FontFamily.Source == name)
                        {
                            lstfontFamily.SelectedItem = item;
                        }
                    }
                    else if (inline is HyperlinkAdv)
                    {
                        string name = fontfamily.ToString();
                        if (((HyperlinkAdv)inline).FontFamily.Source == name)
                        {
                            lstfontFamily.SelectedItem = item;
                        }
                    }
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
                    if (((SpanAdv)inline).FontSize == double.Parse(size, CultureInfo.InvariantCulture))
                        lstfontsize.SelectedItem = item;
                }
                if (inline is HyperlinkAdv)
                {
                    string size = item.Content as string;
                    if (((HyperlinkAdv)inline).FontSize == double.Parse(size, CultureInfo.InvariantCulture))
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
                    if (((SpanAdv)inline).FontWeight.ToString() == "Normal")
                        lstfontstyle.SelectedIndex = 0;
                    else if (((SpanAdv)inline).FontWeight.ToString() == "Bold")
                        lstfontstyle.SelectedIndex = 1;
                    if (((SpanAdv)inline).FontStyle.ToString() == "Italic")
                        lstfontstyle.SelectedIndex = 2;
                }
                else if (inline is HyperlinkAdv)
                {
                    string style = item.Content as string;
                    if (((HyperlinkAdv)inline).FontWeight.ToString() == "Normal")
                        lstfontstyle.SelectedIndex = 0;
                    else if (((HyperlinkAdv)inline).FontWeight.ToString() == "Bold")
                        lstfontstyle.SelectedIndex = 1;
                    if (((HyperlinkAdv)inline).FontStyle.ToString() == "Italic")
                        lstfontstyle.SelectedIndex = 2;
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
                        if (RichTB.CurrentInlineStyle.FontWeight != FontWeights.Bold)
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
                        if (RichTB.CurrentInlineStyle.FontStyle != FontStyles.Italic)
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
                        if (RichTB.CurrentInlineStyle.FontWeight != FontWeights.Bold)
                            RichTB.Selection.Bold();
                        if (RichTB.CurrentInlineStyle.FontStyle != FontStyles.Italic)
                            RichTB.Selection.Italic();
                        IsFontStyleChanged = false;
                    }
                }
            }
        }

        private void ApplyFontColor()
        {
            if (colorpicker != null && IsTextColorChanged)
                RichTB.Selection.ChangeForeground(colorpicker.Color);
            IsTextColorChanged = false;
        }

        private void ApplyHighlightColor()
        {
            //if (highlightcolorpicker != null && IsHighlightColorChanged)
            //    RichTB.Selection.ChangeHighlightColor(highlightcolorpicker.Color);
            //IsHighlightColorChanged = false;
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

            FontFamily fontFamily = lstfontFamily.SelectedItem as FontFamily;
            if (fontFamily != null)
            {
                if (IsFontFamilySelectionChanged)
                {
                    RichTB.Selection.ChangeFontFamily(fontFamily);
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
            if (chksubscript != null)
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

        void highlightcolorpicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsHighlightColorChanged = true;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fontDialog.Close();
            this.RichTB.Focus();
        }

        private void OnMouseLeftButtonDownOnRectangle(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            RichTB.Selection.ChangeHighlightColor((rectangle.Fill as SolidColorBrush).Color);
            IsHighlightColorChanged = false;
         //   cmbhightlight.IsDropDownOpen = false;
        }

        private void contextmenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            ContextMenuItemAdv menitem = sender as ContextMenuItemAdv;
            if (menitem != null)
            {
                Rectangle rect = (Rectangle)menitem.Icon;
                RichTB.Selection.ChangeHighlightColor((rect.Fill as SolidColorBrush).Color);
                IsHighlightColorChanged = false;
                cmbhightlight.IsDropDownOpen = false;
            }
#endif
        }
    }



    public static class WindowIcon
    {
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                   int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
                   IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;

        public static void SetNull(Window window)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Update the window's non-client area to reflect the changes
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE |
                  SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

    }
 
}
       