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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Media;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
{
	/// <summary>
	/// Implements the data store for the <see cref="CellBordersInfo"/> object.
	/// </summary>
	/// <seealso cref="StyleInfoStore"/>
	[
	Serializable,
	StaticDataField("sd")
	]
	public class CellBordersInfoStore: StyleInfoStore
	{
		static StaticData sd = new StaticData(typeof(CellBordersInfoStore), typeof(CellBordersInfo), true);
		
		/// <summary>
		/// Provides information about the <see cref="CellBordersInfo.Top"/> property. 
		/// </summary>
		public readonly static StyleInfoProperty TopProperty = sd.CreateStyleInfoProperty(typeof(Pen), "Top"); 
		
		/// <summary>
		/// Provides information about the <see cref="CellBordersInfo.Left"/> property. 
		/// </summary>
		public readonly static StyleInfoProperty LeftProperty = sd.CreateStyleInfoProperty(typeof(Pen), "Left"); 
		
		/// <summary>
		/// Provides information about the <see cref="CellBordersInfo.Bottom"/> property. 
		/// </summary>
		public readonly static StyleInfoProperty BottomProperty = sd.CreateStyleInfoProperty(typeof(Pen), "Bottom"); 
		
		/// <summary>
		/// Provides information about the <see cref="CellBordersInfo.Right"/> property. 
		/// </summary>
		public readonly static StyleInfoProperty RightProperty = sd.CreateStyleInfoProperty(typeof(Pen), "Right"); 

		/// <override/>
		protected override StaticData StaticDataStore
		{
			get { return sd; }
		}
				
		/// <overload>
		/// Initializes a <see cref="CellBordersInfoStore"/>
		/// </overload>
		/// <summary>
		/// Initializes an empty <see cref="CellBordersInfoStore"/>
		/// </summary>
		public CellBordersInfoStore()
		{
            TopProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            LeftProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            BottomProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            RightProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
		}


#if !SyncfusionFramework4_0
#if !SILVERLIGHT
		/// <summary>
		/// Initializes a new <see cref="CellBordersInfoStore"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected CellBordersInfoStore(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#else
			;
#endif

		}
#endif
#endif
		// Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
		// I assume calling new directly is more efficient. Otherwise this override is obsolete.

		/// <override/>
		public override object Clone()
		{
			StyleInfoStore target = new CellBordersInfoStore();
			CopyTo(target);
			return target;
		}
	}
	
	/// <summary>
	/// Provides a <see cref="StyleInfoSubObjectBase"/> object for borders in a cell. Each border side of
	/// the cell can be configured individually with a <see cref="Pen"/> value. Border sides that
	/// have not been initialized will inherit default values from a base style.
	/// </summary>
	/// <example>
	/// The following code changes border information for cells:
	/// <code lang="C#">
	/// 
	///             Pen border = new Pen(GridBorderStyle.Solid, Color.FromArgb(57, 73, 122));
	///             model[rowIndex, colIndex].Borders.Bottom = border;
	///             model[rowIndex, colIndex].Borders.Right = border;
	/// </code>
	/// The following code hides grid lines for specific cells:
	/// <code lang="C#">
	///             Pen border = new Pen(GridBorderStyle.None);
	///             model[rowIndex, colIndex].Borders.Bottom = border;
	///             model[rowIndex, colIndex].Borders.Right = border;
	/// </code>
	/// </example>
    public class CellBordersInfo : StyleInfoSubObjectBase
	{
		// Static Fields
		private static CellBordersInfo defaultBorders;

        /// <summary>
        /// Creates the CellBordersInfo object.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="store">The store.</param>
        /// <returns></returns>
		public static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
		{
			if (store != null)
				return new CellBordersInfo(identity, store as CellBordersInfoStore);
			return new CellBordersInfo(identity);
		}

		// Constructors
		/// <overload>
		/// Initializes a new empty <see cref="CellBordersInfo"/> object.
		/// </overload>
		/// <summary>
		/// Initializes a new empty <see cref="CellBordersInfo"/> object.
		/// </summary>
		[DebuggerStepThrough()] public CellBordersInfo()
			: base(new CellBordersInfoStore())
		{
		}

		/// <summary>
		/// Initalizes a new <see cref="CellBordersInfo"/>  object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
		/// </summary>
		/// <param name="identity">A <see cref="CachedStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="CellBordersInfo"/>.
		/// </param>
		[DebuggerStepThrough()] public CellBordersInfo(StyleInfoSubObjectIdentity identity)
			: base(identity, new CellBordersInfoStore())
		{
		}

		/// <summary>
		/// Initalizes a new <see cref="CellBordersInfo"/>  object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
		/// </summary>
		/// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="CellBordersInfo"/>.
		/// <param name="store">A <see cref="CellBordersInfoStore"/> that holds data for this <see cref="CellBordersInfo"/>.
		/// All changes in this style object will saved in the <see cref="CellBordersInfoStore"/> object.</param>
		/// </param>
		[DebuggerStepThrough()] public CellBordersInfo(StyleInfoSubObjectIdentity identity, CellBordersInfoStore store)
			: base(identity, store)
		{
		}

		/// <override/>
		[DebuggerStepThrough()] public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
		{
			return new CellBordersInfo(newOwner.CreateSubObjectIdentity(sip), (CellBordersInfoStore) Store.Clone());
		}

        public override void Dispose()
        {
            _store.Clear();
            base.Dispose();
        }

		// Default
		/// <summary>
		/// Returns a default <see cref="CellBordersInfo"/> to be used with a default style.
		/// </summary>
		public static CellBordersInfo Default
		{
			get
			{
				if (defaultBorders == null)
				{
					defaultBorders = new CellBordersInfo();
					defaultBorders.Top = null;
					defaultBorders.Left = null;
					defaultBorders.Bottom = null;
					defaultBorders.Right = null;
				}

				return defaultBorders;
			}
		}

		/// <summary>
		/// Returns <see cref="CellBordersInfo.Default"/>
		/// </summary>
		/// <returns>A <see cref="CellBordersInfo"/> object with default values.</returns>
		protected internal override StyleInfoBase GetDefaultStyle()
		{
			return Default;
		}

		/// <summary>
		/// Allows you to set all four border sides with one command.
		/// </summary>
		/// <example>
		/// <code lang="C#">
		/// model[2, 2].Borders.All = new Pen(GridBorderStyle.Solid, Color.FromArgb(100, 238, 122, 3));
		/// </code>
		/// </example>
		[
		Browsable(false),
		]
		public Pen All
		{
			[DebuggerStepThrough()] 
			set 
			{
				SetValue(CellBordersInfoStore.TopProperty, value);
				SetValue(CellBordersInfoStore.LeftProperty, value);
				SetValue(CellBordersInfoStore.BottomProperty, value);
				SetValue(CellBordersInfoStore.RightProperty, value);
			}
		}
		/// <summary>
		/// Resets all four border sides with one command.
		/// </summary>
		[DebuggerStepThrough()] public void ResetAll()
		{
			ResetValue(CellBordersInfoStore.TopProperty);
			ResetValue(CellBordersInfoStore.LeftProperty);
			ResetValue(CellBordersInfoStore.BottomProperty);
			ResetValue(CellBordersInfoStore.RightProperty);
		}

		/// <summary>
		/// Returns the <see cref="Pen"/> for the specified <see cref="CellBorderSide"/>
		/// </summary>
		public Pen this[CellBorderSide side]
		{
			[DebuggerStepThrough()] 
			get
			{
				switch(side)
				{
					case CellBorderSide.Top:	return Top;
					case CellBorderSide.Left:	return Left;
					case CellBorderSide.Right:	return Right;
					case CellBorderSide.Bottom:	return Bottom;
				}
				throw new ArgumentException("Unknown value", "side");
			}
			[DebuggerStepThrough()] 
			set
			{
				switch(side)
				{
					case CellBorderSide.Top:	Top = value; break;
					case CellBorderSide.Left:	Left = value; break;
					case CellBorderSide.Right:	Right = value; break;
					case CellBorderSide.Bottom:	Bottom = value; break;
					default: 
						throw new ArgumentException("Unknown value", "side");
				}
			}
		}


		// Properties

		#region Top
		/// <summary>
		/// The top border
		/// </summary>
		[
		Browsable(true),
		Category(""),
		Description("The top border"),
		]
		public Pen Top
		{
			[DebuggerStepThrough()] 
			get 
			{
				return (Pen) GetValue(CellBordersInfoStore.TopProperty);
			}
			[DebuggerStepThrough()] 
			set 
			{
				SetValue(CellBordersInfoStore.TopProperty, value);
			}
		}
		/// <summary>
		/// Resets the top border
		/// </summary>
		[DebuggerStepThrough()] public void ResetTop()
		{
			ResetValue(CellBordersInfoStore.TopProperty);
		}
		[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		private bool ShouldSerializeTop()
		{
			return HasValue(CellBordersInfoStore.TopProperty);
		}
		/// <summary>
		/// Determines if the top border has been initialized.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasTop
		{
			[DebuggerStepThrough()] 
			get
			{
				return HasValue(CellBordersInfoStore.TopProperty);
			}
		}
		#endregion
		#region Left
		/// <summary>
		/// The left border
		/// </summary>
		[
		Browsable(true),
		Category(""),
		Description("The left border"),
		]
		public Pen Left
		{
			[DebuggerStepThrough()] 
			get 
			{
				return (Pen) GetValue(CellBordersInfoStore.LeftProperty);
			}
			[DebuggerStepThrough()] 
			set 
			{
				SetValue(CellBordersInfoStore.LeftProperty, value);
			}
		}
		/// <summary>
		/// Resets the left border
		/// </summary>
		[DebuggerStepThrough()] public void ResetLeft()
		{
			ResetValue(CellBordersInfoStore.LeftProperty);
		}
		[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		private bool ShouldSerializeLeft()
		{
			return HasValue(CellBordersInfoStore.LeftProperty);
		}
		/// <summary>
		/// Determines if the left border has been initialized.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasLeft
		{
			[DebuggerStepThrough()] 
			get
			{
				return HasValue(CellBordersInfoStore.LeftProperty);
			}
		}
		#endregion
		#region Bottom
		/// <summary>
		/// The bottom border
		/// </summary>
		[
		Browsable(true),
		Category(""),
		Description("The bottom border"),
		]
		public Pen Bottom
		{
			[DebuggerStepThrough()] 
			get 
			{
				return (Pen) GetValue(CellBordersInfoStore.BottomProperty);
			}
			[DebuggerStepThrough()] 
			set 
			{
				SetValue(CellBordersInfoStore.BottomProperty, value);
			}
		}
		/// <summary>
		/// Resets the bottom border
		/// </summary>
		[DebuggerStepThrough()] public void ResetBottom()
		{
			ResetValue(CellBordersInfoStore.BottomProperty);
		}
		[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		private bool ShouldSerializeBottom()
		{
			return HasValue(CellBordersInfoStore.BottomProperty);
		}
		/// <summary>
		/// Determines if the bottom border has been initialized.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasBottom
		{
			get
			{
				return HasValue(CellBordersInfoStore.BottomProperty);
			}
		}
		#endregion
		#region Right
		/// <summary>
		/// The right border
		/// </summary>
		[
		Browsable(true),
		Category(""),
		Description("The right border"),
		]
		public Pen Right
		{
			[DebuggerStepThrough()] 
			get 
			{
				return (Pen) GetValue(CellBordersInfoStore.RightProperty);
			}
			[DebuggerStepThrough()] 
			set 
			{
				SetValue(CellBordersInfoStore.RightProperty, value);
			}
		}
		/// <summary>
		/// Resets the right border
		/// </summary>
		[DebuggerStepThrough()] public void ResetRight()
		{
			ResetValue(CellBordersInfoStore.RightProperty);
		}
		[EditorBrowsableAttribute(EditorBrowsableState.Never)]
		private bool ShouldSerializeRight()
		{
			return HasValue(CellBordersInfoStore.RightProperty);
		}
		/// <summary>
		/// Determines if the right border has been initialized.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasRight
		{
			[DebuggerStepThrough()] 
			get
			{
				return HasValue(CellBordersInfoStore.RightProperty);
			}
		}
		#endregion
	};
}
