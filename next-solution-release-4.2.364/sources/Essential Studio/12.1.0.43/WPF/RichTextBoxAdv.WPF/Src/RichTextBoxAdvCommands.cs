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
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using Syncfusion.Windows.Shared;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    public partial class RichTextBoxAdv
    {
        public static readonly RoutedUICommand Cut = new RoutedUICommand("Cut","Cut", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand Copy = new RoutedUICommand("Copy", "Copy", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand Paste = new RoutedUICommand("Paste", "Paste", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ToggleBold = new RoutedUICommand("ToggleBold", "ToggleBold", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ToggleItalic = new RoutedUICommand("ToggleItalic", "ToggleItalic", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ToggleUnderline = new RoutedUICommand("ToggleUnderline", "ToggleUnderline", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ToggleSubScript = new RoutedUICommand("ToggleSubScript", "ToggleSubScript", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ToggleSuperScript = new RoutedUICommand("ToggleSuperScript", "ToggleSuperScript", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ToggleSingleStrikeThrough = new RoutedUICommand("ToggleSingleStrikeThrough", "ToggleSingleStrikeThrough", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ToggleDoubleStrikeThrough = new RoutedUICommand("ToggleDoubleStrikeThrough", "ToggleDoubleStrikeThrough", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand AfterSpacing = new RoutedUICommand("AfterSpacing", "AfterSpacing", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand BeforeSpacing = new RoutedUICommand("BeforeSpacing", "BeforeSpacing", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ChangeFontFamily = new RoutedUICommand("ChangeFontFamily", "ChangeFontFamily", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ChangeFontSize = new RoutedUICommand("ChangeFontSize", "ChangeFontSize", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand HighlightColor = new RoutedUICommand("HighlightColor", "HighlightColor", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ListType = new RoutedUICommand("ListType", "ListType", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ChangeLayout = new RoutedUICommand("ChangeLayout", "ChangeLayout", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand TextColor = new RoutedUICommand("TextColor", "TextColor", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand DeleteColumn = new RoutedUICommand("DeleteColumn", "DeleteColumn", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand DeleteRow = new RoutedUICommand("DeleteRow", "DeleteRow", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand DeleteTable = new RoutedUICommand("DeleteTable", "DeleteTable", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand FontDialog = new RoutedUICommand("FontDialog", "FontDialog", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand HyperlinkDialog = new RoutedUICommand("HyperlinkDialog", "HyperlinkDialog", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand InsertColumn = new RoutedUICommand("InsertColumn", "InsertColumn", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand InsertInline = new RoutedUICommand("InsertInline", "InsertInline", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand InsertPicture = new RoutedUICommand("InsertPicture", "InsertPicture", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand InsertRow = new RoutedUICommand("InsertRow", "InsertRow", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand InsertTable = new RoutedUICommand("InsertTable", "InsertTable", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand InsertTableDialog = new RoutedUICommand("InsertTableDialog", "InsertTableDialog", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand Italic = new RoutedUICommand("Italic", "Italic", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand LeftIndent = new RoutedUICommand("LeftIndent", "LeftIndent", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand LineSpacing = new RoutedUICommand("LineSpacing", "LineSpacing", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand MergeSelectedCells = new RoutedUICommand("MergeSelectedCells", "MergeSelectedCells", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand New = new RoutedUICommand("New", "New", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand Open = new RoutedUICommand("Open", "Open", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand ParagraphDialog = new RoutedUICommand("ParagraphDialog", "ParagraphDialog", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand Print = new RoutedUICommand("Print", "Print", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand Redo = new RoutedUICommand("Redo", "Redo", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand RightIndent = new RoutedUICommand("RightIndent", "RightIndent", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand SaveAs = new RoutedUICommand("SaveAs", "SaveAs", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand Save = new RoutedUICommand("Save", "Save", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand SelectCell = new RoutedUICommand("SelectCell", "SelectCell", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand SelectColumn = new RoutedUICommand("SelectColumn", "SelectColumn", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand SelectRow = new RoutedUICommand("SelectRow", "SelectRow", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand SelectTable = new RoutedUICommand("SelectTable", "SelectTable", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand TextAlignment = new RoutedUICommand("TextAlignment","TextAlignment", typeof(RichTextBoxAdv));

        public static readonly RoutedUICommand Undo = new RoutedUICommand("Undo","Undo", typeof(RichTextBoxAdv));


        internal void RegisterCommands()
        {
            CommandBindings.Add(new CommandBinding(Cut, OnCutExecuted, OnCutCanExecute));
            CommandBindings.Add(new CommandBinding(Copy, OnCopyExecuted, OnCopyCanExecute));
            CommandBindings.Add(new CommandBinding(Paste, OnPasteExecuted, OnPasteCanExecute));
            CommandBindings.Add(new CommandBinding(ToggleBold, OnBoldExecuted));
            CommandBindings.Add(new CommandBinding(ToggleItalic, OnItalicExecuted));
            CommandBindings.Add(new CommandBinding(ToggleUnderline, OnUnderlineExecuted));
            CommandBindings.Add(new CommandBinding(ToggleSubScript, OnSubscriptExecuted));
            CommandBindings.Add(new CommandBinding(ToggleSuperScript, OnSuperscriptExecuted));
            CommandBindings.Add(new CommandBinding(ToggleSingleStrikeThrough, OnSingleStrikeThroughExecuted));
            CommandBindings.Add(new CommandBinding(ToggleDoubleStrikeThrough, OnDoubleStrikeThroughExecuted));
            CommandBindings.Add(new CommandBinding(AfterSpacing, OnAfterSpacingExecuted));
            CommandBindings.Add(new CommandBinding(BeforeSpacing, OnBeforeSpacingExecuted));
            CommandBindings.Add(new CommandBinding(ChangeFontFamily, OnFontFamilyExecuted));
            CommandBindings.Add(new CommandBinding(ChangeFontSize, OnFontSizeExecuted));
            CommandBindings.Add(new CommandBinding(HighlightColor, OnHighlightColorExecuted));
            CommandBindings.Add(new CommandBinding(ListType, OnListTypeExecuted));
            CommandBindings.Add(new CommandBinding(ChangeLayout, OnPageLayoutExecuted, OnPageLayoutCanExecute));
            CommandBindings.Add(new CommandBinding(TextColor, OnTextColorExecuted));
            CommandBindings.Add(new CommandBinding(DeleteColumn, OnDeleteColumnExecuted, OnDeleteColumnCanExecute));
            CommandBindings.Add(new CommandBinding(DeleteRow, OnDeleteRowExecuted, OnDeleteRowCanExecute));
            CommandBindings.Add(new CommandBinding(DeleteTable, OnDeleteTableExecuted, OnDeleteTableCanExecute));
            CommandBindings.Add(new CommandBinding(FontDialog, OnFontDialogExecuted));
            CommandBindings.Add(new CommandBinding(HyperlinkDialog, OnHyperlinkDialogExecuted));
            CommandBindings.Add(new CommandBinding(InsertColumn, OnInsertColumnExecuted, OnInsertColumnCanExecute));
            CommandBindings.Add(new CommandBinding(InsertInline, OnInsertInlineExecuted));
            CommandBindings.Add(new CommandBinding(InsertPicture, OnInsertPictureExecuted));
            CommandBindings.Add(new CommandBinding(InsertRow, OnInsertRowExecuted, OnInsertRowCanExecute));
            CommandBindings.Add(new CommandBinding(InsertTable, OnInsertTableExecuted, OnInsertTableCanExecute));
            CommandBindings.Add(new CommandBinding(InsertTableDialog, OnInsertTableDialogExecuted, OnInsertTableDialogCanExecute));
            CommandBindings.Add(new CommandBinding(LeftIndent, OnLeftIndentExecuted));
            CommandBindings.Add(new CommandBinding(RightIndent, OnRightIndentExecuted));
            CommandBindings.Add(new CommandBinding(LineSpacing, OnLineSpacingExecuted));
            CommandBindings.Add(new CommandBinding(MergeSelectedCells, OnMergeSelectedCellsExecuted, OnMergeSelectedCellsCanExecute));
            CommandBindings.Add(new CommandBinding(New, OnNewExecuted, OnNewCanExecute));
            CommandBindings.Add(new CommandBinding(Open, OnOpenExecuted, OnOpenCanExecute));
            CommandBindings.Add(new CommandBinding(ParagraphDialog, OnParagraphDialogExecuted));
            CommandBindings.Add(new CommandBinding(Print, OnPrintExecuted));
            CommandBindings.Add(new CommandBinding(Redo, OnRedoExecuted, OnRedoCanExecute));
            CommandBindings.Add(new CommandBinding(SaveAs, OnSaveAsExecuted));
            CommandBindings.Add(new CommandBinding(Save, OnSaveExecuted));
            CommandBindings.Add(new CommandBinding(SelectCell, OnSelectCellExecuted, OnSelectCellCanExecute));
            CommandBindings.Add(new CommandBinding(SelectColumn, OnSelectColumnExecuted, OnSelectColumnCanExecute));
            CommandBindings.Add(new CommandBinding(SelectRow, OnSelectRowExecuted, OnSelectRowCanExecute));
            CommandBindings.Add(new CommandBinding(SelectTable, OnSelectTableExecuted, OnSelectTableCanExecute));
            CommandBindings.Add(new CommandBinding(TextAlignment, OnTextAlignmentExecuted));
            CommandBindings.Add(new CommandBinding(Undo, OnUndoExecuted, OnUndoCanExecute));

            InputBindings.Add(new InputBinding(Cut, new KeyGesture(Key.X, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(Copy, new KeyGesture(Key.C, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(Paste, new KeyGesture(Key.V, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(ToggleBold, new KeyGesture(Key.B, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(ToggleItalic, new KeyGesture(Key.I, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(ToggleUnderline, new KeyGesture(Key.U, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(ToggleSubScript, new KeyGesture(Key.OemPlus, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(ToggleSuperScript, new KeyGesture(Key.OemPlus, ModifierKeys.Control | ModifierKeys.Shift)));
            InputBindings.Add(new InputBinding(TextAlignment, new KeyGesture(Key.L, ModifierKeys.Control)) { CommandParameter = System.Windows.TextAlignment.Left });
            InputBindings.Add(new InputBinding(TextAlignment, new KeyGesture(Key.E, ModifierKeys.Control)) { CommandParameter = System.Windows.TextAlignment.Center });
            InputBindings.Add(new InputBinding(TextAlignment, new KeyGesture(Key.R, ModifierKeys.Control)) { CommandParameter = System.Windows.TextAlignment.Right });
            InputBindings.Add(new InputBinding(TextAlignment, new KeyGesture(Key.J, ModifierKeys.Control)) { CommandParameter = System.Windows.TextAlignment.Justify });
            InputBindings.Add(new InputBinding(Undo, new KeyGesture(Key.Z, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(Redo, new KeyGesture(Key.Y, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(Save, new KeyGesture(Key.S, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(Print, new KeyGesture(Key.P, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(Open, new KeyGesture(Key.O, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(New, new KeyGesture(Key.N, ModifierKeys.Control)));
        }

        internal void OnCutCanExecute(object sender,CanExecuteRoutedEventArgs e)
        {
            e.CanExecute= Viewer.IsSelected;
        }

        internal void OnCutExecuted(object sender,ExecutedRoutedEventArgs e)
        {
            Selection.Cut();
            Focus();
        }

        internal void OnCopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute= Viewer.IsSelected;
        }

        internal void OnCopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.Copy();
            Focus();
        }

        internal void OnPasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute=ClipboardAdv.ContainsText();
        }

        internal void OnPasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.Paste();
            Focus();
        }

        internal void OnBoldExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.Bold();
            Focus();
        }

        internal void OnItalicExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.Italic();
            Focus();
        }

        internal void OnUnderlineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.ChangeUnderline();
            Focus();
        }

        internal void OnSubscriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.ChangeSubscript();
            Focus();
        }

        internal void OnSuperscriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.ChangeSuperscript();
            Focus();
        }

        internal void OnSingleStrikeThroughExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.ChangeSingleStrikeThrough();
            Focus();
        }

        internal void OnDoubleStrikeThroughExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Selection.ChangeDoubleStrikeThrough();
            Focus();
        }

        internal void OnAfterSpacingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            double spacingLength = 0.0;
            if (e.Parameter is double)
                spacingLength = Convert.ToDouble(e.Parameter);
            else if (e.Parameter is string)
                spacingLength = double.Parse(e.Parameter.ToString(), CultureInfo.InvariantCulture);
            if (spacingLength > 0.0)
            {
                Selection.ChangeAfterSpacing(spacingLength);
            }
            Focus();
        }

        internal void OnBeforeSpacingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            double spacingLength = 0.0;
            if (e.Parameter is double)
                spacingLength = Convert.ToDouble(e.Parameter);
            else if (e.Parameter is string)
                spacingLength = double.Parse(e.Parameter.ToString(), CultureInfo.InvariantCulture);
            if (spacingLength > 0.0)
            {
                Selection.ChangeBeforeSpacing(spacingLength);
            }
            Focus();
        }

        internal void OnFontFamilyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Media.FontFamily fontFamily = null;
            if (e.Parameter != null)
            {
                string fontName = e.Parameter.ToString();
                fontFamily = new System.Windows.Media.FontFamily(fontName);

                Selection.ChangeFontFamily(fontFamily);
                Focus();
            }
            Focus();
        }

        internal void OnFontSizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            double fontSize = 0.0;
            if (e.Parameter is double)
                fontSize = Convert.ToDouble(e.Parameter);
            else if (e.Parameter is string)
                fontSize = double.Parse(e.Parameter.ToString(), CultureInfo.InvariantCulture);
            if (fontSize > 0.0)
                Selection.ChangeFontSize(fontSize);
            Focus();
        }

        internal void OnHighlightColorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                Color color = (Color)e.Parameter;
                Brush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                Selection.ChangeHighlightColor(color);
                Focus();
            }
        }

        internal void OnListTypeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ListType listType = Syncfusion.Windows.Tools.Controls.ListType.None;
            if (e.Parameter != null)
            {
                listType = (ListType)Enum.Parse(typeof(ListType), e.Parameter.ToString());
                listType = ToggleBullet(listType);
                ChangeListType(listType);
            }
            Focus();
        }

        internal void OnPageLayoutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                PageLayout layout;
                if (e.Parameter != null)
                {
                    layout = (PageLayout)Enum.Parse(typeof(PageLayout), e.Parameter.ToString());

                    e.CanExecute= PageLayout != layout;
                }
            }
        }

        internal void OnPageLayoutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PageLayout layout;
            if (e.Parameter != null)
            {
                layout = (PageLayout)Enum.Parse(typeof(PageLayout), e.Parameter.ToString());
                ChangePageLayout(layout);
            }
            Focus();
        }

        internal void OnTextColorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Color color = (Color)e.Parameter;
            Selection.ChangeForeground(color);
            Focus();
        }

        internal void OnDeleteColumnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnDeleteColumnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteColumnFromTable();
            Focus();
        }

        internal void OnDeleteRowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnDeleteRowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteRowFromTable();
            Focus();
        }

        internal void OnDeleteTableCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnDeleteTableExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteTableFromBlocks();
            Focus();
        }

        internal void OnFontDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog(this);
            fontDialog.ShowDialog();
        }

        internal void OnHyperlinkDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HyperlinkDialog hyperlinkDialog = new HyperlinkDialog(this);
            hyperlinkDialog.ShowDialog();
        }

        internal void OnInsertColumnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnInsertColumnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ColumnPlacement placement;
            if (e.Parameter != null)
            {
                placement = (ColumnPlacement)Enum.Parse(typeof(ColumnPlacement), e.Parameter.ToString());
                InsertColumnInTable(placement);
                Focus();
            }
        }

        internal void OnInsertInlineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            InsertInlineInParagraph(e.Parameter as Inline);
            Focus();
        }

        internal void OnInsertPictureExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog opendialog = new OpenFileDialog()
            {
                Filter = "Images (*.png,*.jpeg,*.jpg)|*.png;*.jpeg;*.jpg",
                FilterIndex = 1
            };
            if (opendialog.ShowDialog() != null)
            {
                BitmapImage bitmapimage = new BitmapImage();
                ImageContainerAdv imagadv;
                imagadv = new ImageContainerAdv();
                if (opendialog.FileName != string.Empty)
                {
                    bitmapimage.BeginInit();
                    string filename = opendialog.FileName;
                    Uri newUri = new Uri("file://" + filename, UriKind.RelativeOrAbsolute);
                    bitmapimage.UriSource = newUri;
                    bitmapimage.EndInit();
                    imagadv.ImageBytes = File.ReadAllBytes(filename);
                    imagadv.ImageSource = bitmapimage;
                    imagadv.Height = bitmapimage.PixelHeight;
                    imagadv.Width = bitmapimage.PixelWidth;
                    InsertInlineInParagraph(imagadv);
                    Focus();
                }
            }
        }

        internal void OnInsertRowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnInsertRowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RowPlacement rowplacement;
            if (e.Parameter != null)
            {
                rowplacement = (RowPlacement)Enum.Parse(typeof(RowPlacement), e.Parameter.ToString());
                InsertRowInTable(rowplacement);
                Focus();
            }
        }

        internal void OnInsertTableCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Viewer.IsSelected;
        }

        internal void OnInsertTableExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string)
            {
                string[] collection = e.Parameter.ToString().Split(new string[] { ",", " " }, StringSplitOptions.None);
                InsertTableInBlocks(int.Parse(collection[0]), int.Parse(collection[1]));
            }
            else if (e.Parameter is int[])
            {
                int[] rowcolumn = (int[])e.Parameter;
                InsertTableInBlocks(rowcolumn[0], rowcolumn[1]);
            }
            Focus();
        }

        internal void OnInsertTableDialogCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Viewer.IsSelected;
        }

        internal void OnInsertTableDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            InsertTableDialog tableDialog = new InsertTableDialog(this);
            tableDialog.ShowDialog();
        }

        internal void OnLeftIndentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            bool leftindent = false;
            if (e.Parameter != null && Boolean.TryParse(e.Parameter.ToString(), out leftindent))
            {
                double defleftindent = 36.0;
                double leftIndentValue = 0;
                if ((bool)leftindent)
                {
                    leftIndentValue = CurrentParagraphStyle.LeftIndent + defleftindent;
                }
                else
                {
                    leftIndentValue = CurrentParagraphStyle.LeftIndent - defleftindent;
                }
                Selection.ChangeLeftIndent(leftIndentValue);
                Focus();
            }
        }

        internal void OnLineSpacingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            double lineSpacing = 0.0;
            if (e.Parameter is double)
                lineSpacing = Convert.ToDouble(e.Parameter);
            else if (e.Parameter is string)
                lineSpacing = double.Parse(e.Parameter.ToString(), CultureInfo.InvariantCulture);
            if (lineSpacing > 0.0)
                Selection.ChangeLineSpacing(lineSpacing);
            Focus();
        }

        internal void OnMergeSelectedCellsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanMerge();
        }

        internal void OnMergeSelectedCellsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MergeSelectedCellsInTable();
            Focus();
        }

        internal void OnNewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        internal void OnNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CreateEmptyDocument();
            Focus();
        }

        internal void OnOpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        internal void OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenDocument();
            Focus();
        }

        internal void OnParagraphDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ParagraphDialog paragraphDialog = new ParagraphDialog(this);
            paragraphDialog.ShowDialog();
        }

        internal void OnPrintExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDocument();
            Focus();
        }

        internal void OnRedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = History.CanRedo();
        }

        internal void OnRedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            History.Redo();
            Focus();
        }

        internal void OnRightIndentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is bool)
            {
                double rightindent = 36.0;
                double rightIndentValue = 0;
                if ((bool)e.Parameter)
                {
                    rightIndentValue = CurrentParagraphStyle.RightIndent + rightindent;
                }
                else
                {
                    rightIndentValue = CurrentParagraphStyle.RightIndent - rightindent;
                }
                Selection.ChangeRightIndent(rightIndentValue);
            }
            Focus();
        }

        internal void OnSaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            string extension=string.Empty;
            if (e.Parameter == null)
                extension = ".docx";
            else
                extension= e.Parameter.ToString();
            SaveDocument(extension);
        }

        internal void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDocument(string.Empty);
            Focus();
        }

        internal void OnSelectCellCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnSelectCellExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectCellInTable();
            Focus();
        }

        internal void OnSelectColumnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnSelectColumnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectColumnInTable();
            Focus();
        }

        internal void OnSelectRowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnSelectRowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectRowInTable();
            Focus();
        }

        internal void OnSelectTableCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PositionHandler.TextPosition.IsInsideTable;
        }

        internal void OnSelectTableExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectTableInBlocks();
        }

        internal void OnTextAlignmentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                string text = e.Parameter.ToString();
                System.Windows.TextAlignment textAlignment = System.Windows.TextAlignment.Left;
                if (text == "Right")
                {
                    textAlignment = System.Windows.TextAlignment.Right;
                }
                else if (text == "Left")
                {
                    textAlignment = System.Windows.TextAlignment.Left;
                }
                else if (text == "Center")
                {
                    textAlignment = System.Windows.TextAlignment.Center;
                }
                else if (text == "Justify")
                {
                    textAlignment = System.Windows.TextAlignment.Justify;
                }
                Selection.ChangeTextAlignment(textAlignment);
                Focus();
            } 
        }

        internal void OnUndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = History.CanUndo();
        }

        internal void OnUndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            History.Undo();
            Focus();
        }
    }
}
