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
using Syncfusion.Windows.Chart;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer
{
    /// <summary>
    /// Interaction logic for PropertyGrid.xaml
    /// </summary>
    public partial class ReportPropertyGrid : UserControl
    {
        internal object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        internal static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(ReportPropertyGrid), new UIPropertyMetadata(null, OnSelectedItemChanged));

        internal static void OnSelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportPropertyGrid propertygrid = dependencyObject as ReportPropertyGrid;
            propertygrid.UpdateSelectedItem(e.NewValue);            
        }

        private Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor imageValueEditor;

        private Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor dataSetEditor;

        private Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor toggleItemEditor;

        private Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor documentMapEditor;

        public ReportDesignView ReportDesignView
        {
            get { return (ReportDesignView)GetValue(ReportDesignViewProperty); }
            set { SetValue(ReportDesignViewProperty, value); }
        }

        public static readonly DependencyProperty ReportDesignViewProperty =
            DependencyProperty.Register("ReportDesignView", typeof(ReportDesignView), typeof(ReportPropertyGrid), new UIPropertyMetadata(null, OnReportDesignViewPropertyChanged));

        internal static void OnReportDesignViewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportPropertyGrid propertyGrid = dependencyObject as ReportPropertyGrid;

            if (propertyGrid != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    if (e.OldValue != null)
                    {
                        ReportDesignView view = e.OldValue as ReportDesignView;
                        view.RemoveReportProperty(propertyGrid);
                    }

                    if (e.NewValue != null)
                    {
                        ReportDesignView view = e.NewValue as ReportDesignView;
                        view.AddReportProperty(propertyGrid);
                    }
                }
            }
        }

        public ReportPropertyGrid()
        {
            InitializeComponent();
            this.propertyGridNameBox.Text = Syncfusion.Windows.ReportDesigner.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "PropertyGrid");
        }

        private void PropertygridInitialize(ReportDesignView designview)
        {
            Syncfusion.Windows.PropertyGrid.CustomEditor textProperties = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            textProperties.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomTextBoxUIEditor();
            textProperties.Properties.Add("Name");
            textProperties.Properties.Add("Height");
            textProperties.Properties.Add("Width");
            textProperties.Properties.Add("Vertical");
            textProperties.Properties.Add("Horizontal");
            textProperties.Properties.Add("Left");
            textProperties.Properties.Add("Top");
            textProperties.Properties.Add("ListLevel");
            textProperties.Properties.Add("HeaderHeight");
            textProperties.Properties.Add("FooterHeight");
            textProperties.Properties.Add("PageWidth");
            textProperties.Properties.Add("PageHeight");
            textProperties.Properties.Add("ReportWidth");
            textProperties.Properties.Add("BodyHeight");
            textProperties.Properties.Add("AutoRefresh");
            textProperties.Properties.Add("DataElementName");
            textProperties.Properties.Add("GroupDataElementName");
            textProperties.Properties.Add("DataSchema");
            textProperties.Properties.Add("DataTransform");
            textProperties.Properties.Add("Author");
            textProperties.Properties.Add("Description");
            textProperties.Properties.Add("ReportName");
            textProperties.Properties.Add("DomainScope");
            //Nested properties.
            textProperties.Properties.Add("Padding");
            textProperties.Properties.Add("Margins");
            textProperties.Properties.Add("BorderWidths");
            textProperties.Properties.Add("BorderColors");
            textProperties.Properties.Add("BorderStyles");
            textProperties.Properties.Add("Indents");
            textProperties.Properties.Add("BackgroundImage");
            PropertiesWindow.CustomEditorCollection.Add(textProperties);

            Syncfusion.Windows.PropertyGrid.CustomEditor colorProperties = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            colorProperties.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomColorUIEditor(designview);
            colorProperties.Properties.Add("FillColor");
            colorProperties.Properties.Add("FontColor");
            colorProperties.Properties.Add("TitleFontColor");
            colorProperties.Properties.Add("BorderColor");
            colorProperties.Properties.Add("DefaultBorderColor");
            colorProperties.Properties.Add("LeftBorderColor");
            colorProperties.Properties.Add("RightBorderColor");
            colorProperties.Properties.Add("TopBorderColor");
            colorProperties.Properties.Add("BottomBorderColor");
            colorProperties.Properties.Add("LineColor");
            //   colorProperties.Properties.Add("Source");
            colorProperties.Properties.Add("PrimaryColor");
            colorProperties.Properties.Add("SecondaryColor");
            colorProperties.Properties.Add("ChartBackground");
            colorProperties.Properties.Add("BackFill");
            colorProperties.Properties.Add("TitleBackFill");
            colorProperties.Properties.Add("TickColor");
            colorProperties.Properties.Add("FrameFill");
            colorProperties.Properties.Add("CapColor");
            colorProperties.Properties.Add("MajorTickColor");
            colorProperties.Properties.Add("MinorTickColor");
            colorProperties.Properties.Add("MajorGridLinesColor");
            colorProperties.Properties.Add("MinorGridLinesColor");
            colorProperties.Properties.Add("BackgroundColor");
            colorProperties.Properties.Add("LegendBorderColor");
            colorProperties.Properties.Add("SeriesColor");
            colorProperties.Properties.Add("AdornmentColor");
            PropertiesWindow.CustomEditorCollection.Add(colorProperties);

            Syncfusion.Windows.PropertyGrid.CustomEditor imagevalue = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor imageSource = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor horizontalAlignment = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor toolTip = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor borderThickness = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor verticalAlignment = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor fontEffects = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor fontFamily = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor fontSize = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor fontStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor fontWeight = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor lineSpacing = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor hidden = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor sizing = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor lineStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor lineWidth = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor fillStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor gradientStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor borderWidth = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor chartType = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor layout = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor position = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor showLegend = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor reverseDirection = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor hideAxsisLabel = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor hideMajorTickMarks = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor hideMinorTickMarks = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor enableMajorGridLines = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor enableMinorGridLines = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor tickStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor tickLength = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor tickWidth = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor minorTickStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor majorGridLinesStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor minorGridLinesStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor titleAlignment = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor thickness = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor type = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor style = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor pageBreak = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor placement = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor needleType = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor pointerType = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor majorTickPlacement = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor minorTickPlacement = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor majorTickShape = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor minorTickShape = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor borderStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor keeptogether = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor dataset = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor adornmentType = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor seriesColor = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor adornmentColor = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor showDataLabels = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor datalabelsPosition = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor majorGridLinesWidth = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor minorGridLineWidth = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor categoryAxisName = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor valueAxisName = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor toggleItem = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor documentMapLabel = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor canGrow = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor canShrink = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor listStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor printOnFirstPage = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor printOnLastPage = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor format = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor mimeType = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor dataElementOutput = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor dataElementStyle = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor spaceAfter = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor spaceBefore = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor leftValue = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor rightValue = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor topValue = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor bottomValue = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor defaultValue = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor hangingIndent = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor leftIndent = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor rightIndent = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor hideDuplicate = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor writingMode = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor initialToggleState = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor keepWithGroup = new Syncfusion.Windows.PropertyGrid.CustomEditor();

            imagevalue.Properties.Add("ImageValue");
            imageSource.Properties.Add("Source");
            toolTip.Properties.Add("ToolTip");
            borderThickness.Properties.Add("BorderThickness");
            horizontalAlignment.Properties.Add("HorizontalAlignment");
            verticalAlignment.Properties.Add("VerticalAlignment");
            fontEffects.Properties.Add("FontEffects");
            fontFamily.Properties.Add("FontFamily");
            fontFamily.Properties.Add("TitleFontFamily");
            fontSize.Properties.Add("FontSize");
            fontSize.Properties.Add("TitleFontSize");
            fontStyle.Properties.Add("FontStyle");
            fontStyle.Properties.Add("TitleFontStyle");
            fontWeight.Properties.Add("FontWeight");
            lineSpacing.Properties.Add("LineSpacing");
            hidden.Properties.Add("Hidden");
            keeptogether.Properties.Add("KeepTogether");
            keeptogether.Properties.Add("FixedColumnHeaders");
            keeptogether.Properties.Add("FixedRowHeaders");
            keeptogether.Properties.Add("RepeatColumnHeaders");
            keeptogether.Properties.Add("RepeatRowHeaders");
            keeptogether.Properties.Add("OmitBorderOnPageBreak");
            keeptogether.Properties.Add("FixedData");
            keeptogether.Properties.Add("HideIfNoRows");
            keeptogether.Properties.Add("RepeatOnNewPage");
            sizing.Properties.Add("Sizing");
            lineStyle.Properties.Add("LineStyle");
            lineWidth.Properties.Add("LineWidth");
            fillStyle.Properties.Add("FillStyle");
            gradientStyle.Properties.Add("GradientStyle");
            borderWidth.Properties.Add("BorderWidth");
            defaultValue.Properties.Add("DefaultBorderWidth");
            leftValue.Properties.Add("LeftBorderWidth");
            leftValue.Properties.Add("PaddingLeft");
            leftValue.Properties.Add("LeftMargin");
            rightValue.Properties.Add("RightBorderWidth");
            rightValue.Properties.Add("PaddingRight");
            rightValue.Properties.Add("RightMargin");
            topValue.Properties.Add("TopBorderWidth");
            topValue.Properties.Add("PaddingTop");
            topValue.Properties.Add("TopMargin");
            bottomValue.Properties.Add("BottomBorderWidth");
            bottomValue.Properties.Add("PaddingBottom");
            bottomValue.Properties.Add("BottomMargin");
            chartType.Properties.Add("ChartType");
            layout.Properties.Add("Layout");
            position.Properties.Add("Position");
            showLegend.Properties.Add("ShowLegend");
            reverseDirection.Properties.Add("ReverseDirection");
            hideAxsisLabel.Properties.Add("HideAxisLabels");
            hideMajorTickMarks.Properties.Add("EnableMajorTickMarks");
            hideMinorTickMarks.Properties.Add("EnableMinorTickMarks");
            enableMinorGridLines.Properties.Add("EnableMajorGridLines");
            enableMinorGridLines.Properties.Add("EnableMinorGridLines");
            tickStyle.Properties.Add("TickStyle");
            tickLength.Properties.Add("TickLength");
            tickWidth.Properties.Add("TickWidth");
            minorTickStyle.Properties.Add("MinorTickStyle");
            majorGridLinesStyle.Properties.Add("MajorGridLinesStyle");
            minorGridLinesStyle.Properties.Add("MinorGridLinesStyle");
            titleAlignment.Properties.Add("TitleAlignment");
            thickness.Properties.Add("Thickness");
            type.Properties.Add("Type");
            style.Properties.Add("Style");
            pageBreak.Properties.Add("PageBreak");
            placement.Properties.Add("Placement");
            needleType.Properties.Add("NeedleType");
            pointerType.Properties.Add("PointerType");
            majorTickPlacement.Properties.Add("MajorTickPlacement");
            minorTickPlacement.Properties.Add("MinorTickPlacement");
            majorTickShape.Properties.Add("MajorTickShape");
            minorTickShape.Properties.Add("MinorTickShape");
            borderStyle.Properties.Add("BorderStyle");
            borderStyle.Properties.Add("DefaultBorderStyle");
            borderStyle.Properties.Add("LeftBorderStyle");
            borderStyle.Properties.Add("RightBorderStyle");
            borderStyle.Properties.Add("TopBorderStyle");
            borderStyle.Properties.Add("BottomBorderStyle");
            dataset.Properties.Add("Dataset");
            adornmentType.Properties.Add("AdornmentType");
            showDataLabels.Properties.Add("ShowDataLabels");
            datalabelsPosition.Properties.Add("DataLabelsPosition");
            majorGridLinesWidth.Properties.Add("MajorGridLinesWidth");
            minorGridLineWidth.Properties.Add("MinorGridLinesWidth");
            categoryAxisName.Properties.Add("CategoryAxisName");
            valueAxisName.Properties.Add("ValueAxisName");
            toggleItem.Properties.Add("ToggleItem");
            documentMapLabel.Properties.Add("DocumentMapLabel");
            documentMapLabel.Properties.Add("Parent");
            canGrow.Properties.Add("CanGrow");
            canShrink.Properties.Add("CanShrink");
            listStyle.Properties.Add("ListStyle");
            printOnFirstPage.Properties.Add("PrintOnFirstPage");
            printOnLastPage.Properties.Add("PrintOnLastPage");
            format.Properties.Add("Format");
            mimeType.Properties.Add("MIMEType");
            dataElementOutput.Properties.Add("DataElementOutput");
            dataElementOutput.Properties.Add("GroupDataElementOutput");
            dataElementStyle.Properties.Add("DataElementStyle");
            spaceAfter.Properties.Add("SpaceAfter");
            spaceBefore.Properties.Add("SpaceBefore");
            hangingIndent.Properties.Add("HangingIndent");
            leftIndent.Properties.Add("LeftIndent");
            rightIndent.Properties.Add("RightIndent");
            hideDuplicate.Properties.Add("HideDuplicates");
            writingMode.Properties.Add("WritingMode");
            initialToggleState.Properties.Add("InitialToggleState");
            keepWithGroup.Properties.Add("KeepWithGroup");

            this.imageValueEditor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.ImageValue, "Image Value", designview);
            this.imageValueEditor.ChoiceItems = designview.embeddedImages;
            imagevalue.Editor = imageValueEditor;

            this.dataSetEditor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.DatasetName, "Dataset Name", designview);
            this.dataSetEditor.ChoiceItems = designview.DataSetNames;
            dataset.Editor = dataSetEditor;

            this.toggleItemEditor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.ToggleItem, "ToggleItem", designview);
            this.toggleItemEditor.ChoiceItems = designview.toggleItems;
            toggleItem.Editor = toggleItemEditor;

            this.documentMapEditor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.DocumentMapLabel, "DocumentMap Label", designview);
            this.documentMapEditor.ChoiceItems = designview.DataSetFields;
            documentMapLabel.Editor = this.documentMapEditor;

            imageSource.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Source, "Image Source", designview);
            thickness.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BorderWidth, "Thickness", designview);

            toolTip.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.StringValue, "ToolTip", designview);
            horizontalAlignment.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.HorizontalAlignment, "HorizontalAlignment", designview);
            verticalAlignment.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.VerticalAlignment, "VerticalAlignment", designview);
            fontEffects.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontEffects, "Font Effect", designview);
            fontFamily.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontFamily, "Font Family", designview);
            fontSize.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontSize, "Font Size", designview);
            fontStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontStyle, "Font Style", designview);
            fontWeight.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontWeight, "Font Weight", designview);
            lineSpacing.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LineSpacing, "Line Spacing", designview);
            hidden.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Hidden", designview);
            keeptogether.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.KeepTogether, "KeepTogether", designview);
            sizing.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Sizing, "Sizing", designview);
            lineStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LineStyle, "Line Style", designview);
            lineWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LineWidth, "Line Width", designview);
            fillStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FillStyle, "Fill Style", designview);
            gradientStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GradientStyle, "GradientStyle", designview);
            borderWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BorderWidth, "Border Width", designview);
            chartType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.ChartType, "Chart Type", designview);
            layout.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Layout, "Layout", designview);
            position.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Position, "Poition", designview);
            showLegend.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Show Legend", designview);
            reverseDirection.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Reverse Direction", designview);
            hideAxsisLabel.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Hide Axis Label", designview);
            hideMajorTickMarks.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BooleanOptions, "Enable Major Tick Marks", designview);
            hideMinorTickMarks.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BooleanOptions, "Enable Minor Tick Marks", designview);
            enableMinorGridLines.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BooleanOptions, "Enable Major Grid Lines", designview);
            enableMinorGridLines.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BooleanOptions, "Enable Minor Grid Lines", designview);
            tickStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickStyle, " Tick Style", designview);
            tickLength.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GridLineWidth, "Tick Length", designview);
            tickWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GridLineWidth, "Tick Width", designview);
            minorTickStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickStyle, "Minor Tick Style", designview);
            majorGridLinesStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickStyle, "Major Grid Lines Style", designview);
            minorGridLinesStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickStyle, "Minor Grid Lines Style", designview);
            titleAlignment.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TitleAlignment, " Title Alignment", designview);
            type.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.CircularType, "Circular Type", designview);
            style.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LineStyle, "Style", designview);
            pageBreak.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.PageBreak, "Page Break", designview);
            placement.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickPlacement, "Placement", designview);
            needleType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.NeedleType, "Needle Type", designview);
            pointerType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.PointerType, "Pointer Type", designview);
            majorTickPlacement.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickPlacement, "Major Tick Placement", designview);
            minorTickPlacement.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickPlacement, "Minor Tick placement", designview);
            majorTickShape.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickShape, " Major Tick Shape", designview);
            minorTickShape.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickShape, "Minor Tick Shape", designview);
            borderStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BorderStyle, "BorderStyle", designview);
            borderThickness.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BorderWidth, "Border Thickenss", designview);
            adornmentType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.AdornmentType, "AdornmentType", designview);
            showDataLabels.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Show Data Labels", designview);
            datalabelsPosition.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Positions, "Data Labels Position", designview);
            majorGridLinesWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GridLineWidth, "Grid Lines Width", designview);
            minorGridLineWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GridLineWidth, "Grid Lines Width", designview);
            categoryAxisName.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.AxisName, "Category Axis Name", designview);
            valueAxisName.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.AxisName, "Value Axis Name", designview);
            canGrow.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.CanGrow, "CanGrow", designview);
            canShrink.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.CanShrink, "CanShrink", designview);
            listStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.ListStyle, "ListStyle", designview);
            printOnFirstPage.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.PrintOnFirstPage, "PrintOnFirstPage", designview);
            printOnLastPage.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.PrintOnLastPage, "PrintOnLastPage", designview);
            format.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Format, "Format", designview);
            mimeType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.MIMEType, "MIMEType", designview);
            dataElementOutput.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.DataElementOutput, "DataElementOutput", designview);
            dataElementStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.DataElementStyle, "DataElementStyle", designview);
            spaceAfter.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.SpaceAfter, "SpaceAfter", designview);
            spaceBefore.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.SpaceBefore, "SpaceBefore", designview);
            defaultValue.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.DefaultValue, "Default", designview);
            leftValue.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LeftValue, "Left", designview);
            rightValue.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.RightValue, "Right", designview);
            topValue.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TopValue, "Top", designview);
            bottomValue.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BottomValue, "Bottom", designview);
            hangingIndent.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.HangingIndent, "HangingIndent", designview);
            leftIndent.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LeftIndent, "LeftIndent", designview);
            rightIndent.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.RightIndent, "RightIndent", designview);
            hideDuplicate.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.HideDuplicates, "HideDuplicates", designview);
            writingMode.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.WritingMode, "WritingMode", designview);
            initialToggleState.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.InitialToggleState, "InitialToggleState", designview);
            keepWithGroup.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.KeepWithGroup, "KeepWithGroup", designview);

            PropertiesWindow.CustomEditorCollection.Add(showDataLabels);
            PropertiesWindow.CustomEditorCollection.Add(datalabelsPosition);
            PropertiesWindow.CustomEditorCollection.Add(seriesColor);
            PropertiesWindow.CustomEditorCollection.Add(adornmentColor);
            PropertiesWindow.CustomEditorCollection.Add(imageSource);
            PropertiesWindow.CustomEditorCollection.Add(imagevalue);
            PropertiesWindow.CustomEditorCollection.Add(toolTip);
            PropertiesWindow.CustomEditorCollection.Add(horizontalAlignment);
            PropertiesWindow.CustomEditorCollection.Add(verticalAlignment);
            PropertiesWindow.CustomEditorCollection.Add(fontEffects);
            PropertiesWindow.CustomEditorCollection.Add(fontFamily);
            PropertiesWindow.CustomEditorCollection.Add(fontWeight);
            PropertiesWindow.CustomEditorCollection.Add(fontSize);
            PropertiesWindow.CustomEditorCollection.Add(fontStyle);
            PropertiesWindow.CustomEditorCollection.Add(lineSpacing);
            PropertiesWindow.CustomEditorCollection.Add(hidden);
            PropertiesWindow.CustomEditorCollection.Add(sizing);
            PropertiesWindow.CustomEditorCollection.Add(lineStyle);
            PropertiesWindow.CustomEditorCollection.Add(lineWidth);
            PropertiesWindow.CustomEditorCollection.Add(fillStyle);
            PropertiesWindow.CustomEditorCollection.Add(gradientStyle);
            PropertiesWindow.CustomEditorCollection.Add(borderWidth);
            PropertiesWindow.CustomEditorCollection.Add(chartType);
            PropertiesWindow.CustomEditorCollection.Add(layout);
            PropertiesWindow.CustomEditorCollection.Add(position);
            PropertiesWindow.CustomEditorCollection.Add(showLegend);
            PropertiesWindow.CustomEditorCollection.Add(reverseDirection);
            PropertiesWindow.CustomEditorCollection.Add(hideAxsisLabel);
            PropertiesWindow.CustomEditorCollection.Add(hideMajorTickMarks);
            PropertiesWindow.CustomEditorCollection.Add(hideMinorTickMarks);
            PropertiesWindow.CustomEditorCollection.Add(enableMajorGridLines);
            PropertiesWindow.CustomEditorCollection.Add(enableMinorGridLines);
            PropertiesWindow.CustomEditorCollection.Add(tickStyle);
            PropertiesWindow.CustomEditorCollection.Add(tickWidth);
            PropertiesWindow.CustomEditorCollection.Add(tickLength);
            PropertiesWindow.CustomEditorCollection.Add(minorTickStyle);
            PropertiesWindow.CustomEditorCollection.Add(majorGridLinesStyle);
            PropertiesWindow.CustomEditorCollection.Add(minorGridLinesStyle);
            PropertiesWindow.CustomEditorCollection.Add(titleAlignment);
            PropertiesWindow.CustomEditorCollection.Add(thickness);
            PropertiesWindow.CustomEditorCollection.Add(type);
            PropertiesWindow.CustomEditorCollection.Add(style);
            PropertiesWindow.CustomEditorCollection.Add(pageBreak);
            PropertiesWindow.CustomEditorCollection.Add(placement);
            PropertiesWindow.CustomEditorCollection.Add(needleType);
            PropertiesWindow.CustomEditorCollection.Add(pointerType);
            PropertiesWindow.CustomEditorCollection.Add(majorTickPlacement);
            PropertiesWindow.CustomEditorCollection.Add(minorTickPlacement);
            PropertiesWindow.CustomEditorCollection.Add(majorTickShape);
            PropertiesWindow.CustomEditorCollection.Add(minorTickShape);
            PropertiesWindow.CustomEditorCollection.Add(borderStyle);
            PropertiesWindow.CustomEditorCollection.Add(borderThickness);
            PropertiesWindow.CustomEditorCollection.Add(dataset);
            PropertiesWindow.CustomEditorCollection.Add(keeptogether);
            PropertiesWindow.CustomEditorCollection.Add(adornmentType);
            PropertiesWindow.CustomEditorCollection.Add(majorGridLinesWidth);
            PropertiesWindow.CustomEditorCollection.Add(minorGridLineWidth);
            PropertiesWindow.CustomEditorCollection.Add(categoryAxisName);
            PropertiesWindow.CustomEditorCollection.Add(valueAxisName);
            PropertiesWindow.CustomEditorCollection.Add(toggleItem);
            PropertiesWindow.CustomEditorCollection.Add(documentMapLabel);
            PropertiesWindow.CustomEditorCollection.Add(canGrow);
            PropertiesWindow.CustomEditorCollection.Add(canShrink);
            PropertiesWindow.CustomEditorCollection.Add(listStyle);
            PropertiesWindow.CustomEditorCollection.Add(printOnFirstPage);
            PropertiesWindow.CustomEditorCollection.Add(printOnLastPage);
            PropertiesWindow.CustomEditorCollection.Add(format);
            PropertiesWindow.CustomEditorCollection.Add(mimeType);
            PropertiesWindow.CustomEditorCollection.Add(dataElementOutput);
            PropertiesWindow.CustomEditorCollection.Add(dataElementStyle);
            PropertiesWindow.CustomEditorCollection.Add(spaceAfter);
            PropertiesWindow.CustomEditorCollection.Add(spaceBefore);
            PropertiesWindow.CustomEditorCollection.Add(defaultValue);
            PropertiesWindow.CustomEditorCollection.Add(leftValue);
            PropertiesWindow.CustomEditorCollection.Add(rightValue);
            PropertiesWindow.CustomEditorCollection.Add(topValue);
            PropertiesWindow.CustomEditorCollection.Add(bottomValue);
            PropertiesWindow.CustomEditorCollection.Add(hangingIndent);
            PropertiesWindow.CustomEditorCollection.Add(leftIndent);
            PropertiesWindow.CustomEditorCollection.Add(rightIndent);
            PropertiesWindow.CustomEditorCollection.Add(writingMode);
            PropertiesWindow.CustomEditorCollection.Add(initialToggleState);
            PropertiesWindow.CustomEditorCollection.Add(hideDuplicate);
            PropertiesWindow.CustomEditorCollection.Add(keepWithGroup);

            PropertiesWindow.RefreshPropertygrid();

        }


        #region custom_events
     
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        internal void UpdateReportDesignerObject(ReportDesignView designerpanel)
        {
            this.PropertygridInitialize(designerpanel);
        }

        internal void UpdateSelectedItem(object selectedObject)
        {
            if (selectedObject != null && selectedObject is IDesignerProperties)
            {
                IDesignerProperties property = selectedObject as IDesignerProperties;

                Binding binding = new Binding();
                binding.Source = property;
                binding.Path = new PropertyPath("Name");
                this.propertyGridNameBox.SetBinding(TextBlock.TextProperty, binding);

                this.PropertiesWindow.HidePropertiesCollection.Clear();

                if (selectedObject is TablixMemberProperties)
                {
                    Editors.TablixMemberProperties memberProperties = selectedObject as Editors.TablixMemberProperties;

                    if (memberProperties.HasGroup)
                    {
                        this.PropertiesWindow.HidePropertiesCollection.Add("HideIfNoRows");
                        this.PropertiesWindow.HidePropertiesCollection.Add("KeepWithGroup");
                        this.PropertiesWindow.HidePropertiesCollection.Add("RepeatOnNewPage");
                    }
                    else
                    {
                        this.PropertiesWindow.HidePropertiesCollection.Add("Name");
                        this.PropertiesWindow.HidePropertiesCollection.Add("Parent");
                        this.PropertiesWindow.HidePropertiesCollection.Add("PageBreak");
                        this.PropertiesWindow.HidePropertiesCollection.Add("DomainScope");
                        this.PropertiesWindow.HidePropertiesCollection.Add("GroupDataElementName");
                        this.PropertiesWindow.HidePropertiesCollection.Add("GroupDataElementOutput");
                        this.PropertiesWindow.HidePropertiesCollection.Add("DocumentMapLabel");
                    }
                }
                else
                {
                    //      if (property.IsTablixCell)
                    {
                        //this.PropertiesWindow.HidePropertiesCollection.Add("Height");
                        //this.PropertiesWindow.HidePropertiesCollection.Add("Width");
                        //this.PropertiesWindow.HidePropertiesCollection.Add("Left");
                        //this.PropertiesWindow.HidePropertiesCollection.Add("Top");
                    }

                    if (selectedObject is Editors.ChartProperties)
                    {
                        Editors.ChartProperties chartProperties = selectedObject as Editors.ChartProperties;
                        ChartTypes chartType;

                        if (!string.IsNullOrEmpty(chartProperties.DesignerMode) && (chartProperties.DesignerMode == "DataBar") || (chartProperties.DesignerMode == "Sparkline"))
                        {
                            this.PropertiesWindow.HidePropertiesCollection.Add("ChartType");
                        }
                        if (string.IsNullOrEmpty(chartProperties.ChartType) || chartProperties.ChartType != null && chartProperties.ChartType.StartsWith("="))
                        {
                            chartType = ChartTypes.Column;
                        }
                        else
                        {
                            chartType = (ChartTypes)Enum.Parse(typeof(ChartTypes), chartProperties.ChartType, true);
                        }

                        if (chartType == ChartTypes.Doughnut || chartType == ChartTypes.Funnel || chartType == ChartTypes.Pie || chartType == ChartTypes.Pyramid || chartType == ChartTypes.StackingArea || chartType == ChartTypes.StackingColumn100 || chartType == ChartTypes.StackingBar)
                        {
                            this.PropertiesWindow.HidePropertiesCollection.Add("AdornmentType");
                        }
                    }
                    else if (selectedObject is Editors.ChartSeriesProperties)
                    {
                        Editors.ChartSeriesProperties chartProperties = selectedObject as Editors.ChartSeriesProperties;
                        ChartTypes chartType;

                        if (string.IsNullOrEmpty(chartProperties.SeriesType) || chartProperties.ChartType != null && chartProperties.ChartType.StartsWith("="))
                        {
                            chartType = ChartTypes.Column;
                        }
                        else
                        {
                            chartType = (ChartTypes)Enum.Parse(typeof(ChartTypes), chartProperties.SeriesType, true);
                        }

                        if (chartType == ChartTypes.StackingArea || chartType == ChartTypes.StackingColumn100 || chartType == ChartTypes.StackingBar)
                        {
                            this.PropertiesWindow.HidePropertiesCollection.Add("AdornmentType");
                            this.PropertiesWindow.HidePropertiesCollection.Add("AdornmentColor");
                            this.PropertiesWindow.HidePropertiesCollection.Add("Size");
                        }
                        else if (chartType == ChartTypes.Polar || chartType == ChartTypes.Radar)
                        {
                            this.PropertiesWindow.HidePropertiesCollection.Add("CategoryAxisName");
                            this.PropertiesWindow.HidePropertiesCollection.Add("ValueAxisName");
                        }
                        else if (chartType == ChartTypes.Doughnut || chartType == ChartTypes.Funnel || chartType == ChartTypes.Pie || chartType == ChartTypes.Pyramid)
                        {
                            this.PropertiesWindow.HidePropertiesCollection.Add("CategoryAxisName");
                            this.PropertiesWindow.HidePropertiesCollection.Add("ValueAxisName");
                            this.PropertiesWindow.HidePropertiesCollection.Add("AdornmentType");
                            this.PropertiesWindow.HidePropertiesCollection.Add("AdornmentColor");
                            this.PropertiesWindow.HidePropertiesCollection.Add("Size");
                        }
                    }
                    else if (selectedObject is Editors.DefaultChartProperties)
                    {
                        Editors.DefaultChartProperties chartProperties = selectedObject as Editors.DefaultChartProperties;
                        ChartTypes chartType;

                        if (string.IsNullOrEmpty(chartProperties.ChartType) || chartProperties.ChartType != null && chartProperties.ChartType.StartsWith("="))
                        {
                            chartType = ChartTypes.Column;
                        }
                        else
                        {
                            chartType = (ChartTypes)Enum.Parse(typeof(ChartTypes), chartProperties.ChartType, true);
                        }

                        if (chartType == ChartTypes.Doughnut || chartType == ChartTypes.Funnel || chartType == ChartTypes.Pie || chartType == ChartTypes.Pyramid || chartType == ChartTypes.Polar || chartType == ChartTypes.Radar)
                        {
                            this.PropertiesWindow.HidePropertiesCollection.Add("CategoryAxisMajorGridLines");
                            this.PropertiesWindow.HidePropertiesCollection.Add("CategoryAxisMinorGridLines");
                            this.PropertiesWindow.HidePropertiesCollection.Add("ValueAxisMajorGridLines");
                            this.PropertiesWindow.HidePropertiesCollection.Add("ValueAxisMinorGridLines");
                        }
                    }
                }
            }

            this.PropertiesWindow.SelectedObject = selectedObject;
        }
    }
}