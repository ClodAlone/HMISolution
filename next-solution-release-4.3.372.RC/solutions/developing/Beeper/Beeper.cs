using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beeper
{
    public class Beeper : IDisposable
    {
        Thread threadBeeper;
        bool bDisposing;
        int nfrequency1;
        int ndelay1;
        int nfrequency2;
        int ndelay2;
        
        volatile bool inPause;
        volatile bool isBeeping;

        readonly ManualResetEvent play = new ManualResetEvent(false);
        
        readonly Object lockObject = new Object();

        public void StartBeeping(int frequency1, int delay1, int frequency2, int delay2)
        {
            if (bDisposing)
                return;

            isBeeping = true;
            lock (lockObject)
            {
                nfrequency1 = frequency1;
                ndelay1 = delay1;
                nfrequency2 = frequency2;
                ndelay2 = delay2;

                if (threadBeeper == null)
                {
                    threadBeeper = new Thread((o) =>
                    {
                        while (play.WaitOne())
                        {
                            if (bDisposing)
                                break;
#if !NET_STANDARD
                            Console.Beep(nfrequency1, ndelay1);
                            Console.Beep(nfrequency2, ndelay2);
                            Thread.Sleep(ndelay1 + ndelay2);
#else
                            Console.Beep();
                            Thread.Sleep(ndelay1);
                            Console.Beep();
                            Thread.Sleep(ndelay2);
#endif
                        }
                    }) 
                    { 
                        Name = "Beeper", 
                        IsBackground = true 
                    };
                    threadBeeper.Start();
                }
            }

            if (!inPause)
                play.Set();
        }

        public void StopBeeping()
        {
            if (bDisposing)
                return;

            isBeeping = false;

            play.Reset();
        }

        public void PauseBeeping()
        {
            if (bDisposing || inPause)
                return;

            inPause = true;

            play.Reset();
        }

        public void ResumeBeeping()
        {
            if (bDisposing || !inPause)
                return;

            inPause = false;

            if (isBeeping)
                play.Set();
        }

        public bool IsBeeping
        {
            get
            {
                return isBeeping/* && !inPause*/;
            }
        }

        public void Dispose()
        {
            bDisposing = true;
            lock (lockObject)
            {
                if (threadBeeper != null)
                {
                    play.Set();
                    threadBeeper.Join();
                    threadBeeper = null;
                }
            }
            play.Dispose();
        }
    }
}
