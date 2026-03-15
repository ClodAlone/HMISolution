using System;

namespace DeployServer.Services
{
    public interface IApplicationConfiguration
    {
        /*
            Note that each property here needs to exactly match the 
            name of each property in my appsettings.json config object
        */
        string ProxyPath { get; set; }
    }
}
