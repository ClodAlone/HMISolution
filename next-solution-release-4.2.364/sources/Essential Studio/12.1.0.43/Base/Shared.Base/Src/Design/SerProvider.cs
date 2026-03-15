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
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.CodeDom;
using System.Reflection;

namespace Syncfusion.ComponentModel.Design.Serialization
{
	// When you use this IDesignerSerializationProvider to serialize your types,
	// this will insert an IContainer argument to the base class provided Constructor, in code.
	// Usage Example:
	//	public class YourComponentDesigner : ComponentDesigner
	//	{
	//		...
	//		ContainerInsertingSerializationProvider provider;
	//		IDesignerSerializationManager idsm;
	//		public override void Initialize(IComponent component)
	//		{
	//			this.provider = new ContainerInsertingSerializationProvider(typeof(YourComponent), true); // true to do this for derived classes as well.
	//			idsm = (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
	//			if(this.idsm != null)
	//				this.idsm.AddSerializationProvider(this.provider);
	//		}
	//		protected override void Dispose(bool disposing)
	//		{
	//			if(this.idsm != null)
	//				serManager.RemoveSerializationProvider(this.provider);
	//		}
	//		...
	//	}
	//
	//	Also, you can optionally control the param-insertion by providing the following field:
	//	protected bool insertContainerWhileSerializing = true;
	//	in the corresponding type, which will be queried using reflection.
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ContainerInsertingSerializationProvider : IDesignerSerializationProvider
	{
		ContainerInsertingCodeDomSerializer serializer;
		Type sourceType;
		bool applyOnDerivedClasses = false;
		public ContainerInsertingSerializationProvider(Type sourceType, bool applyOnDerivedClasses)
		{
			this.serializer = new ContainerInsertingCodeDomSerializer();
			this.sourceType = sourceType;
			this.applyOnDerivedClasses = applyOnDerivedClasses;
		}

		public virtual object GetSerializer(IDesignerSerializationManager manager, object currentSerializer,
			Type objectType, Type serializerType)
		{
			if( (objectType != null) && ((objectType == this.sourceType)
				|| (this.applyOnDerivedClasses && (objectType.IsSubclassOf(this.sourceType)))) )
				return this.serializer;

			return null;
		}

		public void Dispose()
		{
			this.serializer = null;
		}
	}

	// This serializer will add a "container" parameter to the incoming constructor, if the
	// resultant constructor is available in the incoming type.
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ContainerInsertingCodeDomSerializer : CodeDomSerializer
	{
		public ContainerInsertingCodeDomSerializer()
		{
		}

		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			CodeDomSerializer baseClassSerializer =
				(CodeDomSerializer)manager.GetSerializer(typeof(Control), typeof(CodeDomSerializer));

			return baseClassSerializer.Deserialize(manager, codeObject);
		}

		public override object Serialize(IDesignerSerializationManager manager, object obj)
		{
			CodeDomSerializer baseClassSerializer =
				(CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));

			object serializedInfo = baseClassSerializer.Serialize(manager, obj);

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if(serializedInfo is CodeStatementCollection && this.ContinueInsertion(obj))
			{
				CodeStatementCollection stmtColl = serializedInfo as CodeStatementCollection;
				if(stmtColl.Count > 0)
				{
					CodeAssignStatement stmt = stmtColl[0] as CodeAssignStatement;
					if(stmt != null)
					{
						// If the first one is the constructor.
						CodeObjectCreateExpression createExpr = stmt.Right as CodeObjectCreateExpression;
						if(createExpr != null)
						{
							CodeFieldReferenceExpression containerFieldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "components");
							createExpr.Parameters.Insert(0, containerFieldRef);
							this.EnsureContainer(manager);
						}
					}
				}
			}
#endif

			return serializedInfo;
		}
		// This will check for an InsertContainerWhileSerializing property in the object and
		// query its value before continuing.
		private bool ContinueInsertion(object value)
		{
			FieldInfo fInfo = value.GetType().GetField("insertContainerWhileSerializing", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
			if(fInfo != null)
			{
				return (bool)fInfo.GetValue(value);
			}

			return true;
		}

		private void EnsureContainer(IDesignerSerializationManager manager)
		{
			Type type = Type.GetType("System.ComponentModel.Design.Serialization.RootCodeDomSerializer");
			if(type != null)
			{
				object rootCodeDomSerializer = manager.Context[type];
				if(rootCodeDomSerializer != null)
				{
					PropertyDescriptor propDesc = TypeDescriptor.GetProperties(rootCodeDomSerializer)["ContainerRequired"];
					propDesc.SetValue(rootCodeDomSerializer, true);
				}
			}
		}
	}

}