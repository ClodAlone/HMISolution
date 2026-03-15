#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows.Media;
#endif

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the collection of immutable default brushes.
    /// </summary>
    public sealed class PdfBrushes
    {
        #region Static Fields
        /// <summary>
        /// Local variable to store the brushes.
        /// </summary>
        private static Dictionary<object, object> s_brushes = new Dictionary<object, object>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBrushes"/> class.
        /// </summary>
        private PdfBrushes()
        {
        }
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the AliceBlue brush.
        /// </summary>
        public static PdfBrush AliceBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.AliceBlue))
                        brush = s_brushes[KnownColor.AliceBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.AliceBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the antique white brush.
        /// </summary>
        public static PdfBrush AntiqueWhite
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.AntiqueWhite))
                        brush = s_brushes[KnownColor.AntiqueWhite] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.AntiqueWhite);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Aqua default brush.
        /// </summary>
        public static PdfBrush Aqua
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Aqua))
                        brush = s_brushes[KnownColor.Aqua] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Aqua);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Aquamarine default brush.
        /// </summary>
        public static PdfBrush Aquamarine
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Aquamarine))
                        brush = s_brushes[KnownColor.Aquamarine] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Aquamarine);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Azure default brush.
        /// </summary>
        public static PdfBrush Azure
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Azure))
                        brush = s_brushes[KnownColor.Azure] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Azure);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Beige default brush.
        /// </summary>
        public static PdfBrush Beige
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Beige))
                        brush = s_brushes[KnownColor.Beige] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Beige);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Bisque default brush.
        /// </summary>
        public static PdfBrush Bisque
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Bisque))
                        brush = s_brushes[KnownColor.Bisque] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Bisque);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Black default brush.
        /// </summary>
        public static PdfBrush Black
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Black))
                        brush = s_brushes[KnownColor.Black] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Black);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the BlanchedAlmond default brush.
        /// </summary>
        public static PdfBrush BlanchedAlmond
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.BlanchedAlmond))
                        brush = s_brushes[KnownColor.BlanchedAlmond] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.BlanchedAlmond);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Blue default brush.
        /// </summary>
        public static PdfBrush Blue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Blue))
                        brush = s_brushes[KnownColor.Blue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Blue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the BlueViolet default brush.
        /// </summary>
        public static PdfBrush BlueViolet
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.BlueViolet))
                        brush = s_brushes[KnownColor.BlueViolet] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.BlueViolet);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Brown default brush.
        /// </summary>
        public static PdfBrush Brown
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Brown))
                        brush = s_brushes[KnownColor.Brown] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Brown);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the BurlyWood default brush.
        /// </summary>
        public static PdfBrush BurlyWood
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.BurlyWood))
                        brush = s_brushes[KnownColor.BurlyWood] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.BurlyWood);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the CadetBlue default brush.
        /// </summary>
        public static PdfBrush CadetBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.CadetBlue))
                        brush = s_brushes[KnownColor.CadetBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.CadetBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Chartreuse default brush.
        /// </summary>
        public static PdfBrush Chartreuse
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Chartreuse))
                        brush = s_brushes[KnownColor.Chartreuse] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Chartreuse);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Chocolate default brush.
        /// </summary>
        public static PdfBrush Chocolate
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Chocolate))
                        brush = s_brushes[KnownColor.Chocolate] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Chocolate);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Coral default brush.
        /// </summary>
        public static PdfBrush Coral
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Coral))
                        brush = s_brushes[KnownColor.Coral] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Coral);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the CornflowerBlue default brush.
        /// </summary>
        public static PdfBrush CornflowerBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.CornflowerBlue))
                        brush = s_brushes[KnownColor.CornflowerBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.CornflowerBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Corn silk default brush.
        /// </summary>
        public static PdfBrush Cornsilk
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Cornsilk))
                        brush = s_brushes[KnownColor.Cornsilk] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Cornsilk);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Crimson default brush.
        /// </summary>
        public static PdfBrush Crimson
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Crimson))
                        brush = s_brushes[KnownColor.Crimson] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Crimson);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Cyan default brush.
        /// </summary>
        public static PdfBrush Cyan
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Cyan))
                        brush = s_brushes[KnownColor.Cyan] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Cyan);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkBlue default brush.
        /// </summary>
        public static PdfBrush DarkBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkBlue))
                        brush = s_brushes[KnownColor.DarkBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkCyan default brush.
        /// </summary>
        public static PdfBrush DarkCyan
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkCyan))
                        brush = s_brushes[KnownColor.DarkCyan] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkCyan);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkGoldenrod default brush.
        /// </summary>
        public static PdfBrush DarkGoldenrod
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkGoldenrod))
                        brush = s_brushes[KnownColor.DarkGoldenrod] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkGoldenrod);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkGray default brush.
        /// </summary>
        public static PdfBrush DarkGray
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkGray))
                        brush = s_brushes[KnownColor.DarkGray] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkGray);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkGreen default brush.
        /// </summary>
        public static PdfBrush DarkGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkGreen))
                        brush = s_brushes[KnownColor.DarkGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkKhaki default brush.
        /// </summary>
        public static PdfBrush DarkKhaki
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkKhaki))
                        brush = s_brushes[KnownColor.DarkKhaki] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkKhaki);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkMagenta default brush.
        /// </summary>
        public static PdfBrush DarkMagenta
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkMagenta))
                        brush = s_brushes[KnownColor.DarkMagenta] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkMagenta);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkOliveGreen default brush.
        /// </summary>
        public static PdfBrush DarkOliveGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkOliveGreen))
                        brush = s_brushes[KnownColor.DarkOliveGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkOliveGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkOrange default brush.
        /// </summary>
        public static PdfBrush DarkOrange
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkOrange))
                        brush = s_brushes[KnownColor.DarkOrange] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkOrange);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkOrchid default brush.
        /// </summary>
        public static PdfBrush DarkOrchid
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkOrchid))
                        brush = s_brushes[KnownColor.DarkOrchid] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkOrchid);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkRed default brush.
        /// </summary>
        public static PdfBrush DarkRed
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkRed))
                        brush = s_brushes[KnownColor.DarkRed] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkRed);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkSalmon default brush.
        /// </summary>
        public static PdfBrush DarkSalmon
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkSalmon))
                        brush = s_brushes[KnownColor.DarkSalmon] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkSalmon);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkSeaGreen default brush.
        /// </summary>
        public static PdfBrush DarkSeaGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkSeaGreen))
                        brush = s_brushes[KnownColor.DarkSeaGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkSeaGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkSlateBlue default brush.
        /// </summary>
        public static PdfBrush DarkSlateBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkSlateBlue))
                        brush = s_brushes[KnownColor.DarkSlateBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkSlateBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkSlateGray default brush.
        /// </summary>
        public static PdfBrush DarkSlateGray
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkSlateGray))
                        brush = s_brushes[KnownColor.DarkSlateGray] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkSlateGray);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkTurquoise default brush.
        /// </summary>
        public static PdfBrush DarkTurquoise
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkTurquoise))
                        brush = s_brushes[KnownColor.DarkTurquoise] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkTurquoise);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DarkViolet default brush.
        /// </summary>
        public static PdfBrush DarkViolet
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DarkViolet))
                        brush = s_brushes[KnownColor.DarkViolet] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DarkViolet);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DeepPink default brush.
        /// </summary>
        public static PdfBrush DeepPink
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DeepPink))
                        brush = s_brushes[KnownColor.DeepPink] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DeepPink);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DeepSkyBlue default brush.
        /// </summary>
        public static PdfBrush DeepSkyBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DeepSkyBlue))
                        brush = s_brushes[KnownColor.DeepSkyBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DeepSkyBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DimGray default brush.
        /// </summary>
        public static PdfBrush DimGray
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DimGray))
                        brush = s_brushes[KnownColor.DimGray] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DimGray);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the DodgerBlue default brush.
        /// </summary>
        public static PdfBrush DodgerBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.DodgerBlue))
                        brush = s_brushes[KnownColor.DodgerBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.DodgerBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Firebrick default brush.
        /// </summary>
        public static PdfBrush Firebrick
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Firebrick))
                        brush = s_brushes[KnownColor.Firebrick] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Firebrick);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the FloralWhite default brush.
        /// </summary>
        public static PdfBrush FloralWhite
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.FloralWhite))
                        brush = s_brushes[KnownColor.FloralWhite] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.FloralWhite);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the ForestGreen default brush.
        /// </summary>
        public static PdfBrush ForestGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.ForestGreen))
                        brush = s_brushes[KnownColor.ForestGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.ForestGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Fuchsia default brush.
        /// </summary>
        public static PdfBrush Fuchsia
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Fuchsia))
                        brush = s_brushes[KnownColor.Fuchsia] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Fuchsia);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Gainsborough default brush.
        /// </summary>
        public static PdfBrush Gainsboro
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Gainsboro))
                        brush = s_brushes[KnownColor.Gainsboro] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Gainsboro);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the GhostWhite default brush.
        /// </summary>
        public static PdfBrush GhostWhite
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.GhostWhite))
                        brush = s_brushes[KnownColor.GhostWhite] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.GhostWhite);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Gold default brush.
        /// </summary>
        public static PdfBrush Gold
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Gold))
                        brush = s_brushes[KnownColor.Gold] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Gold);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Goldenrod default brush.
        /// </summary>
        public static PdfBrush Goldenrod
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Goldenrod))
                        brush = s_brushes[KnownColor.Goldenrod] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Goldenrod);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Gray default brush.
        /// </summary>
        public static PdfBrush Gray
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Gray))
                        brush = s_brushes[KnownColor.Gray] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Gray);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Green default brush.
        /// </summary>
        public static PdfBrush Green
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Green))
                        brush = s_brushes[KnownColor.Green] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Green);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the GreenYellow default brush.
        /// </summary>
        public static PdfBrush GreenYellow
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.GreenYellow))
                        brush = s_brushes[KnownColor.GreenYellow] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.GreenYellow);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Honeydew default brush.
        /// </summary>
        public static PdfBrush Honeydew
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Honeydew))
                        brush = s_brushes[KnownColor.Honeydew] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Honeydew);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the HotPink default brush.
        /// </summary>
        public static PdfBrush HotPink
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.HotPink))
                        brush = s_brushes[KnownColor.HotPink] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.HotPink);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the IndianRed default brush.
        /// </summary>
        public static PdfBrush IndianRed
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.IndianRed))
                        brush = s_brushes[KnownColor.IndianRed] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.IndianRed);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Indigo default brush.
        /// </summary>
        public static PdfBrush Indigo
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Indigo))
                        brush = s_brushes[KnownColor.Indigo] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Indigo);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Ivory default brush.
        /// </summary>
        public static PdfBrush Ivory
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Ivory))
                        brush = s_brushes[KnownColor.Ivory] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Ivory);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Khaki default brush.
        /// </summary>
        public static PdfBrush Khaki
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Khaki))
                        brush = s_brushes[KnownColor.Khaki] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Khaki);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Lavender default brush.
        /// </summary>
        public static PdfBrush Lavender
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Lavender))
                        brush = s_brushes[KnownColor.Lavender] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Lavender);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LavenderBlush default brush.
        /// </summary>
        public static PdfBrush LavenderBlush
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LavenderBlush))
                        brush = s_brushes[KnownColor.LavenderBlush] as PdfBrush;


                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LavenderBlush);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LawnGreen default brush.
        /// </summary>
        public static PdfBrush LawnGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LawnGreen))
                        brush = s_brushes[KnownColor.LawnGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LawnGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LemonChiffon default brush.
        /// </summary>
        public static PdfBrush LemonChiffon
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LemonChiffon))
                        brush = s_brushes[KnownColor.LemonChiffon] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LemonChiffon);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightBlue default brush.
        /// </summary>
        public static PdfBrush LightBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightBlue))
                        brush = s_brushes[KnownColor.LightBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightCoral default brush.
        /// </summary>
        public static PdfBrush LightCoral
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightCoral))
                        brush = s_brushes[KnownColor.LightCoral] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightCoral);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightCyan default brush.
        /// </summary>
        public static PdfBrush LightCyan
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightCyan))
                        brush = s_brushes[KnownColor.LightCyan] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightCyan);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightGoldenrodYellow default brush.
        /// </summary>
        public static PdfBrush LightGoldenrodYellow
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightGoldenrodYellow))
                        brush = s_brushes[KnownColor.LightGoldenrodYellow] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightGoldenrodYellow);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightGray default brush.
        /// </summary>
        public static PdfBrush LightGray
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightGray))
                        brush = s_brushes[KnownColor.LightGray] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightGray);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightGreen default brush.
        /// </summary>
        public static PdfBrush LightGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightGreen))
                        brush = s_brushes[KnownColor.LightGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightPink default brush.
        /// </summary>
        public static PdfBrush LightPink
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightPink))
                        brush = s_brushes[KnownColor.LightPink] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightPink);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightSalmon default brush.
        /// </summary>
        public static PdfBrush LightSalmon
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightSalmon))
                        brush = s_brushes[KnownColor.LightSalmon] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightSalmon);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightSeaGreen default brush.
        /// </summary>
        public static PdfBrush LightSeaGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightSeaGreen))
                        brush = s_brushes[KnownColor.LightSeaGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightSeaGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightSkyBlue default brush.
        /// </summary>
        public static PdfBrush LightSkyBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightSkyBlue))
                        brush = s_brushes[KnownColor.LightSkyBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightSkyBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightSlateGray default brush.
        /// </summary>
        public static PdfBrush LightSlateGray
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightSlateGray))
                        brush = s_brushes[KnownColor.LightSlateGray] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightSlateGray);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightSteelBlue default brush.
        /// </summary>
        public static PdfBrush LightSteelBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightSteelBlue))
                        brush = s_brushes[KnownColor.LightSteelBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightSteelBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LightYellow default brush.
        /// </summary>
        public static PdfBrush LightYellow
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LightYellow))
                        brush = s_brushes[KnownColor.LightYellow] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LightYellow);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Lime default brush.
        /// </summary>
        public static PdfBrush Lime
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Lime))
                        brush = s_brushes[KnownColor.Lime] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Lime);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the LimeGreen default brush.
        /// </summary>
        public static PdfBrush LimeGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.LimeGreen))
                        brush = s_brushes[KnownColor.LimeGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.LimeGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Linen default brush.
        /// </summary>
        public static PdfBrush Linen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Linen))
                        brush = s_brushes[KnownColor.Linen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Linen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Magenta default brush.
        /// </summary>
        public static PdfBrush Magenta
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Magenta))
                        brush = s_brushes[KnownColor.Magenta] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Magenta);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Maroon default brush.
        /// </summary>
        public static PdfBrush Maroon
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Maroon))
                        brush = s_brushes[KnownColor.Maroon] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Maroon);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumAquamarine default brush.
        /// </summary>
        public static PdfBrush MediumAquamarine
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumAquamarine))
                        brush = s_brushes[KnownColor.MediumAquamarine] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumAquamarine);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumBlue default brush.
        /// </summary>
        public static PdfBrush MediumBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumBlue))
                        brush = s_brushes[KnownColor.MediumBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumOrchid default brush.
        /// </summary>
        public static PdfBrush MediumOrchid
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumOrchid))
                        brush = s_brushes[KnownColor.MediumOrchid] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumOrchid);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumPurple default brush.
        /// </summary>
        public static PdfBrush MediumPurple
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumPurple))
                        brush = s_brushes[KnownColor.MediumPurple] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumPurple);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumSeaGreen default brush.
        /// </summary>
        public static PdfBrush MediumSeaGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumSeaGreen))
                        brush = s_brushes[KnownColor.MediumSeaGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumSeaGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumSlateBlue default brush.
        /// </summary>
        public static PdfBrush MediumSlateBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumSlateBlue))
                        brush = s_brushes[KnownColor.MediumSlateBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumSlateBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumSpringGreen default brush.
        /// </summary>
        public static PdfBrush MediumSpringGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumSpringGreen))
                        brush = s_brushes[KnownColor.MediumSpringGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumSpringGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumTurquoise default brush.
        /// </summary>
        public static PdfBrush MediumTurquoise
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumTurquoise))
                        brush = s_brushes[KnownColor.MediumTurquoise] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumTurquoise);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MediumVioletRed default brush.
        /// </summary>
        public static PdfBrush MediumVioletRed
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MediumVioletRed))
                        brush = s_brushes[KnownColor.MediumVioletRed] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MediumVioletRed);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MidnightBlue default brush.
        /// </summary>
        public static PdfBrush MidnightBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MidnightBlue))
                        brush = s_brushes[KnownColor.MidnightBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MidnightBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MintCream default brush.
        /// </summary>
        public static PdfBrush MintCream
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MintCream))
                        brush = s_brushes[KnownColor.MintCream] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MintCream);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the MistyRose default brush.
        /// </summary>
        public static PdfBrush MistyRose
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.MistyRose))
                        brush = s_brushes[KnownColor.MistyRose] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.MistyRose);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Moccasin default brush.
        /// </summary>
        public static PdfBrush Moccasin
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Moccasin))
                        brush = s_brushes[KnownColor.Moccasin] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Moccasin);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the NavajoWhite default brush.
        /// </summary>
        public static PdfBrush NavajoWhite
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.NavajoWhite))
                        brush = s_brushes[KnownColor.NavajoWhite] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.NavajoWhite);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Navy default brush.
        /// </summary>
        public static PdfBrush Navy
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Navy))
                        brush = s_brushes[KnownColor.Navy] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Navy);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the OldLace default brush.
        /// </summary>
        public static PdfBrush OldLace
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.OldLace))
                        brush = s_brushes[KnownColor.OldLace] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.OldLace);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Olive default brush.
        /// </summary>
        public static PdfBrush Olive
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Olive))
                        brush = s_brushes[KnownColor.Olive] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Olive);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the OliveDrab default brush.
        /// </summary>
        public static PdfBrush OliveDrab
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.OliveDrab))
                        brush = s_brushes[KnownColor.OliveDrab] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.OliveDrab);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Orange default brush.
        /// </summary>
        public static PdfBrush Orange
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Orange))
                        brush = s_brushes[KnownColor.Orange] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Orange);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the OrangeRed default brush.
        /// </summary>
        public static PdfBrush OrangeRed
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.OrangeRed))
                        brush = s_brushes[KnownColor.OrangeRed] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.OrangeRed);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Orchid default brush.
        /// </summary>
        public static PdfBrush Orchid
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Orchid))
                        brush = s_brushes[KnownColor.Orchid] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Orchid);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the PaleGoldenrod default brush.
        /// </summary>
        public static PdfBrush PaleGoldenrod
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.PaleGoldenrod))
                        brush = s_brushes[KnownColor.PaleGoldenrod] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.PaleGoldenrod);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the PaleGreen default brush.
        /// </summary>
        public static PdfBrush PaleGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.PaleGreen))
                        brush = s_brushes[KnownColor.PaleGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.PaleGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the PaleTurquoise default brush.
        /// </summary>
        public static PdfBrush PaleTurquoise
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.PaleTurquoise))
                        brush = s_brushes[KnownColor.PaleTurquoise] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.PaleTurquoise);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the PaleVioletRed default brush.
        /// </summary>
        public static PdfBrush PaleVioletRed
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.PaleVioletRed))
                        brush = s_brushes[KnownColor.PaleVioletRed] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.PaleVioletRed);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the PapayaWhip default brush.
        /// </summary>
        public static PdfBrush PapayaWhip
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.PapayaWhip))
                        brush = s_brushes[KnownColor.PapayaWhip] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.PapayaWhip);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the PeachPuff default brush.
        /// </summary>
        public static PdfBrush PeachPuff
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.PeachPuff))
                        brush = s_brushes[KnownColor.PeachPuff] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.PeachPuff);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Peru default brush.
        /// </summary>
        public static PdfBrush Peru
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Peru))
                        brush = s_brushes[KnownColor.Peru] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Peru);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Pink default brush.
        /// </summary>
        public static PdfBrush Pink
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Pink))
                        brush = s_brushes[KnownColor.Pink] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Pink);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Plum default brush.
        /// </summary>
        public static PdfBrush Plum
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Plum))
                        brush = s_brushes[KnownColor.Plum] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Plum);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the PowderBlue default brush.
        /// </summary>
        public static PdfBrush PowderBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.PowderBlue))
                        brush = s_brushes[KnownColor.PowderBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.PowderBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Purple default brush.
        /// </summary>
        public static PdfBrush Purple
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Purple))
                        brush = s_brushes[KnownColor.Purple] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Purple);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Red default brush.
        /// </summary>
        public static PdfBrush Red
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Red))
                        brush = s_brushes[KnownColor.Red] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Red);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the RosyBrown default brush.
        /// </summary>
        public static PdfBrush RosyBrown
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.RosyBrown))
                        brush = s_brushes[KnownColor.RosyBrown] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.RosyBrown);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the RoyalBlue default brush.
        /// </summary>
        public static PdfBrush RoyalBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.RoyalBlue))
                        brush = s_brushes[KnownColor.RoyalBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.RoyalBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SaddleBrown default brush.
        /// </summary>
        public static PdfBrush SaddleBrown
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SaddleBrown))
                        brush = s_brushes[KnownColor.SaddleBrown] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SaddleBrown);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Salmon default brush.
        /// </summary>
        public static PdfBrush Salmon
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Salmon))
                        brush = s_brushes[KnownColor.Salmon] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Salmon);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SandyBrown default brush.
        /// </summary>
        public static PdfBrush SandyBrown
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SandyBrown))
                        brush = s_brushes[KnownColor.SandyBrown] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SandyBrown);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SeaGreen default brush.
        /// </summary>
        public static PdfBrush SeaGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SeaGreen))
                        brush = s_brushes[KnownColor.SeaGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SeaGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SeaShell default brush.
        /// </summary>
        public static PdfBrush SeaShell
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SeaShell))
                        brush = s_brushes[KnownColor.SeaShell] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SeaShell);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Sienna default brush.
        /// </summary>
        public static PdfBrush Sienna
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Sienna))
                        brush = s_brushes[KnownColor.Sienna] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Sienna);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Silver default brush.
        /// </summary>
        public static PdfBrush Silver
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Silver))
                        brush = s_brushes[KnownColor.Silver] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Silver);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SkyBlue default brush.
        /// </summary>
        public static PdfBrush SkyBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SkyBlue))
                        brush = s_brushes[KnownColor.SkyBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SkyBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SlateBlue default brush.
        /// </summary>
        public static PdfBrush SlateBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SlateBlue))
                        brush = s_brushes[KnownColor.SlateBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SlateBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SlateGray default brush.
        /// </summary>
        public static PdfBrush SlateGray
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SlateGray))
                        brush = s_brushes[KnownColor.SlateGray] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SlateGray);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Snow default brush.
        /// </summary>
        public static PdfBrush Snow
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Snow))
                        brush = s_brushes[KnownColor.Snow] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Snow);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SpringGreen default brush.
        /// </summary>
        public static PdfBrush SpringGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SpringGreen))
                        brush = s_brushes[KnownColor.SpringGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SpringGreen);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the SteelBlue default brush.
        /// </summary>
        public static PdfBrush SteelBlue
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.SteelBlue))
                        brush = s_brushes[KnownColor.SteelBlue] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.SteelBlue);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Tan default brush.
        /// </summary>
        public static PdfBrush Tan
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Tan))
                        brush = s_brushes[KnownColor.Tan] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Tan);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Teal default brush.
        /// </summary>
        public static PdfBrush Teal
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Teal))
                        brush = s_brushes[KnownColor.Teal] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Teal);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Thistle default brush.
        /// </summary>
        public static PdfBrush Thistle
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Thistle))
                        brush = s_brushes[KnownColor.Thistle] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Thistle);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Tomato default brush.
        /// </summary>
        public static PdfBrush Tomato
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Tomato))
                        brush = s_brushes[KnownColor.Tomato] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Tomato);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Transparent default brush.
        /// </summary>
        public static PdfBrush Transparent
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Transparent))
                        brush = s_brushes[KnownColor.Transparent] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Transparent);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Turquoise default brush.
        /// </summary>
        public static PdfBrush Turquoise
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Turquoise))
                        brush = s_brushes[KnownColor.Turquoise] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Turquoise);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Violet default brush.
        /// </summary>
        public static PdfBrush Violet
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Violet))
                        brush = s_brushes[KnownColor.Violet] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Violet);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Wheat default brush.
        /// </summary>
        public static PdfBrush Wheat
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Wheat))
                        brush = s_brushes[KnownColor.Wheat] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Wheat);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the White default brush.
        /// </summary>
        public static PdfBrush White
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.White))
                        brush = s_brushes[KnownColor.White] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.White);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the WhiteSmoke default brush.
        /// </summary>
        public static PdfBrush WhiteSmoke
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.WhiteSmoke))
                        brush = s_brushes[KnownColor.WhiteSmoke] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.WhiteSmoke);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the Yellow default brush.
        /// </summary>
        public static PdfBrush Yellow
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.Yellow))
                        brush = s_brushes[KnownColor.Yellow] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.Yellow);
                    }

                    return brush;
                }
            }
        }

        /// <summary>
        /// Gets the YellowGreen default brush.
        /// </summary>
        public static PdfBrush YellowGreen
        {
            get
            {
                lock (s_brushes)
                {
                    PdfBrush brush = null;

                    if (s_brushes.ContainsKey(KnownColor.YellowGreen))
                        brush = s_brushes[KnownColor.YellowGreen] as PdfBrush;

                    if (brush == null)
                    {
                        brush = GetBrush(KnownColor.YellowGreen);
                    }

                    return brush;
                }
            }
        }
        #endregion


        #region Static Methods
        /// <summary>
        /// Creates the default brush.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="colorName">Name of the color.</param>
        /// <returns>The proper PdfBrush instance.</returns>
        private static PdfBrush GetBrush(KnownColor colorName)
        {
#if SILVERLIGHT || NETFX_CORE || WP
            Color color = ColorConverter.FromKnownColor(colorName);
#else
            Color color = System.Drawing.Color.FromKnownColor(colorName);
#endif
            PdfColor pdfColor = new PdfColor(color);

            PdfBrush brush = new PdfSolidBrush(pdfColor, true);

            s_brushes[color] = brush;

            return brush;
        }
        #endregion
    }
}
