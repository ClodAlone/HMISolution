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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region *** RibbonControlAdvHeaderSerializer
	/// <summary>
	/// Serializer for RibbonControlAdvHeader.
	/// </summary>
	class RibbonControlAdvHeaderSerializer : CodeDomSerializer
	{
		#region Overrides
		/// <summary>
		/// Serializes quick and main items.
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			object result = null;

			RibbonControlAdvHeader header = value as RibbonControlAdvHeader;
			if (header != null)
			{
				ExpressionContext context = manager.Context.Current as ExpressionContext;
				if (context != null)
				{
					CodeStatementCollection statements = new CodeStatementCollection();

					CodeExpression targetProperty = context.Expression;

					foreach (ToolStripItem item in header.MainItems)
					{
						string sItem = manager.GetName(item);
						if (sItem != null)
						{
							CodeExpression itemExp = new CodeVariableReferenceExpression(sItem);
							CodeStatement addExp = new CodeExpressionStatement(new CodeMethodInvokeExpression(targetProperty, "AddMainItem", new CodeExpression[] { itemExp }));
							statements.Add(addExp);
						}
					}
					foreach (ToolStripItem quickItem in header.QuickItems)
					{
						IQuickItem item = quickItem as IQuickItem;
						if (item != null)
						{
							string sItem = manager.GetName(item.ReflectedComponent);
							if (sItem != null)
							{
								CodeExpression itemExp = new CodeVariableReferenceExpression(sItem);
								CodeExpression quickItemExp = new CodeObjectCreateExpression(item.GetType(), new CodeExpression[] { itemExp });
								CodeStatement addExp = new CodeExpressionStatement(new CodeMethodInvokeExpression(targetProperty, "AddQuickItem", quickItemExp));
								statements.Add(addExp);
							}
						}
					}

					result = statements;
				}
			}

			return result;
		}
		#endregion
	}
	#endregion
}
#endif
