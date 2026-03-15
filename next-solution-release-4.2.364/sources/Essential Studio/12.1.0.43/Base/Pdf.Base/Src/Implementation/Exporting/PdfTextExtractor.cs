#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE

using System;
using System.Collections.Generic;
using System.Text;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Exporting
{
    /// <summary>
    /// Represents the utility class for extracting text from the stream.
    /// </summary>
    internal class PdfTextExtractor
    {
        #region Constants
        /// <summary>
        /// Internal variable to store the number of characters.
        /// </summary>
        private static int m_numberOfChars = 50;
        #endregion
        #region Fields
        /// <summary>
        /// variable to hold  font name and difference arrary.
        /// </summary>
        private static Dictionary<string, List<string>> m_differenceArray;
        /// <summary>
        /// variable to hold decoded Character
        /// </summary>
        private static List<string> m_decodedChar;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextExtractor"/> class.
        /// </summary>
        PdfTextExtractor()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Extracts text from the bytes.
        /// </summary>
        /// <param name="input">The byte array.</param>
        /// <returns>The Extracted Text.</returns>
        public static string ExtractTextFromBytes(Byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return " ";
            }
            
            try
            {
                string resultantText = string.Empty;
                bool inText = false;
                string prevText = null;
                string currentText = null;
                string nextText = null;
                string text = null;
                bool modify=false;
                bool nextLiteral = false;
                int bracketDepth = 0;
                string result = "";
                 bool type=false;
                bool even=false;
                bool encoded = false;
                //Changed Rev-->
                string Prev_TmYposition = string.Empty;
                string Curr_TmYposition = string.Empty;
                string Prev_TmXposition = string.Empty;
                string Curr_TmXposition = string.Empty;
                //Changed Rev--> 
                char[] previousCharacters = new char[m_numberOfChars];
                int count = 0;

                for (int j = 0; j < m_numberOfChars; j++)
                {
                    previousCharacters[j] = ' ';
                }
                float prevYPosition = 0f;
                bool InitYPostion = true;
                bool Isopen = false;
                for (int i = 0; i < data.Length; i++)
                {
                    char c = (char)data[i];
                    if (m_differenceArray.Count > 0)
                    {
                        if (CheckToken(new string[] { "Tf" }, previousCharacters))
                        {
                            int startIndex = 0;
                            int stopIndex = 0;
                            for (int charIndex = 0; charIndex < previousCharacters.Length; charIndex++)
                            {
                                if (previousCharacters[charIndex] == '/')
                                {
                                    startIndex = charIndex;
                                }
                                else if (charIndex > 0)
                                {
                                    if ((previousCharacters[charIndex] == 'f') && (previousCharacters[charIndex - 1] == 'T'))
                                    {
                                        stopIndex = charIndex;
                                       // break;
                                    }
                                }
                            }

                            string fontName = new string(previousCharacters);
                            string tempFont = fontName.Substring(startIndex + 1, ((stopIndex - 1) - startIndex));
                            int startPosition = tempFont.IndexOf(' ');
                            string m_fontName = tempFont.Substring(0, (startPosition));

                            if (m_differenceArray.ContainsKey(m_fontName))
                            {
                                foreach (KeyValuePair<string, List<string>> item in m_differenceArray)
                                {
                                    if (item.Key.Equals(m_fontName))
                                    {
                                        m_decodedChar = new List<string>();
                                        m_decodedChar = item.Value;
                                        encoded = true;
                                    }

                                }
                            }
                            else
                            {
                                encoded = false;
                            }
                        }
                    }
                    if (inText)
                    {
                        if ((c == '[') && ((char)data[i + 1] == '(') && ((char)data[i -1] == '\n'))
                        {
                            Isopen = true;
                        }

                        if ((c == ']') && ((char)data[i - 1] == ')'))
                        {
                            if (((char)data[i + 1] == ' ') && ((char)data[i + 2] == 'T') && ((char)data[i + 2] == 'J'))
                            {
                                Isopen = false;
                            }
                        }

                        if ((c == 'T') && ((char)data[i + 1] == '*') && (((char)data[i + 2] == '[') || ((char)data[i + 2] == '(')))
                        {
                            resultantText += Environment.NewLine;
                        }

                        if (((int)data[i] == 39) && ((char)data[i - 1] == ')'))
                        {
                            if((resultantText.Length>0)&& (resultantText[resultantText.Length-1]!='\n'))
                            {
                                resultantText += Environment.NewLine;
                            }
                        }
                        else if (((char)data[i] == 'T') && ((char)data[i + 1] == 'D') && ((char)data[i - 1] == ' '))
                        {
                            string TDValue=string.Empty;
                            string value=string.Empty;
                            for (int length = previousCharacters.Length - 2; length >=0; length--)
                            {
                                char space = (char)previousCharacters[length];
                                if (space != ' ')
                                {
                                    value += space;
                                }
                                else
                                {
                                    for (int stlen = value.Length - 1; stlen >= 0; stlen--)
                                    {
                                        TDValue += value[stlen];
                                    }
                                    break;
                                }
                            }
                            if (TDValue != string.Empty)
                            if (InitYPostion)
                            {
                                prevYPosition = Convert.ToSingle(TDValue);
                                InitYPostion = false;
                            }
                            else
                            {
                                float Position = Convert.ToSingle(TDValue);
                                float diff = prevYPosition - Position;
                                if ((diff != 0) && (Position != 0))
                                {
                                    resultantText += Environment.NewLine;
                                }
                                prevYPosition = Position;
                            }                         

                        }
                        else if (((char)data[i] == 'T') && ((char)data[i + 1] == 'd') && ((char)data[i - 1] == ' '))
                        {
                            string TDValue = string.Empty;
                            string value = string.Empty;
                            for (int length = previousCharacters.Length - 2; length >= 0; length--)
                            {
                                char space = (char)previousCharacters[length];
                                if (space != ' ')
                                {
                                    value += space;
                                }
                                else
                                {
                                    for (int stlen = value.Length - 1; stlen >= 0; stlen--)
                                    {
                                        TDValue += value[stlen];
                                    }
                                    break;
                                }
                            }

                            if (InitYPostion)
                            {
                                prevYPosition = Convert.ToSingle(TDValue);
                                InitYPostion = false;
                            }
                            else
                            {
                                float Position = Convert.ToSingle(TDValue);
                                float diff = prevYPosition - Position;
                                if ((diff != 0) && (Position!=0))
                                {
                                    resultantText += Environment.NewLine;
                                }
                                prevYPosition = Position;
                            }
                           
                        }
                        else if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                        {
                            resultantText += Environment.NewLine;
                        }
                        else if (((char)data[i] == 'T') && ((char)data[i + 1] == 'm') && ((char)data[i - 1] == ' ') && (((char)data[i +2] == '\n')||((char)data[i+2]=='\r')))
                        {
                            //Changed Rev-->
                            string Tm_Yposition = string.Empty;
                            string Tm_Xposition = string.Empty;
                            int temp = i - 2;
                            char character;
                           /* while ((temp != 0) && (data[temp] != ' '))
                            {
                                character = (char)data[temp];
                                Tm_Xposition = character.ToString() + Tm_Xposition;
                                temp--;
                            }
                            temp--;*/

                            while (data[temp] != ' ')
                            {
                                character = (char)data[temp];
                                Tm_Yposition = character.ToString() + Tm_Yposition;
                                temp--;
                            }
                            Curr_TmYposition = Tm_Yposition;
                            if (Prev_TmYposition == "")
                                Prev_TmYposition = "0";
                            if (Curr_TmXposition != string.Empty)
                            if (Convert.ToSingle(Curr_TmYposition)< Convert.ToSingle(Prev_TmYposition)||Prev_TmYposition=="0")
                                //Changed rev-->
                            resultantText += Environment.NewLine;
                        //Changed Rev
                        if (Curr_TmXposition != Prev_TmXposition)
                            resultantText += " ";
                        Prev_TmXposition = Curr_TmXposition;
                        Prev_TmYposition = Curr_TmYposition;
                        //Changed Rev
                        }

                        if (bracketDepth == 0)
                        {
                            if (((char)data[i] == 'T') && ((char)data[i + 1] == 'm') && ((char)data[i + 2] == '\n'))
                            {
                                if (resultantText.Length > 0 && (resultantText[resultantText.Length - 1] == ' '))
                                {
                                    resultantText = resultantText.Remove(resultantText.Length - 1, 1);
                                }
                            }

                            if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                            {
                                //resultantText += Environment.NewLine;
                            }
                            else if (data[i - 1] == '\n')
                            {
                                if (!resultantText.EndsWith(Environment.NewLine))
                                {
                                    if (prevText == null)
                                    {
                                        prevText = " ";
                                    }
                                    currentText = resultantText;
                                    string temp = resultantText;

                                    if (prevText == " ")
                                    {
                                        prevText = currentText;
                                    }

                                    if (prevText.Length != currentText.Length)
                                    {
                                        try
                                        {
                                            nextText = currentText.Substring(prevText.Length, (currentText.Length - prevText.Length));
                                            nextText = nextText.Trim('\r');
                                            nextText = nextText.Trim('\n');
                                            nextText = nextText.Trim(' ');
                                            if (nextText == text)
                                            {
                                                if (nextText.Length > 0)
                                                {
                                                    resultantText = resultantText.Substring(0, (resultantText.Length - nextText.Length) - 1);
                                                }
                                                
                                                if ((resultantText.Length > 0) && (resultantText[resultantText.Length - 1] != '\n'))
                                                {
                                                    resultantText += Environment.NewLine;
                                                }
                                                currentText = resultantText;
                                            }
                                            text = nextText;
                                        }
                                        catch (Exception exception)
                                        {
                                        }
                                    }
                                    prevText = currentText;
                                }
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                     resultantText += Environment.NewLine;
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        resultantText += string.Empty;
                                    }
                                }

                            }
                        }

                        if (bracketDepth == 0 && CheckToken(new string[] { "ET" }, previousCharacters))
                        {
                            inText = false;
                            if (!Isopen)
                            {
                                resultantText += " ";
                            }
                        }
                        else
                        {
                            if ((c == '<') && (bracketDepth == 0) && (!nextLiteral))
                            {
                               type=false;

                               bool inner = true;
                               char cha = (char)data[i + 1];
                               if (!char.IsDigit(cha))
                                   inner = false;
                                for (int j = i; j < data.Length - 1; j++)
                                {
                                    char ch = (char)data[j];
                                    if (ch == '>' && inner)
                                    
                                    {
                                        byte[] hexEncode = new byte[j - i - 1];
                                        int counter = 0;
                                        for (int k = i + 1; k < j; k++)
                                        {
                                            hexEncode[counter] = data[k];
                                            counter++;
                                        }
                                        UTF8Encoding utf8 = new UTF8Encoding();
                                        count = 0;
                                        byte[] temp = new byte[4];
                                        for (int a = 0; a < j - i - 1; a++)
                                        {
                                            temp[count] = hexEncode[a];
                                            if (count == 3)
                                            {
                                                String decodedString = utf8.GetString(temp);
                                                Int64 tmp = Int64.Parse(decodedString, System.Globalization.NumberStyles.HexNumber);
                                                if (tmp < 5000)
                                                {
                                                    char tmp1 = (char)tmp;
                                                    resultantText += tmp1;
                                                    count = 0;
                                                }
                                            }
                                            else
                                                count++;
                                        }                                        
                                        char[] Tjarray=new char[3];
                                        Array.Copy(data, j + 1, Tjarray, 0, 3);
                                        string checkTj = new string(Tjarray);
                                        if (checkTj.IndexOf("Tj") != -1 || checkTj.IndexOf("TJ") != -1)
                                        {
                                            type = true;
                                            bracketDepth = 1;
                                         }
                                        break;
                                       /* if (((char)data[j + 1] == ' ') || ((char)data[j + 1] == 'T'))
                                        {
                                            if (((char)data[j + 2] == 'T') || ((char)data[j + 2] == 'j') || ((char)data[j + 2] == 'J'))
                                            {
                                                type = true;
                                                bracketDepth = 1;
                                                break;
                                                if (((char)data[j + 3] == 'T') || ((char)data[j + 3] == 'j') || ((char)data[j + 3] == 'J'))
                                                {
                                                    type = true;
                                                    bracketDepth = 1;
                                                    break;
                                                }
                                            }
                                        }*/
                                    }
                                 }
                                /*if (type)
                                {
                                    resultantText  = ExtractTextFromBytes(data, true);
                                    return resultantText;
                                }*/

                            }
                           
                            else if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                                for(int j=i;data[j]!=')';j++)
                                {
                                if (!((data[i + 1] >= ' ' && data[i + 1] <= '~') || ((data[i + 1] >= 128) && (data[i + 1] < 255))))
                                    modify = true;
                                }
                                /*else
                                    modify = true;
                                 * */
                            }
                            else
                            {
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                    if (modify = true)
                                        modify = false;
                                    if (!Isopen)
                                    {
                                        resultantText +=string.Empty;
                                    }
                                }
                                else if ((c == '>') && (bracketDepth == 1) && (!nextLiteral)&&(type==true))
                                {
                                    bracketDepth = 0;
                                    type = false;
                                }
                                else
                                {
                                    if (bracketDepth == 1)
                                    {
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~')) ||
                                                ((c >= 128) && (c < 255)))
                                            {
                                                if (type)
                                                {

                                                    if (even)
                                                    {
                                                        result += c.ToString();
                                                        ulong uiHex2 = 0;
                                                        uiHex2 = Convert.ToUInt64(result.ToString(), 16);
                                                        char ch = Convert.ToChar(uiHex2);
                                                        if (ch != 0)
                                                            resultantText += ch.ToString();
                                                        even = false;
                                                        result = string.Empty;
                                                    }
                                                    else
                                                    {
                                                        result += c.ToString();
                                                        even = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (encoded == true)
                                                    {
                                                        if ((int)c > m_decodedChar.Count)
                                                        {
                                                            if (c == 'n' && (int)data[i - 1] == 92)
                                                            {
                                                                char newLine = '\n';
                                                                int position = (int)newLine;
                                                                resultantText += m_decodedChar[position];
                                                            }
                                                            else if (c == 'r' && (int)data[i - 1] == 92)
                                                            {
                                                                char carriageReturn = '\r';
                                                                int position = (int)carriageReturn;
                                                                resultantText += m_decodedChar[position];
                                                            }
                                                            else
                                                                resultantText += c.ToString();
                                                        }
                                                        else
                                                            resultantText += m_decodedChar[(int)c];
                                                    }
                                                    else
                                                    {
                                                        if (modify)
                                                        {
                                                            resultantText += (char)((int)c + 29);
                                                        }
                                                        else
                                                            resultantText += c.ToString();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (encoded == true)
                                                {
                                                    if ((int)c > m_decodedChar.Count)
                                                    {
                                                        if (c == 'n' && (int)data[i - 1] == 92)
                                                        {
                                                            char newLine = '\n';
                                                            int position = (int)newLine;
                                                            resultantText += m_decodedChar[position];
                                                        }
                                                        else if (c == 'r' && (int)data[i - 1] == 92)
                                                        {
                                                            char carriageReturn = '\r';
                                                            int position = (int)carriageReturn;
                                                            resultantText += m_decodedChar[position];
                                                        }
                                                        else
                                                            resultantText += c.ToString();
                                                    }
                                                    else
                                                        resultantText += m_decodedChar[(int)c];
                                                }
                                                else
                                                {
                                                    if (modify)
                                                    {
                                                        resultantText += (char)((int)c + 29);
                                                    }
                                                    else
                                                        resultantText += c.ToString();
                                                }

                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }

                    for (int j = 0; j < m_numberOfChars - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[m_numberOfChars - 1] = c;
                    
                    if (!inText && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inText = true;
                    }
                }
               
                return resultantText;
            }
            catch
            {
                return " ";
            }
        }

        /// <summary>
        /// Extracts text from the bytes.
        /// </summary>
        /// <param name="input">The byte array.</param>
        /// <param name="type">type</param>
        /// <returns>The Extracted Text.</returns>
        internal static string ExtractTextFromBytes(Byte[] data,bool type)
        {
            if (data == null || data.Length == 0)
            {
                return " ";
            }

            try
            {
                string resultantText = string.Empty;
                bool inText = false;
                string prevText = null;
                string currentText = null;
                string nextText = null;
                string text = null;
                bool nextLiteral = false;
                int bracketDepth = 0;
                char[] previousCharacters = new char[m_numberOfChars];

                for (int j = 0; j < m_numberOfChars; j++)
                {
                    previousCharacters[j] = ' ';
                }

                bool even = false;
                string result = string.Empty;
                float prevYPosition = 0f;
                bool InitYPostion = true;
                for (int i = 0; i < data.Length; i++)
                {
                    char c = (char)data[i];
                    if (inText)
                    {
                        if (((int)data[i] == 39) && ((char)data[i - 1] == ')'))
                        {
                            if ((resultantText.Length > 0) && (resultantText[resultantText.Length - 1] != '\n'))
                            {
                                resultantText += Environment.NewLine;
                            }
                        }
                        else if (((char)data[i] == 'T') && ((char)data[i + 1] == 'D') && ((char)data[i - 1] == ' '))
                        {
                            string TDValue = string.Empty;
                            string value = string.Empty;
                            for (int length = previousCharacters.Length - 2; length >= 0; length--)
                            {
                                char space = (char)previousCharacters[length];
                                if (space != ' ')
                                {
                                    value += space;
                                }
                                else
                                {
                                    for (int stlen = value.Length - 1; stlen >= 0; stlen--)
                                    {
                                        TDValue += value[stlen];
                                    }
                                    break;
                                }
                            }
                            if (InitYPostion)
                            {
                                prevYPosition = Convert.ToSingle(TDValue);
                                InitYPostion = false;
                            }
                            else
                            {
                                float Position = Convert.ToSingle(TDValue);
                                float diff = prevYPosition - Position;
                                if ((diff != 0) && (Position != 0))
                                {
                                    resultantText += Environment.NewLine;
                                }
                                prevYPosition = Position;
                            }
                           
                        }
                        else if (((char)data[i] == 'T') && ((char)data[i + 1] == 'd') && ((char)data[i - 1] == ' '))
                        {
                            string TDValue = string.Empty;
                            string value = string.Empty;
                            for (int length = previousCharacters.Length - 2; length >= 0; length--)
                            {
                                char space = (char)previousCharacters[length];
                                if (space != ' ')
                                {
                                    value += space;
                                }
                                else
                                {
                                    for (int stlen = value.Length - 1; stlen >= 0; stlen--)
                                    {
                                        TDValue += value[stlen];
                                    }
                                    break;
                                }
                            }

                            if (InitYPostion)
                            {
                                prevYPosition = Convert.ToSingle(TDValue);
                                InitYPostion = false;
                            }
                            else
                            {
                                float Position = Convert.ToSingle(TDValue);
                                float diff = prevYPosition - Position;
                                if ((diff != 0) && (Position != 0))
                                {
                                    resultantText += Environment.NewLine;
                                }
                                prevYPosition = Position;
                            }
                        }
                        else if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                        {
                            resultantText += Environment.NewLine;
                        }
                        else if (((char)data[i] == 'T') && ((char)data[i + 1] == 'm') && ((char)data[i - 1] == ' ') && ((char)data[i + 2] == '\n'))
                        {
                            resultantText += Environment.NewLine;
                        }

                        if (bracketDepth == 0)
                        {
                            if (((char)data[i] == 'T') && ((char)data[i + 1] == 'm') && ((char)data[i + 2] == '\n'))
                            {
                                if (resultantText.Length > 0 && (resultantText[resultantText.Length - 1] == ' '))
                                {
                                    resultantText = resultantText.Remove(resultantText.Length - 1, 1);
                                }
                            }

                            if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                            {
                                //resultantText += Environment.NewLine;
                            }
                            else if (data[i - 1] == '\n')
                            {
                                if (!resultantText.EndsWith(Environment.NewLine))
                                {
                                    if (prevText == null)
                                    {
                                        prevText = " ";
                                    }
                                    currentText = resultantText;
                                    string temp = resultantText;

                                    if (prevText == " ")
                                    {
                                        prevText = currentText;
                                    }

                                    if (prevText.Length != currentText.Length)
                                    {
                                        try
                                        {
                                            nextText = currentText.Substring(prevText.Length, (currentText.Length - prevText.Length));
                                            nextText = nextText.Trim('\r');
                                            nextText = nextText.Trim('\n');
                                            nextText = nextText.Trim(' ');
                                            if (nextText == text)
                                            {
                                                if (nextText.Length > 0)
                                                {
                                                    resultantText = resultantText.Substring(0, (resultantText.Length - nextText.Length) - 1);
                                                }

                                                if ((resultantText.Length > 0) && (resultantText[resultantText.Length - 1] != '\n'))
                                                {
                                                    resultantText += Environment.NewLine;
                                                }
                                                currentText = resultantText;
                                            }
                                            text = nextText;
                                        }
                                        catch (Exception e)
                                        {
                                            throw e;
                                        }
                                    }
                                    prevText = currentText;
                                }
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                    resultantText += Environment.NewLine;
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        resultantText += string.Empty;
                                    }
                                }

                            }
                        }

                        if (bracketDepth == 0 && CheckToken(new string[] { "ET" }, previousCharacters))
                        {
                            inText = false;
                            resultantText += " ";
                        }
                        else
                        {
                            if ((c == '<') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                if ((c == '>') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                    resultantText += " ";
                                }
                                else
                                {
                                    if (bracketDepth == 1)
                                    {
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~')) ||
                                                ((c >= 128) && (c < 255)))
                                            {
                                             
                                                if (even)
                                                {
                                                    result += c.ToString();
                                                    ulong uiHex2 = 0;
                                                    uiHex2 = Convert.ToUInt64(result.ToString(), 16);
                                                    char ch = Convert.ToChar(uiHex2);
                                                    resultantText += ch.ToString();
                                                    even = false;
                                                    result = string.Empty;
                                                }
                                                else
                                                {
                                                    result += c.ToString();
                                                    even = true;
                                                }

                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (int j = 0; j < m_numberOfChars - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }

                    previousCharacters[m_numberOfChars - 1] = c;

                    if (!inText && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inText = true;
                    }
                }
                return resultantText;
            }
            catch
            {
                return " ";
            }
        }

        /// <summary>
        /// Extracts text from the bytes.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <param name="lpage">The Current Page Object.</param>
        /// <param name="fontname">Font Name Collection.</param>
        /// <param name="fontref">Font Reference Holder Colllection.</param>
        /// <returns>The Extracted Text.</returns>
        public static string ExtractTextFromBytes(Byte[] data, PdfPageBase lpage, List<PdfName> fontname, List<IPdfPrimitive> fontref)
        {
            if (fontname != null)
            {
                string result = null;

                PdfCrossTable crosstable = new PdfCrossTable();
                if (lpage is PdfLoadedPage)
                    crosstable = (lpage as PdfLoadedPage).Document.CrossTable;
                else if (lpage is PdfPageBase)
                    crosstable = (lpage as PdfPage).Document.CrossTable;

                m_differenceArray = new Dictionary<string, List<string>>();

                for (int loop = 0; loop < fontname.Count; loop++)
                {
                    if (fontref[loop] is PdfReferenceHolder)
                    {
                        PdfReferenceHolder font = fontref[loop] as PdfReferenceHolder;
                        PdfDictionary fontDictionary = crosstable.GetObject(font) as PdfDictionary;
                        List<string> m_list = new List<string>();
                        if (fontDictionary[DictionaryProperties.Subtype].ToString() != "/Type3")
                            if (fontDictionary.ContainsKey(DictionaryProperties.Encoding))
                            {
                                PdfReferenceHolder fontencode = fontDictionary[DictionaryProperties.Encoding] as PdfReferenceHolder;
                                PdfDictionary encodingDictionary = crosstable.GetObject(fontencode) as PdfDictionary;

                                if (encodingDictionary != null)
                                    if (encodingDictionary.ContainsKey(DictionaryProperties.Differences))
                                    {
                                        Dictionary<PdfName, IPdfPrimitive> m_encode = encodingDictionary.Items;

                                        foreach (KeyValuePair<PdfName, IPdfPrimitive> item in encodingDictionary.Items)
                                        {
                                            if (item.Key.Value.Equals("Differences"))
                                            {
                                                string latinChar;
                                                PdfArray array = crosstable.GetObject(item.Value) as PdfArray;
                                                int count = array.Count;

                                                for (int i = 0; i < count; i++)
                                                {
                                                    if (array[i] is PdfNumber)
                                                    {
                                                        PdfNumber no = array[i] as PdfNumber;
                                                        latinChar = no.IntValue.ToString();
                                                    }
                                                    else
                                                    {
                                                        latinChar = GetLatinCharacter((array[i]).ToString().Trim('/'));
                                                    }
                                                    m_list.Add(latinChar);
                                                }

                                            }
                                        }
                                        m_differenceArray.Add(fontname[loop].ToString().Trim('/'), m_list);
                                    }
                            }
                    }
                }
                for (int loop = 0; loop < fontname.Count; loop++)
                {
                    if (fontref[loop] is PdfReferenceHolder)
                    {
                        PdfReferenceHolder font = fontref[loop] as PdfReferenceHolder;
                        PdfDictionary fontdic = crosstable.GetObject(font) as PdfDictionary;
                        string fname = string.Empty;
                        if (fontdic.ContainsKey(DictionaryProperties.BaseFont))
                            fname = fontdic[DictionaryProperties.BaseFont].ToString();
                        else if (fontdic.ContainsKey(DictionaryProperties.Name))
                            fname = fontdic[DictionaryProperties.Name].ToString();
                        //Changed ver-89428
                        if (fontdic[DictionaryProperties.Subtype].ToString() == "/Type0")
                        {
                            if (fontdic.ContainsKey(DictionaryProperties.ToUnicode) && (!(fontdic[DictionaryProperties.ToUnicode].ToString() == "/Identity-H")))
                            {
                                result = ExtractTextFromBytesEmbedFonts(data, lpage, fontname, fontref);
                                return result;
                            }
                            result = ExtractTextFrom_Type0Fonts(data,fontref,fontname,crosstable);
                              return result;
                        }
                        //Changed ver-89428 
                        if (!(fontdic.ContainsKey(DictionaryProperties.ToUnicode)) || (fname.Equals("/Times−Roman")) || (fname.Equals("/Times-Bold")) || (fname.Equals("/Times-Italic")) || (fname.Equals("/Times−BoldItalic")) || (fname.Equals("/Helvetica")) || (fname.Equals("/Helvetica−Bold")) || (fname.Equals("/Helvetica−Oblique")) || (fname.Equals("/Helvetica−BoldOblique")) || (fname.Equals("/Courier")) || (fname.Equals("/Courier−Bold")) || (fname.Equals("/Courier−Oblique")) || (fname.Equals("/Courier−BoldOblique")) || (fname.Equals("/Symbol")) || (fname.Equals("/ZapfDingbats")))
                        {
                            result = ExtractTextFromBytes(data);
                            return result;
                        }

                        if (fontdic.ContainsKey(DictionaryProperties.Encoding))
                        {
                            PdfReferenceHolder fontencode = fontdic[DictionaryProperties.Encoding] as PdfReferenceHolder;
                            if (fontencode == null)
                            {
                                string encode = fontdic[DictionaryProperties.Encoding].ToString();
                                if (encode == "/WinAnsiEncoding")
                                {
                                    result = ExtractTextFromBytes(data);
                                    return result;
                                }
                                result = ExtractTextFromBytesTrueTypeFonts(data, fontref, fontname, crosstable);
                                return result;
                            }
                            else
                            {
                                result = ExtractTextFromBytesEmbedFonts(data, lpage, fontname, fontref);
                                return result;
                            }
                        }
                        else if (fontdic.ContainsKey(DictionaryProperties.ToUnicode))
                        {
                            result = ExtractTextFromBytesEmbedFonts(data, lpage, fontname, fontref);
                            return result;
                        }
                    }
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  Extracts text from the bytes(True Type Font Documents).
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The Extracted Text.</returns>
        internal static string ExtractTextFromBytesTrueTypeFonts(Byte[] data,List<IPdfPrimitive> fontref, List<PdfName> fontname,PdfCrossTable crosstable)
        {
            string fontType = string.Empty;

            if (data == null || data.Length == 0)
            {
                return " ";
            }
           
            try
            {
                string resultantText = string.Empty;
                bool inText = false;
                bool nextLiteral = false;
                int bracketDepth = 0;
                bool modify = true;
                string prevTDvalue = "";
                string Encodedtext = Encoding.Default.GetString(data);
                string currentTDvalue="";
                char[] previousCharacters = new char[m_numberOfChars];
                for (int j = 0; j < m_numberOfChars; j++)
                {
                    previousCharacters[j] = ' ';
                }

                for (int i = 0; i < data.Length; i++)
                {
                    char c = (char)data[i];

                    if (CheckToken(new string[] { "Tf" }, previousCharacters))
                    {
                        int startIndex = 0;
                        int stopIndex = 0;
                        for (int charIndex = 0; charIndex < previousCharacters.Length; charIndex++)
                        {
                            if (previousCharacters[charIndex] == '/')
                            {
                                startIndex = charIndex;
                            }
                            else if ((previousCharacters[charIndex] == 'f') && (previousCharacters[charIndex - 1] == 'T'))
                            {
                                stopIndex = charIndex;
                                break;
                            }
                        }

                        string fontName = new string(previousCharacters);
                        string tempFont = fontName.Substring(startIndex + 1, ((stopIndex - 1) - startIndex));
                        int startPosition = tempFont.IndexOf(' ');
                        string m_fontName = tempFont.Substring(0, (startPosition));
                        if (fontname.Contains((PdfName)m_fontName))
                        {
                            PdfReferenceHolder font = fontref[fontname.IndexOf((PdfName)m_fontName)] as PdfReferenceHolder;
                            PdfDictionary fontdic = crosstable.GetObject(font) as PdfDictionary;
                            string fname = fontdic[DictionaryProperties.BaseFont].ToString();

                            if (fontdic[DictionaryProperties.Subtype].ToString() == "/Type0")
                                fontType = "Type0";
                            else if (fontdic[DictionaryProperties.Subtype].ToString() == "/TrueType")
                                fontType = "TrueType";
                            else if (fontdic[DictionaryProperties.Subtype].ToString() == "/Type1")
                                fontType = "Type1";
                        }
                    }

                    
                    if (inText)
                    {
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                            {
                                string Tdtext =new string(previousCharacters);
                                int tdStartpos = Tdtext.IndexOf(' ');
                                int tdEndpos = Tdtext.LastIndexOf(' ');
                                currentTDvalue = Tdtext.Substring(tdStartpos, tdEndpos - tdStartpos);
                                if(currentTDvalue!=prevTDvalue)
                                resultantText += Environment.NewLine;
                                prevTDvalue = currentTDvalue;
                            }
                            else if (data[i - 1] == '\n')
                            {
                                //if (!resultantText.EndsWith(Environment.NewLine))
                                //{
                                  //  resultantText += Environment.NewLine;
                                //}
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                    resultantText += Environment.NewLine;
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        //resultantText += " ";
                                        resultantText += string.Empty;
                                    }
                                }
                            }
                        }

                        if (bracketDepth == 0 && CheckToken(new string[] { "ET" }, previousCharacters))
                        {
                            inText = false;
                            resultantText += " ";
                        }
                        else
                        {
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                
                                bracketDepth = 1;
                            }
                            else
                            {
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    if (bracketDepth == 1)
                                    {
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {

                                            if (fontType == "Type1")
                                            {
                                                resultantText += c.ToString();

                                            }
                                            else if (((c >= 3) && (c <= 97)) ||
                                              ((c >= 99) && (c < 226)))
                                            {
                                                int tempi = (int)c;
                                                tempi = tempi + 29;
                                                char temp = (char)tempi;
                                                c = temp;
                                                resultantText += c.ToString();
                                            }
                                            else
                                            {
                                                char c1 = previousCharacters[m_numberOfChars - 1];
                                                if (c1 == '\0')
                                                {
                                                    resultantText += " ";
                                               }
                                            }

                                            nextLiteral = false;
                                       }
                                    }
                                }
                            }
                        }
                    }

                    for (int j = 0; j < m_numberOfChars - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }

                    previousCharacters[m_numberOfChars - 1] = c;

                    if (!inText && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inText = true;
                    
                    }
                }

                return resultantText;
            }
            catch
            {
                return " ";
            }
        }

        /// <summary>
        /// Extracts text from the bytes(Embed Fonts).
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <param name="lpage">The Current Page Object.</param>
        /// <param name="m_font">Font Name Collection.</param>
        /// <param name="m_fref">Font Reference Holder Colllection.</param>
        /// <returns>The Extracted Text.</returns>
        internal static string ExtractTextFromBytesEmbedFonts(Byte[] data, PdfPageBase lpage, List<PdfName> m_font, List<IPdfPrimitive> m_fref)
        {

            if (m_font != null)
            {

                bool textExtractedFromRow = false;
                List<PdfArray> f_dif = new List<PdfArray>();

                List<List<string>> Tounicode = new List<List<string>>();

                List<Dictionary<double, double>> cMap = new List<Dictionary<double, double>>();
                PdfCrossTable crosstable = null;

                if (lpage is PdfLoadedPage)
                    crosstable = (lpage as PdfLoadedPage).Document.CrossTable;
                else if (lpage is PdfPageBase)
                    crosstable = (lpage as PdfPage).Document.CrossTable;

                for (int loop = 0; loop < m_font.Count; loop++)
                {
                    PdfReferenceHolder font = m_fref[loop] as PdfReferenceHolder;
                    PdfDictionary fontdic = crosstable.GetObject(font) as PdfDictionary;
                    PdfReferenceHolder fontencode = fontdic[DictionaryProperties.Encoding] as PdfReferenceHolder;

                    if (fontencode != null)
                    {
                        PdfDictionary encodedic = crosstable.GetObject(fontencode) as PdfDictionary;
                        PdfArray fontDiff = encodedic[DictionaryProperties.Differences] as PdfArray;
                        f_dif.Add(fontDiff);
                    }

                    PdfReferenceHolder unicode = fontdic[DictionaryProperties.ToUnicode] as PdfReferenceHolder;
                    if (unicode != null)
                    {
                        PdfStream stream = crosstable.GetObject(unicode) as PdfStream;
                        stream.Decompress();
                        byte[] m_unicode = stream.Data;
                        string text = Encoding.UTF8.GetString(m_unicode, 0, m_unicode.Length);
                        int start = text.IndexOf("beginbfchar");
                        int end = text.IndexOf("endbfchar");
                        if (start < 0 && end < 0)
                        {
                            start = text.IndexOf("begincmap");
                            end = text.IndexOf("endcmap");
                        }
                        int bfrangestart = text.IndexOf("beginbfrange");
                        int bfrangeend = text.IndexOf("endbfrange");
                        if (bfrangestart < 0 && bfrangeend < 0)
                        {
                            bfrangestart = text.IndexOf("begincidrange");
                            bfrangeend = text.IndexOf("endcidrange");
                        }
                        if (bfrangestart > 0)
                        {
                            //string result = ExtractTextFromBytes(data);
                           // return result;
                            start = bfrangestart;
                            end = bfrangeend;
                        }

                        string sub = text.Substring(start + 11, (end - start - 11));

                        string tempstr = sub;

                        List<string> tmp = new List<string>();

                        string m_tmp = sub;
                        int m_start = 0;
                        int m_stop = 0;
                        string m_txt = null;
                        Dictionary<double, double> mapTable = new Dictionary<double, double>();
                        for (int j1 = 0; m_start >= 0; j1++)
                        {
                            m_start = m_tmp.IndexOf('<');
                            m_stop = m_tmp.IndexOf('>');
                            if (m_start >= 0 && m_stop >= 0)
                            {
                                m_txt = m_tmp.Substring(m_start + 1, ((m_stop - 1) - m_start));
                                tmp.Add(m_txt);
                                m_tmp = m_tmp.Substring(m_stop + 1, ((m_tmp.Length - 1) - m_stop));
                            }
                        }
                        bool isMapRangeThree=false ;
                        for (int i = 0; i < tmp.Count; )
                        {
                            if ((i + 2) < tmp.Count)
                            if ((tmp[i] != tmp[i + 1]) && (Int64.Parse(tmp[i], System.Globalization.NumberStyles.HexNumber) == Int64.Parse(tmp[i + 2], System.Globalization.NumberStyles.HexNumber)))
                            {
                                isMapRangeThree = true;
                                break;
                            }
                            i = i + 3;
                        }
                        for (int i = 0; i < tmp.Count; )
                        {
                            if (!isMapRangeThree)
                            {
                                if ((tmp[i] != tmp[i + 1]))
                                {
                                    mapTable.Add(Int64.Parse(tmp[i], System.Globalization.NumberStyles.HexNumber), Int64.Parse(tmp[i + 1], System.Globalization.NumberStyles.HexNumber));
                                    i = i + 2;
                                }
                                else
                                {
                                    if ((i + 2) < tmp.Count)
                                    {
                                        mapTable.Add(Int64.Parse(tmp[i], System.Globalization.NumberStyles.HexNumber), Int64.Parse(tmp[i + 2], System.Globalization.NumberStyles.HexNumber));
                                        i = i + 3;
                                    }
                                    else
                                    {
                                        mapTable.Add(Int64.Parse(tmp[i], System.Globalization.NumberStyles.HexNumber), Int64.Parse(tmp[i + 1], System.Globalization.NumberStyles.HexNumber));
                                        i = i + 2;
                                    }
                                }
                            }
                            else
                            {
                                if ((i + 2) < tmp.Count)
                                {
                                    mapTable.Add(Int64.Parse(tmp[i], System.Globalization.NumberStyles.HexNumber), Int64.Parse(tmp[i + 2], System.Globalization.NumberStyles.HexNumber));
                                    i = i + 3;
                                }
                            }
                        }
                        cMap.Add(mapTable);
                        Tounicode.Add(tmp);
                    }
                }

                if (data == null || data.Length == 0)
                {
                    return " ";
                }
                string resultantText = string.Empty;
                try
                {

                    bool inText = false;
                    bool nextLiteral = false;
                    int bracketDepth = 0;
                    char[] previousCharacters = new char[m_numberOfChars];

                    for (int j = 0; j < m_numberOfChars; j++)
                    {
                        previousCharacters[j] = ' ';
                    }

                    //(text>)Tj.
                    bool type1 = false;
                    //<t>Tj <e>Tj <x>Tj <t> Tj.
                    bool type2 = false;
                    string m_fontname = null;
                    string resultChar = string.Empty;
                    string fontEncoding = string.Empty;
                    int fontIndex = 0;
                    List<Byte> encodedBytes = new List<byte>();
                    List<string> m_fontnames = new List<string>();
                    List<string> m_fontName = new List<string>();
                    bool myFlag = false;
                    int count;
                    for (int i = 0; i < data.Length; i++)
                    {
                        char c = (char)data[i];
                        
                        int charInt = (int)c;

                        if (CheckToken(new string[] { "Tf" }, previousCharacters))
                        {
                            int startIndex = 0;
                            int stopIndex = 0;
                            for (int charIndex = 0; charIndex < previousCharacters.Length; charIndex++)
                            {
                                if (previousCharacters[charIndex] == '/')
                                {
                                    startIndex = charIndex;
                                }
                                else if ((previousCharacters[charIndex] == 'T') && (previousCharacters[charIndex + 1] == 'f'))
                                {
                                    stopIndex = charIndex;
                                    //  break;
                                }
                            }

                            string fontName = new string(previousCharacters);
                            string tempName = fontName.Substring(startIndex + 1, ((stopIndex - 1) - startIndex));
                            int sp = tempName.IndexOf(' ');
                            m_fontname = tempName.Substring(0, (sp));
                            m_fontnames.Add(m_fontname);
                        }
                        if (CheckToken(new string[] { "'", "T*", "Tj", "Td","\n" }, previousCharacters))
                        {
                            if (encodedBytes.Count > 0 && encodedBytes.Contains(41))
                            {
                                string decodedText = string.Empty;
                                encodedBytes.RemoveRange(encodedBytes.LastIndexOf(41), (encodedBytes.Count - encodedBytes.LastIndexOf(41)));
                                string s = Encoding.BigEndianUnicode.GetString(encodedBytes.ToArray());
                                string decodedString = Encoding.ASCII.GetString(encodedBytes.ToArray());
                                int noOfChar = 0;
                                foreach (char mappingChar in s)
                                {
                                    if (cMap.Count > fontIndex)
                                    {
                                        if (cMap[fontIndex].ContainsKey((int)mappingChar))
                                        {
                                            noOfChar++;
                                            string ch = ((char)cMap[fontIndex][(int)mappingChar]).ToString();
                                            decodedText += ch;
                                        }
                                    }
                                }  
                                encodedBytes.Clear();
                                if (noOfChar != s.Length)
                                {
                                    decodedText = "";
                                    foreach (char mappingChar in decodedString)
                                    {
                                        if (cMap.Count > fontIndex)
                                        {
                                            if (cMap[fontIndex].ContainsKey((int)mappingChar))
                                            {
                                                string decodedChar = ((char)cMap[fontIndex][(int)mappingChar]).ToString();
                                                decodedText += decodedChar;
                                            }
                                        }
                                    }
                                }
                                resultantText += decodedText;                                
                            }
                            else
                                encodedBytes.Clear();
                            bracketDepth = 0;
                            textExtractedFromRow = false;
                            
                        }

                        if (inText)
                        {
                            if (bracketDepth == 0)
                            {
                                if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                                {
                                    // resultantText += Environment.NewLine;
                                    if (type1)
                                    {
                                        resultantText += string.Empty ;
                                    }
                                }
                                else if (CheckToken(new string[] { "Tf" }, previousCharacters))
                                {
                                    int startIndex = 0;
                                    int stopIndex = 0;
                                    for (int charIndex = 0; charIndex < previousCharacters.Length; charIndex++)
                                    {
                                        if (previousCharacters[charIndex] == '/')
                                        {
                                            startIndex = charIndex;
                                        }
                                        else if ((previousCharacters[charIndex] == 'T') && (previousCharacters[charIndex + 1] == 'f'))
                                        {
                                            stopIndex = charIndex;
                                            //  break;
                                        }
                                    }

                                    string fontname1 = new string(previousCharacters);
                                    string m_font_temp = fontname1.Substring(startIndex + 1, ((stopIndex - 1) - startIndex));
                                    int sp = m_font_temp.IndexOf(' ');
                                    m_fontname = m_font_temp.Substring(0, (sp));
                                    m_fontnames.Add(m_fontname);
                                }
                                else if (data[i - 1] == '\n')
                                {
                                    //if (!resultantText.EndsWith(Environment.NewLine))
                                    //    resultantText += Environment.NewLine;
                                }
                                
                            }

                            if (bracketDepth == 0 && CheckToken(new string[] { "ET" }, previousCharacters))
                            {
                                inText = false;
                                resultantText += " ";
                            }
                            else
                            {                               
                                if ((c == '<') && (bracketDepth == 0) && (!nextLiteral))
                                {
                                    if (!type1)
                                    {
                                        bracketDepth = 1;
                                        for (int j = i; j < data.Length - 1; j++)
                                        {
                                            char ch = (char)data[j];
                                            if (ch == '>')
                                            {
                                                byte[] hexEncode = new byte[j - i - 1];
                                                int counter = 0;
                                                for (int k = i + 1; k < j; k++)
                                                {
                                                    hexEncode[counter] = data[k];
                                                    counter++;
                                                }
                                                UTF8Encoding utf8 = new UTF8Encoding();
                                                count = 0;
                                                int cnt = j - i - 1;
                                                byte[] temp = new byte[cnt];
                                                if (cnt > 3)
                                                {
                                                    for (int a = 0; a < cnt; a++)
                                                    {
                                                        temp[count] = hexEncode[a];
                                                        String decodedString = utf8.GetString(temp);
                                                        Int64 tmp;
                                                        int ct = 0;
                                                        
                                                        
                                                            if (myFlag&&count==cnt-1)
                                                            {
                                                                char[] myChar = decodedString.ToCharArray();
                                                                for (int y = 0; y < cnt/2; y++)
                                                                {
                                                                    string myStr = null;
                                                                    for (int x = 0; x < 2; x++)
                                                                    {
                                                                        myStr += myChar[ct];
                                                                        ct++;
                                                                    }
                                                                    int index = Tounicode[0].IndexOf(myStr);
                                                                    index++;
                                                                    string STmp = Tounicode[0][index];
                                                                    tmp = Int64.Parse(STmp, System.Globalization.NumberStyles.HexNumber);
                                                                    char tmp1 = (char)tmp;
                                                                    resultantText += tmp1;
                                                                }
                                                                count = 0;
                                                            }
                                                            else if (count==3&&!myFlag)
                                                            {
                                                                tmp = Int64.Parse(decodedString, System.Globalization.NumberStyles.HexNumber);
                                                                char tmp1 = (char)tmp;
                                                                resultantText += tmp1;
                                                                count = 0;
                                                            }

                                                        
                                                        
                                                        else
                                                            count++;
                                                    }
                                                }
                                                else
                                                {
                                                    myFlag = true;
                                                    int b;
                                                    for (b = 0; b < 2; b++) 
                                                    {
                                                        temp[count] = hexEncode[b];
                                                        if (count == 1)
                                                        {
                                                            String decodedString = utf8.GetString(temp);                                                            
                                                            char[] myChar = decodedString.ToCharArray();
                                                            string myStr = null;
                                                            myStr += myChar[0];
                                                            myStr += myChar[1];
                                                            int index = Tounicode[0].IndexOf(myStr);
                                                            index++;
                                                            string STmp = Tounicode[0][index];
                                                            Int64 tmp = Int64.Parse(STmp, System.Globalization.NumberStyles.HexNumber);
                                                            char tmp1 = (char)tmp;
                                                            resultantText += tmp1;
                                                            count = 0;
                                                        }
                                                        else
                                                            count++;
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                                    {
                                        bracketDepth = 1;
                                        type1 = true;
                                    }
                                    else
                                    {
                                        if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                        {
                                            resultantText += Environment.NewLine;
                                            textExtractedFromRow = false;
                                        }
                                        else if (CheckToken(new string[] { "T*" }, previousCharacters))
                                        {
                                            resultantText += Environment.NewLine;
                                            textExtractedFromRow = false;
                                        }
                                        else
                                        {
                                            if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                            {
                                                bracketDepth = 0;
                                               // resultantText = resultantText.Substring(0, resultantText.Length - 5);
                                                textExtractedFromRow = false;
                                            }
                                        }
                                    }
                                    if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                    {
                                        if (encodedBytes.Count <= 0)
                                        {
                                            bracketDepth = 0;
                                            textExtractedFromRow = false;
                                        }
                                    }
                                    if ((c == '>') && (bracketDepth == 1) && (!nextLiteral))
                                    {
                                        if (!type1)
                                        {
                                            bracketDepth = 0;
                                            resultChar = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        if (bracketDepth == 1)
                                        {
                                            if (c == '\\' && !nextLiteral)
                                            {
                                                nextLiteral = true;
                                            }
                                            else
                                            {
                                                if (((c >= ' ') && (c <= '~')) ||
                                                    ((c >= 128) && (c < 255)))
                                                {
                                                    string str = null;
                                                    string textfont = "/" + m_fontname;
                                                    if (m_fontName.Count >= 1)
                                                    {
                                                        string f_test = m_fontName[(m_fontName.Count) - 1].ToString();
                                                        if (f_test != textfont)
                                                        {
                                                            m_fontName.Add(textfont);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        m_fontName.Add(textfont);
                                                    }


                                                    for (int k = 0; k < m_font.Count; k++)
                                                    {
                                                        str = m_font[k].ToString();
                                                        if (textfont.Equals(str))
                                                        {
                                                            fontIndex = k;
                                                            break;
                                                        }
                                                    }
                                                    fontEncoding = ((m_fref[fontIndex] as PdfReferenceHolder).Object as PdfDictionary)[DictionaryProperties.Subtype].ToString();
                                                    PdfDictionary toUnicode = (m_fref[fontIndex] as PdfReferenceHolder).Object as PdfDictionary;
                                                    if (toUnicode.ContainsKey(DictionaryProperties.ToUnicode))
                                                    {
                                                        if (Tounicode.Count > fontIndex)
                                                        {
                                                            List<string> fontArray = Tounicode[fontIndex] as List<string>;

                                                            char[] charcode = new char[fontArray.Count];
                                                            char[] unicode = new char[fontArray.Count];

                                                            if (!type1)
                                                            {

                                                                for (int index = 0, position = 0; index < fontArray.Count; )
                                                                {
                                                                    ulong hexValue = 0;
                                                                    hexValue = Convert.ToUInt64(fontArray[index].ToString(), 16);
                                                                    string codeString = hexValue.ToString();
                                                                    int charvalue = Convert.ToInt32(codeString);
                                                                    char cvalue = (char)charvalue;
                                                                    charcode[position] = cvalue;
                                                                    position++;
                                                                    index = index + 2;
                                                                }

                                                                for (int index = 1, position = 0; index < fontArray.Count; )
                                                                {
                                                                    ulong uiHex2 = 0;
                                                                    uiHex2 = Convert.ToUInt64(fontArray[index].ToString(), 16);
                                                                    string cstr = uiHex2.ToString();
                                                                    int value = Convert.ToInt32(cstr);
                                                                    char tmpchar = (char)value;
                                                                    unicode[position] = tmpchar;
                                                                    position++;
                                                                    index = index + 2;
                                                                }
                                                            }

                                                            if (resultChar.Length == 0)
                                                            {
                                                                if (cMap[fontIndex].ContainsKey((int)c))
                                                                {
                                                                    string ch = ((char)cMap[fontIndex][(int)c]).ToString();
                                                                    resultChar += ch;
                                                                }
                                                            }
                                                            else if (resultChar.Length == 1)
                                                            {
                                                                if (!type1)
                                                                {
                                                                    resultChar += c.ToString();
                                                                    ulong uniCodeHexValue = 0;
                                                                    uniCodeHexValue = Convert.ToUInt64(resultChar, 16);
                                                                    string codeString = uniCodeHexValue.ToString();
                                                                    int charValue = Convert.ToInt32(codeString);
                                                                    char cvalue = (char)charValue;
                                                                    c = cvalue;
                                                                    resultChar = string.Empty;
                                                                    for (int m = 0; m < charcode.Length; m++)
                                                                    {
                                                                        if (c == charcode[m])
                                                                        {
                                                                            c = unicode[m];
                                                                            if (c != '\0')
                                                                            {
                                                                                resultantText += c.ToString();
                                                                            }
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }
                                                    if (type1)
                                                    {

                                                        if (textExtractedFromRow)
                                                        {
                                                            if (fontEncoding == "/Type0")
                                                            {
                                                                encodedBytes.Add((Byte)c);
                                                            }
                                                            else
                                                            {
                                                                if (toUnicode.ContainsKey(DictionaryProperties.ToUnicode))
                                                                {
                                                                    if (cMap.Count > fontIndex)
                                                                    {
                                                                        if (cMap[fontIndex].ContainsKey((int)c))
                                                                        {
                                                                            string ch = ((char)cMap[fontIndex][(int)c]).ToString();
                                                                            resultantText += ch;
                                                                        }
                                                                        else
                                                                        {
                                                                            resultantText += c.ToString();
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        resultantText += c.ToString();
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    resultantText += c.ToString();
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            textExtractedFromRow = true;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    string str = null;
                                                    string textfont = "/" + m_fontname;
                                                    if (m_fontName.Count >= 1)
                                                    {
                                                        string f_test = m_fontName[(m_fontName.Count) - 1].ToString();
                                                        if (f_test != textfont)
                                                        {
                                                            m_fontName.Add(textfont);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        m_fontName.Add(textfont);
                                                    }

                                                    for (int k = 0; k < m_font.Count; k++)
                                                    {
                                                        str = m_font[k].ToString();
                                                        if (textfont.Equals(str))
                                                        {
                                                            fontIndex = k;
                                                            break;
                                                        }
                                                    }
                                                    fontEncoding = ((m_fref[fontIndex] as PdfReferenceHolder).Object as PdfDictionary)[DictionaryProperties.Subtype].ToString();
                                                    if (Tounicode.Count > fontIndex)
                                                    {
                                                        List<string> fontArray = Tounicode[fontIndex] as List<string>;

                                                        char[] charcode = new char[fontArray.Count];
                                                        char[] unicode = new char[fontArray.Count];

                                                        for (int index = 0, position = 0; index < fontArray.Count; )
                                                        {

                                                            ulong hexValue = 0;
                                                            hexValue = Convert.ToUInt64(fontArray[index].ToString(), 16);
                                                            string codeString = hexValue.ToString();
                                                            int charvalue = Convert.ToInt32(codeString);
                                                            char cvalue = (char)charvalue;
                                                            charcode[position] = cvalue;
                                                            position++;
                                                            index = index + 2;
                                                        }

                                                        for (int index = 0, position = 0; index < fontArray.Count; )
                                                        {
                                                            //Changed ver-89428 
                                                            while (((fontArray[index].ToString()).Length) < 4)
                                                                index++;
                                                            //Changed ver-89428 
                                                            ulong uiHex2 = 0;
                                                            uiHex2 = Convert.ToUInt64(fontArray[index].ToString(), 16);
                                                            string cstr = uiHex2.ToString();
                                                            int value = Convert.ToInt32(cstr);
                                                            char tmpchar = (char)value;
                                                            if ((index > 2) && (fontArray[index - 1] != fontArray[index - 2]))
                                                            {
                                                                unicode[position] = tmpchar;
                                                                unicode[position + 1] = (char)(tmpchar + 1);
                                                                position = position + 2;
                                                            }
                                                            else
                                                            {
                                                                unicode[position] = tmpchar;
                                                                position++;
                                                            }

                                                            index = index + 2;
                                                        }

                                                        if (resultChar.Length == 0)
                                                        {
                                                            resultChar += c.ToString();
                                                        }
                                                        else if (resultChar.Length == 1)
                                                        {
                                                            if (!type1)
                                                            {
                                                                resultChar += c.ToString();
                                                                ulong uniCodeHexValue = 0;
                                                                uniCodeHexValue = Convert.ToUInt64(resultChar, 16);
                                                                string codeString = uniCodeHexValue.ToString();
                                                                int charValue = Convert.ToInt32(codeString);
                                                                char cvalue = (char)charValue;
                                                                c = cvalue;
                                                                resultChar = string.Empty;
                                                                for (int m = 0; m < charcode.Length; m++)
                                                                {
                                                                    if (c == charcode[m])
                                                                    {
                                                                        c = unicode[m];
                                                                        if (c != '\0')
                                                                        {
                                                                            resultantText += c.ToString();
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (type1)
                                                        {
                                                            /*Changed ver-89428 
                                                            for (int m = 0; m < charcode.Length; m++)
                                                            {
                                                                if (c == charcode[m])
                                                                {
                                                                    char t_c = c;
                                                                    c = unicode[m];
                                                                    resultantText += c.ToString();
                                                                    break;
                                                                }
                                                            }Changed ver-89428 */

                                                            //Changed ver-89428
                                                            if (fontEncoding == "/Type0")
                                                            {
                                                                encodedBytes.Add((Byte)c);
                                                            }
                                                            else if (((int)c) < unicode.Length)
                                                            {
                                                                char t_c = unicode[((int)c) - 1];
                                                                resultantText += t_c.ToString();

                                                            }
                                                            //Changed ver-89428 

                                                        }
                                                    }
                                                }
                                            }
                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }

                        for (int j = 0; j < m_numberOfChars - 1; j++)
                        {
                            previousCharacters[j] = previousCharacters[j + 1];
                        }

                        previousCharacters[m_numberOfChars - 1] = c;

                        if (!inText && CheckToken(new string[] { "BT" }, previousCharacters))
                        {
                            inText = true;
                        }
                    }

                    return resultantText;
                }
                catch
                {
                    return " ";
                }
            }
            else
            {
                return " ";
            }
        }
        //Changed ver-89428 
        ///<summarry>
        /// Extracts text from Type0
        /// </summarry>
        /// <param name ="data">The byte array.</param>
        /// <returns>The Extracted Text.</returns>
        internal static string ExtractTextFrom_Type0Fonts(Byte[] data, List<IPdfPrimitive> fontref, List<PdfName> fontname,PdfCrossTable crosstable)
        {
            string fontType = string.Empty;

            if (data == null || data.Length == 0)
            {
                return " ";
            }

            try
            {
                string resultantText = string.Empty;
                bool inText = false;
                bool nextLiteral = false;
                int bracketDepth = 0;
                bool tjval = false;
                bool modify = false;
                char[] previousCharacters = new char[m_numberOfChars];

                for (int j = 0; j < m_numberOfChars; j++)
                {
                    previousCharacters[j] = ' ';
                }

                for (int i = 0; i < data.Length; i++)
                {
                    char c = (char)data[i];
                   
                    if (CheckToken(new string[] { "Tf" }, previousCharacters))
                    {
                        int startIndex = 0;
                        int stopIndex = 0;
                        for (int charIndex = 0; charIndex < previousCharacters.Length; charIndex++)
                        {
                            if (previousCharacters[charIndex] == '/')
                            {
                                startIndex = charIndex;
                            }
                            else if ((charIndex > 0) &&(previousCharacters[charIndex] == 'f') && (previousCharacters[charIndex - 1] == 'T'))
                            {
                                stopIndex = charIndex;
                                break;
                            }
                        }

                        string fontname1 = new string(previousCharacters);
                        string m_font_temp = fontname1.Substring(startIndex + 1, ((stopIndex - 1) - startIndex));
                        int sp = m_font_temp.IndexOf(' ');
                        string m_fontname = m_font_temp.Substring(0, (sp));
                        if (fontname.Contains((PdfName)m_fontname))
                        {
                            PdfReferenceHolder font = fontref[fontname.IndexOf((PdfName)m_fontname)] as PdfReferenceHolder;
                            PdfDictionary fontdic = crosstable.GetObject(font) as PdfDictionary;
                            string fname = fontdic[DictionaryProperties.BaseFont].ToString();

                            if (fontdic[DictionaryProperties.Subtype].ToString() == "/Type0")
                            {
                                fontType = "Type0";
                                if (fontdic.ContainsKey(DictionaryProperties.Encoding))
                                {
                                    PdfName encoding = fontdic[DictionaryProperties.Encoding] as PdfName;
                                    if (encoding != null && (encoding.Value == "Identity-H" || encoding.Value == "Identity-V"))
                                        modify = true;
                                }
                                if (fontdic.ContainsKey(DictionaryProperties.ToUnicode))
                                {
                                    PdfReferenceHolder unicode = fontdic[DictionaryProperties.ToUnicode] as PdfReferenceHolder;
                                    if (unicode != null && modify)
                                        modify = false;
                                }
                            }
                            else if (fontdic[DictionaryProperties.Subtype].ToString() == "/TrueType")
                                fontType = "TrueType";
                        }

                    }

                    if (inText)
                    {
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new string[] { "TD" }, previousCharacters))
                            {
                                resultantText += Environment.NewLine;
                                tjval = !tjval;
                            }
                            
                            else if (data[i - 1] == '\n' && c == '-')
                            {
                                if (!resultantText.EndsWith(Environment.NewLine))
                                {
                                    resultantText += Environment.NewLine;
                                    tjval = !tjval;
                                }
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                    resultantText += Environment.NewLine;
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        //resultantText += " ";
                                        resultantText += string.Empty;
                                    }
                                }
                            }
                        }

                        if (bracketDepth == 0 && CheckToken(new string[] { "ET" }, previousCharacters))
                        {
                            inText = false;
                            resultantText += " ";
                        }
                        else
                        {
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    if (bracketDepth == 1)
                                    {
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                            char[] temp1 = new char[4];
                                            if (data.Length >= i + 4)
                                            {
                                                Array.Copy(data, i, temp1, 0, 4);
                                                string t = new string(temp1);

                                                if (t == "\\000")
                                                {
                                                    i = i + 3;
                                                    continue;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if(fontType == "TrueType" && (((c >= ' ') && (c <= '~')) ||
                                              ((c >= 128) && (c < 255))))
                                            {
                                                resultantText += c.ToString();
                                            }
                                            else if (fontType == "Type0" && modify)
                                                resultantText += c.ToString();
                                            else  if (fontType == "Type0" &&(((c >= 3) && (c <= 97)) ||
                                         ((c >= 99) && (c < 226))))
                                            {
                                                int tempi = (int)c;
                                                tempi = tempi + 29;
                                                char temp = (char)tempi;
                                                c = temp;
                                                resultantText += c.ToString();
                                            }

                                            else
                                            {
                                                char c1 = previousCharacters[m_numberOfChars - 1];
                                                if (c1 == '\0')
                                                {
                                                    resultantText += " ";
                                                }
                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (int j = 0; j < m_numberOfChars - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }

                    previousCharacters[m_numberOfChars - 1] = c;

                    if (!inText && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inText = true;
                    }
                }

                return resultantText;
            }
            catch
            {
                return " ";
            }
        }
        //Changed ver-89428 

        /// <summary>
        /// Checks the token.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="recent">The recent.</param>
        /// <returns>token</returns>
        private static bool CheckToken(string[] tokens, char[] recent)
        {
            foreach (string token in tokens)
            {
                if (token.Length > 1)
                {
                    if ((recent[m_numberOfChars - 3] == token[0]) &&
                        (recent[m_numberOfChars - 2] == token[1]) &&
                        ((recent[m_numberOfChars - 1] == ' ') ||
                        (recent[m_numberOfChars - 1] == 0x0d) ||
                        (recent[m_numberOfChars - 1] == 0x0a)) &&
                        ((recent[m_numberOfChars - 4] == ' ') ||
                        (recent[m_numberOfChars - 4] == 0x0d) ||
                        (recent[m_numberOfChars - 4] == 0x0a)))
                    {
                        return true;
                    }
                }
                else
                {
                    if ((recent[m_numberOfChars - 3] == token[0]) &&
                        ((recent[m_numberOfChars - 1] == ' ') ||
                        (recent[m_numberOfChars - 1] == 0x0d) ||
                        (recent[m_numberOfChars - 1] == 0x0a)) &&
                        ((recent[m_numberOfChars - 4] == ' ') ||
                        (recent[m_numberOfChars - 4] == 0x0d) ||
                        (recent[m_numberOfChars - 4] == 0x0a))

                        )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  Gets Latin Character
        /// </summary>
        /// <param name="decodedCharacter">The decodedCharacter.</param>        
        /// <returns>decodedCharacter</returns>
        //Latin Character Set (APPENDIX D Pdf version-1.7) Page- 997
        internal static string GetLatinCharacter(string decodedCharacter)
        {
            switch (decodedCharacter)
            {
                case "zero":
                    return "0";
                case "one":
                    return "1";
                case "two":
                    return "2";
                case "three":
                    return "3";
                case "four":
                    return "4";
                case "five":
                    return "5";
                case "six":
                    return "6";
                case "seven":
                    return "7";
                case "eight":
                    return "8";
                case "nine":
                    return "9";
                case "aring":
                    return "å";
                case "asciicircum":
                    return "^";
                case "asciitilde":
                    return "~";
                case "asterisk":
                    return "*";
                case "at":
                    return "@";
                case "atilde":
                    return "ã";
                case "backslash":
                    return "\\";
                case "bar":
                    return "|";
                case "braceleft":
                    return "{";
                case "braceright":
                    return "}";
                case "bracketleft":
                    return "[";
                case "bracketright":
                    return "]";
                case "breve":
                    return "˘";
                case "brokenbar":
                    return "|";
                case "bullet3":
                    return "•";
                case "bullet":
                    return "•";
                case "caron":
                    return "ˇ";
                case "ccedilla":
                    return "ç";
                case "cedilla":
                    return "¸";
                case "cent":
                    return "¢";
                case "circumflex":
                    return "ˆ";
                case "colon":
                    return ":";
                case "comma":
                    return ",";
                case "copyright":
                    return "©";
                case "currency1":
                    return "¤";
                case "dagger":
                    return "†";
                case "daggerdbl":
                    return "‡";
                case "degree":
                    return "°";
                case "dieresis":
                    return "¨";
                case "divide":
                    return "÷";
                case "dollar":
                    return "$";
                case "dotaccent":
                    return "˙";
                case "dotlessi":
                    return "ı";
                case "eacute":
                    return "é";
                case "ecircumflex":
                    return "˙";
                case "edieresis":
                    return "ë";
                case "egrave":
                    return "è";
                case "ellipsis":
                    return "...";
                case "emdash":
                    return "——";
                case "endash":
                    return "–";
                case "equal":
                    return "=";
                case "eth":
                    return "ð";
                case "exclam":
                    return "!";
                case "exclamdown":
                    return "¡";
                case "fi":
                    return "fl";
                case "florin":
                    return "ƒ";
                case "fraction":
                    return "⁄";
                case "germandbls":
                    return "ß";
                case "grave":
                    return "`";
                case "greater":
                    return ">";
                case "guillemotleft4":
                    return "«";
                case "guillemotright4":
                    return "»";
                case "guilsinglleft":
                    return "‹";
                case "guilsinglright":
                    return "›";
                case "hungarumlaut":
                    return "˝";
                case "hyphen5":
                    return "-";
                case "iacute":
                    return "í";
                case "icircumflex":
                    return "î";
                case "idieresis":
                    return "ï";
                case "igrave":
                    return "ì";
                case "less":
                    return "<";
                case "logicalnot":
                    return "¬";
                case "lslash":
                    return "ł";
                case "macron":
                    return "¯";
                case "minus":
                    return "−";
                case "mu":
                    return "μ";
                case "multiply":
                    return "×";
                case "ntilde":
                    return "ñ";
                case "numbersign":
                    return "#";
                case "oacute":
                    return "ó";
                case "ocircumflex":
                    return "ô";
                case "odieresis":
                    return "ö";
                case "oe":
                    return "oe";
                case "ogonek":
                    return "˛";
                case "ograve":
                    return "ò";
                case "onehalf":
                    return "1/2";
                case "onequarter":
                    return "1/4";
                case "onesuperior":
                    return "¹";
                case "ordfeminine":
                    return "ª";
                case "ordmasculine":
                    return "º";
                case "oslash":
                    return "ø";
                case "otilde":
                    return "õ";
                case "paragraph":
                    return "¶";
                case "parenleft":
                    return "(";
                case "parenright":
                    return ")";
                case "percent":
                    return "%";
                case "period":
                    return ".";
                case "periodcentered":
                    return "·";
                case "perthousand":
                    return "‰";
                case "plus":
                    return "+";
                case "plusminus":
                    return "±";
                case "question":
                    return "?";
                case "questiondown":
                    return "¿";
                case "quotedbl":
                    return "\"";
                case "quotedblbase":
                    return "„";
                case "quotedblleft":
                    return "“";
                case "quotedblright":
                    return "”";
                case "quoteleft":
                    return "‘";
                case "quoteright":
                    return "’";
                case "quotesinglbase":
                    return "‚";
                case "quotesingle":
                    return "'";
                case "registered":
                    return "®";
                case "ring":
                    return "˚";
                case "scaron":
                    return "š";
                case "section":
                    return "§";
                case "semicolon":
                    return ";";
                case "slash":
                    return "/";
                case "space6":
                    return " ";
                case "space":
                    return " ";
                case "udieresis":
                    return "ü";
                case "hyphen":
                    return "-";
                case "underscore":
                    return "_";
                case "adieresis":
                    return "ä";
                case "ampersand":
                    return "&";
                case "Adieresis":
                    return "Ä";
                case "Udieresis":
                    return "Ü";
                case "ccaron":
                    return "č";
                case "Scaron":
                    return "Š";
                case "zcaron":
                    return "ž";
                default:
                    return decodedCharacter;

            }
        }

        /// <summary>
        ///  Gets Latin Character
        /// </summary>
        /// <param name="decodedCharacter">The decodedCharacter.</param>        
        /// <returns>decodedCharacter</returns>
        //Latin Character Set (APPENDIX D Pdf version-1.7) Page- 997
        internal static string GetSpecialCharacter(string decodedCharacter)
        {            
            switch (decodedCharacter)
            { 
                case "head2right":
                    return "\u27A2";
                case "aacute":
                    return "a\u0301";
                case "eacute":
                    return "e\u0301";
                case "iacute":
                    return "i\u0301";
                case "oacute":
                    return "o\u0301";
                case "uacute":
                    return "u\u0301";
                case "circleright":
                    return "\u27B2";
                case "bleft":
                    return "\u21E6";
                case "bright":
                    return "\u21E8";
                case "bup":
                    return "\u21E7";
                case "bdown":
                    return "\u21E9";
                case "barb4right":
                    return "\u2794";
                case "bleftright":
                    return "\u2B04";
                case "bupdown":
                    return "\u21F3";
                case "bnw":
                    return "\u2B00";
                case "bne":
                    return "\u2B01";
                case "bsw":
                    return "\u2B03";
                case "bse":
                    return "\u2B02";
                case "bdash1":
                    return "\u25AD";
                case "bdash2":
                    return "\u25AB";
                case "xmarkbld":
                    return "\u2717";
                case "checkbld":
                    return "\u2713";
                case "boxxmarkbld":
                    return "\u2612";
                case "boxcheckbld":
                    return "\u2611";
                case "space":
                    return "\u0020";
                case "pencil":
                    return "\u270F";
                case "scissors":
                    return "\u2702";
                case "scissorscutting":
                    return "\u2701";
                case "readingglasses":
                    return "\u2701";
                case "bell":
                    return "\u2701";
                case "book":
                    return "\u2701";
                case "telephonesolid":
                    return "\u2701";
                case "telhandsetcirc":
                    return "\u2701";
                case "envelopeback":
                    return "\u2701";
                case "hourglass":
                    return "\u231B";
                case "keyboard":
                    return "\u2328";
                case "tapereel":
                    return "\u2707";
                case "handwrite":
                    return "\u270D";
                case "handv":
                    return "\u270C";
                case "handptleft":
                    return "\u261C";
                case "handptright":
                    return "\u261E";
                case "handptup":
                    return "\u261D";
                case "handptdown":
                    return "\u261F";
                case "smileface":
                    return "\u263A";
                case "frownface":
                    return "\u2639";
                case "skullcrossbones":
                    return "\u2620";
                case "flag":
                    return "\u2690";
                case "pennant":
                    return "\u1F6A9";
                case "airplane":
                    return "\u2708";
                case "sunshine":
                    return "\u263C";
                case "droplet":
                    return "\u1F4A7";
                case "snowflake":
                    return "\u2744";
                case "crossshadow":
                    return "\u271E";
                case "crossmaltese":
                    return "\u2720";
                case "starofdavid":
                    return "\u2721";
                case "crescentstar":
                    return "\u262A";
                case "yinyang":
                    return "\u262F";
                case "om":
                    return "\u0950";
                case "wheel":
                    return "\u2638";
                case "aries":
                    return "\u2648";
                case "taurus":
                    return "\u2649";
                case "gemini":
                    return "\u264A";
                case "cancer":
                    return "\u264B";
                case "leo":
                    return "\u264C";
                case "virgo":
                    return "\u264D";
                case "libra":
                    return "\u264E";
                case "scorpio":
                    return "\u264F";
                case "saggitarius":
                    return "\u2650";
                case "capricorn":
                    return "\u2651";
                case "aquarius":
                    return "\u2652";
                case "pisces":
                    return "\u2653";
                case "ampersanditlc":
                    return "\u0026";
                case "ampersandit":
                    return "\u0026";
                case "circle6":
                    return "\u25CF";
                case "circleshadowdwn":
                    return "\u274D";
                case "square6":
                    return "\u25A0";
                case "box3":
                    return "\u25A1";
                case "boxshadowdwn":
                    return "\u2751";
                case "boxshadowup":
                    return "\u2752";
                case "lozenge4":
                    return "\u2B27";
                case "lozenge6":
                    return "\u29EB";
                case "rhombus6":
                    return "\u25C6";
                case "xrhombus":
                    return "\u2756";
                case "rhombus4":
                    return "\u2B25";
                case "clear":
                    return "\u2327";
                case "escape":
                    return "\u2353";
                case "command":
                    return "\u2318";
                case "rosette":
                    return "\u2740";
                case "rosettesolid":
                    return "\u273F";
                case "quotedbllftbld":
                    return "\u275D";
                case "quotedblrtbld":
                    return "\u275E";
                case ".notdef":
                    return "\u25AF";
                case "zerosans":
                    return "\u24EA";
                case "onesans":
                    return "\u2460";
                case "twosans":
                    return "\u2461";
                case "threesans":
                    return "\u2462";
                case "foursans":
                    return "\u2463";
                case "fivesans":
                    return "\u2464";
                case "sixsans":
                    return "\u2465";
                case "sevensans":
                    return "\u2466";
                case "eightsans":
                    return "\u2467";
                case "ninesans":
                    return "\u2468";
                case "tensans":
                    return "\u2469";
                case "zerosansinv":
                    return "\u24FF";
                case "onesansinv":
                    return "\u2776";
                case "twosansinv":
                    return "\u2777";
                case "threesansinv":
                    return "\u2778";
                case "foursansinv":
                    return "\u2779";
                case "circle2":
                    return "\u00B7";
                case "circle4":
                    return "\u2022";
                case "square2":
                    return "\u25AA";
                case "ring2":
                    return "\u25CB";
                case "ringbutton2":
                    return "\u25C9";
                case "target":
                    return "\u25CE";
                case "square4":
                    return "\u25AA";
                case "box2":
                    return "\u25FB";
                case "crosstar2":
                    return "\u2726";
                case "pentastar2":
                    return "\u2605";
                case "hexstar2":
                    return "\u2736";
                case "octastar2":
                    return "\u2734";
                case "dodecastar3":
                    return "\u2739";
                case "octastar4":
                    return "\u2735";
                case "registercircle":
                    return "\u2316";
                case "cuspopen":
                    return "\u27E1";
                case "cuspopen1":
                    return "\u2311";
                case "circlestar":
                    return "\u2605";
                case "starshadow":
                    return "\u2730";
                case "deleteleft":
                    return "\u232B";
                case "deleteright":
                    return "\u2326";
                case "scissorsoutline":
                    return "\u2704";
                case "telephone":
                    return "\u260F";
                case "telhandset":
                    return "\u1F4DE";
                case "handptlft1":
                    return "\u261C";
                case "handptrt1":
                    return "\u261E";
                case "handptlftsld1":
                    return "\u261A";
                case "handptrtsld1":
                    return "\u261B";
                case "handptup1":
                    return "\u261D";
                case "handptdwn1":
                    return "\u261F";
                case "xmark":
                    return "\u2717";
                case "check":
                    return "\u2713";
                case "boxcheck":
                    return "\u2611";
                case "boxx":
                    return "\u2612";
                case "boxxbld":
                    return "\u2612";
                case "circlex":
                    return "=\u2314";
                case "circlexbld":
                    return "\u2314";
                case "prohibit":
                case "prohibitbld":
                    return "\u29B8";
                case "ampersanditaldm":
                case "ampersandbld":
                case "ampersandsans":
                case "ampersandsandm":
                    return "\u0026";
                case "interrobang":
                case "interrobangdm":
                case "interrobangsans":
                case "interrobngsandm":
                    return "\u203D";
                case "park":
                    return "\uE0E0";
                case "g120":
                    return "\u00B7";
                case "g383":
                case "g45":
                    return "\u263A";
                default:
                    return decodedCharacter;
            }

        }
        #endregion
    }
}

#endif