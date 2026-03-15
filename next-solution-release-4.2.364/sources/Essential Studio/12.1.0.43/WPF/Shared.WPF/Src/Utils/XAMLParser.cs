// <copyright file="XAMLParser.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// XAML parse which help us to format and display XAML text in rich format.
    /// </summary>
    public class XAMLParser
    {
        /// <summary>
        /// Colors the XAML.
        /// </summary>
        /// <param name="xamlText">The xaml text.</param>
        /// <returns>return formatted paragraph text.</returns>
        public static Paragraph GetFomattedXAML(string xamlText)
        {
            xamlText = Regex.Replace(xamlText, "[ ]*=[ ]*", "=", RegexOptions.Multiline);
            XamlTokenizer tokenizer = new XamlTokenizer();
            XamlTokenizerMode mode = XamlTokenizerMode.OutsideElement;
            List<XamlToken> tokens = tokenizer.Tokenize(xamlText, ref mode);
            List<string> tokenTexts = new List<string>(tokens.Count);
            List<Color> colors = new List<Color>(tokens.Count);
            int position = 0;
            foreach (XamlToken token in tokens)
            {
                string tokenText = xamlText.Substring(position, token.Length);
                tokenTexts.Add(tokenText);
                Color color = XamlTokenizer.ColorForToken(token, tokenText);
                colors.Add(color);
                position += token.Length;
            }

            Paragraph paragraphText = new Paragraph();
            for (int i = 0; i < tokenTexts.Count; i++)
            {
                Run r = new Run(tokenTexts[i]);
                r.Foreground = new SolidColorBrush(colors[i]);
                paragraphText.Inlines.Add(r);
            }

            return paragraphText;
        }
    }

    /// <summary>
    /// Tokenizer for the next line of XAML 
    /// </summary>
    public enum XamlTokenizerMode
    {
        /// <summary>
        /// Define Inside Comment.
        /// </summary>
        InsideComment,

        /// <summary>
        /// Define Inside Processing Instruction. 
        /// </summary>
        InsideProcessingInstruction,

        /// <summary>
        /// Define the AfterOpen Mode.
        /// </summary>
        AfterOpen,

        /// <summary>
        /// Define the AfterAttributeName Mode.
        /// </summary>
        AfterAttributeName,

        /// <summary>
        /// Define the AfterAttributeEquals Mode.
        /// </summary>
        AfterAttributeEquals,

        /// <summary>
        /// Define InsideElement Mode.
        /// </summary>
        InsideElement,

        /// <summary>
        /// Define OutsideElement Mode.
        /// </summary>
        OutsideElement,

        /// <summary>
        /// Define InsideData Mode.
        /// </summary>
        InsideCData,
    }

    /// <summary>
    /// XAML Tokens
    /// </summary>
   public struct XamlToken
    {
        /// <summary>
        /// Define XamlToken Kind.
        /// </summary>
        public XamlTokenKind Kind;

        /// <summary>
        /// Define the Length.
        /// </summary>
        public short Length;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlToken"/> struct.
        /// </summary>
        /// <param name="kind">The kind value.</param>
        /// <param name="length">The length.</param>
        public XamlToken(XamlTokenKind kind, int length)
        {
            Kind = kind;
            Length = (short)length;
        }
    }

    /// <summary>
    /// XAML token kinds
    /// </summary>
    public enum XamlTokenKind : short
    {
        /// <summary>
        /// Represents open symbol.
        /// </summary>
        Open,

        /// <summary>
        /// Represents close symbol.
        /// </summary>
        Close,

        /// <summary>
        /// Represents self closed symbol.
        /// </summary>
        SelfClose,

        /// <summary>
        /// Represents open and close symbol.
        /// </summary>
        OpenClose,

        /// <summary>
        /// Represents element name.
        /// </summary>
        ElementName,

        /// <summary>
        /// Represents white space.
        /// </summary>
        ElementWhitespace,

        /// <summary>
        /// Represents attribute name.
        /// </summary>
        AttributeName,

        /// <summary>
        /// Represents equal symbol.
        /// </summary>
        Equals,

        /// <summary>
        /// Represents attribute value.
        /// </summary>
        AttributeValue,

        /// <summary>
        /// Represents comments beginning.
        /// </summary>
        CommentBegin,

        /// <summary>
        /// Represents comments text.
        /// </summary>
        CommentText,

        /// <summary>
        /// Represents comments end.
        /// </summary>
        CommentEnd,

        /// <summary>
        /// Represents entity.
        /// </summary>
        Entity,

        /// <summary>
        /// Represents open processing instruction.
        /// </summary>
        OpenProcessingInstruction,

        /// <summary>
        /// Represents close processing instruction.
        /// </summary>
        CloseProcessingInstruction,

        /// <summary>
        /// Represents CData begin.
        /// </summary>
        CDataBegin,

        /// <summary>
        /// Represents CData end.
        /// </summary>
        CDataEnd,

        /// <summary>
        /// Represents text constant.
        /// </summary>
        TextContent,

        /// <summary>
        /// Represents end of file.
        /// </summary>
        EOF
    }

    /// <summary>
    /// XAML tokenizer which tokens are designed to match VS syntax highlighting. 
    /// </summary>
    public class XamlTokenizer
    {
        /// <summary>
        /// Define the Input value.
        /// </summary>
        private string input;

        /// <summary>
        /// Define the Position.
        /// </summary>
        private int position = 0;

        /// <summary>
        /// Define the mode
        /// </summary>
        private XamlTokenizerMode mode = XamlTokenizerMode.OutsideElement;

        /// <summary>
        /// Tokenizes the specified input.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <returns>Return Xaml Token</returns>
        public static List<XamlToken> Tokenize(string input)
        {
            XamlTokenizerMode mode = XamlTokenizerMode.OutsideElement;
            XamlTokenizer tokenizer = new XamlTokenizer();
            return tokenizer.Tokenize(input, ref mode);
        }

        /// <summary>
        /// Tokenizes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="x_mode">The x_mode.</param>
        /// <returns>Return Xaml Token</returns>
        public List<XamlToken> Tokenize(string input, ref XamlTokenizerMode x_mode)
        {
            this.input = input;
            this.mode = x_mode;
            this.position = 0;
            List<XamlToken> result = Tokenize();
            x_mode = this.mode;
            return result;
        }

        /// <summary>
        /// Tokenizes this instance.
        /// </summary>
        /// <returns>Return Xaml Token.</returns>
        private List<XamlToken> Tokenize()
        {
            List<XamlToken> list = new List<XamlToken>();
            try
            {
                XamlToken token;
                do
                {
                    int previousPosition = position;
                    token = NextToken();
                    string tokenText = input.Substring(previousPosition, token.Length);
                    list.Add(token);
                }
                while (token.Kind != XamlTokenKind.EOF);

                List<string> strings = TokensToStrings(list, input);
            }
            catch (Exception) 
            {
            }

            return list;
        }

        /// <summary>
        /// Tokens to strings.
        /// </summary>
        /// <param name="list">The list of XamlToken.</param>
        /// <param name="input">The input value.</param>
        /// <returns>return output</returns>
        private List<string> TokensToStrings(List<XamlToken> list, string input)
        {
            List<string> output = new List<string>();
            int position = 0;
            foreach (XamlToken token in list)
            {
                output.Add(input.Substring(position, token.Length));
                position += token.Length;
            }

            return output;
        }

        /// <summary>
        /// Gets the remaining text.
        /// </summary>
        /// <value>The remaining text.</value>
        public string RemainingText
        {
            get
            {
                return input.Substring(position);
            }
        }

        /// <summary>
        /// Next's the token.
        /// </summary>
        /// <returns>Return XamlToken</returns>
        private XamlToken NextToken()
        {
            if (position >= input.Length)
            {
                return new XamlToken(XamlTokenKind.EOF, 0);
            }

            XamlToken token;
            switch (mode)
            {
                case XamlTokenizerMode.AfterAttributeEquals:
                    token = TokenizeAttributeValue();
                    break;
                case XamlTokenizerMode.AfterAttributeName:
                    token = TokenizeSimple("=", XamlTokenKind.Equals, XamlTokenizerMode.AfterAttributeEquals);
                    break;
                case XamlTokenizerMode.AfterOpen:
                    token = TokenizeName(XamlTokenKind.ElementName, XamlTokenizerMode.InsideElement);
                    break;
                case XamlTokenizerMode.InsideCData:
                    token = TokenizeInsideCData();
                    break;
                case XamlTokenizerMode.InsideComment:
                    token = TokenizeInsideComment();
                    break;
                case XamlTokenizerMode.InsideElement:
                    token = TokenizeInsideElement();
                    break;
                case XamlTokenizerMode.InsideProcessingInstruction:
                    token = TokenizeInsideProcessingInstruction();
                    break;
                case XamlTokenizerMode.OutsideElement:
                    token = TokenizeOutsideElement();
                    break;
                default:
                    token = new XamlToken(XamlTokenKind.EOF, 0);
                    Debug.Fail("missing case");
                    break;
            }

            return token;
        }

        /// <summary>
        /// Determines whether [is name character] [the specified character].
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>
        /// <c>true</c> if [is name character] [the specified character]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNameCharacter(char character)
        {            
            bool result = char.IsLetterOrDigit(character) || character == '.' | character == '-' | character == '_' | character == ':';
            return result;
        }

        /// <summary>
        /// Tokenizes the attribute value.
        /// </summary>
        /// <returns>Return the Xaml Token</returns>
        private XamlToken TokenizeAttributeValue()
        {
            Debug.Assert(mode == XamlTokenizerMode.AfterAttributeEquals, "The mode must always be AfterAttributeEquals.");
            int closePosition = input.IndexOf(input[position], position + 1);
            XamlToken token = new XamlToken(XamlTokenKind.AttributeValue, closePosition + 1 - position);
            position = closePosition + 1;
            mode = XamlTokenizerMode.InsideElement;
            return token;
        }

        /// <summary>
        /// Tokenizes the name.
        /// </summary>
        /// <param name="kind">The kind token value.</param>
        /// <param name="nextMode">The next mode.</param>
        /// <returns>Return XamlToken</returns>
        private XamlToken TokenizeName(XamlTokenKind kind, XamlTokenizerMode nextMode)
        {
            Debug.Assert(mode == XamlTokenizerMode.AfterOpen || mode == XamlTokenizerMode.InsideElement, "The mode must always be either AfterOpen or InsideElement.");
            int i;
            for (i = position; i < input.Length; i++)
            {
                if (!IsNameCharacter(input[i]))
                {
                    break;
                }
            }

            XamlToken token = new XamlToken(kind, i - position);
            mode = nextMode;
            position = i;
            return token;
        }

        /// <summary>
        /// Tokenizes the element whitespace.
        /// </summary>
        /// <returns>Return the XamlToken</returns>
        private XamlToken TokenizeElementWhitespace()
        {
            int i;
            for (i = position; i < input.Length; i++)
            {
                if (!char.IsWhiteSpace(input[i]))
                {
                    break;
                }
            }

            XamlToken token = new XamlToken(XamlTokenKind.ElementWhitespace, i - position);
            position = i;
            return token;
        }

        /// <summary>
        /// Starts the with.
        /// </summary>
        /// <param name="text">The text value.</param>
        /// <returns>Return the bool value.</returns>
        private bool StartsWith(string text)
        {
            if (position + text.Length > input.Length)
            {
                return false;
            }
            else
            {
                return input.Substring(position, text.Length) == text;
            }
        }

        /// <summary>
        /// Tokenizes the inside element.
        /// </summary>
        /// <returns>Return the XamlToken</returns>
        private XamlToken TokenizeInsideElement()
        {
            if (char.IsWhiteSpace(input[position]))
            {
                return TokenizeElementWhitespace();
            }
            else if (StartsWith("/>"))
            {
                return TokenizeSimple("/>", XamlTokenKind.SelfClose, XamlTokenizerMode.OutsideElement);
            }
            else if (StartsWith(">"))
            {
                return TokenizeSimple(">", XamlTokenKind.Close, XamlTokenizerMode.OutsideElement);
            }
            else
            {
                return TokenizeName(XamlTokenKind.AttributeName, XamlTokenizerMode.AfterAttributeName);
            }
        }

        /// <summary>
        /// Tokenizes the text.
        /// </summary>
        /// <returns>Return the XamlToken</returns>
        private XamlToken TokenizeText()
        {
            Debug.Assert(input[position] != '<', "The input value must not be equal to <.");
            Debug.Assert(input[position] != '&', "The input value must not be equal to &.");
            Debug.Assert(mode == XamlTokenizerMode.OutsideElement, "The mode must always be OutsideElement.");
            int i;
            for (i = position; i < input.Length; i++)
            {
                if (input[i] == '<' || input[i] == '&')
                {
                    break;
                }
            }

            XamlToken token = new XamlToken(XamlTokenKind.TextContent, i - position);
            position = i;
            return token;
        }

        /// <summary>
        /// Tokenizes the outside element.
        /// </summary>
        /// <returns>Return the XamlToken.</returns>
        private XamlToken TokenizeOutsideElement()
        {
            Debug.Assert(mode == XamlTokenizerMode.OutsideElement, "The mode must always be OutsideElement.");
            if (position >= input.Length)
            {
                return new XamlToken(XamlTokenKind.EOF, 0);
            }

            switch (input[position])
            {
                case '<':
                    return TokenizeOpen();
                case '&':
                    return TokenizeEntity();
                default:
                    return TokenizeText();
            }
        }

        /// <summary>
        /// Tokenizes the simple.
        /// </summary>
        /// <param name="text">The text value.</param>
        /// <param name="kind">The kind token value.</param>
        /// <param name="nextMode">The next mode.</param>
        /// <returns>Return XamlToken</returns>
        private XamlToken TokenizeSimple(string text, XamlTokenKind kind, XamlTokenizerMode nextMode)
        {
            XamlToken token = new XamlToken(kind, text.Length);
            position += text.Length;
            mode = nextMode;
            return token;
        }

        /// <summary>
        /// Tokenizes the open.
        /// </summary>
        /// <returns>Return the XamlToken.</returns>
        private XamlToken TokenizeOpen()
        {
            Debug.Assert(input[position] == '<', "The input value must always be <.");
            if (StartsWith("<!--"))
            {
                return TokenizeSimple("<!--", XamlTokenKind.CommentBegin, XamlTokenizerMode.InsideComment);
            }
            else if (StartsWith("<![CDATA["))
            {
                return TokenizeSimple("<![CDATA[", XamlTokenKind.CDataBegin, XamlTokenizerMode.InsideCData);
            }
            else if (StartsWith("<?"))
            {
                return TokenizeSimple("<?", XamlTokenKind.OpenProcessingInstruction, XamlTokenizerMode.InsideProcessingInstruction);
            }
            else if (StartsWith("</"))
            {
                return TokenizeSimple("</", XamlTokenKind.OpenClose, XamlTokenizerMode.AfterOpen);
            }
            else
            {
                return TokenizeSimple("<", XamlTokenKind.Open, XamlTokenizerMode.AfterOpen);
            }
        }

        /// <summary>
        /// Tokenizes the entity.
        /// </summary>
        /// <returns>Return the XamlToken.</returns>
        private XamlToken TokenizeEntity()
        {
            Debug.Assert(mode == XamlTokenizerMode.OutsideElement, "The mode must always be OutsideElement.");
            Debug.Assert(input[position] == '&', "The input value must always be &.");
            XamlToken token = new XamlToken(XamlTokenKind.Entity, input.IndexOf(';', position) - position);
            position += token.Length;
            return token;
        }

        /// <summary>
        /// Tokenizes the inside processing instruction.
        /// </summary>
        /// <returns>Return the XamlToken.</returns>
        private XamlToken TokenizeInsideProcessingInstruction()
        {
            Debug.Assert(mode == XamlTokenizerMode.InsideProcessingInstruction, "The mode must always be InsideProcessingInstruction.");
            int tokenend = input.IndexOf("?>", position);
            if (position == tokenend)
            {
                position += "?>".Length;
                mode = XamlTokenizerMode.OutsideElement;
                return new XamlToken(XamlTokenKind.CloseProcessingInstruction, "?>".Length);
            }
            else
            {
                XamlToken token = new XamlToken(XamlTokenKind.TextContent, tokenend - position);
                position = tokenend;
                return token;
            }
        }

        /// <summary>
        /// Tokenizes the inside C data.
        /// </summary>
        /// <returns>Return the XamlToken.</returns>
        private XamlToken TokenizeInsideCData()
        {
            Debug.Assert(mode == XamlTokenizerMode.InsideCData, "The mode must always be InsideCData.");
            int tokenend = input.IndexOf("]]>", position);
            if (position == tokenend)
            {
                position += "]]>".Length;
                mode = XamlTokenizerMode.OutsideElement;
                return new XamlToken(XamlTokenKind.CDataEnd, "]]>".Length);
            }
            else
            {
                XamlToken token = new XamlToken(XamlTokenKind.TextContent, tokenend - position);
                position = tokenend;
                return token;
            }
        }

        /// <summary>
        /// Tokenizes the inside comment.
        /// </summary>
        /// <returns>Return the XamlToken.</returns>
        private XamlToken TokenizeInsideComment()
        {
            Debug.Assert(mode == XamlTokenizerMode.InsideComment, "The mode must always be InsideComment.");
            int tokenend = input.IndexOf("-->", position);
            if (position == tokenend)
            {
                position += "-->".Length;
                mode = XamlTokenizerMode.OutsideElement;
                return new XamlToken(XamlTokenKind.CommentEnd, "-->".Length);
            }
            else
            {
                XamlToken token = new XamlToken(XamlTokenKind.CommentText, tokenend - position);
                position = tokenend;
                return token;
            }
        }
        
        /// <summary>
        /// Colors for token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="tokenText">The token text.</param>
        /// <returns>return color value.</returns>
        public static Color ColorForToken(XamlToken token, string tokenText)
        {
            Color color = Color.FromRgb(0, 0, 0);
            switch (token.Kind)
            {
                case XamlTokenKind.Open:
                case XamlTokenKind.OpenClose:
                case XamlTokenKind.Close:
                case XamlTokenKind.SelfClose:
                case XamlTokenKind.CommentBegin:
                case XamlTokenKind.CommentEnd:
                case XamlTokenKind.CDataBegin:
                case XamlTokenKind.CDataEnd:
                case XamlTokenKind.Equals:
                case XamlTokenKind.OpenProcessingInstruction:
                case XamlTokenKind.CloseProcessingInstruction:
                case XamlTokenKind.AttributeValue:
                    color = Color.FromRgb(0, 0, 255);
                    break;
                case XamlTokenKind.ElementName:
                    color = Color.FromRgb(163, 21, 21);
                    break;
                case XamlTokenKind.TextContent:
                    color = Color.FromRgb(0, 0, 0);
                    break;
                case XamlTokenKind.AttributeName:
                case XamlTokenKind.Entity:
                    color = Color.FromRgb(255, 0, 0);
                    break;
                case XamlTokenKind.CommentText:
                    color = Color.FromRgb(0, 128, 0);
                    break;
            }

            if (token.Kind == XamlTokenKind.ElementWhitespace || (token.Kind == XamlTokenKind.TextContent && tokenText.Trim() == String.Empty))
            {
            }

            return color;
        }
    }
}
