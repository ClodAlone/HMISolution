using System;
using System.Collections.Generic;
using System.Net;
using IpDriverCodeBaseEx;

namespace BACnet
{
    public class BACnetBBMDDevice
    {
        #region Constructors
        public BACnetBBMDDevice()
        {
            _Id = string.Empty;
            _Address = string.Empty;
            _Port = 47808;
            _Lifetime = 60;     //min
            _BacnetState = BACnetState.None;
            _IsRegistred = false;
            _RelatedStations = new List<BACnetStation>();
        }    

        public BACnetBBMDDevice(string id,string address,int port, uint lifetime)
        {
            _Id = id;
            _Address = address;
            _Port = port;
            _Lifetime = lifetime;
            _BacnetState = BACnetState.None;
            _LastSend = DateTime.UtcNow;
            _IsRegistred = false;
            _RelatedStations = new List<BACnetStation>();
        }

        #endregion        

        #region Properties

        public enum BACnetState : int
        {
            None = 0,
            //BBMDWhoIs,
            RegisterAsForeignDevice,
            //ReadDFTTable,
            //ReadBDTTable,
            RefreshRegistration
        }

        private string _Id;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
                
        private bool _IsRegistred;
        public bool IsRegistred
        {
            get
            {
                return _IsRegistred;
            }            
        }

        private uint _Lifetime;
        public uint Lifetime
        {
            get { return _Lifetime; }
            set { _Lifetime = value; }
        }

        private string _Address;
        private int _Port;       

        /// <summary>   The UDP remote end point. </summary>
        IPEndPoint _RemoteEndPoint = null;
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (_RemoteEndPoint == null && !String.IsNullOrEmpty(_Address))
                {
                    IPAddress resolvedIPAddress;
                    if (UDPManager.GetResolvedConnecionIPAddress(_Address, out resolvedIPAddress))
                        _RemoteEndPoint = new IPEndPoint(resolvedIPAddress, _Port);
                }
                return _RemoteEndPoint;
            }
        }

        private BACnetState _BacnetState;
        public BACnetState BacnetState
        {
            get { return _BacnetState; }
            set { _BacnetState = value; }
        }

        private DateTime _LastSend;
        public DateTime LastSend
        {
            get { return _LastSend; }
            set { _LastSend = value; }
        }

        public List<BACnetStation> _RelatedStations;
        public List<BACnetStation> RelatedStations
        {
            get { return _RelatedStations; }
            set { _RelatedStations = value; }
        }
        #endregion

        #region Members        
        public bool IsRegistrationExpired()
        {
            return (_IsRegistred && DateTime.UtcNow.Subtract(_LastSend).TotalSeconds > (_Lifetime * 60));
        }

        public void SetRegistred()
        {
            _IsRegistred = true;
            _LastSend = DateTime.UtcNow;
            _BacnetState = BACnetBBMDDevice.BACnetState.RefreshRegistration;
        }

        public void SetUnRegistred()
        {
            _IsRegistred = false;
            _LastSend = DateTime.UtcNow;
            _BacnetState = BACnetBBMDDevice.BACnetState.None;
        }

        public void CheckBacNetState()
        {
            if (BacnetState == BACnetState.None)
            {
                BacnetState = BACnetState.RegisterAsForeignDevice;
            }
            //if (BacnetState == BACnetState.BBMDWhoIs)
            //{
            //    if (((BACnetStation)Station).WhoIsDisabled)
            //    {
            //        if (!((BACnetStation)Station).BACnetInitDone)
            //            ((BACnetChannel)Station.GetChannel()).InitStationWithNoWhoIsData((BACnetStation)Station);
            //        BacnetState = BACnetState.BBMDRegisterAsForeignDevice;
            //    }
            //    else
            //    {
            //        if (((BACnetStation)Station).BBMDRegister && !((BACnetStation)Station).IsBBMDRegistred)
            //            BacnetState = BACnetState.BBMDRegisterAsForeignDevice;
            //        else
            //            return; // send WhoIs request
            //    }
            //}
            if (BacnetState == BACnetState.RegisterAsForeignDevice)
            {
                return; // send BBMDRegisterAsForeignDevice
            }
            //if (BacnetState == BACnetState.ReadDFTTable)
            //{
            //    return; // send BBMDReadDFTTable
            //}
            //if (BacnetState == BACnetState.ReadBDTTable)
            //{
            //    return; // send BBMDReadBDTTable
            //}
            if (BacnetState == BACnetState.RefreshRegistration)
            {
                return; // send BBMDRefreshRegistration
            }
        }
        #endregion
    }
}
