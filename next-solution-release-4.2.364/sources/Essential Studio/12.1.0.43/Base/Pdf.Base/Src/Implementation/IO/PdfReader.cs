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
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Syncfusion.Pdf.Primitives;


namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Summary description for PDFReader.
    /// </summary>
#if NETFX_CORE || WP
    public class PdfReader
#else
    internal class PdfReader
#endif
        : TextReader
    {
        #region Fields
        /// <summary>
        /// The stream our reader works with.
        /// </summary>
        private Stream m_stream;
        /// <summary>
        /// A string with all delimeter characters except whitespaces,
        /// which are listed elsewhere.
        /// </summary>
        private string m_delimiters = "()<>[]{}/%";
        /// <summary>
        /// Holds peeked byte.
        /// </summary>
        private int m_peekedByte;
        /// <summary>
        /// Indicates if a byte was peeked.
        /// </summary>
        private bool m_bBytePeeked;
        #endregion

        #region Properties
        /// <summary>
        /// Sets or gets the position within the stream.
        /// </summary>
        public long Position
        {
            get
            {
                return m_stream.Position;
            }
            set
            {
                m_stream.Position = value;
            }
        }
        /// <summary>
        /// Returns the underlying stream.
        /// </summary>
        public Stream Stream
        {
            get
            {
                return m_stream;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize an instance of the PDFReader class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public PdfReader(Stream stream)
            : base()
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            m_stream = stream;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            m_stream = null;
            base.Dispose(disposing);
        }
        /// <summary>
        /// Closes the object.
        /// </summary>
#if !NETFX_CORE   && !WP
        public override void Close()
        {
            Dispose(true);
        }
#endif
        /// <summary>
        /// Reads a single line from the stream.
        /// </summary>
        /// <returns>The line read.</returns>
        public override string ReadLine()
        {
            string line = string.Empty;
            int character;

            //Read the line.
            character = m_stream.ReadByte();
            while (character != -1 && !IsEol((char)character))
            {
                line = line.Insert(line.Length, ((char)character).ToString());
                character = m_stream.ReadByte();
            }

            //Read EOL.
            if (character == '\r')
            {
                if (m_stream.ReadByte() != '\n') m_stream.Position -= 1;
            }

            return line;
        }

        /// <summary>
        /// Reads a character from the stream and andvances the current position.
        /// </summary>
        /// <returns>A character read, or -1 if EOF reached.</returns>
        public override int Read()
        {
            int retVal;

            if (m_bBytePeeked)
            {
                GetPeeked(out retVal);
            }
            else
            {
                retVal = m_stream.ReadByte();
            }

            return retVal;
        }
        /// <summary>
        /// Reads a character from the stream and preserves the current position of
        /// the stream.
        /// </summary>
        /// <returns>A character read, or -1 if EOF reached.</returns>
        public override int Peek()
        {
            int retVal;

            if (m_bBytePeeked)
            {
                GetPeeked(out retVal);
            }
            else
            {
                m_peekedByte = Read();
                retVal = m_peekedByte;
            }

            if (m_peekedByte != -1)
                m_bBytePeeked = true;

            return retVal;
        }
        /// <summary>
        /// Read the sequence of bytes from the stream.
        /// </summary>
        /// <param name="buffer">The storage for the characters.</param>
        /// <param name="index">The index in the buffer.</param>
        /// <param name="count">The number of characters to read.</param>
        /// <returns>The count of the characters read.</returns>
        public override int Read(char[] buffer, int index, int count)
        {
            if (count < 0)
                throw new ArgumentException("The value can't be less then zero", "count");

            int i = index;

            if (m_bBytePeeked && count > 0)
            {
                buffer[i] = (char)m_peekedByte;
                m_bBytePeeked = false;
                --count;
                ++i;
            }

            if (count > 0)
            {
                byte[] buf = new byte[count];
                count = m_stream.Read(buf, 0, count);

                for (int k = 0; k < count; ++k)
                {
                    char ch = (char)buf[k];
                    buffer[i + k] = ch;
                }

                i += count;
            }

            /*for( i = index; i < count + index; ++i )
            {
                int character = Read();
                if( character == -1 ) break;
                buffer[ i ] = ( char )character;
            }*/

            return i - index;
        }
        /// <summary>
        /// Read the sequence of bytes from the stream.
        /// </summary>
        /// <param name="buffer">The storage for the characters.</param>
        /// <param name="index">The index in the buffer.</param>
        /// <param name="count">The number of characters to read.</param>
        /// <returns>The count of the characters read.</returns>
        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return Read(buffer, index, count);
        }
        /// <summary>
        /// Read all bytes to the end of the stream and returns them as a single string.
        /// </summary>
        /// <returns>The characters read.</returns>
        public override string ReadToEnd()
        {
            string line = string.Empty;
            int character;

            //Read the line.
            character = Read();
            while (character != -1)
            {
                line = line.Insert(line.Length, ((char)character).ToString());
                character = m_stream.ReadByte();
            }

            return line;
        }

        internal string ReadStream()
        {
            StreamReader reader = new StreamReader(m_stream);

            return reader.ReadToEnd();
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Informs whether the character is from the EOL character.
        /// </summary>
        /// <param name="character">The character to test.</param>
        /// <returns>True if the character is from EOL characters, false otherwise.
        /// </returns>
        public bool IsEol(char character)
        {
            return (character == '\n' || character == '\r');
        }
        /// <summary>
        /// Determines if the character specified is a separator character.
        /// </summary>
        /// <param name="character">The character to test.</param>
        /// <returns>True is the character is a separator character.</returns>
        public bool IsSeparator(char character)
        {
            return (Char.IsWhiteSpace(character) || IsDelimiter(character));
        }
        /// <summary>
        /// Determines if the character specified is a delimeter character.
        /// </summary>
        /// <param name="character">The character to test.</param>
        /// <returns>True is the character is a delimeter character, false otherwise.
        /// </returns>
        public bool IsDelimiter(char character)
        {
            foreach (char c in m_delimiters)
            {
                if (c == character) return true;
            }
            return false;
        }
        /// <summary>
        /// Looks up for the token.
        /// </summary>
        /// <param name="token">What to look for.</param>
        /// <returns></returns>
        public long SearchBack(string token)
        {
            long pos = Position;

            // Skip any whitespace at the tail.
            SkipWSBack();
            if (Position < token.Length)
                return -1;
            string str = ReadBack(token.Length);
            pos = Position - token.Length;

            while (str.CompareTo(token) != 0)
            {
                if (pos < 0) throw new PdfDocumentException(PdfMessages.InvalidFormat +
                                               "\nUnable to find token \'" +
                                               token + "\'");
                // one byte after 'string' and one byte forward (sorry, backward).
                Position -= 1;
                if (Position < token.Length)
                    return -1;
                str = ReadBack(token.Length);
                pos = Position - token.Length;
            }
            Position = pos;
            return pos;
        }
        /// <summary>
        /// Searches for a token specified by 'token' string.
        /// </summary>
        /// <param name="token">What to search.</param>
        /// <returns>The position of the found token.</returns>
        public long SearchForward(string token)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            byte[] buf = new byte[token.Length];

            while (true)
            {
                long position = Position;

                // Read a byte from the stream...
                int character = Read();
                buf[0] = (byte)character;
                // ...and compare it with the first character from the token string.
                if (buf[0] == token[0])
                {
                    position = Position - 1;
                    // If the are equal, compare whole string.
                    int length = m_stream.Read(buf, 1, token.Length - 1);

                    Position = position;
                    if (length < token.Length - 1) // we've reached the end of the file (stream).
                    {
                        return -1;// throw new PdfDocumentException(PdfMessages.InvalidFormat);
                    }
                    else if (token.CompareTo(encoding.GetString(buf, 0, buf.Length)) == 0) // equal
                    {
                        return position;
                    }
                    else Position += 1;
                }
                else if (character == -1)
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// Reads a string from a stream in the oppozite direction.
        /// </summary>
        /// <returns>The string.</returns>
        public string ReadBack(int length)
        {
            // TODO: Think about using GetLine here.
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;

            byte[] buf = new byte[length];

            if (Position < length) throw new PdfDocumentException(PdfMessages.InvalidFormat);
            Position -= length;
            int size = m_stream.Read(buf, 0, length);
            if (size < length) throw new PdfDocumentException("Read failure.");

            return encoding.GetString(buf, 0, buf.Length);
        }
        /// <summary>
        /// Skip any whitespace at the tail.
        /// NOTE: stream.Position points to the last encounted whitespace.
        /// </summary>
        public void SkipWSBack()
        {
            if (Position == 0)
                throw new PdfDocumentException(PdfMessages.InvalidFormat);
            Position -= 1;
            while (Char.IsWhiteSpace((char)Read()))
            {
                Position -= 2;
            }
        }
        /// <summary>
        /// Skips all white spaces. stream.Position will point to the first nonspase
        /// character or EOF.
        /// </summary>
        public void SkipWS()
        {
            if (Position == m_stream.Length)
                return;

            int c;

            do
            {
                c = Read();
            }
            while (Char.IsWhiteSpace((char)c));
            if (c == -1) Position = m_stream.Length;
            else Position -= 1;
        }
        /// <summary>
        /// Returns next string separated by delimiters.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetNextToken()
        {
            string token = string.Empty;
            int character;

            SkipWS();
            character = Peek();

            //Return the character if it is a delimiter character.
            if (IsDelimiter((char)character))
            {
                character = AppendChar(ref token);
                return token;
            }

            //Read all character sequentially until separator read.
            while (character != -1 && !IsSeparator((char)character))
            {
                character = AppendChar(ref token);
                character = Peek();
            }

            return token;
        }

        /// <summary>
        /// Seeks for the position specified.
        /// </summary>
        /// <param name="offset">The origin relative offset.</param>
        /// <param name="origin">The origin for the offset.</param>
        /// <returns>The zero based position in the stream.</returns>
        public long Seek(long offset, SeekOrigin origin)
        {
            return m_stream.Seek(offset, origin);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Appends a line with the next character from the stream.
        /// Also it advances the current stream position.
        /// </summary>
        /// <param name="line">A text string.</param>
        /// <returns>The resulting the character read,
        /// or -1 if EOF was reached during reading.</returns>
        private int AppendChar(ref string line)
        {
            int character = Read();

            if (character != -1)
            {
                line = line.Insert(line.Length, ((char)character).ToString());
            }

            return character;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the peeked byte value.
        /// </summary>
        /// <param name="byteValue">The byte value.</param>
        /// <returns>True if the byte was acquired.</returns>
        private bool GetPeeked(out int byteValue)
        {
            bool retVal = m_bBytePeeked;

            if (m_bBytePeeked)
            {
                m_bBytePeeked = false;
                byteValue = m_peekedByte;
            }
            else
            {
                byteValue = 0;
            }

            return retVal;
        }
        #endregion
    }
}
