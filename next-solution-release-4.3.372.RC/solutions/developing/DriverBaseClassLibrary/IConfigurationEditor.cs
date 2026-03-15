using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverBaseInterfaces
{
    public interface IConfigurationEditor
    {
        void Init(string connectionString, string drivername, bool protection, Guid Code);
        object AddChannel();
        object AddStation(string channel);
        object GetChannel(string channel);
        object GetStation(string channelname, string stationname);
        bool RemoveChannelSettings(string channelname, bool removestations = false);
        bool RemoveStationSettings(string channelname, string stationname);
        List<object> GetChannelList();
        List<object> GetStationList();
        bool Save();
        bool Save(out string msg, bool protection, Guid code);
        void Dispose();

        bool IsValid { get; }
        object Configuration { get; }
    }
}
