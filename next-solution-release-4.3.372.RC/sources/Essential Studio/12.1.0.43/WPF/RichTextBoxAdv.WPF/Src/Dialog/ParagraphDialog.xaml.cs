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
using Syncfusion.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    public partial class ParagraphDialog : Window
    {

        private bool IsTextAlignChanged;

        public ParagraphDialog()
        {
            InitializeComponent();
        }

        public ParagraphDialog(RichTextBoxAdv _rich)
            : this()
        {
            RichTB = _rich;
            this.cmbalignment.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(cmbalignment_SelectionChanged);
        }

        private RichTextBoxAdv RichTB
        {
            get;
            set;
        }

        protected override void OnActivated(EventArgs e)
        {
            this.UpdateControls();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            Execute();
            paragraphDialog.Close();
        }

        private void Execute()
        {
            ApplySpacing();
            ApplyIndent();
            ApplyTextAlign();
            RichTB.Focus();
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowIcon.SetNull(this);
        }

        private bool IsSelectedParagraphsEqual()
        {
            bool isEqual = false;
            List<BlockAdv> blocks = RichTB.Selection.GetSelectedBlocks();
            if (blocks != null)
            {
                for (int i = 0; i < blocks.Count - 1; i++)
                {
                    if (blocks[i].IsTable || blocks[i + 1].IsTable)
                        return false;

                    if (i + 1 < blocks.Count)
                        isEqual = (blocks[i] as ParagraphAdv).IsEqualInStyle(blocks[i + 1] as ParagraphAdv);
                }
            }

            return isEqual;
        }

        private void UpdateControls()
        {
            if (RichTB.Viewer.IsSelected && IsSelectedParagraphsEqual())
            {
                ParagraphAdv parastyle = RichTB.CurrentParagraph;
                UpdateUpDowns(parastyle);
                TriggerAlignment(parastyle);
            }
            else if(RichTB.Viewer.IsSelected && RichTB.Selection.GetSelectedBlocks().Count==1 &&
                RichTB.Selection.GetSelectedBlocks()[0].IsParagraph)
            {
                ParagraphAdv parastyle = RichTB.CurrentParagraph;
                UpdateUpDowns(parastyle);
                TriggerAlignment(parastyle);
            }
            else if (!RichTB.Viewer.IsSelected)
            {
                if (RichTB != null)
                {
                    UpdateUpDowns(RichTB.CurrentParagraph);
                    TriggerAlignment(RichTB.CurrentParagraph);
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
        }

        private void UpdateUpDowns(ParagraphAdv paragraph)
        {
            if (paragraph != null)
            {
                if (leftIndention != null)
                    leftIndention.Value = paragraph.LeftIndent / 96;
                if (rightIndention != null)
                    rightIndention.Value = paragraph.RightIndent / 96;
                if (beforeSpacing != null)
                    beforeSpacing.Value = paragraph.BeforeSpacing * 1.333;
                if (afterSpacing != null)
                    afterSpacing.Value = paragraph.AfterSpacing * 1.333;
                if (linespacing != null)
                    linespacing.Value = paragraph.LineSpacing;
            }
        }

        private void TriggerAlignment(ParagraphAdv paragraph)
        {
            if (cmbalignment != null && paragraph !=null)
            {
                foreach (ComboBoxItem item in cmbalignment.Items)
                {
                    if (paragraph.TextAlignment == String2Alignment(item.Content as string))
                        cmbalignment.SelectedItem = item;
                }
            }
        }

        private void ApplySpacing()
        {
            if (afterSpacing != null)
                RichTB.Selection.ChangeAfterSpacing((double)afterSpacing.Value * 0.75);

            if (beforeSpacing != null)
                RichTB.Selection.ChangeBeforeSpacing((double)beforeSpacing.Value * 0.75);

            if (linespacing != null)
                RichTB.Selection.ChangeLineSpacing((double)linespacing.Value);
        }

        private void ApplyIndent()
        {
            if (leftIndention != null)
                if (!RichTB.PositionHandler.Paragraph.IsInsideTable)
                {
                    RichTB.Selection.ChangeLeftIndent((double)leftIndention.Value * 96);
                }
            if (rightIndention != null)
                if (!RichTB.PositionHandler.Paragraph.IsInsideTable)
                {
                    RichTB.Selection.ChangeRightIndent((double)rightIndention.Value * 96);
                }
        }

        private void ApplyTextAlign()
        {
            if (cmbalignment == null) return;

            var content = (cmbalignment.SelectedItem as ComboBoxItem).Content as string;
            if (IsTextAlignChanged)
                RichTB.Selection.ChangeTextAlignment(String2Alignment(content));
        }

        private TextAlignment String2Alignment(string align)
        {
            switch (align.ToLower())
            {
                case "left":
                    return System.Windows.TextAlignment.Left;
                case "right":
                    return System.Windows.TextAlignment.Right;
                case "centered":
                    return System.Windows.TextAlignment.Center;
                case "justified":
                    return System.Windows.TextAlignment.Justify;
                default:
                    break;
            }
            return System.Windows.TextAlignment.Left;
        }

        void cmbalignment_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IsTextAlignChanged = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            paragraphDialog.Close();

            this.RichTB.Focus();
        }


    }
}
