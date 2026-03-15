#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Interactive
{
	/// <summary>
	/// Base class for annotation objects.
	/// </summary>
    public abstract class PdfAnnotation1 : IPdfWrapper
	{
		#region Fields
		/// <summary>
		/// Color of the annotation.
		/// </summary>
		private PdfColor m_color = PdfColor.Empty;
		/// <summary>
		/// Border of the annotation.
		/// </summary>
		private PdfAnnotationBorder m_border;
		/// <summary>
		/// Bounds of the annotation.
		/// </summary>
		private RectangleF m_rectangle = RectangleF.Empty;
		/// <summary>
		/// Parent page of the annotation.
		/// </summary>
		private PdfPage m_page;
		/// <summary>
		/// Text of the annotation.
		/// </summary>
		private string m_text = String.Empty;
		/// <summary>
		/// NAnootation's style flags.
		/// </summary>
		private PdfAnnotationFlags m_annotationFlags;
		/// <summary>
		/// Internal variable to store dictionary.
		/// </summary>
		private PdfDictionary m_dictionary = new PdfDictionary();

        private PdfAnnotationCollection m_annotations;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the background of the annotation�s icon when closed.
		/// The title bar of the annotation�s pop-up window.
		/// The border of a link annotation.
		/// </summary>
		/// <value>The color.</value>
		public PdfColor Color
		{
			get
			{
				return m_color;
			}
			set
			{
				if( m_color != value )
				{
					m_color = value;

					PdfColorSpace cs = PdfColorSpace.RGB;

					if( Page != null )
					{
						cs = Page.Section.Parent.Document.ColorSpace;
					}

					PdfArray colours = m_color.ToArray( cs );

					m_dictionary.SetProperty( DictionaryProperties.C, colours );
				}
			}
		}

		/// <summary>
		/// Gets or sets annotation's border.
		/// </summary>
		public PdfAnnotationBorder Border
		{
			get
			{
				if( m_border == null )
				{
					m_border = new PdfAnnotationBorder();
				}

				return m_border;
			}
			set
			{
				m_border = value;
			}
		}

		/// <summary>
		/// Gets or sets annotation's bounds. If this property is not set bounds are calculated automatically
		/// based on <see cref="Location">Location</see> property and content of annotation.
		/// </summary>
		public RectangleF Bounds
		{
			get
			{
				return m_rectangle;
			}
			set
			{
				if( m_rectangle != value )
				{
					m_rectangle = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets location of the annotation.
		/// </summary>
		public PointF Location
		{
			get
			{
				return m_rectangle.Location;
			}
			set
			{
				m_rectangle.Location = value;
			}
		}

		/// <summary>
		/// Gets or sets size of the annotation.
		/// </summary>
		public SizeF Size
		{
			get
			{
				return m_rectangle.Size;
			}
			set
			{
				m_rectangle.Size = value;
			}
		}

		/// <summary>
		/// Gets a page which this annotation is connected to.
		/// </summary>
		public PdfPage Page
		{
			get
			{
				return m_page;
			}
		}

		/// <summary>
		/// Gets or sets content of the annotation.
		/// </summary>
		public string Text
		{
			get
			{
				return m_text;
			}
			set
			{
				if( value == null )
					throw new ArgumentNullException( "Text" );

				if( m_text != value )
				{
					m_text = value;
					m_dictionary.SetString( DictionaryProperties.Contents, m_text );
				}
			}
		}

		/// <summary>
		/// Gets or sets annotation flags.
		/// </summary>
		public PdfAnnotationFlags AnnotationFlags
		{
			get
			{
				return m_annotationFlags;
			}
			set
			{
				if( m_annotationFlags != value )
				{
					m_annotationFlags = value;
					m_dictionary.SetNumber( DictionaryProperties.F, ( int )m_annotationFlags );
				}
			}
		}

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <value>The dictionary.</value>
		internal PdfDictionary Dictionary
		{
			get
			{
				return m_dictionary;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Creates new annotation object.
		/// </summary>
		protected PdfAnnotation1()
			: base()
		{
            if(this.Dictionary.ContainsKey( DictionaryProperties.Annots))
                m_annotations.Annotations = this.Dictionary[ DictionaryProperties.Annots ] as PdfArray;
			Initialize();
		}

		/// <summary>
		/// Creates new annotation object with the specified bounds.
		/// </summary>
		/// <param name="bounds">Bounds of the annotation.</param>
		protected PdfAnnotation1( RectangleF bounds )
			: base()
		{
			Initialize();
			Bounds = bounds;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Sets related page of the annotation.
		/// </summary>
		/// <param name="page"></param>
		internal void SetPage( PdfPage page )
		{
            m_page = page;

			if( m_page == null )
			{
				m_dictionary.Remove( new PdfName( DictionaryProperties.P ) );
			}
			else
			{
				m_dictionary.SetProperty( DictionaryProperties.P, new PdfReferenceHolder( m_page ) );
			}
		}

		/// <summary>
		/// Sets the location.
		/// </summary>
		/// <param name="location">The location.</param>
		internal void SetLocation( PointF location )
		{
			m_rectangle.Location = location;
		}

		/// <summary>
		/// Sets the size.
		/// </summary>
		/// <param name="size">The size.</param>
		internal void SetSize( SizeF size )
		{
			m_rectangle.Size = size;
		}

		/// <summary>
		/// Initializes annotation object.
		/// </summary>
		protected virtual void Initialize()
		{
			m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler( Dictionary_BeginSave );
			m_dictionary.SetProperty( DictionaryProperties.Type, new PdfName( DictionaryProperties.Annot ) );
		}

		/// <summary>
		/// Handles the BeginSave event of the Dictionary.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
		private void Dictionary_BeginSave( object sender, SavePdfPrimitiveEventArgs ars )
		{
			Save();
		}

		/// <summary>
		/// Saves an annotation.
		/// </summary>
		protected virtual void Save()
		{
            if ((this.GetType().ToString().Contains("Pdf3DAnnotation")
                || this.GetType().ToString().Contains("PdfAttachmentAnnotation")
                || this.GetType().ToString().Contains("PdfSoundAnnotation")
                || this.GetType().ToString().Contains("PdfActionAnnotation")) && (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B || PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_X1A2001))
            {
#if NETFX_CORE || WP
                throw new Exception("The specified annotation type is not supported by PDF/A1-B standard document.");
#else
                throw new PdfConformanceException("The specified annotation type is not supported by PDF/A1-B standard document.");
#endif
            }

			if( m_border != null )
			{
				m_dictionary.SetProperty( DictionaryProperties.Border, m_border );
			}

			RectangleF nativeRectangle = new RectangleF( m_rectangle.X, m_rectangle.Bottom,
				m_rectangle.Width, m_rectangle.Height );

			if( m_page != null )
			{
				PdfSection section = m_page.Section;
				nativeRectangle.Location = section.PointToNativePdf( Page, nativeRectangle.Location );
			}

			m_dictionary.SetProperty( DictionaryProperties.Rect, PdfArray.FromRectangle( nativeRectangle ) );
		}
		#endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion
	}
}
