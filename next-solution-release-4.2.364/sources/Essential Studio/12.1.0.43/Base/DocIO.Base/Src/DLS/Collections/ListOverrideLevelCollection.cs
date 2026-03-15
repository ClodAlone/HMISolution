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
using System.Collections;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the list levels of ListOverrideStyle.
    /// </summary>
    internal class ListOverrideLevelCollection : XDLSSerializableCollection
    {
        #region Private members
        private Dictionary<int, int> m_levelIndex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.OverrideLevelFormat"/> at the specified index.
        /// </summary>
        /// <value></value>
        public OverrideLevelFormat this[int levelNumber]
        {
            get
            {
                return (OverrideLevelFormat)InnerList[LevelIndex[levelNumber]];
            }
        }
        /// <summary>
        /// Gets the owner style.
        /// </summary>
        /// <value>The owner style.</value>
        private ListOverrideStyle OwnerStyle
        {
            get
            {
                return OwnerBase as ListOverrideStyle;
            }
        }
        /// <summary>
        /// Gets or sets the level index.
        /// </summary>
        /// <value>The .</value>
        internal Dictionary<int, int> LevelIndex
        {
            get
            {
                if (m_levelIndex == null)
                    m_levelIndex = new Dictionary<int, int>();
                return m_levelIndex;
            }
            set
            {
                m_levelIndex = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListOverrideLevelCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal ListOverrideLevelCollection(WordDocument doc)
            : base(doc, doc)
        { }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds List level into collection. 
        /// </summary>
        /// <param name="lfoLevel"></param>
        /// <returns></returns>
        internal int Add(int levelNumber, OverrideLevelFormat lfoLevel)
        {
            lfoLevel.SetOwner(OwnerStyle);
            int index = InnerList.Add(lfoLevel);
            if (LevelIndex.ContainsKey(levelNumber))
                LevelIndex[levelNumber] = index;
            else
                LevelIndex.Add(levelNumber, index);
            return index;
        }
        /// <summary>
        /// Gets the level number.
        /// </summary>
        /// <param name="levelFormat">The level format.</param>
        /// <returns></returns>
        internal int GetLevelNumber(OverrideLevelFormat levelFormat)
        {
            int index = InnerList.IndexOf(levelFormat);
            int levelNumber = index;
            foreach (KeyValuePair<int, int> keyPair in LevelIndex)
            {
                if (keyPair.Value == index)
                {
                    levelNumber = keyPair.Key;
                    break;
                }
            }
            return levelNumber;
        }
        /// <summary>
        /// Determines whether the current list override style has specified level.
        /// </summary>
        /// <param name="levelNumber">The level number.</param>
        /// <returns>
        /// 	<c>true</c> if the current list override style has specified level; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasOverrideLevel(int levelNumber)
        {
            if(LevelIndex.Count > 0)
                return LevelIndex.ContainsKey(levelNumber);
            return false;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <param name="collection">The collection.</param>
        internal override void CloneToImpl(CollectionImpl collection)
        {
            base.CloneToImpl(collection);

            foreach (KeyValuePair<int, int> keyValuePair in LevelIndex)
            {
                (collection as ListOverrideLevelCollection).LevelIndex.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new OverrideLevelFormat(Document);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        /// <value></value>
        protected override string GetTagItemName()
        {
            return XDLSConstants.OverrideListLevelTag;
        }
        #endregion
    }
}
