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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Documentation;

namespace Syncfusion.Drawing
{

	/// <summary>
	/// Holds information on how to fill the background of a window or grid cell.
	/// </summary>
	/// <remarks>
	/// BrushInfo lets you specify a solid backcolor, gradient or pattern style with both
	/// back and forecolor.
	/// <para/>
	/// This information can be persisted with serialization.
	/// <para/>
	/// You can also convert the information to a string and recreate it from a string.
	/// <para/>
	/// BrushInfo is immutable (just like <see cref="System.String"/>). You cannot change its values.
	/// Instead you have to create a new BrushInfo object.
	/// <para/>
	/// BrushInfo can also be persisted into code when used as a property in a component designer.
	/// <para/>
	/// <see cref="BrushPaint.FillRectangle(System.Drawing.Graphics, System.Drawing.Rectangle, 
	/// Syncfusion.Drawing.BrushInfo)"/> shows how to fill a rectangle using <see cref="BrushInfo"/> 
	/// information.
	/// </remarks>
	[
	TypeConverter(typeof(BrushInfoConverter)),
	DesignerSerializer(typeof(BrushInfoCodeDomSerializer),
		typeof(CodeDomSerializer)),
	Serializable,
	RefreshProperties(RefreshProperties.Repaint),
	ImmutableObject(true),
	]
	public sealed class BrushInfo: IFormattable, ICloneable, ISerializable, IXmlSerializable
	{
		//private Color backColor;
		//private Color foreColor;
		private byte style;
		private byte styleInfo;
		// The backcolor is the first entry in this list and the forecolor is the LAST (not the 2nd) entry in the list.
		private BrushInfoColorArrayList gradientColors;

		private const char separator = ';';
		private static readonly char[] separators = new char[] {separator};

		/// <summary>
		/// An empty BrushInfo.
		/// </summary>
		public static BrushInfo Empty
		{
			get
			{
				return new BrushInfo();
			}
		}

		// ctor's

		private void InitGradientColors(Color backColor, Color foreColor)
		{
			this.gradientColors = new BrushInfoColorArrayList();
			this.gradientColors.Add(backColor);
			this.gradientColors.Add(foreColor);
			// This array can no longer be edited.
			this.gradientColors.Freeze = true;
		}
		private void InitGradientColors(BrushInfoColorArrayList list)
		{
			this.gradientColors = new BrushInfoColorArrayList();
			foreach(Color color in list)
				this.gradientColors.Add(color);

			if(this.gradientColors.Count < 2)
				this.gradientColors.Add(SystemColors.WindowText);
			if(this.gradientColors.Count < 2)
				this.gradientColors.Insert(0, SystemColors.Window);
			// This array can no longer be edited.
			this.gradientColors.Freeze = true;
		}
		/// <summary>
		/// Overloaded. Initializes a new empty instance of BrushInfo.
		/// </summary>
		public BrushInfo()
		{
			this.style = (byte) BrushStyle.None;
			this.styleInfo = (byte) 0;
			this.InitGradientColors(SystemColors.Window, SystemColors.WindowText);
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with the specified solid backcolor.
		/// </summary>
		/// <param name="color">A <see cref="Color"/> used as solid background.</param>
		public BrushInfo(Color color)
		{
			this.style = (byte) BrushStyle.Solid;
			this.styleInfo = (byte) 0;
			this.InitGradientColors(color, SystemColors.WindowText);
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with a solid backcolor and forecolor.
		/// </summary>
		BrushInfo(Color foreColor, Color backColor)
		{
			this.style = (byte) BrushStyle.Solid;
			this.styleInfo = (byte) 0;
			this.InitGradientColors(backColor, foreColor);
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with a hatch style and pattern colors.
		/// </summary>
		/// <param name="hatchStyle">A <see cref="PatternStyle"/>.</param>
		/// <param name="foreColor">A <see cref="Color"/> used for the pattern.</param>
		/// <param name="backColor">A <see cref="Color"/> used for the pattern.</param>
		public BrushInfo(PatternStyle hatchStyle, Color foreColor, Color backColor)
		{
			if (!Enum.IsDefined(typeof(PatternStyle), hatchStyle))
				throw new ArgumentException("Invalid PatternStyle value");

			this.style = (byte) BrushStyle.Pattern;
			this.styleInfo = (byte) hatchStyle;
			this.InitGradientColors(backColor, foreColor);
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with a hatch style and pattern colors.
		/// </summary>
		/// <param name="hatchStyle">A <see cref="PatternStyle"/>.</param>
		/// <param name="colors">List of colors.</param>
		public BrushInfo(PatternStyle hatchStyle, BrushInfoColorArrayList colors)
		{
			if (!Enum.IsDefined(typeof(PatternStyle), hatchStyle))
				throw new ArgumentException("Invalid PatternStyle value");

			this.style = (byte) BrushStyle.Pattern;
			this.styleInfo = (byte) hatchStyle;
			this.InitGradientColors(colors);
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with a hatch style and pattern colors.
		/// </summary>
		/// <param name="hatchStyle">A <see cref="PatternStyle"/>.</param>
		/// <param name="colors">List of colors.</param>
		public BrushInfo(PatternStyle hatchStyle, Color[] colors)
		{
			if (!Enum.IsDefined(typeof(PatternStyle), hatchStyle))
				throw new ArgumentException("Invalid PatternStyle value");

			this.style = (byte) BrushStyle.Pattern;
			this.styleInfo = (byte) hatchStyle;
			this.InitGradientColors(new BrushInfoColorArrayList(colors));
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with a gradient style and gradient fill colors.
		/// </summary>
		/// <param name="gradientStyle">A <see cref="PatternStyle"/>.</param>
		/// <param name="foreColor">A <see cref="Color"/> used for the gradient fill.</param>
		/// <param name="backColor">A <see cref="Color"/> used for the gradient fill.</param>
		public BrushInfo(GradientStyle gradientStyle, Color foreColor, Color backColor)
		{
			if (!Enum.IsDefined(typeof(GradientStyle), gradientStyle))
				throw new ArgumentException("Invalid GradientStyle value");

			this.style = (byte) BrushStyle.Gradient;
			this.styleInfo = (byte) gradientStyle;
			this.InitGradientColors(backColor, foreColor);
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with a gradient style and gradient fill colors.
		/// </summary>
		/// <param name="gradientStyle">A <see cref="PatternStyle"/>.</param>
		/// <param name="colors">List of gradient fill colors.</param>
		public BrushInfo(GradientStyle gradientStyle, BrushInfoColorArrayList colors)
		{
			if (!Enum.IsDefined(typeof(GradientStyle), gradientStyle))
				throw new ArgumentException("Invalid GradientStyle value");

			this.style = (byte) BrushStyle.Gradient;
			this.styleInfo = (byte) gradientStyle;
			this.InitGradientColors(colors);
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with a hatch style and pattern colors.
		/// </summary>
		/// <param name="gradientStyle">A <see cref="PatternStyle"/>.</param>
		/// <param name="colors">List of colors.</param>
		public BrushInfo(GradientStyle gradientStyle, Color[] colors)
		{
			if (!Enum.IsDefined(typeof(GradientStyle), gradientStyle))
				throw new ArgumentException("Invalid GradientStyle value");

			this.style = (byte) BrushStyle.Gradient;
			this.styleInfo = (byte) gradientStyle;
			this.InitGradientColors(new BrushInfoColorArrayList(colors));
		}


		/// <summary>
		/// Initializes a new instance of BrushInfo with any BrushStyle. Internal only.
		/// </summary>
		internal BrushInfo(BrushStyle style, object styleInfo, Color foreColor, Color backColor)
		{
			if (!Enum.IsDefined(typeof(BrushStyle), style))
				throw new ArgumentException("Invalid BrushStyle value");

			this.style = (byte) style;
			this.styleInfo = (byte) (int) styleInfo;
			this.InitGradientColors(backColor, foreColor);
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo with a new alpha-blend value and copies other information from a given BrushInfo.
		/// </summary>
		/// <param name="alpha">The alpha value that should be applied to the forecolor and backcolor of the new brush.</param>
		/// <param name="br">A BrushInfo that holds information for this BrushInfo.</param>
		public BrushInfo(int alpha, BrushInfo br)
			: this(br.Style, (int) br.styleInfo, Color.FromArgb(alpha, br.ForeColor), Color.FromArgb(alpha, br.BackColor))
		{
		}

		/// <summary>
		/// Initializes a new instance of BrushInfo and copies its information from a given BrushInfo.
		/// </summary>
		/// <param name="brush">A BrushInfo that holds information for this BrushInfo.</param>
		public BrushInfo(BrushInfo brush)
		{
			this.style = brush.style;
			this.styleInfo = brush.styleInfo;
			this.InitGradientColors(brush.gradientColors);
		}

		/// <summary>
		/// Creates a new BrushInfo object and initializes it from a string.
		/// </summary>
		/// <param name="s">A string in the format BrushStyle;Style;ForeColor;BackColor.</param>
		/// <returns>A new BrushInfo object.</returns>
		public static BrushInfo Parse(string s)
		{
			BrushInfo brush = new BrushInfo();
			brush.gradientColors.Freeze = false;
			brush.SetDescription(s);
			brush.gradientColors.Freeze = true;
			return brush;
		}

		BrushInfo(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#endif


			SerializationInfoEnumerator sie = info.GetEnumerator();

			// old versions may not have it, if a new version is loaded then this
			// initialized list will be replaced by the loaded one.
			this.InitGradientColors(SystemColors.Window, SystemColors.WindowText);

			while (sie.MoveNext())
			{
				switch(sie.Name)
				{
					case "GradientColors":
						this.gradientColors = (BrushInfoColorArrayList)sie.Value;
						break;
					case "Style":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (sie.Value is string)
							// This is faster than calling info.GetByte("Style");
							style = (byte)Convert.ChangeType(sie.Value, typeof(byte));
						else
							style = (byte)sie.Value;

						break;
					case "StyleInfo":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (sie.Value is string)
							// This is faster than calling info.GetByte("StyleInfo");
							styleInfo = (byte)Convert.ChangeType(sie.Value, typeof(byte));
						else
							styleInfo = (byte)sie.Value;
						break;
						// To preserve compatibility
					case "BackColor":
						this.gradientColors.Freeze = false;
						this.gradientColors[0] = (Color) sie.Value;
						this.gradientColors.Freeze = true;
						break;
					case "ForeColor":
						this.gradientColors.Freeze = false;
						this.gradientColors[this.gradientColors.Count - 1] = (Color) sie.Value;
						this.gradientColors.Freeze = true;
						break;
				}
			}
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#endif


			info.AddValue("GradientColors", this.gradientColors); // BrushInfoColorArrayList
			info.AddValue("Style", style); // Byte
			info.AddValue("StyleInfo", styleInfo); // Byte
		}

		/// <summary>
		/// Overloaded. Returns the string representation of the brush in the format BrushStyle;Style;ForeColor;BackColor.
		/// </summary>
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		/// <summary>
		/// Returns the string representation of the brush in the format BrushStyle;Style;ForeColor;BackColor.
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public string ToString(IFormatProvider provider)
		{
			return this.ToString(null, provider);
		}

		/// <summary>
		/// Returns the string representation of the brush in the format BrushStyle;Style;ForeColor;BackColor.
		/// </summary>
		/// <param name="format">Specifies the format for string. NULL for default, "compact" for a compact string, "G" for more descriptive text.</param>
		/// <returns></returns>
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		/// <summary>
		/// Returns the string representation of the brush in the format BrushStyle;Style;ForeColor;BackColor.
		/// </summary>
		/// <param name="format">Specifies the format for string. NULL for default, "compact", for a compact string or "G" for more descriptive text.</param>
		/// <param name="formatProvider">
		/// The IFormatProvider to use to format the value. <para/>
		/// -or- <para/>
		/// A <see langword="NULL"/> reference to obtain the numeric format information from the current locale setting
		/// of the operating system.
		///</param>
		/// <returns></returns>
		public string /*IFormattable*/ ToString(string format, IFormatProvider formatProvider)
		{
			StringBuilder sb = new StringBuilder();
			string separate = separator.ToString() + " ";
			ColorConverter cc = new ColorConverter();
			if ( format != null && format == "compact" )
			{
				sb.Append(Style.ToString());

				if (Style == BrushStyle.Pattern)
				{
					sb.Append(String.Concat(new String[]
					{
						separate,
						PatternStyle.ToString(),
						separate,
						//ColorConvert.ColorToString(ForeColor, false),
						cc.ConvertToString(null, CultureInfo.InvariantCulture, ForeColor)
					}));
				}
				else if (Style == BrushStyle.Gradient)
				{
					sb.Append(String.Concat(new String[]
					{
						separate,
						GradientStyle.ToString(),
						separate,
						//ColorConvert.ColorToString(ForeColor, false),
						cc.ConvertToString(null, CultureInfo.InvariantCulture, ForeColor)
					}));
				}

				if (Style != BrushStyle.None)
				{
					sb.Append(String.Concat(new String[]
					{
						separate,
						//ColorConvert.ColorToString(BackColor, false),
						cc.ConvertToString(null, CultureInfo.InvariantCulture, BackColor)
					}));

					if(Style != BrushStyle.Solid)
					{
						// Add the remining colors for the gradient style.
						if(this.gradientColors.Count > 2)
						{
							for(int i = 1; i < this.gradientColors.Count - 1; i++)
							{
								sb.Append(String.Concat(new String[]
				  {
					  separate,
					  //ColorConvert.ColorToString(this.gradientColors[i], false)
					  cc.ConvertToString(null, CultureInfo.InvariantCulture, this.gradientColors[i])
				  }));
							}
						}
					}
				}
			}
			else
			{
				sb.Append(Enum.Format(typeof(BrushStyle), Style, "G"));

				if (Style == BrushStyle.Pattern)
				{
					sb.Append(String.Concat(new String[]
					{
						separate,
						Enum.Format(typeof(PatternStyle), PatternStyle, "G"),
						separate,
						//ColorConvert.ColorToString(ForeColor, true),
						cc.ConvertToString(null, CultureInfo.InvariantCulture, ForeColor)
					}));
				}
				else if (Style == BrushStyle.Gradient)
				{
					sb.Append(String.Concat(new String[]
					{
						separate,
						Enum.Format(typeof(GradientStyle), GradientStyle, "G"),
						separate,
						//ColorConvert.ColorToString(ForeColor, true),
						cc.ConvertToString(null, CultureInfo.InvariantCulture, ForeColor)
					}));
				}

				if (Style != BrushStyle.None)
				{
					sb.Append(String.Concat(new String[]
					{
						separate,
						//ColorConvert.ColorToString(BackColor, true),
						cc.ConvertToString(null, CultureInfo.InvariantCulture, BackColor)
					}));

					if(Style != BrushStyle.Solid)
					{
						// Add the remining colors for the gradient style.
						if(this.gradientColors.Count > 2)
						{
							for(int i = 1; i < this.gradientColors.Count - 1; i++)
							{
								sb.Append(String.Concat(new String[]
				  {
					  separate,
					  //ColorConvert.ColorToString(this.gradientColors[i], false)
					  cc.ConvertToString(null, CultureInfo.InvariantCulture, this.gradientColors[i])
				  }));
							}
						}
					}
				}
			}

			return sb.ToString();
		}


		/// <summary>
		/// Overridden. Compares two BrushInfo object and indicates whether they are equal.
		/// </summary>
		/// <param name="obj">The <see cref="BrushInfo"/> to compare with the current <see cref="BrushInfo"/>. </param>
		/// <returns>True if the specified Object is equal to the current <see cref="BrushInfo"/>; false otherwise.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is BrushInfo))
				return false;

			BrushInfo brush = (BrushInfo) obj;

			bool isEqual = (this.style == brush.style);
			if (isEqual && (BrushStyle) this.style != BrushStyle.None)
			{
				isEqual = (this.BackColor == brush.BackColor);
				if (isEqual && (BrushStyle) this.style != BrushStyle.Solid)
				{
					isEqual = this.gradientColors.Count == brush.gradientColors.Count
						&& this.styleInfo == brush.styleInfo;
					for(int i = 0; i < this.gradientColors.Count && isEqual; i++)
					{
						isEqual = this.gradientColors[i] == brush.gradientColors[i];
					}
				}
			}
			return isEqual;
		}

				public static bool operator==( BrushInfo lhs, BrushInfo rhs )
				{
					if((object)lhs == null && (object)rhs == null)
						return true;
					if ((object) lhs == null || (object) rhs == null)
						return false;
		
					return lhs.Equals(rhs);
				}
		
		
				public static bool operator!=( BrushInfo lhs, BrushInfo rhs )
				{
					if((object)lhs == null && (object)rhs == null)
						return false;
		
					if ((object) lhs == null || (object) rhs == null)
						return true;
		
					return !lhs.Equals(rhs);
				}


		/// <override/>
		public override int GetHashCode()
		{
			int hash = ( ((Int32) this.Style) << 28 );
			if (this.style > 0)
				hash = hash | ( this.BackColor.ToArgb()&0x00fffff );
			if (this.style > 1)
				hash = hash | ( ((Int32) this.styleInfo) << 24 );
			return hash;
		}

		// ICloneable interface
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public BrushInfo Clone()
		{
			return new BrushInfo(this);
		}

		/// <summary>
		/// Creates a new BrushInfo with the same style but with only black and white colors.
		/// </summary>
		/// <returns>A new object with the same style and black and white colors.</returns>
		public BrushInfo MakeBlackAndWhite()
		{
			BrushInfo br = new BrushInfo(this);

			br.gradientColors.Freeze = false;
			br.gradientColors[0] = Color.White;
			br.gradientColors[1] = Color.Black;
			if(br.gradientColors.Count > 2)
				br.gradientColors.RemoveRange(2, br.gradientColors.Count - 2);

			br.gradientColors.Freeze = true;

			return br;
		}

		/// <summary>
		/// Returns a string id that you can use to store the BrushInfo in a HashTable.
		/// </summary>
		/// <returns>A string with compact identitiy information about the BrushInfo.</returns>
		public String GetBrushKey()
		{
			return ToString("compact");
		}

		private BrushInfo SetDescription(string brushDescription)
		{
			string s = brushDescription;

			if (s == null)
				return this;

			string[] words = s.Split(separators);

			int wordCount = words.GetLength(0);
			if (wordCount == 0) return this;
			int n = 0;
			ColorConverter cc = new ColorConverter();

			// Style
			if (words[n].Length > 0)
				SetStyle((BrushStyle) Enum.Parse(typeof(BrushStyle), words[n], true));
			if (wordCount == ++n) return this;

			// Pattern or Gradient
			if (Style != BrushStyle.Solid)
			{
				if (words[n].Length > 0)
				{
					if (Style == BrushStyle.Pattern)
						SetPatternStyle((PatternStyle) Enum.Parse(typeof(PatternStyle), words[n], true));
					else if (Style == BrushStyle.Gradient)
						SetGradientStyle((GradientStyle) Enum.Parse(typeof(GradientStyle), words[n], true));
				}
				if (wordCount == ++n) return this;

				// ForeColor
				if (words[n].Length > 0)
				{
					Color color;
					// Color never gets converted to string with ";" as separators between RGB values, so commenting out.
					//                    if (Char.IsDigit(words[n].Trim()[0]) && n+3 <= wordCount)
					//                    {
					//			    	    color = ColorConvert.ColorFromString(words[n]+";"+words[n+1]+";"+words[n+2]);
					//                        n += 2;
					//                    }
					//                    else

					// Not using this because we need to specify invariant culture.
					//color = ColorConvert.ColorFromString(words[n]);
					color = (Color)cc.ConvertFromString(null, CultureInfo.InvariantCulture, words[n]);

					SetForeColor(color);
					if (wordCount == ++n) return this;
				}
			}

			// BackColor
			if (Style != BrushStyle.None)
			{
				if (words[n].Length > 0)
				{
					Color color;
					// Color never gets converted to string with ";" as separators between RGB values, so commenting out.
					//                    if (Char.IsDigit(words[n].Trim()[0]) && n+3 <= wordCount)
					//                    {
					//			    	    color = ColorConvert.ColorFromString(words[n]+";"+words[n+1]+";"+words[n+2]);
					//                        n += 2;
					//                    }
					//                    else
					// Not using this because we need to specify invariant culture.
					//color = ColorConvert.ColorFromString(words[n]);
					color = (Color)cc.ConvertFromString(null, CultureInfo.InvariantCulture, words[n]);
					SetBackColor(color);
				}
				//if (wordCount == ++n) return this;

				while(++n < wordCount)
				{
					// There are more colors between the backcolor and forecolor
					if (words[n].Length > 0)
					{
						string scolor = words[n].Trim();
						if(scolor == String.Empty)
							continue;
						Color color;
						// Color never gets converted to string with ";" as separators between RGB values, so commenting out.
						//						if (Char.IsDigit(words[n].Trim()[0]) && n+3 <= wordCount)
						//						{
						//							color = ColorConvert.ColorFromString(words[n]+";"+words[n+1]+";"+words[n+2]);
						//							n += 2;
						//						}
						//						else

						// Not using this because we need to specify invariant culture.
						//color = ColorConvert.ColorFromString(words[n].Trim());
						color = (Color)cc.ConvertFromString(null, CultureInfo.InvariantCulture, words[n].Trim());
						this.gradientColors.Insert(this.gradientColors.Count - 1, color);
					}
				}
			}

			if (wordCount != n)
				throw new FormatException("obsolete arguments: " + words[n]);

			return this;
		}

		private String GetDescription()
		{
			return ToString("compact");
		}

		/// <summary>
		/// Returns a string description of the BrushInfo. See <see cref="ToString(string)"/>.
		/// </summary>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public string Description
		{
			get
			{
				return GetDescription();
			}
		}

		/// <summary>
		/// Indicates whether this is an empty object.
		/// </summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),]
		public bool IsEmpty
		{
			get { return GetStyle() == BrushStyle.None; }
		}

		private BrushStyle GetStyle()
		{
			return (BrushStyle) style;
		}
		private BrushInfo SetStyle(BrushStyle style)
		{
			if (!Enum.IsDefined(typeof(BrushStyle), style))
				throw new ArgumentException("Invalid BrushStyle value");
			this.style = (byte) style;
			return this;
		}

		private Color GetBackColor()
		{
			return this.gradientColors[0];
		}
		private BrushInfo SetBackColor(Color color)
		{
			this.gradientColors[0] = color;
			return this;
		}

		private PatternStyle GetPatternStyle()
		{
			if (Style != BrushStyle.Pattern)
				return PatternStyle.None;
			return (PatternStyle) styleInfo;
		}
		private BrushInfo SetPatternStyle(PatternStyle hatchStyle)
		{
			if (!Enum.IsDefined(typeof(PatternStyle), hatchStyle))
				throw new ArgumentException("Invalid PatternStyle value");
			style = (byte) BrushStyle.Pattern;
			styleInfo = (byte) hatchStyle;
			return this;
		}

		private GradientStyle GetGradientStyle()
		{
			if (Style != BrushStyle.Gradient)
				return GradientStyle.None;
			//throw new InvalidOperationException();
			return (GradientStyle) styleInfo;
		}
		private BrushInfo SetGradientStyle(GradientStyle gradientStyle)
		{
			if (!Enum.IsDefined(typeof(GradientStyle), gradientStyle))
				throw new ArgumentException("Invalid GradientStyle value");
			style = (byte) BrushStyle.Gradient;
			styleInfo = (byte) gradientStyle;
			return this;
		}

		private Color GetForeColor()
		{
			return this.gradientColors[this.gradientColors.Count - 1];
		}
		private BrushInfo SetForeColor(Color color)
		{
			this.gradientColors[this.gradientColors.Count - 1] = color;
			return this;
		}

		/// <summary>
		/// Returns the backcolor.
		/// </summary>
		[Description("Specifies the backcolor")]
		public Color BackColor
		{
			get
			{
				return GetBackColor();
			}
		}

		/// <summary>
		/// Returns the forecolor.
		/// </summary>
		[Description("Specifies the forecolor")]
		public Color ForeColor
		{
			get
			{
				return GetForeColor();
			}
		}

		/// <summary>
		/// Returns the gradient colors.
		/// </summary>
		/// <value>A reference to the <see cref="BrushInfoColorArrayList"/> instance.</value>
		/// <remarks><p>This color list will be used to specify the <see cref="System.Drawing.Drawing2D.LinearGradientBrush.InterpolationColors"/>
		/// or the <see cref="System.Drawing.Drawing2D.PathGradientBrush.SurroundColors"/> depending on the
		/// <see cref="GradientStyle"/> selected.</p>
		/// <p>The first entry in this list will be the same as the <see cref="BackColor"/> property and
		/// the last entry (not the 2nd) will be the same as the <see cref="ForeColor"/> property.</p>
		/// <p>
		/// Note that this list is Read-only.
		/// </p>
		/// </remarks>
		[Description("Specifies the gradient colors.The first entry in this list will be the same as the backcolor property,the lastentry will be same as the forecolor property.")]
		public BrushInfoColorArrayList GradientColors
		{
			get{return this.gradientColors;}
		}

		/// <summary>
		/// Returns the pattern style.
		/// </summary>
	    [Description("Specifies the pattern style.")]
		public PatternStyle PatternStyle
		{
			get
			{
				return GetPatternStyle();
			}
		}

		/// <summary>
		/// Returns the gradient style.
		/// </summary>
		[Description( "Returns the gradient style." )]
		public GradientStyle GradientStyle
		{
			get
			{
				return GetGradientStyle();
			}
		}

		/// <summary>
		/// Returns the brush style (solid, gradient or pattern).
		/// </summary>
		[Description("Specifies the brush style solid,gradient or pattern")]
		public BrushStyle Style
		{
			get
			{
				return GetStyle();
			}
		}

    #region IXmlSerializable Members

		/// <summary>
		/// Serializes the contents of this object into an XML stream.
		/// </summary>
		/// <param name="writer">Represents the XML stream.</param>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteString(this.GetDescription());
		}

		/// <summary>
		/// Not implemented and returns NULL.
		/// </summary>
		/// <returns></returns>
		XmlSchema IXmlSerializable.GetSchema()
		{
			// TODO:  Add BrushInfo.GetSchema implementation
			return null;
		}

		/// <summary>
		/// Deserializes the contents of this object from an XML stream.
		/// </summary>
		/// <param name="reader">Represents the XML stream.</param>
		public void ReadXml(XmlReader reader)
		{
			string s = reader.ReadString();
			this.gradientColors.Freeze = false;
			this.SetDescription(s);
			this.gradientColors.Freeze = true;
		}

    #endregion

        
        public void ClearColorInfo()
        {
            if (this.gradientColors != null)
            {
                this.gradientColors.Freeze = false;
                this.gradientColors.Clear();
            }
        }
    }

	/// <summary>
	/// A list of colors returned by the <see cref="Syncfusion.Drawing.BrushInfo.GradientColors"/> property
	/// in the <see cref="Syncfusion.Drawing.BrushInfo"/> type.
	/// </summary>
	/// <remarks>
	/// When returned by the <see cref="Syncfusion.Drawing.BrushInfo.GradientColors"/> property, this list will
	/// be made Read-only.
	/// </remarks>
	[TypeConverter(typeof(ColorListConverter)),
	Editor(typeof(BrushInfoColorsCollectionEditor), typeof(UITypeEditor)),
	Serializable()]
	public class BrushInfoColorArrayList : ArrayListExt, ISerializable
	{
		/// <summary>
		/// Overloaded. Creates a new instance of this class.
		/// </summary>
		public BrushInfoColorArrayList(){}
		/// <summary>
		/// Creates a new instance of this class with some colors.
		/// </summary>
		/// <param name="colors">An array of colors.</param>
		public BrushInfoColorArrayList(Color[] colors)
		{
			this.AddRange(colors);
		}
		/// <summary>
		/// Initializes a new <see cref="BrushInfoColorArrayList"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
		protected BrushInfoColorArrayList(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator sie = info.GetEnumerator();
			while (sie.MoveNext())
			{
				this.Add((Color)sie.Value);
			}
		}
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			for (int index = 0; index < Count; index++)
				info.AddValue(index.ToString(), this[index]);
		}
		/// <summary>
		/// Returns the color at the specified index.
		/// </summary>
		public new Color this[int index]
		{
			get{return (Color)base[index];}
			set{base[index] = value;}
		}
		internal void Add(Color color)
		{
			base.Add(color);
		}
		internal void AddRange(Color[] colors)
		{
			base.AddRange(colors);
		}
		internal bool Freeze
		{
			get{return this.ForceFixedSize = true;}
			set{this.ForceFixedSize = value;this.ForceReadOnly = value;}
		}

		//		/// </override>
		//		public override /*IList*/ void RemoveAt(int index)
		//		{
		//			// Need atleast 2 items in this list.
		//			if(this.Count <= 1)
		//				return;
		//
		//			base.RemoveAt(index);
		//		}
		//		/// </override>
		//		public override void RemoveRange(int index, int count)
		//		{
		//			if(this.Count - count <= 1)
		//				return;
		//
		//			base.RemoveRange(index, count);
		//		}
	}

	internal class BrushInfoColorsCollectionEditor : CollectionEditor
	{
		BrushInfoColorArrayList coll;
		private CollectionForm collectionForm;

		public BrushInfoColorsCollectionEditor(Type type)
			:base(type)
		{
		}
		public override object EditValue(
			ITypeDescriptorContext context,
			IServiceProvider provider,
			object value
			)
		{
			this.coll = value as BrushInfoColorArrayList;
			bool oldValue = this.coll.Freeze;
			this.coll.Freeze = false;
			value = base.EditValue(context, provider, value);
			this.coll.Freeze = oldValue;
			return value;
		}

		protected override object SetItems(object editValue, object[] value)
		{
			// base class will just update the existing collection, we will instead create a new collection
			// since BrushInfo is immutable
			BrushInfoColorArrayList list = new BrushInfoColorArrayList();
			foreach(object o in value)
			{
				list.Add((Color)o);
			}
			return list;
		}
		// Gets a reference to the collection form.
		protected override CollectionForm CreateCollectionForm()
		{
			this.collectionForm = base.CreateCollectionForm();
			return this.collectionForm;
		}
		// Prevent deleting items when there are only 2 items (or less) in the collection.
		protected override bool CanRemoveInstance(object value)
		{
			int listCount = this.GetItemsInCollectionEditorFormListBox();
			if(listCount > -1 && listCount <= 2)
				return false;

			return base.CanRemoveInstance(value);
		}
		private int GetItemsInCollectionEditorFormListBox()
		{
			foreach(Control c in this.collectionForm.Controls)
			{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				foreach (Control innerControl in c.Controls)
				{
					if (innerControl is ListBox)
					{
						ListBox list = innerControl as ListBox;
						return list.Items.Count;
					}
				}
#else
				if(c is ListBox)
				{
                    ListBox list = c as ListBox;
					return list.Items.Count;
				}
#endif
			}
			return -1;
		}
	}
	[DocumentationExclude()]
	internal class ColorListConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)
				&& value is BrushInfoColorArrayList)
			{
				BrushInfoColorArrayList colorList = (BrushInfoColorArrayList)value;
				Type[] args;
				args = new Type[1];

				args[0] = typeof(Color[]);

					ConstructorInfo constructorInfo;
				constructorInfo = typeof(BrushInfoColorArrayList).GetConstructor(args);
				if (constructorInfo != null)
				{
					object[] argValues;
					argValues = new Object[1];
					Color[] colorArray = new Color[colorList.Count];
					for(int i = 0; i < colorList.Count; i++)
					{
						colorArray[i] = colorList[i];
					}
					argValues[0] = colorArray;

					return new InstanceDescriptor(constructorInfo,argValues);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if(destinationType == typeof(InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destinationType);
		}
	}
}