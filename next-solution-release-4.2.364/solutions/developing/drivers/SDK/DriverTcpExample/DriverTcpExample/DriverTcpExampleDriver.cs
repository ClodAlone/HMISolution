////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverTcpExampleDriver.cs
//
// summary:	Implements the driver TCP example driver class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
//using DriverSettingsInterfaces;

namespace DriverTcpExample
{
    /// <summary>   DriverTcpExample communication driver. </summary>
    public class DriverTcpExampleDriver : CommunicationDriver//, ICommunicationDriverWebEditing
    {
        /// <summary>   Default constructor. </summary>
        public DriverTcpExampleDriver()
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
                    var configuration = (from tag in new XPQuery<DriverTcpExampleDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as DriverTcpExampleDriverSettings;
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
                var configuration = (from tag in new XPQuery<DriverTcpExampleDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new DriverTcpExampleDriverSettings(ufw));
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

            var conf = settings as DriverTcpExampleDriverSettings;
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
            var conf = settings as DriverTcpExampleChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new DriverTcpExampleChannel(this, conf);
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
            var conf = settings as DriverTcpExampleStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new DriverTcpExampleStation(this, conf);
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
            return new DriverTcpExampleTag(tagtoAdd);
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
            return new DriverTcpExampleTag(tagtoAdd, byteoffset, bitoffset);
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
            var tagItem = tag as DriverTcpExampleTag;
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load Change Settings. </summary>
        ///
        /// <param name="idl">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<DriverTcpExampleChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
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

            switch ((DriverTcpExampleErrorCodes)errorcode)
            {
                case DriverTcpExampleErrorCodes.ErrorCRCError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCRCError;
                    break;
                case DriverTcpExampleErrorCodes.ErrorUnknownFunctionCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknownFunctionCode;
                    break;
                case DriverTcpExampleErrorCodes.ErrorReadError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadError;
                    break;
                case DriverTcpExampleErrorCodes.ErrorReceiveFrameError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReceiveFrameError;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolIllegalFunction:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalFunction;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolIllegalDataAddress:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalDataAddress;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolIllegalDataValue:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalDataValue;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolSlaveDeviceFailure:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolSlaveDeviceFailure;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolAcknowledge:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolAcknowledge;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolSlaveDeviceBusy:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolSlaveDeviceBusy;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolMemoryParityError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolMemoryParityError;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolGatewayPathUnavailable:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolGatewayPathUnavailable;
                    break;
                case DriverTcpExampleErrorCodes.ErrorProtocolGatewayTargetFailResponse:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolGatewayTargetFailResponse;
                    break;
            }
        }

        #endregion


        //#region ICommunicationDriverWebEditing Interface



        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   GeneralSettingsEditor ICommunicationDriverWebEditing property interface. </summary>
        /////
        ///// <value> The general settings editor. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //object ICommunicationDriverWebEditing.GeneralSettingsEditor
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>   The dynamic settings. </summary>
        //public static DriverTcpExampleDynTagSettings DynSettings;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   DynamicSettingsEditor ICommunicationDriverWebEditing interface. </summary>
        /////
        ///// <param name="dynamicSettings" type="string">    The dynamic settings. </param>
        /////
        ///// <returns>   An object. </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //object ICommunicationDriverWebEditing.DynamicSettingsEditor(string dynamicSettings)
        //{
        //    if (DynSettings == null)
        //        DynSettings = new DriverTcpExampleDynTagSettings();
        //    DynSettings.Parse(dynamicSettings);

        //    return DynSettings;
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   GetDynamicSettings ICommunicationDriverWebEditing interface. </summary>
        /////
        ///// <param name="value" type="object">  The value. </param>
        /////
        ///// <returns>   The dynamic settings. </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public string GetDynamicSettings(object value)
        //{
        //    var dynsettings = value as DriverTcpExampleDynTagSettings;
        //    if (dynsettings == null)
        //        return String.Empty;

        //    return dynsettings.ToString();
        //}


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   ImportTagsEditor ICommunicationDriverWebEditing property interface. </summary>
        /////
        ///// <value> The import tags editor. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public object ImportTagsEditor
        //{
        //    get { return null; }
        //}

        //#endregion
    }
    public class DriverTcpExampleChangeTag : ChangeTag
    {
        #region Constructors

        public DriverTcpExampleChangeTag(Session session, Tag tag)
            : base(session, tag)
        {
        }

        public DriverTcpExampleChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DriverTcpExampleChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion
    }
}
