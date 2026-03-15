using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    public class SNMPOid : Object
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
 
        #region Methods
        void Reset()
        {
            _stringValue = String.Empty;
            _IsValid = false;
            _oidLength = 0;
            _oidIntValues = null;
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
            _oidIntValues = new uint[nodeCounter];
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
                _oidIntValues[i++] = tempElementNumber;
            }

            // Encode the OID
            _oidLength = nodeCounter;

            // Valid OID
            _stringValue = oidAddress;
            _IsValid = true;
        }

        int getEncodeOIDLength(int[] intValuesArray, int nodeCounter)
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

        //void EncodeOID(int[] intValuesArray, int nodeCounter)
        //{
        //    // First encoded byte = 40*<1st OID element> + <2nd OID element>
        //    encodedOID[0] = (byte)(intValuesArray[0]*40 + intValuesArray[1]);

        //    int nodeIndex = 2;
        //    int encodedBytes = 1;
        //    while (nodeIndex < nodeCounter)
        //    {
        //        int requiredBytes = 0;
        //        int val1 = intValuesArray[nodeIndex++];
        //        int val2 = val1;
        //        if (val1 > 0)
        //        {
        //            while (val2 > 0)
        //            {
        //                requiredBytes++;
        //                val2 >>= 7;
        //            }
        //        }
        //        else
        //        {
        //            requiredBytes++;
        //        }
                
        //        while(requiredBytes > 0)
        //        {
        //            val2 = val1 >> (7 * (requiredBytes - 1));
        //            val2 &= 0x7f;
        //            if(requiredBytes > 1)
        //            {
        //                val2 += 128;
        //            }
        //            encodedOID[encodedBytes++] = (byte)val2;
        //            requiredBytes--;
        //        }
        //    }
        //}

        //public void AddEncodedOidToRequest(ref byte[] requestBuffer, ref int bufferIndex)
        //{
        //    int i = 0;
        //    for(i=0; i<_encodedOIDLength; i++)
        //    {
        //        requestBuffer[bufferIndex++] = encodedOID[i];
        //    }
        //}
        #endregion

        #region DataMembers
        //byte[] encodedOID;
        #endregion

        #region Properties
        /// <summary>
        /// Property that indicates if an OID is valid
        /// </summary>
        private bool _IsValid;
        public bool IsValid
        {
            get
            {
                return _IsValid;
            }

        }

        /// <summary>
        /// OID in string format
        /// </summary>
        private string _stringValue;
        public string stringValue
        {
            get
            {
                return _stringValue;
            }
        }

        /// <summary>
        /// Number of the integer values of the OID
        /// </summary>
        private int _oidLength;
        public int oidLength
        {
            get
            {
                return _oidLength;
            }
        }
        #endregion

        /// <summary>
        /// OID in array format
        /// </summary>
        private uint[] _oidIntValues;
        public uint[] oidIntValues
        {
            get
            {
                return _oidIntValues;
            }
        }
    }
}
