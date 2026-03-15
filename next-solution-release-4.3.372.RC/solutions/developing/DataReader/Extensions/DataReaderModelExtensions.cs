using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpoHelpers;

namespace DataReader.Extensions
{
    public static class DataReaderModelExtensions
    {
        public static bool IsValid(this DataReaderModel model)
        {
            return !String.IsNullOrEmpty(model.DataSourceName) &&
                    !String.IsNullOrEmpty(model.DataProvider) &&
                    !String.IsNullOrEmpty(model.Connection);
        }

        public static bool IsEmpty(this DataReaderModel model)
        {
            return /*String.IsNullOrEmpty(model.DataSourceName) &&*/
                    String.IsNullOrEmpty(model.DataProvider) &&
                    String.IsNullOrEmpty(model.Connection) &&
                    String.IsNullOrEmpty(model.Select) &&
                    String.IsNullOrEmpty(model.Where) &&
                    String.IsNullOrEmpty(model.Sort);
        }

        public static string GetFullConnection(this DataReaderModel model)
        {
            var conn = new StringBuilder();
            conn.AppendFormat("DataProvider={0}", model.DataProvider);
            if (!string.IsNullOrWhiteSpace(model.Connection))
                conn.AppendFormat(";{0}", model.Connection);

            return conn.ToString();
        }

        public static string GetFullQuery(this DataReaderModel model)
        {
            var query = new StringBuilder(model.Select);
            if (!string.IsNullOrWhiteSpace(model.Where))
                query.AppendFormat(" where {0}", model.WhereClause());
            if (!string.IsNullOrWhiteSpace(model.GroupBy))
                query.AppendFormat(" group by {0}", model.GroupBy);
            if (!string.IsNullOrWhiteSpace(model.Sort))
                query.AppendFormat(" order by {0}", model.Sort);

            return query.ToString();
        }

        public static string WhereClause(this DataReaderModel model)
        {
            var where = model.Where;
            if (!String.IsNullOrEmpty(where))
            {
                var index1 = where.IndexOf('#');
                var index2 = where.IndexOf('#', index1 + 1);
                while (index1 != -1 && index2 != -1 && index1 < index2)
                {
                    var datestring = where.Substring(index1 + 1, index2 - index1 - 1);
                    var dateindex1 = datestring.IndexOf('/');
                    var dateindex2 = datestring.IndexOf('/', dateindex1 + 1);
                    if (dateindex1 != -1 && dateindex2 != -1 && dateindex1 < index2)
                    {
                        datestring = String.Concat(datestring.Substring(dateindex2 + 1), "-", datestring.Substring(0, 2), "-", datestring.Substring(dateindex1 + 1, 2));
                        where = String.Concat(where.Substring(0, index1), "{ d '", datestring, "' }", where.Substring(index2 + 1));
                        index1 = where.IndexOf('#');
                        index2 = where.IndexOf('#', index1 + 1);
                    }
                }
            }

            return where;
        }

        const String searchSelectClause = "select ";

        public static void AddTopClause(this DataReaderModel model, int maxTake)
        {
            var select = model.Select;
            if (!String.IsNullOrEmpty(select))
            {
                var index = select.IndexOf(searchSelectClause, StringComparison.OrdinalIgnoreCase);
                if (index != -1)
                    model.Select = string.Format("{0} top {1} {2}", select.Substring(0, index + searchSelectClause.Length), maxTake, select.Substring(index + searchSelectClause.Length));
            }
        }
    }
}
