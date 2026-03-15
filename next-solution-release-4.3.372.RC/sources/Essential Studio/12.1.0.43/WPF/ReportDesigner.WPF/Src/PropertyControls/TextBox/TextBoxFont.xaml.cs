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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.ComponentModel;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for TextBoxFont.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class TextBoxFont
        : UserControl
    {
        #region Constructor
        public TextBoxFont()
        {
            InitializeComponent();
            //this.cmb_FontName.ItemsSource = Fonts.SystemFontFamilies;
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            this.updwn_Custom.MinValue = 1;
            number.PercentSymbol = "pt";
            this.updwn_Custom.NumberFormatInfo = number;
            this.chk_Bold.Checked += new RoutedEventHandler(chk_Bold_Checked);
            this.chk_Bold.Unchecked += new RoutedEventHandler(chk_Bold_Unchecked);
            this.chk_Italic.Checked += new RoutedEventHandler(chk_Italic_Checked);
            this.chk_Italic.Unchecked += new RoutedEventHandler(chk_Italic_Unchecked);
            this.cmb_FontName.SelectionChanged += new SelectionChangedEventHandler(cmb_FontName_SelectionChanged);
            this.cmb_FontSize.SelectionChanged += new SelectionChangedEventHandler(cmb_FontSize_SelectionChanged);
            this.cmb_Effects.SelectionChanged += new SelectionChangedEventHandler(cmb_Effects_SelectionChanged);
            //this.cpkr_Font.ColorChanged += new PropertyChangedCallback(cpkr_Font_ColorChanged);
            //this.cpkr_Font.ColorChanged += new PropertyChangedCallback(colorEdit_ColorChanged);
            this.rbtn_UseFontSize.Checked += new RoutedEventHandler(rbtn_UseFontSize_Checked);
            this.rbtn_UseFontSize.Unchecked += new RoutedEventHandler(rbtn_UseFontSize_Unchecked);
        }

        void colorEdit_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }

        void rbtn_UseFontSize_Unchecked(object sender, RoutedEventArgs e)
        {
            this.LineSpacingCustom.IsChecked = (bool)true;
            this.LineSpacingCustomValue.IsEnabled = true;
            //this.LineSpacingCustomValue.Value = updwn_Custom.MinValue;
        }

        void rbtn_UseFontSize_Checked(object sender, RoutedEventArgs e)
        {            
            this.LineSpacingCustomValue.IsEnabled = false;
        }

        void cpkr_Font_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.tblk_Sample.SetValue(TextBlock.ForegroundProperty, new BrushConverter().ConvertFromInvariantString(e.NewValue.ToString()));
        }

        void cmb_Effects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                switch ((e.AddedItems[0] as object).ToString())
                {
                    case "Default":
                        {
                            this.tblk_Sample.SetValue(TextBlock.TextDecorationsProperty, new TextDecorationCollectionConverter().ConvertFromInvariantString(EffectsType.None.ToString()));
                            break;
                        }
                    case "None":
                        {
                            this.tblk_Sample.SetValue(TextBlock.TextDecorationsProperty, new TextDecorationCollectionConverter().ConvertFromInvariantString(EffectsType.None.ToString()));
                            break;
                        }
                    case "Underline":
                        {
                            this.tblk_Sample.SetValue(TextBlock.TextDecorationsProperty, new TextDecorationCollectionConverter().ConvertFromInvariantString(EffectsType.Underline.ToString()));
                            break;
                        }
                    case "Overline":
                        {
                            this.tblk_Sample.SetValue(TextBlock.TextDecorationsProperty, new TextDecorationCollectionConverter().ConvertFromInvariantString(EffectsType.Overline.ToString()));
                            break;
                        }
                    case "Strikethrough":
                        {
                            this.tblk_Sample.SetValue(TextBlock.TextDecorationsProperty, new TextDecorationCollectionConverter().ConvertFromInvariantString(EffectsType.Strikethrough.ToString()));
                            break;
                        }
                }
            }
        }

        void cmb_FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.tblk_Sample.SetValue(TextBlock.FontSizeProperty, new FontSizeConverter().ConvertFromInvariantString((e.AddedItems[0] as object).ToString()));
            }
        }

        void cmb_FontName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.tblk_Sample.SetValue(TextBlock.FontFamilyProperty, new FontFamilyConverter().ConvertFromInvariantString((e.AddedItems[0] as object).ToString()));
            }
        }

        void chk_Italic_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tblk_Sample.SetValue(TextBlock.FontStyleProperty, new FontStyleConverter().ConvertFromInvariantString(FontStyles.Normal.ToString()));
        }

        void chk_Italic_Checked(object sender, RoutedEventArgs e)
        {
            this.tblk_Sample.SetValue(TextBlock.FontStyleProperty, new FontStyleConverter().ConvertFromInvariantString(FontStyles.Italic.ToString()));
        }

        void chk_Bold_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tblk_Sample.SetValue(TextBlock.FontWeightProperty, new FontWeightConverter().ConvertFromInvariantString(FontWeights.Normal.ToString()));

        }

        void chk_Bold_Checked(object sender, RoutedEventArgs e)
        {
            this.tblk_Sample.SetValue(TextBlock.FontWeightProperty, new FontWeightConverter().ConvertFromInvariantString(FontWeights.Bold.ToString()));
        }
        #endregion

        #region Public Properties
        public ExpressionComboBox ComboFontName
        {
            get
            {
                return this.cmb_FontName;
            }
            set
            {
                this.cmb_FontName = value;
            }
        }

        public ExpressionComboBox ComboFontSize
        {
            get
            {
                return this.cmb_FontSize;
            }
            set
            {
                this.cmb_FontSize = value;
            }
        }

        public System.Windows.Controls.CheckBox Bold
        {
            get
            {
                return this.chk_Bold;
            }
            set
            {
                this.chk_Bold = value;
            }
        }

        public System.Windows.Controls.CheckBox Italic
        {
            get
            {
                return this.chk_Italic;
            }
            set
            {
                this.chk_Italic = value;
            }
        }

        public CustomUIEditorDropDown FontColor
        {
            get
            {
                return this.cpkr_Font;
            }
            set
            {
                this.cpkr_Font = value;
            }
        }

        public ExpressionComboBox Effects
        {
            get
            {
                return this.cmb_Effects;
            }
            set
            {
                this.cmb_Effects = value;
            }
        }

        public System.Windows.Controls.RadioButton LineSpacingFontSize
        {
            get
            {
                return this.rbtn_UseFontSize;
            }
            set
            {
                this.rbtn_UseFontSize = value;
            }
        }

        public System.Windows.Controls.RadioButton LineSpacingCustom
        {
            get
            {
                return this.rbtn_Custom;
            }
            set
            {
                this.rbtn_Custom = value;
            }
        }

        public UpDown LineSpacingCustomValue
        {
            get
            {
                return this.updwn_Custom;
            }
            set
            {
                this.updwn_Custom = value;
            }
        }

        //public System.Windows.Controls.TextBlock LineSpacingCustomValueUnit
        //{
        //    get
        //    {
        //        return this.txt_LeftUnit;
        //    }
        //    set
        //    {
        //        this.txt_LeftUnit = value;
        //    }
        //}
        #endregion
    }
}
