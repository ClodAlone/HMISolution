using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACnet
{
    public static partial class BufferExpand
    {
        public static UInt32 toUInt32_WithSwap(ref byte[] DataArray, ref int Offset, bool incremetOffset = false)
        {
            checkSize(DataArray, Offset, 4);
            UInt32 outval = 0;
            byte[] array = new byte[4] { 0, 0, 0, 0 };
            array[3] = DataArray[Offset++];
            array[2] = DataArray[Offset++];
            array[1] = DataArray[Offset++];
            array[0] = DataArray[Offset++];
            outval = BitConverter.ToUInt32(array, 0);

            return outval;
        }
        public class StructContextTag
        {
            public byte TagClass = 0;
            public byte ContexTagNumber = 0;
            public int Length = 0;
        }
        public class StructObjectIdentifier
        {
            public byte ObjectType = 0;
            public UInt32 IstanceNumber = 0;
            public BACnetObjectIdentifier Object;
        }
        public static StructObjectIdentifier GetObjectIdentifier(byte[] DataArray, ref int Offset)
        {
            StructObjectIdentifier outval = new StructObjectIdentifier();
            UInt32 valueObj = toUInt32_WithSwap(ref DataArray, ref Offset, true);
            outval.ObjectType = (byte)(valueObj >> 22);
            outval.IstanceNumber = (0x003fffff & valueObj);
            outval.Object = new BACnetObjectIdentifier((BACnetEnums.ObjectTypes)outval.ObjectType, (UInt32)outval.IstanceNumber);
            return outval;
        }
        public static StructContextTag GetContextTag(byte[] DataArray, ref int Offset)
        {
            StructContextTag outval = new StructContextTag();
            byte value = toByte(ref DataArray, ref Offset);
            outval.TagClass = (byte)((value & 0x08) >> 3);
            outval.ContexTagNumber = (byte)((value & 0xF0) >> 4);
            if (outval.ContexTagNumber != 3)
                outval.Length = (byte)(0x07 & value);
            else
                outval.Length = 0;
            return outval;
        }

        public const int BACNET_MAX_OBJECT = 0x3FF;
        public const int BACNET_INSTANCE_BITS = 22;
        public const int BACNET_MAX_INSTANCE = 0x3FFFFF;
        public const int MAX_BITSTRING_BYTES = 15;
        public const uint BACNET_ARRAY_ALL = 0xFFFFFFFFU;
        public const uint BACNET_NO_PRIORITY = 0;
        public const uint BACNET_MIN_PRIORITY = 1;
        public const uint BACNET_MAX_PRIORITY = 16;


        public static int DecodeReadPropertyMultipleAcknowledge(byte[] buffer, int offset, int apdu_len, ref List<List<BacnetReadAccessResult>> itemList)
        {
            int len = 0;
            int tmp;

            List<BacnetReadAccessResult> objectPropertyList = new List<BacnetReadAccessResult>();

            while ((apdu_len - len) > 0)
            {
                BacnetReadAccessResult value;
                tmp = decode_read_access_result(buffer, offset + len, apdu_len - len, out value);
                if (tmp < 0)
                    return -1;
                len += tmp;
                objectPropertyList.Add(value);
            }
            itemList.Add(objectPropertyList);
            return len;
        }
        public static int decode_read_access_result(byte[] buffer, int offset, int apdu_len, out BacnetReadAccessResult value)
        {
            int len = 0;
            byte tag_number;
            uint len_value_type;
            int tag_len;
            value = new BacnetReadAccessResult();

            if (!decode_is_context_tag(buffer, offset + len, 0))
                return -1;
            len = 1;
            len += decode_object_id(buffer, offset + len, out value.objectIdentifier.type, out value.objectIdentifier.instance);

            /* Tag 1: listOfResults */
            if (!decode_is_opening_tag_number(buffer, offset + len, 1))
                return -1;
            len++;

            List<BacnetPropertyValue> _value_list = new List<BacnetPropertyValue>();
            while ((apdu_len - len) > 0)
            {
                BacnetPropertyValue new_entry = new BacnetPropertyValue();

                /* end */
                if (decode_is_closing_tag_number(buffer, offset + len, 1))
                {
                    len++;
                    break;
                }

                /* Tag 2: propertyIdentifier */
                len += decode_tag_number_and_value(buffer, offset + len, out tag_number, out len_value_type);
                if (tag_number != 2)
                    return -1;
                len += decode_enumerated(buffer, offset + len, len_value_type, out new_entry.property.propertyIdentifier);
                /* Tag 3: Optional Array Index */
                tag_len = decode_tag_number_and_value(buffer, offset + len, out tag_number, out len_value_type);
                if (tag_number == 3)
                {
                    len += tag_len;
                    len += decode_unsigned(buffer, offset + len, len_value_type, out new_entry.property.propertyArrayIndex);
                }
                else
                    new_entry.property.propertyArrayIndex = BACNET_ARRAY_ALL;

                /* Tag 4: Value */
                tag_len = decode_tag_number_and_value(buffer, offset + len, out tag_number, out len_value_type);
                len += tag_len;
                if (tag_number == 4)
                {
                    BacnetValue v;
                    List<BacnetValue> local_value_list = new List<BacnetValue>();
                    while (!decode_is_closing_tag_number(buffer, offset + len, 4))
                    {
                        tag_len = bacapp_decode_application_data(buffer, offset + len, apdu_len + offset - 1, (BACnetEnums.BacnetObjectTypes)value.objectIdentifier.type, (BACnetEnums.BacnetPropertyIds)new_entry.property.propertyIdentifier, out v);
                        if (tag_len < 0) return -1;
                        len += tag_len;
                        local_value_list.Add(v);
                    }
                    // FC : two values one Date & one Time => change to one datetime
                    if ((local_value_list.Count == 2) && (local_value_list[0].Tag == BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_DATE) && (local_value_list[1].Tag == BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_TIME))
                    {
                        DateTime date, time;
                        date = (DateTime)local_value_list[0].Value;
                        time = (DateTime)local_value_list[1].Value;
                        DateTime bdatetime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
                        local_value_list.Clear();
                        local_value_list.Add(new BacnetValue(BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_DATETIME, bdatetime));
                        new_entry.value = local_value_list;
                    }
                    else
                        new_entry.value = local_value_list;
                    len++;
                }
                else if (tag_number == 5)
                {
                    /* Tag 5: Error */
                    BacnetError err = new BacnetError();
                    len += decode_tag_number_and_value(buffer, offset + len, out tag_number, out len_value_type);
                    len += decode_enumerated(buffer, offset + len, len_value_type, out len_value_type);      //error_class
                    err.error_class = (BACnetEnums.ERROR_CLASS)len_value_type;
                    len += decode_tag_number_and_value(buffer, offset + len, out tag_number, out len_value_type);
                    len += decode_enumerated(buffer, offset + len, len_value_type, out len_value_type);       //error_code
                    err.error_code = (BACnetEnums.BacnetErrorCodes)len_value_type;
                    if (!decode_is_closing_tag_number(buffer, offset + len, 5))
                        return -1;
                    len++;

                    new_entry.value = new BacnetValue[] { new BacnetValue(BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_ERROR, err) };
                }

                _value_list.Add(new_entry);

            }
            value.values = _value_list;

            return len;
        }
        public static bool decode_is_context_tag(byte[] buffer, int offset, byte tag_number)
        {
            byte my_tag_number = 0;

            decode_tag_number(buffer, offset, out my_tag_number);
            return (bool)(IS_CONTEXT_SPECIFIC(buffer[offset]) && (my_tag_number == tag_number));
        }
        public static bool decode_is_opening_tag_number(byte[] buffer, int offset, byte tag_number)
        {
            byte my_tag_number = 0;

            decode_tag_number(buffer, offset, out my_tag_number);
            return (bool)(IS_OPENING_TAG(buffer[offset]) && (my_tag_number == tag_number));
        }
        public static int decode_object_id(byte[] buffer, int offset, out ushort object_type, out uint instance)
        {
            uint value = 0;
            int len = 0;

            len = decode_unsigned32(buffer, offset, out value);
            object_type =
                (ushort)(((value >> BACNET_INSTANCE_BITS) & BACNET_MAX_OBJECT));
            instance = (value & BACNET_MAX_INSTANCE);

            return len;
        }
        public static int decode_object_id(byte[] buffer, int offset, out BACnetEnums.BacnetObjectTypes object_type, out uint instance)
        {
            uint value = 0;
            int len = 0;

            len = decode_unsigned32(buffer, offset, out value);
            object_type = (BACnetEnums.BacnetObjectTypes)(((value >> BACNET_INSTANCE_BITS) & BACNET_MAX_OBJECT));
            instance = (value & BACNET_MAX_INSTANCE);

            return len;
        }
        public static int decode_enumerated(byte[] buffer, int offset, uint len_value, out uint value)
        {
            int len;
            len = decode_unsigned(buffer, offset, len_value, out value);
            return len;
        }
        public static int decode_tag_number_and_value(byte[] buffer, int offset, out byte tag_number, out uint value)
        {
            int len = 1;
            ushort value16;
            uint value32;

            len = decode_tag_number(buffer, offset, out tag_number);
            if (IS_EXTENDED_VALUE(buffer[offset]))
            {
                /* tagged as uint32_t */
                if (buffer[offset + len] == 255)
                {
                    len++;
                    len += decode_unsigned32(buffer, offset + len, out value32);
                    value = value32;
                }
                /* tagged as uint16_t */
                else if (buffer[offset + len] == 254)
                {
                    len++;
                    len += decode_unsigned16(buffer, offset + len, out value16);
                    value = value16;
                }
                /* no tag - must be uint8_t */
                else
                {
                    value = buffer[offset + len];
                    len++;
                }
            }
            else if (IS_OPENING_TAG(buffer[offset]))
            {
                value = 0;
            }
            else if (IS_CLOSING_TAG(buffer[offset]))
            {
                /* closing tag */
                value = 0;
            }
            else
            {
                /* small value */
                value = (uint)(buffer[offset] & 0x07);
            }

            return len;
        }

        public static int decode_tag_number(byte[] buffer, int offset, out byte tag_number)
        {
            int len = 1;        /* return value */

            /* decode the tag number first */
            if (IS_EXTENDED_TAG_NUMBER(buffer[offset]))
            {
                /* extended tag */
                tag_number = buffer[offset + 1];
                len++;
            }
            else
            {
                tag_number = (byte)(buffer[offset] >> 4);
            }

            return len;
        }
        public static bool decode_is_closing_tag_number(byte[] buffer, int offset, byte tag_number)
        {
            byte my_tag_number = 0;

            decode_tag_number(buffer, offset, out my_tag_number);
            return (bool)(IS_CLOSING_TAG(buffer[offset]) && (my_tag_number == tag_number));
        }
        public struct BacnetValue
        {
            public BACnetEnums.BacnetApplicationTags Tag;
            public object Value;
            public BacnetValue(BACnetEnums.BacnetApplicationTags tag, object value)
            {
                this.Tag = tag;
                this.Value = value;
            }
            public BacnetValue(object value)
            {
                this.Value = value;
                Tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_NULL;

                //guess at the tag
                if (value != null)
                    Tag = TagFromType(value.GetType());
            }

            public BACnetEnums.BacnetApplicationTags TagFromType(Type t)
            {
                if (t == typeof(string))
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING;
                else if (t == typeof(int) || t == typeof(short) || t == typeof(sbyte))
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_SIGNED_INT;
                else if (t == typeof(uint) || t == typeof(ushort) || t == typeof(byte))
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                else if (t == typeof(bool))
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_BOOLEAN;
                else if (t == typeof(float))
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_REAL;
                else if (t == typeof(double))
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_DOUBLE;
                else if (t == typeof(BacnetBitString))
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_BIT_STRING;
                else if (t == typeof(BacnetObjectId))
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID;
                else
                    return BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_CONTEXT_SPECIFIC_ENCODED;
            }
        }
        public struct BacnetReadAccessResult
        {
            public BacnetObjectId objectIdentifier;
            public IList<BacnetPropertyValue> values;
            public BacnetReadAccessResult(BacnetObjectId objectIdentifier, IList<BacnetPropertyValue> values)
            {
                this.objectIdentifier = objectIdentifier;
                this.values = values;
            }
        }
        public struct BacnetPropertyValue
        {
            public BacnetPropertyReference property;
            public IList<BacnetValue> value;
            public byte priority;

            public override string ToString()
            {
                return property.ToString();
            }
        }

        public struct BacnetPropertyReference
        {
            public UInt32 propertyIdentifier;
            public UInt32 propertyArrayIndex;        /* optional */
            public BacnetPropertyReference(uint id, uint array_index)
            {
                propertyIdentifier = id;
                propertyArrayIndex = array_index;
            }
            public override string ToString()
            {
                return ((BACnetEnums.BACnetPropertyIdentifier)propertyIdentifier).ToString();
            }
        };

        public struct BacnetBitString
        {
            public byte bits_used;
            public byte[] value;

            public override string ToString()
            {
                string ret = "";
                for (int i = 0; i < bits_used; i++)
                {
                    ret = ret + ((value[i / 8] & (1 << (i % 8))) > 0 ? "1" : "0");
                }
                return ret;
            }

            public void SetBit(byte bit_number, bool v)
            {
                byte byte_number = (byte)(bit_number / 8);
                byte bit_mask = 1;

                if (value == null) value = new byte[MAX_BITSTRING_BYTES];

                if (byte_number < MAX_BITSTRING_BYTES)
                {
                    /* set max bits used */
                    if (bits_used < (bit_number + 1))
                        bits_used = (byte)(bit_number + 1);
                    bit_mask = (byte)(bit_mask << (bit_number - (byte_number * 8)));
                    if (v)
                        value[byte_number] |= bit_mask;
                    else
                        value[byte_number] &= (byte)(~(bit_mask));
                }
            }
            public static BacnetBitString Parse(string str)
            {
                BacnetBitString ret = new BacnetBitString();
                ret.value = new byte[MAX_BITSTRING_BYTES];

                if (!string.IsNullOrEmpty(str))
                {
                    ret.bits_used = (byte)str.Length;
                    for (int i = 0; i < ret.bits_used; i++)
                    {
                        bool is_set = str[i] == '1';
                        if (is_set) ret.value[i / 8] |= (byte)(1 << (i % 8));
                    }
                }

                return ret;
            }

            public uint ConvertToInt()
            {
                return value == null ? 0 : BitConverter.ToUInt32(value, 0);
            }

            public static BacnetBitString ConvertFromInt(uint value)
            {
                BacnetBitString ret = new BacnetBitString();
                ret.value = BitConverter.GetBytes(value);
                ret.bits_used = (byte)Math.Ceiling(Math.Log(value, 2));
                return ret;
            }
        };

        public struct BacnetObjectId : IComparable<BacnetObjectId>
        {
            public BACnetEnums.BacnetObjectTypes type;
            public UInt32 instance;
            public BacnetObjectId(BACnetEnums.BacnetObjectTypes type, UInt32 instance)
            {
                this.type = type;
                this.instance = instance;
            }
            public BACnetEnums.BacnetObjectTypes Type
            {
                get { return type; }
                set { type = value; }
            }
            public UInt32 Instance
            {
                get { return instance; }
                set { instance = value; }
            }
            public override string ToString()
            {
                return type.ToString() + ":" + instance;
            }
            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                else return obj.ToString().Equals(this.ToString());
            }
            public int CompareTo(BacnetObjectId other)
            {
                if (this.type == other.type)
                    return this.instance.CompareTo(other.instance);
                else
                {
                    if (this.type == BACnetEnums.BacnetObjectTypes.OBJECT_DEVICE) return -1;
                    if (other.type == BACnetEnums.BacnetObjectTypes.OBJECT_DEVICE) return 1;
                    // cast to int for comparison otherwise unpredictable behaviour with outbound enum (proprietary type)
                    return ((int)(this.type)).CompareTo((int)other.type);
                }
            }
        };
        public static int bacapp_decode_application_data(byte[] buffer, int offset, int max_offset, BACnetEnums.BacnetObjectTypes object_type, BACnetEnums.BacnetPropertyIds property_id, out BacnetValue value)
        {
            int len = 0;
            int tag_len = 0;
            int decode_len = 0;
            byte tag_number = 0;
            uint len_value_type = 0;

            value = new BacnetValue();

            /* FIXME: use max_apdu_len! */
            if (!IS_CONTEXT_SPECIFIC(buffer[offset]))
            {
                tag_len = decode_tag_number_and_value(buffer, offset, out tag_number, out len_value_type);
                if (tag_len > 0)
                {
                    len += tag_len;

                    decode_len = bacapp_decode_data(buffer, offset + len, max_offset, (BACnetEnums.BacnetApplicationTags)tag_number, len_value_type, out value);
                    if (decode_len < 0) return decode_len;
                    len += decode_len;
                }
            }
            else
            {
                return bacapp_decode_context_application_data(buffer, offset, max_offset, object_type, property_id, out value);
            }

            return len;
        }
        public struct BacnetError
        {
            public BACnetEnums.ERROR_CLASS error_class;
            public BACnetEnums.BacnetErrorCodes error_code;
            public BacnetError(BACnetEnums.ERROR_CLASS error_class, BACnetEnums.BacnetErrorCodes error_code)
            {
                this.error_class = error_class;
                this.error_code = error_code;
            }
            public BacnetError(uint error_class, uint error_code)
            {
                this.error_class = (BACnetEnums.ERROR_CLASS)error_class;
                this.error_code = (BACnetEnums.BacnetErrorCodes)error_code;
            }
            public override string ToString()
            {
                return error_class.ToString() + ": " + error_code.ToString();
            }
        }

        public static int decode_unsigned(byte[] buffer, int offset, uint len_value, out uint value)
        {
            ushort unsigned16_value = 0;

            switch (len_value)
            {
                case 1:
                    value = buffer[offset];
                    break;
                case 2:
                    decode_unsigned16(buffer, offset, out unsigned16_value);
                    value = unsigned16_value;
                    break;
                case 3:
                    decode_unsigned24(buffer, offset, out value);
                    break;
                case 4:
                    decode_unsigned32(buffer, offset, out value);
                    break;
                default:
                    value = 0;
                    break;
            }

            return (int)len_value;
        }

        public static int decode_unsigned8(byte[] buffer, int offset, out byte value)
        {
            value = buffer[offset + 0];
            return 1;
        }
        public static bool IS_OPENING_TAG(byte x)
        {
            return ((x & 0x07) == 6);
        }

        public static bool IS_CLOSING_TAG(byte x)
        {
            return ((x & 0x07) == 7);
        }
        public static bool IS_EXTENDED_VALUE(byte x)
        {
            return ((x & 0x07) == 5);
        }
        public static bool IS_EXTENDED_TAG_NUMBER(byte x)
        {
            return ((x & 0xF0) == 0xF0);
        }
        public static bool IS_CONTEXT_SPECIFIC(byte x)
        {
            return ((x & 0x8) == 0x8);
        }
        public static int bacapp_decode_data(byte[] buffer, int offset, int max_length, BACnetEnums.BacnetApplicationTags tag_data_type, uint len_value_type, out BacnetValue value)
        {
            int len = 0;
            uint uint_value;
            int int_value;

            value = new BacnetValue();
            value.Tag = tag_data_type;

            switch (tag_data_type)
            {
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_NULL:
                    /* nothing else to do */
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_BOOLEAN:
                    value.Value = len_value_type > 0 ? true : false;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT:
                    len = decode_unsigned(buffer, offset, len_value_type, out uint_value);
                    value.Value = uint_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_SIGNED_INT:
                    len = decode_signed(buffer, offset, len_value_type, out int_value);
                    value.Value = int_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_REAL:
                    float float_value;
                    len = decode_real_safe(buffer, offset, len_value_type, out float_value);
                    value.Value = float_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_DOUBLE:
                    double double_value;
                    len = decode_double_safe(buffer, offset, len_value_type, out double_value);
                    value.Value = double_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OCTET_STRING:
                    byte[] octet_string = new byte[len_value_type];
                    len = decode_octet_string(buffer, offset, max_length, octet_string, 0, len_value_type);
                    value.Value = octet_string;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING:
                    string string_value;
                    len = decode_character_string(buffer, offset, max_length, len_value_type, out string_value);
                    value.Value = string_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_BIT_STRING:
                    BacnetBitString bit_value;
                    len = decode_bitstring(buffer, offset, len_value_type, out bit_value);
                    value.Value = bit_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED:
                    len = decode_enumerated(buffer, offset, len_value_type, out uint_value);
                    value.Value = uint_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_DATE:
                    DateTime date_value;
                    len = decode_date_safe(buffer, offset, len_value_type, out date_value);
                    value.Value = date_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_TIME:
                    DateTime time_value;
                    len = decode_bacnet_time_safe(buffer, offset, len_value_type, out time_value);
                    value.Value = time_value;
                    break;
                case BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID:
                    {
                        ushort object_type = 0;
                        uint instance = 0;
                        len = decode_object_id_safe(buffer, offset, len_value_type, out object_type, out instance);
                        value.Value = new BacnetObjectId((BACnetEnums.BacnetObjectTypes)object_type, instance);
                    }
                    break;
                default:
                    break;
            }

            return len;
        }
        public static int decode_signed(byte[] buffer, int offset, uint len_value, out int value)
        {
            switch (len_value)
            {
                case 1:
                    sbyte sbyte_value;
                    decode_signed8(buffer, offset, out sbyte_value);
                    value = sbyte_value;
                    break;
                case 2:
                    short short_value;
                    decode_signed16(buffer, offset, out short_value);
                    value = short_value;
                    break;
                case 3:
                    decode_signed24(buffer, offset, out value);
                    break;
                case 4:
                    decode_signed32(buffer, offset, out value);
                    break;
                default:
                    value = 0;
                    break;
            }

            return (int)len_value;
        }
        public static int decode_octet_string(byte[] buffer, int offset, int max_length, byte[] octet_string, int octet_string_offset, uint octet_string_length)
        {
            int len = 0;        /* return value */

            octetstring_copy(buffer, offset, max_length, octet_string, octet_string_offset, octet_string_length);
            len = (int)octet_string_length;

            return len;
        }
        private static bool octetstring_copy(byte[] buffer, int offset, int max_offset, byte[] octet_string, int octet_string_offset, uint octet_string_length)
        {
            bool status = false;        /* return value */

            if (octet_string_length <= (max_offset + offset))
            {
                if (octet_string != null) Array.Copy(buffer, offset, octet_string, octet_string_offset, Math.Min(octet_string.Length, buffer.Length - offset));
                status = true;
            }

            return status;
        }
        public static int decode_character_string(byte[] buffer, int offset, int max_length, uint len_value, out string char_string)
        {
            int len = 0;        /* return value */
            bool status = false;

            status = multi_charset_characterstring_decode(buffer, offset + 1, max_length, buffer[offset], len_value - 1, out char_string);
            if (status)
            {
                len = (int)len_value;
            }

            return len;
        }
        private static bool multi_charset_characterstring_decode(byte[] buffer, int offset, int max_length, byte encoding, uint length, out string char_string)
        {
            char_string = "";
            try
            {
                Encoding e;

                switch ((BACnetEnums.BacnetCharacterStringEncodings)encoding)
                {
                    // 'normal' encoding, backward compatible ANSI_X34 (for decoding only)
                    case BACnetEnums.BacnetCharacterStringEncodings.CHARACTER_UTF8:
                        e = Encoding.UTF8;
                        break;

                    // UCS2 is backward compatible UTF16 (for decoding only)
                    // http://hackipedia.org/Character%20sets/Unicode,%20UTF%20and%20UCS%20encodings/UCS-2.htm
                    // https://en.wikipedia.org/wiki/Byte_order_mark
                    case BACnetEnums.BacnetCharacterStringEncodings.CHARACTER_UCS2:
                        if ((buffer[offset] == 0xFF) && (buffer[offset + 1] == 0xFE)) // Byte Order Mark 
                            e = Encoding.Unicode; // little endian encoding
                        else
                            e = Encoding.BigEndianUnicode; // big endian encoding if BOM is not set, or 0xFE-0xFF
                        break;

                    // eq. UTF32. In usage somewhere for transmission ? A bad idea !
                    case BACnetEnums.BacnetCharacterStringEncodings.CHARACTER_UCS4:
                        if ((buffer[offset] == 0xFF) && (buffer[offset + 1] == 0xFE) && (buffer[offset + 2] == 0) && (buffer[offset + 3] == 0))
                            e = Encoding.UTF32; // UTF32 little endian encoding
                        else
                            e = Encoding.GetEncoding(12001); // UTF32 big endian encoding if BOM is not set, or 0-0-0xFE-0xFF
                        break;

                    case BACnetEnums.BacnetCharacterStringEncodings.CHARACTER_ISO8859:
                        e = Encoding.GetEncoding(28591); // "iso-8859-1"
                        break;

                    // FIXME: somebody in Japan (or elsewhere) could help,test&validate if such devices exist ?
                    // http://cgproducts.johnsoncontrols.com/met_pdf/1201531.pdf?ref=binfind.com/web page 18
                    case BACnetEnums.BacnetCharacterStringEncodings.CHARACTER_MS_DBCS:
                        e = Encoding.GetEncoding("shift_jis");
                        break;

                    // FIXME: somebody in Japan (or elsewhere) could help,test&validate if such devices exist ?
                    // http://www.sljfaq.org/afaq/encodings.html
                    case BACnetEnums.BacnetCharacterStringEncodings.CHARACTER_JISX_0208:
                        e = Encoding.GetEncoding("shift_jis"); // maybe "iso-2022-jp" ?
                        break;

                    // unknown code (wrong code, experimental, ...) 
                    // decoded as ISO-8859-1 (removing controls) : displays certainly a strange content !
                    default:
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < length; i++)
                        {
                            char oneChar = (char)buffer[offset + i]; // byte to char on .NET : ISO-8859-1
                            if (char.IsSymbol(oneChar)) sb.Append(oneChar);
                        }
                        char_string = sb.ToString();
                        return true;
                }

                char_string = e.GetString(buffer, offset, (int)length);
            }
            catch
            {
                char_string = "string decoding error !";
            }

            return true; // always OK
        }
        public static int decode_bitstring(byte[] buffer, int offset, uint len_value, out BacnetBitString bit_string)
        {
            int len = 0;
            byte unused_bits = 0;
            uint i = 0;
            uint bytes_used = 0;

            bit_string = new BacnetBitString();
            bit_string.value = new byte[MAX_BITSTRING_BYTES];
            if (len_value > 0)
            {
                /* the first octet contains the unused bits */
                bytes_used = len_value - 1;
                if (bytes_used <= MAX_BITSTRING_BYTES)
                {
                    len = 1;
                    for (i = 0; i < bytes_used; i++)
                    {
                        bitstring_set_octet(ref bit_string, (byte)i, byte_reverse_bits(buffer[offset + len++]));
                    }
                    unused_bits = (byte)(buffer[offset] & 0x07);
                    bitstring_set_bits_used(ref bit_string, (byte)bytes_used, unused_bits);
                }
            }

            return len;
        }
        private static byte byte_reverse_bits(byte in_byte)
        {
            byte out_byte = 0;

            if ((in_byte & 1) > 0)
            {
                out_byte |= 0x80;
            }
            if ((in_byte & 2) > 0)
            {
                out_byte |= 0x40;
            }
            if ((in_byte & 4) > 0)
            {
                out_byte |= 0x20;
            }
            if ((in_byte & 8) > 0)
            {
                out_byte |= 0x10;
            }
            if ((in_byte & 16) > 0)
            {
                out_byte |= 0x8;
            }
            if ((in_byte & 32) > 0)
            {
                out_byte |= 0x4;
            }
            if ((in_byte & 64) > 0)
            {
                out_byte |= 0x2;
            }
            if ((in_byte & 128) > 0)
            {
                out_byte |= 1;
            }

            return out_byte;
        }
        private static bool bitstring_set_bits_used(ref BacnetBitString bit_string, byte bytes_used, byte unused_bits)
        {
            bool status = false;

            /* FIXME: check that bytes_used is at least one? */
            bit_string.bits_used = (byte)(bytes_used * 8);
            bit_string.bits_used -= unused_bits;
            status = true;

            return status;
        }
        private static bool bitstring_set_octet(ref BacnetBitString bit_string, byte index, byte octet)
        {
            bool status = false;

            if (index < MAX_BITSTRING_BYTES)
            {
                bit_string.value[index] = octet;
                status = true;
            }

            return status;

        }
        public static int decode_date_safe(byte[] buffer, int offset, uint len_value, out DateTime bdate)
        {
            if (len_value != 4)
            {
                bdate = new DateTime(1, 1, 1);
                return (int)len_value;
            }
            else
            {
                return decode_date(buffer, offset, out bdate);
            }
        }
        public static int decode_date(byte[] buffer, int offset, out DateTime bdate)
        {
            int year = (ushort)(buffer[offset] + 1900);
            int month = buffer[offset + 1];
            int day = buffer[offset + 2];
            //int wday = buffer[offset + 3];

            if (month == 0xFF && day == 0xFF /*&& wday == 0xFF */&& (year - 1900) == 0xFF)
                bdate = new DateTime(1, 1, 1);
            else
                bdate = new DateTime(year, month, day);

            return 4;
        }
        public static int decode_bacnet_time_safe(byte[] buffer, int offset, uint len_value, out DateTime btime)
        {
            if (len_value != 4)
            {
                btime = new DateTime(1, 1, 1);
                return (int)len_value;
            }
            else
            {
                return decode_bacnet_time(buffer, offset, out btime);
            }
        }
        public static int decode_bacnet_time(byte[] buffer, int offset, out DateTime btime)
        {
            int hour = buffer[offset + 0];
            int min = buffer[offset + 1];
            int sec = buffer[offset + 2];
            int hundredths = buffer[offset + 3];
            if (hour == 0xFF && min == 0xFF && sec == 0xFF && hundredths == 0xFF)
                btime = new DateTime(1, 1, 1);
            else
            {
                if (hundredths > 100) hundredths = 0;   // sometimes set to 255
                btime = new DateTime(1, 1, 1, hour, min, sec, hundredths * 10);
            }
            return 4;
        }
        public static int decode_object_id_safe(byte[] buffer, int offset, uint len_value, out ushort object_type, out uint instance)
        {
            if (len_value != 4)
            {
                object_type = 0;
                instance = 0;
                return 0;
            }
            else
            {
                return decode_object_id(buffer, offset, out object_type, out instance);
            }
        }
        public static int decode_unsigned32(byte[] buffer, int offset, out uint value)
        {
            value = ((uint)((((uint)buffer[offset + 0]) << 24) & 0xff000000));
            value |= ((uint)((((uint)buffer[offset + 1]) << 16) & 0x00ff0000));
            value |= ((uint)((((uint)buffer[offset + 2]) << 8) & 0x0000ff00));
            value |= ((uint)(((uint)buffer[offset + 3]) & 0x000000ff));
            return 4;
        }

        public static int decode_unsigned24(byte[] buffer, int offset, out uint value)
        {
            value = ((uint)((((uint)buffer[offset + 0]) << 16) & 0x00ff0000));
            value |= ((uint)((((uint)buffer[offset + 1]) << 8) & 0x0000ff00));
            value |= ((uint)(((uint)buffer[offset + 2]) & 0x000000ff));
            return 3;
        }

        public static int decode_unsigned16(byte[] buffer, int offset, out ushort value)
        {
            value = ((ushort)((((uint)buffer[offset + 0]) << 8) & 0x0000ff00));
            value |= ((ushort)(((uint)buffer[offset + 1]) & 0x000000ff));
            return 2;
        }
        public static int decode_signed8(byte[] buffer, int offset, out sbyte value)
        {
            value = (sbyte)buffer[offset + 0];
            return 1;
        }
        public static int decode_signed32(byte[] buffer, int offset, out int value)
        {
            value = ((int)((((int)buffer[offset + 0]) << 24) & 0xff000000));
            value |= ((int)((((int)buffer[offset + 1]) << 16) & 0x00ff0000));
            value |= ((int)((((int)buffer[offset + 2]) << 8) & 0x0000ff00));
            value |= ((int)(((int)buffer[offset + 3]) & 0x000000ff));
            return 4;
        }

        public static int decode_signed24(byte[] buffer, int offset, out int value)
        {
            value = ((int)((((int)buffer[offset + 0]) << 16) & 0x00ff0000));
            value |= ((int)((((int)buffer[offset + 1]) << 8) & 0x0000ff00));
            value |= ((int)(((int)buffer[offset + 2]) & 0x000000ff));
            return 3;
        }

        public static int decode_signed16(byte[] buffer, int offset, out short value)
        {
            value = ((short)((((int)buffer[offset + 0]) << 8) & 0x0000ff00));
            value |= ((short)(((int)buffer[offset + 1]) & 0x000000ff));
            return 2;
        }


        public static int decode_real_safe(byte[] buffer, int offset, uint len_value, out float value)
        {
            if (len_value != 4)
            {
                value = 0.0f;
                return (int)len_value;
            }
            else
            {
                return decode_real(buffer, offset, out value);
            }
        }
        public static int decode_real(byte[] buffer, int offset, out float value)
        {
            byte[] tmp = new byte[] { buffer[offset + 3], buffer[offset + 2], buffer[offset + 1], buffer[offset + 0] };
            value = BitConverter.ToSingle(tmp, 0);
            return 4;
        }
        public static int decode_double(byte[] buffer, int offset, out double value)
        {
            byte[] tmp = new byte[] { buffer[offset + 7], buffer[offset + 6], buffer[offset + 5], buffer[offset + 4], buffer[offset + 3], buffer[offset + 2], buffer[offset + 1], buffer[offset + 0] };
            value = BitConverter.ToDouble(tmp, 0);
            return 8;
        }

        public static int decode_double_safe(byte[] buffer, int offset, uint len_value, out double value)
        {
            if (len_value != 8)
            {
                value = 0.0f;
                return (int)len_value;
            }
            else
            {
                return decode_double(buffer, offset, out value);
            }
        }
        ///
        //////////////////////////////////////////////////////////////////////////////////////////
        public static int bacapp_decode_context_application_data(byte[] buffer, int offset, int max_offset, BACnetEnums.BacnetObjectTypes object_type, BACnetEnums.BacnetPropertyIds property_id, out BacnetValue value)
        {
            int len = 0;
            int tag_len = 0;
            byte tag_number = 0;
            byte sub_tag_number = 0;
            uint len_value_type = 0;

            value = new BacnetValue();

            if (IS_CONTEXT_SPECIFIC(buffer[offset]))
            {
                ////this seems to be a strange way to determine object encodings
                if (property_id == BACnetEnums.BacnetPropertyIds.PROP_LIST_OF_GROUP_MEMBERS)
                {
                    //BacnetReadAccessSpecification v;
                    //tag_len = decode_read_access_specification(buffer, offset, max_offset, out v);
                    //if (tag_len < 0) return -1;
                    //value.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_READ_ACCESS_SPECIFICATION;
                    //value.Value = v;
                    //return tag_len;
                    return -1;
                }
                else if (property_id == BACnetEnums.BacnetPropertyIds.PROP_ACTIVE_COV_SUBSCRIPTIONS)
                {
                    //BacnetCOVSubscription v;
                    //tag_len = decode_cov_subscription(buffer, offset, max_offset, out v);
                    //if (tag_len < 0) return -1;
                    //value.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_COV_SUBSCRIPTION;
                    //value.Value = v;
                    //return tag_len;
                    return -1;
                }
                else if (object_type == BACnetEnums.BacnetObjectTypes.OBJECT_GROUP && property_id == BACnetEnums.BacnetPropertyIds.PROP_PRESENT_VALUE)
                {
                    BacnetReadAccessResult v;
                    tag_len = decode_read_access_result(buffer, offset, max_offset, out v);
                    if (tag_len < 0) return -1;
                    value.Tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_READ_ACCESS_RESULT;
                    value.Value = v;
                    return tag_len;
                }
                else if ((property_id == BACnetEnums.BacnetPropertyIds.PROP_LIST_OF_OBJECT_PROPERTY_REFERENCES) || (property_id == BACnetEnums.BacnetPropertyIds.PROP_LOG_DEVICE_OBJECT_PROPERTY))
                {
                    //BacnetDeviceObjectPropertyReference v;
                    //tag_len = decode_device_obj_property_ref(buffer, offset, max_offset, out v);
                    //if (tag_len < 0) return -1;
                    //value.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_PROPERTY_REFERENCE;
                    //value.Value = v;
                    //return tag_len;
                    return -1;
                }
                else if (property_id == BACnetEnums.BacnetPropertyIds.PROP_DATE_LIST)
                {
                    //BACnetCalendarEntry v = new BACnetCalendarEntry();
                    //tag_len = v.ASN1decode(buffer, offset, (uint)max_offset);
                    //if (tag_len < 0) return -1;
                    //value.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_CONTEXT_SPECIFIC_DECODED;
                    //value.Value = v;
                    //return tag_len;
                    return -1;
                }
                else if (property_id == BACnetEnums.BacnetPropertyIds.PROP_SUBORDINATE_LIST)
                {
                    //BacnetDeviceObjectReference v = new BacnetDeviceObjectReference();
                    //tag_len = v.ASN1decode(buffer, offset, (uint)max_offset);
                    //if (tag_len < 0) return -1;
                    //value.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_DEVICE_OBJECT_REFERENCE;
                    //value.Value = v;
                    //return tag_len;
                    return -1;
                }

                else if (property_id == BACnetEnums.BacnetPropertyIds.PROP_EVENT_TIME_STAMPS)
                {

                    decode_tag_number_and_value(buffer, offset + len, out tag_number, out len_value_type);
                    len++; // skip Tag

                    if (tag_number == 0) // Time without date
                    {
                        DateTime dt;
                        len += decode_bacnet_time(buffer, offset + len, out dt);
                        value.Tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_TIMESTAMP;
                        value.Value = dt;
                    }
                    else if (tag_number == 1) // sequence number
                    {
                        uint val;
                        len += decode_unsigned(buffer, offset + len, len_value_type, out val);
                        value.Tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                        value.Value = val;
                    }
                    else if (tag_number == 2) // date + time
                    {
                        DateTime dt;
                        len += decode_bacnet_datetime(buffer, offset + len, out dt);
                        value.Tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_TIMESTAMP;
                        len++;  // closing Tag
                        value.Value = dt;
                    }
                    else
                        return -1;

                    return len;
                }

                value.Tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_CONTEXT_SPECIFIC_DECODED;
                List<BacnetValue> list = new List<BacnetValue>();

                decode_tag_number_and_value(buffer, offset + len, out tag_number, out len_value_type);
                // If an opening tag is not present, no loop to get the values
                bool MultiplValue = IS_OPENING_TAG(buffer[offset + len]);

                while (((len + offset) <= max_offset) && !IS_CLOSING_TAG(buffer[offset + len]))
                {
                    tag_len = decode_tag_number_and_value(buffer, offset + len, out sub_tag_number, out len_value_type);
                    if (tag_len < 0) return -1;

                    if (len_value_type == 0)
                    {
                        BacnetValue sub_value;
                        len += tag_len;
                        tag_len = bacapp_decode_application_data(buffer, offset + len, max_offset, BACnetEnums.BacnetObjectTypes.MAX_BACNET_OBJECT_TYPE, BACnetEnums.BacnetPropertyIds.MAX_BACNET_PROPERTY_ID, out sub_value);
                        if (tag_len < 0) return -1;
                        list.Add(sub_value);
                        len += tag_len;
                    }
                    else
                    {
                        BacnetValue sub_value = new BacnetValue();

                        //override tag_number
                        BACnetEnums.BacnetApplicationTags override_tag_number = bacapp_context_tag_type(property_id, sub_tag_number);
                        if (override_tag_number != BACnetEnums.BacnetApplicationTags.MAX_BACNET_APPLICATION_TAG) sub_tag_number = (byte)override_tag_number;

                        //try app decode
                        int sub_tag_len = bacapp_decode_data(buffer, offset + len + tag_len, max_offset, (BACnetEnums.BacnetApplicationTags)sub_tag_number, len_value_type, out sub_value);
                        if (sub_tag_len == (int)len_value_type)
                        {
                            list.Add(sub_value);
                            len += tag_len + (int)len_value_type;
                        }
                        else
                        {
                            //fallback to copy byte array
                            byte[] context_specific = new byte[(int)len_value_type];
                            Array.Copy(buffer, offset + len + tag_len, context_specific, 0, (int)len_value_type);
                            sub_value = new BacnetValue(BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_CONTEXT_SPECIFIC_ENCODED, context_specific);

                            list.Add(sub_value);
                            len += tag_len + (int)len_value_type;
                        }
                    }

                    if (MultiplValue == false)
                    {
                        value = list[0];
                        return len;
                    }
                }
                if ((len + offset) > max_offset) return -1;

                //end tag
                if (decode_is_closing_tag_number(buffer, offset + len, tag_number))
                    len++;

                //context specifique is array of BACNET_VALUE
                value.Value = list.ToArray();
            }
            else
            {
                return -1;
            }

            return len;
        }
        private static BACnetEnums.BacnetApplicationTags bacapp_context_tag_type(BACnetEnums.BacnetPropertyIds property, byte tag_number)
        {
            BACnetEnums.BacnetApplicationTags tag = BACnetEnums.BacnetApplicationTags.MAX_BACNET_APPLICATION_TAG;

            switch (property)
            {
                case BACnetEnums.BacnetPropertyIds.PROP_ACTUAL_SHED_LEVEL:
                case BACnetEnums.BacnetPropertyIds.PROP_REQUESTED_SHED_LEVEL:
                case BACnetEnums.BacnetPropertyIds.PROP_EXPECTED_SHED_LEVEL:
                    switch (tag_number)
                    {
                        case 0:
                        case 1:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                            break;
                        case 2:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_REAL;
                            break;
                        default:
                            break;
                    }
                    break;
                case BACnetEnums.BacnetPropertyIds.PROP_ACTION:
                    switch (tag_number)
                    {
                        case 0:
                        case 1:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID;
                            break;
                        case 2:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED;
                            break;
                        case 3:
                        case 5:
                        case 6:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                            break;
                        case 7:
                        case 8:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_BOOLEAN;
                            break;
                        case 4:        /* propertyValue: abstract syntax */
                        default:
                            break;
                    }
                    break;
                case BACnetEnums.BacnetPropertyIds.PROP_LIST_OF_GROUP_MEMBERS:
                    /* Sequence of ReadAccessSpecification */
                    switch (tag_number)
                    {
                        case 0:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID;
                            break;
                        default:
                            break;
                    }
                    break;
                case BACnetEnums.BacnetPropertyIds.PROP_EXCEPTION_SCHEDULE:
                    switch (tag_number)
                    {
                        case 1:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID;
                            break;
                        case 3:
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                            break;
                        case 0:        /* calendarEntry: abstract syntax + context */
                        case 2:        /* list of BACnetTimeValue: abstract syntax */
                        default:
                            break;
                    }
                    break;
                case BACnetEnums.BacnetPropertyIds.PROP_LOG_DEVICE_OBJECT_PROPERTY:
                    switch (tag_number)
                    {
                        case 0:        /* Object ID */
                        case 3:        /* Device ID */
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID;
                            break;
                        case 1:        /* Property ID */
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED;
                            break;
                        case 2:        /* Array index */
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                            break;
                        default:
                            break;
                    }
                    break;
                case BACnetEnums.BacnetPropertyIds.PROP_SUBORDINATE_LIST:
                    /* BACnetARRAY[N] of BACnetDeviceObjectReference */
                    switch (tag_number)
                    {
                        case 0:        /* Optional Device ID */
                        case 1:        /* Object ID */
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID;
                            break;
                        default:
                            break;
                    }
                    break;

                case BACnetEnums.BacnetPropertyIds.PROP_RECIPIENT_LIST:
                    /* List of BACnetDestination */
                    switch (tag_number)
                    {
                        case 0:        /* Device Object ID */
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID;
                            break;
                        default:
                            break;
                    }
                    break;
                case BACnetEnums.BacnetPropertyIds.PROP_ACTIVE_COV_SUBSCRIPTIONS:
                    /* BACnetCOVSubscription */
                    switch (tag_number)
                    {
                        case 0:        /* BACnetRecipientProcess */
                        case 1:        /* BACnetObjectPropertyReference */
                            break;
                        case 2:        /* issueConfirmedNotifications */
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_BOOLEAN;
                            break;
                        case 3:        /* timeRemaining */
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                            break;
                        case 4:        /* covIncrement */
                            tag = BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_REAL;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return tag;
        }
        public static int decode_bacnet_datetime(byte[] buffer, int offset, out DateTime bdatetime)
        {
            int len = 0;
            DateTime date;
            len += decode_application_date(buffer, offset + len, out date); // Date
            DateTime time;
            len += decode_application_time(buffer, offset + len, out time); // Time
            bdatetime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
            return len;
        }
        public static int decode_application_date(byte[] buffer, int offset, out DateTime bdate)
        {
            int len = 0;
            byte tag_number;
            decode_tag_number(buffer, offset + len, out tag_number);

            if (tag_number == (byte)BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_DATE)
            {
                len++;
                len += decode_date(buffer, offset + len, out bdate);
            }
            else
            {
                bdate = new DateTime(1, 1, 1);
                len = -1;
            }
            return len;
        }
        public static int decode_application_time(byte[] buffer, int offset, out DateTime btime)
        {
            int len = 0;
            byte tag_number;
            decode_tag_number(buffer, offset + len, out tag_number);

            if (tag_number == (byte)BACnetEnums.BacnetApplicationTags.BACNET_APPLICATION_TAG_TIME)
            {
                len++;
                len += decode_bacnet_time(buffer, offset + len, out btime);
            }
            else
            {
                btime = new DateTime(1, 1, 1);
                len = -1;
            }
            return len;
        }

    }

    public static partial class IPFrameFactory
    {

        public static BACnetIPFrame ReadProp(BACnetStation Station, ServiceTagList inTagList)
        {
            BACnetIPFrame BACnetIPFrame = null;
            if (inTagList == null || inTagList.Count() == 0)
            {
                return null;
            }
            if (inTagList.Count() == 1)
            {
                BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.READ_PROPERTY);
                BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(((BACnetStation)Station).DeviceIdentifier, 0));
                BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(((ServiceTag)inTagList.List[0]).UInt, 1));
                BACnetIPFrame.Npdu.Apdu.SA = true;
                BACnetIPFrame.Npdu.Apdu.MaxSegs = (byte)BACnetEnums.BacnetMaxSegments.MAX_SEG65;
                BACnetIPFrame.Npdu.Apdu.MaxResp = (byte)BACnetEnums.BacnetMaxAdpu.MAX_APDU1476;
                BACnetIPFrame.Npdu.Apdu.InvokeId = 1;
            }
            else
            {
                BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.READ_PROP_MULTIPLE);
                BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(((BACnetStation)Station).DeviceIdentifier, 0));
                //add the list to the service tag 
                ServiceTag tag = new ServiceTag((ServiceTagList)inTagList, 1);
                //Added at Service request
                BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(tag);
            }
            if (Station.DestinationSpecifierPresent)
            {
                BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                BACnetIPFrame.Npdu.DNET = Station.DNET;
                BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                BACnetIPFrame.Npdu.DADR = Station.DADR;
            }
            return BACnetIPFrame;
        }
    }   
}
