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

namespace Syncfusion.Windows.Forms.Design
{
	/// <summary>
	/// Attribute used to specify the default child type for a parent type.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A parent type designer, say TabControl for example, usually lets you add child types,
	/// TabPage in this case, during design-time. However, when you create custom types deriving
	/// from TabControl and TabPage, the designer needs to be informed about this change in "default child type"
	/// for your derived parent type. This attribute lets you declare this relationship.
	/// </para>
	/// <para>
	/// Some of our components like TabControlExt and XPTaskBar use this attribute to declare
	/// their default child type. Their corresponding designers query this attribute before creating a new child instance. 
	/// This way, when you derive custom types for the above Controls, you
	/// can specify the new default child type using this attribute on your parent type.
	/// </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class DefaultChildTypeAttribute : Attribute
	{
		private Type childType;
		/// <summary>
		/// Creates a new instance of the DefaultChildTypeAttribute specifying the child type.
		/// </summary>
		/// <param name="childType">An Type instance.</param>
		public DefaultChildTypeAttribute(Type childType)
		{
			this.childType = childType;
		}
		/// <summary>
		/// Returns the specified child Type.
		/// </summary>
		public Type ChildType
		{
			get{return this.childType;}
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IAllowMakeDirty
	{
		void SetDirty();
	}
}