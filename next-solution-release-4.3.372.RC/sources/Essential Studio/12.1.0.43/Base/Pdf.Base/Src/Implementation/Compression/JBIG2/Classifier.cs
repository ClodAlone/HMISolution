#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Pdf.Compression
{
    class Pixa
    {
        private int m_n;
        private int m_nalloc;
        private int m_refCount;
        private List<Pix> m_pix;
        private Boxa m_boxa;

        internal int N            /* number of Pix in ptr array        */
        { get { return m_n; } set { m_n = value; } }
        internal int Nalloc       /* number of Pix ptrs allocated      */
        { get { return m_nalloc; } set { m_nalloc = value; } }
        internal int RefCount     /* reference count (1 if no clones)  */
        { get { return m_refCount; } set { m_refCount = value; } }
        internal List<Pix> Pix          /* the array of ptrs to pix          */
        { get { return m_pix; } set { m_pix = value; } }
        internal Boxa Boxa         /* array of boxes                    */
        { get { return m_boxa; } set { m_boxa = value; } }

        internal Pixa(int n)
        {
            Nalloc = n;
            N = 0;
            RefCount = 1;
            Pix = new List<Pix>();
        }
    }

    class Pix
    {
        private int m_w;
        private int m_h;
        private int m_d;
        private int m_wpl;
        private int m_xRes;
        private int m_yRes;
        private int m_informat;
        private char m_text;
        private PixColormap m_colormap;
        private uint[] m_data;

        internal int W           /* width in pixels                   */
        { get { return m_w; } set { m_w = value; } }
        internal int H           /* height in pixels                  */
        { get { return m_h; } set { m_h = value; } }
        internal int D           /* depth in bits                     */
        { get { return m_d; } set { m_d = value; } }
        internal int Wpl         /* 32-bit words/line                 */
        { get { return m_wpl; } set { m_wpl = value; } }
        internal int XRes        /* image res (ppi) in x direction    */
        { get { return m_xRes; } set { m_xRes = value; } }
        /* (use 0 if unknown)                */
        internal int YRes        /* image res (ppi) in y direction    */
        { get { return m_yRes; } set { m_yRes = value; } }
        /* (use 0 if unknown)                */
        internal int Informat    /* input file format, IFF_*          */
        { get { return m_informat; } set { m_informat = value; } }
        internal char Text        /* text string associated with pix   */
        { get { return m_text; } set { m_text = value; } }
        internal PixColormap Colormap    /* colormap (may be null)            */
        { get { return m_colormap; } set { m_colormap = value; } }
        internal uint[] Data        /* the image data                    */
        { get { return m_data; } set { m_data = value; } }
    }

    class PixColormap
    {
        private RGBA_Quad[] m_array;
        private int m_depth;
        private int m_nalloc;
        private int m_n;

        internal RGBA_Quad[] Array     /* colormap table (array of RGBA_QUAD)     */
        { get { return m_array; } set { m_array = value; } }
        internal int Depth     /* of pix (1, 2, 4 or 8 bpp)               */
        { get { return m_depth; } set { m_depth = value; } }
        internal int Nalloc    /* number of color entries allocated       */
        { get { return m_nalloc; } set { m_nalloc = value; } }
        internal int N         /* number of color entries used            */
        { get { return m_n; } set { m_n = value; } }
    }

    class Boxa
    {
        private int m_n;
        private int m_nalloc;
        private uint m_refCount;
        private List<Box> m_box;

        internal int N             /* number of box in ptr array        */
        { get { return m_n; } set { m_n = value; } }
        internal int Nalloc        /* number of box ptrs allocated      */
        { get { return m_nalloc; } set { m_nalloc = value; } }
        internal uint RefCount      /* reference count (1 if no clones)  */
        { get { return m_refCount; } set { m_refCount = value; } }
        internal List<Box> Box           /* box ptr array                     */
        { get { return m_box; } set { m_box = value; } }

        internal Boxa(int n)
        {
            Nalloc = n;
            N = 0;
            RefCount = 1;
            Box = new List<Box>();
        }
    }

    class Box
    {
        private int m_x;
        private int m_y;
        private int m_w;
        private int m_h;
        private uint m_refCount;

        internal int X
        { get { return m_x; } set { m_x = value; } }
        internal int Y
        { get { return m_y; } set { m_y = value; } }
        internal int W
        { get { return m_w; } set { m_w = value; } }
        internal int H
        { get { return m_h; } set { m_h = value; } }
        internal uint RefCount      /* reference count (1 if no clones)  */
        { get { return m_refCount; } set { m_refCount = value; } }

        internal Box()
        {
        }

        internal Box(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }

    struct JBIG2FileHeader
    {
        private byte[] m_id;
        private byte m_organisationType;
        private byte m_unknownNPages;
        private byte m_reserved;
        private uint m_nPages;

        internal byte[] Id
        { get { return m_id; } set { m_id = value; } }
        internal byte OrganisationType
        { get { return m_organisationType; } set { m_organisationType = value; } }
        internal byte UnknownNPages
        { get { return m_unknownNPages; } set { m_unknownNPages = value; } }
        internal byte Reserved
        { get { return m_reserved; } set { m_reserved = value; } }
        internal uint NPages
        { get { return m_nPages; } set { m_nPages = value; } }
    }

    class JBIG2EncoderContext
    {
        const int JBIG2_MAX_CTX = 65536;
        const int JBIG2_OUTPUTBUFFER_SIZE = 20 * 1024;

        private int m_c;
        private int m_a;
        private int m_ct;
        private byte m_b;
        private int m_bp;
        private Dictionary<int, byte[]> m_outputChunks;
        private byte[] m_outbuf;
        private int m_outbufUsed;
        private List<int> m_context;
        private Dictionary<int, List<int>> m_indexContext;
        private List<int> m_iaidctx;

        // these are the current state of the arithmetic coder
        internal int C
        { get { return m_c; } set { m_c = value; } }
        internal int A
        { get { return m_a; } set { m_a = value; } }
        internal int CT
        { get { return m_ct; } set { m_ct = value; } }
        internal byte B
        { get { return m_b; } set { m_b = value; } }
        internal int BP
        { get { return m_bp; } set { m_bp = value; } }

        // This is a list of output chunks, not including the current one
        internal Dictionary<int, byte[]> OutputChunks
        { get { return m_outputChunks; } set { m_outputChunks = value; } }
        internal byte[] Outbuf  // this is the current output chunk
        { get { return m_outbuf; } set { m_outbuf = value; } }
        internal int OutbufUsed  // number of bytes used in outbuf
        { get { return m_outbufUsed; } set { m_outbufUsed = value; } }
        internal List<int> Context  // state machine context for encoding images
        { get { return m_context; } set { m_context = value; } }
        internal Dictionary<int, List<int>> IndexContext  // 512 bytes of context indexes for each of 13 different int decodings
        { get { return m_indexContext; } set { m_indexContext = value; } }
        // this data is also used for refinement coding
        internal List<int> Iaidctx  // size of this context not known at construction time
        { get { return m_iaidctx; } set { m_iaidctx = value; } }

        internal JBIG2EncoderContext()
        {
            IndexContext = new Dictionary<int, List<int>>(13);//, 512];
            for (int i = 0; i < 13; i++)
                IndexContext[i] = new List<int>(512);
            Context = new List<int>(JBIG2_MAX_CTX); //new short[JBIG2_MAX_CTX];
            A = 0x8000;
            C = 0;
            CT = 12;
            BP = -1;
            B = 0;
            OutbufUsed = 0;
            Outbuf = new byte[JBIG2_OUTPUTBUFFER_SIZE];
            OutputChunks = new Dictionary<int, byte[]>();
            Iaidctx = new List<int>();
        }
    }

    class JBIG2Classifier
    {
        private int m_method;
        private int m_components;
        private int m_maxWidth;
        private int m_maxHeight;
        private int m_nPages;
        private int m_baseIndex;
        private Numa m_naComps;
        private int m_sizeHaus;
        private float m_rankHaus;
        private float m_thresh;
        private float m_weightFactor;
        private Numa m_naArea;
        private int m_w;
        private int m_h;
        private int m_nClass;
        private int m_keepPixaa;
        private Pixaa m_pixaa;
        private Pixa m_pixat;
        private Pixa m_pixatd;
        private NumaHash m_naHash;
        private Numa m_nafgt;
        private Pta m_ptac;
        private Pta m_ptaTemplate;
        private  Numa m_naClass;
        private Numa m_naPage;
        private Pta m_ptaUL;
        private Pta m_ptaLL;

        internal int Method       /* JB_RANKHAUS, JB_CORRELATION            */
        { get { return m_method; } set { m_method = value; } }
        internal int Components   /* JB_CONN_COMPS, JB_CHARACTERS or        */
        { get { return m_components; } set { m_components = value; } }
        /* JB_WORDS                               */
        internal int MaxWidth     /* max component width allowed            */
        { get { return m_maxWidth; } set { m_maxWidth = value; } }
        internal int MaxHeight    /* max component height allowed           */
        { get { return m_maxHeight; } set { m_maxHeight = value; } }
        internal int NPages       /* number of pages already processed      */
        { get { return m_nPages; } set { m_nPages = value; } }
        internal int BaseIndex    /* number of components already processed */
        { get { return m_baseIndex; } set { m_baseIndex = value; } }
        /* on fully processed pages               */
        internal Numa NaComps      /* number of components on each page      */
        { get { return m_naComps; } set { m_naComps = value; } }
        internal int SizeHaus     /* size of square struct element for haus */
        { get { return m_sizeHaus; } set { m_sizeHaus = value; } }
        internal float RankHaus     /* rank val of haus match, each way       */
        { get { return m_rankHaus; } set { m_rankHaus = value; } }
        internal float Thresh       /* thresh value for correlation score     */
        { get { return m_thresh; } set { m_thresh = value; } }
        internal float WeightFactor /* corrects thresh value for heaver       */
        { get { return m_weightFactor; } set { m_weightFactor = value; } }
        internal Numa NaArea       /* w * h of each template, without extra  */
        { get { return m_naArea; } set { m_naArea = value; } }
        internal int W            /* max width of original src images       */
        { get { return m_w; } set { m_w = value; } }
        internal int H            /* max height of original src images      */
        { get { return m_h; } set { m_h = value; } }
        internal int NClass       /* current number of classes              */
        { get { return m_nClass; } set { m_nClass = value; } }
        internal int KeepPixaa   /* If zero, pixaa isn't filled            */
        { get { return m_keepPixaa; } set { m_keepPixaa = value; } }
        internal Pixaa Pixaa        /* instances for each class; unbordered   */
        { get { return m_pixaa; } set { m_pixaa = value; } }
        internal Pixa Pixat        /* templates for each class; bordered     */
        { get { return m_pixat; } set { m_pixat = value; } }
        /* and not dilated                        */
        internal Pixa Pixatd       /* templates for each class; bordered     */
        { get { return m_pixatd; } set { m_pixatd = value; } }
        /* and dilated                            */
        internal NumaHash NaHash       /* Hash table to find templates by size   */
        { get { return m_naHash; } set { m_naHash = value; } }
        internal Numa Nafgt        /* fg areas of undilated templates;       */
        { get { return m_nafgt; } set { m_nafgt = value; } }
        /* only used for rank < 1.0               */
        internal Pta Ptac         /* centroids of all bordered cc           */
        { get { return m_ptac; } set { m_ptac = value; } }
        internal Pta PtaTemplate        /* centroids of all bordered template cc  */
        { get { return m_ptaTemplate; } set { m_ptaTemplate = value; } }
        internal Numa NaClass      /* array of class ids for each component  */
        { get { return m_naClass; } set { m_naClass = value; } }
        internal Numa NaPage       /* array of page nums for each component  */
        { get { return m_naPage; } set { m_naPage = value; } }
        internal Pta PtaUL        /* array of UL corners at which the       */
        { get { return m_ptaUL; } set { m_ptaUL = value; } }
        /* template is to be placed for each      */
        /* component                              */
        internal Pta PtaLL        /* similar to ptaul, but for LL corners   */
        { get { return m_ptaLL; } set { m_ptaLL = value; } }

        internal JBIG2Classifier(int method, int components, Numa nacomps, Pixaa pixaa, Pixa pixat, Pixa pixatd, Numa nafgt, Numa naarea, Pta ptac, Pta ptact, Numa naclass, Numa napage, Pta ptaul)
        {
            Method = method;
            Components = components;
            NaComps = nacomps;
            Pixaa = pixaa;
            Pixat = pixat;
            Pixatd = pixatd;
            Nafgt = nafgt;
            NaArea = naarea;
            Ptac = ptac;
            PtaTemplate = ptact;
            NaClass = naclass;
            NaPage = napage;
            PtaUL = ptaul;
        }
    }

    class Segment
    {
        private uint m_length;
        private uint m_number;
        private int m_sType;
        private int deferred_non_retain;  // see JBIG2 spec
        private int m_retainBits;
        private List<int> m_referredTo;
        private uint m_page;

        internal uint Number  // segment number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }
        internal int SType  // segment type (see enum in jbig2structs.h)
        {
            get
            {
                return m_sType;
            }
            set
            {
                m_sType = value;
            }
        }
        internal int RetainBits
        {
            get
            {
                return m_retainBits;
            }
            set
            {
                m_retainBits = value;
            }
        }
        internal List<int> ReferredTo  // list of segment numbers referred to
        {
            get
            {
                return m_referredTo;
            }
            set
            {
                m_referredTo = value;
            }
        }
        internal uint Page  // page number
        {
            get
            {
                return m_page;
            }
            set
            {
                m_page = value;
            }
        }
        internal uint Length   // length of trailing data
        {
            get
            {
                return m_length;
            }
            set
            {
                m_length = value;
            }
        }

        internal Segment()
        {
            ReferredTo = new List<int>();
        }
        
        private int ReferenceSize
        {
            get
            {
                int refsize;
                if (Number <= 256)
                    refsize = 1;
                else if (Number <= 65536)
                    refsize = 2;
                else
                    refsize = 4;

                return refsize;
            }
        }

        private int PageSize
        {
            get
            {
                return Page <= 255 ? 1 : 4;
            }
        }

        private uint Size
        {
            get
            {
                int refsize = ReferenceSize;
                int pagesize = PageSize;

                return 0;// sizeof(JBIG2Segment) + refsize * referred_to.Count + pagesize + sizeof(int);
            }
        }

        internal void Write(List<byte> buf)
        {
            JBIG2Segment s = new JBIG2Segment();
            s.Number = JBIG2Statics.Htonl((object)Number);
            s.SType = (byte)SType;
            s.DeferredNonRetain = (byte)deferred_non_retain;
            s.RetainBits = (byte)RetainBits;
            s.SegmentCount = (byte)ReferredTo.Count;

            int pagesize = PageSize;
            int refsize = ReferenceSize;

            if (pagesize == 4)
                s.PageAssocSize = 1;

            buf.AddRange(s.Serialize());

            foreach (uint i in ReferredTo)
            {
                if (refsize == 4)
                    buf.AddRange(JBIG2Statics.Htonl(i));
                else if (refsize == 2)
                    buf.Add((byte)JBIG2Statics.Htonl((object)i));//implement htons
                else
                    buf.Add((byte)i);
            }

            if (pagesize == 4)
                buf.AddRange(JBIG2Statics.Htonl(Page));
            else
                buf.Add((byte)Page);
            //if (j != size()) 
            //    return;

            buf.AddRange(JBIG2Statics.Htonl(Length));
        }
    }

    struct JBIG2SymbolDict
    {
        byte sdhuff;
        byte sdrefagg;
        byte sdhuffdh;
        byte sdhuffdw;
        byte sdhuffbmsize;
        byte sdhuffagginst;
        byte bmcontext;
        byte bmcontextretained;
        byte reserved;

        private byte m_sdtemplate;
        private byte m_sdrtemplate;
        private sbyte m_a1x;
        private sbyte m_a1y;
        private sbyte m_a2x;
        private sbyte m_a2y;
        private sbyte m_a3x;
        private sbyte m_a3y;
        private sbyte m_a4x;
        private sbyte m_a4y;
        private uint m_exSyms;
        private uint m_newSyms;

        internal byte sdtemplate
        { get { return m_sdtemplate; } set { m_sdtemplate = value; } }
        internal byte sdrtemplate
        { get { return m_sdrtemplate; } set { m_sdrtemplate = value; } }
        internal sbyte a1x
        { get { return m_a1x; } set { m_a1x = value; } }
        internal sbyte a1y
        { get { return m_a1y; } set { m_a1y = value; } }
        internal sbyte a2x
        { get { return m_a2x; } set { m_a2x = value; } }
        internal sbyte a2y
        { get { return m_a2y; } set { m_a2y = value; } }
        internal sbyte a3x
        { get { return m_a3x; } set { m_a3x = value; } }
        internal sbyte a3y
        { get { return m_a3y; } set { m_a3y = value; } }
        internal sbyte a4x
        { get { return m_a4x; } set { m_a4x = value; } }
        internal sbyte a4y
        { get { return m_a4y; } set { m_a4y = value; } }
        internal uint ExSyms
        { get { return m_exSyms; } set { m_exSyms = value; } }
        internal uint NewSyms
        { get { return m_newSyms; } set { m_newSyms = value; } }

        //public JBIG2SymbolDict()
        //{
        //    sdhuff = sdrefagg = 0;// 1;
        //    sdhuffdh = sdhuffdw = 0;// 2;
        //    sdhuffbmsize = sdhuffagginst = bmcontext = bmcontextretained = 0;// 1;
        //    sdtemplate = 0;// 2;
        //    sdrtemplate = 0;// 1;
        //    reserved = 0;// 3;
        //}
    }

    struct JBIG2Segment
    {
        private uint m_number;
        private byte m_deferredNonRetain;
        private byte m_pageAssocSize;
        private byte m_sType;
        private byte m_segmentCount;
        private byte m_retainBits;

        internal uint Number
        { get { return m_number; } set { m_number = value; } }
        internal byte DeferredNonRetain //1
        { get { return m_deferredNonRetain; } set { m_deferredNonRetain = value; } }
        internal byte PageAssocSize //1
        { get { return m_pageAssocSize; } set { m_pageAssocSize = value; } }
        internal byte SType //6
        { get { return m_sType; } set { m_sType = value; } }
        internal byte SegmentCount //5
        { get { return m_segmentCount; } set { m_segmentCount = value; } }
        internal byte RetainBits //3
        { get { return m_retainBits; } set { m_retainBits = value; } }

        internal byte[] Serialize()
        {
            byte[] memory = new byte[6];
            Array.Copy(BitConverter.GetBytes(Number), memory, 4);

            string deferrBits = Convert.ToString(DeferredNonRetain, 2);
            string pageBits = Convert.ToString(PageAssocSize, 2);
            string type = Convert.ToString(SType, 2);

            while (type.Length < 6)
                type = string.Concat("0" + type);

            string cont = string.Concat(deferrBits, pageBits, type);

            int[] bits = new int[cont.Length];
            int k = 0;
            foreach (char c in cont.ToCharArray())
            {
                bits[k] = int.Parse(cont[k].ToString());
                k++;
            }

            byte[] check = ToByteArray(bits);
            memory[4] = check[0];

            string segBits = Convert.ToString(SegmentCount, 2);
            while (segBits.Length < 3)
            {
                segBits = string.Concat("0" + segBits);
            }
            string reBits = Convert.ToString(RetainBits, 2);
            while (reBits.Length < 5)
            {
                reBits = string.Concat("0" + reBits);
            }

            string cont1 = string.Concat(segBits, reBits);

            bits = new int[cont1.Length];
            k = 0;
            foreach (char c in cont1.ToCharArray())
            {
                bits[k] = int.Parse(cont1[k].ToString());
                k++;
            }
            check = ToByteArray(bits);
            memory[5] = check[0];

            return memory;
        }

        byte[] ToByteArray(int[] bits)
        {
            int numBytes = bits.Length / 8;

            if (bits.Length % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];

            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] != 0)
                    bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }
    }

    class Sarray
    {
        private int m_nalloc;
        private int m_n;
        private List<string> m_array;

        internal int Nalloc    /* size of allocated ptr array         */
        { get { return m_nalloc; } set { m_nalloc = value; } }
        internal int N         /* number of strings allocated         */
        { get { return m_n; } set { m_n = value; } }
        int refcount;  /* reference count (1 if no clones)    */
        internal List<string> Array     /* string array                        */
        { get { return m_array; } set { m_array = value; } }

        internal Sarray(int n)
        {
            Nalloc = n;
            N = 0;
            refcount = 1;
        }
    }

    class IntEncRange
    {
        private int m_bot;
        private int m_top;
        private int m_data;
        private int m_bits;
        private short m_delta;
        private int m_intBits;

        internal int Bot
        { get { return m_bot; } set { m_bot = value; } }
        internal int Top  // the range of numbers for which this is valid
        { get { return m_top; } set { m_top = value; } }
        internal int Data
        { get { return m_data; } set { m_data = value; } }
        internal int Bits // the bits of data to write first, and the number which are valid
        { get { return m_bits; } set { m_bits = value; } }
        // These bits are taken from the bottom of the u8, in reverse order
        internal short Delta     // the amount to subtract from the value before encoding it
        { get { return m_delta; } set { m_delta = value; } }
        internal int IntBits    // number of bits to use to encode the integer
        { get { return m_intBits; } set { m_intBits = value; } }

        internal IntEncRange(int bot, int top, int data, int bits, short delta, int intbits)
        {
            Bot = bot;
            Top = top;
            Data = data;
            Bits = bits;
            Delta = delta;
            IntBits = intbits;
        }
    }

    class Context
    {
        private short m_qe;
        private char m_mps;
        private char m_lps;

        internal short Qe
        { get { return m_qe; } set { m_qe = value; } }
        internal char Mps
        { get { return m_mps; } set { m_mps = value; } }
        internal char Lps
        { get { return m_lps; } set { m_lps = value; } }

        internal Context(short qe, char mps, char lps)
        {
            Qe = qe;
            Mps = mps;
            Lps = lps;
        }
    }

    //struct tiff_transform
    //{
    //    internal int vflip    /* if non-zero, image needs a vertical fip */
    //    { get; set; }
    //    internal int hflip    /* if non-zero, image needs a horizontal flip */
    //    { get; set; }
    //    internal int rotate
    //    { get; set; }         /* -1 -> counterclockwise 90-degree rotation, 0 -> no rotation 1 -> clockwise 90-degree rotation */
    //}

    struct JBIG2TextRegionAtFlags
    {
        private int m_a1x;
        private int m_a1y;
        private int m_a2x;
        private int m_a2y;

        internal int a1x
        { get { return m_a1x; } set { m_a1x = value; } }
        internal int a1y
        { get { return m_a1y; } set { m_a1y = value; } }
        internal int a2x
        { get { return m_a2x; } set { m_a2x = value; } }
        internal int a2y
        { get { return m_a2y; } set { m_a2y = value; } }
    }

    struct JBIG2TextRegionSym
    {
        private uint m_sbNumInstances;

        internal uint SBNumInstances
        { get { return m_sbNumInstances; } set { m_sbNumInstances = value; } }
    }

    struct JBIG2PageInfo
    {
        byte reserved;
        byte operator_override;
        byte aux_buffers;
        byte default_operator;
        ushort segment_flags;
        private uint m_width;
        private uint m_height;
        private uint m_xRes;
        private uint m_yRes;
        private byte m_defaultPixel;
        private byte m_containsRefinements;
        private byte m_isLossless;

        internal uint Width
        { get { return m_width; } set { m_width = value; } }
        internal uint Height
        { get { return m_height; } set { m_height = value; } }
        internal uint XRes
        { get { return m_xRes; } set { m_xRes = value; } }
        internal uint YRes
        { get { return m_yRes; } set { m_yRes = value; } }

        internal byte DefaultPixel
        { get { return m_defaultPixel; } set { m_defaultPixel = value; } }
        internal byte ContainsRefinements
        { get { return m_containsRefinements; } set { m_containsRefinements = value; } }
        internal byte IsLossless
        { get { return m_isLossless; } set { m_isLossless = value; } }
    }

    struct JBIG2TextRegion
    {
        byte comb_operator;

        private uint m_width;
        private uint m_height;
        private uint m_x;
        private uint m_y;
        private byte m_sbcombop2;
        private byte m_sbdefpixel;
        private byte m_sbdsoffset;
        private byte m_sbrtemplate;
        private byte m_sbhuff;
        private byte m_sbRefine;
        private byte m_logSBStrips;
        private byte m_refcorner;
        private byte m_transposed;
        private byte m_sbcombop1;

        internal uint Width
        { get { return m_width; } set { m_width = value; } }
        internal uint Height
        { get { return m_height; } set { m_height = value; } }
        internal uint X
        { get { return m_x; } set { m_x = value; } }
        internal uint Y
        { get { return m_y; } set { m_y = value; } }

        byte sbcombop2
        { get { return m_sbcombop2; } set { m_sbcombop2 = value; } }
        byte sbdefpixel
        { get { return m_sbdefpixel; } set { m_sbdefpixel = value; } }
        byte sbdsoffset
        { get { return m_sbdsoffset; } set { m_sbdsoffset = value; } }
        internal byte Sbrtemplate
        { get { return m_sbrtemplate; } set { m_sbrtemplate = value; } }
        byte sbhuff
        { get { return m_sbhuff; } set { m_sbhuff = value; } }
        internal byte SBRefine
        { get { return m_sbRefine; } set { m_sbRefine = value; } }
        internal byte LogSBStrips
        { get { return m_logSBStrips; } set { m_logSBStrips = value; } }
        byte refcorner
        { get { return m_refcorner; } set { m_refcorner = value; } }
        byte transposed
        { get { return m_transposed; } set { m_transposed = value; } }
        byte sbcombop1
        { get { return m_sbcombop1; } set { m_sbcombop1 = value; } }
    }

    class Numa
    {
        private int m_nalloc;
        private int m_n;
        private int m_refCount;
        private List<float> m_array;

        internal int Nalloc    /* size of allocated number array      */
        { get { return m_nalloc; } set { m_nalloc = value; } }
        internal int N         /* number of numbers saved             */
        { get { return m_n; } set { m_n = value; } }
        internal int RefCount  /* reference count (1 if no clones)    */
        { get { return m_refCount; } set { m_refCount = value; } }
        float startx;    /* x value assigned to array[0]        */
        float delx;      /* change in x value as i --> i + 1    */
        internal List<float> Array     /* number array                        */
        { get { return m_array; } set { m_array = value; } }

        public Numa(int n)
        {
            Nalloc = n;
            N = 0;
            RefCount = 1;
            startx = 0.0f;
            delx = 1f;
            Array = new List<float>();
        }
    }

    class Pixaa
    {
        private int m_nalloc;
        private int m_n;
        private List<Pixa> m_pixa;
        private List<Boxa> m_boxa;

        internal int N            /* number of Pixa in ptr array       */
        { get { return m_n; } set { m_n = value; } }
        internal int Nalloc       /* number of Pixa ptrs allocated     */
        { get { return m_nalloc; } set { m_nalloc = value; } }
        internal List<Pixa> Pixa         /* array of ptrs to pixa             */
        { get { return m_pixa; } set { m_pixa = value; } }
        internal List<Boxa> Boxa         /* array of boxes                    */
        { get { return m_boxa; } set { m_boxa = value; } }

        internal Pixaa(int n)
        {
            this.Nalloc = n;
            this.N = 0;
            Pixa = new List<Pixa>();
            Boxa = new List<Boxa>();
        }
    }

    class NumaHash
    {
        private int m_nBuckets;
        private int m_initSize;
        private Dictionary<int, Numa> m_numa;

        internal int NBuckets
        { get { return m_nBuckets; } set { m_nBuckets = value; } }
        internal int InitSize   /* initial size of each numa that is made  */
        { get { return m_initSize; } set { m_initSize = value; } }
        internal Dictionary<int, Numa> Numa
        { get { return m_numa; } set { m_numa = value; } }

        internal NumaHash(int nbuckets, int initsize)
        {
            this.NBuckets = nbuckets;
            this.InitSize = initsize;
            Numa = new Dictionary<int, Numa>();
        }
    }

    class Pta
    {
        private int m_n;
        private int m_nalloc;
        private int m_refCount;
        private List<float> m_x;
        private List<float> m_y;

        internal int N             /* actual number of pts              */
        { get { return m_n; } set { m_n = value; } }
        internal int Nalloc        /* size of allocated arrays          */
        { get { return m_nalloc; } set { m_nalloc = value; } }
        internal int RefCount      /* reference count (1 if no clones)  */
        { get { return m_refCount; } set { m_refCount = value; } }
        internal List<float> X         /* arrays of floats                  */
        { get { return m_x; } set { m_x = value; } }
        internal List<float> Y
        { get { return m_y; } set { m_y = value; } }

        internal Pta(int n)
        {
            Nalloc = n;
            N = 0;
            X = new List<float>();
            Y = new List<float>();
        }
    }

    class RGBA_Quad
    {
        private int m_blue;
        private int m_green;
        private int m_red;

        internal int Blue
        { get { return m_blue; } set { m_blue = value; } }
        internal int Green
        { get { return m_green; } set { m_green = value; } }
        internal int Red
        { get { return m_red; } set { m_red = value; } }
    }

    class L_Stack
    {
        private int m_nalloc;
        private List<Object> m_array;
        private L_Stack m_auxStack;

        internal int Nalloc       /* size of ptr array              */
        { get { return m_nalloc; } set { m_nalloc = value; } }
        internal List<Object> Array        /* ptr array                      */
        { get { return m_array; } set { m_array = value; } }
        internal L_Stack AuxStack     /* auxiliary stack                */
        { get { return m_auxStack; } set { m_auxStack = value; } }

        internal L_Stack(int arg)
        {
            Nalloc = arg;
            Array = new List<object>();
        }
    }

    class FillSeg
    {
        private int m_xLeft;
        private int m_xRight;
        private int m_y;
        private int m_dy;

        internal int XLeft    /* left edge of run */
        { get { return m_xLeft; } set { m_xLeft = value; } }
        internal int XRight   /* right edge of run */
        { get { return m_xRight; } set { m_xRight = value; } }
        internal int Y        /* run y  */
        { get { return m_y; } set { m_y = value; } }
        internal int Dy       /* parent segment direction: 1 above, -1 below) */
        { get { return m_dy; } set { m_dy = value; } }
    }

    class Sel
    {
        private int m_sy;
        private int m_sx;
        private int m_cy;
        private int m_cx;
        private List<int[]> m_data;
        private string m_name;

        internal int SY          /* sel height                               */
        { get { return m_sy; } set { m_sy = value; } }
        internal int SX          /* sel width                                */
        { get { return m_sx; } set { m_sx = value; } }
        internal int CY          /* y location of sel origin                 */
        { get { return m_cy; } set { m_cy = value; } }
        internal int CX          /* x location of sel origin                 */
        { get { return m_cx; } set { m_cx = value; } }
        internal List<int[]> Data        /* {0,1,2}; data[i][j] in [row][col] order  */
        { get { return m_data; } set { m_data = value; } }
        internal string Name        /* used to find sel by name                 */
        { get { return m_name; } set { m_name = value; } }
    }

    class Pixacc
    {
        private int m_w;
        private int m_h;
        private uint m_offset;
        private Pix m_pix;

        internal int W            /* array width                       */
        { get { return m_w; } set { m_w = value; } }
        internal int H            /* array height                      */
        { get { return m_h; } set { m_h = value; } }
        internal uint Offset       /* used to allow negative            */
        { get { return m_offset; } set { m_offset = value; } }
        /* intermediate results              */
        internal Pix Pix          /* the 32 bit accumulator pix        */
        { get { return m_pix; } set { m_pix = value; } }
    }

    class FPix
    {
        private int m_w;
        private int m_h;
        private int m_wpl;
        private int m_refCount;
        private int m_xRes;
        private int m_yRes;
        private float[] m_data;

        internal int W           /* width in pixels                   */
        { get { return m_w; } set { m_w = value; } }
        internal int H           /* height in pixels                  */
        { get { return m_h; } set { m_h = value; } }
        internal int Wpl         /* 32-bit words/line                 */
        { get { return m_wpl; } set { m_wpl = value; } }
        internal int RefCount    /* reference count (1 if no clones)  */
        { get { return m_refCount; } set { m_refCount = value; } }
        internal int XRes      /* image res (ppi) in x direction    */
        { get { return m_xRes; } set { m_xRes = value; } }
        /* (use 0 if unknown)                */
        internal int YRes       /* image res (ppi) in y direction    */
        { get { return m_yRes; } set { m_yRes = value; } }
        /* (use 0 if unknown)                */
        internal float[] Data        /* the float image data              */
        { get { return m_data; } set { m_data = value; } }
    }

    class JbTemplatesState
    {
        private JBIG2Classifier m_classer;
        private int m_w;
        private int m_h;
        private int m_i;
        private Numa m_numa;
        private int m_n;

        internal JBIG2Classifier Classer    /* classer                               */
        { get { return m_classer; } set { m_classer = value; } }
        internal int W          /* desired width                         */
        { get { return m_w; } set { m_w = value; } }
        internal int H          /* desired height                        */
        { get { return m_h; } set { m_h = value; } }
        internal int I          /* index into two_by_two step array      */
        { get { return m_i; } set { m_i = value; } }
        internal Numa Numa       /* current number array                  */
        { get { return m_numa; } set { m_numa = value; } }
        internal int N          /* current element of numa               */
        { get { return m_n; } set { m_n = value; } }
    }

    static class ContextCollection
    {
        private static List<Context> m_stateTable;

        internal static List<Context> StateTable
        { get { return m_stateTable; } set { m_stateTable = value; } }

        static ContextCollection()
        {
            m_stateTable = new List<Context>();
            m_stateTable.Add(new Context(0x5601, F(1), Switch(F(1))));
            m_stateTable.Add(new Context(0x3401, F(2), F(6)));
            m_stateTable.Add(new Context(0x1801, F(3), F(9)));
            m_stateTable.Add(new Context(0x0ac1, F(4), F(12)));
            m_stateTable.Add(new Context(0x0521, F(5), F(29)));
            m_stateTable.Add(new Context(0x0221, F(38), F(33)));
            m_stateTable.Add(new Context(0x5601, F(7), Switch(F(6))));
            m_stateTable.Add(new Context(0x5401, F(8), F(14)));
            m_stateTable.Add(new Context(0x4801, F(9), F(14)));
            m_stateTable.Add(new Context(0x3801, F(10), F(14)));
            m_stateTable.Add(new Context(0x3001, F(11), F(17)));
            m_stateTable.Add(new Context(0x2401, F(12), F(18)));
            m_stateTable.Add(new Context(0x1c01, F(13), F(20)));
            m_stateTable.Add(new Context(0x1601, F(29), F(21)));
            m_stateTable.Add(new Context(0x5601, F(15), Switch(F(14))));
            m_stateTable.Add(new Context(0x5401, F(16), F(14)));
            m_stateTable.Add(new Context(0x5101, F(17), F(15)));
            m_stateTable.Add(new Context(0x4801, F(18), F(16)));
            m_stateTable.Add(new Context(0x3801, F(19), F(17)));
            m_stateTable.Add(new Context(0x3401, F(20), F(18)));
            m_stateTable.Add(new Context(0x3001, F(21), F(19)));
            m_stateTable.Add(new Context(0x2801, F(22), F(19)));
            m_stateTable.Add(new Context(0x2401, F(23), F(20)));
            m_stateTable.Add(new Context(0x2201, F(24), F(21)));
            m_stateTable.Add(new Context(0x1c01, F(25), F(22)));
            m_stateTable.Add(new Context(0x1801, F(26), F(23)));
            m_stateTable.Add(new Context(0x1601, F(27), F(24)));
            m_stateTable.Add(new Context(0x1401, F(28), F(25)));
            m_stateTable.Add(new Context(0x1201, F(29), F(26)));
            m_stateTable.Add(new Context(0x1101, F(30), F(27)));
            m_stateTable.Add(new Context(0x0ac1, F(31), F(28)));
            m_stateTable.Add(new Context(0x09c1, F(32), F(29)));
            m_stateTable.Add(new Context(0x08a1, F(33), F(30)));
            m_stateTable.Add(new Context(0x0521, F(34), F(31)));
            m_stateTable.Add(new Context(0x0441, F(35), F(32)));
            m_stateTable.Add(new Context(0x02a1, F(36), F(33)));
            m_stateTable.Add(new Context(0x0221, F(37), F(34)));
            m_stateTable.Add(new Context(0x0141, F(38), F(35)));
            m_stateTable.Add(new Context(0x0111, F(39), F(36)));
            m_stateTable.Add(new Context(0x0085, F(40), F(37)));
            m_stateTable.Add(new Context(0x0049, F(41), F(38)));
            m_stateTable.Add(new Context(0x0025, F(42), F(39)));
            m_stateTable.Add(new Context(0x0015, F(43), F(40)));
            m_stateTable.Add(new Context(0x0009, F(44), F(41)));
            m_stateTable.Add(new Context(0x0005, F(45), F(42)));
            m_stateTable.Add(new Context(0x0001, F(45), F(43)));

            m_stateTable.Add(new Context(0x5601, F(47), Switch(F(47))));
            m_stateTable.Add(new Context(0x3401, F(48), F(52)));
            m_stateTable.Add(new Context(0x1801, F(49), F(55)));
            m_stateTable.Add(new Context(0x0ac1, F(50), F(58)));
            m_stateTable.Add(new Context(0x0521, F(51), F(75)));
            m_stateTable.Add(new Context(0x0221, F(84), F(79)));
            m_stateTable.Add(new Context(0x5601, F(53), Switch(F(52))));
            m_stateTable.Add(new Context(0x5401, F(54), F(60)));
            m_stateTable.Add(new Context(0x4801, F(55), F(60)));
            m_stateTable.Add(new Context(0x3801, F(56), F(60)));
            m_stateTable.Add(new Context(0x3001, F(57), F(63)));
            m_stateTable.Add(new Context(0x2401, F(58), F(64)));
            m_stateTable.Add(new Context(0x1c01, F(59), F(66)));
            m_stateTable.Add(new Context(0x1601, F(75), F(67)));
            m_stateTable.Add(new Context(0x5601, F(61), Switch(F(60))));
            m_stateTable.Add(new Context(0x5401, F(62), F(60)));
            m_stateTable.Add(new Context(0x5101, F(63), F(61)));
            m_stateTable.Add(new Context(0x4801, F(64), F(62)));
            m_stateTable.Add(new Context(0x3801, F(65), F(63)));
            m_stateTable.Add(new Context(0x3401, F(66), F(64)));
            m_stateTable.Add(new Context(0x3001, F(67), F(65)));
            m_stateTable.Add(new Context(0x2801, F(68), F(65)));
            m_stateTable.Add(new Context(0x2401, F(69), F(66)));
            m_stateTable.Add(new Context(0x2201, F(70), F(67)));
            m_stateTable.Add(new Context(0x1c01, F(71), F(68)));
            m_stateTable.Add(new Context(0x1801, F(72), F(69)));
            m_stateTable.Add(new Context(0x1601, F(73), F(70)));
            m_stateTable.Add(new Context(0x1401, F(74), F(71)));
            m_stateTable.Add(new Context(0x1201, F(75), F(72)));
            m_stateTable.Add(new Context(0x1101, F(76), F(73)));
            m_stateTable.Add(new Context(0x0ac1, F(77), F(74)));
            m_stateTable.Add(new Context(0x09c1, F(78), F(75)));
            m_stateTable.Add(new Context(0x08a1, F(79), F(76)));
            m_stateTable.Add(new Context(0x0521, F(80), F(77)));
            m_stateTable.Add(new Context(0x0441, F(81), F(78)));
            m_stateTable.Add(new Context(0x02a1, F(82), F(79)));
            m_stateTable.Add(new Context(0x0221, F(83), F(80)));
            m_stateTable.Add(new Context(0x0141, F(84), F(81)));
            m_stateTable.Add(new Context(0x0111, F(85), F(82)));
            m_stateTable.Add(new Context(0x0085, F(86), F(83)));
            m_stateTable.Add(new Context(0x0049, F(87), F(84)));
            m_stateTable.Add(new Context(0x0025, F(88), F(85)));
            m_stateTable.Add(new Context(0x0015, F(89), F(86)));
            m_stateTable.Add(new Context(0x0009, F(90), F(87)));
            m_stateTable.Add(new Context(0x0005, F(91), F(88)));
            m_stateTable.Add(new Context(0x0001, F(91), F(89)));
        }

        private static char F(int x)
        {
            return (char)x;
        }

        private static char Switch(int x)
        {
            if (x <= 46)
                return (char)(x + 46);
            else
                return (char)(x - 46);
        }
    }
}