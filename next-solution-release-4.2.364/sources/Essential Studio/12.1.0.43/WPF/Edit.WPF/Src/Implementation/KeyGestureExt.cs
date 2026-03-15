#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// KeyGestureExt class is an extension of KeyGesture class. This class contains
    /// implementation for supporting multiple key gestures additionally.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class KeyGestureExt : KeyGesture
    {
        #region local Variables

        /// <summary>
        /// Local variable to hold any number of Keys in a KeyGesture.
        /// </summary>
        private List<Key> keyCollection;

        /// <summary>
        /// Local static variable holds the index of key pressed to match keys from KeyCollection.
        /// </summary>
        internal static int keyIndex = 0;

        /// <summary>
        /// Local static variable holds the DateTime value when the last matching key was pressed.
        /// </summary>
        internal static DateTime prevKeyTime = DateTime.Now;

        #endregion local Variables

        #region Properties

        /// <summary>
        /// Gets or sets the Keys collection for key gesture.
        /// </summary>
        /// <remarks>
        /// Keys should be given in the order in which they have to be matched.
        /// </remarks>
        public List<Key> KeyCollection
        {
            get
            {
                return keyCollection;
            }
            set
            {
                keyCollection = value;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Edit.KeyGestureExt">KeyGestureExt</see> class.
        /// </summary>
        /// <param name="key">specifies the key in a KeyGesture</param>
        public KeyGestureExt(Key key)
            : this(new List<Key>(new Key[] { key }), ModifierKeys.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Edit.KeyGestureExt">KeyGestureExt</see> class.
        /// </summary>
        /// <param name="key">Specifies the key in a KeyGesture</param>
        /// <param name="modfiers">Specifies the modifier key</param>
        public KeyGestureExt(Key key, ModifierKeys modfiers)
            : this(new List<Key>(new Key[] { key }), modfiers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Edit.KeyGestureExt">KeyGestureExt</see> class.
        /// </summary>
        /// <param name="keys">Specifies the collection of Keys for multiple Key KeyGesture.
        /// Keys should be given in the order they have to be matched.</param>
        /// <param name="modifiers">Specifies the Modifier key.</param>
        public KeyGestureExt(List<Key> keys, ModifierKeys modifiers)
            : base(Key.None, modifiers)
        {
            if (keys == null || keys.Count == 0)
            {
                throw new ArgumentOutOfRangeException("Keys collection cannot be empty");
            }

            keyCollection = keys;
        }

        #endregion Constructors

        #region Overrides

        /// <summary>
        /// Overridden method of KeyGesture class used to match the KeyGestures.
        /// </summary>
        /// <param name="targetElement">Specifies the element on which the KeyGesture is
        /// set.</param>
        /// <param name="inputEventArgs">Specifies the KeyEventArgs containing information
        /// related to the key pressed and modifiers.</param>
        /// <returns>
        /// returns true if the key gestures match else returns false.
        /// </returns>
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!(inputEventArgs is KeyEventArgs))
                return false;

            Key tempKeyPressed = (inputEventArgs as KeyEventArgs).Key;

            if (KeyGestureExt.keyIndex == 0 && Keyboard.Modifiers != this.Modifiers)
            {
                prevKeyTime = DateTime.Now;
                return false;
            }

            if ((DateTime.Now - prevKeyTime) > TimeSpan.FromSeconds(1d))
            {
                KeyGestureExt.keyIndex = 0;
                return false;
            }

            if (keyCollection.Count > KeyGestureExt.keyIndex && keyCollection[KeyGestureExt.keyIndex] == tempKeyPressed)
            {
                if (keyCollection.Count - 1 == KeyGestureExt.keyIndex)
                {
                    KeyGestureExt.keyIndex = 0;
                    return true;
                }
                else
                {
                    KeyGestureExt.keyIndex += 1;
                    prevKeyTime = DateTime.Now;
                    return false;
                }
            }

            return false;
        }

        #endregion Overrides
    }
}