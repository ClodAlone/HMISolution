using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ClientServices.Providers;

namespace MSZUtilsServiceHelper
{
    public enum RequestType
    {
        UserLogOn,
        UserLogOff,
        GetLicTypeList,
        GetBoolOptionsList,
        GetNumericOptionsList,
        GetCustomerList,
        GetSerialListInfo,
        GetSerialInfo,
        GetSerialLogOptions,
        GetSerialOptions,
        InserOrUpdateSerialOptions,
        InserOrUpdateLicTypeOptions,
        InitSerial,
        Illegal,
        Error,
        GetAreeGeoIDList,
        InsertOrUpdateCustomer,
    }
    public class ClientAutenticationCredentials: Object
    {
        public int UserID { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public int UserType { get; set; }
        public ClientAutenticationCredentials(ClientAutenticationCredentials userInfo)
        {
            UserID = userInfo.UserID;
            Password = userInfo.Password;
            UserName = userInfo.UserName;
            UserType = userInfo.UserType;
        }
        public ClientAutenticationCredentials()
        {
            InitValues();
        }

        private void InitValues()
        {
            UserID = -1;
            Password = string.Empty;
            UserName = string.Empty;
            UserType = -1;
        }

        public ClientAutenticationCredentials(string user, string passwor, int userId, int userType)
        {
            UserID = userId;
            Password = passwor;
            UserName = user;
            UserType = userType;
        }
        public override string ToString()
        {
            return $"{UserName}|{Password}|{UserID}|{UserType}";
        }
        public ClientAutenticationCredentials(string userInfo)
        {
            InitValues();

            string[] info = userInfo?.Split('|');
            if (info == null)
                return;

            int ret = -1;
            int retType = -1;
            if (info.Count() > 0)
                UserName = info[0];
            if (info.Count() > 1)
                Password = info[1];
            if (info.Count() > 2)
                int.TryParse(info[2], out ret);
            if (info.Count() > 3)
                int.TryParse(info[3], out retType);
            UserID = ret;
            UserType = retType;
        }
    }
}
