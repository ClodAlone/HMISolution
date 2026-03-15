#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Drawing;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Interactive;

namespace Syncfusion.Pdf
{
	/// <summary>
	/// Represents a single PDF page.
	/// </summary>
	public class PdfPage : PdfPageBase
	{
		#region Fields
		private PdfSection m_section;

		private PdfAnnotationCollection m_annotations = null;

		private bool m_isProgressOn;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the parent section of the page.
		/// </summary>
		public PdfSection Section
		{
			get
			{
				if( m_section == null )
					throw new PdfException( "Page must be added to some section before using." );

				return m_section;
			}
            internal set
            {
                m_section = value;
            }
		}

		/// <summary>
		/// Gets the size of the page.
		/// </summary>
		public override SizeF Size
		{
			get
			{
				return Section.PageSettings.Size;
			}
		}

        /// <summary>
        /// Gets the origin of the page
        /// </summary>
        internal override PointF Origin
        {
            get
            {
                return Section.PageSettings.Origin;
            }
        }
		/// <summary>
		/// Gets a collection of the annotations of the page.
		/// </summary>
		public PdfAnnotationCollection Annotations
		{
			get
			{
				if( m_annotations == null )
				{
					m_annotations = new PdfAnnotationCollection( this );
					Dictionary[ DictionaryProperties.Annots ] = ( ( IPdfWrapper )m_annotations ).Element;
				}

				return m_annotations;
			}
		}

		/// <summary>
		/// Gets current document.
		/// </summary>
		/// <value>The pdf document.</value>
		internal PdfDocument Document
		{
			get
			{
				return m_section == null ? null : m_section.Parent.Document;
			}
		}

        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_section.Parent == null ? m_section.ParentDocument.CrossTable : m_section.Parent.Document.CrossTable;
            }
        }
		#endregion

		#region Events
		/// <summary>
		/// Raises before the page saves.
		/// </summary>
		public event EventHandler BeginSave;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:PdfPage"/> class.
		/// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Add a page
        /// PdfPage page = doc.Pages.Add();
        /// </code>
        /// </example>
		public PdfPage()
			: base( new PdfDictionary() )
		{
			Initialize();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Returns a page size reduced by page margins and page template dimensions.
		/// </summary>
		/// <remarks>It's the actual size of the page where some output can be performed.</remarks>
		/// <returns>Returns a page size reduced by page margins and page template dimensions.</returns>
		public SizeF GetClientSize()
		{
			return Section.GetActualBounds( this, true ).Size;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Raises <see cref="BeginSave"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnBeginSave( EventArgs e )
		{
			if( BeginSave != null )
			{
				BeginSave( this, e );
			}
		}

        /// <summary>
        /// Clears PdfPage.
        /// </summary>
        internal override void Clear()
        {
            base.Clear();

            if (m_annotations != null)
                m_annotations.Clear();

            m_section = null;
        }

		/// <summary>
		/// Sets parent section to the page.
		/// </summary>
		/// <param name="section">The parent section.</param>
		internal void SetSection( PdfSection section )
		{
			if( m_section != null )
				throw new PdfException( "The page already exists in some section, it can't be contained by several sections" );

			m_section = section;
			Dictionary[ DictionaryProperties.Parent ] = new PdfReferenceHolder( section );
		}

		/// <summary>
		/// Sets the progress.
		/// </summary>
		internal void SetProgress()
		{
			m_isProgressOn = true;
		}

		/// <summary>
		/// Resets the progress.
		/// </summary>
		internal void ResetProgress()
		{
			m_isProgressOn = false;
		}

		/// <summary>
		/// Initializes a page.
		/// </summary>
		private void Initialize()
		{
			Dictionary[ DictionaryProperties.Type ] = new PdfName( "Page" );

			Dictionary.BeginSave += new SavePdfPrimitiveEventHandler( PageBeginSave );
			Dictionary.EndSave += new SavePdfPrimitiveEventHandler( PageEndSave );
		}

		/// <summary>
		/// Draws page templates.
		/// </summary>
		/// <param name="document">Parent document.</param>
		private void DrawPageTemplates( PdfDocument document )
		{
			if( document == null )
				//throw new ArgumentNullException( "document" );
				return;

			// Draw Background templates.
			bool hasBackTemplates = Section.ContainsTemplates( document, this, false );

			if( hasBackTemplates )
			{
				PdfPageLayer backLayer = new PdfPageLayer( this, false );

				Layers.Insert( 0, backLayer );
				Section.DrawTemplates( this, backLayer, document, false );
			}

			// Draw Foreground templates.
			bool hasFrontTemplates = Section.ContainsTemplates( document, this, true );

			if( hasFrontTemplates )
			{
				PdfPageLayer frontLayer = new PdfPageLayer( this, false );
				Layers.Add( frontLayer );
				Section.DrawTemplates( this, frontLayer, document, true );
			}
		}

		/// <summary>
		/// Removes template layers from the page layers.
		/// </summary>
		/// <param name="document">Parent document.</param>
		private void RemoveTemplateLayers( PdfDocument document )
		{
			if( document == null )
				throw new ArgumentNullException( "document" );

			bool hasBackTemplates = Section.ContainsTemplates( document, this, false );
			bool hasFrontTemplates = Section.ContainsTemplates( document, this, true );

			if( hasBackTemplates )
			{
				Layers.RemoveAt( 0 );
			}

			if( hasFrontTemplates )
			{
				Layers.RemoveAt( Layers.Count - 1 );
			}
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// Raises when page dictionary is going to be saved.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="args">Event arguments.</param>
		private void PageBeginSave( object sender, SavePdfPrimitiveEventArgs args )
		{
			PdfDocument doc = args.Writer.Document as PdfDocument;

			if( doc != null )
			{

				DrawPageTemplates( doc );

				if( m_isProgressOn )
				{
					Section.OnPageSaving( this );
				}

                if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_X1A2001)
                {
                    Dictionary[DictionaryProperties.MediaBox] = PdfArray.FromRectangle(new RectangleF(PointF.Empty, this.Size));
                    Dictionary[DictionaryProperties.TrimBox] = PdfArray.FromRectangle(new RectangleF(PointF.Empty, this.Size));
                }
                
				PdfPageTransition transition = Section.GetTransitionSettings();
				if( transition != null )
				{
					Dictionary.SetProperty( DictionaryProperties.PageDuration,
						new PdfNumber( transition.PageDuration ) );

					Dictionary.SetProperty( DictionaryProperties.Transition,
						( transition as IPdfWrapper ).Element );
                }

# if !SILVERLIGHT && !NETFX_CORE && !WP
                // Updates page dictionary for tagged PDF.
                if (doc.FileStructure.TaggedPdf)
                {
                    PdfStructTreeRoot structTreeRoot = PdfCrossTable.Dereference(doc.Catalog[DictionaryProperties.StructTreeRoot]) as PdfStructTreeRoot;
                    if (structTreeRoot != null)
                        Dictionary[DictionaryProperties.StructParents] = new PdfNumber(0);
                }                    
# endif
            }

			OnBeginSave( new EventArgs() );
		}

		/// <summary>
		/// Raises after the page dictionary was saved.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="args">The <see cref="T:Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> 
		/// instance containing the event data.</param>
		private void PageEndSave( object sender, SavePdfPrimitiveEventArgs args )
		{
			// Remove layers.
			PdfDocument doc = args.Writer.Document as PdfDocument;

			if( doc != null )
			{
				RemoveTemplateLayers( doc );
			}
		}
		#endregion
	}
}
