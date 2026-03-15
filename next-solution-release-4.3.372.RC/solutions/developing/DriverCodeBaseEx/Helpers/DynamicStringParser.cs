////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Helpers\DynamicStringParser.cs
//
// summary:	Implements the dynamic string parser class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverCodeBaseEx.Helpers
{
    /// <summary>   A dynamic string parser. </summary>
    public class DynamicStringParser
    {
        /// <summary>   Options for controlling the operation. </summary>
        private readonly Dictionary<string, string> Parameters;
        /// <summary>   Name of the driver. </summary>
        private readonly string DriverName;
        /// <summary>   The character driver. </summary>
        public static readonly Char CharDriver = '.';
        /// <summary>   The character separator. </summary>
        public static readonly Char CharSep = '|';
        /// <summary>   . </summary>
        public static readonly Char CharAssign = '=';
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="dynamicString" type="string">  The dynamic string. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DynamicStringParser(string dynamicString)
        {
            int index = dynamicString.IndexOf(CharDriver);
            if (index != -1)
            {
                DriverName = dynamicString.Substring(0, index);
                dynamicString = dynamicString.Substring(index + 1);
            }

            var parameters = dynamicString.Split(CharSep);
            foreach (var parameter in parameters)
            {
                index = parameter.IndexOf(CharAssign);
                if (index <= 0)
                    continue;

                if (Parameters == null)
                    Parameters = new Dictionary<string, string>();

                Parameters[parameter.Substring(0, index)] = parameter.Substring(index + 1);
            }
        }

        public DynamicStringParser(string dynamicString, List<string> listParameterNames)
        {
            int index = dynamicString.IndexOf(CharDriver);
            if (index != -1)
            {
                DriverName = dynamicString.Substring(0, index);
                dynamicString = dynamicString.Substring(index + 1);
            }
            if (string.IsNullOrEmpty(DriverName))
                return;
            if (dynamicString.Length > 0)
            {
                string prefix;
                string value;
                int idx = -1;
                int i;
                do
                {
                    for (i = 0; i < listParameterNames.Count; i++)
                    {
                        idx = dynamicString.IndexOf(string.Format("{0}=", listParameterNames[i]));
                        if (idx == 0)
                            break;
                    }
                    if (idx != 0)
                        break;

                    value = string.Empty;
                    prefix = listParameterNames[i];
                    listParameterNames.RemoveAt(i);
                    int idx_i = -1;
                    for (i = 0; i < listParameterNames.Count; i++)
                    {
                        int idx_a = dynamicString.IndexOf(string.Format("|{0}=", listParameterNames[i]));
                        if (idx_a != -1)
                        {
                            if (idx_i == -1 || idx_a < idx_i)
                                idx_i = idx_a;
                        }
                    }
                    if (idx_i > prefix.Length + 1 || (idx_i == -1 && dynamicString.Length > 0))
                    {
                        //found another paramname
                        value = dynamicString.Substring(prefix.Length + 1, (idx_i == -1 ? dynamicString.Length : idx_i) - prefix.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(value))
                    {
                        if (Parameters == null)
                            Parameters = new Dictionary<string, string>();
                        Parameters.Add(prefix, value);
                        dynamicString = (idx_i == -1 ? string.Empty : dynamicString.Substring(idx_i + 1));
                    }
                    else
                        break;

                } while (listParameterNames.Count > 0 || dynamicString.Length > 0);
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets driver name. </summary>
        ///
        /// <returns>   The driver name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetDriverName()
        {
            return DriverName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets dynamic string. </summary>
        ///
        /// <returns>   The dynamic string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetDynamicString()
        {
            if (Parameters != null)
            {
                StringBuilder result = new StringBuilder();
                foreach (var key in Parameters.Keys)
                    result.AppendFormat("{0}{1}{2}{3}", key, CharAssign, Parameters[key], CharSep);

                if (result.Length > 0)
                    return result.ToString(0, result.Length - 1);
            }

            return String.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        ///
        /// <returns>   The part by name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetPartByName(string partName)
        { 
            if (String.IsNullOrEmpty(partName))
                throw new ArgumentNullException("Argument cannot be null or empty");

            if (Parameters != null && Parameters.ContainsKey(partName))
                return Parameters[partName];

            return String.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="Boolean">  . </param>
        ///
        /// <returns>   The part by name. </returns>
        ///
        /// ### <param name="defvalue" type="Int16">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Boolean GetPartByName(string partName, Boolean defvalue)
        {
            Boolean value = defvalue;
            if (!Boolean.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="Int16">    . </param>
        ///
        /// <returns>   The part by name. </returns>
        ///
        /// ### <param name="defvalue" type="Int32">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Int16 GetPartByName(string partName, Int16 defvalue)
        {
            Int16 value = defvalue;
            if (!Int16.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="Int32">    . </param>
        ///
        /// <returns>   The part by name. </returns>
        ///
        /// ### <param name="defvalue" type="UInt16">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Int32 GetPartByName(string partName, Int32 defvalue)
        {
            Int32 value = defvalue;
            if (!Int32.TryParse(GetPartByName(partName), out value))
                return defvalue;
            
            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="UInt16">   . </param>
        ///
        /// <returns>   The part by name. </returns>
        ///
        /// ### <param name="defvalue" type="UInt32">   The defvalue. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt16 GetPartByName(string partName, UInt16 defvalue)
        {
            UInt16 value = defvalue;
            if (!UInt16.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="UInt32">   The defvalue. </param>
        ///
        /// <returns>   The part by name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt32 GetPartByName(string partName, UInt32 defvalue)
        {
            UInt32 value = defvalue;
            if (!UInt32.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets part by name. </summary>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        /// <param name="defvalue" type="Double">   The defvalue. </param>
        ///
        /// <returns>   The part by name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Double GetPartByName(string partName, Double defvalue)
        {
            Double value = defvalue;
            if (!Double.TryParse(GetPartByName(partName), out value))
                return defvalue;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the part by name described by partName. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="partName" type="string">   Name of the part. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RemovePartByName(string partName)
        { 
            if (String.IsNullOrEmpty(partName))
                throw new ArgumentNullException("argument 'partName' cannot be null or empty");

            if (Parameters != null)
                return Parameters.Remove(partName);

            return false;
        }
    }
}
