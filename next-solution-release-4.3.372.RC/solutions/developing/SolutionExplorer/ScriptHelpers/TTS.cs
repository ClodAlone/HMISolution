using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using StringManager.ComponentService;

namespace UFProjectManager.ScriptHelpers
{
    public class TTS : IDisposable
    {
        readonly IStringEditorManager documentManager;
        readonly UFProjectDocument Document;

        SpeechSynthesizer synthesizer;

        public TTS(UFProjectDocument d)
        {
            Document = d;
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
        }

        public void SelectVoice(String voice)
        {
            synthesizer.SelectVoice(voice);
        }

        public void SetRate(int rate)
        {
            synthesizer.Rate = rate;
        }

        public void SetVolume(int volume)
        {
            synthesizer.Volume = volume;
        }

        public void Speak(String speak)
        {
            synthesizer.Speak(speak);
        }

        public void SpeakAsync(String speak)
        {
            synthesizer.SpeakAsync(speak);
        }

        public void SpeakAsyncCancelAll()
        {
            synthesizer.SpeakAsyncCancelAll();
        }

        #region Override Methods
        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetHashCode()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.ToString()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.Equals(object)"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public void Dispose()
        {
            if (synthesizer != null)
            {
                synthesizer.Dispose();
                synthesizer = null;
            }
        }
        #endregion
    }
}
