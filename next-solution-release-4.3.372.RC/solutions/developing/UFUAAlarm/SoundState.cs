using System;
using System.Threading;
using System.Collections.Generic;
using System.Media;
using Opc.Ua;
using log4net;
using VFS;
using System.Linq;

namespace UFUAAlarm
{
    public static class SoundState
    {
        #region Declaration

        static SoundPlayer player;

        static List<AlarmStatus> listAlarmPlayingSoundContinuously;
        static AlarmStatus currentPlayingAlarmSound;

        static FileSystemProviderBase fileSystemProviderBase;
        static String rootFolder;

        static String mutexName = @"Global\UFUAAlarm.SoundState.SoundOwnership";
        static Mutex lockMutex;

        static object lockObject = new object();
        static readonly ILog logServer = LogManager.GetLogger(Properties.Resources.Server);

        #endregion

        #region Static Constructor
        static SoundState()
        {
            bool wasCreated = false;
            try
            {
                lockMutex = new Mutex(true, mutexName, out wasCreated);
                if (wasCreated)
                {
                    player = new SoundPlayer();
                    listAlarmPlayingSoundContinuously = new List<AlarmStatus>();
                }
                else
                    logServer.Info(Properties.Resources.SoundPlayerAlreadyInUse);
            }
            catch (Exception ex)
            {
                logServer.Debug(String.Format(Properties.Resources.CreateMutexError, mutexName), ex);
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

        #region Public Static Methods
        public static void SetSoundFilePath(string connectionSource, string basePath)
        {
            if (fileSystemProviderBase is IDisposable)
                (fileSystemProviderBase as IDisposable).Dispose();

            rootFolder = basePath;
            if (connectionSource != null)
                fileSystemProviderBase = new DataSourceFileSystemProvider("") { ConnectionString = connectionSource };
            else
                fileSystemProviderBase = new PhysicalFileSystemProvider("");
        }

        public static void SetSoundFilePath(string rootPath)
        {
            SetSoundFilePath(null, rootPath);
        }

        public static void PlayAlarmSound(AlarmStatus alarm, bool bPlay)
        {
            if (player == null)
                return;

            if (bPlay)
            {
                lock (lockObject)
                {
                    if (alarm.RepeatSoundContinuously && !listAlarmPlayingSoundContinuously.Contains(alarm))
                        listAlarmPlayingSoundContinuously.Add(alarm);

                    if (currentPlayingAlarmSound == null || alarm.Severity > currentPlayingAlarmSound.Severity)
                    {
                        try
                        {
                            currentPlayingAlarmSound = alarm;
                            if (fileSystemProviderBase == null || System.IO.File.Exists(currentPlayingAlarmSound.SoundFile))
                            {
                                player.Stop();
                                player.SoundLocation = currentPlayingAlarmSound.SoundFile;
                            }
                            else if (fileSystemProviderBase != null)
                            {
                                player.Stop();
                                var fileManagerFileSettings = new FileManagerFile(fileSystemProviderBase, String.Format("{0}{1}{2}", rootFolder, System.IO.Path.DirectorySeparatorChar, currentPlayingAlarmSound.SoundFile));
                                player.Stream = new System.IO.MemoryStream(fileSystemProviderBase.ReadFile(fileManagerFileSettings));
                            }
                            if (currentPlayingAlarmSound.RepeatSoundContinuously)
                                player.PlayLooping();
                            else
                                player.Play();
                        }
                        catch (Exception ex)
                        {
                            logServer.ErrorFormat(Properties.Resources.PlaySoundError, currentPlayingAlarmSound.Name, ex.Message);
                        }
                    }
                }
            }
            else
            {
                lock (lockObject)
                {
                    if (listAlarmPlayingSoundContinuously.Contains(alarm))
                        listAlarmPlayingSoundContinuously.Remove(alarm);

                    if (currentPlayingAlarmSound == alarm)
                    {
                        currentPlayingAlarmSound = null;
                        player.Stop();
                    }
                }

                var next = GetNextPlayAlarm();
                if (next != null)
                    PlayAlarmSound(next, bPlay: true);
            }
        }

        public static void ShutDown()
        {
            lock (lockObject)
            {
                if (lockMutex != null)
                {
                    lockMutex.Dispose();
                    lockMutex = null;
                }

                if (player != null)
                {
                    player.Dispose();
                    player = null;
                }

                if (listAlarmPlayingSoundContinuously != null)
                    listAlarmPlayingSoundContinuously.Clear();
            }
        }
        #endregion

        #region Methods
        static AlarmStatus GetNextPlayAlarm()
        {
            var alarms = new List<AlarmStatus>();
            lock (lockObject)
            {
                alarms.AddRange(listAlarmPlayingSoundContinuously);
            }

            if (alarms.Count == 0)
                return null;

            return (from c in alarms orderby (int)c.Severity descending select c).FirstOrDefault();
        }
        #endregion
    }
}
