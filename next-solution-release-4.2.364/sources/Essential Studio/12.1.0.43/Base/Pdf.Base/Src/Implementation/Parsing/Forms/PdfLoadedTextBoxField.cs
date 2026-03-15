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
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Native;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents the text box field of an existing PDF document`s form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load the text box field
    /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
    /// RectangleF newBounds = new RectangleF(100, 100, 50, 50);
    /// ldField.Bounds = newBounds;
    /// ldField.SpellCheck = true;
    /// ldField.Text = "New text of the field.";
    /// ldField.Password = false;
    /// ldField.BorderStyle = PdfBorderStyle.Dashed;
    /// doc.Save("LoadedForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load the text box field
    /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
    /// Dim newBounds As RectangleF = New RectangleF(100, 100, 50, 50)
    /// ldField.Bounds = newBounds
    /// ldField.SpellCheck = True
    /// ldField.Text = "New text of the field."
    /// ldField.Password = False
    /// ldField.BorderStyle = PdfBorderStyle.Dashed
    /// doc.Save("LoadedForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedDocument"/> Class    
    /// <seealso cref="PdfLoadedStyledField"/> Class
    public class PdfLoadedTextBoxField : PdfLoadedStyledField
    {
        #region Constants
        /// <summary>
        /// The password chrackter.
        /// </summary>
        const string m_passwordValue = "*";
        #endregion

        #region Fields
        /// <summary>
        /// Collection of textbox items.
        /// </summary>
        private PdfLoadedTextBoxItemCollection m_items;
        /// <summary>
        /// Internal variable to stroe field`s fore color.
        /// </summary>
        private PdfColor m_foreColor = new PdfColor(0, 0, 0);
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set the back color of the field
        /// </summary>
        /// <value>A <see cref="PdfColor"/> object specifying the background color of field. </value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field.
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.BackColor = new PdfColor(Color.Transparent);
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.BackColor = New PdfColor(Color.Transparent)
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class    
        /// <seealso cref="PdfColor"/> Class    
        public PdfColor BackColor
        {
            get
            {
                return GetBackColor();
            }
            set
            {
                SetBackColor(value);
            }
        }

        /// <summary>
        /// Gets or Set the fore color of the field.
        /// </summary>
        /// <value>A <see cref="PdfColor"/> object specifying the background color of field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field.
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.ForeColor = new PdfColor(Color.Red);
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field.
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.ForeColor = New PdfColor(Color.Red)
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class    
        /// <seealso cref="PdfColor"/> Class    
        public PdfColor ForeColor
        {
            get
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

                if ((widget != null) && (widget.ContainsKey(DictionaryProperties.DA)))
                {
                    PdfString defaultAppearance = CrossTable.GetObject(widget[DictionaryProperties.DA]) as PdfString;

                    m_foreColor = GetForeColour(defaultAppearance.Value);
                }
                return m_foreColor;
            }
            set
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

                float height = 0.0f;
                string name = null;
                if ((widget != null) && (widget.ContainsKey(DictionaryProperties.DA)))
                {
                    PdfDictionary fontDictionary = CrossTable.GetObject(Form.Resources[DictionaryProperties.Font]) as PdfDictionary;
                    PdfString str = widget[DictionaryProperties.DA] as PdfString;
                    name = FontName(str.Value, out height);
                    PdfReferenceHolder fontHolder = fontDictionary[name] as PdfReferenceHolder;
                    PdfDictionary fontDic;
                    if (fontHolder != null)
                        fontDic = fontHolder.Object as PdfDictionary;
                }
                else if (widget != null && Dictionary.ContainsKey(DictionaryProperties.DA))
                {
                    PdfDictionary fontDictionary = CrossTable.GetObject(Form.Resources[DictionaryProperties.Font]) as PdfDictionary;
                    PdfString str = Dictionary[DictionaryProperties.DA] as PdfString;
                    name = FontName(str.Value, out height);
                    PdfReferenceHolder fontHolder = fontDictionary[name] as PdfReferenceHolder;
                    PdfDictionary fontDic = fontHolder.Object as PdfDictionary;
                }
                if (name != null)
                {
                    PdfDefaultAppearance defaultAppearance = new PdfDefaultAppearance();
                    defaultAppearance.FontName = name;
                    defaultAppearance.FontSize = height;
                    defaultAppearance.ForeColor = value;
                    widget[DictionaryProperties.DA] = new PdfString(defaultAppearance.ToString());
                }
                else
                {
                    PdfDefaultAppearance defaultAppearance = new PdfDefaultAppearance();
                    defaultAppearance.FontName = Font.Name;
                    defaultAppearance.FontSize = Font.Size;
                    defaultAppearance.ForeColor = value;
                    widget[DictionaryProperties.DA] = new PdfString(defaultAppearance.ToString());
                }
                (this as PdfField).Form.SetAppearanceDictionary = true;
            }
        }

        /// <summary>
        /// Get or Set the text alignment in a text box.
        /// </summary>
        /// <value>A <see cref="PdfTextAlignment"/> enumeration member specifying the text alignment in a text box.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field.
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.TextAlignment = PdfTextAlignment.Justify;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field.
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.TextAlignment = PdfTextAlignment.Justify
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class    
        /// <seealso cref="PdfTextAlignment"/> Class    
        public PdfTextAlignment TextAlignment
        {
            get
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                PdfTextAlignment align = new PdfTextAlignment();
                if (widget.ContainsKey(DictionaryProperties.Q))
                {
                    PdfNumber number = widget[DictionaryProperties.Q] as PdfNumber;
                    align = (PdfTextAlignment)Enum.ToObject(typeof(PdfTextAlignment), number.IntValue);
                }
                return align;
            }
            set
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                widget.SetProperty(DictionaryProperties.Q, new PdfNumber((int)value));
                (this as PdfField).Form.SetAppearanceDictionary = true;
            }
        }


        /// <summary>
        /// Get or Set the HighLightMode of the Field.
        /// </summary>
        /// <value>A <see cref="PdfHighlightMode"/> enumeration member specifying the highlight mode in a text box.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.HighlightMode = PdfHighlightMode.Push;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.HighlightMode = PdfHighlightMode.Push
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class    
        /// <seealso cref="PdfHighlightMode"/> Class    
        public PdfHighlightMode HighlightMode
        {
            get
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                PdfHighlightMode align = new PdfHighlightMode();
                if (widget.ContainsKey(DictionaryProperties.H))
                {

                    PdfName name = CrossTable.GetObject(widget[DictionaryProperties.H]) as PdfName;
                    align = GetHighlightModeFromString(name);
                }
                return align;
            }
            set
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                widget[DictionaryProperties.H] = new PdfName(HighlightModeToString(value));
            }
        }

        /// <summary>
        /// Gets or Set value of the text box field.
        /// </summary>
        /// <value>A string value representing the value of the item. </value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.Text = "New Text";
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.Text = "New Text"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class            
        public string Text
        {
            get
            {
                string text = string.Empty;
                PdfString str = GetValue(Dictionary, CrossTable, DictionaryProperties.V, true) as PdfString;

                if (str != null)
                {
                    text = str.Value;
                }
                else 
                {
                   text = string.Empty;
                }
                 
                return text;
            }
            set
            {
                bool readOnly = ((FieldFlags.ReadOnly & Flags) != 0);
                
                if (!readOnly)
                {
                    if (value == null)
                        throw new ArgumentNullException("text");

                    string text = value;
                    Dictionary.SetProperty(DictionaryProperties.V, new PdfString(value));
                    PdfDictionary  widget = GetWidgetAnnotation(Dictionary, CrossTable);
                    PdfDictionary mk = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;                   
                    Changed = true;
                    (this as PdfField).Form.SetAppearanceDictionary = true;
                }
                else
                {
                    Changed = false;
                }
            }
        }

        /// <summary>
        /// Gets or set the default value of the field.
        /// </summary>
        /// <value>A string value representing the default value of the item. </value>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.DefaultValue = "Cris";
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.DefaultValue = "Cris"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public string DefaultValue
        {
            get
            {
                string defaultValue = null;

                PdfString str = GetValue(Dictionary, CrossTable, DictionaryProperties.DV, true) as PdfString;

                if (str != null)
                {
                    defaultValue = str.Value;
                }

                return defaultValue;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("DefaultValue");

                string defaultValue = value;
                Dictionary.SetString(DictionaryProperties.DV, defaultValue);

                Changed = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check spelling.
        /// </summary>
        /// <value>True if the field content should be checked for spelling erorrs, false otherwise. Default is true.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.SpellCheck = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.SpellCheck = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public bool SpellCheck
        {
            get
            {
                bool spellCheck = !((FieldFlags.DoNotSpellCheck & Flags) != 0);

                return spellCheck;
            }
            set
            {
                bool spellCheck = value;

                if (spellCheck)
                {
                    Flags &= ~FieldFlags.DoNotSpellCheck;
                }
                else
                {
                    Flags |= FieldFlags.DoNotSpellCheck;
                }
            }
        }

        /// <summary>
        /// Meaningful only if the MaxLength property is set and the Multiline, Password properties are false.
        /// If set, the field is automatically divided into as many equally spaced positions, or combs, 
        /// as the value of MaxLength, and the text is laid out into those combs.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.InsertSpaces = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.InsertSpaces = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public bool InsertSpaces
        {
            get
            {
                bool insertSpaces = ((FieldFlags.Comb & Flags) != 0);

                return insertSpaces;
            }
            set
            {
                bool insertSpaces = value;

                if (insertSpaces)
                {
                    Flags |= FieldFlags.Comb;
                }
                else
                {
                    Flags &= ~FieldFlags.Comb;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfTextBoxField"/> is multiline.
        /// </summary>       
        /// <value>True if the field is multiline, false otherwise. Default is false.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.Multiline = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.Multiline = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public bool Multiline
        {
            get
            {
                bool multiline = ((FieldFlags.Multiline & Flags) != 0);

                return multiline;
            }
            set
            {
                bool multiline = value;

                if (multiline)
                {
                    Flags |= FieldFlags.Multiline;
                }
                else
                {
                    Flags &= ~FieldFlags.Multiline;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfTextBoxField"/> is password field.
        /// </summary>
        /// <value>True if the field is a password field, false otherwise. Default is false.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.Password = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.Password = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public bool Password
        {
            get
            {
                bool password = ((FieldFlags.Password & Flags) != 0);

                return password;
            }
            set
            {
                bool password = value;

                if (password)
                {
                    Flags |= FieldFlags.Password;
                }
                else
                {
                    Flags &= ~FieldFlags.Password;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfTextBoxField"/> is scrollable.
        /// </summary>
        /// <value>True if the field content can be scrolled, false otherwise. Default is true.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.Scrollable = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.Scrollable = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public bool Scrollable
        {
            get
            {
                bool scrollable = !((FieldFlags.DoNotScroll & Flags) != 0);

                return scrollable;
            }
            set
            {
                bool scrollable = value;

                if (scrollable)
                {
                    Flags &= ~FieldFlags.DoNotScroll;
                }
                else
                {
                    Flags |= FieldFlags.DoNotScroll;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum length of the field, in characters.
        /// </summary>
        /// <value>A positive integer value specifying the maximum number of characters that can be entered in the text edit field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField ldField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// ldField.MaxLength = 10;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim ldField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ldField.MaxLength = 10
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public int MaxLength
        {
            get
            {
                int maxLength = 0;

                PdfNumber number
                    = GetValue(Dictionary, CrossTable, DictionaryProperties.MaxLen, true) as PdfNumber;

                if (number != null)
                {
                    maxLength = number.IntValue;
                }

                return maxLength;
            }
            set
            {
                int maxLength = value;
                Dictionary.SetNumber(DictionaryProperties.MaxLen, maxLength);

                Changed = true;
            }
        }

        /// <summary>
        /// Gets the collection of text box field items. 
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Read the text box field
        /// PdfLoadedTextBoxField textboxField = doc.Form.Fields[0] as PdfLoadedTextBoxField;
        /// // TextBox Item collection
        /// PdfLoadedTextBoxItemCollection textboxFieldCollection = textboxField.Items;
        /// textboxFieldCollection[0].Location = new PointF(10, 20);
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Read the text box field
        /// Dim textboxField As PdfLoadedTextBoxField = TryCast(doc.Form.Fields(0), PdfLoadedTextBoxField)
        /// ' TextBox Item collection
        /// Dim textboxFieldCollection As PdfLoadedTextBoxItemCollection = textboxField.Items
        /// textboxFieldCollection(0).Location = New PointF(10, 20)
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class 
        public PdfLoadedTextBoxItemCollection Items
        {
            get
            {
                return m_items;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedTextBoxField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedTextBoxField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
            PdfArray kids = Kids;
            m_items = new PdfLoadedTextBoxItemCollection();

            if (kids != null)
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfDictionary itemDictionary = crossTable.GetObject(kids[i]) as PdfDictionary;

                    PdfLoadedTexBoxItem item = new PdfLoadedTexBoxItem(this, i, itemDictionary);
                    m_items.Add(item);
                }
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Set the back color of the Field.
        /// </summary>
        /// <param name="value">PdfColor Value.</param>
        private void SetBackColor(PdfColor value)
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            if (widget != null)
            {
                if (widget.ContainsKey(DictionaryProperties.MK))
                {
                    PdfDictionary mk = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;
                    PdfArray array = value.ToArray();
                    mk[DictionaryProperties.BG] = array;
                }
                else
                {
                    PdfDictionary mk = new PdfDictionary();
                    PdfArray array = value.ToArray();
                    mk[DictionaryProperties.BG] = array;
                    widget[DictionaryProperties.MK] = mk;
                }
                (this as PdfField).Form.SetAppearanceDictionary = true;
            }
        }

        /// <summary>
        /// Converts the HighlightMode as String value.
        /// </summary>
        /// <param name="m_highlightingMode">PdfHighlightMode value.</param>
        /// <returns>Returns the PdfHighlightMode as string value.</returns>
        private string HighlightModeToString(PdfHighlightMode m_highlightingMode)
        {
            switch (m_highlightingMode)
            {
                case PdfHighlightMode.Invert:
                default:
                    return "I";

                case PdfHighlightMode.NoHighlighting:
                    return "N";

                case PdfHighlightMode.Outline:
                    return "O";

                case PdfHighlightMode.Push:
                    return "P";
            }
        }

        /// <summary>
        /// Converts the given string value as PdfHighlightMode.
        /// </summary>
        /// <param name="hightlightMode">Given string value.</param>
        /// <returns>Returns the PdfHighlightMode values.</returns>
        private PdfHighlightMode GetHighlightModeFromString(PdfName hightlightMode)
        {
            switch (hightlightMode.Value)
            {
                case "P":
                    return PdfHighlightMode.Push;
                case "N":
                    return PdfHighlightMode.NoHighlighting;

                case "O":
                    return PdfHighlightMode.Outline;
                case "I":
                default:
                    return PdfHighlightMode.Invert;
            }
        }

        /// <summary>
        /// Begins the save.
        /// </summary>
        internal override void BeginSave()
        {
            base.BeginSave();
            
            PdfArray kids = Kids;
            if ((kids != null))
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfDictionary widget = CrossTable.GetObject(kids[i]) as PdfDictionary;
                    ApplyAppearance(widget, Items[i]);
                }
            }
            else
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                ApplyAppearance(widget, null);
            }
        }

        /// <summary>
        /// Creates a copy of PdfLoadedTextBoxField.
        /// </summary>
        internal PdfField Clone(PdfDictionary dictionary, PdfPage page)
        {
            PdfCrossTable newTable = page.Section.ParentDocument.CrossTable;
            PdfLoadedTextBoxField field = new PdfLoadedTextBoxField(dictionary, newTable);
            field.Page = page;
            field.SetName(GetFieldName());
            field.Widget.Dictionary = Widget.Dictionary.Clone(newTable) as PdfDictionary;

            return field;
        }

        /// <summary>
        /// Creates a copy of PdfLoadedTextBoxItem.
        /// </summary>
        internal override PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary)
        {
            base.CreateLoadedItem(dictionary);

            PdfLoadedTexBoxItem item = new PdfLoadedTexBoxItem(this, m_items.Count, dictionary);
            m_items.Add(item);

            if (Kids == null)
                Dictionary[DictionaryProperties.Kids] = new PdfArray();

            Kids.Add(new PdfReferenceHolder(dictionary));

            return item;
        }

        /// <summary>
        /// Applies the appearance.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="item">The item.</param>
        private void ApplyAppearance(PdfDictionary widget, PdfLoadedFieldItem item)
        {
            bool needAppearance = (this as PdfField).Form.NeedAppearances;
            
            if ((this as PdfField).Form.SetAppearanceDictionary)
            {
                if (widget != null && !needAppearance)
                {
                    PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AP]) as PdfDictionary;

                    //if (appearance == null)
                    {
                        appearance = new PdfDictionary();
                        RectangleF bounds = (item == null) ? Bounds : item.Bounds;

                        // For some existing texboxes, markup sequence tags are required.
                        PdfTemplate template = new PdfTemplate(bounds.Size, false);
                        template.Graphics.StreamWriter.BeginMarkupSequence("Tx");
                        // Change co-ordinate after within markup sequence.
                        template.Graphics.InitializeCoordinates();
                        DrawTextBox(template.Graphics, item);
                        template.Graphics.StreamWriter.EndMarkupSequence();

                        appearance.SetProperty(DictionaryProperties.N, new PdfReferenceHolder(template));
                        widget.SetProperty(DictionaryProperties.AP, appearance);
                    }
                }
                else
                    (this as PdfField).Form.NeedAppearances = true;
            }
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();
            PdfArray kids = Kids;
            if ((kids != null))
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfLoadedFieldItem item = Items[i];

                    if (Page is PdfLoadedPage)
                      DrawTextBox(item.Page.Graphics, item);
                    else
                    {
                        if ((((kids[i] as PdfReferenceHolder).Object as PdfDictionary)[DictionaryProperties.P] as PdfReferenceHolder).Reference == null)
                        {
                            PdfPageBase page = (Page as PdfPage).Section.ParentDocument.EnableMemoryOptimization ? item.Page : Page;
                            DrawTextBox(page.Graphics, item);
                        }
                        else
                        {
                            PdfPageBase page = this.Form.m_pageMap[(((kids[i] as PdfReferenceHolder).Object as PdfDictionary)[DictionaryProperties.P] as PdfReferenceHolder).Object as PdfDictionary];
                            PdfArray annots = (page.Dictionary[DictionaryProperties.Annots] as PdfArray);
                            int count = annots.Count;
                            for (int j = 0; j < count - 1; j++)
                            {
                                if ((annots[j] is PdfReferenceHolder))
                                {
                                    PdfDictionary fieldDictionary = (annots[j] as PdfReferenceHolder).Object as PdfDictionary;
                                    if (fieldDictionary.ContainsKey(DictionaryProperties.Parent))
                                    {
                                        if (fieldDictionary[DictionaryProperties.Parent] is PdfReferenceHolder)
                                        {
                                            fieldDictionary = (fieldDictionary[DictionaryProperties.Parent] as PdfReferenceHolder).Object as PdfDictionary;
                                            if ((fieldDictionary[DictionaryProperties.T] as PdfString).Value == this.Name)
                                            {
                                                annots.RemoveAt(j);
                                            }
                                        }
                                    }
                                }
                            }
                            DrawTextBox(page.Graphics, item);
                        }
                       
                    }
                }
            }
            else
            {
                DrawTextBox(Page.Graphics, null);
            }
        }

        /// <summary>
        /// Draws the text box.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="item">The item.</param>
        private void DrawTextBox(PdfGraphics graphics, PdfLoadedFieldItem item)
        {
            PdfLoadedStyledField.GraphicsProperties gp;
            GetGraphicsProperties(out gp, item);

            if (!Flatten)
            {
                gp.Rect.Location = new PointF(0, 0);
            }

            //string text = String.Empty;
            //if (Dictionary.ContainsKey(DictionaryProperties.V))
            //{

            //    PdfString str = GetValue(Dictionary, CrossTable, DictionaryProperties.V, true) as PdfString;

            //    if (str.Value != null)
            //        text = str.Value;
            //}

            string text = Text; 

            if (Password)
            {
                text = string.Empty;
                for (int i = 0; i < Text.Length; ++i)
                {
                    text += m_passwordValue;
                }
            }

            if (gp.BackBrush == null)
            {
                //gp.BackBrush = PdfBrushes.White;
                //BackBrush = PdfBrushes.White;
            }

#if !SILVERLIGHT && AllowUnsafeCode
            // Set RightToLeft property to true if the text contains RTL.
            ushort[] characterCodes = new ushort[text.Length];
            KernelApi.GetStringTypeExW(0x800, StringInfoType.CT_TYPE2, text, text.Length, characterCodes);
            gp.StringFormat.RightToLeft = IsRTLText(characterCodes);
# endif
            gp.StringFormat.LineLimit = false;

            if (!Multiline)
            {
                gp.StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
                gp.StringFormat.WordWrap = PdfWordWrapType.None;
            }
            if (!Multiline && Flatten)
            {
                gp.StringFormat.WordWrap = PdfWordWrapType.Character;
                if (Font.Height < gp.Rect.Height)
                    gp.StringFormat.LineLimit = true;
            }
            PaintParams prms = new PaintParams(gp.Rect, gp.BackBrush, gp.ForeBrush, gp.Pen, gp.Style,
                gp.BorderWidth, gp.ShadowBrush, gp.RotationAngle);

            if (this.Dictionary.ContainsKey(DictionaryProperties.Rect) || this.Dictionary.ContainsKey(DictionaryProperties.Kids))
                FieldPainter.DrawTextBox(graphics, prms, text, gp.Font, gp.StringFormat, Multiline, Scrollable);
        }

        /// <summary>
        /// Checks if the text contains RTL character or number.
        /// </summary>
        /// <param name="characterCodes">Array of symbols.</param>
        /// <returns>True if the text contans RTL character or number.</returns>
        private bool IsRTLText(ushort[] characterCodes)
        {
            bool isRTL = false;

            for (int i = 0, len = characterCodes.Length; i < len; ++i)
            {
                if (characterCodes[i] == (ushort)StringInfoCtype2.C2_RIGHTTOLEFT
                   || characterCodes[i] == (ushort)StringInfoCtype2.C2_ARABICNUMBER)
                {
                    isRTL = true;
                    break;
                }
            }

            return isRTL;
        }

        /// <summary>
        /// Gets the height of the font.
        /// </summary>
        /// <param name="family"></param>
        /// <returns>The calculated size of font.</returns>
        internal override float GetFontHeight(PdfFontFamily family)
        {
            float s = 12.0f;

            PdfStandardFont font = new PdfStandardFont(family, 12);
            SizeF fontSize;
            if (!Multiline)
            {
                fontSize = font.MeasureString(Text);

                float max = fontSize.Width;
                s = ((8 * (Bounds.Size.Width - 4 * BorderWidth)) / max);
                s = (s > 8) ? 8 : s;
            }
            else
            {
                s = 12.5f;
                /*float step = 1;

                do
                {
                    fontSize = font.MeasureString( Text, Bounds.Size.Width );
                    float boundHeight = Bounds.Size.Height;

                    if( fontSize.Height * 1.25 > boundHeight && fontSize.Height * 1.1f < boundHeight ) break;

                    float max = fontSize.Height;

                    float boundDiff = boundHeight - 4 * BorderWidth;
                    float shift = boundDiff / max;
                    step = ( s -  s * shift ) / 2.0f;
                    s -= step;
                    font = new PdfStandardFont( font, s );

                } while( Math.Abs( step ) > 0.0000001 );*/
            }

            return s;
        }
        #endregion
    }
}
