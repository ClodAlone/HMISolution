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
    public partial class PropertyGrid : UserControl
    {
        internal object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        internal static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(PropertyGrid), new UIPropertyMetadata(null, OnSelectedItemChanged));

        internal static void OnSelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertygrid = dependencyObject as PropertyGrid;
            propertygrid.UpdateSelectedItem(e.NewValue);            
        }

        private Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor imageValueEditor;

        private Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor dataSetEditor;

        static PropertyGrid()
        {
        }

        public PropertyGrid()
        {
            InitializeComponent();
            this.Updateimages += new UpdateImmagesEventHandler(PropertyGrid_Updateimages);
            this.UpdateDataSetsEvent += new UpdateDataSetsEventHandler(PropertyGrid_UpdateDataSetsEvent);
        }

        void PropertyGrid_UpdateDataSetsEvent(object sender, UpdateDataSetsEventArgs e)
        {
            this.dataSetEditor.ChoiceItems = e.datasets;
        }

        void PropertyGrid_Updateimages(object sender, UpdateImagesEventArgs e)
        {
            this.imageValueEditor.ChoiceItems = e.embeddedImages;
        }

        private void PropertygridInitialize(DesignerPanel designerpanel)
        {
            Syncfusion.Windows.PropertyGrid.CustomEditor textProperties = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            textProperties.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomTextBoxUIEditor();
            textProperties.Properties.Add("Name");
            textProperties.Properties.Add("Height");
            textProperties.Properties.Add("Width");
            textProperties.Properties.Add("Left");
            textProperties.Properties.Add("Top");
            textProperties.Properties.Add("HeaderHeight");
            textProperties.Properties.Add("FooterHeight");
            textProperties.Properties.Add("PageWidth");
            textProperties.Properties.Add("PageHeight");
            textProperties.Properties.Add("ReportWidth");
            textProperties.Properties.Add("BodyHeight");
            textProperties.Properties.Add("Size");
            PropertiesWindow.CustomEditorCollection.Add(textProperties);

            Syncfusion.Windows.PropertyGrid.CustomEditor colorProperties = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            colorProperties.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomColorUIEditor(designerpanel);
            colorProperties.Properties.Add("FillColor");
            colorProperties.Properties.Add("FontColor");
            colorProperties.Properties.Add("TitleFontColor");
            colorProperties.Properties.Add("BorderColor");
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
            Syncfusion.Windows.PropertyGrid.CustomEditor padding = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor adornmentType = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor seriesColor = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor adornmentColor = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor showDataLabels = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor datalabelsPosition = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor majorGridLinesWidth = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor minorGridLineWidth = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor categoryAxisName = new Syncfusion.Windows.PropertyGrid.CustomEditor();
            Syncfusion.Windows.PropertyGrid.CustomEditor valueAxisName = new Syncfusion.Windows.PropertyGrid.CustomEditor();

            imagevalue.Properties.Add("ImageValue");
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
            sizing.Properties.Add("Sizing");
            lineStyle.Properties.Add("LineStyle");
            lineWidth.Properties.Add("LineWidth");
            fillStyle.Properties.Add("FillStyle");
            gradientStyle.Properties.Add("GradientStyle");
            borderWidth.Properties.Add("BorderWidth");
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
            dataset.Properties.Add("Dataset");
            padding.Properties.Add("Padding");
            adornmentType.Properties.Add("AdornmentType");
            showDataLabels.Properties.Add("ShowDataLabels");
            datalabelsPosition.Properties.Add("DataLabelsPosition");
            majorGridLinesWidth.Properties.Add("MajorGridLinesWidth");
            minorGridLineWidth.Properties.Add("MinorGridLinesWidth");
            categoryAxisName.Properties.Add("CategoryAxisName");
            valueAxisName.Properties.Add("ValueAxisName");

            this.imageValueEditor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.ImageValue, "Image Source", designerpanel);
            this.imageValueEditor.ChoiceItems = designerpanel.embeddedImages;
            imagevalue.Editor = imageValueEditor;

            this.dataSetEditor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.DatasetName, "Dataset Name", designerpanel);
            this.dataSetEditor.ChoiceItems = designerpanel.dataSets;
            dataset.Editor = dataSetEditor;

            imageSource.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Source, "Image Source", designerpanel);
            thickness.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BorderWidth, "Thickness", designerpanel);
            padding.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Padding, "Padding", designerpanel);

            toolTip.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.StringValue, "ToolTip", designerpanel);
            horizontalAlignment.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.HorizontalAlignment, "HorizontalAlignment", designerpanel);
            fontEffects.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontEffects, "Font Effect", designerpanel);
            fontFamily.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontFamily, "Font Family", designerpanel);
            fontSize.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontSize, "Font Size", designerpanel);
            fontStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontStyle, "Font Style", designerpanel);
            fontWeight.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FontWeight, "Font Weight", designerpanel);
            lineSpacing.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LineSpacing, "Line Spacing", designerpanel);
            hidden.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Hidden", designerpanel);
            keeptogether.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.KeepTogether, "KeepTogether", designerpanel);
            sizing.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Sizing, "Sizing", designerpanel);
            lineStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LineStyle, "Line Style", designerpanel);
            lineWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LineWidth, "Line Width", designerpanel);
            fillStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.FillStyle, "Fill Style", designerpanel);
            gradientStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GradientStyle, "GradientStyle", designerpanel);
            borderWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BorderWidth, "Border Width", designerpanel);
            chartType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.ChartType, "Chart Type", designerpanel);
            layout.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Layout, "Layout", designerpanel);
            position.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Position, "Poition", designerpanel);
            showLegend.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Show Legend", designerpanel);
            reverseDirection.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Reverse Direction", designerpanel);
            hideAxsisLabel.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Hide Axis Label", designerpanel);
            hideMajorTickMarks.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BooleanOptions, "Enable Major Tick Marks", designerpanel);
            hideMinorTickMarks.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BooleanOptions, "Enable Minor Tick Marks", designerpanel);
            enableMinorGridLines.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BooleanOptions, "Enable Major Grid Lines", designerpanel);
            enableMinorGridLines.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BooleanOptions, "Enable Minor Grid Lines", designerpanel);
            tickStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickStyle, " Tick Style", designerpanel);
            tickLength.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GridLineWidth, "Tick Length", designerpanel);
            tickWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GridLineWidth, "Tick Width", designerpanel);
            minorTickStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickStyle, "Minor Tick Style", designerpanel);
            majorGridLinesStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickStyle, "Major Grid Lines Style", designerpanel);
            minorGridLinesStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickStyle, "Minor Grid Lines Style", designerpanel);
            titleAlignment.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TitleAlignment, " Title Alignment", designerpanel);
            type.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.CircularType, "Circular Type", designerpanel);
            style.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.LineStyle, "Style", designerpanel);
            pageBreak.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.PageBreak, "Page Break", designerpanel);
            placement.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickPlacement, "Placement", designerpanel);
            needleType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.NeedleType, "Needle Type", designerpanel);
            pointerType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.PointerType, "Pointer Type", designerpanel);
            majorTickPlacement.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickPlacement, "Major Tick Placement", designerpanel);
            minorTickPlacement.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickPlacement, "Minor Tick placement", designerpanel);
            majorTickShape.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickShape, " Major Tick Shape", designerpanel);
            minorTickShape.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.TickShape, "Minor Tick Shape", designerpanel);
            borderStyle.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BorderStyle, "BorderStyle", designerpanel);
            borderThickness.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.BorderWidth, "Border Thickenss", designerpanel);
            adornmentType.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.AdornmentType, "AdornmentType", designerpanel);
            showDataLabels.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Hidden, "Show Data Labels", designerpanel);
            datalabelsPosition.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.Positions, "Data Labels Position", designerpanel);
            majorGridLinesWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GridLineWidth, "Grid Lines Width", designerpanel);
            minorGridLineWidth.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.GridLineWidth, "Grid Lines Width", designerpanel);
            categoryAxisName.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.AxisName, "Category Axis Name", designerpanel);
            valueAxisName.Editor = new Syncfusion.Windows.Reports.Designer.Editors.CustomComboBoxUIEditor(Syncfusion.Windows.Reports.Designer.Editors.ValueType.AxisName, "Value Axis Name", designerpanel);

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
            PropertiesWindow.CustomEditorCollection.Add(padding);
            PropertiesWindow.CustomEditorCollection.Add(adornmentType);
            PropertiesWindow.CustomEditorCollection.Add(majorGridLinesWidth);
            PropertiesWindow.CustomEditorCollection.Add(minorGridLineWidth);
            PropertiesWindow.CustomEditorCollection.Add(categoryAxisName);
            PropertiesWindow.CustomEditorCollection.Add(valueAxisName);

            PropertiesWindow.RefreshPropertygrid();

        }


        #region custom_events
        public event UpdateImmagesEventHandler Updateimages;

        internal void RaiseUpdateImmagesEventHandler(List<string> embeddedImage)
        {
            if (this.Updateimages != null)
            {
                this.Updateimages(this, new UpdateImagesEventArgs { embeddedImages = embeddedImage });
            }
        }
        public event UpdateDataSetsEventHandler UpdateDataSetsEvent;

        internal void RaiseUpdateDataSetsEventHandler(List<string> dataset)
        {
            if (this.UpdateDataSetsEvent != null)
            {
                this.UpdateDataSetsEvent(this, new UpdateDataSetsEventArgs { datasets = dataset });
            }
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        internal void UpdateReportDesignerObject(DesignerPanel designerpanel)
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

                //      if (property.IsTablixCell)
                {
                    this.PropertiesWindow.HidePropertiesCollection.Add("Height");
                    this.PropertiesWindow.HidePropertiesCollection.Add("Width");
                    this.PropertiesWindow.HidePropertiesCollection.Add("Left");
                    this.PropertiesWindow.HidePropertiesCollection.Add("Top");
                }

                if (selectedObject is Editors.ChartProperties)
                {
                    Editors.ChartProperties chartProperties = selectedObject as Editors.ChartProperties;
                    ChartTypes chartType;

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

            this.PropertiesWindow.SelectedObject = selectedObject;
        }
    }
}
