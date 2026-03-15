using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using CommandManager.ComponentService;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
#endif
#endif
using System.Speech.Synthesis;
using UFInterfaces;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System.ComponentModel;

namespace CommandManager
{
    [DataContract(Name = "TTSCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class TTSCommand : CommandManager
    {
        #region Properties

        [DataMember]
        String voiceName;
        public String VoiceName
        {
            get { return voiceName; }
#if !WINDOWS_UWP
            set
            {
                if (value == voiceName)
                    return;
                voiceName = value;
#if !NET_STANDARD
                OnPropertyChanged("VoiceName");
                OnPropertyChanged("CommandSummary");
#endif
            }
#endif
        }

        [DataMember]
        int volume = 100;
        public int Volume
        {
            get { return volume; }
#if !WINDOWS_UWP
            set
            {
                if (value == volume)
                    return;
                volume = value;
#if !NET_STANDARD
                OnPropertyChanged("Volume");
                OnPropertyChanged("CommandSummary");
#endif
            }
#endif
        }

        [DataMember]
        int rate = 0;
        public int Rate
        {
            get { return rate; }
#if !WINDOWS_UWP
            set
            {
                if (value == rate)
                    return;
                rate = value;
#if !NET_STANDARD
                OnPropertyChanged("Rate");
                OnPropertyChanged("CommandSummary");
#endif
            }
#endif
        }

        [DataMember]
        String speak;
        public String Speak
        {
            get { return speak; }
#if !WINDOWS_UWP
            set
            {
                if (value == speak)
                    return;
                speak = value;
#if !NET_STANDARD
                OnPropertyChanged("Speak");
                OnPropertyChanged("CommandSummary");
#endif
            }
#endif
        }

        [DataMember]
        ExecutionMode executionMode;
        public ExecutionMode ExecutionMode
        {
            get
            {
                return executionMode;
            }
#if !WINDOWS_UWP
            set
            {
                if (executionMode == value)
                    return;
                executionMode = value;
#if !NET_STANDARD
                OnPropertyChanged("ExecutionMode");
                OnPropertyChanged("CommandSummary");
#endif
            }
#endif
        }
        #endregion

        #region Overrides
#if !WINDOWS_UWP && !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "OpcuaEntityReference" || 
                    propertyName == "Expression")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
#endif

        public override String CommandSummary
        {
            get
            {
                return Speak;
            }
        }

        #region Validations
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "Volume")
            {
                if (Volume < 0 || volume > 100)
                    return Properties.Resources.InvalidVolume;
            }
            else if (propertyName == "Rate")
            {
                if (Rate < -10 || Rate > 10 )
                    return Properties.Resources.InvalidRate;
            }
            return base.PerformValidation(propertyName);
        }
#endif
        #endregion
        public override String Name
        {
            get
            {
                return Properties.Resources.TTSName;
            }
        }

        public override RemoteExecute RemoteExecute()
        {
            return null;
        }

        public override bool IsUICommand()
        {
            return true;
        }

        public override void Execute()
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute() ||
#if !NET_STANDARD
                !CanExecuteDelayCommand() || 
#endif
                mapSynthesizer == null || Parent == null || !mapSynthesizer.ContainsKey(Parent))
                return;

            var stringManager = Parent.GetService(typeof(IStringEditorManager)) as IStringEditorManager;

            var speak = Speak;
            var active = stringManager.GetActiveCulture(Parent);
            if (!String.IsNullOrEmpty(active))
            {
                var map = stringManager.GetListStringForCulture(Parent, active);
                if (map != null && map.ContainsKey(Speak))
                    speak = map[Speak];
            }

            try
            {
                mapSynthesizer[Parent].Rate = Rate;
                mapSynthesizer[Parent].Volume = Volume;
            }
            catch
            {
            }

            switch (ExecutionMode)
            {
                case ExecutionMode.Normal:
                    mapSynthesizer[Parent].SpeakAsync(speak);
                    break;
                case ExecutionMode.Synchro:
                    mapSynthesizer[Parent].Speak(speak);
                    break;
                case ExecutionMode.Shared:
                    mapSynthesizer[Parent].SpeakAsyncCancelAll();
                    mapSynthesizer[Parent].SpeakAsync(speak);
                    break;
            }
        }

        Dictionary<IDocument, SpeechSynthesizer> mapSynthesizer;

        public override bool Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            var ret = base.Init(entity, parent, sessionname);
            if (Parent == null)
                return false;

            if (mapSynthesizer == null)
                mapSynthesizer = new Dictionary<IDocument, SpeechSynthesizer>();
            if (!mapSynthesizer.ContainsKey(Parent))
                mapSynthesizer.Add(Parent, new SpeechSynthesizer());
            try
            {
                if (!String.IsNullOrEmpty(VoiceName))
                    mapSynthesizer[Parent].SelectVoice(VoiceName);
                mapSynthesizer[Parent].SetOutputToDefaultAudioDevice();
            }
            catch
            {
                mapSynthesizer[Parent].Dispose();
                mapSynthesizer.Remove(Parent);
                ret = false;
            }

            return ret;
        }

        public override void Terminate()
        {
            if (mapSynthesizer != null && Parent != null &&
                mapSynthesizer.ContainsKey(Parent))
            {
                mapSynthesizer[Parent].Dispose();
                mapSynthesizer.Remove(Parent);
            }

            base.Terminate();
        }
        #endregion
    }
}