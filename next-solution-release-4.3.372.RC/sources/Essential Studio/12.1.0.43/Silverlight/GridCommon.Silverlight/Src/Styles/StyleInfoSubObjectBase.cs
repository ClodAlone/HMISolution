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
#if !WinRT
namespace Syncfusion.Windows.Styles
#else
namespace Syncfusion.WinRT.Styles
#endif
{
	/// <summary>
	/// <see cref="StyleInfoSubObjectBase"/> is an abstract base class for classes
	/// to be used as subobjects in a <see cref="StyleInfoBase"/>.
	/// </summary>
	/// <remarks>
	/// <see cref="StyleInfoSubObjectBase"/> is derived from <see cref="StyleInfoBase"/>
	/// and thus provides the same easy way to provide properties that can inherit values
	/// from base styles at run-time.<para/>
	/// The difference is that <see cref="StyleInfoSubObjectBase"/> supports this inheritance
	/// mechanism as a subobject from a <see cref="StyleInfoBase"/>. A subobject needs to
	/// have knowledge about its parent object and be able to walk the base styles from the
	/// parent object.<para/>
	/// Examples for implementation of <see cref="StyleInfoSubObjectBase"/> are the font and border
	/// classes in Essential Grid.<para/>
	/// Programmers can derive their own style classes from <see cref="StyleInfoSubObjectBase"/> 
	/// and add type-safe (and Intelli-sense) 
	/// supported custom properties to the style class. If you write your own 
	/// SpinButton class that needs individual properties, simply add a "CellSpinButtonInfo" 
	/// class as subobject. If you derive CellSpinButtonInfo from StyleInfoSubObjectBase, 
	/// your new object will support property inheritance from base styles.
	/// <para/>
	/// See the overview for <see cref="StyleInfoBase"/> for further discussion about style objects.
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public abstract class StyleInfoSubObjectBase: StyleInfoBase, IStyleInfoSubObject, IDisposable
	{
		StyleInfoBase owner = null;
		StyleInfoProperty sip = null;
		StyleInfoSubObjectIdentity sid = null;

		/// <summary>
		/// Overloaded. Initializes a new <see cref="StyleInfoSubObjectBase"/> object and associates it with an existing <see cref="StyleInfoStore"/>.
		/// </summary>
		/// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.
		/// All changes in this style object will be saved in the <see cref="StyleInfoStore"/> object.
		/// </param>
		protected StyleInfoSubObjectBase(StyleInfoStore store)
			: base(null, store)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="StyleInfoSubObjectBase"/> object and associates it with an existing <see cref="StyleInfoStore"/>.
		/// </summary>
		/// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="StyleInfoBase"/>.
		/// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.</param>
		/// All changes in this style object will be saved in the <see cref="StyleInfoStore"/> object.
		/// </param>
		protected StyleInfoSubObjectBase(StyleInfoSubObjectIdentity identity, StyleInfoStore store)
			: base(identity, store)
		{
			sid = identity as StyleInfoSubObjectIdentity;
			if (sid != null)
			{
				owner = sid.Owner; 
				sip = sid.Sip;
                CacheValues = owner.CacheValues;
			}
		}

		/// <summary>
		/// Returns the <see cref="StyleInfoSubObjectIdentity"/> with identity information about this object.
		/// </summary>
		
		public StyleInfoSubObjectIdentity SubObjectIdentity 
		{
			get
			{
				return this.identity as StyleInfoSubObjectIdentity;
			}
		}

		/// <summary>
		/// Releases all the resources used by the component.
		/// </summary>
		public override void Dispose()
		{
            owner.ClearCache();
			owner = null;
			sip = null;
            sid.Dispose();
			sid = null;
			base.Dispose();
		}


		/// <summary>
		/// Returns a unique identifier for this subobject in the owner style object. 
		/// </summary>
		
		public StyleInfoProperty Sip
		{
			get 
			{
				return sip;
				//return identity != null ? ((StyleInfoSubObjectIdentity) identity).Sip : null; 
			}
		}

		/// <summary>
		/// Returns the data for this object. This is the StyleInfoStore from the constructor.
		/// </summary>
		
		public object Data
		{
			get { return _store; }
		}

		/// <summary>
		/// Returns a reference to the owner style object.
		/// </summary>
		
		public StyleInfoBase Owner 
		{ 
			get 
			{ 
				return owner;
			}
		}

		/// <override/>
		protected override void OnStyleChanged(StyleInfoProperty sip)
		{
			base.OnStyleChanged(sip);
			if (owner != null)
				owner.OnSubObjectChanged(this);
		}

		/// <summary>
		/// Locates the base style that has the specified property and returns its instance.
		/// </summary>
		/// <param name="sip">Identifies the property to look for.</param>
		/// <returns>The style object that has the specified property.</returns>
		protected override StyleInfoBase IntGetDefaultStyleInfo(StyleInfoProperty sip)
		{
			if (identity != null)
				return identity.GetBaseStyle(this, sip);
			return null;
		}
		
		/// <summary>
		/// Makes an exact copy of the current object.
		/// </summary>
		/// <param name="newOwner">The new owner style object for the copied object.</param>
		/// <param name="sip">The identifier for this object.</param>
		/// <returns>A copy of the current object registered with the new owner style object.</returns>
		public virtual IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
		{
			return (IStyleInfoSubObject) Activator.CreateInstance(this.GetType(), 
				new object[] { newOwner.CreateSubObjectIdentity(sip), _store.Clone() });
		}
	}

}
