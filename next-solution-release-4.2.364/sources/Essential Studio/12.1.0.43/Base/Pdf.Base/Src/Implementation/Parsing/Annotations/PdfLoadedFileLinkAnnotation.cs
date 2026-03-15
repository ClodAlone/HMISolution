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
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the loaded file link annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
    /// //Gets the annotation flags
    /// PdfAnnotationFlags flag = attchmentAnnotation.AnnotationFlags;
    /// //Sets the file name.
    /// fileLinkAnnotation.FileName = @"..\..\Data\Manual.txt";
    /// //Gets the file link annotation border.
    /// PdfAnnotationBorder border = fileLinkAnnotation.Border;
    /// //Gets the file link annotation bounds.
    /// RectangleF rectangle = fileLinkAnnotation.Bounds;
    /// //Gets the file link annotation bounds.
    /// PdfColor color = fileLinkAnnotation.Color;
    /// //Gets the file link annotation location.
    /// PointF point = fileLinkAnnotation.Location;
    /// //Gets the file link annotation size.
    /// SizeF size = fileLinkAnnotation.Size;
    /// //Gets the file link annotation text.
    /// string text = fileLinkAnnotation.Text;
    /// //Save the document.
    /// document.Save("fileLinkAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    ///'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
    /// 'Sets the file name.
    /// fileLinkAnnotation.FileName = "..\..\Data\Manual.txt"
    /// 'Gets the annotation flags
    /// Dim flag As PdfAnnotationFlags = fileLinkAnnotation.AnnotationFlags
    /// 'Gets the file link annotation border.
    /// Dim border As PdfAnnotationBorder = fileLinkAnnotation.Border
    /// 'Gets the file link annotation bounds.
    /// Dim rect As RectangleF = fileLinkAnnotation.Bounds
    /// 'Gets the file link annotation bounds.
    /// Dim color As PdfColor = fileLinkAnnotation.Color
    /// 'Gets the file link annotation location.
    /// Dim point As PointF = fileLinkAnnotation.Location
    /// 'Gets the file link annotation size.
    /// Dim size As SizeF = fileLinkAnnotation.Size
    /// 'Gets the file link annotation text.
    /// Dim text As string = fileLinkAnnotation.Text
    /// 'Save the document.
    /// document.Save("fileLinkAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedAttachmentAnnotation"/> Class
    /// <seealso cref="PdfLoadedDocumentLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedLineAnnotation"/> Class
    /// <seealso cref="PdfLoadedPopupAnnotation"/> Class
    /// <seealso cref="PdfLoadedRubberStampAnnotation"/> Class
    /// <seealso cref="PdfLoadedSoundAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedFileLinkAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// Interger array of destination
        /// </summary>
        private int[] destinationArray;
        /// <summary>
        /// CrossTable
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Action of the page
        /// </summary>
        private PdfLaunchAction m_action;
        /// <summary>
        /// Destination array
        /// </summary>
        private PdfArray m_destination;
        #endregion

        #region Properties
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Gets or sets the filename of the annotation.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Gets or sets the filename of the annotation.
        /// </summary>
#endif
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation fileLinkAnnotation = document.Pages[1].Annotations[6] as PdfLoadedFileLinkAnnotation;
        /// //Gets the annotation flags
        /// PdfAnnotationFlags flag = attchmentAnnotation.AnnotationFlags;
        /// //Sets the file name.
        /// fileLinkAnnotation.FileName = @"..\..\Data\Manual.txt";
        /// //Gets the file link annotation border.
        /// //Save the document.
        /// document.Save("fileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim fileLinkAnnotation As PdfLoadedFileLinkAnnotation = document.Pages(1).Annotations(6) as PdfLoadedFileLinkAnnotation
        /// 'Sets the file name.
        /// fileLinkAnnotation.FileName = "..\..\Data\Manual.txt"
        /// 'Save the document.
        /// document.Save("fileLinkAnnotation.pdf")
        /// </code>
        /// </example>
        public string FileName
        {
            get
            {
                return GetFileName();
            }
            set
            {
                PdfDictionary m_dic = Dictionary;
                if (Dictionary.ContainsKey(DictionaryProperties.A))
                {
                    m_dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.A]) as PdfDictionary;
                    PdfDictionary s_dic = m_crossTable.GetObject(m_dic[DictionaryProperties.F]) as PdfDictionary;
                    s_dic.SetString(DictionaryProperties.F, value);
                    if (s_dic.ContainsKey(DictionaryProperties.UF))
                        s_dic.SetString(DictionaryProperties.UF, value);
                    Dictionary.Modify();
                }
            }
        }
        /// <summary>
        /// Gets or sets the destination of the filelinkannotation.
        /// </summary>
        private PdfArray Destination
        {
            get
            {
                return m_destination;
            }
            set
            {
                m_destination = value;
            }
        }
        /// <summary>
        /// Gets or sets the destination array of the annotation
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedFileLinkAnnotation linkAnnotation = lDoc.Pages[1].Annotations[2] as PdfLoadedFileLinkAnnotation;
        /// //Assgin DestinationArray[pagenumber,xaxis,yaxis]
        /// int[] array = new int[3] { 2, 750, 0 };
        /// linkAnnotation.DestinationArray = array;
        /// //Save the document.
        /// document.Save("lineAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        ///   'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim linkAnnotation As PdfLoadedFileLinkAnnotation = lDoc.Pages(1).Annotations(2) as PdfLoadedFileLinkAnnotation        
        /// 'Assgin DestinationArray[pagenumber,xaxis,yaxis]
        /// Dim array As Integer() = New Integer(2) {2, 750, 0}
        /// linkAnnotation.DestinationArray = array
        /// 'Save the document.
        /// document.Save("lineAnnotation.pdf")
        /// </code>
        /// </example>
        public int[] DestinationArray
        {
            get
            {
                return GetDestination();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("DestinationPageNumber");
                }

                if (value != destinationArray)
                {
                    destinationArray = value;
                    Destination.Clear();
                    Destination.Add(new PdfNumber(value[0] - 1));
                    Destination.Add(new PdfName(DictionaryProperties.XYZ));
                    Destination.Add(new PdfNull());
                    Destination.Add(new PdfNumber(value[1]));
                    Destination.Add(new PdfNumber(value[2]));
                    PdfDictionary m_dic = Dictionary;
                    if (Dictionary.ContainsKey(DictionaryProperties.A))
                    {
                        m_dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.A]) as PdfDictionary;
                        m_dic.Remove(DictionaryProperties.D);
                        m_dic.SetProperty(DictionaryProperties.D, Destination);
                        Dictionary.Modify();
                    }
                }

            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <returns>File name</returns>
        private string GetFileName()
        {
            string filename = String.Empty;
            if (Dictionary.ContainsKey(DictionaryProperties.A))
            {
                PdfDictionary Dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.A]) as PdfDictionary;
                PdfDictionary mDic = m_crossTable.GetObject(Dic[DictionaryProperties.F]) as PdfDictionary;
                PdfString text = mDic[DictionaryProperties.F] as PdfString;
                filename = text.Value.ToString();
            }
            return filename;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedLineAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="filename">The File name</param>
        internal PdfLoadedFileLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle, string filename)
            : base(dictionary, crossTable)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            Dictionary = dictionary;
            m_crossTable = crossTable;
            m_action = new PdfLaunchAction(filename,true);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedLineAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param> 
        /// <param name="destination">The destination</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="filename">The File name</param>
        internal PdfLoadedFileLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, PdfArray destination, RectangleF rectangle, string filename)
            : base(dictionary, crossTable)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");
            Dictionary = dictionary;
            m_crossTable = crossTable;
            Destination = destination;
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Gets the destination array
        /// </summary>
        /// <returns>integer destination array</returns>
        private int[] GetDestination()
        {
            int[] intDestinationArray = null;
            if (Dictionary.ContainsKey(DictionaryProperties.A))
            {
                PdfDictionary Dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.A]) as PdfDictionary;
                PdfArray destinationArray = PdfCrossTable.Dereference(Dic[DictionaryProperties.D]) as PdfArray;
                int i = 0;
                intDestinationArray = new int[destinationArray.Count - 1];

                foreach (object value in destinationArray)
                {
                    if (value is PdfNumber)
                    {
                        if (i == 0)
                        {
                            intDestinationArray[i] = (value as PdfNumber).IntValue + 1;
                            i++;
                        }

                        else
                        {
                            intDestinationArray[i] = (value as PdfNumber).IntValue;
                            i++;
                        }
                    }
                    else if (value is PdfNull)
                    {
                        intDestinationArray[i] = 0;
                        i++;
                    }
                }
            }
            return intDestinationArray;
        }
        #endregion
    }
}
