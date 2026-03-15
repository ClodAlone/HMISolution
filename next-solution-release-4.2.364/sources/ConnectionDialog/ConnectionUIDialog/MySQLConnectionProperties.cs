using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.ConnectionUI
{
    public class MySQLConnectionProperties : AdoDotNetConnectionProperties
    {
        public MySQLConnectionProperties()
            : base("MySql.Data.MySqlClient")
        {
            LocalReset();
        }

        public override void Reset()
        {
            base.Reset();
            LocalReset();
        }

        public override bool IsComplete
        {
            get
            {
                if (!(ConnectionStringBuilder["Server"] is string) ||
                    (ConnectionStringBuilder["Server"] as string).Length == 0)
                {
                    return false;
                }
                if (!(bool)ConnectionStringBuilder["Integrated Security"] &&
                    (!(ConnectionStringBuilder["User Id"] is string) ||
                    (ConnectionStringBuilder["User Id"] as string).Length == 0))
                {
                    return false;
                }
                return true;
            }
        }

        protected override string ToTestString()
        {
            bool savedPooling = (bool)ConnectionStringBuilder["Pooling"];
            bool wasDefault = !ConnectionStringBuilder.ShouldSerialize("Pooling");
            ConnectionStringBuilder["Pooling"] = false;
            string testString = ConnectionStringBuilder.ConnectionString;
            ConnectionStringBuilder["Pooling"] = savedPooling;
            if (wasDefault)
            {
                ConnectionStringBuilder.Remove("Pooling");
            }
            return testString;
        }
        private void LocalReset()
        {
            ConnectionStringBuilder.Add("Persist Security Info", false);
            //this["Integrated Security"] = true;
        }
    }
}
