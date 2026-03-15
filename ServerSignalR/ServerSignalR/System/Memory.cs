using System.Diagnostics;

namespace ServerSignalR.System
{
    static public class Memory
    {
        static List<long> lastWorkingSet = new List<long>();
        static public void GetCurrentProcessMemory()
        {
            var me = Process.GetCurrentProcess();
            Console.WriteLine("Working set {0} kb", me.WorkingSet64 / 1024);
            lastWorkingSet.Add(me.WorkingSet64);
            if (lastWorkingSet.Count > 100)
                lastWorkingSet.RemoveAt(0);
            for (int i = 0; i < lastWorkingSet.Count - 1; ++i)
                if (lastWorkingSet[i] > lastWorkingSet[i + 1])
                    return;
            Console.WriteLine("Memory keeps increasing !!!!!");
        }
    }
}
