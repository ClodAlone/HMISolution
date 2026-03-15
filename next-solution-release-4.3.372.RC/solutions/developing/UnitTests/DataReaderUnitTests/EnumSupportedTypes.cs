using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderUnitTests
{
    public enum EnumSupportedTypes
    {
        None = 0x00000,
        Boolean = 0x000001,
        Byte = 0x0000002,
        SByte = 0x000004,
        UInt16 = 0x000008,
        Int16 = 0x000010,
        UInt32 = 0x000020,
        Int32 = 0x000040,
        UInt64 = 0x000080,
        Int64 = 0x000100,
        Decimal = 0x000200,
        Single = 0x000400,
        Double = 0x000800,
        String = 0x001000,
        DateTime = 0x002000,

        Uncheck8Bit = 0xFFFFF9,
        Uncheck16Bit = 0xFFFFE7,
        Uncheck32Bit = 0xFFFF9F,
        Uncheck64Bit = 0xFFFE7F,
        UncheckFloattingPoint = 0xFF31FF,
        UncheckString = 0xFF2FFF,
        UncheckDateTime = 0xFF1FFF,

        CheckAll = 0xFFFFFF,
    }
}
