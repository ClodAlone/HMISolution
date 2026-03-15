#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Property editor for default layer name in a model.
    /// </summary>
    /// <remarks>
    /// This custom property editor displays a combo box filled with the
    /// collection of layers available in a model.
    /// </remarks>
    public class LayerNameEditor : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerNameEditor"/> class.
        /// </summary>
        public LayerNameEditor()
        {
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
            return UITypeEditorEditStyle.DropDown;
        }

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
            this.edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (this.edSvc != null)
            {
                ListBox listBox = new ListBox();
                listBox.SelectedIndexChanged += new System.EventHandler(OnValueSelected);

                Model mdl = context.Instance as Model;

                if (mdl != null)
                {
                    foreach (Layer curLayer in mdl.Layers)
                    {
                        string curItemValue = curLayer.Name;
                        int curIndex = listBox.Items.Add(curItemValue);
                        if (value != null && ((string)value) == curItemValue)
                        {
                            listBox.SelectedIndex = curIndex;
                        }
                    }
                }

                edSvc.DropDownControl(listBox);

                if (listBox.SelectedIndex >= 0 && listBox.SelectedIndex < listBox.Items.Count)
                {
                    value = listBox.Items[listBox.SelectedIndex];
                }
                else
                {
                    value = string.Empty;
                }
            }
            return value;
        }

        /// <summary>
        /// Indicates whether the specified context supports painting a representation of an object's value within the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"/> is implemented; otherwise, false.
        /// </returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        private void OnValueSelected(object sender, System.EventArgs evtArgs)
        {
            this.edSvc.CloseDropDown();
        }
    }
}
