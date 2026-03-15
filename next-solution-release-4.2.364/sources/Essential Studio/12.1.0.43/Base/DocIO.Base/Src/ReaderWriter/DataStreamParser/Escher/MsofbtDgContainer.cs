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

using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Drawing Container msofbtDgContainer
    /// The drawing container contains all per-slide/sheet types of information, including the shapes 
    /// themselves. With a few exceptions, shapes are stored hierarchically according to how they've been 
    /// grouped (through use of the Draw/Group command). For normal shapes, there is a special parent 
    /// group shape called the patriarch that contains all of the top-level shapes (which in turn may 
    /// contain other shapes). The patriarch is always the first msofbtSpgrContainer in the drawing 
    /// container. A few kinds of shapes are stored separately from the patriarch. The background shape, 
    /// if there is one, is saved in its own msofbtSpContainer after the patriarch and its children. 
    /// Additionally, if undo information is being saved and there are deleted shapes that could be brought
    /// back via Undo, the deleted shapes are saved. Note that there is no patriarch for the deleted shapes, 
    /// so the top-level deleted shapes are saved separately into the drawing container. (Deleted groups 
    /// still contain their children, though.)
    /// </summary>
    internal class MsofbtDgContainer : BaseContainer
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ShapeDocType m_shapeDocType;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtDg Dg
        {
            get
            {
                return (FindContainerByType(typeof(MsofbtDg)) as MsofbtDg);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSpgrContainer PatriarchGroupContainer
        {
            get
            {
                return (FindContainerByType(typeof(MsofbtSpgrContainer)) as MsofbtSpgrContainer);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ShapeDocType ShapeDocType
        {
            get
            {
                return m_shapeDocType;
            }
            set
            {
                m_shapeDocType = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtDgContainer(WordDocument doc)
            : base(MSOFBT.msofbtDgContainer, doc)
        { }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <param name="shapeCount"></param>
        /// <param name="spidMax"></param>
        private static void GetShapeCountAndMaxSpid(BaseContainer baseContainer,
                                                         ref int shapeCount, ref int spidMax)
        {
            for (int i = 0; i < baseContainer.Children.Count; i++)
            {
                BaseEscherRecord baseEscherRecord = baseContainer.Children[i] as BaseEscherRecord;
                if (baseEscherRecord is MsofbtSp)
                {
                    MsofbtSp msofbtSp = baseEscherRecord as MsofbtSp;
                    spidMax = Math.Max(spidMax, msofbtSp.ShapeId);
                    shapeCount++;
                }
                if (baseEscherRecord is BaseContainer)
                {
                    GetShapeCountAndMaxSpid(baseEscherRecord as BaseContainer, ref shapeCount, ref spidMax);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal void InitWriting()
        {
            int shapeCount = 0;
            int spidMax = 0;
            GetShapeCountAndMaxSpid(PatriarchGroupContainer, ref shapeCount, ref spidMax);
            Dg.ShapeCount = (shapeCount);
            Dg.SpidLast = (spidMax);
        }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    internal void RemoveDeletedContainers()
        //    {
        //      for( int i = 0; i < Children.Count; i++ )
        //      {
        //        if( Children[ i ] as MsofbtSpContainer != null )
        //        {
        //          if( ( Children[ i ] as MsofbtSpContainer ).Shape.IsDeleted )
        //          {
        //            Children.RemoveAt( i );
        //            i--;
        //          }
        //        }
        //        else if( Children[ i ] as MsofbtSpgrContainer != null )
        //        {
        //          if( ( Children[ i ] as MsofbtSpContainer ).Shape.IsDeleted )
        //          {
        //            Children.RemoveAt( i );
        //            i--;
        //          }
        //        }
        //      }
        //    }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtDgContainer dgContainer = (MsofbtDgContainer)base.Clone();
            dgContainer.m_shapeDocType = m_shapeDocType;
            dgContainer.m_doc = m_doc;
            return dgContainer;
        }

        #endregion
    }
}
