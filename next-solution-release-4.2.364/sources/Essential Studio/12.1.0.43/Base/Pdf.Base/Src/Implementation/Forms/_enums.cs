#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents fields flags enum.
    /// </summary>
    [Flags]
    internal enum FieldFlags
    {
        #region Common flags
        /// <summary>
        /// Default field flag.
        /// </summary>
        Default = 0,

        /// <summary>
        /// If set, the user may not change the value of the field. Any associated widget annotations 
        /// will not interact with the user; that is, they will not respond to mouse clicks or 
        /// change their appearance in response to mouse motions. This flag is useful 
        /// for fields whose values are computed or imported from a database.
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// If set, the field must have a value at the time it is exported by a submit-form action.
        /// </summary>
        Required = 1 << 1,

        /// <summary>
        /// If set, the field must not be exported by a submit-form action
        /// </summary>
        NoExport = 1 << 2,

        #endregion

        #region Text field flags
        /// <summary>
        /// If set, the field can contain multiple lines of text; 
        /// if clear, the field�s text is restricted to a single line.
        /// </summary>
        Multiline = 1 << 12,

        /// <summary>
        /// If set, the field is intended for entering a secure password that should not be 
        /// echoed visibly to the screen. Characters typed from the keyboard should instead 
        /// be echoed in some unreadable form, such as asterisks or bullet characters.
        /// </summary>
        Password = 1 << 13,

        /// <summary>
        /// If set, the text entered in the field represents the pathname of a file whose 
        /// contents are to be submitted as the value of the field.
        /// </summary>
        FileSelect = 1 << 20,

        /// <summary>
        /// If set, text entered in the field is not spell-checked.
        /// </summary>
        DoNotSpellCheck = 1 << 22,

        /// <summary>
        /// If set, the field does not scroll (horizontally for single-line fields, vertically 
        /// for multiple-line fields) to accommodate more text than fits within its annotation 
        /// rectangle. Once the field is full, no further text is accepted.
        /// </summary>
        DoNotScroll = 1 << 23,

        /// <summary>
        /// Meaningful only if the MaxLen entry is present in the text field dictionary and if 
        /// the Multiline, Password, and FileSelect flags are clear. If set, the field is 
        /// automatically divided into as many equally spaced positions, or combs, as the 
        /// value of MaxLen, and the text is laid out into those combs.
        /// </summary>
        Comb = 1 << 24,

        /// <summary>
        /// If set, the value of this field should be represented as a rich text string.
        /// If the field has a value, the RVentry of the field dictionary specifies 
        /// the rich text string.
        /// </summary>
        RichText = 1 << 25,
        #endregion

        #region Button field flags
        /// <summary>
        /// If set, exactly one radio button must be selected at all times; clicking 
        /// the currently selected button has no effect. If clear, clicking the selected 
        /// button reselects it, leaving no button selected.
        /// </summary>
        NoToggleToOff = 1 << 14,

        /// <summary>
        /// If set, the field is a set of radio buttons; if clear, the field is a check box. 
        /// This flag is meaningful only if the Pushbutton flag is clear.
        /// </summary>
        Radio = 1 << 15,

        /// <summary>
        /// If set, the field is a pushbutton that does not retain a permanent value.
        /// </summary>
        PushButton = 1 << 16,

        /// <summary>
        /// If set, a group of radio buttons within a radio button field that use the same value 
        /// for the on state will turn on and off in unison; that is if one is checked, they 
        /// are all checked. If clear, the buttons are mutually exclusive.
        /// </summary>
        RadiosInUnison = 1 << 25,

        #endregion

        #region Choise field flags
        /// <summary>
        /// If set, the field is a combo box; if clear, the field is a list box.
        /// </summary>
        Combo = 1 << 17,

        /// <summary>
        /// If set, the combo box includes an editable text box as well as a drop-down 
        /// list; if clear, it includes only a drop-down list. This flag is meaningful only 
        /// if the Combo flag is set.
        /// </summary>
        Edit = 1 << 18,

        /// <summary>
        /// If set, the field�s option items should be sorted alphabetically. This flag 
        /// is intended for use by form authoring tools, not by PDF viewer applications.
        /// </summary>
        Sort = 1 << 19,

        /// <summary>
        /// If set, more than one of the field�s option items may be selected simultaneously; 
        /// if clear, no more than one item at a time may be selected.
        /// </summary>
        MultiSelect = 1 << 21,

        /// <summary>
        /// If set, the new value is committed as soon as a selection is made with the pointing 
        /// device. This option enables applications to perform an action once a selection is 
        /// made, without requiring the user to exit the field. If clear, the new value is not 
        /// committed until the user exits the field.
        /// </summary>
        CommitOnSelChange = 1 << 26
        #endregion
    }

    /// <summary>
    /// Specifies the available styles for a field border.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// // Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();  
    /// //Create submit button 
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(100, 500, 90, 20);
    /// submitButton.Font = font;
    /// submitButton.Text = "Submit";
    /// // Set the border style for the button field
    /// submitButton.BorderStyle = PdfBorderStyle.Dashed;
    /// document.Form.Fields.Add(submitButton);
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// ' Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create submit button 
    /// Dim submitButton As PdfButtonField = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(100, 500, 90, 20)
    /// submitButton.Font = font
    /// submitButton.Text = "Submit"
    /// ' Set the border style for the button field
    /// submitButton.BorderStyle = PdfBorderStyle.Dashed
    /// document.Form.Fields.Add(submitButton)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <remarks>Defaule value is Solid.</remarks>
    public enum PdfBorderStyle
    {
        /// <summary>
        /// A solid rectangle surrounding the annotation.
        /// </summary>
        Solid,

        /// <summary>
        /// A dashed rectangle surrounding the annotation.
        /// </summary>
        Dashed,

        /// <summary>
        /// A simulated embossed rectangle that appears to be raised above the surface 
        /// of the page.
        /// </summary>
        Beveled,

        /// <summary>
        /// A simulated engraved rectangle that appears to be recessed below the surface 
        /// of the page.
        /// </summary>
        Inset,

        /// <summary>
        /// A single line along the bottom of the annotation rectangle.
        /// </summary>
        Underline
    }

    /// <summary>
    /// Specifies the highlight mode for a field.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create the signature field
    /// PdfSignatureField sign = new PdfSignatureField(page, "sign1");
    /// sign.Bounds = new RectangleF(100, 420, 100, 50);
    /// // Set the high light mode for a signature field
    /// sign.HighlightMode = PdfHighlightMode.Push;
    /// document.Form.Fields.Add(sign);
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create the signature field
    /// Dim sign As PdfSignatureField = New PdfSignatureField(page, "sign1")
    /// sign.Bounds = New RectangleF(100, 420, 100, 50)
    /// ' Set the high light mode for a signature field
    /// sign.HighlightMode = PdfHighlightMode.Push
    /// document.Form.Fields.Add(sign)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <remarks>Defaule value is Invert.</remarks>
    public enum PdfHighlightMode
    {
        /// <summary>
        /// No highlighting.
        /// </summary>
        NoHighlighting,

        /// <summary>
        /// Invert the contents of the field rectangle.
        /// </summary>
        Invert,

        /// <summary>
        /// Invert the field's border.
        /// </summary>
        Outline,

        /// <summary>
        /// Pushed highlighting.
        /// </summary>
        Push
    }

    /// <summary>
    /// Specifies the style for a check box field.
    /// </summary>
    /// <remarks>The default value is Check.</remarks>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();         
    /// //Create a check box
    /// PdfCheckBoxField checkBox = new PdfCheckBoxField(page, "C#.NET");            
    /// checkBox.Bounds = new RectangleF(100, 290, 20, 20);            
    /// // Add the check box field in form`s field collection
    /// document.Form.Fields.Add(checkBox);
    /// checkBox.HighlightMode = PdfHighlightMode.Push;
    /// checkBox.BorderStyle = PdfBorderStyle.Beveled;
    /// // Set the check style
    /// checkBox.Style = PdfCheckBoxStyle.Star;            
    /// checkBox.Checked = true;
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a check box
    /// Dim checkBox As PdfCheckBoxField = New PdfCheckBoxField(page, "C#.NET")
    /// checkBox.Bounds = New RectangleF(100, 290, 20, 20)
    /// ' Add the check box field in form`s field collection
    /// document.Form.Fields.Add(checkBox)
    /// checkBox.HighlightMode = PdfHighlightMode.Push
    /// checkBox.BorderStyle = PdfBorderStyle.Beveled
    /// ' Set the check style
    /// checkBox.Style = PdfCheckBoxStyle.Star
    /// checkBox.Checked = True
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    public enum PdfCheckBoxStyle
    {
        /// <summary>
        /// A check mark is used for the checked state.
        /// </summary>
        Check,

        /// <summary>
        /// A circle is used for the checked state. 
        /// </summary>
        Circle,

        /// <summary>
        /// A cross is used for the checked state.
        /// </summary>
        Cross,

        /// <summary>
        /// A diamond symbol is used for the checked state. 
        /// </summary>
        Diamond,

        /// <summary>
        /// A square is used for the checked state.
        /// </summary>
        Square,

        /// <summary>
        /// A star is used for the checked state.
        /// </summary>
        Star
    }

    /// <summary>
    /// Specifies Http request method.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        /// Data submitted using Http Get method.
        /// </summary>
        Get,

        /// <summary>
        /// Data submitted using Http Post method.
        /// </summary>
        Post
    }

    /// <summary>
    /// Specifies the enumeration of submit data formats.
    /// </summary>
    public enum SubmitDataFormat
    {
        /// <summary>
        /// Data should be transmitted as Html.
        /// </summary>
        Html,

        /// <summary>
        /// Data should be transmitted as Pdf.
        /// </summary>
        Pdf,

        /// <summary>
        /// Data should be transmitted as Forms Data Format.
        /// </summary>
        Fdf,

        /// <summary>
        /// Data should be transmitted as XML Forms Data Format .
        /// </summary>
        Xfdf
    }

    /// <summary>
    /// Represents states of the check field.
    /// </summary>
    enum PdfCheckFieldState
    {
        /// <summary>
        /// Indicated unchecked/unpressed state.
        /// </summary>
        Unchecked,

        /// <summary>
        /// Indicated checked unpressed state.
        /// </summary>
        Checked,

        /// <summary>
        /// Indicated pressed unchecked state.
        /// </summary>
        PressedUnchecked,

        /// <summary>
        /// Indicated pressed checked state.
        /// </summary>
        PressedChecked
    }
}
