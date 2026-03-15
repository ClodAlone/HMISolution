using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Utilities
{
    public class GeneralCommand : RoutedUICommand
    {
        // Static converter for changing strings from Ctrl+Q into the appropriate KeyGesture
        // object. This is the same converter used by the xaml parser, so anything you can put
        // in the KeyGesture="" section of a xaml file, you should be able to put in the 
        // resources for your gestures.
        private static KeyGestureConverter _keyGestureConverter = new KeyGestureConverter();


        private string _tooltip = string.Empty;
        private string _description = string.Empty;

        public string Tooltip
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Create a ScribbleCommand cloned from an existing RoutedUICommand
        /// </summary>
        /// <param name="clone"></param>
        public GeneralCommand(RoutedUICommand clone,
            string accessKey,
            string tooltip,
            string description,
            Type ownerType)
            : base(clone.Text, clone.Name, ownerType, clone.InputGestures)
        {
            this.Text = clone.Text;
            this.Tooltip = tooltip;
            this.Description = description;
            AddMnemonic(accessKey);
        }

        /// <summary>
        /// Create a brand new Scribble command not based on an existing RoutedUICommand
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayText"></param>
        /// <param name="gestures"></param>
        /// <param name="gesturesDisplayText"></param>
        /// <param name="tooltip"></param>
        /// <param name="description"></param>
        /// <param name="ownerType"></param>
        public GeneralCommand(
            string name,
            string displayText,
            string gestures,
            string gesturesDisplayText,
            string tooltip,
            string description,
            Type ownerType)
            : base(displayText, name, ownerType)
        {
            this.Tooltip = tooltip;
            this.Description = description;
            AddGesturesFromResourceStrings(
                gestures,
                gesturesDisplayText);
        }

        public GeneralCommand()
        {
        }

        public GeneralCommand(string text, string name, Type ownerType)
            : base(text, name, ownerType)
        {
            
        }
        public GeneralCommand(string text, string name, Type ownerType, InputGestureCollection inputGestures)
            : base(text, name, ownerType, inputGestures)
        {
            
        }
         

        public void AddMnemonic(string accessKey)
        {
            if (String.IsNullOrEmpty(accessKey))
                return;
            if (String.IsNullOrEmpty(Text))
                return;
            if (Text.Contains("_"))
                return;
            int accessIndex = Text.IndexOf(accessKey);
            if (accessIndex >= 0)
            {
                Text = Text.Insert(accessIndex, "_");
            }
        }

        /// <summary>
        /// This comes from the internal implementation within the ApplicationCommands class. Thanks Lutz!
        /// </summary>
        /// <param name="keyGestures"></param>
        /// <param name="displayStrings"></param>
        /// <param name="gestures"></param>
        private void AddGesturesFromResourceStrings(string keyGestures, string displayStrings)
        {
            while (!string.IsNullOrEmpty(keyGestures))
            {
                string currentDisplay;
                string currentGesture;
                int index = keyGestures.IndexOf(";", StringComparison.Ordinal);
                if (index >= 0)
                {
                    currentGesture = keyGestures.Substring(0, index);
                    keyGestures = keyGestures.Substring(index + 1);
                }
                else
                {
                    currentGesture = keyGestures;
                    keyGestures = string.Empty;
                }
                index = displayStrings.IndexOf(";", StringComparison.Ordinal);
                if (index >= 0)
                {
                    currentDisplay = displayStrings.Substring(0, index);
                    displayStrings = displayStrings.Substring(index + 1);
                }
                else
                {
                    currentDisplay = displayStrings;
                    displayStrings = string.Empty;
                }
                KeyGesture inputGesture = CreateFromResourceStrings(currentGesture, currentDisplay);
                if (inputGesture != null)
                {
                    InputGestures.Add(inputGesture);
                }
            }
        }

        private KeyGesture CreateFromResourceStrings(string keyGestureToken, string keyDisplayString)
        {
            try
            {
                if (!string.IsNullOrEmpty(keyDisplayString))
                {
                    keyGestureToken = keyGestureToken + ',' + keyDisplayString;
                }
                return (_keyGestureConverter.ConvertFromInvariantString(keyGestureToken) as KeyGesture);
            }
            catch { }

            return null;
        }
    }
}
