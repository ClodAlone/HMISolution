using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Speech.Recognition;
using System.IO;
using Microsoft.Speech.AudioFormat;
using Utilities;
using System.Windows.Media.Animation;
using System.Globalization;
using System.Windows.Threading;
using System.Threading;
using DevExpress.Xpf.Core;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for SpeechWindow.xaml
    /// </summary>
    /// 
    public class SpeechCommandEventArgs : EventArgs
    {
        String command;

        public SpeechCommandEventArgs(string c)
        {
            command = c;
        }
        public string Command
        {
            get { return command; }
            set { command = value; }
        }
    }

    public partial class SpeechWindow : ThemedWindow
    {
        public SpeechWindow()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                using (var cursor = new WaitCursor())
                {
                    StartRecognition();
                }
            };

            Closed += (o, e) =>
            {
                if (pendingGrammarOperation != null)
                {
                    pendingGrammarOperation.Abort();
                    pendingGrammarOperation = null;
                }

                var pendingTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        StopRecognition();
                    }
                    catch
                    { }
                });
            };
        }

        #region Events
        public event EventHandler ReadyForContent;
        void OnReadyForContent()
        {
            var e = ReadyForContent;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler MoveBack;
        void OnMoveBack()
        {
            var e = MoveBack;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler MoveForward;
        void OnMoveForward()
        {
            var e = MoveForward;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler Home;
        void OnHome()
        {
            var e = Home;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler ZoomIn;
        void OnZoomIn()
        {
            var e = ZoomIn;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler ZoomOut;
        void OnZoomOut()
        {
            var e = ZoomOut;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler SwipeUp;
        void OnSwipeUp()
        {
            var e = SwipeUp;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler SwipeDown;
        void OnSwipeDown()
        {
            var e = SwipeDown;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler SwipeLeft;
        void OnSwipeLeft()
        {
            var e = SwipeLeft;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler SwipeRight;
        void OnSwipeRight()
        {
            var e = SwipeRight;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler Reset;
        void OnReset()
        {
            var e = Reset;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler<SpeechCommandEventArgs> SpeechCommand;
        void OnSpeechCommand(String command)
        {
            var e = SpeechCommand;
            if (e != null)
                e(null/*this*/, new SpeechCommandEventArgs(command));
        }
        #endregion

        #region DP
        public static readonly DependencyProperty EnableSpeechRecognitionProperty = DependencyProperty.Register("EnableSpeechRecognition", typeof(bool), typeof(SpeechWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableSpeechRecognitionChanged), new CoerceValueCallback(OnCoerceEnableSpeechRecognition)));

        private static object OnCoerceEnableSpeechRecognition(DependencyObject o, object value)
        {
            SpeechWindow SpeechWindow = o as SpeechWindow;
            if (SpeechWindow != null)
                return SpeechWindow.OnCoerceEnableSpeechRecognition((bool)value);
            else
                return value;
        }

        private static void OnEnableSpeechRecognitionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpeechWindow SpeechWindow = o as SpeechWindow;
            if (SpeechWindow != null)
                SpeechWindow.OnEnableSpeechRecognitionChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceEnableSpeechRecognition(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnableSpeechRecognitionChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool EnableSpeechRecognition
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(EnableSpeechRecognitionProperty);
            }
            set
            {
                SetValue(EnableSpeechRecognitionProperty, value);
            }
        }

        public static readonly DependencyProperty SpeechConfidenceLevelProperty = DependencyProperty.Register("SpeechConfidenceLevel", typeof(double), typeof(SpeechWindow), new UIPropertyMetadata(0.7, new PropertyChangedCallback(OnSpeechConfidenceLevelChanged), new CoerceValueCallback(OnCoerceSpeechConfidenceLevel)));

        private static object OnCoerceSpeechConfidenceLevel(DependencyObject o, object value)
        {
            SpeechWindow SpeechWindow = o as SpeechWindow;
            if (SpeechWindow != null)
                return SpeechWindow.OnCoerceSpeechConfidenceLevel((double)value);
            else
                return value;
        }

        private static void OnSpeechConfidenceLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpeechWindow SpeechWindow = o as SpeechWindow;
            if (SpeechWindow != null)
                SpeechWindow.OnSpeechConfidenceLevelChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceSpeechConfidenceLevel(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpeechConfidenceLevelChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double SpeechConfidenceLevel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(SpeechConfidenceLevelProperty);
            }
            set
            {
                SetValue(SpeechConfidenceLevelProperty, value);
            }
        }

        public static readonly DependencyProperty ShowSpeechFailedProperty = DependencyProperty.Register("ShowSpeechFailed", typeof(bool), typeof(SpeechWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowSpeechFailedChanged), new CoerceValueCallback(OnCoerceShowSpeechFailed)));

        private static object OnCoerceShowSpeechFailed(DependencyObject o, object value)
        {
            SpeechWindow SpeechWindow = o as SpeechWindow;
            if (SpeechWindow != null)
                return SpeechWindow.OnCoerceShowSpeechFailed((bool)value);
            else
                return value;
        }

        private static void OnShowSpeechFailedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpeechWindow SpeechWindow = o as SpeechWindow;
            if (SpeechWindow != null)
                SpeechWindow.OnShowSpeechFailedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowSpeechFailed(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowSpeechFailedChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowSpeechFailed
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowSpeechFailedProperty);
            }
            set
            {
                SetValue(ShowSpeechFailedProperty, value);
            }
        }

        public static readonly DependencyProperty ShowSpeechRecognizedProperty = DependencyProperty.Register("ShowSpeechRecognized", typeof(bool), typeof(SpeechWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSpeechRecognizedChanged), new CoerceValueCallback(OnCoerceShowSpeechRecognized)));

        private static object OnCoerceShowSpeechRecognized(DependencyObject o, object value)
        {
            SpeechWindow SpeechWindow = o as SpeechWindow;
            if (SpeechWindow != null)
                return SpeechWindow.OnCoerceShowSpeechRecognized((bool)value);
            else
                return value;
        }

        private static void OnShowSpeechRecognizedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpeechWindow SpeechWindow = o as SpeechWindow;
            if (SpeechWindow != null)
                SpeechWindow.OnShowSpeechRecognizedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowSpeechRecognized(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowSpeechRecognizedChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowSpeechRecognized
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowSpeechRecognizedProperty);
            }
            set
            {
                SetValue(ShowSpeechRecognizedProperty, value);
            }
        }

        public String SpeechCulture { get; set; } = "en-us";

        /*
        private const String nextCommand = "next";
        private const String previousCommand = "back";
        private const String homeCommand = "home";
        private const String resetCommand = "reset";
        private const String swipeUpCommand = "swipe up";
        private const String swipeDownCommand = "swipe down";
        private const String zoomInCommand = "zoom in";
        private const String zoomOutCommand = "zoom out";
        */

        public List<String> DefaultSpeechCommands
        {
            get
            {
                return defaultCommandList;
            }

            set
            {
                defaultCommandList = value;
            }
        }

        #endregion

        #region Speech Recognition

        SpeechRecognitionEngine sre;
        Grammar currentGrammar;
        CultureInfo ci;

        static readonly String speechAnimation = "SpeechAnimation";

        DispatcherOperation currentOperation;
        void SetSpeechText(String text)
        {
            if (currentOperation != null)
                currentOperation.Abort();
            currentOperation = Dispatcher.BeginInvokeAsynchronously(() =>
            {
                lableSpeech.Content = text;
                var storyboard = Resources[speechAnimation] as Storyboard;
                storyboard.Begin();
            });
        }

        void StopRecognition()
        {
            if (sre != null)
            {
                sre.SpeechRecognized -= SreSpeechRecognized;
                sre.SpeechHypothesized -= SreSpeechHypothesized;
                sre.SpeechRecognitionRejected -= SreSpeechRecognitionRejected;

                sre.RecognizeAsyncStop();
                sre.Dispose();
                sre = null;
            }
        }

        List<String> pendingList;
        DispatcherOperation pendingGrammarOperation;
        List<String> cultureBlackList = new List<String>();

        public void LoadDynamicGrammar(List<String> list, String culture)
        {
            if (sre == null || !EnableSpeechRecognition)
            {
                pendingList = list;
                return;
            }

            if (pendingList != null && list != null && list.SequenceEqual<String>(pendingList))
                return;
            pendingList = list;

            if (pendingGrammarOperation != null)
                pendingGrammarOperation.Abort();

            pendingGrammarOperation = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (sre == null)
                    return;

                if (EnableSpeechRecognition && !String.IsNullOrEmpty(culture) && !cultureBlackList.Contains(culture) &&
                    SpeechCulture != culture)
                {
                    using (var cursor = new WaitCursor())
                    {
                        var savedCulture = SpeechCulture;
                        SpeechCulture = culture;

                        var s = sre;
                        sre = null;
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                s.SpeechRecognized -= SreSpeechRecognized;
                                s.SpeechHypothesized -= SreSpeechHypothesized;
                                s.SpeechRecognitionRejected -= SreSpeechRecognitionRejected;

                                s.RecognizeAsyncStop();
                                s.Dispose();
                            }
                            catch
                            { }
                        });

                        StartRecognition();
                        if (sre == null)
                        {
                            if (!cultureBlackList.Contains(culture))
                                cultureBlackList.Add(culture);

                            SpeechCulture = savedCulture;
                            StartRecognition();
                        }
                    }
                }

                var pendingTask = Task.Factory.StartNew(() =>
                {
                    if (sre == null)
                        return;

                    var oldThreadPriority = Thread.CurrentThread.Priority;
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    try
                    {
                        SetSpeechText(Properties.Resources.UnloadingGrammar);

                        try
                        {
                            if (currentGrammar != null)
                                sre.UnloadGrammar(currentGrammar);
                        }
                        catch (Exception ex)
                        {

                        }

                        if (list == null)
                            list = defaultCommandList;
                        else
                            list.AddRange(defaultCommandList);
                        if (list == null || list.Count == 0)
                            return;
                        SetSpeechText(Properties.Resources.PreparingGrammar);
                        currentGrammar = LoadGrammar(list);
                        SetSpeechText(Properties.Resources.GrammarReady);
                    }
                    finally
                    {
                        Thread.CurrentThread.Priority = oldThreadPriority;
                    }
                });
            });

            if (pendingGrammarOperation != null || pendingGrammarOperation.Status == DispatcherOperationStatus.Completed)
            {
                if (pendingGrammarOperation.Status != DispatcherOperationStatus.Completed)
                    pendingGrammarOperation.Completed += (o, e) => { pendingGrammarOperation = null; };
                else
                    pendingGrammarOperation = null;
            }
        }

        Grammar LoadGrammar(List<String> list)
        {
            if (list == null || list.Count == 0)
                return null;

            var commands = new Choices();
            list.ForEach(s => commands.Add(s));

            var gb = new GrammarBuilder { Culture = ci };

            // Specify the culture to match the recognizer in case we are running in a different culture.                                 
            gb.Append(commands);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            var grammar = new Grammar(gb);

            sre.LoadGrammar(grammar);
            return grammar;
        }

        List<String> defaultCommandList = new List<String>()
        {
            "next",
            "back",
            "home",
            "reset",
            "swipe up",
            "swipe down",
            "zoom in",
            "zoom out",
            "swipe left",
            "swipe right"
        };

        void StartRecognition()
        {
            if (!EnableSpeechRecognition)
                return;

            try
            {
                ci = new CultureInfo(SpeechCulture);
                sre = new SpeechRecognitionEngine(ci);
                sre.SetInputToDefaultAudioDevice();
            }
            catch
            {
                SetSpeechText(Properties.Resources.FailedToInitializeAudioInput);

                try
                {
                    StopRecognition();
                }
                catch
                {

                }
                return;
            }

            /*
            defaultCommandList.Add(nextCommand);
            defaultCommandList.Add(previousCommand);
            defaultCommandList.Add(homeCommand);
            defaultCommandList.Add(resetCommand);
            defaultCommandList.Add(swipeUpCommand);
            defaultCommandList.Add(swipeDownCommand);
            defaultCommandList.Add(zoomInCommand);
            defaultCommandList.Add(zoomOutCommand);
            */
            defaultCommandList.AddRange(DefaultSpeechCommands);

            LoadGrammar(defaultCommandList);

            sre.SpeechRecognized += SreSpeechRecognized;
            sre.SpeechHypothesized += SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

            if (pendingList != null)
            {
                LoadDynamicGrammar(pendingList, SpeechCulture);
                pendingList = null;
            }
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            /*
            Console.WriteLine("\nSpeech Rejected");
            if (e.Result != null)
            {
                DumpRecordedAudio(e.Result.Audio);
            }
            */
        }

        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            // Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                if (e.Result.Confidence >= SpeechConfidenceLevel)
                {
                    if (ShowSpeechRecognized)
                        SetSpeechText(String.Format("{0}\t\t({1}:\t{2})", e.Result.Text, Properties.Resources.Confidence, 
                            e.Result.Confidence));

                    Dispatcher.InvokeIfRequired(() =>
                    {
                        if (defaultCommandList.Count > 0 && 
                                String.Compare(e.Result.Text, defaultCommandList[0], true) == 0)
                            OnMoveForward();
                        else if (defaultCommandList.Count > 1 &&
                                String.Compare(e.Result.Text, defaultCommandList[1], true) == 0)
                            OnMoveBack();
                        else if (defaultCommandList.Count > 2 &&
                                String.Compare(e.Result.Text, defaultCommandList[2], true) == 0)
                            OnHome();
                        else if (defaultCommandList.Count > 3 &&
                                String.Compare(e.Result.Text, defaultCommandList[3], true) == 0)
                            OnReset();
                        else if (defaultCommandList.Count > 4 &&
                                String.Compare(e.Result.Text, defaultCommandList[4], true) == 0)
                            OnSwipeUp();
                        else if (defaultCommandList.Count > 5 &&
                                String.Compare(e.Result.Text, defaultCommandList[5], true) == 0)
                            OnSwipeDown();
                        else if (defaultCommandList.Count > 6 &&
                                String.Compare(e.Result.Text, defaultCommandList[6], true) == 0)
                            OnZoomIn();
                        else if (defaultCommandList.Count > 7 &&
                                String.Compare(e.Result.Text, defaultCommandList[7], true) == 0)
                            OnZoomOut();
                        else if (defaultCommandList.Count > 8 &&
                                String.Compare(e.Result.Text, defaultCommandList[8], true) == 0)
                            OnSwipeLeft();
                        else if (defaultCommandList.Count > 9 &&
                                String.Compare(e.Result.Text, defaultCommandList[9], true) == 0)
                            OnSwipeRight();
                        else
                            OnSpeechCommand(e.Result.Text);
                    });
                    // Console.WriteLine("\nSpeech Recognized: \t{0}\tConfidence:\t{1}", e.Result.Text, e.Result.Confidence);
                }
                else
                {
                    if (ShowSpeechFailed)
                        SetSpeechText(String.Format("{0}: \t{1}", Properties.Resources.ConfidenceLow, e.Result.Confidence));
                    // Console.WriteLine("\nSpeech Recognized but confidence was too low: \t{0}", e.Result.Confidence);
                    // DumpRecordedAudio(e.Result.Audio);
                }
            });
        }

        /*
        private static void DumpRecordedAudio(RecognizedAudio audio)
        {
            if (audio == null)
            {
                return;
            }

            int fileId = 0;
            string filename;
            while (File.Exists((filename = "RetainedAudio_" + fileId + ".wav")))
            {
                fileId++;
            }

            Console.WriteLine("\nWriting file: {0}", filename);
            using (var file = new FileStream(filename, System.IO.FileMode.CreateNew))
            {
                audio.WriteToWaveStream(file);
            }
        }
        */

        #endregion
    }
}
