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
    internal class JBIG2Context
    {
        private JBIG2Classifier m_classifier;
        private int m_xRes;
        private int m_yRes;
        private bool m_fullHeaders;
        private bool m_pdfPageNumbering;
        private int m_segNumber;
        private int m_symbolTableSegment;
        private Dictionary<int, List<int>> m_pageComps;
        private Dictionary<int, List<int>> m_singleUseSymbols;
        private int m_numGlobalSymbols;
        private List<int> m_pageXRes;
        private List<int> m_pageYRes;
        private List<int> m_pageWidth;
        private List<int> m_pageHeight;
        private SortedDictionary<int, int> m_symbolMap;
        private bool m_refinement;
        private Pixa m_avgTemplates;
        private int m_refineLevel;
        private List<int> m_baseIndexes;

        internal JBIG2Classifier Classifier  // the leptonica classifier
        {
            get
            {
                return m_classifier;
            }
        }
        internal int XRes
        { get { return m_xRes; } set { m_xRes = value; } }
        internal int YRes  // ppi for the X and Y direction
        { get { return m_yRes; } set { m_yRes = value; } }
        internal bool FullHeaders  // true if we are producing a full JBIG2 file
        { get { return m_fullHeaders; } set { m_fullHeaders = value; } }
        internal bool PDFPageNumbering  // true if all text pages are page "1" (pdf mode)
        { get { return m_pdfPageNumbering; } set { m_pdfPageNumbering = value; } }
        internal int SegNumber  // current segment number
        { get { return m_segNumber; } set { m_segNumber = value; } }
        internal int SymbolTableSegment  // the segment number of the symbol table
        { get { return m_symbolTableSegment; } set { m_symbolTableSegment = value; } }
        // a map from page number a list of components for that page
        internal Dictionary<int, List<int>> PageComps
        { get { return m_pageComps; } set { m_pageComps = value; } }
        // for each page, the list of symbols which are only used on that page
        internal Dictionary<int, List<int>> SingleUseSymbols
        { get { return m_singleUseSymbols; } set { m_singleUseSymbols = value; } }
        // the number of symbols in the global symbol table
        internal int NumGlobalSymbols
        { get { return m_numGlobalSymbols; } set { m_numGlobalSymbols = value; } }
        internal List<int> PageXRes
        { get { return m_pageXRes; } set { m_pageXRes = value; } }
        internal List<int> PageYRes
        { get { return m_pageYRes; } set { m_pageYRes = value; } }
        internal List<int> PageWidth
        { get { return m_pageWidth; } set { m_pageWidth = value; } }
        internal List<int> PageHeight
        { get { return m_pageHeight; } set { m_pageHeight = value; } }
        // Used to store the mapping from symbol number to the index in the global symbol dictionary.
        internal SortedDictionary<int, int> SymbolMap
        { get { return m_symbolMap; } set { m_symbolMap = value; } }
        internal bool Refinement
        { get { return m_refinement; } set { m_refinement = value; } }
        internal Pixa AvgTemplates // grayed templates
        { get { return m_avgTemplates; } set { m_avgTemplates = value; } }
        internal int RefineLevel
        { get { return m_refineLevel; } set { m_refineLevel = value; } }
        // only used when using refinement
        // the number of the first symbol of each page
        internal List<int> BaseIndexes
        { get { return m_baseIndexes; } set { m_baseIndexes = value; } }

        const int MAX_CONN_COMP_WIDTH = 350;  /* default max cc width */
        const int MAX_CHAR_COMP_WIDTH = 350;  /* default max char width */
        const int MAX_WORD_COMP_WIDTH = 1000;  /* default max word width */
        const int MAX_COMP_HEIGHT = 120;
        const int JB_CONN_COMPS = 0, JB_CHARACTERS = 1, JB_WORDS = 2;
        const int JB_RANKHAUS = 0, JB_CORRELATION = 1;

        internal JBIG2Context(float m_threshold, float weight, int xres, int yres, bool full_headers)
        {
            XRes = xres;
            YRes = yres;
            FullHeaders = full_headers;
            PDFPageNumbering = !FullHeaders;
            SegNumber = 0;
            SymbolTableSegment = -1;
            AvgTemplates = null;
            Refinement = false;
            m_classifier = JBCorrelationInitWithoutComponents(JB_CONN_COMPS, 9999, 9999, m_threshold, weight);
            PageWidth = new List<int>();
            PageHeight = new List<int>();
            PageXRes = new List<int>();
            PageYRes = new List<int>();
            PageComps = new Dictionary<int, List<int>>();
            SingleUseSymbols = new Dictionary<int, List<int>>();
            BaseIndexes = new List<int>();
        }

        private JBIG2Classifier JBCorrelationInitWithoutComponents(int components, int maxwidth, int maxheight, float thresh, float weightfactor)
        {
            return JBCorrelationInitInternal(components, maxwidth, maxheight, thresh, weightfactor, 0);
        }

        private JBIG2Classifier JBCorrelationInitInternal(int components, int maxwidth, int maxheight, float thresh, float weightfactor, int keep_components)
        {
            if (components != JB_CONN_COMPS && components != JB_CHARACTERS && components != JB_WORDS)
                throw new Exception();
            if (thresh < 0.4 || thresh > 0.98)
                throw new Exception();
            if (weightfactor < 0.0 || weightfactor > 1.0)
                throw new Exception();
            if (maxwidth == 0)
            {
                if (components == JB_CONN_COMPS)
                    maxwidth = MAX_CONN_COMP_WIDTH;
                else if (components == JB_CHARACTERS)
                    maxwidth = MAX_CHAR_COMP_WIDTH;
                else  /* JB_WORDS */
                    maxwidth = MAX_WORD_COMP_WIDTH;
            }
            if (maxheight == 0)
                maxheight = MAX_COMP_HEIGHT;

            JBIG2Classifier classer = jbClasserCreate(JB_CORRELATION, components);
            if (classer == null)
                throw new NullReferenceException("Classifier is null");
            classer.MaxWidth = maxwidth;
            classer.MaxHeight = maxheight;
            classer.Thresh = thresh;
            classer.WeightFactor = weightfactor;
            classer.NaHash = NumaHashCreate(5507, 4);  /* 5507 is prime */
            classer.KeepPixaa = keep_components;
            return classer;
        }

        private NumaHash NumaHashCreate(int nbuckets, int initsize)
        {
            if (nbuckets <= 0)
                throw new ArgumentOutOfRangeException("Negative hash size");
          
            NumaHash nahash = new NumaHash(nbuckets, initsize);
            return nahash;
        }

        private JBIG2Classifier jbClasserCreate(int method, int components)
        {
            int JB_RANKHAUS = 0, JB_CORRELATION = 1, JB_CONN_COMPS = 0, JB_CHARACTERS = 1, JB_WORDS = 2;

            if (method != JB_RANKHAUS && method != JB_CORRELATION)
            {
                Console.WriteLine("invalid type");
                return null;
            }
            if (components != JB_CONN_COMPS && components != JB_CHARACTERS && components != JB_WORDS)
            {
                Console.WriteLine("invalid type");
                return null;
            }

            JBIG2Classifier classer = new JBIG2Classifier(method, components, JBIG2Statics.CreateNuma(0), PixaaCreate(0), JBIG2Statics.CreatePixa(0), JBIG2Statics.CreatePixa(0), JBIG2Statics.CreateNuma(0), JBIG2Statics.CreateNuma(0), CreatePta(0), CreatePta(0), JBIG2Statics.CreateNuma(0), JBIG2Statics.CreateNuma(0), CreatePta(0));

            return classer;
        }

        private Pixaa PixaaCreate(int n)
        {
            const int INITIAL_PTR_ARRAYSIZE = 20;

            if (n <= 0)
                n = INITIAL_PTR_ARRAYSIZE;

            Pixaa pixaa = new Pixaa(n);
            //pixaa.Boxa = JBIG2Statics.CreateBoxa(n);

            return pixaa;
        }

        private Pta CreatePta(int n)
        {
            const int INITIAL_PTR_ARRAYSIZE = 20;

            if (n <= 0)
                n = INITIAL_PTR_ARRAYSIZE;

            Pta pta = new Pta(n);
            PtaChangeRefcount(pta, 1);  /* sets to 1 */

            return pta;
        }

        private void PtaChangeRefcount(Pta pta, int delta)
        {
            if (pta == null)
                throw new NullReferenceException("pta not defined");

            pta.RefCount += delta;
        }
    }
}