using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace test
{
    /// <summary>
    /// Class to manage the status of the Num lock, Scroll lock, Insert and Caps lock keys.
    /// </summary>
    public class KeyManager : INotifyPropertyChanged
    {
        #region Declarations

        #region Members
        private bool _isScrollPressed;
        private bool _isNumPressed;
        private bool _isCapsPressed;
        private bool _isInsPressed;
        #endregion

        /// <summary>
        /// API call to get the key state for a particular key.
        /// </summary>
        /// <param name="keyCode">The keycode to check.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int keyCode);

        /// <summary>
        /// The virtual keys we want to check.
        /// </summary>
        public enum VKeyStates
        {
            /// <summary>
            /// The caps lock key.
            /// </summary>
            CapsKey = 0x14,
            /// <summary>
            /// The num key.
            /// </summary>
            NumKey = 0x90,
            /// <summary>
            /// The scroll key.
            /// </summary>
            ScrollKey = 0x91,
            /// <summary>
            /// The ins key.
            /// </summary>
            InsKey = 0x2d
        }
        #endregion

        /// <summary>
        /// Initialize a new instance of <see cref="KeyManager"/>.
        /// </summary>
        /// <param name="app">The window owner.</param>
        public KeyManager(Window app)
        {
            app.PreviewKeyUp += new KeyEventHandler(ManageKeyboard);
            ManageKeys();
        }

        /// <summary>
        /// Event handler called before the keyup event fires.
        /// </summary>
        void ManageKeyboard(object sender, KeyEventArgs e)
        {
            ManageKeys();
        }

        /// <summary>
        /// Method to get the state of each key.
        /// </summary>
        internal void ManageKeys()
        {
            IsNumPressed = GetKeyState((int)VKeyStates.NumKey) != 0;
            IsScrollPressed = GetKeyState((int)VKeyStates.ScrollKey) != 0;
            IsInsPressed = GetKeyState((int)VKeyStates.InsKey) != 0;
            IsCapsPressed = GetKeyState((int)VKeyStates.CapsKey) != 0;
        }

        /// <summary>
        /// Property changed notification event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void Changed(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Get or set the scroll key status.
        /// </summary>
        public bool IsScrollPressed
        {
            get
            {
                return _isScrollPressed;
            }
            set
            {
                if (_isScrollPressed != value)
                {
                    _isScrollPressed = value;
                    Changed("IsScrollPressed");
                }
            }
        }

        /// <summary>
        /// Get or set the numlock key status.
        /// </summary>
        public bool IsNumPressed
        {
            get
            {
                return _isNumPressed;
            }
            set
            {
                if (_isNumPressed != value)
                {
                    _isNumPressed = value;
                    Changed("IsNumPressed");
                }
            }
        }

        /// <summary>
        /// Get or set the Caps lock key status.
        /// </summary>
        public bool IsCapsPressed
        {
            get
            {
                return _isCapsPressed;
            }
            set
            {
                if (_isCapsPressed != value)
                {
                    _isCapsPressed = value;
                    Changed("IsCapsPressed");
                }
            }
        }

        /// <summary>
        /// Get or set the Insert key status.
        /// </summary>
        public bool IsInsPressed
        {
            get
            {
                return _isInsPressed;
            }
            set
            {
                if (_isInsPressed != value)
                {
                    _isInsPressed = value;
                    Changed("IsInsPressed");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
