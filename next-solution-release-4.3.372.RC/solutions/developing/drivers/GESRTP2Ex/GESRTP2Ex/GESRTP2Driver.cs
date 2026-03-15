////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2Driver.cs
//
// summary:	Implements the driver GESRTP2 driver class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace GESRTP2
{
    /// <summary>   GESRTP2 communication driver. </summary>
    public class GESRTP2Driver : CommunicationDriver
    {
        private object lockInternalPrototypes = new object();
        private Dictionary<NodeId, List<TagDefinition>> internalPrototypes = new Dictionary<NodeId, List<TagDefinition>>();

        /// <summary>   Default constructor. </summary>
        public GESRTP2Driver()
            : base()
        {
        }

        public GESRTP2Driver(string strSettingPath)
            : base(strSettingPath)
        {
        }

        #region Overrides

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver init interface. </summary>
        ///
        /// <param name="strSettingPath">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxJ0V2Dx2Ij9ozl7B6jwS4IA=="/*MDB*/))
            {
                throw new LicenseOptionMissingException("Missing 'MDB' option in the license");
            }
#endif
            return base.Init(strSettingPath, isProtected, protectionCode);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver load settings init call. </summary>
        ///
        /// <param name="idl">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<GESRTP2DriverSettings>(ufw).AsParallel() select tag).Single();
                    LoadDriverSettings(configuration);
                    bRet = true;
                }
                catch (InvalidOperationException ex)
                {
                    OnSystemEvent(null, String.Format(Properties.Resources.ErrorLoadingDriverSettings, ex.Message), EventSeverity.Min);
                    LoadDefaultSettings();
                    bRet = true;
                }
            }

            return bRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver load settings. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="settings"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void LoadDriverSettings(DriverSettings settings)
        {
            base.LoadDriverSettings(settings);

            var conf = settings as GESRTP2DriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Driver Settings initializzation
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver save settings call. </summary>
        ///
        /// <param name="idl">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<GESRTP2DriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new GESRTP2DriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver save settings. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="settings"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as GESRTP2DriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Driver Settings initializzation
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new channel instance. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="settings"> . </param>
        ///
        /// <returns>   The new channel. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as GESRTP2ChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new GESRTP2Channel(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new station instance. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="settings"> . </param>
        ///
        /// <returns>   The new station. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as GESRTP2StationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new GESRTP2Station(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new tag instance. </summary>
        ///
        /// <param name="tagtoAdd"> . </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new GESRTP2Tag(tagtoAdd);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new tag instance. </summary>
        ///
        /// <param name="tagtoAdd">     . </param>
        /// <param name="byteoffset">   . </param>
        /// <param name="bitoffset">    . </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new GESRTP2Tag(tagtoAdd, byteoffset, bitoffset);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new tag settings instance. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="session">  . </param>
        /// <param name="tag">      . </param>
        ///
        /// <returns>   The new tag settings. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as GESRTP2Tag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enabled/disable prototype split. </summary>
        ///
        /// <returns>   true if a prototype split is enabled, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsPrototypeSplitEnabled()
        {
            return true;
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<GESRTP2ChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return error description. </summary>
        ///
        /// <param name="errorcode">    . </param>
        /// <param name="quality">      [out]. </param>
        /// <param name="error">        [out]. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void GetDriverErrorInfo(int sourceErrorcode, out uint quality, out string error)
        {
            GESRTP2Protocol.GetMainMinMajErrorCodes(sourceErrorcode, out int errorcode, out byte min, out byte maj);

            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((GESRTP2ErrorCodes)errorcode)
            {
                case GESRTP2ErrorCodes.ErrorTxWrite:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTxWrite;
                    break;
                case GESRTP2ErrorCodes.ErrorRxRead:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRxRead;
                    break;
                case GESRTP2ErrorCodes.ErrorUnexpectedConnectionReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedConnectionReply;
                    break;
                case GESRTP2ErrorCodes.ErrorUnexpectedSessionReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedSessionReply;
                    break;
                case GESRTP2ErrorCodes.ErrorUnexpectedPduType:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedPduType;
                    break;
                case GESRTP2ErrorCodes.ErrorUnexpectedInvokeId:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedInvokeId;
                    break;
                case GESRTP2ErrorCodes.ErrorNoRep:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorNoRep;
                    break;
                case GESRTP2ErrorCodes.ErrorUnexpectedMailSeq:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedMailSeq;
                    break;
                case GESRTP2ErrorCodes.ErrorReceiveFewData:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReceiveFewData;
                    break;
                case GESRTP2ErrorCodes.ErrorReceiveNack:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReceiveNack;
                    break;
                case GESRTP2ErrorCodes.ErrorUnexpectedReadReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedReadReply;
                    break;
                case GESRTP2ErrorCodes.ErrorUnexpectedWriteReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedWriteReply;
                    break;
                case GESRTP2ErrorCodes.ErrorUnexpectedCapabilities:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedCapabilities;
                    break;
                case GESRTP2ErrorCodes.ErrorVarBadNotFound:
                    quality = StatusCodes.BadNotFound;
                    error = Properties.Resources.ErrorSymbolicAddressNotFound;
                    break;
                case GESRTP2ErrorCodes.ErrorSymbolicUnsupportedDataType:
                    quality = StatusCodes.BadTypeDefinitionInvalid;
                    error = Properties.Resources.ErrorSymbolicUnsupportedDataType;
                    break;
                case GESRTP2ErrorCodes.ErrorDataTypeMisMatch:
                    quality = StatusCodes.BadTypeDefinitionInvalid;
                    error = Properties.Resources.ErrorSymbolicDataTypeMismatch;
                    break;
                case GESRTP2ErrorCodes.ErrorArraySizeMismatch:
                    quality = StatusCodes.BadTypeDefinitionInvalid;
                    error = Properties.Resources.ErrorSymbolicArraySizeMismatch;
                    break;
                case GESRTP2ErrorCodes.ErrorDirInvalid:
                    quality = StatusCodes.BadNotFound;
                    error = Properties.Resources.ErrorDirInvalid;
                    break;
            }

            if (errorcode != (int)DriverErrorCodes.ErrorNoError && (min != 0 || maj != 0))
                error = string.Format("{0};{1}", error, string.Format(Properties.Resources.ErrorMinMajAddtionalErrorCodes, "0x" + min.ToString("X2"), "0x" + maj.ToString("X2")));
        }

        public override void OnTagPrototypeQuery(NodeId sourceNode, ref List<TagDefinition> listTags)
        {
            lock (lockInternalPrototypes)
            {
                if (internalPrototypes.ContainsKey(sourceNode))
                    listTags.AddRange(internalPrototypes[sourceNode]);
                else
                    base.OnTagPrototypeQuery(sourceNode, ref listTags);
            }
        }

        public void AddInternalPrototype(NodeId sourceNode)
        {
            lock (lockInternalPrototypes)
            {
                if (!internalPrototypes.ContainsKey(sourceNode))
                    internalPrototypes[sourceNode] = new List<TagDefinition>();
            }
        }

        public void AddInternalPrototypeMember(NodeId sourceNode, Tag member)
        {
            lock (lockInternalPrototypes)
            {
                if (internalPrototypes.ContainsKey(sourceNode))
                {
                    TagDefinition memberDefinition = new TagDefinition
                    {
                        NodeId = member.TagNode.NodeId,
                        DynamicSettings = member.TagNode.DynamicSettings,
                        DataType = member.TagNode.DataType,
                        SamplingInterval = member.TagNode.SamplingInterval,
                        ArrayDimension = member.TagNode.ArrayDimension,
                        InitialValue = member.TagNode.InitialValue,
                        MemberOrder = member.TagNode.MemberOrder,
                        Name = member.TagNode.Name,
                    };

                    memberDefinition.MemberOrder = internalPrototypes[sourceNode].Count + 1;

                    internalPrototypes[sourceNode].Add(memberDefinition);
                }
            }
        }

        public void CreateInternalPrototypeFromTagsList(NodeId sourceNode, List<Tag> tagList)
        {
            lock (lockInternalPrototypes)
            {
                if (!internalPrototypes.ContainsKey(sourceNode))
                {
                    AddInternalPrototype(sourceNode);

                    foreach (Tag tag in tagList)
                        AddInternalPrototypeMember(sourceNode, tag);
                }
            }
        }

        public Tag GetInternalPrototypeMember(NodeId sourceNode, int memberOrder)
        {
            Tag firstMember = null;
            lock (lockInternalPrototypes)
            {
                if (internalPrototypes.ContainsKey(sourceNode))
                {
                    if (internalPrototypes[sourceNode].Count() > 0)
                    {
                        if (memberOrder < internalPrototypes[sourceNode].Count)
                            firstMember = CreateTag(internalPrototypes[sourceNode][memberOrder]);
                    }
                }
            }

            return firstMember;
        }

        public int GetInternalPrototypeNrMembers(NodeId sourceNode)
        {
            int nrMembers = 0;
            lock (lockInternalPrototypes)
            {
                if (internalPrototypes.ContainsKey(sourceNode))
                    nrMembers = internalPrototypes[sourceNode].Count();                
            }

            return nrMembers;
        }
        #endregion
    }
}
