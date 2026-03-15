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
using Syncfusion.Windows.Reports.Designer.Editors;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.PropertyGrid;
using System.Windows.Data;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Editors
{
    internal class CustomTextBoxUIEditor : DependencyObject ,ITypeEditor
    {
        DesignerTextBox textBox;

        public static readonly DependencyProperty ComboTypeProperty = DependencyProperty.Register("ComboType", typeof(ValueType), typeof(CustomTextBoxUIEditor), new UIPropertyMetadata(ValueType.None));

        public ValueType ComboType
        {
            get { return (ValueType)GetValue(ComboTypeProperty); }
            set { SetValue(ComboTypeProperty, value); }
        }

        public CustomTextBoxUIEditor()
        {
        }

        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            var binding = new Binding("Value")
            {
                Mode = BindingMode.TwoWay,
                Source = info,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true
            };

            BindingOperations.SetBinding(textBox, DesignerTextBox.TextValueProperty, binding);
        }

        public object Create(PropertyInfo propertyInfo)
        {
            try
            {
                this.ComboType = (ValueType)Enum.Parse(typeof(ValueType), propertyInfo.Name, true);
                textBox = new DesignerTextBox();
                textBox.ValueType = this.ComboType;
            }
            catch 
            {
                textBox = null;
            }
            return textBox;
        }

        public void Detach(PropertyViewItem property)
        {
            //throw new NotImplementedException();
        }
    }

    internal class CustomComboBoxUIEditor : DependencyObject, ITypeEditor
    {
        ExpressionComboBox combobox;

        private ReportDesignView reportDesignView;

        public static readonly DependencyProperty ChoiceItemsProperty = DependencyProperty.Register("ChoiceItems", typeof(List<string>), typeof(CustomComboBoxUIEditor), new UIPropertyMetadata(null, null));

        public List<string> ChoiceItems
        {
            get { return (List<string>)GetValue(ChoiceItemsProperty); }
            set { SetValue(ChoiceItemsProperty, value); }
        }

        public static readonly DependencyProperty ComboTypeProperty = DependencyProperty.Register("ComboType", typeof(ValueType), typeof(CustomComboBoxUIEditor), new UIPropertyMetadata(ValueType.None));

        public ValueType ComboType
        {
            get { return (ValueType)GetValue(ComboTypeProperty); }
            set { SetValue(ComboTypeProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(CustomComboBoxUIEditor), new UIPropertyMetadata(string.Empty));

        public String Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }  

        public CustomComboBoxUIEditor(Editors.ValueType valueType,string title,ReportDesignView designview)
        {
            this.ComboType = valueType;
            this.Title = title;
            this.reportDesignView = designview;
        }

        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            var binding = new Binding("Value")
            {
                Mode = BindingMode.TwoWay,
                Source = info,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true
            };

            BindingOperations.SetBinding(combobox, ExpressionComboBox.TextValueProperty, binding);
        }        

        public object Create(PropertyInfo propertyInfo)
        {
            combobox = new ExpressionComboBox(this.ComboType, this.Title,this.reportDesignView);
            combobox.ChoiceItems = this.ChoiceItems;
            return combobox;
        }

        public void Detach(PropertyViewItem property)
        {
            //throw new NotImplementedException();
        }
    }

    internal class CustomColorUIEditor : ITypeEditor
    {
        CustomUIEditorDropDown colorExpression;

        private ReportDesignView reportDesignView;

        public CustomColorUIEditor(ReportDesignView designview)
        {
            this.reportDesignView = designview;
        }

        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            var binding = new Binding("Value")
            {
                Mode = BindingMode.TwoWay,
                Source = info,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true
            };

            BindingOperations.SetBinding(colorExpression, CustomUIEditorDropDown.TextProperty, binding);
        }

        public object Create(PropertyInfo propertyInfo)
        {
            colorExpression = new CustomUIEditorDropDown("Color", "Color",this.reportDesignView);
            colorExpression.BorderThickness = new Thickness(0);
            return colorExpression;
        }

        public void Detach(PropertyViewItem property)
        {
            //throw new NotImplementedException();
        }
    }

    internal enum ValueType
    {
        Name,
        Author,
        Description,
        ReportName,
        Height,
        Width,
        Left,
        Top,
        AutoRefresh,
        HeaderHeight,
        FooterHeight,
        PageWidth,
        PageHeight,
        ReportWidth,
        BodyHeight,
        StringValue,
        FontStyle,
        FontWeight,
        FontEffects,
        Format,
        HorizontalAlignment,
        VerticalAlignment,
        FontSize,
        ChartType,
        DataBarType,
        SparklineType,
        FillStyle,
        Position,
        GradientStyle,
        Sizing,
        FontFamily,
        Hidden,
        TickStyle,
        TitleAlignment,
        PageBreak,
        TickPlacement,
        TickShape,
        NeedleType,
        BorderStyle,
        LineStyle,
        Horizontal,
        Vertical,
        ListStyle,
        ListLevel,
        Layout,
        CircularType,
        PointerType,
        Angle,
        LineSpacing,
        LineWidth,
        BorderWidth,
        DefaultValue,
        LeftValue,
        RightValue,
        TopValue,
        BottomValue,
        TickLength,
        TickWidth,
        StartWidth,
        EndWidth,
        MinValue,
        MaxValue,
        MIMEType,
        DoubleValue,
        Source,
        KeepTogether,
        FixedData,
        HideIfNoRows,
        RepeatOnNewPage,
        ToggleItem,
        DatasetName,
        DataElementName,
        GroupDataElementName,
        DataElementOutput,
        GroupDataElementOutput,
        DataElementStyle,
        DataSchema,
        DataTransform,
        ToolTip,
        ImageValue,
        AdornmentType,
        Size,
        Positions,
        BooleanOptions,
        GridLineWidth,
        AxisName,
        CanGrow,
        CanShrink,
        DocumentMapLabel,
        PrintOnFirstPage,
        PrintOnLastPage,
        SpaceAfter,
        SpaceBefore,
        HangingIndent,
        LeftIndent,
        RightIndent,
        HideDuplicates,
        WritingMode,
        InitialToggleState,
        DomainScope,
        KeepWithGroup,
        Parent,
        None
    }

    internal class ExpressionComboBox : ComboBox
    {
        ExpressionDialog dialog;

        public static readonly DependencyProperty TextValueProperty = DependencyProperty.Register("TextValue", typeof(string), typeof(ExpressionComboBox), new UIPropertyMetadata(string.Empty));

        public string TextValue
        {
            get { return (string)GetValue(TextValueProperty); }
            set { SetValue(TextValueProperty, value); }
        }

        public static readonly DependencyProperty ChoiceItemsProperty = DependencyProperty.Register("ChoiceItems", typeof(List<string>), typeof(ExpressionComboBox), new UIPropertyMetadata(null, null));

        public List<string> ChoiceItems
        {
            get { return (List<string>)GetValue(ChoiceItemsProperty); }
            set { SetValue(ChoiceItemsProperty, value); }
        }

        public static readonly DependencyProperty ComboTypeProperty = DependencyProperty.Register("ComboType", typeof(ValueType), typeof(ExpressionComboBox), new UIPropertyMetadata(ValueType.None,new PropertyChangedCallback(ComboTypePropertyChanged)));

        public ValueType ComboType
        {
            get { return (ValueType)GetValue(ComboTypeProperty); }
            set { SetValue(ComboTypeProperty, value); }
        }

        private ReportDesignView reportDesignView;

        /// <summary>
        /// View Mode changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void ComboTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ExpressionComboBox expressionCombo = d as ExpressionComboBox;

            if (expressionCombo != null)
            {
                expressionCombo.PopulateComboBox();
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ExpressionComboBox), new UIPropertyMetadata(string.Empty));

        public String Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public ExpressionComboBox(Editors.ValueType valueType,string title,ReportDesignView designcontrol)
            : base()
        {
            this.SetResourceReference(StyleProperty, typeof(ComboBox));
            this.IsEditable = true;
            this.BorderBrush = Brushes.Transparent;
            this.BorderThickness = new Thickness(0);
            this.ComboType = valueType;
            this.reportDesignView = designcontrol;
            PopulateComboBox();
            this.Title = title;

            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("TextValue");
            binding.Mode = BindingMode.OneWay;
            this.SetBinding(ExpressionComboBox.TextProperty, binding);

            this.LostFocus += new RoutedEventHandler(ExpressionComboBox_LostFocus);
            this.KeyDown += new KeyEventHandler(ExpressionComboBox_KeyDown);
            this.DropDownClosed += new EventHandler(ExpressionComboBox_DropDownClosed);
        }

        public ExpressionComboBox()
        {
            this.IsEditable = true;
            this.SetResourceReference(StyleProperty, typeof(ComboBox));

            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("TextValue");
            binding.Mode = BindingMode.OneWay;
            this.SetBinding(ExpressionComboBox.TextProperty, binding);

            this.LostFocus += new RoutedEventHandler(ExpressionComboBox_LostFocus);
            this.KeyDown += new KeyEventHandler(ExpressionComboBox_KeyDown);
            this.DropDownClosed += new EventHandler(ExpressionComboBox_DropDownClosed);

            PopulateComboBox();
        }

        void ExpressionComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (this.SelectedIndex != 0 )
            {
                this.ValidateAndUpdate(this.Text);
            }
        }

        void ExpressionComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ValidateAndUpdate(this.Text);
            }
        }

        void ExpressionComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateAndUpdate(this.Text);
        }

        void PopulateComboBox()
        {
            this.Items.Clear();
            this.IsEditable = true;
            
            ExpressionLabel expressionLabel = new ExpressionLabel();
            expressionLabel.Height = 15;
            expressionLabel.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(expressionLabel_PreviewMouseLeftButtonDown);

            switch (this.ComboType)
            {
                case ValueType.Hidden:
                case ValueType.InitialToggleState:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("True");
                    this.Items.Add("False");
                    break;

                case ValueType.CanGrow:
                case ValueType.CanShrink:
                case ValueType.KeepTogether:
                case ValueType.FixedData:
                case ValueType.HideIfNoRows:
                case ValueType.RepeatOnNewPage:
                case ValueType.PrintOnFirstPage:
                case ValueType.PrintOnLastPage:
                    this.IsReadOnly = true;
                    this.Items.Add("True");
                    this.Items.Add("False");
                    break;

                case ValueType.KeepWithGroup:
                    this.IsReadOnly = true;
                    this.Items.Add("None");
                    this.Items.Add("Before");
                    this.Items.Add("After");
                    break;

                case ValueType.FontFamily:
                    this.Items.Add(expressionLabel);

                    foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
                    {
                        this.Items.Add(font.Name);
                    }
                    break;

                case ValueType.FontEffects:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Default");
                    this.Items.Add("None");
                    this.Items.Add("Underline");
                    this.Items.Add("Overline");
                    this.Items.Add("StrikeThrough");
                    break;

                case ValueType.Source:
                    this.IsReadOnly = true;
                    this.Items.Add("Embedded");
                    this.Items.Add("Database");
                    break;

                case ValueType.FontStyle:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Default");
                    this.Items.Add("Normal");
                    this.Items.Add("Italic");
                    break;

                case ValueType.FontWeight:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Default");
                    this.Items.Add("Normal");
                    this.Items.Add("Thin");
                    this.Items.Add("ExtraLight");
                    this.Items.Add("Light");
                    this.Items.Add("Medium");
                    this.Items.Add("SemiBold");
                    this.Items.Add("Bold");
                    this.Items.Add("ExtraBold");
                    this.Items.Add("Heavy");
                    break;

                case ValueType.VerticalAlignment:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Default");
                    this.Items.Add("Top");
                    this.Items.Add("Middle");
                    this.Items.Add("Bottom");
                    break;

                case ValueType.HorizontalAlignment:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Default");
                    this.Items.Add("General");
                    this.Items.Add("Left");
                    this.Items.Add("Center");
                    this.Items.Add("Right");
                    break;

                case ValueType.BorderStyle:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("None");
                    this.Items.Add("Solid");
                    this.Items.Add("Dashed");
                    this.Items.Add("Dotted");
                    this.Items.Add("Double");
                    break;

                case ValueType.LineStyle:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Solid");
                    this.Items.Add("Dashed");
                    this.Items.Add("Dotted");
                    break;

                case ValueType.ListStyle:
                    this.IsReadOnly = true;
                    this.Items.Add("None");
                    this.Items.Add("Numbered");
                    this.Items.Add("Bulleted");
                    break;

                case ValueType.PageBreak:
                    this.IsReadOnly = true;
                    this.Items.Add("None");
                    this.Items.Add("Start");
                    this.Items.Add("End");
                    this.Items.Add("StartAndEnd");
                    break;

                case ValueType.TickShape:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Rectangle");
                    this.Items.Add("RoundedRectangle");
                    this.Items.Add("Ellipse");
                    this.Items.Add("Triangle");
                    break;

                case ValueType.NeedleType:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Triangle");
                    this.Items.Add("Rectangle");
                    this.Items.Add("Arrow");
                    break;

                case ValueType.PointerType:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Needle");
                    this.Items.Add("Marker");
                    break;

                case ValueType.TickPlacement:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Cross");
                    this.Items.Add("Inside");
                    this.Items.Add("Outside");
                    break;

                case ValueType.CircularType:
                    this.Items.Add(expressionLabel);
                    for (int count = 1; count < 5; count++)
                    {
                        this.Items.Add("Circular" + count);
                    }
                    break;

                case ValueType.TitleAlignment:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Center");
                    this.Items.Add("Far");
                    this.Items.Add("Near");
                    break;


                case ValueType.Position:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("TopCenter");
                    this.Items.Add("RightCenter");
                    this.Items.Add("BottomCenter");
                    this.Items.Add("LeftCenter");
                    break;

                case ValueType.GradientStyle:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("None");
                    this.Items.Add("LeftRight");
                    this.Items.Add("TopBottom");
                    break;

                case ValueType.FillStyle:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Solid");
                    this.Items.Add("Gradient");
                    break;

                case ValueType.ChartType:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Area");
                    this.Items.Add("Bubble");
                    this.Items.Add("Bar");
                    this.Items.Add("Column");
                    this.Items.Add("Doughnut");
                    this.Items.Add("FastLine");
                    this.Items.Add("Funnel");
                    this.Items.Add("Line");
                    this.Items.Add("Pie");
                    this.Items.Add("Polar");
                    this.Items.Add("Pyramid");
                    this.Items.Add("Radar");
                    this.Items.Add("RangeArea");
                    this.Items.Add("StackingArea");
                    this.Items.Add("StackingColumn100");
                    this.Items.Add("StepLine");
                    this.Items.Add("StackingBar");
                    this.Items.Add("Scatter");
                    break;

                case ValueType.DataBarType:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Bar");
                    this.Items.Add("StackingBar");
                    this.Items.Add("StackingBar100");
                    this.Items.Add("Column");
                    this.Items.Add("StackingColumn");
                    this.Items.Add("StackingColumn100");
                    break;

                case ValueType.SparklineType:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Column");
                    this.Items.Add("StackingColumn");
                    this.Items.Add("StackingColumn100");
                    this.Items.Add("StackingArea");
                    this.Items.Add("SplineArea");
                    this.Items.Add("StackingArea100");
                    this.Items.Add("Area");
                    this.Items.Add("StepLine");
                    this.Items.Add("Spline");
                    this.Items.Add("Line");
                    this.Items.Add("Doughnut");
                    this.Items.Add("Pie");
                    this.Items.Add("ExplodedDoughnut");
                    this.Items.Add("ExplodedPie");
                    this.Items.Add("Line with Markers");
                    this.Items.Add("Spline with Markers");
                    break;

                case ValueType.Sizing:
                    this.IsReadOnly = true;
                    this.Items.Add("AutoSize");
                    this.Items.Add("Fit");
                    this.Items.Add("FitProportional");
                    this.Items.Add("Clip");
                    break;

                case ValueType.TickLength:
                case ValueType.TickWidth:
                case ValueType.MinValue:
                case ValueType.MaxValue:
                case ValueType.StartWidth:
                case ValueType.EndWidth:
                    this.Items.Add(expressionLabel);
                    for (double width = 1; width <= 100; width++)
                        this.Items.Add(width);
                    break;

                case ValueType.LineWidth:
                case ValueType.BorderWidth:
                case ValueType.DefaultValue:
                case ValueType.LeftValue:
                case ValueType.RightValue:
                case ValueType.TopValue:
                case ValueType.BottomValue:
                    this.Items.Add(expressionLabel);

                    for (double width = 0.25; width <= 10; width += 0.25)
                        this.Items.Add(width + "pt");
                    break;

                case ValueType.FontSize:
                    this.Items.Add(expressionLabel);

                    for (int fontsize = 6; fontsize <= 72; fontsize += 2)
                        this.Items.Add(fontsize + "pt");
                    break;

                case ValueType.Angle:
                    this.Items.Add(expressionLabel);
                    break;

                case ValueType.Layout:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Row");
                    this.Items.Add("Column");
                    break;

                case ValueType.DatasetName:
                    this.DropDownOpened += (sen, arg) =>
                    {
                        this.Items.Clear();
                        if (this.ChoiceItems != null)
                        {
                            foreach (var value in this.ChoiceItems)
                            {
                                this.Items.Add(value);
                            }
                        }
                    };
                    break;

                case ValueType.ImageValue:
                    this.DropDownOpened += (sen, arg) =>
                        {
                            this.Items.Clear();
                            this.Items.Add(expressionLabel);

                            if (this.ChoiceItems != null)
                            {
                                foreach (var value in this.ChoiceItems)
                                {
                                    this.Items.Add(value);
                                }
                            }
                        };
                    break;

                case ValueType.ToggleItem:
                    this.DropDownOpened += (sen, arg) =>
                    {
                        this.Items.Clear();

                        if (this.ChoiceItems != null)
                        {
                            foreach (var value in this.ChoiceItems)
                            {
                                this.Items.Add(value);
                            }
                        }
                    };
                    break;

                case ValueType.Parent:
                case ValueType.DocumentMapLabel:
                    this.DropDownOpened += (sen, arg) =>
                    {
                        this.Items.Clear();
                        this.Items.Add(expressionLabel);

                        if (this.ChoiceItems != null)
                        {
                            foreach (var field in this.ChoiceItems)
                            {
                                this.Items.Add(field);
                            }
                        }
                    };
                    break;

                case ValueType.AdornmentType:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("None");
                    this.Items.Add("Cross");
                    this.Items.Add("Diamond");
                    this.Items.Add("Square");
                    this.Items.Add("Triangle");
                    break;

                case ValueType.Positions:
                    this.Items.Add("Default");
                    this.Items.Add("Top");
                    this.Items.Add("TopLeft");
                    this.Items.Add("TopRight");
                    this.Items.Add("Left");
                    this.Items.Add("Center");
                    this.Items.Add("Right");
                    this.Items.Add("BottomRight");
                    this.Items.Add("Bottom");
                    this.Items.Add("BottomLeft");
                    this.Items.Add("OutSide");
                    break;

                case ValueType.BooleanOptions:
                    this.IsReadOnly = true;
                    this.Items.Add("Auto");
                    this.Items.Add("True");
                    this.Items.Add("False");
                    break;

                case ValueType.GridLineWidth:
                    this.Items.Add(expressionLabel);

                    for (double width = 1; width <= 5; width += 0.25)
                        this.Items.Add(width + "pt");
                    break;

                case ValueType.Size:
                case ValueType.LineSpacing:
                    this.Items.Add(expressionLabel);
                    break;

                case ValueType.AxisName:
                    this.Items.Add("Primary");
                    this.Items.Add("Secondary");
                    break;

                case ValueType.MIMEType:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("image/bmp");
                    this.Items.Add("image/jpeg");
                    this.Items.Add("image/gif");
                    this.Items.Add("image/png");
                    this.Items.Add("image/x-png");
                    break;

                case ValueType.DataElementOutput:
                case ValueType.GroupDataElementOutput:
                    this.IsReadOnly = true;
                    this.Items.Add("Auto");
                    this.Items.Add("Output");
                    this.Items.Add("NoOutput");
                    break;

                case ValueType.DataElementStyle:
                    this.IsReadOnly = true;
                    this.Items.Add("Auto");
                    this.Items.Add("Attribute");
                    this.Items.Add("Element");
                    break;

                case ValueType.HideDuplicates:
                    this.Items.Add("None");
                    break;

                case ValueType.WritingMode:
                    this.Items.Add(expressionLabel);
                    this.Items.Add("Default");
                    this.Items.Add("Horizontal");
                    this.Items.Add("Vertical");
                    break;

                default:
                    this.Items.Add(expressionLabel);
                    break;
            }
        }

        void ValidateAndUpdate(string value)
        {
            bool IsMessageBoxShow = false;
            List<string> fontFamily = new List<string>();
            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                fontFamily.Add(font.Name);
            }

            if (value != null)
            {
                if (!value.StartsWith("="))
                {
                    switch (this.ComboType)
                    {
                        case ValueType.LineStyle:
                        case ValueType.BorderStyle:
                            if (string.Compare(value, "Solid") != 0 && string.Compare(value, "Dashed") != 0 && string.Compare(value, "Dotted") != 0 && string.Compare(value, "None") != 0 && string.Compare(value, "Double") != 0 && string.Compare(value, "DashDotDot") != 0)
                            {
                                IsMessageBoxShow = true;
                            }
                            break;

                        case ValueType.FontStyle:
                            if (string.Compare(value, "Default") != 0 && string.Compare(value, "Normal") != 0 && string.Compare(value, "Italic") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.FontWeight:
                            if (string.Compare(value, "Default") != 0 && string.Compare(value, "Normal") != 0 && string.Compare(value, "Thin") != 0 && string.Compare(value, "ExtraLight") != 0 && string.Compare(value, "Light") != 0 && string.Compare(value, "Medium") != 0 && string.Compare(value, "SemiBold") != 0 && string.Compare(value, "Bold") != 0 && string.Compare(value, "ExtraBold") != 0 && string.Compare(value, "Heavy") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.FontEffects:
                            if (string.Compare(value, "Default") != 0 && string.Compare(value, "None") != 0 && string.Compare(value, "Underline") != 0 && string.Compare(value, "Overline") != 0 && string.Compare(value, "StrikeThrough") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.FontFamily:
                            if(!fontFamily.Contains(value))
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.HorizontalAlignment:
                            if (string.Compare(value, "Default") != 0 && string.Compare(value, "General") != 0 && string.Compare(value, "Left") != 0 && string.Compare(value, "Center") != 0 && string.Compare(value, "Right") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.VerticalAlignment:
                            if (string.Compare(value, "Default") != 0 && string.Compare(value, "Top") != 0 && string.Compare(value, "Middle") != 0 && string.Compare(value, "Bottom") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.Hidden:
                        case ValueType.InitialToggleState:
                            if (string.Compare(value, "True", true) != 0 && string.Compare(value, "False", true) != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.ChartType:
                            if (string.Compare(value, "Area") != 0 && string.Compare(value, "Bubble") != 0 && string.Compare(value, "Bar") != 0 && string.Compare(value, "Column") != 0 && string.Compare(value, "Doughnut") != 0 && string.Compare(value, "FastLine") != 0 && string.Compare(value, "Funnel") != 0 && string.Compare(value, "Line") != 0
                                && string.Compare(value, "Pie") != 0 && string.Compare(value, "Polar") != 0 && string.Compare(value, "Pyramid") != 0 && string.Compare(value, "Radar") != 0 && string.Compare(value, "RangeArea") != 0 && string.Compare(value, "StackingArea") != 0 && string.Compare(value, "StackingColumn100") != 0 && string.Compare(value, "StepLine") != 0
                                && string.Compare(value, "StackingBar") != 0 && string.Compare(value, "Scatter") != 0)
                            {
                                IsMessageBoxShow = true;
                            }
                            break;

                        case ValueType.DataBarType:
                            if (string.Compare(value, "Bar") != 0 && string.Compare(value, "StackingBar") != 0 && string.Compare(value, "StackingBar100") != 0
                                && string.Compare(value, "Column") != 0 && string.Compare(value, "StackingColumn") != 0 && string.Compare(value, "StackingColumn100") != 0)
                            {
                                IsMessageBoxShow = true;
                            }
                            break;

                       case ValueType.SparklineType:
                            if (string.Compare(value, "Bar") != 0 && string.Compare(value, "StackingBar") != 0 && string.Compare(value, "StackingBar100") != 0
                                && string.Compare(value, "Column") != 0 && string.Compare(value, "StackingColumn") != 0 && string.Compare(value, "StackingColumn100") != 0
                                && string.Compare(value, "StackingArea") != 0 && string.Compare(value, "SplineArea") != 0 && string.Compare(value, "StackingArea100") != 0 && string.Compare(value, "Area") != 0
                                 && string.Compare(value, "StepLine") != 0 && string.Compare(value, "Spline") != 0 && string.Compare(value, "Line") != 0 && string.Compare(value, "Doughnut") != 0
                                 && string.Compare(value, "Pie") != 0 && string.Compare(value, "ExplodedDoughnut") != 0 && string.Compare(value, "ExplodedPie") != 0 && string.Compare(value, "Line with Markers") != 0 && string.Compare(value, "Spline with Markers") != 0)
                            {
                                IsMessageBoxShow = true;
                            }
                            break;

                        case ValueType.AdornmentType:
                            if (string.Compare(value, "None") != 0 && string.Compare(value, "Cross") != 0 && string.Compare(value, "Diamond") != 0 && string.Compare(value, "Square") != 0 && string.Compare(value, "Triangle") != 0)
                            {
                                IsMessageBoxShow = true;
                            }
                            break;

                        case ValueType.FillStyle:
                            if (string.Compare(value, "Solid") != 0 && string.Compare(value, "Gradient") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.GradientStyle:
                            if (string.Compare(value, "None") != 0 && string.Compare(value, "LeftRight") != 0 && string.Compare(value, "TopBottom") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.Position:
                            if (string.Compare(value, "TopCenter") != 0 && string.Compare(value, "RightCenter") != 0 && string.Compare(value, "BottomCenter") != 0 && string.Compare(value, "LeftCenter") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.Layout:
                            if (string.Compare(value, "Row") != 0 && string.Compare(value, "Column") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.TickStyle:
                            if (string.Compare(value, "Solid") != 0 && string.Compare(value, "Dashed") != 0 && string.Compare(value, "Dotted") != 0 && string.Compare(value, "DashDot") != 0 && string.Compare(value, "DashDotDot") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.TitleAlignment:
                            if (string.Compare(value, "Center") != 0 && string.Compare(value, "Far") != 0 && string.Compare(value, "Near") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.CircularType:
                            if (string.Compare(value, "Circular1") != 0 && string.Compare(value, "Circular2") != 0 && string.Compare(value, "Circular3") != 0 && string.Compare(value, "Circular4") != 0 && string.Compare(value, "Circular5") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.TickPlacement:
                            if (string.Compare(value, "Cross") != 0 && string.Compare(value, "Inside") != 0 && string.Compare(value, "Outside") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.PointerType:
                            if (string.Compare(value, "Needle") != 0 && string.Compare(value, "Marker") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.NeedleType:
                            if (string.Compare(value, "Triangle") != 0 && string.Compare(value, "Rectangle") != 0 && string.Compare(value, "Arrow") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.TickShape:
                            if (string.Compare(value, "Rectangle") != 0 && string.Compare(value, "RoundedRectangle") != 0 && string.Compare(value, "Ellipse") != 0 && string.Compare(value, "Triangle") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.AxisName:
                            if (string.Compare(value, "Primary") != 0 && string.Compare(value, "Secondary") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.MIMEType:
                            if (string.Compare(value, "image/bmp") != 0 && string.Compare(value, "image/jpeg") != 0 && string.Compare(value, "image/gif") != 0 &&
                                string.Compare(value, "image/png") != 0 && string.Compare(value, "image/x-png") != 0)
                                IsMessageBoxShow = true;
                            break;

                        case ValueType.WritingMode:
                            if (string.Compare(value, "Default") != 0 && string.Compare(value, "Horizontal") != 0 && string.Compare(value, "Vertical") != 0)
                                IsMessageBoxShow = true;
                            break;
                    }

                    switch (this.ComboType)
                    {
                        case ValueType.BorderWidth:
                        case ValueType.DefaultValue:
                        case ValueType.LeftValue:
                        case ValueType.RightValue:
                        case ValueType.TopValue:
                        case ValueType.BottomValue:
                        case ValueType.SpaceAfter:
                        case ValueType.SpaceBefore:
                        case ValueType.LeftIndent:
                        case ValueType.RightIndent:
                        case ValueType.HangingIndent:
                        case ValueType.LineSpacing:
                            try
                            {
                                RDL.DOM.Size size = new RDL.DOM.Size(value);

                                if (size.MeasurementUnit == RDL.DOM.MeasurementUnits.None)
                                {
                                    this.Text = size.FloatValue + "pt";
                                }
                            }
                            catch
                            {
                                IsMessageBoxShow = true;
                            }
                            break;
                    }
                }
            }

            if (IsMessageBoxShow == true)
            {
                MessageBoxResult outValue = MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidProperty"), SR.GetString(CultureInfo.CurrentUICulture, "titleControlProperty"), MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (outValue == MessageBoxResult.OK)
                {
                    this.Text = this.TextValue;
                }
                else
                {
                    this.Text = this.TextValue;
                }
            }
            else
            {
                this.SetValue(ExpressionComboBox.TextValueProperty, this.Text);
            }
        }

        void expressionLabel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dialog = new ExpressionDialog(this.ComboType, this.Title);
            Binding binding = new Binding();
            binding.Path = new PropertyPath("Text");
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            dialog.SetBinding(ExpressionDialog.TextProperty, binding);
            if (this.reportDesignView != null)
            {
                this.reportDesignView.UpdateOwnerWindow(dialog);
            }
            else
            {
                this.dialog.Owner = Window.GetWindow(this);
                SkinStorage.SetVisualStyle(this.dialog, SkinStorage.GetVisualStyle(this.dialog.Owner));
            }
            if (dialog.ShowDialog() == true)
            {
                this.ValidateAndUpdate(this.Text);
            }
        }
    }
}
