using System;
using System.Collections.Generic;
using System.Text;

namespace DeployServer.Utils
{
    public static class PlaceHolders
    {
        public const string DeployRootPathPlaceholder = "{{targetDeployRootPath}}";
        public const string AbsoluteRootPathPlaceholder = "{{absoluteDeployRootPath}}";
        public const string DeployWebHMIProxyPathPlaceholder = "{{targetWebHMIProxyPath}}";
    }
}
