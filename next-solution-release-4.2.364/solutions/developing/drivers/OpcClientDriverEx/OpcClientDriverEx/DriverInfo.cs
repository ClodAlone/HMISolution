using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Opc.Ua;

namespace OpcClientDriver
{
    public static class DriverInfo
    {
        private static Assembly Driver;
        internal static void Initialize(object commDriver)
        {
            if (Driver == null)
                Driver = Assembly.GetAssembly(commDriver.GetType());
            
            _privateID = new NodeId(DriverInfo.GetDriverName());
        }

        private static string _DriverName;
        public static string GetDriverName()
        {
            if (Driver == null)
                throw new MemberAccessException("class not properly initialized.");

            if (_DriverName == null)
                _DriverName = Driver.GetName().Name;

            return _DriverName;
        }

        private static NodeId _privateID;
        public static NodeId GetDriverNodeID()
        {
            return _privateID;
        }
    }
}
