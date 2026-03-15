using System;

namespace UFRecipeSettings.UFRecipeModel
{
    enum TransportDefaultPorts : int
    {
        UriSchemeNetPipePort = -1, 
        UriSchemeOpcTcpPort = 62871,
        UriSchemeHttpsPort = 62872,
        UriSchemeNoSecurityHttpPort = 62873,
        UriSchemeHttpPort = 62874,
        UriSchemeNetTcpPort = 62876
    }
}
