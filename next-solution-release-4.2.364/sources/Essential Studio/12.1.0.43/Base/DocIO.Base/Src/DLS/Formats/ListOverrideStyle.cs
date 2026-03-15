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
using System.Collections.Generic;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the overrides for list style.
    /// </summary>
    internal class ListOverrideStyle : Style
    {
        #region Fields
        private ListOverrideLevelCollection m_overrideLevels;
        internal int m_res1;
        internal int m_res2;

        internal int m_unused1;
        internal int m_unused2;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the style.
        /// </summary>
        /// <value>The type of the style.</value>
        public override StyleType StyleType
        {
            get
            {
                return StyleType.OtherStyle;
            }
        }
        /// <summary>
        /// Gets the override levels.
        /// </summary>
        /// <value>The override levels.</value>
        internal ListOverrideLevelCollection OverrideLevels
        {
            get
            {
                return m_overrideLevels;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListOverrideStyle"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal ListOverrideStyle(WordDocument doc)
            : base(doc)
        {
            m_overrideLevels = new ListOverrideLevelCollection(doc);
            m_overrideLevels.SetOwner(this);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Clones itself
        /// </summary>
        /// <returns></returns>
        public override IStyle Clone()
        {
            return (IStyle)CloneImpl();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            ListOverrideStyle listOverrideStyle = (ListOverrideStyle)base.CloneImpl();

            listOverrideStyle.m_overrideLevels = new ListOverrideLevelCollection(Document);
            //Set the owner for the override level collection.
            listOverrideStyle.m_overrideLevels.SetOwner(listOverrideStyle);
            m_overrideLevels.CloneToImpl(listOverrideStyle.m_overrideLevels);

            return listOverrideStyle;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="nextOwner"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            if (doc == Document)
                return;
            //Set the owner for the list override style.
            SetOwner(doc);
            //Set the owner for the override level collection.
            OverrideLevels.SetOwner(this);
           foreach(OverrideLevelFormat overrideLevelFormat in OverrideLevels)
            {
                //Set the owner for the override level format.
                overrideLevelFormat.SetOwner(this);
                //Set the owner for the override list level.
                overrideLevelFormat.OverrideListLevel.SetOwner(overrideLevelFormat);
                //Set the owner for the list level character format.
                overrideLevelFormat.OverrideListLevel.CharacterFormat.SetOwner(overrideLevelFormat.OverrideListLevel);
                //Set the owner for the list level paragraph format.
                overrideLevelFormat.OverrideListLevel.ParagraphFormat.SetOwner(overrideLevelFormat.OverrideListLevel);
                if (overrideLevelFormat.OverrideListLevel.PicBullet != null)
                    overrideLevelFormat.OverrideListLevel.PicBullet.CloneRelationsTo(Document, overrideLevelFormat.OverrideListLevel);
            }
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (m_overrideLevels == null || m_overrideLevels.Count == 0)
            {
                m_overrideLevels = null;
                return;
            }

            OverrideLevelFormat level = null;
            int cnt = m_overrideLevels.Count;
            foreach (KeyValuePair<int, int> keyValuePair in m_overrideLevels.LevelIndex)
            {
                level = m_overrideLevels[keyValuePair.Key];
                level.Close();
                level = null;
            }
        }
        #endregion

        #region Implementation / xml
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();
            XDLSHolder.AddElement(XDLSConstants.OverrideListLevelsTag, m_overrideLevels);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (m_res1 != 0)
            {
                writer.WriteValue(XDLSConstants.OverrideListRes1Attr, m_res1);
            }
            if (m_res2 != 0)
            {
                writer.WriteValue(XDLSConstants.OverrideListRes2Attr, m_res2);
            }
            if (m_unused1 != 0)
            {
                writer.WriteValue(XDLSConstants.OverrideListUnused1Attr, m_unused1);
            }
            if (m_unused2 != 0)
            {
                writer.WriteValue(XDLSConstants.OverrideListUnused2Attr, m_unused2);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.OverrideListRes1Attr))
            {
                m_res1 = reader.ReadInt(XDLSConstants.OverrideListRes1Attr);
            }
            if (reader.HasAttribute(XDLSConstants.OverrideListRes2Attr))
            {
                m_res2 = reader.ReadInt(XDLSConstants.OverrideListRes2Attr);
            }
            if (reader.HasAttribute(XDLSConstants.OverrideListUnused1Attr))
            {
                m_unused1 = reader.ReadInt(XDLSConstants.OverrideListUnused1Attr);
            }
            if (reader.HasAttribute(XDLSConstants.OverrideListUnused2Attr))
            {
                m_unused2 = reader.ReadInt(XDLSConstants.OverrideListUnused2Attr);
            }

        }
//#endif
        #endregion
    }
}
