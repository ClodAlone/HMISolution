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

#region File using directives

using System;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for OverrideLevelFormat.
    /// </summary>
    public class OverrideLevelFormat : XDLSSerializableBase
    {
        #region Fileds
        /// <summary>
        /// 
        /// </summary>
        private int m_startAt;
        private bool m_bStartAt;
        private bool m_bFormatting;
        private WListLevel m_lfoLevel;

        internal int m_reserved1;
        internal int m_reserved2;
        internal int m_reserved3;
        #endregion

        #region Class constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideLevelFormat"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal OverrideLevelFormat(WordDocument doc)
            : base(doc, null)
        {
            m_lfoLevel = new WListLevel(Document);
            m_lfoLevel.SetOwner(this);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether [override start at value].
        /// </summary>
        /// <value>
        /// 	if it override start at value, set to <c>true</c>.
        /// </value>
        internal bool OverrideStartAtValue
        {
            get
            {
                return m_bStartAt;
            }
            set
            {
                m_bStartAt = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [override formatting].
        /// </summary>
        /// <value>if it override formatting, set to <c>true</c>.</value>
        internal bool OverrideFormatting
        {
            get
            {
                return m_bFormatting;
            }
            set
            {
                m_bFormatting = value;
            }
        }
        /// <summary>
        /// Gets or sets the start at.
        /// </summary>
        /// <value>The start at.</value>
        internal int StartAt
        {
            get
            {
                return m_startAt;
            }
            set
            {
                m_startAt = value;
            }
        }
        /// <summary>
        /// Gets the override list level.
        /// </summary>
        /// <value>The override list level.</value>
        internal WListLevel OverrideListLevel
        {
            get
            {
                return m_lfoLevel;
            }
            set
            {
                m_lfoLevel = value;
            }
        }
        #endregion

        #region Class overides
//#if !SILVERLIGHT
        /// <summary>
        /// Serialize paragraph and character properties.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();
            XDLSHolder.AddElement(XDLSConstants.LevelOverrideTag, m_lfoLevel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            if (m_bStartAt)
            {
                writer.WriteValue(XDLSConstants.LevelOverrideStartAttr, m_bStartAt);
                writer.WriteValue(XDLSConstants.LevelOverrideStartAtAttr, m_startAt);
            }
            if (m_bFormatting)
            {
                writer.WriteValue(XDLSConstants.LevelOverrideFormatAttr, m_bFormatting);
            }
            if (m_reserved1 != 0)
            {
                writer.WriteValue(XDLSConstants.LevelOverrideReserved1Attr, m_reserved1);
            }
            if (m_reserved2 != 0)
            {
                writer.WriteValue(XDLSConstants.LevelOverrideReserved2Attr, m_reserved2);
            }
            if (m_reserved3 != 0)
            {
                writer.WriteValue(XDLSConstants.LevelOverrideReserved3Attr, m_reserved3);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.LevelOverrideFormatAttr))
            {
                m_bFormatting = reader.ReadBoolean(XDLSConstants.LevelOverrideFormatAttr);
            }
            if (reader.HasAttribute(XDLSConstants.LevelOverrideStartAttr))
            {
                m_bStartAt = reader.ReadBoolean(XDLSConstants.LevelOverrideStartAttr);
            }
            if (reader.HasAttribute(XDLSConstants.LevelOverrideStartAtAttr))
            {
                m_startAt = reader.ReadInt(XDLSConstants.LevelOverrideStartAtAttr);
            }
            if (reader.HasAttribute(XDLSConstants.LevelOverrideReserved1Attr))
            {
                m_reserved1 = reader.ReadInt(XDLSConstants.LevelOverrideReserved1Attr);
            }
            if (reader.HasAttribute(XDLSConstants.LevelOverrideReserved2Attr))
            {
                m_reserved2 = reader.ReadInt(XDLSConstants.LevelOverrideReserved2Attr);
            }
            if (reader.HasAttribute(XDLSConstants.LevelOverrideReserved3Attr))
            {
                m_reserved3 = reader.ReadInt(XDLSConstants.LevelOverrideReserved3Attr);
            }
        }
//#endif
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            OverrideLevelFormat overrideLevelFormat = (OverrideLevelFormat)base.CloneImpl();
            overrideLevelFormat.OverrideListLevel = OverrideListLevel.Clone();
            //Set the owner for the override level format.
            overrideLevelFormat.OverrideListLevel.SetOwner(overrideLevelFormat);
            return overrideLevelFormat;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (m_lfoLevel != null)
            {
                m_lfoLevel.Close();
                m_lfoLevel = null;
            }
        }
        #endregion
    }
}
