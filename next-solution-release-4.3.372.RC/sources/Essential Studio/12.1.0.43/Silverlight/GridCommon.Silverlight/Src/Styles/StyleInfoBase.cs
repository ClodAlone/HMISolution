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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#if !WinRT
using Syncfusion.Windows.ComponentModel;

using BitVector32 = Syncfusion.Windows.Collections.BitVectorInt32;

namespace Syncfusion.Windows.Styles
#else
using Syncfusion.WinRT.ComponentModel;
using BitVector32 = Syncfusion.WinRT.Collections.BitVectorInt32;

namespace Syncfusion.WinRT.Styles
#endif
{
	/// <exclude/>
	public delegate object CreateSubObjectHandler(StyleInfoSubObjectIdentity identity, object store);

	// StyleInfoStore has
	// static collection of staticData.styleInfoProperties
	// BitVector32[] for Short and boolean values (wraptext, readonly etc.)
	// BitVector32[] for include bits (HasXXX)
	// BitVector32[] for changed bits (ApplyChanges)
	// StyleInfoObjectStore for object (Value, Font, ValueList etc.)

	// staticData.styleInfoProperties must be registered in static constructor. This is product specific
	// 
	// There is no changed event, nor Begin / EndUpdate etc. StyleInfoStore
	// is pure data. It has no knowledge about the owner of the object nor
	// does it support base style inheritance.
	//
	// The StyleInfoBase object that wraps StyleInfoStore
	// should be used to modify data in StyleInfoStore. StyleInfoBase is
	// created in volatile data store and offers StyleChanging, StyleChanged
	// events and Begin / EndUpdate methods. StyleInfoBase has also a link
	// back to the owner of the style (identity) and also supports inheritance
	// of base styles.

	/// <summary>
	/// Defines an interface implemented both by <see cref="StyleInfoBase"/> and <see cref="StyleInfoStore"/>
	/// that allows you to check the state of the object, read and write specific property and execute
	/// style operations with the <see cref="ModifyStyle"/>. method.
	/// </summary>
	public interface IStyleInfo
	{
		/// <summary>
		/// Indicates whether the style is empty.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Indicates whether any properties for this object have changed since it was applied last time.
		/// </summary>
		bool IsChanged { get; }

		/// <summary>
		/// Compares all properties with another style object and indicates whether
		/// the current set of initialized properties is a subset of
		/// the other style object.
		/// </summary>
		/// <param name="style">The other style to compare with.</param>
		/// <returns>True if this style object is a subset of the other style object.</returns>
		bool IsSubset(IStyleInfo style);

		
		/// <summary>
		/// Applies changes to a style object as specified with <see cref="StyleModifyType"/>.
		/// </summary>
		/// <param name="style">The style object to be applied on the current object.</param>
		/// <param name="mt">The actual operation to be performed.</param>
		void ModifyStyle(IStyleInfo style, StyleModifyType mt);

		/// <summary>
		/// Merges two styles. Resets all properties that differ among the two style object
		/// and keeps only those properties that are equal.
		/// </summary>
		/// <param name="style">The other style object this style object should merge with.</param>
		void MergeStyle(IStyleInfo style);

		/// <summary>
		/// Returns the <see cref="StyleInfoStore"/> object that holds all the data for this style object.
		/// </summary>
		StyleInfoStore Store { get; }

		/// <summary>
		/// Parses a given string and applies the results to affected properties in this style object.
		/// </summary>
		/// <param name="s">The string to be interpreted.</param>
		/// <remarks>
		/// <see cref="ParseString"/> consumes strings previously generated with
		/// a <see cref="StyleInfoBase.ToString(string, System.IFormatProvider)"/> method call.
		/// </remarks>
		void ParseString(string s);

		/// <summary>
		/// Indicates whether a specific property has been initialized for the current object.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		bool HasValue(StyleInfoProperty sip);

		/// <summary>
		/// Queries the value for a specific property that has been initialized for the current object.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		object GetValue(StyleInfoProperty sip);
	}


	/// <summary>
	/// <see cref="IStyleInfoSubObject"/> defines an interface for classes
	/// used as sub-objects in a <see cref="StyleInfoBase"/>.
	/// </summary>
	/// <remarks>
	/// <see cref="StyleInfoSubObjectBase"/> implements this interface.
	/// </remarks>
	public interface IStyleInfoSubObject
	{
		/// <summary>
		/// Returns a unique identifier for this sub object in the owner style object. 
		/// </summary>
		StyleInfoProperty Sip { get; }
		
		/// <summary>
		/// Returns a reference to the owner style object.
		/// </summary>
		StyleInfoBase Owner { get; }
		
		/// <summary>
		/// Returns the data for this object. This is the StyleInfoStore from the constructor.
		/// </summary>
		object Data { get; }
		
		/// <summary>
		/// Makes an exact copy of the current object.
		/// </summary>
		/// <param name="newOwner">The new owner style object for the copied object.</param>
		/// <param name="sip">The identifier for this object.</param>
		/// <returns>A copy of the current object and registered with the new owner style object.</returns>
		IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip);
	}

    // IStyleInfo is good for subobjects in a style.

	/// <summary>
	/// Provides a wrapper object for the <see cref="StyleInfoStore"/> object with type
	/// safe access to all properties stored in the style object.
	/// <para/>
	/// Style objects provide a very user friendly way to modify data. It is very much like in Excel VBA.
	/// For example, to change the bold setting for a cell, you simply call grid[5,2].Font.Bold = True.
	/// </summary>
	/// <remarks>
	/// The <see cref="StyleInfoBase"/> is a wrapper around the <see cref="StyleInfoStore"/>.
	/// It provides type safe accessor properties to modify data of the underlying
	/// data store and can hold temporary information about the style object that
	/// does not need to be persisted.
	/// <para/>
	/// In Essential Grid for the example, the GridStyleInfo class holds extensive identity
	/// information about a style object such as cached base styles, row and column index,
	/// a reference to the grid model, and more. This is all the information that can be discarded
	/// when the style is no longer used (because maybe the cell is not visible anymore). Only
	/// the <see cref="StyleInfoStore"/> part needs to be kept alive.
	/// <para/>
	/// Style objects only exist temporarily and will be created as a weak reference in a
	/// volatile data store. Once Garbage Collection kicks in smart style objects that are not
	/// referenced any more will be garbage collected. The volatile data cache can also be
	/// cleared manually.
	/// <para/>
	/// Because Style objects know their identity they can notify their owner of changes or
	/// load base style information when the user interacts with the style object. This allows
	/// you to make changes to a style object directly, such as Cell.Font.Bold = True;
	/// <para/>
	/// Style objects support property inheritance from parent styles, e.g. in a grid a cell
	/// can inherit properties from a parent row, column, table or a collection of names styles
	/// in a base styles map.
	/// <para/>
	/// Style objects support subobjects. Subobjects can support inheritance (e.g. a Font.Bold
	/// can be inherited). Immutable subobjects like BrushInfo don't support inheritance of
	/// individual properties.
	/// <para/>
	/// <see cref="StyleInfoStore"/> allows you to register any number of properties but keeps the data
	/// very memory efficient. Only properties that are actually used for a style
	/// object will be allocated for an object. The StyleObjectStore handles the storage of the object.
	/// For short integers, enums and Boolean values, the data will be stored in a BitVector32
	/// structure to save even more memory.
	/// <para/>
	/// Programmers can derive their own style classes from <see cref="StyleInfoSubObjectBase"/>
	/// and add type-safe (and intellisense)
	/// supported custom properties to the style class. If you write, for example, your own
	/// SpinButton class that needs individual properties, simply add a �CellSpinButtonInfo�
	/// class as subobject. If you derive CellSpinButtonInfo from StyleInfoSubObjectBase,
	/// your new object will support property inheritance from base styles.
	/// <para/>
	/// Style objects can be written into a string (see <see cref="StyleInfoBase.ToString(string, System.IFormatProvider)"/>) and later be recreated
	/// using the <see cref="StyleInfoBase.ParseString"/> method. When writing the string you have the option to show
	/// default values (use the �d� format). Subobjects will be identified with a dot
	/// �.�, e.g. �Font.Bold�
	/// <para/>
	/// Style object support several operations how to combine information from two styles. Style operations
	/// include: apply changes, apply initialized properties, override initialized properties, exclude properties.
	/// See the <see cref="ModifyStyle"/> method.
	/// <para/>
	/// Style objects support BeginUpdate, EndUpdate mechanism. This allows users to batch several operations
	/// on a style object into one transaction.
	/// <para/>
	/// </remarks>
	/// <example>The following example shows how you can use the GridFontInfo class in Essential Grid:
	/// <code lang="C#">
	///         standard.Font.Facename = "Helvetica";
	///         model[1, 3].Font.Bold = true;
	///         string faceName = model[1, 3].Font.Facename; // any cell inherits standard style
	///         Console.WriteLIne(faceName); // will output "Helvetica"
	///         Console.WriteLIne(model[1, 3].Font.Bold); // will output "true"
	///         Console.WriteLIne(model[1, 3].Font.HasFaceName); // will output "False"
	/// </code>
	/// </example>
	/// <seealso cref="StyleInfoIdentityBase"/>
	/// <seealso cref="StyleInfoStore"/>
#if !WinRT	
    [
	TypeConverter(typeof(StyleInfoBaseConverter))
	
	]
#endif
#if !WinRT
	public abstract class StyleInfoBase: IDisposable, IStyleInfo, ISupportInitialize, IFormattable, IConvertible, IXmlSerializable
#else
    [ClassReference(IsReviewed = false)]
    public abstract class StyleInfoBase : IDisposable, IStyleInfo, ISupportInitialize, IFormattable, IXmlSerializable
#endif
	{
		// StyleInfoIdentityBase for identity information (owner, row, col). If NULL, not owned.
		/// <exclude/>
		internal StyleInfoIdentityBase identity;
        
        /// <summary>
		/// Gets / sets the identity information for the current <see cref="StyleInfoBase"/>.
		/// </summary>
		
		public StyleInfoIdentityBase Identity
		{
			get
			{
				return identity;
			}
			set
			{
				identity = value;
			}
		}

		// Store holds actual data.
		/// <exclude/>
		internal StyleInfoStore _store;

		/// <exclude/>
		internal StyleInfoObjectStore expandableObjects = null;

		/// <summary>
		/// The <see cref="StyleInfoStore"/> object that holds all the data for this style object.
		/// </summary>
		
		public StyleInfoStore Store
		{
			get { return _store; }
		}

        /// <summary>
        /// Sets the <see cref="StyleInfoStore"/> object that holds all the data for this style object.
        /// </summary>
        /// <param name="store"></param>
        protected void SetStore(StyleInfoStore store)
        {
            this._store = store;
            ClearCache();
        }

		/// <exclude/>
		
		internal StyleInfoObjectStore ExpandableObjects 
		{
			get 
			{
				if (expandableObjects == null)
					expandableObjects = new StyleInfoObjectStore();
				return expandableObjects;
			}
		}

#if CacheAsFrugalMap
        FrugalMap baseStyleValuesCache;
#else
        Dictionary<int, object> baseStyleValuesCache;
#endif
        bool cacheValues = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance should cache values for resolved base style properties.
        /// </summary>
        /// <value><c>true</c> if instance should cache values; otherwise, <c>false</c>.</value>
        
        public bool CacheValues
        {
            get
            {
                return cacheValues;
            }
            set
            {
                if (value != CacheValues)
                {
                    cacheValues = value;
#if CacheAsFrugalMap
                    baseStyleValuesCache = new FrugalMap();
#else
                    if (value)
                        baseStyleValuesCache = new Dictionary<int, object>();
                    else
                        baseStyleValuesCache = null;
#endif
                }
            }
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            bool b = CacheValues;
            CacheValues = false;
            CacheValues = b;
        }


		// Helpers for update pattern (BeginUpdate, EndUpdate, Updating).
		int updateCount = 0;
		bool changePending = false;

		// Events
		// public event EventHandler Changing;

		/// <summary>
		/// Occurs when a property in the style object or in a sub object is changed.
		/// </summary>
		
		public event StyleChangedEventHandler Changed;

        List<WeakReference> weakReferenceChangedListeners;

        /// <summary>
        /// A list of listeners that will be referenced using a WeakReference. The
        /// listeners must implement the <see cref="IStyleChanged"/> interface. When
        /// this style object <see cref="OnStyleChanged"/> method is called it
        /// will then loop through all objects in this list and call each objects
        /// <see cref="IStyleChanged.StyleChanged"/> method.
        /// </summary>
        public List<WeakReference> WeakReferenceChangedListeners
        {
            get
            {
                if (weakReferenceChangedListeners == null)
                    weakReferenceChangedListeners = new List<WeakReference>();
                return weakReferenceChangedListeners;
            }
        }


		/// <summary>
		/// Occurs before a property in the style object or in a sub object is changed.
		/// </summary>
		
		public event StyleChangedEventHandler Changing;

		// Constructors
		/// <summary>
		/// Overloaded. Initializes a new style object with no identity and data initialized.
		/// </summary>
		protected StyleInfoBase()
		{
			this.identity = null;
			this._store = null;
		}

		/// <summary>
		/// Initalizes a new <see cref="StyleInfoBase"/> object and associates it with an existing <see cref="StyleInfoStore"/>.
		/// </summary>
		/// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.
		/// All changes in this style object will be saved in the <see cref="StyleInfoStore"/> object.
		/// </param>
		protected StyleInfoBase(StyleInfoStore store)
		{
			this.identity = null;
			this._store = store;
		}

		/// <summary>
		/// Initalizes a new <see cref="StyleInfoBase"/> object and associates it with an existing <see cref="StyleInfoStore"/> and <see cref="StyleInfoIdentityBase"/>.
		/// </summary>
        /// <param name="identity">A <see cref="StyleInfoIdentityBase"/> that holds the identity for this <see cref="StyleInfoBase"/>.</param>
		/// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.
		/// All changes in this style object will be saved in the <see cref="StyleInfoStore"/> object.
		/// </param>
		protected StyleInfoBase(StyleInfoIdentityBase identity, StyleInfoStore store)
            : this(identity, store, false)
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="StyleInfoBase"/> object and associates it with an existing <see cref="StyleInfoStore"/> and <see cref="StyleInfoIdentityBase"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoIdentityBase"/> that holds the identity for this <see cref="StyleInfoBase"/>.</param>
        /// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.
        /// All changes in this style object will be saved in the <see cref="StyleInfoStore"/> object.</param>
        /// <param name="cacheValues">if set to <c>true</c> the style the resulting value of a property when inherited from a basestyle
        /// so that the property does not have to be evaluated multiple times when called repeatedly.</param>
        protected StyleInfoBase(StyleInfoIdentityBase identity, StyleInfoStore store, bool cacheValues)
		{
			this.identity = identity;
			this._store = store;
            this.CacheValues = cacheValues;
		}

		/// <summary>
		/// Releases all resources used by the component.
		/// </summary>
		public virtual void Dispose()
		{
            if (identity != null && !identity.IsDisposable)
                return;

            if (weakReferenceChangedListeners != null)
            {
                weakReferenceChangedListeners.Clear();
                weakReferenceChangedListeners = null;
            }

            if (expandableObjects != null)
			{
				expandableObjects.Dispose();
				expandableObjects = null;
			}
			if (identity != null)
			{
				identity.Dispose();
				identity = null;
			}
			if (_store != null)
			{
				_store.Dispose();
				_store = null;
			}
			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// Indicates whether two style objects are equal. Identity is left out with this comparison,
		/// only the data (<see cref="StyleInfoBase.Store"/>) are compared.
		/// </summary>
		/// <param name="obj">The other style object to compare the current object with.</param>
		/// <returns>True if both objects have equal data; false otherwise.</returns>
		public override bool Equals(object obj)
		{
			StyleInfoBase style = obj as StyleInfoBase;
			if (style == null)
				return false;

			return EqualsObject(identity, style.identity) && EqualsObject(_store, style._store);
		}

		/// <summary>
		/// Compares two objects for equality. Works also with NULL references.
		/// </summary>
		/// <param name="obj1">The first object to compare.</param>
		/// <param name="obj2">The second object to compare.</param>
		/// <returns>True if both objects are equal.</returns>
		protected static bool EqualsObject(object obj1, object obj2)
		{
			return obj1 == obj2
				|| obj1 != null && obj2 != null && obj1.Equals(obj2);
		}

		/// <summary>
		/// Returns a hash code which is based on values inside the <see cref="StyleInfoBase.Store"/>.
		/// </summary>
		/// <returns>An integer hash code.</returns>
		public override int GetHashCode()
		{
			return _store != null ? _store.GetHashCode() : base.GetHashCode();
		}

		/// <summary>
		/// Overloaded. Creates a formatted string for this style object. This string can
		/// later be consumed by <see cref="StyleInfoBase.ParseString"/>.
		/// </summary>
		/// <returns>A string with formatted style information.</returns>
		/// <remarks>
		/// Style objects can be formatted into a string that can be consumed 
		/// by <see cref="StyleInfoBase.ParseString"/> to recreate style information.
		/// <para/>
		/// When writing the string, you have the option to show default values 
		/// (use the �d� format). <para/>
		/// Subobjects will be identified with a period ".", e.g. "Font.Bold".
		/// </remarks>
		public override string ToString()
		{
			return ToString("", null);
		}
		
		/// <summary>
		/// Creates a formatted string for this style object. This string can
		/// later be consumed by <see cref="StyleInfoBase.ParseString"/>. You can specify
		/// "d" as format if you want to write out default values inherited from a base style.
		/// </summary>
		/// <param name="format">Use "d" if default values should be included; "G" and NULL are default.</param>
		/// <returns>A string with formatted style information.</returns>
		/// <remarks>
		/// Style objects can be formatted into a string that can be consumed 
		/// by <see cref="StyleInfoBase.ParseString"/> to recreate style information.
		/// <para/>
		/// When writing the string you have the option to show default values 
		/// (use the "d" format) or not. <para/>
		/// Subobjects will be identified with a period ".", e.g. "Font.Bold".
		/// </remarks>
		public string ToString(string format)
		{
			return ToString(format, null);
		}

		/// <summary>
		/// Creates a formatted string for this style object. This string can
		/// later be consumed by <see cref="StyleInfoBase.ParseString"/>. You can specify
		/// "d" as format if you want to write out default values inherited from a base style.
		/// </summary>
		/// <param name="format">Use "d" if default values should be included; "G" and NULL are default.</param>
		/// <returns>A string with formatted style information.</returns>
		/// <param name="provider">An <see cref="IFormatProvider"/> to be used
        /// for the <see cref="ToString(string, System.IFormatProvider)"/> operation. Can be NULL.</param>
		/// <remarks>
		/// Style objects can be formatted into a string that can be consumed 
		/// by <see cref="StyleInfoBase.ParseString"/> to recreate style information.
		/// <para/>
		/// When writing the string you have the option to show default values 
		/// (use the "d" format) or not. <para/>
		/// Subobjects will be identified with a period ".", e.g. "Font.Bold".
		/// </remarks>
		public string ToString(string format, IFormatProvider provider)
		{
			if (_store == null)
				return "Disposed";

			string prefix = "";
			bool showDefault = false;
			if (format != null)
			{
				string[] formats = format.Split(',');
				foreach (string s in formats)
				{
					if (s.Length > 0)
					{
#if !WinRT
						switch (Char.ToLower(s[0], System.Globalization.CultureInfo.InvariantCulture))
#else
                        switch (Char.ToLower(s[0]))
#endif
                        {
							case 'p':
								prefix = s.Substring(1);
								if (!prefix.EndsWith("."))
									prefix += ".";
								break;
							case 'd':
								showDefault = true;
								break;
						}
					}
				}
			}

			StringBuilder sb = new StringBuilder();

			foreach (StyleInfoProperty sip in _store.StyleInfoProperties)
			{
                if (!sip.IsSerializable)
                    continue;

				if (showDefault || HasValue(sip))
				{
					if (sip.IsExpandable)
					{
						IFormattable styleInfo = this._GetExpandableObjectProperty(sip) as IFormattable;
						format = "p" + prefix + sip.PropertyName;
						if (showDefault)
							format += ",d";
						sb.Append(styleInfo.ToString(format, provider));
					}
					else
					{
						sb.Append(prefix);
						sb.Append(sip.PropertyName);
						sb.Append(" = ");
						string stringValue = sip.FormatValue(GetValue(sip));
						if (HasValue(sip))
							sb.Append(stringValue);
						else
							sb.Append("Default(" + stringValue +")");
						sb.Append(Environment.NewLine);
					}
				}				
			}
			
			return sb.ToString();
		}

		/// <summary>
		/// Parses a given string and applies it's results to affected properties in this style object.
		/// </summary>
		/// <param name="s">The string to be parsed.</param>
		/// <remarks>
		/// <see cref="ParseString"/> consumes strings previously generated with
        /// a <see cref="ToString(string, System.IFormatProvider)"/> method call.
		/// </remarks>
		public void ParseString(string s)
		{
			string[] lines = s.Split('\n');
			foreach (string line in lines)
			{
				int lh = line.IndexOf("=");
				if (lh != -1)
				{
					string name = line.Substring(0, lh-1).Trim();
					StyleInfoProperty sip;
					IStyleInfo styleInfo;
					int dot = name.IndexOf(".");
					if (dot != -1)
					{
						// This is an expandable object.
						name = name.Substring(0, dot);
						sip = _store.FindStyleInfoProperty(name);
						if (sip != null)
						{
							styleInfo = this._GetExpandableObjectProperty(sip) as IStyleInfo;
							styleInfo.ParseString(line.Substring(dot+1));
						}
					}
					else
					{
						sip = _store.FindStyleInfoProperty(name);
						if (sip != null)
						{
							string detail = line.Substring(lh+1).Trim();
							if (detail.StartsWith("Default("))
								this.ResetValue(sip);
							else
							{
								object value = sip.ParseValue(detail);
								this.SetValue(sip, value);
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Suspends raising <see cref="Changed"/> events until <see cref="EndInit"/> is called 
		/// and will not change the <see cref="IsChanged"/> state of this object.
		/// </summary>
		public void BeginInit()
		{
			if (Updating)
				throw new InvalidOperationException("BeginInit called twice.");
			BeginUpdate();
		}

		/// <summary>
		/// Resumes raising <see cref="Changed"/> events and resets the <see cref="IsChanged"/> state of this object.
		/// </summary>
		public void EndInit()
		{
			changePending = false;
			// mark style object as "unmodified"
			Store.ResetChangedBits(); 
			EndUpdate();
		}

		/// <summary>
		/// Suspends raising <see cref="Changed"/> events until <see cref="EndUpdate"/> is called.
		/// </summary>
		public void BeginUpdate()
		{
			updateCount++;
		}

		/// <summary>
		/// Suspends raising <see cref="Changed"/> events and if changes were
		/// made before the <see cref="EndUpdate"/> call, it will raise a changed
		/// notification immediately.
		/// </summary>
		public void EndUpdate()
		{
			if (--updateCount == 0)
			{
				if (changePending)
				{
					if (!inStyleChanged)
					{
						inStyleChanged = true;
						OnStyleChanged(null);
						inStyleChanged = false;
					}
					changePending = false;
				}
			}
		}

		/// <summary>
		/// Indicates whether <see cref="BeginUpdate"/> was called for this object.
		/// </summary>
		
		public bool Updating
		{
			get 
			{
				return updateCount > 0;
			}
		}

		/// <summary>
		/// Override this method to return a default style object for your derived class.
		/// </summary>
		/// <returns>A default style object.</returns>
		/// <remarks>
		/// You should cache the default style object in a static field.
		/// </remarks>
		protected internal abstract StyleInfoBase GetDefaultStyle();

		/// <summary>
		/// Override this method to create a product-specific identity object for a sub object.
		/// </summary>
		/// <returns>An identity object for a subobject of this style.</returns>
		/// <example>
		/// The following code is an example how Essential Grid creates GridStyleInfoSubObjectIdentity:
		/// <code lange="C#">
		/// public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
		///	{
		///		return new GridStyleInfoSubObjectIdentity(this, sip);
		/// }
		/// </code>
		/// </example>
		public virtual StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
		{
			throw new NotSupportedException("Nested subobjects are not supported.");
		}

		/// <summary>
		/// Locates the <see cref="StyleInfoStore"/> in the list of base styles that 
		/// provides a specific property.
		/// </summary>
		/// <param name="sip">Identifies the property to look for.</param>
		/// <returns>The style store object that has the specified property.</returns>
		protected virtual StyleInfoStore GetDefaultStyleInfoStore(StyleInfoProperty sip)
		{
			StyleInfoBase style = IntGetDefaultStyleInfo(sip);
			if (style != null)
				return style._store;
			return GetDefaultStyle()._store;
		}

		/// <summary>
		/// Locates the base style that has the specified property and returns its instance.
		/// </summary>
		/// <param name="sip">Identifies the property to look for.</param>
		/// <returns>The style object that has the specified property.</returns>
		protected virtual StyleInfoBase IntGetDefaultStyleInfo(StyleInfoProperty sip)
		{
			if (identity != null)
				return identity.GetBaseStyle(this, sip);
			return null;
		}


		/// <exclude/>
		internal bool inStyleChanged = false;
  
		/// <summary>
		/// Notifies the associated identity object that a specific property
		/// was changed and raises a <see cref="StyleInfoBase.Changed"/> event.
		/// </summary>
		/// <param name="sip">Identifies the property to look for.</param>
		protected virtual void OnStyleChanged(StyleInfoProperty sip)
		{
			if (sip != null)
				this._store.SetValueModified(sip, true);

			if (updateCount > 0)
				changePending = true;
			else 
			{
				if (this.identity != null)
					this.identity.OnStyleChanged(this, sip);
                StyleChangedEventArgs e = new StyleChangedEventArgs(sip);
				if (Changed != null)
					Changed(this, e);
                if (weakReferenceChangedListeners != null)
                {
                    WeakReference[] list = new WeakReference[weakReferenceChangedListeners.Count];
                    weakReferenceChangedListeners.CopyTo(list, 0);
                    foreach (WeakReference wr in list)
                    {
                        IStyleChanged sc = wr.Target as IStyleChanged;
                        if (sc != null)
                            sc.StyleChanged(e);
                    }
                }
			}
		}

		/// <summary>
		/// Notifies the associated identity object that a specific property
		/// will be changed and raises a <see cref="StyleInfoBase.Changing"/> event.
		/// </summary>
		/// <param name="sip">Identifies the property to look for.</param>
		protected virtual void OnStyleChanging(StyleInfoProperty sip)
		{
			if (updateCount == 0)
			{
				if (this.identity != null)
					this.identity.OnStyleChanging(this, sip);
				if (Changing != null)
					Changing(this, new StyleChangedEventArgs(sip));
			}
		}

		/// <summary>
		/// Indicates whether the style is empty.
		/// </summary>
		
		public bool IsEmpty
		{
			get { return _store.IsEmpty; }
		}

		/// <summary>
		/// Indicates whether any properties for this object have changed since it was applied last time.
		/// </summary>
		
		public bool IsChanged
		{
			get { return _store.IsChanged; }
		}

		/// <summary>
		/// Compares all properties with another style object and indicates
		/// whether the current set of initialized properties is a subset of
		/// the other style object.
		/// </summary>
		/// <param name="istyle">The other style to compare with.</param>
		/// <returns>True if this style object is a subset of the other style object.</returns>
		public bool IsSubset(IStyleInfo istyle)
		{
			StyleInfoBase style = istyle as StyleInfoBase;
			if (style == null)
				throw new ArgumentNullException("style");
			return _store.IsSubset(style._store);
		}

		/// <summary>
		/// Copies properties from another style object. This method raises Changing and Changed 
		/// notifications if the other object differs. (ModifyStyle does not raise these events).
		/// </summary>
		/// <param name="istyle">The style object to be applied on the current object.</param>
		public void CopyFrom(IStyleInfo istyle)
		{
			if (istyle == null)
				throw new ArgumentNullException("style");
			if (!EqualsObject(this.Store, istyle.Store))
			{
                if (expandableObjects != null)
                {
                    // Clear out cached ExpandableObjects shadows. This is needed in case objects in store are replaced and
                    // then this expandableobjects cache would be out of sync. ExpandableObjects is allocated on demand when ExpandableObjects 
                    // property is accessed.
                    expandableObjects.Dispose();
                    expandableObjects = null;
                }

				if (!inStyleChanged)
					this.OnStyleChanging(null);
				_store.ModifyStyleKeepChanges(istyle.Store, StyleModifyType.Copy);
				if (!inStyleChanged)
				{
					inStyleChanged = true;
					this.OnStyleChanged(null);
					inStyleChanged = false;
				}
			}
		}

		/// <summary>
		/// Applies changes to a style object as specified with <see cref="StyleModifyType"/>.
		/// </summary>
		/// <param name="istyle">The style object to be applied on the current object.</param>
		/// <param name="mt">The actual operation to be performed.</param>
		public virtual void ModifyStyle(IStyleInfo istyle, StyleModifyType mt)
		{
			if (istyle == null)
				throw new ArgumentNullException("style");
            if (!inStyleChanged)
                this.OnStyleChanging(null);
            _store.ModifyStyle(istyle.Store, mt);
            if (!inStyleChanged)
            {
                inStyleChanged = true;
                this.OnStyleChanged(null);
                inStyleChanged = false;
            }
        }

		/// <summary>
		/// Merges two styles. Resets all properties that differ among the two style objects
		/// and keeps only those properties that are equal.
		/// </summary>
		/// <param name="istyle">The other style object this style object should merge with.</param>
		public void MergeStyle(IStyleInfo istyle)
		{
			StyleInfoBase style = istyle as StyleInfoBase;
			if (style == null)
				throw new ArgumentNullException("style");
			_store.MergeStyle(style._store);
		}

		/// <summary>
		/// Indicates whether the specified property has been initialized for the current object.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		public bool HasValue(StyleInfoProperty sip)
		{
#if DEBUG
            if (_store.include == null || sip.BitVectorIndex >= _store.include.Length)
                throw new InvalidOperationException("style was probably disposed or wrong StyleInfoProperty was used.");
#endif

            return _store.include[sip.BitVectorIndex][sip.BitVectorMask];
        }

		/// <summary>
		/// Indicates whether the specified property has been modified for the current object.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		protected bool IsValueModified(StyleInfoProperty sip)
		{
			// What about changes in expandable object?
			return _store.IsValueModified(sip);
		}

		/// <summary>
		/// Marks the specified property as uninitialized for the current object.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		public void ResetValue(StyleInfoProperty sip)
		{
			if (!inStyleChanged)
				this.OnStyleChanging(sip);
			
			_store.ResetValue(sip);
			if (expandableObjects != null && sip.IsExpandable)
				expandableObjects.RemoveObject(sip.ExpandableObjectStoreKey);

			if (!inStyleChanged)
			{
				inStyleChanged = true;
				this.OnStyleChanged(sip);
				inStyleChanged = false;
			}
		}


		internal object _GetExpandableObjectProperty(StyleInfoProperty sip)
		{
			bool found;
			int key = sip.ExpandableObjectStoreKey;
			object subObject = ExpandableObjects.GetObject(key, out found);
			if (subObject == null)
			{
				// Allocates a GridFontInfo with a new or existing GridFontInfoStore.
				// Modifying GridFontInto should force GridFontInfoStore be saved in _store
				object objectStore = _store.GetValue(sip);
				if (sip.CreateObject != null)
					subObject = sip.CreateObject(this.CreateSubObjectIdentity(sip), objectStore);
					// fall back on Reflection if no CreateObject method was supplied (slow !!!)
				else if (objectStore == null)
					subObject = Activator.CreateInstance(sip.PropertyType, 
						new object[] { this.CreateSubObjectIdentity(sip) } );
				else
					subObject = Activator.CreateInstance(sip.PropertyType,
						new object[] { this.CreateSubObjectIdentity(sip), objectStore });
				expandableObjects.SetObject(key, subObject);

			}
			return subObject;
		}

		internal void OnSubObjectChanged(IStyleInfoSubObject subObject)
		{
			if (!inStyleChanged)
				OnStyleChanging(subObject.Sip);

			_store.SetValue(subObject.Sip, subObject.Data);
			if (!inStyleChanged)
			{
				inStyleChanged = true;
				OnStyleChanged(subObject.Sip);
				inStyleChanged = false;
			}
		}

		/// <exclude/>
		public void NotifySubObjectChanged(IStyleInfoSubObject subObject)
		{
			OnSubObjectChanged(subObject);
		}

		/// <summary>
		/// Queries the value for the specified property that has been initialized for the current object
		/// or locates it in a base style.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		public object GetValue(StyleInfoProperty sip)
		{
			if (sip.IsExpandable)
				return _GetExpandableObjectProperty(sip);
			else 
			{
                if (_store != null && _store.HasValue(sip))
                    return _store.GetValue(sip);
                else if (CacheValues)
                {
#if CacheAsFrugalMap
                    object value = baseStyleValuesCache[sip.PropertyKey];
                    if (value == DependencyProperty.UnsetValue)
                    {
                        value = GetDefaultStyleInfoStore(sip).GetValue(sip);
                        baseStyleValuesCache[sip.PropertyKey] = value;
                    }
#else
                    object value;
                    if (!baseStyleValuesCache.TryGetValue(sip.PropertyKey, out value))
                    {
                        value = GetDefaultStyleInfoStore(sip).GetValue(sip);
                        baseStyleValuesCache[sip.PropertyKey] = value;
                    }
#endif
                    return value;
                }
                else
                {
                    return GetDefaultStyleInfoStore(sip).GetValue(sip);
                }
			}
		}

        /// <summary>
        /// Gets the default value from a base style.
        /// </summary>
        /// <param name="sip">The sip.</param>
        /// <returns></returns>
        protected virtual object GetDefaultValue(StyleInfoProperty sip)
        {
            return GetDefaultStyleInfoStore(sip).GetValue(sip); 
        }

		/// <summary>
		/// Queries the <see cref="System.Int16"/> value for the specified property that has been initialized for the current object
		/// or locates it in a base style.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		public short GetShortValue(StyleInfoProperty sip)
		{
			if (_store.HasValue(sip))
				return _store.GetShortValue(sip);
			else
				return GetDefaultStyleInfoStore(sip).GetShortValue(sip);
		}

		/// <summary>
		/// Overloaded. Initializes the value for the specified property.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		/// <param name="value">The value to be saved for the property.</param>
		public void SetValue(StyleInfoProperty sip, object value)
		{
			// Following line commented out to avoid request beeing sent
			// and object being calculated 
			//if (!_store.HasValue(sip) || _store.GetValue(sip) != value)
			{
				if (!inStyleChanged)
					this.OnStyleChanging(sip);

				IStyleInfoSubObject siso;
				if (sip.IsExpandable && value != null)
				{
					siso = (IStyleInfoSubObject) value;
					if (siso.Owner != this)
					{
						// object belongs to a different style - _store copy
						siso = siso.MakeCopy(this, sip);
					}

                    if (expandableObjects != null)
                        expandableObjects.RemoveObject(sip.ExpandableObjectStoreKey);
					_store.SetValue(sip, siso.Data);
				}
				else
					_store.SetValue(sip, value);

				if (!inStyleChanged)
				{
					inStyleChanged = true;
					this.OnStyleChanged(sip);
					inStyleChanged = false;
				}
			}
		}

		/// <summary>
		/// Initializes the <see cref="System.Int16"/> value for the specified property.
		/// </summary>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		/// <param name="value">The value to be saved for the property.</param>
		public void SetValue(StyleInfoProperty sip, short value)
		{
			// Following line commented out to avoid request beeing sent
			// and object being calculated 
			//if (!_store.HasValue(sip) || _store.GetShortValue(sip) != value)
			//{
			if (!inStyleChanged)
				this.OnStyleChanging(sip);
			
			_store.SetValue(sip, value);
			if (!inStyleChanged)
			{
				inStyleChanged = true;
				this.OnStyleChanged(sip);
				inStyleChanged = false;
			}
			//}
		}
#if !WinRT
		#region IConvertible Members

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return this.ToString();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			try
			{
				return Activator.CreateInstance(conversionType, new object[] { this });
			}
			catch (Exception ex)
			{
				throw new InvalidCastException("", ex);
			}
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		#endregion
#endif
        #region IXmlSerializable Members

        /// <summary>
        /// Serializes the contents of this object into an XML stream.
        /// </summary>
        /// <param name="writer">Represents the XML stream.</param>
        public void WriteXml(XmlWriter writer)
        {
            Store.WriteXml(writer);
        }

        /// <summary>
        /// Not implemented and returns NULL.
        /// </summary>
        /// <returns></returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            // TODO:  Add GetSchema implementation
            return null;
        }

        /// <summary>
        /// Deserializes the contents of this object from an XML stream.
        /// </summary>
        /// <param name="reader">Represents the XML stream.</param>
        public void ReadXml(XmlReader reader)
        {
            Store.ReadXml(reader);
        }

        #endregion
	}


	/// <summary>
	/// Specifies whether a property should be serialized.
	/// </summary>
	/// <seealso cref="SyncfusionEventArgs"/>
	[AttributeUsageAttribute(AttributeTargets.Property)]
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class SerializePropertyAttribute : Attribute 
	{
		// Fields
		private bool serializeProperty;
        
		/// <summary>
		///   <para>Specifies that a property should be serialized. 
		///   This <see langword="static" /> field is Read-only.</para>
		/// </summary>
		public static readonly SerializePropertyAttribute Yes;

		/// <summary>
		///   <para>Specifies that a property should not be serialized. 
		///   This <see langword="static" /> field is Read-only.</para>
		/// </summary>
		public static readonly SerializePropertyAttribute No;

		/// <summary>
		/// The default setting for this attribute.
		/// </summary>
		public static readonly SerializePropertyAttribute Default;
        
		// Constructors
        
		static SerializePropertyAttribute()
		{
			SerializePropertyAttribute.Yes = new SerializePropertyAttribute(true);
			SerializePropertyAttribute.No = new SerializePropertyAttribute(false);
			SerializePropertyAttribute.Default = SerializePropertyAttribute.Yes;
		} 
        
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="SerializePropertyAttribute" /> class.</para>
		/// </summary>
		/// <param name="serializeProperty">
		///   <see langword="True" /> if a property should be serialized; <see langword="False" /> otherwise. The default is <see langword="True" />.</param>
		public SerializePropertyAttribute(bool serializeProperty)
		{
			this.serializeProperty = serializeProperty;
		} 
        
		// Methods
        
		/// <override/>
		public override int GetHashCode()
		{
			return this.serializeProperty.GetHashCode();
		} 
        
		/// <override/>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			SerializePropertyAttribute attr = obj as SerializePropertyAttribute;
			if (attr != null)
				return (attr.SerializeProperty == this.serializeProperty);
			return false;
		} 
        
		/// <summary>
		///   <para>Indicates whether a property is shown in the ToString result.</para>
		/// </summary>
		public bool SerializeProperty 
		{ 
			get
			{
				return this.serializeProperty;
			} 
		}

	
		/// <summary>
		/// Indicates whether the <see cref="SerializePropertyAttribute"/> has been set for the property.
		/// </summary>
		/// <param name="info">A <see cref="System.Reflection.PropertyInfo"/></param>
        /// <returns>True if property has a <see cref="SerializePropertyAttribute"/>; False otherwise.</returns>
		public static bool IsSerializeProperty(PropertyInfo info)
		{
			SerializePropertyAttribute attr = SerializePropertyAttribute.Default;
			if (info != null && info.IsDefined(typeof(SerializePropertyAttribute), true))
			{
#if !WinRT
				object[] array = info.GetCustomAttributes(typeof(SerializePropertyAttribute), true);
				attr = (SerializePropertyAttribute) array[0];
#else
                IEnumerable<Attribute> array = info.GetCustomAttributes(typeof(SerializePropertyAttribute), true);
                attr = (SerializePropertyAttribute)array.GetEnumerator().Current;
#endif
			}
			return attr.SerializeProperty;
		}
	}

	/// <summary>
	/// Specifies whether a property should be cloned.
	/// </summary>
	/// <seealso cref="SyncfusionEventArgs"/>
	[AttributeUsageAttribute(AttributeTargets.Property)]
	public sealed class CloneablePropertyAttribute : Attribute 
	{
		// Fields
		private bool cloneableProperty;
        
		/// <summary>
		///   <para>Specifies that a property should be cloned if the assigned object implements ICloneable. 
		///   This <see langword="static" /> field is Read-only.</para>
		/// </summary>
		public static readonly CloneablePropertyAttribute Yes;

		/// <summary>
		///   <para>Specifies that a property should never be cloned even if the assigned object implements ICloneable
		///   This <see langword="static" /> field is Read-only.</para>
		/// </summary>
		public static readonly CloneablePropertyAttribute No;

		/// <summary>
		/// The default setting for this attribute.
		/// </summary>
		public static readonly CloneablePropertyAttribute Default;
        
		// Constructors
        
		static CloneablePropertyAttribute()
		{
			CloneablePropertyAttribute.Yes = new CloneablePropertyAttribute(true);
			CloneablePropertyAttribute.No = new CloneablePropertyAttribute(false);
			CloneablePropertyAttribute.Default = CloneablePropertyAttribute.Yes;
		} 
        
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="CloneablePropertyAttribute" /> class.</para>
		/// </summary>
		/// <param name="cloneableProperty">
        ///   <see langword="True" /> if a property should be should be cloned if the assigned object implements ICloneable; <see langword="False" /> otherwise. The default is <see langword="True"/></param>
		public CloneablePropertyAttribute(bool cloneableProperty)
		{
			this.cloneableProperty = cloneableProperty;
		} 
        
		// Methods
     
		/// <override/>
		public override int GetHashCode()
		{
			return this.cloneableProperty.GetHashCode();
		} 
        
		/// <override/>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			CloneablePropertyAttribute attr = obj as CloneablePropertyAttribute;
			if (attr != null)
				return (attr.CloneableProperty == this.cloneableProperty);
			return false;
		} 
        
		/// <summary>
		///   <para>Indicates whether a property should be cloned if the assigned object implements ICloneable.</para>
		/// </summary>
		public bool CloneableProperty 
		{ 
			get
			{
				return this.cloneableProperty;
			} 
		}

	
		/// <summary>
		/// Indicates whether the <see cref="CloneablePropertyAttribute"/> has been set for the property.
		/// </summary>
		/// <param name="info">A <see cref="System.Reflection.PropertyInfo"/></param>
		/// <returns>True if property has a <see cref="CloneablePropertyAttribute"/>; false otherwise.</returns>
		public static bool IsCloneableProperty(PropertyInfo info)
		{
			CloneablePropertyAttribute attr = CloneablePropertyAttribute.Default;
			if (info != null && info.IsDefined(typeof(CloneablePropertyAttribute), true))
			{
#if !WinRT
				object[] array = info.GetCustomAttributes(typeof(CloneablePropertyAttribute), true);
				attr = (CloneablePropertyAttribute) array[0];
#else
                IEnumerable<Attribute> array = info.GetCustomAttributes(typeof(SerializePropertyAttribute), true);
                attr = (CloneablePropertyAttribute)array.GetEnumerator().Current;
#endif
			}
			return attr.CloneableProperty;
		}
	}

	/// <summary>
	/// Specifies whether a property should be disposed.
	/// </summary>
	/// <seealso cref="SyncfusionEventArgs"/>
	[AttributeUsageAttribute(AttributeTargets.Property)]
	public sealed class DisposeablePropertyAttribute : Attribute 
	{
		// Fields
		private bool disposeableProperty;
        
		/// <summary>
		///   <para>Specifies that a property should be disposed if the assigned object implements IDisposeable. 
		///   This <see langword="static" /> field is Read-only.</para>
		/// </summary>
		public static readonly DisposeablePropertyAttribute Yes;

		/// <summary>
		///   <para>Specifies that a property should never be disposed even if the assigned object implements IDisposeable
		///   This <see langword="static" /> field is Read-only.</para>
		/// </summary>
		public static readonly DisposeablePropertyAttribute No;

		/// <summary>
		/// The default setting for this attribute.
		/// </summary>
		public static readonly DisposeablePropertyAttribute Default;
        
		// Constructors
        
		static DisposeablePropertyAttribute()
		{
			DisposeablePropertyAttribute.Yes = new DisposeablePropertyAttribute(true);
			DisposeablePropertyAttribute.No = new DisposeablePropertyAttribute(false);
			DisposeablePropertyAttribute.Default = DisposeablePropertyAttribute.Yes;
		} 
        
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="DisposeablePropertyAttribute" /> class.</para>
		/// </summary>
		/// <param name="disposeableProperty">
        ///   <see langword="True" /> if a property should be should be disposed if the assigned object implements IDisposeable; <see langword="False" /> otherwise. The default is <see langword="True"/></param>
		public DisposeablePropertyAttribute(bool disposeableProperty)
		{
			this.disposeableProperty = disposeableProperty;
		} 
        
		// Methods
        
		/// <override/>
		public override int GetHashCode()
		{
			return this.disposeableProperty.GetHashCode();
		} 
        
		/// <override/>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			DisposeablePropertyAttribute attr = obj as DisposeablePropertyAttribute;
			if (attr != null)
				return (attr.DisposeableProperty == this.disposeableProperty);
			return false;
		} 
        
		/// <summary>
		///   <para>Indicates whether a property should be disposed if the assigned object implements IDisposeable.</para>
		/// </summary>
		public bool DisposeableProperty 
		{ 
			get
			{
				return this.disposeableProperty;
			} 
		}

	
		/// <summary>
		/// Indicates whether the <see cref="DisposeablePropertyAttribute"/> has been set for the property.
		/// </summary>
		/// <param name="info">A <see cref="System.Reflection.PropertyInfo"/>.</param>
		/// <returns>True if property has a <see cref="DisposeablePropertyAttribute"/>; false otherwise.</returns>
		public static bool IsDisposeableProperty(PropertyInfo info)
		{
			DisposeablePropertyAttribute attr = DisposeablePropertyAttribute.Default;
			if (info != null && info.IsDefined(typeof(DisposeablePropertyAttribute), true))
			{
#if !WinRT
				object[] array = info.GetCustomAttributes(typeof(DisposeablePropertyAttribute), true);
				attr = (DisposeablePropertyAttribute) array[0];
#else
                IEnumerable<Attribute> array = info.GetCustomAttributes(typeof(SerializePropertyAttribute), true);
                attr = (DisposeablePropertyAttribute)array.GetEnumerator().Current;
#endif
			}
			return attr.DisposeableProperty;
		}
	}

	/// <summary>
	/// Implement this interface if you want to assign this class to a <see cref="StyleInfoBase"/> object's
	/// property and you need to control whether the object should be cloned.
	/// </summary>
	/// <remarks>
	/// This interface is only considered if the <see cref="CloneablePropertyAttribute"/> of
	/// <see cref="StyleInfoProperty.IsCloneable"/> of the <seea cref="StyleInfoProperty"/> is True. 
	/// (This is the default.) 
	/// </remarks>
	/// <seealso cref="StyleInfoProperty.IsCloneable"/> 
	/// <seealso cref="CloneablePropertyAttribute"/>
	public interface IStyleCloneable 
	{
		/// <summary>
		/// Clones this object.
		/// </summary>
		/// <returns>A reference to a clone of this object.</returns>
		object Clone();

		/// <summary>
		/// Disposes this object.
		/// </summary>
		void Dispose();

		/// <summary>
		/// Returns True if this object should be cloned if you assign it to a <see cref="StyleInfoBase"/> object's
		/// property; false otherwise.
		/// </summary>
		/// <returns>True if this object should be cloned if you assign it to a <see cref="StyleInfoBase"/> object's
		/// property; false otherwise.</returns>
		bool ShouldClone();

		/// <summary>
        /// Returns True if this object should be disposed if it is reset in a <see cref="StyleInfoBase"/> object's
		/// property; false otherwise.
		/// </summary>
        /// <returns>True if this object should be disposed if it is reset in a <see cref="StyleInfoBase"/> object's
		/// property; false otherwise.</returns>
		bool ShouldDispose();
	}

    /// <summary>
    /// Implements the <see cref="StyleChanged"/> method which is called from
    /// <see cref="StyleInfoBase.OnStyleChanged"/> of a <see cref="StyleInfoBase"/>
    /// if the object is in <see cref="StyleInfoBase.WeakReferenceChangedListeners"/>
    /// collection.
    /// </summary>
    public interface IStyleChanged
    {
        /// <summary>
        /// Called from <see cref="StyleInfoBase.OnStyleChanged"/> of a 
        /// <see cref="StyleInfoBase"/> object.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Styles.StyleChangedEventArgs"/> instance containing the event data.</param>
        void StyleChanged(StyleChangedEventArgs e);
    }
}
