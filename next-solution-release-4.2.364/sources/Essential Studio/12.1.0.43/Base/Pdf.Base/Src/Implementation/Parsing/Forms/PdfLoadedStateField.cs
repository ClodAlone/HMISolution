#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents the base class for loaded state field.
    /// </summary>   
    /// <seealso cref="PdfLoadedStyledField"/> Class
    public abstract class PdfLoadedStateField : PdfLoadedStyledField
    {
        #region Fields
        private PdfLoadedStateItemCollection m_items;
        private bool m_bUnchecking;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the items collection.
        /// </summary>
        /// <seealso cref="PdfLoadedStateItemCollection"/> Class
        public PdfLoadedStateItemCollection Items
        {
            get
            {
                return m_items;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedStateField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedStateField(PdfDictionary dictionary, PdfCrossTable crossTable,
            PdfLoadedStateItemCollection items)
            : base(dictionary, crossTable)
        {
            if (crossTable == null)
                throw new ArgumentNullException("crossTable");

            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (items == null)
                throw new ArgumentNullException("items");

            PdfArray kids = Kids;

            m_items = items;

            if (kids != null)
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfDictionary itemDictionary = crossTable.GetObject(kids[i]) as PdfDictionary;

                    PdfLoadedStateItem item = GetItem(i, itemDictionary);
                    // new PdfLoadedCheckBoxItem( this, i, itemDictionary );
                    m_items.Add(item);
                }
            }
            else
            {
                PdfLoadedStateItem item = GetItem(0, dictionary);

                if (item is PdfLoadedRadioButtonItem)
                {
                    PdfLoadedRadioButtonItem radio = item as PdfLoadedRadioButtonItem;
                    if (radio.Value != "")
                    {
                        m_items.Add(item);
                    }
                }
                else
                {
                    m_items.Add(item);
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="itemDictionary">The item dictionary.</param>
        /// <returns>The proper state item.</returns>
        internal abstract PdfLoadedStateItem GetItem(int index, PdfDictionary itemDictionary);

        /// <summary>
        /// Gets the state template.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="item">The item.</param>
        /// <returns>The proper PdfTemplate instance.</returns>
        private PdfTemplate GetStateTemplate(PdfCheckFieldState state, PdfLoadedStateItem item)
        {
            PdfDictionary dic = (item != null) ? item.Dictionary : Dictionary;
            string value = (state == PdfCheckFieldState.Checked) ?
                GetItemValue(dic, CrossTable) :
                DictionaryProperties.Off;

            PdfTemplate template = null;
            if (dic.ContainsKey(DictionaryProperties.AP))
            {
                PdfDictionary appearance = PdfCrossTable.Dereference(dic[DictionaryProperties.AP]) as PdfDictionary;
                PdfDictionary norm = PdfCrossTable.Dereference(appearance[DictionaryProperties.N]) as PdfDictionary;
                PdfStream xobject = PdfCrossTable.Dereference(norm[value]) as PdfStream;
                if (xobject != null)
                {
                    template = new PdfTemplate(xobject);
                }
            }

            return template;
        }

        /// <summary>
        /// Sets checked status of the field.
        /// </summary>
        /// <param name="value">Checked status.</param>
        protected void SetCheckedStatus(bool value)
        {
            bool check = value;

            if (check)
            {
                string val = GetItemValue(Dictionary, CrossTable);

                Dictionary.SetName(DictionaryProperties.V, val);
                Dictionary.SetProperty(DictionaryProperties.AS, new PdfName(val));
            }
            else
            {
                Dictionary.Remove(DictionaryProperties.V);
                Dictionary.SetProperty(DictionaryProperties.AS, new PdfName(DictionaryProperties.Off));
            }

            Changed = true;
        }

        /// <summary>
        /// Gets the item value.
        /// </summary>
        /// <returns>The value of the item.</returns>
        internal static string GetItemValue(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            string value = String.Empty;
            PdfName name = null;

            if (dictionary.ContainsKey(DictionaryProperties.AS))
            {
                name = crossTable.GetObject(dictionary[DictionaryProperties.AS]) as PdfName;

                if (name != null && name.Value != DictionaryProperties.Off)
                {
                    value = name.Value;
                }
            }
            if (value == String.Empty)
            {
                {
                    if (dictionary.ContainsKey(DictionaryProperties.AP))
                    {
                        PdfDictionary dic = crossTable.GetObject(dictionary[DictionaryProperties.AP]) as PdfDictionary;

                        if (dic.ContainsKey(DictionaryProperties.N))
                        {
                            PdfReference reference = crossTable.GetReference(dic[DictionaryProperties.N]) as PdfReference;

                            PdfDictionary normalAppearance = crossTable.GetObject(reference) as PdfDictionary;

                            List<object> list = new List<object>();
                            foreach(PdfName pdfName in normalAppearance.Keys)
                            {
                                list.Add(pdfName);
                            }
                            

                            for (int i = 0, size = list.Count; i < size; ++i)
                            {
                                name = list[i] as PdfName;

                                if (name.Value != DictionaryProperties.Off)
                                {
                                    value = name.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Unchecks the others kids.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="value">The value.</param>
        internal void UncheckOthers(PdfLoadedStateItem child, string value, bool check)
        {
            if (!m_bUnchecking)
            {
                m_bUnchecking = true;

                for (int i = 0, count = Items.Count; i < count; ++i)
                {
                    PdfLoadedStateItem item = Items[i];

                    if (item != child)
                    {
                        bool v = (GetItemValue(item.Dictionary, CrossTable) == value);
                        item.Checked = v && check;
                    }
                }

                m_bUnchecking = false;
            }
        }

        /// <summary>
        /// Applies the appearance.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="item">The item.</param>
        internal void ApplyAppearance(PdfDictionary widget, PdfLoadedCheckBoxItem item)
        {
            if ((widget != null) && (widget.ContainsKey(DictionaryProperties.AP)))
            {
                PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AP]) as PdfDictionary;

                if ((appearance != null) && (appearance.ContainsKey(DictionaryProperties.N)))
                {
                    string value = String.Empty;

                    if (item != null)
                    {
                        value = GetItemValue(item.Dictionary, item.CrossTable);
                    }
                    else
                    {
                        value = GetItemValue(Dictionary, CrossTable);
                    }

                    RectangleF rect = (item == null) ? Bounds : item.Bounds;

                    IPdfPrimitive holder = PdfCrossTable.Dereference(appearance[DictionaryProperties.N]);

                    PdfDictionary normal = holder as PdfDictionary;

                    if ((normal == null) /*&& ( normal.ContainsKey( value ) )*/ )
                    {
                        normal = new PdfDictionary();
                        PdfTemplate checkedTemplate = new PdfTemplate(rect.Size);
                        PdfTemplate unchekedTemplate = new PdfTemplate(rect.Size);

                        DrawStateItem(checkedTemplate.Graphics, PdfCheckFieldState.Checked, item);
                        DrawStateItem(unchekedTemplate.Graphics, PdfCheckFieldState.Unchecked, item);

                        normal.SetProperty(DictionaryProperties.Off, new PdfReferenceHolder(unchekedTemplate));
                        normal.SetProperty(value, new PdfReferenceHolder(checkedTemplate));

                        appearance[DictionaryProperties.N] = new PdfReferenceHolder(normal);
                    }

                    holder = PdfCrossTable.Dereference(appearance[DictionaryProperties.D]);
                    PdfDictionary pressed = holder as PdfDictionary;

                    if ((pressed == null) /*&& ( pressed.ContainsKey( value ) )*/ )
                    {
                        PdfTemplate checkedTemplate = new PdfTemplate(rect.Size);
                        PdfTemplate unchekedTemplate = new PdfTemplate(rect.Size);

                        DrawStateItem(checkedTemplate.Graphics, PdfCheckFieldState.PressedChecked, item);
                        DrawStateItem(unchekedTemplate.Graphics, PdfCheckFieldState.PressedUnchecked, item);

                        if (pressed != null)
                        {
                            pressed.SetProperty(DictionaryProperties.Off, new PdfReferenceHolder(unchekedTemplate));
                            pressed.SetProperty(value, new PdfReferenceHolder(checkedTemplate));

                            appearance[DictionaryProperties.D] = new PdfReferenceHolder(pressed);
                        }
                    }
                }

                widget.SetProperty(DictionaryProperties.AP, appearance);
            }
            else if ((this as PdfField).Form.SetAppearanceDictionary)
                (this as PdfField).Form.NeedAppearances = true;
        }

        /// <summary>
        /// Draws the check box item.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="state">The state.</param>
        /// <param name="item">The item.</param>
        internal void DrawStateItem(PdfGraphics graphics, PdfCheckFieldState state, PdfLoadedStateItem item)
        {
            GraphicsProperties gp;
            GetGraphicsProperties(out gp, item);

            if (!Flatten)
            {
                gp.Rect.Location = PointF.Empty;
            }

            PaintParams prms = new PaintParams(gp.Rect, gp.BackBrush, gp.ForeBrush, gp.Pen,
                gp.Style, gp.BorderWidth, gp.ShadowBrush, gp.RotationAngle);



            //FieldPainter.DrawCheckBox( graphics, prms, CHECK_SYMBOL, state, gp.Font );
            graphics.StreamWriter.SetTextRenderingMode(TextRenderingMode.Fill);
            PdfTemplate stateTemplate = GetStateTemplate(state, item);
            if (stateTemplate != null)
            {
                RectangleF bounds = (item == null) ? Bounds : item.Bounds;

                graphics.DrawPdfTemplate(stateTemplate, bounds.Location);
            }
        }
        #endregion
    }
}
