#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The PrintZoom class encapsulates the zoom settings used by the diagram for print and print preview.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.PrintZoom"/>
    /// </summary>
    [Serializable()]
    public class PrintZoom : ICloneable, ISerializable
    {
        #region Fields
        /// <summary>
        ///  Print zooming or fit to page.
        /// </summary>
        private bool m_bUsePrintZoom = true;

        /// <summary>
        /// Number of sheets to fit across.
        /// </summary>
        private int m_nSheetsAcross = 1;

        /// <summary>
        /// Number of sheets to fit down.
        /// </summary>
        private int m_nSheetsDown = 1;

        /// <summary>
        /// Print zooming.
        /// </summary>
        private int m_nPrintZooming = 100;
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether zoom should be enabled for print and printpreview. Default is True.
        /// </summary>
        [
        DefaultValue(true),
        Description("Specifies whether zoom should be enabled for print and printpreview.")
        ]
        public bool UsePrintingZoom
        {
            get
            {
                return m_bUsePrintZoom;
            }
            set
            {
                m_bUsePrintZoom = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the zoom ratio. Values range from 0 to 100.
        /// </summary>
        [
        DefaultValue(100),
        Description("Specifies the zoom ratio.")
        ]
        public int PrintingZoom
        {
            get
            {
                return m_nPrintZooming;
            }
            set
            {
                if (m_nPrintZooming != value)
                    m_nPrintZooming = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of horizontal pages to fit the print data into.
        /// </summary>
        [DefaultValue(1),
        Description("The number of horizontal pages to fit the print data into.")
        ]
        public int SheetsAcross
        {
            get
            {
                return m_nSheetsAcross;
            }
            set
            {
                if (m_nSheetsAcross != value)
                    m_nSheetsAcross = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of vertical pages to fit the print data into.
        /// </summary>
        [DefaultValue(1),
        Description("The number of vertical pages to fit the print data into.")
        ]
        public int SheetsDown
        {
            get
            {
                return m_nSheetsDown;
            }
            set
            {
                if (m_nSheetsDown != value)
                    m_nSheetsDown = value;
            }
        }
        #endregion Properties
        public PrintZoom()
        {
        }
        public PrintZoom(PrintZoom src)
        {
            m_bUsePrintZoom = src.m_bUsePrintZoom;
            m_nPrintZooming = src.m_nPrintZooming;
            m_nSheetsAcross = src.m_nSheetsAcross;
            m_nSheetsDown = src.m_nSheetsDown;
        }
        protected PrintZoom(SerializationInfo info, StreamingContext context)
        {
            m_bUsePrintZoom = info.GetBoolean("usePrintZoom");
            m_nPrintZooming = info.GetInt32("printingZoom");
            m_nSheetsAcross = info.GetInt32("sheetsAcross");
            m_nSheetsDown = info.GetInt32("sheetsDown");

        }      

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("usePrintZoom", m_bUsePrintZoom);
            info.AddValue("printingZoom", m_nPrintZooming);
            info.AddValue("sheetsAcross", m_nSheetsAcross);
            info.AddValue("sheetsDown", m_nSheetsDown);
        }

        #endregion
    }
}