using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EIB
{
    class EIBAddress : Object
    {
        #region Data members
        byte addMainPart;
        byte addMiddlePart;
        byte AddSubPart;
        #endregion

        #region Methods
        public void ParseAddress(string newAddress)
        {
            string[] AddressSplit = newAddress.Split(new Char[] { '/' });
            addMainPart = byte.Parse(AddressSplit[0]);
            addMiddlePart = byte.Parse(AddressSplit[1]);
            AddSubPart = byte.Parse(AddressSplit[2]);
            ushort usAux = addMainPart;
            usAux <<= 11;
            _addressWord = usAux;
            usAux = addMiddlePart;
            usAux <<= 8;
            _addressWord += usAux;
            _addressWord += AddSubPart;
            _addressString = newAddress;
        }

        public bool SetAddress(string newAddress)
        {
            if (!IsValidAddress(newAddress))
            {
                return false;
            }

            ParseAddress(newAddress);

            return true;
        }

        public void ParseAddress(ushort newAddress)
        {
            addMainPart = (byte) ((newAddress >> 11) & 0x1F);
            addMiddlePart = (byte) ((newAddress >> 8) & 0x7);
            AddSubPart = (byte) (newAddress & 0xFF);
            _addressWord = newAddress;
            _addressString = String.Format("{0}/{1}/{2}", addMainPart,
                                           addMiddlePart, AddSubPart);
        }

        public bool SetAddress(ushort newAddress)
        {
            if (!IsValidAddress(newAddress))
            {
                return false;
            }

            ParseAddress(newAddress);

            return true;
        }
        #endregion

        #region Static methods
        public static bool IsValidAddress(string Address)
        {
            // At least 5 characters
            if (Address.Length < 5)
            {
                return false;
            }

            // Check address format: x/x/x 
            string[] AddressSplit = Address.Split(new Char[] { '/' });
            if (AddressSplit.Count() != 3)
            {
                return false;
            }

            // Check values of the three elements of the address
            int AddressMainPart = int.Parse(AddressSplit[0]);
            int AddressMiddlePart = int.Parse(AddressSplit[1]);
            int AddressSubPart = int.Parse(AddressSplit[2]);
            if ((AddressMainPart < 0) || (AddressMiddlePart < 0) ||
               (AddressSubPart < 0))
            {
                return false;
            }
            if ((AddressMainPart > 31) || (AddressMiddlePart > 7) ||
               (AddressSubPart > 255))
            {
                return false;
            }
            if ((AddressMainPart == 0) && (AddressMiddlePart == 0) &&
               (AddressSubPart == 0))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidAddress(ushort Address)
        {
            if((Address == 0) || (Address > 32767))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Properties

        /// <summary>
        /// EIB Address in string format
        /// </summary>
        private string _addressString;
        public string addressString
        {
            get
            {
                return _addressString;
            }
        }

        /// <summary>
        /// EIB Address in ushort format
        /// </summary>
        private ushort _addressWord;
        public ushort addressWord
        {
            get
            {
                return _addressWord;
            }
        }
        #endregion
    }
}
