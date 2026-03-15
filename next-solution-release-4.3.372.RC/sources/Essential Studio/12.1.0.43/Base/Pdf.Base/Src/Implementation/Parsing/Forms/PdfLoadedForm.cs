#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Text;
using System.Xml;
using System.IO;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Security;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System.Collections.Generic;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
	/// <summary>
	/// Represents Loaded form.
	/// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load the existing form
    /// PdfLoadedForm form = doc.Form;
    /// //load the form field
    /// PdfLoadedField field = form.Fields[0] as PdfLoadedField;
    /// field.Export = true;
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load the existing form
    /// Dim form As PdfLoadedForm = doc.Form
    /// 'load the form field
    /// Dim field As PdfLoadedField = TryCast(form.Fields(0), PdfLoadedField)
    /// field.Export = True
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfForm"/> Class
    /// <seealso cref="PdfLoadedDocument"/> Class  
	public class PdfLoadedForm : PdfForm
	{
		#region Fields
		/// <summary>
		/// Collection of fields
		/// </summary>
		private PdfLoadedFormFieldCollection m_fields;

		/// <summary>
		/// Store crooss table.
		/// </summary>
		private PdfCrossTable m_crossTable;

		/// <summary>
		/// Dictionaries of tremil fields.
		/// </summary>
        private List<PdfDictionary> m_terminalFields = new List<PdfDictionary>();

		/// <summary>
		/// Indicates is field modified or not.
		/// </summary>
		private bool m_isModified;

        /// <summary>
        /// Indicates is xfa form or not.
        /// </summary>
        private bool m_isXFAForm=false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the field collection.
		/// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the existing form
        /// PdfLoadedForm form = doc.Form;
        /// //load the form field
        /// PdfLoadedField field = form.Fields[0] as PdfLoadedField;
        /// field.Export = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the existing form
        /// Dim form As PdfLoadedForm = doc.Form
        /// 'load the form field
        /// Dim field As PdfLoadedField = TryCast(form.Fields(0), PdfLoadedField)
        /// field.Export = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedForm"/> Class
		public new PdfLoadedFormFieldCollection Fields
		{
			get
			{
				if( m_fields == null )
				{
					m_fields = new PdfLoadedFormFieldCollection( this );
				}

				return m_fields;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the form is read only.
		/// </summary>
        /// <value>True if the field is read-only, false otherwise. Default is false.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the existing form
        /// PdfLoadedForm form = doc.Form;         
        /// //Set the form as read only
        /// form.ReadOnly = true;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the existing form
        /// Dim form As PdfLoadedForm = doc.Form
        /// 'Set the form as read only
        /// form.ReadOnly = True
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedForm"/> Class
		public override bool ReadOnly
		{
			get
			{
				return base.ReadOnly;
			}
			set
			{
				base.ReadOnly = value;

				foreach( PdfField field in Fields )
				{
					field.ReadOnly = value;
				}
			}
		}

		/// <summary>
		/// Gets the signature flags.
		/// </summary>
		internal override SignatureFlags SignatureFlags
		{
			get
			{
				return base.SignatureFlags;
			}
			set
			{
				base.SignatureFlags = value;
				IsModified = true;
				Dictionary.SetNumber( DictionaryProperties.SigFlags, ( int )value );
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether need appearances.
		/// </summary>
		internal override bool NeedAppearances
		{
			get
			{
				return base.NeedAppearances;
			}
			set
			{
				base.NeedAppearances = value;
				IsModified = true;
                //Dictionary.SetBoolean( DictionaryProperties.NeedAppearances, value );
			}
		}

		/// <summary>
		/// Gets the resources.
		/// </summary>
		internal override PdfResources Resources
		{
			get
			{
				return base.Resources;
			}
			set
			{
				base.Resources = value;
				IsModified = true;
				Dictionary.SetProperty( DictionaryProperties.DR, value );
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is modified.
		/// </summary>
		internal bool IsModified
		{
			get
			{
				return m_isModified;
			}
			set
			{
				m_isModified = value;
			}
		}

		/// <summary>
		/// Gets the cross table.
		/// </summary>
		internal PdfCrossTable CrossTable
		{
			get
			{
				return m_crossTable;
			}
		}

		/// <summary>
		/// Gets or sets the terminal fields.
		/// </summary>
		internal List<PdfDictionary> TerminalFields
		{
			get
			{
				return m_terminalFields;
			}
			set
			{
				m_terminalFields = value;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this form is XFA Form or AcroForm.
        /// </summary>
        internal bool IsXFAForm
        {
            get
            {
                return m_isXFAForm;
            }
           
            set
            {
                m_isXFAForm = value;
            }
        }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="PdfLoadedForm"/> class.
		/// </summary>
		/// <param name="formDictionary">The form dictionary.</param>
		/// <param name="crossTable">The cross table.</param>
		internal PdfLoadedForm( PdfDictionary formDictionary, PdfCrossTable crossTable )
			: this( crossTable )
		{
			Initialize( formDictionary, crossTable );
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="PdfLoadedForm"/> class.
		/// </summary>
		/// <param name="crossTable">The cross table.</param>
		internal PdfLoadedForm( PdfCrossTable crossTable )
		{
			m_crossTable = crossTable;
            Dictionary.SetBoolean(DictionaryProperties.NeedAppearances, NeedAppearances);
			CrossTable.Document.Catalog.BeginSave += new SavePdfPrimitiveEventHandler( Dictionary_BeginSave );
			CrossTable.Document.Catalog.Modify();
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Initializes the specified form dictionary.
		/// </summary>
		/// <param name="formDictionary">The form dictionary.</param>
		/// <param name="crossTable">The cross table.</param>
		private void Initialize( PdfDictionary formDictionary, PdfCrossTable crossTable )
		{
			if( formDictionary == null )
				throw new ArgumentNullException( "dictionary" );

			if( crossTable == null )
				throw new ArgumentNullException( "crossTable" );

			Dictionary = formDictionary;

            if (Dictionary.ContainsKey(DictionaryProperties.XFA))
            {
                m_isXFAForm = true;
                Dictionary.Remove(DictionaryProperties.XFA);
                //throw new ArgumentException( "We don\'t support XFA." );
            }

			//Get terminal fields.
			CreateFields();

			//Gets NeedAppearance
			if( Dictionary.ContainsKey( DictionaryProperties.NeedAppearances ) )
			{
				PdfBoolean needAppearance =
				m_crossTable.GetObject( Dictionary[ DictionaryProperties.NeedAppearances ] ) as PdfBoolean;

				base.NeedAppearances = needAppearance.Value;
                base.SetAppearanceDictionary = true;
			}
            else
                base.SetAppearanceDictionary = false;

			//Gets signature flag
			if( Dictionary.ContainsKey( DictionaryProperties.SigFlags ) )
			{
				PdfNumber sigFlag =
				m_crossTable.GetObject( Dictionary[ DictionaryProperties.SigFlags ] ) as PdfNumber;

				base.SignatureFlags = ( SignatureFlags )sigFlag.IntValue;
			}

			//Gets resource dictionary
			if( Dictionary.ContainsKey( DictionaryProperties.DR ) )
			{
				PdfDictionary resources =
					m_crossTable.GetObject( Dictionary[ DictionaryProperties.DR ] ) as PdfDictionary;

				PdfResources res = new PdfResources( resources );
                this.Resources = res;
				base.Resources = res;
			}
		}

		/// <summary>
		/// Retrieves the terminal fields.
		/// </summary>
		private void CreateFields()
		{
			PdfArray fields = null;

			if( Dictionary.ContainsKey( DictionaryProperties.Fields ) )
			{

				fields = m_crossTable.GetObject( Dictionary[ DictionaryProperties.Fields ] ) as PdfArray;
			}

			int count = 0;
            Stack<NodeInfo> nodes = new Stack<NodeInfo>();

			while( true && fields != null)
			{
				for( ; count < fields.Count; ++count )
				{
					PdfDictionary fieldDictionary = m_crossTable.GetObject( fields[ count ] ) as PdfDictionary;

					PdfArray fieldKids = null;

					if( fieldDictionary.ContainsKey( DictionaryProperties.Kids ) )
					{
						fieldKids = m_crossTable.GetObject( fieldDictionary[ DictionaryProperties.Kids ] ) as PdfArray;
					}

					if( fieldKids == null )
					{
                        if(fieldDictionary!=null)
                        {
                            if (!m_terminalFields.Contains(fieldDictionary))
                            {
						        m_terminalFields.Add( fieldDictionary );
                            }
                        }
					}
					else
					{
						bool isNode = !fieldDictionary.ContainsKey( DictionaryProperties.FT) || IsNode( fieldKids );

						if( isNode )
						{
							NodeInfo info = new NodeInfo( fields, count );
							nodes.Push( info );

							count = -1;
							fields = fieldKids;
						}
						else
						{
							m_terminalFields.Add( fieldDictionary );
						}
					}
				}

				if( nodes.Count == 0 )
				{
					break;
				}

				NodeInfo nInfo = nodes.Pop() as NodeInfo;
				fields = nInfo.Fields;
				count = nInfo.Count + 1;
			}
		}

		/// <summary>
		/// Determines whether the specified kids is node.
		/// </summary>
		/// <param name="kids">The kids.</param>
		/// <returns>
		/// 	<c>true</c> if the specified kids is node; otherwise, <c>false</c>.
		/// </returns>
		private bool IsNode( PdfArray kids )
		{
			bool isNode = false;

			if( kids.Count >= 1 )
			{
				PdfDictionary dictionary = m_crossTable.GetObject( kids[ 0 ] ) as PdfDictionary;

				if( dictionary.ContainsKey( DictionaryProperties.Subtype ) )
				{
					PdfName name =
						m_crossTable.GetObject( dictionary[ DictionaryProperties.Subtype ] ) as PdfName;

					if( name.Value != DictionaryProperties.Widget )
					{
						isNode = true;
					}
				}
			}

			return isNode;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
		 /// <summary>
        /// Export the form data to a file.
        /// </summary>
        /// <param name="fileName">Name of the document which is need to export.</param>
        /// <param name="dataFormat">The format of exported data.</param>
        /// <param name="formName"> The name of the PDF file the data is exported from.</param>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the existing form
        /// PdfLoadedForm form = doc.Form;
        /// form.ExportData("Export.xml", DataFormat.Xml, "SourceForm.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the existing form
        /// Dim form As PdfLoadedForm = doc.Form
        /// form.ExportData("Export.xml", DataFormat.Xml, "SourceForm.pdf")
        /// </code>
        /// </example>
        public void ExportData(String fileName, DataFormat dataFormat, string formName)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                this.ExportData(stream, dataFormat, formName);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }


        /// <summary>
        /// Export the form data to a file.
        /// </summary>
        /// <param name="fileName">The stream where form data will be exported.</param>
        /// <param name="dataFormat">The format of exported data</param>
        /// <param name="formName"> The name of the PDF file the data is exported from</param>
        /// <example>
        /// <code lang="C#">
        ///  // Loads an existing document
        ///  PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the existing form
        /// PdfLoadedForm form = doc.Form;
        /// MemoryStream stream = new MemoryStream();
        /// form.ExportData(stream, DataFormat.Xml, "SourceForm.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the existing form
        /// Dim form As PdfLoadedForm = doc.Form
        /// Dim stream As MemoryStream = New MemoryStream()
        /// form.ExportData(stream, DataFormat.Xml, "SourceForm.pdf")
        /// </code>
        /// </example>
        public void ExportData(Stream stream, DataFormat dataFormat, string formName)
        {
            if (dataFormat == DataFormat.Xml)
            {
                this.ExportDataXML(stream);
            } 
			if (dataFormat == DataFormat.Fdf)
            {
                this.ExportDataFDF(stream, formName);
            }
            if (dataFormat == DataFormat.XFdf)
            {
                this.ExportDataXFDF(stream, formName);
            }

        }

        /// <summary>
        /// Export the form data in XML Forms Data Format file format.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="formName">Name of the form.</param>
        private void ExportDataXFDF(Stream stream, string formName)
        {
            XFdfDocument Xfdf = new XFdfDocument(formName);

            for (int i = 0; i < this.Fields.Count; i++)
            {
                PdfLoadedField field = (PdfLoadedField)this.Fields[i];
                if (field.Export)
                {
                    PdfName name = PdfLoadedField.GetValue(field.Dictionary, field.CrossTable, DictionaryProperties.FT, true) as PdfName;

                    switch (name.Value)
                    {
                        case "Tx":
                            PdfString str = PdfLoadedField.GetValue(field.Dictionary, field.CrossTable, DictionaryProperties.V, true) as PdfString;

                            if (str != null)
                            {
                                Xfdf.SetFields(field.Name, str.Value);
                            }
                            break;
                        case "Ch":

                            if (field.GetType().Name == "PdfLoadedListBoxField")
                            {
                                PdfArray array = PdfLoadedField.GetValue(field.Dictionary, field.CrossTable, DictionaryProperties.V, true) as PdfArray;

                                if (array != null)
                                {
                                    Xfdf.SetFields(field.Name, array);
                                }
                                else 
                                {
                                   PdfString listValue = PdfLoadedField.GetValue(field.Dictionary, field.CrossTable, DictionaryProperties.V, true) as PdfString;
                                     if(listValue != null)
                                        Xfdf.SetFields(field.Name, listValue.Value);
                                //   PdfNumber SelectedIndex =  PdfLoadedField.GetValue(field.Dictionary, field.CrossTable, DictionaryProperties.I, true) as PdfNumber;
                                //   array = (field as PdfLoadedListBoxField).Values[SelectedIndex.IntValue] as PdfArray;
                                    
                                //    if(array != null)
                                //        Xfdf.SetFields(field.Name, array);
                                }

                            }
                            else
                            {
                                PdfName str1 = PdfLoadedField.GetValue(field.Dictionary, field.CrossTable, DictionaryProperties.V, true) as PdfName;

                                if (str1 != null)
                                    Xfdf.SetFields(field.Name, str1.Value);
                                else
                                {
                                    PdfString ComboValue = PdfLoadedField.GetValue(field.Dictionary, field.CrossTable, DictionaryProperties.V, true) as PdfString;
                                    if( ComboValue != null)
                                      Xfdf.SetFields(field.Name, ComboValue.Value);
                                }
                            }

                            break;

                        case "Btn":
                            PdfName str2 = PdfLoadedField.GetValue(field.Dictionary, field.CrossTable, DictionaryProperties.V, true) as PdfName;
                            if (str2 != null)
                            {
                                Xfdf.SetFields(field.Name, str2.Value);
                            }
                            else
                            {
                                PdfDictionary holder = field.GetWidgetAnnotation(field.Dictionary, field.CrossTable) as PdfDictionary;
                                if ((holder["AS"] as PdfName) != null)
                                {
                                    Xfdf.SetFields(field.Name, (holder["AS"] as PdfName).Value);
                                }
                            }
                            break;
                    }

                }
            }

            Xfdf.Save(stream);
        }
		
		   /// <summary>
        /// Export the form data in FDF file format.
        /// </summary>
        /// <param name="stream">The stream where form data will be exported.</param>
        /// <param name="formName">The name of the PDF file the data is exported from.</param>
        private void ExportDataFDF(Stream stream, string formName)
        {
            new BinaryWriter(stream);

            PdfString headerString = new PdfString("%FDF-1.2\n");
            headerString.Encode = PdfString.ForceEncoding.ASCII; 
            byte[] buffer = Encoding.GetEncoding("windows-1252").GetBytes(headerString.Value);

            stream.Write(buffer, 0, buffer.Length);
            int count = 1;

            for (int i = 0; i < this.Fields.Count; i++)
            {
                PdfLoadedField field = (PdfLoadedField)this.Fields[i];
                if (field.Export)
                {
                    field.ExportField(stream,ref count);
                }
            }

            StringBuilder builder = new StringBuilder();
            PdfString formNameEncodedString = new PdfString(formName);
            formNameEncodedString.Encode = PdfString.ForceEncoding.ASCII;
            buffer = Encoding.GetEncoding("windows-1252").GetBytes(formNameEncodedString.Value);
            
            builder.AppendFormat("{0} 0 obj<</F <{1}>  /Fields [", count, PdfString.BytesToHex(buffer));

              for (int i = 0; i < this.Fields.Count; i++)
            {
                PdfLoadedField field = (PdfLoadedField)this.Fields[i];
                if (field.Export && field.ObjectID !=0)
                {              
                    builder.AppendFormat("{0} 0 R ", field.ObjectID);
                }
            }
            builder.Append("]>>endobj\n");
            builder.AppendFormat("{0} 0 obj<</Version /1.4 /FDF {1} 0 R>>endobj\n", count + 1, count);
            builder.AppendFormat("trailer\n<</Root {0} 0 R>>\n", count + 1);

            PdfString fdfString = new PdfString(builder.ToString());
            fdfString.Encode = PdfString.ForceEncoding.ASCII;
            buffer = Encoding.GetEncoding("windows-1252").GetBytes(fdfString.Value);
  
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush(); 
        }   

        /// <summary>
        /// Exports the form data in XML file format
        /// </summary>
        /// <param name="stream"></param>
        internal void ExportDataXML(Stream stream)
        {
            XmlTextWriter textWriter = new XmlTextWriter(stream, new UTF8Encoding());
            textWriter.Formatting = Formatting.Indented;
            textWriter.WriteStartDocument();
            textWriter.WriteStartElement("Fields", "");

            for (int i = 0; i < this.Fields.Count; i++)
            {
                PdfLoadedField field = (PdfLoadedField)this.Fields[i];
                if (field.Export)
                {
                    field.ExportField(textWriter);
                }
            }
            textWriter.WriteEndElement();

            textWriter.Flush();

        }
#endif

        /// <summary>
        /// Validate the XML node element
        /// </summary>
        /// <param name="nodeName">name of the XML element node.</param>
        internal void OnValidate(string nodeName)
        {
            if (nodeName.StartsWith("XML"))
                throw new Exception("Element type names may not start with XML");
            if (nodeName.StartsWith("_"))
                throw new Exception("Element type names must start with a letter or underscore");
            if (!char.IsLetter(nodeName[0]) && !char.IsNumber(nodeName[0]))
                throw new Exception("Element type names must start with a letter or underscore");

        }
		
		/// <summary>
		/// Handles the BeginSave event of the Dictionary control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
		internal override void Dictionary_BeginSave( object sender, SavePdfPrimitiveEventArgs ars )
		{
			int i = 0;

            if (base.SignatureFlags != SignatureFlags.None)
            {
                NeedAppearances = false;
                if (Dictionary.ContainsKey(DictionaryProperties.NeedAppearances))
                    Dictionary.SetBoolean(DictionaryProperties.NeedAppearances, NeedAppearances);
            }

			while( i < Fields.Count )
			{
				PdfLoadedField field = Fields[ i ] as PdfLoadedField;

                if (field != null)
                {
                    if (field.DisableAutoFormat && field.Dictionary.ContainsKey("AA"))
                    {
                        field.Dictionary.Remove("AA");
                        field.BeginSave();
                    }
                }
				if( field != null )
				{
                    int fieldFlag = 0;
                    PdfDictionary dic = field.Dictionary;
                    if (dic.ContainsKey(DictionaryProperties.F))
                    {
                        fieldFlag = (dic[DictionaryProperties.F] as PdfNumber).IntValue;
                    }
                    if( field.Flatten&&fieldFlag!=6)
					{
						field.Draw();
						Fields.Remove( field );
						--i;
					}
					else if( field.Changed )
					{
						field.BeginSave();
					}
				}
				else
				{
					PdfField simplefield = Fields[ i ] as PdfField;

					if( simplefield.Flatten )
					{
						Fields.Remove( simplefield );

						simplefield.Draw();

						--i;
					}
					else
					{
						simplefield.Save();
					}
				}

				++i;
			}

            if (m_fields.Count == 0)
            {
                Dictionary.Clear(); 
            }
            else if (SetAppearanceDictionary)
                Dictionary.SetBoolean(DictionaryProperties.NeedAppearances, NeedAppearances);
		}

        /// <summary>
        /// Clears PdfLoadedForm.
        /// </summary>
        internal override void Clear()
        {
            if (m_fields != null)
                m_fields.Clear();
            if (m_pageMap != null)
                m_pageMap.Clear();
            if (m_terminalFields != null)
                m_terminalFields.Clear();
            Dictionary.Clear();
        }
 
		/// <summary>
		/// Removes field and kids annotation from dictionaries.
		/// </summary>
		/// <param name="field">The field.</param>
		internal void RemoveFromDictionaries( PdfField field )
		{
            if (m_fields != null && m_fields.Count > 0)
			{
                PdfName fieldsDict = new PdfName(DictionaryProperties.Fields);
                PdfArray array = m_crossTable.GetObject(Dictionary[fieldsDict]) as PdfArray;

				PdfReferenceHolder holder = new PdfReferenceHolder( field.Dictionary );

				array.Remove( holder );
				array.MarkChanged();

                Dictionary.SetProperty(fieldsDict, array);
			}

			if( field is PdfLoadedField )
			{
				DeleteFromPages( field );
				DeleteAnnottation( field );
			}
		}

		/// <summary>
		/// Deletes from pages.
		/// </summary>
		/// <param name="field">The field.</param>
		internal void DeleteFromPages( PdfField field )
		{
			PdfDictionary dic = field.Dictionary;
            PdfName kidsName = new PdfName(DictionaryProperties.Kids);
            PdfName annotsName = new PdfName(DictionaryProperties.Annots);
            PdfName pName = new PdfName(DictionaryProperties.P);

            if (dic.ContainsKey(kidsName))
			{
                PdfArray array = CrossTable.GetObject(dic[kidsName]) as PdfArray;

				for( int i = 0, size = array.Count; i < size; ++i )
				{
					PdfReferenceHolder holder = array[ i ] as PdfReferenceHolder;

					PdfDictionary widget = CrossTable.GetObject( holder ) as PdfDictionary;

                    PdfReference pageRef = null;
                    if (widget.ContainsKey(pName))
                        pageRef = CrossTable.GetReference(widget[pName]) as PdfReference;
                    else if (dic.ContainsKey(pName))
                        pageRef = CrossTable.GetReference(dic[pName]) as PdfReference;
                    else if (field.Page != null)
                        pageRef = CrossTable.GetReference(field.Page.Dictionary) as PdfReference;
					PdfDictionary page = CrossTable.GetObject( pageRef ) as PdfDictionary;

                    if (page != null && page.ContainsKey(annotsName))
					{
                        PdfArray annots = CrossTable.GetObject(page[annotsName]) as PdfArray;
						annots.Remove( holder );
						annots.MarkChanged();
                        page.SetProperty(annotsName, annots);
					}
				}
			}
			else
			{
                PdfReference pageRef = null;

                if (dic.ContainsKey(pName))
                    pageRef = CrossTable.GetReference(dic[pName]) as PdfReference;
                else if (field.Page != null)
                    pageRef = CrossTable.GetReference(field.Page.Dictionary) as PdfReference;

				PdfDictionary page = CrossTable.GetObject( pageRef ) as PdfDictionary;

                if (page != null && page.ContainsKey(annotsName))
				{
                    PdfArray annots = CrossTable.GetObject(page[annotsName]) as PdfArray;
					annots.Remove( new PdfReferenceHolder( dic ) );
					annots.MarkChanged();
                    page.SetProperty(annotsName, annots);
				}
			}
		}

		/// <summary>
        /// Deletes the annotation from the page dictionary.
		/// </summary>
		/// <param name="field">The field.</param>
		internal void DeleteAnnottation( PdfField field )
		{
			PdfDictionary dic = field.Dictionary;
            PdfName kidsName = new PdfName(DictionaryProperties.Kids);

            if (dic.ContainsKey(kidsName))
			{
                PdfArray array = m_crossTable.GetObject(dic[kidsName]) as PdfArray;

				array.Clear();

                dic.SetProperty(kidsName, array);
			}
		}

		/// <summary>
		/// Gets the new name of the field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The field name.</returns>
		internal override string GetCorrectName( string name )
		{
            List<string> list = new List<string>();
			for( int i = 0; i < Fields.Count; ++i )
			{
				list.Add( Fields[ i ].Name );
			}

			string correctName = name;

			int index = 0;

			while( list.IndexOf( correctName ) != -1 )
			{
				correctName = name + index;
				++index;
			}

			return correctName;
		}

#if !SILVERLIGHT && !NETFX_CORE && !WP

        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dataFormat">The data format.</param>
        public void ImportData(string fileName, DataFormat dataFormat)
        {
            this.ImportDataField(fileName, dataFormat, false);
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dataFormat">The data format.</param>
        /// <param name="errorFlag">if it is error flag, set to <c>true</c>.</param>
        /// <returns></returns>
        /// <example>
        /// <code lang="C#">
        ///     //Load an existing document
        ///     PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        ///     // Load the existing form
        ///     PdfLoadedForm form = doc.Form;            
        ///     form.ImportData("ImportData.xml",DataFormat.Xml, false);
        ///     doc.Save("Import.pdf");
        /// </code>
        /// <code lang="VB">
        /// 	 'Load an existing document
        /// 	 Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// 	 ' Load the existing form
        /// 	 Dim form As PdfLoadedForm = doc.Form
        /// 	 form.ImportData("ImportData.xml",DataFormat.Xml, False)
        /// 	 doc.Save("Import.pdf")
        /// </code>
        /// </example>
        public PdfLoadedFieldImportError[] ImportData(string fileName, DataFormat dataFormat, bool errorFlag)
        {
            PdfLoadedFieldImportError[] errorArray = this.ImportDataField(fileName, dataFormat, errorFlag);
            return errorArray;
        }

        /// <summary>
        /// Imports the data field.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dataFormat">The data format.</param>
        /// <param name="continueImportOnError">if it is continue import on error, set to <c>true</c>.</param>
        /// <returns></returns>
        private PdfLoadedFieldImportError[] ImportDataField(string fileName, DataFormat dataFormat, bool continueImportOnError)
        {
            FileStream stream = null;
            PdfLoadedFieldImportError[] errorArray = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                errorArray = this.ImportData(stream, dataFormat, continueImportOnError);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return errorArray;           
        }

        /// <summary>
        /// Imports Form value from XML file
        /// </summary>
        /// <param name="fileName">Name of the imported file.</param>
        /// <param name="dataFormat">The input file format</param>
        /// <param name="continueImportOnError">False if the import should stop on the first field that generates an error, or true if the import should ignore the error and continue with the next field.</param>
        /// <returns>Document form fields filled with data which are imported from XML.</returns>
        private PdfLoadedFieldImportError[] ImportData(Stream fileName, DataFormat dataFormat, bool continueImportOnError)
        {
            if (dataFormat == DataFormat.Xml)
            {
               return this.ImportData(fileName, continueImportOnError);
            }
            if (dataFormat == DataFormat.Fdf)
            {
                return this.ImportDataFDF(fileName, continueImportOnError);
            }
            return null;
        }


        /// <summary>
        /// Import form data from FDF file.
        /// </summary>
        /// <param name="stream">The FDF file stream</param>
        /// <param name="continueImportOnError">False if the import should stop on the first field that generates an error, or true if the import should ignore the error and continue with the next field.</param>
        /// <returns>Document form fields filled with data which are imported from FDF.</returns>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the existing form
        /// PdfLoadedForm form = doc.Form;
        /// // Load the FDF file
        /// FileStream stream = new FileStream("ImportFDF.fdf", FileMode.Open);
        /// // Import the FDF stream
        /// form.ImportDataFDF(stream,true);
        /// doc.Save("Import.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the existing form
        /// Dim form As PdfLoadedForm = doc.Form
        /// ' Load the FDF file
        /// Dim stream As FileStream = New FileStream("ImportFDF.fdf", FileMode.Open)
        /// ' Import the FDF stream
        /// form.ImportDataFDF(stream,True)
        /// doc.Save("Import.pdf")
        /// </code>
        /// </example>
        public PdfLoadedFieldImportError[] ImportDataFDF(Stream stream, bool continueImportOnError)
        {
            PdfReader reader = new PdfReader(stream);

            reader.Position = 0;
            string token = reader.GetNextToken();

            #region FDFHeader
            if (token.StartsWith("%"))
            {
                token = reader.GetNextToken();
                if (!token.StartsWith("FDF-"))
                {
                    throw new Exception("The source is not a valid FDF file because it does not start with\"%FDF-\"");
                }

            }
            #endregion

            Hashtable table = new Hashtable();

            string field_name = "";
            string form_name = "";
            token = reader.GetNextToken();
            while (token != null && token != string.Empty)
            {
                if (token.ToUpper() == "T")
                {
                    token = reader.GetNextToken();
                    while (token != ">" && token != ")")
                    {
                        token = reader.GetNextToken();
                        if (token != ">" && token != ")")
                        {
                            if (OnlyHexInString(token))
                            {
                                PdfString str = new PdfString();
                                byte[] buffer = str.HexToBytes(token);
                                field_name = PdfString.ByteToString(buffer);
                                break;
                            }
                            else
                            {
                                field_name = token;
                            }
                        }
                    }
                }
                if (token.ToUpper() == "V")
                {
                    token = reader.GetNextToken();
                    while (token != ">" && token != ")")
                    {
                        if (token == Operators.Slash || token != ")")
                        {
                            token = reader.GetNextToken();
                            if (token != ">" && token != ")")
                            {
                                if (OnlyHexInString(token))
                                {
                                    PdfString str = new PdfString();
                                    byte[] buffer = str.HexToBytes(token);
                                    token = PdfString.ByteToString(buffer);
                                    table.Add(field_name, token);
                                    break;
                                }
                                else
                                {
                                    table.Add(field_name, token);
                                }
                            }

                        }
                        token = reader.GetNextToken();
                    }
                }
                if (token.ToUpper() == "F")
                {
                    token = reader.GetNextToken();
                    while (token != ">")
                    {
                        token = reader.GetNextToken();
                        if (token != ">")
                        {
                            PdfString str = new PdfString();
                            byte[] buffer = str.HexToBytes(token);
                            form_name = PdfString.ByteToString(buffer);
                        }
                    }
                }

                Console.WriteLine(token.ToString() + "\n");
                token = reader.GetNextToken();
            }
            PdfLoadedField field = null;

            #region UpdateFormFields
            foreach (DictionaryEntry entry in table)
            {
                try
                {
                    string s = entry.Key.ToString();
                    field = (PdfLoadedField)Fields[s];
                    if (field != null)
                    {
                        field.ImportFieldValue(entry.Value.ToString());
                    }
                }
                catch
                {
                    if (!continueImportOnError)
                    {
                        throw;
                    }

                }

            }
            #endregion

            return null;
        }
#endif

        /// <summary>
        /// Sets/Resets the form field highlight option.
        /// </summary>
        public void HighlightFields(bool highlight)
        {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Action);
            dictionary[DictionaryProperties.S] = new PdfName(DictionaryProperties.JavaScript);

            if (highlight)
            {
                dictionary[DictionaryProperties.JS] = new PdfString("app.runtimeHighlight = true;");
            }
            else
            {
                dictionary[DictionaryProperties.JS] = new PdfString("app.runtimeHighlight = false;");
            }

            CrossTable.Document.Catalog[DictionaryProperties.OpenAction] = dictionary;
            CrossTable.Document.Catalog.Modify();
        }

        /// <summary>
        /// Called when [hex in string].
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns></returns>
        public bool OnlyHexInString(string test)
        {
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Import form data
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="continueImportOnError"></param>
        /// <returns></returns>
        private PdfLoadedFieldImportError[] ImportData(Stream stream, bool continueImportOnError)
        {
            XmlDocument document = new XmlDocument();
            document.Load(stream);
            if (document.DocumentElement.LocalName.ToUpper() != "fields".ToUpper())
            {
                throw new ArgumentException("The XML form data stream is not valid");
            }

            ArrayList list = new ArrayList();

            this.ImportXMLData(document.DocumentElement.ChildNodes, continueImportOnError, list);
            if (list.Count == 0)
            {
                return null;
            }
            return (PdfLoadedFieldImportError[])list.ToArray(typeof(PdfLoadedFieldImportError));
        }


        /// <summary>
        /// Import XML Data
        /// </summary>
        /// <param name="xmlnode"></param>
        /// <param name="continueImportOnError"></param>
        /// <param name="list"></param>
        private void ImportXMLData(XmlNodeList xmlnode, bool continueImportOnError, ArrayList list)
        {
            for (int i = 0; i < xmlnode.Count; i++)
            {
                XmlText text = xmlnode[i] as XmlText;
                if (text != null)
                {
                    string data = text.Data;
                    XmlNode parentNode = text.ParentNode;
                    string str2 = "";
                    while (parentNode.LocalName.ToUpper() != "fields".ToUpper())
                    {
                        if (str2.Length > 0)
                        {
                            str2 = "." + str2;
                        }
                        str2 = parentNode.LocalName + str2;
                        parentNode = parentNode.ParentNode;
                    }
                    PdfLoadedField field = null;
                    try
                    {                 
                      field = (PdfLoadedField)Fields[str2];                 
                      if (field != null)
                      {
                          field.ImportFieldValue(data);
                      }                
                  }
                  catch (Exception exception)
                  {
                      if (!continueImportOnError)
                      {
                          throw;
                      }
                      PdfLoadedFieldImportError error = new PdfLoadedFieldImportError(field, exception);
                      list.Add(error);
                  }
                }
                if (xmlnode[i].ChildNodes != null)
                {
                    this.ImportXMLData(xmlnode[i].ChildNodes, continueImportOnError, list);
                }
            }
        }

        /// <summary>
        /// Imports XFDF Data
        /// </summary>
        /// <param name="fileName"></param>
        public void ImportDataXFDF(string fileName)
        {
            Stream stream = new FileStream(fileName,FileMode.Open);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);
            
            XmlNodeList fieldNodes = xmlDoc.GetElementsByTagName("field");
            XmlNodeList valueNodes = xmlDoc.GetElementsByTagName("value");
            
            string[] fieldName = new string[fieldNodes.Count];
            string[] fieldValue = new string[valueNodes.Count];

			PdfLoadedField formField = null;

            for (int i = 0; i < fieldNodes.Count;i++)
            {
                fieldName[i] = fieldNodes[i].Attributes["name"].Value;

                formField = (PdfLoadedField)Fields[fieldName[i]];

                fieldValue[i] = valueNodes[i].InnerText;

                formField.ImportFieldValue(fieldValue[i]);
            }
            stream.Dispose();
        }

        /// <summary>
        /// Imports XFDF Data
        /// </summary>
        /// <param name="Stream"></param>
        public void ImportDataXFDF(Stream stream)
        {
            if (stream != null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(stream);

                XmlNodeList fieldNodes = xmlDoc.GetElementsByTagName("field");
                XmlNodeList valueNodes = xmlDoc.GetElementsByTagName("value");

                string[] fieldName = new string[fieldNodes.Count];
                string[] fieldValue = new string[valueNodes.Count];

                PdfLoadedField formField = null;

                for (int i = 0; i < fieldNodes.Count; i++)
                {
                    fieldName[i] = fieldNodes[i].Attributes["name"].Value;

                    formField = (PdfLoadedField)Fields[fieldName[i]];

                    fieldValue[i] = valueNodes[i].InnerText;

                    formField.ImportFieldValue(fieldValue[i]);
                }
                stream.Dispose();
            }
        }
#endif
        #endregion

        #region Internal declarations
        /// <summary>
        /// Represents node information.
        /// </summary>
        private class NodeInfo
        {
            #region Fields
            /// <summary>
            /// Parsed field count.
            /// </summary>
            private int m_count;
            /// <summary>
            /// Current kids array.
            /// </summary>
            private PdfArray m_fields;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the current array.
            /// </summary>
            internal PdfArray Fields
            {
                get
                {
                    return m_fields;
                }
                set
                {
                    m_fields = value;
                }
            }

            /// <summary>
            /// Gets or sets the count.
            /// </summary>
            internal int Count
            {
                get
                {
                    return m_count;
                }
                set
                {
                    m_count = value;
                }
            }
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="NodeInfo"/> class.
            /// </summary>
            /// <param name="fields">The fields.</param>
            /// <param name="count">The count.</param>
            internal NodeInfo(PdfArray fields, int count)
            {
                m_fields = fields;
                m_count = count;
            }
            #endregion
        }
        #endregion
    }

    #region InternalClass
    /// <summary>
    /// Represents errors on importing loaded field.
    /// </summary>
    public class PdfLoadedFieldImportError
    {
        // Fields
        private PdfLoadedField loadedFieldName;
        private Exception exceptionDetails;

        // Methods
        internal PdfLoadedFieldImportError(PdfLoadedField field, Exception exception)
        {
            this.loadedFieldName = field;
            this.exceptionDetails = exception;
        }

        // Properties
        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception
        {
            get
            {
                return this.exceptionDetails;
            }
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>The field.</value>
        public PdfLoadedField Field
        {
            get
            {
                return this.loadedFieldName;
            }
        }
    }

 

    #endregion

}
