#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	internal interface ITreeNodeAdvPaintFilter
	{
		bool OnBeforeNodePaint(TreeNodeAdvPaintEventArgs e);
		bool OnNodeBackgroundPaint(TreeNodeAdvPaintBackgroundEventArgs e);
		void OnAfterNodePaint(TreeNodeAdvPaintEventArgs e);
	}

	/// <summary>
	/// Event args that are passed in the DrawNode event of the TreeViewAdv control. 
	/// Contains information about the appearance of the node and the location and sizes of different parts of the node.
	/// </summary>
	public class TreeNodeAdvPaintEventArgs : System.EventArgs
	{
		private Graphics graphics;
		private int level;
		private int indent;
		private bool selected;
		private bool active;
		private bool fullRowSelect;
		private bool hotTracked;
		private Rectangle bounds;
		private Point textLocation;
		private bool handled = false;
		private bool handledPlusMinus = false;
		private bool handledCheckBox = false;
		private bool handledOptionButton = false;
		private bool handledLeftImageList = false;
		private bool handledStateImageList = false;
		private bool handledText = false;
		private bool handledRightImageList = false;
		private Color foreColor = Color.Empty;
		
		private TreeNodeAdv node;
		private int rightMargin = 0;
        /// <summary>
        /// Gets / Sets the RightMargin
        /// </summary>
		public int RightMargin
		{
			get{return rightMargin;}
			set{rightMargin = value;}
		}
		/// <summary>
        /// Gets the <see cref="TreeNodeAdv"/> which is associated with the action.
		/// </summary>
		public TreeNodeAdv Node
		{
			get{return node;}
		}
        /// <summary>
        /// Gets the location of text as <see cref="System.Drawing.Point"/>
        /// </summary>
		public Point TextLocation
		{
			get{return textLocation;}
		}
        /// <summary>
        /// Get the bounds of <see cref="TreeNodeAdv"/>
        /// </summary>
		public Rectangle Bounds 
		{
			get{return bounds;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether the event was handled
        /// </summary>
		public bool Handled
		{
			get{return handled;}
			set{handled = value;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether painting PlusMinus button was handled
        /// </summary>
		public bool HandledPlusMinus
		{
			get{return handledPlusMinus;}
			set{handledPlusMinus = value;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether painting CheckBox was handled
        /// </summary>
		public bool HandledCheckBox
		{
			get{return handledCheckBox;}
			set{handledCheckBox = value;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether painting Option button was handled
        /// </summary>
		public bool HandledOptionButton
		{
			get{return handledOptionButton;}
			set{handledOptionButton = value;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether painting images in left side was handled
        /// </summary>
		public bool HandledLeftImageList
		{
			get{return handledLeftImageList;}
			set{handledLeftImageList = value;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether painting state image was handled
        /// </summary>
		public bool HandledStateImageList
		{
			get{return handledStateImageList;}
			set{handledStateImageList = value;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether painting text was handled
        /// </summary>
		public bool HandledText
		{
			get{return handledText;}
			set{handledText = value;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether painting images in right side was handled
        /// </summary>
		public bool HandledRightImageList
		{
			get{return handledRightImageList;}
			set{handledRightImageList = value;}
		}
        /// <summary>
        /// Gets the <see cref="System.Drawing.Graphics"/> object associated with the event
        /// </summary>
		public Graphics Graphics
		{
			get{return graphics;}
		}
        /// <summary>
        /// Gets the level of node
        /// </summary>
        /// <remarks>
        /// An instance of <see cref="System.Int32"/>
        /// </remarks>
		public int Level
		{
			get{return level;}
		}
        /// <summary>
        /// Gets the indent of node
        /// </summary>
        /// <remarks>
        /// An instance of <see cref="System.Int32"/>
        /// </remarks>
		public int Indent
		{
			get{return indent;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether the node is selected
        /// </summary>
        public bool Selected
		{
			get{return selected;}
			set{this.selected = value;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether the node is Active
        /// </summary>
		public bool Active
		{
			get{return active;}
			set{this.active = value;}
		}
        /// <summary>
        /// Gets a value indicating whether the FullRowSelect is enabled
        /// </summary>
		public bool FullRowSelect
		{
			get{return fullRowSelect;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether the HotTracking is enabled
        /// </summary>
	
		public bool HotTracked
		{
			get{return hotTracked;}
			set{this.hotTracked = value;}
		}
        /// <summary>
        /// Gets / Sets foreground color
        /// </summary>
		public Color ForeColor
		{
			get{return this.foreColor;}
			set{this.foreColor = value;}
		}
        /// <summary>
        /// Initializes the TreeNodeAdvPaintEventArgs class 
        /// </summary>
        /// <param name="node">The node associated with event</param>
        /// <param name="g">The instance of Graphics class</param>
        /// <param name="bounds">Bounds of the Node</param>
        /// <param name="textLocation">Location of Text</param>
        /// <param name="level">The Level of Node</param>
        /// <param name="indent">The Indent of Node</param>
        /// <param name="selected">Indicates whether the Node is selected</param>
        /// <param name="active">Indicates whether the Node is active</param>
        /// <param name="fullRowSelect">Indicates whether FullRowSelect is enabled</param>
        /// <param name="hotTracked">Indicates whether HotTracking is enabled</param>
        /// <param name="foreColor">The foreground color of node</param>
		public TreeNodeAdvPaintEventArgs(TreeNodeAdv node,Graphics g ,Rectangle bounds,Point textLocation,int level,int indent,bool selected,bool active,bool fullRowSelect,bool hotTracked,
			Color foreColor):base()
		{
			this.node = node;
			graphics = g;
			this.bounds = bounds;
			this.textLocation = textLocation;
			this.level = level;
			this.indent = indent;
			this.selected = selected;
			this.active = active;
			this.fullRowSelect = fullRowSelect;
			this.hotTracked = hotTracked;			
			this.foreColor = foreColor;
		}
	}
    /// <summary>
    /// Event args that are passed in the NodeBackGround event of the TreeViewAdv control. 
    /// Contains information about the appearance of the node background and the location and sizes of different parts of the node.
    /// </summary>
	public class TreeNodeAdvPaintBackgroundEventArgs : System.EventArgs
	{
		private Graphics graphics;
		private bool selected;
		private bool active;
		private bool fullRowSelect;
		private bool hotTracked;
		private Rectangle bounds;
		private TreeNodeAdv node;
		private BrushInfo brushInfo;
		private bool handled = false;
        /// <summary>
        /// Gets the <see cref="TreeNodeAdv"/> which is associated with the action.
        /// </summary>
		
		public TreeNodeAdv Node
		{
			get{return node;}
		}
        /// <summary>
        /// Get the bounds of <see cref="TreeNodeAdv"/>
        /// </summary>
	
		public Rectangle Bounds
		{
			get{return bounds;}
		}
        /// <summary>
        /// Gets the <see cref="System.Drawing.Graphics"/> object associated with the event
        /// </summary>
	
		public Graphics Graphics
		{
			get{return graphics;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether the node is selected
        /// </summary>
 
		public bool Selected
		{
			get{return selected;}
			set{this.selected = value;}
			
		}
        /// <summary>
        /// Gets / Sets a value indicating whether the node is Active
        /// </summary>
	
		public bool Active
		{
			get{return active;}
			set{this.active = value;}
		}
        /// <summary>
        /// Gets a value indicating whether the FullRowSelect is enabled
        /// </summary>
	
		public bool FullRowSelect
		{
			get{return fullRowSelect;}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether the HotTracking is enabled
        /// </summary>
	
		public bool HotTracked
		{
			get{return hotTracked;}
			set{this.hotTracked = value;}
		}
		/// <summary>
		/// Gets / Sets the BrushInfo with which the background will be painted by default, if you don't 
		/// mark this event as handled.
		/// </summary>
		/// <remarks>You can optionally change the properties of this BrushInfo object 
		/// or provide a new BrushInfo without
		/// marking this event as handled <see cref="Handled"/>.</remarks>
		public BrushInfo BrushInfo
		{
			get{return this.brushInfo;}
			set
			{
				this.brushInfo = value;
			}
		}
        /// <summary>
        /// Gets / Sets a value indicating whether the event was handled
        /// </summary>
		public bool Handled
		{
			get{return this.handled;}
			set
			{
				this.handled = value;
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">The node associated with event</param>
        /// <param name="g">The instance of Graphics class</param>
        /// <param name="selected">Indicates whether the Node is selected</param>
        /// <param name="active">Indicates whether the Node is active</param>
        /// <param name="fullRowSelect">Indicates whether FullRowSelect is enabled</param>
        /// <param name="hotTracked">Indicates whether HotTracking is enabled</param>
        /// <param name="brushInfo">The BrushInfo with which the background will be painted </param>
		public TreeNodeAdvPaintBackgroundEventArgs(TreeNodeAdv node,Graphics g ,bool selected,bool active,bool fullRowSelect,bool hotTracked
			,BrushInfo brushInfo):base()
		{
			this.node = node;
			graphics = g;
			this.bounds = node.Bounds;
			this.selected = selected;
			this.active = active;
			this.fullRowSelect = fullRowSelect;
			this.hotTracked = hotTracked;			
			this.brushInfo = brushInfo;
		}
	}
}
