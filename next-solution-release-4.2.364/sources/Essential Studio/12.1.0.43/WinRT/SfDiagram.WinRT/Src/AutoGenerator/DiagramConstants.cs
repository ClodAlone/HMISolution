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
#if WINRT
using System.Threading.Tasks;
using Windows.UI.Xaml; 
#endif
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Controller;
namespace Syncfusion.UI.Xaml.Diagram
{
	internal static class NodePortConstants
	{
        public const string NodeOffsetX = "NodeOffsetX";
        public const string NodeOffsetY = "NodeOffsetY";
        public const string Node = "Node";
        public const string UnitMode = "UnitMode";
        public const string ID = "ID";
        public const string Key = "Key";
        public const string Shape = "Shape";
        public const string ShapeStyle = "ShapeStyle";
        public const string Constraints = "Constraints";
		public static List<Tuple<DependencyProperty, string>> NodePortProperties = new List<Tuple<DependencyProperty, string>>()
        {
			new Tuple<DependencyProperty, string>(NodePort.NodeOffsetXProperty, NodeOffsetX),
						new Tuple<DependencyProperty, string>(NodePort.NodeOffsetYProperty, NodeOffsetY),
						new Tuple<DependencyProperty, string>(NodePort.NodeProperty, Node),
						new Tuple<DependencyProperty, string>(NodePort.UnitModeProperty, UnitMode),
						new Tuple<DependencyProperty, string>(NodePort.IDProperty, ID),
						new Tuple<DependencyProperty, string>(NodePort.KeyProperty, Key),
						new Tuple<DependencyProperty, string>(NodePort.ShapeProperty, Shape),
						new Tuple<DependencyProperty, string>(NodePort.ShapeStyleProperty, ShapeStyle),
						new Tuple<DependencyProperty, string>(NodePort.ConstraintsProperty, Constraints),
					};
	}
	internal static class AnnotationEditorConstants
	{
        public const string Content = "Content";
        public const string EditTemplate = "EditTemplate";
        public const string ViewTemplate = "ViewTemplate";
        public const string Mode = "Mode";
        public const string HorizontalAlignment = "HorizontalAlignment";
        public const string VerticalAlignment = "VerticalAlignment";
        public const string Alignment = "Alignment";
		public static List<Tuple<DependencyProperty, string>> AnnotationEditorProperties = new List<Tuple<DependencyProperty, string>>()
        {
			new Tuple<DependencyProperty, string>(AnnotationEditor.ContentProperty, Content),
						new Tuple<DependencyProperty, string>(AnnotationEditor.EditTemplateProperty, EditTemplate),
						new Tuple<DependencyProperty, string>(AnnotationEditor.ViewTemplateProperty, ViewTemplate),
						new Tuple<DependencyProperty, string>(AnnotationEditor.ModeProperty, Mode),
						new Tuple<DependencyProperty, string>(AnnotationEditor.HorizontalAlignmentProperty, HorizontalAlignment),
						new Tuple<DependencyProperty, string>(AnnotationEditor.VerticalAlignmentProperty, VerticalAlignment),
					};
	}
	internal static class DiagramElementConstants
	{
        public const string ID = "ID";
        public const string Key = "Key";
		public static List<Tuple<DependencyProperty, string>> DiagramElementProperties = new List<Tuple<DependencyProperty, string>>()
        {
		};
	}
	internal static class NodeConstants
	{
        public const string OffsetX = "OffsetX";
        public const string OffsetY = "OffsetY";
        public const string RotateAngle = "RotateAngle";
        public const string SnapToObject = "SnapToObject";
        public const string Flip = "Flip";
        public const string MinWidth = "MinWidth";
        public const string MaxWidth = "MaxWidth";
        public const string UnitWidth = "UnitWidth";
        public const string MinHeight = "MinHeight";
        public const string MaxHeight = "MaxHeight";
        public const string UnitHeight = "UnitHeight";
        public const string Content = "Content";
        public const string ContentTemplate = "ContentTemplate";
        public const string Shape = "Shape";
        public const string ShapeStyle = "ShapeStyle";
        public const string ParentGroup = "ParentGroup";
        public const string IsExpanded = "IsExpanded";
        public const string Pivot = "Pivot";
        public const string AutoBind = "AutoBind";
        public const string ID = "ID";
        public const string Key = "Key";
        public const string IsSelected = "IsSelected";
        public const string ZIndex = "ZIndex";
        public const string Annotations = "Annotations";
        public const string Constraints = "Constraints";
        public const string Ports = "Ports";
        public const string InternalPorts = "InternalPorts";
		public static List<Tuple<DependencyProperty, string>> NodeProperties = new List<Tuple<DependencyProperty, string>>()
        {
			new Tuple<DependencyProperty, string>(Node.OffsetXProperty, OffsetX),
						new Tuple<DependencyProperty, string>(Node.OffsetYProperty, OffsetY),
						new Tuple<DependencyProperty, string>(Node.RotateAngleProperty, RotateAngle),
						new Tuple<DependencyProperty, string>(Node.SnapToObjectProperty, SnapToObject),
						new Tuple<DependencyProperty, string>(Node.FlipProperty, Flip),
						new Tuple<DependencyProperty, string>(Node.MinWidthProperty, MinWidth),
						new Tuple<DependencyProperty, string>(Node.MaxWidthProperty, MaxWidth),
						new Tuple<DependencyProperty, string>(Node.UnitWidthProperty, UnitWidth),
						new Tuple<DependencyProperty, string>(Node.MinHeightProperty, MinHeight),
						new Tuple<DependencyProperty, string>(Node.MaxHeightProperty, MaxHeight),
						new Tuple<DependencyProperty, string>(Node.UnitHeightProperty, UnitHeight),
						new Tuple<DependencyProperty, string>(Node.ContentProperty, Content),
						new Tuple<DependencyProperty, string>(Node.ContentTemplateProperty, ContentTemplate),
						new Tuple<DependencyProperty, string>(Node.ShapeProperty, Shape),
						new Tuple<DependencyProperty, string>(Node.ShapeStyleProperty, ShapeStyle),
						new Tuple<DependencyProperty, string>(Node.ParentGroupProperty, ParentGroup),
						new Tuple<DependencyProperty, string>(Node.IsExpandedProperty, IsExpanded),
						new Tuple<DependencyProperty, string>(Node.PivotProperty, Pivot),
						new Tuple<DependencyProperty, string>(Node.IDProperty, ID),
						new Tuple<DependencyProperty, string>(Node.KeyProperty, Key),
						new Tuple<DependencyProperty, string>(Node.IsSelectedProperty, IsSelected),
						new Tuple<DependencyProperty, string>(Node.ZIndexProperty, ZIndex),
						new Tuple<DependencyProperty, string>(Node.AnnotationsProperty, Annotations),
						new Tuple<DependencyProperty, string>(Node.ConstraintsProperty, Constraints),
						new Tuple<DependencyProperty, string>(Node.PortsProperty, Ports),
					};
	}
	internal static class GroupableConstants
	{
        public const string IsSelected = "IsSelected";
        public const string ZIndex = "ZIndex";
        public const string Annotations = "Annotations";
        public const string InternalAnnotations = "InternalAnnotations";
        public const string IsGrouped = "IsGrouped";
        public const string Info = "Info";
        public const string Bounds = "Bounds";
        public const string Center = "Center";
        public const string VirtualizationState = "VirtualizationState";
        public const string ParentGroup = "ParentGroup";
		public static List<Tuple<DependencyProperty, string>> GroupableProperties = new List<Tuple<DependencyProperty, string>>()
        {
		};
	}
	internal static class ConnectorConstants
	{
        public const string SourceNode = "SourceNode";
        public const string TargetNode = "TargetNode";
        public const string SourcePort = "SourcePort";
        public const string TargetPort = "TargetPort";
        public const string Segments = "Segments";
        public const string SourcePoint = "SourcePoint";
        public const string TargetPoint = "TargetPoint";
        public const string ConnectorGeometryStyle = "ConnectorGeometryStyle";
        public const string SourceDecorator = "SourceDecorator";
        public const string TargetDecorator = "TargetDecorator";
        public const string SourceDecoratorStyle = "SourceDecoratorStyle";
        public const string TargetDecoratorStyle = "TargetDecoratorStyle";
        public const string BridgeSpace = "BridgeSpace";
        public const string AutoBind = "AutoBind";
        public const string ID = "ID";
        public const string Key = "Key";
        public const string IsSelected = "IsSelected";
        public const string ZIndex = "ZIndex";
        public const string Annotations = "Annotations";
        public const string Constraints = "Constraints";
        public const string BezierSmoothness = "BezierSmoothness";
        public const string ParentGroup = "ParentGroup";
		public static List<Tuple<DependencyProperty, string>> ConnectorProperties = new List<Tuple<DependencyProperty, string>>()
        {
			new Tuple<DependencyProperty, string>(Connector.SourceNodeProperty, SourceNode),
						new Tuple<DependencyProperty, string>(Connector.TargetNodeProperty, TargetNode),
						new Tuple<DependencyProperty, string>(Connector.SourcePortProperty, SourcePort),
						new Tuple<DependencyProperty, string>(Connector.TargetPortProperty, TargetPort),
						new Tuple<DependencyProperty, string>(Connector.SegmentsProperty, Segments),
						new Tuple<DependencyProperty, string>(Connector.SourcePointProperty, SourcePoint),
						new Tuple<DependencyProperty, string>(Connector.TargetPointProperty, TargetPoint),
						new Tuple<DependencyProperty, string>(Connector.ConnectorGeometryStyleProperty, ConnectorGeometryStyle),
						new Tuple<DependencyProperty, string>(Connector.SourceDecoratorProperty, SourceDecorator),
						new Tuple<DependencyProperty, string>(Connector.TargetDecoratorProperty, TargetDecorator),
						new Tuple<DependencyProperty, string>(Connector.SourceDecoratorStyleProperty, SourceDecoratorStyle),
						new Tuple<DependencyProperty, string>(Connector.TargetDecoratorStyleProperty, TargetDecoratorStyle),
						new Tuple<DependencyProperty, string>(Connector.BridgeSpaceProperty, BridgeSpace),
						new Tuple<DependencyProperty, string>(Connector.IDProperty, ID),
						new Tuple<DependencyProperty, string>(Connector.KeyProperty, Key),
						new Tuple<DependencyProperty, string>(Connector.IsSelectedProperty, IsSelected),
						new Tuple<DependencyProperty, string>(Connector.ZIndexProperty, ZIndex),
						new Tuple<DependencyProperty, string>(Connector.AnnotationsProperty, Annotations),
						new Tuple<DependencyProperty, string>(Connector.ConstraintsProperty, Constraints),
						new Tuple<DependencyProperty, string>(Connector.BezierSmoothnessProperty, BezierSmoothness),
						new Tuple<DependencyProperty, string>(Connector.ParentGroupProperty, ParentGroup),
					};
	}
	internal static class GroupConstants
	{
        public const string OffsetX = "OffsetX";
        public const string OffsetY = "OffsetY";
        public const string RotateAngle = "RotateAngle";
        public const string MinWidth = "MinWidth";
        public const string MaxWidth = "MaxWidth";
        public const string MinHeight = "MinHeight";
        public const string MaxHeight = "MaxHeight";
        public const string Content = "Content";
        public const string ContentTemplate = "ContentTemplate";
        public const string Shape = "Shape";
        public const string ShapeStyle = "ShapeStyle";
        public const string Pivot = "Pivot";
        public const string ParentGroup = "ParentGroup";
        public const string Nodes = "Nodes";
        public const string Connectors = "Connectors";
        public const string Groups = "Groups";
        public const string Ports = "Ports";
        public const string ID = "ID";
        public const string Key = "Key";
        public const string IsSelected = "IsSelected";
        public const string ZIndex = "ZIndex";
        public const string Annotations = "Annotations";
        public const string Constraints = "Constraints";
        public const string AutoBind = "AutoBind";
        public const string InternalNodes = "InternalNodes";
        public const string InternalConnectors = "InternalConnectors";
        public const string InternalGroups = "InternalGroups";
		public static List<Tuple<DependencyProperty, string>> GroupProperties = new List<Tuple<DependencyProperty, string>>()
        {
			new Tuple<DependencyProperty, string>(Group.OffsetXProperty, OffsetX),
						new Tuple<DependencyProperty, string>(Group.OffsetYProperty, OffsetY),
						new Tuple<DependencyProperty, string>(Group.RotateAngleProperty, RotateAngle),
						new Tuple<DependencyProperty, string>(Group.MinWidthProperty, MinWidth),
						new Tuple<DependencyProperty, string>(Group.MaxWidthProperty, MaxWidth),
						new Tuple<DependencyProperty, string>(Group.MinHeightProperty, MinHeight),
						new Tuple<DependencyProperty, string>(Group.MaxHeightProperty, MaxHeight),
						new Tuple<DependencyProperty, string>(Group.ContentProperty, Content),
						new Tuple<DependencyProperty, string>(Group.ContentTemplateProperty, ContentTemplate),
						new Tuple<DependencyProperty, string>(Group.ShapeProperty, Shape),
						new Tuple<DependencyProperty, string>(Group.ShapeStyleProperty, ShapeStyle),
						new Tuple<DependencyProperty, string>(Group.PivotProperty, Pivot),
						new Tuple<DependencyProperty, string>(Group.ParentGroupProperty, ParentGroup),
						new Tuple<DependencyProperty, string>(Group.NodesProperty, Nodes),
						new Tuple<DependencyProperty, string>(Group.ConnectorsProperty, Connectors),
						new Tuple<DependencyProperty, string>(Group.GroupsProperty, Groups),
						new Tuple<DependencyProperty, string>(Group.PortsProperty, Ports),
						new Tuple<DependencyProperty, string>(Group.IDProperty, ID),
						new Tuple<DependencyProperty, string>(Group.KeyProperty, Key),
						new Tuple<DependencyProperty, string>(Group.IsSelectedProperty, IsSelected),
						new Tuple<DependencyProperty, string>(Group.ZIndexProperty, ZIndex),
						new Tuple<DependencyProperty, string>(Group.AnnotationsProperty, Annotations),
						new Tuple<DependencyProperty, string>(Group.ConstraintsProperty, Constraints),
					};
	}
	internal static class SelectorConstants
	{
        public const string QuickCommands = "QuickCommands";
		public static List<Tuple<DependencyProperty, string>> SelectorProperties = new List<Tuple<DependencyProperty, string>>()
        {
			new Tuple<DependencyProperty, string>(Selector.QuickCommandsProperty, QuickCommands),
					};
	}
	internal static class PageSettingsConstants
	{
        public const string PageWidth = "PageWidth";
        public const string PageHeight = "PageHeight";
        public const string MultiplePage = "MultiplePage";
        public const string OffPageMinMargin = "OffPageMinMargin";
        public const string OffPageMaxMargin = "OffPageMaxMargin";
        public const string PageOrientation = "PageOrientation";
        public const string PageBackground = "PageBackground";
        public const string PageBorderBrush = "PageBorderBrush";
        public const string PageBorderThickness = "PageBorderThickness";
        public const string Unit = "Unit";
        public const string ShowPageBreaks = "ShowPageBreaks";
        public const string PrintMargin = "PrintMargin";
		public static List<Tuple<DependencyProperty, string>> PageSettingsProperties = new List<Tuple<DependencyProperty, string>>()
        {
		};
	}
	internal static class SnapSettingsConstants
	{
        public const string HorizontalGridlines = "HorizontalGridlines";
        public const string VerticalGridlines = "VerticalGridlines";
        public const string SnapToObject = "SnapToObject";
        public const string SnapConstraints = "SnapConstraints";
        public const string SnapAngle = "SnapAngle";
		public static List<Tuple<DependencyProperty, string>> SnapSettingsProperties = new List<Tuple<DependencyProperty, string>>()
        {
		};
	}
	internal static class SfDiagramConstants
	{
        public const string Nodes = "Nodes";
        public const string Connectors = "Connectors";
        public const string Groups = "Groups";
        public const string DefaultConnectorType = "DefaultConnectorType";
        public const string SelectedItems = "SelectedItems";
        public const string Constraints = "Constraints";
        public const string BezierSmoothness = "BezierSmoothness";
        public const string Tool = "Tool";
        public const string DrawingTool = "DrawingTool";
        public const string MultipleSelectionMode = "MultipleSelectionMode";
        public const string ViewDictionary = "ViewDictionary";
        public const string Viewport = "Viewport";
        public const string Info = "Info";
        public const string LayoutManager = "LayoutManager";
        public const string KnownTypes = "KnownTypes";
        public const string SnapSettings = "SnapSettings";
        public const string PageSettings = "PageSettings";
        public const string ExportSettings = "ExportSettings";
        public const string PrintingService = "PrintingService";
        public const string InternalSelectedItems = "InternalSelectedItems";
        public const string HorizontalRuler = "HorizontalRuler";
        public const string VerticalRuler = "VerticalRuler";
		public static List<Tuple<DependencyProperty, string>> SfDiagramProperties = new List<Tuple<DependencyProperty, string>>()
        {
			new Tuple<DependencyProperty, string>(SfDiagram.NodesProperty, Nodes),
						new Tuple<DependencyProperty, string>(SfDiagram.ConnectorsProperty, Connectors),
						new Tuple<DependencyProperty, string>(SfDiagram.GroupsProperty, Groups),
						new Tuple<DependencyProperty, string>(SfDiagram.DefaultConnectorTypeProperty, DefaultConnectorType),
						new Tuple<DependencyProperty, string>(SfDiagram.SelectedItemsProperty, SelectedItems),
						new Tuple<DependencyProperty, string>(SfDiagram.ConstraintsProperty, Constraints),
						new Tuple<DependencyProperty, string>(SfDiagram.BezierSmoothnessProperty, BezierSmoothness),
						new Tuple<DependencyProperty, string>(SfDiagram.ToolProperty, Tool),
						new Tuple<DependencyProperty, string>(SfDiagram.DrawingToolProperty, DrawingTool),
						new Tuple<DependencyProperty, string>(SfDiagram.MultipleSelectionModeProperty, MultipleSelectionMode),
						new Tuple<DependencyProperty, string>(SfDiagram.ViewDictionaryProperty, ViewDictionary),
						new Tuple<DependencyProperty, string>(SfDiagram.LayoutManagerProperty, LayoutManager),
						new Tuple<DependencyProperty, string>(SfDiagram.KnownTypesProperty, KnownTypes),
						new Tuple<DependencyProperty, string>(SfDiagram.SnapSettingsProperty, SnapSettings),
						new Tuple<DependencyProperty, string>(SfDiagram.PageSettingsProperty, PageSettings),
			  #if SyncfusionFramework4_5_1 && WINRT		
					new Tuple<DependencyProperty, string>(SfDiagram.ExportSettingsProperty, ExportSettings),
				#endif
	  #if SyncfusionFramework4_5_1 && WINRT		
					new Tuple<DependencyProperty, string>(SfDiagram.PrintingServiceProperty, PrintingService),
				#endif
				new Tuple<DependencyProperty, string>(SfDiagram.HorizontalRulerProperty, HorizontalRuler),
						new Tuple<DependencyProperty, string>(SfDiagram.VerticalRulerProperty, VerticalRuler),
					};
	}
	internal static class DummyConstants
	{
        public const string Undo = "Undo";
        public const string Zoom = "Zoom";
        public const string Reset = "Reset";
        public const string SelectAll = "SelectAll";
        public const string Redo = "Redo";
        public const string Group = "Group";
        public const string UnGroup = "UnGroup";
        public const string SameSize = "SameSize";
        public const string SameHeight = "SameHeight";
        public const string SameWidth = "SameWidth";
        public const string AlignBottom = "AlignBottom";
        public const string AlignTop = "AlignTop";
        public const string AlignLeft = "AlignLeft";
        public const string AlignCenter = "AlignCenter";
        public const string AlignRight = "AlignRight";
        public const string AlignMiddle = "AlignMiddle";
        public const string SpaceAcross = "SpaceAcross";
        public const string SpaceDown = "SpaceDown";
        public const string SendToBack = "SendToBack";
        public const string SendBackward = "SendBackward";
        public const string BringToFront = "BringToFront";
        public const string BringForward = "BringForward";
        public const string MoveDown = "MoveDown";
        public const string MoveUp = "MoveUp";
        public const string MoveLeft = "MoveLeft";
        public const string MoveRight = "MoveRight";
        public const string Cut = "Cut";
        public const string Copy = "Copy";
        public const string Paste = "Paste";
        public const string Delete = "Delete";
        public const string Draw = "Draw";
        public const string Flip = "Flip";
        public const string FitToPage = "FitToPage";
        public const string Duplicate = "Duplicate";
		public static List<Tuple<DependencyProperty, string>> DummyProperties = new List<Tuple<DependencyProperty, string>>()
        {
		};
	}
}
