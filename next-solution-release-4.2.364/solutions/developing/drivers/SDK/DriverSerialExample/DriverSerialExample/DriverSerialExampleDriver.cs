////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverSerialExampleDriver.cs
//
// summary:	Implements the driver serial example driver class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
//using DriverSettingsInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace DriverSerialExample
{
    /// <summary>   Error codes of DriverSerialExample. </summary>
    public enum DriverSerialExampleErrorCodes
    {
        /// <summary>   An enum constant representing the error CRC error option. </summary>
        ErrorCRCError = 1000,
        /// <summary>   An enum constant representing the error unknown function code option. </summary>
        ErrorUnknownFunctionCode
    }

    /// <summary>   DriverSerialExample communication driver. </summary>
    public class DriverSerialExampleDriver : CommunicationDriver//, ICommunicationDriverWebEditing
    {
        /// <summary>   Default constructor. </summary>
        public DriverSerialExampleDriver()
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
                    var configuration = (from tag in new XPQuery<DriverSerialExampleDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as DriverSerialExampleDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra DriverSerialExample Driver Settings initializzation
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver save settings call. </summary>
        ///
        /// <param name="idl">  . </param>
        ///
        /// ### <returns>   . </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<DriverSerialExampleDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new DriverSerialExampleDriverSettings(ufw));
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

            var conf = settings as DriverSerialExampleDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra DriverSerialExample Driver Settings initializzation
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
            var conf = settings as DriverSerialExampleChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new DriverSerialExampleChannel(this, conf);
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
            var conf = settings as DriverSerialExampleStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new DriverSerialExampleStation(this, conf);
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
            return new DriverSerialExampleTag(tagtoAdd);
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
            return new DriverSerialExampleTag(tagtoAdd, byteoffset, bitoffset);
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
            var tagItem = tag as DriverSerialExampleTag;
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
                return new List<ChangeTag>((new XPQuery<DriverSerialExampleChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
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

            switch ((DriverSerialExampleErrorCodes)errorcode)
            {
                case DriverSerialExampleErrorCodes.ErrorCRCError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCRCError;
                    break;
                case DriverSerialExampleErrorCodes.ErrorUnknownFunctionCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknownFunctionCode;
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
        //public static DriverSerialExampleDynTagSettings DynSettings = new DriverSerialExampleDynTagSettings();
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   DynamicSettingsEditor ICommunicationDriverWebEditing interface. </summary>
        /////
        ///// <param name="dynamicSettings" type="string">    The dynamic settings. </param>
        /////
        ///// <returns>   An object. </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //object ICommunicationDriverWebEditing.DynamicSettingsEditor(string dynamicSettings)
        //{
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
        //    var dynsettings = value as DriverSerialExampleDynTagSettings;
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
    public class DriverSerialExampleChangeTag : ChangeTag
    {
        #region Constructors

        public DriverSerialExampleChangeTag(Session session, Tag tag)
            : base(session, tag)
        {
        }

        public DriverSerialExampleChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DriverSerialExampleChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }

}
