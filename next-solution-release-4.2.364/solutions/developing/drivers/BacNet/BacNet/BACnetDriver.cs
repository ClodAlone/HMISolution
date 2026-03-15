////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetDriver.cs
//
// summary:	Implements the driver BACnet driver class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace BACnet
{
    /// <summary>   BACnet communication driver. </summary>
    public class BACnetDriver : CommunicationDriver
    {
        /// <summary>   Default constructor. </summary>
        public BACnetDriver()
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
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxDkmnbdFT0I5Fy1y5SsWaqg=="/*FCS*/))
            {
                throw new LicenseOptionMissingException("Missing 'FCS' option in the license");
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
                    var configuration = (from tag in new XPQuery<BACnetDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as BACnetDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Driver Settings initializzation
            _AllowOtherBACnetClients = conf.AllowOtherBACnetClients.Value;
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
                var configuration = (from tag in new XPQuery<BACnetDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new BACnetDriverSettings(ufw));
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

            var conf = settings as BACnetDriverSettings;
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
            var conf = settings as BACnetChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new BACnetChannel(this, conf);
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
            var conf = settings as BACnetStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new BACnetStation(this, conf);
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
            return new BACnetTag(tagtoAdd);
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
            return new BACnetTag(tagtoAdd, byteoffset, bitoffset);
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
            var tagItem = tag as BACnetTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<BACnetChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
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

            switch ((BACnetErrorCodes)errorcode)
            {
                case BACnetErrorCodes.ErrorTxWrite:
                    quality = StatusCodes.BadDisconnect;
                    error = Properties.Resources.ErrorTxWrite;
                    break;
                case BACnetErrorCodes.ErrorUnexpectedInvokeId:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedInvokeId;
                    break;
                case BACnetErrorCodes.ErrorProtocolError:
                    quality = StatusCodes.BadDecodingError;
                    error = Properties.Resources.ErrorProtocolError;
                    break;
                case BACnetErrorCodes.ErrorUnknownProperty:
                    quality = StatusCodes.BadDataUnavailable;
                    error = Properties.Resources.ErrorUnknownProperty;
                    break;
                case BACnetErrorCodes.ErrorWriteAccessDenied:
                    quality = StatusCodes.BadNotWritable;
                    error = Properties.Resources.ErrorWriteAccessDenied;
                    break;
                case BACnetErrorCodes.ErrorValueOutOfRange:
                    quality = StatusCodes.BadOutOfRange;
                    error = Properties.Resources.ErrorValueOutOfRange;
                    break;
                case BACnetErrorCodes.ErrorInvalidDataType:
                    quality = StatusCodes.BadDataTypeIdUnknown;
                    error = Properties.Resources.ErrorInvalidDataType;
                    break;
                case BACnetErrorCodes.ErrorCovSubscriptionFailed:
                    quality = StatusCodes.BadConnectionRejected;
                    error = Properties.Resources.ErrorCovSubscriptionFailed;
                    break;
                case BACnetErrorCodes.ErrorBACnetTimeOut:
                    quality = StatusCodes.BadTimeout;
                    error = Properties.Resources.ErrorBACnetTimeOut;
                    break;
                case BACnetErrorCodes.ErrorWhoHasFailed:
                    quality = StatusCodes.BadDataUnavailable;
                    error = Properties.Resources.ErrorWhoHasFailed;
                    break;
                case BACnetErrorCodes.ErrorWhoHisFailed:
                    quality = StatusCodes.BadConnectionRejected;
                    error = Properties.Resources.ErrorWhoHisFailed;
                    break;
            }
        }

        #endregion


        #region Properties

        private bool _AllowOtherBACnetClients;

        // Setup driver to coexist with other BACnet clients        
        public bool AllowOtherBACnetClients
        {
            get { return _AllowOtherBACnetClients; }            
        }
        #endregion
    }
}
