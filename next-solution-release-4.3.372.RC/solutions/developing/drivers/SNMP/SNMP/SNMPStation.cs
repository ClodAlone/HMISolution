using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;


namespace SNMP
{
    class SNMPStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public SNMPStation(CommunicationDriver commdriver, SNMPStationSettings settings)
            : base(commdriver, settings)
        {
            //_DestinationNetworkAddress = settings.DestinationNetworkAddress;
            //_DestinationNode = settings.DestinationNode;
            //_DestinationUnit = settings.DestinationUnit;
            //_SID = 0;
        }

        #endregion

        #region Abstract Methods

        public override bool Startup()
        {
            // Call the base class method
            if (!base.Startup())
            {
                return false;
            }

            // Build the cannel dictionary that associates OIDs and Jobs
            ThreadPool.QueueUserWorkItem(o =>
            {
                if (Channel != null)
                {
                    SNMPChannel snmpChannel = (SNMPChannel)Channel;
                    lock (lockListObject)
                    {
                        foreach (var job in ListWholeJob)
                        {
                            SNMPCommJob snmpJob = (SNMPCommJob)job;
                            if(snmpJob.IsValid && ((snmpJob.Type == LinkType.InputOutput) || (snmpJob.Type == LinkType.Input)))
                            {
                                snmpChannel.AddToMapOIDJobs(snmpJob.snmpOid_Address, snmpJob);
                            }
                        }
                    }
                }
            });

            return true;
        }

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as SNMPCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new SNMPCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as SNMPTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new SNMPCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new SNMPTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as SNMPCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new SNMPCommJobSettings(session, commJob);
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            SNMPCommJob sJ = e.Job as SNMPCommJob;
            if (sJ == null)
            {
                return;
            }

            // Analyze answer if no error occurred before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    DriverErrorCodes parsingError = SNMPProtocol.ParseData(Answer, ref sJ, ref ChangedTags);
                    if (parsingError == DriverErrorCodes.ErrorNoError)
                    {
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                            {
                                e.ChangedTags.Add(j);
                            }
                        }
                    }
                    else
                    {

                        //{
                        //    String curTimeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
                        //    String DbgTxt = String.Format("SNMP DBG - {0} parsingError = {1}", curTimeTxt, parsingError);
                        //    System.Diagnostics.Trace.TraceInformation(DbgTxt);
                        //}

                        e.ErrorCode = parsingError;
                    }
                }
            }

            // Put all the jobs in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
            sJ.ExecuteTask = false;
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            SNMPCommJob mj = job as SNMPCommJob;

            SNMPProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #endregion

        #region Methods
        //public bool CheckSID(byte inSID)
        //{
        //    return inSID ==_SID;
        //}
       
        #endregion



        private static int CompareTagByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                //if (y == null)
                //    return 1; //x > y
                //else
                //{
                //    SNMPDynTagSettings dts = new SNMPDynTagSettings();
                //    dts.TryParse(x.TagNode.DynamicSettings);
                //    string cx = string.Format("DS{0}TL{1}MI{2}DCT{3}Addr{4}",
                //        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                //        dts.MethodID.ToString("000"), dts.DataConversionType, dts.SNMPAddress());
                //    dts.TryParse(y.TagNode.DynamicSettings);
                //    string cy = string.Format("DS{0}TL{1}MI{2}DCT{3}Addr{4}",
                //        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                //        dts.MethodID.ToString("000"), dts.DataConversionType, dts.SNMPAddress());
                //    return cx.CompareTo(cy);
                //}
                return 1; //x > y
            }
        }

        #region Properties

        //private byte _DestinationNetworkAddress;
        //public byte DestinationNetworkAddress
        //{
        //    get
        //    {
        //        return _DestinationNetworkAddress;
        //    }
        //    set
        //    {
        //        _DestinationNetworkAddress = value;
        //    }
        //}

        //private byte _DestinationNode;
        //public byte DestinationNode
        //{
        //    get
        //    {
        //        return _DestinationNode;
        //    }
        //    set
        //    {
        //        _DestinationNode = value;
        //    }
        //}

        //private byte _DestinationUnit;
        //public byte DestinationUnit
        //{
        //    get
        //    {
        //        return _DestinationUnit;
        //    }
        //    set
        //    {
        //        _DestinationUnit = value;
        //    }
        //}

        //private byte _SID;
        //public byte SID
        //{
        //    get
        //    {
        //        return ++_SID;
        //    }
        //}
        
        #endregion

    }
}
