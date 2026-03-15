#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Data;
using System.Globalization;
using System.IO;
using Syncfusion.Windows.Controls.Theming;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using Syncfusion.Windows.Shared;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public class RichTextRibbon : Control,ISkinStylePropagator,IDisposable
    {
        
        private RichTextBoxAdv richtextbox;
        public ColorPickerPalette Colorpickerbutton;
        internal RibbonButton RestrictEdit;
        internal RibbonComboBox FontFamilyCombo;
        internal RibbonComboBox FontSizeCombo;

        internal RibbonButton GrowFont;
        internal RibbonButton ShrinkFont;

        internal HighlightColorPicker Highlightcolorpicker;
        internal ColorPickerPalette FontColorPicker;
        internal ColorPickerPalette BackgroundColor;
        internal ColorPickerPalette BorderColor;

        internal RibbonSplitButton FontColorSplitButton;
        internal RibbonSplitButton HighlightColorSplitButton;

        TablePickerUI TablePicker;
        RibbonButton InsertTableButton;
        RibbonButton RTEDocumentButton;
        Backstage BackStage;



        public Ribbon Ribbon
        {
            get { return (Ribbon)GetValue(RibbonProperty); }
            set { SetValue(RibbonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ribbon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RibbonProperty =
            DependencyProperty.Register("Ribbon", typeof(Ribbon), typeof(RichTextRibbon), null);



        internal object MyDataContext
        {
            get { return (object)GetValue(MyDataContextProperty); }
            set { SetValue(MyDataContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyDataContext.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MyDataContextProperty =
            DependencyProperty.Register("MyDataContext", typeof(object), typeof(RichTextRibbon), new PropertyMetadata(null, new PropertyChangedCallback(OnMyDataContextChanged)));


        public IEnumerable FontFamilySource
        {
            get { return (IEnumerable)GetValue(FontFamilySourceProperty); }
            set { SetValue(FontFamilySourceProperty, value); }
        }

        public static readonly DependencyProperty FontFamilySourceProperty =
            DependencyProperty.Register("FontFamilySource", typeof(IEnumerable), typeof(RichTextRibbon), new PropertyMetadata(null));


        public static readonly DependencyProperty FontSizeSourceProperty = DependencyProperty.Register("FontSizeSource", typeof(double[]), typeof(RichTextRibbon),
            new PropertyMetadata(new double[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 26, 28, 36, 48, 72 }));

        public double BackStageHeight
        {
            get { return (double)GetValue(BackStageHeightProperty); }
            set { SetValue(BackStageHeightProperty, value); }
        }

        public double BackStageWidth
        {
            get { return (double)GetValue(BackStageWidthProperty); }
            set { SetValue(BackStageWidthProperty, value); }
        }

        public static readonly DependencyProperty BackStageHeightProperty =
            DependencyProperty.Register("BackStageHeight", typeof(double), typeof(RichTextRibbon), new PropertyMetadata(null));

        public static readonly DependencyProperty BackStageWidthProperty =
            DependencyProperty.Register("BackStageWidth", typeof(double), typeof(RichTextRibbon), new PropertyMetadata(null));

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



        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(RichTextRibbon), new PropertyMetadata(new SolidColorBrush(Colors.Blue),new PropertyChangedCallback(OnBackStageColorChanged)));

        
        private static void OnBackStageColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            RichTextRibbon ribbon = obj as RichTextRibbon;
            if (ribbon != null && ribbon.Ribbon!=null)
            {
                ribbon.Ribbon.BackStageColor = (Brush)e.NewValue;
            }
        }


        private Windows.Controls.Theming.VisualStyle VisualStyle
        {
            get;
            set;
        }

        public RichTextBoxAdv RichTextEdit
        {
            get
            {
                return richtextbox;
            }
            set
            {
                richtextbox = value;
                RichTextEdit.Loaded+=new RoutedEventHandler(RichTextEdit_Loaded);
            }
        }

        public RichTextRibbon()
        {
            DefaultStyleKey = typeof(RichTextRibbon);
            TriggerDataContextChanged();

            FontFamilySource = new ObservableCollection<FontFamily>
                                   {
                                       new FontFamily("Arial"),
                                       new FontFamily("Arial Black"),
                                       new FontFamily("Calibri(Body)"),
                                       new FontFamily("Comic Sans MS"),
                                       new FontFamily("Courier New"),
                                       new FontFamily("Georgia"),
                                       new FontFamily("Lucida Sans Unicode"),
                                       new FontFamily("Portable User Interface"),
                                       new FontFamily("Times New Roman"),
                                       new FontFamily("Trebuchet MS"),
                                       new FontFamily("Verdana"),
                                       new FontFamily("Webdings")
                                   };

        }
        
        internal void TriggerDataContextChanged()
        {
            Binding bind = new Binding("DataContext");
            bind.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            bind.Mode = BindingMode.TwoWay;
            this.SetBinding(RichTextRibbon.MyDataContextProperty, bind);
        }

        public override void OnApplyTemplate()
        {
            FontFamilyCombo = (RibbonComboBox)GetTemplateChild("PART_FontFamilyCombo");
            Ribbon= (Ribbon)GetTemplateChild("PART_Ribbon");
            FontSizeCombo=(RibbonComboBox)GetTemplateChild("PART_FontSizeCombo");
            GrowFont=(RibbonButton)GetTemplateChild("PART_GrowFont");
            ShrinkFont=(RibbonButton)GetTemplateChild("PART_ShrinkFont");
            Highlightcolorpicker = (HighlightColorPicker)GetTemplateChild("PART_Highlightcolorpicker");
            FontColorPicker = (ColorPickerPalette)GetTemplateChild("PART_Colorpicker");
            TablePicker = (TablePickerUI)GetTemplateChild("PART_TablePicker");
            InsertTableButton = (RibbonButton)GetTemplateChild("PART_InsertTable");
            BackgroundColor = (ColorPickerPalette)GetTemplateChild("PART_CellBackgroundColor");
            BorderColor = (ColorPickerPalette)GetTemplateChild("PART_TableBorderColor");
            RestrictEdit = (RibbonButton)GetTemplateChild("PART_DisableEdit");
            FontColorSplitButton = (RibbonSplitButton)GetTemplateChild("PART_FontColorSplitButton");
            HighlightColorSplitButton = (RibbonSplitButton)GetTemplateChild("PART_HighlightColorSplitButton");
            RTEDocumentButton = (RibbonButton)GetTemplateChild("PART_RTEDocument");
            BackStage = (Backstage)GetTemplateChild("PART_BackStage");

            AddBackStageTabItem();

            if (FontFamilyCombo != null)
                FontFamilyCombo.DropDownClosed += (sender, args) =>
                    {
                        if (RichTextEdit != null)
                        {
                            RichTextEdit.ChangeFontFamilyCommand.Execute(FontFamilyCombo.SelectedItem);
                        }
                    };

            if (FontSizeCombo != null)
                FontSizeCombo.DropDownClosed += (sender, args) =>
                    {
                        if (RichTextEdit != null)
                        {
                            double fontsize;
                            if (double.TryParse(FontSizeCombo.SelectedValue.ToString(), out fontsize))
                            {
                                RichTextEdit.ChangeFontSizeCommand.Execute(fontsize);
                            }
                        }
                    };

            if (GrowFont != null)
                GrowFont.Click += (sender, args) =>
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
                            RichTextEdit.ChangeHighlightColorCommand.Execute(Highlightcolorpicker.Color);
                            RibbonSplitButton splitbutton = (RibbonSplitButton)Highlightcolorpicker.Parent;
                            splitbutton.IsDropDownOpen = false;
                        }
                    };

            if (FontColorPicker != null)
            {
                FontColorPicker.ColorChanged += (sender, args) =>
                    {
                        if (RichTextEdit != null)
                        {
                            RichTextEdit.ChangeTextColorCommand.Execute(FontColorPicker.Color);
                            RibbonSplitButton splitbutton = (RibbonSplitButton)FontColorPicker.Parent;
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
                            RichTextEdit.InsertTableCommand.Execute((TablePicker.SelectedCell.Row + 1).ToString() + "," + (TablePicker.SelectedCell.Column + 1).ToString());

                            object obj = TablePicker.Parent;

                            while (!(obj is RibbonDropDownButton))
                            {
                                obj = (obj as FrameworkElement).Parent;
                            }
                            RibbonDropDownButton dropdown = obj as RibbonDropDownButton;
                            dropdown.IsDropDownOpen = false;
                        }
                    };
            }

            if (InsertTableButton != null)
            {
                InsertTableButton.Click += (sender, args) =>
                    {
                        object obj = InsertTableButton.Parent;

                        while (!(obj is RibbonDropDownButton))
                        {
                            obj = (obj as FrameworkElement).Parent;
                        }
                        RibbonDropDownButton dropdown = obj as RibbonDropDownButton;
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
                RestrictEdit.Click += (sender, e) =>
                    {
                        if (RichTextEdit != null)
                        {
                            RichTextEdit.IsReadOnly = RestrictEdit.IsChecked;
                        }
                    };
            }

            if(FontColorSplitButton !=null)
                FontColorSplitButton.MouseLeftButtonDown += (sender, e) =>
                    {
                        if (FontColorPicker != null && RichTextEdit !=null)
                        {
                            RichTextEdit.ChangeTextColorCommand.Execute(FontColorPicker.Color);
                        }
                    };

            if(HighlightColorSplitButton !=null)
                HighlightColorSplitButton.MouseLeftButtonDown += (sender, e) =>
                    {
                        if (Highlightcolorpicker != null && RichTextEdit !=null)
                        {
                            RichTextEdit.ChangeHighlightColorCommand.Execute(Highlightcolorpicker.Color);
                        }
                    };

            if (RTEDocumentButton != null)
                RTEDocumentButton.Click += (sender, e) =>
                    {
                        Ribbon.BackStageButton.IsOpen = false;
                    };

            
        }

        void RichTextEdit_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent is FrameworkElement)
            {
                double height = (this.Parent as FrameworkElement).ActualHeight;
                BackStageHeight = height - 49 > 0 ? height - 49 : 0;
                BackStageWidth = (this.Parent as FrameworkElement).ActualWidth;
            }
        }

        protected static void OnMyDataContextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            RichTextRibbon rich = (RichTextRibbon)dependencyObject;
            rich.OnMyDataContextChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMyDataContextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is RichTextBoxAdv)
            {
                RichTextEdit = (RichTextBoxAdv)e.NewValue;
            }
        }

        public void OnStyleChanged(Windows.Controls.Theming.VisualStyle visualStyle)
        {
            VisualStyle = visualStyle;
            SkinManager.SetVisualStyle(Ribbon, visualStyle);
            SkinManager.SetVisualStyle(FontColorPicker, visualStyle);
            SkinManager.SetVisualStyle(Highlightcolorpicker, visualStyle);
            SkinManager.SetVisualStyle(BackgroundColor, visualStyle);
            SkinManager.SetVisualStyle(BorderColor, visualStyle);
        }

        internal void AddBackStageTabItem()
        {
            if (BackStage != null)
            {

                string tabitem1 = @"<Border xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:ribbon=""clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Ribbon.Silverlight"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                    <Grid Background=""#FFFEFEFE""> 
                                        <Grid.ColumnDefinitions>
                                            <ColumnDefinition Width=""*""/>
                                            <ColumnDefinition Width=""Auto""/>
                                            <ColumnDefinition Width=""*""/>
                                        </Grid.ColumnDefinitions>
                                        <Grid.Resources>
                                            <BitmapImage x:Key=""NewDocumentIcon"" UriSource=""/Syncfusion.RichTextRibbon.Silverlight;component/OfficeUI/New.png""/>
                                            <BitmapImage x:Key=""HelpIcon"" UriSource=""/Syncfusion.RichTextRibbon.Silverlight;component/OfficeUI/BackStageHelp.png""/>
                                        </Grid.Resources>
                                        <StackPanel Margin=""25,20,0,0"">
                                            <Grid >
                                                <TextBlock Text=""Available Template"" FontSize=""14"" FontWeight=""Bold"" FontFamily=""Segoe UI"" Grid.Row=""0"" /> 
                                            </Grid>
                                            <Rectangle Stroke=""#FFBCBCBC"" Height=""1"" StrokeDashArray=""4,3"" Margin=""0,15,0,0""/>
                                            <Grid Margin=""0,10""> 
                                                <Grid.ColumnDefinitions>
                                                    <ColumnDefinition Width=""Auto""/>
                                                    <ColumnDefinition Width=""Auto""/>
                                                </Grid.ColumnDefinitions>
                                                 <ribbon:RibbonButton SizeMode=""Large"" Margin=""10"" IsMultiLine=""True"" Grid.Column=""0"" Label=""Blank Document"" Width=""87"" 
                                                                             Height=""80"" LargeIcon=""{StaticResource NewDocumentIcon}""
                                                                             HorizontalAlignment=""Center"" VerticalAlignment=""Center""
                                                                             Command=""{Binding NewDocumentCommand}"">
                                                     <ribbon:RibbonButton.Background>
                                                        <LinearGradientBrush EndPoint=""0.5,1"" StartPoint=""0.5,0"">
                                                            <GradientStop Color=""White"" Offset=""0.116""/>
                                                            <GradientStop Color=""#FFF3F5F6"" Offset=""1""/>
                                                        </LinearGradientBrush>
                                                    </ribbon:RibbonButton.Background>
                                                    <ribbon:RibbonButton.BorderBrush>
                                                        <LinearGradientBrush EndPoint=""0.5,1"" StartPoint=""0.5,0"">
                                                            <GradientStop Color=""#FFDDDEE0"" Offset=""0""/>
                                                            <GradientStop Color=""#FFA1A2A4"" Offset=""1""/>
                                                        </LinearGradientBrush>
                                                    </ribbon:RibbonButton.BorderBrush>
                                                    <ribbon:RibbonButton.Effect>
                                                        <DropShadowEffect ShadowDepth=""2"" Opacity=""0.21"" BlurRadius=""3"" Direction=""310""/>
                                                    </ribbon:RibbonButton.Effect>
                                                </ribbon:RibbonButton>

                                                <StackPanel VerticalAlignment=""Top"" Grid.Column=""1""  Margin=""3"">
                                                    <TextBlock Text=""Blank Document"" FontWeight=""Bold"" Margin=""3""/>
                                                    <TextBlock Text=""Create an empty Document."" TextWrapping=""Wrap"" Margin=""3""/>
                                                </StackPanel>
                                            </Grid>
                                            <Rectangle VerticalAlignment=""Top"" Stroke=""#FFBCBCBC"" Height=""1"" StrokeDashArray=""4,3"" />
                                        </StackPanel>
                                        <Border BorderThickness=""0.4"" BorderBrush=""#FFBCBCBC"" Grid.Column=""1"" HorizontalAlignment=""Left"" />
                                    </Grid>
                                  </Border>";

                Border border1 = (Border)XamlReader.Load(tabitem1);
                BackstageTabItem tab = new BackstageTabItem();
                tab.Header = "New";
                tab.Content = border1;
                BackStage.Items.Add(tab);


                string tabitem2 = @"<Border xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:ribbon=""clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Ribbon.Silverlight"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                    <Grid Background=""#FFFEFEFE""> 
                                        <Grid.Resources>
                                            <BitmapImage x:Key=""PrintIcon"" UriSource=""/Syncfusion.RichTextRibbon.Silverlight;component/OfficeUI/Print32.png"" />
                                        </Grid.Resources>
                                        <Grid.ColumnDefinitions>
                                            <ColumnDefinition Width=""*""/>
                                            <ColumnDefinition Width=""Auto""/>
                                            <ColumnDefinition Width=""*""/>
                                        </Grid.ColumnDefinitions>
                                        <StackPanel Margin=""25,20,0,0"">
                                            <Grid >
                                                <TextBlock Text=""Print"" FontSize=""14"" FontWeight=""Bold"" FontFamily=""Segoe UI"" Grid.Row=""0"" /> 
                                            </Grid>
                                            <Rectangle Stroke=""#FFBCBCBC"" Height=""1"" StrokeDashArray=""4,3"" Margin=""0,15,0,0""/>
                                            <Grid Margin=""0,10""> 
                                                <Grid.ColumnDefinitions>
                                                    <ColumnDefinition Width=""Auto""/>
                                                    <ColumnDefinition Width=""Auto""/>
                                                </Grid.ColumnDefinitions>
                                                 <ribbon:RibbonButton SizeMode=""Large"" Margin=""10"" IsMultiLine=""True"" Grid.Column=""0"" Label=""Print Document"" Width=""87"" 
                                                                             Height=""80"" LargeIcon=""{StaticResource PrintIcon}""
                                                                             HorizontalAlignment=""Center"" VerticalAlignment=""Center""
                                                                             Command=""{Binding PrintDocumentCommand}"">
                                                     <ribbon:RibbonButton.Background>
                                                        <LinearGradientBrush EndPoint=""0.5,1"" StartPoint=""0.5,0"">
                                                            <GradientStop Color=""White"" Offset=""0.116""/>
                                                            <GradientStop Color=""#FFF3F5F6"" Offset=""1""/>
                                                        </LinearGradientBrush>
                                                    </ribbon:RibbonButton.Background>
                                                    <ribbon:RibbonButton.BorderBrush>
                                                        <LinearGradientBrush EndPoint=""0.5,1"" StartPoint=""0.5,0"">
                                                            <GradientStop Color=""#FFDDDEE0"" Offset=""0""/>
                                                            <GradientStop Color=""#FFA1A2A4"" Offset=""1""/>
                                                        </LinearGradientBrush>
                                                    </ribbon:RibbonButton.BorderBrush>
                                                    <ribbon:RibbonButton.Effect>
                                                        <DropShadowEffect ShadowDepth=""2"" Opacity=""0.21"" BlurRadius=""3"" Direction=""310""/>
                                                    </ribbon:RibbonButton.Effect>
                                                </ribbon:RibbonButton>

                                                <StackPanel VerticalAlignment=""Top"" Grid.Column=""1""  Margin=""3"">
                                                    <TextBlock Text=""Print Document"" FontWeight=""Bold"" Margin=""3""/>
                                                    <TextBlock Text=""It prints the existing content of the document."" TextWrapping=""Wrap"" Margin=""3""/>
                                                </StackPanel>
                                            </Grid>
                                            <Rectangle VerticalAlignment=""Top"" Stroke=""#FFBCBCBC"" Height=""1"" StrokeDashArray=""4,3"" />
                                        </StackPanel>
                                        <Border BorderThickness=""0.4"" BorderBrush=""#FFBCBCBC"" Grid.Column=""1"" HorizontalAlignment=""Left"" />
                                    </Grid>
                                  </Border>";
                
                Border border = (Border)XamlReader.Load(tabitem2);
                BackstageTabItem tab2 = new BackstageTabItem();
                tab2.Header = "Print";
                tab2.Content = border;
                BackStage.Items.Add(tab2);
                
                BackStage.Height = BackStageHeight;
                BackStage.Width = BackStageWidth;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
