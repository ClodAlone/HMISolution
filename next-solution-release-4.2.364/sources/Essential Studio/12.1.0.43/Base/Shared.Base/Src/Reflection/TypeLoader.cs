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
using System.ComponentModel.Design;
using System.Reflection;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using Syncfusion.ComponentModel;
using Syncfusion.Collections;
using System.Windows.Forms;

namespace Syncfusion.Reflection
{
	class TypeNameConverter : TypeConverter
	{
		public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string) ||
				destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		} // end of method CanConvertTo
        
		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
		{
			if(sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if(value is string)
			{
				TypeName typename = new TypeName();
				typename.TypeFullName = (string)value;

				return typename;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string)
				&& value is TypeName)
			{
				TypeName typename = (TypeName)value;
				return typename.TypeFullName;
			}
			else if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
				&& (value is TypeName))
			{
				TypeName typename = (TypeName)value;
				System.Type[] args;
				args = new System.Type[1];
				args[0] = typeof(string);
				
				System.Reflection.ConstructorInfo constructorInfo;
				constructorInfo = typeof(TypeName).GetConstructor(args);
				if (constructorInfo != null)
				{
					object[] argValues;
					argValues = (object[])new System.Object[1];
					argValues[0] = typename.TypeFullName;
					return (object)new InstanceDescriptor(constructorInfo,argValues);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		} // end of method ConvertTo
	}
	/// <summary>
	/// Encapsulates a type's name and exposes it to the <see cref="TypesToLoadList"/> class.
	/// </summary>
	[TypeConverter(typeof(TypeNameConverter)),
	]
	public class TypeName : IChangeNotifyingItem
	{
		/// <summary>
		/// Occurs when the TypeFullName property has changed.
		/// </summary>
		public event SyncfusionPropertyChangedEventHandler PropertyChanged;
		private string typename;
		/// <summary>
		/// Gets / sets the type's full name.
		/// </summary>
		public string TypeFullName
		{
			get{return typename;}
			set
			{
				if(typename != value)
				{
					string oldValue = typename;
					typename = value;
					if(PropertyChanged != null)
					{
						PropertyChanged(this, new SyncfusionPropertyChangedEventArgs(
							PropertyChangeEffect.None, "TypeFullName", oldValue, this.typename));
					}
				}
			}
		}
		/// <overload>
		/// Initializes a new <see cref="TypeName"/> .
		/// </overload>
		/// <summary>
		/// Creates a new TypeName class with empty type name.
		/// </summary>
		public TypeName(){}
		/// <summary>
		/// Creates a new TypeName class and sets its type name.
		/// </summary>
		/// <param name="typename">The full name of the type.</param>
		public TypeName(string typename){this.typename = typename;}
	}

	/// <summary>
	/// Lets you specify a list of <see cref="TypeName"/>s and invoke a member in those types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class is used in Essential Tools to load certain types in memory.
	/// </para>
	/// <para>
	/// Once you add the types to load into this list, you can call the <see cref="Syncfusion.Reflection.TypesToLoadList.InitInvokeMemberSettings"/>
	/// method to specify a member in those types to invoke and then also call <see cref="InvokeMemberOnExisitingTypes"/>
	/// later to repeat the invoke.
	/// </para>
	/// </remarks>
	public class TypesToLoadList : ArrayListExt
	{
		/// <summary>
		/// Gets / sets the indexer for this list.
		/// </summary>
		/// <value>Specifies the <see cref="TypeName"/> object at this index.</value>
		public new TypeName this[int index] 
		{
			get
			{
				return (TypeName)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
		/// <override/>
		protected override void OnItemPropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			if(sender is TypeName && e.PropertyName == "TypeFullName")
			{
				string typename = ((TypeName)sender).TypeFullName;
				if(typename != null && typename.Length > 0)
					InvokeMemberOnType(((TypeName)sender));	
			}
			base.OnItemPropertyChanged(sender, e);
		}
		/// <override/>
		protected override void OnCollectionChanged(CollectionChangeEventArgs args)
		{
			if(args.Action == CollectionChangeAction.Add
				&& args.Element is TypeName)
			{
				string typename = ((TypeName)args.Element).TypeFullName;
				if(typename != null && typename.Length > 0)
					InvokeMemberOnType((TypeName)args.Element);	
				else
				{
					TypeName typeName = args.Element as TypeName;
					if(typeName != null && typeName.TypeFullName == null)
						typeName.TypeFullName = String.Empty;
				}
			}
			base.OnCollectionChanged(args);
		}

		private string memberName;
		private BindingFlags invokeAttr;
		private Binder binder;
		private object[] args;
		private ParameterModifier[] modifiers;
		private CultureInfo culture;
		private string[] namedParameters;

		/// <summary>
		/// Returns the number of arguments to be used in the method call when invoked.
		/// </summary>
		/// <remarks>
		/// This will be zero if invoking a property. Call <see cref="Syncfusion.Reflection.TypesToLoadList.InitInvokeMemberSettings"/>
		/// to reset this property.
		/// </remarks>
		public int ArgsCount
		{
			get{return args.Length;}
		}

		/// <summary>
        /// Returns the argument at the specified index, that will be used during invoking.
		/// </summary>
		/// <param name="i">The argument index.</param>
		/// <returns>The argument at the specified index. NULL if index is out of range.</returns>
		public object GetArg(int i)
		{
			if(i < args.Length)
				return args[i];
			return null;
		}

		/// <summary>
		/// Returns the static member name to invoke.
		/// </summary>
		public string InvokeMemberName
		{
			get{return memberName;}
            set { this.memberName = value; }
		}

		/// <summary>
		/// Call this method to provide information for the member invoke.
		/// </summary>
		/// <param name="memberName"></param>
		/// <param name="invokeAttr"></param>
		/// <param name="binder"></param>
		/// <param name="args"></param>
		/// <param name="modifiers"></param>
		/// <param name="culture"></param>
		/// <param name="namedParameters"></param>
		/// <remarks>
		/// Take a look at <see cref="System.Type.InvokeMember(string, System.Reflection.BindingFlags,
		/// System.Reflection.Binder, object, object[], System.Reflection.ParameterModifier [], 
		/// System.Globalization.CultureInfo, string[])"/> method for information
		/// on these parameters. This method will also call <see cref="InvokeMemberOnExisitingTypes"/>.
		/// </remarks>
		public void InitInvokeMemberSettings( string memberName,
			BindingFlags invokeAttr,
			Binder binder,
			object[] args,
			ParameterModifier[] modifiers,
			CultureInfo culture,
			string[] namedParameters
			)
		{
			this.memberName = memberName;
			this.invokeAttr = invokeAttr;
			this.binder = binder;
			this.args = args;
			this.modifiers = modifiers;
			this.culture = culture;
			this.namedParameters = namedParameters;

			InvokeMemberOnExisitingTypes();
		}

		/// <summary>
		/// Invokes the member specified using <see cref="Syncfusion.Reflection.TypesToLoadList.InitInvokeMemberSettings"/> on the
		/// specified types in this list.
		/// </summary>
		public void InvokeMemberOnExisitingTypes()
		{
			foreach(TypeName typeName in this)
			{
				string typename = typeName.TypeFullName;
				if(typename != null && typename.Length > 0)
					InvokeMemberOnType(typeName);
			}
		}

		/// <summary>
		/// Invokes the member on each type.
		/// </summary>
		/// <param name="typename">The <see cref="TypeName"/> on which to invoke.</param>
		protected void InvokeMemberOnType(TypeName typename)
		{
			if(this.memberName == null
				|| typename.TypeFullName.IndexOf(TypeLoader.UnresolvedType) != -1)
				return;

			Type type = Type.GetType(typename.TypeFullName);
			if(type != null && this.memberName != null && this.memberName.Length > 0)
			{
				try
				{
					type.InvokeMember(this.memberName, 
						this.invokeAttr, this.binder, type, this.args,
						this.modifiers, this.culture, this.namedParameters);
				}
				catch{}
			}
			else
			{
				if(this.TypeNotFound != null)
				{
					this.TypeNotFound(this, new TypeNotFoundEventArgs(typename));
				}
			}
		}
		/// <summary>
		/// Fired when a type to invoke is not found.
		/// </summary>
		public event TypeNotFoundEventHandler TypeNotFound;
	}
    ///<exclude/>
	/// <summary>
	/// Handles the <see cref="TypesToLoadList.TypeNotFound"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="TypeNotFoundEventArgs"/> that contains the event data.</param>
	public delegate void TypeNotFoundEventHandler(object sender, TypeNotFoundEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="TypesToLoadList.TypeNotFound"/> event.
	/// </summary>
	public class TypeNotFoundEventArgs : EventArgs
	{
		private TypeName typeName;
		/// <summary>
		/// Creates a new instance of the TypeNotFoundEventArgs.
		/// </summary>
		/// <param name="typeName">The <see cref="TypeName"/> that was not found.</param>
		public TypeNotFoundEventArgs(TypeName typeName)
		{
			this.typeName = typeName;
		}
		/// <summary>
		/// Returns the <see cref="TypeName"/> that was not found.
		/// </summary>
		public TypeName InvalidTypeName
		{
			get{return this.typeName;}
		}
	}

    /// <exclude/>
	/// <summary>
	/// This component lets you load custom types into the design time.
	/// </summary>
    /// <remarks>
    /// You can specify the static member name you want invoked through the InvokeMemberName property. This will load the types and
    /// invoke the specified member every time you load the designer.
    /// </remarks>
	[
	// Specifying the PopupControlContainer class because that is what has the same namespace
	// as the default namespace of this assembly.
	System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.TypeLoader.bmp"),
	Description("A component lets you load custom types into the design time.")
	]
	public class TypeLoader : Component
	{
		// This class doesn't derive from ArrayList directly because of designer plugin convenience.
		// (Otherwise need to write a custom TypeConverter)
		// and also it needs to be a component for designer plugin.

		internal static string UnresolvedType =  " (Unresolved type)";
		private TypesToLoadList typesList = new TypesToLoadList();

		/// <overload>
		/// Initializes a new <see cref="TypeLoader"/> .
		/// </overload>
		/// <summary>
		/// Creates a new instance of the TypeLoader class.
		/// </summary>
		public TypeLoader()
		{
			typesList.TypeNotFound += new TypeNotFoundEventHandler(this.OnTypeNotFound);
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(TypeLoader));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
		}

		/// <summary>
		/// Creates a new instance of the TypeLoader class and adds itself to the container specified.
		/// </summary>
		/// <param name="container">The container to add to.</param>
		public TypeLoader(IContainer container)
		{
			if(container != null)
				container.Add(this);
		}

        /// <summary>
        /// Returns the static member name to invoke.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Syncfusion.Reflection.TypesToLoadList.InitInvokeMemberSettings"/>
        /// to reset this property.
        /// </remarks>
        [DefaultValue("")]
        [Description("Returns the static member name to invoke.")]
        public string InvokeMemberName
        {
            get 
            {
                return this.typesList.InvokeMemberName;
            }
            set
            {
                this.typesList.InvokeMemberName = value;
            }
        }

		/// <summary>
		/// Handler for the <see cref="Syncfusion.Reflection.TypesToLoadList.TypeNotFound"/> event.
		/// </summary>
		/// <param name="sender">The sender of this event.</param>
		/// <param name="args">Data for this event.</param>
		/// <remarks>
		/// If in design mode, this method shows a message box with appropriate information.
		/// </remarks>
		protected void OnTypeNotFound(object sender, TypeNotFoundEventArgs args)
		{
			if(this.DesignMode)
			{
				string typeFullName = args.InvalidTypeName.TypeFullName;
				typeFullName = typeFullName.Replace(UnresolvedType, String.Empty);				

				MessageBox.Show("Cannot resolve " + typeFullName + " to a valid type. Make sure the corresponding assembly is referenced in your project."
					, "TypeLoader Design Time Notification:");
				if(args.InvalidTypeName.TypeFullName.IndexOf(UnresolvedType) == -1)
				{
					typesList.SuspendEvents();
					args.InvalidTypeName.TypeFullName += UnresolvedType;
					typesList.ResumeEvents(false);
				}
			}
		}
		/// <summary>
		/// Specifies the <see cref="TypesToLoadList"/> containing the list of <see cref="TypeName"/>s 
		/// to load.
		/// </summary>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
        Description("Specifies the TypesToLoadList containing the list of TypeNames.")
		]
		public TypesToLoadList TypesToLoadList
		{
			get{return typesList;}
		}
	}
}

