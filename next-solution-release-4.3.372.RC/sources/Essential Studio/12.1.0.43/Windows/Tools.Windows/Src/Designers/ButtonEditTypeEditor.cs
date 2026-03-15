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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Resources;
using System.Reflection;

using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// UITypeEditor for <see cref="ButtonEdit"/> class.
    /// </summary>
    public class ButtonEditTypeEditor : UITypeEditor 
    {
		/// <summary>
		///    <para>
		///       Indicates whether this editor supports the painting of a representation
		///       of an object's value.
		///    </para>
		/// </summary>
		/// <param name="context">
		///    An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to provide additional context information.
		/// </param>
		/// <returns>
		///    <para>
		///    <see langword="true"/> if PaintValue is implemented;
        ///    <see langword="false"/> otherwise.
		///    </para>
		/// </returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)  
        {
            return true;
        }

		/// <overload>
		///    <para>Paints a representative value of the specified object to the specified canvas.</para>
		/// </overload>
		/// <summary>
		///    <para>Paints a representative value of the specified object to the
		///       specified canvas.</para>
		/// </summary>
		/// <param name='pe'>A drawing canvas to paint the value's representation on.</param>
		/// <remarks>
		///    <para> Painting will occur within the boundaries of the specified rectangle.</para>
		/// </remarks>
		public override void PaintValue(PaintValueEventArgs pe)  
		{
			ResourceManager resources = new ResourceManager("ButtonEdit.ButtonEditIcons", Assembly.GetExecutingAssembly());

			Bitmap b = null;
			ButtonTypes button = (ButtonTypes) pe.Value;

			switch(button)
			{
				case ButtonTypes.Calculator:
					b = (Bitmap) ButtonEdit.GetImage("Calculator.bmp");
					break;

				case ButtonTypes.Check:
					b = (Bitmap) ButtonEdit.GetImage("Check.bmp");
					break;

				case ButtonTypes.Currency:
					b = (Bitmap) ButtonEdit.GetImage("Currency.bmp");
					
					break;

				case ButtonTypes.Down:
					b = (Bitmap) ButtonEdit.GetImage("Down.bmp");
					
					break;

				case ButtonTypes.Left:
					b = (Bitmap) ButtonEdit.GetImage("Left.bmp");
					break;


				case ButtonTypes.Browse:
					b = (Bitmap) ButtonEdit.GetImage("Browse.bmp");
					break;


				case ButtonTypes.Redo :
					b = (Bitmap) ButtonEdit.GetImage("Redo.bmp");
					break;

				case ButtonTypes.Right :
					b = (Bitmap) ButtonEdit.GetImage("Right.bmp");
					break;

				case ButtonTypes.Undo:
					b = (Bitmap) ButtonEdit.GetImage("Undo.bmp");
					break;

				case ButtonTypes.Up:
					b = (Bitmap) ButtonEdit.GetImage("Up.bmp");
					break;

                case ButtonTypes.LeftEnd:
                    b = (Bitmap)ButtonEdit.GetImage("Leftend.bmp");
                    break;

                case ButtonTypes.RightEnd:
                    b = (Bitmap)ButtonEdit.GetImage("Rightend.bmp");
                    break;

				default:
					b = (Bitmap) ButtonEdit.GetImage("Browse.bmp");
					break;
			}
			pe.Graphics.DrawImage(b, pe.Bounds);
			b.Dispose();
			resources.ReleaseAllResources();
        }

    }
}

