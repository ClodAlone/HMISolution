using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderEditor.Helper
{
    public class SqlParser
    {
        #region Constructors
        public SqlParser()
        { }
        #endregion

        #region Properties
        string tableName;
        public string TableName
        {
            get
            {
                return tableName;
            }
        }

        string[] selectColumnNames;
        public string[] SelectColumnNames
        {
            get
            {
                return selectColumnNames;
            }
        }

        string topClause;
        public string TopClause
        {
            get
            {
                return topClause;
            }
        }
        #endregion

        #region Methods
        public void Parse(string query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            query = query.Trim();
            var lowerQuery = query.ToLower();
            if (lowerQuery.StartsWith("select "))
            {
                var indexLastColumn = lowerQuery.IndexOf(" from ");
                if (indexLastColumn != -1)
                {
                    tableName = query.Substring(indexLastColumn + 6).Trim();
                    var index = tableName.IndexOf(' ');
                    if (index != -1)
                        tableName = tableName.Substring(0, index);
                }

                var indexFirstColumn = lowerQuery.IndexOf(',');
                if (indexFirstColumn == -1 || indexFirstColumn > indexLastColumn)
                    indexFirstColumn = indexLastColumn;
                if (indexFirstColumn != -1)
                {
                    var select = lowerQuery.Substring(0, indexFirstColumn);
                    indexFirstColumn = select.LastIndexOf(' ');
                    if (indexFirstColumn != -1)
                    {
                        selectColumnNames = query.Substring(indexFirstColumn, indexLastColumn - indexFirstColumn).Split(',');
                        for (int ii = 0; ii < selectColumnNames.Length; ii++)
                            selectColumnNames[ii] = selectColumnNames[ii].Trim();
                    }
                }

                var indexTop = lowerQuery.IndexOf(" top ");
                if (indexTop != -1)
                {
                    indexTop += 5;
                    var index = lowerQuery.IndexOf(' ', indexTop);
                    if (index > indexTop)
                        topClause = lowerQuery.Substring(indexTop, index - indexTop);
                }
            }
        }
        #endregion
    }
}
