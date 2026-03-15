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
using System.IO;
using System.Collections.Specialized;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for Escher.
    /// </summary>
    internal class EscherClass
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MAIN_DRAWING_ID = 1;
        private const int DEF_HF_DRAWING_ID = 2;
        private const int DEF_MAIN_SPID = 1024;
        private const int DEF_HF_SPID = 2048;
        private const int DEF_MAIN_SPIDMAX = 2050;
        private const int DEF_HF_SPIDMAX = 3074;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtDggContainer m_msofbtDggContainer;
        /// <summary>
        /// 
        /// </summary>
        internal ContainerCollection m_dgContainers;
        /// <summary>
        /// Collection which is used for fast container's search.
        /// </summary>
        private Dictionary<int, BaseContainer> m_containers;
        /// <summary>
        /// Background container.
        /// </summary>
        private MsofbtSpContainer m_backgroundContainer;
        private WordDocument m_doc;
        #endregion

        #region Class properties
        internal WordDocument Document
        {
            get
            {
                return m_doc;
            }
        }
        /// <summary>
        /// Get all containers that have spids.
        /// </summary>
        internal Dictionary<int, BaseContainer> Containers
        {
            get
            {
                if (m_containers == null)
                {
                    m_containers = new Dictionary<int, BaseContainer>();
                }
                return m_containers;
            }
        }
        /// <summary>
        /// Gets background container.
        /// </summary>
        internal MsofbtSpContainer BackgroundContainer
        {
            get
            {
                if (m_backgroundContainer == null)
                {
                    m_backgroundContainer = GetBackgroundContainer();
                }
                return m_backgroundContainer;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal EscherClass(WordDocument doc)
        {
            m_doc = doc;
            m_dgContainers = new ContainerCollection(m_doc);
            CreateDefaultDgg();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EscherClass"/> class.
        /// </summary>
        /// <param name="tableStream">The table stream.</param>
        /// <param name="docStream">The doc stream.</param>
        /// <param name="dggInfoOffset">The DGG info offset.</param>
        /// <param name="dggInfoLength">Length of the DGG info.</param>
        internal EscherClass(Stream tableStream, Stream docStream, int dggInfoOffset, int dggInfoLength, WordDocument doc)
            : this(doc)
        {
            tableStream.Position = dggInfoOffset;
            if (dggInfoLength != 0)
            {
                Read(tableStream, dggInfoLength, docStream);
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableStream"></param>
        /// <param name="dggInfoLength"></param>
        /// <param name="docStream"></param>
        internal void Read(Stream tableStream, int dggInfoLength, Stream docStream)
        {
            long endPos = tableStream.Position + dggInfoLength;
            m_msofbtDggContainer = _MSOFBH.ReadHeaderWithRecord(tableStream, m_doc) as MsofbtDggContainer;

            if (m_msofbtDggContainer == null)
            {
                throw new ArgumentException("First Escher record is not DggContainer.");
            }

            while (tableStream.Position < endPos)
            {
                int docType = tableStream.ReadByte();
                MsofbtDgContainer dgContainer = _MSOFBH.ReadHeaderWithRecord(tableStream, m_doc) as MsofbtDgContainer;

                if (dgContainer == null)
                {
                    throw new ArgumentException("Expected DgContainer records only.");
                }

                dgContainer.ShapeDocType = (ShapeDocType)docType;
                m_dgContainers.Add(dgContainer);
            }
            FillCollectionForSearch();
            ReadContainersData(docStream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Stream"></param>
        internal void ReadContainersData(Stream Stream)
        {
            foreach (MsofbtDgContainer dgContainer in m_dgContainers)
            {
                ReadBseData(dgContainer, Stream);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void WriteContainersData(Stream stream)
        {
            if ((m_msofbtDggContainer != null) && (m_msofbtDggContainer.BstoreContainer != null))
            {
                //foreach( MsofbtBSE msofbtBSE in m_msofbtDggContainer.BstoreContainer.Children )
                //{
                //  msofbtBSE.Write( stream );
                //}

                BaseEscherRecord record = null;
                for (int i = 0, cnt = m_msofbtDggContainer.BstoreContainer.Children.Count; i < cnt; i++)
                {
                    record = m_msofbtDggContainer.BstoreContainer.Children[i] as BaseEscherRecord;
                    if (record is MsofbtBSE)
                        (record as MsofbtBSE).Write(stream);
                    else
                        record.WriteMsofbhWithRecord(stream);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal uint WriteContainers(Stream stream)
        {
            if (m_msofbtDggContainer == null)
            {
                return 0;
            }

            long startPos = stream.Position;

            InitWriting();
            WriteDggContainer(stream);
            WriteDgContainers(stream);

            //      FileStream dgg = new FileStream( "d:\\dgg1.dat", FileMode.Open, FileAccess.Read );
            //      for( int i = 0; i < dgg.Length; i++ )
            //      {
            //        stream.WriteByte( ( byte )dgg.ReadByte() );
            //      }
            //      dgg.Close();


            return (uint)(stream.Position - startPos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="baseEscherRecord"></param>
        internal void AddContainerForSubDocument(WordSubdocument documentType, BaseEscherRecord baseEscherRecord)
        {
            if (m_msofbtDggContainer == null)
            {
                CreateDefaultDgg();
            }
            CreateDgForSubDocuments();
            //Add msofbtSp to group shape
            ShapeDocType shapeDocType = ConvertToShapeDocType(documentType);
            MsofbtDgContainer dgContainer = FindDgContainerForSubDocType(shapeDocType);
            dgContainer.PatriarchGroupContainer.Children.Add(baseEscherRecord);
            AddParentContainer(baseEscherRecord as BaseContainer);
            //Add container to search collection.
            FillCollectionForSearch(baseEscherRecord as BaseContainer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ShapeDocType"></param>
        /// <returns></returns>
        internal MsofbtDgContainer FindDgContainerForSubDocType(ShapeDocType ShapeDocType)
        {
            foreach (MsofbtDgContainer dgContainer in m_dgContainers)
            {
                if (dgContainer.ShapeDocType == ShapeDocType)
                {
                    return dgContainer;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        internal void InitShapeSpids()
        {
            //      MsofbtDgContainer mainDgContainer = FindDgContainerForSubDocType( ShapeDocType.Main );
            //      MsofbtDgContainer hfDgContainer = FindDgContainerForSubDocType( ShapeDocType.HeaderFooter );
            //      foreach(BaseContainer container in hfDgContainer.PatriarchGroupContainer.Children )
            //      {
            //      }
            //TODO: impl. later if need.
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// 
        /// </summary>
        internal void RemoveHeaderContainer()
        {
            if (m_dgContainers.Count > 1)
            {
                MsofbtDgContainer msofbtDgContainer = m_dgContainers[1] as MsofbtDgContainer;
                if (msofbtDgContainer.ShapeDocType == ShapeDocType.HeaderFooter)
                {
                    m_dgContainers.RemoveAt(1);
                }
                else
                {
                    throw new ArgumentException("Expected header drawing, but got something else.");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        internal BaseContainer FindParentContainer(BaseContainer baseContainer)
        {
            foreach (BaseContainer parentContainer in m_dgContainers)
            {
                BaseContainer container = parentContainer.FindParentContainer(baseContainer);
                if (container != null)
                {
                    return container;
                }
            }
#if DEFUG
      throw new ArgumentException( "Cannot find a parent container." );
#endif
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spid"></param>
        /// <returns></returns>
        internal int GetTxid(int spid)
        {
            return FindInDgContainers(spid).Txid;
        }
        /// <summary>
        /// Get shape order index
        /// </summary>
        /// <param name="spId"></param>
        /// <returns></returns>
        internal int GetShapeOrderIndex(int spId)
        {
            int orderIndex = -1;
            if (this.Containers != null && this.Containers.ContainsKey(spId))
            {
                MsofbtSpContainer container = this.Containers[spId] as MsofbtSpContainer;
                if (container != null)
                {
                    int[] keys = new int[this.Containers.Keys.Count];
                    this.Containers.Keys.CopyTo(keys, 0);
                    orderIndex = Array.IndexOf(keys, spId);
                }
            }
            return orderIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spid"></param>
        /// <param name="txid"></param>
        internal void SetTxid(int spid, int txid)
        {
            MsofbtSpContainer spContainer = FindInDgContainers(spid);
            spContainer.Txid = (txid);
            MsofbtClientTextbox clientTextbox =
              spContainer.FindContainerByMsofbt(MSOFBT.msofbtClientTextbox) as MsofbtClientTextbox;
            if (clientTextbox != null)
            {
                clientTextbox.Txid = (txid);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spid"></param>
        /// <returns></returns>
        internal BaseContainer FindContainerBySpid(int spid)
        {
            MsofbtSpContainer spContainer = FindInDgContainers(spid);
            if (spContainer == null) return null;
            if (spContainer.Shape.ShapeType == EscherShapeType.msosptMin
              && spContainer.Shape.IsGroup)
            {
                return FindParentContainer(spContainer);
            }
            return spContainer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spid"></param>
        /// <returns></returns>
        internal MsofbtSpContainer FindInDgContainers(int spid)
        {
            foreach (BaseContainer container in m_dgContainers)
            {
                MsofbtSpContainer spContainer = FindContainerAmongChildren(container, spid);
                if (spContainer != null)
                {
                    return spContainer;
                }
            }
#if DEFUG
      throw new ArgumentException( "Cannot find a shape by spid." );
#endif
            return null;
        }
        /// <summary>
        /// Get shape type of BaseEscherRecord
        /// </summary>
        /// <returns></returns>
        internal EscherShapeType GetBaseEscherRecordType(MsofbtSpContainer spCon)
        {
            return spCon.Shape.ShapeType;
        }

        /// <summary>
        /// Clone container in escher.
        /// </summary>
        /// <param name="destDoc">Destination document</param>
        /// <param name="docType">Wordsubdocument( main or header/footer)</param>
        /// <param name="spid">Spid of container, which will be cloned</param>
        /// <param name="newSpid">Result container spid</param>
        internal int CloneContainerBySpid(WordDocument destDoc, WordSubdocument docType, int spid, int newSpid)
        {
            BaseContainer checkContainer = null;
            if (destDoc.Escher.Containers.ContainsKey(newSpid))
                checkContainer = destDoc.Escher.Containers[newSpid];

            while (checkContainer != null)
            {
                if (destDoc.Escher.Containers.ContainsKey(newSpid))
                    checkContainer = destDoc.Escher.Containers[newSpid];
                else
                    break;
                newSpid += 1;
            }

            if (Containers.ContainsKey(spid))
            {
                BaseContainer clonedContainer = (BaseContainer)Containers[spid].Clone();
                clonedContainer.SetSpid(newSpid);
                destDoc.Escher.AddContainerForSubDocument(docType, clonedContainer);
                clonedContainer.CloneRelationsTo(destDoc);
            }
            else
            {
                return -1;
            }

            return newSpid;
        }

        /// <summary>
        /// Remove OLE from escher.
        /// </summary>
        internal void RemoveEscherOle()
        {
            foreach (BaseContainer baseContainer in m_dgContainers)
            {
                baseContainer.RemoveBaseContainerOle();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spid"></param>
        /// <param name="isHeaderContainer"></param>
        internal void RemoveContainerBySpid(int spid, bool isHeaderContainer)
        {
            MsofbtDgContainer dgContainer = FindDgContainerForSubDocType((isHeaderContainer) ?
              ShapeDocType.HeaderFooter : ShapeDocType.Main);
            bool stopFlag = false;
            BaseEscherRecord baseRecord = null;
            Containers.Remove(spid);

            for (int i = 0, cnt = dgContainer.Children.Count; i < cnt; i++)
            {
                if (stopFlag) break;
                baseRecord = dgContainer.Children[i] as BaseEscherRecord;
                if (baseRecord is MsofbtSpContainer)
                {
                    if ((baseRecord as MsofbtSpContainer).Shape.ShapeId == spid)
                    {
                        dgContainer.Children.Remove(baseRecord);
                        stopFlag = true;
                    }
                }
                else if (baseRecord is BaseContainer)
                {
                    stopFlag = BaseContainer.RemoveContainerBySpid(baseRecord as BaseContainer, spid);
                }
            }
        }
        /// <summary>
        /// Remove container with data from BStoreContainer by PID.
        /// </summary>
        /// <param name="pib"></param>
        internal void RemoveBStoreByPid(int pib)
        {
            foreach (BaseEscherRecord baseRecord in m_msofbtDggContainer.Children)
            {
                if (baseRecord is MsofbtBstoreContainer)
                {
                    if (pib <= (baseRecord as MsofbtBstoreContainer).Children.Count)
                    {
                        (baseRecord as MsofbtBstoreContainer).Children.RemoveAt(pib - 1);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Remove container with data from BStoreContainer by PID.
        /// </summary>
        /// <param name="pib"></param>
        /// <param name="bse"></param>
        internal void ModifyBStoreByPid(int pib, MsofbtBSE bse)
        {
            foreach (BaseEscherRecord baseRecord in m_msofbtDggContainer.Children)
            {
                if (baseRecord is MsofbtBstoreContainer)
                {
                    if (pib <= (baseRecord as MsofbtBstoreContainer).Children.Count)
                    {
                        ((baseRecord as MsofbtBstoreContainer).Children[pib - 1] as MsofbtBSE).Blip.ImageRecord = bse.Blip.ImageRecord;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Get background container. 
        /// </summary>
        /// <returns></returns>
        internal MsofbtSpContainer GetBackgroundContainer()
        {
            MsofbtDgContainer dgContainer = FindDgContainerForSubDocType(ShapeDocType.Main);
            if (dgContainer != null)
            {
                foreach (BaseEscherRecord baseRecord in dgContainer.Children)
                {
                    if (baseRecord is MsofbtSpContainer)
                    {
                        return baseRecord as MsofbtSpContainer;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Checks the existance of container with such pib and checks if Blip != null
        /// </summary>
        /// <param name="pib">The pib.</param>
        /// <returns></returns>
        internal bool CheckBStoreContByPid(int pib)
        {
            bool checkResult = false;
            if (m_msofbtDggContainer.BstoreContainer != null)
            {
                MsofbtBstoreContainer storeContainer = m_msofbtDggContainer.BstoreContainer;
                if (storeContainer.Children.Count >= pib)
                {
                    MsofbtBSE msofbtBse = (MsofbtBSE)storeContainer.Children[pib - 1];
                    if (msofbtBse.Blip != null)
                    {
                        checkResult = true;
                    }
                }
            }
            return checkResult;
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <param name="stream"></param>
        private void ReadBseData(BaseContainer baseContainer, Stream stream)
        {
            for (int i = 0; i < baseContainer.Children.Count; i++)
            {
                BaseEscherRecord child = baseContainer.Children[i] as BaseEscherRecord;
                if (child is MsofbtSpContainer)
                {
                    MsofbtSpContainer spContainer = child as MsofbtSpContainer;
                    if (spContainer.Shape != null)
                    {
                        int blipId = GetBlipId(spContainer);

                        if (blipId >= 0)
                        {
                            MsofbtBSE msofbtBSE = m_msofbtDggContainer.BstoreContainer.Children[blipId] as MsofbtBSE;
                            msofbtBSE.Read(stream);
                            spContainer.Bse = msofbtBSE;
                        }
                    }
                    //          if( spContainer.Shape != null && spContainer.Shape.ShapeType == EscherShapeType.msosptPictureFrame )
                    //          {
                    //            int blipId = spContainer.Pib - 1;
                    //            if( blipId >= 0 )
                    //            {
                    //              MsofbtBSE msofbtBSE = m_msofbtDggContainer.BstoreContainer.Children[ blipId ] as MsofbtBSE;
                    //              msofbtBSE.Read( stream );
                    //              spContainer.Bse = msofbtBSE;
                    //            }
                    //          }
                }
                if (child is BaseContainer)
                {
                    ReadBseData(child as BaseContainer, stream);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitWriting()
        {
            MsofbtDgg msofbtDgg = m_msofbtDggContainer.Dgg;
            int generalShapeCount = 0;
            msofbtDgg.Fidcls.Clear();
            for (int i = 0; i < m_dgContainers.Count; i++)
            {
                MsofbtDgContainer dgContainer = m_dgContainers[i] as MsofbtDgContainer;
                dgContainer.InitWriting();
                // Remove header/footer dg container if not needed
                if (dgContainer.ShapeDocType == ShapeDocType.HeaderFooter)
                {
                    if (dgContainer.Dg.ShapeCount <= 1)
                    {
                        RemoveContainerBySpid(DEF_HF_SPID, true);
                        m_dgContainers.RemoveAt(i);
                        i--;
                        generalShapeCount -= 1;
                        continue;
                    }
                }
                int docShapeCount = GetShapeCount(dgContainer.PatriarchGroupContainer);
                m_msofbtDggContainer.Dgg.Fidcls.Add(new FIDCL(dgContainer.Dg.DrawingId, docShapeCount + 1));
                generalShapeCount += docShapeCount + 1;
            }
            msofbtDgg.DrawingCount = (m_dgContainers.Count);
            msofbtDgg.ShapeCount = (generalShapeCount);
            msofbtDgg.SpidMax = (((m_dgContainers.Count + 1) * DEF_MAIN_SPID) + 2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void WriteDggContainer(Stream stream)
        {
            m_msofbtDggContainer.WriteMsofbhWithRecord(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void WriteDgContainers(Stream stream)
        {
            foreach (MsofbtDgContainer dgContainer in m_dgContainers)
            {
                stream.WriteByte((byte)dgContainer.ShapeDocType);
                dgContainer.WriteMsofbhWithRecord(stream);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal void CreateDgForSubDocuments()
        {
            if (FindDgContainerForSubDocType(ShapeDocType.Main) == null)
            {
                CreateDgForSubDocument(ShapeDocType.Main, DEF_MAIN_DRAWING_ID, DEF_MAIN_SPID);
            }
            if (FindDgContainerForSubDocType(ShapeDocType.HeaderFooter) == null)
            {
                CreateDgForSubDocument(ShapeDocType.HeaderFooter, DEF_HF_DRAWING_ID, DEF_HF_SPID);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void CreateDefaultDgg()
        {
            m_msofbtDggContainer = new MsofbtDggContainer(m_doc);
            MsofbtDgg dgg = new MsofbtDgg(m_doc);
            m_msofbtDggContainer.Children.Add(dgg);
            m_msofbtDggContainer.Children.Add(new MsofbtBstoreContainer(m_doc));
            //m_msofbtDggContainer.Children.Add( CreateGeneralData() );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeDocType"></param>
        /// <param name="drawingId"></param>
        /// <param name="shapeId"></param>
        private void CreateDgForSubDocument(ShapeDocType shapeDocType, int drawingId, int shapeId)
        {
            // Create and add created dg container to the collection
            MsofbtDgContainer dgContainer = new MsofbtDgContainer(m_doc);
            dgContainer.ShapeDocType = (shapeDocType);
            m_dgContainers.Add(dgContainer);

            // Create new Dg
            MsofbtDg msofbtDg = new MsofbtDg(m_doc);
            msofbtDg.DrawingId = drawingId;
            msofbtDg.ShapeCount = 1;
            msofbtDg.SpidLast = shapeId;

            dgContainer.Children.Add(msofbtDg);

            // Create new Spgr container
            MsofbtSpgrContainer spgrContainer = new MsofbtSpgrContainer(m_doc);
            dgContainer.Children.Add(spgrContainer);

            // Create new Sp container
            MsofbtSpContainer spContainer = new MsofbtSpContainer(m_doc);
            spgrContainer.Children.Add(spContainer);

            // Create new Spgr
            MsofbtSpgr spgr = new MsofbtSpgr(m_doc);
            spContainer.Children.Add(spgr);

            // Create new Sp
            MsofbtSp msofbtSp = new MsofbtSp(m_doc);
            msofbtSp.ShapeId = shapeId;
            msofbtSp.IsGroup = true;
            msofbtSp.IsPatriarch = true;
            msofbtSp.ShapeType = EscherShapeType.msosptMin;
            spContainer.Children.Add(msofbtSp);
            Containers.Add(spContainer.Shape.ShapeId, spContainer);

            if (shapeDocType == ShapeDocType.Main)
            {
                MsofbtSpContainer rectContainer = new MsofbtSpContainer(m_doc);
                dgContainer.Children.Add(rectContainer.CreateRectangleContainer());
                Containers.Add(rectContainer.Shape.ShapeId, spContainer);
            }

            // Create and add new Solver container
            dgContainer.Children.Add(new MsofbtSolverContainer(m_doc));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentContainer"></param>
        /// <param name="spid"></param>
        /// <returns></returns>
        private static MsofbtSpContainer FindContainerAmongChildren(BaseContainer parentContainer,
                                                                     int spid)
        {
            BaseEscherRecord baseContainer = null;
            for (int i = 0, cnt = parentContainer.Children.Count; i < cnt; i++)
            {
                baseContainer = parentContainer.Children[i] as BaseEscherRecord;

                if (baseContainer is MsofbtSp)
                {
                    MsofbtSp msofbtSp = baseContainer as MsofbtSp;
                    //          if( msofbtSp.ShapeId != spid )
                    //          {
                    //            continue;
                    //          }
                    //          return ( parentContainer as MsofbtSpContainer );
                    if (msofbtSp.ShapeId == spid)
                    {
                        return (parentContainer as MsofbtSpContainer);
                    }
                    else
                    {
                        continue;
                    }
                }
                if (baseContainer is BaseContainer)
                {
                    MsofbtSpContainer spContainer =
                      FindContainerAmongChildren(baseContainer as BaseContainer, spid);
                    if (spContainer != null)
                    {
                        return spContainer;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spContainer"></param>
        private void AddShapeBse(MsofbtSpContainer spContainer)
        {
            //if( spContainer.Shape.ShapeType == EscherShapeType.msosptPictureFrame  && spContainer.Bse != null )
            if (spContainer.IsWatermark && spContainer.Pib != -1) return;

            if (spContainer.Bse != null)
            {
                if (m_msofbtDggContainer.BstoreContainer == null)
                {
                    m_msofbtDggContainer.Children.Add(new MsofbtBstoreContainer(m_doc));
                }
                m_msofbtDggContainer.BstoreContainer.Children.Add(spContainer.Bse);
                if (spContainer.Shape.ShapeType == EscherShapeType.msosptPictureFrame)
                {
                    spContainer.Pib = (m_msofbtDggContainer.BstoreContainer.Children.Count);
                }
                else
                {
                    FOPTEBid fopteBid = spContainer.ShapeOptions.Properties[(int)FOPTEFillStyle.fillBlip] as FOPTEBid;
                    if (fopteBid != null)
                    {
                        fopteBid.Value = (uint)m_msofbtDggContainer.BstoreContainer.Children.Count;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docType"></param>
        /// <returns></returns>
        internal static ShapeDocType ConvertToShapeDocType(WordSubdocument docType)
        {
            switch (docType)
            {
                case WordSubdocument.Main:
                    {
                        return ShapeDocType.Main;
                    }
                case WordSubdocument.HeaderFooter:
                    {
                        return ShapeDocType.HeaderFooter;
                    }
            }
            throw new Exception("Windows.Media for " + docType.ToString() + " document is not available");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseContainer"></param>
        private void AddParentContainer(BaseContainer baseContainer)
        {
            if (baseContainer is MsofbtSpContainer)
            {
                AddShapeBse(baseContainer as MsofbtSpContainer);
            }
            for (int i = 0; i < baseContainer.Children.Count; i++)
            {
                BaseEscherRecord escherRecord = baseContainer.Children[i] as BaseEscherRecord;
                if (escherRecord is MsofbtSpContainer)
                {
                    AddShapeBse(escherRecord as MsofbtSpContainer);
                }
                else if (escherRecord is BaseContainer)
                {
                    AddParentContainer(escherRecord as BaseContainer);
                }
            }
        }
        /// <summary>
        /// Create and fill MsofbtGeneral (textboxes only)
        /// </summary>
        /// <returns></returns>
        private MsofbtGeneral CreateGeneralData()
        {
            MsofbtGeneral genData = new MsofbtGeneral(m_doc);
            genData.Data = new byte[16] { 255, 255, 0, 0, 0, 0,
                                    255, 0, 128, 128, 128,
                                    0, 247, 0, 0, 16 };
            return genData;
        }
        /// <summary>
        /// Initialize FIDCL for textboxes
        /// </summary>
        /// <param name="dgid"></param>
        /// <param name="cspidCur"></param>
        private void InitFidcl(int dgid, int cspidCur)
        {
            FIDCL fidclObj = new FIDCL(dgid, cspidCur);
            if ((m_msofbtDggContainer.Dgg.Fidcls.Count == 0) || (!FindFIDCLDgid(dgid, fidclObj)))
            {
                m_msofbtDggContainer.Dgg.Fidcls.Add(fidclObj);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dgid"></param>
        /// <param name="fidclObj"></param>
        /// <returns></returns>
        private bool FindFIDCLDgid(int dgid, FIDCL fidclObj)
        {
            int fidClCounter = 0;
            foreach (FIDCL fidClItem in m_msofbtDggContainer.Dgg.Fidcls)
            {
                if (fidClItem.m_dgid == dgid)
                {
                    m_msofbtDggContainer.Dgg.Fidcls[fidClCounter] = fidclObj;
                    return true;
                }
                fidClCounter++;
            }
            return false;
        }
        /// <summary>
        /// Get the number of shaped in DgContainer.
        /// </summary>
        private int GetShapeCount(BaseContainer baseContainer)
        {
            int docShapeCount = 0;
            foreach (BaseEscherRecord baseRecord in baseContainer.Children)
            {
                if (baseRecord is MsofbtSpContainer)
                {
                    docShapeCount++;
                }
                else if (baseRecord is BaseContainer)
                {
                    docShapeCount += GetShapeCount(baseRecord as BaseContainer);
                }
            }
            return docShapeCount;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="spContainer"></param>
        /// <returns></returns>
        private int GetBlipId(MsofbtSpContainer spContainer)
        {
            uint fillBlip = spContainer.GetPropertyValue((int)FOPTEFillStyle.fillBlip);
            if (fillBlip != uint.MaxValue)
            {
                return (int)(fillBlip - 1);
            }
            uint pib = spContainer.GetPropertyValue((int)FOPTEBlip.pib);
            if (pib != uint.MaxValue)
            {
                return (int)(pib - 1);
            }
            return -1;
        }
        /// <summary>
        /// Fill m_containers hash table
        /// </summary>
        private void FillCollectionForSearch()
        {
            foreach (MsofbtDgContainer dgContainer in m_dgContainers)
            {
                FillCollectionForSearch(dgContainer);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseContainer"></param>
        internal void FillCollectionForSearch(BaseContainer baseContainer)
        {
            if (baseContainer is MsofbtSpContainer)
            {
                AddSpContToSearchCol(baseContainer as MsofbtSpContainer);
            }
            else
            {
                foreach (BaseEscherRecord baseRecord in baseContainer.Children)
                {
                    if (baseRecord is BaseContainer)
                    {
                        FillCollectionForSearch(baseRecord as BaseContainer);
                    }
                    if (baseRecord is MsofbtSpgrContainer)
                    {
                        MsofbtSpgrContainer groupContainer = baseRecord as MsofbtSpgrContainer;
                        if (!Containers.ContainsKey(groupContainer.Shape.ShapeId))
                        {
                            Containers.Add(groupContainer.Shape.ShapeId, groupContainer);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spContainer"></param>
        private void AddSpContToSearchCol(MsofbtSpContainer spContainer)
        {
            if (!Containers.ContainsKey(spContainer.Shape.ShapeId))
            {
                if (spContainer.Shape.IsGroup)
                {
                    Containers.Add(spContainer.Shape.ShapeId, FindParentContainer(spContainer));
                }
                else
                {
                    Containers.Add(spContainer.Shape.ShapeId, spContainer);
                }
            }
        }
        #endregion

        #region Implementation / Close
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (m_containers != null)
            {
                m_containers.Clear();
                m_containers = null;
            }

            MsofbtDgContainer dgContainer = null;
            for (int i = 0, cnt = m_dgContainers.Count; i < cnt; i++)
            {
                dgContainer = m_dgContainers[i] as MsofbtDgContainer;
                dgContainer.Close();
                dgContainer = null;
            }

            if (m_msofbtDggContainer != null)
            {
                m_msofbtDggContainer.Close();
            }
        }

        #endregion
    }
}