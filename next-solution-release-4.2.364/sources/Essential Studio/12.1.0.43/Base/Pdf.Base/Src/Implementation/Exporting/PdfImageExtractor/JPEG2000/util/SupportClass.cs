#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
internal class SupportClass
{
    public static byte[] ToByteArray(sbyte[] sbyteArray)
    {
        byte[] byteArray = null;
        if (sbyteArray != null)
        {
            byteArray = new byte[sbyteArray.Length];
            for (int index = 0; index < sbyteArray.Length; index++)
                byteArray[index] = (byte)sbyteArray[index];
        }
        return byteArray;
    }
    public static byte[] ToByteArray(System.String sourceString)
    {
        return System.Text.UTF8Encoding.UTF8.GetBytes(sourceString);
    }
    public static byte[] ToByteArray(System.Object[] tempObjectArray)
    {
        byte[] byteArray = null;
        if (tempObjectArray != null)
        {
            byteArray = new byte[tempObjectArray.Length];
            for (int index = 0; index < tempObjectArray.Length; index++)
                byteArray[index] = (byte)tempObjectArray[index];
        }
        return byteArray;
    }
    public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
    {
        stream.Write(throwable.StackTrace);
        stream.Flush();
    }
    public static sbyte[] ToSByteArray(byte[] byteArray)
    {
        sbyte[] sbyteArray = null;
        if (byteArray != null)
        {
            sbyteArray = new sbyte[byteArray.Length];
            for (int index = 0; index < byteArray.Length; index++)
                sbyteArray[index] = (sbyte)byteArray[index];
        }
        return sbyteArray;
    }
    public static char[] ToCharArray(sbyte[] sByteArray)
    {
        return System.Text.UTF8Encoding.UTF8.GetChars(ToByteArray(sByteArray));
    }
    public static char[] ToCharArray(byte[] byteArray)
    {
        return System.Text.UTF8Encoding.UTF8.GetChars(byteArray);
    }
    public static long Identity(long literal)
    {
        return literal;
    }
    public static ulong Identity(ulong literal)
    {
        return literal;
    }
    public static float Identity(float literal)
    {
        return literal;
    }
    public static double Identity(double literal)
    {
        return literal;
    }

    public static int URShift(int number, int bits)
    {
        if (number >= 0)
            return number >> bits;
        else
            return (number >> bits) + (2 << ~bits);
    }
    public static int URShift(int number, long bits)
    {
        return URShift(number, (int)bits);
    }
    public static long URShift(long number, int bits)
    {
        if (number >= 0)
            return number >> bits;
        else
            return (number >> bits) + (2L << ~bits);
    }
    public static long URShift(long number, long bits)
    {
        return URShift(number, (int)bits);
    }
    public static System.Int32 ReadInput(System.IO.Stream sourceStream, sbyte[] target, int start, int count)
    {

        if (target.Length == 0)
            return 0;
        byte[] receiver = new byte[target.Length];
        int bytesRead = sourceStream.Read(receiver, start, count);

        if (bytesRead == 0)
            return -1;
        for (int i = start; i < start + bytesRead; i++)
            target[i] = (sbyte)receiver[i];
        return bytesRead;
    }
    public static System.Int32 ReadInput(System.IO.TextReader sourceTextReader, sbyte[] target, int start, int count)
    {
        if (target.Length == 0) return 0;
        char[] charArray = new char[target.Length];
        int bytesRead = sourceTextReader.Read(charArray, start, count);
        if (bytesRead == 0) return -1;
        for (int index = start; index < start + bytesRead; index++)
            target[index] = (sbyte)charArray[index];
        return bytesRead;
    }
    internal class Tokenizer : System.Collections.IEnumerator
    {
        private long currentPos = 0;
        private bool includeDelims = false;
        private char[] chars = null;
        private string delimiters = " \t\n\r\f";
        public Tokenizer(System.String source)
        {
            this.chars = source.ToCharArray();
        }
        public System.String NextToken()
        {
            return NextToken(this.delimiters);
        }
        public System.String NextToken(System.String delimiters)
        {
            this.delimiters = delimiters;
            if (this.currentPos == this.chars.Length)
                throw new System.ArgumentOutOfRangeException();
            else if ((System.Array.IndexOf(delimiters.ToCharArray(), chars[this.currentPos]) != -1)
                     && this.includeDelims)
                return "" + this.chars[this.currentPos++];
            else
                return nextToken(delimiters.ToCharArray());
        }
        private System.String nextToken(char[] delimiters)
        {
            string token = "";
            long pos = this.currentPos;
            while (System.Array.IndexOf(delimiters, this.chars[currentPos]) != -1)
                if (++this.currentPos == this.chars.Length)
                {
                    this.currentPos = pos;
                    throw new System.ArgumentOutOfRangeException();
                }
            while (System.Array.IndexOf(delimiters, this.chars[this.currentPos]) == -1)
            {
                token += this.chars[this.currentPos];
                if (++this.currentPos == this.chars.Length)
                    break;
            }
            return token;
        }
        public bool HasMoreTokens()
        {
            long pos = this.currentPos;
            try
            {
                this.NextToken();
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return false;
            }
            finally
            {
                this.currentPos = pos;
            }
            return true;
        }
        public int Count
        {
            get
            {
                long pos = this.currentPos;
                int i = 0;
                try
                {
                    while (true)
                    {
                        this.NextToken();
                        i++;
                    }
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    this.currentPos = pos;
                    return i;
                }
            }
        }
        public System.Object Current
        {
            get
            {
                return (Object)this.NextToken();
            }
        }
        public bool MoveNext()
        {
            return this.HasMoreTokens();
        }
        public void Reset()
        {
            ;
        }
    }
    private class BackStringReader : System.IO.StringReader
    {
        private char[] buffer;
        private int position = 1;
        public BackStringReader(String s)
            : base(s)
        {
            this.buffer = new char[position];
        }
        public override int Read()
        {
            if (this.position >= 0 && this.position < this.buffer.Length)
                return (int)this.buffer[this.position++];
            return base.Read();
        }
        public override int Read(char[] array, int index, int count)
        {
            int readLimit = this.buffer.Length - this.position;
            if (count <= 0)
                return 0;
            if (readLimit > 0)
            {
                if (count < readLimit)
                    readLimit = count;
                System.Array.Copy(this.buffer, this.position, array, index, readLimit);
                count -= readLimit;
                index += readLimit;
                this.position += readLimit;
            }
            if (count > 0)
            {
                count = base.Read(array, index, count);
                if (count == -1)
                {
                    if (readLimit == 0)
                        return -1;
                    return readLimit;
                }
                return readLimit + count;
            }
            return readLimit;
        }
        public void UnRead(int unReadChar)
        {
            this.position--;
            this.buffer[this.position] = (char)unReadChar;
        }
        public void UnRead(char[] array, int index, int count)
        {
            this.Move(array, index, count);
        }
        public void UnRead(char[] array)
        {
            this.Move(array, 0, array.Length - 1);
        }
        private void Move(char[] array, int index, int count)
        {
            for (int arrayPosition = index + count; arrayPosition >= index; arrayPosition--)
                this.UnRead(array[arrayPosition]);
        }
    }
    internal class StreamTokenizerSupport
    {
        private const System.String TOKEN = "Token[";
        private const System.String NOTHING = "NOTHING";
        private const System.String NUMBER = "number=";
        private const System.String EOF = "EOF";
        private const System.String EOL = "EOL";
        private const System.String QUOTED = "quoted string=";
        private const System.String LINE = "], Line ";
        private const System.String DASH = "-.";
        private const System.String DOT = ".";
        private const int TT_NOTHING = -4;
        private const sbyte ORDINARYCHAR = 0x00;
        private const sbyte WORDCHAR = 0x01;
        private const sbyte WHITESPACECHAR = 0x02;
        private const sbyte COMMENTCHAR = 0x04;
        private const sbyte QUOTECHAR = 0x08;
        private const sbyte NUMBERCHAR = 0x10;
        private const int STATE_NEUTRAL = 0;
        private const int STATE_WORD = 1;
        private const int STATE_NUMBER1 = 2;
        private const int STATE_NUMBER2 = 3;
        private const int STATE_NUMBER3 = 4;
        private const int STATE_NUMBER4 = 5;
        private const int STATE_STRING = 6;
        private const int STATE_LINECOMMENT = 7;
        private const int STATE_DONE_ON_EOL = 8;
        private const int STATE_PROCEED_ON_EOL = 9;
        private const int STATE_POSSIBLEC_COMMENT = 10;
        private const int STATE_POSSIBLEC_COMMENT_END = 11;
        private const int STATE_C_COMMENT = 12;
        private const int STATE_STRING_ESCAPE_SEQ = 13;
        private const int STATE_STRING_ESCAPE_SEQ_OCTAL = 14;
        private const int STATE_DONE = 100;
        private sbyte[] attribute = new sbyte[256];
        private bool eolIsSignificant = false;
        private bool slashStarComments = false;
        private bool slashSlashComments = false;
        private bool lowerCaseMode = false;
        private bool pushedback = false;
        private int lineno = 1;
        private BackReader inReader;
        private BackStringReader inStringReader;
        private BackInputStream inStream;
        private System.Text.StringBuilder buf;
        public const int TT_EOF = -1;
        public const int TT_EOL = '\n';
        public const int TT_NUMBER = -2;
        public const int TT_WORD = -3;
        public double nval;
        public System.String sval;
        public int ttype;
        private int read()
        {
            if (this.inReader != null)
                return this.inReader.Read();
            else if (this.inStream != null)
                return this.inStream.Read();
            else
                return this.inStringReader.Read();
        }
        private void unread(int ch)
        {
            if (this.inReader != null)
                this.inReader.UnRead(ch);
            else if (this.inStream != null)
                this.inStream.UnRead(ch);
            else
                this.inStringReader.UnRead(ch);
        }
        private void init()
        {
            this.buf = new System.Text.StringBuilder();
            this.ttype = StreamTokenizerSupport.TT_NOTHING;
            this.WordChars('A', 'Z');
            this.WordChars('a', 'z');
            this.WordChars(160, 255);
            this.WhitespaceChars(0x00, 0x20);
            this.CommentChar('/');
            this.QuoteChar('\'');
            this.QuoteChar('\"');
            this.ParseNumbers();
        }
        private void setAttributes(int low, int hi, sbyte attrib)
        {
            int l = System.Math.Max(0, low);
            int h = System.Math.Min(255, hi);
            for (int i = l; i <= h; i++)
                this.attribute[i] = attrib;
        }
        private bool isWordChar(int data)
        {
            char ch = (char)data;
            return (data != -1 && (ch > 255 || this.attribute[ch] == StreamTokenizerSupport.WORDCHAR || this.attribute[ch] == StreamTokenizerSupport.NUMBERCHAR));
        }
        public StreamTokenizerSupport(System.IO.StringReader reader)
        {
            string s = "";
            for (int i = reader.Read(); i != -1; i = reader.Read())
            {
                s += (char)i;
            }
            //reader.Close();
            this.inStringReader = new BackStringReader(s);
            this.init();
        }
        public StreamTokenizerSupport(System.IO.StreamReader reader)
        {
            this.inReader = new BackReader(new System.IO.StreamReader(reader.BaseStream, reader.CurrentEncoding).BaseStream, 2, reader.CurrentEncoding);
            this.init();
        }
        public StreamTokenizerSupport(System.IO.Stream stream)
        {
            //this.inStream = new BackInputStream(new System.IO.BufferedStream(stream), 2);
            this.inStream = new BackInputStream(stream, 2);
            this.init();
        }
        public virtual void CommentChar(int ch)
        {
            if (ch >= 0 && ch <= 255)
                this.attribute[ch] = StreamTokenizerSupport.COMMENTCHAR;
        }
        public virtual void EOLIsSignificant(bool flag)
        {
            this.eolIsSignificant = flag;
        }
        public virtual int Lineno()
        {
            return this.lineno;
        }
        public virtual void LowerCaseMode(bool flag)
        {
            this.lowerCaseMode = flag;
        }
        public virtual int NextToken()
        {
            char prevChar = (char)(0);
            char ch = (char)(0);
            char qChar = (char)(0);
            int octalNumber = 0;
            int state;
            if (this.pushedback)
            {
                this.pushedback = false;
                return this.ttype;
            }
            this.ttype = StreamTokenizerSupport.TT_NOTHING;
            state = StreamTokenizerSupport.STATE_NEUTRAL;
            this.nval = 0.0;
            this.sval = null;
            this.buf.Length = 0;
            do
            {
                int data = this.read();
                prevChar = ch;
                ch = (char)data;
                switch (state)
                {
                    case StreamTokenizerSupport.STATE_NEUTRAL:
                        {
                            if (data == -1)
                            {
                                this.ttype = TT_EOF;
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            else if (ch > 255)
                            {
                                this.buf.Append(ch);
                                this.ttype = StreamTokenizerSupport.TT_WORD;
                                state = StreamTokenizerSupport.STATE_WORD;
                            }
                            else if (this.attribute[ch] == StreamTokenizerSupport.COMMENTCHAR)
                            {
                                state = StreamTokenizerSupport.STATE_LINECOMMENT;
                            }
                            else if (this.attribute[ch] == StreamTokenizerSupport.WORDCHAR)
                            {
                                this.buf.Append(ch);
                                this.ttype = StreamTokenizerSupport.TT_WORD;
                                state = StreamTokenizerSupport.STATE_WORD;
                            }
                            else if (this.attribute[ch] == StreamTokenizerSupport.NUMBERCHAR)
                            {
                                this.ttype = StreamTokenizerSupport.TT_NUMBER;
                                this.buf.Append(ch);
                                if (ch == '-')
                                    state = StreamTokenizerSupport.STATE_NUMBER1;
                                else if (ch == '.')
                                    state = StreamTokenizerSupport.STATE_NUMBER3;
                                else
                                    state = StreamTokenizerSupport.STATE_NUMBER2;
                            }
                            else if (this.attribute[ch] == StreamTokenizerSupport.QUOTECHAR)
                            {
                                qChar = ch;
                                this.ttype = ch;
                                state = StreamTokenizerSupport.STATE_STRING;
                            }
                            else if ((this.slashSlashComments || this.slashStarComments) && ch == '/')
                                state = StreamTokenizerSupport.STATE_POSSIBLEC_COMMENT;
                            else if (this.attribute[ch] == StreamTokenizerSupport.ORDINARYCHAR)
                            {
                                this.ttype = ch;
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            else if (ch == '\n' || ch == '\r')
                            {
                                this.lineno++;
                                if (this.eolIsSignificant)
                                {
                                    this.ttype = StreamTokenizerSupport.TT_EOL;
                                    if (ch == '\n')
                                        state = StreamTokenizerSupport.STATE_DONE;
                                    else if (ch == '\r')
                                        state = StreamTokenizerSupport.STATE_DONE_ON_EOL;
                                }
                                else if (ch == '\r')
                                    state = StreamTokenizerSupport.STATE_PROCEED_ON_EOL;
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_WORD:
                        {
                            if (this.isWordChar(data))
                                this.buf.Append(ch);
                            else
                            {
                                if (data != -1)
                                    this.unread(ch);
                                this.sval = this.buf.ToString();
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_NUMBER1:
                        {
                            if (data == -1 || this.attribute[ch] != StreamTokenizerSupport.NUMBERCHAR || ch == '-')
                            {
                                if (this.attribute[ch] == StreamTokenizerSupport.COMMENTCHAR && System.Char.IsNumber(ch))
                                {
                                    this.buf.Append(ch);
                                    state = StreamTokenizerSupport.STATE_NUMBER2;
                                }
                                else
                                {
                                    if (data != -1)
                                        this.unread(ch);
                                    this.ttype = '-';
                                    state = StreamTokenizerSupport.STATE_DONE;
                                }
                            }
                            else
                            {
                                this.buf.Append(ch);
                                if (ch == '.')
                                    state = StreamTokenizerSupport.STATE_NUMBER3;
                                else
                                    state = StreamTokenizerSupport.STATE_NUMBER2;
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_NUMBER2:
                        {
                            if (data == -1 || this.attribute[ch] != StreamTokenizerSupport.NUMBERCHAR || ch == '-')
                            {
                                if (System.Char.IsNumber(ch) && this.attribute[ch] == StreamTokenizerSupport.WORDCHAR)
                                {
                                    this.buf.Append(ch);
                                }
                                else if (ch == '.' && this.attribute[ch] == StreamTokenizerSupport.WHITESPACECHAR)
                                {
                                    this.buf.Append(ch);
                                }
                                else if ((data != -1) && (this.attribute[ch] == StreamTokenizerSupport.COMMENTCHAR && System.Char.IsNumber(ch)))
                                {
                                    this.buf.Append(ch);
                                }
                                else
                                {
                                    if (data != -1)
                                        this.unread(ch);
                                    try
                                    {
                                        this.nval = System.Double.Parse(this.buf.ToString());
                                    }
                                    catch (System.FormatException) { }
                                    state = StreamTokenizerSupport.STATE_DONE;
                                }
                            }
                            else
                            {
                                this.buf.Append(ch);
                                if (ch == '.')
                                    state = StreamTokenizerSupport.STATE_NUMBER3;
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_NUMBER3:
                        {
                            if (data == -1 || this.attribute[ch] != StreamTokenizerSupport.NUMBERCHAR || ch == '-' || ch == '.')
                            {
                                if (this.attribute[ch] == StreamTokenizerSupport.COMMENTCHAR && System.Char.IsNumber(ch))
                                {
                                    this.buf.Append(ch);
                                }
                                else
                                {
                                    if (data != -1)
                                        this.unread(ch);
                                    System.String str = this.buf.ToString();
                                    if (str.Equals(StreamTokenizerSupport.DASH))
                                    {
                                        this.unread('.');
                                        this.ttype = '-';
                                    }
                                    else if (str.Equals(StreamTokenizerSupport.DOT) && !(StreamTokenizerSupport.WORDCHAR != this.attribute[prevChar]))
                                        this.ttype = '.';
                                    else
                                    {
                                        try
                                        {
                                            this.nval = System.Double.Parse(str);
                                        }
                                        catch (System.FormatException) { }
                                    }
                                    state = StreamTokenizerSupport.STATE_DONE;
                                }
                            }
                            else
                            {
                                this.buf.Append(ch);
                                state = StreamTokenizerSupport.STATE_NUMBER4;
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_NUMBER4:
                        {
                            if (data == -1 || this.attribute[ch] != StreamTokenizerSupport.NUMBERCHAR || ch == '-' || ch == '.')
                            {
                                if (data != -1)
                                    this.unread(ch);
                                try
                                {
                                    this.nval = System.Double.Parse(this.buf.ToString());
                                }
                                catch (System.FormatException) { }
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            else
                                this.buf.Append(ch);
                            break;
                        }
                    case StreamTokenizerSupport.STATE_LINECOMMENT:
                        {
                            if (data == -1)
                            {
                                this.ttype = StreamTokenizerSupport.TT_EOF;
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            else if (ch == '\n' || ch == '\r')
                            {
                                this.unread(ch);
                                state = StreamTokenizerSupport.STATE_NEUTRAL;
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_DONE_ON_EOL:
                        {
                            if (ch != '\n' && data != -1)
                                this.unread(ch);
                            state = StreamTokenizerSupport.STATE_DONE;
                            break;
                        }
                    case StreamTokenizerSupport.STATE_PROCEED_ON_EOL:
                        {
                            if (ch != '\n' && data != -1)
                                this.unread(ch);
                            state = StreamTokenizerSupport.STATE_NEUTRAL;
                            break;
                        }
                    case StreamTokenizerSupport.STATE_STRING:
                        {
                            if (data == -1 || ch == qChar || ch == '\r' || ch == '\n')
                            {
                                this.sval = this.buf.ToString();
                                if (ch == '\r' || ch == '\n')
                                    this.unread(ch);
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            else if (ch == '\\')
                                state = StreamTokenizerSupport.STATE_STRING_ESCAPE_SEQ;
                            else
                                this.buf.Append(ch);
                            break;
                        }
                    case StreamTokenizerSupport.STATE_STRING_ESCAPE_SEQ:
                        {
                            if (data == -1)
                            {
                                this.sval = this.buf.ToString();
                                state = StreamTokenizerSupport.STATE_DONE;
                                break;
                            }
                            state = StreamTokenizerSupport.STATE_STRING;
                            if (ch == 'a')
                                this.buf.Append(0x7);
                            else if (ch == 'b')
                                this.buf.Append('\b');
                            else if (ch == 'f')
                                this.buf.Append(0xC);
                            else if (ch == 'n')
                                this.buf.Append('\n');
                            else if (ch == 'r')
                                this.buf.Append('\r');
                            else if (ch == 't')
                                this.buf.Append('\t');
                            else if (ch == 'v')
                                this.buf.Append(0xB);
                            else if (ch >= '0' && ch <= '7')
                            {
                                octalNumber = ch - '0';
                                state = StreamTokenizerSupport.STATE_STRING_ESCAPE_SEQ_OCTAL;
                            }
                            else
                                this.buf.Append(ch);
                            break;
                        }
                    case StreamTokenizerSupport.STATE_STRING_ESCAPE_SEQ_OCTAL:
                        {
                            if (data == -1 || ch < '0' || ch > '7')
                            {
                                this.buf.Append((char)octalNumber);
                                if (data == -1)
                                {
                                    this.sval = buf.ToString();
                                    state = StreamTokenizerSupport.STATE_DONE;
                                }
                                else
                                {
                                    this.unread(ch);
                                    state = StreamTokenizerSupport.STATE_STRING;
                                }
                            }
                            else
                            {
                                int temp = octalNumber * 8 + (ch - '0');
                                if (temp < 256)
                                    octalNumber = temp;
                                else
                                {
                                    buf.Append((char)octalNumber);
                                    buf.Append(ch);
                                    state = StreamTokenizerSupport.STATE_STRING;
                                }
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_POSSIBLEC_COMMENT:
                        {
                            if (ch == '*')
                                state = StreamTokenizerSupport.STATE_C_COMMENT;
                            else if (ch == '/')
                                state = StreamTokenizerSupport.STATE_LINECOMMENT;
                            else
                            {
                                if (data != -1)
                                    this.unread(ch);
                                this.ttype = '/';
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_C_COMMENT:
                        {
                            if (ch == '*')
                                state = StreamTokenizerSupport.STATE_POSSIBLEC_COMMENT_END;
                            if (ch == '\n')
                                this.lineno++;
                            else if (data == -1)
                            {
                                this.ttype = StreamTokenizerSupport.TT_EOF;
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            break;
                        }
                    case StreamTokenizerSupport.STATE_POSSIBLEC_COMMENT_END:
                        {
                            if (data == -1)
                            {
                                this.ttype = StreamTokenizerSupport.TT_EOF;
                                state = StreamTokenizerSupport.STATE_DONE;
                            }
                            else if (ch == '/')
                                state = StreamTokenizerSupport.STATE_NEUTRAL;
                            else if (ch != '*')
                                state = StreamTokenizerSupport.STATE_C_COMMENT;
                            break;
                        }
                }
            }
            while (state != StreamTokenizerSupport.STATE_DONE);
            if (this.ttype == StreamTokenizerSupport.TT_WORD && this.lowerCaseMode)
                this.sval = this.sval.ToLower();
            return this.ttype;
        }
        public virtual void OrdinaryChar(int ch)
        {
            if (ch >= 0 && ch <= 255)
                this.attribute[ch] = StreamTokenizerSupport.ORDINARYCHAR;
        }
        public virtual void OrdinaryChars(int low, int hi)
        {
            this.setAttributes(low, hi, StreamTokenizerSupport.ORDINARYCHAR);
        }
        public virtual void ParseNumbers()
        {
            for (int i = '0'; i <= '9'; i++)
                this.attribute[i] = StreamTokenizerSupport.NUMBERCHAR;
            this.attribute['.'] = StreamTokenizerSupport.NUMBERCHAR;
            this.attribute['-'] = StreamTokenizerSupport.NUMBERCHAR;
        }
        public virtual void PushBack()
        {
            if (this.ttype != StreamTokenizerSupport.TT_NOTHING)
                this.pushedback = true;
        }
        public virtual void QuoteChar(int ch)
        {
            if (ch >= 0 && ch <= 255)
                this.attribute[ch] = QUOTECHAR;
        }
        public virtual void ResetSyntax()
        {
            this.OrdinaryChars(0x00, 0xff);
        }
        public virtual void SlashSlashComments(bool flag)
        {
            this.slashSlashComments = flag;
        }
        public virtual void SlashStarComments(bool flag)
        {
            this.slashStarComments = flag;
        }
        public override System.String ToString()
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder(StreamTokenizerSupport.TOKEN);
            switch (this.ttype)
            {
                case StreamTokenizerSupport.TT_NOTHING:
                    {
                        buffer.Append(StreamTokenizerSupport.NOTHING);
                        break;
                    }
                case StreamTokenizerSupport.TT_WORD:
                    {
                        buffer.Append(this.sval);
                        break;
                    }
                case StreamTokenizerSupport.TT_NUMBER:
                    {
                        buffer.Append(StreamTokenizerSupport.NUMBER);
                        buffer.Append(this.nval);
                        break;
                    }
                case StreamTokenizerSupport.TT_EOF:
                    {
                        buffer.Append(StreamTokenizerSupport.EOF);
                        break;
                    }
                case StreamTokenizerSupport.TT_EOL:
                    {
                        buffer.Append(StreamTokenizerSupport.EOL);
                        break;
                    }
            }
            if (this.ttype > 0)
            {
                if (this.attribute[this.ttype] == StreamTokenizerSupport.QUOTECHAR)
                {
                    buffer.Append(StreamTokenizerSupport.QUOTED);
                    buffer.Append(this.sval);
                }
                else
                {
                    buffer.Append('\'');
                    buffer.Append((char)this.ttype);
                    buffer.Append('\'');
                }
            }
            buffer.Append(StreamTokenizerSupport.LINE);
            buffer.Append(this.lineno);
            return buffer.ToString();
        }
        public virtual void WhitespaceChars(int low, int hi)
        {
            this.setAttributes(low, hi, StreamTokenizerSupport.WHITESPACECHAR);
        }
        public virtual void WordChars(int low, int hi)
        {
            this.setAttributes(low, hi, StreamTokenizerSupport.WORDCHAR);
        }
    }
    internal class BackReader : System.IO.StreamReader
    {
        private char[] buffer;
        private int position = 1;
        public BackReader(System.IO.Stream streamReader, int size, System.Text.Encoding encoding)
            : base(streamReader, encoding)
        {
            this.buffer = new char[size];
            this.position = size;
        }
        public BackReader(System.IO.Stream streamReader, System.Text.Encoding encoding)
            : base(streamReader, encoding)
        {
            this.buffer = new char[this.position];
        }
        public bool MarkSupported()
        {
            return false;
        }
        public void Mark(int position)
        {
            throw new System.IO.IOException("Mark operations are not allowed");
        }
        public void Reset()
        {
            throw new System.IO.IOException("Mark operations are not allowed");
        }
        public override int Read()
        {
            if (this.position >= 0 && this.position < this.buffer.Length)
                return (int)this.buffer[this.position++];
            return base.Read();
        }
        public override int Read(char[] array, int index, int count)
        {
            int readLimit = this.buffer.Length - this.position;
            if (count <= 0)
                return 0;
            if (readLimit > 0)
            {
                if (count < readLimit)
                    readLimit = count;
                System.Array.Copy(this.buffer, this.position, array, index, readLimit);
                count -= readLimit;
                index += readLimit;
                this.position += readLimit;
            }
            if (count > 0)
            {
                count = base.Read(array, index, count);
                if (count == -1)
                {
                    if (readLimit == 0)
                        return -1;
                    return readLimit;
                }
                return readLimit + count;
            }
            return readLimit;
        }
        public bool IsReady()
        {
            return (this.position >= this.buffer.Length || this.BaseStream.Position >= this.BaseStream.Length);
        }
        public void UnRead(int unReadChar)
        {
            this.position--;
            this.buffer[this.position] = (char)unReadChar;
        }
        public void UnRead(char[] array, int index, int count)
        {
            this.Move(array, index, count);
        }
        public void UnRead(char[] array)
        {
            this.Move(array, 0, array.Length - 1);
        }
        private void Move(char[] array, int index, int count)
        {
            for (int arrayPosition = index + count; arrayPosition >= index; arrayPosition--)
                this.UnRead(array[arrayPosition]);
        }
    }
    internal class BackInputStream : System.IO.BinaryReader
    {
        private byte[] buffer;
        private int position = 1;
        public BackInputStream(System.IO.Stream streamReader, System.Int32 size)
            : base(streamReader)
        {
            this.buffer = new byte[size];
            this.position = size;
        }
        public BackInputStream(System.IO.Stream streamReader)
            : base(streamReader)
        {
            this.buffer = new byte[this.position];
        }
        public bool MarkSupported()
        {
            return false;
        }
        public override int Read()
        {
            if (position >= 0 && position < buffer.Length)
                return (int)this.buffer[position++];
            return base.Read();
        }
        public virtual int Read(sbyte[] array, int index, int count)
        {
            int byteCount = 0;
            int readLimit = count + index;
            byte[] aux = ToByteArray(array);
            for (byteCount = 0; position < buffer.Length && index < readLimit; byteCount++)
                aux[index++] = buffer[position++];
            if (index < readLimit)
                byteCount += base.Read(aux, index, readLimit - index);
            for (int i = 0; i < aux.Length; i++)
                array[i] = (sbyte)aux[i];
            return byteCount;
        }
        public void UnRead(int element)
        {
            this.position--;
            if (position >= 0)
                this.buffer[this.position] = (byte)element;
        }
        public void UnRead(byte[] array, int index, int count)
        {
            this.Move(array, index, count);
        }
        public void UnRead(byte[] array)
        {
            this.Move(array, 0, array.Length - 1);
        }
        public long Skip(long numberOfBytes)
        {
            return this.BaseStream.Seek(numberOfBytes, System.IO.SeekOrigin.Current) - this.BaseStream.Position;
        }
        private void Move(byte[] array, int index, int count)
        {
            for (int arrayPosition = index + count; arrayPosition >= index; arrayPosition--)
                this.UnRead(array[arrayPosition]);
        }
    }
}
