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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;

using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms;
using System.Globalization;
using System.Runtime.Serialization;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Syncfusion.ComponentModel;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Tools
{

	/// <summary>
	/// This class holds information about the location and size of the node parts(eg plusminus,checkbox).
	/// </summary>
	public class TreeNodeAdvPart
	{
		private TreeNodeAdv node;
		internal int xIndentFromNodeLeft;
		private Size size;

		internal TreeNodeAdvPart(TreeNodeAdv node)
		{
			this.node = node;
		}

		protected TreeNodeAdv Node
		{
			get{return this.node;}
		}
		private Rectangle GetBounds()
		{
			if(!this.visible || this.node == null || this.node.Bounds == Rectangle.Empty)
				return Rectangle.Empty;

			int left = 0;
			
			if (GetIsMirrored())
			{
				left = this.node.NodeX - this.xIndentFromNodeLeft - this.Size.Width;
			}
			else
			{
				left = this.node.NodeX + this.xIndentFromNodeLeft;
			}

			int top = this.node.Bounds.Top + (this.node.Bounds.Height - this.Size.Height)/2;

            if (this.node.TreeView.EnableTouchMode)
            {
                top = this.node.Bounds.Top + (this.node.Bounds.Height - this.BeforeTouchSize.Height) / 2;
                return new Rectangle(new Point(left, top), this.BeforeTouchSize);
            }
            else
                return new Rectangle(new Point(left, top), this.Size);
		}

        public Size BeforeTouchSize
        {
            get 
            { 
                return new Size((int)(this.size.Height * (1.25)),(int)(this.Size.Width *(1.25)));
            }
        }

		public Rectangle Bounds
		{
			get{return this.GetBounds();}
		}

		public Size Size
		{
			get{return this.size;}
			set{this.size =  value;}
		}

		public int Height
		{
			get{return this.size.Height;}
			set{this.size.Height = value;}
		}

		public int Width
		{
			get{return this.size.Width;}
			set{this.size.Width = value;}
		}

		public Point Location
		{
			get
            {
                if (this.node.TreeView.EnableTouchMode)
                {
                    Point pt = this.GetBounds().Location;
                    pt.X = pt.X - 5;
                    return pt;
                }
                else
                    return this.GetBounds().Location;
            }
		}

		private bool visible = true;
		public virtual bool Visible
		{
			get{return visible && this.node.Visible;}
			set
			{
				visible = value;
			}
		}

		protected bool GetIsMirrored()
		{
			return node.GetIsMirrored();
		}
	}

	internal class CheckBoxPart : TreeNodeAdvPart
	{
		internal CheckBoxPart(TreeNodeAdv node):base(node)
		{

		}
		public override bool Visible
		{
			get{return base.Visible && this.Node.NodeStyle.ShowCheckBox;}
			set{base.Visible = value;}
		}
	}

	internal class OptionButtonPart : TreeNodeAdvPart
	{
		internal OptionButtonPart(TreeNodeAdv node):base(node)
		{

		}
		public override bool Visible
		{
			get{return base.Visible && this.Node.NodeStyle.ShowOptionButton;}
			set{base.Visible = value;}
		}
	}

	public enum PredefinedPrimitiveTypes
	{		
		Text = 0,
		LeftImages = 1,
		RightImages = 2,
		CheckBox = 3,
		StateImage = 4,
		OptionsButton = 5,
		CustomControl = 6
	}

	[ Serializable ]
    [TypeConverter(typeof(TreeNodePrimitiveConverter))]
	public class TreeNodePrimitive : ICloneable
	{
		#region members
		/// <summary>
		/// 
		/// </summary>
		private int m_index;
		/// <summary>
		/// 
		/// </summary>
		private PredefinedPrimitiveTypes m_type = PredefinedPrimitiveTypes.Text;
		#endregion

		#region events
		internal event SyncfusionPropertyChangedEventHandler PropertyChanged;
		internal event SyncfusionPropertyChangedEventHandler PropertyChanging;
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public int Index
		{
			get
			{
				return m_index;
			}
			set
			{
				if( value != m_index )
				{
					int oldValue = m_index;
					SyncfusionPropertyChangedEventArgs args = new SyncfusionPropertyChangedEventArgs( 
						PropertyChangeEffect.None, "Index", oldValue, value );
					OnPropertyChanging( args );

					m_index = value;

					args = new SyncfusionPropertyChangedEventArgs( 
						PropertyChangeEffect.None, "Index", oldValue, value );

					OnPropertyChanged( args );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue( PredefinedPrimitiveTypes.Text ) ]
		public PredefinedPrimitiveTypes PrimitiveType
		{
			get
			{
				return m_type;
			}
			set
			{
				if( value != m_type )
				{	
					PredefinedPrimitiveTypes oldValue = m_type;
					SyncfusionPropertyChangedEventArgs args = new SyncfusionPropertyChangedEventArgs( 
						PropertyChangeEffect.None, "PrimitiveType", oldValue, value );

					OnPropertyChanging( args );

					m_type = value;

					args = new SyncfusionPropertyChangedEventArgs( 
						PropertyChangeEffect.None, "PrimitiveType", oldValue, value );

					OnPropertyChanged( args );
				}
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		public TreeNodePrimitive()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="primitiveType"></param>
		public TreeNodePrimitive( int index, PredefinedPrimitiveTypes primitiveType )
		{
			m_type = primitiveType;
			m_index = index;
		}
		#endregion

		#region Implementation
		protected virtual void OnPropertyChanged( SyncfusionPropertyChangedEventArgs e )
		{			
			if( PropertyChanged != null )
			{
				PropertyChanged( this, e );
			}
		}
		protected virtual void OnPropertyChanging( SyncfusionPropertyChangedEventArgs e )
		{			
			if( PropertyChanging != null )
			{
				PropertyChanging( this, e );
			}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{			
			return new TreeNodePrimitive( m_index, m_type );
		}

		#endregion
	}

	/// <summary>
	/// A collection that stores <see cref="TreeNodePrimitive"/> objects.
	/// </summary>
	[Editor( typeof( PrimitivesCollectionEditor ), typeof( UITypeEditor ) ),
	 Serializable ]
	public class TreeNodePrimitivesCollection : CollectionBase, ICloneable
	{
		#region members
		private Hashtable m_htItems = new Hashtable();
		#endregion

		#region Initialize/Finalize Method

		/// <summary>
		/// Initializes a new instance of 'PrimitiveCollection'.
		/// </summary>
		public TreeNodePrimitivesCollection()
		{}

		#endregion

		#region Class Events

		/// <summary>
		/// Raise by <see cref="OnCollectionChanged"/> method.
		/// </summary>
		public event CollectionChangeEventHandler CollectionChanged;

		#endregion 

		#region Class Event Raisers
		private void RaiseCollectionChanged( CollectionChangeEventArgs args )
		{
			if( this.CollectionChanged != null )
			{
				this.CollectionChanged( this, args );
			}
		}
		
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Adds pt to collection.
		/// </summary>
		public void Add( TreeNodePrimitive primitive )
		{	
			ValidatePrimitive( primitive.PrimitiveType );

			primitive.PropertyChanging += new SyncfusionPropertyChangedEventHandler( OnPrimitivePropertyChanging );

			base.InnerList.Add( primitive );

			m_htItems[ primitive.PrimitiveType ] = primitive.PrimitiveType;

			CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Add, primitive );

			OnCollectionChanged( args );
		}

		/// <summary>
		/// Adds primitives to collection.
		/// </summary>
		public void AddRange( TreeNodePrimitive[] arrPrimitives )
		{
			if( arrPrimitives != null && arrPrimitives.Length > 0 )
			{
				for( int i = 0, len = arrPrimitives.Length; i < len; i++ )
				{
					TreeNodePrimitive primitive = arrPrimitives[ i ];
					ValidatePrimitive( primitive.PrimitiveType );

					primitive.PropertyChanging += new SyncfusionPropertyChangedEventHandler( OnPrimitivePropertyChanging );

					m_htItems[ primitive.PrimitiveType ] = primitive.PrimitiveType;
				}

				this.InnerList.AddRange( arrPrimitives );

				CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Add,
					arrPrimitives );

				OnCollectionChanged( args );
			}
		}

		/// <summary>
		/// Removes pt from collection.
		/// </summary>
		public void Remove( TreeNodePrimitive primitive )
		{
			if( m_htItems[ primitive.PrimitiveType ] != null )
			{

				this.InnerList.Remove( primitive );
				CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Remove, 
					primitive );
				OnCollectionChanged( args );
			}
		}
		
		/// <summary>
		/// Indexer.
		/// </summary>
		public TreeNodePrimitive this[ int index ]
		{
			get
			{
				return ( TreeNodePrimitive )this.InnerList[ index ];
			}
			set
			{
				TreeNodePrimitive oldValue = this[ index ];

				if( value.PrimitiveType != oldValue.PrimitiveType )
				{
					ValidatePrimitive( value.PrimitiveType );
				}

				m_htItems.Remove( oldValue.PrimitiveType );

				this.InnerList[ index ] = value;

				m_htItems[ value.PrimitiveType ] = value.PrimitiveType;
			}
		}

		#endregion

		#region Overrides
		protected virtual void OnCollectionChanged( CollectionChangeEventArgs args )
		{
			RaiseCollectionChanged( args );
		}

		protected override void OnClear()
		{			
			base.OnClear();

			for( int i = 0, len = this.Count; i < len; i++ )
			{
				this[ i ].PropertyChanging -= new SyncfusionPropertyChangedEventHandler( OnPrimitivePropertyChanging );
			}

			OnCollectionChanged( new CollectionChangeEventArgs( CollectionChangeAction.Remove, this.InnerList.ToArray() ) );
			m_htItems.Clear();
		}

		protected override void OnRemove( int index, object value )
		{
			base.OnRemove( index, value );

			TreeNodePrimitive primitive = ( TreeNodePrimitive )value;
			primitive.PropertyChanging -= new SyncfusionPropertyChangedEventHandler( OnPrimitivePropertyChanging );
			m_htItems.Remove( primitive.PrimitiveType );
		}
		#endregion
		#region Supprot ICloneable
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			TreeNodePrimitivesCollection clone = new TreeNodePrimitivesCollection();

			foreach( TreeNodePrimitive primitive in this.InnerList )
			{
				TreeNodePrimitive cloned = new TreeNodePrimitive( primitive.Index,
					primitive.PrimitiveType );

				clone.Add( cloned );
			}

			return clone;
		}
		#endregion

		#region Implementation
		internal bool IsValidPrimitiveType( PredefinedPrimitiveTypes primitiveType )
		{
			if( m_htItems == null ) m_htItems = new Hashtable();

			return ( m_htItems[ primitiveType ] == null );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="primitive"></param>
		private void ValidatePrimitive( PredefinedPrimitiveTypes primitiveType )
		{
			if( !IsValidPrimitiveType( primitiveType ) )
			{
				throw new ArgumentException( "Primitive of this type already has been added." );
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPrimitivePropertyChanging( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			switch( e.PropertyName )
			{
				case "PrimitiveType" :
					PredefinedPrimitiveTypes newValue = ( PredefinedPrimitiveTypes )e.NewValue;
					ValidatePrimitive( newValue );
					m_htItems.Remove( e.OldValue );
					m_htItems[ newValue ] = newValue;
					break;
			}
		}
		#endregion
	}

	/// <summary>
	/// The TreeNodeAdv represents a node in a <see cref="TreeViewAdv"/>. It contains information about the specific node like text, background style and other settings.
	/// </summary>
	/// <remarks>
	/// <p>The <see cref="Nodes"/> collection holds all the child <b>TreeNodeAdv</b> objects assigned to the current 
	/// <b>TreeNodeAdv</b>. You can add, remove or clone a <b>TreeNodeAdv</b>; when doing so, all child tree 
	/// nodes are added, removed or cloned. Each <b>TreeNodeAdv</b> can contain a collection of other 
	/// <b>TreeNodeAdv</b> objects. This can make it difficult to determine where you are in the 
	/// <see cref="TreeViewAdv"/> when iterating through the collection. To determine your location in a tree 
	/// structure, use the <see cref="FullPath"/> property. The <b>FullPath</b> string can be parsed using the 
	/// <see cref="TreeViewAdv.PathSeparator"/> string value to determine where a <b>TreeNodeAdv</b> label begins and ends.
	/// </p>
	/// <p>The <b>TreeNodeAdv</b> label is set by setting the <see cref="Text"/> 
	/// property explicitly. The alternative is to create the tree node using one of 
	/// the <b>TreeNodeAdv</b> constructors that has a string parameter that represents 
	/// the <see cref="Text"/> property.</p>
	/// <p>You can specify images for the node using the <see cref="TreeNodeAdvStyleInfo.LeftImageIndices"/>,
	/// <see cref="TreeNodeAdvStyleInfo.OpenImgIndex"/>, <see cref="TreeNodeAdvStyleInfo.ClosedImgIndex"/>,
	/// <see cref="TreeNodeAdvStyleInfo.NoChildrenImgIndex"/> and <see cref="TreeNodeAdvStyleInfo.RightImageIndices"/> properties.
	/// </p>
	/// <p>The order in which the tree node's contents are drawn is as follows:
	/// <list type="number">
	/// <item><description>Checkbox</description></item>
	/// <item><description>Option Buttons</description></item>
	/// <item><description>Left images</description></item>
	/// <item><description>State image</description></item>
	/// <item><description>Node Label</description></item>
	/// <item><description>Right images</description></item>
	/// </list>
	/// The "State image" will be one of <b>OpenImgIndex</b>, <b>ClosedImgIndex</b> and <b>NoChildrenImgIndex</b>.
	/// </p>
	/// <p>
	/// Selecting specific tree nodes and iterating through the <see cref="Nodes"/> collection can be 
	/// achieved by using the following property values: <see cref="FirstNode"/>, 
	/// <see cref="LastNode"/>, <see cref="NextNode"/>, <see cref="PrevNode"/>, <see cref="NextVisibleNode"/>, 
	/// <see cref="PrevVisibleNode"/>. Assign the <see cref="TreeNodeAdv"/> object returned 
	/// by one of the aforementioned properties to the <see cref="TreeViewAdv.SelectedNode"/> property to select that 
	/// tree node in the <b>TreeViewAdv</b> control.
	/// </p>
	/// <p>
	/// Tree nodes can be expanded to display the next level of child tree nodes. 
	/// The user can expand the tree node by pressing the plus (+) button next to the 
	/// TreeNodeAdv, if one is displayed or you can expand the TreeNodeAdv by calling the 
	/// <see cref="Expand"/> method. To expand all child tree node levels in the <see cref="Nodes"/> 
	/// collection, call the <see cref="ExpandAll"/> method. You can collapse the child 
	/// TreeNodeAdv level by calling the <see cref="CollapseAll"/> method or the user can 
	/// press the minus (-) button next to the TreeNodeAdv, if one is displayed. You can 
	/// also alternate the TreeNode between the expanded and collapsed states using the <see cref="Expanded"/> property.
	/// </p>
	/// </remarks>
	/// <example>
	/// <p>
	///  The following example displays customer information in a <see cref="TreeViewAdv"/> 
	///  control. The root tree nodes display customer names, and the child tree 
	///  nodes display the order numbers assigned to each customer. In this 
	///  example, 1,000 customers are displayed with 15 orders each. The 
	///  repainting of the <b>TreeViewAdv</b> is suppressed by using the <see cref="ScrollControl.BeginUpdate"/> 
	///  and <see cref="ScrollControl.EndUpdate"/> methods, and a wait Cursor is displayed while the 
	///  <b>TreeViewAdv</b> creates and paints the <see cref="TreeNodeAdv"/> objects. This example 
	///  assumes you have a Customer object that can hold a collection of Order 
	///  objects. It also assumes that you have created an instance of a 
	///  <b>TreeViewAdv</b> control on a Form.
	/// </p>
	/// <code lang="C#">
	/// // Create a new ArrayList to hold the Customer objects.
	/// private ArrayList customerArray = new ArrayList(); 
	/// 
	/// private void FillMyTreeView()
	/// {
	///		// Add customers to the ArrayList of Customer objects.
	///		for(int x=0; x!=1000; x++)
	///		{
	///			customerArray.Add(new Customer("Customer" + x.ToString()));
	///		}	
	///		// Add orders to each Customer object in the ArrayList.
	///		foreach(Customer customer1 in customerArray)
	///		{
	///			for(int y=0; y!=15; y++)
	///			{
	///				customer1.CustomerOrders.Add(new Order("Order" + y.ToString()));    
	///			}
	///		}
	///		
	///		// Display a wait cursor while the TreeNodeAdvs are being created.
	///		Cursor.Current = new Cursor("C:\\Cursors\\MyWait.cur");
	///		// Clear the TreeViewAdv each time the method is called.
	///		treeViewAdv1.Nodes.Clear();
	///		// Add a root TreeNodeAdv for each Customer object in the ArrayList.
	///		foreach(Customer customer2 in customerArray)
	///		{
	///			treeViewAdv1.Nodes.Add(new TreeNodeAdv(customer2.CustomerName));
	///			// Add a child treenode for each Order object in the current Customer object.
	///			foreach(Order order1 in customer2.CustomerOrders)
	///			{
	///				treeViewAdv1.Nodes[customerArray.IndexOf(customer2)].Nodes.Add(
	///					new TreeNodeAdv(customer2.CustomerName + "." + order1.OrderID));
	///			}
	///		}
	///		// Reset the cursor to the default for all controls.
	///		Cursor.Current = Cursors.Default;
	///	}
	/// </code>
	/// <code lang="VB">
	/// ' Create a new ArrayList to hold the Customer objects.
	/// Private customerArray As New ArrayList()
	/// Private Sub FillMyTreeView()
	///		' Add customers to the ArrayList of Customer objects.
	///		Dim x As Integer
	///		For x = 0 To 999
	///			customerArray.Add(New Customer("Customer" + x.ToString()))
	///		Next x
	///		
	///		' Add orders to each Customer object in the ArrayList.
	///		Dim customer1 As Customer
	///		For Each customer1 In customerArray
	///			Dim y As Integer
	///			For y = 0 To 14
	///				customer1.CustomerOrders.Add(New Order("Order" + y.ToString()))
	///			Next y
	///		Next customer1
	///		
	///		' Display a wait cursor while the TreeNodeAdvs are being created.
	///		Cursor.Current = New Cursor("C:\Cursors\MyWait.cur")
	///		
	///		' Clear the TreeViewAdv each time the method is called.
	///		treeViewAdv1.Nodes.Clear()
	///		
	///		' Add a root TreeNodeAdv for each Customer object in the ArrayList.
	///		Dim customer2 As Customer
	///		For Each customer2 In customerArray
	///			treeViewAdv1.Nodes.Add(New TreeNodeAdv(customer2.CustomerName))
	///			
	///			' Add a child TreeNodeAdv for each Order object in the current Customer object.
	///			Dim order1 As Order
	///			For Each order1 In customer2.CustomerOrders
	///				treeViewAdv1.Nodes(customerArray.IndexOf(customer2)).Nodes.Add( _
	///					New TreeNodeAdv(customer2.CustomerName + "." + order1.OrderID))
	///			Next order1
	///		Next customer2
	///		
	///		' Reset the cursor to the default for all controls.
	///		Cursor.Current = System.Windows.Forms.Cursors.Default
	///		
	///		' Begin repainting the TreeView.
	///		treeViewAdv1.EndUpdate()
	///		End Sub 'FillMyTreeView
	/// </code>
	/// </example>
	[TypeConverter(typeof(TreeNodeAdvConverter))]
	[Serializable()]
	public class TreeNodeAdv :MarshalByRefObject, ICloneable,IComparable,ISupportInitialize,ISerializable
	{
		/// <summary>
		/// Node custom control.
		/// </summary>
		private Control m_customControl = null;
        /// <summary>
        /// Metro Arrow HighLightColor
        /// </summary>
        internal Color PlusMinusArrowColor = Color.Black;
		/// <summary>
		/// Gets or sets node custom control.
		/// </summary>
 		[ Editor( typeof( CustomControlEditor ), typeof( UITypeEditor ) ) ]
		[ DefaultValue( null ) ]
		[ Description( "Gets or sets node custom control." ) ]
		public Control CustomControl
		{
			get
			{
				return m_customControl;
			}
			set
			{
				if( m_customControl != value )
				{
					m_customControl = value;

					if( this.TreeView != null )
					{
						CustomControlCollectionChanging( this, CollectionChangeAction.Refresh );
					}
				}
			}
		}

		#region Variables
		private TreeNodeAdvStyleInfo nodeData = null;
		private ChildTreeNodeAdvStyleInfo childStyle = null;
		private bool expanded;
		private TreeNodeAdvCollection nodes;
		private TreeViewAdv _treeView = null;
		internal TreeNodeAdv parent = null;
		internal TreeNodeAdvPart plusMinus;
		private OptionButtonPart optionButton;
		private int leftImageListXRel = 0;
		private int stateImageListXRel = 0;
		private int rightImageListXRel = 0;
		private int lineRightRel = 0;
		private Rectangle bounds = Rectangle.Empty;

		/// <summary>
		/// Show plus on expand. Use only on LoadOnDemand mode.
		/// </summary>
		private bool m_bShowPlusOnExpand = false;
		/// <summary>
		/// Horizontal offset of text. 
		/// </summary>
		private int textLocationXRel = 0;
		/// <summary>
		/// Width of node text. 
		/// </summary>
		private int textWidth = 0;
		/// <summary>
		/// Position of node with indents in pixels.
		/// </summary>
		private int nodeXRel = 0;
		private CheckBoxPart checkBox;
		private bool visible = false;
		private bool optioned = false;
		private CultureInfo culture = CultureInfo.CurrentCulture;
		private bool expandedOnce = false;
		private int width = 0;
		private int maxX = 0;
		internal TreeNodeAdvAccessibleObject acsoNode;
		internal static TreeNodeAdv checkStateChangingSourceNode;
		private static Hashtable htRecalculatingNodes = new Hashtable();
		// Caching Partial-Checked-State related
		private Hashtable partialCheckedState = null;

		/// <summary>
		/// Image index of image for expand button.
		/// </summary>
		private int m_expandImageIndex = DEF_DEFAULT_IMAGE_INDEX;

		/// <summary>
		/// Image index of image for collapse button.
		/// </summary>
		private int m_collapseImageIndex = DEF_DEFAULT_IMAGE_INDEX;

		#endregion

		#region Constants
		/// <summary>
		/// Inflate offset for drawing selection rectangle.
		/// </summary>
		internal const int c_nDrawTextFlags = DrawTextFormats.DT_EXPANDTABS | DrawTextFormats.DT_NOPREFIX | DrawTextFormats.DT_NOCLIP
			| DrawTextFormats.DT_SINGLELINE | DrawTextFormats.DT_VCENTER;

		/// <summary>
		/// Default image index.
		/// </summary>
		private const int DEF_DEFAULT_IMAGE_INDEX = TreeViewAdv.DEF_DEFAULT_IMAGE_INDEX;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets show plus on expand. Use only on LoadOnDemand mode.
		/// </summary>
		[DefaultValue(false)]
		public bool ShowPlusOnExpand
		{
			get
			{
				return this.m_bShowPlusOnExpand;
			}
			set
			{
				this.m_bShowPlusOnExpand = value;
			}
		}

		/// <summary>
		/// Returns the horizontal padding used between the different parts of the tree node.
		/// </summary>
		public static int PartsPadX
		{
			get{return spc;}
		}
		/// <summary>
		/// Returns the information about the node's appearance and state.
		/// </summary>
		/// <remarks>This property exposes the node's style information store.</remarks>
		[Description("Contains information about the node's appearance and state.")]
		[Category("Appearance")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdvStyleInfo NodeStyle
		{
			get{return nodeData;}
		}

		/// <summary>
		/// Returns the information about the immediate child-nodes' appearance and state.
		/// </summary>
		[Description("Contains information about the immediate child-nodes' appearance and state.")]
		[Category("Appearance - Children")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeNodeAdvStyleInfo ChildStyle
		{
			get{return childStyle;}
		}

		/// <summary>
		/// Gets / sets the font of the node.
		/// </summary>
		[Description("The font of the node.")]
		[Category("Appearance")]
		[Localizable(true)]
		public Font Font
		{
			get
			{
				return this.NodeStyle.Font;
			}
			set
			{
				this.NodeStyle.Font = value;
			}
		}

		public void ResetFont()
		{
			this.NodeStyle.ResetFont();
		}
		
		public bool ShouldSerializeFont()
		{
			return this.NodeStyle.ShouldSerializeFont();
		}

        protected bool ShouldSerializeMultiLine()
        {
            return this.MultiLine = true;
        }

		/// <summary>
		/// Gets / sets the color of the text.
		/// </summary>
		[Description("The Color of the text.")]
		[Category("Appearance")]
		public Color TextColor
		{
			get{return this.NodeStyle.TextColor;}
			set{this.NodeStyle.TextColor = value;}
		}

		public void ResetTextColor()
		{
			this.NodeStyle.ResetTextColor();
		}
		
		public bool ShouldSerializeTextColor()
		{
			return this.NodeStyle.ShouldSerializeTextColor();
		}

		/// <summary>
		/// Gets / sets the background of the node.
		/// </summary>
		[Description("The background of the node.")]
		[Category("Appearance")]
		public BrushInfo Background
		{
			get{return this.NodeStyle.Background;}
			set{this.NodeStyle.Background = value;}
		}

		public void ResetBackground()
		{
			this.NodeStyle.ResetBackground();
		}
		
		protected bool ShouldSerializeBackground()
		{
			return this.NodeStyle.ShouldSerializeBackground();
		}

		/// <summary>
		/// Gets / sets the text of the node.
		/// </summary>
		[Description("The text of the node.")]
		[Category("Appearance")]
		[Localizable(true)]
		public string Text
		{
			get{return this.NodeStyle.Text;}
			set
			{
				if( value != this.NodeStyle.Text )
				{
                    if (this.TreeView != null && !this.TreeView.IsEditing)
                    {
                        this.TreeView.EndEdit(value, false);
                    }
					this.NodeStyle.Text = value;
                    
				}
			}
		}

		public void ResetText()
		{
			this.NodeStyle.ResetText();
		}
		
		protected bool ShouldSerializeText()
		{
			return this.NodeStyle.ShouldSerializeText();
		}

		/// <summary>
		/// Gets / sets the help text of the node.
		/// </summary>
		[Description("The help text of the node.")]
		[Category("Appearance")]
		[Localizable(true)]
		public string HelpText
		{
			get{return this.NodeStyle.HelpText;}
			set{this.NodeStyle.HelpText = value;}
		}

		public void ResetHelpText()
		{
			this.NodeStyle.ResetHelpText();
		}
		
		protected bool ShouldSerializeHelpText()
		{
			return this.NodeStyle.ShouldSerializeHelpText();
		}
        private bool showLine = true ;
        public bool ShowLine
        {
            get { return showLine; }
            set {
                showLine = value; }
        }

		/// <summary>
		/// Gets / sets the height of the node.
		/// </summary>
		[Description("The height of the node.")]
		[Category("Appearance")]
		public int Height
		{
			get{return this.NodeStyle.Height;}
			set{this.NodeStyle.Height = value;}
		}

        private bool multiLine = false;
        /// <summary>
        /// Gets / sets the MultiLine of the node.
        /// </summary>
        [Description("MultiLine of the node.")]
        [Category("Appearance")]
        public bool MultiLine
        {
            get { return multiLine; }
            set { multiLine = value; }
        }

        /// <summary>
        /// Gets / sets the Size of the PlusMinus.
        /// </summary>
        [Description("The height of the node.")]
        [Category("Appearance")]
        public Size PlusMinusSize
        {
            get { return this.PlusMinus.Size; }
            set { this.plusMinus.Size = value; }
        }
		public void ResetHeight()
		{
			this.NodeStyle.ResetHeight();
		}
		
		protected bool ShouldSerializeHeight()
		{
			return this.NodeStyle.ShouldSerializeHeight();
		}
        /// <summary>
        /// Indicates the color of the Check mark.
        /// </summary>
        [DefaultValueAttribute(typeof(Color), "ControlText"), Category("Appearance"),Description("Indicates the color of the Check mark.")]
        public Color CheckColor 
        { 
            get 
            { 
                return this.NodeStyle.CheckColor ; 
            } 
            set 
            { 
                if (value != this.NodeStyle.CheckColor) this.NodeStyle.CheckColor = value;  
            } 
        }
       /// <summary>
       /// Indicates the color of the check mark when it is in intermediate state.
       /// </summary>

        [DefaultValueAttribute(typeof(Color), "ControlDark"), Category("Appearance"), Description("Indicates the color of the check mark when it is in intermediate state.")]
        public Color IntermediateCheckColor 
        { 
            get 
            {
                return this.NodeStyle.IntermediateCheckColor ; 
            }
            set{
                if (value != this.NodeStyle.IntermediateCheckColor ) this.NodeStyle.IntermediateCheckColor  = value;  
            }
        }
        /// <summary>
        /// Indicates the appearance of checkbox background.
        /// </summary>
        [Browsable(false), DefaultValueAttribute(typeof(Brush ), "Window"),Description("Indicates the appearance of checkbox background.")]
        public Brush CheckBoxBackGround 
        { 
            get 
            { 
                return this.NodeStyle.CheckBoxBackground ; 
            } 
            set 
            { 
                if (value != this.NodeStyle.CheckBoxBackground) this.NodeStyle.CheckBoxBackground = value; 
            } 
        }
        /// <summary>
        /// Indicates the appearance of checkbox background when the checkbox is in intermediate state.
        /// </summary>
        [Browsable(false), DefaultValueAttribute(typeof(Brush ), "Control"),Description(" Indicates the appearance of checkbox background when the checkbox is in intermediate state.")]
        public Brush IntermediateCheckBoxBackGround 
        { 
            get 
            { 
                return this.NodeStyle.IntermediateCheckBoxBackground  ; 
            } 
            set 
            { 
                if (value != this.NodeStyle.IntermediateCheckBoxBackground ) this.NodeStyle.IntermediateCheckBoxBackground  = value; 
            } 
        }

		/// <summary>
		/// Indicates whether the checkbox of the node is visible.
		/// </summary>
		[Description("Indicates if the checkbox of the node is visible.")]
		[Category("Appearance")]
		public bool ShowCheckBox
		{
			get{return this.NodeStyle.ShowCheckBox;}
			set{this.NodeStyle.ShowCheckBox = value;}
		}
		public void ResetShowCheckBox()
		{
			this.NodeStyle.ResetShowCheckBox();
		}
		
		protected bool ShouldSerializeShowCheckBox()
		{
			return this.NodeStyle.ShouldSerializeShowCheckBox();
		}

		/// <summary>
        /// Indicates the color of the Option button.
        /// </summary>
        [DefaultValueAttribute(typeof(Color), "White"), Category("Appearance"), Description("Indicates the color of the Option button.")]
        public Color OptionButtonColor
        {
            get
            {
                return this.NodeStyle.OptionButtonColor;
            }
            set
            {
                if (value != this.NodeStyle.OptionButtonColor) this.NodeStyle.OptionButtonColor = value;
            }
        }
        /// <summary>
        /// Indicates the color of the Option button in selected state.
        /// </summary>
        [DefaultValueAttribute(typeof(Color), "Black"), Category("Appearance"), Description("Indicates the color of the Option button in selected state.")]
        public Color SelectedOptionButtonColor
        {
            get
            {
                return this.NodeStyle.SelectedOptionButtonColor;
            }
            set
            {
                if (value != this.NodeStyle.SelectedOptionButtonColor) this.NodeStyle.SelectedOptionButtonColor = value;
            }
        }
		/// <summary>
		/// Indicates whether the option button of the node is visible.
		/// </summary>
		[Description("Indicates if the optionbutton of the node is visible.")]
		[Category("Appearance")]
		public bool ShowOptionButton
		{
			get 
			{
				return this.NodeStyle.ShowOptionButton;
			}
			set 
			{
				this.NodeStyle.ShowOptionButton = value;
			}
		}
		
		
		public void ResetShowOptionButton()
		{
			this.NodeStyle.ResetShowOptionButton();
		}
		
		protected bool ShouldSerializeShowOptionButton()
		{
			return this.NodeStyle.ShouldSerializeShowOptionButton();
		}

		/// <summary>
		/// Indicates whether the plus/minus of the node is visible.
		/// </summary>
		[Description("Indicates if the plus/minus of the node is visible.")]
		[Category("Appearance")]
		public bool ShowPlusMinus
		{
			get 
			{
				return this.NodeStyle.ShowPlusMinus;
			}
			set 
			{
				this.NodeStyle.ShowPlusMinus = value;
			}
		}
		
		
		public void ResetShowPlusMinus()
		{
			this.NodeStyle.ResetShowPlusMinus();
		}
		
		protected bool ShouldSerializeShowPlusMinus()
		{
			return this.NodeStyle.ShouldSerializeShowPlusMinus();
		}

		/// <summary>
		/// Gets / sets the sort order of the node.
		/// </summary>
		[Description("Indicates the sort order of the node.")]
		[Category("Sorting")]
		[Localizable(true)]
		public SortOrder SortOrder
		{
			get 
			{
				return this.NodeStyle.SortOrder;
			}
			set 
			{
				this.NodeStyle.SortOrder = value;
			}
		}
		
		
		public void ResetSortOrder()
		{
			this.NodeStyle.ResetSortOrder();
		}
		
		protected bool ShouldSerializeSortOrder()
		{
			return this.NodeStyle.ShouldSerializeSortOrder();
		}

		/// <summary>
		/// Gets / sets the sort type of the node.
		/// </summary>
		[Description("Indicates the sort type of the node.")]
		[Category("Sorting")]
		[Localizable(true)]
		public TreeNodeAdvSortType SortType
		{
			get 
			{
				return this.NodeStyle.SortType;
			}
			set 
			{
				this.NodeStyle.SortType = value;
			}
		}
		
		public void ResetSortType()
		{
			this.NodeStyle.ResetSortType();
		}
		
		protected bool ShouldSerializeSortType()
		{
			return this.NodeStyle.ShouldSerializeSortType();
		}

		/// <summary>
		/// Gets / sets the <see cref="IComparer"/> object that compares two nodes.
		/// </summary>
		[Description("Indicates the IComparer object that compares two nodes.")]
		[Category("Sorting")]
		public IComparer Comparer
		{
			get 
			{
				return this.NodeStyle.Comparer;
			}
			set 
			{
				this.NodeStyle.Comparer = value;
			}
		}
		
		
		public void ResetComparer()
		{
			this.NodeStyle.ResetComparer();
		}
		
		protected bool ShouldSerializeComparer()
		{
			return this.NodeStyle.ShouldSerializeComparer();
		}

		/// <summary>
		/// Gets / sets the compare options used in the sorting of the node.
		/// </summary>
		[Description("Indicates the compare options used in the sorting of the node.")]
		[Category("Sorting")]
		[Localizable(true)]
		public CompareOptions CompareOptions
		{
			get 
			{
				return this.NodeStyle.CompareOptions;
			}
			set 
			{
				this.NodeStyle.CompareOptions = value;
			}
		}
		
		
		public void ResetCompareOptions()
		{
			this.NodeStyle.ResetCompareOptions();
		}
		
		protected bool ShouldSerializeCompareOptions()
		{
			return this.NodeStyle.ShouldSerializeCompareOptions();
		}

		/// <summary>
		/// Gets / sets the CheckState of the node.
		/// </summary>
		/// <remarks>
		/// <p>Note that setting this property will fire the <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.BeforeCheck"/>
		/// event. If you do not want this event to be fired, you can access the tree's 
		/// internal data structure as follows:</p>
		/// <code lang="C#">
		/// treeNodeAdv.NodeStyle.CheckState = CheckState.Checked;
		/// </code>
		/// <code lang="VB">
		/// treeNodeAdv.NodeStyle.CheckState = CheckState.Checked
		/// </code>
		/// </remarks>
		[Description("Indicates the checkState of the node.")]
		[Category("Appearance")]
		public CheckState CheckState
		{
			get 
			{
				return this.NodeStyle.CheckState;
			}
			set 
			{
				if(this.CheckState != value)
				{
					if(TreeNodeAdv.checkStateChangingSourceNode == null)
						TreeNodeAdv.checkStateChangingSourceNode = this;
		
					try
					{
						// Let the user cancel this setting:
						TreeViewAdv tree = this.TreeView;
						if(tree != null)
						{
							TreeNodeAdvBeforeCheckEventArgs args = 
								new TreeNodeAdvBeforeCheckEventArgs(this, false, value);
							tree.OnBeforeCheck(args);
							if(args.Cancel)
								return;
						}

						// Caching Partial-Checked-State related
						// Cache the state if changing from partial checked state.
						if(this.InteractiveCheckBox && this.CheckState == CheckState.Indeterminate)
							this.CachePartialCheckedState();

						this.NodeStyle.CheckState = value;	

						if(tree != null)
						{
							tree.OnAfterCheck(new TreeNodeAdvEventArgs(this,TreeViewAdvAction.Unknown));

							if(TreeNodeAdv.checkStateChangingSourceNode == this)
								tree.OnAfterInteractiveChecks(new TreeNodeAdvEventArgs(this));
						}
					}
					finally
					{
						if(TreeNodeAdv.checkStateChangingSourceNode == this)
							TreeNodeAdv.checkStateChangingSourceNode = null;
					}
				}
			}
		}
		public void ResetCheckState()
		{
			this.NodeStyle.ResetCheckState();
		}
		
		protected bool ShouldSerializeCheckState()
		{
			return this.NodeStyle.ShouldSerializeCheckState();
		}
		/// <summary>
		/// Gets / sets the base style for the node from which to inherit.
		/// </summary>
		/// <remarks>The specified base style should be available in the <see cref="TreeViewAdv.BaseStyles"/>
		/// collection.</remarks>
		[Description("The base style for the node")]
		[Category("Appearance - Inherited")]
		public string BaseStyle
		{
			get 
			{
				return this.NodeStyle.BaseStyle;
			}
			set 
			{
				this.NodeStyle.BaseStyle = value;
			}
		}
		
		
		public void ResetBaseStyle()
		{
			this.NodeStyle.ResetBaseStyle();
		}
		
		protected bool ShouldSerializeBaseStyle()
		{
			return this.NodeStyle.ShouldSerializeBaseStyle();
		}

		/// <summary>
		/// Gets / sets the object that contains data about the tree node.
		/// </summary>
		/// <value>
		/// An <see cref="System.Object"/> that contains data about the tree node. The default is a null reference (Nothing in Visual Basic).
		/// </value>
		/// <remarks>
		/// <p>Any Object derived type may be assigned to this property. If this property is 
		/// being set through the Windows Forms designer, only text may be assigned.</p>
		/// <p>When the tree node is cloned, if this object is cloneable (implements ICloneable
		/// interface) then it will be.</p>
		/// </remarks>
		[Description("The tag of the node")]
		[Category("Misc"),TypeConverter(typeof(System.ComponentModel.StringConverter))]
		public object Tag
		{
			get{return nodeData.Tag;}
			set{nodeData.Tag = value;}
		}
		public void ResetTag()
		{
			this.NodeStyle.ResetTag();
		}
		
		protected bool ShouldSerializeTag()
		{
			return this.NodeStyle.ShouldSerializeTag();
		}

		/// <summary>
		/// Gets / sets the image indices of the images to be drawn on the left of the node's text.
		/// </summary>
		[Description("The imageindex to be drawn on the left of the node's text.")]
		[Category("Appearance - Images")]
			//		[Editor(typeof(ImageIndexEditor),typeof(UITypeEditor))]
		public int[] LeftImageIndices
		{
			get 
			{
				return this.NodeStyle.LeftImageIndices;
			}
			set 
			{
				this.NodeStyle.LeftImageIndices = value;
			}
		}
		
		public void ResetLeftImageIndices()
		{
			this.NodeStyle.ResetLeftImageIndices();
		}
		
		protected bool ShouldSerializeLeftImageIndices()
		{
			return this.NodeStyle.ShouldSerializeLeftImageIndices();
		}

		/// <summary>
		/// Gets / sets the image indices of the images to be drawn on the right of the node's text.
		/// </summary>
		[Description("The imageindex to be drawn on the right of the node's text.")]
		[Category("Appearance - Images")]
		[Localizable(true)]
		public int[] RightImageIndices
		{
			get 
			{
				return this.NodeStyle.RightImageIndices;
			}
			set 
			{
				this.NodeStyle.RightImageIndices = value;
			}
		}
		
		
		public void ResetRightImageIndices()
		{
			this.NodeStyle.ResetRightImageIndices();
		}
		
		protected bool ShouldSerializeRightImageIndices()
		{
			return this.NodeStyle.ShouldSerializeRightImageIndices();
		}

		/// <summary>
		/// Gets / sets the image index indicating the image in the StateImageList where the node has no children.
		/// </summary>
		[Description("The imageindex indicating the image in the StateImageList where the node has no children.")]
		[Category("Appearance - Images")]
		[Localizable(true)]
		public int NoChildrenImgIndex
		{
			get 
			{
				return this.NodeStyle.NoChildrenImgIndex;
			}
			set 
			{
				this.NodeStyle.NoChildrenImgIndex = value;
			}
		}
		
		
		public void ResetNoChildrenImgIndex()
		{
			this.NodeStyle.ResetNoChildrenImgIndex();
		}
		
		protected bool ShouldSerializeNoChildrenImgIndex()
		{
			return this.NodeStyle.ShouldSerializeNoChildrenImgIndex();
		}

		/// <summary>
		/// Gets / sets the image index in the StateImageList where the node is not expanded.
		/// </summary>
		[Description("Indicates the imageindex in the StateImageList where the node is not expanded.")]
		[Category("Appearance - Images")]
		[Localizable(true)]
		public int ClosedImgIndex
		{
			get 
			{
				return this.NodeStyle.ClosedImgIndex;
			}
			set 
			{
				this.NodeStyle.ClosedImgIndex = value;
			}
		}
		
		
		public void ResetClosedImgIndex()
		{
			this.NodeStyle.ResetClosedImgIndex();
		}
		
		protected bool ShouldSerializeClosedImgIndex()
		{
			return this.NodeStyle.ShouldSerializeClosedImgIndex();
		}

		/// <summary>
		/// Gets / sets the image index in the StateImageList where the node is expanded.
		/// </summary>
		[Description("Indicates the imageindex in the StateImageList where the node is expanded.")]
		[Category("Appearance - Images")]
		[Localizable(true)]
		public int OpenImgIndex
		{
			get 
			{
				return this.NodeStyle.OpenImgIndex;
			}
			set 
			{
				this.NodeStyle.OpenImgIndex = value;
			}
		}
		
		
		public void ResetOpenImgIndex()
		{
			this.NodeStyle.ResetOpenImgIndex();
		}
		
		protected bool ShouldSerializeOpenImgIndex()
		{
			return this.NodeStyle.ShouldSerializeOpenImgIndex();
		}

		/// <summary>
		/// Indicates whether the node's controls will be themed.
		/// </summary>
		[Description("Indicates if the node's controls will be themed.")]
		[Category("Appearance")]
		public bool ThemesEnabled
		{
			get 
			{
				return this.NodeStyle.ThemesEnabled;
			}
			set 
			{
				this.NodeStyle.ThemesEnabled = value;
			}
		}
		
		
		public void ResetThemesEnabled()
		{
			this.NodeStyle.ResetThemesEnabled();
		}
		
		protected bool ShouldSerializeThemesEnabled()
		{
			return this.NodeStyle.ShouldSerializeThemesEnabled();
		}

		/// <summary>
		/// Indicates whether the node will have an interactive checkbox.
		/// </summary>
		[Description("Indicates if the node will have an interactive checkbox.")]
		[Category("Behavior")]
		public bool InteractiveCheckBox
		{
			get 
			{
				return this.NodeStyle.InteractiveCheckBox;
			}
			set 
			{
				this.NodeStyle.InteractiveCheckBox = value;
			}
		}
		
		public void ResetInteractiveCheckBox()
		{
			this.NodeStyle.ResetInteractiveCheckBox();
		}
		
		protected bool ShouldSerializeInteractiveCheckBox()
		{
			return this.NodeStyle.ShouldSerializeInteractiveCheckBox();
		}

		internal int TreeRowIndex
		{
			get
			{
				return Parent != null ? Parent.GetTreeRowIndexOfChild(this) : 0;
			}
		}

		/// <summary>
		/// Indicates whether the node has been expanded at least once.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
		Description("Indicates if the node has been expanded at least once."),
		DefaultValue(false)]
		public bool ExpandedOnce
		{
			get
			{
				return expandedOnce;
			}
			set
			{
				if(this.expandedOnce != value)
				{
					this.expandedOnce = value;
					TreeViewAdv tree = this.TreeView;
					if(tree != null && tree.LoadOnDemand)
					{
						this.UpdatePlusMinusVisibility();
					}
				}
			}
		}

		/// <summary>
		/// Gets / sets the culture of the node.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[Localizable(true)]
		public CultureInfo Culture
		{
			get
			{
				return this.NodeStyle.Culture;;
			}
			set
			{
				this.NodeStyle.Culture = value;
			}
		}

		public void ResetCulture()
		{
			this.NodeStyle.ResetCulture();
		}

		protected bool ShouldSerializeCulture()
		{
			return this.NodeStyle.ShouldSerializeCulture();
		}

		/// <summary>
		/// Indicates whether the node is in editing state.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public bool IsEditing
		{
			get
			{
				return (TreeView.SelectedNode == this) && (TreeView.IsEditing);
			}
		}

		/// <summary>
		/// Indicates whether the node is selected.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public bool IsSelected
		{
			get
			{
				if(this.TreeView != null)
					return TreeView.SelectedNodes.Contains(this);
				else
					return false;
			}
		}

		/// <summary>
		/// Indicates whether the node is the currently active node.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public bool IsActiveNode
		{
			get
			{
				if(this.TreeView != null)
					return TreeView.ActiveNode == this;
				else
					return false;
			}
		}

		[Documentation.DocumentationExclude()]
		internal bool GetIsMirrored()
		{
			if (TreeView != null)
				return TreeView.GetIsMirrored();
			else
				return false;
		}

		/// <summary>
		/// Returns the horizontal distance between the tree border and the beginning
		/// of the node's drawing bounds.
		/// </summary>
		/// <remarks>This property returns a valid value only when queried from
		/// an owner draw paint event like <see cref="TreeViewAdv.AfterNodePaint"/>.</remarks>
		public int NodeX
		{
			get
			{
				int nX = 0;
				if (GetIsMirrored())
				{
					nX = this.bounds.Right - nodeXRel;
				}
				else
				{
					nX = this.bounds.X + nodeXRel;
				}

				return nX;
			}
		}

		/// <summary>
		/// Returns the horizontal distance between the tree border and the beginning
		/// of the node's left images.
		/// </summary>
		/// <remarks>This property returns a valid value only when queried from
		/// an owner draw paint event like <see cref="TreeViewAdv.AfterNodePaint"/>.</remarks>
		public int LeftImagesX
		{
			get
			{
				return this.bounds.X + nodeXRel + this.leftImageListXRel;
			}
		}

		/// <summary>
		/// Returns the horizontal distance between the tree border and the beginning
		/// of the node's state image.
		/// </summary>
		/// <remarks>This property returns a valid value only when queried from
		/// an owner draw paint event like <see cref="TreeViewAdv.AfterNodePaint"/>.</remarks>
		public int StateImageX
		{
			get
			{
				return this.bounds.X + nodeXRel + this.stateImageListXRel;
			}
		}

		/// <summary>
		/// Returns the horizontal distance between the tree border and the beginning
		/// of the node's right images.
		/// </summary>
		/// <remarks>This property returns a valid value only when queried from
		/// an owner draw paint event like <see cref="TreeViewAdv.AfterNodePaint"/>.</remarks>
		public int RightImagesX
		{
			get
			{
				return this.bounds.X + nodeXRel + this.rightImageListXRel;
			}
		}

		/// <summary>
		/// Returns the horizontal distance between the tree border and the beginning
		/// of the node's checkbox.
		/// </summary>
		/// <remarks>This property returns a valid value only when queried from
		/// an owner draw paint event like <see cref="TreeViewAdv.AfterNodePaint"/>.</remarks>
		public int CheckBoxX
		{
			get
			{
				return this.checkBox.Bounds.X;
			}
		}

		/// <summary>
		/// Returns the horizontal distance between the tree border and the beginning
		/// of the node's option button.
		/// </summary>
		/// <remarks>This property returns a valid value only when queried from
		/// an owner draw paint event like <see cref="TreeViewAdv.AfterNodePaint"/>.</remarks>
		public int OptionButtonX
		{
			get
			{
				return this.optionButton.Bounds.X;
			}
		}

		/// <summary>
		/// Returns a <see cref="TreeNodeAdvPart"/> corresponding to the checkbox of a tree node.
		/// </summary>
		public TreeNodeAdvPart CheckBox
		{
			get
			{
				return checkBox;
			}
		}

		/// <summary>
		/// Returns a <see cref="TreeNodeAdvPart"/> corresponding to the option button part of a tree node.
		/// </summary>
		public TreeNodeAdvPart OptionButton
		{
			get
			{
				return optionButton;
			}
		}

		Font HotFont
		{
			get
			{
				Font dFont = NodeStyle.Font;
				return Syncfusion.Drawing.FontUtil.CreateFont(dFont,dFont.Style | FontStyle.Underline);
			}
		}


		/// <summary>
		/// Indicates whether the node's checkbox is checked.
		/// </summary>
		[Description("Indicates if the node's checkbox is checked.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Checked
		{
			get
			{
				return (this.CheckState==CheckState.Checked);
			}
			set
			{
				this.CheckState = (value?CheckState.Checked:CheckState.Unchecked);
			}
		}

		/// <summary>
		/// Indicates whether the node is enabled.
		/// </summary>
		[Description("Specifies if the node is enabled.")]
		[Category("Appearance")]
		[Localizable(true)]
		public bool Enabled
		{
			get
			{
				return NodeStyle.Enabled;
			}
			set
			{
				NodeStyle.Enabled = value;
			}
		}

		public void ResetEnabled()
		{
			this.NodeStyle.ResetEnabled();
		}

		protected bool ShouldSerializeEnabled()
		{
			return this.NodeStyle.ShouldSerializeEnabled();
		}

		/// <summary>
		/// Indicates whether the buttons in the node are enabled.
		/// </summary>
		/// <value>True to enable the buttons; False otherwise.</value>
		/// <remarks>The checkbox and option buttons can be disabled keeping the rest of the node enabled
		/// using this property.</remarks>
		[Description("Specifies if the buttons in the node are enabled.")]
		[Category("Appearance")]
		[Localizable(true)]
		public bool EnabledButtons
		{
			get
			{
				return NodeStyle.EnabledButtons;
			}
			set
			{
				NodeStyle.EnabledButtons = value;
			}
		}

		public void ResetEnabledButtons()
		{
			this.NodeStyle.ResetEnabledButtons();
		}

		protected bool ShouldSerializeEnabledButtons()
		{
			return this.NodeStyle.ShouldSerializeEnabledButtons();
		}

		/// <summary>
		/// Indicates whether the first child should be marked as <see cref="Optioned"/> and this node's <see cref="OptionedChild"/> if none of the other children is Optioned in a parent node.
		/// </summary>
		/// <value>True to ensure a default optioned child; False otherwise.</value>
		[Description("Specifies if atleast one child of the parent node should be Optioned at all times.")]
		[Category("Behavior")]
		public bool EnsureDefaultOptionedChild
		{
			get
			{
				return NodeStyle.EnsureDefaultOptionedChild;
			}
			set
			{
				NodeStyle.EnsureDefaultOptionedChild = value;
			}
		}
		/// <summary>
		/// Resets the <see cref="EnsureDefaultOptionedChild"/> property to its default value.
		/// </summary>
		public void ResetEnsureDefaultOptinedChild()
		{
			this.NodeStyle.ResetEnsureDefaultOptinedChild();
		}
		/// <summary>
		/// Determines if the <see cref="TreeNodeAdvStyleInfo.EnsureDefaultOptionedChild"/> property was modified.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeEnsureDefaultOptinedChild()
		{
			return this.NodeStyle.ShouldSerializeEnsureDefaultOptinedChild();
		}

		/// <summary>
		/// Indicates whether the node's option button is checked.
		/// </summary>
		[Description("Indicates if the node's option button is checked.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Optioned
		{
			get
			{
				return optioned;
			}
			set
			{
				if(optioned!=value)
				{
					optioned = value;
					this.OnCheckStateChanged(EventArgs.Empty);
					TreeViewAdv tree = this.TreeView;
					if(tree != null)
					{
						// Calling Invalidate fixes defect 345  - Clicking Radio/Option buttons fail to cause a Refresh
						if (tree.IsHandleCreated && tree.Visible)
							tree.Invalidate(this.Bounds);
                        tree.OptionedNode = this;
                        tree.OnAfterCheck(new TreeNodeAdvEventArgs(this));
					}
				}
			}
		}

		// Caching Partial-Checked-State related : Start
		private void ClearCachedPartialCheckedState()
		{
			if(this.partialCheckedState != null)
			{
				this.partialCheckedState.Clear();
				this.partialCheckedState = null;
			}
		}
		private void CachePartialCheckedState()
		{
			// Cache the states of the children, anew.
			if(this.partialCheckedState != null)
				this.partialCheckedState.Clear();

			this.partialCheckedState = new Hashtable();
			foreach(TreeNodeAdv node in this.Nodes)
			{
				if(node.ShowCheckBox)
					this.partialCheckedState[node] = node.CheckState;
			}
		}
		private Hashtable CachedPartialCheckedState
		{
			get{return this.partialCheckedState;}
		}
		// Caching Partial-Checked-State related : End
		private void CheckState_Changed()
		{
			//Fire event.
			OnCheckStateChanged(EventArgs.Empty);

			// Changes with respct to Caching Partial-Checked-State related:
			if(NodeStyle.InteractiveCheckBox)
			{
				if((this.CheckState == CheckState.Checked || this.CheckState == CheckState.Unchecked))
				{
					CheckState state = this.CheckState;
					for(int i=0;i<nodes.Count;i++)
					{
						//Set checkstate to the child nodes if interactive checkbox is set.
						if(nodes[i].ShowCheckBox)
							nodes[i].CheckState = state;
					}
				}
				else if(this.CheckState == CheckState.Indeterminate)
				{
					// If there is a cached partial checked state, apply it.
					if(this.CachedPartialCheckedState != null)
					{
						foreach(TreeNodeAdv child in this.Nodes)
						{
							if(child.ShowCheckBox && this.CachedPartialCheckedState.Contains(child))
								child.CheckState = (CheckState)this.CachedPartialCheckedState[child];
						}
					}

				}
			}
		}
		
		private void ShowPlusMinusChanged()
		{
			this.UpdatePlusMinusVisibility();

			this.InvalidateTreeView();
		}
		internal void UpdateAllPlusMinusVisibility()
		{
			foreach(TreeNodeAdv node in this.Nodes)
				node.UpdateAllPlusMinusVisibility();

			this.UpdatePlusMinusVisibility();
		}
		internal void UpdatePlusMinusVisibility()
		{
			bool oldValue = plusMinus.Visible;

			if(NodeStyle.ShowPlusMinus &&
				(this.HasChildren
				||
				(this.TreeView != null && this.TreeView.LoadOnDemand && !this.ExpandedOnce))
				)
			{
				if(this.Level == 1 && this.TreeView != null)
					plusMinus.Visible = this.TreeView.NeedRootLinesSpace;
				else
					plusMinus.Visible = true;
			}
			else 
				plusMinus.Visible = false;

			if(oldValue != plusMinus.Visible && this.TreeView != null)
				this.TreeView.Invalidate(this.Bounds);
		}

		/// <summary>
		/// Indicates whether the node has child nodes.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public bool HasChildren
		{
			get
			{
				return nodes.Count>0;
			}
		}
		internal int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				if(Right > maxX)
				{
					MaxX = Right;
				}
			}
		}

		internal int Right
		{
			get
			{
				return this.bounds.X + nodeXRel + width;
			}
		}
		/// <summary>
		/// Gets / sets the maximum width of all the children and subchildren of this given node.
		/// </summary>
		internal int MaxX
		{
			get
			{
				return maxX;
			}
			set
			{
				if(maxX != value)
				{
					maxX = value;
					if(parent!=null)
					{
						//Notify the parent if maxX has changed and if the parent's maxX is smaller then this value it will be changed to this value.
						parent.childMaxXChanged(maxX);				
					}
					else
					{
						//Notify the TreeView that the Root's maxX has changed and set the HScrollBar's values.
						if(_treeView!=null)
							_treeView.rootMaxXChanged(maxX);
					}
				}
			}
		}

		/// <summary>
		/// Called when the child maxX is changed.
		/// </summary>
		/// <param name="childMax">The child's maxX</param>
		internal void childMaxXChanged(int childMax)
		{
			if(this.Expanded)
			{
				if(childMax>maxX)
				{
					MaxX = childMax;
				}
				else
				{
					// If, in the middle of recalculating, ignore this.
					if(htRecalculatingNodes[this] == null )
						RecalculateMaxX();
				}
			}
		}
		
		/// <summary>
		/// Returns the first child tree node in the tree node collection.
		/// </summary>
		/// <value>The first child TreeNodeAdv in the <see cref="Nodes"/> collection.</value>
		/// <remarks>
		/// The <b>FirstNode</b> is the first child TreeNodeAdv in the 
		/// <see cref="TreeNodeAdvCollection"/> stored in the <see cref="Nodes"/> property of 
		/// the current tree node. If the <see cref="TreeNode"/> has no child tree node, the 
		/// <b>FirstNode</b> property returns a null reference (Nothing in Visual Basic).
		/// </remarks>
		[Browsable(false),EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv FirstNode
		{
			get
			{
				if(this.Nodes.Count <= 0)
					return null;
				else
					return this.Nodes[0];
			}
		}
		/// <summary>
		/// Returns the last child tree node in the tree node collection.
		/// </summary>
		/// <value>The last child TreeNodeAdv in the <see cref="Nodes"/> collection.</value>
		/// <remarks>
		/// The <b>LastNode</b> is the last child TreeNodeAdv in the 
		/// <see cref="TreeNodeAdvCollection"/> stored in the <see cref="Nodes"/> property of 
		/// the current tree node. If the <see cref="TreeNodeAdv"/> has no child tree node, the 
		/// <b>LastNode</b> property returns a null reference (Nothing in Visual Basic).
		/// </remarks>
		[Browsable(false),EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv LastNode
		{
			get
			{
				if(this.Nodes.Count <= 0)
					return null;
				else
					return this.Nodes[this.Nodes.Count - 1];
			}
		}
		/// <summary>
		/// Returns the previous sibling tree node.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> that represents the previous sibling tree node.</value>
		/// <remarks>
		/// The <b>PrevNode</b> is the previous sibling <see cref="TreeNodeAdv"/> in the 
		/// <see cref="TreeNodeAdvCollection"/> stored in the <see cref="TreeViewAdv.Nodes"/> 
		/// property of the tree node's parent <b>TreeNodeAdv</b>. If there is no previous 
		/// tree node, the <b>PrevNode</b> property returns a null reference (Nothing in 
		/// Visual Basic).
		/// </remarks>
		[Browsable(false),EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv PrevNode
		{
			get
			{
				if(parent==null) return null;
				if(parent.Nodes.IndexOf(this)>0)
				{
					return parent.Nodes[parent.Nodes.IndexOf(this)-1];					
				}
				return null;
			}
		}
		//		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		//		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		//		public TreeNodeAdv AnyPrevNode
		//		{
		//			get
		//			{
		//				if(parent==null) return null;
		//				if(parent.Nodes.IndexOf(this)>0)
		//				{
		//					TreeNodeAdv prev = parent.Nodes[parent.Nodes.IndexOf(this)-1];
		//					while(prev.Nodes.Count>0)
		//						prev = prev.Nodes[prev.Nodes.Count-1];
		//					return prev;
		//					
		//				}
		//				else return parent;
		//			}
		//		}

		/// <summary>
		/// Returns the previous visible tree node.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> that represents the previous 
		/// visible tree node.</value>
		/// <remarks>
		/// The <b>PrevVisibleNode</b> can be a child, sibling or a tree node from 
		/// another branch. If there is no previous tree node, the <b>PrevVisibleNode</b> 
		/// property returns a null reference (Nothing in Visual Basic).
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv PrevVisibleNode
		{
			get
			{
				if(parent==null) return null;
				if(parent.Nodes.IndexOf(this)>0)
				{
					TreeNodeAdv prev = parent.Nodes[parent.Nodes.IndexOf(this)-1];
					while(prev.Expanded && prev.Nodes.Count>0)
						prev = prev.Nodes[prev.Nodes.Count-1];
					return prev;
					
				}
				else return parent;
			}
		}

		/// <summary>
		/// Returns the previous selectable tree node.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> that represents the previous 
		/// selectable tree node.</value>
		/// <remarks>
		/// The <b>PrevSelectableNode</b> can be a child, sibling or a tree node from 
		/// another branch. If there is no previous tree node, the <b>PrevSelectableNode</b> 
		/// property returns a null reference (Nothing in Visual Basic).
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv PrevSelectableNode
		{
			get
			{
				TreeNodeAdv prev = this.PrevVisibleNode;
				while(prev != null && prev.Enabled == false)
				{
					prev = prev.PrevVisibleNode;
				}
				return prev;
			}
		}

		/// <summary>
		/// Returns the next sibling tree node.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> that represents the next sibling tree node.</value>
		/// <remarks>
		/// The <b>NextNode</b> is the next sibling <b>TreeNodeAdv</b> in the 
		/// <see cref="TreeNodeCollection"/> stored in the <see cref="TreeNodeAdv.Nodes"/> 
		/// property of the tree node's parent <b>TreeNodeAdv</b>. If there is no next 
		/// tree node, the <b>NextNode</b> property returns a null reference (Nothing in 
		/// Visual Basic).
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv NextNode
		{
			get
			{
				if(parent==null) return null;
				if(parent.Nodes.IndexOf(this) < parent.Nodes.Count-1) return parent.Nodes[parent.Nodes.IndexOf(this)+1];

				return null;
			}
		}
		//		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		//		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		//		public TreeNodeAdv AnyNextNode
		//		{
		//			get
		//			{
		//				if(parent==null) return null;
		//				if(nodes.Count>0) return nodes[0];
		//				if(parent.Nodes.IndexOf(this) < parent.Nodes.Count-1) return parent.Nodes[parent.Nodes.IndexOf(this)+1];
		//
		//				TreeNodeAdv node = parent;
		//				while(node !=null)
		//				{
		//					if(node.Parent==null) return null;
		//					if(node.Parent.Nodes.IndexOf(node) < node.Parent.Nodes.Count-1) return node.Parent.Nodes[node.Parent.Nodes.IndexOf(node)+1];
		//					node = node.Parent;
		//				}
		//				return null;
		//			}
		//		}
		/// <summary>
		/// Returns the next visible tree node.
		/// </summary>
		/// <value>
		/// A TreeNodeAdv that represents the next visible tree node.
		/// </value>
		/// <remarks>
		/// The <b>NextVisibleNode</b> can be a child, sibling or a tree node from 
		/// another branch. If there is no next tree node, the <b>NextVisibleNode</b> property 
		/// returns a null reference (Nothing in Visual Basic).
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv NextVisibleNode
		{
			get
			{
				if(nodes.Count>0 && expanded) return nodes[0];
				if(parent==null) return null;
				if(parent.Nodes.IndexOf(this) < parent.Nodes.Count-1) return parent.Nodes[parent.Nodes.IndexOf(this)+1];

				TreeNodeAdv node = parent;
				while(node !=null)
				{
					if(node.Parent==null) return null;
					if(node.Parent.Nodes.IndexOf(node) < node.Parent.Nodes.Count-1) return node.Parent.Nodes[node.Parent.Nodes.IndexOf(node)+1];
					node = node.Parent;
				}
				return null;
			}
		}

		/// <summary>
		/// Returns the next node.
		/// </summary>
		/// <returns>TreeNodeAdv that represents the next node from the current treenode.</returns>
		/// <remarks>
		/// This method will returns the next node regardless whether the node is collapsed state or not.
		/// </remarks>
		public TreeNodeAdv GetNextNode()
		{
			if (nodes.Count > 0) return nodes[0];
			if (parent == null) return null;
			if (parent.Nodes.IndexOf(this) < parent.Nodes.Count - 1) return parent.Nodes[parent.Nodes.IndexOf(this) + 1];

			TreeNodeAdv node = parent;
			while (node != null)
			{
				if (node.Parent == null) return null;
				if (node.Parent.Nodes.IndexOf(node) < node.Parent.Nodes.Count - 1) return node.Parent.Nodes[node.Parent.Nodes.IndexOf(node) + 1];
				node = node.Parent;
			}
			return null;
		}

		/// <summary>
		/// Returns the next selectable tree node.
		/// </summary>
		/// <value>
		/// A TreeNodeAdv that represents the next selectable tree node.
		/// </value>
		/// <remarks>
		/// The <b>NextSelectableNode</b> can be a child, sibling or a tree node from 
		/// another branch. If there is no next tree node, the <b>NextSelectableNode</b> property 
		/// returns a null reference (Nothing in Visual Basic).
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv NextSelectableNode
		{
			get
			{
				TreeNodeAdv next = this.NextVisibleNode;
	        
        		while(next != null && next.Enabled == false)
				{
					next = next.NextVisibleNode;
				}
          
				return next;
			}
		}
		/// <summary>
		/// Returns the child node who's option button is checked.
		/// </summary>
		/// <returns></returns>
		protected internal TreeNodeAdv GetOptionedChild()
		{
			if( nodes != null && nodes.Count > 0 )
			{
				for( int i = 0; i < nodes.Count ; i++ )
				{
					if( nodes[ i ].Optioned ) return nodes[ i ];
				}
			}

			return null;

		}
		/// <summary>
		/// Returns the child node who's option button is checked.
		/// </summary>
		/// <value>
		/// A TreeNodeAdv that represents the next visible tree node.
		/// </value>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNodeAdv OptionedChild
		{
			get
			{
				for(int i=0;i<nodes.Count;i++)
				{
					if(nodes[i].Optioned || (this.TreeView.OptionedNode != null && this.nodes[i] == this.TreeView.OptionedNode)) 
                        return nodes[i];
				}
				if(this.EnsureDefaultOptionedChild && nodes.Count>0)
				{
					TreeNodeAdv node = nodes[0];
					node.optioned = true;
					if (this.TreeView != null && node.OptionButton.Visible)
					{
						this.TreeView.Invalidate(node.OptionButton.Bounds);
					}

					return node;
				}
				return null;
			}
		}


		/// <summary>
		/// Returns the bounds of the tree node.
		/// </summary>
		/// <value>
		/// The <see cref="System.Drawing.Rectangle"/> that represents the bounds of the tree node.
		/// </value>
		/// <remarks>
		/// The coordinates are relative to the upper left corner of the <see cref="TreeViewAdv"/> control.
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle Bounds
		{
			get
			{
				return bounds;
			}
		}
		
		[Documentation.DocumentationExclude()]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public Rectangle DragCueBounds
		{
			get
			{

				if(this.TreeView != null && this.TreeView.StateImageList != null)
				{
					int left = this.bounds.X + nodeXRel + this.stateImageListXRel;
					int right = this.TextBounds.Right;
					return new Rectangle(left, bounds.Y, right - left, bounds.Height);
				}
				else
					return this.TextBounds;
			}
		}
		
		private Size m_printTextSize = Size.Empty;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal Rectangle PrintTextBounds
		{
			get
			{
				return new Rectangle( TextBounds.Location, m_printTextSize );
			}
		}
		/// <summary>
		/// Returns the bounds of the text area of the node.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle TextBounds
		{
			get
			{
				int nX = 0;
				int nInc = this.nodeXRel + this.textLocationXRel;
				if (GetIsMirrored())
				{
					nX = this.bounds.Right - nInc - this.textWidth;
				}
				else
				{
					nX = this.bounds.X + nInc;
				}

				return new Rectangle( nX, this.Bounds.Y, this.textWidth, this.Bounds.Height);
			}
		}

		/// <summary>
		/// Returns the bounds of the left images, state images, text area and the right images of the node.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle TextAndImageBounds
		{
			get
			{
				int nInc = this.nodeXRel;
				if(this.leftImageListXRel > 0)
					nInc += this.leftImageListXRel;
				else if(this.stateImageListXRel > 0)
					nInc += this.stateImageListXRel;
				else
					nInc += this.textLocationXRel;

				int nWidth = nodeXRel + width - nInc;
				
				int nX = 0;
				if (GetIsMirrored())
				{
					nX = this.bounds.Right - nInc - nWidth;
				}
				else
				{
					nX = this.bounds.X + nInc;
				}

				return new Rectangle( nX, this.Bounds.Y, nWidth, this.Bounds.Height );
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal Rectangle iBounds
		{
			get{return bounds;}
			set{bounds = value;}
		}
		
		/// <summary>
		/// Indicates whether the tree node is visible.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsVisible
		{
			get{return visible;}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool Visible
		{
			get{return visible;}
			set
			{
				this.RecalculateDimensions();
				if( this.TreeView != null )
				{
					this.TreeView.NeedUpdateCustomControls = true;
				}

				if (visible && !value)
					this.AdjustVisibleNodeCount(-1);
				else if (!visible && value)
					this.AdjustVisibleNodeCount(1);

				visible = value;

				if(expanded || !value)
					SetChildrenVisible(value);
				
			}
		}
		/// <summary>
		/// Returns a <see cref="TreeNodeAdvPart"/> corresponding to the plus-minus part of a tree node.
		/// </summary>
		public TreeNodeAdvPart PlusMinus
		{
			get{return plusMinus;}
		}

		/// <summary>
		/// Returns the parent tree node of the current tree node, if there is any.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> that represents the parent of the current 
		/// tree node.</value>
		/// <remarks>
		/// If this is the top most node in the tree, the Parent property returns the
		/// TreeViewAdv's <see cref="TreeViewAdv.Root"/> node.
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Gets the parent tree node of the current tree node, if there is any.")]
		public TreeNodeAdv Parent
		{
			get{return parent;}
		}

		/// <summary>
		/// Returns the position of the tree node in the <see cref="Parent"/>'s tree node collection.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Gets the position of the tree node in the parent's tree nodes collection.")]
		public int Index
		{
			get
			{
				if(this.Parent != null)
					return this.Parent.Nodes.IndexOf(this);
				else
					return -1;
			}
		}

		/// <summary>
		/// Returns the level of the node.
		/// </summary>
		/// <remarks>
		/// Specifies how deep a node is in the tree. The top-most visible nodes belong
		/// to level 1. The <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.Root"/> node is level 0.
		/// </remarks>
		public int Level
		{
			get 
			{ 
				if (parent == null)
					return 0;
				else 
					return parent.Level+1;
			}
		}

		/// <summary>
		/// Gets / sets the parent tree view that the tree node is assigned to.
		/// </summary>
		/// <value>A <see cref="TreeViewAdv"/> that represents the parent tree view that 
		/// the tree node is assigned to.</value>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeViewAdv TreeView
		{
			get{ if(parent==null) return _treeView; return parent.TreeView;}
			set{_treeView = value;}
		}

		/// <summary>
		/// Returns the path from the root tree node to the current tree node.
		/// </summary>
		/// <value>The path from the root tree node to the current tree node.</value>
		/// <remarks>
		/// <p>You can also use the more flexible <see cref="GetPath"/> method to
        /// get the path with a specific path separator.</p>
		/// <p>The path consists of the labels of all of the tree nodes that must be 
		/// navigated to get to this tree node, starting at the root tree node. The node 
		/// labels are separated by the delimiter character specified in the 
		/// <see cref="TreeViewAdv.PathSeparator"/> property of the TreeViewAdv control that 
		/// contains this node. For example, if the delimiter character of the tree view 
		/// control named "Location" is set to the backslash character, (\), the <b>FullPath</b> 
		/// property value is "Country\Region\State".</p>
		/// </remarks>
		public string FullPath
		{
			get
			{
				if(this.TreeView != null)
				{
					return this.TreeView.GetPathFromNode(this);
				}
				return String.Empty;
			}
		}

		private object tag=null;
		/// <summary>
		/// Gets / sets the object that contains data about the tree node.
		/// </summary>
		/// <value>
		/// An <see cref="System.Object"/> that contains data about the tree node. The default is a null reference (Nothing in Visual Basic).
		/// </value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object TagObject
		{
			get
			{
				return tag;
			}
			set
			{
				this.tag = value;
			}
		}


		/// <summary>
		/// Returns the collection of <see cref="TreeNodeAdv"/> objects assigned to the 
		/// current tree node.
		/// </summary>
		/// <value>
		/// A <see cref="TreeNodeAdvCollection"/> that represents the tree nodes assigned 
		/// to the current tree node.
		/// </value>
		/// <remarks>
		/// The <see cref="Nodes"/> property can hold a collection of other <see cref="TreeNodeAdv"/> 
		/// objects. Each of the tree node in the collection has a <see cref="Nodes"/> property 
		/// that can contain its own <see cref="TreeNodeAdvCollection"/>. This nesting of 
		/// tree nodes can make it difficult to navigate a tree structure. The <see cref="FullPath"/> 
		/// property makes it easier to determine your location in a tree.
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeNodeAdvCollection Nodes
		{
			get{return nodes;}
		}
		/// <summary>
		/// Indicates the expanded state of a tree node.
		/// </summary>
		/// <value>True if the tree node is in the expanded state; false otherwise.
		/// </value>
		[Description("Indicates if the node is expanded.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Expanded
		{
			get{return expanded;}
			set
			{
				if(expanded == value) return;

				TreeViewAdv tree = this.TreeView;
				bool bUpdating = false;
				if(tree!=null)
					bUpdating=this.TreeView.SuspendExpandRecalculate;

				
				if( tree != null  )
				{
					if(this.parent != null )
					{
						if(!tree.ExpandedChanging(this, value))
							return;
					}
					if(value && this.parent!=null)
					{
						this.ExpandedOnce = true;
					}
				}
				

				expanded = value;

				// Set the children visibile only if I am visible
				SetChildrenVisible(value && this.visible);

				if( tree != null && HasChildren && !( this.ShowPlusOnExpand && tree.LoadOnDemand ) )
				{
					tree.ExpandedChanged(this, value);
				}

				if(!bUpdating)
					this.RecalculateMaxX();

                if (this.TreeView!=null && !this.TreeView.RecalculateExpansion)
                {
                    foreach (TreeNodeAdv node in Nodes)
                    {
                        node.RecalculateDimensions();
                        node.InvalidateTreeView();
                    }
				}
			}

		}

		/// <summary>
		/// Gets or sets image index of image for expand button.
		/// </summary>
		[
		DefaultValue( DEF_DEFAULT_IMAGE_INDEX ),
		Description( "Image index of image for expand button." ),
		Category( "Appearance - Images" )
		]
		public int ExpandImageIndex
		{
			get
			{
				return m_expandImageIndex;
			}
			set
			{
				if( value != m_expandImageIndex )
				{
					m_expandImageIndex = value;
					OnExpandImageIndexChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets or sets image index of image for collapse button.
		/// </summary>
		[
		DefaultValue( DEF_DEFAULT_IMAGE_INDEX ),
		Description( "Image index of image for collapse button" ),
		Category( "Appearance - Images" )
		]
		public int CollapseImageIndex
		{
			get
			{
				return m_collapseImageIndex;
			}
			set
			{
				if( value != m_collapseImageIndex )
				{
					m_collapseImageIndex = value;
					OnCollapseImageIndexChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets image for collapse button.
		/// </summary>
		private Image CollapseImage
		{
			get
			{
				return GetNodeStateImage();
			}
		}

		
		/// <summary>
		/// Gets image for expand button.
		/// </summary>
		private Image ExpandImage
		{
			get
			{
				return GetNodeStateImage();
			}
		}

		/// <summary>
		/// Gets image for collapse or expand button.
		/// </summary>
		private Image GetNodeStateImage()
		{
			Image stateImage = null;
				
			if( this.TreeView != null && this.TreeView.NodeStateImageList != null )
			{
				ImageList.ImageCollection images = this.TreeView.NodeStateImageList.Images;
				
				if( images != null && images.Count > 0 )
				{
					int imageIndex = this.Expanded ? this.ExpandImageIndex : this.CollapseImageIndex;
					int defaultImageIndex = this.Expanded ? this.TreeView.DefaultExpandImageIndex 
						: this.TreeView.DefaultCollapseImageIndex;

					if( imageIndex > DEF_DEFAULT_IMAGE_INDEX && imageIndex < images.Count )
					{
						stateImage = images[ imageIndex ];
					}
					else if( defaultImageIndex > DEF_DEFAULT_IMAGE_INDEX && defaultImageIndex < images.Count )
					{
						stateImage = images[ defaultImageIndex ];
					}
				}
			}

			return stateImage;
		}
		

		#endregion

		#region Node & Rows

		/// <summary>
		/// Removes itself from the parent node, if there is any.
		/// </summary>
		public void Remove()
		{
			if(this.Parent != null)
				this.Parent.Nodes.Remove(this);
		}

		internal int GetTreeRowIndexOfChild(TreeNodeAdv child)
		{
			int rowIndex = TreeRowIndex+1;
			int count = nodes.Count;
			for (int n = 0; n < count; n++)
			{
				TreeNodeAdv node = nodes[n];
				if (node == child)
					break;
				rowIndex += node.VisibleNodeCount;
			}
			return rowIndex;
		}

		int visibleNodeCount = -1;

		internal int VisibleNodeCount
		{
			get
			{
				if (visibleNodeCount == -1)
				{
					// Calculate visible node count only if this has been added to the tree.
					if(this.TreeView == null)
						return 0;

					visibleNodeCount = 0;
					for (int n = 0; n < nodes.Count; n++)
					{
						TreeNodeAdv node = nodes[n];
						if (node.Visible)
							visibleNodeCount += nodes[n].VisibleNodeCount;
					}
					if (Visible)
						visibleNodeCount++;
				}
				return visibleNodeCount;
			}
		}

		internal void ResetVisibleNodeCount(bool recurseOnChildren)
		{
			bool recurseOnParent = visibleNodeCount > 0;

			visibleNodeCount = -1;

			if(recurseOnParent && this.Parent != null)
				this.Parent.ResetVisibleNodeCount(false);

			if(recurseOnChildren)
			{
				for (int n = 0; n < nodes.Count; n++)
				{
					TreeNodeAdv node = nodes[n];
					node.ResetVisibleNodeCount(true);
				}
			}
		}

		// call this when node gets expanded or collapsed, nodes are changed or height is changed.
		internal void AdjustVisibleNodeCount(int delta)
		{
			
			if (visibleNodeCount != -1)
			{
				visibleNodeCount += delta;
			}
			if (Parent != null)
				Parent.AdjustVisibleNodeCount(delta);
		}

		/// <summary>
		/// Returns the number of child tree nodes.
		/// </summary>
		/// <param name="includeSubTrees"><b>True</b> if the resulting count includes all tree 
		/// nodes indirectly rooted at this tree node; <b>false</b> otherwise. </param>
		/// <returns>The number of child tree nodes assigned to the <see cref="Nodes"/> collection.</returns>
		public int GetNodeCount(bool includeSubTrees)
		{
			int count = this.Nodes.Count;
			if(includeSubTrees)
			{
				foreach(TreeNodeAdv node in this.Nodes)
				{
					count += node.GetNodeCount(true);
				}
			}
			return count;
		}

		internal TreeNodeAdv GetNodeAtAbsoluteRowIndex(int rowIndex)
		{
			return GetNodeAtRelativeRowIndex(rowIndex-TreeRowIndex);
		}

		internal TreeNodeAdv GetNodeAtRelativeRowIndex(int rowIndex)
		{
			if (rowIndex == 0)
				return this;
			
			rowIndex--;
			int count = nodes.Count;
			for (int n = 0; n < count; n++)
			{
				TreeNodeAdv node = nodes[n];
				int nodeVisibleNodeCount = node.VisibleNodeCount;
				if (rowIndex < nodeVisibleNodeCount)
					return node.GetNodeAtRelativeRowIndex(rowIndex);
				rowIndex -= nodeVisibleNodeCount;
			}
			return null;
		}

		//		int visibleHeight;
		//
		//		internal int VisibleHeight
		//		{
		//			get
		//			{
		//				return visibleHeight;
		//			}
		//		}
		//
		//		// call this when node gets expanded, collapsed, nodes are changed or height is changed.
		//		internal int AdjustVisibleHeight(int delta)
		//		{
		//			visibleHeight += delta;
		//			if (Parent != null)
		//				Parent.AdjustVisibleHeight(delta);
		//		}
		//		comment out because it is tricky when Height of a base style changes...


		#endregion

		#region ImagePadding
		private int m_leftImagePadding = 0;
		private int m_rightImagePadding = 0;
		private int m_leftStateImagePadding = 0;
		private int m_rightStateImagePadding = 0;

		/// <summary>
		/// Gets / sets the space between images for LeftImageList.
		/// </summary>
		[ 
		Category( "Appearance - Images" ),
		DefaultValue( 0 )
		]
		public int LeftImagePadding
		{
			get
			{
				return m_leftImagePadding;
			}
			set
			{
				if( value != m_leftImagePadding )
				{
					if( value < 0 ) throw new ArgumentOutOfRangeException( "value" );

					m_leftImagePadding = value;
					
					OnLeftImagePaddingChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the space between images for RightImageList.
		/// </summary>
		[ 
		Category( "Appearance - Images" ),
		DefaultValue( 0 )
		]
		public int RightImagePadding
		{
			get
			{
				return m_rightImagePadding;
			}
			set
			{
				if( value != m_rightImagePadding )
				{
					if( value < 0 ) throw new ArgumentOutOfRangeException( "value" );

					m_rightImagePadding = value;
					
					OnRightImagePaddingChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the space before StateImage.
		/// </summary>
		[ 
		Category( "Appearance - Images" ),
		DefaultValue( 0 )
		]
		public int LeftStateImagePadding
		{
			get
			{
				return m_leftStateImagePadding;
			}
			set
			{
				if( value != m_leftStateImagePadding )
				{
					if( value < 0 ) throw new ArgumentOutOfRangeException( "value" );

					m_leftStateImagePadding = value;
					
					OnStateImagePaddingChanged();
				}
			}
		}

		/// <summary>
		/// Gets / sets the space after StateImage.
		/// </summary>
		[ 
		Category( "Appearance - Images" ),
		DefaultValue( 0 )
		]
		public int RightStateImagePadding
		{
			get
			{
				return m_rightStateImagePadding;
			}
			set
			{
				if( value != m_rightStateImagePadding )
				{
					if( value < 0 ) throw new ArgumentOutOfRangeException( "value" );

					m_rightStateImagePadding = value;
					
					OnStateImagePaddingChanged();
				}
			}
		}

		/// <summary>
		/// Determines whether the distance between the node's text and Leftimage is changed.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnLeftImagePaddingChanged()
		{
			RecalculateDimensions();
		}
		/// <summary>
		/// Determines whether the distance between the node's text and Rightimage is changed.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnRightImagePaddingChanged()
		{
			RecalculateDimensions();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnStateImagePaddingChanged()
		{
			RecalculateDimensions();
		}
		#endregion

		internal void StyleChanged(object sender,StyleChangedEventArgs e)
		{
			StyleInfoProperty sip = e.Sip;

			if( null != sip )
			{
				switch( sip.PropertyName )
				{
					case "CheckState":
						CheckState_Changed();
						break;

					case "ShowPlusMinus":
						ShowPlusMinusChanged();
						RecalculateDimensions();
						break;

					case "ShowOptionButton":					
					case "ShowCheckBox":
					case "LeftImageIndices":
					case "StateImage":
					case "RightImageIndices":
					case "Text":
					case "Font":
						RecalculateDimensions();
						break;

					case "Comparer":
						this.Nodes.comparer = this.NodeStyle.Comparer;
						break;
				} 
			}

			TreeViewAdv tree = this.TreeView;

			if( tree != null )
			{
				tree.NeedUpdateCustomControls = true;
			}

			InvalidateTreeView();
		}


		#region Undo\Redo implementation
		/// <summary>
		/// Returns the TreeView History manager this node belongs to.
		/// </summary>
		protected HistoryManager HistoryManager
		{
			get
			{
				HistoryManager historyManager = null;

				if( this.TreeView != null && this.TreeView.HistoryEnabled )
				{
					historyManager = this.TreeView.HistoryManager;
				}

				return historyManager;
			}
		}
		private bool m_bIsUndoRedoPerforming = false;
		/// <summary>
		/// Indicates whether the node is in UndoRedo state.
		/// </summary>
		protected internal bool IsUndoRedoPerforming
		{
			get
			{
				return m_bIsUndoRedoPerforming;
			}
			set
			{
				if( value != m_bIsUndoRedoPerforming )
				{
					m_bIsUndoRedoPerforming = value;
				}
			}
		}
		#endregion


		#region Serialization

		private TreeNodeAdv(SerializationInfo info,	StreamingContext context) :this()
		{
			nodeData = new TreeNodeAdvStyleInfo(info.GetValue("NodeStyle",typeof(TreeNodeAdvStyleInfoStore)) as TreeNodeAdvStyleInfoStore);
			nodeData.Changed += new StyleChangedEventHandler(StyleChanged);

			this.childStyle = new ChildTreeNodeAdvStyleInfo(new TreeViewAdvStyleInfoIdentity(this), info.GetValue("ChildStyle",typeof(TreeNodeAdvStyleInfoStore)) as TreeNodeAdvStyleInfoStore);
		}
        
		/// <summary>
		/// Populates the provided SerializationInfo with the data needed to serialize the object .
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData(SerializationInfo info,StreamingContext context) 
		{
			info.AddValue("NodeStyle",NodeStyle.Store);
			info.AddValue("ChildStyle",ChildStyle.Store);
		}

		#endregion

		#region Events
		/// <summary>
		/// Occurs when the check state of the node changes.
		/// </summary>
		/// <remarks>
		/// <para>This event will be fired when the CheckedState property of the node has changed or when a new node has been Optioned.</para>
		/// <para>You could alternatively listen to the <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.AfterCheck"/>
		/// event of the tree which will be called when the CheckState is changing for any node in the tree.
		/// If you want to cancel the check state change, then listen to 
		/// <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.BeforeCheck"/> of the tree.</para>
		/// </remarks>
		public event EventHandler CheckStateChanged;

		/// <summary>
		/// Raises the CheckStateChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnCheckStateChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnCheckStateChanged in a derived
		/// class, be sure to call the base class's OnCheckStateChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnCheckStateChanged(EventArgs e)
		{
			if(CheckStateChanged != null)
			{
				CheckStateChanged(this,e);
			}
		}

		/// <summary>
		/// Occurs when <see cref="ExpandImageIndex"/> is changed.
		/// </summary>
		[
		Description("Occurs when ExpandImageIndex is changed."),
		Category("Property Changed")
		]
		public event EventHandler ExpandImageIndexChanged;

		/// <summary>
		/// Occurs when <see cref="CollapseImageIndex"/> is changed.
		/// </summary>
		[
		Description("Occurs when CollapseImageIndex is changed."),
		Category("Property Changed")
		]
		public event EventHandler CollapseImageIndexChanged;

		private void RaiseExpandImageIndexChanged()
		{
			if( ExpandImageIndexChanged != null )
			{
				ExpandImageIndexChanged( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Raises the ExpandImageIndexChanged event.
		/// </summary>
		protected virtual void OnExpandImageIndexChanged()
		{
			InvalidateTreeView();

			RaiseExpandImageIndexChanged();
		}

		private void RaiseCollapseImageIndexChanged()
		{
			if( CollapseImageIndexChanged != null )
			{
				CollapseImageIndexChanged( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Raises the CollapseImageIndexChanged event.
		/// </summary>
		protected virtual void OnCollapseImageIndexChanged()
		{
			InvalidateTreeView();

			RaiseCollapseImageIndexChanged();
		}

		#endregion

		#region Initialization
		void ISupportInitialize.BeginInit(){}
		void ISupportInitialize.EndInit()
		{
			nodes.Sort(NodeStyle.SortOrder);
		}
		/// <overloaded>
		/// Initializes a new instance of the <see cref="TreeNodeAdv"/> class.
		/// </overloaded>
		/// <summary>
		/// Initializes a new instance of the <see cref="TreeNodeAdv"/> class.
		/// </summary>
		public TreeNodeAdv()
		{
			Initialize("TreeNodeAdv",null);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="TreeNodeAdv"/> class with the specified label text.
		/// </summary>
		public TreeNodeAdv(string text)
		{
			Initialize(text,null);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="TreeNodeAdv"/> class 
		/// with the specified label text and child tree nodes.
		/// </summary>
		public TreeNodeAdv(string text,TreeNodeAdv[] nodes)
		{
			Initialize(text,nodes);
		}

		private void Initialize(string text,TreeNodeAdv[] nodeArray)
		{
			m_primitives = new TreeNodePrimitivesCollection();
			m_primitives.CollectionChanged += new CollectionChangeEventHandler( OnPrimitivesCollectionChanged );
			this.nodeData = new TreeNodeAdvStyleInfo(new TreeNodeAdvStyleInfoIdentity(this));
			nodeData.Changed += new StyleChangedEventHandler(StyleChanged);
			nodeData.Text = text;

			this.childStyle = new ChildTreeNodeAdvStyleInfo(new TreeViewAdvStyleInfoIdentity(this));

			plusMinus = new TreeNodeAdvPart(this);
			plusMinus.Size = new Size(9,9);


			checkBox = new CheckBoxPart(this);
			checkBox.Size = new Size(13,13);

			optionButton = new OptionButtonPart(this);
			optionButton.Size = new Size(14,14);

			nodes = new TreeNodeAdvCollection();
			nodes.CollectionChanged += new CollectionChangeEventHandler(this.Nodes_CollectionChanged);
			nodes.BeforeRemoving += new CollectionChangeEventHandler( nodes_BeforeRemoving );
			if(nodeArray!=null) nodes.AddRange(nodeArray);
			this.RecalculateDimensions();
		}

		#endregion

		#region Sorting
		int IComparable.CompareTo(object obj)
		{
			if(obj is TreeNodeAdv)
			{
				TreeNodeAdv node = (TreeNodeAdv)obj;
				if(parent==null)
				{
					return NodeStyle.Text.CompareTo(node.NodeStyle.Text);
				}
				switch(parent.NodeStyle.SortType)
				{
					case TreeNodeAdvSortType.Text:return this.NodeTextCompare(node.Text);
					case TreeNodeAdvSortType.Tag:
					{
						int rVal = this.NodeTagCompare(node.Tag);
						return rVal;
					}
					case TreeNodeAdvSortType.CheckBox:
					{
						if(this.Checked)
						{
							if(node.Checked) return 0;
							return 1;
						}
						else
						{
							if(node.Checked) return -1;
							else
							{
								if(this.CheckState == CheckState.Indeterminate)
								{
									if(node.CheckState == CheckState.Indeterminate) return 0;
									else return 1;									
								}
								else
								{
									if(node.CheckState == CheckState.Unchecked) return 0;
									else return -1;
								}
							}
						}
					}
				}
			}
				// obj will be string when BinarySearch is called with a Text.
			else if(obj is string)
			{
				string text = (string)obj;

				if(parent==null)
					return this.NodeTextCompare(text);

				switch(parent.SortType)
				{
					case TreeNodeAdvSortType.Tag:return this.NodeTagCompare(text);
					default:
					case TreeNodeAdvSortType.Text:return this.NodeTextCompare(text);
				}
			}
				// obj could of some other type. This will be the case when sorting is by tag and the tags are of unique type.
			else
			{
				if(parent != null && parent.SortType == TreeNodeAdvSortType.Tag)
				{
					return this.NodeTagCompare(obj);
				}
			}
			return 0;
		}
		private int NodeTextCompare(string compareText)
		{
			return culture.CompareInfo.Compare(NodeStyle.Text,compareText,parent != null ? parent.NodeStyle.CompareOptions : CompareOptions.None);
		}
		private int NodeTagCompare(object compareTag)
		{
			if(this.Tag !=null)
			{
				if(compareTag!=null)
				{
					if(this.Tag is IComparable)
					{
						if( compareTag is IComparable)
						{
							return ((IComparable)this.Tag).CompareTo(compareTag);
						}
						else
						{
							return 1;
						}
					}
					else
					{
						return -1;
					}
				}
				else
				{
					return 1;
				}
			}
			else
			{
				if(compareTag!=null) return -1;
				else return 0;
			}
		}

		/// <overloaded>
		/// Sorts the tree nodes.
		/// </overloaded>
		/// <summary>
		/// Sorts the tree nodes with the current 
		/// <see cref="TreeNodeAdvStyleInfo.SortOrder"/> and <see cref="TreeNodeAdvStyleInfo.SortType"/>.
		/// </summary>
		public void Sort()
		{
			if (this.TreeView.SortWithChildNodes && this.HasChildren)
			{
				nodes.Sort(NodeStyle.SortOrder);
				foreach (TreeNodeAdv tna in this.Nodes)
				{
					tna.SortOrder = this.SortOrder;
					tna.Sort();
				}
			}
			else if (!this.TreeView.SortWithChildNodes && this.HasChildren)
				nodes.Sort(NodeStyle.SortOrder);

		}
		/// <summary>
		/// Sorts the tree nodes with the specified sort type and the current
		/// <see cref="NodeStyle.SortOrder"/>.
		/// </summary>
		/// <param name="sortType">One of the <see cref="TreeNodeAdvSortType"/> value.</param>
		/// <remarks>This will also set the value in the <see cref="NodeStyle.SortOrder"/> to the
		/// specified sort type.
		/// </remarks>
		public void Sort(TreeNodeAdvSortType sortType)
		{
			if (this.TreeView.SortWithChildNodes && this.HasChildren)
			{
				this.nodeData.SortType = sortType;
				Sort();
				foreach (TreeNodeAdv tna in this.Nodes)
				{
					tna.Sort(sortType);
				}
			}
			else if (!this.TreeView.SortWithChildNodes && this.HasChildren)
			{
				this.nodeData.SortType = sortType;
				Sort();
			}
		}

		#endregion

		#region Mouse Hit Test

		internal TreeNodeAdvAccessibleObject AccesibleObject
		{
			get
			{
                if (this.acsoNode == null && this.parent != null && this.parent.Expanded)
					this.acsoNode = new TreeNodeAdvAccessibleObject(this);

				return this.acsoNode;
			}
		}
		internal Rectangle MouseInControl(Point pt)
		{
			if(plusMinus.Visible && plusMinus.Bounds.Contains(pt))
			{
				return plusMinus.Bounds;
			}
			if(checkBox.Visible && checkBox.Bounds.Contains(pt))
			{
				return checkBox.Bounds;
			}
			if(optionButton.Visible && optionButton.Bounds.Contains(pt))
			{
				return optionButton.Bounds;
			}
			return Rectangle.Empty;
		}

        internal bool ProcessPlusMinusColor(TreeNodeAdv nodeatpoint, Point pt)
        {
            if (!nodeatpoint.Enabled)
                return false;
            if (nodeatpoint.plusMinus.Visible)
            {
                Rectangle pmBounds = nodeatpoint.plusMinus.Bounds;
				// Provide some leeway around the bounds.
				pmBounds.Inflate(4, 3);
                if (pmBounds.Contains(pt))
                {
                    PlusMinusArrowColor = this.TreeView.MetroColor;
                    return true;
                }
            }
            PlusMinusArrowColor = Color.Black;
            return false;
        }

		internal bool ProcessMouseDown(Point pt)
		{
			if(!this.Enabled)
				return false;

			if(plusMinus.Visible)
			{
				Rectangle pmBounds = plusMinus.Bounds;
				// Provide some leeway around the bounds.
				pmBounds.Inflate(4, 3);
				if(pmBounds.Contains(pt))
				{
                    if (!this.TreeView.RecalculateExpansion)
                    {
                        foreach (TreeNodeAdv node in Nodes)
                            node.RecalculateDimensions();
                    }
					if( !this.ShowPlusOnExpand || !this.TreeView.LoadOnDemand )
					{
						this.Expanded = !this.Expanded;
					}
					else
					{
						if( !this.Expanded )
						{
							this.Expanded = true;
						}
						else
						{
							this.TreeView.ExpandedChanging(this, true);
						}
					}
					return true;
				}
			}
			if(checkBox.Visible && this.EnabledButtons && checkBox.Bounds.Contains(pt))
			{
				bool differentSelectionBaseNode = this.TreeView != null && this.TreeView.SelectionBaseNode != this;
				this.ToggleCheckState((Control.ModifierKeys & Keys.Shift) > 0 && differentSelectionBaseNode);
				return true;
			}

            Rectangle optionRect = this.optionButton.Bounds;
            if (optionButton.Visible && this.EnabledButtons && this.TreeView.EnableTouchMode)
            {
                optionRect.X += 10;
                if (optionRect.Contains(pt) || optionRect.X > pt.X)
                {
                    if (!optioned && parent != null)
                    {
                        TreeNodeAdv optionedChild = parent.GetOptionedChild();

                        if (optionedChild != null)
                        {
                            optionedChild.Optioned = false;
                        }

                        Optioned = true;
                    }
                    return true;
                }
            }
			if(optionButton.Visible && this.EnabledButtons && optionButton.Bounds.Contains(pt) && !this.TreeView.EnableTouchMode)
			{
				if(!optioned && parent!=null)
				{
					TreeNodeAdv optionedChild = parent.GetOptionedChild();

					if( optionedChild != null )
					{
						optionedChild.Optioned = false;
					}

					Optioned = true;
				}
				return true;
			}
			return false;
		}

		internal CheckState GetToggledState(CheckState current)
		{
			CheckState newState = CheckState.Checked;

			// SIngle node toggle.
			if(current == CheckState.Checked)
			{
				newState = CheckState.Unchecked;
			}
			else if(current == CheckState.Unchecked && this.InteractiveCheckBox

				&& this.partialCheckedState != null)
			{
				newState = CheckState.Indeterminate;
			}
			else
				newState = CheckState.Checked;


			return newState;
		}
        
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void ToggleCheckState(bool multiNodeToggle)
		{
			TreeViewAdv tree = this.TreeView;

			if(multiNodeToggle == false || tree.SelectionBaseNode == null)
			{
				this.CheckState = this.GetToggledState(this.CheckState);
			}
			else if(this.TreeView != null)
			{
				TreeNodeAdv selectionBaseNode = tree.SelectionBaseNode;

				CheckState newCheckState = selectionBaseNode.CheckState;

				bool up = selectionBaseNode.Bounds.Y>this.Bounds.Y;

				ArrayList newNodes = new ArrayList();
				TreeNodeAdv tna = selectionBaseNode;
				// Parse through all the nodes and set the new check state.
				// This will trigger multiple AfterInteractiveChecks events in the tree.
				while(tna!=null && tna!=this)
				{
					if(tna.Enabled)
					{
						newNodes.Add(tna);
						// Set the CheckState only if ShowCheckBox is on.
						if(tna.ShowCheckBox)
							tna.CheckState = newCheckState;
					}
					tna = (up?tna.PrevVisibleNode:tna.NextVisibleNode);
				}

				// Finally toggle my check state as well.
				this.CheckState = newCheckState;
			}
		}
		#endregion

		#region Moving Nodes
		/// <overloaded>
		/// Moves this node to a different collection.
		/// </overloaded>
		/// <summary>
		/// Moves the node to the end of the specified collection.
		/// </summary>
		/// <param name="newNodesCollection">
		/// A <see cref="TreeNodeAdvCollection"/> to which this node will move.
		/// </param>
		/// <remarks>
		/// <para>A node can be positioned to any other TreeNodeAdvCollection in the same tree or in a different TreeViewAdv control.</para>
		/// <para>Note: All of the descendants of the node will move along with it.</para>
		/// <para>Note: A node cannot be moved to one of it's own Descendants.</para>
		/// </remarks>
		public void Move(TreeNodeAdvCollection newNodesCollection)
		{
			this.Move(newNodesCollection, -1);
		}
		/// <summary>
		/// Moves the node to a new collection at the specified index.
		/// </summary>
		/// <param name="newNodesCollection">
		/// A <see cref="TreeNodeAdvCollection"/> to which this node will move.</param>
		/// <param name="index">
		/// The new index of the node in the new collection.
		/// </param>
		/// <remarks>
		/// Moving a node by index will ensure that the node ends up at the index 
		/// specified. Note that the node will first be removed from its existing
		/// collection and then added to the specified collection at the specified index.
		/// If the source collection and destination collection are the same, make sure
		/// to take into account the above semantics while specifying the index, or
		/// use the Move override that lets you specify a relative position.
		/// </remarks>
		public void Move(TreeNodeAdvCollection newNodesCollection, int index)
		{
			HistoryManager historyManager = this.HistoryManager;

			if( historyManager != null )
			{
				historyManager.BeginBlock();
			}

			if(newNodesCollection == null)
				return;

			if(this.Parent != null)
			{
				// First remove from the parent's collection
				this.Parent.Nodes.Remove(this);
			}
			// Add to the newNodesCollection:
			if(index == -1)
				newNodesCollection.Add(this);
			else
				newNodesCollection.Insert(index, this);

			if( historyManager != null )
			{
				historyManager.CloseBlock();
			}
		}
		/// <summary>
		/// Moves the node to a specified position in relation to the specified 
		/// "relative node".
		/// </summary>
		/// <param name="relativeNode">The "relative node" that determines this node's new position.</param>
		/// <param name="nodePosition">Specifies where this node will be moved in relation to the "relative node". </param>
		/// <remarks>
		/// <para>A node can be positioned relative to any other node in the same tree or even a different TreeViewAdv Control.</para>
		/// </remarks>
		public void Move(TreeNodeAdv relativeNode, NodePositions nodePosition)
		{
			HistoryManager historyManager = this.HistoryManager;

			if( historyManager != null )
			{
				historyManager.BeginBlock();
			}

			if(relativeNode == null ||
				(
				(nodePosition == NodePositions.Next || nodePosition == NodePositions.Previous)
				&& relativeNode.Parent == null
				)
				)
				return;

			if(this.Parent != null)
			{
				// First remove from the parent's collection
				this.Parent.Nodes.Remove(this);
			}
			switch(nodePosition)
			{
				case NodePositions.First:
					this.Move(relativeNode.Nodes, 0);
					break;
				case NodePositions.Last:
					this.Move(relativeNode.Nodes, -1);
					break;
				case NodePositions.Next:
					this.Move(relativeNode.Parent.Nodes,
						relativeNode.Parent.Nodes.IndexOf(relativeNode) + 1);
					break;
				case NodePositions.Previous:
					this.Move(relativeNode.Parent.Nodes,
						relativeNode.Parent.Nodes.IndexOf(relativeNode));
					break;
			}

			if( historyManager != null )
			{
				historyManager.CloseBlock();
			}
		}
		public void Move( TreeNodeAdvCollection col, NodePositions nodePosition  )
		{
			HistoryManager historyManager = this.HistoryManager;

			if( historyManager != null )
			{
				historyManager.BeginBlock();
			}

			if( col == null )
				throw new ArgumentNullException( "col" );	

			if( this.Parent != null )
			{
				// First remove from the parent's collection
				this.Parent.Nodes.Remove( this );
			}

			switch( nodePosition )
			{
				case NodePositions.Previous:
				case NodePositions.First:
					Move( col, 0 );
					break;

				case NodePositions.Next:
				case NodePositions.Last:
					Move( col, -1 );
					break;
			}

			if( historyManager != null )
			{
				historyManager.CloseBlock();
			}
		}
		#endregion Moving Nodes
		#region Nodes Changed
		internal void AddedNode(TreeNodeAdv node)
		{
			if(node.Parent != this && node.Parent != null)
			{
				// Remove this from the current parent.
				node.Parent.Nodes.Remove(node);
			}

			// Simply call OptionedChild, it will make sure that an optioned child exists.
			//			TreeNodeAdv optionedChild = this.OptionedChild;
			// We don't need this anymore, the above call will take care of this.
			//			if(this.OptionedChild == null) 
			//				node.Optioned = true;

			bool oldVisibility = node.Visible;
			node.Visible = this.Visible && this.Expanded;
			bool childMadeVisible = node.Visible && oldVisibility == false;

			// Do the parenting after setting the Visibility, so that
			// the AdjustVisibleNodeCount method doesn't get triggered on myself.
			node.parent = this;
            
			if(!this.IsRoot)
			{
                bool skipEventHandler = false;
                if (this.TreeView != null && this.TreeView.inNodeRefresh)
                {
                    skipEventHandler = true;
                }
                if(!skipEventHandler)
				    node.CheckStateChanged += new EventHandler(childCheckStateChanged);
				this.UpdateInteractiveCheckState();
			}
			TreeViewAdv tree = TreeView;

			if(tree!=null)
			{
				// Recursively call this method so that bounds get updated.
                if (tree.RecalculateExpansion)
                {
					foreach(TreeNodeAdv childNode in node.Nodes)
						node.AddedNode(childNode);
                }

				if(tree.ShouldPrepareUpdate(false))
				{
					node.RecalculateDimensions();
					if(node.Right > this.maxX)
					{
						this.MaxX = node.Right;
					}
					if(this.Nodes.Count == 1)
						// When the first child is added certain widths might change.
						this.RecalculateDimensions();
				}
				//Updating because of an issue when adding a node and begining editing it in the same method the tree not invalidating the node's location wasn't set and the editor's position was 0,0.
				//				tree.Invalidate();
				//				tree.Update();
			}
			if(childMadeVisible)
			{
				this.AdjustVisibleNodeCount(node.VisibleNodeCount);
				// Already called this:
				//node.Visible = true;
			}
			this.UpdatePlusMinusVisibility();
			node.UpdateAllPlusMinusVisibility();
		}
		internal void RemovedNodes(ArrayList nodes)
		{
			foreach(TreeNodeAdv node in nodes)
			{
				RemovedNode(node);
			}
			MakeDirty();
		}
		//		internal void Clearing()
		//		{
		//			if(this.TreeView !=null)
		//			{
		//				TreeView.Clearing(this);
		//			}
		//		}

		internal void RemovedNode(TreeNodeAdv node)
		{
			if(Visible & Expanded)
				this.AdjustVisibleNodeCount(node.VisibleNodeCount);

			if(this.TreeView !=null)
			{
				TreeView.RemovedNode(node);
			}
			// Necessary to make the parent null and hide it b'cos
			// the VisibileNodeCount needs to be udpated appropriately
			// so that the logic will work when adding the node back to the tree.
			node.parent = null;
			node.Visible = false;
			// This will trigger recalculating the maxX.
			this.childMaxXChanged(0);

			// Caching Partial-Checked-State related
			if(this.InteractiveCheckBox)
			{
				if(this.partialCheckedState != null)
				{
					this.partialCheckedState.Remove(node);
					if(this.partialCheckedState.Count == 0)
						this.partialCheckedState = null;
				}
				this.UpdateInteractiveCheckState();
			}
		}
        private void RemoveCheckedNode(TreeNodeAdv node, CheckedNodesColection nodes)
        {
            foreach (TreeNodeAdv subNode in node.Nodes)
            {
                RemoveCheckedNode(subNode, nodes);
            }
            nodes.Remove(node);
        }
		private void Nodes_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if( e.Action != CollectionChangeAction.Refresh )
			{
				this.CustomControlCollectionChanging( e.Element as TreeNodeAdv, e.Action );
			}

			if(TreeView!=null) TreeView.NodesChanging();

			if(e.Action == CollectionChangeAction.Remove)
			{
				TreeNodeAdv node = ((TreeNodeAdv) e.Element);
				TreeViewAdv tree = this.TreeView;
				if(tree != null && tree.SelectedNode != null
					// If the selected node is the removed node or the selected node is a child of the remvoed node
					&& (tree.SelectedNode == node || tree.SelectedNode.IsParent(node)))
					this.AdjustSelectedNode(tree.curSelectedNodeIndex);

                // remove nodes from the checked list
                if (TreeView != null)
                {
                    this.RemoveCheckedNode(node, TreeView.CheckedNodes);
                }

				this.RemovedNode(node);
			}
			else if(e.Action == CollectionChangeAction.Add)
			{
				TreeNodeAdv node = e.Element as TreeNodeAdv;

		        if( null != node )
		        {
					this.UpdateSelectedNodeIndexCache( );
					this.AddedNode( node );
					this.MakeDirty( );
					
					if( TreeView != null )
					{
						// if node is checked, add it to checked nodes List in parent TreeView
						TreeView.CheckedNodes.ResolveNode( node );
						
						HistoryManager historyManager = this.HistoryManager;
						
						if( historyManager != null && !node.IsUndoRedoPerforming )
						{
							TreeViewCommand cmd = new TreeViewCommand( node, Action.Add );
							historyManager.Do( cmd );
						}
					}
		        }
			}
				// This will be called while sorting, clearing and moving items in the list.
			else if(e.Action == CollectionChangeAction.Refresh)
			{
                this.UpdateSelectedNodeIndexCache();

				TreeViewAdv tva = this.TreeView;

				if( tva != null )
				{
                    tva.inNodeRefresh = true;

					foreach( TreeNodeAdv node in this.Nodes )
					{
						this.AddedNode(node);

						// if node is checked, add it to checked nodes List in parent TreeView
						tva.CheckedNodes.ResolveNode( node );

						HistoryManager historyManager = this.HistoryManager;

						if( historyManager != null && !node.IsUndoRedoPerforming )
						{
							TreeViewCommand cmd = new TreeViewCommand( node, Action.Add );
							historyManager.Do( cmd );
						}
					}

                    tva.inNodeRefresh = false;
				}

				this.MakeDirty();
			}
			// Calling OptionedChild will take care of setting a child as Optioned.
			//			TreeNodeAdv optionedChild = this.OptionedChild;
			// The above call will take care of setting an optioned child.
			//			if(this.OptionedChild == null && this.HasChildren)
			//				nodes[0].Optioned = true;

			this.UpdatePlusMinusVisibility();

			// Reset only my visible node count setting (not the children's)
			this.ResetVisibleNodeCount(false);

			MakeDirty();
			if(TreeView!=null) TreeView.NodesChanged();
		}
		private bool IsRoot
		{
			get
			{
				if(this._treeView != null && this._treeView.Root == this)
					return true;
				else
					return false;
			}
		}
		private void AdjustSelectedNode(int prevIndex)
		{
			TreeNodeAdv newSelectedNode = null;
			// No children so make myself the selected node.
			if(this.Nodes.Count == 0 && !this.IsRoot)
			{
				if(this.Enabled)
				{
					newSelectedNode = this;
				}
			}

			if(this.Nodes.Count > 0)
			{
				if(newSelectedNode == null)
				{
					// There were children so use the prev index to determine the new selection.
					if(prevIndex >= this.Nodes.Count)
						prevIndex = this.Nodes.Count - 1;

					if(prevIndex >= 0)
					{
						TreeNodeAdv newNode = this.Nodes[prevIndex];
						while((newNode.Parent != this || !newNode.Enabled) && ++prevIndex < this.Nodes.Count)
						{
							newNode = this.Nodes[prevIndex];
						}

						if(newNode.Parent == this && newNode.Enabled)
							newSelectedNode = newNode;
					}
				}
			}
			if(newSelectedNode == null)
			{
				// Still can't find a selectable node, so parse down and up.
				TreeNodeAdv selectableNode = this.NextSelectableNode;
				if(selectableNode == null)
					selectableNode = this.PrevSelectableNode;
				newSelectedNode = selectableNode;
			}
			TreeViewAdv tree = this.TreeView;
			if(tree != null)
			{
				if(tree.SetSelectedNode(newSelectedNode, tree.SelectedNodes, TreeViewAdvAction.Unknown))
				{
					tree.ActiveNode = newSelectedNode;
					tree.SetSelectionBaseNode( newSelectedNode );
				}
				else
				{
					tree.SetSelectedNode(null, tree.SelectedNodes, TreeViewAdvAction.Unknown, false, true);
					tree.ActiveNode = null;
				}
			}
		}
		private void UpdateSelectedNodeIndexCache()
		{
			TreeViewAdv tree = this.TreeView;
			if(tree != null && tree.SelectedNode != null &&
				tree.SelectedNode.Parent == this)
			{
				if(this.Nodes.IndexOf(tree.SelectedNode) == -1)
				{
					tree.SetSelectedNode(null, tree.SelectedNodes, TreeViewAdvAction.Unknown, false, true);
					tree.ActiveNode = null;
				}
				else
					tree.curSelectedNodeIndex = this.Nodes.IndexOf(tree.SelectedNode);
			}
		}
		#endregion
		/// <summary>
		/// Indicates whether the current node is a direct or indirect child of the specified node.
		/// </summary>
        /// <param name="targetNode">The node that is to be tested for ancestry.</param>
		/// <returns>True if the targetNode is a parent of this node; False otherwise.</returns>
		public bool IsParent(TreeNodeAdv targetNode)
		{
			if(targetNode == null)
				return false;

			TreeNodeAdv node = this;
			while(node != null && node.Parent != targetNode)
				node = node.Parent;

			if(node != null && node.Parent == targetNode)
				return true;
			else
				return false;
		}
		private void childCheckStateChanged(object sender,EventArgs e)
		{
			this.UpdateInteractiveCheckState();
		}
		private void UpdateInteractiveCheckState()
		{
			if(!NodeStyle.InteractiveCheckBox
				|| TreeNodeAdv.checkStateChangingSourceNode == this
				|| this.IsParent(TreeNodeAdv.checkStateChangingSourceNode)
				) return;

			int firstChildWithCheckBox = -1;

			for(int i = 0; i < nodes.Count; i++)
			{
				if(nodes[i].ShowCheckBox)
				{
					firstChildWithCheckBox = i;
					break;
				}
			}

			if(firstChildWithCheckBox == -1)
				return;

			CheckState first = nodes[firstChildWithCheckBox].CheckState;
			bool same = true;

			for(int i=firstChildWithCheckBox + 1;i<nodes.Count;i++)
			{
				if(nodes[i].ShowCheckBox &&
					first!=nodes[i].CheckState) same = false;
			}

			// Make sure to clear the cached checked state before setting this.
			this.ClearCachedPartialCheckedState();

			// Changes wrt Caching Partial-Checked-State related : Start
			if(same) 
			{
				this.CheckState = first;
				// We just went from indeterminate to determinate state, so no need to cache;
				this.ClearCachedPartialCheckedState();
			}
			else
			{
				this.CheckState = CheckState.Indeterminate;
			}
			// Changes wrt Caching Partial-Checked-State related : End
		}

		private void InvalidateTreeView()
		{
			if(this.TreeView!=null)
			{
				TreeView.Invalidate();
			}
		}

		object ICloneable.Clone()
		{
			return this.MemberwiseClone();
		}

        public Control CreateControlInstance(string controlName, string namespaceName)
        {
            try
            {
                Control ctrl;
                switch (controlName)
                {
                    case "Label":
                        ctrl = new Label();
                        break;
                    case "TextBox":
                        ctrl = new TextBox();
                        break;
                    case "PictureBox":
                        ctrl = new PictureBox();
                        break;
                    case "ListView":
                        ctrl = new ListView();
                        break;
                    case "ComboBox":
                        ctrl = new ComboBox();
                        break;
                    case "Button":
                        ctrl = new Button();
                        break;
                    case "CheckBox":
                        ctrl = new CheckBox();
                        break;
                    case "MonthCalender":
                        ctrl = new MonthCalendar();
                        break;
                    case "DateTimePicker":
                        ctrl = new DateTimePicker();
                        break;
                    case "TreeViewAdv":
                        ctrl = new TreeViewAdv();
                        break;
                    default:
                        Assembly controlAsm = Assembly.Load(namespaceName);
                        Type controlType = controlAsm.GetType(namespaceName + "." + controlName);
                        ctrl = (Control)Activator.CreateInstance(controlType);
                        break;

                }
                return ctrl;

            }
            catch
            {
                return new Control();
            }
        }

        public void SetControlProperties(Control ctrl, Hashtable propertyList)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(ctrl);

            foreach (PropertyDescriptor myProperty in properties)
            {
                if (propertyList.Contains(myProperty.Name))
                {
                    Object obj = propertyList[myProperty.Name];
                    try
                    {
                        myProperty.SetValue(ctrl, obj);
                    }
                    catch
                    { }

                }

            }

        }

        public Control CloneCustomControl(Control ctrl)
        {

            Control newControl = CreateControlInstance(ctrl.GetType().Name, ctrl.GetType().Namespace);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(ctrl);

            Hashtable propertyList = new Hashtable();
            foreach (PropertyDescriptor myProperty in properties)
            {
                try
                {
                    if (myProperty.PropertyType.IsSerializable)
                        propertyList.Add(myProperty.Name, myProperty.GetValue(ctrl));
                }
                catch
                {
                }

            }
            SetControlProperties(newControl, propertyList);

            return newControl;
        }

		/// <summary>
		/// Creates a clone of this node.
		/// </summary>
		/// <returns>The clone of the node.</returns>
		public TreeNodeAdv Clone()
		{
			TreeNodeAdv newNode = new TreeNodeAdv(this.NodeStyle.Text);

			newNode.LeftStateImagePadding = this.LeftStateImagePadding;
			newNode.RightStateImagePadding = this.RightStateImagePadding;
			newNode.LeftImagePadding = this.LeftImagePadding;
			newNode.RightImagePadding = this.RightImagePadding;

			newNode.ExpandImageIndex = this.ExpandImageIndex;
			newNode.CollapseImageIndex = this.CollapseImageIndex;

			newNode.NodeStyle.ModifyStyle(NodeStyle, StyleModifyType.Copy);
			newNode.ChildStyle.ModifyStyle(ChildStyle, StyleModifyType.Copy);
			newNode.iBounds = Bounds;
			
			newNode.SetPrimitives( ( TreeNodePrimitivesCollection )this.Primitives.Clone() );

            if (this.CustomControl != null)
            {
                //Control control = newNode.CloneCustomControl(this.CustomControl);
                if (this.TreeView != null)
                {
                    //control.CreateControl();
                    this.TreeView.CustomControlsImage.Add(this.CustomControl, this.CustomControl.Location);
                }
            }
			
			newNode.Expanded = Expanded;
			newNode.ShowPlusOnExpand = ShowPlusOnExpand;
			newNode.Nodes.Clear();
			for(int i=0;i<Nodes.Count;i++)
			{
				newNode.Nodes.Add(Nodes[i].Clone());
			}
			newNode.Visible = Visible;
			if(this.Tag is ICloneable)
			{
				newNode.Tag = ((ICloneable)this.Tag).Clone();
			}

			return newNode;
		}

		internal void SetCustomControlMember( Control control )
		{
			m_customControl = control;
		}

		private void ExpandParentSelf()
		{
			//Fixed defect that the BringIntoView method does not expand the tree when its already expanded but it collapsed by its parent(Forum: 42052).
			if (this.parent != null && (!this.parent.Expanded || !this.parent.visible))
				this.parent.ExpandParentSelf();

			this.Expand();
		}
		/// <summary>
		/// Expands parent nodes to make this node visible and also scrolls
		/// the tree such that this node is brought into view.
		/// </summary>
		public void BringIntoView()
		{
			if(!this.Visible && this.parent != null)
				this.parent.ExpandParentSelf();
	
			if(!this.Visible)
				return;

			TreeViewAdv tree = this.TreeView;
			if(tree != null)
			{
				// Update the tree so that any unhidden node's bounds will be set.
				tree.Update();
				tree.EnsureVisible(this);
			}
			this.TreeView.IsBroughtIntoView = true;
		}

		/// <summary>
		/// Expands the node.
		/// </summary>
		public void Expand()
        {
            Expanded = true;
            if ( this.TreeView!=null && !this.TreeView.RecalculateExpansion)
            {
                foreach (TreeNodeAdv node in Nodes)
                {
                    node.RecalculateDimensions();
                    node.InvalidateTreeView();
                }
            }
        }
		/// <summary>
		/// Expands this node and all the subnodes.
		/// </summary>
		public void ExpandAll()
		{
			Expanded = true;
			for(int i=0;i<Nodes.Count;i++)
			{
				Nodes[i].ExpandAll();
                if (!this.TreeView.RecalculateExpansion)
                    Nodes[i].RecalculateDimensions();
			}
		}
		/// <summary>
		/// Collapses this node and all it's children.
		/// </summary>
		public void CollapseAll()
		{
			for(int i=0;i<Nodes.Count;i++)
			{
				Nodes[i].CollapseAll();
			}
			this.Expanded = false;
		}
		private void SetChildrenVisible(bool value)
		{
			for(int i=0;i<nodes.Count;i++)
			{
				nodes[i].Visible = value;
			}
		}

		#region Drawing
		//		const int lineWidth = 20;
		private const int spc = 3;
		private const int c_nDisplayNodeTextWidthPadding = 4;

		internal Color GetForeColor(bool selected, bool hotTracked)
		{
			TreeViewAdv tree = this.TreeView;
			if(tree != null)
			{
				if(selected)
				{
					if(tree.Focused)
						return tree.SelectedNodeForeColor;
					else if(!tree.HideSelection)
						return tree.InactiveSelectedNodeForeColor;
				}
				if(!tree.Enabled || !this.Enabled)
					return SystemColors.GrayText;
				else
					return hotTracked?SystemColors.HotTrack:NodeStyle.TextColor;
			}
			return NodeStyle.TextColor;
		}

		internal void Draw(ThemedControlDrawing treeTD,ThemedControlDrawing buttonTD,Pen linePen,Point mousePos,bool mouseDown, bool drawFocusRect,  TreeNodeAdvPaintEventArgs e)
		{
			TreeViewAdv tree = TreeView;

			bool bIsMirrored = tree.GetIsMirrored();

			//			int parentIndent = tree.Indent;
                if (this.parent.showLine)
                    DrawHorizontalLine(tree, e, bIsMirrored, linePen);
      
			DrawLeftImageList( tree, e, bIsMirrored );
			DrawStateImageList( tree, e, bIsMirrored );

			DrawText( tree, e );

			if( e.Active && drawFocusRect )
			{	
				DrawFocusRect( e.Graphics, tree );
			}

			DrawRightImageList( tree, e, bIsMirrored );
			DrawControls(e,treeTD,buttonTD,mousePos,mouseDown);
		}
	
		private void DrawHorizontalLine( TreeViewAdv tvaTree, TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored, Pen linePen )
		{
			// Draw the Horizontal line, if necessary
			if(this.Level != 1 || tvaTree.ShowRootLines)
			{
				int left = 0;
				int right = 0;

				if (bIsMirrored)
				{
					int nOffset = this.bounds.Right - nodeXRel;
					right = nOffset - plusMinus.Width/2;
					left = nOffset - this.lineRightRel;
				}
				else
				{
					int nOffset = this.bounds.X + nodeXRel;
					left = nOffset + plusMinus.Width/2;	
					right = nOffset + this.lineRightRel;
				}				

				if(parent!=null && tvaTree.ShowLines)
				{
					if(HasChildren && this.nodeData.ShowPlusMinus)
					{
						if (bIsMirrored)
						{
							right -= plusMinus.Width/2;
						}
						else
						{
							left += plusMinus.Width/2;
						}
					}

					int nLineY = bounds.Y+NodeStyle.Height/2;
					eaEventArgs.Graphics.DrawLine( linePen, left, nLineY, right, nLineY );
				}
			}
		}
		
		private void DrawLeftImageList( TreeViewAdv tvaTree, TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored )
		{
			ImageList leftImageList = tvaTree.LeftImageList;
			if (leftImageList != null && !eaEventArgs.HandledLeftImageList)
			{
				int x = 0;
				if( leftImageListXRel != int.MinValue )
				{
					int nInc = nodeXRel + this.leftImageListXRel;

					if (bIsMirrored)
					{
						x = this.bounds.Right - nInc;
					}
					else
					{
						x = this.bounds.X + nInc;
					}

					int nImgWidth = leftImageList.ImageSize.Width;
					int nY = bounds.Y+(NodeStyle.Height-leftImageList.ImageSize.Height)/2;

					int[] indexes = NodeStyle.LeftImageIndices;					
					for(int i=0;i<indexes.Length;i++)
					{
						int index = indexes[i];
						if(index>=0 && index < leftImageList.Images.Count)
						{
							if (bIsMirrored)
							{
								x -= nImgWidth;
								x -= LeftImagePadding;
							}

							eaEventArgs.Graphics.DrawImage( leftImageList.Images[ index ], x, nY );
						
							if (!bIsMirrored)
							{
								x += nImgWidth;
								x += LeftImagePadding;
							}
						}
					}
				}
			}
		}

		private void DrawStateImageList( TreeViewAdv tvaTree, TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored )
		{
			ImageList stateImgList = tvaTree.StateImageList;
			if (stateImgList != null && !eaEventArgs.HandledStateImageList && stateImageListXRel != int.MinValue)
			{
				int imgIndex = 0;
				if(!HasChildren && !(tvaTree.LoadOnDemand && !this.expandedOnce)) imgIndex = this.NodeStyle.NoChildrenImgIndex;
				else
				{
					if(Expanded) imgIndex = this.NodeStyle.OpenImgIndex;
					else	imgIndex = this.NodeStyle.ClosedImgIndex;
				}

				if(imgIndex >= 0 && imgIndex< stateImgList.Images.Count)
				{
					int x = 0;
					int nInc = this.stateImageListXRel + nodeXRel;

					if (bIsMirrored)
					{
						x = this.bounds.Right - nInc - stateImgList.ImageSize.Width;
						x -= LeftStateImagePadding;
					}
					else
					{
						x = this.bounds.X + nInc;
						x += LeftStateImagePadding;
					}
					
					eaEventArgs.Graphics.DrawImage( stateImgList.Images[ imgIndex ], x, bounds.Y + (NodeStyle.Height-stateImgList.ImageSize.Height)/2 );
				}
			}
		}

		/// <summary>
		/// Calculates width of string which must be drawn with specified font.
		/// </summary>
		/// <param name="g">Context device for drawing.</param>
		/// <param name="f">Specified font.</param>
		/// <returns>Width of specified string in pixel.</returns>
		private Size GetNodeTextSize( Graphics g, Font f )
		{
			Size size = Size.Empty;

			if( this.Text != null && this.Text.Length > 0 )
			{
				size = this.TreeView.MeasureDisplayStringSize( g, this.Text, f );
			}
			
			size.Width += c_nDisplayNodeTextWidthPadding;

			return size;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tvaTree"></param>
		/// <param name="eaEventArgs"></param>
		private void DrawText( TreeViewAdv tvaTree, TreeNodeAdvPaintEventArgs eaEventArgs )
		{	
			Graphics g = eaEventArgs.Graphics;

            SizeF size = g.MeasureString(this.Text, this.Font);
			if( textLocationXRel != int.MinValue )
			{
				if( tvaTree.Printing )
				{
					Point textLocation = this.PrintTextBounds.Location;
					int offsetX = PrintTextBounds.Width - TextBounds.Width;
					if( offsetX > 0 && GetIsMirrored() )
					{
						textLocation.Offset( -offsetX, 0 );
					}
                    using(Brush brush = new SolidBrush( eaEventArgs.ForeColor ))
                        g.DrawString(this.Text, this.Font, brush, textLocation);
				}
				else
				{
					if(!eaEventArgs.HandledText && (!tvaTree.IsEditing || tvaTree.ActiveNode != this))
					{					

						// Font which will be used for drawing.
						Font f = eaEventArgs.HotTracked ? HotFont : NodeStyle.Font;

						IntPtr hdc = g.GetHdc();
						IntPtr hFont = f.ToHfont();
                        try
                        {
                            IntPtr prevFont = NativeMethods.SelectObject(hdc, hFont);

                            Color invertedColor = Color.FromArgb(0, eaEventArgs.ForeColor.B, eaEventArgs.ForeColor.G, eaEventArgs.ForeColor.R);
                            NativeMethods.SetTextColor(hdc, invertedColor.ToArgb() & 0xFFFFFF);
                            NativeMethods.SetBkMode(hdc, 1);	// TRANSPARENT
                            Rectangle textrect = this.TextBounds;
                            if (this.TreeView.EnableTouchMode && (this.ShowOptionButton || this.ShowCheckBox))
                            {
                                textrect.X = this.TextBounds.X + 20;
                            }
                            NativeMethods.RECT rect = new NativeMethods.RECT(textrect);
                            rect.left += c_nDisplayNodeTextWidthPadding / 2;
                            int nFlags = c_nDrawTextFlags;

                            if (this.MultiLine)
                            {
                                string search = "\n";
                                string Input = this.Text;
                                int length = search.Length;
                                int TimestoIterate = Input.Length - length;
                                int count = 0;
                                for (int index = 0; index < TimestoIterate; index++)
                                {
                                    string theSubString = Input.Substring(index, length);
                                    if (theSubString.ToLower() == search.ToLower())
                                    {
                                        count++;
                                    }
                                }               
                                if (count > 0)
                                {                                  
                                        if (this.Height < (int)size.Height)
                                        {
                                            this.Height = (int)size.Height + (count) / 2 + 2;
                                        }
                                        else
                                        {                                          
                                            rect.top += ((int)this.Height - (int)size.Height) / 2 - (count) / 2 + 1 ;  
                                        }
                                   
                                    nFlags -= DrawTextFormats.DT_SINGLELINE;
                                }                              
                            }
                            if (GetIsMirrored())
                            {
                                nFlags |= DrawTextFormats.DT_RTLREADING;
                            }
                            
                            if(this.Text != null)
                            NativeMethods.DrawText(hdc, this.Text, this.Text.Length, ref rect, nFlags);

                            prevFont = NativeMethods.SelectObject(hdc, prevFont);
                        }
                        finally
                        {
                            NativeMethods.DeleteObject(hFont);
                            g.ReleaseHdc(hdc);
                        }
					}
				}
			}
		}

		private void DrawFocusRect( TreeViewAdv tvaTree, TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored )
		{
			if (eaEventArgs.Active && tvaTree.Focused)
			{
				Rectangle rect;
				Graphics g = eaEventArgs.Graphics;
				if (eaEventArgs.FullRowSelect)
				{
					rect = bounds;
					
					rect.Inflate( bounds.X, 0 );
					rect.Width -= bounds.X;
				}
				else
				{
					rect = tvaTree.Printing ? this.PrintTextBounds : this.TextBounds;
				}
				
				ControlPaint.DrawFocusRectangle( g, rect, this.NodeStyle.TextColor, this.NodeStyle.Background.BackColor );
			}
		}
		/// <summary>
		/// Draws dotted border around selected node.
		/// This will be used to fast drawing when TreeCtrl loses focus.
		/// </summary>
		/// <param name="g">Device context needed for drawing.</param>
		/// <param name="tvaTree">Node's parent.</param>
		protected internal void DrawFocusRect( Graphics g, TreeViewAdv tvaTree )
		{
			if( g == null )
				throw new ArgumentNullException( "g" );

			if( tvaTree == null )
				throw new ArgumentNullException( "tvaTree" );

			Rectangle rect;

			if ( !tvaTree.Focused && !tvaTree.KeepDottedSelection ) return;

			if ( tvaTree.FullRowSelect )
			{
				rect = bounds;

				rect.Inflate( bounds.X, 0 );
				rect.Width -= bounds.X;
				if (tvaTree.GetIsMirrored())
					rect.Width += tvaTree.GutterSpace;
			}
			else
			{
				rect = tvaTree.Printing ? this.PrintTextBounds : this.TextBounds;
                if (this.TreeView.EnableTouchMode && (this.ShowOptionButton || this.ShowCheckBox))
                {
                    rect.X = this.TextBounds.X + 20;
                }
			}
			
			if( !this.IsEditing )
			{
				if( g.ClipBounds.IntersectsWith( rect ) )
				{
					ControlPaint.DrawFocusRectangle( g, rect,
						NodeStyle.TextColor, NodeStyle.Background.BackColor );
				}
			}
		}

		private void DrawRightImageList( TreeViewAdv tvaTree, TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored )
		{
			ImageList rightImgList = tvaTree.RightImageList;
			if (rightImgList!=null && !eaEventArgs.HandledRightImageList)
			{
				if( rightImageListXRel != int.MinValue )
				{
					int x = 0;
					int nInc = this.rightImageListXRel + nodeXRel;

					if (bIsMirrored)
					{
						x = this.bounds.Right - nInc;
					}
					else
					{
						x = this.bounds.X + nInc + RightImagePadding;
					}

					int nImgWidth = rightImgList.ImageSize.Width;
					int nY = bounds.Y + (NodeStyle.Height-rightImgList.ImageSize.Height)/2;

					int[] indexes = NodeStyle.RightImageIndices;
				
					if( tvaTree.Printing )
					{
						int offsetX = this.PrintTextBounds.Width - this.TextBounds.Width;
						if( offsetX > 0 )
						{
							x = bIsMirrored ? x - offsetX : x + offsetX;
						}
					}
					for(int i=0;i<indexes.Length;i++)
					{
						int index = indexes[i];
						if(index>=0 && index < rightImgList.Images.Count)
						{
							if (bIsMirrored)
							{
								x -= nImgWidth;
								x -= RightImagePadding;
							}
							
						
							eaEventArgs.Graphics.DrawImage( rightImgList.Images[ index ], x, nY );

							if (!bIsMirrored)
							{
								x += nImgWidth;
								x += RightImagePadding;
							}
						}
					}
				}
			}
		}

		internal bool ShouldDrawPlusMinus()
		{
			return plusMinus.Visible && ((parent!=null && parent.Expanded && HasChildren)||TreeView.LoadOnDemand && !this.expandedOnce);
		}

		private void DrawControls(TreeNodeAdvPaintEventArgs e, ThemedControlDrawing treeTD,ThemedControlDrawing buttonTD,Point pt,bool mouseDown)
		{
			// Draw the themed controls
			if(this.NodeStyle.ThemesEnabled && XPThemes.IsThemedOS&&  XPThemes.IsAppThemed && XPThemes.IsThemeActive)
			{
				//Draw the plus minus.
				if(!e.HandledPlusMinus && this.ShouldDrawPlusMinus())
				{
					int stateID = ( !Expanded || ( this.ShowPlusOnExpand && this.TreeView.LoadOnDemand ) ) ? 1 : 2;
					treeTD.DrawThemeBackground( e.Graphics, 2, stateID, this.plusMinus.Bounds );
				}
				//Draw the checkbox
				if(!e.HandledCheckBox && checkBox.Visible && parent!=null && parent.Expanded)
				{
					//Calculating the part needed
					int partNr = 1;
					switch(this.CheckState)
					{
						case CheckState.Unchecked: partNr = 1;break;
						case CheckState.Checked: partNr = 5;break;
						case CheckState.Indeterminate: partNr = 9; break;
					}
					if(TreeView.Enabled && this.Enabled && this.EnabledButtons)
					{
						if(checkBox.Bounds.Contains(pt))
						{
							if(mouseDown)
								partNr+=2;
							else
								partNr++;
						}
					}
					else
					{
						partNr+=3;
					}
					
					buttonTD.DrawThemeBackground(e.Graphics,3,partNr,this.checkBox.Bounds);
				}
				//Draw the OptionButton
				if(!e.HandledOptionButton && optionButton.Visible && parent!=null && parent.Expanded)
				{
					int partNr = 1;
					if(this.optioned) partNr = 5;

					if(TreeView.Enabled && this.Enabled && this.EnabledButtons)
					{
						if(optionButton.Bounds.Contains(pt))
						{
							if(mouseDown)
								partNr+=2;
							else
								partNr++;
						}
					}
					else
					{
						partNr+=3;
					}


					buttonTD.DrawThemeBackground(e.Graphics,2,partNr,this.optionButton.Bounds);
				}
			}
			else
			{
				bool transparent = TreeView.TransparentControls;
				Brush backBrush = new SolidBrush(TreeView.BackColor);
				//Draw the plusminus.
				if(!e.HandledPlusMinus && this.ShouldDrawPlusMinus())
				{
					int Width = plusMinus.Width;
					int Height = plusMinus.Height;
					Rectangle rc = new Rectangle(plusMinus.Location.X,plusMinus.Location.Y,Width-1,Height-1);
                    if (this.TreeView.EnableTouchMode && this.TreeView.Style == TreeStyle.Metro)
                    {
                        rc = new Rectangle(plusMinus.Location.X + 5, plusMinus.Location.Y , Width - 1, Height - 1);
                    }
                    SolidBrush brush = new SolidBrush(this.TreeView.MetroColor);
                    if(this.TreeView.Style==TreeStyle.Office2010)
                        brush = new SolidBrush(this.TreeView.Office2010ColorTable.TreeNodeArrowColor);
                    if (!transparent && this.TreeView.Style != TreeStyle.Metro && this.TreeView.Style != TreeStyle.Office2010)
					{
						e.Graphics.FillRectangle(backBrush,plusMinus.Bounds);
					}

					if( ExpandImage != null && this.Expanded )
					{
						e.Graphics.DrawImage( this.ExpandImage, rc );
					}
					else
					{
                        if (this.TreeView.Style == TreeStyle.Default)
                        {
                            if (Expanded)
                            {
                                e.Graphics.DrawRectangle(Pens.Gray, rc);
                            }

                            e.Graphics.DrawLine(Pens.Black, plusMinus.Location.X + 2, plusMinus.Location.Y + Height / 2, plusMinus.Location.X + Width - 3, plusMinus.Location.Y + Height / 2);
                        }
                        else if (this.TreeView.Style == TreeStyle.Office2007)
						{
                            Blend blend = new Blend();
                            blend.Positions = new float[] { 0.0F, 0.30F, 0.45F, 1.0F };
                            blend.Factors = new float[] { 0.0F, 0.5F, 1.1F, 0.5F };
                            using (LinearGradientBrush lgb = new LinearGradientBrush(rc, this.TreeView.Office2007ColorTable.TreeNodeArrowColor, this.TreeView.Office2007ColorTable.SelectedNodeBackground, LinearGradientMode.Vertical))
                            {
                                lgb.Blend=blend;
                                RendererUtils.GetRoundedPolygon(rc, 1);
                                lgb.WrapMode = WrapMode.TileFlipY;
                                e.Graphics.FillRectangle(lgb, rc);
                            }
							if (Expanded)
							{
                                e.Graphics.DrawRectangle(new Pen(this.TreeView.Office2007ColorTable.TreeNodeArrowColor), rc);
							}

                            e.Graphics.DrawLine(Pens.Black, plusMinus.Location.X + 2, plusMinus.Location.Y + Height / 2, plusMinus.Location.X + Width - 3, plusMinus.Location.Y + Height / 2);
						}
                        else if ((this.TreeView.Style == TreeStyle.Metro || this.TreeView.Style == TreeStyle.Office2010) && this.Expanded)
						{
							Point[] points =  new Point[3];
							points[0] = new Point (rc.Left ,rc.Height +rc.Top-1 );
							points[1] = new Point (rc.Left +rc.Width-1 ,rc.Height +rc.Top-1 );
							points [2] = new Point (rc.Left+rc.Width-1 , rc.Top );
                            if (this.TreeView.Style == TreeStyle.Metro)
                                brush = new SolidBrush(this.PlusMinusArrowColor);
                            e.Graphics.FillPolygon(brush, points);
                            if (this.TreeView.Style == TreeStyle.Office2010)
                                e.Graphics.DrawPolygon(new Pen(this.TreeView.Office2010ColorTable.TreeNodeArrowColor), points);
						}
					}

					if( !Expanded || ( this.ShowPlusOnExpand && this.TreeView.LoadOnDemand ) )
					{
						if( CollapseImage != null )
						{
							e.Graphics.DrawImage( this.CollapseImage, rc );
						}
						else
						{
                            if (this.TreeView.Style == TreeStyle.Default)
                            {
                                e.Graphics.DrawRectangle(Pens.Gray, rc);
                                e.Graphics.DrawLine(Pens.Black, plusMinus.Location.X + Width / 2, plusMinus.Location.Y + 2, plusMinus.Location.X + Width / 2, plusMinus.Location.Y + Height - 3);
                            }
                            if (this.TreeView.Style == TreeStyle.Office2007)
							{
                                e.Graphics.DrawRectangle( new Pen(this.TreeView.Office2007ColorTable.TreeNodeArrowColor), rc);
								e.Graphics.DrawLine(Pens.Black, plusMinus.Location.X + Width / 2, plusMinus.Location.Y + 2, plusMinus.Location.X + Width / 2, plusMinus.Location.Y + Height - 3);
                                
							}
                            else if (this.TreeView.Style == TreeStyle.Metro || this.TreeView.Style == TreeStyle.Office2010)
							{
								Point[] points = new Point[3];
								points[0] = new Point(rc.Left +3, rc.Height + rc.Top+2);
								points[1] = new Point(rc.Left + rc.Width, rc.Height / 2 + rc.Top+1);
								points[2] = new Point(rc.Left + 3, rc.Top);
                                if (this.TreeView.Style == TreeStyle.Metro)
                                    brush = new SolidBrush(this.PlusMinusArrowColor);
                                e.Graphics.FillPolygon(brush, points);
                                if (this.TreeView.Style == TreeStyle.Office2010)
                                {                                    
                                    e.Graphics.FillPolygon(new SolidBrush(this.TreeView.Office2010ColorTable.TreeNodeArrowColor), points);
                                    e.Graphics.DrawPolygon(new Pen(this.TreeView.Office2010ColorTable.TreeNodeArrowColor), points);
                                }
							}
						}
					}
                    brush.Dispose();
				}
				//Draw the checkbox.
				if(!e.HandledCheckBox && checkBox.Visible && parent!=null && parent.Expanded &&
					checkBox.xIndentFromNodeLeft != int.MinValue)
				{
					ButtonState bstate = ButtonState.Normal;
					switch(this.CheckState)
					{
						case CheckState.Checked: bstate = ButtonState.Checked;break;
						case CheckState.Indeterminate: bstate = ButtonState.All;break;
					}
					if(!this.Enabled || !this.EnabledButtons)
						bstate |= ButtonState.Inactive;
                    ControlPaintAdv.DrawCheckBox(this, e.Graphics, checkBox.Bounds, bstate,this.TreeView .Style == TreeStyle.Metro ,this.TreeView.MetroColor);
					//ControlPaint.DrawCheckBox(e.Graphics,checkBox.Bounds,bstate);
				}
				//Draw the optionbutton.
				if(!e.HandledOptionButton && optionButton.Visible && parent!=null && parent.Expanded &&
					optionButton.xIndentFromNodeLeft != int.MinValue)
				{
					ButtonState bstate = ButtonState.Checked;
					if(!optioned)
					{
						bstate = ButtonState.Normal;
					}
					if(!this.Enabled || !this.EnabledButtons)
						bstate |= ButtonState.Inactive;

#if (SyncfusionFramework1_0 || SyncfusionFramework1_1)
      ControlPaint.DrawRadioButton(e.Graphics,this.optionButton.Bounds,bstate);
#endif

#if SyncfusionFramework2_0
                    Rectangle optionRect = this.optionButton.Bounds;
                    if (this.TreeView.EnableTouchMode && this.ShowCheckBox)
                        optionRect.X = this.optionButton.Bounds.X + 10;
                    ControlPaintAdv.DrawRadioButton(this, e.Graphics, optionRect, bstate, this.TreeView.Style == TreeStyle.Metro, this.TreeView.MetroColor);
#endif

                    //ControlPaintAdv.DrawRadioButton(this,e.Graphics, this.optionButton.Bounds, bstate);
                    //ControlPaint.DrawRadioButton(e.Graphics,this.optionButton.Bounds,bstate);
				}
				backBrush.Dispose();
			}

		}

		#endregion

		internal void MakeDirty()
		{
			if(TreeView!=null)
			{
				this.TreeView.MakeDirty();
			}
		}

		/// <summary>
		/// Indicates whether node is contained in it's nodes collection or in it's subnodes nodes collection.
		/// </summary>
		/// <param name="node">Node to look for.</param>
		/// <returns>True if node is contained.</returns>
		public bool HasNode(TreeNodeAdv node)
		{
			if(nodes.Contains(node)) return true;
			for(int i=0;i<nodes.Count;i++)
			{
				if(nodes[i].HasNode(node)) return true;
			}
			return false;
		}

		internal void SetHeightIfChanged(int oldHeight,int newHeight)
		{
			if(NodeStyle.Height == oldHeight) this.NodeStyle.Height = newHeight;
			for(int i=0;i<nodes.Count;i++)
			{
				nodes[i].SetHeightIfChanged(oldHeight,newHeight);
			}
		}
		/// <summary>
		/// Returns the path of the node.
		/// </summary>
		/// <param name="separator">The separator string.</param>
		/// <returns>The path of the node.</returns>
		/// <remarks>
		/// <p>You can also use the <see cref="FullPath"/> property to get the full path
        /// with the path separator specified in the <see cref="TreeViewAdv.PathSeparator"/>
		/// property.</p>
		/// </remarks>
		public string GetPath(string separator)
		{
			if(parent == null) return "";
			string par = parent.GetPath(separator);
			if(TreeView!=null && TreeView.AddSeparatorAtEnd)
			{
				if(par=="") return NodeStyle.Text+separator;
				else
					return par+this.NodeStyle.Text+separator;
			}
			else
			{
				if(par == "") return NodeStyle.Text;
				else
					return par+separator+this.NodeStyle.Text;
			}
		}

		private TreeNodePrimitivesCollection m_primitives;
		/// <summary>
		/// 
		/// </summary>
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Content ) ]
		public TreeNodePrimitivesCollection Primitives
		{
			get
			{
				return m_primitives;
			}			
		}

		internal void SetPrimitives( TreeNodePrimitivesCollection primitives )
		{
			if( primitives != null )
			{
				m_primitives.CollectionChanged -= new CollectionChangeEventHandler( OnPrimitivesCollectionChanged );
				m_primitives = primitives;
				m_primitives.CollectionChanged += new CollectionChangeEventHandler( OnPrimitivesCollectionChanged );

				RecalculateAllDimensions();
			}
		}

		private bool LayoutPrimitive( ref int relativeLocation, ref int indent, int pmWidthWithSpace,
            int primitiveWidth, bool leftMostPartFound ) 
		{
			if( !leftMostPartFound )
			{
				leftMostPartFound = true;

				// If possible center the vertical line to the checkbox 
				if(relativeLocation-primitiveWidth/2 > pmWidthWithSpace)
				{
					relativeLocation -= primitiveWidth/2;
				}

				this.lineRightRel = relativeLocation - 1;
			}

			indent = relativeLocation;

			relativeLocation += primitiveWidth+spc;

			return leftMostPartFound;
		}

		private bool LayoutCheckBox( ref int relativeLocation, int pmWidthWithSpace, bool leftMostPartFound )
		{
			return LayoutPrimitive( ref relativeLocation, 
				ref CheckBox.xIndentFromNodeLeft, pmWidthWithSpace, 
				CheckBox.Width, leftMostPartFound );
		}

		private bool LayoutOptionsButton( ref int relativeLocation, int pmWidthWithSpace, bool leftMostPartFound )
		{
			return LayoutPrimitive( ref relativeLocation, 
				ref OptionButton.xIndentFromNodeLeft, pmWidthWithSpace, 
                OptionButton.Width, leftMostPartFound );
		}

		private bool LayoutImages( ImageList imageList, int[] arrIndexes,
			ref int relativeLocation, int pmWidthWithSpace, int imagePadding, bool leftMostPartFound )
		{
			if( imageList !=null && imageList.Images.Count > 0 && 
				arrIndexes != null && arrIndexes.Length > 0 )
			{
				bool atleast1 = false;
				for( int i = 0; i < arrIndexes.Length; i++ )
				{
					int index = arrIndexes[ i ];

					if( index >= 0 && index < imageList.Images.Count )
					{
						if( !leftMostPartFound )
						{
							leftMostPartFound = true;
							// If possible center the vertical line to the Left Image 
							if(relativeLocation-imageList.ImageSize.Width/2 > pmWidthWithSpace)
								relativeLocation -= imageList.ImageSize.Width/2;
							this.lineRightRel = relativeLocation - 1;
						}
						if( !atleast1 )
						{							
							atleast1 = true;
						}

						relativeLocation += imageList.ImageSize.Width;
						relativeLocation += imagePadding;
					}
				}

				if( atleast1 )
				{
					relativeLocation += spc;
				}
			}

			return leftMostPartFound;
		}

		private bool LayoutStateImage(ImageList imageList, int imageIndex,
			ref int relativeLocation, int pmWidthWithSpace,
			int imagePadding, bool leftMostPartFound )
		{
			if( imageList != null && imageList.Images.Count > 0 )
			{	
				if( imageIndex >= 0 && imageIndex < imageList.Images.Count )
				{					
					if( !leftMostPartFound )
					{
						leftMostPartFound = true;

						// If possible center the vertical line to the state Image. 
						if( relativeLocation-imageList.ImageSize.Width/2 > pmWidthWithSpace)
						{
							relativeLocation -= imageList.ImageSize.Width/2;							
						}
						this.lineRightRel = relativeLocation - 1;
					}

					stateImageListXRel = relativeLocation;

					relativeLocation += imageList.ImageSize.Width + spc;
					relativeLocation += imagePadding;
				}
			}

			return leftMostPartFound;
		}

		private bool LayoutText( ref int relativeLocation, bool leftMostPartFound )
		{
			using( Graphics g = TreeView.CreateGraphics() )
			{
				this.textWidth = GetNodeTextSize( g, nodeData.Font ).Width;
            
				this.m_printTextSize =  Size.Round( g.MeasureString( this.Text, this.Font ) );

				if( !leftMostPartFound )
				{
					leftMostPartFound = true;
					lineRightRel = relativeLocation - 1;
				}

				if( textWidth > 0 )
				{
					relativeLocation += textWidth+spc;
				}

				this.Width = relativeLocation;
				int myMax = this.bounds.X+nodeXRel+width;
				if(!HasChildren) this.MaxX = myMax;
				else
				{
					// If, in the middle of recalculating, ignore this.
					if(htRecalculatingNodes[this] == null)
						RecalculateMaxX();
					else if(myMax > this.MaxX)
						this.MaxX = myMax;
				}				
			}

			return leftMostPartFound;
		}

		/// <summary>
		/// CustomControl relative location.
		/// </summary>
		private int m_customControlRelativeLocation = 0;

		internal void UpdateCustomConrtol()
		{
			if( this.CustomControl != null && this.CustomControl.Visible )
			{
				if(!this.GetIsMirrored())
					this.CustomControl.Location = new Point( this.Bounds.X + m_customControlRelativeLocation + this.nodeXRel, this.Bounds.Y );
				else
					this.CustomControl.Location = new Point(this.Bounds.Right - m_customControlRelativeLocation - this.nodeXRel - this.CustomControl.Width, this.Bounds.Y);
				this.CustomControl.Height = this.Height;

				this.CustomControl.Update();
			}
		}

		/// <summary>
		/// Return custom control bounds.
		/// </summary>
		/// <returns></returns>
		internal Rectangle GetCustomControlBounds()
		{
			return new Rectangle(	
				this.Bounds.X + m_customControlRelativeLocation + this.nodeXRel, this.Bounds.Y,
				 this.CustomControl.Width, this.Bounds.Height);
		}


		internal void SubscribeControlEvents( Control control )
		{
			control.SizeChanged += new EventHandler(CustomControl_SizeChanged);
			control.LocationChanged += new EventHandler(CustomControl_LocationChanged);
		}

		internal void UnSubscribeControlEvents( Control control )
		{
			control.SizeChanged -= new EventHandler(CustomControl_SizeChanged);
			control.LocationChanged -= new EventHandler(CustomControl_LocationChanged);
		}

		private void CustomControl_SizeChanged(object sender, EventArgs e)
		{
			Control control = (Control) sender;

			if( control.Visible && control.Size.Height != this.Height )
			{
				control.Size = new Size( control.Width, this.Height );
			}
		}

		private void CustomControl_LocationChanged(object sender, EventArgs e)
		{
			Control control = (Control) sender;
			Point controlLocation ;
			if (!this.GetIsMirrored())
				controlLocation = new Point(this.Bounds.X + m_customControlRelativeLocation + this.nodeXRel, this.Bounds.Y);
			else
				controlLocation = new Point(this.Bounds.Right - m_customControlRelativeLocation -this.nodeXRel - control.Width, this.Bounds.Y);

			if( control.Visible && control.Location != controlLocation )
			{
				control.Location = controlLocation;
			}
		}

		/// <summary>
		/// Custom control collection changing.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="action"></param>
		internal void CustomControlCollectionChanging( TreeNodeAdv node, CollectionChangeAction action )
		{
			TreeViewAdv tree = this.TreeView;
			if( tree != null && node != null )
			{
				switch( action )
				{
					case CollectionChangeAction.Add:
						if( node.CustomControl != null )
						{
							if(	!tree.CustomControlCollection.ContainsKey( node.CustomControl ) )
							{
								tree.CustomControlCollection.Add( node.CustomControl, node );
								
								if( tree.m_bAutoControlsAdding )
								{
									tree.Controls.Add( node.CustomControl );
									node.SubscribeControlEvents( node.CustomControl );
								}
							}
							else if( tree.CustomControlCollection[ node.CustomControl ] != node )
							{
								node.CustomControl = null;
							}
						}

						foreach( TreeNodeAdv childNode in node.Nodes )
						{
							CustomControlCollectionChanging( childNode, action );
						}

						tree.NeedUpdateCustomControls = true;

						break;

					case CollectionChangeAction.Remove:
						if( node.CustomControl != null )
						{
							if(	tree.CustomControlCollection.ContainsKey( node.CustomControl ) )
							{
								tree.CustomControlCollection.Remove( node.CustomControl );
							}
						
							if( tree.Controls.Contains( node.CustomControl ) )
							{
								tree.Controls.Remove( node.CustomControl );
								node.UnSubscribeControlEvents( node.CustomControl );
							}
						}

						foreach( TreeNodeAdv childNode in node.Nodes )
						{
							CustomControlCollectionChanging( childNode, action );
						}

						tree.NeedUpdateCustomControls = true;

						break;

					case CollectionChangeAction.Refresh:
						if( tree.CustomControlCollection.ContainsValue( node ) )
						{
							foreach( DictionaryEntry htElement in tree.CustomControlCollection )
							{
								if( htElement.Value == node )
								{
									tree.CustomControlCollection.Remove( htElement.Key );

									Control control = htElement.Key as Control;
                                    if ((node.CustomControl == null && control != null && tree.Parent is TreeViewAdvEditorForm) ||
                                        (node.CustomControl != null && htElement.Key != node.CustomControl))
                                    {
                                        TreeViewAdv originalTree = (tree.Parent as TreeViewAdvEditorForm).Tree;
                                        if (!originalTree.m_RemovedCustomControls.Contains(control))
                                            originalTree.m_RemovedCustomControls.Add(control);
                                    }                                    
									if( tree.Controls.Contains( control ) )
									{
										tree.Controls.Remove( control );
										this.UnSubscribeControlEvents( control );
									}

									break;
								}
							}
						}
					
						if( node.CustomControl != null )
						{
							if(	!tree.CustomControlCollection.ContainsKey( node.CustomControl ) )
							{
                                if (tree.Parent is TreeViewAdvEditorForm)
                                {
                                    TreeViewAdv originalTree = (tree.Parent as TreeViewAdvEditorForm).Tree;
                                    if(!originalTree.m_ControlBounds.ContainsKey(node.CustomControl))
                                        originalTree.m_ControlBounds[node.CustomControl] = node.CustomControl.Bounds;
                                    if (!originalTree.m_ControlParent.ContainsKey(node.CustomControl))
                                        originalTree.m_ControlParent[node.CustomControl] = node.CustomControl.Parent;
                                    if(originalTree.m_RemovedCustomControls.Contains(node.CustomControl))
                                        originalTree.m_RemovedCustomControls.Remove(node.CustomControl);
                                }
								tree.CustomControlCollection.Add( node.CustomControl, node );
								
								if( tree.m_bAutoControlsAdding )
								{
									tree.Controls.Add( node.CustomControl );
									this.SubscribeControlEvents( node.CustomControl );
								}
							}
							else if( tree.CustomControlCollection[ node.CustomControl ] != node )
							{
								node.CustomControl = null;
							}
						}

						this.RecalculateDimensions();
						tree.NeedUpdateCustomControls = true;
						tree.Invalidate();
                        UpdateCustomConrtol();
						break;
				}
			}
		}

		private bool LayoutCustomControl( ref int relativeLocation  )
		{

			int indent = 0;

			m_customControlRelativeLocation = relativeLocation;

			return LayoutPrimitive( ref relativeLocation, 
				ref indent, 0, 
				this.CustomControl.Width, true );
		}

		private void LayoutPrimitives( int width )
		{
			bool leftMostPartFound = false;
			int pmWidthWithSpace = 0;

			// sort primitives by their indexes
			Hashtable htItems = new Hashtable();
			ArrayList arrItems = new ArrayList();

			for( int i = 0, len = m_primitives.Count; i < len; i++ )
			{
				TreeNodePrimitive pt = m_primitives[ i ];
				ArrayList arrItemsWithSameIndex = (ArrayList)htItems[ pt.Index ];

				if(arrItemsWithSameIndex == null )
				{
					arrItemsWithSameIndex = new ArrayList();
					arrItems.Add( pt.Index );					
				}

				arrItemsWithSameIndex.Add( pt );

				htItems[ pt.Index ] = arrItemsWithSameIndex;
			}

			arrItems.Sort();

			CheckBox.xIndentFromNodeLeft = -1;
			CheckBox.xIndentFromNodeLeft = int.MinValue;
			OptionButton.xIndentFromNodeLeft = int.MinValue;
			leftImageListXRel = int.MinValue;
			rightImageListXRel = int.MinValue;
			stateImageListXRel = int.MinValue;
			textLocationXRel = int.MinValue;

			for( int k = 0, len = arrItems.Count; k < len; k++ )
			{
				ArrayList itemsList = htItems[ arrItems[ k ] ] as ArrayList;
 
				if( itemsList == null || itemsList.Count == 0 ) continue;

				foreach( TreeNodePrimitive pt in itemsList )
				{
					switch( pt.PrimitiveType )
					{
						case PredefinedPrimitiveTypes.CheckBox:
							if( NodeStyle.ShowCheckBox )
							{
								leftMostPartFound = LayoutCheckBox( ref width,
									pmWidthWithSpace, leftMostPartFound );
							}
							break;

						case PredefinedPrimitiveTypes.OptionsButton :
							if( NodeStyle.ShowOptionButton )
							leftMostPartFound = LayoutOptionsButton( ref width,
								pmWidthWithSpace, leftMostPartFound );
							break;

						case PredefinedPrimitiveTypes.LeftImages :
							leftImageListXRel = width;

							leftMostPartFound = LayoutImages( TreeView.LeftImageList,							
								NodeStyle.LeftImageIndices, ref width, pmWidthWithSpace, 
								LeftImagePadding, leftMostPartFound );						
							break;

						case PredefinedPrimitiveTypes.RightImages :
							rightImageListXRel = width;

							leftMostPartFound = LayoutImages( TreeView.RightImageList,							
								NodeStyle.RightImageIndices, ref width, pmWidthWithSpace, 
								RightImagePadding, leftMostPartFound );
							break;
                        
						case PredefinedPrimitiveTypes.StateImage :
							int imgIndex = 0;

							if ( !HasChildren && !( TreeView.LoadOnDemand && !this.expandedOnce ) ) 
							{
								imgIndex = this.NodeStyle.NoChildrenImgIndex;
							}
							else
							{
								imgIndex = ( Expanded ) ? NodeStyle.OpenImgIndex : NodeStyle.ClosedImgIndex;
							}						

							leftMostPartFound = LayoutStateImage( TreeView.StateImageList, imgIndex, ref width,
								pmWidthWithSpace, LeftStateImagePadding + RightStateImagePadding,
								leftMostPartFound );					
							break;

						case PredefinedPrimitiveTypes.Text :
							textLocationXRel = width;
							leftMostPartFound = LayoutText( ref width, leftMostPartFound );
							break;

						case PredefinedPrimitiveTypes.CustomControl :
							if( this.CustomControl != null )
							{
								leftMostPartFound = LayoutCustomControl( ref width );
							}

							break;
					}
				}
			}

		}

		// Calculating only the width, not the locations (as we don't know the Y)
		private void RecalculateDimensions()
		{
			if(this.TreeView == null) return;
			TreeViewAdv tree = TreeView;

			int parentIndent = tree.Indent;
			this.nodeXRel = (Level-1)*parentIndent;

			int width = 0;
			bool leftMostPartFound = false;
			int pmWidthWithSpace = 0;

			if(this.Level != 1 || tree.NeedRootLinesSpace)
			{
				if(this.NodeStyle.ShowPlusMinus)
				{
					PlusMinus.xIndentFromNodeLeft = 0;
				}
				// Whether or not we show plus-minus:
				width += this.plusMinus.Width + spc;

				pmWidthWithSpace = width;

				if(width < parentIndent)
					width = parentIndent + this.plusMinus.Width/2;
			}
			if(this.Level != 1 && !tree.NeedRootLinesSpace)
			{
				nodeXRel -= parentIndent;
				nodeXRel += this.plusMinus.Width/2;
			}

			this.lineRightRel = width - 1;

			if( m_primitives != null && m_primitives.Count > 0 )
			{
				LayoutPrimitives( width );
			}
			else
			{
				#region default logic
				if(this.NodeStyle.ShowCheckBox)
				{
					if(!leftMostPartFound)
					{
						leftMostPartFound = true;
						// If possible center the vertical line to the checkbox 
						if(width-CheckBox.Width/2 > pmWidthWithSpace)
							width -= CheckBox.Width/2;
						this.lineRightRel = width - 1;
					}
					CheckBox.xIndentFromNodeLeft = width;
					width += CheckBox.Width+spc;
				}
				if(this.NodeStyle.ShowOptionButton)
				{
					if(!leftMostPartFound)
					{
						leftMostPartFound = true;
						// If possible center the vertical line to the option button 
						if(width-OptionButton.Width/2 > pmWidthWithSpace)
							width -= OptionButton.Width/2;
						this.lineRightRel = width - 1;
					}
					OptionButton.xIndentFromNodeLeft = width;

					width += OptionButton.Width +spc;
				}

				ImageList leftImageList = tree.LeftImageList;
				if(leftImageList!=null)
				{
					int[] indexes = NodeStyle.LeftImageIndices;

					bool atleast1 = false;
					for(int i=0;i<indexes.Length;i++)
					{
						int index = indexes[i];
						if(index>=0 && index < leftImageList.Images.Count)
						{
							if(!leftMostPartFound)
							{
								leftMostPartFound = true;
								// If possible center the vertical line to the Left Image 
								if(width-leftImageList.ImageSize.Width/2 > pmWidthWithSpace)
									width -= leftImageList.ImageSize.Width/2;
								this.lineRightRel = width - 1;
							}
							if(!atleast1)
							{
								this.leftImageListXRel = width;
								atleast1 = true;
							}
							width += leftImageList.ImageSize.Width;
							width += LeftImagePadding;
						}
					}
					if(atleast1)
					{
						width += spc;
					}
				}

				ImageList stateImgList = tree.StateImageList;
				if(stateImgList!=null)
				{	
					int imgIndex = 0;
					if(!HasChildren && !(TreeView.LoadOnDemand && !this.expandedOnce)) imgIndex = this.NodeStyle.NoChildrenImgIndex;
					else
					{
						if(Expanded) imgIndex = this.NodeStyle.OpenImgIndex;
						else	imgIndex = this.NodeStyle.ClosedImgIndex;
					}
					if(imgIndex >= 0 && imgIndex< stateImgList.Images.Count)
					{
						if(!leftMostPartFound)
						{
							leftMostPartFound = true;
							// If possible center the vertical line to the state Image. 
							if(width-stateImgList.ImageSize.Width/2 > pmWidthWithSpace)
								width -= stateImgList.ImageSize.Width/2;
							this.lineRightRel = width - 1;
						}
						this.stateImageListXRel = width;
						width += stateImgList.ImageSize.Width+spc;
						width += LeftStateImagePadding + RightStateImagePadding;
					}
				}
				if(!leftMostPartFound)
				{
					leftMostPartFound = true;
					// If possible make the text start a few pixels to the left of the vertical line 
					if(width-9 > pmWidthWithSpace)
						width -= 9;
					this.lineRightRel = width - 1;
				}
				this.textLocationXRel = width;

				#region /* comments */
				// This string was commented by Lucas in order to change method of
				// string width measuring.
				// this.textWidth = tree.MeasureString(nodeData.Text,nodeData.Font) + 3;
				#endregion

				if (TreeView.IsHandleCreated)
				{
					Graphics g = TreeView.CreateGraphics();
					this.textWidth = GetNodeTextSize(g, nodeData.Font).Width;

					this.m_printTextSize = Size.Round(g.MeasureString(this.Text, this.Font));

					g.Dispose();
				}
				else
				{
					this.textWidth = TextRenderer.MeasureText(this.Text, nodeData.Font).Width;
					this.m_printTextSize = Size.Round(TextRenderer.MeasureText(this.Text, this.Font));
				}

				if(textWidth > 0)
					width += textWidth+spc;

				this.rightImageListXRel = width;

				if(tree.RightImageList!=null)
				{
					width += tree.RightImageList.ImageSize.Width * nodeData.RightImageIndices.Length + spc;
					width += nodeData.RightImageIndices.Length * RightImagePadding;
				}

				if( this.CustomControl != null )
				{
                    m_customControlRelativeLocation = width;
					width += this.CustomControl.Width;
				}

				this.Width = width;
				int myMax = this.bounds.X+nodeXRel+width;
				if(!HasChildren) this.MaxX = myMax;
				else
				{
					// If, in the middle of recalculating, ignore this.
					if(htRecalculatingNodes[this] == null)
						RecalculateMaxX();
					else if(myMax > this.MaxX)
						this.MaxX = myMax;
				}
				#endregion
			}
		}

		private int ParentIndent
		{
			get
			{
				if(TreeView!=null)
					return TreeView.Indent;
				else return 19;
			}
		}
		
		/// <summary>
		/// Be very discrete about calling this, as it could cause performance problems.
		/// </summary>
		private void RecalculateMaxX()
		{
			int max = spc+(Level-1)*ParentIndent+this.Width;
			if(Expanded)
				for(int i=0;i<nodes.Count;i++)
				{
					max = Math.Max(max,nodes[i].MaxX);
				}
			MaxX = max;
		}

		/// <summary>
		/// Recalculates the dimensions of all the UI elements in this node and it's children.
		/// </summary>
		public void RecalculateAllDimensions()
		{
			TreeViewAdv tree = this.TreeView;

			if( tree != null )
			{
				tree.NeedUpdateCustomControls = true;
			}

			// Marking a flag that says I am in the process of recalculating my MaxX
			htRecalculatingNodes[this] = 1;
			// Lose my previous maxX
			this.maxX = 0;

			if (this.Expanded)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					nodes[i].RecalculateAllDimensions();
				}
			}
			this.RecalculateDimensions();

			// Clearing the flag.
			htRecalculatingNodes.Remove(this);
		}

		private void nodes_BeforeRemoving( object sender, CollectionChangeEventArgs e )
		{
			TreeViewAdv tree = this.TreeView;
			TreeNodeAdv node = e.Element as TreeNodeAdv;

			if( node != null && tree != null && tree.HistoryEnabled &&
				!node.IsUndoRedoPerforming )
			{
				HistoryManager historyManager = tree.HistoryManager;

				if( historyManager != null )
				{
					TreeViewCommand cmd = new TreeViewCommand( node, Action.Remove );
					historyManager.Do( cmd );
				}
			}
		}

		private void OnPrimitivesCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			RecalculateDimensions();
		}
	}
	/// <summary>
	/// Specifies the different sort types that can be specified in the <see cref="TreeNodeAdv.Sort"/> method.
	/// </summary>
	public enum TreeNodeAdvSortType
	{
		/// <summary>
		/// Sorts by text.
		/// </summary>
		Text,
		/// <summary>
		/// Sorts by the tag value.
		/// </summary>
		Tag,
		/// <summary>
		/// Sorts by the checkbox value.
		/// </summary>
		CheckBox
	}
	/// <summary>
	/// Specifies the node positions in a node collection.
	/// </summary>
	public enum NodePositions
	{
		First, Last, Previous, Next
	}
	// Special class to support serializing the ChildStyle info.
	[Documentation.DocumentationExclude()]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ChildTreeNodeAdvStyleInfo : TreeNodeAdvStyleInfo
	{
		public ChildTreeNodeAdvStyleInfo(){}
		public ChildTreeNodeAdvStyleInfo(StyleInfoIdentityBase identity, TreeNodeAdvStyleInfoStore store)
			: base(identity, store)
		{
		}
		public ChildTreeNodeAdvStyleInfo(StyleInfoIdentityBase identity)
			: base(identity)
		{
		}
	}

	public class CustomControlEditor : UITypeEditor
	{
		private IWindowsFormsEditorService m_editorService = null;
		private ListBox m_listBox = null;

		public CustomControlEditor()
		{
			m_listBox = new ListBox();
			m_listBox.BorderStyle = BorderStyle.None;

			m_listBox.Click += new EventHandler(listBox_Click);
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if( provider != null )
			{
				m_editorService = (IWindowsFormsEditorService) provider.GetService( typeof(IWindowsFormsEditorService) );

				if( m_editorService != null )
				{
					m_listBox.Items.Clear();
					
					IDesignerHost designerHost = (IDesignerHost) provider.GetService( typeof(IDesignerHost) );

					if( designerHost != null )
					{
						ArrayList controlsList = new ArrayList();

						m_listBox.Items.Add( "(none)" );
						m_listBox.SelectedIndex = 0;
						controlsList.Add( null );

						TreeNodeAdv node = (TreeNodeAdv) context.Instance;

						foreach( Component component in designerHost.Container.Components )
						{
							Control control = component as Control;

							if( control != null )
							{
								Type controlType = control.GetType();

								if( controlType != typeof( Form ) && controlType != typeof( TreeViewAdv ) )
								{
									controlsList.Add( control );
									m_listBox.Items.Add( control.Name );

									if( node.CustomControl == control )
									{
										m_listBox.SelectedIndex = m_listBox.Items.Count - 1;
									}
								}
							}
						}
						
						m_editorService.DropDownControl( m_listBox );
						
						value = controlsList[ m_listBox.SelectedIndex ];
					}
				}
			}

			return value;
		}

		private void listBox_Click(object sender, EventArgs e)
		{
			if( m_editorService != null )
			{
				m_editorService.CloseDropDown();
			}
		}
	}
	
	internal class ControlPaintAdv
    {
        public static void DrawCheckBox(TreeNodeAdv tna, Graphics g, Rectangle r, ButtonState bs,bool metro, Color metroColor)
        {
            //            Color check = (ButtonState.Inactive == bs) ? tna.InActiveCheckColor : tna.ActiveCheckColor;

            Brush brush1 = ((bs & ButtonState.Inactive) == ButtonState.Inactive) ? tna.IntermediateCheckBoxBackGround  /*intermediate*/ : tna.CheckBoxBackGround;
            Color check = ((bs & ButtonState.Inactive) == ButtonState.Inactive) ? tna.IntermediateCheckColor  : tna.CheckColor;
            
            MethodInfo[] mis = typeof(ControlPaint).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo mii = null;
            foreach (MethodInfo mi in mis)
            {
                if (mi.Name == "DrawFlatCheckBox")
                {
                    if (mi.GetParameters().Length > 3)
                    {
                        mii = mi; break;
                    }
                }
            }

            //MethodInfo mi = typeof(ControlPaint).GetMethod("DrawFlatCheckBox", BindingFlags.NonPublic | BindingFlags.Static);
            if (metro)
            {
                mii.Invoke(null, new object[] { g, r, metroColor, brush1, bs });

            }
            else
                mii.Invoke(null, new object[] { g, r, check, brush1, bs });
           

        }
        public static void DrawRadioButton(TreeNodeAdv tna, Graphics g, Rectangle r, ButtonState bs, bool metro, Color metroColor)
        {
            if (metro)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle (r.Left +2,r.Top +2,r.Width -4,r.Height -4);
                Pen pen = new Pen(Color.Gray );
                g.DrawEllipse(pen, rect);
                pen.Dispose ();
                if (bs == ButtonState.Checked)
                {
                    rect = new Rectangle(rect.Left + 2, rect.Top + 2, rect.Width - 4, rect.Height - 4);
                    SolidBrush brush = new SolidBrush(metroColor);
                    g.FillEllipse(brush, rect);
                    brush.Dispose();
                }
            }
            else
            {
                MethodInfo[] mis = typeof(ControlPaint).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo mii = null;
                foreach (MethodInfo mi in mis)
                {
                    if (mi.Name == "DrawFrameControl")
                    {
                        if (mi.GetParameters().Length > 7)
                        {
                            mii = mi; break;
                        }
                    }
                }

                mii.Invoke(null, new object[] { g, r.Left, r.Top, r.Width, r.Height, 4, (int)(((ButtonState)4) | bs), tna.SelectedOptionButtonColor, tna.OptionButtonColor });
            }
           
        }
    }
}