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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;

// using Syncfusion.Windows.Forms.Shared.Drawing;

namespace Syncfusion.Drawing
{
	[Syncfusion.Documentation.DocumentationExclude()]
	sealed class BrushInfoCodeDomSerializer: CodeDomSerializer
	{
		// Methods
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)  
		{
			return null;
		}

		CodeExpression SerializeToExpression(IDesignerSerializationManager manager, BrushInfoColorArrayList gradientColors)
		{
			CodeExpression[] parameters = new CodeExpression[gradientColors.Count];
			for (int n = 0; n < gradientColors.Count; n++)
			{
				parameters[n] = this.SerializeToExpression(manager, gradientColors[n]);
			}

			return new CodeArrayCreateExpression(typeof(System.Drawing.Color[]), parameters);
		}

		public override object Serialize(IDesignerSerializationManager manager, object value)  
		{
			if (manager == null)
				throw new ArgumentNullException(@"manager");

			if (!(value is Syncfusion.Drawing.BrushInfo)
				|| value == null)
				throw new ArgumentException(@"value");

			BrushInfo br = (BrushInfo) value;

			CodeObjectCreateExpression coce = new CodeObjectCreateExpression();
			coce.CreateType = new CodeTypeReference(typeof(BrushInfo)); //"Syncfusion.Drawing.BrushInfo");

			switch (br.Style)
			{
			case BrushStyle.None:
				break;

			case BrushStyle.Solid:
				coce.Parameters.Add(this.SerializeToExpression(manager, br.BackColor));
				break;
                            
			case BrushStyle.Pattern:
				coce.Parameters.Add(this.SerializeToExpression(manager, br.PatternStyle));
				if (br.GradientColors.Count == 2)
				{
					// old format, works with Whidbey
					coce.Parameters.Add(this.SerializeToExpression(manager, br.GradientColors[1]));
					coce.Parameters.Add(this.SerializeToExpression(manager, br.GradientColors[0]));
				}
				else
					coce.Parameters.Add(this.SerializeToExpression(manager, br.GradientColors));
				break;
                            
			case BrushStyle.Gradient:
				coce.Parameters.Add(this.SerializeToExpression(manager, br.GradientStyle));
				if (br.GradientColors.Count == 2)
				{
					// old format, works with Whidbey
					coce.Parameters.Add(this.SerializeToExpression(manager, br.GradientColors[1]));
					coce.Parameters.Add(this.SerializeToExpression(manager, br.GradientColors[0]));
				}
				else
					coce.Parameters.Add(this.SerializeToExpression(manager, br.GradientColors));
				break;
			}

			return coce;
		}

		// Properties
		public static BrushInfoCodeDomSerializer Default
		{
			get 
			{
				if (BrushInfoCodeDomSerializer.defaultSerializer == null) 
					BrushInfoCodeDomSerializer.defaultSerializer = new BrushInfoCodeDomSerializer();
				return BrushInfoCodeDomSerializer.defaultSerializer;
			}
		}

		// Fields
		private static BrushInfoCodeDomSerializer defaultSerializer;

	}
}
