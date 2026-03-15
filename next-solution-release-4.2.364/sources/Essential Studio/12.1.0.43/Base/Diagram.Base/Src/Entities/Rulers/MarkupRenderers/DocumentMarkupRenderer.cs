#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Markup in Document
	/// </summary>
	public class DocumentMarkupRenderer
		: RulerMarkupRenderer
	{
		#region Class properies
		/// <summary>
		/// Gets minimum particle of the ruler.
		/// </summary>
		public override float Precision
		{
			get
			{
				return 10F;
			}
		}

		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// Creates instance of the <see cref="Syncfusion.Windows.Forms.Diagram.DocumentMarkupRenderer"/> with
		/// given <see cref="Syncfusion.Windows.Forms.Diagram.Ruler"/> to render markup to.
		/// </summary>
		/// <param name="ruler"><see cref="Syncfusion.Windows.Forms.Diagram.Ruler"/> to render markup to.</param>
		public DocumentMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
		}
		#endregion
	}
}
