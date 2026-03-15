#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// This class implements a design-time editor for opening a file containing
    /// a symbol palette.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is used by the PaletteGroupView to allow the user to load a
    /// symbol palette from the disk. This class can be used as the design-time editor
    /// for any property of type SymbolPalette using the following code:
    /// <code>
    /// [Editor(typeof(PaletteOpener), typeof(System.Drawing.Design.UITypeEditor))]
    /// </code>
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteGroupView"/>
    /// <seealso cref="SymbolPalette"/>
    /// </remarks>
    public class PaletteOpener : UITypeEditor
    {
        private System.ComponentModel.TypeConverter typeConverter = new SymbolPaletteConverter();

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Symbol palette files (*.edp)|*.edp|All files (*.*)|*.*";
            openDlg.DefaultExt = "edp";
            openDlg.Title = "Open symbol palette";
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                // call context chaging method to generate design code
                context.OnComponentChanging();
                value = this.typeConverter.ConvertFrom(context, null, openDlg.FileName);
                context.OnComponentChanged();
            }
            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
