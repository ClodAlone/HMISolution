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
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Layout;
using Syncfusion.UI.Xaml.Diagram.Controls;
using System.ComponentModel;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Diagram.Controller;
namespace Syncfusion.UI.Xaml.Diagram
{
	public partial class NodePortViewModel
	{
		
        		
        double _mNodeOffsetX = 0d;
		public double NodeOffsetX
        {
            get
            {
				return _mNodeOffsetX;
			}
            set
            {
				if(_mNodeOffsetX != value)
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
				return _mNodeOffsetY;
			}
            set
            {
				if(_mNodeOffsetY != value)
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
				return _mNode;
			}
            set
            {
				if(_mNode == null || !_mNode.Equals(value))
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
				return _mUnitMode;
			}
            set
            {
				if(_mUnitMode != value)
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
				return _mShape;
			}
            set
            {
				if(_mShape != value)
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
				return _mShapeStyle;
			}
            set
            {
				if(_mShapeStyle != value)
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
				return _mConstraints;
			}
            set
            {
				if(_mConstraints != value)
				{
					_mConstraints = value;
					OnPropertyChanged(NodePortConstants.Constraints);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class AnnotationEditorViewModel
	{
		
        		
        object _mContent = null;
		public object Content
        {
            get
            {
				return _mContent;
			}
            set
            {
				if(_mContent != value)
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
				return _mEditTemplate;
			}
            set
            {
				if(_mEditTemplate != value)
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
				return _mViewTemplate;
			}
            set
            {
				if(_mViewTemplate != value)
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
				return _mMode;
			}
            set
            {
				if(_mMode != value)
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
				return _mHorizontalAlignment;
			}
            set
            {
				if(_mHorizontalAlignment != value)
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
				return _mVerticalAlignment;
			}
            set
            {
				if(_mVerticalAlignment != value)
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
				return _mAlignment;
			}
            set
            {
				if(_mAlignment != value)
				{
					_mAlignment = value;
					OnPropertyChanged(AnnotationEditorConstants.Alignment);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class DiagramElementViewModel
	{
		
        		
        object _mID = null;
		public object ID
        {
            get
            {
				return _mID;
			}
            set
            {
				if(_mID == null || !_mID.Equals(value))
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
				return _mKey;
			}
            set
            {
				if(_mKey != value)
				{
					_mKey = value;
					OnPropertyChanged(DiagramElementConstants.Key);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class NodeViewModel
	{
		
        		
        double _mOffsetX = 0d;
		public double OffsetX
        {
            get
            {
				return _mOffsetX;
			}
            set
            {
				if(_mOffsetX != value)
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
				return _mOffsetY;
			}
            set
            {
				if(_mOffsetY != value)
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
				return _mRotateAngle;
			}
            set
            {
				if(_mRotateAngle != value)
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
				return _mSnapToObject;
			}
            set
            {
				if(_mSnapToObject != value)
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
				return _mFlip;
			}
            set
            {
				if(_mFlip != value)
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
				return _mMinWidth;
			}
            set
            {
				if(_mMinWidth != value)
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
				return _mMaxWidth;
			}
            set
            {
				if(_mMaxWidth != value)
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
				return _mUnitWidth;
			}
            set
            {
				if(_mUnitWidth != value)
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
				return _mMinHeight;
			}
            set
            {
				if(_mMinHeight != value)
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
				return _mMaxHeight;
			}
            set
            {
				if(_mMaxHeight != value)
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
				return _mUnitHeight;
			}
            set
            {
				if(_mUnitHeight != value)
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
				return _mContent;
			}
            set
            {
				if(_mContent != value)
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
				return _mContentTemplate;
			}
            set
            {
				if(_mContentTemplate != value)
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
				return _mShape;
			}
            set
            {
				if(_mShape != value)
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
				return _mShapeStyle;
			}
            set
            {
				if(_mShapeStyle != value)
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
				return _mIsExpanded;
			}
            set
            {
				if(_mIsExpanded != value)
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
				return _mPivot;
			}
            set
            {
				if(_mPivot != value)
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
				return _mConstraints;
			}
            set
            {
				if(_mConstraints != value)
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
				return _mPorts;
			}
            set
            {
				if(_mPorts != value)
				{
					_mPorts = value;
					OnPropertyChanged(NodeConstants.Ports);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class GroupableViewModel
	{
		
        		
        bool _mIsSelected = false;
		public bool IsSelected
        {
            get
            {
				return _mIsSelected;
			}
            set
            {
				if(_mIsSelected != value)
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
				return _mZIndex;
			}
            set
            {
				if(_mZIndex != value)
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
				return _mAnnotations;
			}
            set
            {
				if(_mAnnotations != value)
				{
					_mAnnotations = value;
					OnPropertyChanged(GroupableConstants.Annotations);
				}
            }
        }
				
        		
        object _mInfo = null;
		public object Info
        {
            get
            {
				return _mInfo;
			}
            set
            {
				if(_mInfo != value)
				{
					_mInfo = value;
					OnPropertyChanged(GroupableConstants.Info);
				}
            }
        }
				
        		
        object _mParentGroup = null;
		public object ParentGroup
        {
            get
            {
				return _mParentGroup;
			}
            set
            {
				if(_mParentGroup == null || !_mParentGroup.Equals(value))
				{
					_mParentGroup = value;
					OnPropertyChanged(GroupableConstants.ParentGroup);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class ConnectorViewModel
	{
		
        		
        object _mSourceNode = null;
		public object SourceNode
        {
            get
            {
				return _mSourceNode;
			}
            set
            {
				if(_mSourceNode == null || !_mSourceNode.Equals(value))
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
				return _mTargetNode;
			}
            set
            {
				if(_mTargetNode == null || !_mTargetNode.Equals(value))
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
				return _mSourcePort;
			}
            set
            {
				if(_mSourcePort != value)
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
				return _mTargetPort;
			}
            set
            {
				if(_mTargetPort != value)
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
				return _mSegments;
			}
            set
            {
				if(_mSegments != value)
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
				return _mSourcePoint;
			}
            set
            {
				if(_mSourcePoint != value)
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
				return _mTargetPoint;
			}
            set
            {
				if(_mTargetPoint != value)
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
				return _mConnectorGeometryStyle;
			}
            set
            {
				if(_mConnectorGeometryStyle != value)
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
				return _mSourceDecorator;
			}
            set
            {
				if(_mSourceDecorator != value)
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
				return _mTargetDecorator;
			}
            set
            {
				if(_mTargetDecorator != value)
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
				return _mSourceDecoratorStyle;
			}
            set
            {
				if(_mSourceDecoratorStyle != value)
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
				return _mTargetDecoratorStyle;
			}
            set
            {
				if(_mTargetDecoratorStyle != value)
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
				return _mBridgeSpace;
			}
            set
            {
				if(_mBridgeSpace != value)
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
				return _mConstraints;
			}
            set
            {
				if(_mConstraints != value)
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
				return _mBezierSmoothness;
			}
            set
            {
				if(_mBezierSmoothness != value)
				{
					_mBezierSmoothness = value;
					OnPropertyChanged(ConnectorConstants.BezierSmoothness);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class GroupViewModel
	{
		
        		
        object _mNodes = null;
		public object Nodes
        {
            get
            {
				return _mNodes;
			}
            set
            {
				if(_mNodes != value)
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
				return _mConnectors;
			}
            set
            {
				if(_mConnectors != value)
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
				return _mGroups;
			}
            set
            {
				if(_mGroups != value)
				{
					_mGroups = value;
					OnPropertyChanged(GroupConstants.Groups);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class SelectorViewModel
	{
		
        		
        Visibility _mQuickCommands = Visibility.Visible;
		public Visibility QuickCommands
        {
            get
            {
				return _mQuickCommands;
			}
            set
            {
				if(_mQuickCommands != value)
				{
					_mQuickCommands = value;
					OnPropertyChanged(SelectorConstants.QuickCommands);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class PageSettings
	{
		
        		
        double _mPageWidth = double.NaN;
		public double PageWidth
        {
            get
            {
				return _mPageWidth;
			}
            set
            {
				if(_mPageWidth != value)
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
				return _mPageHeight;
			}
            set
            {
				if(_mPageHeight != value)
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
				return _mMultiplePage;
			}
            set
            {
				if(_mMultiplePage != value)
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
				return _mOffPageMinMargin;
			}
            set
            {
				if(_mOffPageMinMargin != value)
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
				return _mOffPageMaxMargin;
			}
            set
            {
				if(_mOffPageMaxMargin != value)
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
				return _mPageOrientation;
			}
            set
            {
				if(_mPageOrientation != value)
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
				return _mPageBackground;
			}
            set
            {
				if(_mPageBackground != value)
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
				return _mPageBorderBrush;
			}
            set
            {
				if(_mPageBorderBrush != value)
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
				return _mPageBorderThickness;
			}
            set
            {
				if(_mPageBorderThickness != value)
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
				return _mUnit;
			}
            set
            {
				if(_mUnit != value)
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
				return _mShowPageBreaks;
			}
            set
            {
				if(_mShowPageBreaks != value)
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
				return _mPrintMargin;
			}
            set
            {
				if(_mPrintMargin != value)
				{
					_mPrintMargin = value;
					OnPropertyChanged(PageSettingsConstants.PrintMargin);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class SnapSettings
	{
		
        		
        Gridlines _mHorizontalGridlines = null;
		public Gridlines HorizontalGridlines
        {
            get
            {
				return _mHorizontalGridlines;
			}
            set
            {
				if(_mHorizontalGridlines != value)
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
				return _mVerticalGridlines;
			}
            set
            {
				if(_mVerticalGridlines != value)
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
				return _mSnapToObject;
			}
            set
            {
				if(_mSnapToObject != value)
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
				return _mSnapConstraints;
			}
            set
            {
				if(_mSnapConstraints != value)
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
				return _mSnapAngle;
			}
            set
            {
				if(_mSnapAngle != value)
				{
					_mSnapAngle = value;
					OnPropertyChanged(SnapSettingsConstants.SnapAngle);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	public partial class DiagramViewModel
	{
		
        		
        object _mNodes = null;
		public object Nodes
        {
            get
            {
				return _mNodes;
			}
            set
            {
				if(_mNodes != value)
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
				return _mConnectors;
			}
            set
            {
				if(_mConnectors != value)
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
				return _mGroups;
			}
            set
            {
				if(_mGroups != value)
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
				return _mDefaultConnectorType;
			}
            set
            {
				if(_mDefaultConnectorType != value)
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
				return _mSelectedItems;
			}
            set
            {
				if(_mSelectedItems == null || !_mSelectedItems.Equals(value))
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
				return _mConstraints;
			}
            set
            {
				if(_mConstraints != value)
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
				return _mBezierSmoothness;
			}
            set
            {
				if(_mBezierSmoothness != value)
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
				return _mTool;
			}
            set
            {
				if(_mTool != value)
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
				return _mDrawingTool;
			}
            set
            {
				if(_mDrawingTool != value)
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
				return _mMultipleSelectionMode;
			}
            set
            {
				if(_mMultipleSelectionMode != value)
				{
					_mMultipleSelectionMode = value;
					OnPropertyChanged(SfDiagramConstants.MultipleSelectionMode);
				}
            }
        }
				
        		
        DataTemplateDictionary _mViewDictionary = null;
		public DataTemplateDictionary ViewDictionary
        {
            get
            {
				return _mViewDictionary;
			}
            set
            {
				if(_mViewDictionary != value)
				{
					_mViewDictionary = value;
					OnPropertyChanged(SfDiagramConstants.ViewDictionary);
				}
            }
        }
				
        		
        object _mInfo = null;
		public object Info
        {
            get
            {
				return _mInfo;
			}
            set
            {
				if(_mInfo != value)
				{
					_mInfo = value;
					OnPropertyChanged(SfDiagramConstants.Info);
				}
            }
        }
				
        		
        LayoutManager _mLayoutManager = null;
		public LayoutManager LayoutManager
        {
            get
            {
				return _mLayoutManager;
			}
            set
            {
				if(_mLayoutManager != value)
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
				return _mKnownTypes;
			}
            set
            {
				if(_mKnownTypes != value)
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
				return _mSnapSettings;
			}
            set
            {
				if(_mSnapSettings != value)
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
				return _mPageSettings;
			}
            set
            {
				if(_mPageSettings != value)
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
				return _mExportSettings;
			}
            set
            {
				if(_mExportSettings != value)
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
				return _mPrintingService;
			}
            set
            {
				if(_mPrintingService != value)
				{
					_mPrintingService = value;
					OnPropertyChanged(SfDiagramConstants.PrintingService);
				}
            }
        }
			#endif
			
        		
        Ruler _mHorizontalRuler = null;
		public Ruler HorizontalRuler
        {
            get
            {
				return _mHorizontalRuler;
			}
            set
            {
				if(_mHorizontalRuler != value)
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
				return _mVerticalRuler;
			}
            set
            {
				if(_mVerticalRuler != value)
				{
					_mVerticalRuler = value;
					OnPropertyChanged(SfDiagramConstants.VerticalRuler);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
	internal partial class DiagramCommands
	{
		
        		
        ICommand _mUndo = null;
		public ICommand Undo
        {
            get
            {
				return _mUndo;
			}
            set
            {
				if(_mUndo != value)
				{
					_mUndo = value;
					OnPropertyChanged(DummyConstants.Undo);
				}
            }
        }
				
        		
        ICommand _mZoom = null;
		public ICommand Zoom
        {
            get
            {
				return _mZoom;
			}
            set
            {
				if(_mZoom != value)
				{
					_mZoom = value;
					OnPropertyChanged(DummyConstants.Zoom);
				}
            }
        }
				
        		
        ICommand _mReset = null;
		public ICommand Reset
        {
            get
            {
				return _mReset;
			}
            set
            {
				if(_mReset != value)
				{
					_mReset = value;
					OnPropertyChanged(DummyConstants.Reset);
				}
            }
        }
				
        		
        ICommand _mSelectAll = null;
		public ICommand SelectAll
        {
            get
            {
				return _mSelectAll;
			}
            set
            {
				if(_mSelectAll != value)
				{
					_mSelectAll = value;
					OnPropertyChanged(DummyConstants.SelectAll);
				}
            }
        }
				
        		
        ICommand _mRedo = null;
		public ICommand Redo
        {
            get
            {
				return _mRedo;
			}
            set
            {
				if(_mRedo != value)
				{
					_mRedo = value;
					OnPropertyChanged(DummyConstants.Redo);
				}
            }
        }
				
        		
        ICommand _mGroup = null;
		public ICommand Group
        {
            get
            {
				return _mGroup;
			}
            set
            {
				if(_mGroup != value)
				{
					_mGroup = value;
					OnPropertyChanged(DummyConstants.Group);
				}
            }
        }
				
        		
        ICommand _mUnGroup = null;
		public ICommand UnGroup
        {
            get
            {
				return _mUnGroup;
			}
            set
            {
				if(_mUnGroup != value)
				{
					_mUnGroup = value;
					OnPropertyChanged(DummyConstants.UnGroup);
				}
            }
        }
				
        		
        ICommand _mSameSize = null;
		public ICommand SameSize
        {
            get
            {
				return _mSameSize;
			}
            set
            {
				if(_mSameSize != value)
				{
					_mSameSize = value;
					OnPropertyChanged(DummyConstants.SameSize);
				}
            }
        }
				
        		
        ICommand _mSameHeight = null;
		public ICommand SameHeight
        {
            get
            {
				return _mSameHeight;
			}
            set
            {
				if(_mSameHeight != value)
				{
					_mSameHeight = value;
					OnPropertyChanged(DummyConstants.SameHeight);
				}
            }
        }
				
        		
        ICommand _mSameWidth = null;
		public ICommand SameWidth
        {
            get
            {
				return _mSameWidth;
			}
            set
            {
				if(_mSameWidth != value)
				{
					_mSameWidth = value;
					OnPropertyChanged(DummyConstants.SameWidth);
				}
            }
        }
				
        		
        ICommand _mAlignBottom = null;
		public ICommand AlignBottom
        {
            get
            {
				return _mAlignBottom;
			}
            set
            {
				if(_mAlignBottom != value)
				{
					_mAlignBottom = value;
					OnPropertyChanged(DummyConstants.AlignBottom);
				}
            }
        }
				
        		
        ICommand _mAlignTop = null;
		public ICommand AlignTop
        {
            get
            {
				return _mAlignTop;
			}
            set
            {
				if(_mAlignTop != value)
				{
					_mAlignTop = value;
					OnPropertyChanged(DummyConstants.AlignTop);
				}
            }
        }
				
        		
        ICommand _mAlignLeft = null;
		public ICommand AlignLeft
        {
            get
            {
				return _mAlignLeft;
			}
            set
            {
				if(_mAlignLeft != value)
				{
					_mAlignLeft = value;
					OnPropertyChanged(DummyConstants.AlignLeft);
				}
            }
        }
				
        		
        ICommand _mAlignCenter = null;
		public ICommand AlignCenter
        {
            get
            {
				return _mAlignCenter;
			}
            set
            {
				if(_mAlignCenter != value)
				{
					_mAlignCenter = value;
					OnPropertyChanged(DummyConstants.AlignCenter);
				}
            }
        }
				
        		
        ICommand _mAlignRight = null;
		public ICommand AlignRight
        {
            get
            {
				return _mAlignRight;
			}
            set
            {
				if(_mAlignRight != value)
				{
					_mAlignRight = value;
					OnPropertyChanged(DummyConstants.AlignRight);
				}
            }
        }
				
        		
        ICommand _mAlignMiddle = null;
		public ICommand AlignMiddle
        {
            get
            {
				return _mAlignMiddle;
			}
            set
            {
				if(_mAlignMiddle != value)
				{
					_mAlignMiddle = value;
					OnPropertyChanged(DummyConstants.AlignMiddle);
				}
            }
        }
				
        		
        ICommand _mSpaceAcross = null;
		public ICommand SpaceAcross
        {
            get
            {
				return _mSpaceAcross;
			}
            set
            {
				if(_mSpaceAcross != value)
				{
					_mSpaceAcross = value;
					OnPropertyChanged(DummyConstants.SpaceAcross);
				}
            }
        }
				
        		
        ICommand _mSpaceDown = null;
		public ICommand SpaceDown
        {
            get
            {
				return _mSpaceDown;
			}
            set
            {
				if(_mSpaceDown != value)
				{
					_mSpaceDown = value;
					OnPropertyChanged(DummyConstants.SpaceDown);
				}
            }
        }
				
        		
        ICommand _mSendToBack = null;
		public ICommand SendToBack
        {
            get
            {
				return _mSendToBack;
			}
            set
            {
				if(_mSendToBack != value)
				{
					_mSendToBack = value;
					OnPropertyChanged(DummyConstants.SendToBack);
				}
            }
        }
				
        		
        ICommand _mSendBackward = null;
		public ICommand SendBackward
        {
            get
            {
				return _mSendBackward;
			}
            set
            {
				if(_mSendBackward != value)
				{
					_mSendBackward = value;
					OnPropertyChanged(DummyConstants.SendBackward);
				}
            }
        }
				
        		
        ICommand _mBringToFront = null;
		public ICommand BringToFront
        {
            get
            {
				return _mBringToFront;
			}
            set
            {
				if(_mBringToFront != value)
				{
					_mBringToFront = value;
					OnPropertyChanged(DummyConstants.BringToFront);
				}
            }
        }
				
        		
        ICommand _mBringForward = null;
		public ICommand BringForward
        {
            get
            {
				return _mBringForward;
			}
            set
            {
				if(_mBringForward != value)
				{
					_mBringForward = value;
					OnPropertyChanged(DummyConstants.BringForward);
				}
            }
        }
				
        		
        ICommand _mMoveDown = null;
		public ICommand MoveDown
        {
            get
            {
				return _mMoveDown;
			}
            set
            {
				if(_mMoveDown != value)
				{
					_mMoveDown = value;
					OnPropertyChanged(DummyConstants.MoveDown);
				}
            }
        }
				
        		
        ICommand _mMoveUp = null;
		public ICommand MoveUp
        {
            get
            {
				return _mMoveUp;
			}
            set
            {
				if(_mMoveUp != value)
				{
					_mMoveUp = value;
					OnPropertyChanged(DummyConstants.MoveUp);
				}
            }
        }
				
        		
        ICommand _mMoveLeft = null;
		public ICommand MoveLeft
        {
            get
            {
				return _mMoveLeft;
			}
            set
            {
				if(_mMoveLeft != value)
				{
					_mMoveLeft = value;
					OnPropertyChanged(DummyConstants.MoveLeft);
				}
            }
        }
				
        		
        ICommand _mMoveRight = null;
		public ICommand MoveRight
        {
            get
            {
				return _mMoveRight;
			}
            set
            {
				if(_mMoveRight != value)
				{
					_mMoveRight = value;
					OnPropertyChanged(DummyConstants.MoveRight);
				}
            }
        }
				
        		
        ICommand _mCut = null;
		public ICommand Cut
        {
            get
            {
				return _mCut;
			}
            set
            {
				if(_mCut != value)
				{
					_mCut = value;
					OnPropertyChanged(DummyConstants.Cut);
				}
            }
        }
				
        		
        ICommand _mCopy = null;
		public ICommand Copy
        {
            get
            {
				return _mCopy;
			}
            set
            {
				if(_mCopy != value)
				{
					_mCopy = value;
					OnPropertyChanged(DummyConstants.Copy);
				}
            }
        }
				
        		
        ICommand _mPaste = null;
		public ICommand Paste
        {
            get
            {
				return _mPaste;
			}
            set
            {
				if(_mPaste != value)
				{
					_mPaste = value;
					OnPropertyChanged(DummyConstants.Paste);
				}
            }
        }
				
        		
        ICommand _mDelete = null;
		public ICommand Delete
        {
            get
            {
				return _mDelete;
			}
            set
            {
				if(_mDelete != value)
				{
					_mDelete = value;
					OnPropertyChanged(DummyConstants.Delete);
				}
            }
        }
				
        		
        ICommand _mDraw = null;
		public ICommand Draw
        {
            get
            {
				return _mDraw;
			}
            set
            {
				if(_mDraw != value)
				{
					_mDraw = value;
					OnPropertyChanged(DummyConstants.Draw);
				}
            }
        }
				
        		
        ICommand _mFlip = null;
		public ICommand Flip
        {
            get
            {
				return _mFlip;
			}
            set
            {
				if(_mFlip != value)
				{
					_mFlip = value;
					OnPropertyChanged(DummyConstants.Flip);
				}
            }
        }
				
        		
        ICommand _mFitToPage = null;
		public ICommand FitToPage
        {
            get
            {
				return _mFitToPage;
			}
            set
            {
				if(_mFitToPage != value)
				{
					_mFitToPage = value;
					OnPropertyChanged(DummyConstants.FitToPage);
				}
            }
        }
				
        		
        ICommand _mDuplicate = null;
		public ICommand Duplicate
        {
            get
            {
				return _mDuplicate;
			}
            set
            {
				if(_mDuplicate != value)
				{
					_mDuplicate = value;
					OnPropertyChanged(DummyConstants.Duplicate);
				}
            }
        }
		    

        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        
        //public event PropertyChangedEventHandler PropertyChanged;
	}
}
