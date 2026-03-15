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
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Outline Wrapper for Outline. Redirects the properties to the Outline 
	/// </summary>
	public class OutlineWrapper
    : CommonWrapper
    , IOutline
    , IOutlineWrapper
	{
    #region Class members
        /// <summary>
        /// Represents the outline first index
        /// </summary>
        private int m_firstIndex;
        /// <summary>
        /// Represents the outline last index
        /// </summary>        
        private int m_lastIndex;
        /// <summary>
        /// Represents the IOutline object
        /// <summary>        
        private IOutline m_outline;
        /// <summary>
        /// Represents the Outline range
        /// </summary>
        private IRange m_outlineRange;
        /// <summary>
        /// Represents the Group by enumeration
        /// </summary>
        private ExcelGroupBy m_groupBy;
        /// <summary>
        /// Represents the Outline Level
        /// </summary>
        private ushort m_outlineLevel;
        /// <summary>
        /// Represents Is Collapsed property of IOutline
        /// </summary>
        private bool m_bIsCollapsed;
        /// <summary>
        /// Represents Is Hidden property of IOutline
        /// </summary>
        private bool m_bIsHidden;
        /// <summary>
        /// Represents the Extended format index of outline object
        /// </summary>
        private ushort m_formatIndex;
        /// <summary>
        /// Represents the index value
        /// </summary>
        private ushort m_index;        
    #endregion
    
    #region Class Properties
    /// <summary>
    /// Returns or sets the First Row/Column index for the Outline
    /// </summary>
    public int FirstIndex
    {
        get
        {
            return m_firstIndex;
        }
        set
        {
            m_firstIndex = value;
        }
    }
    /// <summary>
    /// Returns or sets the Last Row/Column index for the Outline
    /// </summary>
    public int LastIndex
    {
        get
        {
            return m_lastIndex;
        }
        set
        {
            m_lastIndex = value;
        }
    }
    /// <summary>
    /// Returns or sets the IOutline object 
    /// </summary>
    public IOutline Outline
    {
        get
        {
            return m_outline;
        }
        set
        {
            m_outline = value;
        }
    }

    /// <summary>
    /// Returns or sets the index value
    /// </summary>
    public ushort Index
    {
        get
        {
            return m_index;
        }
        set
        {
            m_index = value;
        }
    }
    /// <summary>
    /// Returns or sets the IRange value for a specified outline
    /// </summary>
    public IRange OutlineRange
    {
        get
        {
            return m_outlineRange;
        }
        set
        {
            m_outlineRange = value;
        }
    }
    /// <summary>
    /// Returns or sets the Excel Group by Enum value
    /// </summary>
    public ExcelGroupBy GroupBy
    {
        get
        {
            return m_groupBy;
        }
        set
        {
            m_groupBy = value;
        }
    }

    /// <summary>
    /// Returns or sets the Extended format index value for an outline range
    /// </summary>
    public ushort ExtendedFormatIndex
    {
        get
        {
            return m_formatIndex;
        }
        set
        {
            m_formatIndex = value;
        }
    }
    
    /// <summary>
    /// Returns or sets the IsHidden value for an outline
    /// </summary>
    public bool IsHidden
    {
        get
        {
            return m_bIsHidden;
        }
        set
        {
            m_bIsHidden = value;
        }
    }

    /// <summary>
    /// Returns or sets the IsCollapsed value for an outline
    /// </summary>  
    public bool IsCollapsed
    {
        get
        {
            return m_bIsCollapsed;
        }
        set
        {
            m_bIsCollapsed = value;
        }
    }

    /// <summary>
    /// Returns or sets the outline level value for an outline
    /// </summary>
    public ushort OutlineLevel
    {
        get
        {
            return m_outlineLevel;
        }
        set
        {
            m_outlineLevel = value;
        }
    }
    #endregion       
  }
}
