#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// This class implements a design-time editor for opening a file containing
    /// a diagram document.
    /// </summary>
    /// <remarks>
    /// Use [Editor(typeof(PaletteOpener), typeof(System.Drawing.Design.UITypeEditor))] attribute.
    /// </remarks>
    public class DiagramOpener : UITypeEditor
    {
        private System.ComponentModel.TypeConverter typeConverter = new DiagramDocumentConverter();

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
            openDlg.Filter = "Essential diagram document files (*.edd)|*.edd|All files (*.*)|*.*";
            openDlg.DefaultExt = "edd";
            openDlg.Title = "Open diagram document";

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                // call component changing to generate design code
                context.OnComponentChanging();
                
                // set new value
                value = this.typeConverter.ConvertFrom(context, null, openDlg.FileName);
                
                // call component changed to generate design code
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
