#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.util
{
    internal class JPXParameters : System.Collections.Generic.Dictionary<string, string>
    {
        private JPXParameters defaults;
        
        public JPXParameters DefaultParameterList
        {
            get
            {
                return (JPXParameters)defaults;
            }
        }
        public JPXParameters()
            : base()
        {
        }
        public JPXParameters(JPXParameters def)
            : base()
        {
            defaults = def;
        }
      
      
        public virtual System.String getParameter(System.String pname)
        {        
            System.String pval;
            if (this.ContainsKey(pname))
            {
                pval = ((System.String)this[(System.String)pname]);
            }
            else
            {
                defaults.TryGetValue(pname,out pval);
            }
            return pval;
        }
        public virtual bool getBooleanParameter(System.String pname)
        {
            System.String s = (System.String)getParameter(pname);
            if (s == null)
            {
                throw new System.ArgumentException("No parameter with name " + pname);
            }
            else if (s.Equals("on"))
            {
                return true;
            }
            else if (s.Equals("off"))
            {
                return false;
            }
            else
            {
                throw new System.Exception();                
            }
        }
        public virtual int getIntParameter(System.String pname)
        {
            System.String s = (System.String)getParameter(pname);
            if (s == null)
            {
                throw new System.ArgumentException("No parameter with name " + pname);
            }
            else
            {
                try
                {
                    return System.Int32.Parse(s);
                }
                catch (System.FormatException e)
                {
                    throw new System.FormatException("Parameter \"" + pname + "\" is not integer: " + e.Message);
                }
            }
        }
        public virtual float getFloatParameter(System.String pname)
        {
            System.String s = (System.String)getParameter(pname);
            if (s == null)
            {
                throw new System.ArgumentException("No parameter with name " + pname);
            }
            else
            {
                try
                {
                    return (float)(System.Single.Parse(s));
                }
                catch (System.FormatException e)
                {
                    throw new System.FormatException("Parameter \"" + pname + "\" is not floating-point: " + e.Message);
                }
            }
        }
        public virtual void checkList(char prfx, System.String[] plist)
        {
            System.Collections.IEnumerator args;
            System.String val;
            int i;
            bool isvalid;
            args = Keys.GetEnumerator();
            while (args.MoveNext())
            {
                val = ((System.String)args.Current);
                if (val.Length > 0 && val[0] == prfx)
                {
                    isvalid = false;
                    if (plist != null)
                    {
                        for (i = plist.Length - 1; i >= 0; i--)
                        {
                            if (val.Equals(plist[i]))
                            {
                                isvalid = true;
                                break;
                            }
                        }
                    }
                    if (!isvalid)
                    {
                        throw new System.ArgumentException("Option '" + val + "' is " + "not a valid one.");
                    }
                }
            }
        }
        public static System.String[] toNameArray(System.String[][] pinfo)
        {
            System.String[] pnames;
            if (pinfo == null)
            {
                return null;
            }
            pnames = new System.String[pinfo.Length];
            for (int i = pinfo.Length - 1; i >= 0; i--)
            {
                pnames[i] = pinfo[i][0];
            }
            return pnames;
        }
    }
}