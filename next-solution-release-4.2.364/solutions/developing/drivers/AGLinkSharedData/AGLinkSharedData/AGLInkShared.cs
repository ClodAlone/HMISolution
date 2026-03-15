using System;
using System.Threading;

namespace AGLinkSharedData
{
    public static class AGLinkShared 
    {
        private static long devNr = -1;

        public static Int32 GetNewDevNr()
        {
            return (Int32)Interlocked.Increment(ref devNr);
        }

        public static Int32 GetCurrentDevNr()
        {
            return (Int32)Interlocked.Read(ref devNr);
        }
    }
}
