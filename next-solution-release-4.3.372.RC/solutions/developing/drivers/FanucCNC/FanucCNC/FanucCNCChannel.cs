using System;
using System.Collections.Generic;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using FanucCNC.Focas_Library;
using System.Text;

namespace FanucCNC
{
    public class FanucCNCChannel : Channel, IDisposable
    { 
        #region Constructors

        /// <summary>
        /// Initializes the EtherNetIPChannel object.
        /// </summary>
        public FanucCNCChannel(CommunicationDriver commdriver, FanucCNCChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _DeviceAddress = settings.DeviceAddress;
            _DevicePort = settings.DevicePort;
            _MachineSerie = settings.MachineSerie;
        }
        #endregion

        #region Members        
        private ushort _FocasHandle;
        private short _SetGetCNCPath = -1; // -1 = undefined path        
        private short _CurrentCNCPath = -1; // -1 = undefined path

        #endregion

        #region Override Methods

        // Not used
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override uint GetBytesToRead() { return 1; }
        // Not used
        public override uint GetBytesToWrite() { return 1; }

        List<FanucCNCCommJob> nextlist = new List<FanucCNCCommJob>();
        List<CommJob> ListJobExec = new List<CommJob>();
        protected override void WorkingThread(object data)
        {            
            if (CommDriver.GetChannelStations(this).Count == 0)
                return;

            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;

            int loop = 0;
            while (true)
            {                
                nextlist.Clear();
                if (ListJobPending.Count == 0 || MultiPointProtocol) {
                    
                    ScheduleListJob();
                    lock (lockThreadObject)
                    {
                        if (SynchroJob != null)
                            GetNextSynchroJob(ref nextlist);
                        else
                            GetNextPendingList(ref nextlist);
                    }
                    if (nextlist.Count > 0)
                        ListJobPending.AddRange(nextlist);
                }

                if (nextlist.Count > 0)
                {
                    if (!IsDeviceOpen())
                    {
                        DeviceClose();
                        DeviceOpen();
                    }
                    lock (lockThreadObject)
                    {
                        //if (IsDeviceOpen())
                        //{
                            DriverErrorCodes ret = ExecuteJobList(nextlist);
                            if (ret == (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConnectionBroken)
                            {
                                DeviceClose();
                            }
                        //}                        
                    }
                }                

                lock (lockThreadObject)
                {
                    if (ListJobPending.Count > 0)
                    {
                        ListJobExec.Clear();
                        foreach (var job in ListJobPending)
                        {
                            ListJobExecuted.Add(job);
                        }
                        foreach (var job in ListJobExecuted)
                        {
                            job.LastExecutionTime = DateTime.UtcNow;
                            if (SynchroJob != null && SynchroJob == job)
                            {
                                job.ResetSynchro.WaitOne(Timeout);
                                SynchroJob = null;
                                job.ResetSynchro.Reset();
                            }
                            ListJobPending.Remove(job);
                        }
                        ListJobExecuted.Clear();
                    }
                }

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                    DeviceClose();

                if (StopWorkerThread.WaitOne(sleepCycle))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }
            }
        }

        public override bool TestChannelComm()
        {
            return DeviceOpen();
        }                        

        public override bool IsDeviceOpen()
        {
            return (_FocasHandle != 0);
        }

        private bool _IsLastConnectionInError = false;
        public override bool DeviceOpen()
        {                       
            int res_handle = Focas1.cnc_allclibhndl3(DeviceAddress, (ushort)DevicePort, TimeOutInSecond, out _FocasHandle);
            if (res_handle == Focas1.EW_OK)
            {
                CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.DeviceConnected, this.Name), EventSeverity.High);

                // first time, get path from device
                if (_SetGetCNCPath == -1)
                    res_handle = Focas1.cnc_getpath(_FocasHandle, out _SetGetCNCPath, out short dummy);
                else // else set the last one 
                    res_handle = Focas1.cnc_setpath(_FocasHandle, _SetGetCNCPath);

                if (res_handle == Focas1.EW_OK)
                    _CurrentCNCPath = _SetGetCNCPath;

                //Focas1.ODBSYS sysinfo = new Focas1.ODBSYS();
                //res_handle = Focas1.cnc_sysinfo(_FocasHandle, sysinfo);
                //if (res_handle == Focas1.EW_OK)
                //{
                //    int o = 3;
                //}
            }
            else
            {
                if (!_IsLastConnectionInError)
                    CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.ErrorDeviceConnection, this.Name), Opc.Ua.EventSeverity.High);

                _FocasHandle = 0;
            }

            #region update variable state
            _IsLastConnectionInError = (_FocasHandle == 0);            
            SetStateCommandVariableBit(_IsLastConnectionInError, (UInt16)ChannelVariableBits.ChannelUnconnected);
            foreach (var station in CommDriver.GetChannelStations(this))
                ((FanucCNCStation)station).SetStateCommandVariableBit(_IsLastConnectionInError, (UInt16)StationVariableBits.StationErrorState);
            #endregion
            
            return (IsDeviceOpen());
        }
       
        public override bool DeviceClose()
        {
            if (_FocasHandle != 0)
            {
                CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.DeviceDisconnected, this.Name), Opc.Ua.EventSeverity.High);

                Focas1.cnc_freelibhndl(_FocasHandle);
                _FocasHandle = 0;
            }            
            return true;
        }

        //protected override void OnJobExecuted(ExecutedJobArgs e)
        //{
        //    if (StatisticsData != null)
        //    {
        //        // set exchanged quantity of data for specific job
        //        DiagnLastTaskRxBytes = ((FanucCNCCommJob)e.Job).DiagnRxBytes;
        //        DiagnLastTaskTxBytes = ((FanucCNCCommJob)e.Job).DiagnTxBytes;
        //        ((FanucCNCCommJob)e.Job).ResetDiagnRxTxBytes();
        //    }
        //    base.OnJobExecuted(e);
        //}
        #endregion

        #region methods

        protected void GetNextPendingList(ref List<FanucCNCCommJob> list)
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();
                if (queue != null)
                {
                    for (int i = queue.Count; i > 0; i--)
                    {
                        CommJob job = null;
                        if (!queue.TryDequeue(out job))
                        {
                            break;
                        }
                        if (job.IsPending == false)
                        {
                            list.Add((FanucCNCCommJob)job);
                            break;
                        }
                        else
                        {
                            queue.Enqueue(job);
                        }
                    }
                }
            }            
        }

        private void GetNextSynchroJob(ref List<FanucCNCCommJob> list)
        {
            list.Add(SynchroJob as FanucCNCCommJob);
        }

        /// <summary>
        /// Using Focas Library result, check if error result is a connection error --> if is, force driver disconnection
        /// </summary>
        /// <param name="retJob"></param>
        /// <param name="cnc_ret"></param>
        /// <returns></returns>
        private DriverErrorCodes CheckFatalError(DriverErrorCodes retJob, short cnc_ret)
        {
            if (retJob != DriverErrorCodes.ErrorNoError)
            {
                // check connection error
                if (cnc_ret == (short)Focas1.focas_ret.EW_SOCKET || cnc_ret == (short)Focas1.focas_ret.EW_HANDLE)
                    retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConnectionBroken;
            }

            return (DriverErrorCodes)retJob;
        }
        
        private DriverErrorCodes ReadData(List<FanucCNCCommJob> list)
        {
            FanucCNCCommJob j = list[0];
            //long diagnRxBytes = 0;
            short cnc_ret = Focas1.EW_OK;
            DriverErrorCodes retJob = DriverErrorCodes.ErrorNoError;
            byte[] dataJob = null;


            ExecuteJob(list[0]);

            if (j.IsCNCPathChangeRequired(_SetGetCNCPath)) {
                cnc_ret = Focas1.cnc_setpath(_FocasHandle, j.CNCPath);
                if (cnc_ret != Focas1.EW_OK) {
                    ExecutedJobArgs eJobError = new ExecutedJobArgs();
                    eJobError.Job = j;
                    eJobError.Values = null;                    
                    eJobError.ErrorCode = CheckFatalError((DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorChangingCNCPath, cnc_ret);
                    OnJobExecuted(eJobError);

                    return eJobError.ErrorCode;
                }
                else
                {
                    _CurrentCNCPath = j.CNCPath;
                }
            }

            switch (j.FunctionCode)
            {
                #region PMC Function
                case FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng:
                    //Reads the PMC data of the specified PMC address / range.
                    //This function is used to exchange the data between the application on MMC function and LADDER software on PMC
                    //Legge un'area "generica di dati dal CNC" --> memoria condivisa come il PLC
                    //1 solo valore uscita
                    {                        
                        FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng param = j.FunctionSettings as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng;

                        param.PrepareReadRequest(out ushort addressNumber, out ushort addressENumber, out Focas1.IODBPMCXX cnc_struct);
                        cnc_ret = Focas1.pmc_rdpmcrng(_FocasHandle, param.AddressType, param.RDataType, addressNumber, addressENumber, param.Size, cnc_struct);
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //diagnRxBytes += j.TotalJobSize;
                            retJob = param.ParseReadData(j.TagsList, cnc_struct, out dataJob);
                        }
                        else
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                    }
                    break;

                #endregion

                #region CNC Function
                case FanucCNCProtocol.FunctionCode.Func_cnc_actf:
                    //Reads the actual feed rate of the controlled axes of CNC --> presente nel codice ma non richiamata
                    //--> legge 1 valore alla volta
                    {
                        FanucCNCDynTag_cnc_actf param = j.FunctionSettings as FanucCNCDynTag_cnc_actf;

                        Focas1.ODBACT cnc_struct = new Focas1.ODBACT();
                        cnc_ret = Focas1.cnc_actf(_FocasHandle, cnc_struct);
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //diagnRxBytes += j.TotalJobSize;
                            retJob = param.ParseReadData(cnc_struct, out dataJob);
                        }
                        else
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                    }
                    break;


                case FanucCNCProtocol.FunctionCode.Func_cnc_absolute:
                    //Read absolute axis position --> presente nel codice ma non richiamata

                    //setta il path se e' diverso da quello corrente
                    //  In: asse da leggere (o l'array di tutti gli assi)
                    //  Out : array con valori assi
                    //GetInfoPath(IndirizzoIpTavola, PortNumberTavola, CNC_TimeOut, 1);
                    {
                        FanucCNCDynTag_cnc_absolute param = j.FunctionSettings as FanucCNCDynTag_cnc_absolute;                        
                        retJob = param.PrepareReadRequest(out Focas1.ODBAXIS cnc_struct);
                        if (retJob == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_absolute(_FocasHandle, param.AxisNr, param.Size, cnc_struct);
                            if (cnc_ret == Focas1.EW_OK)
                            {
                                //diagnRxBytes += j.TotalJobSize;
                                retJob = param.ParseReadData(cnc_struct, out dataJob);                                
                            }
                            else
                            {
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdaxisdata:
                    //Read various data relating servo axis or spindle axis 
                    {
                        //Focas1.cnc_setpath(_FocasHandle, Convert.ToInt16(1));
                        Focas1.ODBAXDT cnc_struct = new Focas1.ODBAXDT();
                        
                        FanucCNCDynTag_cnc_rdaxisdata param = j.FunctionSettings as FanucCNCDynTag_cnc_rdaxisdata;                                                
                        retJob = param.PrepareReadRequest(out short CNC_Len);
                        if (retJob == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_rdaxisdata(_FocasHandle, param.Class, param.TypeData, param.Size, ref CNC_Len, cnc_struct);
                            if (cnc_ret == Focas1.EW_OK) {
                                //diagnRxBytes += j.TotalJobSize;
                                retJob = param.ParseReadData(cnc_struct, CNC_Len, out dataJob);
                            }
                            else
                            {
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }
                    }
                    break;
                
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdspeed:
                    //Reads the actual feed rate and the actual rotational speed of the spindle. 

                    //legge insieme allo stato della macchina
                    //  In: indice (fisso -1)
                    //  Out : valore letto
                    //Usato un solo dato
                    //public int data;       /* speed data */
                    //public short dec;        /* decimal position */
                    //public short unit;       /* data unit */
                    //public short disp;       /* display flag */
                    //public byte name;       /* name of data */
                    //public byte suff;       /* suffix */
                    {
                        FanucCNCDynTag_cnc_rdspeed param = j.FunctionSettings as FanucCNCDynTag_cnc_rdspeed;

                        Focas1.ODBSPEED cnc_struct = new Focas1.ODBSPEED();
                        // read always all data
                        cnc_ret = Focas1.cnc_rdspeed(_FocasHandle, param.NrData, cnc_struct);
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //FeedRate = string.Format("{0:0.000}", ((float)speed.actf.data / (float)1000));
                            //Spindle = string.Format("{0:0.000}", ((float)speed.acts.data / (float)1000));
                            //diagnRxBytes += j.TotalJobSize;
                            retJob = param.ParseReadData(cnc_struct, out dataJob);
                            if (retJob == DriverErrorCodes.ErrorNoError)
                            {
                            }
                        }
                        else
                        {
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdalmmsg2:
                    {
                        //GetInfoPath(IpAddress, PortNumber, TimeOut, Path);
                        FanucCNCDynTag_cnc_rdalmmsg2 param = j.FunctionSettings as FanucCNCDynTag_cnc_rdalmmsg2;                        
                        retJob= param.PrepareReadRequest(out Focas1.ODBALMMSG2_data_custom[] cnc_struct, out short CNC_NumAlm);
                        if (retJob == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_rdalmmsg2_custom(_FocasHandle, param.AlarmType, ref CNC_NumAlm, cnc_struct);
                            if (cnc_ret == Focas1.EW_OK)
                            {
                                //diagnRxBytes += j.TotalJobSize;
                                retJob = param.ParseReadData(cnc_struct, CNC_NumAlm, out dataJob);
                                if (retJob == DriverErrorCodes.ErrorNoError)
                                {
                                    //path_scop = Uni.NumeroStazione.ToString() + "." + Uni.Posizione.ToString();
                                    //Text = "Station " + Alarm[i].path_scop + " - " + Alarm[i].CNC_OStruct_ReadAlmmMessage[j].alm_no + " - " + Alarm[i].CNC_OStruct_ReadAlmmMessage[j].alm_msg,
                                }
                                //CNC_OStruct_ReadAlmmMessage[param.AlarmID]
                            }
                            else
                            {
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdopmsg3:
                    {
                        //GetInfoMaster(IpAddress, PortNumber, TimeOut);
                        FanucCNCDynTag_cnc_rdopmsg3 param = j.FunctionSettings as FanucCNCDynTag_cnc_rdopmsg3;
                        retJob = param.PrepareReadRequest(out Focas1.OPMSG3_data_custom[] cnc_struct, out short CNC_NumMsg);
                        if (retJob == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_rdopmsg3_custom(_FocasHandle, param.OperatorType, ref CNC_NumMsg, cnc_struct);
                            if (cnc_ret == Focas1.EW_OK)
                            {
                                //diagnRxBytes += j.TotalJobSize;
                                retJob = param.ParseReadData(cnc_struct, CNC_NumMsg, out dataJob);
                                if (retJob == DriverErrorCodes.ErrorNoError)
                                {
                                    //FanucCNCDynTag_cnc_rdopmsg3 param = j.FunctionSettings as FanucCNCDynTag_cnc_rdopmsg3;
                                    //CNC_OStruct_ReadAlmmMessage[param.AlarmID]
                                }
                            }
                            else
                            {
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdactpt:
                    {
                        //GetInfoPath(IpAddress, PortNumber, TimeOut, Path);
                        FanucCNCDynTag_cnc_pdf_rdactpt param = j.FunctionSettings as FanucCNCDynTag_cnc_pdf_rdactpt;
                        byte[] CNC_ProgramName = new byte[1024];

                        cnc_ret = Focas1.cnc_pdf_rdactpt(_FocasHandle, CNC_ProgramName, out int CNC_PosizioneAttuale);
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //diagnRxBytes += j.TotalJobSize;
                            retJob = param.ParseReadData(CNC_ProgramName, CNC_PosizioneAttuale, out dataJob);
                            if (retJob == DriverErrorCodes.ErrorNoError)
                            {
                                //CNC_OStruct_ReadAlmmMessage[param.AlarmID]
                            }
                        }
                        else
                        {
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdmain:
                    {
                        FanucCNCDynTag_cnc_pdf_rdmain param = j.FunctionSettings as FanucCNCDynTag_cnc_pdf_rdmain;
                        param.PrepareReadRequest(out byte[] vs);
                        cnc_ret = Focas1.cnc_pdf_rdmain(_FocasHandle, vs);
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            retJob = param.ParseReadData(vs, out dataJob);
                        }
                        else
                        {
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdzofs_cnc_wrzofs:
                    //Reads the work zero offset value specified by "number", "axis". 
                    //presente nel codice ma non richiamata
                    //  In: indice asse da leggere (o l'array di tutti gli assi)
                    //  Out : array con valori assi

                    //torna un array con i dati di tutti gli assi --> usato solo X valore
                    {
                        //setta il path se e' diverso da quello corrente
                        //GetInfoPath(IpAddress, PortNumber, TimeOut, Path);                        

                        FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs param = j.FunctionSettings as FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs;
                        Focas1.IODBZOFS CNC_OStruct = new Focas1.IODBZOFS();                        
                        cnc_ret = Focas1.cnc_rdzofs(_FocasHandle, param.DataNo, (short)param.AxisNr, param.Size, CNC_OStruct);
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //diagnRxBytes += j.TotalJobSize;
                            retJob = param.ParseReadData(CNC_OStruct, out dataJob);
                            if (retJob == DriverErrorCodes.ErrorNoError)
                            {
                            }
                        }
                        else
                        {
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdtofsr_cnc_wrtofsr:
                    //Reads the tool offset value specified by "s_number","e_number","type".The offset value is stored in "IODBTO" with signed binary format.
                    //per ogni lettura legge un valore specifico  --> tutti / 1000
                    //Usata per leggere i tools dal CNC e salvarli su file
                    //  In: 3 parametro + elemento da leggere
                    //  Out : valore letto
                    {
                        FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr param = j.FunctionSettings as FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr;
                        Focas1.IODBTO_1_1 CNC_Ostruct = new Focas1.IODBTO_1_1();
                        cnc_ret = Focas1.cnc_rdtofsr(_FocasHandle, param.S_Number, (short)param.OffsetType, param.E_Number, param.Size, CNC_Ostruct);
                        switch (cnc_ret)
                        {
                            case Focas1.EW_OK:
                                //diagnRxBytes += j.TotalJobSize;
                                retJob = param.ParseReadData(CNC_Ostruct, out dataJob);
                                break;
                            case (short)Focas1.focas_ret.EW_ATTRIB:
                                //diagnRxBytes += j.TotalJobSize;
                                retJob = param.SetDefaultValueNotExistingVariable(out dataJob);
                                break;
                            default:
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                                break;
                        }
                    }
                    break;
                
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdparam_cnc_wrparam:
                    //Writes the parameter specified by "datano","type"(only for the parameter with axis).The data format depends on each parameter. The format of Byte/ Word / 2 - Word parameter is generally signed binary.
                    //Usate per scrivere dei parametri di configutazione della macchina (usato a volte subito dopo una cnc_rdparam)
                    //In : array struttura da scrivere
                    //out
                    //usata struttura --> scrive X valore                    
                    //public short datano;    /* data number */
                    //public short type;      /* axis number */
                    //public IODBPSD_U u = new IODBPSD_U();
                    {
                        FanucCNCDynTag_cnc_rdparam_cnc_wrparam param = j.FunctionSettings as FanucCNCDynTag_cnc_rdparam_cnc_wrparam;
                        Focas1.IODBPSD cnc_struct = new Focas1.IODBPSD();
                        cnc_ret = Focas1.cnc_rdparam(_FocasHandle, param.PNumber, param.AxisNr, param.Size, cnc_struct);
                        switch (cnc_ret)
                        {
                            case Focas1.EW_OK:
                                //diagnRxBytes += j.TotalJobSize;
                                retJob = param.ParseReadData(cnc_struct, out dataJob);
                                break;
                            case (short)Focas1.focas_ret.EW_ATTRIB:
                                //retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorBadWaitingForInitialData;
                                //j.SetQuality((uint)StatusCodes.BadWaitingForInitialData);
                                //diagnRxBytes += j.TotalJobSize;
                                retJob = param.SetDefaultValueNotExistingVariable(out dataJob);
                                break;
                            default:
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                                break;
                        }
                    }                
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_statinfo:
                    //Reads the status information of CNC. The various information is stored in each member of "ODBST".
                    //Usato per avere lo stato del CNC prima di svolgere certe operazioni (leggi/scrivi programmi, ecc)                    
                    {
                        FanucCNCDynTag_cnc_statinfo param = j.FunctionSettings as FanucCNCDynTag_cnc_statinfo;
                        Focas1.ODBST cnc_struct = new Focas1.ODBST();
                        cnc_ret = Focas1.cnc_statinfo(_FocasHandle, cnc_struct);                        
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //diagnRxBytes += j.TotalJobSize;
                            retJob = param.ParseReadData(cnc_struct, out dataJob);
                            if (retJob == DriverErrorCodes.ErrorNoError)
                            {

                            }                            
                        }
                        else
                        {
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                        }
                    }
                    break;
                    
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdsvmeter:
                    //Reads the servo load meter data from 1st axis to the specified axis number.
                    //usata per leggere dei servo motori su tutti gli assi disponibili
                    //usata struttura ma e letti tutti i valori
                    //In : nr assi da leggere
                    //out : array o indice asse
                    //public struct LOADELM_custom
                    //{
                    //    public int data;       /* load meter */
                    //    public short dec;        /* decimal position */
                    //    public short unit;       /* unit */
                    //    public byte name;       /* name of data */
                    //    public byte suff1;      /* suffix */
                    //    public byte suff2;      /* suffix */
                    //    public byte reserve;    /* reserve */
                    //}
                    {
                        //  GetInfoPath(IpAddress, PortNumber, TimeOut, Path);
                        FanucCNCDynTag_cnc_rdsvmeter param = j.FunctionSettings as FanucCNCDynTag_cnc_rdsvmeter;
                        param.PrepareReadRequest(out Focas1.LOADELM_custom[] cnc_struct, out short Num_Axis);
                        cnc_ret = Focas1.cnc_rdsvmeter_custom(_FocasHandle, ref Num_Axis, cnc_struct);
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //diagnRxBytes += j.TotalJobSize;
                            retJob = param.ParseReadData(cnc_struct, Num_Axis, out dataJob);
                            if (retJob == DriverErrorCodes.ErrorNoError)
                            {

                            }
                        }
                        else
                        {
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_getpath_cnc_setpath:
                    {
                        //Reads the current selected path number which is the target path of the Data window functions.
                        //Non e' chiaro il suo uso
                        //1 solo valore
                        FanucCNCDynTag_cnc_getpath_cnc_setpath param = j.FunctionSettings as FanucCNCDynTag_cnc_getpath_cnc_setpath;

                        cnc_ret = Focas1.cnc_getpath(_FocasHandle, out short CNC_Path, out short CNC_MaxPathNumber);
                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //diagnRxBytes += j.TotalJobSize;
                            retJob = param.ParseReadData(CNC_Path, out dataJob);
                            if (retJob == DriverErrorCodes.ErrorNoError)
                            {
                                _CurrentCNCPath = CNC_Path;
                                _SetGetCNCPath = CNC_Path;
                            }
                        }
                        else
                        {
                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                        }
                    }
                    break;

                #endregion

                #region File management function
                case FanucCNCProtocol.FunctionCode.Func_Custom_UploadProgramFromCnc:
                    {
                        //GetInfoPath(IpAddress, PortNumber, TimeOut, Path);
                        FanucCNCDynTag_Custom_UploadProgramFromCnc param = j.FunctionSettings as FanucCNCDynTag_Custom_UploadProgramFromCnc;
                        DriverErrorCodes retUp = param.PrepareReadRequest(_CurrentCNCPath, out string cncProgramPathAndName);
                        if (retUp == DriverErrorCodes.ErrorNoError)
                        {
                            byte[] program = new byte[0];
                            bool endOfUpload = false;
                            cnc_ret = Focas1.cnc_upstart4(_FocasHandle, (short)(Focas1.DownloadData.NCProgram), cncProgramPathAndName);
                            if (cnc_ret == Focas1.EW_OK)
                            {
                                while (!endOfUpload)
                                {
                                    int readBufferSize = 1024;
                                    byte[] readBuffer = new byte[readBufferSize];
                                    cnc_ret = Focas1.cnc_upload4(_FocasHandle, ref readBufferSize, readBuffer);
                                    switch (cnc_ret)
                                    {
                                        case (short)Focas1.focas_ret.EW_BUFFER:
                                            continue;
                                        case (short)Focas1.EW_OK:
                                            if (readBufferSize > 0)
                                            {
                                                //diagnRxBytes += readBufferSize;
                                                int currenSize = program.Length;
                                                Array.Resize(ref program, program.Length + readBufferSize);
                                                Array.Copy(readBuffer, 0, program, currenSize, readBufferSize);
                                                if (program[program.Length - 1] == FanucCNCProtocol.CNC_PROGRAM_CHAR_START_END)
                                                    endOfUpload = true;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                            break;
                                        // unmanaged error
                                        default:
                                            endOfUpload = true;
                                            break;
                                    }
                                }
                            }
                            Focas1.cnc_upend4(_FocasHandle);

#if DEBUG
                            if (dataJob != null)
                            {
                                string programText = ASCIIEncoding.ASCII.GetString(dataJob);
                            }
#endif

                            if (cnc_ret == Focas1.EW_OK)
                                retUp = param.ParseReadData(program, out dataJob);
                            else
                                retUp = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;

                        }
                        // file manegement job always return ErrorNoError (retJob) --> error is reported only into conditional variable
                        if (retUp != DriverErrorCodes.ErrorNoError)
                        {
                            CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.Error_UploadProgramFromCnc, cncProgramPathAndName, string.Format("{0} - {1}", cnc_ret, Focas1.GetFocasErrorDescription(cnc_ret))), EventSeverity.High);

                            j.SetConditionalVariableBit(true, (int)FanucCNCProtocol.ConditionalVariableState.ERROR_DURING_WRITE);
                        }
                    }
                    #endregion
                    break;
            }

            //if (StatisticsData != null)
            //    // increase receive bytes diagnostic var
            //    j.DiagnRxBytes += diagnRxBytes;

            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Job = j;
            eJob.Values = dataJob;            
            eJob.ErrorCode = CheckFatalError(retJob, cnc_ret);
            OnJobExecuted(eJob);

            // restore "current" path
            if (_CurrentCNCPath != _SetGetCNCPath)
            {
                cnc_ret = Focas1.cnc_setpath(_FocasHandle, _SetGetCNCPath);
                _CurrentCNCPath = _SetGetCNCPath;
            }

            return eJob.ErrorCode;
        }

        private DriverErrorCodes WriteData(List<FanucCNCCommJob> list)
        {
            DriverErrorCodes retJob = DriverErrorCodes.ErrorNoError;
            //long diagnTxBytes = 0;
            short cnc_ret = Focas1.EW_OK;

            ExecuteJob(list[0]);

            FanucCNCCommJob j = list[0];

            object objectData = null;
            byte[] jobdata = null;

            // for a job not access to PMC area (Plc area) data, estract data to write as a single array of byte
            if (j.FunctionCode != FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng) {
                j.GetJobData(ref objectData);
                if (j.TagsListOnWriting.Count == 0)
                    return retJob;
                jobdata = (byte[])objectData;
            }

            if (j.IsCNCPathChangeRequired(_SetGetCNCPath))
            {
                cnc_ret = Focas1.cnc_setpath(_FocasHandle, j.CNCPath);
                if (cnc_ret != Focas1.EW_OK)
                {
                    ExecutedJobArgs eJobError = new ExecutedJobArgs();
                    eJobError.Job = j;
                    eJobError.Values = null;                    
                    eJobError.ErrorCode = CheckFatalError(retJob, cnc_ret);
                    OnJobExecuted(eJobError);

                    return eJobError.ErrorCode;
                }
                else
                {
                    _CurrentCNCPath = j.CNCPath;
                }
            }

            switch (j.FunctionCode)
            {                
                case FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng:
                    {
                        //Writes the PMC data of the specified PMC address / range.
                        //This function is used to exchange the data between the application on MMC function and LADDER software on PMC.
                        //Scrive un'area "generica di dati dal CNC" --> memoria condivisa come il PLC                       
                        FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng param = j.FunctionSettings as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng;

                        lock (j.retLockList())
                        {
                            int writeDataIndex = 0;
                            int writtenData = 0;
                            
                            while (j.TagsListToWrite.Count > 0 && writeDataIndex < j.TagsListToWrite.Count) {

                                cnc_ret = Focas1.EW_OK;

                                jobdata = null;
                                j.GetJobData(ref objectData);
                                if (objectData != null)
                                    jobdata = (byte[])objectData;
                                
                                // any data to write ?
                                if (jobdata != null)
                                {
                                    // before write bit value, read value from CNC's device and mask with Movicon's data                                
                                    if ((uint)j.TagsListOnWriting[writeDataIndex].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean) {
                                        retJob = param.PrepareReadRequestBeforeWriteBit(j.IsStructOrAggregateJob(), j.TagsListOnWriting[writeDataIndex], out ushort addressNumber, out ushort addressENumber, out Focas1.IODBPMCXX cnc_struct, out ushort size);
                                        if (retJob == DriverErrorCodes.ErrorNoError)
                                        {
                                            cnc_ret = Focas1.pmc_rdpmcrng(_FocasHandle, param.AddressType, param.RDataType, addressNumber, addressENumber, size, cnc_struct);
                                            if (cnc_ret == Focas1.EW_OK)
                                            {
                                                //diagnTxBytes += j.TotalJobSize;
                                                // mask device's data with data to write
                                                param.MaskValueBeforeWriteBit(j.TagsListOnWriting[writeDataIndex], cnc_struct, ref jobdata);
                                            }
                                            else
                                            {
                                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                                            }
                                        }
                                    }

                                    if (retJob == DriverErrorCodes.ErrorNoError)
                                    {
                                        param.PrepareWriteRequest(j.IsStructOrAggregateJob(), j.TagsListOnWriting[writeDataIndex], jobdata, out Focas1.IODBPMCXX PMC_Struct, out ushort size);
                                        cnc_ret = Focas1.pmc_wrpmcrng(_FocasHandle, size, PMC_Struct);
                                        if (cnc_ret != Focas1.EW_OK)
                                        //{
                                        //    //diagnTxBytes += j.TotalJobSize;
                                        //}
                                        //else
                                        {
                                            retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                                        }
                                    }
                                }

                                // data was written, so remove from pending 
                                if (cnc_ret == Focas1.EW_OK)
                                {
                                    j.TagsListOnWriting.RemoveAt(writeDataIndex);
                                    // increase good write counter
                                    writtenData++;
                                }
                                else
                                {
                                    // error during write --> move to next tag (index of TagsListOnWriting)
                                    writeDataIndex++;
                                }

                                // still pending writing tag --> some error occours
                                if (writeDataIndex > 0)
                                {
                                    j.TagsListToWrite.AddRange(j.TagsListOnWriting);
                                    j.TagsListOnWriting.Clear();

                                    // final job result is bad
                                    retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                                }
                            }
                        }
                    }
                    break;

                #region CNC Data                                
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdtofsr_cnc_wrtofsr:
                    //Writes the tool offset value specified by "datano_s","datano_e","type".The offset value must be stored in "IODBTO" with signed binary format.
                    //Usata per scrivere i parametri dei tools sul CNC letti da file
                    //  In: parametri struttura + elemento da scrivere
                    //  Out:
                    //usata una struttura --> servono tutti i parametri
                    //public short datano_s;  /* start offset number */
                    //public short type;      /* offset type */
                    //public short datano_e;  /* end offset number */
                    //public OFS_1 ofs = new OFS_1();
                    {
                        //Focas1.BuildUnitàViewModel Unità_passaggio = new BuildUnitàViewModel();//Creo Unità
                        //Unità_passaggio.LeggiFileUnità(FileUnità[p]);
                        //CNC_Ostruct.ofs.t_ofs[0] = Convert.ToInt32(Convert.ToDouble(tool1_comp_r) * 1000);
                        FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr param = j.FunctionSettings as FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr;
                        retJob = param.PrepareWriteRequest(jobdata, out Focas1.IODBTO_1_1 cnc_struct);
                        if (retJob == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_wrtofsr(_FocasHandle, param.Size, cnc_struct);
                            if (cnc_ret != Focas1.EW_OK)
                            //{
                            //    diagnTxBytes += j.TotalJobSize;
                            //}
                            //else
                            {
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdzofs_cnc_wrzofs:
                    //Writes the work zero offset value specified by "datano", "type".The offset value must be stored in "data[0]" of "IODBZOFS" with signed binary format.
                    //Usata per scrivere i parametri dei tools sul CNC letti da file
                    //usata una struttura --> servono tutti i parametri
                    //  In: parametri struttura + elemento da scrivere
                    //  Out:
                    //public short datano;    /* offset NO. */
                    //public short type;      /* axis number */
                    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_AXIS)]
                    //public int[] data = new int[MAX_AXIS];       /* data value */
                    {
                        //Focas1.cnc_setpath(_FocasHandle, Convert.ToInt16(Unità_passaggio.NumeroUnitàSuCn));
                        FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs param = j.FunctionSettings as FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs;                        
                        retJob = param.PrepareWriteRequest(jobdata, out Focas1.IODBZOFS cnc_struct);
                        if (retJob == DriverErrorCodes.ErrorNoError) {
                            cnc_ret = Focas1.cnc_wrzofs(_FocasHandle, param.Size, cnc_struct);
                            if (cnc_ret != Focas1.EW_OK)
                            //{
                            //    diagnTxBytes += j.TotalJobSize;
                            //}
                            //else
                            {
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdparam_cnc_wrparam:
                    //Writes the parameter specified by "datano","type"(only for the parameter with axis).The data format depends on each parameter. The format of Byte/ Word / 2 - Word parameter is generally signed binary.                    
                    {
                        FanucCNCDynTag_cnc_rdparam_cnc_wrparam param = j.FunctionSettings as FanucCNCDynTag_cnc_rdparam_cnc_wrparam;
                        retJob = param.PrepareWriteRequest(jobdata, out Focas1.IODBPSD cnc_struct);
                        if (retJob == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_wrparam(_FocasHandle, param.Size, cnc_struct);
                            if (cnc_ret != Focas1.EW_OK)
                            //{
                            //    diagnTxBytes += j.TotalJobSize;
                            //}
                            //else
                            {
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_getpath_cnc_setpath:
                    {
                        FanucCNCDynTag_cnc_getpath_cnc_setpath param = j.FunctionSettings as FanucCNCDynTag_cnc_getpath_cnc_setpath;
                        retJob = param.PrepareWriteRequest(jobdata, out short CNC_Path);
                        if (retJob == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_setpath(_FocasHandle, CNC_Path);
                            if (cnc_ret == Focas1.EW_OK)
                            {
                                //diagnTxBytes += j.TotalJobSize;
                                _SetGetCNCPath = CNC_Path;
                                _CurrentCNCPath = CNC_Path;
                            }
                            else
                            {
                                retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }
                    }
                    break;                
                #endregion

                #region File Management
                //case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_add:
                //    //Creates the folder or file under the specified folder.
                //    //Execution of this function is kept waiting when CNC is in editing(including the background edit state).
                //    //The file having the same number or name against the current existing file cannot be created.
                //    //The folder having the same name against the current existing folder cannot be created.
                //    //And when the attribute of the folder is prohibited from writing, new folder/ file cannot be created.
                //    //When the making of file is executed on EDIT mode, the making file is automatically selected as the main program.
                //    //Usato durante il download per creare la cartella di destinazione in cui scaricare il file
                //    {
                //        //setta il path se e' diverso da quello corrente
                //        //GetInfoPath(CNC_IpAddress, CNC_PortNumber, CNC_TimeOut, CNC_Path);

                //        int CNC_Path = 1;
                //        short cnc_ret = Focas1.cnc_pdf_add(_FocasHandle, "//CNC_MEM/USER/PATH" + CNC_Path + "/APPOGGIO");
                //    }
                //    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_slctmain:
                    //    //Selects the file under the specified folder as the main program.
                    //    //Execution of this function is kept waiting when CNC is in editing(including the background edit state)
                    //    //Usato durante il download per identificare il nome del file in cui verra' scaricato il programma
                    {
                        //setta il path se e' diverso da quello corrente
                        //GetInfoPath(CNC_IpAddress, CNC_PortNumber, CNC_TimeOut, CNC_Path);
                        FanucCNCDynTag_cnc_pdf_slctmain param = j.FunctionSettings as FanucCNCDynTag_cnc_pdf_slctmain;
                        //int CNC_Path = 1;
                        DriverErrorCodes retSet = param.PrepareWriteRequest(jobdata, _CurrentCNCPath, out string cncSourcePathAndName);
                        if (retSet == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_pdf_slctmain(_FocasHandle, cncSourcePathAndName);
                            if (cnc_ret != Focas1.EW_OK)
                            //{
                            //    diagnTxBytes += j.TotalJobSize;
                            //}
                            //else
                            {                                
                                retSet = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }

                        // file manegement job always return ErrorNoError (retJob) --> error is reported only into conditional variable
                        if (retSet != DriverErrorCodes.ErrorNoError)
                        {
                            CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.Error_cnc_pdf_slctmain, cncSourcePathAndName, string.Format("{0} - {1}", cnc_ret, Focas1.GetFocasErrorDescription(cnc_ret))), EventSeverity.High);

                            j.SetConditionalVariableBit(true, (int)FanucCNCProtocol.ConditionalVariableState.ERROR_DURING_WRITE);
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_del:
                    //Deletes the folder or file under the specified folder.
                    //Execution of this function is kept waiting when CNC is in editing including the background edit state).
                    //Usato durante il download per cancellare il file prima di scaricarlo sul CNC
                    {
                        //setta il path se e' diverso da quello corrente
                        //GetInfoPath(CNC_IpAddress, CNC_PortNumber, CNC_TimeOut, CNC_Path);
                        FanucCNCDynTag_cnc_pdf_del param = j.FunctionSettings as FanucCNCDynTag_cnc_pdf_del;
                        //int CNC_Path = 1;
                        DriverErrorCodes retDel = param.PrepareWriteRequest(jobdata, _CurrentCNCPath, out string cncSourcePathAndName);
                        if (retDel == DriverErrorCodes.ErrorNoError)
                        {
                            cnc_ret = Focas1.cnc_pdf_del(_FocasHandle, cncSourcePathAndName);
                            if (cnc_ret != Focas1.EW_OK)
                            //{
                            //    diagnTxBytes += j.TotalJobSize;
                            //}
                            //else
                            {
                                retDel = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorGenericCommunicationError;
                            }
                        }

                        // file manegement job always return ErrorNoError (retJob) --> error is reported only into conditional variable
                        if (retDel != DriverErrorCodes.ErrorNoError)
                        {
                            CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.Error_cnc_pdf_del_Failed, cncSourcePathAndName, string.Format("{0} - {1}", cnc_ret, Focas1.GetFocasErrorDescription(cnc_ret))), EventSeverity.High);

                            j.SetConditionalVariableBit(true, (int)FanucCNCProtocol.ConditionalVariableState.ERROR_DURING_WRITE);
                        }
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_Custom_DownloadProgramToCnc:
                    {
                        //CNC_Download Program = new CNC_Download("192.168.10.10", 8193, 1, (short)unit, "\nO1\n" + str + "\n%");
                        //CNC_Program = File.ReadAllText("");
                        //CNC_Program =  "\nO1\n" + CNC_Program + "\n%");
                        FanucCNCDynTag_Custom_DownloadProgramToCnc param = j.FunctionSettings as FanucCNCDynTag_Custom_DownloadProgramToCnc;

                        DriverErrorCodes retDownload = DriverErrorCodes.ErrorNoError;
                        
                        retDownload = param.PrepareWriteRequest(jobdata, _CurrentCNCPath, out string cncProgramPath, out string cncProgramName, out string cncProgramBody);
                        if (retDownload != DriverErrorCodes.ErrorNoError)
                        {
                            CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.Error_DownloaProgramToCnc_InvalidParameters, string.Format("{0}{1}",cncProgramPath, cncProgramName), cncProgramBody, string.Format("{0} - {1}", cnc_ret, Focas1.GetFocasErrorDescription(cnc_ret))), EventSeverity.High);

                            j.SetConditionalVariableBit(true, (int)FanucCNCProtocol.ConditionalVariableState.ERROR_DURING_WRITE);
                            break;
                        }
                        #endregion

                #region Manage Appoggio program
                        if (param.ManageTemporaryProgram)
                        {
                            // get active program from cnc
                            #region Get active program from CNC
                            // get active program
                            string activeProgram = string.Empty;
                            FanucCNCDynTag_cnc_pdf_rdmain paramRAP = new FanucCNCDynTag_cnc_pdf_rdmain(null, string.Empty);
                            paramRAP.PrepareReadRequest(out byte[] vs);
                            cnc_ret = Focas1.cnc_pdf_rdmain(_FocasHandle, vs);
                            if (cnc_ret == Focas1.EW_OK) {
                                paramRAP.ParseReadData(vs, out activeProgram);
                                if (string.IsNullOrEmpty(activeProgram))
                                    cnc_ret = (short)Focas1.focas_ret.EW_DATA;
                            }


                            if (cnc_ret == Focas1.EW_OK) { 
                                Focas1.IDBPDFADIR CNC_IStruct_ReadFile = new Focas1.IDBPDFADIR
                                {
                                    path = cncProgramPath,
                                    req_num = 0,
                                    type = 0,
                                    size_kind = 2
                                };
                                Focas1.ODBPDFADIR CNC_OStruct_ReadFile = new Focas1.ODBPDFADIR();

                                //'file name' ending with '/' indicate path, file otherwise
                                // set APPOGGIO as current program
                                if (activeProgram != string.Format("{0}{1}", cncProgramPath, "APPOGGIO"))
                                {
                                    short CNC_ProgramsToBeRead = 1;
                                    cnc_ret = Focas1.cnc_rdpdf_alldir(_FocasHandle, ref CNC_ProgramsToBeRead, CNC_IStruct_ReadFile, CNC_OStruct_ReadFile);
                                    if (string.Format("{0}{1}", cncProgramPath, CNC_OStruct_ReadFile.d_f) != string.Format("{0}{1}", cncProgramPath, "APPOGGIO"))
                                    {
                                        //diagnTxBytes += 2;
                                        cnc_ret = Focas1.cnc_pdf_add(_FocasHandle, string.Format("{0}{1}", cncProgramPath, "APPOGGIO"));
                                        //diagnTxBytes += 2;
                                        cnc_ret = Focas1.cnc_pdf_slctmain(_FocasHandle, string.Format("{0}{1}", cncProgramPath, "APPOGGIO"));
                                        //diagnTxBytes += 2;
                                    }
                                    else
                                    {
                                        cnc_ret = Focas1.cnc_pdf_slctmain(_FocasHandle, string.Format("{0}{1}", cncProgramPath, "APPOGGIO"));
                                        //diagnTxBytes += 2;
                                    }
                                }
                            }

                            if (cnc_ret != Focas1.EW_OK)
                            {
                                CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.Error_DownloaProgramToCnc_ManageTemporaryProgram, string.Format("{0}{1}", cncProgramPath, "APPOGGIO"), string.Format("{0} - {1}", cnc_ret, Focas1.GetFocasErrorDescription(cnc_ret))), EventSeverity.High);

                                j.SetConditionalVariableBit(true, (int)FanucCNCProtocol.ConditionalVariableState.ERROR_DURING_WRITE);
                                break;
                            }
                        }
                        #endregion

                        #region Download and activate program
                        //"//CNC_MEM/USER/PATH"
                        //download program
                        cnc_ret = Focas1.cnc_pdf_del(_FocasHandle,  string.Format("{0}{1}", cncProgramPath, cncProgramName));
                        //diagnTxBytes += 2;

                        cnc_ret = Focas1.cnc_dwnstart4(_FocasHandle, (short)(Focas1.DownloadData.NCProgram), cncProgramPath);
                        if (cnc_ret != Focas1.EW_OK) {
                            CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.Error_DownloaProgramToCnc_SelectMainProgramBefore, string.Format("{0}{1}", cncProgramPath, cncProgramName), string.Format("{0} - {1}", cnc_ret, Focas1.GetFocasErrorDescription(cnc_ret))), EventSeverity.High);

                            j.SetConditionalVariableBit(true, (int)FanucCNCProtocol.ConditionalVariableState.ERROR_DURING_WRITE);
                            break;
                        }
                        //diagnTxBytes += 2;

                        int prgStartPos = 0;
                        int prgSize = cncProgramBody.Length;
                        char[] prgBuffer;
                        int prgBufferSize;
                        bool endOfDownload = false;

                        while (!endOfDownload)
                        {
                            if (prgSize > 1024)
                                prgBufferSize = 1024;
                            else
                                prgBufferSize = prgSize;

                            prgBuffer = new char[prgBufferSize];
                            cncProgramBody.CopyTo(prgStartPos, prgBuffer, 0, prgBufferSize);

                            cnc_ret = Focas1.cnc_download4(_FocasHandle, ref prgBufferSize, prgBuffer);
                            switch(cnc_ret) {
                                case (short)Focas1.focas_ret.EW_BUFFER:
                                    continue;
                                case Focas1.EW_OK:
                                    //diagnTxBytes += prgBufferSize;

                                    prgStartPos += prgBufferSize;
                                    prgSize -= prgBufferSize;
                                    if (prgSize <= 0)
                                        endOfDownload = true;
                                    break;
                                default:
                                    // force to exit to loop
                                    endOfDownload = true;
                                    break;
                            }
                        }                                               

                        if (cnc_ret == Focas1.EW_OK)
                        {
                            //activate downloaded program
                            cnc_ret = Focas1.cnc_pdf_slctmain(_FocasHandle, string.Format("{0}{1}", cncProgramPath, cncProgramName));
                            if (cnc_ret != Focas1.EW_OK)
                            {
                                CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.Error_DownloaProgramToCnc_SelectMainProgramAfter, string.Format("{0}{1}", cncProgramPath, cncProgramName), string.Format("{0} - {1}", cnc_ret, Focas1.GetFocasErrorDescription(cnc_ret))), EventSeverity.High);

                                cnc_ret = Focas1.cnc_pdf_slctmain(_FocasHandle, string.Format("{0}{1}", cncProgramPath, cncProgramName));

                                break;
                            }
                        }

                        Focas1.cnc_dwnend4(_FocasHandle);

                        if (cnc_ret != Focas1.EW_OK)
                        {
                            CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.Error_DownloaProgramToCnc_DuringDownload, string.Format("{0}{1}", cncProgramPath, cncProgramName), string.Format("{0} - {1}", cnc_ret, Focas1.GetFocasErrorDescription(cnc_ret))), EventSeverity.High);

                            j.SetConditionalVariableBit(true, (int)FanucCNCProtocol.ConditionalVariableState.ERROR_DURING_WRITE);
                        }
                        #endregion
                    }
                    break;                
                    #endregion
            }

            //if (StatisticsData != null)
            //    // increase receive bytes diagnostic var
            //    j.DiagnTxBytes += diagnTxBytes;

            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Job = j;
            eJob.ErrorCode = CheckFatalError(retJob, cnc_ret);
            OnJobExecuted(eJob);

            // restore "current" path
            if (_CurrentCNCPath != _SetGetCNCPath)
            {
                cnc_ret = Focas1.cnc_setpath(_FocasHandle, _SetGetCNCPath);
                _CurrentCNCPath = _SetGetCNCPath;
            }

            return eJob.ErrorCode;
        }

        protected DriverErrorCodes ExecuteJobList(List<FanucCNCCommJob> list)
        {
            DriverErrorCodes ret = DriverErrorCodes.ErrorNoError;

            for (int index = 0; index < list.Count; index++)
            {
                if (list[index].Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                {
                    if (list[index].TagsListToWrite.Count == 0)
                    {
                        list[index].TagsListToWrite.AddRange(list[index].TagsList);
                    }
                }
            }
            if (list[0].Type == DriverCodeBase.Enumerators.LinkType.Input || (list[0].Type == DriverCodeBase.Enumerators.LinkType.InputOutput && list[0].TagsListToWrite.Count == 0))
                ret = ReadData(list);
            else
                ret = WriteData(list);

            return ret;
        }        
        #endregion

        #region Properties

        /// <summary>
        /// IP Address or name of Device
        /// </summary>
        private string _DeviceAddress;
        public string DeviceAddress
        {
            get { return _DeviceAddress; }
            set { _DeviceAddress = value; }
        }
        
        /// <summary>
        /// Port Number
        /// </summary>
        private uint _DevicePort;
        public uint DevicePort
        {
            get { return _DevicePort; }
            set { _DevicePort = value; }
        }

        /// <summary>
        /// Machine series (30i, ecc)
        /// </summary>
        private FanucCNCProtocol.MachineSeries _MachineSerie;
        public FanucCNCProtocol.MachineSeries MachineSerie
        {
            get { return _MachineSerie; }
            set { _MachineSerie = value; }
        }

        private int TimeOutInSecond
        {
            get { return (int)Math.Ceiling((float)Timeout / 1000); }
        }

        /// <summary>
        /// Specificy the CNC path
        /// </summary>
        //private short _CNCPath;
        #endregion

        #region IDisposable Interface

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            if (StopWorkerThread != null)
                StopWorkerThread.Set();
            base.Dispose();
        }

        #endregion
    }
}


