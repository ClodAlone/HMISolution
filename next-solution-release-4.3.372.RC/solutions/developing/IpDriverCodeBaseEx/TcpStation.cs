using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using System.Net.Sockets;

namespace IpDriverCodeBaseEx
{
    public class TcpStation : Station
    {
        TcpClient socket;
    }
}
