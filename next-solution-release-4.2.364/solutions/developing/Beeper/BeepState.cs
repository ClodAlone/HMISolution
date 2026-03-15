using System;
using System.Threading;
using System.Collections.Generic;

namespace Beeper
{
    public static class BeepState
    {
        #region Declaration
        
        static readonly Beeper beeper;
        static bool isBeeping;
        static Mutex lockMutex;

        #endregion

        #region Static Constructor
        static BeepState()
        {
            bool wasCreated = false;
            try
            {
                if (Environment.UserInteractive)
                    lockMutex = new Mutex(true, @"Global\UFUAAlarm.BeepState.BeepOwnership", out wasCreated);
                if (wasCreated)
                    beeper = new Beeper();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Unable to create Mutex 'UFUAAlarm.BeepState.BeepOwnership', error '{0}'", ex.Message));
            }
            finally
            {
                if (!wasCreated && lockMutex != null)
                {
                    lockMutex.Dispose();
                    lockMutex = null;
                }
            }
        }
        #endregion

        #region Public Static Properties
        public static bool IsBeeping
        {
            get
            {
                if (beeper != null)
                    return beeper.IsBeeping;
                else
                    return isBeeping;
            }
        }
        #endregion

        #region Public Static Methods

        public static void ChangePlayState(bool newState)
        {
            isBeeping = newState;
            if (beeper == null)
                return;

            if (newState)
                beeper.StartBeeping(500, 250, 1000, 250);
            else
                beeper.StopBeeping();
        }

        public static void ChangeShelveState(bool newState)
        {
            if (beeper == null)
                return;

            if (newState)
                beeper.PauseBeeping();
            else
                beeper.ResumeBeeping();
        }

        public static void ShutDown()
        {
            if (lockMutex != null)
            {
                lockMutex.Dispose();
                lockMutex = null;
            }

            if (beeper != null)
                beeper.Dispose();
        }

        #endregion
    }
}
