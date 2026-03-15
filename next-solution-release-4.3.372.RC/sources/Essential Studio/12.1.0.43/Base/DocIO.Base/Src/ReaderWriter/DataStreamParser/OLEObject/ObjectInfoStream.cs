#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.CompoundFile;
using Syncfusion.CompoundFile.DocIO.Native;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.DocIO.DLS;

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject
{
    internal class ObjectInfoStream : DataStructure
    {
        #region Constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_STRUCT_SIZE = 6;
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_dataBytes;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the size of the structure.
        /// </summary>
        /// <value>The length.</value>
        internal override int Length
        {
            get
            {
                return DEF_STRUCT_SIZE;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectInfoStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal ObjectInfoStream(Stream stream )
        {
            Parse((stream as MemoryStream).ToArray(), 0);
        }
        /// <summary>
        /// Initializes a default instance of the <see cref="ObjectInfoStream"/> class.
        /// </summary>
        internal ObjectInfoStream()
        {
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse( byte[] arrData, int iOffset )
        {
            m_dataBytes = arrData;
        }
        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns>Length</returns>
        internal override int Save( byte[] arrData, int iOffset )
        {
            throw new NotImplementedException( "Not implemented" );
        }
        /// <summary>
        /// Saves the data to stream.
        /// </summary>
        /// <param name="stgStream">The STG stream.</param>
        internal void SaveTo(Stream stream, OleLinkType linkType, OleObjectType oleType)
        {
            switch (oleType)
            {
                case OleObjectType.WordDocument:
                case OleObjectType.PowerPoint_97_2003_Slide:
                    if (linkType == OleLinkType.Embed)
                        m_dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 0, 3, 0, 1, 0 };
                    else
                        m_dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 0, 3, 0, 13, 0 };
                    break;
                case OleObjectType.Equation:
                    if (linkType == OleLinkType.Embed)
                        m_dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 0, 3, 0, 4, 0 };
                    break;
                case OleObjectType.GraphChart:
                case OleObjectType.ExcelChart:
                    if (linkType == OleLinkType.Embed)
                        m_dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 2, 3, 0, 13, 0 };
                    else
                        m_dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 0, 3, 0, 13, 0 };
                    break;
                case OleObjectType.AdobeAcrobatDocument:                
                case OleObjectType.WordMacroDocument:
                case OleObjectType.Excel_97_2003_Worksheet:
                case OleObjectType.ExcelBinaryWorksheet:
                case OleObjectType.ExcelWorksheet:
                case OleObjectType.ExcelMacroWorksheet:                
                case OleObjectType.PowerPoint_97_2003_Presentation:
                case OleObjectType.PowerPointPresentation:
                case OleObjectType.PowerPointMacroPresentation:
                case OleObjectType.PowerPointMacroSlide:
                case OleObjectType.PowerPointSlide:
                case OleObjectType.VisioDrawing:
                case OleObjectType.OpenDocumentPresentation:
                case OleObjectType.OpenDocumentSpreadsheet:
                case OleObjectType.OpenOfficeSpreadsheet:
                case OleObjectType.OpenOfficeText:
                case OleObjectType.OpenOfficeSpreadsheet1_1:
                case OleObjectType.OpenOfficeText_1_1:
                    {
                        if (linkType == OleLinkType.Embed)
                            m_dataBytes = new byte[DEF_STRUCT_SIZE] { 64, 0, 3, 0, 4, 0 };
                        else
                            m_dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 0, 3, 0, 13, 0 };
                    }
                    break;
                case OleObjectType.BitmapImage:
                case OleObjectType.VideoClip:
                case OleObjectType.MIDISequence:
                    {
                        if (linkType == OleLinkType.Embed)
                            m_dataBytes = new byte[DEF_STRUCT_SIZE]{ 0, 0, 3, 0, 4, 0 };
                        else
                            m_dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 0, 3, 0, 4, 0 };
                    }
                    break;
                case OleObjectType.WaveSound:
                case OleObjectType.MediaClip:
                case OleObjectType.Package:
                    m_dataBytes = new byte[DEF_STRUCT_SIZE] { 64, 0, 3, 0, 4, 0 };
                    break;
                case OleObjectType.WordPadDocument:
                case OleObjectType.Undefined:
                    {
                        if (linkType == OleLinkType.Embed)
                            m_dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 0, 3, 0, 4, 0 };
                        else
                            m_dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 2, 3, 0, 13, 0 };
                    }
                    break;
                case OleObjectType.Word_97_2003_Document:
                    {
                        if (linkType == OleLinkType.Embed)
                            m_dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 2, 3, 0, 1, 0 };
                        else
                            m_dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 2, 3, 0, 13, 0 };
                    }
                    break;
            }

            stream.Write(m_dataBytes, 0, m_dataBytes.Length);
        }
        #endregion
    }
}
