#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WinRT
using System.Reflection;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public delegate bool TryParseMethod<T>(string input, out T value);


    public interface ITryParser
    {
        bool TryParse(string input, out object value);
    }

    public class TryParser<T> : ITryParser
    {
        private TryParseMethod<T> ParsingMethod;

        public TryParser(TryParseMethod<T> parsingMethod)
        {
            this.ParsingMethod = parsingMethod;
        }

        public bool TryParse(string input, out object value)
        {
            T parsedOutput;
            bool success = ParsingMethod(input, out parsedOutput);
            value = parsedOutput;
            return success;
        }
    }
 
    public static class TypeConverterHelper
    {
        private static Dictionary<Type, ITryParser> Parsers;

        static TypeConverterHelper()
        {
            Parsers = new Dictionary<Type, ITryParser>();
            AddParser<int>(int.TryParse);
            AddParser<uint>(uint.TryParse);
            AddParser<double>(double.TryParse);
            AddParser<decimal>(decimal.TryParse);
            AddParser<bool>(bool.TryParse);
            AddParser<string>((string input, out string value) => { value = input; return true; });
            AddParser<DateTime>(DateTime.TryParse);
            AddParser<TimeSpan>(TimeSpan.TryParse);
            AddParser<long>(long.TryParse);
            AddParser<ulong>(ulong.TryParse);
            AddParser<short>(short.TryParse);
            AddParser<ushort>(ushort.TryParse);
            AddParser<byte>(byte.TryParse);
            AddParser<sbyte>(sbyte.TryParse);
            AddParser<float>(float.TryParse);
        }


        public static void AddParser<T>(TryParseMethod<T> parseMethod)
        {
            Parsers.Add(typeof(T), new TryParser<T>(parseMethod));
        }

        public static bool Convert<T>(string input, out T value)
        {
            object parseResult;
            bool success = Convert(typeof(T), input, out parseResult);
            if (success)
                value = (T)parseResult;
            else
                value = default(T);
            return success;
        }

        public static bool Convert(Type type, string input, out object value)
        {
            ITryParser parser;
            if (Parsers.TryGetValue(type, out parser))
                return parser.TryParse(input, out value);
            else
                throw new NotSupportedException(String.Format("The specified type \"{0}\" is not supported.", type.FullName));
        }

        public static bool CanConvert(Type type, string input)
        {
            ITryParser parser;
            object value;
            if (IsNullableType(type))
            {
                if (input == null)
                    return true;
                else
                    type = Nullable.GetUnderlyingType(type);
            }

            if (Parsers.TryGetValue(type, out parser))
                return parser.TryParse(input, out value);
            else
                return false;
        }
      
        public static bool IsNullableType(Type nullableType)
        {
            if (nullableType == null)
            {
                throw new ArgumentNullException("nullableType");
            }
#if WinRT
            bool result = (nullableType.GetTypeInfo().IsGenericType && !nullableType.GetTypeInfo().IsGenericTypeDefinition) &&
                        (nullableType.GetGenericTypeDefinition() == typeof(Nullable<>));
#else
            bool result = (nullableType.IsGenericType && !nullableType.IsGenericTypeDefinition) &&
                          (nullableType.GetGenericTypeDefinition() == typeof(Nullable<>));
#endif
            return result;
        }
    }
}
