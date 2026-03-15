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
using System.Text;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;

namespace Syncfusion.Pdf.IO
{
#if NETFX_CORE || WP
    public class PdfParser
# else
    internal class PdfParser
#endif
    {
        #region Fields
        /// <summary>
        /// The cross-reference table.
        /// </summary>
        private CrossTable m_cTable;
        /// <summary>
        /// The reader.
        /// </summary>
        private PdfReader m_reader;
        /// <summary>
        /// PDF lexer.
        /// </summary>
        private PdfLexer m_lexer;
        /// <summary>
        /// The next token.
        /// </summary>
        private TokenType m_next;
        /// <summary>
        /// Holds all integers that have been read ahead.
        /// </summary>
        private Queue<int> m_integerQueue = new Queue<int>();
        /// <summary>
        /// he high level cross-table. It's required for convertion
        /// PDFReferences into PDFReferenceHolders.
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Internal variable to identify the current object is PdfEncryptor or not.
        /// </summary>
        private bool m_bEncrypt = false;
        /// <summary>
        /// To identify whether the dictionary contains colorspace
        /// </summary>
        private bool m_colorSpace = false;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PDFParser"/> class.
        /// </summary>
        /// <param name="cTable">The cross-reference table.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="crossTable">The cross table.</param>
        public PdfParser(CrossTable cTable, PdfReader reader, PdfCrossTable crossTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cTable == null)
                throw new ArgumentNullException("cTable");

            if (crossTable == null)
                throw new ArgumentNullException("crossTable");

            m_reader = reader;
            m_cTable = cTable;
            m_crossTable = crossTable;

            m_lexer = new PdfLexer(reader);
        }
        #endregion

        #region Properties.
        /// <summary>
        /// Gets or sets the current object is PdfEncryptor or not. 
        /// </summary>
        internal bool Encrypted
        {
            get
            {
                return m_bEncrypt;
            }
            set
            {
                m_bEncrypt = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parses a PDF object.
        /// </summary>
        /// <param name="offset">The offset to the object.</param>
        /// <returns>The object.</returns>
        public IPdfPrimitive Parse(long offset)
        {
            SetOffset(offset);
            Advance();

            return Parse();
        }

        /// <summary>
        /// Parses a PDF object.
        /// </summary>
        /// <returns>The object.</returns>
        public IPdfPrimitive Parse()
        {
            Match(m_next, TokenType.Number);
            PdfNumber num1 = Simple() as PdfNumber;

            PdfNumber num2 = Simple() as PdfNumber;

            Match(m_next, TokenType.ObjectStart);
            Advance();

            IPdfPrimitive obj = Simple();
            if (m_next != TokenType.ObjectEnd)
            {
                m_next = TokenType.ObjectEnd;
            }
            Match(m_next, TokenType.ObjectEnd);
            if (!m_lexer.Skip)
            {
                Advance();
            }
            else
            {
                m_lexer.Skip = false;
            }

            return obj;
        }

        /// <summary>
        /// Reads a trailer from the stream at the offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The trailer dictionary.</returns>
        public IPdfPrimitive Trailer(long offset)
        {
            SetOffset(offset);
            return Trailer();
        }

        /// <summary>
        /// Reads a trailer from the stream at the offset.
        /// </summary>
        /// <returns>The trailer dictionary.</returns>
        public IPdfPrimitive Trailer()
        {
            Match(m_next, TokenType.Trailer);

            Advance();
            return Dictionary();
        }

        /// <summary>
        /// Reads startxref entry.
        /// </summary>
        /// <returns>The offset to the XRef table.</returns>
        public long StartXRef()
        {
            Advance();
            Match(m_next, TokenType.StartXRef);
            Advance();
            PdfNumber number = Number() as PdfNumber;

            return number.IntValue;
        }

        /// <summary>
        /// Sets the offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void SetOffset(long offset)
        {
            m_reader.Position = offset;
            if (m_integerQueue.Count > 0)
                m_integerQueue.Clear();
            m_lexer.Reset();
        }

        /// <summary>
        /// Parses the XRef table.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <param name="cTable">The cross-reference table.</param>
        /// <returns>The trailer dictionary.</returns>
        public IPdfPrimitive ParseXRefTable(Dictionary<long, Syncfusion.Pdf.IO.CrossTable.ObjectInformation> objects, CrossTable cTable)
        {
            IPdfPrimitive obj = null;

            Advance();

            if (m_next == TokenType.XRef) // The old table.
            {
                ParseOldXRef(cTable, objects);
                obj = Trailer();
                PdfDictionary trailerDic = obj as PdfDictionary;
                if (trailerDic.ContainsKey("Size"))
                {
                    int size = (trailerDic["Size"] as PdfNumber).IntValue;
                    int initialNumber = (int)cTable.m_initialNumberOfSubsection;
                    int total = (int)cTable.m_totalNumberOfSubsection;
                    if (size < initialNumber + total && initialNumber > 0 && size == total)
                    {
                        int difference = initialNumber + total - size;
                        Dictionary<long, Syncfusion.Pdf.IO.CrossTable.ObjectInformation> newObjects = new Dictionary<long, CrossTable.ObjectInformation>();
                        foreach (KeyValuePair<long, Syncfusion.Pdf.IO.CrossTable.ObjectInformation> item in objects)
                        {
                            newObjects.Add(item.Key - difference, item.Value);
                        }
                        objects = newObjects;
                        cTable.m_objects = newObjects;
                    }
                }
            }
            else // The new table.
            {
                obj = Parse();
                cTable.ParseNewTable(obj as PdfStream, objects);
            }

            return obj;
        }
       
        /// <summary>
        /// Rebuild the xref table for corrupted PDF Documents
        /// </summary>
        /// <param name="newObjects"></param>
        /// <param name="crosstable"></param>
        public void RebuildXrefTable(Dictionary<long, Syncfusion.Pdf.IO.CrossTable.ObjectInformation> newObjects, CrossTable crosstable)
        {

            PdfReader reader = new PdfReader(this.m_reader.Stream);
            reader.Position = 0;
            newObjects.Clear();
            long objNumber, marker, type;
            while (true)
            {
                if (m_reader.Position >= reader.Stream.Length - 1)
                {
                    break;
                }
                string str = "";
                while (str == "") { str = reader.ReadLine(); }

                char[] tokens = str.ToCharArray();
                if (tokens[0] >= '0' && tokens[0] <= '9')
                {

                    string[] words = str.Split(' ');
                    if (words.Length > 1)
                    {
                        //check the condition to find the object using object number and "obj" keyword
                        if (Int64.TryParse(words[0], out objNumber))
                        {
                            if (Int64.TryParse(words[1], out marker))
                            {
                                if (marker == 0 && words[2].Equals(DictionaryProperties.Obj))
                                {

                                    Syncfusion.Pdf.IO.CrossTable.ObjectInformation objectInfo = new CrossTable.ObjectInformation(CrossTable.ObjectType.Normal, m_reader.Position - tokens.Length - 1, null, crosstable);
                                    newObjects.Add(objNumber, objectInfo);
                                }
                            }

                        }
                    }
                }
            }
        }


        /// <summary>
        /// Reads a simple object from the stream.
        /// </summary>
        /// <returns></returns>
        internal IPdfPrimitive Simple()
        {
            IPdfPrimitive obj;

            if (m_integerQueue.Count != 0)
            {
                obj = Number();
            }
            else
            {
                switch (m_next)
                {
                    case TokenType.DictionaryStart:
                        obj = Dictionary();
                        break;

                    case TokenType.ArrayStart:
                        obj = Array();
                        break;

                    case TokenType.HexStringStart:
                        obj = HexString();
                        break;

                    case TokenType.String:
                        obj = ReadString();
                        break;

                    case TokenType.UnicodeString:
                        obj = ReadUnicodeString();
                        break;

                    case TokenType.Name:
                        obj = ReadName();
                        break;

                    case TokenType.Boolean:
                        obj = ReadBoolean();
                        break;

                    case TokenType.Real:
                        obj = Real();
                        break;

                    case TokenType.Number:
                        obj = Number();
                        break;

                    case TokenType.Null:
                        obj = new PdfNull();
                        Advance();
                        break;

                    default:
                        //  Error( ErrorType.Unexpected, m_next.ToString() );
                        obj = null;
                        break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Reads the object flag.
        /// </summary>
        /// <returns>A string that holds the flag.</returns>
        internal char GetObjectFlag()
        {
            Match(m_next, TokenType.ObjectType);
            char type = m_lexer.Text[0];
            Advance();

            return type;
        }

        /// <summary>
        /// Starts from the offset specified.
        /// </summary>
        /// <param name="offset">The offset.</param>
        internal void StartFrom(long offset)
        {
            SetOffset(offset);
            Advance();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Parses old xref table.
        /// </summary>
        /// <param name="cTable">Cross table object.</param>
        /// <param name="objects">A collection of the objects.</param>
        private void ParseOldXRef(CrossTable cTable, Dictionary<long, Syncfusion.Pdf.IO.CrossTable.ObjectInformation> objects)
        {
            Advance();
            while (IsSubsection())
            {
                // read the start object number and the number of objects in the subsection.
                cTable.ParseSubsection(this, objects);
            }

        }

        /// <summary>
        /// Determines whether there is a subsection.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if there is subsection; otherwise <c>false</c>.
        /// </returns>
        private bool IsSubsection()
        {
            bool result = false;

            if (m_next == TokenType.Trailer) // Stop.
            {
                result = false;
            }
            else if (m_next == TokenType.Number) // Subsection.
            {
                result = true;
            }
            else // Error.
            {
                throw new PdfDocumentException(PdfMessages.InvalidFormat);
            }

            return result;
        }

        /// <summary>
        /// Reacts on an error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="additional">The additional information.</param>
        private void Error(ErrorType error, string additional)
        {
            string message;

            switch (error)
            {
                case ErrorType.Unexpected:
                    message = "Unexpected token ";
                    break;

                case ErrorType.BadlyFormedReal:
                    message = "Badly formed real number ";
                    break;

                case ErrorType.BadlyFormedInteger:
                    message = "Badly formed integer number ";
                    break;

                case ErrorType.UnknownStreamLength:
                    message = "Unknown stream length";
                    break;

                case ErrorType.BadlyFormedDictionary:
                    message = "Badly formed dictionary ";
                    break;

                case ErrorType.None:
                default:
                    message = "Internal error.";
                    break;
            }

            if (additional != null)
            {
                message = message + additional + " before " + m_lexer.Position;
            }

            throw new PdfException(message);
        }

        /// <summary>
        /// Matches the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="match">The match.</param>
        private void Match(TokenType token, TokenType match)
        {
            if (token != match)
                Error(ErrorType.Unexpected, token.ToString());
        }

        /// <summary>
        /// Reads the next token.
        /// </summary>
        private void Advance()
        {
            m_next = m_lexer.GetNextToken();
        }

        /// <summary>
        /// Reads the name.
        /// </summary>
        /// <returns>The PDFName.</returns>
        private IPdfPrimitive ReadName()
        {
            Match(m_next, TokenType.Name);

            string name = m_lexer.Text.Substring(1);
            PdfName result = new PdfName(name);
            Advance();

            return result;
        }

        /// <summary>
        /// Reads the boolean.
        /// </summary>
        /// <returns>The PDF boolean object.</returns>
        private IPdfPrimitive ReadBoolean()
        {
            Match(m_next, TokenType.Boolean);

            bool value = (m_lexer.Text == "true");

            PdfBoolean result = new PdfBoolean(value);
            Advance();

            return result;
        }

        /// <summary>
        /// Reads the unicode string.
        /// </summary>
        /// <returns>The PDF string object.</returns>
        private IPdfPrimitive ReadUnicodeString()
        {
            char[] text = m_lexer.Text.ToCharArray();

            string value = new string(text, 1, text.Length - 2);
            //Console.WriteLine( "Unicode string: {0}", value );

            byte[] preamble = Encoding.BigEndianUnicode.GetPreamble();

            string preambleString = PdfString.ByteToString(preamble);

            if (text.Length > 1)
            {
                if (value.Substring(0, 2).Equals(preambleString))
                {
                    ProcessUnicodeWithPreamble(ref value);
                }

                else
                {
                    value = ProcessUnicodeEscapes(value);
                }
            }
            else
            {
                value = ProcessUnicodeEscapes(value);
            }
            //value = ProcessUnicodeEscapes(value);

            PdfString str = new PdfString(value);
            if (!m_lexer.Skip)
            {
                Advance();
            }
            else
            {
                m_next = TokenType.DictionaryEnd;
            }
            return str;
        }

        /// <summary>
        /// Processes the unicode escapes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A nornal string.</returns>
        private string ProcessUnicodeEscapes(string text)
        {
            StringBuilder sb = new StringBuilder(text.Length / 2);
            bool start = true;
            char value = '\0';

            foreach (char c in text)
            {
                if (start)
                {
                    if (c == ' ')
                    {
                        sb.Append(c);
                        start = !start;
                    }
                    else
                    {
                        value = (char)(c << 8);
                    }
                }
                else
                {
                    if (c != '\\' && c != '\r')
                    {
                        if ((int)(value + c) <= 257)
                        {
                            value += c;
                            sb.Append(value);
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                value = '\0';
                                value += c;
                                sb.Append(value);
                            }
                        }
                    }
                    else
                    {
                        start = !start;
                    }
                }

                start = !start;
            }

            string result = ProcessEscapes(sb.ToString());

            return result;
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <returns>The string.</returns>
        private IPdfPrimitive ReadString()
        {
            Match(m_next, TokenType.String);
            string text = m_lexer.StringText.ToString();
            bool unicode = false;

            // Process all escape sequences.
            if (!m_colorSpace)
            {
                if (CheckForPreamble(text))
                {
                    ProcessUnicodeWithPreamble(ref text);
                    unicode = true;
                }
                else
                {
                    text = ProcessEscapes(text);
                    if (CheckForPreamble(text))
                    {
                        ProcessUnicodeWithPreamble(ref text);
                        unicode = true;
                    }
                }
            }
            else
                text = "ColorFound" + text;

            PdfString str = new PdfString(text);
            if (!unicode)
                str.Encode = PdfString.ForceEncoding.ASCII;
            Advance();

            return str;
        }

        /// <summary>
        /// Checks if the string is Big Endian Encoded
        /// </summary>
        /// <param name="text">Encoded string</param>
        /// <returns>True if the string is Big Endian Encoded</returns>
        private bool CheckForPreamble(string text)
        {
            byte[] preamble = Encoding.BigEndianUnicode.GetPreamble();
            string preambleString = PdfString.ByteToString(preamble);
            if (text.Length > 1)
            {
                if (text.Substring(0, 2).Equals(preambleString))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Processes escapes.
        /// </summary>
        /// <param name="text">A text string.</param>
        /// <returns>A string without escape sequences.</returns>
        private string ProcessEscapes(string text)
        {
            text = text.Replace("\r", "");
            StringBuilder sb = new StringBuilder(text.Length);

            bool escape = false;

            for (int i = 0, count = text.Length; i < count; ++i)
            {
                char c = text[i];

                if (!escape)
                {
                    if (c == '\\') // start escape sequence.
                    {
                        escape = true;
                    }
                    else
                    {
                        if (c != '\0')
                        {
                            sb.Append(c);
                        }
                        else if (Encrypted)
                        {
                            sb.Append(c);
                        }
                    }
                }
                else // process escape.
                {
                    switch (c)
                    {
                        case 'r':
                            sb.Append('\r');
                            break;

                        case 'n':
                            sb.Append('\n');
                            break;

                        case 't':
                            sb.Append('\t');
                            break;

                        case 'b':
                            sb.Append('\b');
                            break;

                        case 'f':
                            sb.Append('\f');
                            break;

                        case '(':
                        case ')':
                        case '\\':
                            sb.Append(c);
                            break;

                        default:
                            if (c <= '7' && c >= '0')
                            {
                                //next three characters form octal digit.
                                c = ProcessOctal(text, ref i);
                                i--;

                            }
                            int value = (int)c;
                            if (value < 256)
                            {
                                sb.Append(c);
                            }
                            break;
                    }

                    escape = false;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Processes the octal number.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="i">The index.</param>
        /// <returns>The caracter which value is equal to the octal number.</returns>
        private char ProcessOctal(string text, ref int i)
        {
            int length = text.Length;
            int count = 0;
            int num = 0;
            string octalText = string.Empty;

            while (i < length && count < 3)
            {
                char c = text[i];

                if (c <= '7' && c >= '0')
                {
                    octalText += c.ToString();
                }
                ++i;
                ++count;
            }
            num = Convert.ToInt32(octalText, 8);

            return (char)num;
        }

        /// <summary>
        /// Read a string coded in hexadecimal digits.
        /// </summary>
        /// <returns>A string object.</returns>
        private IPdfPrimitive HexString()
        {
            Match(m_next, TokenType.HexStringStart);

            Advance();

            StringBuilder sb = new StringBuilder(100);
            bool isHex = true;

            while (m_next != TokenType.HexStringEnd)
            {
                string text = m_lexer.Text;
                /*if( m_next == TokenType.HexDigit )
                {
                    // Read hex digits and save them.
                }
                else if( m_next == TokenType.WhiteSpace )
                {
                    // Do nothing.
                }
                else*/
                if (m_next == TokenType.HexStringWeird)
                {
                    isHex = false;
                }
                else if (m_next == TokenType.HexStringWeirdEscape)
                {
                    isHex = false;
                    text = text.Substring(1);
                }
                //else
                //{
                //  Error( ErrorType.BadlyFormedHexString, null );
                //}

                sb.Append(text);
                Advance();
            }

            Match(m_next, TokenType.HexStringEnd);
            Advance();

            PdfString result;
            result = new PdfString(sb.ToString(), !isHex);

            return result;
        }

        /// <summary>
        /// Reads an integer number.
        /// </summary>
        /// <returns>A PDF integer.</returns>
        private IPdfPrimitive Number()
        {
            IPdfPrimitive obj;
            PdfNumber integer;

            if (m_integerQueue.Count > 0)
            {
                integer = new PdfNumber(m_integerQueue.Dequeue());
            }
            else
            {
                Match(m_next, TokenType.Number);

                integer = ParseInteger();
            }

            obj = integer;

            if (m_next == TokenType.Number) // There might be either reference or two integers.
            {
                PdfNumber integer2 = ParseInteger();

                if (m_next == TokenType.Reference) // Reference
                {
                    PdfReference reference = new PdfReference(integer.IntValue, integer2.IntValue);
                    obj = new PdfReferenceHolder(reference, m_crossTable);
                    Advance();
                }
                else
                {
                    // Remember the integer.
                    m_integerQueue.Enqueue(integer2.IntValue);
                }
            }

            return obj;
        }

        /// <summary>
        /// Parses the integer.
        /// </summary>
        /// <returns>The integer.</returns>
        private PdfNumber ParseInteger()
        {
            double value;
            bool result = double.TryParse(m_lexer.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out value);

            PdfNumber integer = null;

            if (result)
            {
                integer = new PdfNumber((long)value);
            }
            else
            {
                Error(ErrorType.BadlyFormedInteger, m_lexer.Text);
            }

            Advance();

            return integer;
        }

        /// <summary>
        /// Read a real number.
        /// </summary>
        /// <returns>A PDF real.</returns>
        private IPdfPrimitive Real()
        {
            Match(m_next, TokenType.Real);
            double value;

            bool result = double.TryParse(m_lexer.Text, NumberStyles.Float,
                CultureInfo.InvariantCulture, out value);

            PdfNumber real = null;

            if (result)
            {
                real = new PdfNumber((float)value);
            }
            else
            {
                Error(ErrorType.BadlyFormedReal, m_lexer.Text);
            }

            Advance();

            return real;
        }

        /// <summary>
        /// Reads an array.
        /// </summary>
        /// <returns>The well formed array.</returns>
        private IPdfPrimitive Array()
        {
            Match(m_next, TokenType.ArrayStart);
            Advance();

            IPdfPrimitive obj;
            PdfArray array = new PdfArray();

            while ((obj = Simple()) != null)
            {
                array.Add(obj);
                if (array[0].ToString() == "/Indexed")
                    m_colorSpace = true;
                else
                    m_colorSpace = false;
            }

            Match(m_next, TokenType.ArrayEnd);
            Advance();

            array.FreezeChanges(this);

            return array;
        }

        /// <summary>
        /// Reads a dictionary from the stream.
        /// </summary>
        /// <returns>The filled PDF dictionary object.</returns>
        private IPdfPrimitive Dictionary()
        {
            Match(m_next, TokenType.DictionaryStart);
            Advance();

            PdfDictionary dic = new PdfDictionary();
            Pair pair;

            while ((pair = ReadPair()) != Pair.Empty)
            {
                //dic.Add( pair.Name, pair.Value );
                dic[pair.Name] = pair.Value;
            }
            if (m_next != TokenType.DictionaryEnd)
                m_next = TokenType.DictionaryEnd;
            Match(m_next, TokenType.DictionaryEnd);
            if (!m_lexer.Skip)
            {
                Advance();
            }
            else
            {

                m_next = TokenType.ObjectEnd;
                m_lexer.Skip = false;
            }

            IPdfPrimitive result = null;

            if (m_next == TokenType.StreamStart)
            {
                result = ReadStream(dic);
            }
            else
            {
                result = dic;
            }

            (result as IPdfChangable).FreezeChanges(this);

            return result;
        }

        /// <summary>
        /// Reads the stream.
        /// </summary>
        /// <param name="dic">The stream dictionary.</param>
        /// <returns>The PDFStream.</returns>
        private IPdfPrimitive ReadStream(PdfDictionary dic)
        {
            Match(m_next, TokenType.StreamStart);
            m_lexer.SkipToken();
            m_lexer.SkipNewLine();

            IPdfPrimitive obj = dic[DictionaryProperties.Length];
            PdfNumber length = obj as PdfNumber;
            PdfReferenceHolder reference = obj as PdfReferenceHolder;

            if (length == null && reference == null)
            {
                PdfLexer lex = m_lexer;
                long position = m_reader.Position;
                m_lexer = new PdfLexer(m_reader);
                long start = m_reader.SearchBack("stream");
                long end = m_reader.SearchForward("endstream");
                long streamLength = end - start;

                m_reader.Position = position;
                m_lexer = lex;

                byte[] buffer = m_lexer.Read((int)streamLength);

                PdfStream innerStream = new PdfStream(dic, buffer);

                Advance();
                if (m_next != TokenType.StreamEnd)
                    m_next = TokenType.StreamEnd;

                Match(m_next, TokenType.StreamEnd);
                Advance();

                if (m_next != TokenType.ObjectEnd)
                    m_next = TokenType.ObjectEnd;

                return innerStream;
            }
            else if (reference != null)
            {
                PdfLexer lex = m_lexer;
                long position = m_reader.Position;

                m_lexer = new PdfLexer(m_reader);

                obj = m_cTable.GetObject(reference.Reference);
                length = obj as PdfNumber;

                m_reader.Position = position;
                m_lexer = lex;
            }

            int intLength = length.IntValue;

            byte[] buf = m_lexer.Read(intLength);

            PdfStream stream = new PdfStream(dic, buf);

            Advance();
            if (m_next != TokenType.StreamEnd)
                m_next = TokenType.StreamEnd;

            Match(m_next, TokenType.StreamEnd);
            Advance();

            if (m_next != TokenType.ObjectEnd)
                m_next = TokenType.ObjectEnd;

            return stream;

        }

        /// <summary>
        /// Reads the pair.
        /// </summary>
        /// <returns>The well filled pair on success or Pair.Emty on failure.</returns>
        private Pair ReadPair()
        {

            //if( m_next != TokenType.Name )
            //{
            //  return Pair.Empty;
            //}

            //string name = m_lexer.Text.Substring( 1 );
            //Advance();

            IPdfPrimitive obj=null;
            try
            {
                obj = Simple();
            }
            catch
            {
                obj = null;
            }

            if (obj == null)
            {
                return Pair.Empty;
            }

            PdfName name = obj as PdfName;

            if (name == null)
            {
                Error(ErrorType.BadlyFormedDictionary, "next should be a name.");
            }

            /*IPdfSavable*/
            obj = Simple();

            return new Pair(name, obj);
        }

        private void ProcessUnicodeWithPreamble(ref string text)
        {
            byte[] data = PdfString.StringToByte(text.Substring(2));
            int textIndex = 0;
            string tempText = null;
            bool isNewLine = false;
            for (int i = 0; i < data.Length - 1; i++)
            {
                if ((data[i] == 92 && (data[i + 1] == 40 || data[i + 1] == 41 || data[i + 1] == 13 || data[i + 1] == 0x3e || data[i + 1] == 92)) || data[i] == 13)
                {
                    for (int j = i; j < data.Length - 1; j++)
                        data[j] = data[j + 1];
                    byte[] tempSub = new byte[data.Length - 1];
                    System.Buffer.BlockCopy(data, 0, tempSub, 0, data.Length - 1);
                    data = tempSub;
                    i--;
                }
                else if (data[i] == 92 && data[i + 1] == 114)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    for (int j = 0; j < i; j++)
                        ms.WriteByte(data[j]);
                    ms.WriteByte(13);
                    for (int j = i + 2; j < data.Length; j++)
                        ms.WriteByte(data[j]);
                    data = PdfStream.StreamToBytes(ms);
                    ms.Dispose();
                }
                else if (data[i] == 92 && data[i + 1] == 110)
                {
                    isNewLine = true;
                    int tempTextRange = i - 1 - textIndex;
                    tempText += Encoding.BigEndianUnicode.GetString(data, textIndex, tempTextRange);
                    tempText += "\r\n";
                    textIndex = i + 2;
                    i += 2;
                }

            }
            int remaining = data.Length - textIndex;
            if (isNewLine)
            {
                text = tempText;
                text += Encoding.BigEndianUnicode.GetString(data, textIndex, remaining);
            }
            else
                text = Encoding.BigEndianUnicode.GetString(data, textIndex, remaining);
        }

        #endregion

        #region Internals
        /// <summary>
        /// Contains constants of errors.
        /// </summary>
        private enum ErrorType
        {
            None = 0,
            Unexpected, // Unexpected token
            BadlyFormedReal,
            BadlyFormedInteger,
            BadlyFormedHexString,
            BadlyFormedDictionary,
            UnknownStreamLength,
        }
        /// <summary>
        /// Holds the name-value pair.
        /// </summary>
        private struct Pair
        {
            #region Static Fields
            /// <summary>
            /// Holds the empty pair.
            /// </summary>
            public static readonly Pair Empty = new Pair(null, null);
            #endregion

            #region Operators
            /// <summary>
            /// Compares an object and a pair.
            /// </summary>
            /// <param name="pair">The pair.</param>
            /// <param name="obj">The obj.</param>
            /// <returns>True if the object is equal to the pair.</returns>
            static public bool operator ==(Pair pair, object obj)
            {
                bool result = false;

                if (obj == null || !(obj is Pair))
                {
                    result = false;
                }
                else
                {
                    Pair pair2 = (Pair)obj;
                    result = (pair2.Name == pair.Name) && (pair.Value == pair2.Value);
                }

                return result;
            }

            /// <summary>
            /// Compares an object and a pair.
            /// </summary>
            /// <param name="pair">The pair.</param>
            /// <param name="obj">The obj.</param>
            /// <returns>True if the object is not equal to the pair.</returns>
            static public bool operator !=(Pair pair, object obj)
            {
                return !(pair == obj);
            }
            #endregion

            #region Fields
            /// <summary>
            /// The name.
            /// </summary>
            public PdfName Name;
            /// <summary>
            /// The value.
            /// </summary>
            public IPdfPrimitive Value;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Pair"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="value">The value.</param>
            public Pair(PdfName name, IPdfPrimitive value)
            {
                Name = name;
                Value = value;
            }
            #endregion

            #region Overrides
            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <param name="obj">Another object to compare to.</param>
            /// <returns>
            /// true if obj and this instance are the same type and represent the same value; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                return (this == obj);
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            /// A 32-bit signed integer that is the hash code for this instance.
            /// </returns>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            #endregion
        }
        #endregion
    }
}
