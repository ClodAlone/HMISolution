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

namespace Syncfusion.Styles
{
	/// <summary>
	/// Allows you to specify a custom name for the StaticData field
	/// in a <see cref="StyleInfoStore"/>. 
	/// </summary>
	[
	AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false),
	] 
	public sealed class StaticDataFieldAttribute : Attribute
	{
        
		// Fields
		private string fieldName;
        
		/// <summary>
		/// Specifies the default field name as "staticDataStore".
		/// </summary>
		public static readonly StaticDataFieldAttribute Default = new StaticDataFieldAttribute("staticDataStore");
        
		// Constructors
        
		/// <summary>
		/// Initializes a new instance of the <see cref="StaticDataFieldAttribute"/> class.
		/// </summary>
		public StaticDataFieldAttribute(string fieldName)
		{
			this.fieldName = fieldName;
		} // end of method .ctor
        
		/// <override/>
		public override /*Attribute*/ bool IsDefaultAttribute()
		{
			return this.Equals(StaticDataFieldAttribute.Default);
		} // end of method IsDefaultAttribute
        
		/// <override/>
		public override /*Attribute*/ int GetHashCode()
		{
			return this.fieldName.GetHashCode();
		} // end of method GetHashCode
        
		/// <override/>
		public override /*Attribute*/ bool Equals(object obj)
		{
			if (obj == this)
				return true;

			StaticDataFieldAttribute sdfa = obj as StaticDataFieldAttribute;
			if (sdfa != null)
				return sdfa.fieldName == fieldName;
			return false;
		} // end of method Equals
        
		/// <summary>
		/// Returns the field name in the <see cref="StyleInfoStore"/> class 
		/// that identifies the static data store.
		/// </summary>
		public string FieldName 
		{ 
			get
			{
				return this.fieldName;
			} 
		}
	} 
} 

