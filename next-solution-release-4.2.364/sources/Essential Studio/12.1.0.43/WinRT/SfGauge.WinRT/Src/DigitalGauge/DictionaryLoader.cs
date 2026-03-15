#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    /// <summary>
    /// It is a static class which  contains the list of brushes for each characters for
    /// all the four type of character segments
    /// </summary>
    public  class DictionaryLoader
    {

        #region Constructor

        public DictionaryLoader()
        {

        }

        #endregion

        #region Private Members

        /// <summary>
        /// Collection of bool values indicating what character segments should be 
        /// drawn with foreground brush.
        /// </summary>
        //  private List<bool> ListSegments = new List<bool>();

        internal  Dictionary<char, List<Brush>> sixteenSegmentDictionary;

        internal  Dictionary<char, List<Brush>> fourteenSegmentDictionary;

        internal  Dictionary<char, List<Brush>> sevenSegmentDictionary;

        internal  Dictionary<char, List<Brush>> eightMatrixDictionary;

        internal  void InitializeEightMatrixDictionary(Brush BrightBrush, Brush DimmedBrush)
        {
            eightMatrixDictionary = new Dictionary<char, List<Brush>>();

            eightMatrixDictionary.Add('A', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                              });

            eightMatrixDictionary.Add('B', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('C', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('D', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('E', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('F', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('G', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('H', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('I', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('J', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('K', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('L', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                              });
            eightMatrixDictionary.Add('M', new List<Brush>() {  BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('N', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('O', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('P', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('Q', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('R', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('S', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('T', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('U', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('V', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('W', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                });
            eightMatrixDictionary.Add('X', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('Y', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('Z', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,

                                                              });

            eightMatrixDictionary.Add('a', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	BrightBrush,


                                                              });

            eightMatrixDictionary.Add('b', new List<Brush>() { BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('c', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('d', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,


                                                              });
            eightMatrixDictionary.Add('e', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('f', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('g', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('h', new List<Brush>() { BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('i', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });

            eightMatrixDictionary.Add('j', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('k', new List<Brush>() { BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                              });

            eightMatrixDictionary.Add('l', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('m', new List<Brush>() { DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('n', new List<Brush>() { BrightBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('o', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('p', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('q', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('r', new List<Brush>() { BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('s', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                              });

            eightMatrixDictionary.Add('t', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('u', new List<Brush>() { DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,  
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                              });

            eightMatrixDictionary.Add('v', new List<Brush>() { DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('w', new List<Brush>() { DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('x', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,


                                                              });
            eightMatrixDictionary.Add('y', new List<Brush>() { DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('z', new List<Brush>() { DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('0', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('1', new List<Brush>() { DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
            });

            eightMatrixDictionary.Add('2', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,


                                                              });
            eightMatrixDictionary.Add('3', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('4', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                              });
            eightMatrixDictionary.Add('5', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('6', new List<Brush>() {  DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('7', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('8', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,


                                                              });
            eightMatrixDictionary.Add('9', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('+', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('-', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('*', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('/', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('\\', new List<Brush>() {BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,

                                                              });
            eightMatrixDictionary.Add('_', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                              });
            eightMatrixDictionary.Add('|', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add(' ', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('[', new List<Brush>() {  BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });

            eightMatrixDictionary.Add(']', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                              });
            eightMatrixDictionary.Add('?', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                            });
            eightMatrixDictionary.Add('=', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('<', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('>', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('^', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('$', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('.', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add(',', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('"', new List<Brush>() {  DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,

                                                              });
            eightMatrixDictionary.Add('\'', new List<Brush>() { DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('%', new List<Brush>() {  BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,
                                                              });
            eightMatrixDictionary.Add('@', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('&', new List<Brush>() {  DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,
                                                            });
            eightMatrixDictionary.Add('~', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,
                                                                BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                             });
            eightMatrixDictionary.Add('`', new List<Brush>() {  BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                          });

            eightMatrixDictionary.Add('#', new List<Brush>() {  DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,	BrightBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('(', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add(')', new List<Brush>() {  DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                });
            eightMatrixDictionary.Add('!', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('{', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add('}', new List<Brush>() {  DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	BrightBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });
            eightMatrixDictionary.Add(':', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                             });
            eightMatrixDictionary.Add(';', new List<Brush>() {  DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                                DimmedBrush,	DimmedBrush,	DimmedBrush,	BrightBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,	DimmedBrush,
                                                              });

        }

        internal  void InitializeSevenDictionary(Brush BrightBrush, Brush DimmedBrush)
        {
            sevenSegmentDictionary = new Dictionary<char, List<Brush>>();
            sevenSegmentDictionary.Add('0', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('1', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('2', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add('3', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('4', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('5', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('6', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('7', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('8', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('9', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('A', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('B', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, });
            sevenSegmentDictionary.Add('C', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add('D', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('E', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add('F', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sevenSegmentDictionary.Add('G', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('H', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('I', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('J', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('K', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('L', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add('M', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('N', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, });
            sevenSegmentDictionary.Add('O', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('P', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sevenSegmentDictionary.Add('Q', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add('R', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sevenSegmentDictionary.Add('S', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('T', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, });
            sevenSegmentDictionary.Add('U', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('V', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('W', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('X', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('Y', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('Z', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add('a', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('b', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, });
            sevenSegmentDictionary.Add('c', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add('d', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('e', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add('f', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sevenSegmentDictionary.Add('g', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('h', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('i', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('j', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('k', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('l', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sevenSegmentDictionary.Add('m', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('n', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, });
            sevenSegmentDictionary.Add('o', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('p', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sevenSegmentDictionary.Add('q', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('r', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sevenSegmentDictionary.Add('s', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('t', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, });
            sevenSegmentDictionary.Add('u', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('v', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sevenSegmentDictionary.Add('w', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sevenSegmentDictionary.Add('x', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('y', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sevenSegmentDictionary.Add('z', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush });
            sevenSegmentDictionary.Add(' ', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
        }

        internal  void InitializeFourteenDictionaryList(Brush BrightBrush, Brush DimmedBrush)
        {
            fourteenSegmentDictionary = new Dictionary<char, List<Brush>>();
            fourteenSegmentDictionary.Add('0', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('1', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('2', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('3', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('4', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('5', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('6', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('7', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('8', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('9', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('A', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('B', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('C', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('D', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('E', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('F', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('G', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('H', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('I', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('J', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('K', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('L', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('M', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('N', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('O', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('P', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('Q', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('R', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('S', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('T', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('U', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('V', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('W', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('X', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('Y', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('Z', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('a', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('b', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('c', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('d', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('e', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('f', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('g', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('h', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('i', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('j', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('k', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('l', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('m', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('n', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('o', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('p', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('q', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('r', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('s', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('t', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('u', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('v', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('w', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('x', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('y', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('z', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('-', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('+', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('*', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('|', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('\\', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('/', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('_', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add(' ', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('[', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add(']', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('?', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('=', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('<', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('>', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('^', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('$', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('.', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add(',', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('\'', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('"', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('%', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('&', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('@', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('(', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add(')', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('!', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('~', new List<Brush>() { DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('`', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            fourteenSegmentDictionary.Add('#', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            fourteenSegmentDictionary.Add('{', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add('}', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add(':', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            fourteenSegmentDictionary.Add(';', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            
        }


        internal  void InitializeSixteenDictionaryList(Brush BrightBrush, Brush DimmedBrush)
        {
            sixteenSegmentDictionary = new Dictionary<char, List<Brush>>();
            sixteenSegmentDictionary.Add('0', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('1', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('2', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('3', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('4', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('5', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('6', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('7', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('8', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('9', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('A', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('B', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('C', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('D', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('E', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('F', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('G', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('H', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('I', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('J', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('K', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('L', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('M', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('N', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('O', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('P', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('Q', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('R', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('S', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('T', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('U', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('V', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('W', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('X', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('Y', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('Z', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('a', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('b', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('c', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('d', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('e', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('f', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('g', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('h', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('i', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('j', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('k', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('l', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('m', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('n', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('o', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('p', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('q', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('r', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('s', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('t', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('u', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('v', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('w', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('x', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('y', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('z', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('-', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('+', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('*', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('|', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('\\', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('/', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('_', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add(' ', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('[', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sixteenSegmentDictionary.Add(']', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush });
            sixteenSegmentDictionary.Add('?', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('=', new List<Brush>() { BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('<', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('>', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('^', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('$', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('.', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sixteenSegmentDictionary.Add(',', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('\'', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('"', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('%', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('&', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('@', new List<Brush>() { BrightBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('~', new List<Brush>() { DimmedBrush, DimmedBrush, BrightBrush, BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('`', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('#', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush, BrightBrush, BrightBrush });
            sixteenSegmentDictionary.Add('!', new List<Brush>() { DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add('(', new List<Brush>() { BrightBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sixteenSegmentDictionary.Add(')', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, BrightBrush });
            sixteenSegmentDictionary.Add('{', new List<Brush>() { DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush });
            sixteenSegmentDictionary.Add('}', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush });
            sixteenSegmentDictionary.Add(':', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
            sixteenSegmentDictionary.Add(';', new List<Brush>() { BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, BrightBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush, DimmedBrush });
          }

        #endregion Private Members

    }
}
