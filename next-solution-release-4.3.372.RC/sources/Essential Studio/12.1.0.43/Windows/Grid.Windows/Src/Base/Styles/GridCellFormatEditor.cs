//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellFormatEditor.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridCellFormatEditor : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc = null;

        public struct FormatMask
        {
            string mask;
            string description;
            Type type;

            public FormatMask(Type type, string mask, string description)
            {
                this.type = type;
                this.mask = mask;
                this.description = description;
            }

            /// <summary>
            /// Gets or sets this is the format string.
            /// </summary>
            public string Mask
            {
                get
                {
                    return mask;
                }

                set
                {
                    mask = value;
                }
            }

            /// <summary>
            /// Gets or sets this is the format description.
            /// </summary>
            public string Description
            {
                get
                {
                    return description;
                }

                set
                {
                    description = value;
                }
            }

            /// <summary>
            /// Gets or sets this is the type.
            /// </summary>
            public Type Type
            {
                get
                {
                    return type;
                }

                set
                {
                    type = value;
                }
            }

            public static bool IsSafeWideningConversion(Type target, Type source)
            {
                if (target == null)
                {
                    throw new ArgumentNullException("target");
                }

                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                if (source == typeof(byte))
                {
                    return target == typeof(char)
                        || target == typeof(ushort)
                        || target == typeof(short)
                        || target == typeof(uint)
                        || target == typeof(int)
                        || target == typeof(ulong)
                        || target == typeof(long)
                        || target == typeof(float)
                        || target == typeof(double)
                        || target == typeof(decimal)
                        || target == typeof(object);
                }
                else if (source == typeof(sbyte))
                {
                    return target == typeof(short)
                         || target == typeof(int)
                         || target == typeof(long)
                         || target == typeof(float)
                         || target == typeof(double)
                         || target == typeof(decimal)
                         || target == typeof(object);
                }
                else if (source == typeof(short))
                {
                    return target == typeof(int)
                         || target == typeof(long)
                         || target == typeof(float)
                         || target == typeof(double)
                         || target == typeof(decimal)
                         || target == typeof(object);
                }
                else if (source == typeof(ushort))
                {
                    return target == typeof(uint)
                        || target == typeof(int)
                        || target == typeof(ulong)
                        || target == typeof(long)
                        || target == typeof(float)
                        || target == typeof(double)
                        || target == typeof(decimal)
                        || target == typeof(object);
                }
                else if (source == typeof(char))
                {
                    return target == typeof(ushort)
                         || target == typeof(short)
                         || target == typeof(uint)
                         || target == typeof(int)
                         || target == typeof(ulong)
                         || target == typeof(long)
                         || target == typeof(float)
                         || target == typeof(double)
                         || target == typeof(decimal)
                         || target == typeof(object);
                }
                else if (source == typeof(int))
                {
                    return target == typeof(long)
                        || target == typeof(float)
                        || target == typeof(double)
                        || target == typeof(decimal)
                        || target == typeof(object);
                }
                else if (source == typeof(uint))
                {
                    return target == typeof(ulong)
                      || target == typeof(long)
                      || target == typeof(float)
                      || target == typeof(double)
                      || target == typeof(decimal)
                      || target == typeof(object);
                }
                else if (source == typeof(long))
                {
                    return target == typeof(float)
                         || target == typeof(double)
                         || target == typeof(decimal)
                         || target == typeof(object);
                }
                else if (source == typeof(ulong))
                {
                    return target == typeof(float)
                         || target == typeof(double)
                         || target == typeof(decimal)
                         || target == typeof(object);
                }
                else if (source == typeof(float))
                {
                    return target == typeof(double)
                        || target == typeof(object);
                }
                else if (source == typeof(double))
                {
                    return target == typeof(object);
                }
                else if (source == typeof(decimal))
                {
                    return target == typeof(object);
                }

                return false;
            }
        }

        private static FormatMask[] formatMasks = 
        {
            new FormatMask(typeof(byte), "C", "NumberFormat_C"),
            new FormatMask(typeof(byte), "D", "NumberFormat_D"),
            new FormatMask(typeof(byte), "E", "NumberFormat_E"),
            new FormatMask(typeof(byte), "F", "NumberFormat_F"),
            new FormatMask(typeof(byte), "G", "NumberFormat_G"),
            new FormatMask(typeof(byte), "N", "NumberFormat_N"),
            new FormatMask(typeof(byte), "P", "NumberFormat_P"),
            new FormatMask(typeof(byte), "X", "NumberFormat_X"),
            new FormatMask(typeof(ushort), "C", "NumberFormat_C"),
            new FormatMask(typeof(ushort), "D", "NumberFormat_D"),
            new FormatMask(typeof(ushort), "E", "NumberFormat_E"),
            new FormatMask(typeof(ushort), "F", "NumberFormat_F"),
            new FormatMask(typeof(ushort), "G", "NumberFormat_G"),
            new FormatMask(typeof(ushort), "N", "NumberFormat_N"),
            new FormatMask(typeof(ushort), "P", "NumberFormat_P"),
            new FormatMask(typeof(ushort), "X", "NumberFormat_X"),
            new FormatMask(typeof(short), "C", "NumberFormat_C"),
            new FormatMask(typeof(short), "D", "NumberFormat_D"),
            new FormatMask(typeof(short), "E", "NumberFormat_E"),
            new FormatMask(typeof(short), "F", "NumberFormat_F"),
            new FormatMask(typeof(short), "G", "NumberFormat_G"),
            new FormatMask(typeof(short), "N", "NumberFormat_N"),
            new FormatMask(typeof(short), "P", "NumberFormat_P"),
            new FormatMask(typeof(short), "X", "NumberFormat_X"),
            new FormatMask(typeof(uint), "C", "NumberFormat_C"),
            new FormatMask(typeof(uint), "D", "NumberFormat_D"),
            new FormatMask(typeof(uint), "E", "NumberFormat_E"),
            new FormatMask(typeof(uint), "F", "NumberFormat_F"),
            new FormatMask(typeof(uint), "G", "NumberFormat_G"),
            new FormatMask(typeof(uint), "N", "NumberFormat_N"),
            new FormatMask(typeof(uint), "P", "NumberFormat_P"),
            new FormatMask(typeof(uint), "X", "NumberFormat_X"),
            new FormatMask(typeof(int), "C", "NumberFormat_C"),
            new FormatMask(typeof(int), "D", "NumberFormat_D"),
            new FormatMask(typeof(int), "E", "NumberFormat_E"),
            new FormatMask(typeof(int), "F", "NumberFormat_F"),
            new FormatMask(typeof(int), "G", "NumberFormat_G"),
            new FormatMask(typeof(int), "N", "NumberFormat_N"),
            new FormatMask(typeof(int), "P", "NumberFormat_P"),
            new FormatMask(typeof(int), "X", "NumberFormat_X"),
            new FormatMask(typeof(ulong), "C", "NumberFormat_C"),
            new FormatMask(typeof(ulong), "D", "NumberFormat_D"),
            new FormatMask(typeof(ulong), "E", "NumberFormat_E"),
            new FormatMask(typeof(ulong), "F", "NumberFormat_F"),
            new FormatMask(typeof(ulong), "G", "NumberFormat_G"),
            new FormatMask(typeof(ulong), "N", "NumberFormat_N"),
            new FormatMask(typeof(ulong), "P", "NumberFormat_P"),
            new FormatMask(typeof(ulong), "X", "NumberFormat_X"),
            new FormatMask(typeof(long), "C", "NumberFormat_C"),
            new FormatMask(typeof(long), "D", "NumberFormat_D"),
            new FormatMask(typeof(long), "E", "NumberFormat_E"),
            new FormatMask(typeof(long), "F", "NumberFormat_F"),
            new FormatMask(typeof(long), "G", "NumberFormat_G"),
            new FormatMask(typeof(long), "N", "NumberFormat_N"),
            new FormatMask(typeof(long), "P", "NumberFormat_P"),
            new FormatMask(typeof(long), "X", "NumberFormat_X"),
            new FormatMask(typeof(float), "C", "NumberFormat_C"),
            new FormatMask(typeof(float), "E", "NumberFormat_E"),
            new FormatMask(typeof(float), "F", "NumberFormat_F"),
            new FormatMask(typeof(float), "G", "NumberFormat_G"),
            new FormatMask(typeof(float), "N", "NumberFormat_N"),
            new FormatMask(typeof(float), "P", "NumberFormat_P"),
            new FormatMask(typeof(float), "R", "NumberFormat_R"),
            new FormatMask(typeof(double), "C", "NumberFormat_C"),
            new FormatMask(typeof(double), "E", "NumberFormat_E"),
            new FormatMask(typeof(double), "F", "NumberFormat_F"),
            new FormatMask(typeof(double), "G", "NumberFormat_G"),
            new FormatMask(typeof(double), "N", "NumberFormat_N"),
            new FormatMask(typeof(double), "P", "NumberFormat_P"),
            new FormatMask(typeof(double), "R", "NumberFormat_R"),
            new FormatMask(typeof(decimal), "C", "NumberFormat_C"),
            new FormatMask(typeof(decimal), "E", "NumberFormat_E"),
            new FormatMask(typeof(decimal), "F", "NumberFormat_F"),
            new FormatMask(typeof(decimal), "G", "NumberFormat_G"),
            new FormatMask(typeof(decimal), "N", "NumberFormat_N"),
            new FormatMask(typeof(decimal), "P", "NumberFormat_P"),
            new FormatMask(typeof(DateTime), "d", "DateTimeFormat_dd"),
            new FormatMask(typeof(DateTime), "D", "DateTimeFormat_D"),
            new FormatMask(typeof(DateTime), "f", "DateTimeFormat_ff"),
            new FormatMask(typeof(DateTime), "F", "DateTimeFormat_F"),
            new FormatMask(typeof(DateTime), "g", "DateTimeFormat_gg"),
            new FormatMask(typeof(DateTime), "G", "DateTimeFormat_G"),
            new FormatMask(typeof(DateTime), "M", "DateTimeFormat_M"),
            new FormatMask(typeof(DateTime), "R", "DateTimeFormat_R"),
            new FormatMask(typeof(DateTime), "s", "DateTimeFormat_ss"),
            new FormatMask(typeof(DateTime), "t", "DateTimeFormat_tt"),
            new FormatMask(typeof(DateTime), "T", "DateTimeFormat_T"),
            new FormatMask(typeof(DateTime), "u", "DateTimeFormat_uu"),
            new FormatMask(typeof(DateTime), "U", "DateTimeFormat_U"),
            new FormatMask(typeof(DateTime), "Y", "DateTimeFormat_Y"),
            new FormatMask(typeof(System.Enum), "G", "EnumFormat_G"),
            new FormatMask(typeof(System.Enum), "X", "EnumFormat_X"),
            new FormatMask(typeof(System.Enum), "D", "EnumFormat_D"),
            new FormatMask(typeof(object), "G", "GeneralFormat_G"), 
        };

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        /// <override/>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            return base.GetEditStyle(context);
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
        /// <override/>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            FormatUI ef;
            if (context == null || context.Instance == null || provider == null)
            {
                return value;
            }

            this.edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (this.edSvc == null)
            {
                return value;
            }

            ef = new FormatUI(this, context);
            ef.EditValue = value;
            this.edSvc.DropDownControl(ef);
            value = ef.EditValue;
            return value;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private class FormatUI : Form
        {
            // Fields
            private GridCellFormatEditor mainEditor;
            private ListView listView;
            private object editValue;
            private object originalValue;
            private ITypeDescriptorContext context;

            /// <override/>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    listView.Click -= new System.EventHandler(this.listView1_Click);
                    this.Controls.Remove(listView);
                    editValue = null;
                    originalValue = null;
                    listView = null;
                    mainEditor = null;
                }

                base.Dispose(disposing);
            }

            // ctor
            public FormatUI(GridCellFormatEditor editor, ITypeDescriptorContext context)
            {
                this.mainEditor = editor;
                this.context = context;

                this.StartPosition = FormStartPosition.WindowsDefaultBounds;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.TopLevel = false;
                this.ShowInTaskbar = false;
                this.TopMost = true;
                this.listView = new ListView();
                this.listView.Dock = DockStyle.Fill;
                this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;

                this.listView.FullRowSelect = true;
                this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
                this.listView.Location = new System.Drawing.Point(8, 16);
                this.listView.MultiSelect = false;
                this.listView.Name = "listView";
                this.listView.Size = new System.Drawing.Size(272, 248);
                this.listView.TabIndex = 0;
                this.listView.View = System.Windows.Forms.View.Details;
                ////this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
                this.listView.Click += new System.EventHandler(this.listView1_Click);

                ColumnHeader hdrMask = new ColumnHeader();
                hdrMask.Text = "Mask";
                ColumnHeader hdrDescription = new ColumnHeader();
                hdrDescription.Text = "Description";
                ////hdrDescription.Width = 250;

                Type cellValueType = null;
                if (context != null)
                {
                    GridStyleInfo style = context.Instance as GridStyleInfo;
                    cellValueType = style.CellValueType;
                }

                if (cellValueType == null)
                {
                    cellValueType = typeof(object);
                }

                int minWidth = 0;
                int height = 0;
                Graphics g = this.CreateGraphics();
                Hashtable used = new Hashtable();
                foreach (FormatMask formatEntry in formatMasks)
                {
                    if (used.Contains(formatEntry.Mask))
                    {
                        continue;
                    }

                    if (cellValueType == formatEntry.Type ||
                        formatEntry.Type.IsAssignableFrom(cellValueType))                 
                    { 
                        ////||
                        /*FormatMask.IsSafeWideningConversion(formatEntry.Type, cellValueType)*/      
                        string description = SR.GetString(formatEntry.Description);
                        ListViewItem item = new ListViewItem(new string[] { formatEntry.Mask, description });
                        this.listView.Items.Add(item);
                        Size size = g.MeasureString(description, listView.Font).ToSize();
                        minWidth = Math.Max(size.Width, minWidth);
                        height = size.Height;
                        used[formatEntry.Mask] = string.Empty;
                    }
                }

                g.Dispose();
                hdrDescription.Width = minWidth + 2;
                height = SystemInformation.VerticalScrollBarWidth
                    + ((height + 4) * Math.Min(12, listView.Items.Count + 1));
                this.Height = height;
                this.VScroll = listView.Items.Count > 11;

                this.listView.Columns.Add(hdrMask);
                this.listView.Columns.Add(hdrDescription);

                this.Controls.Add(this.listView);
            }

            // Methods.

            /// <summary>
            /// Process Key Preview
            /// </summary>
            /// <param name="m">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the window message to process.</param>
            /// <returns>
            /// true if the message was processed by the control; otherwise, false.
            /// </returns>
            /// <override/>
            protected override bool ProcessKeyPreview(ref Message m)
            {
#if DEBUG
                Trace.WriteLineIf(Switches.Development.TraceVerbose, "GFM.ProcessKeyMessage: " + m.ToString());
#endif

                return base.ProcessKeyPreview(ref m);
            }

            public object EditValue
            {
                get
                {
                    return this.editValue;
                }

                set
                {
                    if (this.editValue != value)
                    {
                        object obj = value;
                        this.editValue = value;
                        this.originalValue = obj;

                        foreach (ListViewItem item in listView.Items)
                        {
                            if (item.Text == (string)this.editValue)
                            {
                                item.Selected = true;
                                // TODO: listView.FocusedItem = item;
                            }
                            else
                            {
                                item.Selected = false;
                            }
                        }
                    }
                }
            }

            private void listView1_Click(object sender, System.EventArgs e)
            {
                ////MessageBox.Show(listView.SelectedItems[0].Text);
                if (listView != null && listView.SelectedItems.Count > 0)
                {
                    this.editValue = listView.SelectedItems[0].Text;
                    this.mainEditor.edSvc.CloseDropDown();
                }
            }

            /*private void listView1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                    MessageBox.Show(listView.SelectedItems[0].Text);
            }*/
        }
    }
}
