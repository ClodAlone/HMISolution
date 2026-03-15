#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Data.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.ComponentModel;
    using System.Reflection;

    public static class StringExtensions
    {
        public static string FormatByName(this string format, IDictionary<string, object> values)
        {
            return FormatByName(format, null, values);
        }

        public static string FormatByName(this string format, IFormatProvider provider,
                                          IDictionary<string, object> values)
        {
            return FormatByName(format, provider, (key) =>
                {
                    if (values.ContainsKey(key))
                    {
                        return values[key];
                    }

                    return null;
                });
        }

        public static string FormatByName(this string format, IFormatProvider provider,
                                          Func<string, object> valueProvider)
        {
            string expression = @"(?<=\{)(?<key>\w.*?)(?=\}|,|:)";
            string resultString = string.Empty;

            List<string> args = new List<string>();
            List<object> valuesInOrder = new List<object>();
            resultString = Regex.Replace(format, expression, m =>
                {
                    int index = -1;

                    string keyName = m.Groups["key"].Value;
                    var value = valueProvider(keyName);
                    if (value != null)
                    {
                        if (!args.Contains(keyName))
                        {
                            args.Add(keyName);
                            valuesInOrder.Add(value);
                        }

                        index = args.IndexOf(keyName);
                    }

                    return index.ToString();
                }, RegexOptions.IgnoreCase);

            resultString = Regex.Replace(resultString, @"{-1(\:\w?\})", m => { return string.Empty; },
                                         RegexOptions.IgnoreCase);

            if (resultString.Contains("{-1}"))
            {
                resultString = resultString.Replace("{-1}", string.Empty);
            }

            return string.Format(provider, resultString, valuesInOrder.ToArray());
        }

#if WPF
        public static void ParseFormat(this string value, bool raiseException, Dictionary<string, PropertyDescriptor> pdc, out string result, out PropertyDescriptor[] pds)
#else
        public static void ParseFormat(this string value, bool raiseException, Dictionary<string, PropertyInfo> pdc,
                                       out string result, out PropertyInfo[] pds)
#endif
        {
#if WPF
            List<PropertyDescriptor> al = new List<PropertyDescriptor>();
#else
            var al = new List<PropertyInfo>();
#endif
            int n1 = value.IndexOf("{");
            StringBuilder sb = new StringBuilder();
            int n2 = 0;
            if (n1 == -1)
            {
                sb.Append(value);
            }
            else
            {
                sb.Append(value.Substring(0, n1 + 1));
            }

            while (n1 != -1)
            {
                n2 = value.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = value.IndexOfAny(new char[] {'}', ':'}, n1);
                    string name = value.Substring(n1 + 1, n3 - n1 - 1);
                    var pd = pdc.ContainsKey(name) ? pdc[name] : null;
                    if (pd != null)
                    {
                        sb.Append(al.Count.ToString());
                        al.Add(pd);
                    }
                    else
                    {
                        sb.Append(name);
                    }
                    //else
                    //{
                    //    if (raiseException)
                    //        throw new FormatException("Property not found: " + name);
                    //    else
                    //    {
                    //        result = string.Empty;
                    //        pds = new PropertyDescriptor[0];
                    //        return;
                    //    }
                    //}
                    n1 = value.IndexOf("{", n2);
                    if (n1 == -1)
                    {
                        sb.Append(value.Substring(n3));
                    }
                    else
                    {
                        sb.Append(value.Substring(n3, n1 - n3 + 1));
                    }
                }
                else
                {
                    if (raiseException)
                    {
                        throw new FormatException("No closing char found: " + sb.ToString());
                    }
                    else
                    {
                        result = string.Empty;
#if WPF
                        pds = new PropertyDescriptor[0];
#else
                        pds = new PropertyInfo[0];
#endif
                        return;
                    }
                }
            }
            result = sb.ToString();
            pds = al.ToArray();
        }
    }
}