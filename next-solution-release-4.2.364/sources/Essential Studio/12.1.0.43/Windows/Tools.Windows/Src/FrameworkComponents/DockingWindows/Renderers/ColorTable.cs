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
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// Base class to others that keep colors for different part of docking windows.
	/// </summary>
	public class ColorTableBase
	{
		#region Constructors

		public ColorTableBase()
		{
			InitColorTable();
		}


		#endregion

		#region Public methods

		public virtual bool RefreshColors()
		{
			Theme theme = GetCurrentTheme();
			if( theme != m_theme )
			{
				m_theme = theme;
				InitColorTable( theme );
				return true;
			}
			return false;
		}
		public virtual void InitColorTable( Theme theme )
		{
			switch( theme )
			{
				case Theme.WindowsClassic:
				{
					InitWindowsClassicColors();
					break;
				}
				case Theme.XPBlue:
				{
					InitXPBlueColors();
					break;
				}
				case Theme.XPSilver:
				{
					InitXPSilverColors();
					break;
				}
				case Theme.XPOlive:
				{
					InitXPOliveColors();
					break;
				}
                case Theme.XPZune:
                {
                    InitXPZuneColors();
                    break;
                }
			}
		}
		public virtual void InitColorTable()
		{
			m_theme = GetCurrentTheme();
			InitColorTable( m_theme );
		}


		#endregion

		#region Protected virtual methods

		protected virtual Theme GetCurrentTheme()
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(
				@"Software\Microsoft\Windows\CurrentVersion\ThemeManager");
			if (key != null) 
			{
				if ("1" == (string) key.GetValue("ThemeActive"))
				{
					string colorName = (string) key.GetValue("ColorName");
                    string dllName = (string)key.GetValue("DllName");

					if (colorName != null)
					{	
                        if (String.Compare(colorName, "NormalColor", true) == 0 && dllName.ToLower().Contains("zune"))
                            return Theme.XPZune;
						if (String.Compare(colorName, "NormalColor", true) == 0)
							return Theme.XPBlue;
						if (String.Compare(colorName, "HomeStead", true) == 0)
							return Theme.XPOlive;
						if (String.Compare(colorName, "Metallic", true) == 0)
							return Theme.XPSilver;                        
					}
				} 
			}
			return Theme.WindowsClassic;
		}

		protected virtual void InitWindowsClassicColors() {}
		protected virtual void InitXPBlueColors() {}
		protected virtual void InitXPSilverColors() {}
		protected virtual void InitXPOliveColors() {}
        protected virtual void InitXPZuneColors() { }


		#endregion

		#region Properties

		public Theme Theme
		{
			get
			{
				return m_theme;
			}
		}


		#endregion

		#region Protected members

		protected Theme m_theme;

		#endregion
	}

    /// <summary>
    /// Base class to other Office2010-like color tables.
    /// </summary>
    public class ColorTableOffice2010Base : ColorTableBase
    {
        #region Constructors

        public ColorTableOffice2010Base()
            : base()
        {
        }

        #endregion

        #region Public methods

        public override void InitColorTable()
        {
            switch (m_Office2010Theme)
            {
                case Office2010Theme.Blue:
                    InitOffice2010BlueColors();
                    break;

                case Office2010Theme.Black:
                    InitOffice2010BlackColors();
                    break;

                case Office2010Theme.Silver:
                    InitOffice2010SilverColors();
                    break;

                case Office2010Theme.Managed:
                    InitOffice2010ManagedColors();
                    break;

            }
        }

        public void InitColorTable(Office2010Theme theme)
        {
            switch (theme)
            {
                case Office2010Theme.Blue:
                    InitOffice2010BlueColors();
                    break;
                case Office2010Theme.Silver:
                    InitOffice2010SilverColors();
                    break;
                case Office2010Theme.Black:
                    InitOffice2010BlackColors();
                    break;
                case Office2010Theme.Managed:
                    InitOffice2010ManagedColors();
                    break;
            }
        }

        public override bool RefreshColors()
        {
            InitColorTable();
            return true;
        }

        #endregion

        #region Protected virtual methods

        protected virtual void InitOffice2010BlueColors()
        {
        }

        protected virtual void InitOffice2010SilverColors()
        {
        }

        protected virtual void InitOffice2010BlackColors()
        {
        }

        protected virtual void InitOffice2010ManagedColors()
        {
        }

        #endregion

        #region Properties

        public Office2010Theme Office2010Theme
        {
            get
            {
                return m_Office2010Theme;
            }
            set
            {
                if (m_Office2010Theme != value || value == Office2010Theme.Managed)
                {
                    m_Office2010Theme = value;
                    InitColorTable();
                }
            }
        }

        /// <summary>
        /// Contains the TextColor of appropriate themes.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return m_TextColor;
            }
        }
        /// <summary>
        /// Contains the BorderColor of appropriate themes.
        /// </summary>
        public Color OuterBorderColor
        {
            get
            {
                return m_BorderColor;
            }
        }
        /// <summary>
        /// Contains the InnerBorderColor of appropriate themes.
        /// </summary>
        public Color InnerBorderColor
        {
            get
            {
                return m_InnerBorderColor;
            }
        }
        /// <summary>
        /// Contains the CaptionUpperTopColor of appropriate themes.
        /// </summary>
        public Color CaptionUpperTopColor
        {
            get
            {
                return m_CaptionUpperTopColor;
            }
        }
        /// <summary>
        /// Contains the CaptionUpperBottomColor of appropriate themes.
        /// </summary>
        public Color CaptionUpperBottomColor
        {
            get
            {
                return m_CaptionUpperBottomColor;
            }
        }
        /// <summary>
        /// Contains the CaptionLowerTopColor of appropriate themes.
        /// </summary>
        public Color CaptionLowerTopColor
        {
            get
            {
                return m_CaptionLowerTopColor;
            }
        }
        /// <summary>
        /// Contains the CaptionLowerBottomColor of appropriate themes.
        /// </summary>
        public Color CaptionLowerBottomColor
        {
            get
            {
                return m_CaptionLowerBottomColor;
            }
        }
        /// <summary>
        /// Contains the ActiveCaptionUpperTopColor of appropriate themes.
        /// </summary>
        public Color ActiveCaptionUpperTopColor
        {
            get
            {
                return m_ActiveCaptionUpperTopColor;
            }
        }
        /// <summary>
        /// Contains the ActiveCaptionUpperBottomColor of appropriate themes.
        /// </summary>
        public Color ActiveCaptionUpperBottomColor
        {
            get
            {
                return m_ActiveCaptionUpperBottomColor;
            }
        }
        /// <summary>
        /// Contains the ActiveCaptionLowerTopColor of appropriate themes.
        /// </summary>
        public Color ActiveCaptionLowerTopColor
        {
            get
            {
                return m_ActiveCaptionLowerTopColor;
            }
        }
        /// <summary>
        /// Contains the ActiveCaptionLowerBottomColor of appropriate themes.
        /// </summary>
        public Color ActiveCaptionLowerBottomColor
        {
            get
            {
                return m_ActiveCaptionLowerBottomColor;
            }
        }
        /// <summary>
        /// Contains the ActiveButtonColor of appropriate themes.
        /// </summary>
        public Color ActiveButtonColor
        {
            get
            {
                return m_ActiveButtonColor;
            }
        }
        /// <summary>
        /// Contains the PushedButtonColor of appropriate themes.
        /// </summary>
        public Color PushedButtonColor
        {
            get
            {
                return m_PushedButtonColor;
            }
        }
        /// <summary>
        /// Contains the ButtonColor of appropriate themes.
        /// </summary>
        public Color ButtonColor
        {
            get
            {
                return m_ButtonColor;
            }
        }
        /// <summary>
        /// Contains the SplitterColor of appropriate themes.
        /// </summary>
        public Color SplitterColor
        {
            get
            {
                return m_SplitterColor;
            }
        }
        /// <summary>
        /// Contains the ButtonImageColor of appropriate themes.
        /// </summary>
        public Color ButtonImageColor
        {
            get
            {
                return m_ButtonImageColor;
            }
        }
        /// <summary>
        /// Contains the ActiveButtonImageColor of appropriate themes.
        /// </summary>
        public Color ActiveButtonImageColor
        {
            get
            {
                return m_ActiveButtonImageColor;
            }
        }

        #endregion

        #region Protected members

        protected Office2010Theme m_Office2010Theme = Office2010Theme.Blue;

        protected Color m_TextColor;
        protected Color m_ActiveTextColor;
        protected Color m_BorderColor;
        protected Color m_InnerBorderColor;
        protected Color m_CaptionUpperTopColor;
        protected Color m_CaptionUpperBottomColor;
        protected Color m_CaptionLowerTopColor;
        protected Color m_CaptionLowerBottomColor;
        protected Color m_ActiveCaptionUpperTopColor;
        protected Color m_ActiveCaptionUpperBottomColor;
        protected Color m_ActiveCaptionLowerTopColor;
        protected Color m_ActiveCaptionLowerBottomColor;
        protected Color m_ActiveButtonColor;
        protected Color m_PushedButtonColor;
        protected Color m_ButtonColor;
        protected Color m_SplitterColor;
        protected Color m_ButtonImageColor;
        protected Color m_ActiveButtonImageColor;

        #endregion
    }

    /// <summary>
    /// Keeps the different colors for different part of docking windows (Office 2010-like appearance).
    /// </summary>
    public class ColorTableOffice2010 : ColorTableOffice2010Base
    {
        #region Constructors

        public ColorTableOffice2010()
            : base()
        {
        }

        #endregion

        #region Protected overrided methods

        protected override void InitOffice2010BlueColors()
        {
            m_TextColor = Color.FromArgb(16, 65, 140);
            m_BorderColor = Color.FromArgb(181, 207, 247);
            m_InnerBorderColor = Color.FromArgb(222, 235, 255);
            m_CaptionUpperTopColor = Color.FromArgb(231, 235, 255);
            m_CaptionUpperBottomColor = Color.FromArgb(222, 231, 255);
            m_CaptionLowerTopColor = Color.FromArgb(206, 223, 247);
            m_CaptionLowerBottomColor = Color.FromArgb(222, 247, 255);
            m_ActiveCaptionUpperTopColor = Color.FromArgb(198, 223, 255);
            m_ActiveCaptionUpperBottomColor = Color.FromArgb(231, 231, 231);
            m_ActiveCaptionLowerTopColor = Color.FromArgb(214, 227, 227);
            m_ActiveCaptionLowerBottomColor = Color.FromArgb(231, 211, 165);
            m_ButtonBorderColor = Color.FromArgb(101, 147, 207);
            m_ActiveButtonBorderColor = Color.FromArgb(255, 189, 105);
            m_PushedButtonBorderColor = Color.FromArgb(251, 140, 60);
            m_ActiveButtonColor = Color.FromArgb(255, 231, 162);
            m_PushedButtonColor = Color.FromArgb(251, 140, 60);
            m_ButtonColor = Color.FromArgb(214, 232, 255);
            m_SplitterColor = Color.FromArgb(219, 230, 244);
            m_ButtonImageColor = Color.FromArgb(140, 166, 206);
            m_ActiveButtonImageColor = Color.FromArgb(99, 101, 99);
        }

        protected override void InitOffice2010BlackColors()
        {
            m_TextColor = Color.FromArgb(255, 255, 255);
            m_BorderColor = Color.FromArgb(41, 44, 41);
            m_InnerBorderColor = Color.FromArgb(148, 146, 148);
            m_CaptionUpperTopColor = Color.FromArgb(66, 69, 82);
            m_CaptionUpperBottomColor = Color.FromArgb(57, 60, 66);
            m_CaptionLowerTopColor = Color.FromArgb(41, 48, 49);
            m_CaptionLowerBottomColor = Color.FromArgb(57, 60, 57);
            m_ActiveCaptionUpperTopColor = Color.FromArgb(148, 150, 148);
            m_ActiveCaptionUpperBottomColor = Color.FromArgb(115, 109, 90);
            m_ActiveCaptionLowerTopColor = Color.FromArgb(107, 105, 90);
            m_ActiveCaptionLowerBottomColor = Color.FromArgb(231, 190, 41);
            m_ButtonBorderColor = Color.FromArgb(82, 93, 99);
            m_ActiveButtonBorderColor = Color.FromArgb(239, 186, 107);
            m_PushedButtonBorderColor = Color.FromArgb(255, 162, 49);
            m_ActiveButtonColor = Color.FromArgb(255, 207, 74);
            m_PushedButtonColor = Color.FromArgb(255, 162, 49);
            m_ButtonColor = Color.FromArgb(115, 125, 140);
            m_SplitterColor = Color.FromArgb(82, 81, 82);
            m_ButtonImageColor = Color.FromArgb(206, 211, 214);
            m_ActiveButtonImageColor = Color.FromArgb(99, 101, 99);
        }

        protected override void InitOffice2010SilverColors()
        {
            m_TextColor = Color.FromArgb(90, 97, 107);
            m_BorderColor = Color.FromArgb(198, 190, 198);
            m_InnerBorderColor = Color.FromArgb(247, 247, 255);
            m_CaptionUpperTopColor = Color.FromArgb(231, 227, 231);
            m_CaptionUpperBottomColor = Color.FromArgb(206, 208, 214);
            m_CaptionLowerTopColor = Color.FromArgb(189, 195, 206);
            m_CaptionLowerBottomColor = Color.FromArgb(239, 239, 247);
            m_ActiveCaptionUpperTopColor = Color.FromArgb(222, 219, 231);
            m_ActiveCaptionUpperBottomColor = Color.FromArgb(239, 231, 222);
            m_ActiveCaptionLowerTopColor = Color.FromArgb(231, 223, 214);
            m_ActiveCaptionLowerBottomColor = Color.FromArgb(239, 211, 148);
            m_ButtonBorderColor = Color.FromArgb(206, 207, 206);
            m_ActiveButtonBorderColor = Color.FromArgb(206, 182, 157);
            m_PushedButtonBorderColor = Color.FromArgb(255, 162, 49);
            m_ActiveButtonColor = Color.FromArgb(255, 211, 74);
            m_PushedButtonColor = Color.FromArgb(255, 162, 49);
            m_ButtonColor = Color.FromArgb(247, 251, 255);
            m_SplitterColor = Color.FromArgb(239, 243, 239);
            m_ButtonImageColor = Color.FromArgb(140, 154, 181);
            m_ActiveButtonImageColor = Color.FromArgb(66, 69, 66);
        }

        protected override void InitOffice2010ManagedColors()
        {
            Office2007Colors oc = Office2007Colors.GetColorTable(Office2007Theme.Managed);

            m_TextColor = oc.FormTextColor; ;
            m_BorderColor = oc.ActiveFormBorderColor;
            m_InnerBorderColor = oc.ActiveFormBorderColor;
            m_CaptionUpperTopColor = oc.InactiveTitleGradientBegin;
            m_CaptionUpperBottomColor = oc.InactiveTitleGradientBegin;
            m_CaptionLowerTopColor = oc.InactiveTitleGradientBegin;
            m_CaptionLowerBottomColor = oc.InactiveTitleGradientEnd;
            m_ActiveCaptionUpperTopColor = oc.ActiveTitleGradientBegin;
            m_ActiveCaptionUpperBottomColor = oc.ActiveTitleGradientBegin;//Color.FromArgb( 231, 231, 231 );
            m_ActiveCaptionLowerTopColor = oc.ActiveTitleGradientBegin;
            m_ActiveCaptionLowerBottomColor = oc.ActiveTitleGradientEnd;//Color.FromArgb( 231, 211, 165 );
            m_ButtonBorderColor = oc.ButtonDefaultBorderColor;
            m_ActiveButtonBorderColor = oc.ButtonSelectedBorderColor;
            m_PushedButtonBorderColor = oc.ButtonPressedBorderColor;
            m_ActiveButtonColor = oc.SystemButtonSelectedGradientBegin;
            m_PushedButtonColor = oc.SystemButtonPressedGradientBegin;
            m_ButtonColor = Color.FromArgb(247, 251, 255);
            m_SplitterColor = oc.TabBarSplitterBackColor;
            m_ButtonImageColor = Color.FromArgb(140, 154, 181);
            m_ActiveButtonImageColor = Color.FromArgb(66, 69, 66);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Contains the ButtonBorderColor of appropriate themes.
        /// </summary>
        public Color ButtonBorderColor
        {
            get
            {
                return m_ButtonBorderColor;
            }
        }
        /// <summary>
        /// Contains the ActiveButtonBorderColor of appropriate themes.
        /// </summary>
        public Color ActiveButtonBorderColor
        {
            get
            {
                return m_ActiveButtonBorderColor;
            }
        }
        /// <summary>
        /// Contains the PushedButtonBorderColor of appropriate themes.
        /// </summary>
        public Color PushedButtonBorderColor
        {
            get
            {
                return m_PushedButtonBorderColor;
            }
        }

        #endregion

        #region Protected members

        protected Color m_ButtonBorderColor;
        protected Color m_ActiveButtonBorderColor;
        protected Color m_PushedButtonBorderColor;

        #endregion
    }

	/// <summary>
	/// Base class to other Office2007-like color tables.
	/// </summary>
	public class ColorTableOffice2007Base : ColorTableBase
	{
		#region Constructors

		public ColorTableOffice2007Base()
			: base()
		{
		}

		#endregion

		#region Public methods

		public override void InitColorTable() 
		{
			switch (m_Office2007Theme)
			{
				case Office2007Theme.Blue:
					InitOffice2007BlueColors();
					break;

				case Office2007Theme.Black:
					InitOffice2007BlackColors();
					break;

				case Office2007Theme.Silver:
					InitOffice2007SilverColors();
					break;

                case Office2007Theme.Managed:
                    InitOffice2007ManagedColors();
                    break;

			}
		}

		public void InitColorTable(Office2007Theme theme)
		{
			switch (theme)
			{
				case Office2007Theme.Blue:
					InitOffice2007BlueColors();
					break;
				case Office2007Theme.Silver:
					InitOffice2007SilverColors();
					break;
				case Office2007Theme.Black:
					InitOffice2007BlackColors();
					break;
                case Office2007Theme.Managed:
                    InitOffice2007ManagedColors();
                    break;
			}
		}

		public override bool RefreshColors()
		{
			InitColorTable();
			return true;
		}

		#endregion

		#region Protected virtual methods

		protected virtual void InitOffice2007BlueColors()
		{
		}

		protected virtual void InitOffice2007SilverColors()
		{ 
		}

		protected virtual void InitOffice2007BlackColors()
		{ 
		}

        protected virtual void InitOffice2007ManagedColors()
        {
        }

		#endregion

		#region Properties

		public Office2007Theme Office2007Theme
		{
			get
			{
				return m_Office2007Theme;
			}
			set
			{
                if( m_Office2007Theme != value || value == Office2007Theme.Managed )
                {
                    m_Office2007Theme = value;
                    InitColorTable();
                }
			}
		}

		/// <summary>
		/// Contains the TextColor of appropriate themes.
		/// </summary>
		public Color TextColor
		{
			get
			{
				return m_TextColor;
			}
		}
		/// <summary>
		/// Contains the BorderColor of appropriate themes.
		/// </summary>
		public Color OuterBorderColor
		{
			get
			{
				return m_BorderColor;
			}
		}
		/// <summary>
		/// Contains the InnerBorderColor of appropriate themes.
		/// </summary>
		public Color InnerBorderColor
		{
			get
			{
				return m_InnerBorderColor;
			}
		}
		/// <summary>
		/// Contains the CaptionUpperTopColor of appropriate themes.
		/// </summary>
		public Color CaptionUpperTopColor
		{
			get
			{
				return m_CaptionUpperTopColor;
			}
		}
		/// <summary>
		/// Contains the CaptionUpperBottomColor of appropriate themes.
		/// </summary>
		public Color CaptionUpperBottomColor
		{
			get
			{
				return m_CaptionUpperBottomColor;
			}
		}
		/// <summary>
		/// Contains the CaptionLowerTopColor of appropriate themes.
		/// </summary>
		public Color CaptionLowerTopColor
		{
			get
			{
				return m_CaptionLowerTopColor;
			}
		}
		/// <summary>
		/// Contains the CaptionLowerBottomColor of appropriate themes.
		/// </summary>
		public Color CaptionLowerBottomColor
		{
			get
			{
				return m_CaptionLowerBottomColor;
			}
		}
		/// <summary>
		/// Contains the ActiveCaptionUpperTopColor of appropriate themes.
		/// </summary>
		public Color ActiveCaptionUpperTopColor
		{
			get
			{
				return m_ActiveCaptionUpperTopColor;
			}
		}
		/// <summary>
		/// Contains the ActiveCaptionUpperBottomColor of appropriate themes.
		/// </summary>
		public Color ActiveCaptionUpperBottomColor
		{
			get
			{
				return m_ActiveCaptionUpperBottomColor;
			}
		}
		/// <summary>
		/// Contains the ActiveCaptionLowerTopColor of appropriate themes.
		/// </summary>
		public Color ActiveCaptionLowerTopColor
		{
			get
			{
				return m_ActiveCaptionLowerTopColor;
			}
		}
		/// <summary>
		/// Contains the ActiveCaptionLowerBottomColor of appropriate themes.
		/// </summary>
		public Color ActiveCaptionLowerBottomColor
		{
			get
			{
				return m_ActiveCaptionLowerBottomColor;
			}
		}
		/// <summary>
		/// Contains the ActiveButtonColor of appropriate themes.
		/// </summary>
		public Color ActiveButtonColor
		{
			get
			{
				return m_ActiveButtonColor;
			}
		}
		/// <summary>
		/// Contains the PushedButtonColor of appropriate themes.
		/// </summary>
		public Color PushedButtonColor
		{
			get
			{
				return m_PushedButtonColor;
			}
		}
		/// <summary>
		/// Contains the ButtonColor of appropriate themes.
		/// </summary>
		public Color ButtonColor
		{
			get
			{
				return m_ButtonColor;
			}
		}
		/// <summary>
		/// Contains the SplitterColor of appropriate themes.
		/// </summary>
		public Color SplitterColor
		{
			get
			{
				return m_SplitterColor;
			}
		}
		/// <summary>
		/// Contains the ButtonImageColor of appropriate themes.
		/// </summary>
		public Color ButtonImageColor
		{
			get
			{
				return m_ButtonImageColor;
			}
		}
		/// <summary>
		/// Contains the ActiveButtonImageColor of appropriate themes.
		/// </summary>
		public Color ActiveButtonImageColor
		{
			get
			{
				return m_ActiveButtonImageColor;
			}
		}

		#endregion

		#region Protected members

		protected Office2007Theme m_Office2007Theme = Office2007Theme.Blue;

		protected Color m_TextColor;
		protected Color m_ActiveTextColor;
		protected Color m_BorderColor;
		protected Color m_InnerBorderColor;
		protected Color m_CaptionUpperTopColor;
		protected Color m_CaptionUpperBottomColor;
		protected Color m_CaptionLowerTopColor;
		protected Color m_CaptionLowerBottomColor;
		protected Color m_ActiveCaptionUpperTopColor;
		protected Color m_ActiveCaptionUpperBottomColor;
		protected Color m_ActiveCaptionLowerTopColor;
		protected Color m_ActiveCaptionLowerBottomColor;
		protected Color m_ActiveButtonColor;
		protected Color m_PushedButtonColor;
		protected Color m_ButtonColor;
		protected Color m_SplitterColor;
		protected Color m_ButtonImageColor;
		protected Color m_ActiveButtonImageColor;

		#endregion
	}

	/// <summary>
	/// Keeps the different colors for different part of docking windows (Office 2003-like appearance).
	/// </summary>
	public class ColorTableOffice2003 : ColorTableBase
	{
		#region Constructors

		public ColorTableOffice2003() : base()
		{
		}


		#endregion

		#region Protected overrided methods

		protected override void InitWindowsClassicColors()
		{
			m_TextColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveTextColor = Color.FromArgb( 0, 0, 0 );
			m_OuterBorderColor = Color.FromArgb( 128, 128, 128 );
			m_InnerBorderColor = Color.FromArgb( 234, 232, 228 );
			m_CaptionTopColor = Color.FromArgb( 243, 242, 240 );
			m_CaptionBottomColor = Color.FromArgb( 212, 208, 200 );
			m_ActiveCaptionTopColor = Color.FromArgb( 212, 213, 216 );
			m_ActiveCaptionBottomColor = Color.FromArgb( 212, 213, 216 );
			m_GripperForegroundColor = Color.FromArgb( 160, 160, 160 );
			m_GripperBackgroundColor = Color.FromArgb( 255, 255, 255 );
			m_ButtonBorderColor = Color.FromArgb( 10, 36, 106 );
			m_ActiveButtonTopColor = Color.FromArgb( 182, 189, 210 );
			m_ActiveButtonBottomColor = Color.FromArgb( 182, 189, 210 );
			m_PushedButtonTopColor = Color.FromArgb( 133, 146, 181 );
			m_PushedButtonBottomColor = Color.FromArgb( 133, 146, 181 );
			m_ButtonImageColor = Color.FromArgb( 0, 0, 0 );
			m_PushedButtonImageColor = Color.FromArgb( 255, 255, 255 );
		}

		protected override void InitXPBlueColors()
		{
			m_TextColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveTextColor = Color.FromArgb( 0, 0, 0 );
			m_OuterBorderColor = Color.FromArgb( 42, 102, 201 );
			m_InnerBorderColor = Color.FromArgb( 185, 212, 249 );
			m_CaptionTopColor = Color.FromArgb( 218, 234, 253 );
			m_CaptionBottomColor = Color.FromArgb( 123, 164, 224 );
			m_ActiveCaptionTopColor = Color.FromArgb( 255, 213, 140 );
			m_ActiveCaptionBottomColor = Color.FromArgb( 255, 166, 76 );
			m_GripperForegroundColor = Color.FromArgb( 39, 65, 118 );
			m_GripperBackgroundColor = Color.FromArgb( 255, 255, 255 );
			m_ButtonBorderColor = Color.FromArgb( 0, 0, 128 );
			m_ActiveButtonTopColor = Color.FromArgb( 255, 233, 186 );
			m_ActiveButtonBottomColor = Color.FromArgb( 255, 208, 145 );
			m_PushedButtonTopColor = Color.FromArgb( 254, 163, 95 );
			m_PushedButtonBottomColor = Color.FromArgb( 255, 211, 142 );
			m_ButtonImageColor = Color.FromArgb( 0, 0, 0 );
			m_PushedButtonImageColor = Color.FromArgb( 255, 255, 255 );
		}

		protected override void InitXPSilverColors()
		{
			m_TextColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveTextColor = Color.FromArgb( 0, 0, 0 );
			m_OuterBorderColor = Color.FromArgb( 122, 121, 153 );
			m_InnerBorderColor = Color.FromArgb( 255, 255, 255 );
			m_CaptionTopColor = Color.FromArgb( 240, 240, 248 );
			m_CaptionBottomColor = Color.FromArgb( 147, 145, 176 );
			m_ActiveCaptionTopColor = Color.FromArgb( 255, 213, 140 );
			m_ActiveCaptionBottomColor = Color.FromArgb( 255, 166, 76 );
			m_GripperForegroundColor = Color.FromArgb( 84, 84, 117 );
			m_GripperBackgroundColor = Color.FromArgb( 255, 255, 255 );
			m_ButtonBorderColor = Color.FromArgb( 75, 75, 111 );
			m_ActiveButtonTopColor = Color.FromArgb( 255, 233, 186 );
			m_ActiveButtonBottomColor = Color.FromArgb( 255, 208, 145 );
			m_PushedButtonTopColor = Color.FromArgb( 254, 163, 95 );
			m_PushedButtonBottomColor = Color.FromArgb( 255, 211, 142 );
			m_ButtonImageColor = Color.FromArgb( 0, 0, 0 );
			m_PushedButtonImageColor = Color.FromArgb( 255, 255, 255 );
		}

		protected override void InitXPOliveColors()
		{
			m_TextColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveTextColor = Color.FromArgb( 0, 0, 0 );
			m_OuterBorderColor = Color.FromArgb( 116, 134, 94 );
			m_InnerBorderColor = Color.FromArgb( 255, 255, 255 );
			m_CaptionTopColor = Color.FromArgb( 237, 242, 212 );
			m_CaptionBottomColor = Color.FromArgb( 181, 196, 143 );
			m_ActiveCaptionTopColor = Color.FromArgb( 255, 213, 140 );
			m_ActiveCaptionBottomColor = Color.FromArgb( 255, 166, 76 );
			m_GripperForegroundColor = Color.FromArgb( 81, 94, 51 );
			m_GripperBackgroundColor = Color.FromArgb( 255, 255, 255 );
			m_ButtonBorderColor = Color.FromArgb( 63, 93,56 );
			m_ActiveButtonTopColor = Color.FromArgb( 255, 233, 186 );
			m_ActiveButtonBottomColor = Color.FromArgb( 255, 208, 145 );
			m_PushedButtonTopColor = Color.FromArgb( 254, 163, 95 );
			m_PushedButtonBottomColor = Color.FromArgb( 255, 211, 142 );
			m_ButtonImageColor = Color.FromArgb( 0, 0, 0 );
			m_PushedButtonImageColor = Color.FromArgb( 255, 255, 255 );
		}


		#endregion

		#region Properties

		/// <summary>
		/// Contains the TextColor of appropriate themes.
		/// </summary>
		public Color TextColor
		{
			get
			{
				return m_TextColor;
			}
		}
		/// <summary>
		/// Contains the ActiveTextColor of appropriate themes.
		/// </summary>
		public Color ActiveTextColor
		{
			get
			{
				return m_ActiveTextColor;
			}
		}
		/// <summary>
		/// Contains the OuterBorderColor of appropriate themes.
		/// </summary>
		public Color OuterBorderColor
		{
			get
			{
				return m_OuterBorderColor;
			}
		}
		/// <summary>
		/// Contains the InnerBorderColor of appropriate themes.
		/// </summary>
		public Color InnerBorderColor
		{
			get
			{
				return m_InnerBorderColor;
			}
		}
		/// <summary>
		/// Contains the CaptionTopColor of appropriate themes.
		/// </summary>
		public Color CaptionTopColor
		{
			get
			{
				return m_CaptionTopColor;
			}
		}
		/// <summary>
		/// Contains the CaptionBottomColor of appropriate themes.
		/// </summary>
		public Color CaptionBottomColor
		{
			get
			{
				return m_CaptionBottomColor;
			}
		}
		/// <summary>
		/// Contains the ActiveCaptionTopColor of appropriate themes.
		/// </summary>
		public Color ActiveCaptionTopColor
		{
			get
			{
				return m_ActiveCaptionTopColor;
			}
		}
		/// <summary>
		/// Contains the ActiveCaptionBottomColor of appropriate themes.
		/// </summary>
		public Color ActiveCaptionBottomColor
		{
			get
			{
				return m_ActiveCaptionBottomColor;
			}
		}
		/// <summary>
		/// Contains the GripperForegroundColor of appropriate themes.
		/// </summary>
		public Color GripperForegroundColor
		{
			get
			{
				return m_GripperForegroundColor;
			}
		}
		/// <summary>
		/// Contains the GripperBackgroundColor of appropriate themes.
		/// </summary>
		public Color GripperBackgroundColor
		{
			get
			{
				return m_GripperBackgroundColor;
			}
		}
		/// <summary>
		/// Contains the ButtonBorderColor of appropriate themes.
		/// </summary>
		public Color ButtonBorderColor
		{
			get
			{
				return m_ButtonBorderColor;
			}
		}
		/// <summary>
		/// Contains the ActiveButtonTopColor of appropriate themes.
		/// </summary>
		public Color ActiveButtonTopColor
		{
			get
			{
				return m_ActiveButtonTopColor;
			}
		}
		/// <summary>
		/// Contains the ActiveButtonBottomColor of appropriate themes.
		/// </summary>
		public Color ActiveButtonBottomColor
		{
			get
			{
				return m_ActiveButtonBottomColor;
			}
		}
		/// <summary>
		/// Contains the PushedButtonTopColor of appropriate themes.
		/// </summary>
		public Color PushedButtonTopColor
		{
			get
			{
				return m_PushedButtonTopColor;
			}
		}
		/// <summary>
		/// Contains the PushedButtonBottomColor of appropriate themes.
		/// </summary>
		public Color PushedButtonBottomColor
		{
			get
			{
				return m_PushedButtonBottomColor;
			}
		}
		/// <summary>
		/// Contains the ButtonImageColor of appropriate themes.
		/// </summary>
		public Color ButtonImageColor
		{
			get
			{
				return m_ButtonImageColor;
			}
		}

		/// <summary>
		/// Contains the PushedButtonImageColor of appropriate themes.
		/// </summary>
		public Color PushedButtonImageColor
		{
			get
			{
				return m_PushedButtonImageColor;
			}
		}


		#endregion

		#region Protected members

		protected Color m_TextColor;
		protected Color m_ActiveTextColor;
		protected Color m_OuterBorderColor;
		protected Color m_InnerBorderColor;
		protected Color m_CaptionTopColor;
		protected Color m_CaptionBottomColor;
		protected Color m_ActiveCaptionTopColor;
		protected Color m_ActiveCaptionBottomColor;
		protected Color m_GripperForegroundColor;
		protected Color m_GripperBackgroundColor;
		protected Color m_ButtonBorderColor;
		protected Color m_ActiveButtonTopColor;
		protected Color m_ActiveButtonBottomColor;
		protected Color m_PushedButtonTopColor;
		protected Color m_PushedButtonBottomColor;
		protected Color m_ButtonImageColor;
		protected Color m_PushedButtonImageColor;

		#endregion
	}

	/// <summary>
	/// Keeps the different colors for different part of docking windows (Visual Studio 2005-like appearance).
	/// </summary>
	public class ColorTableVS2005 : ColorTableBase
	{
		#region Constructors

		public ColorTableVS2005() : base()
		{
		}


		#endregion

		#region Protected overrided methods

		protected override void InitWindowsClassicColors()
		{
			m_bThemesXP = false;
			m_BorderColor = Color.FromArgb( 128, 128, 128 );
			m_InnerBorderColor = Color.FromArgb( 230, 230, 230 );
			m_ButtonBorderColor = Color.FromArgb( 255, 255, 255 );
			m_ActiveButtonBorderColor = Color.FromArgb( 255, 255, 255 );
			m_SplitterColor = Color.FromArgb( 246, 245, 244 );
		}

		protected override void InitXPBlueColors()
		{
			m_bThemesXP = true;
			m_TextColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveTextColor = Color.FromArgb( 255, 255, 255 );
			m_BorderColor = Color.FromArgb( 172, 168, 153 );
			m_InnerBorderColor = Color.FromArgb( 239, 238, 235 );
			m_CaptionTopColor = Color.FromArgb( 204, 199, 186 );
			m_CaptionBottomColor = Color.FromArgb( 204, 199, 186 );
			m_ActiveCaptionTopColor = Color.FromArgb( 59, 128, 237 );
			m_ActiveCaptionBottomColor = Color.FromArgb( 49, 106, 197 );
			m_ButtonBorderColor = Color.FromArgb( 140, 134, 123 );
			m_ActiveButtonBorderColor = Color.FromArgb( 60, 90, 170 );
			m_ActiveButtonColor = Color.FromArgb( 156, 182, 231 );
			m_PushedButtonColor = Color.FromArgb( 120, 150, 210 );
			m_ButtonColor = Color.FromArgb( 236, 233, 216 );
			m_SplitterColor = Color.FromArgb( 244, 242, 232 );
			m_ButtonImageColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveButtonImageColor = Color.FromArgb( 255, 255, 255 );
		}

		protected override void InitXPSilverColors()
		{
			m_bThemesXP = true;
			m_TextColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveTextColor = Color.FromArgb( 0, 0, 0 );
			m_BorderColor = Color.FromArgb( 145, 155, 156 );
			m_InnerBorderColor = Color.FromArgb( 233, 235, 236 );
			m_CaptionTopColor = Color.FromArgb( 240, 240, 245 );
			m_CaptionBottomColor = Color.FromArgb( 240, 240, 245 );
			m_ActiveCaptionTopColor = Color.FromArgb( 211, 212, 221 );
			m_ActiveCaptionBottomColor = Color.FromArgb( 166, 165, 191 );
			m_ButtonBorderColor = Color.FromArgb( 123, 125, 148 );
			m_ActiveButtonBorderColor = Color.FromArgb( 74, 73, 107 );
			m_ActiveButtonColor = Color.FromArgb( 255, 227, 173 );
			m_PushedButtonColor = Color.FromArgb( 255, 182, 115 );
			m_ButtonColor = Color.FromArgb( 214, 215, 222 );
			m_SplitterColor = Color.FromArgb( 243, 243, 247 );
			m_ButtonImageColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveButtonImageColor = Color.FromArgb( 0, 0, 0 );
		}

		protected override void InitXPOliveColors()
		{
			m_bThemesXP = true;
			m_TextColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveTextColor = Color.FromArgb( 255, 255, 255 );
			m_BorderColor = Color.FromArgb( 172, 168, 153 );
			m_InnerBorderColor = Color.FromArgb( 239, 238, 235 );
			m_CaptionTopColor = Color.FromArgb( 204, 199, 186 );
			m_CaptionBottomColor = Color.FromArgb( 204, 199, 186 );
			m_ActiveCaptionTopColor = Color.FromArgb( 182, 195, 146 );
			m_ActiveCaptionBottomColor = Color.FromArgb( 145, 160, 117 );
			m_ButtonBorderColor = Color.FromArgb( 140, 134, 123 );
			m_ActiveButtonBorderColor = Color.FromArgb( 118, 128, 95 );
			m_ActiveButtonColor = Color.FromArgb( 181, 199, 140 );
			m_PushedButtonColor = Color.FromArgb( 148, 162, 115 );
			m_ButtonColor = Color.FromArgb( 236, 233, 216 );
			m_SplitterColor = Color.FromArgb( 244, 242, 232 );
			m_ButtonImageColor = Color.FromArgb( 0, 0, 0 );
			m_ActiveButtonImageColor = Color.FromArgb( 255, 255, 255 );
		}

        protected override void InitXPZuneColors()
        {
            m_bThemesXP = true;
            m_TextColor = Color.FromArgb(0, 0, 0);
            m_ActiveTextColor = Color.FromArgb(255, 255, 255);
            m_BorderColor = Color.FromArgb(172, 168, 153);
            m_InnerBorderColor = Color.FromArgb(239, 238, 235);
            m_CaptionTopColor = Color.FromArgb(204, 199, 186);
            m_CaptionBottomColor = Color.FromArgb(204, 199, 186);
            m_ActiveCaptionTopColor = Color.FromArgb(45, 45, 45);
            m_ActiveCaptionBottomColor = Color.FromArgb(75, 75, 75);
            m_ButtonBorderColor = Color.FromArgb(140, 134, 123);
            m_ActiveButtonBorderColor = Color.FromArgb(68, 68, 68);
            m_ActiveButtonColor = Color.FromArgb(52, 52, 52);
            m_PushedButtonColor = Color.FromArgb(52, 52, 52);
            m_ButtonColor = Color.FromArgb(236, 233, 216);
            m_SplitterColor = Color.FromArgb(244, 242, 232);
            m_ButtonImageColor = Color.FromArgb(0, 0, 0);
            m_ActiveButtonImageColor = Color.FromArgb(255, 255, 255);
        }

		#endregion

		#region Properties

		/// <summary>
		/// Contains the TextColor of appropriate themes.
		/// </summary>
		public Color TextColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_TextColor;
				}
				else
				{
					return SystemColors.InactiveCaptionText;
				}
			}
		}
		/// <summary>
		/// Contains the ActiveTextColor of appropriate themes.
		/// </summary>
		public Color ActiveTextColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_ActiveTextColor;
				}
				else
				{
					return SystemColors.ActiveCaptionText;
				}
			}
		}
		/// <summary>
		/// Contains the BorderColor of appropriate themes.
		/// </summary>
		public Color BorderColor
		{
			get
			{
				return m_BorderColor;
			}
		}
		/// <summary>
		/// Contains the InnerBorderColor of appropriate themes.
		/// </summary>
		public Color InnerBorderColor
		{
			get
			{
				return m_InnerBorderColor;
			}
		}
		/// <summary>
		/// Contains the CaptionTopColor of appropriate themes.
		/// </summary>
		public Color CaptionTopColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_CaptionTopColor;
				}
				else
				{
					return SystemColors.InactiveCaption;
				}
			}
		}
		/// <summary>
		/// Contains the CaptionBottomColor of appropriate themes.
		/// </summary>
		public Color CaptionBottomColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_CaptionBottomColor;
				}
				else
				{
					return SystemColors.InactiveCaption;
				}
			}
		}
		/// <summary>
		/// Contains the ActiveCaptionTopColor of appropriate themes.
		/// </summary>
		public Color ActiveCaptionTopColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_ActiveCaptionTopColor;
				}
				else
				{
					return SystemColors.ActiveCaption;
				}
			}
		}
		/// <summary>
		/// Contains the ActiveCaptionBottomColor of appropriate themes.
		/// </summary>
		public Color ActiveCaptionBottomColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_ActiveCaptionBottomColor;
				}
				else
				{
					return SystemColors.ActiveCaption;
				}
			}
		}

		/// <summary>
		/// Contains the ButtonBorderColor of appropriate themes.
		/// </summary>
		public Color ButtonBorderColor
		{
			get
			{
				return m_ButtonBorderColor;
			}
		}
		/// <summary>
		/// Contains the ActiveButtonBorderColor of appropriate themes.
		/// </summary>
		public Color ActiveButtonBorderColor
		{
			get
			{
				return m_ActiveButtonBorderColor;
			}
		}
		/// <summary>
		/// Contains the ButtonImageColor of appropriate themes.
		/// </summary>
		public Color ButtonImageColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_ButtonImageColor;
				}
				else
				{
					return SystemColors.InactiveCaptionText;
				}
			}
		}
		/// <summary>
		/// Contains the ActiveButtonImageColor of appropriate themes.
		/// </summary>
		public Color ActiveButtonImageColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_ActiveButtonImageColor;
				}
				else
				{
					return SystemColors.ActiveCaptionText;
				}
			}
		}
		/// <summary>
		/// Contains the ActiveButtonColor of appropriate themes.
		/// </summary>
		public Color ActiveButtonColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_ActiveButtonColor;
				}
				else
				{
					return SystemColors.ActiveCaption;
				}
			}
		}
		/// <summary>
		/// Contains the PushedButtonColor of appropriate themes.
		/// </summary>
		public Color PushedButtonColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_PushedButtonColor;
				}
				else
				{
					return Color.FromArgb( 
						SystemColors.ActiveCaption.R + (255 - SystemColors.ActiveCaption.R) / 4,
						SystemColors.ActiveCaption.G + (255 - SystemColors.ActiveCaption.G) / 4,
						SystemColors.ActiveCaption.B + (255 - SystemColors.ActiveCaption.B) / 4 );
				}
			}
		}

		/// <summary>
		/// Contains the ButtonColor of appropriate themes.
		/// </summary>
		public Color ButtonColor
		{
			get
			{
				if( m_bThemesXP )
				{
					return m_ButtonColor;
				}
				else
				{
					return SystemColors.InactiveCaption;
				}
			}
		}

		/// <summary>
		/// Contains the SplitterColor of appropriate themes.
		/// </summary>
		public Color SplitterColor
		{
			get
			{
				return m_SplitterColor;
			}
		}

		#endregion

		#region Protected members

		protected Color m_TextColor;
		protected Color m_ActiveTextColor;
		protected Color m_BorderColor;
		protected Color m_InnerBorderColor;
		protected Color m_CaptionTopColor;
		protected Color m_CaptionBottomColor;
		protected Color m_ActiveCaptionTopColor;
		protected Color m_ActiveCaptionBottomColor;
		protected Color m_ButtonBorderColor;
		protected Color m_ActiveButtonBorderColor;
		protected Color m_ActiveButtonColor;
		protected Color m_PushedButtonColor;
		protected Color m_ButtonColor;
		protected Color m_SplitterColor;
		protected Color m_ButtonImageColor;
		protected Color m_ActiveButtonImageColor;

		protected bool m_bThemesXP;

		#endregion
	}
    /// <summary>
    /// Keeps the different colors for different part of docking windows (Office VS2010-like appearance).
    /// </summary>
    public class ColorTableVS2010 : ColorTableBase
    {
        #region Constructors

        public ColorTableVS2010()
            : base()
        {
        }


        #endregion

        #region Protected overrided methods

        protected override void InitWindowsClassicColors()
        {
            m_TextColor = Color.FromArgb(0, 0, 0);
            m_ActiveTextColor = Color.FromArgb(0, 0, 0);
            m_OuterBorderColor = Color.FromArgb(128, 128, 128);
            m_InnerBorderColor = Color.FromArgb(234, 232, 228);
            m_CaptionTopColor = Color.FromArgb(243, 242, 240);
            m_CaptionBottomColor = Color.FromArgb(212, 208, 200);
            m_ActiveCaptionTopColor = Color.Red;
            m_ActiveCaptionBottomColor = Color.FromArgb(254, 252, 234);
            m_GripperForegroundColor = Color.FromArgb(160, 160, 160);
            m_GripperBackgroundColor = Color.FromArgb(255, 255, 255);
            m_ButtonBorderColor = Color.FromArgb(10, 36, 106);
            m_ActiveButtonTopColor = Color.FromArgb(182, 189, 210);
            m_ActiveButtonBottomColor = Color.FromArgb(182, 189, 210);
            m_PushedButtonTopColor = Color.FromArgb(133, 146, 181);
            m_PushedButtonBottomColor = Color.FromArgb(133, 146, 181);
            m_ButtonImageColor = Color.FromArgb(0, 0, 0);
            m_PushedButtonImageColor = Color.FromArgb(255, 255, 255);
        }

        protected override void InitXPBlueColors()
        {
            m_TextColor = Color.FromArgb(0, 0, 0);
            m_ActiveTextColor = Color.Black;
            m_OuterBorderColor = Color.FromArgb(68, 88, 124);
            m_InnerBorderColor = Color.FromArgb(68, 88, 124);
            m_CaptionTopColor = Color.FromArgb(68, 88, 124);
            m_CaptionBottomColor = Color.FromArgb(68, 88, 124);
            m_ActiveCaptionTopColor = Color.FromArgb(255, 252, 242);
            m_ActiveCaptionBottomColor = Color.FromArgb(255, 232, 166);
            m_GripperForegroundColor = Color.FromArgb(39, 65, 118);
            m_GripperBackgroundColor = Color.FromArgb(255, 255, 255);
            m_ButtonBorderColor = Color.FromArgb(204, 171, 78);
            m_ActiveButtonTopColor = Color.FromArgb(255, 255, 255);
            m_ActiveButtonBottomColor = Color.FromArgb(255,255,255);
            m_PushedButtonTopColor = Color.FromArgb(255, 235, 171);
            m_PushedButtonBottomColor = Color.FromArgb(255, 235, 171);
            m_ButtonImageColor = Color.FromArgb(206, 212, 221);
            m_PushedButtonImageColor = Color.FromArgb(255, 235, 171);
        }

        protected override void InitXPSilverColors()
        {
            m_TextColor = Color.FromArgb(0, 0, 0);
            m_ActiveTextColor = Color.FromArgb(0, 0, 0);
            m_OuterBorderColor = Color.FromArgb(122, 121, 153);
            m_InnerBorderColor = Color.FromArgb(255, 255, 255);
            m_CaptionTopColor = Color.FromArgb(240, 240, 248);
            m_CaptionBottomColor = Color.FromArgb(147, 145, 176);
            m_ActiveCaptionTopColor = Color.FromArgb(255, 213, 140);
            m_ActiveCaptionBottomColor = Color.FromArgb(255, 166, 76);
            m_GripperForegroundColor = Color.FromArgb(84, 84, 117);
            m_GripperBackgroundColor = Color.FromArgb(255, 255, 255);
            m_ButtonBorderColor = Color.FromArgb(75, 75, 111);
            m_ActiveButtonTopColor = Color.FromArgb(255, 233, 186);
            m_ActiveButtonBottomColor = Color.FromArgb(255, 208, 145);
            m_PushedButtonTopColor = Color.FromArgb(254, 163, 95);
            m_PushedButtonBottomColor = Color.FromArgb(255, 211, 142);
            m_ButtonImageColor = Color.FromArgb(0, 0, 0);
            m_PushedButtonImageColor = Color.FromArgb(255, 255, 255);
        }

        protected override void InitXPOliveColors()
        {
            m_TextColor = Color.FromArgb(0, 0, 0);
            m_ActiveTextColor = Color.FromArgb(0, 0, 0);
            m_OuterBorderColor = Color.FromArgb(116, 134, 94);
            m_InnerBorderColor = Color.FromArgb(255, 255, 255);
            m_CaptionTopColor = Color.FromArgb(237, 242, 212);
            m_CaptionBottomColor = Color.FromArgb(181, 196, 143);
            m_ActiveCaptionTopColor = Color.FromArgb(255, 213, 140);
            m_ActiveCaptionBottomColor = Color.FromArgb(255, 166, 76);
            m_GripperForegroundColor = Color.FromArgb(81, 94, 51);
            m_GripperBackgroundColor = Color.FromArgb(255, 255, 255);
            m_ButtonBorderColor = Color.FromArgb(63, 93, 56);
            m_ActiveButtonTopColor = Color.FromArgb(255, 233, 186);
            m_ActiveButtonBottomColor = Color.FromArgb(255, 208, 145);
            m_PushedButtonTopColor = Color.FromArgb(254, 163, 95);
            m_PushedButtonBottomColor = Color.FromArgb(255, 211, 142);
            m_ButtonImageColor = Color.FromArgb(0, 0, 0);
            m_PushedButtonImageColor = Color.FromArgb(255, 255, 255);
        }


        #endregion

        #region Properties

        /// <summary>
        /// Contains the TextColor of appropriate themes.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return m_TextColor;
            }
        }
        /// <summary>
        /// Contains the ActiveTextColor of appropriate themes.
        /// </summary>
        public Color ActiveTextColor
        {
            get
            {
                return m_ActiveTextColor;
            }
        }
        /// <summary>
        /// Contains the OuterBorderColor of appropriate themes.
        /// </summary>
        public Color OuterBorderColor
        {
            get
            {
                return m_OuterBorderColor;
            }
        }
        /// <summary>
        /// Contains the InnerBorderColor of appropriate themes.
        /// </summary>
        public Color InnerBorderColor
        {
            get
            {
                return m_InnerBorderColor;
            }
        }
        /// <summary>
        /// Contains the CaptionTopColor of appropriate themes.
        /// </summary>
        public Color CaptionTopColor
        {
            get
            {
                return m_CaptionTopColor;
            }
        }
        /// <summary>
        /// Contains the CaptionBottomColor of appropriate themes.
        /// </summary>
        public Color CaptionBottomColor
        {
            get
            {
                return m_CaptionBottomColor;
            }
        }
        /// <summary>
        /// Contains the ActiveCaptionTopColor of appropriate themes.
        /// </summary>
        public Color ActiveCaptionTopColor
        {
            get
            {
                return m_ActiveCaptionTopColor;
            }
        }
        /// <summary>
        /// Contains the ActiveCaptionBottomColor of appropriate themes.
        /// </summary>
        public Color ActiveCaptionBottomColor
        {
            get
            {
                return m_ActiveCaptionBottomColor;
            }
        }
        /// <summary>
        /// Contains the GripperForegroundColor of appropriate themes.
        /// </summary>
        public Color GripperForegroundColor
        {
            get
            {
                return m_GripperForegroundColor;
            }
        }
        /// <summary>
        /// Contains the GripperBackgroundColor of appropriate themes.
        /// </summary>
        public Color GripperBackgroundColor
        {
            get
            {
                return m_GripperBackgroundColor;
            }
        }
        /// <summary>
        /// Contains the ButtonBorderColor of appropriate themes.
        /// </summary>
        public Color ButtonBorderColor
        {
            get
            {
                return m_ButtonBorderColor;
            }
        }
        /// <summary>
        /// Contains the ActiveButtonTopColor of appropriate themes.
        /// </summary>
        public Color ActiveButtonTopColor
        {
            get
            {
                return m_ActiveButtonTopColor;
            }
        }
        /// <summary>
        /// Contains the ActiveButtonBottomColor of appropriate themes.
        /// </summary>
        public Color ActiveButtonBottomColor
        {
            get
            {
                return m_ActiveButtonBottomColor;
            }
        }
        /// <summary>
        /// Contains the PushedButtonTopColor of appropriate themes.
        /// </summary>
        public Color PushedButtonTopColor
        {
            get
            {
                return m_PushedButtonTopColor;
            }
        }
        /// <summary>
        /// Contains the PushedButtonBottomColor of appropriate themes.
        /// </summary>
        public Color PushedButtonBottomColor
        {
            get
            {
                return m_PushedButtonBottomColor;
            }
        }
        /// <summary>
        /// Contains the ButtonImageColor of appropriate themes.
        /// </summary>
        public Color ButtonImageColor
        {
            get
            {
                return m_ButtonImageColor;
            }
        }

        /// <summary>
        /// Contains the PushedButtonImageColor of appropriate themes.
        /// </summary>
        public Color PushedButtonImageColor
        {
            get
            {
                return m_PushedButtonImageColor;
            }
        }


        #endregion

        #region Protected members

        protected Color m_TextColor;
        protected Color m_ActiveTextColor;
        protected Color m_OuterBorderColor;
        protected Color m_InnerBorderColor;
        protected Color m_CaptionTopColor;
        protected Color m_CaptionBottomColor;
        protected Color m_ActiveCaptionTopColor;
        protected Color m_ActiveCaptionBottomColor;
        protected Color m_GripperForegroundColor;
        protected Color m_GripperBackgroundColor;
        protected Color m_ButtonBorderColor;
        protected Color m_ActiveButtonTopColor;
        protected Color m_ActiveButtonBottomColor;
        protected Color m_PushedButtonTopColor;
        protected Color m_PushedButtonBottomColor;
        protected Color m_ButtonImageColor;
        protected Color m_PushedButtonImageColor;

        #endregion
    }
    /// <summary>
    /// Keeps the different colors for different part of docking windows (Visual Studio 2012-like appearance).
    /// </summary>
    public class ColorTableVS2012 : ColorTableBase
    {
        #region Constructors

        public ColorTableVS2012()
            : base()
        {
        }


        #endregion

        #region Protected overrided methods

        protected override void InitWindowsClassicColors()
        {
            m_bThemesXP = false;
            m_BorderColor = Color.FromArgb(128, 128, 128);
            m_InnerBorderColor = Color.FromArgb(230, 230, 230);
            m_ButtonBorderColor = Color.Red ;
            m_ActiveButtonBorderColor = Color.FromArgb(255, 255, 255);
            m_SplitterColor = Color.FromArgb(246, 245, 244);
        }
        protected override void InitXPBlueColors()
        {
            m_bThemesXP = true;
            m_TextColor = Color.FromArgb(0, 0, 0);
            m_ActiveTextColor = Color.FromArgb(255, 255, 255);
            m_BorderColor = Color.FromArgb(239, 239, 242);
            m_InnerBorderColor = Color.FromArgb(0, 122, 204);
            m_CaptionTopColor = Color.FromArgb(239, 239, 242);
            m_CaptionBottomColor = Color.FromArgb(239, 239, 242);
            m_ActiveCaptionTopColor = Color.FromArgb(0, 122, 204);
            m_ActiveCaptionBottomColor = Color.FromArgb(0, 122, 204);
            m_ButtonBorderColor = Color.FromArgb(239, 239, 242);
            m_ActiveButtonBorderColor = Color.FromArgb(82, 176, 239);
            m_ActiveButtonColor = Color.FromArgb(82, 176, 239);
            m_PushedButtonColor = Color.FromArgb(120, 150, 210);
            m_ButtonColor = Color.FromArgb(247, 247, 249);
            m_SplitterColor = Color.Green;
            m_ButtonImageColor = Color.FromArgb(0, 0, 0);
            m_ActiveButtonImageColor = Color.FromArgb(255, 255, 255);
        }
       

        protected override void InitXPSilverColors()
        {
            InitXPBlueColors();
        }

        protected override void InitXPOliveColors()
        {
            InitXPBlueColors();
        }

        protected override void InitXPZuneColors()
        {
            InitXPBlueColors();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Contains the TextColor of appropriate themes.
        /// </summary>
        public Color TextColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_TextColor;
                }
                else
                {
                    return SystemColors.InactiveCaptionText;
                }
            }
        }
        /// <summary>
        /// Contains the ActiveTextColor of appropriate themes.
        /// </summary>
        public Color ActiveTextColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_ActiveTextColor;
                }
                else
                {
                    return SystemColors.ActiveCaptionText;
                }
            }
        }
        /// <summary>
        /// Contains the BorderColor of appropriate themes.
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return m_BorderColor;
            }
        }
        /// <summary>
        /// Contains the InnerBorderColor of appropriate themes.
        /// </summary>
        public Color InnerBorderColor
        {
            get
            {
                return m_InnerBorderColor;
            }
        }
        /// <summary>
        /// Contains the CaptionTopColor of appropriate themes.
        /// </summary>
        public Color CaptionTopColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_CaptionTopColor;
                }
                else
                {
                    return SystemColors.InactiveCaption;
                }
            }
        }
        /// <summary>
        /// Contains the CaptionBottomColor of appropriate themes.
        /// </summary>
        public Color CaptionBottomColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_CaptionBottomColor;
                }
                else
                {
                    return SystemColors.InactiveCaption;
                }
            }
        }
        /// <summary>
        /// Contains the ActiveCaptionTopColor of appropriate themes.
        /// </summary>
        public Color ActiveCaptionTopColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_ActiveCaptionTopColor;
                }
                else
                {
                    return SystemColors.ActiveCaption;
                }
            }
        }
        /// <summary>
        /// Contains the ActiveCaptionBottomColor of appropriate themes.
        /// </summary>
        public Color ActiveCaptionBottomColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_ActiveCaptionBottomColor;
                }
                else
                {
                    return SystemColors.ActiveCaption;
                }
            }
        }

        /// <summary>
        /// Contains the ButtonBorderColor of appropriate themes.
        /// </summary>
        public Color ButtonBorderColor
        {
            get
            {
                return m_ButtonBorderColor;
            }
        }
        /// <summary>
        /// Contains the ActiveButtonBorderColor of appropriate themes.
        /// </summary>
        public Color ActiveButtonBorderColor
        {
            get
            {
                return m_ActiveButtonBorderColor;
            }
        }
        /// <summary>
        /// Contains the ButtonImageColor of appropriate themes.
        /// </summary>
        public Color ButtonImageColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_ButtonImageColor;
                }
                else
                {
                    return SystemColors.InactiveCaptionText;
                }
            }
        }
        /// <summary>
        /// Contains the ActiveButtonImageColor of appropriate themes.
        /// </summary>
        public Color ActiveButtonImageColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_ActiveButtonImageColor;
                }
                else
                {
                    return SystemColors.ActiveCaptionText;
                }
            }
        }
        /// <summary>
        /// Contains the ActiveButtonColor of appropriate themes.
        /// </summary>
        public Color ActiveButtonColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_ActiveButtonColor;
                }
                else
                {
                    return SystemColors.ActiveCaption;
                }
            }
        }
        /// <summary>
        /// Contains the PushedButtonColor of appropriate themes.
        /// </summary>
        public Color PushedButtonColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_PushedButtonColor;
                }
                else
                {
                    return Color.FromArgb(
                        SystemColors.ActiveCaption.R + (255 - SystemColors.ActiveCaption.R) / 4,
                        SystemColors.ActiveCaption.G + (255 - SystemColors.ActiveCaption.G) / 4,
                        SystemColors.ActiveCaption.B + (255 - SystemColors.ActiveCaption.B) / 4);
                }
            }
        }

        /// <summary>
        /// Contains the ButtonColor of appropriate themes.
        /// </summary>
        public Color ButtonColor
        {
            get
            {
                if (m_bThemesXP)
                {
                    return m_ButtonColor;
                }
                else
                {
                    return SystemColors.InactiveCaption;
                }
            }
        }

        /// <summary>
        /// Contains the SplitterColor of appropriate themes.
        /// </summary>
        public Color SplitterColor
        {
            get
            {
                return m_SplitterColor;
            }
        }

        #endregion

        #region Protected members

        protected Color m_TextColor;
        protected Color m_ActiveTextColor;
        protected Color m_BorderColor;
        protected Color m_InnerBorderColor;
        protected Color m_CaptionTopColor;
        protected Color m_CaptionBottomColor;
        protected Color m_ActiveCaptionTopColor;
        protected Color m_ActiveCaptionBottomColor;
        protected Color m_ButtonBorderColor;
        protected Color m_ActiveButtonBorderColor;
        protected Color m_ActiveButtonColor;
        protected Color m_PushedButtonColor;
        protected Color m_ButtonColor;
        protected Color m_SplitterColor;
        protected Color m_ButtonImageColor;
        protected Color m_ActiveButtonImageColor;

        protected bool m_bThemesXP;

        #endregion
    }
	/// <summary>
	/// Keeps the different colors for different part of docking windows (Office 2007-like appearance).
	/// </summary>
	public class ColorTableOffice2007 : ColorTableOffice2007Base
	{
		#region Constructors

		public ColorTableOffice2007() : base()
		{
		}

		#endregion

		#region Protected overrided methods

		protected override void InitOffice2007BlueColors()
		{
			m_TextColor = Color.FromArgb( 16, 65, 140 );
			m_BorderColor = Color.FromArgb( 181, 207, 247 );
			m_InnerBorderColor = Color.FromArgb( 222, 235, 255 );
			m_CaptionUpperTopColor = Color.FromArgb( 231, 235, 255 );
			m_CaptionUpperBottomColor = Color.FromArgb( 222, 231, 255 );
			m_CaptionLowerTopColor = Color.FromArgb(206, 223, 247);
			m_CaptionLowerBottomColor = Color.FromArgb(222, 247, 255);
			m_ActiveCaptionUpperTopColor = Color.FromArgb( 198, 223, 255 );
			m_ActiveCaptionUpperBottomColor = Color.FromArgb( 231, 231, 231 );
			m_ActiveCaptionLowerTopColor = Color.FromArgb(214, 227, 227);
			m_ActiveCaptionLowerBottomColor = Color.FromArgb(231, 211, 165);
			m_ButtonBorderColor = Color.FromArgb( 101, 147, 207 );
			m_ActiveButtonBorderColor = Color.FromArgb( 255, 189, 105 );
			m_PushedButtonBorderColor = Color.FromArgb( 251, 140, 60 );
			m_ActiveButtonColor = Color.FromArgb( 255, 231, 162 );
			m_PushedButtonColor = Color.FromArgb( 251, 140, 60 );
			m_ButtonColor = Color.FromArgb( 214, 232, 255 );
			m_SplitterColor = Color.FromArgb( 219, 230, 244 );
			m_ButtonImageColor = Color.FromArgb( 140, 166, 206 );
			m_ActiveButtonImageColor = Color.FromArgb( 99, 101, 99 );
		}

		protected override void InitOffice2007BlackColors()
		{
			m_TextColor = Color.FromArgb(255, 255, 255);
			m_BorderColor = Color.FromArgb(41, 44, 41);
			m_InnerBorderColor = Color.FromArgb(148, 146, 148);
			m_CaptionUpperTopColor = Color.FromArgb(66, 69, 82);
			m_CaptionUpperBottomColor = Color.FromArgb(57, 60, 66);
			m_CaptionLowerTopColor = Color.FromArgb(41, 48, 49);
			m_CaptionLowerBottomColor = Color.FromArgb(57, 60, 57);
			m_ActiveCaptionUpperTopColor = Color.FromArgb(148, 150, 148);
			m_ActiveCaptionUpperBottomColor = Color.FromArgb(115, 109, 90);
			m_ActiveCaptionLowerTopColor = Color.FromArgb(107, 105, 90);
			m_ActiveCaptionLowerBottomColor = Color.FromArgb(231, 190, 41);
			m_ButtonBorderColor = Color.FromArgb(82, 93, 99);
			m_ActiveButtonBorderColor = Color.FromArgb(239, 186, 107);
			m_PushedButtonBorderColor = Color.FromArgb(255, 162, 49);
			m_ActiveButtonColor = Color.FromArgb(255, 207, 74);
			m_PushedButtonColor = Color.FromArgb(255, 162, 49);
			m_ButtonColor = Color.FromArgb(115, 125, 140);
			m_SplitterColor = Color.FromArgb(82, 81, 82);
			m_ButtonImageColor = Color.FromArgb(206, 211, 214);
			m_ActiveButtonImageColor = Color.FromArgb(99, 101, 99);
		}

		protected override void InitOffice2007SilverColors()
		{
			m_TextColor = Color.FromArgb(90, 97, 107);
			m_BorderColor = Color.FromArgb(198, 190, 198);
			m_InnerBorderColor = Color.FromArgb(247, 247, 255);
			m_CaptionUpperTopColor = Color.FromArgb(231, 227, 231);
			m_CaptionUpperBottomColor = Color.FromArgb(206, 208, 214);
			m_CaptionLowerTopColor = Color.FromArgb(189, 195, 206);
			m_CaptionLowerBottomColor = Color.FromArgb(239, 239, 247);
			m_ActiveCaptionUpperTopColor = Color.FromArgb(222, 219, 231);
			m_ActiveCaptionUpperBottomColor = Color.FromArgb(239, 231, 222);
			m_ActiveCaptionLowerTopColor = Color.FromArgb(231, 223, 214);
			m_ActiveCaptionLowerBottomColor = Color.FromArgb(239, 211, 148);
			m_ButtonBorderColor = Color.FromArgb(206, 207, 206);
			m_ActiveButtonBorderColor = Color.FromArgb(206, 182, 157);
			m_PushedButtonBorderColor = Color.FromArgb(255, 162, 49);
			m_ActiveButtonColor = Color.FromArgb(255, 211, 74);
			m_PushedButtonColor = Color.FromArgb(255, 162, 49);
			m_ButtonColor = Color.FromArgb(247, 251, 255);
			m_SplitterColor = Color.FromArgb(239, 243, 239);
			m_ButtonImageColor = Color.FromArgb(140, 154, 181);
			m_ActiveButtonImageColor = Color.FromArgb(66, 69, 66);
		}

        protected override void  InitOffice2007ManagedColors()
        {
            Office2007Colors oc = Office2007Colors.GetColorTable( Office2007Theme.Managed );

            m_TextColor = oc.FormTextColor;;
            m_BorderColor = oc.ActiveFormBorderColor ;
            m_InnerBorderColor = oc.ActiveFormBorderColor;
            m_CaptionUpperTopColor = oc.InactiveTitleGradientBegin;
            m_CaptionUpperBottomColor = oc.InactiveTitleGradientBegin;
            m_CaptionLowerTopColor = oc.InactiveTitleGradientBegin;
            m_CaptionLowerBottomColor = oc.InactiveTitleGradientEnd;
            m_ActiveCaptionUpperTopColor = oc.ActiveTitleGradientBegin;
            m_ActiveCaptionUpperBottomColor = oc.ActiveTitleGradientBegin;//Color.FromArgb( 231, 231, 231 );
            m_ActiveCaptionLowerTopColor = oc.ActiveTitleGradientBegin;
            m_ActiveCaptionLowerBottomColor = oc.ActiveTitleGradientEnd;//Color.FromArgb( 231, 211, 165 );
            m_ButtonBorderColor = oc.ButtonDefaultBorderColor;
            m_ActiveButtonBorderColor = oc.ButtonSelectedBorderColor;
            m_PushedButtonBorderColor = oc.ButtonPressedBorderColor;
            m_ActiveButtonColor = oc.SystemButtonSelectedGradientBegin;
            m_PushedButtonColor = oc.SystemButtonPressedGradientBegin;
            m_ButtonColor = Color.FromArgb( 247, 251, 255 );
            m_SplitterColor = oc.TabBarSplitterBackColor;
            m_ButtonImageColor = Color.FromArgb( 140, 154, 181 );
            m_ActiveButtonImageColor = Color.FromArgb( 66, 69, 66 );
        }
		#endregion

		#region Properties

		/// <summary>
		/// Contains the ButtonBorderColor of appropriate themes.
		/// </summary>
		public Color ButtonBorderColor
		{
			get
			{
				return m_ButtonBorderColor;
			}
		}
		/// <summary>
		/// Contains the ActiveButtonBorderColor of appropriate themes.
		/// </summary>
		public Color ActiveButtonBorderColor
		{
			get
			{
				return m_ActiveButtonBorderColor;
			}
		}
		/// <summary>
		/// Contains the PushedButtonBorderColor of appropriate themes.
		/// </summary>
		public Color PushedButtonBorderColor
		{
			get
			{
				return m_PushedButtonBorderColor;
			}
		}

		#endregion

		#region Protected members

		protected Color m_ButtonBorderColor;
		protected Color m_ActiveButtonBorderColor;
		protected Color m_PushedButtonBorderColor;

		#endregion
	}

	/// <summary>
	/// Keeps the different colors for different part of docking windows (Office 2007-like appearance).
	/// </summary>
	public class ColorTableOffice2007Outlook : ColorTableOffice2007Base
	{ 
		#region Constructors

		public ColorTableOffice2007Outlook() : base()
		{
		}

		#endregion

		#region Protected overrided methods

		protected override void InitOffice2007BlueColors()
		{
			m_TextColor = Color.FromArgb( 33, 77, 140 );
			m_BorderColor = Color.FromArgb( 181, 207, 247 );
			m_InnerBorderColor = Color.FromArgb( 222, 235, 255 );
			m_CaptionUpperTopColor = Color.FromArgb( 231, 235, 255 );
			m_CaptionUpperBottomColor = Color.FromArgb( 222, 231, 255 );
			m_CaptionLowerTopColor = Color.FromArgb(206, 223, 247);
			m_CaptionLowerBottomColor = Color.FromArgb(222, 247, 255);
			m_ActiveCaptionUpperTopColor = Color.FromArgb(255, 223, 156);
			m_ActiveCaptionUpperBottomColor = Color.FromArgb(255, 203, 123);
			m_ActiveCaptionLowerTopColor = Color.FromArgb(255, 170, 82);
			m_ActiveCaptionLowerBottomColor = Color.FromArgb(255, 219, 140);
			m_ActiveButtonColor = Color.FromArgb( 255, 199, 99 );
			m_PushedButtonColor = Color.FromArgb( 239, 130, 8 );
			m_ButtonColor = Color.FromArgb( 214, 232, 255 );
			m_SplitterColor = Color.FromArgb( 219, 230, 244 );
			m_ButtonImageColor = Color.FromArgb( 82, 125, 181 );
			m_ActiveButtonImageColor = Color.FromArgb( 0, 0, 0 );
		}

		protected override void InitOffice2007BlackColors()
		{
			m_TextColor = Color.FromArgb(255, 252, 248);
			m_BorderColor = Color.FromArgb(41, 44, 41);
			m_InnerBorderColor = Color.FromArgb(148, 158, 165);
			m_CaptionUpperTopColor = Color.FromArgb(99, 105, 115);
			m_CaptionUpperBottomColor = Color.FromArgb(57, 60, 66);
			m_CaptionLowerTopColor = Color.FromArgb(41, 40, 41);
			m_CaptionLowerBottomColor = Color.FromArgb(66, 65, 66);
			m_ActiveCaptionUpperTopColor = Color.FromArgb(225, 193, 136);
			m_ActiveCaptionUpperBottomColor = Color.FromArgb(225, 183, 113);
			m_ActiveCaptionLowerTopColor = Color.FromArgb(225, 150, 62);
			m_ActiveCaptionLowerBottomColor = Color.FromArgb(225, 199, 120);
			m_ActiveButtonColor = Color.FromArgb(255, 199, 99);
			m_PushedButtonColor = Color.FromArgb(239, 130, 8);
			m_ButtonColor = Color.FromArgb(181, 182, 198);
			m_SplitterColor = Color.FromArgb(82, 81, 82);
			m_ButtonImageColor = Color.FromArgb(206, 211, 214);
			m_ActiveButtonImageColor = Color.FromArgb(99, 101, 99);
		}

		protected override void InitOffice2007SilverColors()
		{
			m_TextColor = Color.FromArgb(74, 81, 90);
			m_BorderColor = Color.FromArgb(198, 190, 198);
			m_InnerBorderColor = Color.FromArgb(247, 247, 255);
			m_CaptionUpperTopColor = Color.FromArgb(247, 243, 235);
			m_CaptionUpperBottomColor = Color.FromArgb(222, 223, 239);
			m_CaptionLowerTopColor = Color.FromArgb(206, 203, 214);
			m_CaptionLowerBottomColor = Color.FromArgb(222, 223, 222);
			m_ActiveCaptionUpperTopColor = Color.FromArgb(255, 223, 156);
			m_ActiveCaptionUpperBottomColor = Color.FromArgb(255, 203, 123);
			m_ActiveCaptionLowerTopColor = Color.FromArgb(255, 170, 82);
			m_ActiveCaptionLowerBottomColor = Color.FromArgb(255, 219, 140);
			m_ActiveButtonColor = Color.FromArgb(255, 195, 99);
			m_PushedButtonColor = Color.FromArgb(239, 125, 8);
			m_ButtonColor = Color.FromArgb(255, 251, 255);
			m_SplitterColor = Color.FromArgb(231, 235, 247);
			m_ButtonImageColor = Color.FromArgb(99, 105, 115);
			m_ActiveButtonImageColor = Color.FromArgb(74, 77, 82);
		}

		#endregion
	}
}