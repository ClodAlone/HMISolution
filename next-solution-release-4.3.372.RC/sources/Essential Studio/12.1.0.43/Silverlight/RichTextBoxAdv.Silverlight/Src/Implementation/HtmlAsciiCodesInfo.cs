#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    public class HtmlAsciiCodesInfo
    {

        #region Private Members


        string[] HtmlCodeSymbols ={" ","\"\"","!","#","$","%","&","'","(",")","*","+",",","-",".","/","0","1","2","3","4","5","6","7","8","9",":",";","<","=",">","?","@",
                                  "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","[","\"","]","^","_","`",
                                  "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z","{","|","}","~"," ","¡","¢","£","¤",
                                  "¥","¦","§","¨","©","ª","«","¬","","­®","¯","°","±","²","³","´","µ","¶","•","¸","¹","º","»","¼","½","¾","¿","À","Á","Â","Ã","Ä","Å","Æ",
                                  "Ç","È","É","Ê","Ë","Ì","Í","Î","Ï","Ð","Ñ","Ò","Ó","Ô","Õ","Ö","×","Ø","Ù","Ú","Û","Ü","Ý","Þ","ß","à","á","â","ã","ä","å","æ","ç","è",
                                  "é","ê","ë","ì","í","î","ï","ð","ñ","ò","ó","ô","õ","ö","÷","ø","ù","ú","û","ü","ý","þ","ÿ","Œ","œ","Š","š","Ÿ","ƒ","–","—","‘","’","‚",
                                  "“","”","„","†","‡","•","…","‰","€","™"};
        #endregion


        #region Properties

        internal Dictionary<string, string> HtmlAsciiCodeTable
        {
            get;
            set;
        }

        internal Dictionary<string, string> HtmlNameTable
        {
            get;
            set;
        }


        #endregion

        public HtmlAsciiCodesInfo()
        {
            HtmlAsciiCodeTable = new Dictionary<string, string>();
            HtmlNameTable = new Dictionary<string, string>();
            AddHtmlAsciiSymbols();
            AddHtmlNames();
        }

        private void AddHtmlAsciiSymbols()
        {
            int j = -1;
            for (int i = 32; i <= 8482; i++)
            {
                if ((i > 126 && i < 160) || (i > 255 && i < 338) || (i > 339 && i < 352) || (i > 353 && i < 376) || (i > 376 && i < 402) || (i > 402 && i < 8211) || (i > 8212 && i < 8216)
                    || i == 8219 || i == 8223 || (i > 8226 && i < 8230) || (i > 8230 && i < 8240) || (i > 8240 && i < 8364) || (i > 8364 && i < 8482))
                    continue;
                else
                {
                    j++;
                    if (j < HtmlCodeSymbols.Length)
                        HtmlAsciiCodeTable.Add("&#" + i + ";", HtmlCodeSymbols[j]);
                }
            }
        }


        private void AddHtmlNames()
        {
            #region Dicionary Members

            HtmlNameTable.Add("&nbsp;", " ");
            HtmlNameTable.Add("&lt;", "<");
            HtmlNameTable.Add("&gt;", ">");
            HtmlNameTable.Add("&amp;", "&");
            HtmlNameTable.Add("&quot;", "\"");
            HtmlNameTable.Add("&iexcl;", "¡");
            HtmlNameTable.Add("&cent;", "¢");
            HtmlNameTable.Add("&pound;", "£");
            HtmlNameTable.Add("&curren;", "¤");
            HtmlNameTable.Add("&yen;", "¥");
            HtmlNameTable.Add("&brvbar;", "¦");
            HtmlNameTable.Add("&sect;", "§");
            HtmlNameTable.Add("&uml;", "¨");
            HtmlNameTable.Add("&copy;", "©");
            HtmlNameTable.Add("&ordf;", "ª");
            HtmlNameTable.Add("&laquo;", "«");
            HtmlNameTable.Add("&not;", "¬");
            HtmlNameTable.Add("&reg;", "­®");
            HtmlNameTable.Add("&macr;", "¯");
            HtmlNameTable.Add("&deg;", "°");
            HtmlNameTable.Add("&plusmn;", "±");
            HtmlNameTable.Add("&sup2;", "²");
            HtmlNameTable.Add("&sup3;", "³");
            HtmlNameTable.Add("&acute;", "´");
            HtmlNameTable.Add("&micro;", "µ");
            HtmlNameTable.Add("&para;", "¶");
            HtmlNameTable.Add("&middot;", "•");
            HtmlNameTable.Add("&cedil;", "¸");
            HtmlNameTable.Add("&sup1;", "¹");
            HtmlNameTable.Add("&ordm;", "º");
            HtmlNameTable.Add("&raquo;", "»");
            HtmlNameTable.Add("&frac14;", "¼");
            HtmlNameTable.Add("&frac12;", "½");
            HtmlNameTable.Add("&frac34;", "¾");
            HtmlNameTable.Add("&iquest;", "¿");
            HtmlNameTable.Add("&Agrave;", "À");
            HtmlNameTable.Add("&Aacute;", "Á");
            HtmlNameTable.Add("&Acirc;", "Â");
            HtmlNameTable.Add("&Atilde;", "Ã");
            HtmlNameTable.Add("&Auml;", "Ä");
            HtmlNameTable.Add("&Aring;", "Å");
            HtmlNameTable.Add("&AElig;", "Æ");
            HtmlNameTable.Add("&Ccedil;", "Ç");
            HtmlNameTable.Add("&Egrave;", "È");
            HtmlNameTable.Add("&Eacute;", "É");
            HtmlNameTable.Add("&Ecirc;", "Ê");
            HtmlNameTable.Add("&Euml;", "Ë");
            HtmlNameTable.Add("&Igrave;", "Ì");
            HtmlNameTable.Add("&Iacute;", "Í");
            HtmlNameTable.Add("&Icirc;", "Î");
            HtmlNameTable.Add("&Iuml;", "Ï");
            HtmlNameTable.Add("&ETH;", "Ð");
            HtmlNameTable.Add("&Ntilde;", "Ñ");
            HtmlNameTable.Add("&Ograve;", "Ò");
            HtmlNameTable.Add("&Oacute;", "Ó");
            HtmlNameTable.Add("&Ocirc;", "Ô");
            HtmlNameTable.Add("&Otilde;", "Õ");
            HtmlNameTable.Add("&Ouml;", "Ö");
            HtmlNameTable.Add("&times;", "×");
            HtmlNameTable.Add("&Oslash;", "Ø");
            HtmlNameTable.Add("&Ugrave;", "Ù");
            HtmlNameTable.Add("&Uacute;", "Ú");
            HtmlNameTable.Add("&Ucirc;", "Û");
            HtmlNameTable.Add("&Uuml;", "Ü");
            HtmlNameTable.Add("&Yacute;", "Ý");
            HtmlNameTable.Add("&THORN;", "Þ");
            HtmlNameTable.Add("&szlig;", "ß");
            HtmlNameTable.Add("&agrave;", "à");
            HtmlNameTable.Add("&aacute;", "á");
            HtmlNameTable.Add("&acirc;", "â");
            HtmlNameTable.Add("&atilde;", "ã");
            HtmlNameTable.Add("&auml;", "ä");
            HtmlNameTable.Add("&aring;", "å");
            HtmlNameTable.Add("&aelig;", "æ");
            HtmlNameTable.Add("&ccedil;", "ç");
            HtmlNameTable.Add("&egrave;", "è");
            HtmlNameTable.Add("&eacute;", "é");
            HtmlNameTable.Add("&ecirc;", "ê");
            HtmlNameTable.Add("&euml;", "ë");
            HtmlNameTable.Add("&igrave;", "ì");
            HtmlNameTable.Add("&iacute;", "í");
            HtmlNameTable.Add("&icirc;", "î");
            HtmlNameTable.Add("&iuml;", "ï");
            HtmlNameTable.Add("&eth;", "ð");
            HtmlNameTable.Add("&ntilde;", "ñ");
            HtmlNameTable.Add("&ograve;", "ò");
            HtmlNameTable.Add("&oacute;", "ó");
            HtmlNameTable.Add("&ocirc;", "ô");
            HtmlNameTable.Add("&otilde;", "õ");
            HtmlNameTable.Add("&ouml;", "ö");
            HtmlNameTable.Add("&divide;", "÷");
            HtmlNameTable.Add("&oslash;", "ø");
            HtmlNameTable.Add("&ugrave;", "ù");
            HtmlNameTable.Add("&uacute;", "ú");
            HtmlNameTable.Add("&ucirce;", "û");
            HtmlNameTable.Add("&uuml;", "ü");
            HtmlNameTable.Add("&yacute;", "ý");
            HtmlNameTable.Add("&thorn;", "þ");
            HtmlNameTable.Add("&yuml;", "ÿ");
            HtmlNameTable.Add("&lcap;", "Œ");
            HtmlNameTable.Add("&lsmall;", "œ");
            HtmlNameTable.Add("&Scaron;", "Š");
            HtmlNameTable.Add("&scaron;", "š");
            HtmlNameTable.Add("&Ydiaresis;", "Ÿ");
            HtmlNameTable.Add("&fhook;", "ƒ");
            HtmlNameTable.Add("&endash;", "–");
            HtmlNameTable.Add("&emdash;", "—");
            HtmlNameTable.Add("&lsquot;", "‘");
            HtmlNameTable.Add("&rsquot;", "’");
            HtmlNameTable.Add("&ldquo;", "“");
            HtmlNameTable.Add("&rdquo;", "”");
            HtmlNameTable.Add("&dlquo;", "„");
            HtmlNameTable.Add("&dagger;", "†");
            HtmlNameTable.Add("&ddagger;", "‡");
            HtmlNameTable.Add("&bullet;", "•");
            HtmlNameTable.Add("&helip;", "…");
            HtmlNameTable.Add("&pthou;", "‰");
            HtmlNameTable.Add("&tmark;", "™");
            HtmlNameTable.Add("&euro;", "€");

            #endregion
        }

    }
}
