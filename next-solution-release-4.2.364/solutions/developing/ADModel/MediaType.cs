using System;
using System.Collections.Generic;
using System.Linq;

namespace ADModel
{
    public enum MediaType : int
    {
        email,
        sms,
        voice,
        fax
    }
    
    public enum NotificationTypes
    {
        Local,
        Server
    }
}
