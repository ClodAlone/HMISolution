#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SILVERLIGHT
namespace Syncfusion.OlapSilverlight.MDXQueryParser
#else
namespace Syncfusion.Olap.MDXQueryParser
#endif
{
    /// <summary>
    /// All keyword constants which are used as names
    /// </summary>
    public class KeywordConstants
    {
        #region Public Constants
        public const string Columns = "COLUMNS";
        public const string ColumnsNum = "0";
        public const string Cell = "CELL";
        public const string CrossJoin = "CROSSJOIN";
        public const string DrillDownLevel = "DRILLDOWNLEVEL";
        public const string DrillDownMember = "DRILLDOWNMEMBER";
        public const string DimensionS = "DIMENSIONS";
        public const string Dimension = "DIMENSION";
        public const string Except = "EXCEPT";
        public const string Empty = "EMPTY";
        public const string Filter = "FILTER";
        public const string From = "FROM";
        public const string FormatString = "FORMAT_STRING";        
        public const string FormattedValue = "FORMATTED_VALUE";        
        public const string Hierarchize = "HIERARCHIZE";
        public const string Measures = "MEASURES";
        public const string MemberType = "MEMBER_TYPE";
        public const string Members = "MEMBERS";
        public const string NamedSet = "NAMEDSET";
        public const string Non = "NON";
        public const string On = "ON";
        public const string Order = "ORDER";
        public const string Properties = "PROPERTIES";        
        public const string Parentheses = "PARAM";
        public const string ParentUniqueName = "PARENT_UNIQUE_NAME";
        public const string RowsNum = "1";
        public const string Rows = "ROWS";
        public const string Select = "SELECT";
        public const string SBracket = "SBRACKET";
        public const string TopCount = "TOPCOUNT";
        public const string Value = "VALUE";
        public const string With = "WITH";
        public const string Where = "WHERE";      
        #endregion

        #region new constants
        public const string WITH = "WITH";
        public const string SELECT = "SELECT";
        public const string NON = "NON";
        public const string EMPTY = "EMPTY";
        public const string NON_EMPTY = "NON EMPTY";
        public const string NONEMPTY = "NONEMPTY";
        public const string HIERARCHIZE = "HIERARCHIZE";
        public const string VISUALTOTALS = "VISUALTOTALS";
        public const string DRILLDOWNLEVEL = "DRILLDOWNLEVEL";
        public const string COLUMNS = "COLUMNS";
        public const string ROWS = "ROWS";
        public const string AXIS0 = "AXIS(0)";
        public const string AXIS1 = "AXIS(1)";
        public const string FROM = "FROM";
        public const string WHERE = "WHERE";
        public const string WS = " ";

        public const string DIMENSION_PROPERTIES = "DIMENSION PROPERTIES";
        public const string CELL_PROPERTIES = "CELL PROPERTIES";

        public const string TOPCOUNT = "TOPCOUNT";
        public const string DRILLDOWNMEMBER = "DRILLDOWNMEMBER";
        public const string CROSSJOIN = "CROSSJOIN";
        public const string EXCEPT = "EXCEPT";
        public const string NONEMPTYCROSSJOIN = "NONEMPTYCROSSJOIN";
        public const string MEASURES = "MEASURES";
        public const string SUBSET = "SUBSET";
        public const string ORDER = "ORDER";
        public const string FILTER = "FILTER";

        public const string UNION = "UNION";
        public const string INTERSECT = "INTERSECT";
        public const string DESCENDANTS = "DESCENDANTS";

        public const string MEMBER = "MEMBER ";

        public const string MEASURES_EXP = @"\[MEASURES\]\.\[[A-Z0-9 ]*\]";
        public const string DIMENSION_EXP = @"(\[[A-Z0-9 ]*\]\.*)+";

        public const string FILTER_EXP = "[>=|<=|!=|<>]";
        public const string QUOTED_ID = "^[[][\\w+]";

        public const char LBRACE = '{';
        public const char RBRACE = '}';
        public const char LPAREN = '(';
        public const char RPAREN = ')';
        public const char RANGE_IDENTIFIER = ':';
        public const char DOT = '.';
        public const char ASTERISK = '*';
        public const char COMMA = ',';
        #endregion
    }
}
