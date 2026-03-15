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
using Syncfusion.DocIO.DLS.XML;
using WListLevel = Syncfusion.DocIO.DLS.WListLevel;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a ListStyle.
    /// </summary>
    public class ListStyle : XDLSSerializableBase, IStyle
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MULTIPLIER = 72;
        /// <summary>
        /// List bullets type
        /// </summary>
        internal const string DEF_BULLLET_FIRST = "\uf0b7";
        internal const string DEF_BULLLET_SECOND = "o";
        internal const string DEF_BULLLET_THIRD = "\uf0a7";

        #endregion

        #region Class member
        /// <summary>
        /// 
        /// </summary>
        private ListLevelCollection m_levels;
        /// <summary>
        /// 
        /// </summary>
        private ListType m_listType;
        /// <summary>
        /// 
        /// </summary>
        private string m_name;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isHybrid;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isSimple;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isBuiltInStyle;
        /// <summary>
        /// 
        /// </summary>
        private string m_baseLstStyle;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or Sets the style name.
        /// </summary>
        /// <value></value>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        /// <summary>
        /// Gets / sets list type
        /// </summary>
        public ListType ListType
        {
            get
            {
                return m_listType;
            }
            set
            {
                m_listType = value;
            }
        }
        /// <summary>
        /// Gets list levels collection
        /// </summary>
        public ListLevelCollection Levels
        {
            get
            {
                return m_levels;
            }
        }
        /// <summary>
        /// Gets the type of the style.
        /// </summary>
        /// <value>The type of the style.</value>
        public StyleType StyleType
        {
            get
            {
                return StyleType.OtherStyle;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this list style is hybrid multilevel/simple.
        /// </summary>
        /// <value>
        /// 	if this instance is hybrid multilevel, set to <c>true</c>.
        /// </value>
        internal bool IsHybrid
        {
            get
            {
                return m_isHybrid;
            }
            set
            {
                m_isHybrid = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the list style is simple.
        /// </summary>
        /// <value>if this instance is simple, set to <c>true</c>.</value>
        internal bool IsSimple
        {
            get
            {
                return m_isSimple;
            }
            set
            {
                m_isSimple = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is built in style.
        /// </summary>
        /// <value>
        /// 	if this instance is built in style, set to <c>true</c>.
        /// </value>
        internal bool IsBuiltInStyle
        {
            get
            {
                return m_isBuiltInStyle;
            }
            set
            {
                m_isBuiltInStyle = value;
            }
        }
        /// <summary>
        /// Gets or Sets the base list style identifier.
        /// </summary>
        /// <value>The base list style id.</value>
        internal string BaseListStyleName
        {
            get
            {
                return m_baseLstStyle;
            }
            set
            {
                m_baseLstStyle = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ListStyle"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="listType">Type of the list.</param>
        public ListStyle(IWordDocument doc, ListType listType)
            : this((WordDocument)doc)
        {
            m_listType = listType;
            CreateDefListLevels(listType);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ListStyle"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="listType">Type of the list.</param>
        /// <param name="isOneLevelList">if it specifies one level list, set to <c>true</c>.</param>
        internal ListStyle(WordDocument doc, ListType listType, bool isOneLevelList)
            : this(doc)
        {
            m_listType = listType;
            CreateEmptyListLevels(isOneLevelList);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ListStyle"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal ListStyle(WordDocument doc)
            : base(doc, doc)
        {
            m_levels = new ListLevelCollection(this);
            m_levels.SetOwner(this);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="listType">List type(bulleted or numbered)</param>
        /// <param name="isOneLevelList"> Is it list that consist of 1 level only.</param>
        /// <returns></returns>
        public static ListStyle CreateEmptyListStyle(IWordDocument doc, ListType listType, bool isOneLevelList)
        {
            ListStyle emptyStyle = new ListStyle((WordDocument)doc, listType, isOneLevelList);
            return emptyStyle;
        }
        /// <summary>
        /// Clones current style object
        /// </summary>
        /// <returns></returns>
        public IStyle Clone()
        {
            return CloneImpl() as IStyle;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (m_levels == null || m_levels.Count == 0)
            {
                m_levels = null;
                return;
            }

            int cnt = m_levels.Count;
            WListLevel level = null;
            for (int i = 0; i < cnt; i++)
            {
                level = m_levels[i];
                level.Close();
                level = null;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            ListStyle ls = (ListStyle)base.CloneImpl();
            ls.m_levels = new ListLevelCollection(ls);
            m_levels.CloneToImpl(ls.m_levels);

            return ls;
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();
            XDLSHolder.AddElement(XDLSConstants.ListLevelsTag, Levels);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.StyleNameAttr, Name);
            writer.WriteValue(XDLSConstants.ListFormatTypeAttr, ListType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);
            m_name = reader.ReadString(XDLSConstants.StyleNameAttr);
            ListType = (ListType)reader.ReadEnum(XDLSConstants.ListFormatTypeAttr, typeof(Syncfusion.DocIO.DLS.ListType));
        }
//#endif
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal void CreateDefListLevels(ListType listType)
        {
            Levels.Clear();
            WListLevel tempLevel = Document.CreateListLevelImpl(this);// new ListLevel( this );
            if (listType == ListType.Bulleted)
            {
                for (float i = 0.5f; i < 4.5f; i += 1.5f)
                {
                    Levels.Add(WListLevel.CreateDefBulletLvl((int)(DEF_MULTIPLIER * i), DEF_BULLLET_FIRST, this));
                    Levels.Add(WListLevel.CreateDefBulletLvl((int)(DEF_MULTIPLIER * (i + 0.5)), DEF_BULLLET_SECOND, this));
                    Levels.Add(WListLevel.CreateDefBulletLvl((int)(DEF_MULTIPLIER * (i + 1)), DEF_BULLLET_THIRD, this));
                }
            }
            else
            {
                int level = 0;
                for (float i = 0.5f; i < 4.5f; i += 1.5f)
                {
                    Levels.Add(
                      WListLevel.CreateDefNumberLvl((int)(DEF_MULTIPLIER * i),
                      level++,
                      ListPatternType.Arabic,
                      ListNumberAlignment.Left,
                      this));
                    Levels.Add(
                      WListLevel.CreateDefNumberLvl((int)(DEF_MULTIPLIER * (i + 0.5)),
                      level++,
                      ListPatternType.LowLetter,
                      ListNumberAlignment.Right,
                      this));
                    Levels.Add(
                      WListLevel.CreateDefNumberLvl((int)(DEF_MULTIPLIER * (i + 1)),
                      level++,
                      ListPatternType.LowRoman,
                      ListNumberAlignment.Left,
                      this));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public WListLevel GetNearLevel(int levelNumber)
        {
            if (levelNumber < 0)
                throw new ArgumentOutOfRangeException("number", "Value can not be less than 0");

            if (levelNumber > Levels.Count - 1)
            {
                levelNumber = Levels.Count - 1;
            }

            return Levels[levelNumber];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listType"></param>
        /// <param name="isOneLevelList"></param>
        internal void CreateEmptyListLevels(bool isOneLevelList)
        {
            int cnt = (isOneLevelList) ? 1 : 9;
            for (int i = 0; i < cnt; i++)
            {
                Levels.Add(Document.CreateListLevelImpl(this));
                // new ListLevel( this ));
            }
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
            //Set the owner property for the list style
            SetOwner(doc);
            //Set the owner property for all levels for all list styles.
            Levels.SetOwner(this);
            //Iterate the list level for set owner property for all inner level list style property
            for (int i = 0; i < Levels.Count; i++)
            {
                Levels[i].SetOwner(this);
                Levels[i].CharacterFormat.SetOwner(Levels[i]);
                Levels[i].ParagraphFormat.SetOwner(Levels[i]);
                if (Levels[i].PicBullet != null)
                {
                    Levels[i].PicBullet.CloneRelationsTo(Document, Levels[i]);
                }
            }
        }
        #endregion
    }
}
