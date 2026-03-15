using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace AutoNotifyPropertyChange
{
    class PropertyDependencyAnalyzer : IEnumerable<MethodBase>
    {
        Byte[] bytes;
        Int32 pos;
        MethodBase method;
        static OpCode[] smallOpCodes = new OpCode[0x100];
        static OpCode[] largeOpCodes = new OpCode[0x100];
        static PropertyDependencyAnalyzer()
        {
            foreach (FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OpCode opCode = (OpCode)fi.GetValue(null);
                UInt16 value = (UInt16)opCode.Value;
                if (value < 0x100)
                    smallOpCodes[value] = opCode;
                else if ((value & 0xff00) == 0xfe00)
                    largeOpCodes[value & 0xff] = opCode;
            }
        }


        public static Dictionary<string, List<string>> GetPropertyInfluences(Type t)
        {
            Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();
            try
            {
                foreach (PropertyInfo pi in t.GetProperties().Where(q => q.IsVirtual()))
                {
                    var analyzer = new PropertyDependencyAnalyzer(pi.GetGetMethod());
                    foreach (var methodBase in analyzer)
                    {
                        if (methodBase.DeclaringType == t && methodBase.IsSpecialName && methodBase.Name.StartsWith("get_"))
                        {
                            // property dependency found
                            StoreFound(map, pi.Name, methodBase.Name.Substring(4));
                        }
                    }
                }
            }
            catch 
            { 
                //ignore IL exception
            }
            return map;
        }

        private static void StoreFound(Dictionary<string, List<string>> map, string influenced, string by)
        {
            if (!map.ContainsKey(by))
            {
                map[by] = new List<string>();
            }
            if (!map[by].Contains(influenced))
                map[by].Add(influenced);
        }
        Module module;
        private PropertyDependencyAnalyzer(MethodBase enclosingMethod)
        {
            this.method = enclosingMethod;
            module = method.DeclaringType.Assembly.GetModules().Where(m=>m.GetTypes().Any(t=>t==method.DeclaringType)).FirstOrDefault();
            MethodBody methodBody = method.GetMethodBody();
            this.bytes = (methodBody == null) ? new Byte[0] : methodBody.GetILAsByteArray();
            this.pos = 0;
        }

        public IEnumerator<MethodBase> GetEnumerator()
        {
            while (pos < bytes.Length)
            {
                var x = Next();
                if (null == x)
                {
                    pos = 0;
                    yield break;
                }
                yield return x;
            }
            pos = 0;
            yield break;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

       MethodBase Next()
       {
           while (pos < bytes.Length)
           {
               Int32 offset = pos;
               OpCode opCode = OpCodes.Nop;
               Int32 token = 0;

               // read first 1 or 2 bytes as opCode
               Byte code = ReadByte();
               if (code != 0xFE)
                   opCode = smallOpCodes[code];
               else
               {
                   code = ReadByte();
                   opCode = largeOpCodes[code];
               }

               switch (opCode.OperandType)
               {
                   case OperandType.InlineNone:
                       continue;

                   case OperandType.ShortInlineBrTarget:
                       SByte shortDelta = ReadSByte();
                       continue;

                   case OperandType.InlineBrTarget: Int32 delta = ReadInt32(); continue;
                   case OperandType.ShortInlineI: Byte int8 = ReadByte(); continue;
                   case OperandType.InlineI: Int32 int32 = ReadInt32(); continue;
                   case OperandType.InlineI8: Int64 int64 = ReadInt64(); continue;
                   case OperandType.ShortInlineR: Single float32 = ReadSingle(); continue;
                   case OperandType.InlineR: Double float64 = ReadDouble(); continue;
                   case OperandType.ShortInlineVar: Byte index8 = ReadByte(); continue;
                   case OperandType.InlineVar: UInt16 index16 = ReadUInt16(); continue;
                   case OperandType.InlineString: token = ReadInt32(); continue;
                   case OperandType.InlineSig: token = ReadInt32(); continue;
                   case OperandType.InlineField: token = ReadInt32(); continue;
                   case OperandType.InlineType: token = ReadInt32(); continue;
                   case OperandType.InlineTok: token = ReadInt32(); continue;

                   case OperandType.InlineMethod:
                       token = ReadInt32();
                       return  module.ResolveMethod(token);

                   case OperandType.InlineSwitch:
                       Int32 cases = ReadInt32();
                       Int32[] deltas = new Int32[cases];
                       for (Int32 i = 0; i < cases; i++) deltas[i] = ReadInt32();
                       continue;

                   default:
                       throw new BadImageFormatException("unexpected OperandType " + opCode.OperandType);
               }
           }
           return null;
       }

      

        Byte ReadByte() { return (Byte)bytes[pos++]; }
        SByte ReadSByte() { return (SByte)ReadByte(); }

        UInt16 ReadUInt16() { pos += 2; return BitConverter.ToUInt16(bytes, pos - 2); }
        UInt32 ReadUInt32() { pos += 4; return BitConverter.ToUInt32(bytes, pos - 4); }
        UInt64 ReadUInt64() { pos += 8; return BitConverter.ToUInt64(bytes, pos - 8); }

        Int32 ReadInt32() { pos += 4; return BitConverter.ToInt32(bytes, pos - 4); }
        Int64 ReadInt64() { pos += 8; return BitConverter.ToInt64(bytes, pos - 8); }

        Single ReadSingle() { pos += 4; return BitConverter.ToSingle(bytes, pos - 4); }
        Double ReadDouble() { pos += 8; return BitConverter.ToDouble(bytes, pos - 8); }
    } 
}
