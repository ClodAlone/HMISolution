//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Speech.Recognition;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Color image format used in the display presented to user as feedback.
        /// </summary>
        private const ColorImageFormat ImageFormat = ColorImageFormat.RgbResolution640x480Fps30;

        /// <summary>
        /// Number of active players that must be present in a Tic-Tac-Toe game.
        /// </summary>
        private const int ActivePlayersPerGame = 2;

        /// <summary>
        /// Name of speech grammar corresponding to speech acceptable during welcome screen.
        /// </summary>
        private const string WelcomeSpeechRule = "welcomeRule";

        /// <summary>
        /// Name of speech grammar corresponding to speech acceptable during game play.
        /// </summary>
        private const string GameSpeechRule = "gameRule";

        /// <summary>
        /// Name of speech grammar corresponding to speech acceptable during game end experience.
        /// </summary>
        private const string GameEndSpeechRule = "gameEndRule";

        /// <summary>
        /// Tracker used to manage player state.
        /// </summary>
        private readonly PlayerTracker<Player> playerTracker = new PlayerTracker<Player>(new PlayerFactory());

        /// <summary>
        /// Maping from player symbols to active players.
        /// </summary>
        private readonly Dictionary<PlayerSymbol, Player> symbol2PlayerMap = new Dictionary<PlayerSymbol, Player>();

        /// <summary>
        /// Mapping from player symbols to the viewers used to render player data.
        /// </summary>
        private readonly Dictionary<PlayerSymbol, PlayerViewer> symbol2ViewerMap = new Dictionary<PlayerSymbol, PlayerViewer>();

        /// <summary>
        /// Mapping from active players to their corresponding player symbols.
        /// </summary>
        private readonly Dictionary<Player, PlayerSymbol> player2SymbolMap = new Dictionary<Player, PlayerSymbol>();

        /// <summary>
        /// GameLogic object used to manage game state and determine move validity.
        /// </summary>
        private readonly GameLogic gameLogic = new GameLogic();

        /// <summary>
        /// KinectSensorChooser used to manage kinect sensors.
        /// </summary>
        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Speech recognizer used to detect voice commands issued by application users.
        /// </summary>
        private SpeechRecognizer speechRecognizer;

        /// <summary>
        /// Speech grammar used during welcome screen.
        /// </summary>
        private Grammar welcomeGrammar;

        /// <summary>
        /// Speech grammar used during game play.
        /// </summary>
        private Grammar gameGrammar;

        /// <summary>
        /// Speech grammar used after a game ends, to control starting a new one.
        /// </summary>
        private Grammar gameEndGrammar;

        /// <summary>
        /// true if placement rules are being enforced. false if players can play out of turn and in any board space.
        /// </summary>
        private bool rulesEnabled = true;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            InitializeWelcomePanel();

            // Initialize viewer mappings
            XViewer.Symbol = PlayerSymbol.XSymbol;
            this.symbol2ViewerMap[PlayerSymbol.XSymbol] = XViewer;
            OViewer.Symbol = PlayerSymbol.OSymbol;
            this.symbol2ViewerMap[PlayerSymbol.OSymbol] = OViewer;

            SensorChooserUI.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.KinectChanged += KinectChanged;
            this.sensorChooser.Start();
        }

        /// <summary>
        /// Initialize demo board state for welcome screen.
        /// </summary>
        private void InitializeWelcomePanel()
        {
            var welcomeBoard = new Board();

            // Prepopulate board with a sample state
            welcomeBoard.PopulateDefaultIds();
            welcomeBoard.GetAt(0, 0).Symbol = PlayerSymbol.XSymbol;
            welcomeBoard.GetAt(1, 2).Symbol = PlayerSymbol.OSymbol;
            WelcomeBoard.Board = welcomeBoard;

            // Configure X and O squares in welcome text.
            WelcomeTextX.Square = new BoardSquare(0, 0) { Symbol = PlayerSymbol.XSymbol };
            WelcomeTextO.Square = new BoardSquare(0, 0) { Symbol = PlayerSymbol.OSymbol };
        }

        /// <summary>
        /// Create a grammar from grammar definition XML file.
        /// </summary>
        /// <param name="ruleName">
        /// Rule corresponding to grammar we want to use.
        /// </param>
        /// <returns>
        /// New grammar object corresponding to specified rule.
        /// </returns>
        private Grammar CreateGrammar(string ruleName)
        {
            Grammar grammar;

            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
            {
                grammar = new Grammar(memoryStream, ruleName);
            }

            return grammar;
        }

        /// <summary>
        /// Enable specified grammar and disable all others.
        /// </summary>
        /// <param name="grammar">
        /// Grammar to be enabled. May be null to disable all grammars.
        /// </param>
        private void EnableGrammar(Grammar grammar)
        {
            if (null != this.gameGrammar)
            {
                this.gameGrammar.Enabled = grammar == this.gameGrammar;
            }

            if (null != this.welcomeGrammar)
            {
                this.welcomeGrammar.Enabled = grammar == this.welcomeGrammar;
            }

            if (null != this.gameEndGrammar)
            {
                this.gameEndGrammar.Enabled = grammar == this.gameEndGrammar;
            }
        }

        /// <summary>
        /// Find the next available symbol to be associated with a player that recently joined.
        /// </summary>
        /// <returns>
        /// First symbol available, in play order, or PlayerSymbol.None if all symbols are
        /// already in use.
        /// </returns>
        private PlayerSymbol FindAvailableSymbol()
        {
            PlayerSymbol available = PlayerSymbol.None;

            foreach (PlayerSymbol symbol in GameLogic.SymbolOrder)
            {
                if (!symbol2PlayerMap.ContainsKey(symbol))
                {
                    available = symbol;
                    break;
                }
            }

            return available;
        }

        /// <summary>
        /// Get number of currently active players.
        /// </summary>
        /// <returns>
        /// The number of currently active players.
        /// </returns>
        private int GetNumActivePlayers()
        {
            return player2SymbolMap.Count;
        }

        /// <summary>
        /// Determines whether there is a game currently in progress.
        /// </summary>
        /// <returns>
        /// True if the number of currently active players is the same as the expected number of players per game.
        /// False otherwise.
        /// </returns>
        private bool IsGameInProgress()
        {
            return GetNumActivePlayers() == ActivePlayersPerGame;
        }

        /// <summary>
        /// Gets the default status text for the current game state.
        /// </summary>
        /// <returns>
        /// Default status text string for current game state.
        /// </returns>
        private string GetDefaultStatusText()
        {
            // If game is not in progress, status should encourage players to join
            if (!IsGameInProgress())
            {
                return 0 == GetNumActivePlayers() ? Properties.Resources.WaitingForPlayers : Properties.Resources.WaitingForSecondPlayer;
            }

            // If we have expected number of players, give instructions about what to do
            return gameLogic.CurrentSymbol.GetSayNumberText();
        }

        /// <summary>
        /// Updates the status display area with the default text for the current game state.
        /// </summary>
        /// <param name="statusString">
        /// String to display in status area. If <code>null</code>, default status string for 
        /// current game state will be used.
        /// </param>
        private void UpdateStatusDisplay(string statusString)
        {
            if (null == statusString)
            {
                statusString = this.GetDefaultStatusText();
            }

            if (null != statusString)
            {
                StatusText.Text = statusString;
            }
        }

        /// <summary>
        /// Dismiss welcome screen and start playing a game.
        /// </summary>
        private void DismissWelcome()
        {
            StartNewGame();

            WelcomePanel.Visibility = Visibility.Collapsed;
            RematchButton.Visibility = Visibility.Collapsed;
            GamePanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Indicate in UI that game has ended and allow players to start playing a new game.
        /// </summary>
        private void ShowGameEnd()
        {
            this.EnableGrammar(this.gameEndGrammar);
            RematchButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Start a new game with fresh board state and indicate in UI that a game is
        /// currently in progress.
        /// </summary>
        private void StartNewGame()
        {
            this.EnableGrammar(this.gameGrammar);

            ResetGameState();
            RematchButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Resets the game board and status without affecting the rest of the UI.
        /// </summary>
        private void ResetGameState()
        {
            GameBoard.Board = gameLogic.StartGame();
            this.UpdateStatusDisplay(null);
        }

        /// <summary>
        /// Execute initialization tasks associated with a Kinect sensor.
        /// </summary>
        /// <param name="sensor">
        /// Sensor to initialize.
        /// </param>
        private void InitializeSensor(KinectSensor sensor)
        {
            if (null == sensor)
            {
                return;
            }

            // Turn on the color stream to receive color frames
            sensor.ColorStream.Enable(ImageFormat);

            // Allocate space to put the pixels we'll receive
            this.colorPixels = new byte[sensor.ColorStream.FramePixelDataLength];

            // This is the bitmap we'll display on-screen
            this.colorBitmap = new WriteableBitmap(sensor.ColorStream.FrameWidth, sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

            this.Image.Source = this.colorBitmap;

            // Add an event handler to be called whenever there is new color frame data
            sensor.ColorFrameReady += this.ColorFrameReady;

            this.playerTracker.SkeletonFilter = new ClosestSkeletonFilter { KeepCount = ActivePlayersPerGame };
            this.playerTracker.InitializeSensor(sensor);
            this.playerTracker.PlayerStatusChanged += this.PlayerStatusChanged;

            // Create and configure speech grammars and recognizer
            this.welcomeGrammar = CreateGrammar(WelcomeSpeechRule);
            this.gameGrammar = CreateGrammar(GameSpeechRule);
            this.gameEndGrammar = CreateGrammar(GameEndSpeechRule);
            this.EnableGrammar(this.welcomeGrammar);
            this.speechRecognizer = SpeechRecognizer.Create(new[] { welcomeGrammar, gameGrammar, gameEndGrammar });

            if (null != speechRecognizer)
            {
                this.speechRecognizer.SpeechRecognized += SpeechRecognized;

                this.speechRecognizer.Start(sensor.AudioSource);
            }

            ResetGameState();
        }

        /// <summary>
        /// Execute uninitialization tasks associated with a Kinect sensor
        /// </summary>
        /// <param name="sensor">
        /// Sensor to uninitialize.
        /// </param>
        private void UninitializeSensor(KinectSensor sensor)
        {
            if (null == sensor)
            {
                return;
            }

            if (null != this.speechRecognizer)
            {
                this.speechRecognizer.Stop();

                this.speechRecognizer.SpeechRecognized -= SpeechRecognized;
            }

            this.playerTracker.PlayerStatusChanged -= this.PlayerStatusChanged;
            this.playerTracker.UninitializeSensor();
        }

        /// <summary>
        /// Event handler for window closing event.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            // Stop choosing Kinect sensors
            this.sensorChooser.Stop();
            this.sensorChooser.KinectChanged -= KinectChanged;
        }

        /// <summary>
        /// Event handler for KinectSensorChooser's KinectChanged event.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void KinectChanged(object sender, KinectChangedEventArgs e)
        {
            try
            {
                this.UninitializeSensor(e.OldSensor);

                this.InitializeSensor(e.NewSensor);
            }
            catch (InvalidOperationException)
            {
                // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                // E.g.: sensor might be abruptly unplugged.
            }
        }

        /// <summary>
        /// Handles clicks on the start button in Welcome screen.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void WelcomeStartButtonClicked(object sender, RoutedEventArgs e)
        {
            DismissWelcome();
        }

        /// <summary>
        /// Handles clicks on the rematch button showing during game ending.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void RematchButtonClicked(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        /// <summary>
        /// Try to place symbol in specified board square and update game logic state and
        /// status display accordingly.
        /// </summary>
        /// <param name="symbol">
        /// PlayerSymbol to try to place in board square.
        /// </param>
        /// <param name="square">
        /// BoardSquare where symbol should be placed, if possible.
        /// </param>
        private void UpdateGameState(PlayerSymbol symbol, BoardSquare square)
        {
            // If we're in cheating mode, we place symbols on board without checking any game rules.
            // We also don't check for victory when in cheating mode.
            if (!this.rulesEnabled)
            {
                square.Symbol = symbol;
                return;
            }

            // Place symbol on square according to game rules
            var status = this.gameLogic.PlaceSymbol(symbol, square);
            switch (status)
            {
                case PlacingStatus.SquareOccupied:
                    UpdateStatusDisplay(Properties.Resources.OccupiedSquare);
                    break;

                case PlacingStatus.OutOfTurn:
                    UpdateStatusDisplay(symbol.Opponent().GetTurnWarningText());
                    break;

                case PlacingStatus.Valid:
                    UpdateStatusDisplay(null);
                    break;

                case PlacingStatus.Draw:
                    UpdateStatusDisplay(Properties.Resources.GameDraw);
                    ShowGameEnd();
                    break;

                case PlacingStatus.Victory:
                    UpdateStatusDisplay(symbol.GetWinAnnouncementText());
                    ShowGameEnd();
                    break;

                case PlacingStatus.GameEnded:
                    this.ShowGameEnd();
                    break;
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        /// <summary>
        /// Handles speech recognition events.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void SpeechRecognized(object sender, SpeechRecognizerEventArgs e)
        {
            // Semantic value associated with speech commands meant to start a new game.
            const string StartGameSpeechCommand = "START";

            // Semantic value associated with speech commands meant to enable cheating mode (for demo purposes).
            const string EnableRulesSpeechCommand = "RULESENABLE";

            // Semantic value associated with speech commands meant to disable cheating mode (for an honest game).
            const string DisableRulesSpeechCommand = "RULESDISABLE";

            // Semantic value associated with speech commands meant to start a rematch game
            const string RematchSpeechCommand = "REMATCH";

            if (null == e.SemanticValue)
            {
                return;
            }
            
            // Handle game mode control commands
            switch (e.SemanticValue)
            {
                case StartGameSpeechCommand:
                    DismissWelcome();
                    return;

                case DisableRulesSpeechCommand:
                    this.rulesEnabled = false;
                    NoRulesText.Visibility = Visibility.Visible;
                    return;

                case EnableRulesSpeechCommand:
                    // Only restart game if we were currently cheating
                    if (!this.rulesEnabled)
                    {
                        StartNewGame();
                    }

                    this.rulesEnabled = true;
                    NoRulesText.Visibility = Visibility.Collapsed;
                    return;

                case RematchSpeechCommand:
                    StartNewGame();
                    return;
            }
            
            // Handle game play commands
            var square = gameLogic.FindSquare(e.SemanticValue);
            if (null == square)
            {
                UpdateStatusDisplay(Properties.Resources.WrongNumber);
                return;
            }

            // We only handle speech commands with an associated sound source angle, so we can find the
            // associated player
            if (!e.SourceAngle.HasValue)
            {
                return;
            }
                
            var player = playerTracker.GetClosestPlayer(e.SourceAngle.Value);
            if (null == player)
            {
                // No players found matching sound source angle. Give visual indication of the problem.
                var viewer = GameBoard.GetAt(square.Row, square.Column);
                viewer.BlinkId();
                return;
            }

            var symbol = player2SymbolMap[player];

            this.UpdateGameState(symbol, square);
        }

        /// <summary>
        /// Handles player status changes, e.g.: players joining or leaving game and player state updates.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void PlayerStatusChanged(object sender, PlayerStatusEventArgs<Player> e)
        {
            switch (e.Status)
            {
                case PlayerStatus.Joined:
                    // As long as there's an available symbol to play with, add new player to game.
                    // Otherwise, just ignore this new player.
                    PlayerSymbol available = FindAvailableSymbol();
                    if (PlayerSymbol.None != available)
                    {
                        player2SymbolMap[e.Player] = available;
                        symbol2PlayerMap[available] = e.Player;
                        symbol2ViewerMap[available].Player = e.Player;
                    }

                    UpdateStatusDisplay(null);
                    break;
                    
                case PlayerStatus.Left:
                    // If player was actively playing the game, stop tracking player and let others join.
                    if (player2SymbolMap.ContainsKey(e.Player))
                    {
                        var symbol = player2SymbolMap[e.Player];
                        symbol2PlayerMap.Remove(symbol);
                        symbol2ViewerMap[symbol].Player = null;
                        player2SymbolMap.Remove(e.Player);
                    }

                    UpdateStatusDisplay(null);
                    break;
            }
        }
    }
}