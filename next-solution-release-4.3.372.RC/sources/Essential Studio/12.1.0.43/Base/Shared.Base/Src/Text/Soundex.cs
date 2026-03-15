#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Text;

namespace Syncfusion.Text
{

    /// <summary>
    ///  Utility class for performing soundex algorithm.
    ///  </summary>
    // The Soundex algorithm is used to convert a word to a
    // code based upon the phonetic sound of the word.

    // The soundex algorithm is outlined below:
    //     Rule 1. Keep the first character of the name.
    //     Rule 2. Perform a transformation on each remaining characters:
    //                 A,E,I,O,U,Y     = A
    //                 H,W             = S
    //                 B,F,P,V         = 1
    //                 C,G,J,K,Q,S,X,Z = 2
    //                 D,T             = 3
    //                 L               = 4
    //                 M,N             = 5
    //                 R               = 6
    //     Rule 3. If a character is the same as the previous, do not include in the code.
    //     Rule 4. If character is "A" or "S" do not include in the code.
    //     Rule 5. If a character is blank, then do not include in the code.
    //     Rule 6. A soundex code must be exactly 4 characters long.  If the
    //             code is too short then pad with zeros, otherwise truncate.


    public class Soundex
    {
        #region Soundex Algorithm
        /// <summary>
        /// Initializing the soundex object.
        /// </summary>
        public Soundex()
        {

        }
        /// <summary>
        /// Return the soundex code for a given string.
        /// </summary>
        public static String ToSoundexCode(String aString)
        {

            String word = aString.ToUpper();
            StringBuilder soundexCode = new StringBuilder();
            int wordLength = word.Length;


            // Rule 1:

            soundexCode.Append(word.Substring(0, 1));

            // Rule 2:

            for (int i = 1; i < wordLength; i++)
            {
                String transformedChar = Transform(word.Substring(i, 1));

                // Rule 3:

                if (!transformedChar.Equals(soundexCode.ToString().Substring(soundexCode.Length - 1)))
                {

                    // Rule 4:

                    if (!transformedChar.Equals("A") && !transformedChar.Equals("S"))
                    {

                        // Rule 5:

                        if (!transformedChar.Equals(" "))
                        {
                            soundexCode.Append(transformedChar);
                        }
                    }
                }
            }
            // Rule 6:

            soundexCode.Append("0000");

            return soundexCode.ToString().Substring(0, 4);
        }
        /// <summary>
        /// Transform the A-Z alphabetic characters to the appropriate soundex code.
        /// </summary>.
        /// <param name="aString">String</param>
        /// <returns>Soundex code for the given word.
        /// </returns>
        private static String Transform(String aString)
        {

            switch (aString)
            {
                case "A":
                case "E":
                case "I":
                case "O":
                case "U":
                case "Y":
                    return "A";
                case "H":
                case "W":
                    return "S";
                case "B":
                case "F":
                case "P":
                case "V":
                    return "1";
                case "C":
                case "G":
                case "J":
                case "K":
                case "Q":
                case "S":
                case "X":
                case "Z":
                    return "2";
                case "D":
                case "T":
                    return "3";
                case "L":
                    return "4";
                case "M":
                case "N":
                    return "5";
                case "R":
                    return "6";
            }

            return " ";
        }
        #endregion

        #region EditDistanceAlgortihm
        // Compute Levenshtein Distance Between The Two strings. The Larger The Number, The Bigger The Difference.
        /// <summary>
        /// Compute Levenshtein distance.
        /// </summary>
        /// <param name="s">String 1</param>
        /// <param name="t">String 2</param>
        /// <returns>Distance between the two strings.
        /// The larger the number, the bigger the difference.
        /// </returns>
        // Compute Levenshtein Distance Between The Two strings. The Larger The Number, The Bigger The Difference.

        // Rule 1.
        // Set n to be the length of First String source.
        // Set m to be the length of Second String temp.
        // If n = 0, return m and exit.
        // If m = 0, return n and exit.
        // Construct a matrix containing 0..m rows and 0..n columns. 

        //Rule 2.
        // Initialize the first row to 0..n.
        // Initialize the first column to 0..m.

        //Rule 3.
        // Examine each character of source (i from 1 to n).

        //Rule 4.
        // Examine each character of temp (j from 1 to m).

        //Rule 5.
        // If s[i] equals t[j], the cost is 0.
        // If s[i] doesn't equal t[j], the cost is 1.

        //Rule 6.
        // Set cell d[i,j] of the matrix equal to the minimum of:
        // a. The cell immediately above plus 1: d[i-1,j] + 1.
        // b. The cell immediately to the left plus 1: d[i,j-1] + 1.
        // c. The cell diagonally above and to the left plus the cost: d[i-1,j-1] + cost.

        //Rule 7.
        // After the iteration steps (3, 4, 5, 6) are complete, the distance is found in cell d[n,m]. 

        public static int EditDistanceCompute(string s, string t)
        {
            int distance; //Indicates the distance between two strings.
            string source = s;
            string temp = t;
            int n = source.Length;
            int m = temp.Length;
            int[,] d = new int[n + 1, m + 1];
            int cost;

            //Rule 1:

            if (n == 0) distance = m;
            if (m == 0) distance = n;

            //Rule 2: 

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            //Rule 3:

            for (int i = 1; i <= n; i++)
            {

                //Rule 4:

                for (int j = 1; j <= m; j++)
                {

                    //Rule 5:

                    cost = (temp.Substring(j - 1, 1) == source.Substring(i - 1, 1) ? 0 : 1);

                    //Rule 6:

                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);

                }
            }

            distance = d[n, m];

            //Rule 7:

            return distance;
        }

        #endregion

        #region NextAlbhapetWord
        /// <summary>
        /// Gets the next word from the alphabet list.
        /// </summary>
        public static string NextLetter(string aString)
        {

            switch (aString)
            {
                case "a": return ("b");
                case "b": return ("c");
                case "c": return ("d");
                case "d": return ("e");
                case "e": return ("f");
                case "f": return ("g");
                case "g": return ("h");
                case "h": return ("i");
                case "i": return ("j");
                case "j": return ("k");
                case "k": return ("l");
                case "l": return ("m");
                case "m": return ("n");
                case "n": return ("o");
                case "o": return ("p");
                case "p": return ("q");
                case "q": return ("r");
                case "r": return ("s");
                case "s": return ("t");
                case "t": return ("u");
                case "u": return ("v");
                case "v": return ("w");
                case "w": return ("x");
                case "x": return ("y");
                case "y": return ("z");
                case "z": return (" ");

            }

            return " ";
        }
        #endregion

    }
}
