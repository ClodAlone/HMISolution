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
#if WINRT
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Media; 
#endif
#if !WINRT
using System.Windows.Media; 
#endif
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Layout;
using Syncfusion.UI.Xaml.Diagram.Controller;
namespace Syncfusion.UI.Xaml.Diagram
{
	internal partial class NodePortWrapper
	{

  		double _mNodeOffsetX = 0d;
		public double NodeOffsetX
        {
            get
            {
				double returnValue;
				if(_mSource is INodePort)
				{
                    returnValue = (_mSource as INodePort).NodeOffsetX;
				}
                else if(TryGetValue(NodePortConstants.NodeOffsetX, out returnValue))
				{
				}
				else
                {
                    returnValue = _mNodeOffsetX;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INodePort)
                {
                    (_mSource as INodePort).NodeOffsetX = value;
                }
                else if(SetValue(NodePortConstants.NodeOffsetX, value))
				{
				}
				else if(_mNodeOffsetX != value)
                {
                    _mNodeOffsetX = value;
                    OnPropertyChanged(NodePortConstants.NodeOffsetX);
                }
            }
        }
		
  		double _mNodeOffsetY = 0d;
		public double NodeOffsetY
        {
            get
            {
				double returnValue;
				if(_mSource is INodePort)
				{
                    returnValue = (_mSource as INodePort).NodeOffsetY;
				}
                else if(TryGetValue(NodePortConstants.NodeOffsetY, out returnValue))
				{
				}
				else
                {
                    returnValue = _mNodeOffsetY;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INodePort)
                {
                    (_mSource as INodePort).NodeOffsetY = value;
                }
                else if(SetValue(NodePortConstants.NodeOffsetY, value))
				{
				}
				else if(_mNodeOffsetY != value)
                {
                    _mNodeOffsetY = value;
                    OnPropertyChanged(NodePortConstants.NodeOffsetY);
                }
            }
        }
		
  		object _mNode = null;
		public object Node
        {
            get
            {
				object returnValue;
				if(_mSource is INodePort)
				{
                    returnValue = (_mSource as INodePort).Node;
				}
                else if(TryGetValue(NodePortConstants.Node, out returnValue))
				{
				}
				else
                {
                    returnValue = _mNode;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INodePort)
                {
                    (_mSource as INodePort).Node = value;
                }
                else if(SetValue(NodePortConstants.Node, value))
				{
				}
				else if(_mNode == null || !_mNode.Equals(value))
                {
                    _mNode = value;
                    OnPropertyChanged(NodePortConstants.Node);
                }
            }
        }
		
  		UnitMode _mUnitMode = UnitMode.Fraction;
		public UnitMode UnitMode
        {
            get
            {
				UnitMode returnValue;
				if(_mSource is INodePort)
				{
                    returnValue = (_mSource as INodePort).UnitMode;
				}
                else if(TryGetValue(NodePortConstants.UnitMode, out returnValue))
				{
				}
				else
                {
                    returnValue = _mUnitMode;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INodePort)
                {
                    (_mSource as INodePort).UnitMode = value;
                }
                else if(SetValue(NodePortConstants.UnitMode, value))
				{
				}
				else if(_mUnitMode != value)
                {
                    _mUnitMode = value;
                    OnPropertyChanged(NodePortConstants.UnitMode);
                }
            }
        }
		
  		object _mShape = null;
		public object Shape
        {
            get
            {
				object returnValue;
				if(_mSource is IPort)
				{
                    returnValue = (_mSource as IPort).Shape;
				}
                else if(TryGetValue(NodePortConstants.Shape, out returnValue))
				{
				}
				else
                {
                    returnValue = _mShape;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPort)
                {
                    (_mSource as IPort).Shape = value;
                }
                else if(SetValue(NodePortConstants.Shape, value))
				{
				}
				else if(_mShape != value)
                {
                    _mShape = value;
                    OnPropertyChanged(NodePortConstants.Shape);
                }
            }
        }
		
  		Style _mShapeStyle = null;
		public Style ShapeStyle
        {
            get
            {
				Style returnValue;
				if(_mSource is IPort)
				{
                    returnValue = (_mSource as IPort).ShapeStyle;
				}
                else if(TryGetValue(NodePortConstants.ShapeStyle, out returnValue))
				{
				}
				else
                {
                    returnValue = _mShapeStyle;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPort)
                {
                    (_mSource as IPort).ShapeStyle = value;
                }
                else if(SetValue(NodePortConstants.ShapeStyle, value))
				{
				}
				else if(_mShapeStyle != value)
                {
                    _mShapeStyle = value;
                    OnPropertyChanged(NodePortConstants.ShapeStyle);
                }
            }
        }
		
  		PortConstraints _mConstraints = PortConstraints.Inherit;
		public PortConstraints Constraints
        {
            get
            {
				PortConstraints returnValue;
				if(_mSource is IPort)
				{
                    returnValue = (_mSource as IPort).Constraints;
				}
                else if(TryGetValue(NodePortConstants.Constraints, out returnValue))
				{
				}
				else
                {
                    returnValue = _mConstraints;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPort)
                {
                    (_mSource as IPort).Constraints = value;
                }
                else if(SetValue(NodePortConstants.Constraints, value))
				{
				}
				else if(_mConstraints != value)
                {
                    _mConstraints = value;
                    OnPropertyChanged(NodePortConstants.Constraints);
                }
            }
        }
			}
	internal partial class AnnotationEditorWrapper
	{

  		object _mContent = null;
		public object Content
        {
            get
            {
				object returnValue;
				if(_mSource is IAnnotation)
				{
                    returnValue = (_mSource as IAnnotation).Content;
				}
                else if(TryGetValue(AnnotationEditorConstants.Content, out returnValue))
				{
				}
				else
                {
                    returnValue = _mContent;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IAnnotation)
                {
                    (_mSource as IAnnotation).Content = value;
                }
                else if(SetValue(AnnotationEditorConstants.Content, value))
				{
				}
				else if(_mContent != value)
                {
                    _mContent = value;
                    OnPropertyChanged(AnnotationEditorConstants.Content);
                }
            }
        }
		
  		DataTemplate _mEditTemplate = null;
		public DataTemplate EditTemplate
        {
            get
            {
				DataTemplate returnValue;
				if(_mSource is IAnnotation)
				{
                    returnValue = (_mSource as IAnnotation).EditTemplate;
				}
                else if(TryGetValue(AnnotationEditorConstants.EditTemplate, out returnValue))
				{
				}
				else
                {
                    returnValue = _mEditTemplate;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IAnnotation)
                {
                    (_mSource as IAnnotation).EditTemplate = value;
                }
                else if(SetValue(AnnotationEditorConstants.EditTemplate, value))
				{
				}
				else if(_mEditTemplate != value)
                {
                    _mEditTemplate = value;
                    OnPropertyChanged(AnnotationEditorConstants.EditTemplate);
                }
            }
        }
		
  		DataTemplate _mViewTemplate = null;
		public DataTemplate ViewTemplate
        {
            get
            {
				DataTemplate returnValue;
				if(_mSource is IAnnotation)
				{
                    returnValue = (_mSource as IAnnotation).ViewTemplate;
				}
                else if(TryGetValue(AnnotationEditorConstants.ViewTemplate, out returnValue))
				{
				}
				else
                {
                    returnValue = _mViewTemplate;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IAnnotation)
                {
                    (_mSource as IAnnotation).ViewTemplate = value;
                }
                else if(SetValue(AnnotationEditorConstants.ViewTemplate, value))
				{
				}
				else if(_mViewTemplate != value)
                {
                    _mViewTemplate = value;
                    OnPropertyChanged(AnnotationEditorConstants.ViewTemplate);
                }
            }
        }
		
  		ContentEditorMode _mMode = ContentEditorMode.View;
		public ContentEditorMode Mode
        {
            get
            {
				ContentEditorMode returnValue;
				if(_mSource is IAnnotation)
				{
                    returnValue = (_mSource as IAnnotation).Mode;
				}
                else if(TryGetValue(AnnotationEditorConstants.Mode, out returnValue))
				{
				}
				else
                {
                    returnValue = _mMode;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IAnnotation)
                {
                    (_mSource as IAnnotation).Mode = value;
                }
                else if(SetValue(AnnotationEditorConstants.Mode, value))
				{
				}
				else if(_mMode != value)
                {
                    _mMode = value;
                    OnPropertyChanged(AnnotationEditorConstants.Mode);
                }
            }
        }
		
  		HorizontalAlignment _mHorizontalAlignment = HorizontalAlignment.Center;
		public HorizontalAlignment HorizontalAlignment
        {
            get
            {
				HorizontalAlignment returnValue;
				if(_mSource is INodeAnnotation)
				{
                    returnValue = (_mSource as INodeAnnotation).HorizontalAlignment;
				}
                else if(TryGetValue(AnnotationEditorConstants.HorizontalAlignment, out returnValue))
				{
				}
				else
                {
                    returnValue = _mHorizontalAlignment;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INodeAnnotation)
                {
                    (_mSource as INodeAnnotation).HorizontalAlignment = value;
                }
                else if(SetValue(AnnotationEditorConstants.HorizontalAlignment, value))
				{
				}
				else if(_mHorizontalAlignment != value)
                {
                    _mHorizontalAlignment = value;
                    OnPropertyChanged(AnnotationEditorConstants.HorizontalAlignment);
                }
            }
        }
		
  		VerticalAlignment _mVerticalAlignment = VerticalAlignment.Center;
		public VerticalAlignment VerticalAlignment
        {
            get
            {
				VerticalAlignment returnValue;
				if(_mSource is INodeAnnotation)
				{
                    returnValue = (_mSource as INodeAnnotation).VerticalAlignment;
				}
                else if(TryGetValue(AnnotationEditorConstants.VerticalAlignment, out returnValue))
				{
				}
				else
                {
                    returnValue = _mVerticalAlignment;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INodeAnnotation)
                {
                    (_mSource as INodeAnnotation).VerticalAlignment = value;
                }
                else if(SetValue(AnnotationEditorConstants.VerticalAlignment, value))
				{
				}
				else if(_mVerticalAlignment != value)
                {
                    _mVerticalAlignment = value;
                    OnPropertyChanged(AnnotationEditorConstants.VerticalAlignment);
                }
            }
        }
		
  		ConnectorAnnotationAlignment _mAlignment = ConnectorAnnotationAlignment.Center;
		public ConnectorAnnotationAlignment Alignment
        {
            get
            {
				ConnectorAnnotationAlignment returnValue;
				if(_mSource is IConnectorAnnotation)
				{
                    returnValue = (_mSource as IConnectorAnnotation).Alignment;
				}
                else if(TryGetValue(AnnotationEditorConstants.Alignment, out returnValue))
				{
				}
				else
                {
                    returnValue = _mAlignment;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnectorAnnotation)
                {
                    (_mSource as IConnectorAnnotation).Alignment = value;
                }
                else if(SetValue(AnnotationEditorConstants.Alignment, value))
				{
				}
				else if(_mAlignment != value)
                {
                    _mAlignment = value;
                    OnPropertyChanged(AnnotationEditorConstants.Alignment);
                }
            }
        }
			}
	internal partial class DiagramElementWrapper
	{

  		object _mID = null;
		public object ID
        {
            get
            {
				object returnValue;
				if(_mSource is IDiagramElement)
				{
                    returnValue = (_mSource as IDiagramElement).ID;
				}
                else if(TryGetValue(DiagramElementConstants.ID, out returnValue))
				{
				}
				else
                {
                    returnValue = _mID;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IDiagramElement)
                {
                    (_mSource as IDiagramElement).ID = value;
                }
                else if(SetValue(DiagramElementConstants.ID, value))
				{
				}
				else if(_mID == null || !_mID.Equals(value))
                {
                    _mID = value;
                    OnPropertyChanged(DiagramElementConstants.ID);
                }
            }
        }
		
  		object _mKey = null;
		public object Key
        {
            get
            {
				object returnValue;
				if(_mSource is IDiagramElement)
				{
                    returnValue = (_mSource as IDiagramElement).Key;
				}
                else if(TryGetValue(DiagramElementConstants.Key, out returnValue))
				{
				}
				else
                {
                    returnValue = _mKey;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IDiagramElement)
                {
                    (_mSource as IDiagramElement).Key = value;
                }
                else if(SetValue(DiagramElementConstants.Key, value))
				{
				}
				else if(_mKey != value)
                {
                    _mKey = value;
                    OnPropertyChanged(DiagramElementConstants.Key);
                }
            }
        }
			}
	internal partial class NodeWrapper
	{

  		double _mOffsetX = 0d;
		public double OffsetX
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).OffsetX;
				}
                else if(TryGetValue(NodeConstants.OffsetX, out returnValue))
				{
				}
				else
                {
                    returnValue = _mOffsetX;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is INode)
                {
                    (_mSource as INode).OffsetX = value;
                }
                else if(SetValue(NodeConstants.OffsetX, value))
				{
				}
				else if(_mOffsetX != value)
                {
                    _mOffsetX = value;
                    OnPropertyChanged(NodeConstants.OffsetX);
                }
            }
        }
		
  		double _mOffsetY = 0d;
		public double OffsetY
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).OffsetY;
				}
                else if(TryGetValue(NodeConstants.OffsetY, out returnValue))
				{
				}
				else
                {
                    returnValue = _mOffsetY;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is INode)
                {
                    (_mSource as INode).OffsetY = value;
                }
                else if(SetValue(NodeConstants.OffsetY, value))
				{
				}
				else if(_mOffsetY != value)
                {
                    _mOffsetY = value;
                    OnPropertyChanged(NodeConstants.OffsetY);
                }
            }
        }
		
  		double _mRotateAngle = 0d;
		public double RotateAngle
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).RotateAngle;
				}
                else if(TryGetValue(NodeConstants.RotateAngle, out returnValue))
				{
				}
				else
                {
                    returnValue = _mRotateAngle;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).RotateAngle = value;
                }
                else if(SetValue(NodeConstants.RotateAngle, value))
				{
				}
				else if(_mRotateAngle != value)
                {
                    _mRotateAngle = value;
                    OnPropertyChanged(NodeConstants.RotateAngle);
                }
            }
        }
		
  		SnapToObject _mSnapToObject = SnapToObject.None;
		public SnapToObject SnapToObject
        {
            get
            {
				SnapToObject returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).SnapToObject;
				}
                else if(TryGetValue(NodeConstants.SnapToObject, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSnapToObject;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).SnapToObject = value;
                }
                else if(SetValue(NodeConstants.SnapToObject, value))
				{
				}
				else if(_mSnapToObject != value)
                {
                    _mSnapToObject = value;
                    OnPropertyChanged(NodeConstants.SnapToObject);
                }
            }
        }
		
  		Flip _mFlip = Flip.None;
		public Flip Flip
        {
            get
            {
				Flip returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).Flip;
				}
                else if(TryGetValue(NodeConstants.Flip, out returnValue))
				{
				}
				else
                {
                    returnValue = _mFlip;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).Flip = value;
                }
                else if(SetValue(NodeConstants.Flip, value))
				{
				}
				else if(_mFlip != value)
                {
                    _mFlip = value;
                    OnPropertyChanged(NodeConstants.Flip);
                }
            }
        }
		
  		double _mMinWidth = 0d;
		public double MinWidth
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).MinWidth;
				}
                else if(TryGetValue(NodeConstants.MinWidth, out returnValue))
				{
				}
				else
                {
                    returnValue = _mMinWidth;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).MinWidth = value;
                }
                else if(SetValue(NodeConstants.MinWidth, value))
				{
				}
				else if(_mMinWidth != value)
                {
                    _mMinWidth = value;
                    OnPropertyChanged(NodeConstants.MinWidth);
                }
            }
        }
		
  		double _mMaxWidth = double.PositiveInfinity;
		public double MaxWidth
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).MaxWidth;
				}
                else if(TryGetValue(NodeConstants.MaxWidth, out returnValue))
				{
				}
				else
                {
                    returnValue = _mMaxWidth;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).MaxWidth = value;
                }
                else if(SetValue(NodeConstants.MaxWidth, value))
				{
				}
				else if(_mMaxWidth != value)
                {
                    _mMaxWidth = value;
                    OnPropertyChanged(NodeConstants.MaxWidth);
                }
            }
        }
		
  		double _mUnitWidth = double.NaN;
		public double UnitWidth
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).UnitWidth;
				}
                else if(TryGetValue(NodeConstants.UnitWidth, out returnValue))
				{
				}
				else
                {
                    returnValue = _mUnitWidth;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is INode)
                {
                    (_mSource as INode).UnitWidth = value;
                }
                else if(SetValue(NodeConstants.UnitWidth, value))
				{
				}
				else if(_mUnitWidth != value)
                {
                    _mUnitWidth = value;
                    OnPropertyChanged(NodeConstants.UnitWidth);
                }
            }
        }
		
  		double _mMinHeight = 0d;
		public double MinHeight
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).MinHeight;
				}
                else if(TryGetValue(NodeConstants.MinHeight, out returnValue))
				{
				}
				else
                {
                    returnValue = _mMinHeight;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).MinHeight = value;
                }
                else if(SetValue(NodeConstants.MinHeight, value))
				{
				}
				else if(_mMinHeight != value)
                {
                    _mMinHeight = value;
                    OnPropertyChanged(NodeConstants.MinHeight);
                }
            }
        }
		
  		double _mMaxHeight = double.PositiveInfinity;
		public double MaxHeight
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).MaxHeight;
				}
                else if(TryGetValue(NodeConstants.MaxHeight, out returnValue))
				{
				}
				else
                {
                    returnValue = _mMaxHeight;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).MaxHeight = value;
                }
                else if(SetValue(NodeConstants.MaxHeight, value))
				{
				}
				else if(_mMaxHeight != value)
                {
                    _mMaxHeight = value;
                    OnPropertyChanged(NodeConstants.MaxHeight);
                }
            }
        }
		
  		double _mUnitHeight = double.NaN;
		public double UnitHeight
        {
            get
            {
				double returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).UnitHeight;
				}
                else if(TryGetValue(NodeConstants.UnitHeight, out returnValue))
				{
				}
				else
                {
                    returnValue = _mUnitHeight;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is INode)
                {
                    (_mSource as INode).UnitHeight = value;
                }
                else if(SetValue(NodeConstants.UnitHeight, value))
				{
				}
				else if(_mUnitHeight != value)
                {
                    _mUnitHeight = value;
                    OnPropertyChanged(NodeConstants.UnitHeight);
                }
            }
        }
		
  		object _mContent = null;
		public object Content
        {
            get
            {
				object returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).Content;
				}
                else if(TryGetValue(NodeConstants.Content, out returnValue))
				{
				}
				else
                {
                    returnValue = _mContent;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).Content = value;
                }
                else if(SetValue(NodeConstants.Content, value))
				{
				}
				else if(_mContent != value)
                {
                    _mContent = value;
                    OnPropertyChanged(NodeConstants.Content);
                }
            }
        }
		
  		DataTemplate _mContentTemplate = null;
		public DataTemplate ContentTemplate
        {
            get
            {
				DataTemplate returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).ContentTemplate;
				}
                else if(TryGetValue(NodeConstants.ContentTemplate, out returnValue))
				{
				}
				else
                {
                    returnValue = _mContentTemplate;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).ContentTemplate = value;
                }
                else if(SetValue(NodeConstants.ContentTemplate, value))
				{
				}
				else if(_mContentTemplate != value)
                {
                    _mContentTemplate = value;
                    OnPropertyChanged(NodeConstants.ContentTemplate);
                }
            }
        }
		
  		object _mShape = null;
		public object Shape
        {
            get
            {
				object returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).Shape;
				}
                else if(TryGetValue(NodeConstants.Shape, out returnValue))
				{
				}
				else
                {
                    returnValue = _mShape;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).Shape = value;
                }
                else if(SetValue(NodeConstants.Shape, value))
				{
				}
				else if(_mShape != value)
                {
                    _mShape = value;
                    OnPropertyChanged(NodeConstants.Shape);
                }
            }
        }
		
  		Style _mShapeStyle = null;
		public Style ShapeStyle
        {
            get
            {
				Style returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).ShapeStyle;
				}
                else if(TryGetValue(NodeConstants.ShapeStyle, out returnValue))
				{
				}
				else
                {
                    returnValue = _mShapeStyle;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).ShapeStyle = value;
                }
                else if(SetValue(NodeConstants.ShapeStyle, value))
				{
				}
				else if(_mShapeStyle != value)
                {
                    _mShapeStyle = value;
                    OnPropertyChanged(NodeConstants.ShapeStyle);
                }
            }
        }
		
  		bool _mIsExpanded = true;
		public bool IsExpanded
        {
            get
            {
				bool returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).IsExpanded;
				}
                else if(TryGetValue(NodeConstants.IsExpanded, out returnValue))
				{
				}
				else
                {
                    returnValue = _mIsExpanded;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).IsExpanded = value;
                }
                else if(SetValue(NodeConstants.IsExpanded, value))
				{
				}
				else if(_mIsExpanded != value)
                {
                    _mIsExpanded = value;
                    OnPropertyChanged(NodeConstants.IsExpanded);
                }
            }
        }
		
  		Point _mPivot = new Point(0.5, 0.5);
		public Point Pivot
        {
            get
            {
				Point returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).Pivot;
				}
                else if(TryGetValue(NodeConstants.Pivot, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPivot;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).Pivot = value;
                }
                else if(SetValue(NodeConstants.Pivot, value))
				{
				}
				else if(_mPivot != value)
                {
                    _mPivot = value;
                    OnPropertyChanged(NodeConstants.Pivot);
                }
            }
        }
		
  		NodeConstraints _mConstraints = NodeConstraints.Default;
		public NodeConstraints Constraints
        {
            get
            {
				NodeConstraints returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).Constraints;
				}
                else if(TryGetValue(NodeConstants.Constraints, out returnValue))
				{
				}
				else
                {
                    returnValue = _mConstraints;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).Constraints = value;
                }
                else if(SetValue(NodeConstants.Constraints, value))
				{
				}
				else if(_mConstraints != value)
                {
                    _mConstraints = value;
                    OnPropertyChanged(NodeConstants.Constraints);
                }
            }
        }
		
  		object _mPorts = null;
		public object Ports
        {
            get
            {
				object returnValue;
				if(_mSource is INode)
				{
                    returnValue = (_mSource as INode).Ports;
				}
                else if(TryGetValue(NodeConstants.Ports, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPorts;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is INode)
                {
                    (_mSource as INode).Ports = value;
                }
                else if(SetValue(NodeConstants.Ports, value))
				{
				}
				else if(_mPorts != value)
                {
                    _mPorts = value;
                    OnPropertyChanged(NodeConstants.Ports);
                }
            }
        }
		
  		ObservableElements<INodePort, IInternalNodePort> _mInternalPorts = null;
		public ObservableElements<INodePort, IInternalNodePort> InternalPorts
        {
			get
			{
				return _mInternalPorts;
			}
			set
			{
				if(_mInternalPorts != value)
				{
					_mInternalPorts = value;
					OnPropertyChanged(NodeConstants.InternalPorts);
				}
			}
        }
			}
	internal partial class GroupableWrapper
	{

  		bool _mIsSelected = false;
		public bool IsSelected
        {
            get
            {
				bool returnValue;
				if(_mSource is IGroupable)
				{
                    returnValue = (_mSource as IGroupable).IsSelected;
				}
                else if(TryGetValue(GroupableConstants.IsSelected, out returnValue))
				{
				}
				else
                {
                    returnValue = _mIsSelected;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGroupable)
                {
                    (_mSource as IGroupable).IsSelected = value;
                }
                else if(SetValue(GroupableConstants.IsSelected, value))
				{
				}
				else if(_mIsSelected != value)
                {
                    _mIsSelected = value;
                    OnPropertyChanged(GroupableConstants.IsSelected);
                }
            }
        }
		
  		int _mZIndex = 0;
		public int ZIndex
        {
            get
            {
				int returnValue;
				if(_mSource is IGroupable)
				{
                    returnValue = (_mSource as IGroupable).ZIndex;
				}
                else if(TryGetValue(GroupableConstants.ZIndex, out returnValue))
				{
				}
				else
                {
                    returnValue = _mZIndex;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGroupable)
                {
                    (_mSource as IGroupable).ZIndex = value;
                }
                else if(SetValue(GroupableConstants.ZIndex, value))
				{
				}
				else if(_mZIndex != value)
                {
                    _mZIndex = value;
                    OnPropertyChanged(GroupableConstants.ZIndex);
                }
            }
        }
		
  		object _mAnnotations = null;
		public object Annotations
        {
            get
            {
				object returnValue;
				if(_mSource is IGroupable)
				{
                    returnValue = (_mSource as IGroupable).Annotations;
				}
                else if(TryGetValue(GroupableConstants.Annotations, out returnValue))
				{
				}
				else
                {
                    returnValue = _mAnnotations;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGroupable)
                {
                    (_mSource as IGroupable).Annotations = value;
                }
                else if(SetValue(GroupableConstants.Annotations, value))
				{
				}
				else if(_mAnnotations != value)
                {
                    _mAnnotations = value;
                    OnPropertyChanged(GroupableConstants.Annotations);
                }
            }
        }
		
  		ObservableElements<IAnnotation, AnnotationEditorWrapper> _mInternalAnnotations = null;
		public ObservableElements<IAnnotation, AnnotationEditorWrapper> InternalAnnotations
        {
			get
			{
				return _mInternalAnnotations;
			}
			set
			{
				if(_mInternalAnnotations != value)
				{
					_mInternalAnnotations = value;
					OnPropertyChanged(GroupableConstants.InternalAnnotations);
				}
			}
        }
		
  		bool _mIsGrouped = false;
		public bool IsGrouped
        {
			get
			{
				return _mIsGrouped;
			}
			set
			{
				if(_mIsGrouped != value)
				{
					_mIsGrouped = value;
					OnPropertyChanged(GroupableConstants.IsGrouped);
				}
			}
        }
		
  		Rect _mBounds = Rect.Empty;
		public Rect Bounds
        {
			get
			{
				return _mBounds;
			}
			set
			{
				if(_mBounds != value)
				{
					_mBounds = value;
					OnPropertyChanged(GroupableConstants.Bounds);
				}
			}
        }
		
  		Point _mCenter = new Point(0, 0);
		public Point Center
        {
			get
			{
				return _mCenter;
			}
			set
			{
				if(_mCenter != value)
				{
					_mCenter = value;
				}
			}
        }
		
  		VirtualizationState _mVirtualizationState = VirtualizationState.Virtualized;
		public VirtualizationState VirtualizationState
        {
			get
			{
				return _mVirtualizationState;
			}
			set
			{
				if(_mVirtualizationState != value)
				{
					_mVirtualizationState = value;
					OnPropertyChanged(GroupableConstants.VirtualizationState);
				}
			}
        }
		
  		object _mParentGroup = null;
		public object ParentGroup
        {
            get
            {
				object returnValue;
				if(_mSource is IGroupable)
				{
                    returnValue = (_mSource as IGroupable).ParentGroup;
				}
                else if(TryGetValue(GroupableConstants.ParentGroup, out returnValue))
				{
				}
				else
                {
                    returnValue = _mParentGroup;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGroupable)
                {
                    (_mSource as IGroupable).ParentGroup = value;
                }
                else if(SetValue(GroupableConstants.ParentGroup, value))
				{
				}
				else if(_mParentGroup == null || !_mParentGroup.Equals(value))
                {
                    _mParentGroup = value;
                    OnPropertyChanged(GroupableConstants.ParentGroup);
                }
            }
        }
			}
	internal partial class ConnectorWrapper
	{

  		object _mSourceNode = null;
		public object SourceNode
        {
            get
            {
				object returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).SourceNode;
				}
                else if(TryGetValue(ConnectorConstants.SourceNode, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSourceNode;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).SourceNode = value;
                }
                else if(SetValue(ConnectorConstants.SourceNode, value))
				{
				}
				else if(_mSourceNode == null || !_mSourceNode.Equals(value))
                {
                    _mSourceNode = value;
                    OnPropertyChanged(ConnectorConstants.SourceNode);
                }
            }
        }
		
  		object _mTargetNode = null;
		public object TargetNode
        {
            get
            {
				object returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).TargetNode;
				}
                else if(TryGetValue(ConnectorConstants.TargetNode, out returnValue))
				{
				}
				else
                {
                    returnValue = _mTargetNode;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).TargetNode = value;
                }
                else if(SetValue(ConnectorConstants.TargetNode, value))
				{
				}
				else if(_mTargetNode == null || !_mTargetNode.Equals(value))
                {
                    _mTargetNode = value;
                    OnPropertyChanged(ConnectorConstants.TargetNode);
                }
            }
        }
		
  		IPort _mSourcePort = null;
		public IPort SourcePort
        {
            get
            {
				IPort returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).SourcePort;
				}
                else if(TryGetValue(ConnectorConstants.SourcePort, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSourcePort;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).SourcePort = value;
                }
                else if(SetValue(ConnectorConstants.SourcePort, value))
				{
				}
				else if(_mSourcePort != value)
                {
                    _mSourcePort = value;
                    OnPropertyChanged(ConnectorConstants.SourcePort);
                }
            }
        }
		
  		IPort _mTargetPort = null;
		public IPort TargetPort
        {
            get
            {
				IPort returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).TargetPort;
				}
                else if(TryGetValue(ConnectorConstants.TargetPort, out returnValue))
				{
				}
				else
                {
                    returnValue = _mTargetPort;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).TargetPort = value;
                }
                else if(SetValue(ConnectorConstants.TargetPort, value))
				{
				}
				else if(_mTargetPort != value)
                {
                    _mTargetPort = value;
                    OnPropertyChanged(ConnectorConstants.TargetPort);
                }
            }
        }
		
  		IList<IConnectorSegment> _mSegments = null;
		public IList<IConnectorSegment> Segments
        {
            get
            {
				IList<IConnectorSegment> returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).Segments;
				}
                else if(TryGetValue(ConnectorConstants.Segments, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSegments;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).Segments = value;
                }
                else if(SetValue(ConnectorConstants.Segments, value))
				{
				}
				else if(_mSegments != value)
                {
                    _mSegments = value;
                    OnPropertyChanged(ConnectorConstants.Segments);
                }
            }
        }
		
  		Point _mSourcePoint = new Point(0,0);
		public Point SourcePoint
        {
            get
            {
				Point returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).SourcePoint;
				}
                else if(TryGetValue(ConnectorConstants.SourcePoint, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSourcePoint;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).SourcePoint = value;
                }
                else if(SetValue(ConnectorConstants.SourcePoint, value))
				{
				}
				else if(_mSourcePoint != value)
                {
                    _mSourcePoint = value;
                    OnPropertyChanged(ConnectorConstants.SourcePoint);
                }
            }
        }
		
  		Point _mTargetPoint = new Point(0,0);
		public Point TargetPoint
        {
            get
            {
				Point returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).TargetPoint;
				}
                else if(TryGetValue(ConnectorConstants.TargetPoint, out returnValue))
				{
				}
				else
                {
                    returnValue = _mTargetPoint;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).TargetPoint = value;
                }
                else if(SetValue(ConnectorConstants.TargetPoint, value))
				{
				}
				else if(_mTargetPoint != value)
                {
                    _mTargetPoint = value;
                    OnPropertyChanged(ConnectorConstants.TargetPoint);
                }
            }
        }
		
  		Style _mConnectorGeometryStyle = null;
		public Style ConnectorGeometryStyle
        {
            get
            {
				Style returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).ConnectorGeometryStyle;
				}
                else if(TryGetValue(ConnectorConstants.ConnectorGeometryStyle, out returnValue))
				{
				}
				else
                {
                    returnValue = _mConnectorGeometryStyle;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).ConnectorGeometryStyle = value;
                }
                else if(SetValue(ConnectorConstants.ConnectorGeometryStyle, value))
				{
				}
				else if(_mConnectorGeometryStyle != value)
                {
                    _mConnectorGeometryStyle = value;
                    OnPropertyChanged(ConnectorConstants.ConnectorGeometryStyle);
                }
            }
        }
		
  		object _mSourceDecorator = null;
		public object SourceDecorator
        {
            get
            {
				object returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).SourceDecorator;
				}
                else if(TryGetValue(ConnectorConstants.SourceDecorator, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSourceDecorator;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).SourceDecorator = value;
                }
                else if(SetValue(ConnectorConstants.SourceDecorator, value))
				{
				}
				else if(_mSourceDecorator != value)
                {
                    _mSourceDecorator = value;
                    OnPropertyChanged(ConnectorConstants.SourceDecorator);
                }
            }
        }
		
  		object _mTargetDecorator = "M0,0 L10,5 L0,10 L 0,0";
		public object TargetDecorator
        {
            get
            {
				object returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).TargetDecorator;
				}
                else if(TryGetValue(ConnectorConstants.TargetDecorator, out returnValue))
				{
				}
				else
                {
                    returnValue = _mTargetDecorator;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).TargetDecorator = value;
                }
                else if(SetValue(ConnectorConstants.TargetDecorator, value))
				{
				}
				else if(_mTargetDecorator != value)
                {
                    _mTargetDecorator = value;
                    OnPropertyChanged(ConnectorConstants.TargetDecorator);
                }
            }
        }
		
  		Style _mSourceDecoratorStyle = null;
		public Style SourceDecoratorStyle
        {
            get
            {
				Style returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).SourceDecoratorStyle;
				}
                else if(TryGetValue(ConnectorConstants.SourceDecoratorStyle, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSourceDecoratorStyle;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).SourceDecoratorStyle = value;
                }
                else if(SetValue(ConnectorConstants.SourceDecoratorStyle, value))
				{
				}
				else if(_mSourceDecoratorStyle != value)
                {
                    _mSourceDecoratorStyle = value;
                    OnPropertyChanged(ConnectorConstants.SourceDecoratorStyle);
                }
            }
        }
		
  		Style _mTargetDecoratorStyle = null;
		public Style TargetDecoratorStyle
        {
            get
            {
				Style returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).TargetDecoratorStyle;
				}
                else if(TryGetValue(ConnectorConstants.TargetDecoratorStyle, out returnValue))
				{
				}
				else
                {
                    returnValue = _mTargetDecoratorStyle;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).TargetDecoratorStyle = value;
                }
                else if(SetValue(ConnectorConstants.TargetDecoratorStyle, value))
				{
				}
				else if(_mTargetDecoratorStyle != value)
                {
                    _mTargetDecoratorStyle = value;
                    OnPropertyChanged(ConnectorConstants.TargetDecoratorStyle);
                }
            }
        }
		
  		double _mBridgeSpace = 15d;
		public double BridgeSpace
        {
            get
            {
				double returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).BridgeSpace;
				}
                else if(TryGetValue(ConnectorConstants.BridgeSpace, out returnValue))
				{
				}
				else
                {
                    returnValue = _mBridgeSpace;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).BridgeSpace = value;
                }
                else if(SetValue(ConnectorConstants.BridgeSpace, value))
				{
				}
				else if(_mBridgeSpace != value)
                {
                    _mBridgeSpace = value;
                    OnPropertyChanged(ConnectorConstants.BridgeSpace);
                }
            }
        }
		
  		ConnectorConstraints _mConstraints = ConnectorConstraints.Default;
		public ConnectorConstraints Constraints
        {
            get
            {
				ConnectorConstraints returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).Constraints;
				}
                else if(TryGetValue(ConnectorConstants.Constraints, out returnValue))
				{
				}
				else
                {
                    returnValue = _mConstraints;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).Constraints = value;
                }
                else if(SetValue(ConnectorConstants.Constraints, value))
				{
				}
				else if(_mConstraints != value)
                {
                    _mConstraints = value;
                    OnPropertyChanged(ConnectorConstants.Constraints);
                }
            }
        }
		
  		BezierSmoothness _mBezierSmoothness = BezierSmoothness.None;
		public BezierSmoothness BezierSmoothness
        {
            get
            {
				BezierSmoothness returnValue;
				if(_mSource is IConnector)
				{
                    returnValue = (_mSource as IConnector).BezierSmoothness;
				}
                else if(TryGetValue(ConnectorConstants.BezierSmoothness, out returnValue))
				{
				}
				else
                {
                    returnValue = _mBezierSmoothness;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IConnector)
                {
                    (_mSource as IConnector).BezierSmoothness = value;
                }
                else if(SetValue(ConnectorConstants.BezierSmoothness, value))
				{
				}
				else if(_mBezierSmoothness != value)
                {
                    _mBezierSmoothness = value;
                    OnPropertyChanged(ConnectorConstants.BezierSmoothness);
                }
            }
        }
			}
	internal partial class GroupWrapper
	{

  		object _mNodes = null;
		public object Nodes
        {
            get
            {
				object returnValue;
				if(_mSource is IGroup)
				{
                    returnValue = (_mSource as IGroup).Nodes;
				}
                else if(TryGetValue(GroupConstants.Nodes, out returnValue))
				{
				}
				else
                {
                    returnValue = _mNodes;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGroup)
                {
                    (_mSource as IGroup).Nodes = value;
                }
                else if(SetValue(GroupConstants.Nodes, value))
				{
				}
				else if(_mNodes != value)
                {
                    _mNodes = value;
                    OnPropertyChanged(GroupConstants.Nodes);
                }
            }
        }
		
  		object _mConnectors = null;
		public object Connectors
        {
            get
            {
				object returnValue;
				if(_mSource is IGroup)
				{
                    returnValue = (_mSource as IGroup).Connectors;
				}
                else if(TryGetValue(GroupConstants.Connectors, out returnValue))
				{
				}
				else
                {
                    returnValue = _mConnectors;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGroup)
                {
                    (_mSource as IGroup).Connectors = value;
                }
                else if(SetValue(GroupConstants.Connectors, value))
				{
				}
				else if(_mConnectors != value)
                {
                    _mConnectors = value;
                    OnPropertyChanged(GroupConstants.Connectors);
                }
            }
        }
		
  		object _mGroups = null;
		public object Groups
        {
            get
            {
				object returnValue;
				if(_mSource is IGroup)
				{
                    returnValue = (_mSource as IGroup).Groups;
				}
                else if(TryGetValue(GroupConstants.Groups, out returnValue))
				{
				}
				else
                {
                    returnValue = _mGroups;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGroup)
                {
                    (_mSource as IGroup).Groups = value;
                }
                else if(SetValue(GroupConstants.Groups, value))
				{
				}
				else if(_mGroups != value)
                {
                    _mGroups = value;
                    OnPropertyChanged(GroupConstants.Groups);
                }
            }
        }
		
  		ObservableElements<object, IInternalNode> _mInternalNodes = null;
		public ObservableElements<object, IInternalNode> InternalNodes
        {
			get
			{
				return _mInternalNodes;
			}
			set
			{
				if(_mInternalNodes != value)
				{
					_mInternalNodes = value;
					OnPropertyChanged(GroupConstants.InternalNodes);
				}
			}
        }
		
  		ObservableElements<object, IInternalConnector> _mInternalConnectors = null;
		public ObservableElements<object, IInternalConnector> InternalConnectors
        {
			get
			{
				return _mInternalConnectors;
			}
			set
			{
				if(_mInternalConnectors != value)
				{
					_mInternalConnectors = value;
					OnPropertyChanged(GroupConstants.InternalConnectors);
				}
			}
        }
		
  		ObservableElements<object, IInternalGroup> _mInternalGroups = null;
		public ObservableElements<object, IInternalGroup> InternalGroups
        {
			get
			{
				return _mInternalGroups;
			}
			set
			{
				if(_mInternalGroups != value)
				{
					_mInternalGroups = value;
					OnPropertyChanged(GroupConstants.InternalGroups);
				}
			}
        }
			}
	internal partial class SelectorWrapper
	{

  		Visibility _mQuickCommands = Visibility.Visible;
		public Visibility QuickCommands
        {
            get
            {
				Visibility returnValue;
				if(_mSource is ISelector)
				{
                    returnValue = (_mSource as ISelector).QuickCommands;
				}
                else if(TryGetValue(SelectorConstants.QuickCommands, out returnValue))
				{
				}
				else
                {
                    returnValue = _mQuickCommands;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is ISelector)
                {
                    (_mSource as ISelector).QuickCommands = value;
                }
                else if(SetValue(SelectorConstants.QuickCommands, value))
				{
				}
				else if(_mQuickCommands != value)
                {
                    _mQuickCommands = value;
                    OnPropertyChanged(SelectorConstants.QuickCommands);
                }
            }
        }
			}
	internal partial class PageSettingsWrapper
	{

  		double _mPageWidth = double.NaN;
		public double PageWidth
        {
            get
            {
				double returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).PageWidth;
				}
                else if(TryGetValue(PageSettingsConstants.PageWidth, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPageWidth;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).PageWidth = value;
                }
                else if(SetValue(PageSettingsConstants.PageWidth, value))
				{
				}
				else if(_mPageWidth != value)
                {
                    _mPageWidth = value;
                    OnPropertyChanged(PageSettingsConstants.PageWidth);
                }
            }
        }
		
  		double _mPageHeight = double.NaN;
		public double PageHeight
        {
            get
            {
				double returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).PageHeight;
				}
                else if(TryGetValue(PageSettingsConstants.PageHeight, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPageHeight;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).PageHeight = value;
                }
                else if(SetValue(PageSettingsConstants.PageHeight, value))
				{
				}
				else if(_mPageHeight != value)
                {
                    _mPageHeight = value;
                    OnPropertyChanged(PageSettingsConstants.PageHeight);
                }
            }
        }
		
  		bool _mMultiplePage = false;
		public bool MultiplePage
        {
            get
            {
				bool returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).MultiplePage;
				}
                else if(TryGetValue(PageSettingsConstants.MultiplePage, out returnValue))
				{
				}
				else
                {
                    returnValue = _mMultiplePage;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).MultiplePage = value;
                }
                else if(SetValue(PageSettingsConstants.MultiplePage, value))
				{
				}
				else if(_mMultiplePage != value)
                {
                    _mMultiplePage = value;
                    OnPropertyChanged(PageSettingsConstants.MultiplePage);
                }
            }
        }
		
  		Thickness? _mOffPageMinMargin = null;
		public Thickness? OffPageMinMargin
        {
            get
            {
				Thickness? returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).OffPageMinMargin;
				}
                else if(TryGetValue(PageSettingsConstants.OffPageMinMargin, out returnValue))
				{
				}
				else
                {
                    returnValue = _mOffPageMinMargin;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).OffPageMinMargin = value;
                }
                else if(SetValue(PageSettingsConstants.OffPageMinMargin, value))
				{
				}
				else if(_mOffPageMinMargin != value)
                {
                    _mOffPageMinMargin = value;
                    OnPropertyChanged(PageSettingsConstants.OffPageMinMargin);
                }
            }
        }
		
  		Thickness? _mOffPageMaxMargin = null;
		public Thickness? OffPageMaxMargin
        {
            get
            {
				Thickness? returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).OffPageMaxMargin;
				}
                else if(TryGetValue(PageSettingsConstants.OffPageMaxMargin, out returnValue))
				{
				}
				else
                {
                    returnValue = _mOffPageMaxMargin;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).OffPageMaxMargin = value;
                }
                else if(SetValue(PageSettingsConstants.OffPageMaxMargin, value))
				{
				}
				else if(_mOffPageMaxMargin != value)
                {
                    _mOffPageMaxMargin = value;
                    OnPropertyChanged(PageSettingsConstants.OffPageMaxMargin);
                }
            }
        }
		
  		PageOrientation _mPageOrientation = PageOrientation.Landscape;
		public PageOrientation PageOrientation
        {
            get
            {
				PageOrientation returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).PageOrientation;
				}
                else if(TryGetValue(PageSettingsConstants.PageOrientation, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPageOrientation;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).PageOrientation = value;
                }
                else if(SetValue(PageSettingsConstants.PageOrientation, value))
				{
				}
				else if(_mPageOrientation != value)
                {
                    _mPageOrientation = value;
                    OnPropertyChanged(PageSettingsConstants.PageOrientation);
                }
            }
        }
		
  		Brush _mPageBackground = null;
		public Brush PageBackground
        {
            get
            {
				Brush returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).PageBackground;
				}
                else if(TryGetValue(PageSettingsConstants.PageBackground, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPageBackground;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).PageBackground = value;
                }
                else if(SetValue(PageSettingsConstants.PageBackground, value))
				{
				}
				else if(_mPageBackground != value)
                {
                    _mPageBackground = value;
                    OnPropertyChanged(PageSettingsConstants.PageBackground);
                }
            }
        }
		
  		Brush _mPageBorderBrush = null;
		public Brush PageBorderBrush
        {
            get
            {
				Brush returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).PageBorderBrush;
				}
                else if(TryGetValue(PageSettingsConstants.PageBorderBrush, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPageBorderBrush;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).PageBorderBrush = value;
                }
                else if(SetValue(PageSettingsConstants.PageBorderBrush, value))
				{
				}
				else if(_mPageBorderBrush != value)
                {
                    _mPageBorderBrush = value;
                    OnPropertyChanged(PageSettingsConstants.PageBorderBrush);
                }
            }
        }
		
  		Thickness? _mPageBorderThickness = null;
		public Thickness? PageBorderThickness
        {
            get
            {
				Thickness? returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).PageBorderThickness;
				}
                else if(TryGetValue(PageSettingsConstants.PageBorderThickness, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPageBorderThickness;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).PageBorderThickness = value;
                }
                else if(SetValue(PageSettingsConstants.PageBorderThickness, value))
				{
				}
				else if(_mPageBorderThickness != value)
                {
                    _mPageBorderThickness = value;
                    OnPropertyChanged(PageSettingsConstants.PageBorderThickness);
                }
            }
        }
		
  		MeasurementUnit _mUnit = new LengthUnit();
		public MeasurementUnit Unit
        {
            get
            {
				MeasurementUnit returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).Unit;
				}
                else if(TryGetValue(PageSettingsConstants.Unit, out returnValue))
				{
				}
				else
                {
                    returnValue = _mUnit;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).Unit = value;
                }
                else if(SetValue(PageSettingsConstants.Unit, value))
				{
				}
				else if(_mUnit != value)
                {
                    _mUnit = value;
                    OnPropertyChanged(PageSettingsConstants.Unit);
                }
            }
        }
		
  		bool _mShowPageBreaks = false;
		public bool ShowPageBreaks
        {
            get
            {
				bool returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).ShowPageBreaks;
				}
                else if(TryGetValue(PageSettingsConstants.ShowPageBreaks, out returnValue))
				{
				}
				else
                {
                    returnValue = _mShowPageBreaks;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).ShowPageBreaks = value;
                }
                else if(SetValue(PageSettingsConstants.ShowPageBreaks, value))
				{
				}
				else if(_mShowPageBreaks != value)
                {
                    _mShowPageBreaks = value;
                    OnPropertyChanged(PageSettingsConstants.ShowPageBreaks);
                }
            }
        }
		
  		Thickness _mPrintMargin = new Thickness(24,24,24,24);
		public Thickness PrintMargin
        {
            get
            {
				Thickness returnValue;
				if(_mSource is IPageSettings)
				{
                    returnValue = (_mSource as IPageSettings).PrintMargin;
				}
                else if(TryGetValue(PageSettingsConstants.PrintMargin, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPrintMargin;
                }
				return SharedData.Unit.ToPixel(returnValue);
            }
            set
            {
				value = SharedData.Unit.ToUnit(value);
                if(_mSource is IPageSettings)
                {
                    (_mSource as IPageSettings).PrintMargin = value;
                }
                else if(SetValue(PageSettingsConstants.PrintMargin, value))
				{
				}
				else if(_mPrintMargin != value)
                {
                    _mPrintMargin = value;
                    OnPropertyChanged(PageSettingsConstants.PrintMargin);
                }
            }
        }
			}
	internal partial class SnapSettingsWrapper
	{

  		Gridlines _mHorizontalGridlines = null;
		public Gridlines HorizontalGridlines
        {
            get
            {
				Gridlines returnValue;
				if(_mSource is ISnapSettings)
				{
                    returnValue = (_mSource as ISnapSettings).HorizontalGridlines;
				}
                else if(TryGetValue(SnapSettingsConstants.HorizontalGridlines, out returnValue))
				{
				}
				else
                {
                    returnValue = _mHorizontalGridlines;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is ISnapSettings)
                {
                    (_mSource as ISnapSettings).HorizontalGridlines = value;
                }
                else if(SetValue(SnapSettingsConstants.HorizontalGridlines, value))
				{
				}
				else if(_mHorizontalGridlines != value)
                {
                    _mHorizontalGridlines = value;
                    OnPropertyChanged(SnapSettingsConstants.HorizontalGridlines);
                }
            }
        }
		
  		Gridlines _mVerticalGridlines = null;
		public Gridlines VerticalGridlines
        {
            get
            {
				Gridlines returnValue;
				if(_mSource is ISnapSettings)
				{
                    returnValue = (_mSource as ISnapSettings).VerticalGridlines;
				}
                else if(TryGetValue(SnapSettingsConstants.VerticalGridlines, out returnValue))
				{
				}
				else
                {
                    returnValue = _mVerticalGridlines;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is ISnapSettings)
                {
                    (_mSource as ISnapSettings).VerticalGridlines = value;
                }
                else if(SetValue(SnapSettingsConstants.VerticalGridlines, value))
				{
				}
				else if(_mVerticalGridlines != value)
                {
                    _mVerticalGridlines = value;
                    OnPropertyChanged(SnapSettingsConstants.VerticalGridlines);
                }
            }
        }
		
  		SnapToObject _mSnapToObject = SnapToObject.None;
		public SnapToObject SnapToObject
        {
            get
            {
				SnapToObject returnValue;
				if(_mSource is ISnapSettings)
				{
                    returnValue = (_mSource as ISnapSettings).SnapToObject;
				}
                else if(TryGetValue(SnapSettingsConstants.SnapToObject, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSnapToObject;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is ISnapSettings)
                {
                    (_mSource as ISnapSettings).SnapToObject = value;
                }
                else if(SetValue(SnapSettingsConstants.SnapToObject, value))
				{
				}
				else if(_mSnapToObject != value)
                {
                    _mSnapToObject = value;
                    OnPropertyChanged(SnapSettingsConstants.SnapToObject);
                }
            }
        }
		
  		SnapConstraints _mSnapConstraints = SnapConstraints.None;
		public SnapConstraints SnapConstraints
        {
            get
            {
				SnapConstraints returnValue;
				if(_mSource is ISnapSettings)
				{
                    returnValue = (_mSource as ISnapSettings).SnapConstraints;
				}
                else if(TryGetValue(SnapSettingsConstants.SnapConstraints, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSnapConstraints;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is ISnapSettings)
                {
                    (_mSource as ISnapSettings).SnapConstraints = value;
                }
                else if(SetValue(SnapSettingsConstants.SnapConstraints, value))
				{
				}
				else if(_mSnapConstraints != value)
                {
                    _mSnapConstraints = value;
                    OnPropertyChanged(SnapSettingsConstants.SnapConstraints);
                }
            }
        }
		
  		double _mSnapAngle = 10;
		public double SnapAngle
        {
            get
            {
				double returnValue;
				if(_mSource is ISnapSettings)
				{
                    returnValue = (_mSource as ISnapSettings).SnapAngle;
				}
                else if(TryGetValue(SnapSettingsConstants.SnapAngle, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSnapAngle;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is ISnapSettings)
                {
                    (_mSource as ISnapSettings).SnapAngle = value;
                }
                else if(SetValue(SnapSettingsConstants.SnapAngle, value))
				{
				}
				else if(_mSnapAngle != value)
                {
                    _mSnapAngle = value;
                    OnPropertyChanged(SnapSettingsConstants.SnapAngle);
                }
            }
        }
			}
	internal partial class SfDiagramWrapper
	{

  		object _mNodes = null;
		public object Nodes
        {
            get
            {
				object returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).Nodes;
				}
                else if(TryGetValue(SfDiagramConstants.Nodes, out returnValue))
				{
				}
				else
                {
                    returnValue = _mNodes;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).Nodes = value;
                }
                else if(SetValue(SfDiagramConstants.Nodes, value))
				{
				}
				else if(_mNodes != value)
                {
                    _mNodes = value;
                    OnPropertyChanged(SfDiagramConstants.Nodes);
                }
            }
        }
		
  		object _mConnectors = null;
		public object Connectors
        {
            get
            {
				object returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).Connectors;
				}
                else if(TryGetValue(SfDiagramConstants.Connectors, out returnValue))
				{
				}
				else
                {
                    returnValue = _mConnectors;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).Connectors = value;
                }
                else if(SetValue(SfDiagramConstants.Connectors, value))
				{
				}
				else if(_mConnectors != value)
                {
                    _mConnectors = value;
                    OnPropertyChanged(SfDiagramConstants.Connectors);
                }
            }
        }
		
  		object _mGroups = null;
		public object Groups
        {
            get
            {
				object returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).Groups;
				}
                else if(TryGetValue(SfDiagramConstants.Groups, out returnValue))
				{
				}
				else
                {
                    returnValue = _mGroups;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).Groups = value;
                }
                else if(SetValue(SfDiagramConstants.Groups, value))
				{
				}
				else if(_mGroups != value)
                {
                    _mGroups = value;
                    OnPropertyChanged(SfDiagramConstants.Groups);
                }
            }
        }
		
  		ConnectorType _mDefaultConnectorType = ConnectorType.Orthogonal;
		public ConnectorType DefaultConnectorType
        {
            get
            {
				ConnectorType returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).DefaultConnectorType;
				}
                else if(TryGetValue(SfDiagramConstants.DefaultConnectorType, out returnValue))
				{
				}
				else
                {
                    returnValue = _mDefaultConnectorType;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).DefaultConnectorType = value;
                }
                else if(SetValue(SfDiagramConstants.DefaultConnectorType, value))
				{
				}
				else if(_mDefaultConnectorType != value)
                {
                    _mDefaultConnectorType = value;
                    OnPropertyChanged(SfDiagramConstants.DefaultConnectorType);
                }
            }
        }
		
  		object _mSelectedItems = null;
		public object SelectedItems
        {
            get
            {
				object returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).SelectedItems;
				}
                else if(TryGetValue(SfDiagramConstants.SelectedItems, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSelectedItems;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).SelectedItems = value;
                }
                else if(SetValue(SfDiagramConstants.SelectedItems, value))
				{
				}
				else if(_mSelectedItems == null || !_mSelectedItems.Equals(value))
                {
                    _mSelectedItems = value;
                    OnPropertyChanged(SfDiagramConstants.SelectedItems);
                }
            }
        }
		
  		GraphConstraints _mConstraints = GraphConstraints.Default;
		public GraphConstraints Constraints
        {
            get
            {
				GraphConstraints returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).Constraints;
				}
                else if(TryGetValue(SfDiagramConstants.Constraints, out returnValue))
				{
				}
				else
                {
                    returnValue = _mConstraints;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).Constraints = value;
                }
                else if(SetValue(SfDiagramConstants.Constraints, value))
				{
				}
				else if(_mConstraints != value)
                {
                    _mConstraints = value;
                    OnPropertyChanged(SfDiagramConstants.Constraints);
                }
            }
        }
		
  		BezierSmoothness _mBezierSmoothness = BezierSmoothness.None;
		public BezierSmoothness BezierSmoothness
        {
            get
            {
				BezierSmoothness returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).BezierSmoothness;
				}
                else if(TryGetValue(SfDiagramConstants.BezierSmoothness, out returnValue))
				{
				}
				else
                {
                    returnValue = _mBezierSmoothness;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).BezierSmoothness = value;
                }
                else if(SetValue(SfDiagramConstants.BezierSmoothness, value))
				{
				}
				else if(_mBezierSmoothness != value)
                {
                    _mBezierSmoothness = value;
                    OnPropertyChanged(SfDiagramConstants.BezierSmoothness);
                }
            }
        }
		
  		Tool _mTool = Tool.MultipleSelect;
		public Tool Tool
        {
            get
            {
				Tool returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).Tool;
				}
                else if(TryGetValue(SfDiagramConstants.Tool, out returnValue))
				{
				}
				else
                {
                    returnValue = _mTool;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).Tool = value;
                }
                else if(SetValue(SfDiagramConstants.Tool, value))
				{
				}
				else if(_mTool != value)
                {
                    _mTool = value;
                    OnPropertyChanged(SfDiagramConstants.Tool);
                }
            }
        }
		
  		DrawingTool _mDrawingTool = DrawingTool.Connector;
		public DrawingTool DrawingTool
        {
            get
            {
				DrawingTool returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).DrawingTool;
				}
                else if(TryGetValue(SfDiagramConstants.DrawingTool, out returnValue))
				{
				}
				else
                {
                    returnValue = _mDrawingTool;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).DrawingTool = value;
                }
                else if(SetValue(SfDiagramConstants.DrawingTool, value))
				{
				}
				else if(_mDrawingTool != value)
                {
                    _mDrawingTool = value;
                    OnPropertyChanged(SfDiagramConstants.DrawingTool);
                }
            }
        }
		
  		MultipleSelectionMode _mMultipleSelectionMode = MultipleSelectionMode.Default;
		public MultipleSelectionMode MultipleSelectionMode
        {
            get
            {
				MultipleSelectionMode returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).MultipleSelectionMode;
				}
                else if(TryGetValue(SfDiagramConstants.MultipleSelectionMode, out returnValue))
				{
				}
				else
                {
                    returnValue = _mMultipleSelectionMode;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).MultipleSelectionMode = value;
                }
                else if(SetValue(SfDiagramConstants.MultipleSelectionMode, value))
				{
				}
				else if(_mMultipleSelectionMode != value)
                {
                    _mMultipleSelectionMode = value;
                    OnPropertyChanged(SfDiagramConstants.MultipleSelectionMode);
                }
            }
        }
		
  		Rect _mViewport = Rect.Empty;
		public Rect Viewport
        {
			get
			{
				return _mViewport;
			}
			set
			{
				if(_mViewport != value)
				{
					_mViewport = value;
					OnPropertyChanged(SfDiagramConstants.Viewport);
				}
			}
        }
		
  		LayoutManager _mLayoutManager = null;
		public LayoutManager LayoutManager
        {
            get
            {
				LayoutManager returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).LayoutManager;
				}
                else if(TryGetValue(SfDiagramConstants.LayoutManager, out returnValue))
				{
				}
				else
                {
                    returnValue = _mLayoutManager;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).LayoutManager = value;
                }
                else if(SetValue(SfDiagramConstants.LayoutManager, value))
				{
				}
				else if(_mLayoutManager != value)
                {
                    _mLayoutManager = value;
                    OnPropertyChanged(SfDiagramConstants.LayoutManager);
                }
            }
        }
		
  		GetTypes _mKnownTypes = null;
		public GetTypes KnownTypes
        {
            get
            {
				GetTypes returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).KnownTypes;
				}
                else if(TryGetValue(SfDiagramConstants.KnownTypes, out returnValue))
				{
				}
				else
                {
                    returnValue = _mKnownTypes;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).KnownTypes = value;
                }
                else if(SetValue(SfDiagramConstants.KnownTypes, value))
				{
				}
				else if(_mKnownTypes != value)
                {
                    _mKnownTypes = value;
                    OnPropertyChanged(SfDiagramConstants.KnownTypes);
                }
            }
        }
		
  		SnapSettings _mSnapSettings = null;
		public SnapSettings SnapSettings
        {
            get
            {
				SnapSettings returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).SnapSettings;
				}
                else if(TryGetValue(SfDiagramConstants.SnapSettings, out returnValue))
				{
				}
				else
                {
                    returnValue = _mSnapSettings;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).SnapSettings = value;
                }
                else if(SetValue(SfDiagramConstants.SnapSettings, value))
				{
				}
				else if(_mSnapSettings != value)
                {
                    _mSnapSettings = value;
                    OnPropertyChanged(SfDiagramConstants.SnapSettings);
                }
            }
        }
		
  		IPageSettings _mPageSettings = null;
		public IPageSettings PageSettings
        {
            get
            {
				IPageSettings returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).PageSettings;
				}
                else if(TryGetValue(SfDiagramConstants.PageSettings, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPageSettings;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).PageSettings = value;
                }
                else if(SetValue(SfDiagramConstants.PageSettings, value))
				{
				}
				else if(_mPageSettings != value)
                {
                    _mPageSettings = value;
                    OnPropertyChanged(SfDiagramConstants.PageSettings);
                }
            }
        }
		
     #if SyncfusionFramework4_5_1 && WINRT		
				ExportSettings _mExportSettings = null;
		public ExportSettings ExportSettings
        {
            get
            {
				ExportSettings returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).ExportSettings;
				}
                else if(TryGetValue(SfDiagramConstants.ExportSettings, out returnValue))
				{
				}
				else
                {
                    returnValue = _mExportSettings;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).ExportSettings = value;
                }
                else if(SetValue(SfDiagramConstants.ExportSettings, value))
				{
				}
				else if(_mExportSettings != value)
                {
                    _mExportSettings = value;
                    OnPropertyChanged(SfDiagramConstants.ExportSettings);
                }
            }
        }
		#endif
	
     #if SyncfusionFramework4_5_1 && WINRT		
				PrintingService _mPrintingService = null;
		public PrintingService PrintingService
        {
            get
            {
				PrintingService returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).PrintingService;
				}
                else if(TryGetValue(SfDiagramConstants.PrintingService, out returnValue))
				{
				}
				else
                {
                    returnValue = _mPrintingService;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).PrintingService = value;
                }
                else if(SetValue(SfDiagramConstants.PrintingService, value))
				{
				}
				else if(_mPrintingService != value)
                {
                    _mPrintingService = value;
                    OnPropertyChanged(SfDiagramConstants.PrintingService);
                }
            }
        }
		#endif
	
  		IInternalSelector _mInternalSelectedItems = null;
		public IInternalSelector InternalSelectedItems
        {
			get
			{
				return _mInternalSelectedItems;
			}
			set
			{
				if(_mInternalSelectedItems != value)
				{
					_mInternalSelectedItems = value;
					OnPropertyChanged(SfDiagramConstants.InternalSelectedItems);
				}
			}
        }
		
  		Ruler _mHorizontalRuler = null;
		public Ruler HorizontalRuler
        {
            get
            {
				Ruler returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).HorizontalRuler;
				}
                else if(TryGetValue(SfDiagramConstants.HorizontalRuler, out returnValue))
				{
				}
				else
                {
                    returnValue = _mHorizontalRuler;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).HorizontalRuler = value;
                }
                else if(SetValue(SfDiagramConstants.HorizontalRuler, value))
				{
				}
				else if(_mHorizontalRuler != value)
                {
                    _mHorizontalRuler = value;
                    OnPropertyChanged(SfDiagramConstants.HorizontalRuler);
                }
            }
        }
		
  		Ruler _mVerticalRuler = null;
		public Ruler VerticalRuler
        {
            get
            {
				Ruler returnValue;
				if(_mSource is IGraph)
				{
                    returnValue = (_mSource as IGraph).VerticalRuler;
				}
                else if(TryGetValue(SfDiagramConstants.VerticalRuler, out returnValue))
				{
				}
				else
                {
                    returnValue = _mVerticalRuler;
                }
				return returnValue;
            }
            set
            {
                if(_mSource is IGraph)
                {
                    (_mSource as IGraph).VerticalRuler = value;
                }
                else if(SetValue(SfDiagramConstants.VerticalRuler, value))
				{
				}
				else if(_mVerticalRuler != value)
                {
                    _mVerticalRuler = value;
                    OnPropertyChanged(SfDiagramConstants.VerticalRuler);
                }
            }
        }
			}
	internal partial class DummyWrapper
	{
	}
}
