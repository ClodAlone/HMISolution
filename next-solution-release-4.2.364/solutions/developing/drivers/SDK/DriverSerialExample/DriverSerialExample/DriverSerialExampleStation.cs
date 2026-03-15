////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverSerialExampleStation.cs
//
// summary:	Implements the driver serial example station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;

namespace DriverSerialExample
{
    /// <summary>   Communication target device. </summary>
    public class DriverSerialExampleStation : Station
    {
        
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the station object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">                The commdriver. </param>
        /// <param name="settings" type="DriverSerialExampleStationSettings">   Options for controlling
        ///                                                                     the operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleStation(CommunicationDriver commdriver, DriverSerialExampleStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
        }
       
        #endregion

        #region Abstract Methods
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from CommJobSettings. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="JobSettings">  . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as DriverSerialExampleCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new DriverSerialExampleCommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from Tag. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="defTag">   . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as DriverSerialExampleTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new DriverSerialExampleCommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new settings instance for protocol's task. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ///
        /// <returns>   The new job settings. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as DriverSerialExampleCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new DriverSerialExampleCommJobSettings(session, commJob);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new variable instance from TagDefinition. </summary>
        ///
        /// <param name="td">   . </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new DriverSerialExampleTag(td);
        }
        #endregion
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Processed received data in ExecutedJobArgs. To call asyncronously... </summary>
        ///
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            DriverSerialExampleCommJob mJ = e.Job as DriverSerialExampleCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                DriverSerialExampleProtocol P = new DriverSerialExampleProtocol();
                List<Tag> ChangedTags = new List<Tag>();
                if (P.ParseData(Answer, ref mJ, ref ChangedTags))
                    e.ChangedTags.AddRange(ChangedTags);
                else
                    e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
            }

            base.ProcessJobValues(e);
        }

        #region Properties

        /// <summary>   Identifier for the station. </summary>
        private uint _StationID;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Identifier for the station. </summary>
        ///
        /// <value> The identifier of the station. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint StationID
        {
            get { return _StationID; }
            set
            {
                _StationID = value;
            }
        }       
        
        #endregion

    }
}
