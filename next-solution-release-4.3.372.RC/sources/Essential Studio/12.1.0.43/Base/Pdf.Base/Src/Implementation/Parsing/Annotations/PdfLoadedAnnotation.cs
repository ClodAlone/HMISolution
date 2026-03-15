#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the base class for loaded annotation classes.
    /// </summary>
    /// <seealso cref="PdfAnnotation"/> Class
    public abstract class PdfLoadedAnnotation : PdfAnnotation
    {
        #region Internal declarations
        /// <summary>
        /// NameChanged event handler.
        /// </summary>
        /// <param name="name">New name of the field.</param>
        internal delegate void BeforeNameChangesEventHandler(string name);
        #endregion

        #region Events
        /// <summary>
        /// Raises when user manualy chages the name of the field.
        /// </summary>
        internal event BeforeNameChangesEventHandler BeforeNameChanges;
        #endregion

        #region Fields
        /// <summary>
        /// Represents the Form field identifier
        /// </summary>
        public int ObjectID;
        /// <summary>
        /// Cross Table.
        /// </summary>
        private PdfCrossTable m_crossTable;

        /// <summary>
        /// Indicates was field changed or not.
        /// </summary>
        private bool m_Changed;

        /// <summary>
        /// Represents index used to default annotation.
        /// </summary>
        private int m_defaultIndex;

        private string m_fileName;

        private PdfLoadedPage m_loadedpage = null;

        #endregion

        #region Property
        /// <summary>
        /// Gets or sets the changed.
        /// </summary>
        internal bool Changed
        {
            get
            {
                return m_Changed;
            }
            set
            {
                m_Changed = value;
            }
        }
        /// <summary>
        /// Gets or sets the cross table.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("CrossTable");

                if (m_crossTable != value)
                {
                    m_crossTable = value;
                }
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (crossTable == null)
                throw new ArgumentNullException("crossTable");

            Dictionary = dictionary;
            m_crossTable = crossTable;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets and sets the Page.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// //Gets the page.
        /// PdfLoadedPage page =soundAnnotation.Page;
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim soundAnnotation As PdfLoadedSoundAnnotation = document.Pages(1).Annotations(5) as PdfLoadedSoundAnnotation
        /// 'Gets the page.
        /// Dim page As PdfLoadedPage=soundAnnotation.Page
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfLoadedPage Page
        {
            get
            {
                return m_loadedpage;
            }
            set
            {
                m_loadedpage = value;
            }
        }


        /// <summary>
        /// Sets the name of the field.
        /// </summary>
        /// <param name="name">New name of the field.</param>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedSoundAnnotation soundAnnotation = document.Pages[1].Annotations[5] as PdfLoadedSoundAnnotation;
        /// //Sets the annotation text.
        /// soundAnnotation.SetText("Sound Annotation");
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim soundAnnotation As PdfLoadedSoundAnnotation = document.Pages(1).Annotations(5) as PdfLoadedSoundAnnotation
        /// 'Sets the annotation text.
        /// soundAnnotation.SetText("Sound Annotation")
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public void SetText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (text == String.Empty)
                throw new ArgumentException("The text can't be empty");

            if (Text != text)
            {
                PdfString str = new PdfString(text);

                //BeforeTextChanges(text);
                Dictionary.SetString(DictionaryProperties.T, text);
                Changed = true;
            }
        }

        /// <summary>
        /// Searches the in parents.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="value">The value.</param>
        /// <returns>Serched primitive.</returns>
        internal static IPdfPrimitive SearchInParents(PdfDictionary dictionary, PdfCrossTable crossTable, string value)
        {
            IPdfPrimitive primitive = null;
            PdfDictionary dic = dictionary;

            while ((primitive == null) && (dic != null))
            {
                if (dic.ContainsKey(value))
                {
                    primitive = crossTable.GetObject(dic[value]);
                }
                else
                {
                    if (dic.ContainsKey(DictionaryProperties.Parent))
                    {
                        dic = crossTable.GetObject(dic[DictionaryProperties.Parent]) as PdfDictionary;
                    }
                    else
                    {
                        dic = null;
                    }
                }
            }

            return primitive;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="value">The value.</param>
        /// <param name="inheritable">if it is inheritable, set to <c>true</c>.</param>
        /// <returns>The founded value.</returns>
        internal static IPdfPrimitive GetValue(PdfDictionary dictionary, PdfCrossTable crossTable,
            string value, bool inheritable)
        {
            IPdfPrimitive primitive = null;

            if (dictionary.ContainsKey(value))
            {
                primitive = crossTable.GetObject(dictionary[value]);
            }
            else
            {
                if (inheritable)
                {
                    primitive = SearchInParents(dictionary, crossTable, value);
                }
            }

            return primitive;
        }

        /// <summary>
        /// Gets the widget annotation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The widget annotation dictionary.</returns>
        internal PdfDictionary GetWidgetAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfDictionary dic = null;

            if (dictionary.ContainsKey(DictionaryProperties.Kids))
            {
                PdfArray array = crossTable.GetObject(dictionary[DictionaryProperties.Kids]) as PdfArray;

                PdfReference reference = crossTable.GetReference(array[m_defaultIndex]) as PdfReference;
                dic = crossTable.GetObject(reference) as PdfDictionary;

            }

            if (dictionary.ContainsKey(DictionaryProperties.Subtype))
            {
                PdfName type = CrossTable.GetObject(dictionary[DictionaryProperties.Subtype]) as PdfName;

                if (type.Value == DictionaryProperties.Widget)
                {
                    dic = dictionary;
                }
            }

            return dic;
        }

        /// <summary>
        /// Aplies field name
        /// </summary>
        /// <param name="name">specified field name</param>
        internal override void ApplyText(string text)
        {

            SetText(text);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Begins the save.
        /// </summary>
        internal virtual void BeginSave()
        {
        }
        ///// <summary>
        ///// Gets the loaded page.
        ///// </summary>
        ///// <returns>The loaded page in which field draw.</returns>
        //private PdfPage GetLoadedPage()
        //{
        //    PdfPageBase page = base.Page;

        //    if (page == null)
        //    {
        //        PdfLoadedDocument doc =  CrossTable.Document as PdfLoadedDocument;

        //        PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

        //        if (widget == null)
        //        {
        //            widget = Dictionary;
        //        }

        //        if (widget.ContainsKey(DictionaryProperties.P))
        //        {
        //            IPdfPrimitive pageRef = CrossTable.GetObject(widget[DictionaryProperties.P]);
        //            PdfDictionary pageDic = pageRef as PdfDictionary;

        //            if (pageDic != null)
        //                page = doc.Pages.GetPage(pageDic);
        //        }
        //        else
        //        {
        //            PdfReference widgetReference = CrossTable.GetReference(widget);
        //            foreach (PdfLoadedPage loadedpage in doc.Pages)
        //            {
        //                PdfArray lAnnots = loadedpage.GetAnnots();

        //                if (lAnnots != null)
        //                {
        //                    for (int i = 0; i < lAnnots.Count; i++)
        //                    {
        //                        PdfReferenceHolder holder = lAnnots[i] as PdfReferenceHolder;
        //                        if (holder.Reference == widgetReference)
        //                        {
        //                            page = loadedpage;
        //                            return page as PdfPage;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return page as PdfPage;
        //}

        ///// <summary>
        ///// Exports the form fields.
        ///// </summary>
        ///// <param name="textWriter"></param>
        //internal void ExportText(System.Xml.XmlTextWriter textWriter)
        //{
        //    //PdfName name = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.Subtype, true) as PdfName;

        //    PdfString str = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.Contents, true) as PdfString;
        //    if (str != null)
        //    {
        //        textWriter.WriteStartElement(this.Text, "");
        //        textWriter.WriteString(str.Value);
        //        textWriter.WriteEndElement();
        //    }

        //}


        /// <summary>
        /// Exports the form fields.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="objectid">Object identifier.</param>

        internal void ExportText(Stream stream, ref int objectid)
        {
            bool flag = false;
            PdfArray kids = null;

            if (this.Dictionary.ContainsKey(DictionaryProperties.Kids))
            {
                kids = CrossTable.GetObject(Dictionary[DictionaryProperties.Kids]) as PdfArray;

                if (kids != null)
                {
                    for (int i = 0; i < kids.Count; i++)
                    {
                        flag = flag || (kids[i] is PdfLoadedAnnotation);
                    }
                }
            }

            //PdfName name = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.FT, true) as PdfName;

            PdfString tempname = GetValue(this.Dictionary, this.CrossTable, DictionaryProperties.Contents, true) as PdfString;

            string strValue = "";
            if (tempname != null)
                strValue = tempname.Value;

            if (!validateString(strValue) || flag)
            {
                if (flag)
                {
                    for (int i = 0; i < kids.Count; i++)
                    {
                        PdfLoadedAnnotation annot = kids[i] as PdfLoadedAnnotation;
                        if (annot != null)
                        {
                            annot.ExportText(stream, ref objectid);
                        }
                    }

                    this.ObjectID = objectid;
                    objectid++;

                    StringBuilder builder = new StringBuilder();

                    PdfString stringValue = new PdfString(strValue);
                    stringValue.Encode = PdfString.ForceEncoding.ASCII;
                    byte[] tempbuffer = Encoding.GetEncoding("windows-1252").GetBytes(stringValue.Value);

                    builder.AppendFormat("{0} 0 obj<</T <{1}> /Kids [", this.ObjectID, PdfString.BytesToHex(tempbuffer));

                    for (int i = 0; i < kids.Count; i++)
                    {
                        PdfLoadedAnnotation annot = kids[i] as PdfLoadedAnnotation;
                        if ((annot != null) && (annot.ObjectID != 0))
                        {
                            builder.AppendFormat("{0} 0 R ", annot.ObjectID);
                        }
                    }
                    builder.Append("]>>endobj\n");

                    PdfString builderString = new PdfString(builder.ToString());
                    builderString.Encode = PdfString.ForceEncoding.ASCII;
                    byte[] tempbuffer1 = Encoding.GetEncoding("windows-1252").GetBytes(builderString.Value);

                    stream.Write(tempbuffer1, 0, tempbuffer1.Length);
                }
                else
                {
                    this.ObjectID = objectid;
                    objectid++;

                    if ((this.GetType().Name == "PdfLoadedCheckBoxField") || (this.GetType().Name == "PdfLoadedRadioButtonListField"))
                    {
                        strValue = "/" + strValue;
                    }
                    else
                    {

                        PdfString stringFieldValue = new PdfString(strValue);
                        stringFieldValue.Encode = PdfString.ForceEncoding.ASCII;
                        byte[] tempbuffer2 = Encoding.GetEncoding("windows-1252").GetBytes(stringFieldValue.Value);

                        strValue = "<" + PdfString.BytesToHex(tempbuffer2) + ">";
                    }
                    StringBuilder builder2 = new StringBuilder();


                    PdfString stringAnnotText = new PdfString(this.Text);
                    stringAnnotText.Encode = PdfString.ForceEncoding.ASCII;
                    byte[] tempbuffer3 = Encoding.GetEncoding("windows-1252").GetBytes(stringAnnotText.Value);

                    builder2.AppendFormat("{0} 0 obj<</T <{1}> /Contents {2} >>endobj\n", this.ObjectID, PdfString.BytesToHex(tempbuffer3), strValue);

                    PdfString buildString = new PdfString(builder2.ToString());
                    buildString.Encode = PdfString.ForceEncoding.ASCII;
                    byte[] tempbuffer4 = Encoding.GetEncoding("windows-1252").GetBytes(buildString.Value);

                    stream.Write(tempbuffer4, 0, tempbuffer4.Length);
                }
            }
        }



        internal static bool validateString(string text1)
        {
            if (text1 != null)
            {
                return (text1.Length == 0);
            }
            return true;
        }

        //private string GetText()
        //{
        //    PdfString text = null;
        //    if (Dictionary.ContainsKey(DictionaryProperties.Contents))
        //    {
        //        text = Dictionary[DictionaryProperties.Contents] as PdfString;
        //    }
        //    return text.Value.ToString();
        //}

        #endregion
    }
}
