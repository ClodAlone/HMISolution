#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;
using System.Drawing;
#if SILVERLIGHT
using System.Windows.Media;
#endif

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// The collection of the default pens.
    /// </summary>
    public sealed class PdfPens
    {
        #region Static Fields
        private static Dictionary<object, object> s_pens = new Dictionary<object, object>();
        #endregion


        #region Static Properties
        /// <summary>
        /// Gets the AliceBlue pen.
        /// </summary>
        public static PdfPen AliceBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.AliceBlue))
                        pen = s_pens[KnownColor.AliceBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.AliceBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the antique white pen.
        /// </summary>
        public static PdfPen AntiqueWhite
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.AntiqueWhite))
                        pen = s_pens[KnownColor.AntiqueWhite] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.AntiqueWhite);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Aqua default pen.
        /// </summary>
        public static PdfPen Aqua
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Aqua))
                        pen = s_pens[KnownColor.Aqua] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Aqua);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Aquamarine default pen.
        /// </summary>
        public static PdfPen Aquamarine
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Aquamarine))
                        pen = s_pens[KnownColor.Aquamarine] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Aquamarine);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Azure default pen.
        /// </summary>
        public static PdfPen Azure
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Azure))
                        pen = s_pens[KnownColor.Azure] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Azure);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Beige default pen.
        /// </summary>
        public static PdfPen Beige
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Beige))
                        pen = s_pens[KnownColor.Beige] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Beige);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Bisque default pen.
        /// </summary>
        public static PdfPen Bisque
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Bisque))
                        pen = s_pens[KnownColor.Bisque] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Bisque);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Black default pen.
        /// </summary>
        public static PdfPen Black
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Black))
                        pen = s_pens[KnownColor.Black] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Black);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the BlanchedAlmond default pen.
        /// </summary>
        public static PdfPen BlanchedAlmond
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.BlanchedAlmond))
                        pen = s_pens[KnownColor.BlanchedAlmond] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.BlanchedAlmond);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Blue default pen.
        /// </summary>
        public static PdfPen Blue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Blue))
                        pen = s_pens[KnownColor.Blue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Blue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the BlueViolet default pen.
        /// </summary>
        public static PdfPen BlueViolet
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.BlueViolet))
                        pen = s_pens[KnownColor.BlueViolet] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.BlueViolet);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Brown default pen.
        /// </summary>
        public static PdfPen Brown
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Brown))
                        pen = s_pens[KnownColor.Brown] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Brown);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the BurlyWood default pen.
        /// </summary>
        public static PdfPen BurlyWood
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.BurlyWood))
                        pen = s_pens[KnownColor.BurlyWood] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.BurlyWood);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the CadetBlue default pen.
        /// </summary>
        public static PdfPen CadetBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.CadetBlue))
                        pen = s_pens[KnownColor.CadetBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.CadetBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Chartreuse default pen.
        /// </summary>
        public static PdfPen Chartreuse
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Chartreuse))
                        pen = s_pens[KnownColor.Chartreuse] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Chartreuse);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Chocolate default pen.
        /// </summary>
        public static PdfPen Chocolate
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Chocolate))
                        pen = s_pens[KnownColor.Chocolate] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Chocolate);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Coral default pen.
        /// </summary>
        public static PdfPen Coral
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Coral))
                        pen = s_pens[KnownColor.Coral] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Coral);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the CornflowerBlue default pen.
        /// </summary>
        public static PdfPen CornflowerBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.CornflowerBlue))
                        pen = s_pens[KnownColor.CornflowerBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.CornflowerBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Corn silk default pen.
        /// </summary>
        public static PdfPen Cornsilk
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Cornsilk))
                        pen = s_pens[KnownColor.Cornsilk] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Cornsilk);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Crimson default pen.
        /// </summary>
        public static PdfPen Crimson
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Crimson))
                        pen = s_pens[KnownColor.Crimson] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Crimson);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Cyan default pen.
        /// </summary>
        public static PdfPen Cyan
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Cyan))
                        pen = s_pens[KnownColor.Cyan] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Cyan);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkBlue default pen.
        /// </summary>
        public static PdfPen DarkBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkBlue))
                        pen = s_pens[KnownColor.DarkBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkCyan default pen.
        /// </summary>
        public static PdfPen DarkCyan
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkCyan))
                        pen = s_pens[KnownColor.DarkCyan] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkCyan);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkGoldenrod default pen.
        /// </summary>
        public static PdfPen DarkGoldenrod
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkGoldenrod))
                        pen = s_pens[KnownColor.DarkGoldenrod] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkGoldenrod);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkGray default pen.
        /// </summary>
        public static PdfPen DarkGray
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkGray))
                        pen = s_pens[KnownColor.DarkGray] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkGray);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkGreen default pen.
        /// </summary>
        public static PdfPen DarkGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkGreen))
                        pen = s_pens[KnownColor.DarkGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkKhaki default pen.
        /// </summary>
        public static PdfPen DarkKhaki
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkKhaki))
                        pen = s_pens[KnownColor.DarkKhaki] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkKhaki);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkMagenta default pen.
        /// </summary>
        public static PdfPen DarkMagenta
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkMagenta))
                        pen = s_pens[KnownColor.DarkMagenta] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkMagenta);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkOliveGreen default pen.
        /// </summary>
        public static PdfPen DarkOliveGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkOliveGreen))
                        pen = s_pens[KnownColor.DarkOliveGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkOliveGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkOrange default pen.
        /// </summary>
        public static PdfPen DarkOrange
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkOrange))
                        pen = s_pens[KnownColor.DarkOrange] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkOrange);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkOrchid default pen.
        /// </summary>
        public static PdfPen DarkOrchid
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkOrchid))
                        pen = s_pens[KnownColor.DarkOrchid] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkOrchid);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkRed default pen.
        /// </summary>
        public static PdfPen DarkRed
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkRed))
                        pen = s_pens[KnownColor.DarkRed] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkRed);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkSalmon default pen.
        /// </summary>
        public static PdfPen DarkSalmon
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkSalmon))
                        pen = s_pens[KnownColor.DarkSalmon] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkSalmon);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkSeaGreen default pen.
        /// </summary>
        public static PdfPen DarkSeaGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkSeaGreen))
                        pen = s_pens[KnownColor.DarkSeaGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkSeaGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkSlateBlue default pen.
        /// </summary>
        public static PdfPen DarkSlateBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkSlateBlue))
                        pen = s_pens[KnownColor.DarkSlateBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkSlateBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkSlateGray default pen.
        /// </summary>
        public static PdfPen DarkSlateGray
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkSlateGray))
                        pen = s_pens[KnownColor.DarkSlateGray] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkSlateGray);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkTurquoise default pen.
        /// </summary>
        public static PdfPen DarkTurquoise
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkTurquoise))
                        pen = s_pens[KnownColor.DarkTurquoise] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkTurquoise);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DarkViolet default pen.
        /// </summary>
        public static PdfPen DarkViolet
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DarkViolet))
                        pen = s_pens[KnownColor.DarkViolet] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DarkViolet);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DeepPink default pen.
        /// </summary>
        public static PdfPen DeepPink
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DeepPink))
                        pen = s_pens[KnownColor.DeepPink] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DeepPink);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DeepSkyBlue default pen.
        /// </summary>
        public static PdfPen DeepSkyBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DeepSkyBlue))
                        pen = s_pens[KnownColor.DeepSkyBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DeepSkyBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DimGray default pen.
        /// </summary>
        public static PdfPen DimGray
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DimGray))
                        pen = s_pens[KnownColor.DimGray] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DimGray);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the DodgerBlue default pen.
        /// </summary>
        public static PdfPen DodgerBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.DodgerBlue))
                        pen = s_pens[KnownColor.DodgerBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.DodgerBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Firebrick default pen.
        /// </summary>
        public static PdfPen Firebrick
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Firebrick))
                        pen = s_pens[KnownColor.Firebrick] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Firebrick);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the FloralWhite default pen.
        /// </summary>
        public static PdfPen FloralWhite
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.FloralWhite))
                        pen = s_pens[KnownColor.FloralWhite] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.FloralWhite);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the ForestGreen default pen.
        /// </summary>
        public static PdfPen ForestGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.ForestGreen))
                        pen = s_pens[KnownColor.ForestGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.ForestGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Fuchsia default pen.
        /// </summary>
        public static PdfPen Fuchsia
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Fuchsia))
                        pen = s_pens[KnownColor.Fuchsia] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Fuchsia);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Gainsborough default pen.
        /// </summary>
        public static PdfPen Gainsboro
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Gainsboro))
                        pen = s_pens[KnownColor.Gainsboro] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Gainsboro);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the GhostWhite default pen.
        /// </summary>
        public static PdfPen GhostWhite
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.GhostWhite))
                        pen = s_pens[KnownColor.GhostWhite] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.GhostWhite);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Gold default pen.
        /// </summary>
        public static PdfPen Gold
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Gold))
                        pen = s_pens[KnownColor.Gold] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Gold);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Goldenrod default pen.
        /// </summary>
        public static PdfPen Goldenrod
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Goldenrod))
                        pen = s_pens[KnownColor.Goldenrod] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Goldenrod);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Gray default pen.
        /// </summary>
        public static PdfPen Gray
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Gray))
                        pen = s_pens[KnownColor.Gray] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Gray);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Green default pen.
        /// </summary>
        public static PdfPen Green
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Green))
                        pen = s_pens[KnownColor.Green] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Green);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the GreenYellow default pen.
        /// </summary>
        public static PdfPen GreenYellow
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.GreenYellow))
                        pen = s_pens[KnownColor.GreenYellow] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.GreenYellow);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Honeydew default pen.
        /// </summary>
        public static PdfPen Honeydew
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Honeydew))
                        pen = s_pens[KnownColor.Honeydew] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Honeydew);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the HotPink default pen.
        /// </summary>
        public static PdfPen HotPink
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.HotPink))
                        pen = s_pens[KnownColor.HotPink] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.HotPink);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the IndianRed default pen.
        /// </summary>
        public static PdfPen IndianRed
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.IndianRed))
                        pen = s_pens[KnownColor.IndianRed] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.IndianRed);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Indigo default pen.
        /// </summary>
        public static PdfPen Indigo
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Indigo))
                        pen = s_pens[KnownColor.Indigo] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Indigo);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Ivory default pen.
        /// </summary>
        public static PdfPen Ivory
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Ivory))
                        pen = s_pens[KnownColor.Ivory] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Ivory);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Khaki default pen.
        /// </summary>
        public static PdfPen Khaki
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Khaki))
                        pen = s_pens[KnownColor.Khaki] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Khaki);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Lavender default pen.
        /// </summary>
        public static PdfPen Lavender
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Lavender))
                        pen = s_pens[KnownColor.Lavender] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Lavender);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LavenderBlush default pen.
        /// </summary>
        public static PdfPen LavenderBlush
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LavenderBlush))
                        pen = s_pens[KnownColor.LavenderBlush] as PdfPen;


                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LavenderBlush);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LawnGreen default pen.
        /// </summary>
        public static PdfPen LawnGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LawnGreen))
                        pen = s_pens[KnownColor.LawnGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LawnGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LemonChiffon default pen.
        /// </summary>
        public static PdfPen LemonChiffon
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LemonChiffon))
                        pen = s_pens[KnownColor.LemonChiffon] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LemonChiffon);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightBlue default pen.
        /// </summary>
        public static PdfPen LightBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightBlue))
                        pen = s_pens[KnownColor.LightBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightCoral default pen.
        /// </summary>
        public static PdfPen LightCoral
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightCoral))
                        pen = s_pens[KnownColor.LightCoral] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightCoral);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightCyan default pen.
        /// </summary>
        public static PdfPen LightCyan
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightCyan))
                        pen = s_pens[KnownColor.LightCyan] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightCyan);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightGoldenrodYellow default pen.
        /// </summary>
        public static PdfPen LightGoldenrodYellow
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightGoldenrodYellow))
                        pen = s_pens[KnownColor.LightGoldenrodYellow] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightGoldenrodYellow);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightGray default pen.
        /// </summary>
        public static PdfPen LightGray
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightGray))
                        pen = s_pens[KnownColor.LightGray] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightGray);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightGreen default pen.
        /// </summary>
        public static PdfPen LightGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightGreen))
                        pen = s_pens[KnownColor.LightGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightPink default pen.
        /// </summary>
        public static PdfPen LightPink
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightPink))
                        pen = s_pens[KnownColor.LightPink] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightPink);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightSalmon default pen.
        /// </summary>
        public static PdfPen LightSalmon
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightSalmon))
                        pen = s_pens[KnownColor.LightSalmon] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightSalmon);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightSeaGreen default pen.
        /// </summary>
        public static PdfPen LightSeaGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightSeaGreen))
                        pen = s_pens[KnownColor.LightSeaGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightSeaGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightSkyBlue default pen.
        /// </summary>
        public static PdfPen LightSkyBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightSkyBlue))
                        pen = s_pens[KnownColor.LightSkyBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightSkyBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightSlateGray default pen.
        /// </summary>
        public static PdfPen LightSlateGray
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightSlateGray))
                        pen = s_pens[KnownColor.LightSlateGray] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightSlateGray);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightSteelBlue default pen.
        /// </summary>
        public static PdfPen LightSteelBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightSteelBlue))
                        pen = s_pens[KnownColor.LightSteelBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightSteelBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LightYellow default pen.
        /// </summary>
        public static PdfPen LightYellow
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LightYellow))
                        pen = s_pens[KnownColor.LightYellow] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LightYellow);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Lime default pen.
        /// </summary>
        public static PdfPen Lime
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Lime))
                        pen = s_pens[KnownColor.Lime] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Lime);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the LimeGreen default pen.
        /// </summary>
        public static PdfPen LimeGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.LimeGreen))
                        pen = s_pens[KnownColor.LimeGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.LimeGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Linen default pen.
        /// </summary>
        public static PdfPen Linen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Linen))
                        pen = s_pens[KnownColor.Linen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Linen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Magenta default pen.
        /// </summary>
        public static PdfPen Magenta
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Magenta))
                        pen = s_pens[KnownColor.Magenta] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Magenta);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Maroon default pen.
        /// </summary>
        public static PdfPen Maroon
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Maroon))
                        pen = s_pens[KnownColor.Maroon] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Maroon);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumAquamarine default pen.
        /// </summary>
        public static PdfPen MediumAquamarine
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumAquamarine))
                        pen = s_pens[KnownColor.MediumAquamarine] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumAquamarine);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumBlue default pen.
        /// </summary>
        public static PdfPen MediumBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumBlue))
                        pen = s_pens[KnownColor.MediumBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumOrchid default pen.
        /// </summary>
        public static PdfPen MediumOrchid
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumOrchid))
                        pen = s_pens[KnownColor.MediumOrchid] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumOrchid);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumPurple default pen.
        /// </summary>
        public static PdfPen MediumPurple
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumPurple))
                        pen = s_pens[KnownColor.MediumPurple] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumPurple);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumSeaGreen default pen.
        /// </summary>
        public static PdfPen MediumSeaGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumSeaGreen))
                        pen = s_pens[KnownColor.MediumSeaGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumSeaGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumSlateBlue default pen.
        /// </summary>
        public static PdfPen MediumSlateBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumSlateBlue))
                        pen = s_pens[KnownColor.MediumSlateBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumSlateBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumSpringGreen default pen.
        /// </summary>
        public static PdfPen MediumSpringGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumSpringGreen))
                        pen = s_pens[KnownColor.MediumSpringGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumSpringGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumTurquoise default pen.
        /// </summary>
        public static PdfPen MediumTurquoise
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumTurquoise))
                        pen = s_pens[KnownColor.MediumTurquoise] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumTurquoise);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MediumVioletRed default pen.
        /// </summary>
        public static PdfPen MediumVioletRed
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MediumVioletRed))
                        pen = s_pens[KnownColor.MediumVioletRed] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MediumVioletRed);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MidnightBlue default pen.
        /// </summary>
        public static PdfPen MidnightBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MidnightBlue))
                        pen = s_pens[KnownColor.MidnightBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MidnightBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MintCream default pen.
        /// </summary>
        public static PdfPen MintCream
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MintCream))
                        pen = s_pens[KnownColor.MintCream] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MintCream);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the MistyRose default pen.
        /// </summary>
        public static PdfPen MistyRose
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.MistyRose))
                        pen = s_pens[KnownColor.MistyRose] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.MistyRose);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Moccasin default pen.
        /// </summary>
        public static PdfPen Moccasin
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Moccasin))
                        pen = s_pens[KnownColor.Moccasin] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Moccasin);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the NavajoWhite default pen.
        /// </summary>
        public static PdfPen NavajoWhite
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.NavajoWhite))
                        pen = s_pens[KnownColor.NavajoWhite] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.NavajoWhite);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Navy default pen.
        /// </summary>
        public static PdfPen Navy
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Navy))
                        pen = s_pens[KnownColor.Navy] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Navy);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the OldLace default pen.
        /// </summary>
        public static PdfPen OldLace
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.OldLace))
                        pen = s_pens[KnownColor.OldLace] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.OldLace);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Olive default pen.
        /// </summary>
        public static PdfPen Olive
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Olive))
                        pen = s_pens[KnownColor.Olive] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Olive);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the OliveDrab default pen.
        /// </summary>
        public static PdfPen OliveDrab
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.OliveDrab))
                        pen = s_pens[KnownColor.OliveDrab] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.OliveDrab);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Orange default pen.
        /// </summary>
        public static PdfPen Orange
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Orange))
                        pen = s_pens[KnownColor.Orange] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Orange);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the OrangeRed default pen.
        /// </summary>
        public static PdfPen OrangeRed
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.OrangeRed))
                        pen = s_pens[KnownColor.OrangeRed] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.OrangeRed);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Orchid default pen.
        /// </summary>
        public static PdfPen Orchid
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Orchid))
                        pen = s_pens[KnownColor.Orchid] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Orchid);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the PaleGoldenrod default pen.
        /// </summary>
        public static PdfPen PaleGoldenrod
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.PaleGoldenrod))
                        pen = s_pens[KnownColor.PaleGoldenrod] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.PaleGoldenrod);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the PaleGreen default pen.
        /// </summary>
        public static PdfPen PaleGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.PaleGreen))
                        pen = s_pens[KnownColor.PaleGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.PaleGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the PaleTurquoise default pen.
        /// </summary>
        public static PdfPen PaleTurquoise
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.PaleTurquoise))
                        pen = s_pens[KnownColor.PaleTurquoise] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.PaleTurquoise);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the PaleVioletRed default pen.
        /// </summary>
        public static PdfPen PaleVioletRed
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.PaleVioletRed))
                        pen = s_pens[KnownColor.PaleVioletRed] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.PaleVioletRed);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the PapayaWhip default pen.
        /// </summary>
        public static PdfPen PapayaWhip
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.PapayaWhip))
                        pen = s_pens[KnownColor.PapayaWhip] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.PapayaWhip);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the PeachPuff default pen.
        /// </summary>
        public static PdfPen PeachPuff
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.PeachPuff))
                        pen = s_pens[KnownColor.PeachPuff] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.PeachPuff);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Peru default pen.
        /// </summary>
        public static PdfPen Peru
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Peru))
                        pen = s_pens[KnownColor.Peru] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Peru);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Pink default pen.
        /// </summary>
        public static PdfPen Pink
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Pink))
                        pen = s_pens[KnownColor.Pink] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Pink);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Plum default pen.
        /// </summary>
        public static PdfPen Plum
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Plum))
                        pen = s_pens[KnownColor.Plum] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Plum);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the PowderBlue default pen.
        /// </summary>
        public static PdfPen PowderBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.PowderBlue))
                        pen = s_pens[KnownColor.PowderBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.PowderBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Purple default pen.
        /// </summary>
        public static PdfPen Purple
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Purple))
                        pen = s_pens[KnownColor.Purple] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Purple);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Red default pen.
        /// </summary>
        public static PdfPen Red
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Red))
                        pen = s_pens[KnownColor.Red] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Red);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the RosyBrown default pen.
        /// </summary>
        public static PdfPen RosyBrown
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.RosyBrown))
                        pen = s_pens[KnownColor.RosyBrown] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.RosyBrown);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the RoyalBlue default pen.
        /// </summary>
        public static PdfPen RoyalBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.RoyalBlue))
                        pen = s_pens[KnownColor.RoyalBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.RoyalBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SaddleBrown default pen.
        /// </summary>
        public static PdfPen SaddleBrown
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SaddleBrown))
                        pen = s_pens[KnownColor.SaddleBrown] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SaddleBrown);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Salmon default pen.
        /// </summary>
        public static PdfPen Salmon
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Salmon))
                        pen = s_pens[KnownColor.Salmon] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Salmon);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SandyBrown default pen.
        /// </summary>
        public static PdfPen SandyBrown
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SandyBrown))
                        pen = s_pens[KnownColor.SandyBrown] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SandyBrown);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SeaGreen default pen.
        /// </summary>
        public static PdfPen SeaGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SeaGreen))
                        pen = s_pens[KnownColor.SeaGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SeaGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SeaShell default pen.
        /// </summary>
        public static PdfPen SeaShell
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SeaShell))
                        pen = s_pens[KnownColor.SeaShell] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SeaShell);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Sienna default pen.
        /// </summary>
        public static PdfPen Sienna
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Sienna))
                        pen = s_pens[KnownColor.Sienna] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Sienna);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Silver default pen.
        /// </summary>
        public static PdfPen Silver
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Silver))
                        pen = s_pens[KnownColor.Silver] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Silver);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SkyBlue default pen.
        /// </summary>
        public static PdfPen SkyBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SkyBlue))
                        pen = s_pens[KnownColor.SkyBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SkyBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SlateBlue default pen.
        /// </summary>
        public static PdfPen SlateBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SlateBlue))
                        pen = s_pens[KnownColor.SlateBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SlateBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SlateGray default pen.
        /// </summary>
        public static PdfPen SlateGray
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SlateGray))
                        pen = s_pens[KnownColor.SlateGray] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SlateGray);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Snow default pen.
        /// </summary>
        public static PdfPen Snow
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Snow))
                        pen = s_pens[KnownColor.Snow] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Snow);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SpringGreen default pen.
        /// </summary>
        public static PdfPen SpringGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SpringGreen))
                        pen = s_pens[KnownColor.SpringGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SpringGreen);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the SteelBlue default pen.
        /// </summary>
        public static PdfPen SteelBlue
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.SteelBlue))
                        pen = s_pens[KnownColor.SteelBlue] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.SteelBlue);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Tan default pen.
        /// </summary>
        public static PdfPen Tan
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Tan))
                        pen = s_pens[KnownColor.Tan] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Tan);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Teal default pen.
        /// </summary>
        public static PdfPen Teal
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Teal))
                        pen = s_pens[KnownColor.Teal] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Teal);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Thistle default pen.
        /// </summary>
        public static PdfPen Thistle
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Thistle))
                        pen = s_pens[KnownColor.Thistle] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Thistle);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Tomato default pen.
        /// </summary>
        public static PdfPen Tomato
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Tomato))
                        pen = s_pens[KnownColor.Tomato] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Tomato);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Transparent default pen.
        /// </summary>
        public static PdfPen Transparent
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Transparent))
                        pen = s_pens[KnownColor.Transparent] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Transparent);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Turquoise default pen.
        /// </summary>
        public static PdfPen Turquoise
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Turquoise))
                        pen = s_pens[KnownColor.Turquoise] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Turquoise);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Violet default pen.
        /// </summary>
        public static PdfPen Violet
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Violet))
                        pen = s_pens[KnownColor.Violet] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Violet);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Wheat default pen.
        /// </summary>
        public static PdfPen Wheat
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Wheat))
                        pen = s_pens[KnownColor.Wheat] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Wheat);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the White default pen.
        /// </summary>
        public static PdfPen White
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.White))
                        pen = s_pens[KnownColor.White] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.White);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the WhiteSmoke default pen.
        /// </summary>
        public static PdfPen WhiteSmoke
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.WhiteSmoke))
                        pen = s_pens[KnownColor.WhiteSmoke] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.WhiteSmoke);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the Yellow default pen.
        /// </summary>
        public static PdfPen Yellow
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.Yellow))
                        pen = s_pens[KnownColor.Yellow] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.Yellow);
                    }

                    return pen;
                }
            }
        }

        /// <summary>
        /// Gets the YellowGreen default pen.
        /// </summary>
        public static PdfPen YellowGreen
        {
            get
            {
                lock (s_pens)
                {
                    PdfPen pen = null;

                    if (s_pens.ContainsKey(KnownColor.YellowGreen))
                        pen = s_pens[KnownColor.YellowGreen] as PdfPen;

                    if (pen == null)
                    {
                        pen = GetPen(KnownColor.YellowGreen);
                    }

                    return pen;
                }
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Creates the default pen.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="colorName">Name of the color.</param>
        /// <returns>The proper PdfPen instance.</returns>
        private static PdfPen GetPen(KnownColor colorName)
        {
#if SILVERLIGHT || NETFX_CORE ||WP
            Color color = ColorConverter.FromKnownColor(colorName);
#else
            Color color = System.Drawing.Color.FromKnownColor(colorName);
#endif
            PdfColor pdfColor = new PdfColor(color);

            PdfPen pen = new PdfPen(color, true);

            s_pens[colorName] = pen;

            return pen;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Disallows to create an instance of PDfPens class.
        /// </summary>
        private PdfPens()
        {
        }
        #endregion
    }
}
