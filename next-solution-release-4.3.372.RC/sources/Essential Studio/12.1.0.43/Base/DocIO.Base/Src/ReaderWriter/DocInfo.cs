#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

/// <summary>
/// DocInfo contains stream component data blocks.
/// </summary>
[CLSCompliant(false)]
internal class DocInfo
{
    #region Fields
    private WPFIBData m_fibData;
    private WPTablesData m_tablesData;
    private WordFKPData m_fkpData;
    private WordImageWriter m_imageWriter;
    ////  private WordImageReader m_imageReader;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="DocInfo"/> class.
    /// </summary>
    /// <param name="streamsManager">The streams manager.</param>
    internal DocInfo(StreamsManager streamsManager)
    {
        m_fibData = new WPFIBData();
        m_tablesData = new WPTablesData(m_fibData);
        m_fkpData = new WordFKPData(m_fibData, m_tablesData);
        m_imageWriter = new WordImageWriter(streamsManager.DataStream);
        ////    m_imageReader = new WordImageReader( streamsManager.DataStream, 0, StreamsManager.Converter );
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the fib data.
    /// </summary>
    /// <value>The fib data.</value>
    internal WPFIBData FibData
    {
        get
        {
            return m_fibData;
        }
    }

    /// <summary>
    /// Gets the tables data.
    /// </summary>
    /// <value>The tables data.</value>
    internal WPTablesData TablesData
    {
        get
        {
            return m_tablesData;
        }
    }

    /// <summary>
    /// Gets the FKP data.
    /// </summary>
    /// <value>The FKP data.</value>
    internal WordFKPData FkpData
    {
        get
        {
            return m_fkpData;
        }
    }

    /// <summary>
    /// Gets the image writer.
    /// </summary>
    /// <value>The image writer.</value>
    internal WordImageWriter ImageWriter
    {
        get
        {
            //if( m_imageWriter == null )
            //{
            //  m_imageWriter = new WordImageWriter( m_streamsManager.DataStream, m_memConverter );
            //}
            return m_imageWriter;
        }
    }
    #endregion



    #region internal methods
    /// <summary>
    /// Returns WordImageReader depending on the offset
    /// </summary>
    /// <param name="streamsManager"></param>
    /// <param name="offset"> offset in Data stream of the current image </param>
    /// <returns></returns>
    internal WordImageReader GetImageReader(StreamsManager streamsManager, int offset, WordDocument doc)
    {
        return new WordImageReader(streamsManager.DataStream, offset, doc);
    }

    /// <summary>
    /// 
    /// </summary>
    internal void Close()
    {
        m_tablesData.Close();
        m_tablesData = null;
        m_fibData = null;
        m_fkpData = null;
        m_imageWriter = null;
    }
    #endregion
}