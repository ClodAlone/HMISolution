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
using System.Collections;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Shared;
using System.Diagnostics;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    
    /// </summary>
    public class RichTextRibbon : Control,IDisposable
    {

        #region Private Members

        Ribbon Ribbon;

        RibbonComboBox FontFamilyCombo;
        RibbonComboBox FontSizeCombo;
        RichTextBoxAdv richtextbox;
        RibbonButton RestrictEdit;

        RibbonButton GrowFont; 
        RibbonButton ShrinkFont;

        HighlightColorPicker Highlightcolorpicker;
        ColorPickerPalette FontColorPicker;
        ColorPickerPalette BackgroundColor;
        ColorPickerPalette BorderColor;

        SplitButton FontColorSplitButton;
        SplitButton HighlightColorSplitButton;

        TablePickerUI TablePicker;
        RibbonButton InsertTableButton;
        RibbonButton RTEDocumentButton;
        RibbonButton PrintDocument;
        BackStageCommandButton OpenButton;

        Backstage BackStage;
        SimpleMenuButton BackStageRestrictEditing;
        RibbonButton NewDocumentButton;
        RibbonButton CreateDocumentButton;
        RibbonButton OnlineHelp;
        BackstageTabItem PrintTab;
        Border Preview;

        RoutedCommand m_PrintNext;
        RoutedCommand m_PrintPrevious;
        RoutedCommand m_VisualStyle;

        List<UIElement> pages;

        int m_currentPageNo;

        #endregion

        #region Properties

        public static readonly DependencyProperty FontSizeSourceProperty = DependencyProperty.Register("FontSizeSource", typeof(double[]), typeof(RichTextRibbon),
            new PropertyMetadata(new double[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 26, 28, 36, 48, 72 }));

        public double[] FontSizeSource
        {
            get
            {
                return (double[])GetValue(FontSizeSourceProperty);
            }
            set
            {
                SetValue(FontSizeSourceProperty, value);
            }
        }



        public List<FontFamily> FontFamilySource
        {
            get { return (List<FontFamily>)GetValue(FontFamilySourceProperty); }
            set { SetValue(FontFamilySourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontFamilySource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilySourceProperty =
            DependencyProperty.Register("FontFamilySource", typeof(List<FontFamily>), typeof(RichTextRibbon), new PropertyMetadata(null));



        public RichTextBoxAdv RichTextEdit
        {
            get
            {
                return richtextbox;
            }
            set
            {
                richtextbox = value;
            }
        }

        public RoutedCommand PrintPreviousCommand
        {
           get
            {
                if (m_PrintNext == null)
                {
                    m_PrintNext= new RoutedCommand(p => PreviousExecute(), p => PreviousCanExecute());
                }
                return m_PrintNext;
            }
        }

        public RoutedCommand PrintNextCommand
        {
           get
            {
                if (m_PrintPrevious == null)
                {
                    m_PrintPrevious = new RoutedCommand(p => NextExecute(), p => NextCanExecute());
                }
                return m_PrintPrevious;
            }
        }

        public RoutedCommand VisualStyleCommand
        {
            get
            {
                if (m_VisualStyle == null)
                {
                    m_VisualStyle = new RoutedCommand(VisualStyleExecute,VisualStyleCanExecute);
                }
                return m_VisualStyle;
            }
        }

        #endregion

        static RichTextRibbon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextRibbon), new FrameworkPropertyMetadata(typeof(RichTextRibbon)));
        }

        public RichTextRibbon()
        {
            pages=new List<UIElement>();
            this.DataContextChanged += (sender, e) =>
                {
                    RichTextBoxAdv richtext = e.NewValue as RichTextBoxAdv;
                    if (richtext != null)
                    {
                        RichTextEdit = richtext;
                        //m_PrintNext.UpdateCanExecute();
                        //m_PrintPrevious.UpdateCanExecute();
                        if(m_VisualStyle != null)
                        m_VisualStyle.UpdateCanExecute();
                    }
                };

            Loaded += new RoutedEventHandler(OnLoaded);
            this.Unloaded += (sender, e) =>
                {
                    m_currentPageNo = 0;
                };

            FontFamilySource = new List<FontFamily>();
            AddFontFamily();

            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(RichTextRibbon));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window window = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            if (window != null)
            {
                window.CommandBindings.Add(new CommandBinding(RichTextBoxAdv.Save, SaveExecute));
                window.CommandBindings.Add(new CommandBinding(RichTextBoxAdv.SaveAs, SaveAsExecute));
                window.CommandBindings.Add(new CommandBinding(RichTextBoxAdv.Print, PrintExecute));
                window.CommandBindings.Add(new CommandBinding(RichTextBoxAdv.Open, OpenExecute));
                window.CommandBindings.Add(new CommandBinding(RichTextBoxAdv.New, NewExecute));
            }
            if (RichTextEdit != null)
            {
                RichTextEdit.Focus();
            }
        }

        private void SaveExecute(object sender,ExecutedRoutedEventArgs e)
        {
            if (RichTextEdit != null)
            {
                RichTextEdit.SaveDocument(string.Empty);
                RichTextEdit.Focus();
            }
        }

        private void SaveAsExecute(object sender, ExecutedRoutedEventArgs e)
        {
            string extension = string.Empty;
            if (e.Parameter == null)
                extension = ".docx";
            else
                extension = e.Parameter.ToString();
            if (RichTextEdit != null)
            {
                RichTextEdit.SaveDocument(extension);
            }
        }

        private void PrintExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (RichTextEdit != null)
            {
                RichTextEdit.PrintDocument();
                RichTextEdit.Focus();
            }
        }

        private void NewExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (RichTextEdit != null)
            {
                RichTextEdit.CreateEmptyDocument();
                RichTextEdit.Focus();
            }
        }

        private void OpenExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (RichTextEdit != null)
            {
                RichTextEdit.OpenDocument();
                RichTextEdit.Focus();
            }
        }

        private void AddFontFamily()
        {
            foreach (FontFamily fontfamily in Fonts.SystemFontFamilies)
            {
                FontFamilySource.Add(fontfamily);
            }
        }

        private void PreparePages()
        {
            if (RichTextEdit != null)
            {
                foreach (PageAdv page in RichTextEdit.Viewer.Pages)
                {
                    VisualBrush brush = new VisualBrush(page);
                    Border border = new Border();
                    border.Background = brush;
                    border.Width = page.Width;
                    border.Height = page.Height;
                    pages.Add(border);
                }
            }
        }

        public void PreviousExecute()
        {
            m_currentPageNo -= 1;
            PageAdv page = RichTextEdit.Viewer.SkipPage(m_currentPageNo);
            ShowPreview(page);
        }

        public bool PreviousCanExecute()
        {
            if(RichTextEdit !=null)
                return RichTextEdit.Viewer.Pages.Count > 0 && m_currentPageNo != 0;
            return false;
        }

        public void NextExecute()
        {
            m_currentPageNo += 1;
            PageAdv page = RichTextEdit.Viewer.SkipPage(m_currentPageNo);
            ShowPreview(page);
        }

        public bool NextCanExecute()
        {
            if(RichTextEdit !=null)
                return RichTextEdit.Viewer.Pages.Count > 0 && m_currentPageNo != RichTextEdit.Viewer.Pages.Count - 1;
            return false;
        }

        internal bool VisualStyleCanExecute(object parameter)
        {
            return true;
        }

        internal void VisualStyleExecute(object parameter)
        {
            string str = parameter.ToString();
            DependencyObject root = FindLogicalRoot(this);
            if (root != null)
            {
                SkinStorage.SetVisualStyle(root, str);
            }
        }

        private DependencyObject FindLogicalRoot(DependencyObject obj)
        {
            if (obj == null)
                return null;
            else
            {
                var parent = LogicalTreeHelper.GetParent(obj);
                return parent != null ? FindLogicalRoot(parent) : obj;
            }
        }

        public override void OnApplyTemplate()
        {
            Ribbon = (Ribbon)GetTemplateChild("PART_Ribbon");
            FontFamilyCombo = (RibbonComboBox)GetTemplateChild("PART_FontFamilyCombo");
            FontSizeCombo = (RibbonComboBox)GetTemplateChild("PART_FontSizeCombo");
            GrowFont = (RibbonButton)GetTemplateChild("PART_GrowFont");
            ShrinkFont = (RibbonButton)GetTemplateChild("PART_ShrinkFont");
            Highlightcolorpicker = (HighlightColorPicker)GetTemplateChild("PART_Highlightcolorpicker");
            FontColorPicker = (ColorPickerPalette)GetTemplateChild("PART_Colorpicker");
            TablePicker = (TablePickerUI)GetTemplateChild("PART_TablePicker");
            InsertTableButton = (RibbonButton)GetTemplateChild("PART_InsertTable");
            BackgroundColor = (ColorPickerPalette)GetTemplateChild("PART_CellBackgroundColor");
            BorderColor = (ColorPickerPalette)GetTemplateChild("PART_TableBorderColor");
            RestrictEdit = (RibbonButton)GetTemplateChild("PART_DisableEdit");
            FontColorSplitButton = (SplitButton)GetTemplateChild("PART_FontColorSplitButton");
            HighlightColorSplitButton = (SplitButton)GetTemplateChild("PART_HighlightColorSplitButton");
            RTEDocumentButton = (RibbonButton)GetTemplateChild("PART_RTEDocument");
            BackStage = (Backstage)GetTemplateChild("PART_BackStage");
            BackStageRestrictEditing = (SimpleMenuButton)GetTemplateChild("PART_BackStageRestrictEditing");
            NewDocumentButton = (RibbonButton)GetTemplateChild("PART_NewDocument_BackStage");
            CreateDocumentButton = (RibbonButton)GetTemplateChild("PART_CreateDocument");
            OnlineHelp = (RibbonButton)GetTemplateChild("PART_OnlineHelp");
            PrintTab = (BackstageTabItem)GetTemplateChild("PART_PrintTab");
            Preview = (Border)GetTemplateChild("PART_Preview");
            PrintDocument = (RibbonButton)GetTemplateChild("PART_PrintDocument");
            OpenButton = (BackStageCommandButton)GetTemplateChild("PART_Open");
            HookingEvents();
            
        }

        private void HookingEvents()
        {
            if (FontFamilyCombo != null)
            {
                FontFamilyCombo.DropDownClosed += (sender, e) =>
                {
                    if (RichTextEdit != null)
                    {
                        RichTextBoxAdv.ChangeFontFamily.Execute(FontFamilyCombo.SelectedItem,null);
                    }
                };
            }
            if (FontSizeCombo != null)
            {
                FontSizeCombo.DropDownClosed += (sender, e) =>
                {
                    if (RichTextEdit != null)
                    {
                        RichTextBoxAdv.ChangeFontSize.Execute(FontSizeCombo.SelectedItem,null);
                    }
                };
            }
            if (GrowFont != null)
            {
                GrowFont.Click += (sender, e) =>
                {
                    if (RichTextEdit != null)
                    {
                        if (FontSizeCombo.SelectedIndex < FontSizeCombo.Items.Count - 1)
                        {
                            FontSizeCombo.SelectedIndex += 1;
                            RichTextEdit.Selection.ChangeFontSize(int.Parse((FontSizeCombo.SelectedItem).ToString()));
                        }
                        else
                        {
                            RichTextEdit.Selection.ChangeFontSize(RichTextEdit.CurrentInlineStyle.FontSize + 10);
                        }
                    }
                };
            }

            if (ShrinkFont != null)
                ShrinkFont.Click += (sender, args) =>
                {
                    if (RichTextEdit != null)
                    {
                        if (FontSizeCombo.SelectedIndex > 0)
                        {
                            FontSizeCombo.SelectedIndex -= 1;
                            RichTextEdit.Selection.ChangeFontSize(int.Parse((FontSizeCombo.SelectedItem).ToString()));
                        }
                        else
                        {
                            if (RichTextEdit.CurrentInlineStyle.FontSize > 1)
                                RichTextEdit.Selection.ChangeFontSize(RichTextEdit.CurrentInlineStyle.FontSize - 1);
                        }
                    }
                };

            if (Highlightcolorpicker != null)
                Highlightcolorpicker.ColorClicked += (sender, args) =>
                {
                    if (Highlightcolorpicker != null && RichTextEdit != null)
                    {
                        RichTextEdit.Selection.ChangeHighlightColor(Highlightcolorpicker.Color);
                        SplitButton splitbutton = (SplitButton)Highlightcolorpicker.Parent;
                        splitbutton.IsDropDownOpen = false;
                    }
                };

            if (FontColorPicker != null)
            {
                FontColorPicker.ColorChanged += (sender, args) =>
                {
                    if (RichTextEdit != null)
                    {
                        RichTextEdit.Selection.ChangeForeground(FontColorPicker.Color);
                        SplitButton splitbutton = (SplitButton)FontColorPicker.Parent;
                        splitbutton.IsDropDownOpen = false;
                    }
                };
            }

            if (TablePicker != null)
            {
                TablePicker.Click += (sender, args) =>
                {
                    if (RichTextEdit != null)
                    {
                        RichTextEdit.InsertTableInBlocks(TablePicker.SelectedCell.Row + 1 , TablePicker.SelectedCell.Column + 1);

                        object obj = TablePicker.Parent;

                        while (!(obj is DropDownButton))
                        {
                            obj = (obj as FrameworkElement).Parent;
                        }
                        DropDownButton dropdown = obj as DropDownButton;
                        dropdown.IsDropDownOpen = false;
                    }
                };
            }

            if (PrintDocument != null)
            {
                PrintDocument.Click += (sender, e) =>
                    {
                        Ribbon.BackStageButton.IsOpen = false;
                    };
            }

            if (OpenButton != null)
            {
                OpenButton.Click += (sender, e) =>
                    {
                        Ribbon.BackStageButton.IsOpen = false;
                    };
            }

            if (InsertTableButton != null)
            {
                InsertTableButton.Click += (sender, args) =>
                {
                    object obj = InsertTableButton.Parent;

                    while (!(obj is DropDownButton))
                    {
                        obj = (obj as FrameworkElement).Parent;
                    }
                    DropDownButton dropdown = obj as DropDownButton;
                    dropdown.IsDropDownOpen = false;
                };
            }

            if (BackgroundColor != null)
            {
                BackgroundColor.ColorChanged += (sender, args) =>
                {
                    if (RichTextEdit != null)
                    {
                        //RichTextEdit.ChangeTableCellStyleCommand.Execute((Color)args.NewValue);
                    }
                };
            }
            if (BorderColor != null)
            {
                BorderColor.ColorChanged += (sender, args) =>
                {
                    if (RichTextEdit != null)
                    {
                        //RichTextEdit.ChangeTableBorderColorCommand.Execute((Color)args.NewValue);
                    }
                };
            }

            if (RestrictEdit != null)
            {
                RestrictEdit.IsSelectedChanged += (sender, e) =>
                {
                    if (RichTextEdit != null)
                    {
                        RichTextEdit.IsReadOnly = (bool)e.NewValue;

                        IEnumerable tabs = Ribbon.Items;
                        string visual = SkinStorage.GetVisualStyle(this);
                        foreach (var tab in tabs)
                        {
                            RibbonTab ribbontab = tab as RibbonTab;
                            if (ribbontab != null)
                            {
                                foreach (var bar in ribbontab.Items)
                                {
                                    RibbonBar ribbonBar = bar as RibbonBar;
                                    if (ribbonBar != null && ribbonBar.Header != "Page SetUp" 
                                        && ribbonBar.Header != "Color Scheme")
                                    {
                                        if ((bool)e.NewValue)
                                        {
                                            ribbonBar.IsEnabled = false;
                                            if (visual == "Office2010Blue" || visual == "Office2010Black" || visual == "Office2010Silver")
                                            {
                                                ribbonBar.Effect = new DisableEffect();
                                            }
                                        }
                                        else
                                        {
                                            ribbonBar.Effect = null;
                                            ribbonBar.IsEnabled = true;
                                        }
                                    }
                                }
                            }
                        }

                        RibbonBar parent = (RibbonBar)VisualUtils.FindAncestor(RestrictEdit, typeof(RibbonBar));
                        if (parent != null)
                        {
                            if ((bool)e.NewValue)
                            {
                                parent.IsEnabled = true;
                                parent.Effect = null;
                            }
                        }
                    }
                };
            }

            if (BackStageRestrictEditing != null)
            {
                BackStageRestrictEditing.Click += (sender, e) =>
                {
                    if (RichTextEdit != null)
                    {
                        RichTextEdit.IsReadOnly = true;

                        IEnumerable tabs = Ribbon.Items;

                        foreach (var tab in tabs)
                        {
                            RibbonTab ribbontab = tab as RibbonTab;
                            if (ribbontab != null)
                            {
                                foreach (var bar in ribbontab.Items)
                                {
                                    RibbonBar ribbonBar = bar as RibbonBar;
                                    if (ribbonBar != null)
                                    {
                                        ribbonBar.IsEnabled = false;
                                    }
                                }
                            }
                        }

                        RibbonBar parent = (RibbonBar)VisualUtils.FindAncestor(RestrictEdit, typeof(RibbonBar));
                        if (parent != null)
                            parent.IsEnabled = true;
                    }
                };
            }

            if (NewDocumentButton != null)
            {
                NewDocumentButton.Click += (sender, e) =>
                {
                    if (Ribbon != null)
                    {
                        Ribbon.BackStageButton.IsOpen = false;
                        RichTextEdit.DocumentTitle = "Untitled";
                    }
                };
            }

            if (CreateDocumentButton != null)
            {
                CreateDocumentButton.Click += (sender, e) =>
                {
                    if (Ribbon != null)
                    {
                        Ribbon.BackStageButton.IsOpen = false;
                        RichTextEdit.DocumentTitle = "Untitled";
                    }

                };
            }

            if (FontColorSplitButton != null)
                FontColorSplitButton.Click += (sender, e) =>
                {
                    if (FontColorPicker != null && RichTextEdit != null)
                    {
                        RichTextEdit.Selection.ChangeForeground(FontColorPicker.Color);
                    }
                };

            if (HighlightColorSplitButton != null)
                HighlightColorSplitButton.Click += (sender, e) =>
                {
                    if (Highlightcolorpicker != null && RichTextEdit != null)
                    {
                        RichTextEdit.Selection.ChangeHighlightColor(Highlightcolorpicker.Color);
                    }
                };

            if (RTEDocumentButton != null)
                RTEDocumentButton.Click += (sender, e) =>
                {
                    Ribbon.BackStageButton.IsOpen = false;
                };

            if (OnlineHelp != null)
            {
                OnlineHelp.Click += (sender, e) =>
                {
                    Process.Start("http://help.syncfusion.com/ug_93/Common/Common/index.htm");
                };
            }

            if (Ribbon != null)
            {
                //Ribbon.BackStage.SelectionChanged += (sender, e) =>
                //    {
                //        bool flag=false;

                //        if (e.AddedItems.Count > 0)
                //        {
                //            BackstageTabItem tabitem = null;
                //            object obj = e.AddedItems[0];
                //            tabitem = obj is BackstageTabItem ? obj as BackstageTabItem : null;
                //            if (tabitem !=null && tabitem == PrintTab)
                //            {
                //                if (RichTextEdit != null && PrintTab.IsSelected)
                //                {
                //                    ShowPreview(RichTextEdit.Viewer.SkipPage(m_currentPageNo));
                //                    flag=true;
                //                }
                //            }
                //            else
                //                if (flag)
                //                {
                //                    flag = false;
                //                }
                //        }
                //    };

                Ribbon.BackStageClosed += (sender, e) =>
                    {
                        RichTextEdit.Viewer.SetVisibleLinesToPage();
                    };

                Ribbon.BackStageOpening += (sender2, e2) =>
                {
                    //ShowPreview(RichTextEdit.Viewer.SkipPage(m_currentPageNo));
                };

                Button btn = GetTemplateChild("PART_btnRTE") as Button;
                if (btn != null)
                {
                    btn.Click += (sender, e) =>
                        {
                            Ribbon.BackStageButton.IsOpen=false;
                        };
                }
            }

        }

        private void ShowPreview(PageAdv page)
        {
            page.Background = RichTextEdit.Background;
            if (Preview != null)
            {
                Preview.Background = new VisualBrush(page);
            }
            //m_PrintPrevious.UpdateCanExecute();
            //m_PrintNext.UpdateCanExecute();
        }


        public void Dispose()
        {
            if (Ribbon != null)
            {
                Ribbon.Dispose();
            }
        }
    }

    public class RoutedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        readonly Predicate<Object> _canExecute = null;
        readonly Action<Object> _executeAction = null;


        public RoutedCommand(Action<object> executeAction, Predicate<Object> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        public RoutedCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
            _executeAction = executeAction;
        }


        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }


        public void Execute(object parameter)
        {
            if (_executeAction != null)
                _executeAction(parameter);
            UpdateCanExecute();
        }

    }
}
