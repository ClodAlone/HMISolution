////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104Driver.cs
//
// summary:	Implements the driver IEC60870_5_104 driver class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace IEC60870_5_104
{
    /// <summary>   IEC60870_5_104 communication driver. </summary>
    public class IEC60870_5_104Driver : CommunicationDriver
    {
        /// <summary>   Default constructor. </summary>
        public IEC60870_5_104Driver()
            : base()
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
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxMjY539eIREBouUCiSTGu7w=="/*TLM*/))
            {
                throw new LicenseOptionMissingException("Missing 'TLM' option in the license");
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
                    var configuration = (from tag in new XPQuery<IEC60870_5_104DriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as IEC60870_5_104DriverSettings;
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
                var configuration = (from tag in new XPQuery<IEC60870_5_104DriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new IEC60870_5_104DriverSettings(ufw));
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

            var conf = settings as IEC60870_5_104DriverSettings;
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
            var conf = settings as IEC60870_5_104ChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new IEC60870_5_104Channel(this, conf);
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
            var conf = settings as IEC60870_5_104StationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new IEC60870_5_104Station(this, conf);
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
            return new IEC60870_5_104Tag(tagtoAdd);
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
            return new IEC60870_5_104Tag(tagtoAdd, byteoffset, bitoffset);
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
            var tagItem = tag as IEC60870_5_104Tag;
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
                return new List<ChangeTag>((new XPQuery<IEC60870_5_104ChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return error description. </summary>
        ///
        /// <param name="errorcode">    . </param>
        /// <param name="quality">      [out]. </param>
        /// <param name="error">        [out]. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((IEC60870_5_104ErrorCodes)errorcode)
            {
                case IEC60870_5_104ErrorCodes.ErrorTxWrite:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTxWrite;
                    break;
                case IEC60870_5_104ErrorCodes.unknown_type_identification:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.unknown_type_identification;
                    break;
                case IEC60870_5_104ErrorCodes.unknown_cause_of_transmission:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.unknown_cause_of_transmission;
                    break;
                case IEC60870_5_104ErrorCodes.unknown_common_address_of_ASDU:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.unknown_common_address_of_ASDU;
                    break;
                case IEC60870_5_104ErrorCodes.unknown_information_object_address:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.unknown_information_object_address;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorRxRead:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRxRead;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorConnectionBroken:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConnectionBroken;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorWriteException:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.ErrorWriteException;
                    break;
                case IEC60870_5_104ErrorCodes.cause_of_transmission_pn:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.cause_of_transmission_pn;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorFileTransferException:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.ErrorFileTransferException;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorFileTransferMalformedPacket:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFileTransferMalformedPacket;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorFileTransferFileReadyNegativeConfirm:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFileTransferFileReadyNegativeConfirm;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorFileTransferChecksum:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFileChecksum;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorFileTransferSave:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.ErrorSavingFile;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorDirectoryException:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.ErrorDirectoryException;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorReadingDirectoryContents:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadingDirectoryContents;
                    break;
                case IEC60870_5_104ErrorCodes.ErrorDirectoryMalformedPacket:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorDirectoryMalformedPacket;
                    break;
                    //case IEC60870_5_104ErrorCodes.ErrorUnexpectedConnectionReply:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorUnexpectedConnectionReply;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorUnexpectedSessionReply:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorUnexpectedSessionReply;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorUnexpectedPduType:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorUnexpectedPduType;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorUnexpectedInvokeId:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorUnexpectedInvokeId;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorNoRep:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorNoRep;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorUnexpectedMailSeq:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorUnexpectedMailSeq;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorReceiveFewData:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorReceiveFewData;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorReceiveNack:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorReceiveNack;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorUnexpectedReadReply:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorUnexpectedReadReply;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorUnexpectedWriteReply:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorUnexpectedWriteReply;
                    //    break;
                    //case IEC60870_5_104ErrorCodes.ErrorUnexpectedCapabilities:
                    //    quality = StatusCodes.BadCommunicationError;
                    //    error = Properties.Resources.ErrorUnexpectedCapabilities;
                    //    break;
            }
        }

        #endregion
    }
}
