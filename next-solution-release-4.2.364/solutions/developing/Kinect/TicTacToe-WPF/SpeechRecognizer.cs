// -----------------------------------------------------------------------
// <copyright file="SpeechRecognizer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;

    using RecentAngle = System.Collections.Generic.KeyValuePair<System.DateTime, Microsoft.Kinect.SoundSourceAngleChangedEventArgs>;
    
    /// <summary>
    /// Recognizes speech using Kinect audio stream as input source.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public class SpeechRecognizer
    {
        /// <summary>
        /// Format of Kinect audio stream samples.
        /// </summary>
        private const EncodingFormat AudioFormat = EncodingFormat.Pcm;

        /// <summary>
        /// Samples per second in Kinect audio stream.
        /// </summary>
        private const int AudioSamplesPerSecond = 16000;

        /// <summary>
        /// Bits per audio sample in Kinect audio stream.
        /// </summary>
        private const int AudioBitsPerSample = 16;

        /// <summary>
        /// Number of channels in Kinect audio stream.
        /// </summary>
        private const int AudioChannels = 1;

        /// <summary>
        /// Average bytes per second in Kinect audio stream
        /// </summary>
        private const int AudioAverageBytesPerSecond = 32000;

        /// <summary>
        /// Block alignment in Kinect audio stream.
        /// </summary>
        private const int AudioBlockAlign = 2;

        /// <summary>
        /// Amount of time (in milliseconds) for which we keep sound source angle data coming from Kinect sensor.
        /// </summary>
        private const int AngleRetentionPeriod = 1000;

        /// <summary>
        /// Default threshold value (in [0.0,1.0] interval) used to determine wether we'll propagate a speech
        /// event or drop it as if it had never happened.
        /// </summary>
        private const double DefaultConfidenceThreshold = 0.3;

        /// <summary>
        /// Queue containing Kinect sound source angle information from the most recent AngleRetentionPeriod.
        /// </summary>
        private readonly Queue<RecentAngle> recentSourceAngles = new Queue<RecentAngle>();

        /// <summary>
        /// Engine used to configure and control speech recognition behavior.
        /// </summary>
        private readonly SpeechRecognitionEngine speechEngine;

        /// <summary>
        /// Kinect audio source used to stream audio data from Kinect sensor.
        /// </summary>
        private KinectAudioSource kinectAudioSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechRecognizer"/> class.
        /// </summary>
        /// <param name="recognizerInfo">
        /// Metadata used to identify the recognizer acoustic model to be used.
        /// </param>
        /// <param name="grammars">
        /// Set of grammars to be loaded into speech recognition engine. May NOT be null.
        /// </param>
        private SpeechRecognizer(RecognizerInfo recognizerInfo, IEnumerable<Grammar> grammars)
        {
            this.ConfidenceThreshold = DefaultConfidenceThreshold;
            this.speechEngine = new SpeechRecognitionEngine(recognizerInfo);

            try
            {
                foreach (Grammar grammar in grammars)
                {
                    speechEngine.LoadGrammar(grammar);
                }
            }
            catch (InvalidOperationException)
            {
                // Grammar may not be in a valid state
                this.speechEngine.Dispose();
                this.speechEngine = null;
            }
        }

        public event EventHandler<SpeechRecognizerEventArgs> SpeechRecognized;

        public event EventHandler<SpeechRecognizerEventArgs> SpeechRejected;

        /// <summary>
        /// Threshold value (in [0.0,1.0] interval) used to determine wether we'll propagate a speech
        /// event or drop it as if it had never happened.
        /// </summary>
        public double ConfidenceThreshold { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="SpeechRecognizer"/> class.
        /// </summary>
        /// <param name="grammars">
        /// Array of grammars to be loaded into speech recognition engine.
        /// </param>
        /// <returns>
        /// SpeechRecognizer constructed. May be null if a recognizer couldn't be
        /// constructed from specified parameters, or if a valid acoustic model
        /// could not be found.
        /// </returns>
        public static SpeechRecognizer Create(Grammar[] grammars)
        {
            // Specified grammars should be valid
            if ((null == grammars) || (0 == grammars.Length))
            {
                return null;
            }

            var ri = GetKinectRecognizer();
            if (null == ri)
            {
                // speech prerequisites may not be installed.
                return null;
            }

            return new SpeechRecognizer(ri, grammars);
        }

        /// <summary>
        /// Starts speech recognition using audio stream from specified KinectAudioSource.
        /// </summary>
        /// <param name="audioSource">
        /// Audio source to use as input to speech recognizer.
        /// </param>
        public void Start(KinectAudioSource audioSource)
        {
            if (null == audioSource)
            {
                return;
            }

            this.kinectAudioSource = audioSource;
            this.kinectAudioSource.AutomaticGainControlEnabled = false;
            this.kinectAudioSource.NoiseSuppression = true;
            this.kinectAudioSource.BeamAngleMode = BeamAngleMode.Adaptive;

            this.kinectAudioSource.SoundSourceAngleChanged += this.SoundSourceChanged;
            this.speechEngine.SpeechRecognized += this.SreSpeechRecognized;
            this.speechEngine.SpeechRecognitionRejected += this.SreSpeechRecognitionRejected;

            var kinectStream = this.kinectAudioSource.Start();
            this.speechEngine.SetInputToAudioStream(
                kinectStream, new SpeechAudioFormatInfo(AudioFormat, AudioSamplesPerSecond, AudioBitsPerSample, AudioChannels, AudioAverageBytesPerSecond, AudioBlockAlign, null));
            this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Stop streaming Kinect audio data and recognizing speech.
        /// </summary>
        public void Stop()
        {
            if (this.kinectAudioSource != null)
            {
                this.kinectAudioSource.Stop();
                this.speechEngine.RecognizeAsyncCancel();
                this.speechEngine.RecognizeAsyncStop();

                this.kinectAudioSource.SoundSourceAngleChanged -= this.SoundSourceChanged;
                this.speechEngine.SpeechRecognized -= this.SreSpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= this.SreSpeechRecognitionRejected;
            }
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.OrdinalIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        /// <summary>
        /// Handler for event triggered when sound source angle changes in Kinect audio stream.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void SoundSourceChanged(object sender, SoundSourceAngleChangedEventArgs e)
        {
            DateTime now = DateTime.Now;
            recentSourceAngles.Enqueue(new RecentAngle(now, e));

            // Remove angles past our time range of interest
            while (recentSourceAngles.Peek().Key < now.AddMilliseconds(-AngleRetentionPeriod))
            {
                recentSourceAngles.Dequeue();
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            OnRejected(GetMostRecentAngle());
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < ConfidenceThreshold)
            {
                return;
            }

            OnRecognized(e.Result.Text, e.Result.Semantics.Value.ToString(), GetMostRecentAngle());
        }

        /// <summary>
        /// Helper method that invokes SpeechRecognized event if there are any event subscribers registered.
        /// </summary>
        /// <param name="phrase">
        /// Speech phrase (text) recognized.
        /// </param>
        /// <param name="semanticValue">
        /// Semantic value associated with recognized speech phrase.
        /// </param>
        /// <param name="sourceAngle">
        /// Best guess at source angle from which speech command originated.
        /// </param>
        private void OnRecognized(string phrase, string semanticValue, double? sourceAngle)
        {
            if (null != SpeechRecognized)
            {
                SpeechRecognized(this, new SpeechRecognizerEventArgs { Phrase = phrase, SemanticValue = semanticValue, SourceAngle = sourceAngle });
            }
        }

        /// <summary>
        /// Helper method that invokes SpeechRejected event if there are any event subscribers registered.
        /// </summary>
        /// <param name="sourceAngle">
        /// Best guess at source angle from which speech utterance originated.
        /// </param>
        private void OnRejected(double? sourceAngle)
        {
            if (null != SpeechRejected)
            {
                SpeechRejected(this, new SpeechRecognizerEventArgs { SourceAngle = sourceAngle });
            }
        }

        /// <summary>
        /// Give an estimate for the average angle of sound perceived during the last AngleRetentionPeriod.
        /// </summary>
        /// <returns>
        /// Average angle of sound perceived. May be null if sound source events received had less than
        /// minimum acceptable confidence threshold.
        /// </returns>
        private double? GetMostRecentAngle()
        {
            const double MinimumIndividualConfidence = 0.1;
            const double MinimumTotalConfidence = 0.25;

            if (recentSourceAngles.Count <= 0)
            {
                return null;
            }

            double totalConfidence = 0.0;
            double totalAngle = 0;

            foreach (RecentAngle recentAngle in recentSourceAngles)
            {
                if (recentAngle.Value.ConfidenceLevel < MinimumIndividualConfidence)
                {
                    continue;
                }

                totalConfidence += recentAngle.Value.ConfidenceLevel;
                totalAngle += recentAngle.Value.ConfidenceLevel * recentAngle.Value.Angle;
            }

            if (totalConfidence < MinimumTotalConfidence)
            {
                return null;
            }

            return totalAngle / totalConfidence;
        }
    }
}
