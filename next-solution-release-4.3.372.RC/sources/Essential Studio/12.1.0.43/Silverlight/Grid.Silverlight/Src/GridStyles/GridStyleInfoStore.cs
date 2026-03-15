#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if !WinRT
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Styles;
using System.Windows.Data;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Styles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    [
    StaticDataField("sd")
    ]
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridStyleInfoStore : StyleInfoStore
    {
        public static StaticData sd = new StaticData(typeof(GridStyleInfoStore), typeof(GridStyleInfo), false);

        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.CellType"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty CellRendererProperty = sd.CreateStyleInfoProperty(typeof(ICellRenderer), "CellRenderer");

        public readonly static StyleInfoProperty GridCommentStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridCommentStyleInfo), "GridCommentStyleInfo");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellType"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellTypeProperty = sd.CreateStyleInfoProperty(typeof(string), "CellType");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellValue"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellValueProperty = sd.CreateStyleInfoProperty(typeof(object), "CellValue");

        public readonly static StyleInfoProperty CellValue2Property = sd.CreateStyleInfoProperty(typeof(object), "CellValue2");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellEditTemplate"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellEditTemplateProperty = sd.CreateStyleInfoProperty(typeof(DataTemplate), "CellEditTemplate");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellItemTemplate"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellItemTemplateProperty = sd.CreateStyleInfoProperty(typeof(DataTemplate), "CellItemTemplate");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellEditTemplateKey"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellEditTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "CellEditTemplateKey");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellItemTemplateKey"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellItemTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "CellItemTemplateKey");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellValueType"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellValueTypeProperty = sd.CreateStyleInfoProperty(typeof(Type), "CellValueType");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Format"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FormatProperty = sd.CreateStyleInfoProperty(typeof(string), "Format");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ParseFormats"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ParseFormatsProperty = sd.CreateStyleInfoProperty(typeof(string[]), "ParseFormats");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.BaseStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BaseStyleProperty = sd.CreateStyleInfoProperty(typeof(string), "BaseStyle");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Error"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ErrorProperty = sd.CreateStyleInfoProperty(typeof(string), "Error");
#if !WinRT
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.PropertyDescriptor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty PropertyDescriptorProperty = sd.CreateStyleInfoProperty(typeof(PropertyDescriptor), "PropertyDescriptor", StyleInfoPropertyOptions.None);       
#endif

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ErrorInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ErrorInfoProperty = sd.CreateStyleInfoProperty(typeof(GridErrorStyleInfo), "ErrorInfo");
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Exception"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ExceptionProperty = sd.CreateStyleInfoProperty(typeof(Exception), "Exception");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CultureInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CultureInfoProperty = sd.CreateStyleInfoProperty(typeof(CultureInfo), "CultureInfo", StyleInfoPropertyOptions.Serializable);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ReadOnly"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ReadOnlyProperty = sd.CreateStyleInfoProperty(typeof(bool), "ReadOnly", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.StrictValueType"/> property.
        /// </summary>
        public readonly static StyleInfoProperty StrictValueTypeProperty = sd.CreateStyleInfoProperty(typeof(bool), "StrictValueType", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Background"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "Background");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Foreground"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "Foreground");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.BorderMargins"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BorderMarginsProperty = sd.CreateStyleInfoProperty(typeof(CellMarginsInfo), "BorderMargins");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.BorderMargins"/> property.
        /// </summary>
        public readonly static StyleInfoProperty PaddingProperty = sd.CreateStyleInfoProperty(typeof(CellMarginsInfo), "Padding");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TextMargins"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextMarginsProperty = sd.CreateStyleInfoProperty(typeof(CellMarginsInfo), "TextMargins");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Borders"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BordersProperty = sd.CreateStyleInfoProperty(typeof(CellBordersInfo), "Borders");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Font"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontProperty = sd.CreateStyleInfoProperty(typeof(GridFontInfo), "Font");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Description"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DescriptionProperty = sd.CreateStyleInfoProperty(typeof(string), "Description");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Enabled"/> property.
        /// </summary>
        public readonly static StyleInfoProperty EnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "Enabled", 1, true);
        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.FormulaTag"/> property.
        ///// </summary>
        public readonly static StyleInfoProperty FormulaTagProperty = sd.CreateStyleInfoProperty(typeof(GridFormulaTag), "FormulaTag");
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ValueMember"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ValueMemberProperty = sd.CreateStyleInfoProperty(typeof(string), "ValueMember");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DisplayMember"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DisplayMemberProperty = sd.CreateStyleInfoProperty(typeof(string), "DisplayMember");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ItemsSource"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ItemsSourceProperty = sd.CreateStyleInfoProperty(typeof(IEnumerable), "ItemsSource", StyleInfoPropertyOptions.Serializable);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ChoiceList"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ChoiceListProperty = sd.CreateStyleInfoProperty(typeof(List<string>), "ChoiceList");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.IsEditable"/> property.
        /// </summary>
        public readonly static StyleInfoProperty IsEditableProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsEditable", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Tag"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TagProperty = sd.CreateStyleInfoProperty(typeof(object), "Tag");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.AcceptsReturn"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AcceptsReturnProperty = sd.CreateStyleInfoProperty(typeof(bool), "AcceptsReturn", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.AutoWordSelection"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AutoWordSelectionProperty = sd.CreateStyleInfoProperty(typeof(bool), "AutoWordSelection", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.AllowRowResize"/> property.
        /// </summary>
        internal readonly static StyleInfoProperty AllowRowResizeProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowRowResize", 1, true);

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.CharacterCasing"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty CharacterCasingProperty = sd.CreateStyleInfoProperty(typeof(CharacterCasing), "CharacterCasing", 2, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TextAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextAlignmentProperty = sd.CreateStyleInfoProperty(typeof(TextAlignment), "TextAlignment", 3, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.AcceptsReturn"/> property.
        /// </summary>
        public readonly static StyleInfoProperty EnableFloatingCellProperty = sd.CreateStyleInfoProperty(typeof(bool), "EnableFloatingCell", 1, true);
        
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FloatCellMode"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FloatCellModeProperty = sd.CreateStyleInfoProperty(typeof(GridFloatCellsMode), "FloatCellMode", 2, true);
        
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FloodCell"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FloodCellProperty = sd.CreateStyleInfoProperty(typeof(bool), "FloodCell", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.HorizontalAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty HorizontalAlignmentProperty = sd.CreateStyleInfoProperty(typeof(HorizontalAlignment), "HorizontalAlignment", 3, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TextWrapping"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextWrappingProperty = sd.CreateStyleInfoProperty(typeof(TextWrapping), "TextWrapping", 2, true);

        public readonly static StyleInfoProperty TextTrimmingProperty = sd.CreateStyleInfoProperty(typeof(TextTrimming), "TextTrimming", 2, true);
        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.FlowDirection"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty FlowDirectionProperty = sd.CreateStyleInfoProperty(typeof(FlowDirection), "FlowDirection", 1, true);

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.TextTrimming"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty TextTrimmingProperty = sd.CreateStyleInfoProperty(typeof(TextTrimming), "TextTrimming", 1, true);

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.MaskEdit"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty MaskEditInfoProperty = sd.CreateStyleInfoProperty(typeof(GridMaskEditInfo),
        //    "MaskEdit", 1, true);

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.UpDownEdit"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty UpDownEditInfoProperty = sd.CreateStyleInfoProperty(typeof(GridUpDownEditStyleInfo), "UpDownEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ExclusiveChoiceList"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ExclusiveChoiceListProperty = sd.CreateStyleInfoProperty(typeof(bool), "ExclusiveChoiceList", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DropDownStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AutoCompleteProperty = sd.CreateStyleInfoProperty(typeof(bool), "AutoComplete", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.StaysOpenOnEdit"/> property.
        /// </summary>
        public readonly static StyleInfoProperty StaysOpenOnEditProperty = sd.CreateStyleInfoProperty(typeof(bool), "StaysOpenOnEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.NumberFormat"/> property.
        /// </summary>
        public readonly static StyleInfoProperty NumberFormatProperty = sd.CreateStyleInfoProperty(typeof(NumberFormatInfo), "NumberFormat");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditStyleInfo.NegativeForeground"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty NegativeForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "NegativeForeground");

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.PercentEditMode"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty PercentEditModeProperty = sd.CreateStyleInfoProperty(typeof(PercentEditMode), "PercentEditMode");

        ///// <summary>
        ///// Provides information about the <see cref="GridStyleInfo.DateTimeEdit"/> property.
        ///// </summary>
        //public readonly static StyleInfoProperty DateTimeEditStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridDateTimeEditStyleInfo), "DateTimeEdit");

        //public readonly static StyleInfoProperty CurrencyEditStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridCurrencyEditStyleInfo), "CurrencyEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ToolTip"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ToolTipProperty = sd.CreateStyleInfoProperty(typeof(object), "ToolTip", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.MaxLength"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MaxLengthProperty = sd.CreateStyleInfoProperty(typeof(int), "MaxLength");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.VerticalAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty VerticalAlignmentProperty = sd.CreateStyleInfoProperty(typeof(VerticalAlignment), "VerticalAlignment", 3, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ToolTip"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowTooltipProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowToolTip", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TooltipTemplateKey"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TooltipTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "TooltipTemplateKey");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CommentTemplateKey"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CommentTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "CommentTemplateKey");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Comment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CommentProperty = sd.CreateStyleInfoProperty(typeof(string), "Comment");

        public readonly static StyleInfoProperty IsThreeStateStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsThreeState");

#if !WinRT
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CommentAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CommentAlignmentProperty = sd.CreateStyleInfoProperty(typeof(CommentAlignment), "CommentAlignment");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.IntEditStyleInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty IntegerEditStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridIntegerEditStyleInfo), "IntEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DoubleEditStyleInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DoubleEditStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridDoubleEditStyleInfo), "DoubleEdit");

        public readonly static StyleInfoProperty DateTimeEditStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridDateTimeEditStyleInfo), "DateTimeEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TimeSpanEdit"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TimeSpanEditStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridTimeSpanEditStyleInfo), "TimeSpanEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.PercentEditStyleInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty PercentEditStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridPercentEditStyleInfo), "PercentEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CurrencyEditStyleInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CurrencyEditStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridCurrencyEditStyleInfo), "CurrencyEdit");


        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.NumericUpDownEditStyleInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty UpDownEditInfoProperty = sd.CreateStyleInfoProperty(typeof(GridUpDownEditStyleInfo), "UpDownEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.MaskEditStyleInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MaskEditInfoProperty = sd.CreateStyleInfoProperty(typeof(GridMaskEditStyleInfo), "MaskEdit");
#endif
        public readonly static StyleInfoProperty ImageCellStyleInfoProperty = sd.CreateStyleInfoProperty(typeof(GridImageCellStyleInfo), "ImageCell");

        /// <override/>
        /// 
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageList"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ImageListProperty = sd.CreateStyleInfoProperty(typeof(ObservableCollection<Image>), "ImageList");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageIndex"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ImageIndexProperty = sd.CreateStyleInfoProperty(typeof(int), "ImageIndex");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageContentStretch"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ImageContentStretchProperty = sd.CreateStyleInfoProperty(typeof(ImageContentStretch), "ImageContentStretch");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageContentAlignment"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ImageContentAlignmentProperty = sd.CreateStyleInfoProperty(typeof(ImageContentAlignment), "ImageContentAlignment");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageWidth"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ImageWidthProperty = sd.CreateStyleInfoProperty(typeof(GridLength), "ImageWidth");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageHeight"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ImageHeightProperty = sd.CreateStyleInfoProperty(typeof(GridLength), "ImageHeight");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageMargins"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ImageMarginsProperty = sd.CreateStyleInfoProperty(typeof(CellMarginsInfo), "ImageMargins");
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ConditionalFormat"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ConditionalFormatProperty = sd.CreateStyleInfoProperty(typeof(GridConditionalFormat), "ConditionalFormat");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ApplyConditionalFormatBasedOn"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ApplyConditionalFormatBasedOnProperty = sd.CreateStyleInfoProperty(typeof(ApplyConditionalBasedOn), "ApplyConditionalFormatBasedOn");

        [Obsolete]
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ApplyConditionalFormatBasedOn"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ApplyConditionalFormatBasdOnProperty = sd.CreateStyleInfoProperty(typeof(ApplyConditionalBasedOn), "ApplyConditionalFormatBasdOn");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ShowDataValidationTooltip"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ShowDataValidationTooltipProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowDataValidationTooltip");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DataValidationTooltip"/> property.
        /// </summary>
        public static readonly StyleInfoProperty DataValidationTooltipProperty = sd.CreateStyleInfoProperty(typeof(string), "DataValidationTooltip");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ErrorAlert"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ErrorAlertProperty = sd.CreateStyleInfoProperty(typeof(string), "ErrorAlert");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ErrorAlertTitle"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ErrorAlertTitleProperty = sd.CreateStyleInfoProperty(typeof(string), "ErrorAlertTitle");

        [Obsolete]
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ErrorAlertTitle"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ErrorAlartTitleProperty = sd.CreateStyleInfoProperty(typeof(string), "ErrorAlartTitle");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DataValidationTooltipTemplateKey"/> property.
        /// </summary>
        public static readonly StyleInfoProperty DataValidationTooltipTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "DataValidationTooltipTemplateKey");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DataValidationTooltipLocation"/> property.
        /// </summary>
        public static readonly StyleInfoProperty DataValidationTooltipLocationProperty = sd.CreateStyleInfoProperty(typeof(Point), "DataValidationTooltipLocation");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FormatProvider"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FormatProviderProperty = sd.CreateStyleInfoProperty(typeof(IFormatProvider), "FormatProvider");

        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }


        /// <overload>
        /// Initializes a <see cref="GridStyleInfoStore"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridStyleInfoStore"/>.
        /// </summary>
        public GridStyleInfoStore()
        {
            if (sd.IsEmpty)
                new GridStyleInfo();
        }

        static GridStyleInfoStore()
        {
            BorderMarginsProperty.CreateObject = new CreateSubObjectHandler(CellMarginsInfo.CreateObject);
            TextMarginsProperty.CreateObject = new CreateSubObjectHandler(CellMarginsInfo.CreateObject);
            BordersProperty.CreateObject = new CreateSubObjectHandler(CellBordersInfo.CreateObject);
            FontProperty.CreateObject = new CreateSubObjectHandler(GridFontInfo.CreateObject);
#if !WinRT
            MaskEditInfoProperty.CreateObject = new CreateSubObjectHandler(GridMaskEditStyleInfo.CreateObject);
            UpDownEditInfoProperty.CreateObject = new CreateSubObjectHandler(GridUpDownEditStyleInfo.CreateObject);
            PercentEditStyleInfoProperty.CreateObject = new CreateSubObjectHandler(GridPercentEditStyleInfo.CreateObject);
            DoubleEditStyleInfoProperty.CreateObject = new CreateSubObjectHandler(GridDoubleEditStyleInfo.CreateObject);
            IntegerEditStyleInfoProperty.CreateObject = new CreateSubObjectHandler(GridIntegerEditStyleInfo.CreateObject);
            //DateTimeEditStyleInfoProperty.CreateObject = new CreateSubObjectHandler(GridDateTimeEditStyleInfo.CreateObject);
            ErrorInfoProperty.CreateObject = new CreateSubObjectHandler(GridErrorStyleInfo.CreateObject);
            CurrencyEditStyleInfoProperty.CreateObject = new CreateSubObjectHandler(GridCurrencyEditStyleInfo.CreateObject);
#endif
            CellValueProperty.IsAnyObject = true;
            CultureInfoProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            BackgroundProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            BackgroundProperty.Format += new StyleInfoPropertyConvertEventHandler(BrushPropertyFormat);
            BackgroundProperty.Parse += new StyleInfoPropertyConvertEventHandler(BrushPropertyParse);
            ForegroundProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            ForegroundProperty.Format += BrushPropertyFormat;
            ForegroundProperty.Parse += BrushPropertyParse;
            HorizontalAlignmentProperty.Parse += new StyleInfoPropertyConvertEventHandler(HorizontalAlignmentProperty_Parse);
            VerticalAlignmentProperty.Parse += new StyleInfoPropertyConvertEventHandler(VerticalAlignmentProperty_Parse);
            CellItemTemplateKeyProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
            CellEditTemplateKeyProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
        }

        private static void VerticalAlignmentProperty_Parse(object sender, StyleInfoPropertyConvertEventArgs e)
        {
            var alignment = e.Value.ToString();
            e.Value = Enum.Parse(typeof(VerticalAlignment), alignment, false);
            e.Handled = true;
        }

        private static void HorizontalAlignmentProperty_Parse(object sender, StyleInfoPropertyConvertEventArgs e)
        {
            var alignment = e.Value.ToString();
            e.Value = Enum.Parse(typeof(HorizontalAlignment), alignment, false);
            e.Handled = true;
        }

        private static void BrushPropertyParse(object sender, StyleInfoPropertyConvertEventArgs e)
        {
            var hexaColor = e.Value != null ? e.Value.ToString() : string.Empty;
            if (hexaColor != string.Empty)
            {
                try
                {
                    var color = Color.FromArgb(Convert.ToByte(hexaColor.Substring(1, 2), 16), Convert.ToByte(hexaColor.Substring(3, 2), 16), Convert.ToByte(hexaColor.Substring(5, 2), 16), Convert.ToByte(hexaColor.Substring(7, 2), 16));
                    e.Value = new SolidColorBrush(color);
                    e.Handled = true;
                }
                catch
                {
                    e.Value = string.Empty;
                    e.Handled = true;
                }
            }
        }

        static void BrushPropertyFormat(object sender, StyleInfoPropertyConvertEventArgs e)
        {
            var brush = e.Value as Brush;
            if (brush is SolidColorBrush)
            {
                e.Value = ((SolidColorBrush)brush).Color;
            }
            else if (brush is LinearGradientBrush)
            {
                // var linearBrush = brush as LinearGradientBrush;
                // LGB is not supported
                e.Value = string.Empty;
            }
            e.Handled = true;
        }

        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new GridStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

}
