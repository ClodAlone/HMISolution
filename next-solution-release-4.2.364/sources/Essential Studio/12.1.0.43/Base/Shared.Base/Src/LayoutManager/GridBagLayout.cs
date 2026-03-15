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
using System.Globalization;
using System.Threading;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Syncfusion.Runtime.Serialization;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class InsetsConverter : 
		ExpandableObjectConverter
	{
		public override /*TypeConverter*/ object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			int top = (int)propertyValues["Top"];
			int left = (int)propertyValues["Left"];
			int right = (int)propertyValues["Right"];
			int bottom = (int)propertyValues["Bottom"];
			
			Insets insets = new Insets(left, top, right, bottom);
			return insets;
		} // end of method CreateInstance
 
		public override /*TypeConverter*/ bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		} // end of method GetCreateInstanceSupported
		
		public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		} // end of method CanConvertTo
        
		public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
				&& (value is Insets))
			{
				Insets insetsInst = (Insets)value;
				System.Type[] args;
				args = new System.Type[4];
				args[0] = typeof(int);
				args[1] = typeof(int);
				args[2] = typeof(int);
				args[3] = typeof(int);
				
				System.Reflection.ConstructorInfo constructorInfo;
				constructorInfo = typeof(Insets).GetConstructor(args);
				if (constructorInfo != null)
				{
					object[] argValues;
					argValues = new System.Object[4];
					argValues[0] = insetsInst.Left;
					argValues[1] = insetsInst.Top;
					argValues[2] = insetsInst.Right;
					argValues[3] = insetsInst.Bottom;
					return new InstanceDescriptor(constructorInfo,argValues);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		} // end of method ConvertTo
	}
	/// <summary>
	/// Specifies the inset padding, in pixels, for a component. Used by
	/// the <see cref="GridBagLayout"/> manager.
	/// </summary>
	/// <remarks>
	/// This is the extra space that the manager adds around a component's preferred bounds before
	/// laying out the component.
	/// </remarks>
	[
	TypeConverter(typeof(InsetsConverter)),
	Serializable()
	]
	public struct Insets
	{
		private int left, top, right, bottom;
		/// <summary>
		/// Creates an instance on the Insets class.
		/// </summary>
		/// <param name="left">Number of pixels added to the left of the component.</param>
		/// <param name="top">Number of pixels added to the top of the component.</param>
		/// <param name="right">Number of pixels added to the right of the component.</param>
		/// <param name="bottom">Number of pixels added to the bottom of the component.</param>
		public Insets(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		/// <summary>
		/// Gets / sets the insets to the left of the component.
		/// </summary>
		[Description("Specifies the insets to the left of the component.")]
		public int Left
		{
			get{return this.left;}
			set{this.left = value;}
		}

		/// <summary>
		/// Gets / sets the insets to the right of the component.
		/// </summary>
		[Description("Specifies the insets to the right of the component.")]
		public int Right
		{
			get{return this.right;}
			set{this.right = value;}
		}

		/// <summary>
		/// Gets / sets the insets to the top of the component.
		/// </summary>
		[Description("Specifies the insets to the top of the component.")]
		public int Top
		{
			get{return this.top;}
			set{this.top = value;}
		}

		/// <summary>
		/// Gets / sets the insets to the bottom of the component.
		/// </summary>
		[Description("Specifies the insets to the bottom of the component.")]
		public int Bottom
		{
			get{return this.bottom;}
			set{this.bottom = value;}
		}
		public override /*Object*/ string ToString()
		{
			return "T " + this.top + ", L " + this.left + ", B " + this.bottom
				+ ", R " + this.right;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public override bool Equals(object o)
		{
			bool eq = false;

			if ((object)this == o)
				return true;
		          
			if ( o is Insets )
			{
				Insets insets = (Insets)o;
				if(this.Bottom != insets.Bottom)
					return false;
				else if(this.Left != insets.Left)
					return false;
				else if(this.Right != insets.Right)
					return false;
				else if(this.Top != insets.Top)
					return false;
				else
					return true;
			}
			return eq;
		}
		/// <summary cref="Equals">
		///		The basic == operator.
		/// </summary>
		/// <param name="lhs">The left-hand side of the operator.</param>
		/// <param name="rhs">The right-hand side of the operator.</param>
		/// <returns>
		///		Boolean value.
		///	</returns>
		public static bool operator==( Insets lhs, Insets rhs ) 
		{                            
			if((object)lhs == null && (object)rhs == null)
				return true;

			if ((object) lhs == null || (object) rhs == null)
				return false;
			return lhs.Equals(rhs);
		}

		/// <summary cref="Equals">
		///		The basic != operator
		/// </summary>
		/// <param name="lhs">The left-hand side of the operator.</param>
		/// <param name="rhs">The right-hand side of the operator.</param>
		/// <returns>
		///		Boolean value.
		///	</returns>
		public static bool operator!=( Insets lhs, Insets rhs ) 
		{          
			if((object)lhs == null && (object)rhs == null)
				return false;

			if ((object) lhs == null || (object) rhs == null)
				return true;
			return !lhs.Equals(rhs);
		}

	}
	
	[Syncfusion.Documentation.DocumentationExclude()]
	public class GridBagConstraintsSerializationSurrogate:
		ISerializationSurrogate 
	{
		// Serialize object
		void ISerializationSurrogate.GetObjectData( Object obj, SerializationInfo info, StreamingContext context ) 
		{
			GridBagConstraints constraints = (GridBagConstraints)obj;

			info.AddValue( "gridPosX",	constraints.GridPosX	);
			info.AddValue( "gridPosY",	constraints.GridPosY	);
			info.AddValue( "cellSpanX",	constraints.CellSpanX	);
			info.AddValue( "cellSpanY",	constraints.CellSpanY	);
			info.AddValue( "weightX",	constraints.WeightX		);
			info.AddValue( "weightY",	constraints.WeightY		);
			info.AddValue( "anchor",	constraints.Anchor		);
			info.AddValue( "fill",		constraints.Fill		);
			info.AddValue( "insets",	constraints.Insets		);
			info.AddValue( "ipadX",		constraints.IpadX		);
			info.AddValue( "ipadY",		constraints.IpadY		);
			info.AddValue( "isEmpty",	constraints.IsEmpty		);
		}

		// Deserialize a object
		Object ISerializationSurrogate.SetObjectData( Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector ) 
		{
			GridBagConstraints constraints = (GridBagConstraints)obj;

			constraints.GridPosX	= info.GetInt32( "gridPosX" );
			constraints.GridPosY	= info.GetInt32( "gridPosY" );
			constraints.CellSpanX	= info.GetInt32( "cellSpanX" );
			constraints.CellSpanY	= info.GetInt32( "cellSpanY" );
			constraints.WeightX		= info.GetInt32( "weightX" );
			constraints.WeightY		= info.GetInt32( "weightY" );
			constraints.Anchor		= (AnchorTypes)info.GetValue( "anchor", typeof(AnchorTypes) );
			constraints.Fill		= (FillType)info.GetValue( "fill", typeof(FillType) );
			constraints.Insets		= (Insets)info.GetValue( "insets", typeof(Insets) );
			constraints.IpadX		= info.GetInt32( "ipadX" );
			constraints.IpadY		= info.GetInt32( "ipadY" );
			constraints.IsEmpty		= info.GetBoolean( "isEmpty" );

			return constraints;
		}
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public class GridBagConstraintsConverter : 
		ByteStreamTypeConverter
	{
		public override void OnBeforeDeserialize()
		{
			AppStateSerializer.SetBindingInfo("Syncfusion.Shared.Base", typeof(GridBagConstraints).Assembly);

			// For backward compatibility.
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Shared.Base", typeof(GridBagConstraints).FullName, typeof(GridBagConstraints).Assembly);
		}

		public override void OnAfterDeserialize()
		{
			// Reset.
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Shared.Base", typeof(GridBagConstraints).FullName, null);
		}

		public override /*TypeConverter*/ bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return false;
		} // end of method GetCreateInstanceSupported

		public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		} // end of method CanConvertTo
        
		public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
				&& (value is GridBagConstraints))
			{
				GridBagConstraints gbcInst = (GridBagConstraints)value;

				System.Type[] args;
				args = new System.Type[12];

				args[0] = typeof(int);
				args[1] = typeof(int);
				args[2] = typeof(int);
				args[3] = typeof(int);
				args[4] = typeof(double);
				args[5] = typeof(double);
				args[6] = typeof(AnchorTypes);
				args[7] = typeof(FillType);
				args[8] = typeof(Insets);
				args[9] = typeof(int);
				args[10] = typeof(int);
				args[11] = typeof(bool);
				
				System.Reflection.ConstructorInfo constructorInfo;
				constructorInfo = typeof(GridBagConstraints).GetConstructor(args);
				if (constructorInfo != null)
				{
					object[] argValues;
					argValues = new System.Object[12];
					argValues[0] = gbcInst.GridPosX;
					argValues[1] = gbcInst.GridPosY;
					argValues[2] = gbcInst.CellSpanX;
					argValues[3] = gbcInst.CellSpanY;
					argValues[4] = gbcInst.WeightX;
					argValues[5] = gbcInst.WeightY;
					argValues[6] = gbcInst.Anchor;
					argValues[7] = gbcInst.Fill;
					argValues[8] = gbcInst.Insets;
					argValues[9] = gbcInst.IpadX;
					argValues[10] = gbcInst.IpadY;
					argValues[11] = gbcInst.IsEmpty;

					return new InstanceDescriptor(constructorInfo,argValues);
				}
			}
			else if (destinationType == typeof(string) && value is GridBagConstraints)
			{
				GridBagConstraints gbcInst = (GridBagConstraints)value;

				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(gbcInst);

				string constraints = String.Empty;

				PropertyDescriptor prop = properties.Find("Anchor", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += gbcInst.Anchor.ToString() + "; ";

				prop = properties.Find("CellSpanX", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += "CellSpanX " + gbcInst.CellSpanX.ToString() + "; ";

				prop = properties.Find("CellSpanY", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += "CellSpanY " + gbcInst.CellSpanY.ToString() + "; ";

				prop = properties.Find("Fill", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += gbcInst.Fill.ToString() + "; ";

				prop = properties.Find("GridPosX", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += "GridPosX " + gbcInst.GridPosX.ToString() + "; ";

				prop = properties.Find("GridPosY", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += "GridPosY " + gbcInst.GridPosY.ToString() + "; ";

				prop = properties.Find("Insets", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += "Insets " + gbcInst.Insets.ToString() + "; ";

				prop = properties.Find("IpadX", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += "IpadX " + gbcInst.IpadX.ToString() + "; ";

				prop = properties.Find("IpadY", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += "IpadY " + gbcInst.IpadY.ToString() + "; ";

				prop = properties.Find("WeightX", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += String.Format("WeightX {0:###.##};", gbcInst.WeightX);

				prop = properties.Find("WeightY", false);
				if(prop.ShouldSerializeValue(gbcInst))
					constraints += String.Format("WeightY {0:###.##};", gbcInst.WeightY);

				return constraints;
			}
			else if( (value is GridBagConstraints) && destinationType == typeof(byte[]) )
			{
				GridBagConstraintsSerializationSurrogate surrogate = new GridBagConstraintsSerializationSurrogate();
				SurrogateSelector ss = new SurrogateSelector();
					ss.AddSurrogate( typeof(GridBagConstraints), new StreamingContext(StreamingContextStates.All), surrogate );

				BinaryFormatter bf = new BinaryFormatter();
					bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
					bf.SurrogateSelector = ss;

				MemoryStream ms = new MemoryStream();
				byte[] bytes = null;

				try
				{
					bf.Serialize(ms, value);
					bytes = ms.ToArray();
				}
				finally
				{
					ms.Close();
				}

				return bytes;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		} // end of method ConvertTo

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			object o = null;
			byte[] bytes = value as byte[];

			if( null != bytes )
			{
				GridBagConstraintsSerializationSurrogate surrogate = new GridBagConstraintsSerializationSurrogate();
				SurrogateSelector ss = new SurrogateSelector();
					ss.AddSurrogate( typeof(GridBagConstraints), new StreamingContext(StreamingContextStates.All), surrogate );

				BinaryFormatter bf = new BinaryFormatter();
					bf.Binder = AppStateSerializer.CustomBinder;	
					bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
					bf.SurrogateSelector = ss;

				MemoryStream ms = new MemoryStream(bytes);

				this.OnBeforeDeserialize();

				o = bf.Deserialize(ms);

				this.OnAfterDeserialize();

				ms.Close();
			}
			else
			{
				o = base.ConvertFrom(context, culture, value);
			}

			return o;
		}
	}
	/// <summary>
	/// Specifies how to resize a component when the component's
	/// layout bounds are larger than its preferred size.
	/// </summary>
	public enum FillType
	{
		/// <summary>
		/// The component is not resized.
		/// </summary>
		None,
		/// <summary>
		/// The component's width and height are both changed to fill the 
		/// entire available layout bounds.
		/// </summary>
		Both,
		/// <summary>
		/// The component's width is changed to fill its layout bounds
		/// horizontally.
		/// </summary>
		Horizontal,
		/// <summary>
		/// The component's height is changed to fill its layout bounds
		/// vertically.
		/// </summary>
		Vertical
	};
	/// <summary>
	/// Specifies how a layout component anchors to the edges of its layout bounds when managed
	/// by a <see cref="GridBagLayout"/>.
	/// </summary>
	/// <remarks>
	/// A child component usually gets more space than its preferred size when being laid out.
	/// These values specify how to align such components within the layout bounds.
	/// </remarks>
	public enum AnchorTypes
	{
		/// <summary>
		/// The child component is anchored to the center of its layout bounds.
		/// </summary>
		Center,
		/// <summary>
		/// The child component is anchored to the North of its layout bounds.
		/// </summary>
		North,
		/// <summary>
		/// The child component is anchored to the NorthEast of its layout bounds.
		/// </summary>
		NorthEast,
		/// <summary>
		/// The child component is anchored to the East of its layout bounds.
		/// </summary>
		East,
		/// <summary>
		/// The child component is anchored to the SouthEast of its layout bounds.
		/// </summary>
		SouthEast,
		/// <summary>
		/// The child component is anchored to the South of its layout bounds.
		/// </summary>
		South,
		/// <summary>
		/// The child component is anchored to the SouthWest of its layout bounds.
		/// </summary>
		SouthWest,
		/// <summary>
		/// The child component is anchored to the West of its layout bounds.
		/// </summary>
		West,
		/// <summary>
		/// The child component is anchored to the NorthWest of its layout bounds.
		/// </summary>
		NorthWest
	};

	/// <summary>
	/// Specifies how components will be positioned inside a container managed
	/// by the <see cref="GridBagLayout"/> manager.
	/// </summary>
	/// <remarks>
	/// <para>GridBagConstraints are a set of properties that determine how a 
	/// component will grow, shrink or reposition itself when its container is resized. </para>
	/// <para>Each component has its own GridBagConstraints which means that there is potential for
	/// unforseen size and boundary conflicts. Make sure to manually test the layout design
	/// to determine that it behaves appropriately.</para>
	/// </remarks>
	[
	TypeConverter(typeof(GridBagConstraintsConverter)),
	Serializable()
	]
	public class GridBagConstraints : ICloneable
	{
		/// <summary>
		/// Relative positioning preference.
		/// </summary>
		public const int Relative = -1;
		/// <summary>
		/// Makes the child component occupy the remainder of the row / column.
		/// </summary>
		public const int Remainder = 0;
	
		internal int gridPosX;
		internal int gridPosY;
		internal int cellSpanX;
		internal int cellSpanY;
		internal double weightX;
		internal double weightY;
		internal AnchorTypes anchor;
		internal FillType fill;
		internal Insets insets;
		internal int ipadX;
		internal int ipadY;
		internal bool isEmpty;

		[NonSerialized()]
		internal int tempCurPosX, tempCurPosY;
		[NonSerialized()]
		internal int tempCellSpanX, tempCellSpanY;
		[NonSerialized()]
		internal int minWidth, minHeight;
		
		public static readonly GridBagConstraints Empty;
		static GridBagConstraints()
		{
			Empty = Default();
			Empty.IsEmpty = true;
		}

		/// <summary>
		/// Returns a default GridBagConstraint object that is also empty.
		/// </summary>
		/// <returns>The default GridBagConstraints object.</returns>
		public static GridBagConstraints Default(){return new GridBagConstraints();}

		/// <summary>
		/// Overloaded. Creates a new instance of the GridBagConstraints class and sets its defaults.
		/// </summary>
		public GridBagConstraints () 
			:this(Relative, Relative, 1, 1, 0, 0, AnchorTypes.Center, FillType.None,
		new Insets(0,0,0,0), 0, 0, false)
		{
			
		}

		/// <summary>
		/// Creates a new instance of the GridBagConstraints class 
		/// with the specified values.
		/// </summary>
		public GridBagConstraints (int gridPosX, int gridPosY, int cellSpanX, int cellSpanY,
			double weightX, double weightY, AnchorTypes anchor, FillType fill, Insets insets, int ipadX, int ipadY, bool isEmpty) 
		{
			this.gridPosX = gridPosX;
			this.gridPosY = gridPosY;
			this.cellSpanX = cellSpanX;
			this.cellSpanY = cellSpanY;

			this.weightX = weightX;
			this.weightY = weightY;
			this.anchor = anchor;
			this.fill = fill;

			this.insets = insets;
			this.ipadX = ipadX;
			this.ipadY = ipadY;
			this.isEmpty = isEmpty;
		}
		/// <summary>
		/// Gets / sets the column in the virtual grid where the component's
		/// layout bounds begin.
		/// </summary>
		/// <value>A value specifying the beginning column. Can be -1.
		/// Default is -1.</value>
		/// <remarks>
		/// When -1, the positioning is relative, which means the component will
		/// be positioned immediately to the right of the component that was
		/// most recently added to the container.
		/// </remarks>
		[DefaultValue(Relative),
		Description("Specifies the column in the virtual grid where the component's layout bounds begin.")
		]
		public int GridPosX
		{
			get
			{
				return this.gridPosX;
			}
			set
			{
				if( value != this.	gridPosX )
				{
					gridPosX = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the row in the virtual grid where the component's
		/// layout bounds begin.
		/// </summary>
		/// <value>A value specifying the beginning row. Can be -1.
		/// Default is -1(GridBagConstraints.Relative).</value>
		/// <remarks>
		/// When -1, the positioning is relative, which means the component will
		/// be positioned immediately to the bottom of the component that was
		/// most recently added to the container.
		/// </remarks>
		[DefaultValue(Relative),
		Description("Specifies the row in the virtual grid where the component's layout bounds begin.")
		]
		public int GridPosY
		{
			get
			{
				return this.gridPosY;
			}
			set
			{
				if( value != this.gridPosY )
				{
					this.gridPosY = value;
					OnPropertyChanged();
				}				
			}
		}
		/// <summary>
		/// Gets / sets the number of columns this component should span in the
		/// virtual grid.
		/// </summary>
		/// <value>The number of columns to span, default is 1. Can be zero (GridBagConstraints.Remainder).</value>
		/// <remarks>
		/// When value is zero, the component will be the last one in its row. 
		/// </remarks>
		[DefaultValue(1),
		Description("Specifies the number of columns this component should span in the virtual grid.")
		]
		public int CellSpanX
		{
			get
			{
				return this.cellSpanX;
			}
			set
			{
				if( value != this.cellSpanX )
				{
					this.cellSpanX = value;
					OnPropertyChanged();
				}				
			}
		}
		/// <summary>
		/// Gets / sets the number of rows this component should span in the
		/// virtual grid.
		/// </summary>
		/// <value>The number of rows to span, default is 1. Can be zero (GridBagConstraints.Remainder).</value>
		/// <remarks>
		/// When value is zero, the component will be the last one in its column. 
		/// </remarks>
		[DefaultValue(1),
		Description("Specifies the number of rows this component should span in the virtual grid.")
		]
		public int CellSpanY
		{
			get
			{
				return this.cellSpanY;
			}
			set
			{
				if( value != this.cellSpanY )
				{
					this.cellSpanY = value;
					OnPropertyChanged();
				}				
			}
		}
		/// <summary>
		/// Gets / sets the weight of this component in obtaining the extra
		/// horizontal space.
		/// </summary>
		/// <value>A double value representing the weight. Default is zero.</value>
		/// <remarks>
		/// <para>Specifies how to distribute extra horizontal space for a column.
		/// The weight of a column is calculated as the maximum WeightX of all 
		/// the components in that column.</para>
		/// <para>When there is extra horizontal space it is distributed to each column
		/// based on its weight. A component that has zero weight receives no
		/// extra space.</para>
		/// </remarks>
		[DefaultValue((double)0),
		Description("Specifies the weight of this component in obtaining the extra horizontal space.")]
		public double WeightX
		{
			get
			{
				return this.weightX;
			}
			set
			{
				if( value != this.weightX )
				{
					this.weightX = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the weight of this component in obtaining the extra
		/// vertical space.
		/// </summary>
		/// <value>A double value representing the weight. Default is zero.</value>
		/// <remarks>
		/// <para>Specifies how to distribute extra vertical space for a row.
		/// The weight of a row is calculated as the maximum WeightY of all 
		/// the components in that row.</para>
		/// <para>When there is extra vertical space it is distributed to each row
		/// based on its weight. A component that has zero weight receives no
		/// extra space.</para>
		/// </remarks>
		[DefaultValue((double)0),
		Description("Specifies the weight of this component in obtaining the extra vertical space.")]
		public double WeightY
		{
			get
			{
				return this.weightY;
			}
			set
			{
				if( value != this.weightY )
				{
					this.weightY = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the justification of a component within its available layout
		/// bounds (a cell in the virtual grid).
		/// </summary>
		/// <value>
		/// One of the AnchorTypes values. Default is center.
		/// </value>
		[DefaultValue(AnchorTypes.Center),
		Description("Specifies the justification of a component within its available layout bounds (a cell in the virtual grid).")
		]
		public AnchorTypes Anchor
		{
			get
			{
				return this.anchor;
			}
			set
			{
				if( value != this.anchor )
				{
					this.anchor = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the fill type to resize a component when the component's
		/// layout bounds are larger than its preferred size.
		/// </summary>
		/// <value>One of the FillType values. The default is none.</value>
		[DefaultValue(FillType.None),
		Description("Specifies whether (and how) to resize a component when the component's layout bounds are larger than its preferred size.")
		]
		public FillType Fill
		{
			get
			{
				return this.fill;
			}
			set
			{
				if( value != this.fill )
				{
					this.fill = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the extra space that the manager adds around a component's preferred bounds before
		/// laying out the component.
		/// </summary>
		/// <value>An Inset's instance. Default is zero inset padding on all sides.</value>
		[Description("Specifies the extra space that the manager adds around a component's preferred bounds before laying out the component.")]
		public Insets Insets
		{
			get
			{
				return this.insets;
			}
			set
			{
				if( value != this.insets )
				{
					this.insets = value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the amount in pixels to add to the size of the component
		/// when determining its overall width.
		/// </summary>
		/// <value>The integer value representing the padding in pixels.
		/// Default is zero.</value>
		[DefaultValue(0),
		Description("Specifies the amount in pixels to add to the size of the component when determining its overall width.")]
		public int IpadX
		{
			get
			{
				return this.ipadX;
			}
			set
			{
				if( value != this.ipadX )
				{
					this.ipadX = ( value < 0 ) ? 0 : value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the amount in pixels to add to the size of the component
		/// when determining its overall height.
		/// </summary>
		/// <value>The integer value representing the padding in pixels.
		/// Default is zero.</value>
		[DefaultValue(0),
		Description("Specifies the amount in pixels to add to the size of the component when determining its overall height.")]
		public int IpadY
		{
			get
			{
				return this.ipadY;
			}
			set
			{
				if( value != this.ipadY )
				{
					this.ipadY = (value < 0) ? 0 : value;
					OnPropertyChanged();
				}
			}
		}
		/// <summary>
		/// Gets / sets the GridBagConstraints structure with its properties left uninitialized.
		/// </summary>
		[DefaultValue(false),
		Description("Represents the GridBagConstraints structure with its properties left uninitialized.")]
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
			set
			{
				if( value != this.isEmpty )
				{
					this.isEmpty = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Creates an exact copy of this GridBagConstraints object.
		/// </summary>
		/// <returns>The cloned object.</returns>
		public object Clone () 
		{
			GridBagConstraints c = (GridBagConstraints)this.MemberwiseClone();
			return c;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			bool eq = false;

			if ((object)this == o)
				return true;
		                    
			if ( o is GridBagConstraints )
			{
				GridBagConstraints glc = (GridBagConstraints)o;
				if(this.Anchor != glc.Anchor)
					return false;
				if(this.CellSpanX != glc.CellSpanX)
					return false;
				if(this.CellSpanY != glc.CellSpanY)
					return false;
				if(this.Fill != glc.Fill)
					return false;
				if(this.GridPosX != glc.GridPosX)
					return false;
				if(this.GridPosY != glc.GridPosY)
					return false;
				if(this.Insets != glc.Insets)
					return false;
				if(this.IpadX != glc.IpadX)
					return false;
				if(this.IpadY != glc.IpadY)
					return false;
				if(this.WeightX != glc.WeightX)
					return false;
				if(this.WeightY != glc.WeightY)
					return false;
				if(this.IsEmpty != glc.IsEmpty)
					return false;
				else
					return true;
			}
			return eq;
		}

		/// <summary cref="Equals">
		///		The basic == operator.
		/// </summary>
		/// <param name="lhs">The left-hand side of the operator.</param>
		/// <param name="rhs">The right-hand side of the operator.</param>
		/// <returns>
		///		Boolean value.
		///	</returns>
		public static bool operator==( GridBagConstraints lhs, GridBagConstraints rhs ) 
		{                            
			if((object)lhs == null && (object)rhs == null)
				return true;

			if ((object) lhs == null || (object) rhs == null)
				return false;
			return lhs.Equals(rhs);
		}

		/// <summary cref="Equals">
		///		The basic != operator
		/// </summary>
		/// <param name="lhs">The left-hand side of the operator.</param>
		/// <param name="rhs">The right-hand side of the operator.</param>
		/// <returns>
		///		Boolean value.
		///	</returns>
		public static bool operator!=( GridBagConstraints lhs, GridBagConstraints rhs ) 
		{          
			if((object)lhs == null && (object)rhs == null)
				return false;

			if ((object) lhs == null || (object) rhs == null)
				return true;
			return !lhs.Equals(rhs);
		}


		public event EventHandler PropertyChanged;

		protected virtual void OnPropertyChanged()
		{
			if( PropertyChanged != null )
			{
				PropertyChanged( this, EventArgs.Empty );
			}
		}
	}

	[Serializable,
	Syncfusion.Documentation.DocumentationExclude()]
	public class GridBagLayoutInfo 
	{
		public int columns, rows;		
		public int startX, startY;	/* beginning layout position */
		public int[] minWidth;		/* largest minimum width in each column */
		public int[] minHeight;		/* largest minimum height in each row */
		public double[] weightX;	/* largest weight in each column */
		public double[] weightY;	/* largest weight in each row */

		public GridBagLayoutInfo () 
		{
			minWidth = new int[GridBagLayout.MaxGridSize];
			minHeight = new int[GridBagLayout.MaxGridSize];
			weightX = new double[GridBagLayout.MaxGridSize];
			weightY = new double[GridBagLayout.MaxGridSize];
		}
	}

	/// <summary>
	/// Represents the layout manager that performs GridBag layout.
	/// </summary>
	/// <remarks>
	/// <para>The GridBagLayout manager creates and manages a grid within the
	/// container's layout bounds. Each column and row in this grid can be
	/// of different sizes and a component can span more than one cell.</para>
	/// <para>Each component can be assigned weights to determine how to distribute
	/// extra space between components. A component can also be anchored to a border / corner
	/// of its virtual grid cell. It can also fill its cell.</para>
	/// <para>
	/// Note that if you want the child components to be laid out purely based on their weights,
	/// set their preferred sizes to be (0,0), specify a non-zero weightX and weigthY,
	/// and set their Fill mode to FillMode.Both.
	/// </para>
	/// <para>The GridBagConstraints structure specifies all the above constraints associated
	/// with a component. Use the SetConstraints method to set the constraints for each component.
	/// To exclude a component from layout, call SetConstraints with NULL constraints.
	/// Each component has a constraint, which means that there is potential
	/// for conflicts in size and boundaries between components. Make
	/// sure to manually test the layout design to determine that it
	/// behaves appropriately.</para>
	/// </remarks>
	/// <example>
	/// The following example shows you how to initialize a GridBagLayout manager with a container control and its children:
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\cs\GridBagLayoutForm.cs" name="Initializing GridBagLayout" lang="C#"><code lang="C#">
	///             // Binding a control to the GridBagLayout manager programmatically:
	///             this.gridBagLayout1 = new GridBagLayout();
	/// 
	///             // Set the container control; all the child controls of this container control are
	///             // automatically registered as children with the manager:
	///             this.gridBagLayout1.ContainerControl = this.panel1;
	/// 
	///             this.gridBagLayout1.SetConstraints(
	///                 this.button1,
	///                 new GridBagConstraints(0, 0, 3, 1, 1, 0.2, AnchorTypes.Center, FillType.Both, new Insets(0, 0, 0, 0), 0, 0, false)
	///                 );
	///             this.gridBagLayout1.SetConstraints(
	///                 this.button2,
	///                 new GridBagConstraints(0, 1, 1, 3, 0.2, 0.6, AnchorTypes.Center, FillType.Both, new Insets(0, 0, 0, 0), 0, 0, false)
	///                 );
	/// 
	///             // Exclude button3 from layout:
	///             this.gridBagLayout1.SetConstraints(this.button3, GridBagConstraints.Empty);
	/// 
	///             // Modify an exisiting constraint:
	///             GridBagConstraints constraints1 = this.gridBagLayout1.GetConstraintsRef(this.button1);
	///             constraints1.Fill = FillType.Horizontal;
	/// 
	///             // You can prevent automatic layout during the layout event.
	///             // If you decide to do so, make sure to call gridBagLayout1.LayoutContainer manually:
	///             // this.gridBagLayout1.AutoLayout = false;</code></coderef>
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\VB\GridBagLayoutForm.vb" name="Initializing GridBagLayout" lang="VB"><code lang="VB">
	///            ' Binding a Control to the GridBagLayout manager programmatically:
	///            Me.gridBagLayout1 = New GridBagLayout
	///            ' Set the target control; all the child controls of this target control are
	///            ' automatically registered as children with the manager:
	///            Me.gridBagLayout1.ContainerControl = Me.panel1
	///            Me.gridBagLayout1.SetConstraints(Me.button1, New GridBagConstraints(0, 0, 3, 1, 1, 0.2, AnchorTypes.Center, FillType.Both, New Insets(0, 0, 0, 0), 0, 0, false))
	///            Me.gridBagLayout1.SetConstraints(Me.button2, New GridBagConstraints(0, 1, 1, 3, 0.2, 0.6, AnchorTypes.Center, FillType.Both, New Insets(0, 0, 0, 0), 0, 0, false))
	///            ' Exclude button3 from layout:
	///            Me.gridBagLayout1.SetConstraints(Me.button3, GridBagConstraints.Empty)
	///            ' Modify an exisiting constraint:
	///            Dim constraints1 As GridBagConstraints
	///            constraints1 = Me.gridBagLayout1.GetConstraintsRef(Me.button1)
	///            constraints1.Fill = FillType.Horizontal
	///            ' You can prevent automatic layout during the layout event.
	///            ' If you decide to do so, make sure to call gridBagLayout1.LayoutContainer manually:
	///            ' this.gridBagLayout1.AutoLayout = false;</code></coderef>
	/// <para>Also take a look at the project in Tools/Samples/Quick Start/LayoutManagers for an example.</para>
	/// </example>
	[
		ProvideProperty("Constraints", typeof(Control)),
		System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.GridBagLayout.bmp"),
	ToolboxItemFilter("System.Windows.Forms"),
	Description("Represents the layout manager that arranges control in grid with different size in rows and columns.")
	]
	public class GridBagLayout : LayoutManager
	{
		protected static int PreferredSize = 1;
		protected static int MinimumSize = 2;

		public const int MaxGridSize = 512;
		public const int MinSize = 1;
		
		private Hashtable controlsMap;
		protected Hashtable noFillSizes;
		private GridBagLayoutInfo layoutInfo;

		/// <summary>
		/// Largest minimum width in each column.
		/// </summary>
		public int[] columnWidths;

		/// <summary>
		/// Largest minimum height in each row.
		/// </summary>
		public int[] rowHeights;

		/// <summary>
		/// Largest weight in each column.
		/// </summary>
		public double[] columnWeights;

		/// <summary>
		/// Largest weight in each row.
		/// </summary>
		public double[] rowWeights;

		/// <summary>
		/// Overloaded. Creates a new instance of the GridBagLayout class and sets its defaults.
		/// </summary>
		public GridBagLayout () 
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridBagLayout));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			controlsMap = new Hashtable();
			noFillSizes = new Hashtable();
		}
		/// <summary>
		/// Creates a new instance of the GridBagLayout class and adds itself to the specified container.
		/// </summary>
		/// <param name="container">The logical ContainerControl parent into which to add itself.</param>
		/// <remarks><para>This constructor is used by the design-time to add a component to the form's
		/// IContainer field so that it gets Disposed when the form gets Disposed.</para>
		/// <para>Note that this is not the same as the layout manager's ContainerControl.</para></remarks>
		public GridBagLayout(IContainer container)
			: this()
		{
			if(container != null)
				container.Add(this);
		}
		/// <summary>
		/// Creates a new instance of the GridBagLayout class and sets its ContainerControl.
		/// </summary>
		public GridBagLayout (Control container) 
			:this()
		{
			ContainerControl = container;
		}
		/// <override/>
		protected override Size GetStaticPreferredSize(Control control)
		{
			if(preferredSizes[control] == null)
			{
				if(this.noFillSizes[control] != null)
					return (Size)this.noFillSizes[control];
				else
				{
					Size sz = control.Size;
					GridBagConstraints gbc = this.GetConstraintsRef(control);
					if(gbc.IpadX > 0)
						sz.Width -= gbc.IpadX;
					if(gbc.IpadY > 0)
						sz.Height -= gbc.IpadY;

					return sz;
				}
			}
			else
				return (Size)preferredSizes[control];
		}

		/// <override/>
		protected override Size GetStaticMinimumSize(Control control)
		{
			if(minimumSizes[control] == null)
			{
				if(this.noFillSizes[control] != null)
					return (Size)this.noFillSizes[control];
				else
				{
					Size sz = control.Size;
					GridBagConstraints gbc = this.GetConstraintsRef(control);
					if(gbc.IpadX > 0)
						sz.Width -= gbc.IpadX;
					if(gbc.IpadY > 0)
						sz.Height -= gbc.IpadY;

					return sz;
				}
			}
			else
				return (Size)minimumSizes[control];
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.ResetPreferredSize"/>.
		/// </summary>
		/// <param name="control"></param>
		public override void ResetPreferredSize(Control control)
		{
			if(this.noFillSizes[control] != null)
				MessageBox.Show("Cannot Reset preferred size when the control has a FillType other than FilleMode.None set.");
			else
				this.preferredSizes[control] = null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ShouldSerializePreferredSize(Control control)
		{
			return (base.ShouldSerializePreferredSize(control) 
				|| this.noFillSizes[control] != null);
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.ResetMinimumSize"/>.
		/// </summary>
		/// <param name="control"></param>
		public override void ResetMinimumSize(Control control)
		{
			if(this.noFillSizes[control] != null)
				MessageBox.Show("Cannot Reset minimum size when the control has a FillType other than FillMode.None set.");
			else
				this.minimumSizes[control] = null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ShouldSerializeMinimumSize(Control control)
		{
			return (base.ShouldSerializeMinimumSize(control) 
				|| this.noFillSizes[control] != null);
		}

		/// <summary>
		/// Specifies the constraints associated with the specified control.
		/// </summary>
		/// <param name="control">The control for which to set the constraints.</param>
		/// <param name="value">The constraints of the control. Or NULL to remove the control
		/// from the layout list.</param>
		/// <remarks>
		/// Passing a NULL value will actually remove the component from the layout list.
		/// </remarks>
		public void SetConstraints(Control control, GridBagConstraints value) 
		{
			if(value == null)
			{
				controlsMap.Remove(control);
				base.RemoveLayoutComponent(control);
			}
			else
			{
				GridBagConstraints oldConstraints = controlsMap[ control ] as GridBagConstraints;
				if( oldConstraints != null )
				{
					oldConstraints.PropertyChanged -= new EventHandler( OnConstraintsChanged );
				}

				UpdateNoFillSizes(control, value );
				UpdatePreferredSizeForPadding(control, value);

				GridBagConstraints newValue = value.Clone() as GridBagConstraints;
				controlsMap[control] = newValue;
				base.AddLayoutComponent(control, value);

				newValue.PropertyChanged += new EventHandler( OnConstraintsChanged );

				if( this.ContainerControl != null )
					this.ContainerControl.PerformLayout();
			}
		}

		private void OnConstraintsChanged( object sender, EventArgs e )
		{
			LayoutContainer();
		}


		protected bool ShouldSerializeConstraints(Control control)
		{
			if(this.GetConstraintsRef(control) == GridBagConstraints.Default())
				return false;
			else
				return true;
		}

		protected void ResetConstraints(Control control)
		{
			this.SetConstraints(control, GridBagConstraints.Default());
		}

		private void UpdatePreferredSizeForPadding(Control control, GridBagConstraints newConstraints)
		{
			if(this.DesignMode && this.noFillSizes[control] != null)
			{
				Size noFillSize = (Size)this.noFillSizes[control];
				Size newNoFillSize = noFillSize;
				GridBagConstraints oldConstraints = (GridBagConstraints)controlsMap[control];
				if(oldConstraints == null)
					oldConstraints = GridBagConstraints.Default();

				if(oldConstraints.IpadX != newConstraints.IpadX)
					newNoFillSize.Width -= (newConstraints.IpadX - oldConstraints.IpadX);

				if(oldConstraints.IpadY != newConstraints.IpadY)
					newNoFillSize.Height -= (newConstraints.IpadY - oldConstraints.IpadY);

				if(newNoFillSize != noFillSize)
					this.noFillSizes[control] = newNoFillSize;
			}
		}

		private void UpdateNoFillSizes(Control control, GridBagConstraints newConstraints)
		{
			if(this.DesignMode)
			{
				GridBagConstraints oldConstraints = (GridBagConstraints)controlsMap[control];
				// If going from noFill to Fill store the current sizes.
				if(newConstraints != null && newConstraints.Fill != FillType.None
					&& (oldConstraints == null || oldConstraints.Fill == FillType.None))
				{
					this.noFillSizes[control] = control.Size;
				}
					// If going from Fill to noFill reset the size of the Control
				else if((newConstraints == null || newConstraints.Fill == FillType.None)
					&& oldConstraints != null && oldConstraints.Fill != FillType.None && this.noFillSizes[control] != null)
				{
					control.SuspendLayout();
					this.ContainerControl.SuspendLayout();

					if(this.preferredSizes[control] == this.noFillSizes[control])
						this.preferredSizes[control] = null;

					if(this.minimumSizes[control] == this.noFillSizes[control])
						this.minimumSizes[control] = null;

					control.Size = (Size)this.noFillSizes[control];

					this.ContainerControl.ResumeLayout(false);
					control.ResumeLayout(false);
					this.noFillSizes[control] = null;
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.SetPreferredSize"/>.
		/// </summary>
		public override void SetPreferredSize(Control control, Size value)
		{
			base.SetPreferredSize(control, value);
			if(this.DesignMode)
			{
				GridBagConstraints constraints = this.GetConstraintsRef(control);
				if(constraints != null && constraints.Fill != FillType.None)
				{
					// The layout is being deserialized from code.
					// Let us set the noFillSizes, assuming the preferred size was just the
					// previous noFillSize.
					this.noFillSizes[control] = value;
				}
			}
		}

		/// <summary>
		/// Returns the constraints associated with the specified control.
		/// </summary>
		/// <param name="control">The control whose constraints to retrieve.</param>
		/// <returns>A clone of the stored constraints object.</returns>
		/// <remarks>
		/// The returned value is a clone which can be used independently by itself.
		/// The changes made to the returned instance will not have any effect on the stored
		/// constraints. Use <see cref="GetConstraintsRef"/> to get hold of the actual constraints object
		/// that is used by the manager.
		/// </remarks>
		[
		Category("Layout Manager"),
		Localizable(true)
		]
		public GridBagConstraints GetConstraints(Control control) 
		{
			return (GridBagConstraints)GetConstraintsRef(control);
		}

		/// <summary>
		/// Returns a reference to the constraints associated with the specified control.
		/// </summary>
		/// <param name="control">The control with constraints to retrieve.</param>
		/// <returns>A reference to the actual constraints object.</returns>
		/// <remarks>This is the actual object where the manager stores the constraints for 
		/// the control. Hence, making changes to the returned object will affect the 
		/// layout logic.</remarks>
		public GridBagConstraints GetConstraintsRef(Control control) 
		{
			GridBagConstraints constraints = (GridBagConstraints)controlsMap[control];
			if (constraints == null) 
			{
				SetConstraints(control, GridBagConstraints.Default());
				constraints = (GridBagConstraints)controlsMap[control];
			}
			return constraints;
		}

		/// <summary>
		/// Returns the top-left origin of the virtual grid in the current layout.
		/// </summary>
		/// <returns>A point representing the top-left position.</returns>
		public Point GetLayoutOrigin () 
		{
			Point origin = new Point(0,0);
			if (layoutInfo != null) 
			{
				origin.X= layoutInfo.startX;
				origin.Y = layoutInfo.startY;
			}
			return origin;
		}

		/// <summary>
		/// Returns the row and column dimensions of the current layout.
		/// </summary>
		/// <returns>A 2D integer array containing the dimensions.</returns>
		[CLSCompliant(false)]
		public int[][] GetLayoutDimensions () 
		{
			if (layoutInfo == null)
				return new int[2][];

			int[][] dim = new int [2][];
			dim[0] = new int[layoutInfo.columns];
			dim[1] = new int[layoutInfo.rows];

			System.Array.Copy(layoutInfo.minWidth, dim[0], layoutInfo.columns);
			System.Array.Copy(layoutInfo.minHeight, dim[1], layoutInfo.rows);

			return dim;
		}

		/// <summary>
		/// Returns the row and column weights of the current layout.
		/// </summary>
		/// <returns>A 2D integer array containing the weights.</returns>
		[CLSCompliant(false)]
		public double [][] GetLayoutWeights () 
		{
			if (layoutInfo == null)
				return new double[2][];

			double[][] weights = new double [2][];
			weights[0] = new double[layoutInfo.columns];
			weights[1] = new double[layoutInfo.rows];

			System.Array.Copy(layoutInfo.weightX, weights[0], layoutInfo.columns);
			System.Array.Copy(layoutInfo.weightY, weights[1], layoutInfo.rows);

			return weights;
		}

		/// <summary>
		/// Returns the cell in the virtual grid (as a point) given a location.
		/// </summary>
		/// <param name="x">The x coordinate of the location.</param>
		/// <param name="y">The y coordinate of the location.</param>
		/// <returns>A Point representing the virtual grid cell.</returns>
		public Point GetLocation(int x, int y) 
		{
			Point loc = new Point(0,0);
			int i, d;

			if (layoutInfo == null)
				return loc;

			d = layoutInfo.startX;
			for (i=0; i<layoutInfo.columns; i++) 
			{
				d += layoutInfo.minWidth[i];
				if (d > x)
					break;
			}
			loc.X= i;

			d = layoutInfo.startY;
			for (i=0; i<layoutInfo.rows; i++) 
			{
				d += layoutInfo.minHeight[i];
				if (d > y)
					break;
			}
			loc.Y = i;

			return loc;
		}

		/// <summary>
		/// Adds a component to the child layout list.
		/// </summary>
		/// <param name="control">The control to add to the layout list.</param>
		/// <param name="constraints">The GridBagConstraints associated with this component.</param>
		/// <remarks>If the second argument is not a GridBagConstraints, an exception will be thrown.</remarks>
		public override void AddLayoutComponent(Control control, Object constraints) 
		{
			if (constraints is GridBagConstraints) 
				SetConstraints(control, (GridBagConstraints)constraints);
			else if (constraints != null) 
			  throw new ArgumentException("Invalid GridBag constraints. Constraints must be of type GridBagConstraints");
		}

		/// <override/>
		protected override void OnControlAdded(object sender, ControlEventArgs e)
		{
			// Don't have to call LayoutContainer, since this is usually followed by the Layout event.
			Control control = e.Control;
			if(!this.controlsMap.Contains(control))
				this.SetConstraints(control, GridBagConstraints.Default());
		
			base.OnControlAdded(sender, e);
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.RemoveLayoutComponent"/>.
		/// </summary>
		public override void RemoveLayoutComponent(Control control) 
		{
			SetConstraints(control, null);
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.PreferredLayoutSize"/>.
		/// </summary>
		public override Size PreferredLayoutSize() 
		{
			GridBagLayoutInfo info = GetLayoutInfo(PreferredSize);
			return GetMinSize(info);
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.MinimumLayoutSize"/>.
		/// </summary>
		public override Size MinimumLayoutSize() 
		{
			GridBagLayoutInfo info = GetLayoutInfo(MinSize);
			return GetMinSize(info);
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.ResetLayoutInfo"/>.
		/// </summary>
		protected override void ResetLayoutInfo()
		{
			base.ResetLayoutInfo();
			this.noFillSizes.Clear();
			return;
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.LayoutContainer"/>.
		/// </summary>
		public override void LayoutContainer() 
		{
			if(!IsInit())
				return;

			if( !this.initializing )
			{
				this.ContainerControl.SuspendLayout();
				LayoutGridBag();
				this.ContainerControl.ResumeLayout(false);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected GridBagLayoutInfo GetLayoutInfo(int sizeflag) 
		{
			Monitor.Enter(this);
	
			int i, k, px, py, pixels_diff, nextSize;
			int curPosX, curPosY, curCellSpanX, curCellSpanY, curRow, curCol;
			double weight_diff, weight;

			/* 
			*	First Pass
			*
			*	Figure out the dimensions of the layout grid. Using a value of 1 for
			*	zero or negative widths and heights.
			*/

			GridBagLayoutInfo layoutInfo = new GridBagLayoutInfo();
			layoutInfo.columns = layoutInfo.rows = 0;
			curRow = curCol = -1;
			int[] xMax = new int[MaxGridSize];
			int[] yMax = new int[MaxGridSize];

			GridBagConstraints constraints;
			Control control;
			IList controls = this.GetControls();
			int controlIndex;
			for (controlIndex = 0 ; controlIndex < controls.Count ; controlIndex++) 
			{
				control = controls[controlIndex] as Control;
				if (!this.IsVisible(control))
					continue;
				constraints = GetConstraintsRef(control);
				if(constraints.IsEmpty)
					continue;

				curPosX = constraints.gridPosX;
				curPosY = constraints.gridPosY;
				// Get Cell Spans, (cannot be 0 or negative)
				curCellSpanX = constraints.cellSpanX;
				if (curCellSpanX <= 0)
					curCellSpanX = 1;
				curCellSpanY = constraints.cellSpanY;
				if (curCellSpanY <= 0)
					curCellSpanY = 1;

				// If x or y is negative, then use relative positioning.
				if (curPosX < 0 && curPosY < 0) 
				{
					if (curRow >= 0)
						curPosY = curRow;
					else if (curCol >= 0)
						curPosX = curCol;
					else
						curPosY = 0;
				}
				if (curPosX < 0) 
				{
					px = 0;
					for (i = curPosY; i < (curPosY + curCellSpanY); i++)
						px = Math.Max(px, xMax[i]);

					curPosX = px - curPosX - 1;
					if(curPosX < 0)
						curPosX = 0;
				}
				else if (curPosY < 0) 
				{
					py = 0;
					for (i = curPosX; i < (curPosX + curCellSpanX); i++)
						py = Math.Max(py, yMax[i]);

					curPosY = py - curPosY - 1;
					if(curPosY < 0)
						curPosY = 0;
				}

				// Adjust and ensure sufficient grid width and height.
				for (px = curPosX + curCellSpanX; layoutInfo.columns < px; layoutInfo.columns++);
				for (py = curPosY + curCellSpanY; layoutInfo.rows < py; layoutInfo.rows++);

				// Adjust the xMax and yMax arrays
				for (i = curPosX; i < (curPosX + curCellSpanX); i++) { yMax[i] = py; }
				for (i = curPosY; i < (curPosY + curCellSpanY); i++) { xMax[i] = px; }

				/* Cache the current control's size. */
				Size size;
				if (sizeflag == PreferredSize)
					size = GetPreferredSize(control);
				else
					size = GetMinimumSize(control);
				constraints.minWidth = size.Width;
				constraints.minHeight = size.Height;

				/* Zero width and height must mean that this is the last item (or
					* else something is wrong). */
				if (constraints.cellSpanY == 0 && constraints.cellSpanX == 0)
					curRow = curCol = -1;

				/* Zero width starts a new row */
				if (constraints.cellSpanY == 0 && curRow < 0)
					curCol = curPosX + curCellSpanX;

				/* Zero height starts a new column */
				else if (constraints.cellSpanX == 0 && curCol < 0)
					curRow = curPosY + curCellSpanY;
			}

			/*
				* Apply minimum row / column dimensions.
				*/
			if (columnWidths != null && layoutInfo.columns < columnWidths.Length)
				layoutInfo.columns = columnWidths.Length;
			if (rowHeights != null && layoutInfo.rows < rowHeights.Length)
				layoutInfo.rows = rowHeights.Length;

			/*
			* Second Pass
			*
			* Negative values for gridX are filled in with the current x value.
			* Negative values for gridY are filled in with the current y value.
			* Negative or zero values for gridWidth and gridHeight end the current
			*  row or column, respectively.
			*/

			curRow = curCol = -1;
			xMax = new int[MaxGridSize];
			yMax = new int[MaxGridSize];

			for (controlIndex = 0 ; controlIndex < controls.Count ; controlIndex++) 
			{
				control = controls[controlIndex]as Control;
				if (!this.IsVisible(control))
					continue;
				constraints = GetConstraintsRef(control);
				if(constraints.IsEmpty)
					continue;

				curPosX = constraints.gridPosX;
				curPosY = constraints.gridPosY;
				curCellSpanX = constraints.cellSpanX;
				curCellSpanY = constraints.cellSpanY;

				/* If x or y is negative, then use relative positioning: */
				if (curPosX < 0 && curPosY < 0) 
				{
					if(curRow >= 0)
						curPosY = curRow;
					else if(curCol >= 0)
						curPosX = curCol;
					else
						curPosY = 0;
				}

				if (curPosX < 0) 
				{
					if (curCellSpanY <= 0) 
					{
						curCellSpanY += layoutInfo.rows - curPosY;
						if (curCellSpanY < 1)
							curCellSpanY = 1;
					}

					px = 0;
					for (i = curPosY; i < (curPosY + curCellSpanY); i++)
						px = Math.Max(px, xMax[i]);

					curPosX = px - curPosX - 1;
					if(curPosX < 0)
						curPosX = 0;
				}
				else if (curPosY < 0) 
				{
					if (curCellSpanX <= 0) 
					{
						curCellSpanX += layoutInfo.columns - curPosX;
						if (curCellSpanX < 1)
							curCellSpanX = 1;
					}

					py = 0;
					for (i = curPosX; i < (curPosX + curCellSpanX); i++)
						py = Math.Max(py, yMax[i]);

					curPosY = py - curPosY - 1;
					if(curPosY < 0)
						curPosY = 0;
				}

				if (curCellSpanX <= 0) 
				{
					curCellSpanX += layoutInfo.columns - curPosX;
					if (curCellSpanX < 1)
						curCellSpanX = 1;
				}

				if (curCellSpanY <= 0) 
				{
					curCellSpanY += layoutInfo.rows - curPosY;
					if (curCellSpanY < 1)
						curCellSpanY = 1;
				}

				px = curPosX + curCellSpanX;
				py = curPosY + curCellSpanY;

				for (i = curPosX; i < (curPosX + curCellSpanX); i++) { yMax[i] = py; }
				for (i = curPosY; i < (curPosY + curCellSpanY); i++) { xMax[i] = px; }

				// Make negative sizes start a new row/column
				if (constraints.cellSpanY == 0 && constraints.cellSpanX == 0)
					curRow = curCol = -1;
				if (constraints.cellSpanY == 0 && curRow < 0)
					curCol = curPosX + curCellSpanX;
				else if (constraints.cellSpanX == 0 && curCol < 0)
					curRow = curPosY + curCellSpanY;

				// Assign the new values to the gridbag constraints.
				constraints.tempCurPosX = curPosX;
				constraints.tempCurPosY = curPosY;
				constraints.tempCellSpanX = curCellSpanX;
				constraints.tempCellSpanY = curCellSpanY;
			}

			// Apply minimum row / column dimensions and weights.
			if (columnWidths != null)
				System.Array.Copy(columnWidths, layoutInfo.minWidth, columnWidths.Length);
			if (rowHeights != null)
				System.Array.Copy(rowHeights, layoutInfo.minHeight, rowHeights.Length);
			if (columnWeights != null)
				System.Array.Copy(columnWeights, layoutInfo.weightX, columnWeights.Length);
			if (rowWeights != null)
				System.Array.Copy(rowWeights, layoutInfo.weightY, rowWeights.Length);

			/*
				* Third Pass
				*
				* Distribute the minimun widths and weights:
			*/

			nextSize = Int32.MaxValue;

			for (i = 1;
				i != Int32.MaxValue;
				i = nextSize, nextSize = Int32.MaxValue) 
			{
				for (controlIndex = 0 ; controlIndex < controls.Count ; controlIndex++) 
				{
					control = controls[controlIndex]as Control;
					if (!this.IsVisible(control))
						continue;
					constraints = GetConstraintsRef(control);
					if(constraints.IsEmpty)
						continue;

					if (constraints.tempCellSpanX == i) 
					{
						px = constraints.tempCurPosX + constraints.tempCellSpanX; /* right column */

						/* 
							* Check if we should use this child's weight. If the weight
							* is less than the total weight spanned by the width of the cell,
							* then discard the weight. Otherwise split the difference
							* according to the existing weights.
						*/

						weight_diff = constraints.weightX;
						for (k = constraints.tempCurPosX; k < px; k++)
							weight_diff -= layoutInfo.weightX[k];
						if (weight_diff > 0.0) 
						{
							weight = 0.0;
							for (k = constraints.tempCurPosX; k < px; k++)
								weight += layoutInfo.weightX[k];
							for (k = constraints.tempCurPosX; weight > 0.0 && k < px; k++) 
							{
								double wt = layoutInfo.weightX[k];
								double dx = (wt * weight_diff) / weight;
								layoutInfo.weightX[k] += dx;
								weight_diff -= dx;
								weight -= wt;
							}
							// Assign the remainder to the rightmost cell.
							layoutInfo.weightX[px-1] += weight_diff;
						}

						/*
							* Calculate the minWidth array values.
							* First, figure out how wide the current child needs to be.
							* Then, see if it will fit within the current minWidth values.
							* If it will not fit, add the difference according to the
							* weightX array.
						*/
						pixels_diff =
							constraints.minWidth + constraints.ipadX +
							constraints.Insets.Left + constraints.Insets.Right;

						for (k = constraints.tempCurPosX; k < px; k++)
							pixels_diff -= layoutInfo.minWidth[k];
						if (pixels_diff > 0) 
						{
							weight = 0.0;
							for (k = constraints.tempCurPosX; k < px; k++)
								weight += layoutInfo.weightX[k];
							for (k = constraints.tempCurPosX; weight > 0.0 && k < px; k++) 
							{
								double wt = layoutInfo.weightX[k];
								int dx = (int)((wt * ((double)pixels_diff)) / weight);
								layoutInfo.minWidth[k] += dx;
								pixels_diff -= dx;
								weight -= wt;
							}
							// Any leftovers go into the rightmost cell.
							layoutInfo.minWidth[px-1] += pixels_diff;
						}
					}
					else if (constraints.tempCellSpanX > i && constraints.tempCellSpanX < nextSize)
						nextSize = constraints.tempCellSpanX;


					if (constraints.tempCellSpanY == i) 
					{
						py = constraints.tempCurPosY + constraints.tempCellSpanY; /* bottom row */

						/* 
							* Figure out if we should use this child's weight. If the weight
							* is less than the total weight spanned by the height of the cell,
							* then discard the weight. Otherwise split the difference
							* according to the existing weights.
						*/

						weight_diff = constraints.weightY;
						for (k = constraints.tempCurPosY; k < py; k++)
							weight_diff -= layoutInfo.weightY[k];
						if (weight_diff > 0.0) 
						{
							weight = 0.0;
							for (k = constraints.tempCurPosY; k < py; k++)
								weight += layoutInfo.weightY[k];
							for (k = constraints.tempCurPosY; weight > 0.0 && k < py; k++) 
							{
								double wt = layoutInfo.weightY[k];
								double dy = (wt * weight_diff) / weight;
								layoutInfo.weightY[k] += dy;
								weight_diff -= dy;
								weight -= wt;
							}
							/* Assign the remainder to the bottom cell */
							layoutInfo.weightY[py-1] += weight_diff;
						}

						/*
							* Calculate the minHeight array values.
							* First, figure out how tall the current child needs to be.
							* Then, see if it will fit within the current minHeight values.
							* If it will not fit, add the difference according to the
							* weightY array.
						*/
						pixels_diff =
							constraints.minHeight + constraints.ipadY +
							constraints.Insets.Top + constraints.Insets.Bottom;
						for (k = constraints.tempCurPosY; k < py; k++)
							pixels_diff -= layoutInfo.minHeight[k];
						if (pixels_diff > 0) 
						{
							weight = 0.0;
							for (k = constraints.tempCurPosY; k < py; k++)
								weight += layoutInfo.weightY[k];
							for (k = constraints.tempCurPosY; weight > 0.0 && k < py; k++) 
							{
								double wt = layoutInfo.weightY[k];
								int dy = (int)((wt * ((double)pixels_diff)) / weight);
								layoutInfo.minHeight[k] += dy;
								pixels_diff -= dy;
								weight -= wt;
							}
							/* Any leftovers go into the bottom cell */
							layoutInfo.minHeight[py-1] += pixels_diff;
						}
					}
					else if (constraints.tempCellSpanY > i &&
						constraints.tempCellSpanY < nextSize)
						nextSize = constraints.tempCellSpanY;
				}
			}

			Monitor.Exit(this);
			return layoutInfo;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void AdjustForGravity(GridBagConstraints constraints,
			ref Rectangle r) 
		{
			int diffx, diffy;

			r.X+= constraints.Insets.Left;
			r.Width -= (constraints.Insets.Left + constraints.Insets.Right);
			r.Y += constraints.Insets.Top;
			r.Height -= (constraints.Insets.Top + constraints.Insets.Bottom);
	    
			diffx = 0;
			if ((constraints.fill != FillType.Horizontal &&
				constraints.fill != FillType.Both)
				&& (r.Width > (constraints.minWidth + constraints.ipadX))) 
			{
				diffx = r.Width - (constraints.minWidth + constraints.ipadX);
				r.Width = constraints.minWidth + constraints.ipadX;
			}
	    
			diffy = 0;
			if ((constraints.fill != FillType.Vertical &&
				constraints.fill != FillType.Both)
				&& (r.Height > (constraints.minHeight + constraints.ipadY))) 
			{
				diffy = r.Height - (constraints.minHeight + constraints.ipadY);
				r.Height = constraints.minHeight + constraints.ipadY;
			}
	    
			switch (constraints.anchor) 
			{
				case AnchorTypes.Center:
					r.X+= diffx/2;
					r.Y += diffy/2;
					break;
				case AnchorTypes.North:
					r.X+= diffx/2;
					break;
				case AnchorTypes.NorthEast:
					r.X+= diffx;
					break;
				case AnchorTypes.East:
					r.X+= diffx;
					r.Y += diffy/2;
					break;
				case AnchorTypes.SouthEast:
					r.X+= diffx;
					r.Y += diffy;
					break;
				case AnchorTypes.South:
					r.X+= diffx/2;
					r.Y += diffy;
					break;
				case AnchorTypes.SouthWest:
					r.Y += diffy;
					break;
				case AnchorTypes.West:
					r.Y += diffy/2;
					break;
				case AnchorTypes.NorthWest:
					break;
				default:
					throw new ArgumentException("Illegal GridBag Anchor value");
			}
		}

		protected Size GetMinSize(GridBagLayoutInfo info) 
		{
			Size d = new Size();
			int i, t;

			t = 0;
			for(i = 0; i < info.columns; i++)
				t += info.minWidth[i];
			d.Width = t;

			t = 0;
			for(i = 0; i < info.rows; i++)
				t += info.minHeight[i];
			d.Height = t;

			return d;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void LayoutGridBag() 
		{
			if(!this.IsInit())
				return;

			Rectangle rect = new Rectangle();
			
			IList controls = this.GetControls();
			// If the ContainerControl has no children to lay out,
			// do nothing.
			if (controls.Count == 0 &&
				(this.columnWidths == null || columnWidths.Length == 0) &&
				(this.rowHeights == null || rowHeights.Length == 0)) 
			{
				return;
			}

			Monitor.Enter(this);

			Rectangle containerBounds = this.GetBounds();

			// Scan the children to determine the total space needed.
			GridBagLayoutInfo info = GetLayoutInfo( PreferredSize);
			Size minSize = GetMinSize(info);

			if (containerBounds.Width < minSize.Width || containerBounds.Height < minSize.Height) 
			{
				info = GetLayoutInfo(MinSize);
				minSize = GetMinSize(info);
			}

			layoutInfo = info;
			rect.Width = minSize.Width;
			rect.Height = minSize.Height;

			double weight;
			int i;

			/*
			 * If the current dimensions of the window don't match the desired
			 * dimensions, then adjust the minWidth and minHeight arrays
			 * according to the weights.
			 */
    
			int diffw = containerBounds.Width - rect.Width;
			if (diffw != 0) 
			{
				weight = 0.0;
				for (i = 0; i < info.columns; i++)
				{
					weight += info.weightX[i];
				}
				if (weight > 0.0) 
				{
					int totalWidth = 0;
					
					for (i = 0; i < info.columns; i++) 
					{
						int colWidth = (int)(info.minWidth[i] + (diffw * info.weightX[i]) / weight);
						
						if(colWidth < 0)
						{
							colWidth = 0;
						}

						info.minWidth[i] = colWidth;
						totalWidth += colWidth;
					}
					
					diffw = containerBounds.Width - totalWidth;
				}
			}
    
			int diffh = containerBounds.Height - rect.Height;
			if (diffh != 0)
			{
				weight = 0.0;
				for (i = 0; i < info.rows; i++)
				{
					weight += info.weightY[i];
				}
				if (weight > 0.0)
				{
					int totalHeight = 0;

					for (i = 0; i < info.rows; i++)
					{
						int rowHeight = (int)(info.minHeight[i] + (diffh * info.weightY[i]) / weight);

						if (rowHeight < 0)
						{
							rowHeight = 0;
						}

						info.minHeight[i] = rowHeight;
						totalHeight += rowHeight;
					}

					diffh = containerBounds.Height - totalHeight;
				}
			}

			/*
			 * Now do the actual layout of the children using the layout information
			 * that has been collected.
			 */
    
			info.startX = diffw/2;
			info.startY = diffh/2;

			for (int controlIndex = 0 ; controlIndex < controls.Count ; controlIndex++) 
			{
				Control control = controls[controlIndex]as Control;
				if (!this.IsVisible(control))
					continue;
				GridBagConstraints constraints = GetConstraintsRef(control);
				if(constraints.IsEmpty)
					continue;

				rect.X= info.startX;
				for(i = 0; i < constraints.tempCurPosX; i++)
					rect.X+= info.minWidth[i];
      
				rect.Y = info.startY;
				for(i = 0; i < constraints.tempCurPosY; i++)
					rect.Y += info.minHeight[i];
      
				rect.Width = 0;
				for(i = constraints.tempCurPosX;
					i < (constraints.tempCurPosX + constraints.tempCellSpanX);
					i++) 
				{
					rect.Width += info.minWidth[i];
				}
      
				rect.Height = 0;
				for(i = constraints.tempCurPosY;
					i < (constraints.tempCurPosY + constraints.tempCellSpanY);
					i++) 
				{
					rect.Height += info.minHeight[i];
				}
      
				AdjustForGravity(constraints, ref rect);
      
				/*
				 * If the window is too small to be interesting then
				 * unmap it. Otherwise configure it and then make sure
				 * it's mapped.
				 */
      
				if ((rect.Width <= 0) || (rect.Height <= 0)) 
				{
					control.Bounds = new Rectangle(0, 0, 0, 0);
				}
				else 
				{
					rect = new Rectangle(containerBounds.Left + rect.X, containerBounds.Top + rect.Y,
						rect.Width, rect.Height);
					
//					if (control.Left != rect.X || control.Top != rect.Y ||
//						control.Width != rect.Width || control.Height != rect.Height) 
//					{
//						control.Bounds = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
//					}

					ControlBounds cb = this.GetChildControlBounds(control);
					if (cb.Location.X != rect.X || cb.Location.Y != rect.Y ||
						cb.Width != rect.Width || cb.Height != rect.Height) 
					{
						cb.Bounds = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
					}
				}
			}
			Monitor.Exit(this);
		}
	}
}
