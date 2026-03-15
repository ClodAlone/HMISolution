//-------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Windows.Reports.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Interaction logic for accessing SQL related informations
    /// </summary>
    internal class SqlUtil
    {
        #region Internal Methods
        /// <summary>
        /// Get the Quote identifier for the specified expression.
        /// </summary>
        /// <param name="expression">Represents the input expression as string</param>
        /// <returns>String contains expressions with quote identifiers</returns>
        internal static string QuoteIdentifier(string expression)
        {
            return "[" + expression + "]";
        }

        /// <summary>
        /// Remove the Quote identifier for the specified expression.
        /// </summary>
        /// <param name="expression">Represents the input expression as string</param>
        /// <returns>String contains expressions without quote identifiers</returns>
        internal static string RemoveQuoteIdentifier(string expression)
        {
            if (expression != null && (expression.StartsWith("[") && expression.EndsWith("]")))
            {
                return expression.Substring(1, expression.Length - 2);
            }

            return null;
        }

        /// <summary>
        /// Set Identifier for the speicfield filed name with the data set name.
        /// </summary>
        /// <param name="fieldName">Represents the field name</param>
        /// <param name="dataSetName">Represents the dataset name(table name)</param>
        /// <returns>String containing the field identifier inclusion</returns>
        internal static string FieldIdentifier(string fieldName, string dataSetName)
        {
            return "=First(Fields!" + fieldName + ".Value" + ", \"" + dataSetName + "\")";
        }

        /// <summary>
        /// To Truncate the protected informations of connection string to display.
        /// </summary>
        /// <param name="connectionString">Represents the input connection string</param>
        /// <returns>String contains the truncated connection string</returns>
        internal static string TruncateConnectionSring(string connectionString)
        {
            string[] s = connectionString.Split(';');
            string stemp = string.Empty;
            for (int i = 0; i < s.Length; i++)
            {
                if (i <= 1)
                {
                    if (i > 0)
                    {
                        stemp += ";";
                    }

                    stemp += s[i];
                }
                else
                {
                    break;
                }
            }

            return stemp;
        }

        /// <summary>
        /// To include the integrated security stuffs with input connection string
        /// </summary>
        /// <param name="connectionString">Represents the connection string</param>
        /// <param name="integratedSecurity">Represents whether Integerated Security is On</param>
        /// <returns>String contains with the integerated security stuffs</returns>
        internal static string IncludeIntegSecurity(string connectionString, bool integratedSecurity)
        {                   
            if (connectionString != null)
            {
                if (integratedSecurity)
                {
                    connectionString += ";Integrated Security=true";
                }
                else
                {
                    connectionString += ";Integrated Security=false;";
                }
            }
            
            return connectionString;
        }
        #endregion
    }
}
