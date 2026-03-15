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
using System.Text;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Base class for all Container classes
    /// </summary>
    internal class BaseContainer : BaseEscherRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ContainerCollection m_childrenContainers;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal ContainerCollection Children
        {
            get
            {
                return m_childrenContainers;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal BaseContainer(WordDocument doc) : base(doc)
        {
            m_childrenContainers = new ContainerCollection(doc);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        internal BaseContainer(MSOFBT type, WordDocument doc) : base(doc)
        {
            m_childrenContainers = new ContainerCollection(doc);
            Header.IsContainer = true;
            Header.Type = type;
        }
        #endregion

        #region Class static members
        /// <summary>
        /// Removes the container by spid.
        /// </summary>
        /// <param name="baseContainer">The base container.</param>
        /// <param name="spid">The spid.</param>
        /// <returns></returns>
        internal static bool RemoveContainerBySpid(BaseContainer baseContainer, int spid)
        {
            bool stopFlag = false;
            BaseEscherRecord baseRecord = null;
            for (int i = 0, cnt = baseContainer.Children.Count; i < cnt; i++)
            {
                baseRecord = baseContainer.Children[i] as BaseEscherRecord;
                if (stopFlag) break;
                if (baseRecord is MsofbtSpContainer)
                {
                    if ((baseRecord as MsofbtSpContainer).Shape.ShapeId == spid)
                    {
                        baseContainer.Children.Remove(baseRecord);
                        stopFlag = true;
                        break;
                    }
                }
                else if (baseRecord is BaseContainer)
                {
                    stopFlag = RemoveContainerBySpid(baseRecord as BaseContainer, spid);
                }
            }
            return stopFlag;
        }

        #endregion

        #region Class methods
        /// <summary>
        /// Synchronize identificators in escher container.
        /// </summary>
        /// <param name="autoShapeCollection">The auto shape collection.</param>
        /// <param name="txbxShapeId">The textbox shape id.</param>
        /// <param name="pictShapeId">The pict shape id.</param>
        /// <param name="txId">The text id.</param>
        /// <param name="textColIndex">Index of the text col.</param>
        internal void SynchronizeIdent(WTextBoxCollection autoShapeCollection,
          ref int txbxShapeId, ref int pictShapeId, ref int txId, ref int textColIndex)
        {
            BaseEscherRecord baseRecord = null;

            for (int i = 0, cnt = this.Children.Count; i < cnt; i++)
            {
                baseRecord = this.Children[i] as BaseEscherRecord;
                if (baseRecord is MsofbtSp)
                {
                    MsofbtSp shape = baseRecord as MsofbtSp;
                    SyncSpRecord(shape, autoShapeCollection, ref txbxShapeId, ref pictShapeId, ref textColIndex);
                }
                if (baseRecord is MsofbtOPT)
                {
                    SyncOPTTxid(baseRecord as MsofbtOPT, ref txId);
                }
                if (baseRecord is MsofbtClientTextbox)
                {
                    (baseRecord as MsofbtClientTextbox).Txid = txId;
                }

                if (baseRecord is BaseContainer)
                {
                    (baseRecord as BaseContainer).SynchronizeIdent(autoShapeCollection, ref txbxShapeId,
                      ref pictShapeId, ref txId, ref textColIndex);
                }
            }
        }

        /// <summary>
        /// Gets shape container's identificator.
        /// </summary>
        /// <returns></returns>
        internal int GetSpid()
        {
            int spid = 0;
            BaseEscherRecord baseRecord = null;
            for (int i = 0, cnt = this.Children.Count; i < cnt; i++)
            {
                baseRecord = this.Children[i] as BaseEscherRecord;
                if (spid != 0) break; ;
                if (baseRecord is MsofbtSp)
                {
                    spid = (baseRecord as MsofbtSp).ShapeId;
                    break;
                }
                if (baseRecord is BaseContainer)
                {
                    spid = (baseRecord as BaseContainer).GetSpid();
                }
            }
            return spid;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spid"></param>
        internal bool SetSpid(int spid)
        {
            bool stopFlag = false;
            BaseEscherRecord baseRecord = null;
            for (int i = 0, cnt = this.Children.Count; i < cnt; i++)
            {
                baseRecord = this.Children[i] as BaseEscherRecord;
                if (stopFlag) break;
                if (baseRecord is MsofbtSp)
                {
                    (baseRecord as MsofbtSp).ShapeId = spid;
                    stopFlag = true;
                    break;
                }
                if (baseRecord is BaseContainer)
                {
                    stopFlag = (baseRecord as BaseContainer).SetSpid(spid);
                }
            }
            return stopFlag;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msofbt"></param>
        /// <returns></returns>
        internal BaseEscherRecord FindContainerByMsofbt(MSOFBT msofbt)
        {
            for (int i = 0; i < m_childrenContainers.Count; i++)
            {
                BaseEscherRecord baseEscherRecord = m_childrenContainers[i] as BaseEscherRecord;
                if (baseEscherRecord.Header.Type == msofbt)
                {
                    return baseEscherRecord;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal BaseEscherRecord FindContainerByType(Type type)
        {
            for (int i = 0; i < m_childrenContainers.Count; i++)
            {
                BaseEscherRecord baseEscherRecord = m_childrenContainers[i] as BaseEscherRecord;
                if (baseEscherRecord.GetType() == type)
                {
                    return baseEscherRecord;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseContainer"></param>
        /// <returns></returns>
        internal BaseContainer FindParentContainer(BaseContainer baseContainer)
        {
            for (int i = 0; i < m_childrenContainers.Count; i++)
            {
                BaseContainer container = m_childrenContainers[i] as BaseContainer;
                if (container == baseContainer)
                {
                    return this;
                }
                // if children container is a parent container than try to search amoung it's children.
                if (container != null)
                {
                    BaseContainer found = container.FindParentContainer(baseContainer);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_childrenContainers.Read(stream, Header.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            m_childrenContainers.Write(stream);
        }
        //	  /// <summary>
        //	  /// 
        //	  /// </summary>
        //	  /// <param name="obj"></param>
        //    internal override void Add(object obj)
        //    {
        //      m_childrenContainers.AddToAllContainers(obj);
        //    }
        //	  /// <summary>
        //	  /// 
        //	  /// </summary>
        //	  /// <returns></returns>
        //	  internal override string ToString()
        //    {
        //      StringBuilder builder = new StringBuilder();
        //      builder.AppendFormat("{0} Start\n", GetType().Name);
        //      builder.AppendFormat("Self: {0}\n", base.ToString());
        //      for (int i = 0; i < m_childrenContainers.Count; i++)
        //      {
        //        builder.AppendFormat("{0}\n", m_childrenContainers[i].ToString());
        //      }
        //      builder.AppendFormat("{0} End\n", GetType().Name);
        //      return builder.ToString();
        //    }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>    
        internal override BaseEscherRecord Clone()
        {
            MSOFBT contType = Header.Type;
            BaseEscherRecord destContainer = this.Header.CreateRecordFromHeader();
            foreach (BaseEscherRecord baseRecord in this.Children)
            {
                BaseEscherRecord destRecord = baseRecord.Header.CreateRecordFromHeader();
                destRecord.Header = (_MSOFBH)baseRecord.Header.Clone();
                (destContainer as BaseContainer).Children.Add(baseRecord.Clone());
            }
            destContainer.m_doc = m_doc;
            return destContainer;
        }
        /// <summary>
        /// Clones the relations to.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal virtual void CloneRelationsTo(WordDocument doc)
        {
        }

        /// <summary>
        /// Remove Ole from BaseContainer.
        /// </summary>
        internal void RemoveBaseContainerOle()
        {
            BaseEscherRecord baseRecord = null;
            for (int i = 0, cnt = Children.Count; i < cnt; i++)
            {
                baseRecord = Children[i] as BaseEscherRecord;
                if (baseRecord is MsofbtSpContainer)
                {
                    if ((baseRecord as MsofbtSpContainer).Shape.IsOle)
                    {
                        MsofbtSpContainer spContainer = baseRecord as MsofbtSpContainer;
                        spContainer.RemoveSpContainerOle();
                    }
                }
                else if (baseRecord is BaseContainer)
                {
                    (baseRecord as BaseContainer).RemoveBaseContainerOle();
                }
            }
        }
        #endregion

        #region Class helper methods

        /// <summary>
        /// Syncronize Txid in OPT container.
        /// </summary>
        /// <param name="optRecord">The option record.</param>
        /// <param name="txId">The text id.</param>
        private void SyncOPTTxid(MsofbtOPT optRecord, ref int txId)
        {
            int startTxId = txId;
            FOPTEBid fopteBid = null;
            if (optRecord.Properties.ContainsKey(msofbtRGFOPTE.DEF_TXID))
            {
                fopteBid = optRecord.Properties[msofbtRGFOPTE.DEF_TXID] as FOPTEBid;
                txId += MsofbtSpContainer.DEF_TXID_INCREMENT;
                fopteBid.Value = ((uint)txId);
            }

            // Syncronize text identificator for OLE
            if (optRecord.Properties.ContainsKey((int)FOPTEBlip.pictureId)
                && (this as MsofbtSpContainer).Shape.ShapeType == EscherShapeType.msosptHostControl)
            {
                fopteBid = optRecord.Properties[(int)FOPTEBlip.pictureId] as FOPTEBid;
                if (startTxId == txId)
                    txId += MsofbtSpContainer.DEF_TXID_INCREMENT;
                fopteBid.Value = ((uint)txId);
            }

            //Synchronize linked textbox option    
            if (optRecord.Properties.ContainsKey((int)FOPTEText.hspNext))
            {
                fopteBid = optRecord.Properties[(int)FOPTEText.hspNext] as FOPTEBid;
                //fopteBid.Value = ( uint )( this as MsofbtSpContainer ).Shape.ShapeId;
                optRecord.Properties.Remove((int)FOPTEText.hspNext);
            }
        }

        /// <summary>
        /// Synchronize MsofbtSp.
        /// </summary>
        /// <param name="msofbtSp">The shape record.</param>
        /// <param name="autoShapeCollection">The auto shape collection.</param>
        /// <param name="txbxShapeId">The textbox shape id.</param>
        /// <param name="pictShapeId">The pict shape id.</param>
        /// <param name="textColIndex">Index in the text collection.</param>
        private void SyncSpRecord(MsofbtSp msofbtSp, WTextBoxCollection autoShapeCollection, ref int txbxShapeId,
          ref int pictShapeId, ref int textColIndex)
        {
            if (msofbtSp.ShapeType == EscherShapeType.msosptPictureFrame)
            {
                msofbtSp.ShapeId = pictShapeId;
                pictShapeId += 1;
            }
            else
            {
                msofbtSp.ShapeId = txbxShapeId;
                txbxShapeId += 1;
                if (autoShapeCollection != null && this is MsofbtSpContainer)
                {
                    if ((this as MsofbtSpContainer).ShapeOptions != null &&
                      ((this as MsofbtSpContainer).ShapeOptions.Txid != null ||
                      (this as MsofbtSpContainer).Shape.ShapeType == EscherShapeType.msosptHostControl))
                    {
                        if (autoShapeCollection.Count > 0)
                        {
                            (autoShapeCollection[textColIndex] as WTextBox).TextBoxSpid = msofbtSp.ShapeId;
                            textColIndex += 1;
                        }
                    }
                }
            }
        }
        #endregion

        #region Implementation / Close
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            if (m_childrenContainers == null || m_childrenContainers.Count == 0)
            {
                m_childrenContainers = null;
                return;
            }

            object record = null;
            for (int i = 0, cnt = m_childrenContainers.Count; i < cnt; i++)
            {
                record = m_childrenContainers[i];
                if (record is BaseContainer)
                {
                    (record as BaseContainer).Close();
                }
                else if (record is BaseEscherRecord)
                {
                    (record as BaseEscherRecord).Close();
                }
                else if (record is BaseWordRecord)
                {
                    (record as BaseWordRecord).Close();
                }
            }
        }
        #endregion
    }
}
