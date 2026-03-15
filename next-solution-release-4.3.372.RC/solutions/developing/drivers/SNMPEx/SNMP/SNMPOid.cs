using System;
using System.Linq;

namespace SNMP
{
    public class SNMPOid
    {
        #region Constructors
        public SNMPOid()
        {
            Reset();
        }

        public SNMPOid(string oidAddress)
        {
            Parse(oidAddress);
        }
        #endregion

        #region Data members
        /// Property that indicates if an OID is valid
        private bool _IsValid;
        #endregion

        #region Methods
        void Reset()
        {
            _stringValue = String.Empty;
            _IsValid = false;
            _oidLength = 0;
            _oidIntValue = null;
        }

        void Parse(string oidAddress)
        {
            Reset();

            // Split the OID string
            char delimiter = '.';
            string[] nodeArray = oidAddress.Split(delimiter);

            // Check the number of components of the OID address
            int nodeCounter = nodeArray.Count();
            if(nodeCounter < 2)
            {
                return;
            }

            // Get the integer values of the OID components 
            _oidIntValue = new uint[nodeCounter];
            int i = 0;
            foreach(var substring in nodeArray)
            {
                if(String.IsNullOrWhiteSpace(substring))
                {
                    return;
                }

                // Get the integer value of the next OID component 
                uint tempElementNumber = 0;
                if (!uint.TryParse(substring, out tempElementNumber) ||
                    (tempElementNumber < 0))
                {
                    return;
                }
                _oidIntValue[i++] = tempElementNumber;
            }

            // Encode the OID
            _oidLength = nodeCounter;

            // Valid OID
            _stringValue = oidAddress;


            _codifiedOid = null;
            _codifiedOidLength = 0;

            // Get the number of bytes requested for the codified OID
            uint tempcodifiedOidLength = SNMPProtocol.ASN1GetCodifiedOIDLength(_oidIntValue, _oidLength);
            if (tempcodifiedOidLength > 0)
            {
                // Allocate the array of bytes for the codified OID
                _codifiedOid = new byte[tempcodifiedOidLength];

                // Codify the OID
                _codifiedOidLength = SNMPProtocol.ASN1CodifyOID(_oidIntValue, _oidLength, ref _codifiedOid);
            }

            _IsValid = true;
        }

        private int getEncodeOIDLength(int[] intValuesArray, int nodeCounter)
        {
            int nodeIndex = 2;
            int returnValue = 1;
            while(nodeIndex < nodeCounter)
            {
                int requiredBytes = 0;
                int val1 = intValuesArray[nodeIndex++];
                if(val1 > 0)
                {
                    int val2 = val1;
                    while (val2 > 0)
                    {
                        requiredBytes++;
                        val2 >>= 7;
                    }
                }
                else
                {
                    requiredBytes++;
                }
                returnValue += requiredBytes;
            }

            return (returnValue);
        }

        public bool IsValid()
        {
            return _IsValid;
        }
        #endregion

        #region DataMembers
        //byte[] encodedOID;
        #endregion

        #region Properties

        /// <summary>
        /// OID in string format
        /// </summary>
        private string _stringValue;
        public string stringValue
        {
            get { return _stringValue; }
        }

        /// <summary>
        /// Number of the integer values of the OID
        /// </summary>
        private int _oidLength;
        public int oidLength
        {
            get { return _oidLength; }
        }
        #endregion

        /// <summary>
        /// OID in array format
        /// </summary>
        private uint[] _oidIntValue;
        public uint[] oidIntValue
        {
            get { return _oidIntValue; }
        }

        private uint _codifiedOidLength = 0;
        public uint codifiedOidLength
        {
            get { return _codifiedOidLength; }
            set { _codifiedOidLength = value; }
        }

        private byte[] _codifiedOid;
        public byte[] codifiedOid
        {
            get { return _codifiedOid; ; }
            set { _codifiedOid = value; }
        }        
    }
}
