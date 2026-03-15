using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Threading;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.Xpf.Diagram;
using System.Windows.Media;
using ViewModelLib;
using System.Windows;
using ReportManager.Extensions;
using DevExpress.Diagram.Core;

namespace ReportManager
{
    public class UIReportEditorCommands : INotifyPropertyChanged
    {
        #region Declarations
        readonly ReportEditorUI reportEditor;
        ReportDesignerDocument lastActiveDocument;
        #endregion

        #region Constructors
        public UIReportEditorCommands(ReportEditorUI reportEditor)
        {
            this.reportEditor = reportEditor;

            reportEditor.reportDesigner.ActiveDocumentChanged += ReportDesigner_ActiveDocumentChanged;
        }
        #endregion

        #region Privates
        void ReportDesigner_ActiveDocumentChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (lastActiveDocument != null)
            {
                lastActiveDocument.Diagram.SelectionChanged -= Diagram_SelectionChanged;
                lastActiveDocument.CanSaveChanged -= LastActiveDocument_CanSaveChanged;
            }

            lastActiveDocument = ActiveDocument;
            if (lastActiveDocument != null)
            {
                lastActiveDocument.Diagram.AllowDiagramProperties = false;
                lastActiveDocument.Diagram.PropertiesPanelVisibility = DevExpress.Diagram.Core.PropertiesPanelVisibility.Collapsed;
                lastActiveDocument.Diagram.SelectionChanged += Diagram_SelectionChanged;
                lastActiveDocument.CanSaveChanged += LastActiveDocument_CanSaveChanged;
            }
        }

        private void LastActiveDocument_CanSaveChanged(object sender, EventArgs e)
        {
            reportEditor.Document.NeedsSave = true;
        }

        void Diagram_SelectionChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("CanEditBorderProperties");

            ForceUpdateFontProperties();
            ForceUpdateTextAlignProperties();
            ForceUpdateBorderProperties();

            reportEditor.editorComponent.Workspace.ContextObject = SelectedObject;
        }

        internal void ForceUpdateFontProperties()
        {
            OnPropertyChanged("FontName");
            OnPropertyChanged("FontSize");
            OnPropertyChanged("IsFontBold");
            OnPropertyChanged("IsFontItalic");
            OnPropertyChanged("IsFontUnderline");
            OnPropertyChanged("IsFontStrikethrough");
        }

        internal void ForceUpdateTextAlignProperties()
        {
            OnPropertyChanged("IsTextAlignLeft");
            OnPropertyChanged("IsTextAlignCenter");
            OnPropertyChanged("IsTextAlignRight");
            OnPropertyChanged("IsTextAlignJustify");
        }

        internal void ForceUpdateBorderProperties()
        {
            OnPropertyChanged("BorderWidth");
        }
        #endregion

        #region ViewModel
        public bool CanEditBorderProperties
        {
            get
            {
                return SelectedItems != null && SelectedItems.Count > 0;
            }
        }

        public String FontName
        {
            get
            {
                return PrimarySelection != null ? PrimarySelection.FontFamily.Source : null;
            }
        }

        public double FontSize
        {
            get
            {
                return PrimarySelection != null ? PrimarySelection.FontSize : 0.0;
            }
        }

        public bool IsFontBold
        {
            get
            {
                return PrimarySelection != null && PrimarySelection.FontWeight == FontWeights.Bold;
            }
        }

        public bool IsFontItalic
        {
            get
            {
                return PrimarySelection != null && PrimarySelection.FontStyle == FontStyles.Italic;
            }
        }

        public bool IsFontUnderline
        {
            get
            {
                bool isFontUnderline = false;
                if (PrimarySelection != null && PrimarySelection.TextDecorations != null)
                {
                    foreach (var item in TextDecorations.Underline)
                    {
                        if (PrimarySelection.TextDecorations.Contains(item))
                        {
                            isFontUnderline = true;
                            break;
                        }
                    }
                }

                return isFontUnderline;
            }
        }

        public bool IsFontStrikethrough
        {
            get
            {
                bool isFontStrikethrough = false;
                if (PrimarySelection != null && PrimarySelection.TextDecorations != null)
                {
                    foreach (var item in TextDecorations.Strikethrough)
                    {
                        if (PrimarySelection.TextDecorations.Contains(item))
                        {
                            isFontStrikethrough = true;
                            break;
                        }
                    }
                }

                return isFontStrikethrough;
            }
        }

        public bool IsTextAlignLeft
        {
            get
            {
                return PrimarySelection != null && PrimarySelection.TextAlignment == System.Windows.TextAlignment.Left;
            }
        }

        public bool IsTextAlignCenter
        {
            get
            {
                return PrimarySelection != null && PrimarySelection.TextAlignment == System.Windows.TextAlignment.Center;
            }
        }

        public bool IsTextAlignRight
        {
            get
            {
                return PrimarySelection != null && PrimarySelection.TextAlignment == System.Windows.TextAlignment.Right;
            }
        }

        public bool IsTextAlignJustify
        {
            get
            {
                return PrimarySelection != null && PrimarySelection.TextAlignment == System.Windows.TextAlignment.Justify;
            }
        }

        public double BorderWidth
        {
            get
            {
                return PrimarySelection != null && PrimarySelection.BorderThickness != null ? PrimarySelection.BorderThickness.Left : 0.0;
            }
        }
        #endregion

        #region Command Bindings
        ICommand runWizard;
        public ICommand RunWizard
        {
            get
            {
                if (runWizard == null)
                {
                    runWizard = new RelayCommand(
                        param =>
                        {
                            if (ActiveDocument == null)
                                return;

                            ActiveDocument.RunWizard();
                        }, param => ActiveDocument != null);
                }

                return runWizard;
            }
        }

        ICommand save;
        public ICommand Save
        {
            get
            {
                if (save == null)
                {
                    save = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.SaveFile.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.SaveFile.CanExecute(null));
                }

                return save;
            }
        }

        ICommand edit;
        public ICommand Edit
        {
            get
            {
                if (edit == null)
                {
                    edit = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.Edit.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.Edit.CanExecute(null));
                }

                return save;
            }
        }


        ICommand delete;
        public ICommand Delete
        {
            get
            {
                if (delete == null)
                {
                    delete = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.Delete.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.Delete.CanExecute(null));
                }

                return delete;
            }
        }

        ICommand borderType;
        public ICommand BorderType
        {
            get
            {
                if (borderType == null)
                {
                    borderType = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null)
                                return;

                            BorderSide borderSide;
                            switch (param as String)
                            {
                                case "All":
                                    borderSide = BorderSide.All;
                                    break;
                                case "Left":
                                    borderSide = BorderSide.Left;
                                    break;
                                case "Top":
                                    borderSide = BorderSide.Top;
                                    break;
                                case "Right":
                                    borderSide = BorderSide.Right;
                                    break;
                                case "Bottom":
                                    borderSide = BorderSide.Bottom;
                                    break;
                                default:
                                    borderSide = BorderSide.None;
                                    break;
                            }

                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                            {
                                var dest = SelectedItems[ii].FindPropertyDescriptor("Borders");
                                if (dest != null)
                                    dest.SetValue(SelectedItems[ii], borderSide);
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 0);
                }

                return borderType;
            }
        }

        ICommand borderStyle;
        public ICommand BorderStyle
        {
            get
            {
                if (borderStyle == null)
                {
                    borderStyle = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null)
                                return;

                            BorderDashStyle borderDashStyle;
                            switch (param as String)
                            {
                                case "Dot":
                                    borderDashStyle = BorderDashStyle.Dot;
                                    break;
                                case "Dash":
                                    borderDashStyle = BorderDashStyle.Dash;
                                    break;
                                case "DashDot":
                                    borderDashStyle = BorderDashStyle.DashDot;
                                    break;
                                case "DashDotDot":
                                    borderDashStyle = BorderDashStyle.DashDotDot;
                                    break;
                                case "Double":
                                    borderDashStyle = BorderDashStyle.Double;
                                    break;
                                default:
                                    borderDashStyle = BorderDashStyle.Solid;
                                    break;
                            }

                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                            {
                                var dest = SelectedItems[ii].FindPropertyDescriptor("BorderDashStyle");
                                if (dest != null)
                                    dest.SetValue(SelectedItems[ii], borderDashStyle);
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 0);
                }

                return borderStyle;
            }
        }

        ICommand borderColor;
        public ICommand BorderColor
        {
            get
            {
                if (borderColor == null)
                {
                    borderColor = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || !(param is Color))
                                return;

                            var color = (Color)param;
                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                                SelectedItems[ii].BorderBrush = new System.Windows.Media.SolidColorBrush(color);
                        }, param => SelectedItems != null && SelectedItems.Count > 0);
                }

                return borderColor;
            }
        }

        ICommand increaseFontSize;
        public ICommand IncreaseFontSize
        {
            get
            {
                if (increaseFontSize == null)
                {
                    increaseFontSize = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.IncreaseFontSize.Execute(null);
                            OnPropertyChanged("FontSize");
                        }, param => DiagramCommands != null && DiagramCommands.IncreaseFontSize.CanExecute(null));
                }

                return increaseFontSize;
            }
        }

        ICommand decreaseFontSize;
        public ICommand DecreaseFontSize
        {
            get
            {
                if (decreaseFontSize == null)
                {
                    decreaseFontSize = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.DecreaseFontSize.Execute(null);
                            OnPropertyChanged("FontSize");
                        }, param => DiagramCommands != null && DiagramCommands.DecreaseFontSize.CanExecute(null));
                }

                return decreaseFontSize;
            }
        }

        ICommand foregroundColor;
        public ICommand ForegroundColor
        {
            get
            {
                if (foregroundColor == null)
                {
                    foregroundColor = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || !(param is Color))
                                return;

                            var color = (Color)param;
                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                            {
                                var dest = SelectedItems[ii].FindPropertyDescriptor("ForeColor");
                                if (dest != null)
                                    dest.SetValue(SelectedItems[ii], new System.Windows.Media.SolidColorBrush(color));
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 0);
                }

                return foregroundColor;
            }
        }

        ICommand backgroundColor;
        public ICommand BackgroundColor
        {
            get
            {
                if (backgroundColor == null)
                {
                    backgroundColor = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || !(param is Color))
                                return;

                            var color = (Color)param;
                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                                SelectedItems[ii].Background = new System.Windows.Media.SolidColorBrush(color);
                        }, param => SelectedItems != null && SelectedItems.Count > 0);
                }

                return backgroundColor;
            }
        }

        ICommand toggleFontBold;
        public ICommand ToggleFontBold
        {
            get
            {
                if (toggleFontBold == null)
                {
                    toggleFontBold = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.ToggleFontBold.Execute(null);
                            OnPropertyChanged("IsFontBold");
                        }, param => DiagramCommands != null && DiagramCommands.ToggleFontBold.CanExecute(null));
                }

                return toggleFontBold;
            }
        }

        ICommand toggleFontItalic;
        public ICommand ToggleFontItalic
        {
            get
            {
                if (toggleFontItalic == null)
                {
                    toggleFontItalic = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.ToggleFontItalic.Execute(null);
                            OnPropertyChanged("IsFontItalic");
                        }, param => DiagramCommands != null && DiagramCommands.ToggleFontItalic.CanExecute(null));
                }

                return toggleFontItalic;
            }
        }

        ICommand toggleFontUnderline;
        public ICommand ToggleFontUnderline
        {
            get
            {
                if (toggleFontUnderline == null)
                {
                    toggleFontUnderline = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.ToggleFontUnderline.Execute(null);
                            OnPropertyChanged("IsFontUnderline");
                        }, param => DiagramCommands != null && DiagramCommands.ToggleFontUnderline.CanExecute(null));
                }

                return toggleFontUnderline;
            }
        }

        ICommand toggleFontStrikethrough;
        public ICommand ToggleFontStrikethrough
        {
            get
            {
                if (toggleFontStrikethrough == null)
                {
                    toggleFontStrikethrough = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.ToggleFontStrikethrough.Execute(null);
                            OnPropertyChanged("IsFontStrikethrough");
                        }, param => DiagramCommands != null && DiagramCommands.ToggleFontStrikethrough.CanExecute(null));
                }

                return toggleFontStrikethrough;
            }
        }

        ICommand textAlignmentType;
        public ICommand TextAlignmentType
        {
            get
            {
                if (textAlignmentType == null)
                {
                    textAlignmentType = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null)
                                return;

                            System.Windows.TextAlignment textAlignment;
                            switch (param as String)
                            {
                                case "Justify":
                                    textAlignment = System.Windows.TextAlignment.Justify;
                                    break;
                                case "Left":
                                    textAlignment = System.Windows.TextAlignment.Left;
                                    break;
                                case "Right":
                                    textAlignment = System.Windows.TextAlignment.Right;
                                    break;
                                default:
                                    textAlignment = System.Windows.TextAlignment.Center;
                                    break;
                            }

                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                                SelectedItems[ii].TextAlignment = textAlignment;
                            ForceUpdateTextAlignProperties();
                        }, param => SelectedItems != null && SelectedItems.Count > 0);
                }

                return textAlignmentType;
            }
        }

        ICommand alignLeft;
        public ICommand AlignLeft
        {
            get
            {
                if (alignLeft == null)
                {
                    alignLeft = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    selection.SelectedItems[ii].Position = 
                                        new System.Windows.Point(selection.PrimarySelection.Position.X, selection.SelectedItems[ii].Position.Y);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return alignLeft;
            }
        }

        ICommand alignHorizontalCenter;
        public ICommand AlignHorizontalCenter
        {
            get
            {
                if (alignHorizontalCenter == null)
                {
                    alignHorizontalCenter = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    var deltaX = (selection.PrimarySelection.ActualWidth - selection.SelectedItems[ii].ActualWidth) / 2.0;
                                    selection.SelectedItems[ii].Position = 
                                        new System.Windows.Point(selection.PrimarySelection.Position.X + deltaX, selection.SelectedItems[ii].Position.Y);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return alignHorizontalCenter;
            }
        }

        ICommand alignRight;
        public ICommand AlignRight
        {
            get
            {
                if (alignRight == null)
                {
                    alignRight = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    var pointX = selection.PrimarySelection.Position.X + selection.PrimarySelection.ActualWidth - selection.SelectedItems[ii].ActualWidth;
                                    selection.SelectedItems[ii].Position = 
                                        new System.Windows.Point(pointX, selection.SelectedItems[ii].Position.Y);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return alignRight;
            }
        }

        ICommand alignTop;
        public ICommand AlignTop
        {
            get
            {
                if (alignTop == null)
                {
                    alignTop = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            { 
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    selection.SelectedItems[ii].Position = 
                                        new System.Windows.Point(selection.SelectedItems[ii].Position.X, selection.PrimarySelection.Position.Y);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return alignTop;
            }
        }

        ICommand alignVerticalCenter;
        public ICommand AlignVerticalCenter
        {
            get
            {
                if (alignVerticalCenter == null)
                {
                    alignVerticalCenter = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    var deltaY = (selection.PrimarySelection.ActualHeight - selection.SelectedItems[ii].ActualHeight) / 2.0;
                                    selection.SelectedItems[ii].Position = 
                                        new System.Windows.Point(selection.SelectedItems[ii].Position.X, selection.PrimarySelection.Position.Y + deltaY);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return alignVerticalCenter;
            }
        }

        ICommand alignBottom;
        public ICommand AlignBottom
        {
            get
            {
                if (alignBottom == null)
                {
                    alignBottom = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    var pointY = selection.PrimarySelection.Position.Y + selection.PrimarySelection.ActualHeight - selection.SelectedItems[ii].ActualHeight;
                                    selection.SelectedItems[ii].Position = 
                                        new System.Windows.Point(selection.SelectedItems[ii].Position.X, pointY);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return alignBottom;
            }
        }

        ICommand bringToFront;
        public ICommand BringToFront
        {
            get
            {
                if (bringToFront == null)
                {
                    bringToFront = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.BringToFront.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.BringToFront.CanExecute(null));
                }

                return bringToFront;
            }
        }

        ICommand bringForward;
        public ICommand BringForward
        {
            get
            {
                if (bringForward == null)
                {
                    bringForward = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.BringForward.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.BringForward.CanExecute(null));
                }

                return bringForward;
            }
        }

        ICommand sendToBack;
        public ICommand SendToBack
        {
            get
            {
                if (sendToBack == null)
                {
                    sendToBack = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.SendToBack.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.SendToBack.CanExecute(null));
                }

                return sendToBack;
            }
        }

        ICommand sendBackward;
        public ICommand SendBackward
        {
            get
            {
                if (sendBackward == null)
                {
                    sendBackward = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.SendBackward.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.SendBackward.CanExecute(null));
                }

                return sendBackward;
            }
        }

        ICommand layoutEqualSpacingHorizontally;
        public ICommand LayoutEqualSpacingHorizontally
        {
            get
            {
                if (layoutEqualSpacingHorizontally == null)
                {
                    layoutEqualSpacingHorizontally = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                var pointX = selection.PrimarySelection.Position.X + selection.PrimarySelection.ActualWidth;
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    selection.SelectedItems[ii].Position = new Point(pointX + ActiveDocument.Diagram.SnapToItemsDistance, selection.SelectedItems[ii].Position.Y);
                                    pointX = selection.SelectedItems[ii].Position.X + selection.SelectedItems[ii].ActualWidth;
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutEqualSpacingHorizontally;
            }
        }

        ICommand layoutIncreaseHorizontalSpacing;
        public ICommand LayoutIncreaseHorizontalSpacing
        {
            get
            {
                if (layoutIncreaseHorizontalSpacing == null)
                {
                    layoutIncreaseHorizontalSpacing = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                double factor = 0.0;
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    double pointX = selection.SelectedItems[ii].Position.X;
                                    if (selection.SelectedItems[ii].Position.X < selection.PrimarySelection.Position.X)
                                    {
                                        if (factor > 0.0)
                                            factor = 0.0;
                                        factor -= 1.0;
                                        
                                    }
                                    else
                                    {
                                        if (factor < 0.0)
                                            factor = 0.0;
                                        factor += 1.0;
                                    }

                                    pointX += ActiveDocument.Diagram.SnapToItemsDistance * factor;
                                    selection.SelectedItems[ii].Position =
                                            new System.Windows.Point(pointX, selection.SelectedItems[ii].Position.Y);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutIncreaseHorizontalSpacing;
            }
        }

        ICommand layoutDecreaseHorizontalSpacing;
        public ICommand LayoutDecreaseHorizontalSpacing
        {
            get
            {
                if (layoutDecreaseHorizontalSpacing == null)
                {
                    layoutDecreaseHorizontalSpacing = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                double factor = 0.0;
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    double pointX = selection.SelectedItems[ii].Position.X;
                                    if (selection.SelectedItems[ii].Position.X < selection.PrimarySelection.Position.X)
                                    {
                                        if (factor < 0.0)
                                            factor = 0.0;
                                        factor += 1.0;

                                    }
                                    else
                                    {
                                        if (factor > 0.0)
                                            factor = 0.0;
                                        factor -= 1.0;
                                    }

                                    pointX += ActiveDocument.Diagram.SnapToItemsDistance * factor;
                                    selection.SelectedItems[ii].Position =
                                            new System.Windows.Point(pointX, selection.SelectedItems[ii].Position.Y);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutDecreaseHorizontalSpacing;
            }
        }

        ICommand layoutRemoveHorizontalSpacing;
        public ICommand LayoutRemoveHorizontalSpacing
        {
            get
            {
                if (layoutRemoveHorizontalSpacing == null)
                {
                    layoutRemoveHorizontalSpacing = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                var pointX = selection.PrimarySelection.Position.X + selection.PrimarySelection.ActualWidth;
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    selection.SelectedItems[ii].Position = new Point(pointX, selection.SelectedItems[ii].Position.Y);
                                    pointX = selection.SelectedItems[ii].Position.X + selection.SelectedItems[ii].ActualWidth;
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutRemoveHorizontalSpacing;
            }
        }

        ICommand layoutCenterHorizontally;
        public ICommand LayoutCenterHorizontally
        {
            get
            {
                if (layoutCenterHorizontally == null)
                {
                    layoutCenterHorizontally = new RelayCommand(
                        param =>
                        {
                            if (PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                var pointX = (selection.PrimarySelection.Controller.Owner.ActualSize.Width - selection.PrimarySelection.ActualWidth) / 2.0;
                                selection.PrimarySelection.Position = new System.Windows.Point(pointX, selection.PrimarySelection.Position.Y);
                            }
                        }, param => SelectedItems != null && SelectedItems.Count == 1);
                }

                return layoutCenterHorizontally;
            }
        }

        ICommand layoutEqualSpacingVertically;
        public ICommand LayoutEqualSpacingVertically
        {
            get
            {
                if (layoutEqualSpacingVertically == null)
                {
                    layoutEqualSpacingVertically = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                var pointY = selection.PrimarySelection.Position.Y + selection.PrimarySelection.ActualHeight;
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    selection.SelectedItems[ii].Position = new Point(selection.SelectedItems[ii].Position.X, pointY + ActiveDocument.Diagram.SnapToItemsDistance);
                                    pointY = selection.SelectedItems[ii].Position.Y + selection.SelectedItems[ii].ActualHeight;
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutEqualSpacingVertically;
            }
        }

        ICommand layoutCenterVertically;
        public ICommand LayoutCenterVertically
        {
            get
            {
                if (layoutCenterVertically == null)
                {
                    layoutCenterVertically = new RelayCommand(
                        param =>
                        {
                            if (PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                var pointY = (selection.PrimarySelection.Controller.Owner.ActualSize.Height - selection.PrimarySelection.ActualHeight) / 2.0;
                                selection.PrimarySelection.Position = new System.Windows.Point(selection.PrimarySelection.Position.X, pointY);
                            }
                        }, param => SelectedItems != null && SelectedItems.Count == 1);
                }

                return layoutCenterVertically;
            }
        }

        ICommand layoutIncreaseVerticalSpacing;
        public ICommand LayoutIncreaseVerticalSpacing
        {
            get
            {
                if (layoutIncreaseVerticalSpacing == null)
                {
                    layoutIncreaseVerticalSpacing = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                double factor = 0.0;
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    double pointY = selection.SelectedItems[ii].Position.Y;
                                    if (selection.SelectedItems[ii].Position.Y < selection.PrimarySelection.Position.Y)
                                    {
                                        if (factor > 0.0)
                                            factor = 0.0;
                                        factor -= 1.0;

                                    }
                                    else
                                    {
                                        if (factor < 0.0)
                                            factor = 0.0;
                                        factor += 1.0;
                                    }

                                    pointY += ActiveDocument.Diagram.SnapToItemsDistance * factor;
                                    selection.SelectedItems[ii].Position =
                                            new System.Windows.Point(selection.SelectedItems[ii].Position.X, pointY);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutIncreaseVerticalSpacing;
            }
        }

        ICommand layoutDecreaseVerticalSpacing;
        public ICommand LayoutDecreaseVerticalSpacing
        {
            get
            {
                if (layoutDecreaseVerticalSpacing == null)
                {
                    layoutDecreaseVerticalSpacing = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                double factor = 0.0;
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    double pointY = selection.SelectedItems[ii].Position.Y;
                                    if (selection.SelectedItems[ii].Position.Y < selection.PrimarySelection.Position.Y)
                                    {
                                        if (factor < 0.0)
                                            factor = 0.0;
                                        factor += 1.0;

                                    }
                                    else
                                    {
                                        if (factor > 0.0)
                                            factor = 0.0;
                                        factor -= 1.0;
                                    }

                                    pointY += ActiveDocument.Diagram.SnapToItemsDistance * factor;
                                    selection.SelectedItems[ii].Position =
                                            new System.Windows.Point(selection.SelectedItems[ii].Position.X, pointY);
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutDecreaseVerticalSpacing;
            }
        }              

        ICommand layoutRemoveVerticalSpacing;
        public ICommand LayoutRemoveVerticalSpacing
        {
            get
            {
                if (layoutRemoveVerticalSpacing == null)
                {
                    layoutRemoveVerticalSpacing = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            using (var selection = new RecoverSelectionHelper(ActiveDocument.Diagram))
                            {
                                var pointY = selection.PrimarySelection.Position.Y + selection.PrimarySelection.ActualHeight;
                                for (int ii = 0; ii < selection.SelectedItems.Count; ii++)
                                {
                                    if (selection.SelectedItems[ii] == selection.PrimarySelection)
                                        continue;

                                    selection.SelectedItems[ii].Position = new Point(selection.SelectedItems[ii].Position.X, pointY);
                                    pointY = selection.SelectedItems[ii].Position.Y + selection.SelectedItems[ii].ActualHeight;
                                }
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutRemoveVerticalSpacing;
            }
        }

        ICommand layoutSameHeight;
        public ICommand LayoutSameHeight
        {
            get
            {
                if (layoutSameHeight == null)
                {
                    layoutSameHeight = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                            {
                                if (SelectedItems[ii] == PrimarySelection)
                                    continue;

                                SelectedItems[ii].Height = PrimarySelection.ActualHeight;
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutSameHeight;
            }
        }

        ICommand layoutSameSize;
        public ICommand LayoutSameSize
        {
            get
            {
                if (layoutSameSize == null)
                {
                    layoutSameSize = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                            {
                                if (SelectedItems[ii] == PrimarySelection)
                                    continue;

                                SelectedItems[ii].Width = PrimarySelection.ActualWidth;
                                SelectedItems[ii].Height = PrimarySelection.ActualHeight;
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutSameSize;
            }
        }

        ICommand layoutSameWidth;
        public ICommand LayoutSameWidth
        {
            get
            {
                if (layoutSameWidth == null)
                {
                    layoutSameWidth = new RelayCommand(
                        param =>
                        {
                            if (SelectedItems == null || PrimarySelection == null)
                                return;

                            for (int ii = 0; ii < SelectedItems.Count; ii++)
                            {
                                if (SelectedItems[ii] == PrimarySelection)
                                    continue;

                                SelectedItems[ii].Width = PrimarySelection.ActualWidth;
                            }
                        }, param => SelectedItems != null && SelectedItems.Count > 1);
                }

                return layoutSameWidth;
            }
        }

        ICommand moveDown;
        public ICommand MoveDown
        {
            get
            {
                if (moveDown == null)
                {
                    moveDown = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.MoveSelection.Execute(DevExpress.Diagram.Core.Direction.Down);
                        }, param => DiagramCommands != null && DiagramCommands.MoveSelection.CanExecute(DevExpress.Diagram.Core.Direction.Down));
                }

                return moveDown;
            }
        }

        ICommand moveLeft;
        public ICommand MoveLeft
        {
            get
            {
                if (moveLeft == null)
                {
                    moveLeft = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.MoveSelection.Execute(DevExpress.Diagram.Core.Direction.Left);
                        }, param => DiagramCommands != null && DiagramCommands.MoveSelection.CanExecute(DevExpress.Diagram.Core.Direction.Left));
                }

                return moveLeft;
            }
        }

        ICommand moveRight;
        public ICommand MoveRight
        {
            get
            {
                if (moveRight == null)
                {
                    moveRight = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.MoveSelection.Execute(DevExpress.Diagram.Core.Direction.Right);
                        }, param => DiagramCommands != null && DiagramCommands.MoveSelection.CanExecute(DevExpress.Diagram.Core.Direction.Right));
                }

                return moveRight;
            }
        }

        ICommand moveUp;
        public ICommand MoveUp
        {
            get
            {
                if (moveUp == null)
                {
                    moveUp = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.MoveSelection.Execute(DevExpress.Diagram.Core.Direction.Up);
                        }, param => DiagramCommands != null && DiagramCommands.MoveSelection.CanExecute(DevExpress.Diagram.Core.Direction.Up));
                }

                return moveUp;
            }
        }

        ICommand pageSetup;
        public ICommand PageSetup
        {
            get
            {
                if (pageSetup == null)
                {
                    pageSetup = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.SetPageParameters.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.SetPageParameters.CanExecute(null));
                }

                return pageSetup;
            }
        }

        ICommand editWatermark;
        public ICommand EditWatermark
        {
            get
            {
                if (editWatermark == null)
                {
                    editWatermark = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                        }, param => DiagramCommands != null/* && DiagramCommands.SetPageSize.CanExecute(null)*/);
                }

                return editWatermark;
            }
        }

        ICommand selectPrevItem;
        public ICommand SelectPrevItem
        {
            get
            {
                if (selectPrevItem == null)
                {
                    selectPrevItem = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.SelectPrevItem.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.SelectPrevItem.CanExecute(null));
                }

                return selectPrevItem;
            }
        }

        ICommand selectNextItem;
        public ICommand SelectNextItem
        {
            get
            {
                if (selectNextItem == null)
                {
                    selectNextItem = new RelayCommand(
                        param =>
                        {
                            if (DiagramCommands == null)
                                return;

                            DiagramCommands.SelectNextItem.Execute(null);
                        }, param => DiagramCommands != null && DiagramCommands.SelectNextItem.CanExecute(null));
                }

                return selectNextItem;
            }
        }
        #endregion

        #region Properties

        public ReportDesignerDocument ActiveDocument
        {
            get
            {
                return reportEditor.reportDesigner.ActiveDocument;
            }
        }

        public DiagramCommands DiagramCommands
        {
            get
            {
                if (ActiveDocument != null &&
                   ActiveDocument.Diagram != null)
                    return ActiveDocument.Diagram.Commands;

                return null;
            }
        }

        public IList<DiagramItem> SelectedItems
        {
            get
            {
                if (ActiveDocument != null &&
                    ActiveDocument.Diagram != null)
                    return ActiveDocument.Diagram.SelectedItems;

                return null;
            }
        }

        public DiagramItem PrimarySelection
        {
            get
            {
                if (ActiveDocument != null &&
                    ActiveDocument.Diagram != null)
                    return ActiveDocument.Diagram.PrimarySelection;

                return null;
            }
        }

        public object SelectedObject
        {
            get
            {
                if (ActiveDocument != null &&
                    ActiveDocument.ReportModel != null &&
                    ActiveDocument.ReportModel.SelectedModel != null)
                    return ActiveDocument.ReportModel.SelectedModel.XRObject;

                return null;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members
    }
}
