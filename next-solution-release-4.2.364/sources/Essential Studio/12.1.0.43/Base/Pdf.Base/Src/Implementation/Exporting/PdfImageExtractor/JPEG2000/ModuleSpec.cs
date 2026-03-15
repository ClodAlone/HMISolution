#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
using HashTable = System.Collections.Generic.Dictionary<object, object>;
using ArrayList = System.Collections.Generic.List<object>;
namespace Syncfusion.Pdf.JPEG2000
{
    internal class ModuleSpec
    {
        virtual public ModuleSpec Copy
        {
            get
            {
                return (ModuleSpec)this.Clone();
            }
        }
        public const byte SPEC_TYPE_COMP = 0;
        public const byte SPEC_TYPE_TILE = 1;
        public const byte SPEC_TYPE_TILE_COMP = 2;
        public const byte SPEC_DEF = 0;
        public const byte SPEC_COMP_DEF = 1;
        public const byte SPEC_TILE_DEF = 2;
        public const byte SPEC_TILE_COMP = 3;
        internal int specType;
        internal int nTiles = 0;
        internal int nComp = 0;
        internal byte[][] specValType;
        internal System.Object def = null;
        internal System.Object[] compDef = null;
        internal System.Object[] tileDef = null;
        internal HashTable tileCompVal;
        public virtual System.Object Clone()
        {
            ModuleSpec ms=null;
            try
            {
                ms = (ModuleSpec)base.MemberwiseClone();
            }
            catch (System.Exception)
            {
                //throw new System.ApplicationException("Error when cloning ModuleSpec instance");
            }
            ms.specValType = new byte[nTiles][];
            for (int i = 0; i < nTiles; i++)
            {
                ms.specValType[i] = new byte[nComp];
            }
            for (int t = 0; t < nTiles; t++)
            {
                for (int c = 0; c < nComp; c++)
                {
                    ms.specValType[t][c] = specValType[t][c];
                }
            }
            if (tileDef != null)
            {
                ms.tileDef = new System.Object[nTiles];
                for (int t = 0; t < nTiles; t++)
                {
                    ms.tileDef[t] = tileDef[t];
                }
            }
            if (tileCompVal != null)
            {
                ms.tileCompVal = new HashTable();
                System.String tmpKey;
                System.Object tmpVal;
                for (System.Collections.IEnumerator e = tileCompVal.Keys.GetEnumerator(); e.MoveNext(); )
                {
                    tmpKey = ((System.String)e.Current);
                    tmpVal = tileCompVal[tmpKey];
                    ms.tileCompVal[tmpKey] = tmpVal;
                }
            }
            return ms;
        }
        public virtual void rotate90(JPXImageCoordinates anT)
        {
            byte[][] tmpsvt = new byte[nTiles][];
            int ax, ay;
            JPXImageCoordinates bnT = new JPXImageCoordinates(anT.y, anT.x);
            for (int by = 0; by < bnT.y; by++)
            {
                for (int bx = 0; bx < bnT.x; bx++)
                {
                    ay = bx;
                    ax = bnT.y - by - 1;
                    tmpsvt[ay * anT.x + ax] = specValType[by * bnT.x + bx];
                }
            }
            specValType = tmpsvt;
            if (tileDef != null)
            {
                System.Object[] tmptd = new System.Object[nTiles];
                for (int by = 0; by < bnT.y; by++)
                {
                    for (int bx = 0; bx < bnT.x; bx++)
                    {
                        ay = bx;
                        ax = bnT.y - by - 1;
                        tmptd[ay * anT.x + ax] = tileDef[by * bnT.x + bx];
                    }
                }
                tileDef = tmptd;
            }
            if (tileCompVal != null && tileCompVal.Count > 0)
            {
                HashTable tmptcv = new HashTable();
                System.String tmpKey;
                System.Object tmpVal;
                int btIdx, atIdx;
                int i1, i2;
                int bx, by;
                for (System.Collections.IEnumerator e = tileCompVal.Keys.GetEnumerator(); e.MoveNext(); )
                {
                    tmpKey = ((System.String)e.Current);
                    tmpVal = tileCompVal[tmpKey];
                    i1 = tmpKey.IndexOf('t');
                    i2 = tmpKey.IndexOf('c');
                    btIdx = (System.Int32.Parse(tmpKey.Substring(i1 + 1, (i2) - (i1 + 1))));
                    bx = btIdx % bnT.x;
                    by = btIdx / bnT.x;
                    ay = bx;
                    ax = bnT.y - by - 1;
                    atIdx = ax + ay * anT.x;
                    tmptcv["t" + atIdx + tmpKey.Substring(i2)] = tmpVal;
                }
                tileCompVal = tmptcv;
            }
        }
        public ModuleSpec(int nt, int nc, byte type)
        {
            nTiles = nt;
            nComp = nc;
            specValType = new byte[nt][];
            for (int i = 0; i < nt; i++)
            {
                specValType[i] = new byte[nc];
            }
            switch (type)
            {
                case SPEC_TYPE_TILE:
                    specType = SPEC_TYPE_TILE;
                    break;
                case SPEC_TYPE_COMP:
                    specType = SPEC_TYPE_COMP;
                    break;
                case SPEC_TYPE_TILE_COMP:
                    specType = SPEC_TYPE_TILE_COMP;
                    break;
            }
        }
        public virtual void setDefault(System.Object value_Renamed)
        {
            def = value_Renamed;
        }
        public virtual System.Object getDefault()
        {
            return def;
        }
        public virtual void setCompDef(int c, System.Object value_Renamed)
        {
            if (specType == SPEC_TYPE_TILE)
            {
                System.String errMsg = "Option whose value is '" + value_Renamed + "' cannot be " + "specified for components as it is a 'tile only' specific " + "option";
                //throw new System.ApplicationException(errMsg);
            }
            if (compDef == null)
            {
                compDef = new System.Object[nComp];
            }
            for (int i = 0; i < nTiles; i++)
            {
                if (specValType[i][c] < SPEC_COMP_DEF)
                {
                    specValType[i][c] = SPEC_COMP_DEF;
                }
            }
            compDef[c] = value_Renamed;
        }
        public virtual System.Object getCompDef(int c)
        {
            if (specType == SPEC_TYPE_TILE)
            {
                //throw new System.ApplicationException("Illegal use of ModuleSpec class");
            }
            if (compDef == null || compDef[c] == null)
            {
                return getDefault();
            }
            else
            {
                return compDef[c];
            }
        }
        public virtual void setTileDef(int t, System.Object value_Renamed)
        {
            if (specType == SPEC_TYPE_COMP)
            {
                System.String errMsg = "Option whose value is '" + value_Renamed + "' cannot be " + "specified for tiles as it is a 'component only' specific " + "option";
                //throw new System.ApplicationException(errMsg);
            }
            if (tileDef == null)
            {
                tileDef = new System.Object[nTiles];
            }
            for (int i = 0; i < nComp; i++)
            {
                if (specValType[t][i] < SPEC_TILE_DEF)
                {
                    specValType[t][i] = SPEC_TILE_DEF;
                }
            }
            tileDef[t] = value_Renamed;
        }
        public virtual System.Object getTileDef(int t)
        {
            if (specType == SPEC_TYPE_COMP)
            {
                //throw new System.ApplicationException("Illegal use of ModuleSpec class");
            }
            if (tileDef == null || tileDef[t] == null)
            {
                return getDefault();
            }
            else
            {
                return tileDef[t];
            }
        }
        public virtual void setTileCompVal(int t, int c, System.Object value_Renamed)
        {
            if (specType != SPEC_TYPE_TILE_COMP)
            {
                System.String errMsg = "Option whose value is '" + value_Renamed + "' cannot be " + "specified for ";
                switch (specType)
                {
                    case SPEC_TYPE_TILE:
                        errMsg += "components as it is a 'tile only' specific option";
                        break;
                    case SPEC_TYPE_COMP:
                        errMsg += "tiles as it is a 'component only' specific option";
                        break;
                }
                //throw new System.ApplicationException(errMsg);
            }
            if (tileCompVal == null)
                tileCompVal = new HashTable();
            specValType[t][c] = SPEC_TILE_COMP;
            tileCompVal["t" + t + "c" + c] = value_Renamed;
        }
        public virtual System.Object getTileCompVal(int t, int c)
        {
            if (specType != SPEC_TYPE_TILE_COMP)
            {
                //throw new System.ApplicationException("Illegal use of ModuleSpec class");
            }
            return getSpec(t, c);
        }
        internal virtual System.Object getSpec(int t, int c)
        {
            switch (specValType[t][c])
            {
                case SPEC_DEF:
                    return getDefault();
                case SPEC_COMP_DEF:
                    return getCompDef(c);
                case SPEC_TILE_DEF:
                    return getTileDef(t);
                case SPEC_TILE_COMP:
                    return tileCompVal["t" + t + "c" + c];
                default:
                    throw new System.ArgumentException("Not recognized spec type");
            }
        }
        public virtual byte getSpecValType(int t, int c)
        {
            return specValType[t][c];
        }
        public virtual bool isCompSpecified(int c)
        {
            if (compDef == null || compDef[c] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public virtual bool isTileSpecified(int t)
        {
            if (tileDef == null || tileDef[t] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public virtual bool isTileCompSpecified(int t, int c)
        {
            if (tileCompVal == null || tileCompVal["t" + t + "c" + c] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool[] parseIdx(System.String word, int maxIdx)
        {
            int nChar = word.Length;
            char c = word[0];
            int idx = -1;
            int lastIdx = -1;
            bool isDash = false;
            bool[] idxSet = new bool[maxIdx];
            int i = 1;
            while (i < nChar)
            {
                c = word[i];
                if (System.Char.IsDigit(c))
                {
                    if (idx == -1)
                    {
                        idx = 0;
                    }
                    idx = idx * 10 + (c - '0');
                }
                else
                {
                    if (idx == -1 || (c != ',' && c != '-'))
                    {
                        throw new System.ArgumentException("Bad construction for " + "parameter: " + word);
                    }
                    if (idx < 0 || idx >= maxIdx)
                    {
                        throw new System.ArgumentException("Out of range index " + "in " + "parameter `" + word + "' : " + idx);
                    }
                    if (c == ',')
                    {
                        if (isDash)
                        {
                            for (int j = lastIdx + 1; j < idx; j++)
                            {
                                idxSet[j] = true;
                            }
                        }
                        isDash = false;
                    }
                    else
                    {
                        isDash = true;
                    }
                    idxSet[idx] = true;
                    lastIdx = idx;
                    idx = -1;
                }
                i++;
            }
            if (idx < 0 || idx >= maxIdx)
            {
                throw new System.ArgumentException("Out of range index in " + "parameter `" + word + "' : " + idx);
            }
            if (isDash)
            {
                for (int j = lastIdx + 1; j < idx; j++)
                {
                    idxSet[j] = true;
                }
            }
            idxSet[idx] = true;
            return idxSet;
        }
    }
}