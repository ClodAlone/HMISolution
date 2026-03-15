//-------------------------------------------------------------------------------------------------
// <copyright file="GridAppState.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Lets you specify a custom <see cref="IGridCellModelFactory"/> that instantiates
    /// cell models for the grid on demand using the <see cref="GridStyleInfo.CellType"/>
    /// string as identifier.
    /// </summary>
    public class GridFactoryProvider
    {
        /// <summary>
        /// Static Constructor.
        /// </summary>
        static GridFactoryProvider()
        {
        }

        /// <summary>
        /// Lets you specify a custom <see cref="IGridCellModelFactory"/> that instantiates
        /// cell models for the grid on demand using the <see cref="GridStyleInfo.CellType"/>
        /// string as identifier.
        /// </summary>
        /// <param name="pFactory">A cell model factory.</param>
        public static void Init(IGridCellModelFactory pFactory)
        {
            cellModelFactory = pFactory;
        }

        /// <summary>
        /// Gets the <see cref="IGridCellModelFactory"/> for this process.
        /// </summary>
        public static IGridCellModelFactory CellModelFactory
        {
            get
            {
                return cellModelFactory;
            }
        }

        // Control Factory
        static IGridCellModelFactory cellModelFactory;
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridFontState
    {
        const int _LOGPIXELSX = 88; // 0x0058 
        const int _LOGPIXELSY = 90; // 0x005a 

        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridFontState()
            : base()
        {
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        private static void CheckLogPixels()
        {
            IntPtr hDC = NativeMethods.GetDC(NativeMethods.NullIntPtr);
            if (hDC != NativeMethods.NullIntPtr)
            {
                m_nLogPixelsX = NativeMethods.GetDeviceCaps(hDC, _LOGPIXELSX);
                m_nLogPixelsY = NativeMethods.GetDeviceCaps(hDC, _LOGPIXELSY);
                NativeMethods.ReleaseDC(NativeMethods.NullIntPtr, hDC);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static void Reset()
        {
            m_nLogPixelsX = 0;
            m_nLogPixelsY = 0;
        }

        /// <internalonly/>
        /// <summary>Gets the LogPixelsX. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int LogPixelsX
        {
            get
            {
                if (m_nLogPixelsX == 0)
                {
                    CheckLogPixels();
                }

                return m_nLogPixelsX;
            }
        }

        /// <internalonly/>
        /// <summary>Gets the LogPixelsY. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int LogPixelsY
        {
            get
            {
                if (m_nLogPixelsY == 0)
                {
                    CheckLogPixels();
                }

                return m_nLogPixelsY;
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>       
        /// <returns>return int</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int INCHtoDP(float f)
        {
            return (int)(f * ((float)LogPixelsX));
        }

        /// <summary>
        /// Used internally.
        /// </summary>        
        /// <returns>returns int</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int CMtoDP(float f)
        {
            return (int)(f * ((float)LogPixelsX) / 2.54f);
        }

        /// <summary>
        /// Used internally.
        /// </summary>        
        /// <returns>returns float</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static float DPtoINCH(int f)
        {
            return (float)((float)f / ((float)LogPixelsX));
        }

        /// <summary>
        /// Used internally.
        /// </summary>       
        /// <returns>returns float</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static float DPtoCM(int f)
        {
            return (float)f * 2.54f / ((float)LogPixelsX);
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int m_nLogPixelsY;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int m_nLogPixelsX;
    }
}
